using System;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;
using System.Threading.Tasks;

namespace cod4_compileToolsXaml
{
    public partial class ShaderGenWindow : Window
    {
        enum SHADERGEN_VIEWMODEL_WEAPONS
        {
            viewmodel_desert_eagle_mp   = 0,
            viewmodel_m40a3_mp          = 1,
            viewmodel_ak47_mp           = 2,
            viewmodel_ak74u_mp          = 3,
            viewmodel_barrett_mp        = 4,
            viewmodel_benelli_m4_mp     = 5,
            viewmodel_beretta_mp        = 6,
            viewmodel_colt45_mp         = 7,
            viewmodel_dragunov_mp       = 8,
            viewmodel_g3_mp             = 9,
            viewmodel_g36c_mp           = 10,
            viewmodel_m4_mp             = 11,
            viewmodel_m14_mp            = 12,
            viewmodel_m16_mp            = 13,
            viewmodel_m21_mp            = 14,
            viewmodel_m60_mp            = 15,
            viewmodel_m249_mp           = 16,
            viewmodel_miniuzi_mp        = 17,
            viewmodel_mp5_mp            = 18,
            viewmodel_mp44_mp           = 19,
            viewmodel_p90_mp            = 20,
            viewmodel_remington700_mp   = 21,
            viewmodel_rpd_mp            = 22,
            viewmodel_rpg7              = 23,
            viewmodel_rpg7_rocket       = 24,
            viewmodel_skorpion_mp       = 25,
            viewmodel_usp_mp            = 26,
            viewmodel_winchester1200_mp = 27,
        };

        enum SHADERGEN_VIEWMODEL_MAPPED_MATERIALS
        {
            mtl_weapon_desert_eagle_silver = 0,
            //viewmodel_m40a3_mp = 1,
            //viewmodel_ak47_mp = 2,
            //viewmodel_ak74u_mp = 3,
            //viewmodel_barrett_mp = 4,
            //viewmodel_benelli_m4_mp = 5,
            //viewmodel_beretta_mp = 6,
            //viewmodel_colt45_mp = 7,
            //viewmodel_dragunov_mp = 8,
            //viewmodel_g3_mp = 9,
            //viewmodel_g36c_mp = 10,
            //viewmodel_m4_mp = 11,
            //viewmodel_m14_mp = 12,
            //viewmodel_m16_mp = 13,
            //viewmodel_m21_mp = 14,
            //viewmodel_m60_mp = 15,
            //viewmodel_m249_mp = 16,
            //viewmodel_miniuzi_mp = 17,
            //viewmodel_mp5_mp = 18,
            //viewmodel_mp44_mp = 19,
            //viewmodel_p90_mp = 20,
            //viewmodel_remington700_mp = 21,
            //viewmodel_rpd_mp = 22,
            //viewmodel_rpg7 = 23,
            //viewmodel_rpg7_rocket = 24,
            //viewmodel_skorpion_mp = 25,
            //viewmodel_usp_mp = 26,
            //viewmodel_winchester1200_mp = 27,
        };

        struct shadergen_s
        {
            public string name;
            //public SHADERGEN_TYPE type;
            public string type;
            public string option;
        };

        private MainWindow main;
        private ShaderGenWindow_Object Model
        {
            get;
            set;
        }

        #region UI ++++++++++++++++++++++++++++++++++++++++++++++++

        bool needsVerification = false;
        public string g_materialTemplateForShader = "";

        List<string> combo_ShaderTypes = new List<string>();
        List<string> combo_ShaderOptions2D = new List<string>();
        List<string> combo_ShaderOptionsViewmodel = new List<string>();
        List<string> combo_ShaderOptionsXmodel = new List<string>();
        List<string> combo_ShaderOptionsWorld = new List<string>();
        List<string> combo_ShaderOptionsSky = new List<string>();

        void GetShaderTemplates()
        {
            if (Variables.strShaderGen_Techsets != "")
            {
                main.ClearConsole();
                main.WriteConsole("Gathering shader templates ...\n");
                main.WriteConsole("searching within <bin\\CoD4CompileTools\\ShaderGen\\techsets>\n");

                // get all techset files without .ext
                var techsets = Directory.GetFiles(Variables.strShaderGen_Techsets, "*.techset").
                                Select(filename => Path.GetFileNameWithoutExtension(filename)).
                                ToArray();

                // for each techset
                if (techsets.Length > 0)
                {
                    for (int i = 0; i < techsets.Length; i++)
                    {
                        // remove "shadergen_" prefix
                        // substring 0 is our type (shader types cannot use underscores)
                        // substring 1 should be our complete option

                        if(!techsets[i].Contains("shadergen_"))
                        {
                            main.WriteConsole("|-> WARNING: Skipping bad template [" + techsets[i] + "]");
                            continue;
                        }

                        var type = (techsets[i].Remove(0, 10)).Split(new[] { '_' }, 2);

                        if (type.Length < 2)
                        {
                            main.WriteConsole("|-> WARNING: Skipping bad template [" + techsets[i] + "]");
                            continue;
                        }

                        // CAPS
                        string strType = type[0].ToUpper();

                        // check if we already added this type
                        if (!combo_ShaderTypes.Contains(strType))
                        {
                            combo_ShaderTypes.Add(strType);
                            main.WriteConsole("|-> Added shader type [" + strType + "]");
                        }

                        // *
                        // options for shader type

                        string strOption = type[1].ToUpper();

                        switch(strType)
                        {
                            case "2D":
                                if (!combo_ShaderOptions2D.Contains(strOption)) 
                                {
                                    combo_ShaderOptions2D.Add(strOption);
                                    main.WriteConsole("|-> Added shader option [" + strOption + "] for shader type [2D]");
                                }

                                break;

                            case "VIEWMODEL":
                                if (!combo_ShaderOptionsViewmodel.Contains(strOption))
                                {
                                    combo_ShaderOptionsViewmodel.Add(strOption);
                                    main.WriteConsole("|-> Added shader option [" + strOption + "] for shader type [VIEWMODEL]");
                                }

                                break;

                            case "XMODEL":
                                if (!combo_ShaderOptionsXmodel.Contains(strOption))
                                {
                                    combo_ShaderOptionsXmodel.Add(strOption);
                                    main.WriteConsole("|-> Added shader option [" + strOption + "] for shader type [XMODEL]");
                                }

                                break;

                            case "WORLD":
                                if (!combo_ShaderOptionsWorld.Contains(strOption))
                                {
                                    combo_ShaderOptionsWorld.Add(strOption);
                                    main.WriteConsole("|-> Added shader option [" + strOption + "] for shader type [WORLD]");
                                }

                                break;

                            case "SKY":
                                if (!combo_ShaderOptionsSky.Contains(strOption))
                                {
                                    combo_ShaderOptionsSky.Add(strOption);
                                    main.WriteConsole("|-> Added shader option [" + strOption + "] for shader type [SKY]");
                                }

                                break;

                            default:
                                main.WriteConsole("|-> WARNING: Skipping unkown shader type [" + strType + "]");
                                continue;
                        }
                    }
                }

                shadergen_type.ItemsSource = combo_ShaderTypes;
                shadergen_type.Text = combo_ShaderTypes[0];

                // shader options are set automatically in Shadergen_type_SelectionChanged()
            }
        }

        // hacky, update Text Property in textbox "shadergen_name"
        private void hackyUpdate_ShaderGenText()
        {
            // hacky, update Text Property in textbox "shadergen_name"
            string orgText = shadergen_name.Text;
            shadergen_name.Text += " ";
            shadergen_name.Text = orgText;
        }

        // needed for live validation
        private void Shadergen_injectweapon_Click(object sender, RoutedEventArgs e)
        {
            Variables.ShaderGenUI_InjectChk = (bool)shadergen_injectweapon.IsChecked;
            hackyUpdate_ShaderGenText();
        }

        // disable injection controls
        private void disable_injection_settings()
        {
            needsVerification = false;

            shadergen_injectweapon.IsEnabled = false;
            shadergen_injectweapon_combo.IsEnabled = false;

            Variables.ShaderGenUI_InjectChkEnabled = false;
            hackyUpdate_ShaderGenText();
        }

        // UI shaderType selection changed
        private void Shadergen_type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem = shadergen_type.Items[shadergen_type.SelectedIndex].ToString();

            if (selectedItem == "2D")
            {
                shadergen_option.ItemsSource = combo_ShaderOptions2D;
                shadergen_option.Text = combo_ShaderOptions2D[0];

                disable_injection_settings();
            }

            else if (selectedItem == "VIEWMODEL")
            {
                needsVerification = true;

                shadergen_option.ItemsSource = combo_ShaderOptionsViewmodel;
                shadergen_option.Text = combo_ShaderOptionsViewmodel[0];

                shadergen_injectweapon.IsEnabled = true;
                shadergen_injectweapon_combo.IsEnabled = true;

                Variables.ShaderGenUI_InjectChkEnabled = true;
                hackyUpdate_ShaderGenText();
            }

            else if (selectedItem == "XMODEL")
            {
                shadergen_option.ItemsSource = combo_ShaderOptionsXmodel;
                shadergen_option.Text = combo_ShaderOptionsXmodel[0];

                disable_injection_settings();
            }

            else if (selectedItem == "WORLD")
            {
                shadergen_option.ItemsSource = combo_ShaderOptionsWorld;
                shadergen_option.Text = combo_ShaderOptionsWorld[0];

                disable_injection_settings();
            }

            else if (selectedItem == "SKY")
            {
                shadergen_option.ItemsSource = combo_ShaderOptionsSky;
                shadergen_option.Text = combo_ShaderOptionsSky[0];

                disable_injection_settings();
            }
        }

        public ShaderGenWindow(MainWindow main)
        {
            InitializeComponent();
            this.main = main;

            // access to our viewModel
            Model = new ShaderGenWindow_Object();

            // gather shader templates for comboBoxes
            GetShaderTemplates();

            shadergen_injectweapon_combo.ItemsSource = Enum.GetValues(typeof(SHADERGEN_VIEWMODEL_WEAPONS));
            shadergen_injectweapon_combo.Text = Enum.GetName(typeof(SHADERGEN_VIEWMODEL_WEAPONS), 1);
        }

        // readme button
        private void ReadmeButtonClick(object sender, RoutedEventArgs e)
        {
            string readmePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\ShaderGen\\readme.txt";

            if (CheckFileExists(readmePath, "ERROR: Missing File \"bin/CoD4CompileTools/ShaderGen/readme.txt\""))
            {
                System.Diagnostics.Process.Start(readmePath);
            }
        }

        // validate input data
        private bool checkInput()
        {
            // if no verification is needed
            //if(!needsVerification) 
            //{
            //    return true;
            //}

            Variables.ShaderGenData_Name            = shadergen_name.Text;
            Variables.ShaderGenUI_InjectChk         = (bool)shadergen_injectweapon.IsChecked;
            Variables.ShaderGenUI_InjectChkEnabled  = (bool)shadergen_injectweapon.IsEnabled;

            return Model.isValid;
        }

        // remove default shadername text on first hover
        private void shadergen_name_OnFocus(object sender, RoutedEventArgs e)
        {
            if (shadergen_name.Text == "Shader Name Here") 
                shadergen_name.Text = "";
        }

        #endregion

        // generate shader button
        private async void CreateSampleShaderClick(object sender, RoutedEventArgs e)
        {
            if (MainWindow.TaskActive)
            {
                main.WriteConsole("ERROR: Another thread is still active!");
                return;
            }

            main.WriteConsole("CONSOLESTATUS: Generating Shader");

            main.ClearConsole();
            main.WriteConsole("COD4-SHADERGEN v0.1 (c) 2019 Bouncepatch.com :: #xoxor4d \n\n");

            hackyUpdate_ShaderGenText();

            // if data input is invalid or missing data, return
            if (!checkInput())
            {
                main.WriteConsole("ERROR: " + Variables.ShaderGenData_CurrentError);
                return;
            }

            shadergen_s shader;
            shader.name = shadergen_name.Text;
            shader.type = "_" + shadergen_type.Items[shadergen_type.SelectedIndex].ToString().ToLower();
            shader.option = "_" + shadergen_option.Items[shadergen_option.SelectedIndex].ToString().ToLower();

            main.WriteConsole("Name:\t\t" + shader.name + ";\nType:\t\t" + shader.type + ";\nOption:\t\t" + shader.option);

            string shadercreation_failed = "\nERROR: Shadercreation unsuccessful. Check log above.";

            bool shader_compiled = false;
            bool shader_injected = false;

            // reset global material template string
            g_materialTemplateForShader = "";

            main.WriteConsole("\n ---- Techsets ------------------------------------------------");
            if (create_techset(shader))
            {
                // techset created successfully

                main.WriteConsole("\n ---- Material Template ---------------------------------------");
                if (g_materialTemplateForShader != "")
                {
                    // copy the material template for the shader template (specified inside its techset)
                    copy_materialTemplate(g_materialTemplateForShader);
                }
                else
                {
                    main.WriteConsole("WARNING: No material template was specified inside techset ^");
                }

                main.WriteConsole("\n ---- Techniques ----------------------------------------------");
                if (create_technique(shader))
                {
                    // technique created successfully
                    
                    main.WriteConsole("\n ---- Shaders -------------------------------------------------");
                    if (create_shader(shader))
                    {
                        // hlsl created successfully

                        main.WriteConsole("\n:GREEN: Shader Generation Successful");
                        if (compile_shader(shader))
                        {
                            // shader compilation successful
                            shader_compiled = true;
                        }

                        // inject the shader
                        if (inject_weapon(shader))
                        {
                            // injected shader into material and changed material within xmodel
                            shader_injected = true;
                        }

                        // if we injected but did not compile the shader, notify the user
                        if(shader_injected && shader_injected != shader_compiled)
                        {
                            main.WriteConsole("\nWARNING: You injected the shader without compiling the shader. \nShould be fine if you compiled it yourself or in a previous run.");
                        }
                    }
                    else { main.WriteConsole(shadercreation_failed); }
                }
                else { main.WriteConsole(shadercreation_failed); }
            }
            else { main.WriteConsole(shadercreation_failed); }

            // appending async - redirected streams to a richtextbox is a pain
            if(shader_compiled)
            {
                await Task.Run(() =>
                {
                    while (!MainWindow.Task_FinishedPrints)
                    {
                        // run task till final process prints are done
                    }

                    CreateSampleShader_FinalPrints(shader, shader_injected);
                });
            }
            else
            {
                CreateSampleShader_FinalPrints(shader, shader_injected);
            }

            // Refresh ShaderSrc list
            fillShaderSrcList();

            // Clean up
            GC.Collect();
        }

        private void CreateSampleShader_FinalPrints(shadergen_s shader, bool shader_injected)
        {
            main.WriteConsole("\n ---- Asset Manager : Material Custom Settings ----------------");

            if (g_materialTemplateForShader != "")
            {
                main.WriteConsole(":: Template \t" + (g_materialTemplateForShader.EndsWith(".template")
                                                  ? ("\"" + g_materialTemplateForShader.Remove(g_materialTemplateForShader.Length - 9, 9) + "\"")
                                                  : "Something's wrong. Template name was: \"" + g_materialTemplateForShader + "\""));
            }
            else
            {
                main.WriteConsole(":: Template \t not specified!");
            }

            main.WriteConsole(":: String \t\"" + shader.name + "\"");

            // Final hint if we injected our shader
            if (shader_injected)
            {
                main.WriteConsole("\n ---- Shader Injection : Zone Data ----------------------------\n" +
                                  "material," + Variables.ShaderGenData_CurrentGenMaterialName + "\n" +
                                  "xmodel," + Variables.ShaderGenData_CurrentGenXmodelName + "\n");
            }

            main.WriteConsole("CONSOLESTATUS: DONE");
        }

        /// <summary>
        /// Get MaterialTemplate from first line. Write Header. Write dest file with replaced terms. No header on techsets (cod4rad does not support comments)
        /// </summary>
        /// <param name="templateFolderPath">Src: Path to folder</param>
        /// <param name="templateName">Src: FileName</param>
        /// <param name="outputFile">Dest: FilePath with FileName</param>
        /// <param name="searchTerm">Term to search for</param>
        /// <param name="replaceTerm">Replace term with</param>
        /// <returns>Material Template String (first line of file with format :: // # [template.template])</returns>
        private string WriteTextReplacedOutput_ToFile(string templateFolderPath, string templateName, string outputFile, bool replace, string searchTerm = "", string replaceTerm = "")
        {
            string tempLineValue, parsedTemplate = "";

            bool isTechset = templateName.Contains(".techset");
            bool tempFirstLine = true;

            using (FileStream inputStream = File.OpenRead(templateFolderPath + templateName))
            {
                using (StreamReader inputReader = new StreamReader(inputStream))
                {
                    using (StreamWriter outputWriter = File.AppendText(outputFile))
                    {
                        // techsets (atleast for cod4rad) do not support comments, so do not add any / remove the material template definition after we parsed it
                        if(!isTechset)
                        {
                            // write header
                            outputWriter.WriteLine("// * COD4-SHADERGEN - xoxor4d.github.io");
                            outputWriter.WriteLine("// * Template used : [" + templateName + "]");
                        }

                        while (null != (tempLineValue = inputReader.ReadLine()))
                        {
                            // check first line of file for a material template string
                            if(tempFirstLine)
                            {
                                // if file contains a template string
                                if(tempLineValue.StartsWith("// # ["))
                                {
                                    // remove pre and suffix
                                    parsedTemplate = tempLineValue.Remove(0, 6);
                                    parsedTemplate = parsedTemplate.Remove(parsedTemplate.Length - 1, 1);

                                    if(isTechset)
                                    {
                                        // do not write the template comment header on techsets
                                        continue;
                                    }
                                }

                                // prepend the material template if the file does not contain it (if we know it already)
                                else if(g_materialTemplateForShader != "" && !isTechset)
                                {
                                    outputWriter.WriteLine("// * Mat. Template : [" + g_materialTemplateForShader + "]");
                                    outputWriter.WriteLine("");
                                }

                                else
                                {
                                    outputWriter.WriteLine("");
                                }

                                // write the original first line if it does not contain a comment
                                if(!tempLineValue.Contains("/"))
                                {
                                    outputWriter.WriteLine(tempLineValue);
                                }
                                
                                tempFirstLine = false;
                            }
                            else
                            {
                                if(replace)
                                {
                                    outputWriter.WriteLine(tempLineValue.Replace(searchTerm, replaceTerm));
                                }
                                else
                                {
                                    outputWriter.WriteLine(tempLineValue);
                                }
                                
                            }
                        }
                    }
                }
            }

            return parsedTemplate;
        }

        // replace material name within xmodel
        bool xmodel_replaceTextBinary(string filePath, string oldText, string newText, string fileNameToPrint)
        {
            //// oldText + NUL ␀
            byte[] fileBytes = File.ReadAllBytes(filePath),
                    oldBytes = Encoding.ASCII.GetBytes(oldText + "\0"),
                    newBytes = Encoding.ASCII.GetBytes(newText + "\0");

            // get array with length == amount of strings found, each with its starting offset within the file
            int[] _index = IndexOfBytes(fileBytes, oldBytes);

            if (_index.Length == 0)
            {
                main.WriteConsole(":INFO: Could not find material: \"" + oldText + "\" in xmodel: \"" + fileNameToPrint + "\"");
                main.WriteConsole("=> looking if we already replaced \"" + oldText + "\" with our material.");

                // stock material was not found
                // check if we already placed our own materialname into our file
                byte[] _fileBytes = File.ReadAllBytes(filePath),
                        _oldBytes = Encoding.ASCII.GetBytes(newText + "\0");

                // get array with length == amount of strings found, each with its starting offset within the file
                _index = IndexOfBytes(_fileBytes, _oldBytes);

                if (_index.Length == 0)
                {
                    main.WriteConsole("ERROR: Could not find material: \"" + newText + "\" in xmodel: \"" + fileNameToPrint + "\"");
                    main.WriteConsole("=> Looks like you are using a modified file. ABORTING!");
                    return false;
                }

                main.WriteConsole(":INFO: Material: \"" + newText + "\" is already injected into xmodel: \"" + fileNameToPrint + "\"");
                return true;
            }

            byte[] newFileBytes = new byte[fileBytes.Length + newBytes.Length - oldBytes.Length];

            for(var amount = 0; amount < _index.Length; amount++)
            {
                // copy src into dest on the first run 
                if(amount == 0)
                {
                    Buffer.BlockCopy(fileBytes, 0, newFileBytes, 0, _index[amount]);
                    Buffer.BlockCopy(newBytes, 0, newFileBytes, _index[amount], newBytes.Length);
                    Buffer.BlockCopy(fileBytes, _index[amount] + oldBytes.Length,
                                     newFileBytes, _index[amount] + newBytes.Length,
                                     fileBytes.Length - _index[amount] - oldBytes.Length);
                }

                // then use newFileBytes as src and copy into newFileBytes, so we "add" and not replace
                else
                {
                    Buffer.BlockCopy(newFileBytes, 0, newFileBytes, 0, _index[amount]);
                    Buffer.BlockCopy(newBytes, 0, newFileBytes, _index[amount], newBytes.Length);
                    Buffer.BlockCopy(newFileBytes, _index[amount] + oldBytes.Length,
                                     newFileBytes, _index[amount] + newBytes.Length,
                                     newFileBytes.Length - _index[amount] - oldBytes.Length);
                }
            }

            // write the final file
            File.WriteAllBytes(filePath, newFileBytes);

            main.WriteConsole(":GREEN: Material: \"" + newText + "\" successfully injected into xmodel: \"" + fileNameToPrint + "\"");

            return true;
        }

        // replace techset in viewmodel material
        bool material_replaceTextBinary(string filePath, string oldText, string newText, string fileNameToPrint )
        {
            // oldText + NUL ␀
            byte[] fileBytes = File.ReadAllBytes(filePath),
                    oldBytes = Encoding.ASCII.GetBytes(oldText + "\0"),
                    newBytes = Encoding.ASCII.GetBytes(newText + "\0");

            // get array with length == amount of strings found, each with its starting offset within the file
            int[] _index = IndexOfBytes(fileBytes, oldBytes);

            if (_index.Length == 0)
            {
                main.WriteConsole(":INFO: Could not find techset: \"" + oldText + "\" in material: \"" + fileNameToPrint + "\"");
                main.WriteConsole("=> looking if we already replaced \"" + oldText + "\" with our techset.");

                // stock techset was not found
                // check if we already placed our current shadername into our file
                byte[] _fileBytes = File.ReadAllBytes(filePath),
                        _oldBytes = Encoding.ASCII.GetBytes(newText + "\0");

                // get array with length == amount of strings found, each with its starting offset within the file
                _index = IndexOfBytes(_fileBytes, _oldBytes);

                if (_index.Length == 0)
                {
                    main.WriteConsole("ERROR: Could not find techset: \"" + newText + "\" in material: \"" + fileNameToPrint + "\"");
                    main.WriteConsole("=> Looks like you are using a modified file. ABORTING!");
                    return false;
                }

                main.WriteConsole(":INFO: Techset: \"" + newText + "\" is already injected into material: \"" + fileNameToPrint + "\"\n");
                return true;
            }

            byte[] newFileBytes = new byte[fileBytes.Length + newBytes.Length - oldBytes.Length];

            for (var amount = 0; amount < _index.Length; amount++)
            {
                // copy src into dest on the first run 
                if (amount == 0)
                {
                    Buffer.BlockCopy(fileBytes, 0, newFileBytes, 0, _index[amount]);
                    Buffer.BlockCopy(newBytes, 0, newFileBytes, _index[amount], newBytes.Length);
                    Buffer.BlockCopy(fileBytes, _index[amount] + oldBytes.Length,
                                     newFileBytes, _index[amount] + newBytes.Length,
                                     fileBytes.Length - _index[amount] - oldBytes.Length);
                }

                // then use newFileBytes as src and copy into newFileBytes, so we "add" and not replace
                else
                {
                    Buffer.BlockCopy(newFileBytes, 0, newFileBytes, 0, _index[amount]);
                    Buffer.BlockCopy(newBytes, 0, newFileBytes, _index[amount], newBytes.Length);
                    Buffer.BlockCopy(newFileBytes, _index[amount] + oldBytes.Length,
                                     newFileBytes, _index[amount] + newBytes.Length,
                                     newFileBytes.Length - _index[amount] - oldBytes.Length);
                }
            }

            // write the final file
            File.WriteAllBytes(filePath, newFileBytes);

            main.WriteConsole(":GREEN: Techset: \"" + newText + "\" successfully injected into material: \"" + fileNameToPrint + "\"\n");

            return true;
        }

        int[] IndexOfBytes(byte[] searchBuffer, byte[] bytesToFind)
        {
            List<int> indexList = new List<int>();

            for (int currPos = 0; currPos < searchBuffer.Length - bytesToFind.Length; currPos++)
            {
                bool success = true;

                // look if string at currPos == the string we want to replace
                for (int j = 0; j < bytesToFind.Length; j++)
                {
                    // if we found it then we wont set success to false
                    if (searchBuffer[currPos + j] != bytesToFind[j])
                    {
                        success = false;
                        break;
                    }
                }

                // and we add the start offset of our string to our array
                if (success) 
                {
                    indexList.Add(currPos);
                }
            }

            return indexList.ToArray();
        }

        // inject our shader into a viewmodel, if supported
        private bool inject_weapon(shadergen_s shader)
        {
            if(!shadergen_injectweapon.IsEnabled || !(bool)shadergen_injectweapon.IsChecked) 
            {
                return false;
            }

            if(shader.name.Length != 11) 
            {
                main.WriteConsole("\nERROR: Techsets in viewmodel-materials use 11 chars\n" +
                                  "but your shader uses " + shader.name.Length + " chars!\n");
                return false;
            }

            main.WriteConsole("\n ---- Shader Injection ----------------------------------------");

            // weaponModel name in raw/xmodel
            string weaponModel = shadergen_injectweapon_combo.Text;

            // look if we remapped the material for the viewmodel we want / get weaponModel index in enum
            int weaponModelIndex = (int)Enum.Parse(typeof(SHADERGEN_VIEWMODEL_WEAPONS), weaponModel);

            // get remappedMaterial from SHADERGEN_VIEWMODEL_MAPPED_MATERIALS; string will be null if there is no remapped material
            string weaponMaterial = Enum.GetName(typeof(SHADERGEN_VIEWMODEL_MAPPED_MATERIALS), weaponModelIndex);

            // if we did not remap, weaponMaterial will be null
            if (weaponMaterial == null)
            {
                // weaponMaterial name in raw/materials
                // remove "_mp" suffix and replace prefix "viewmodel_" with "mtl_weapon_" 
                weaponMaterial = weaponModel.Replace("_mp", "");
                weaponMaterial = weaponMaterial.Replace("viewmodel_", "mtl_weapon_");
            }

            // copy material and only the filename of the custom material
            string customMaterialName = material_createCopy(Variables.strMaterials, weaponMaterial, weaponMaterial.Replace("mtl_", "gen_"));
            
            // file was not found when material_createCopy returns ""
            if (customMaterialName == "")
            {
                main.WriteConsole("\nERROR: Material: \"" + weaponMaterial + "\" not found!\nAborting! \n");
                return false;
            }

            // replace techset name in material
            if(!material_replaceTextBinary(Variables.strMaterials + customMaterialName, "l_sm_r0c0s0", shader.name, weaponMaterial.Replace("mtl_", "gen_")))
            {
                // if we didnt do anything to the file, remove it
                if (File.Exists(Variables.strMaterials + customMaterialName))
                {
                    try 
                    {
                        File.Delete(Variables.strMaterials + customMaterialName);
                    }
                    catch (Exception ex) 
                    {
                        main.WriteConsole("ERROR: " + ex.Message);
                    }
                }

                return false;
            }

            Variables.ShaderGenData_CurrentGenMaterialName = customMaterialName;

            // check if backup exists or create one
            if (!xmodel_createBackupOnce(Variables.strXmodel, weaponModel, weaponModel + ".back")) 
            {
                return false;
            }

            // replace materialname within the xmodel
            if (!xmodel_replaceTextBinary(Variables.strXmodel + weaponModel, weaponMaterial, customMaterialName, weaponModel))
            {
                // restore backup, because the user could have modified only 1 string of many that we maybe replaced in the process
                // if backup exists
                if (File.Exists(Variables.strXmodel + weaponModel + ".back"))
                {
                    // if stock file exists
                    if (File.Exists(Variables.strXmodel + weaponModel))
                    {
                        try
                        {
                            // delete our file
                            File.Delete(Variables.strXmodel + weaponModel);

                            // restore backup
                            File.Copy(Variables.strXmodel + weaponModel + ".back", Variables.strXmodel + weaponModel);
                        }
                        catch (Exception ex)
                        {
                            main.WriteConsole("ERROR: " + ex.Message);
                        }
                    }
                }

                return false;
            }

            Variables.ShaderGenData_CurrentGenXmodelName = weaponModel;

            // shader injections successful
            return true;
        }

        // run batch to compile our shader, redirect input to internal console
        private bool compile_shader(shadergen_s shader)
        {
            if((bool)shadergen_chk_compile.IsChecked)
            {
                //string compilerArguments = " \"" + Variables.strShaderBin + "\"" + " " + "\"" + shader.name;
                //main.createCompileProcess(compilerArguments, "cod4compiletools_compileshader_custom.bat", null, null, false);
                if(!main.compileShader(shader.name, false))
                {
                    main.WriteConsole("ERROR: Was unable to start the shader-compiling process!");
                }

                return true;
            }

            return false;
        }

        // create shader, return true if successful
        private bool create_shader(shadergen_s shader)
        {
            string shadergen_shader_src = System.AppDomain.CurrentDomain.BaseDirectory + "\\ShaderGen\\shaders\\";
            string shadergen_shader_raw = Variables.strShaderSrc;

            // Shadertype string _lowercase
            string shaderType = shader.type; //Enum.GetName(typeof(SHADERGEN_TYPE), shader.type).ToLower();

            // Shaderoption string _lowercase
            string shaderOptions = shader.option;

            string SourceFileNameVS = "vs_3_0_shadergen" + shaderType + shaderOptions + ".hlsl";
            string DestFileNameVS = "vs_3_0_" + shader.name + ".hlsl";

            string SourceFileNamePS = "ps_3_0_shadergen" + shaderType + shaderOptions + ".hlsl";
            string DestFileNamePS = "ps_3_0_" + shader.name + ".hlsl";

            // Check ShaderGen Directory and sample files
            if (!CheckDirExists(shadergen_shader_src, "ERROR: Missing Directory \"bin/CoD4CompileTools/ShaderGen/shaders\"") ||
                !CheckFileExists(shadergen_shader_src + SourceFileNameVS, "ERROR: Missing File \"bin/CoD4CompileTools/ShaderGen/shaders/" + SourceFileNameVS + "\"") ||
                !CheckFileExists(shadergen_shader_src + SourceFileNamePS, "ERROR: Missing File \"bin/CoD4CompileTools/ShaderGen/shaders/" + SourceFileNamePS + "\"")) {

                return false;
            }

            // create a backup if the file already exists
            // create our shader :: raw/shader_bin/shader_src
            string raw_shaderVS = shadergen_shader_raw + DestFileNameVS;
            CheckFileExists_CreateBackup(raw_shaderVS, "raw/shader_bin/shader_src/" + DestFileNameVS);

            string raw_shaderPS = shadergen_shader_raw + DestFileNamePS;
            CheckFileExists_CreateBackup(raw_shaderPS, "raw/shader_bin/shader_src/" + DestFileNamePS);

            WriteTextReplacedOutput_ToFile(
                /* SourceFileFolder */ shadergen_shader_src,
                /*  SourceFileName  */ SourceFileNameVS,
                /*   DestFilePath   */ shadergen_shader_raw + DestFileNameVS,
                /*     replace?     */ false);

            WriteTextReplacedOutput_ToFile(
                /* SourceFileFolder */ shadergen_shader_src,
                /*  SourceFileName  */ SourceFileNamePS,
                /*   DestFilePath   */ shadergen_shader_raw + DestFileNamePS,
                /*     replace?     */ false);

            // Copy instead
            //File.Copy(shadergen_shader_src + SourceFileNameVS, shadergen_shader_raw + DestFileNameVS);
            main.WriteConsole(":: \"raw/shader_bin/shader_src/" + DestFileNameVS + "\"");

            //File.Copy(shadergen_shader_src + SourceFileNamePS, shadergen_shader_raw + DestFileNamePS);
            main.WriteConsole(":: \"raw/shader_bin/shader_src/" + DestFileNamePS + "\"");

            return true;
        }

        // create techniques, return true if successful
        private bool create_technique(shadergen_s shader)
        {
            string shadergen_technique_src = System.AppDomain.CurrentDomain.BaseDirectory + "\\ShaderGen\\techniques\\";
            string shadergen_technique_raw = Variables.strTechniques;

            // Shadertype string _lowercase
            string shaderType = shader.type; //Enum.GetName(typeof(SHADERGEN_TYPE), shader.type).ToLower();

            // Shaderoption string _lowercase
            string shaderOptions = shader.option;

            string SourceFileName = "shadergen" + shaderType + shaderOptions + ".tech";
            string DestFileName = shader.name + ".tech";

            // Check ShaderGen Directory and sample file
            if (!CheckFileExists(shadergen_technique_src + SourceFileName, "ERROR: Missing File \"bin/CoD4CompileTools/ShaderGen/techniques/" + SourceFileName + "\"") ||
                !CheckDirExists(shadergen_technique_src, "ERROR: Missing Directory \"bin/CoD4CompileTools/ShaderGen\"")) {

                return false;
            }

            // create a backup if the file already exists
            // create our technique :: raw/technique
            string raw_technique = shadergen_technique_raw + DestFileName;
            CheckFileExists_CreateBackup(raw_technique, "raw/techniques/" + DestFileName);

            WriteTextReplacedOutput_ToFile(
                /* SourceFileFolder */ shadergen_technique_src,
                /*  SourceFileName  */ SourceFileName,
                /*   DestFilePath   */ shadergen_technique_raw + DestFileName,
                /*     replace?     */ true,
                /*  ReplaceStrings  */ "SHADER_NAME",
                /*  ReplacementStr  */ shader.name);

            main.WriteConsole(":: \"raw/techniques/" + DestFileName + "\"");

            return true;
        }

        private bool copy_materialTemplate(string templateFileName)
        {
            string shadergen_template_src = System.AppDomain.CurrentDomain.BaseDirectory + "\\ShaderGen\\templates\\";
            string shadergen_template_raw = Variables.strTemplates;

            if (!CheckFileExists(shadergen_template_src + templateFileName, "ERROR: Missing File \"bin/CoD4CompileTools/ShaderGen/templates/" + templateFileName + "\"") ||
                !CheckDirExists(shadergen_template_src, "ERROR: Missing Directory \"bin/CoD4CompileTools/ShaderGen/templates\""))
            {

                return false;
            }

            // Check if raw/techsets exists
            if (!CheckDirExists(shadergen_template_raw, "ERROR: Missing Directory \"deffiles/materials\""))
            {
                return false;
            }

            // create a backup if the file already exists
            CheckFileExists_CreateBackup(shadergen_template_raw + templateFileName, "deffiles/materials/" + templateFileName);

            File.Copy(shadergen_template_src + templateFileName, shadergen_template_raw + templateFileName);
            main.WriteConsole(":: \"deffiles/materials/" + templateFileName + "\"");

            return true;
        }

        // create techsets, return true if successful
        private bool create_techset(shadergen_s shader)
        {
            string shadergen_techset_src = System.AppDomain.CurrentDomain.BaseDirectory + "\\ShaderGen\\techsets\\";
            string shadergen_techset_raw = Variables.strTechsets;

            // Shadertype string _lowercase
            string shaderType = shader.type; //Enum.GetName(typeof(SHADERGEN_TYPE), shader.type).ToLower();

            // Shaderoption string _lowercase
            string shaderOptions = shader.option;

            string SourceFileName   = "shadergen" + shaderType + shaderOptions + ".techset";
            string DestFileName     = shader.name + ".techset";

            // Check ShaderGen Directory and sample file
            if (!CheckFileExists(shadergen_techset_src + SourceFileName, "ERROR: Missing File \"bin/CoD4CompileTools/ShaderGen/techsets/" + SourceFileName + "\"") ||
                !CheckDirExists(shadergen_techset_src, "ERROR: Missing Directory \"bin/CoD4CompileTools/ShaderGen\"")) {

                return false;
            }

            // Check if raw/techsets exists
            if (!CheckDirExists(shadergen_techset_raw, "ERROR: Missing Directory \"raw/techsets\""))
            {
                return false;
            }

            // create a backup if the file already exists
            // create our techset without prefixes first :: raw/techset
            string raw_techset = shadergen_techset_raw + DestFileName;
            CheckFileExists_CreateBackup(raw_techset, "raw/techsets/" + DestFileName);

            // replace shader name in techset and return the material template
            string templateFileStr = "";
            templateFileStr = WriteTextReplacedOutput_ToFile(
                /* SourceFileFolder */ shadergen_techset_src,
                /*  SourceFileName  */ SourceFileName,
                /*   DestFilePath   */ shadergen_techset_raw + DestFileName,
                /*     replace?     */ true,
                /*  ReplaceStrings  */ "SHADER_NAME",
                /*  ReplacementStr  */ shader.name);

            if(templateFileStr != "")
            {
                g_materialTemplateForShader = templateFileStr;
            }

            main.WriteConsole(":: \"raw/techsets/" + DestFileName + "\"");


            // Get prefix for shadertype (2D has none)
            string _prefix = "";

            //switch (shader.type)
            //{
            //    case SHADERGEN_TYPE._VIEWMODEL:
            //    case SHADERGEN_TYPE._XMODEL:
            //        _prefix = "mc_";
            //        break;

            //    case SHADERGEN_TYPE._SKY:
            //        _prefix = "wc_";
            //        break;

            //    case SHADERGEN_TYPE._2D:
            //    default:
            //        _prefix = "";
            //        break;  
            //}

            if(shader.type.StartsWith("_viewmodel") || shader.type.StartsWith("_xmodel"))
            {
                _prefix = "mc_";
            }
            else if(shader.type.StartsWith("_sky") || shader.type.StartsWith("_world"))
            {
                _prefix = "wc_";
            }

            // create a backup if the file already exists
            // copy the created "non-prefixed" techset into raw/techsets/sm2
            string raw_techset_SM2 = shadergen_techset_raw + "\\sm2\\" + DestFileName;
            CheckFileExists_CreateBackup(raw_techset_SM2, "raw/techsets/sm2/" + DestFileName);
         
            File.Copy(shadergen_techset_raw + DestFileName, raw_techset_SM2);
            main.WriteConsole(":: \"raw/techsets/sm2/" + DestFileName + "\"");

            // if the shader needs a prefix :: copy created techset, add prefix and place it into raw/techsets and raw/techsets/sm2
            if (_prefix != "")
            {
                // create a backup if the file already exists
                // add _prefix :: raw/techsets/sm2
                string raw_prefix_techset_sm2 = shadergen_techset_raw + "\\sm2\\" + _prefix + DestFileName;
                CheckFileExists_CreateBackup(raw_prefix_techset_sm2, "raw/techsets/sm2/" + _prefix + DestFileName);

                File.Copy(shadergen_techset_raw + DestFileName, raw_prefix_techset_sm2);
                main.WriteConsole(":: \"raw/techsets/sm2/" + _prefix + DestFileName + "\"");

                // create a backup if the file already exists
                // add _prefix :: raw/techsets
                string raw_prefix_techset = shadergen_techset_raw + _prefix + DestFileName;
                CheckFileExists_CreateBackup(raw_prefix_techset, "raw/techsets/" + _prefix + DestFileName);

                File.Copy(shadergen_techset_raw + DestFileName, raw_prefix_techset);
                main.WriteConsole(":: \"raw/techsets/" + _prefix + DestFileName + "\"");
            }

            return true;
        }

        // check if file exists, return true if successful, else prints error
        private bool CheckFileExists(string filePath, string printStr = "")
        {
            if (!File.Exists(filePath))
            {
                main.WriteConsole(printStr);
                return false;
            }

            return true;
        }

        // check if file exists, return true if successful, else prints error
        private bool CheckDirExists(string dirPath, string printStr = "")
        {
            if (!Directory.Exists(dirPath))
            {
                main.WriteConsole(printStr);
                return false;
            }

            return true;
        }

        // Moves existing file to same dir and adds ".back" as file ext.
        private bool CheckFileExists_CreateBackup(string filePath, string printStr)
        {
            // if the file we want to create exists
            if (File.Exists(filePath))
            {
                // if there is already a backup of that file, replace that backup file with the current version found
                if (File.Exists(filePath + ".back"))
                {
                    try 
                    {
                        File.Delete(filePath + ".back");
                    }
                    catch(Exception ex) 
                    {
                        main.WriteConsole("ERROR: " + ex.Message);
                    }

                    try 
                    {
                        File.Move(filePath, filePath + ".back");
                    }
                    catch (Exception ex)
                    {
                        main.WriteConsole("ERROR: " + ex.Message);
                    }
                }

                // if no backup exists, create one
                else
                {
                    try 
                    {
                        File.Move(filePath, filePath + ".back");
                    }
                    catch (Exception ex) 
                    {
                        main.WriteConsole("ERROR: " + ex.Message);
                    }
                }

                main.WriteConsole(":INFO: Created Backup: \"" + printStr + "\"");

                return true;
            }

            return false;
        }

        // create copy of material and return only the filename, not the path
        private string material_createCopy(string filePath, string oldFileName, string newFileName)
        {
            // return false if the file does not exist
            if (!File.Exists(filePath + oldFileName)) 
            {
                return "";
            }

            // if custom material does not exist
            if (!File.Exists(filePath + newFileName))
            {
                try
                {
                    // create it
                    File.Copy(filePath + oldFileName, filePath + newFileName);
                }
                catch (Exception ex)
                {
                    main.WriteConsole("ERROR: " + ex.Message);
                    return "";
                }
            }

            // if file exists already or got created, return its path+filename
            return newFileName;
        }

        // create a backup of the stock xmodel, but only once so we keep the stock file
        private bool xmodel_createBackupOnce(string filePath, string oldFileName, string newFileName)
        {
            // return false if the file does not exist
            if (!File.Exists(filePath + oldFileName))
            {
                return false;
            }

            // if no backup of xmodel exists
            if (!File.Exists(filePath + newFileName))
            {
                try
                {
                    // create it
                    File.Copy(filePath + oldFileName, filePath + newFileName);
                    main.WriteConsole(":INFO: Created Backup: \"raw/xmodel/" + newFileName + "\"");
                }
                catch (Exception ex)
                {
                    main.WriteConsole("ERROR: " + ex.Message);
                    return false;
                }
            }

            // backup created / exists already
            return true;
        }

        private void GLSL_to_HLSL_Click(object sender, RoutedEventArgs e)
        {
            SyntaxConverterWindow syntaxWindow = new SyntaxConverterWindow()
            {
                Owner = this
            };

            syntaxWindow.Show();
        }

        public void fillShaderSrcList()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Variables.strShaderSrc);
            
            if (!directoryInfo.Exists)
            {
                sgen_shaderLstBox.Items.Clear();
                main.WriteConsole("ERROR: Could not find shader_src directory [" + Variables.strShaderSrc + "]");
            }
            else
            {
                FileInfo[] fileInfoArray = directoryInfo.GetFiles();

                sgen_shaderLstBox.Items.Clear();
                Variables.strSelectedShaderSrcName = "";

                List<string> validShaders = new List<string>();

                int index = 0;
                while (index < fileInfoArray.Length)
                {
                    FileInfo fileInfo = fileInfoArray[index];
                    if (fileInfo.Name.EndsWith(".hlsl"))
                    {
                        //lstShaderSrcFiles.Items.Add(fileInfo.Name.Remove(checked(fileInfo.Name.Length - 5), 5));
                        validShaders.Add(fileInfo.Name.Remove(checked(fileInfo.Name.Length - 5), 5));
                    }

                    checked { ++index; }
                }

                var sortedShaders = from shader in validShaders
                                    orderby shader.Remove(0, 7)
                                    select shader;

                foreach (var element in sortedShaders)
                {
                    sgen_shaderLstBox.Items.Add(element);
                }
            }
        }

        private void sgen_compileSelectedShader_Click(object sender, RoutedEventArgs e)
        {
            string shaderName = Variables.strSelectedShaderSrcName;
            if(shaderName.StartsWith("ps_3_0_") || shaderName.StartsWith("vs_3_0_"))
            {
                shaderName = shaderName.Remove(0, 7);

                //string compilerArguments = " \"" + Variables.strShaderBin + "\"" + " " + shaderName;
                //main.createCompileProcess(compilerArguments, "cod4compiletools_compileshader_custom.bat", null, null, false);

                if (!main.compileShader(shaderName, false))
                {
                    main.WriteConsole("ERROR: Was unable to start the shader-compiling process!");
                }
            }
            else
            {
                main.WriteConsole("ERROR: Selection is not a valid shader.");
            }
        }

        private void sgen_openSelectedShader_Click(object sender, RoutedEventArgs e)
        {
            if (Variables.strSelectedShaderSrcName != "")
                System.Diagnostics.Process.Start(Variables.strShaderSrc + Variables.strSelectedShaderSrcName + ".hlsl");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fillShaderSrcList();
        }

        private void sgen_shaderLstBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sgen_shaderLstBox.SelectedIndex != -1)
                Variables.strSelectedShaderSrcName = StringType.FromObject(sgen_shaderLstBox.Items[sgen_shaderLstBox.SelectedIndex]);
        }

        private void folder_Templates_Click(object sender, RoutedEventArgs e)
        {
            if (Variables.strTemplates != "")
                System.Diagnostics.Process.Start(Variables.strTemplates);
        }

        private void folder_Techsets_Click(object sender, RoutedEventArgs e)
        {
            if (Variables.strTechsets != "")
                System.Diagnostics.Process.Start(Variables.strTechsets);
        }

        private void folder_Techniques_Click(object sender, RoutedEventArgs e)
        {
            if (Variables.strTechniques != "")
                System.Diagnostics.Process.Start(Variables.strTechniques);
        }

        private void folder_Statemaps_Click(object sender, RoutedEventArgs e)
        {
            if (Variables.strStatemaps != "")
                System.Diagnostics.Process.Start(Variables.strStatemaps);
        }

        private void folder_ShaderSrc_Click(object sender, RoutedEventArgs e)
        {
            if (Variables.strShaderSrc != "")
                System.Diagnostics.Process.Start(Variables.strShaderSrc);
        }

        private void refreshShaderLst_Click(object sender, RoutedEventArgs e)
        {
            fillShaderSrcList();
        }
    }
}
