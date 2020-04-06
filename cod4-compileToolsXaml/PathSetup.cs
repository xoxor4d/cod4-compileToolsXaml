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
        /// PathSetup.cs \\ Update Paths - Global Variables
        /// </summary>
        public void updateLocalPaths()
        {
            string treePath = Variables.strTreePath;

            Variables.strBinPath = treePath + "bin\\";
            Variables.strMainPath = treePath + "main\\";
            Variables.strPCMapsPath = treePath + "raw\\maps\\";
            Variables.strPCMapsPath_MP = treePath + "raw\\maps\\mp\\";
            Variables.strMapSourcePath = treePath + "map_source\\";
            Variables.strPCGamePath = treePath;
            Variables.strZoneSourcePath = treePath + "zone_source\\";
            Variables.strWorkingDir = System.AppDomain.CurrentDomain.BaseDirectory + "\\";

            // ShaderGen
            Variables.strTemplates      = treePath + "deffiles\\materials\\";
            Variables.strStatemaps      = treePath + "raw\\statemaps\\";
            Variables.strTechsets       = treePath + "raw\\techsets\\";
            Variables.strTechsetsSM2    = treePath + "raw\\techsets\\sm2\\";
            Variables.strTechniques     = treePath + "raw\\techniques\\";
            Variables.strShaderBin      = treePath + "raw\\shader_bin\\";
            Variables.strShaderSrc      = treePath + "raw\\shader_bin\\shader_src\\";
            Variables.strMaterials      = treePath + "raw\\materials\\";
            Variables.strXmodel         = treePath + "raw\\xmodel\\";

            Variables.strShaderGen = System.AppDomain.CurrentDomain.BaseDirectory + "\\ShaderGen\\";
            Variables.strShaderGen_Techsets = Variables.strShaderGen + "techsets\\";
        }
    }
}
