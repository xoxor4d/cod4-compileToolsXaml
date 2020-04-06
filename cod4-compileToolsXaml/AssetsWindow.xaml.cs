using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Documents;

namespace cod4_compileToolsXaml
{
    public partial class AssetsWindow : Window
    {
        public string msgbox_msg { get; set; }
        public string strLevelCSVFileName;

        /// <summary>
        /// MsgBoxWindow.xaml.cs \\
        /// </summary>
        public AssetsWindow()
        {
            InitializeComponent();
        }

        private void OnZoneLoaded(object sender, RoutedEventArgs e)
        {
            new Thread(delegate ()
            {
                readMissingAssets();
                loadLevelCSV(Variables.strZoneSourcePath + Variables.selectedMap_Name + ".csv");

            }).Start();
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnSaveLevelCSVClick(object sender, RoutedEventArgs e) 
        {
            saveNewLevelCSV();
        }

        private void readMissingAssets()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                string str = Variables.strTreePath + "main\\" + "missingasset.csv";

                if (!Variables.checkFileExists(str))
                {
                    string msgbox_msg = "Can't find " + str + "\n" +
                                        "Try building the level fast file again. \n" +
                                        "It's possible that the zone file is up to date.";

                    MsgBoxWindow msgBoxWindow = new MsgBoxWindow(msgbox_msg, true) {
                        Owner = this
                    };

                    msgBoxWindow.ShowDialog();
                    Close();
                }
                else
                {
                    StreamReader streamReader = new StreamReader((Stream)File.OpenRead(str));

                    while (streamReader.Peek() != -1)
                    {
                        txtMissingAssets.AppendText(streamReader.ReadLine() + "\n\n");
                    }
                    
                    streamReader.Close();
                }
            }));
        }

        private void loadLevelCSV(string strLevelCSV)
        {
            strLevelCSVFileName = strLevelCSV;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Title = "Zone File Editor => " + Variables.selectedMap_Name + ".csv";

                if (!Variables.checkFileExists(strLevelCSVFileName))
                {
                    string msgbox_msg = "Can't find " + this.strLevelCSVFileName + "\n" +
                                        "You may need to try building the fast file first so a zone source file will be created.";

                    MsgBoxWindow msgBoxWindow = new MsgBoxWindow(msgbox_msg, true) {
                        Owner = this
                    };

                    msgBoxWindow.ShowDialog();
                    Close();
                }
                else
                {
                    // Using textbox because we need \n instead of \r\n
                    string zoneFile = File.ReadAllText(strLevelCSVFileName);
                    txtLevelCSV.Text = zoneFile;
                }
            }));
        }

        private void saveNewLevelCSV()
        {
            try
            {
                StreamWriter streamWriter = new StreamWriter((Stream)new FileStream(strLevelCSVFileName, FileMode.Create));

                streamWriter.Write(txtLevelCSV.Text); // Instead of WriteLine
                streamWriter.Close();

                Close();
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);

                string msgbox_msg = strLevelCSVFileName + " could not be successfully updated." + "\n" +
                                    "Make sure file wasn't removed and is not being used by something else.";

                MsgBoxWindow msgBoxWindow = new MsgBoxWindow(msgbox_msg, true) {
                    Owner = this
                };

                msgBoxWindow.ShowDialog();
                ProjectData.ClearProjectError();

                Close();
            }
        }
    }
}
