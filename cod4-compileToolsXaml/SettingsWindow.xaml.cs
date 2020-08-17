using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Diagnostics;
using System.Windows;
using WinForms = System.Windows.Forms;

namespace cod4_compileToolsXaml
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            txtTreePath.Text = Variables.strTreePath;
            txtLanguage.Content = "Language: " + Variables.language;
        }

        public void BrowseCoD4Click(object sender, RoutedEventArgs e )
        {
            var folderDialog = new WinForms.FolderBrowserDialog();

            folderDialog.ShowNewFolderButton = false;
            folderDialog.SelectedPath = System.AppDomain.CurrentDomain.BaseDirectory;

            //int num1 = (int)folderDialog.ShowDialog();
            folderDialog.ShowDialog();

            if (folderDialog.SelectedPath.Length == 0)
            {
                return;
            }

            if (folderDialog.SelectedPath.Length < 4 || StringType.StrCmp(FileSystem.Dir(folderDialog.SelectedPath + "\\iw3sp.exe", FileAttribute.Normal), "", false) == 0)
            {
                MessageBox.Show("Error: That path is not a valid CoD4 path.");
            }
            else
            {
                Variables.strTreePath = folderDialog.SelectedPath + "\\";
                txtTreePath.Text = Variables.strTreePath;

                (Owner as MainWindow).updateLocalPaths();
                (Owner as MainWindow).fillMapList();
                (Owner as MainWindow).saveUserSettings();
            }
        }

        private void txtExeCoD4Mp_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Variables.strGameMpName = txtExeCoD4Mp.Text;
        }

        private void TxtExeRadiant_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Variables.strRadiantName = txtExeRadiant.Text;
        }

        private void Copy_mapToUsermaps_Changed(object sender, RoutedEventArgs e)
        {
            Variables.copyToUsermaps = (bool)copy_mapToUsermaps.IsChecked;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Variables.strGameMpName != "")
            {
                txtExeCoD4Mp.Text = Variables.strGameMpName;
            }
            else
            {
                txtExeCoD4Mp.Text = "iw3mp.exe";
            }
                

            if (Variables.strRadiantName != "" && Variables.strRadiantName.EndsWith(".exe"))
            {
                txtExeRadiant.Text = Variables.strRadiantName;
            }
            else
            {
                txtExeRadiant.Text = "CoD4Radiant.exe";
            }
        }
    }
}
