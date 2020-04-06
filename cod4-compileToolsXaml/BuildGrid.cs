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
        public void buildGrid()
        {
            if (StringType.StrCmp(Variables.selectedMap_Name, "", false) == 0)
            {
                WriteConsole("ERROR: You must select a map first!");
            }
            else
            {
                string gridOptions = gridOption.SelectedIndex != 0 ? "1" : "2";
                string modelsCollectDots = "1";

                if ((bool)gridCullModels.IsChecked)
                {
                    modelsCollectDots = "0";
                }
                    
                string SpMpInt = "0";

                if (isMPMap(Variables.selectedMap_Name))
                {
                    SpMpInt = "1";
                }

                string compilerArguments = "\"" + Variables.strTreePath + "\"" + " " + "\"" + Variables.strMapSourcePath + "\"" + " " + gridOptions + " " + modelsCollectDots + " " + Variables.selectedMap_Name + " " + SpMpInt;

                createCompileProcess(compilerArguments, "cod4compiletools_grid_custom.bat");
            }
        }
    }
}
