using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace cod4_compileToolsXaml
{
    public partial class MainWindow
    {
        public void onAppLoad()
        {
            setButtonIcons();
            Variables.allowSaveLastMap = false;

            if (!loadUserSettings())
            {
                Variables.strGameMpName = "iw3mp.exe";
                Variables.strRadiantName = "CoD4Radiant.exe";

                LoadDefaultUserSettings();
                fillMapList();
            }

            setLanguage();

            Variables.allowSaveLastMap = true;
            gridOption.SelectedIndex = 0;
        }

        private void setButtonIcons()
        {
            RadiantIco.Source   = new BitmapImage(getIconPath("radiant.png"));
            AssetIco.Source     = new BitmapImage(getIconPath("assetmanager.png"));
            FxIco.Source        = new BitmapImage(getIconPath("fxed.png"));
            ConverterIco.Source = new BitmapImage(getIconPath("converter.png"));

            ShaderGenIco.Source = new BitmapImage(getIconPath("shadergen.png")); //
            SettingsIco.Source  = new BitmapImage(getIconPath("settings.png"));
            AboutIco.Source     = new BitmapImage(getIconPath("about.png"));
        }

        private Uri getIconPath( string iconName )
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\" + iconName;
            Uri resourcePath = new Uri(path);

            if(checkFileExists(path))
            {
                return resourcePath;
            }
            else
            {
                WriteConsole("\nCouldn't find icon \"" + path + "\"");
                return new Uri(@"Resources\cancel.png", UriKind.Relative);
            }
        }

        public void fillMapList()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Variables.strMapSourcePath + "\\");

            if (!directoryInfo.Exists)
            {
                lstMapFiles.Items.Clear();
            }
            else
            {
                FileInfo[] files = directoryInfo.GetFiles();

                lstMapFiles.Items.Clear();

                Variables.selectedMap_Name = "";
                Variables.selectedMap_Index = -1;

                FileInfo[] fileInfoArray = files;
                int index = 0;

                while (index < fileInfoArray.Length)
                {
                    FileInfo fileInfo = fileInfoArray[index];

                    if (fileInfo.Name.EndsWith(".map"))
                    {
                        lstMapFiles.Items.Add((object)fileInfo.Name.Remove(checked(fileInfo.Name.Length - 4), 4));
                    }
 
                    checked { ++index; }
                }
            }
        }
    }
}
