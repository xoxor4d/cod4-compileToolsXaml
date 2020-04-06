using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cod4_compileToolsXaml
{
    public partial class MainWindow
    {
        /// <summary>
        /// BuildFastFile.cs \\
        /// </summary>
        public void buildFastFile()
        {
            if (StringType.StrCmp(Variables.selectedMap_Name, "", false) == 0) 
            {
                WriteConsole("ERROR: You must select a map first!");
            }

            else if (StringType.StrCmp(Variables.selectedMap_Name, "mp_backlot", false) == 0 | StringType.StrCmp(Variables.selectedMap_Name, "mp_backlot_geo", false) == 0)
            {
                WriteConsole("ERROR: This sample map cannot be built using it's original name. Doing so would cause errors when trying to join a multiplayer match on another server.");
            }

            else
            {
                if (!CheckForZoneFiles(Variables.selectedMap_Name))
                {
                    return;
                }
                    
                string fileName = "build_custom.bat";

                if (Variables.copyToUsermaps)
                {
                    fileName = "build_custom_move.bat";
                }

                string filePath = Variables.strZoneSourcePath + "english\\";
                string workingDirPath = Variables.strBinPath;

                createCompileProcess(Variables.language + " " + Variables.selectedMap_Name, fileName, filePath, workingDirPath);
            }
        }
    }
}
