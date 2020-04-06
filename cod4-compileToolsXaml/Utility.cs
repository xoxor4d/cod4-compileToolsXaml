using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cod4_compileToolsXaml
{
    public partial class MainWindow
    {
        /// <summary>
        /// Utility.cs \\
        /// </summary>
        public bool checkFileExists(string strFileName) 
        {
            return StringType.StrCmp(strFileName, "", false) != 0 && StringType.StrCmp(FileSystem.Dir(strFileName, FileAttribute.Normal), "", false) != 0;
        }

        /// <summary>
        /// Utility.cs \\ Run Process with filecheck
        /// </summary>
        /// 
        public bool runProcess(string strFilename, string strWorkingDirectory, string strArguments = "")
        {
            if (checkFileExists(strFilename))
            {
                return runProcess_NoFileCheck(strFilename, strWorkingDirectory, strArguments);
            }

            WriteConsole("Error: could not find the specified file\n" + strFilename);
            return false;
        }

        /// <summary>
        /// Utility.cs \\ Run Process without filecheck
        /// </summary>
        public bool runProcess_NoFileCheck(string strFilename, string strWorkingDirectory, string strArguments = "")
        {
            new Process()
            {
                StartInfo = new ProcessStartInfo(strFilename)
                {
                    WorkingDirectory = strWorkingDirectory,
                    Arguments = strArguments,
                    UseShellExecute = false
                }
            }.Start();

            return true;
        }

        /// <summary>
        /// Utility.cs \\
        /// </summary>
        public bool CheckSavedMap(string strMapName) 
        {
            return StringType.StrCmp(FileSystem.Dir(Variables.strWorkingDir + strMapName + ".settings", FileAttribute.Normal), "", false) != 0;
        }

        /// <summary>
        /// Utility.cs \\
        /// </summary>
        public bool isMPMap(string strMapName) 
        {
            return strMapName.Length > 3 && BooleanType.FromString(Strings.LCase(StringType.FromBoolean(strMapName.StartsWith("mp_"))));
        }

        /// <summary>
        /// Utility.cs \\ Check if Zone Files for map exist
        /// </summary>
        public bool CheckForZoneFiles(string strMapName)
        {
            bool fileExists;
            fileExists = checkFileExists(Variables.strZoneSourcePath + "\\" + strMapName + ".csv");

            if (!fileExists)
            {
                string msgbox_msg = "There are no zone files for " + strMapName + ".\n" +
                                    "Would you like to create them?";

                MsgBoxWindow msgBoxWindow = new MsgBoxWindow(msgbox_msg) {
                    Owner = this
                };

                msgBoxWindow.ShowDialog();

                if( msgBoxWindow.DialogResult.HasValue && msgBoxWindow.DialogResult.Value )
                {
                    if (createZoneFiles(strMapName))
                    {
                        fileExists = true;
                    }
                }
            }

            return fileExists;
        }

        /// <summary>
        /// Utility.cs \\
        /// </summary>
        public bool createZoneFiles(string strMapName)
        {
            StreamWriter streamWriter1 = new StreamWriter((Stream)new FileStream(Variables.strZoneSourcePath + "\\" + strMapName + ".csv", FileMode.Create));

            if (isMPMap(strMapName))
            {
                streamWriter1.WriteLine("ignore,code_post_gfx_mp");
                streamWriter1.WriteLine("ignore,common_mp");
                streamWriter1.WriteLine("ignore,localized_code_post_gfx_mp");
                streamWriter1.WriteLine("ignore,localized_common_mp");
                streamWriter1.WriteLine("col_map_mp,maps/mp/" + strMapName + ".d3dbsp");
                streamWriter1.WriteLine("rawfile,maps/mp/" + strMapName + ".gsc");
                streamWriter1.WriteLine("impactfx," + strMapName);
                streamWriter1.WriteLine("sound,common," + strMapName + ",!all_mp");
                streamWriter1.WriteLine("sound,generic," + strMapName + ",!all_mp");
                streamWriter1.WriteLine("sound,voiceovers," + strMapName + ",!all_mp");
                streamWriter1.WriteLine("sound,multiplayer," + strMapName + ",!all_mp");
                streamWriter1.Close();

                StreamWriter streamWriter2 = new StreamWriter((Stream)new FileStream(Variables.strZoneSourcePath + "\\" + strMapName + "_load.csv", FileMode.Create));
                streamWriter2.WriteLine("ignore,code_post_gfx_mp");
                streamWriter2.WriteLine("ignore,common_mp");
                streamWriter2.WriteLine("ignore,localized_code_post_gfx_mp");
                streamWriter2.WriteLine("ignore,localized_common_mp");
                streamWriter2.WriteLine("ui_map,maps/" + strMapName + ".csv");
                streamWriter2.Close();
            }
            else
            {
                streamWriter1.WriteLine("ignore,code_post_gfx");
                streamWriter1.WriteLine("ignore,common");
                streamWriter1.WriteLine("col_map_sp,maps/" + strMapName + ".d3dbsp");
                streamWriter1.WriteLine("rawfile,maps/" + strMapName + ".gsc");
                streamWriter1.WriteLine("localize," + strMapName);
                streamWriter1.WriteLine("sound,common," + strMapName + ",!all_sp");
                streamWriter1.WriteLine("sound,generic," + strMapName + ",!all_sp");
                streamWriter1.WriteLine("sound,voiceovers," + strMapName + ",!all_sp");
                streamWriter1.WriteLine("sound,requests," + strMapName + ",!all_sp");
                streamWriter1.Close();
            }

            return true;
        }
    }
}
