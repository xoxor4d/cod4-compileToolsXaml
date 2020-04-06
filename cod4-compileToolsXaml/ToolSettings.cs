using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace cod4_compileToolsXaml
{
    public partial class MainWindow
    {
        /// <summary>
        /// ToolSettings.cs \\ Save map-unrelated user settings 
        /// </summary>
        public void saveUserSettings()
        {
            if (!Variables.allowSaveLastMap)
            {
                return;
            }

            StreamWriter streamWriter = new StreamWriter((Stream)new FileStream(Variables.strWorkingDir + "CoD4CompileTools.settings", FileMode.Create));

            streamWriter.WriteLine("tree," + Variables.strTreePath);
            streamWriter.WriteLine("mapname," + Variables.selectedMap_Name);
            streamWriter.WriteLine("developer," + StringType.FromBoolean((bool)chkDeveloper.IsChecked));
            streamWriter.WriteLine("developerscript," + StringType.FromBoolean((bool)chkDeveloperScript.IsChecked));
            streamWriter.WriteLine("cheats," + StringType.FromBoolean((bool)chkCheats.IsChecked));
            streamWriter.WriteLine("tab," + StringType.FromInteger(compileTab.SelectedIndex));
            streamWriter.WriteLine("chkCustomCommandLine," + StringType.FromBoolean((bool)chkCustomCommandLine.IsChecked));
            streamWriter.WriteLine("txtCustomCommandLine," + txtCustomCommandLine.Text);
            streamWriter.WriteLine("moveToUsermaps," + StringType.FromBoolean(Variables.copyToUsermaps));
            streamWriter.WriteLine("exeGameMp," + Variables.strGameMpName);
            streamWriter.WriteLine("exeRadiant," + Variables.strRadiantName);

            // GLSL->HLSL Syntax Converter
            streamWriter.WriteLine("sgen_width," + StringType.FromDouble(Variables.SyntaxConverterWidth));
            streamWriter.WriteLine("sgen_height," + StringType.FromDouble(Variables.SyntaxConverterHeight));
            streamWriter.WriteLine("sgen_font," + StringType.FromDouble(Variables.SyntaxConverterFontSize));
            streamWriter.WriteLine("sgen_optimise," + StringType.FromBoolean(Variables.SyntaxConverterOptimise));

            streamWriter.Close();
        }

        /// <summary>
        /// ToolSettings.cs \\ Load map-unrelated user settings
        /// </summary>
        public bool loadUserSettings()
        {
            string AppPath = System.AppDomain.CurrentDomain.BaseDirectory;
            string toolSettings = AppPath + "\\CoD4CompileTools.settings";
            
            if (StringType.StrCmp(FileSystem.Dir(toolSettings, FileAttribute.Normal), "", false) == 0)
            {
                return false;
            }
                
            File.SetAttributes(AppPath + "\\CoD4CompileTools.settings", FileAttributes.Normal);
            StreamReader streamReader = new StreamReader((Stream)File.OpenRead(toolSettings));
           
            while (streamReader.Peek() != -1)
            {
                string String1 = streamReader.ReadLine();
                int count = Strings.InStr(String1, ",", CompareMethod.Binary);
                
                if (count == 0)
                {
                    return false;
                }  

                string sLeft = String1.Remove(checked(count - 1), checked(String1.Length - count + 1));
                string str2 = String1.Remove(0, count);

                if (StringType.StrCmp(sLeft, "tree", false) == 0)
                {
                    Variables.strTreePath = str2;
                    this.updateLocalPaths();
                    this.fillMapList();
                }
                else if (StringType.StrCmp(sLeft, "mapname", false) == 0)
                {
                    int num1 = -1;
                    int num2 = 0;
                    int num3 = checked(lstMapFiles.Items.Count - 1);
                    int index = num2;

                    while (index <= num3)
                    {
                        if (ObjectType.ObjTst(lstMapFiles.Items[index], (object)str2, false) == 0)
                        {
                            num1 = index;
                        }
                            
                        checked { ++index; }
                    }

                    if (num1 >= 0)
                    {
                        lstMapFiles.SelectedIndex = num1;
                    }
                }


                else if (StringType.StrCmp(sLeft, "developer", false) == 0)
                    chkDeveloper.IsChecked = BooleanType.FromString(str2);
                else if (StringType.StrCmp(sLeft, "developerscript", false) == 0)
                    chkDeveloperScript.IsChecked = BooleanType.FromString(str2);
                else if (StringType.StrCmp(sLeft, "cheats", false) == 0)
                    chkCheats.IsChecked = BooleanType.FromString(str2);
                else if (StringType.StrCmp(sLeft, "tab", false) == 0)
                    compileTab.SelectedIndex = IntegerType.FromString(str2);
                else if (StringType.StrCmp(sLeft, "chkCustomCommandLine", false) == 0)
                    chkCustomCommandLine.IsChecked = BooleanType.FromString(str2);
                else if (StringType.StrCmp(sLeft, "moveToUsermaps", false) == 0)
                    Variables.copyToUsermaps = BooleanType.FromString(str2);
                else if (StringType.StrCmp(sLeft, "txtCustomCommandLine", false) == 0)
                    txtCustomCommandLine.Text = str2;
                else if (StringType.StrCmp(sLeft, "exeGameMp", false) == 0)
                    Variables.strGameMpName = str2;
                else if(StringType.StrCmp(sLeft, "exeRadiant", false) == 0)
                    Variables.strRadiantName = str2;

                // GLSL->HLSL Syntax Converter
                else if (StringType.StrCmp(sLeft, "sgen_width", false) == 0)
                    Variables.SyntaxConverterWidth = DoubleType.FromString(str2);
                else if (StringType.StrCmp(sLeft, "sgen_height", false) == 0)
                    Variables.SyntaxConverterHeight = DoubleType.FromString(str2);
                else if (StringType.StrCmp(sLeft, "sgen_font", false) == 0)
                    Variables.SyntaxConverterFontSize = DoubleType.FromString(str2);
                else if (StringType.StrCmp(sLeft, "sgen_optimise", false) == 0)
                    Variables.SyntaxConverterOptimise = BooleanType.FromString(str2);

            }

            streamReader.Close();
            return true;
        }

        /// <summary>
        /// ToolSettings.cs \\ Save map-related compile settings
        /// </summary>
        public void SaveCompileSettings()
        {
            if (Variables.selectedMap_Index == -1)
            {
                return;
            }

            SaveNewCompileSettings();
        }

        private void SaveNewCompileSettings()
        {
            if (this.CheckSavedMap(Variables.selectedMap_Name))
            {
                File.SetAttributes(Variables.strWorkingDir + Variables.selectedMap_Name + ".settings", FileAttributes.Normal);
            }

            StreamWriter streamWriter = new StreamWriter((Stream)new FileStream(Variables.strWorkingDir + Variables.selectedMap_Name + ".settings", FileMode.Create));

            streamWriter.WriteLine("bsp," + StringType.FromBoolean((bool)chkBSP.IsChecked));
            streamWriter.WriteLine("light," + StringType.FromBoolean((bool)chkLight.IsChecked));
            streamWriter.WriteLine("paths," + StringType.FromBoolean((bool)chkPaths.IsChecked));
            streamWriter.WriteLine("onlyents," + StringType.FromBoolean((bool)chkOnlyEnts.IsChecked));
            streamWriter.WriteLine("blocksize," + StringType.FromBoolean((bool)chkBlockSize.IsChecked));
            streamWriter.WriteLine("blocksize_value," + txtBlockSize.Text);
            streamWriter.WriteLine("samplescale," + StringType.FromBoolean((bool)chkSampleScale.IsChecked));
            streamWriter.WriteLine("samplescale_value," + txtSampleScale.Text);
            streamWriter.WriteLine("debugLightmaps," + StringType.FromBoolean((bool)chkDebugLightMaps.IsChecked));
            streamWriter.WriteLine("chkCustomCommandLineBSP," + StringType.FromBoolean((bool)chkCustomCommandLineBSP.IsChecked));
            streamWriter.WriteLine("bspoptions," + this.txtBSPOptions.Text);
            streamWriter.WriteLine("fast," + StringType.FromBoolean((bool)chkLightFast.IsChecked));
            streamWriter.WriteLine("extra," + StringType.FromBoolean((bool)chkLightExtra.IsChecked));
            streamWriter.WriteLine("verbose," + StringType.FromBoolean((bool)chkLightVerbose.IsChecked));
            streamWriter.WriteLine("modelshadow," + StringType.FromBoolean((bool)chkModelShadow.IsChecked));
            streamWriter.WriteLine("nomodelshadow," + StringType.FromBoolean((bool)chkNoModelShadow.IsChecked));
            streamWriter.WriteLine("dumpoptions," + StringType.FromBoolean((bool)chkDumpOptions.IsChecked));
            streamWriter.WriteLine("traces," + StringType.FromBoolean((bool)chkTraces.IsChecked));
            streamWriter.WriteLine("traces_value," + txtTraces.Text);
            streamWriter.WriteLine("bouncefraction," + StringType.FromBoolean((bool)chkBounceFraction.IsChecked));
            streamWriter.WriteLine("bouncefraction_value," + txtBounceFraction.Text);
            streamWriter.WriteLine("jitter," + StringType.FromBoolean((bool)chkJitter.IsChecked));
            streamWriter.WriteLine("jitter_value," + txtJitter.Text);
            streamWriter.WriteLine("chkCustomCommandLineLight," + StringType.FromBoolean((bool)chkCustomCommandLineLight.IsChecked));
            streamWriter.WriteLine("lightoptions," + txtLightOptions.Text);

            streamWriter.Close();
        }

        /// <summary>
        /// ToolSettings.cs \\ Load map-related compile settings
        /// </summary>
        private void LoadSavedMap(string strMapName)
        {
            StreamReader streamReader = new StreamReader((Stream)File.OpenRead(Variables.strWorkingDir + strMapName + ".settings"));
           
            while (streamReader.Peek() != -1)
            {
                LoadSavedSetting(streamReader.ReadLine());
            }

            streamReader.Close();
        }

        private void LoadSavedSetting(string strLine)
        {
            int count = Strings.InStr(strLine, ",", CompareMethod.Binary);

            if (count == 0)
            {
                return;
            }

            string sLeft = strLine.Remove(checked(count - 1), checked(strLine.Length - count + 1));
            string str = strLine.Remove(0, count);

            if (StringType.StrCmp(sLeft, "bsp", false) == 0)
                chkBSP.IsChecked = BooleanType.FromString(str);
            else if (StringType.StrCmp(sLeft, "light", false) == 0)
                chkLight.IsChecked = BooleanType.FromString(str);
            else if (StringType.StrCmp(sLeft, "paths", false) == 0)
                chkPaths.IsChecked = BooleanType.FromString(str);
            else if (StringType.StrCmp(sLeft, "onlyents", false) == 0)
                chkOnlyEnts.IsChecked = BooleanType.FromString(str);
            else if (StringType.StrCmp(sLeft, "blocksize", false) == 0)
                chkBlockSize.IsChecked = BooleanType.FromString(str);
            else if (StringType.StrCmp(sLeft, "blocksize_value", false) == 0)
                txtBlockSize.Text = str;
            else if (StringType.StrCmp(sLeft, "samplescale", false) == 0)
                chkSampleScale.IsChecked = BooleanType.FromString(str);
            else if (StringType.StrCmp(sLeft, "samplescale_value", false) == 0)
                txtSampleScale.Text = str;
            else if (StringType.StrCmp(sLeft, "debugLightmaps", false) == 0)
                chkDebugLightMaps.IsChecked = BooleanType.FromString(str);
            else if (StringType.StrCmp(sLeft, "bspoptions", false) == 0)
                txtBSPOptions.Text = str;
            else if (StringType.StrCmp(sLeft, "fast", false) == 0)
                chkLightFast.IsChecked = BooleanType.FromString(str);
            else if (StringType.StrCmp(sLeft, "extra", false) == 0)
                chkLightExtra.IsChecked = BooleanType.FromString(str);
            else if (StringType.StrCmp(sLeft, "verbose", false) == 0)
                chkLightVerbose.IsChecked = BooleanType.FromString(str);
            else if (StringType.StrCmp(sLeft, "modelshadow", false) == 0)
                chkModelShadow.IsChecked = BooleanType.FromString(str);
            else if (StringType.StrCmp(sLeft, "nomodelshadow", false) == 0)
                chkNoModelShadow.IsChecked = BooleanType.FromString(str);
            else if (StringType.StrCmp(sLeft, "dumpoptions", false) == 0)
                chkDumpOptions.IsChecked = BooleanType.FromString(str);
            else if (StringType.StrCmp(sLeft, "traces", false) == 0)
                chkTraces.IsChecked = BooleanType.FromString(str);
            else if (StringType.StrCmp(sLeft, "traces_value", false) == 0)
                txtTraces.Text = str;
            else if (StringType.StrCmp(sLeft, "bouncefraction", false) == 0)
                chkBounceFraction.IsChecked = BooleanType.FromString(str);
            else if (StringType.StrCmp(sLeft, "bouncefraction_value", false) == 0)
                txtBounceFraction.Text = str;
            else if (StringType.StrCmp(sLeft, "jitter", false) == 0)
                chkJitter.IsChecked = BooleanType.FromString(str);
            else if (StringType.StrCmp(sLeft, "jitter_value", false) == 0)
                txtJitter.Text = str;
            else if (StringType.StrCmp(sLeft, "chkCustomCommandLineBSP", false) == 0)
                chkCustomCommandLineBSP.IsChecked = BooleanType.FromString(str);
            else if (StringType.StrCmp(sLeft, "chkCustomCommandLineLight", false) == 0)
                chkCustomCommandLineLight.IsChecked = BooleanType.FromString(str);

            else
            {
                if (StringType.StrCmp(sLeft, "lightoptions", false) != 0)
                {
                    return;
                }

                txtLightOptions.Text = str;
            }
        }

        /// <summary>
        /// ToolSettings.cs \\ Load default user settings
        /// </summary>
        public void LoadDefaultUserSettings()
        {
            Variables.strTreePath = System.AppDomain.CurrentDomain.BaseDirectory.Remove(checked(System.AppDomain.CurrentDomain.BaseDirectory.Length - 20), 20);

            updateLocalPaths();
            resetCompileOptions();
        }

        /// <summary>
        /// AppStartup.cs \\ Set compile settings to default
        /// </summary>
        public void resetCompileOptions()
        {
            if (!CheckSavedMap("default"))
            {
                return;
            }

            LoadSavedMap("default");
        }

        /// <summary>
        /// AppStartup.cs \\ Set language for path setup
        /// </summary>
        public void setLanguage()
        {
            string str1 = Variables.strTreePath + "localization.txt";

            if (StringType.StrCmp(Variables.strTreePath, "", false) == 0)
            {
                Variables.language = "english";
            }
            else
            {
                if (this.checkFileExists(str1))
                {
                    StreamReader streamReader = new StreamReader((Stream)File.OpenRead(str1));
                    string str2 = streamReader.ReadLine();
                    streamReader.Close();

                    if (str2.Length > 0)
                    {
                        Variables.language = str2;
                    }
                }
                else
                {
                    Variables.language = "english";
                    string msgbox_msg = "Couldn't find \"" + str1 + "\" to see which language you are using. \n" + "Defaulting to English.";

                    MsgBoxWindow msgBoxWindow = new MsgBoxWindow(msgbox_msg, true);
                    msgBoxWindow.ShowDialog();
                }
            }
        }
    }
}
