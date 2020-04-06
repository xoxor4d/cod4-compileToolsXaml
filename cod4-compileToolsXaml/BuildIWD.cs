using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace cod4_compileToolsXaml
{
    public partial class MainWindow
    {
        private void TimerTickIWD(object sender, EventArgs e)
        {
            if (TaskActive)
            {
                LauncherProcessTimeElapsedTextBox.Content = (DateTime.Now - consoleProcessStartTime).ToString().Substring(0, 8);
            }
        }

        private string[] GetImagesToCopy( string text )
        {
            string stockimagesPath = Variables.strWorkingDir + "stockimages.txt";
            //WriteConsole(stockimagesPath);

            List<string> stockImages;

            if (checkFileExists(stockimagesPath))
            {
                stockImages = new List<string>(System.IO.File.ReadAllLines(stockimagesPath));
                WriteConsole("\n:INFO: Comparing images using cached images from: ");
                WriteConsole(stockimagesPath);

                return IOUtil.GetImagesToCopyCache(text, stockImages);
            }

            else
            {
                WriteConsole("\n:INFO: Searching for custom materials ...");
                return IOUtil.GetImagesToCopy(text);
            }
        }

        public void buildIWD()
        {
            if (TaskActive)
            {
                WriteConsole("ERROR: Thread still active!");
                return;
            }

            TaskActive = true;
            ClearConsole();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += TimerTickIWD;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();

            consoleProcessStartTime = DateTime.Now;

            string text = Variables.selectedMap_Name;

            if (text == null || Variables.selectedMap_Name == null) 
            {
                WriteConsole("ERROR: Selected Map Invalid!");
            }
            else
            {
                string usermap = IOUtil.GetUsermapDirectory(text);
                string usermap_images = usermap + "images";
                string raw_images = Variables.strTreePath + "raw\\images\\";

                WriteConsole("CONSOLESTATUS: BUILDING IWD");
                SetCompilerCancelAbility(true, true);

                string[] imagesToCopy = GetImagesToCopy(text);

                if (imagesToCopy == null)
                {
                    WriteConsole("ERROR: No zone_source/assetlist/" + text + ".csv! Compile the map first!\nABORTING COPYING IMAGES!");
                    WriteConsole("" + imagesToCopy);
                }
                else if (!Directory.Exists(raw_images))
                {
                    WriteConsole("ERROR: You didn't have a raw/images folder, make sure to go through Asset Manager/converter first!\nABORTING COPYING IMAGES!");
                }
                else
                {
                    if (!Directory.Exists(usermap_images))
                    {
                        Directory.CreateDirectory(usermap_images);
                    }

                    foreach (string path2 in imagesToCopy)
                    {
                        if (File.Exists(Path.Combine(raw_images, path2)))
                        {
                            new Thread(delegate ()
                            {
                                File.Copy(Path.Combine(raw_images, path2), Path.Combine(usermap_images, path2), true);
                            }).Start();
                        }
                        else
                        {
                            WriteConsole("\nERROR: " + path2 + " does not exist in raw/images!");
                        }
                    }

                    string iwd = usermap + text + ".iwd";

                    if (!File.Exists(iwd))
                    {
                        new Thread(delegate ()
                        {
                            //this.WriteConsole("WARNING: IWD " + text + ".iwd not found; Creating ...", true);

                            using (FileStream zipToCreate = new FileStream(iwd, FileMode.Create))
                            {
                                using (ZipArchive newArchive = new ZipArchive(zipToCreate, ZipArchiveMode.Update, false))
                                {
                                    ZipArchiveEntry readmeEntry;
                                    DirectoryInfo d = new DirectoryInfo(usermap_images);
                                    FileInfo[] Files = d.GetFiles("*");

                                    foreach (FileInfo file in Files)
                                    {
                                        WriteConsole("++ Copying Image:     " + file.ToString());
                                        readmeEntry = newArchive.CreateEntryFromFile(usermap_images + "\\" + file.Name, "images" + "/" + file.Name);
                                    }

                                    WriteConsole("\n:INFO: Cleaning up ...");
                                    newArchive.Dispose();
                                    d.Delete(true);
                                }

                                zipToCreate.Close();
                                WriteConsole(":INFO: Building IWD ...");
                            }

                            WriteConsole("SUCCESS: Added Images to: " + text + ".iwd");

                            TaskActive = false;
                            SetCompilerCancelAbility(false);

                            // Clean up
                            GC.Collect();
                        }).Start();
                    }

                    else if (File.Exists(iwd))
                    {
                        try
                        {
                            new Thread(delegate ()
                            {
                                using (FileStream zipToOpen = new FileStream(iwd, FileMode.Open))
                                {
                                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                                    {
                                        ZipArchiveEntry readmeEntry;
                                        DirectoryInfo d = new DirectoryInfo(usermap_images);
                                        FileInfo[] Files = d.GetFiles("*");

                                        foreach (FileInfo file in Files)
                                        {
                                            foreach (var item in archive.Entries)
                                            {
                                                if (item.Name.Equals(file.ToString()))
                                                {
                                                    item.Delete();
                                                    break;
                                                }
                                            }

                                            WriteConsole("++ Copying Image:     " + file.ToString());
                                            readmeEntry = archive.CreateEntryFromFile(usermap_images + "\\" + file.Name, "images" + "/" + file.Name);
                                        }

                                        WriteConsole("\n:INFO: Cleaning up ...");
                                        archive.Dispose();
                                        d.Delete(true);
                                    }

                                    WriteConsole(":INFO: Building IWD ...");
                                }

                                WriteConsole("SUCCESS: Added Images to: " + text + ".iwd");

                                TaskActive = false;
                                SetCompilerCancelAbility(false);

                                // Clean up
                                GC.Collect();
                            }).Start();
                        }

                        catch
                        {
                            WriteConsole("ERROR: Can not open: " + iwd + "!");
                            WriteConsole("ERROR: Make sure that no other process is using " + text + ".iwd");

                            TaskActive = false;
                            SetCompilerCancelAbility(false);
                        }
                    }

                    else
                    {
                        TaskActive = false;
                        SetCompilerCancelAbility(false);

                        // Clean up
                        GC.Collect();
                    }
                }
            }
        }
    }
}
