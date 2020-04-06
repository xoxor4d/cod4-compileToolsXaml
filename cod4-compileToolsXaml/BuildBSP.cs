using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Shell;

namespace cod4_compileToolsXaml
{
    public partial class MainWindow
    {
        /// <summary>
        /// BuildBSP.cs \\
        /// </summary>
        public void buildBSP()
        {
            if (StringType.StrCmp(Variables.selectedMap_Name, "", false) == 0)
            {
                WriteConsole("ERROR: You must select a map first!");
            }
            else
            {
                SaveCompileSettings();

                string SpMpPath = Variables.strPCMapsPath;
                string SpMpInt = "0";

                if (this.isMPMap(Variables.selectedMap_Name)) 
                {
                    SpMpInt = "1";
                    SpMpPath = Variables.strPCMapsPath_MP;
                }

                string compileBsp = "0";
                string compileBspArguments = "";

                if ((bool)chkBSP.IsChecked)
                {
                    compileBsp = "1";

                    if ((bool)chkOnlyEnts.IsChecked)
                        compileBspArguments += "-onlyents ";
                    if ((bool)chkBlockSize.IsChecked && StringType.StrCmp(txtBlockSize.Text, "", false) != 0)
                        compileBspArguments += "-blocksize " + txtBlockSize.Text + " ";
                    if ((bool)chkSampleScale.IsChecked && StringType.StrCmp(txtSampleScale.Text, "", false) != 0)
                        compileBspArguments += "-samplescale " + txtSampleScale.Text + " ";
                    if ((bool)chkDebugLightMaps.IsChecked)
                        compileBspArguments += "-debugLightmaps ";
                    if ((bool)chkCustomCommandLineBSP.IsChecked && StringType.StrCmp(txtBSPOptions.Text, "", false) != 0)
                        compileBspArguments += txtBSPOptions.Text + " ";
                }

                string compileLight = "0";
                string compileLightArguments = "";

                if ((bool)chkLight.IsChecked)
                {
                    compileLight = "1";

                    if ((bool)chkLightFast.IsChecked)
                        compileLightArguments += "-fast ";
                    if ((bool)chkLightExtra.IsChecked)
                        compileLightArguments += "-extra ";
                    if ((bool)chkLightVerbose.IsChecked)
                        compileLightArguments += "-verbose ";
                    if ((bool)chkModelShadow.IsChecked)
                        compileLightArguments += "-modelshadow ";
                    if ((bool)chkNoModelShadow.IsChecked)
                        compileLightArguments += "-nomodelshadow ";
                    if ((bool)chkDumpOptions.IsChecked)
                        compileLightArguments += "-dumpoptions ";
                    if ((bool)chkTraces.IsChecked && StringType.StrCmp(txtTraces.Text, "", false) != 0)
                        compileLightArguments += "-traces " + txtTraces.Text + " ";
                    if ((bool)chkBounceFraction.IsChecked && StringType.StrCmp(txtBounceFraction.Text, "", false) != 0)
                        compileLightArguments += "-bouncefraction " + txtBounceFraction.Text + " ";
                    if ((bool)chkJitter.IsChecked && StringType.StrCmp(txtJitter.Text, "", false) != 0)
                        compileLightArguments += "-jitter " + txtJitter.Text + " ";

                    if ((bool)chkCustomCommandLineLight.IsChecked && StringType.StrCmp(txtLightOptions.Text, "", false) != 0)
                        compileLightArguments += txtLightOptions.Text + " ";
                }

                string connectPaths = "0";

                if ((bool)chkPaths.IsChecked)
                {
                    connectPaths = "1";
                }

                string delimitedBspArgs = StringType.StrCmp(compileBspArguments, "", false) != 0 ? "\"" + Strings.Trim(compileBspArguments) + "\"" : "-";
                string delimitedLightArgs = StringType.StrCmp(compileLightArguments, "", false) != 0 ? "\"" + Strings.Trim(compileLightArguments) + "\"" : "-";
                string compilerArguments = " \"" + SpMpPath + "\"" + " " + "\"" + Variables.strMapSourcePath + "\"" + " " + "\"" + Variables.strTreePath + "\"" + " " + Variables.selectedMap_Name + " " + delimitedBspArgs + " " + delimitedLightArgs + " " + compileBsp + " " + compileLight + " " + connectPaths + "  " + SpMpInt; // 2 Spaces?

                createCompileProcess(compilerArguments, "cod4compiletools_compilebsp_custom.bat");
            }
        }
    }
}
