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
        public void runGame()
        {
            if (StringType.StrCmp(Variables.selectedMap_Name, "", false) == 0)
            {
                WriteConsole("ERROR: You must select a map first!");
            }
            else
            {
                string SpMpInt = "0";

                if (isMPMap(Variables.selectedMap_Name))
                {
                    SpMpInt = "1";
                }

                string runArguments = "";

                if ((bool)chkDeveloper.IsChecked)
                    runArguments += "+set developer 1 ";
                if ((bool)chkDeveloperScript.IsChecked)
                    runArguments += "+set developer_script 1 ";
                if ((bool)chkCheats.IsChecked)
                    runArguments += "+set thereisacow 1337 ";
                if ((bool)chkCustomCommandLine.IsChecked & txtCustomCommandLine.Text.Length > 0)
                    runArguments += txtCustomCommandLine.Text;

                string gameExe = "";
                if(Variables.strGameMpName == "")
                {
                    gameExe = "iw3mp.exe";
                }
                else
                {
                    gameExe = Variables.strGameMpName;
                }

                string compilerArguments = "\"" + Variables.strTreePath + "\"" + " " + Variables.selectedMap_Name + " " + SpMpInt + " " + "\"" + runArguments + "\"" + " " + gameExe;
                createCompileProcess(compilerArguments, "cod4compiletools_runmap_custom.bat");
                //runProcess(Variables.strWorkingDir + "cod4compiletools_runmap_custom.bat", Variables.strWorkingDir, compilerArguments);
            }
        }
    }
}
