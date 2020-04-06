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
        /// BuildReflections.cs \\
        /// </summary>
        public void buildReflections()
        {
            if (StringType.StrCmp(Variables.selectedMap_Name, "", false) == 0)
            {
                WriteConsole("ERROR: You must select a map first!");
            }
            else
            {
                string SpMpInt = "0";

                if (this.isMPMap(Variables.selectedMap_Name))
                {
                    SpMpInt = "1";
                }
                    
                string batPath = "cod4compiletools_reflections_custom.bat";
                string compilerArguments = "\"" + Variables.strTreePath + "\"" + " " + Variables.selectedMap_Name + " " + SpMpInt;

                createCompileProcess(compilerArguments, batPath);
            }
        }
    }
}
