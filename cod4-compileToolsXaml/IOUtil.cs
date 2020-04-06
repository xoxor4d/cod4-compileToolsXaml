using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cod4_compileToolsXaml
{
    class IOUtil
    {
        /// <summary>
        /// Utility.cs \\
        /// </summary>
        public static string GetUsermapDirectory(string mapName)
        {
            if (mapName == null)
            {
                return (string)null;
            }

            return Variables.strTreePath + "usermaps\\" + mapName + "\\";
        }

        /// <summary>
        /// Utility.cs \\
        /// </summary>
        public static string[] GetImagesToCopy(string mapName)
        {
            string str = Variables.strTreePath + "zone_source\\" + Variables.language + "\\assetlist\\" + mapName + ".csv";

            if (!File.Exists(str))
            {
                return null;
            }

            return GetNonStockImages(LoadCSVFile(str, "image"));
        }

        /// <summary>
        /// Utility.cs \\
        /// </summary>
        public static string[] GetNonStockImages(string[] modImages)
        {
            List<string> stringList1 = new List<string>((IEnumerable<string>)MainImages);
            List<string> stringList2 = new List<string>();

            using (var writer = new StreamWriter(Variables.strWorkingDir + "stockimages.txt"))
            {
                for (int i = 0; i < stringList1.Count; i++)
                {
                    writer.WriteLine(stringList1[i]);
                }
                writer.Close();
            }

            foreach (string modImage in modImages)
            {
                if (!stringList1.Contains(modImage))
                {
                    stringList2.Add(modImage);
                }
            }

            return stringList2.ToArray();
        }

        public static string[] GetImagesToCopyCache(string mapName, List<string> cacheImages )
        {
            string str = Variables.strTreePath + "zone_source\\" + Variables.language + "\\assetlist\\" + mapName + ".csv";

            if (!File.Exists(str))
            {
                return null;
            }
                
            return GetNonStockImagesCache(LoadCSVFile(str, "image"), cacheImages );
        }

        public static string[] GetNonStockImagesCache(string[] modImages, List<string> cacheImages )
        {
            List<string> stringList2 = new List<string>();

            foreach (string modImage in modImages)
            {
                if (!cacheImages.Contains(modImage))
                {
                    stringList2.Add(modImage);
                }
            }

            return stringList2.ToArray();
        }

        /// <summary>
        /// Utility.cs \\
        /// </summary>
        public static string[] LoadCSVFile(string csvFile) 
        {
            return LoadCSVFile(csvFile, null, null);
        }

        /// <summary>
        /// Utility.cs \\
        /// </summary>
        public static string[] LoadCSVFile(string csvFile, string findsWordsWith) 
        {
            return LoadCSVFile(csvFile, findsWordsWith, null);
        }

        /// <summary>
        /// Utility.cs \\
        /// </summary>
        public static string[] LoadCSVFile(string textFile, string findsWordsWith, string skipCommentLinesStartingWith)
        {
            string[] stringArray = new string[0];
            string str1 = "";

            switch (findsWordsWith)
            {
                case "image":
                    str1 = ".iwi";
                    break;
            }

            try
            {
                using (StreamReader streamReader = new StreamReader(textFile))
                {
                    string str2;

                    while ((str2 = streamReader.ReadLine()) != null)
                    {
                        str2.Trim();

                        if (str2 != "" && (skipCommentLinesStartingWith == null || !str2.StartsWith(skipCommentLinesStartingWith)) && str2.StartsWith(findsWordsWith))
                        {
                            StringArrayAdd(ref stringArray, str2.Substring(findsWordsWith.Length + 1) + str1);
                        }
                    }
                }
            }
            catch { }

            return stringArray;
        }

        /// <summary>
        /// Utility.cs \\
        /// </summary>
        public static string[] GetMainImages()
        {
            string[] stringArray = new string[0];

            foreach (string iwdFile in GetIWDFiles())
            {
                //using (ZipArchive archive = ZipFile.OpenRead(iwdFile))
                using (ZipArchive archive = new ZipArchive(File.OpenRead(iwdFile), ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.Contains(".iwi") && entry.FullName.Contains("images/"))
                        {
                            string stringItem = entry.FullName.ToString().Substring(7);
                            StringArrayAdd(ref stringArray, stringItem);
                        }
                    }
                }
            }

            return stringArray;
        }

        public static string[] MainImages = GetMainImages();

        /// <summary>
        /// Utility.cs \\
        /// </summary>
        public static string[] GetIWDFiles() 
        {
            return GetFilesRecursively(GetMainDirectory(), "*.iwd");
        }

        /// <summary>
        /// Utility.cs \\
        /// </summary>
        public static string[] GetFilesRecursively(string directory) 
        {
            return GetFilesRecursively(directory, "*");
        }

        /// <summary>
        /// Utility.cs \\
        /// </summary>
        public static string[] GetFilesRecursively(string directory, string filesToIncludeFilter)
        {
            string[] files = new string[0];

            GetFilesRecursively(directory, filesToIncludeFilter, ref files);
            return files;
        }

        /// <summary>
        /// Utility.cs \\
        /// </summary>
        public static void GetFilesRecursively(string directory, string filesToIncludeFilter, ref string[] files)
        {
            foreach (DirectoryInfo directory1 in new DirectoryInfo(directory).GetDirectories())
            {
                GetFilesRecursively(Path.Combine(directory, directory1.Name), filesToIncludeFilter, ref files);
            }
                
            foreach (FileInfo file in new DirectoryInfo(directory).GetFiles(filesToIncludeFilter))
            {
                StringArrayAdd(ref files, Path.Combine(directory, file.Name.ToLower()));
            }
        }

        /// <summary>
        /// Utility.cs \\
        /// </summary>
        public static void StringArrayAdd(ref string[] stringArray, string stringItem)
        {
            Array.Resize<string>(ref stringArray, stringArray.Length + 1);
            stringArray[stringArray.Length - 1] = stringItem;
        }

        /// <summary>
        /// Utility.cs \\
        /// </summary>
        public static string GetMainDirectory() 
        {
            return CanonicalDirectory(Path.Combine(GetRootDirectory(), "main"));
        }

        /// <summary>
        /// Utility.cs \\
        /// </summary>
        public static string CanonicalDirectory(string path)
        {
            FileInfo fileInfo = new FileInfo(path + "." + (object)Path.DirectorySeparatorChar);
            MakeDirectory(fileInfo.DirectoryName);
            return fileInfo.DirectoryName + (object)Path.DirectorySeparatorChar;
        }

        /// <summary>
        /// Utility.cs \\
        /// </summary>
        public static void MakeDirectory(string directoryName)
        {
            while (!Directory.Exists(directoryName))
            {
                string directoryName1 = Path.GetDirectoryName(directoryName);

                if (directoryName1 != directoryName)
                {
                    MakeDirectory(directoryName1);
                }
                    
                Directory.CreateDirectory(directoryName);
            }
        }

        /// <summary>
        /// Utility.cs \\
        /// </summary>
        public static string GetMapSourceDirectory() 
        {
            return CanonicalDirectory(Path.Combine(GetRootDirectory(), "map_source"));
        }

        /// <summary>
        /// Utility.cs \\
        /// </summary>
        public static string GetRootDirectory() 
        {
            return Variables.strTreePath;
        }
    }
}
