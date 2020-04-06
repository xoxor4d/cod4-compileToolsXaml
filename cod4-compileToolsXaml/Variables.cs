using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.IO;
using Microsoft.VisualBasic.CompilerServices;

namespace cod4_compileToolsXaml
{
    /// <summary>
    /// Variables.cs \\ Setup Global Variables
    /// </summary>
    internal sealed class Variables
    {
        public static string strTreePath;
        public static string strMapSourcePath;
        public static string strPCMapsPath;
        public static string strPCMapsPath_MP;
        public static string strBinPath;
        public static string strZoneSourcePath;
        public static string strWorkingDir;
        public static string strMainPath;
        public static string strPCGamePath;

        public static string strRadiantName;
        public static string strGameMpName;

        // ShaderGen
        public static string strTemplates;
        public static string strStatemaps;
        public static string strTechsets;
        public static string strTechsetsSM2;
        public static string strTechniques;
        public static string strShaderBin;
        public static string strShaderSrc;
        public static string strMaterials;
        public static string strXmodel;

        public static string strShaderGen;
        public static string strShaderGen_Techsets;

        public static string selectedMap_Name;
        public static string language;
        public static int selectedMap_Index;
        public static bool allowSaveLastMap;
        public static bool copyToUsermaps;

        public static string strSelectedShaderSrcName;

        public static bool checkFileExists(string strFileName)
        {
            return StringType.StrCmp(strFileName, "", false) != 0 && StringType.StrCmp(FileSystem.Dir(strFileName, FileAttribute.Normal), "", false) != 0;
        }

        public static bool ShaderGenUI_InjectChk;
        public static bool ShaderGenUI_InjectChkEnabled;

        public static string ShaderGenData_Name;
        public static string ShaderGenData_CurrentError;
        public static string ShaderGenData_CurrentGenMaterialName;
        public static string ShaderGenData_CurrentGenXmodelName;

        public static double SyntaxConverterWidth;
        public static double SyntaxConverterHeight;
        public static double SyntaxConverterFontSize;
        public static bool SyntaxConverterOptimise;
    }
}
