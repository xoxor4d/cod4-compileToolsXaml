using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace cod4_compileToolsXaml
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool splash = false;

        public MainWindow()
        {
            InitializeComponent();

            ConverterIco.Visibility = Visibility.Hidden;

            Title = String.Format("cod4-compileToolsXaml - Version {0} / xoxor4d.github.io", Assembly.GetExecutingAssembly().GetName().Version);

            ConsoleBox2.Document.FontSize = 13;

            WriteConsole("\n\n\n\n\n\n\n\n");
            WriteConsole("cod4-compileToolsXaml");
            WriteConsole(String.Format("Version {0} / xoxor4d.github.io", Assembly.GetExecutingAssembly().GetName().Version));

            onAppLoad();

            // reduce lag on resize with full RichTextBox
            SizeChanged += OnSizeChanged;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LauncherProcessTimeLabel.Visibility = Visibility.Hidden;
            LauncherProcessTimeElapsedTextBox.Visibility = Visibility.Hidden;

            splash = true;
        }

        private Timer _timer;
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (splash)
            {
                ClearConsole();
            }

            // user started resizing - increase page width
            ConsoleBox2.Document.PageWidth = 1000;

            // start again if already defined
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }

            // this code will run 100ms after user stopped resizing
            _timer = new Timer(_ =>
            {
                // reset page width back to allow wrapping algorithm to execute
                Dispatcher.Invoke(new Action(() =>
                {
                    ConsoleBox2.Document.PageWidth = double.NaN;
                }));
            }, null, 100, Timeout.Infinite);
        }

        private void RadiantClick(object sender, RoutedEventArgs e)
        {
            if (Variables.strRadiantName != "" && Variables.strRadiantName.EndsWith(".exe"))
            {
                runProcess(Variables.strBinPath + Variables.strRadiantName, Variables.strBinPath, "");
            } 
            else
            {
                runProcess(Variables.strBinPath + "CoD4Radiant.exe", Variables.strBinPath, "");
            }
        }

        private void AssetManagerClick(object sender, RoutedEventArgs e)
        {
            runProcess(Variables.strBinPath + "asset_manager.exe", Variables.strBinPath, "");
        }

        private void FxEdClick(object sender, RoutedEventArgs e)
        {
            runProcess(Variables.strBinPath + "CoD4EffectsEd.exe", Variables.strBinPath, "");
        }

        private void ConverterClick(object sender, RoutedEventArgs e)
        {
            runProcess(Variables.strWorkingDir + "cod4compiletools_convert.bat", Variables.strWorkingDir, "\"" + Variables.strBinPath + "\"");
        }

        private void BtnCompile_Click(object sender, RoutedEventArgs e)
        {
            if (TaskActive)
            {
                return;
            }

            buildBSP();
        }

        private void BtnReflections_Click(object sender, RoutedEventArgs e)
        {
            if (TaskActive)
            {
                return;
            }

            buildReflections();
        }

        private void BtnBuildFastFile_Click(object sender, RoutedEventArgs e)
        {
            if (TaskActive)
            {
                return;
            }

            buildFastFile();
        }

        private void BtnMissingAssets_Click(object sender, RoutedEventArgs e)
        {
            AssetsWindow assetsWindow = new AssetsWindow()
            {
                Owner = this
            };

            DimBox.Visibility = Visibility.Visible;
            assetsWindow.ShowDialog();
            DimBox.Visibility = Visibility.Hidden;
        }

        private void btnGridClick(object sender, RoutedEventArgs e)
        {
            if (TaskActive)
            {
                return;
            }

            buildGrid();
        }

        private void Copy_iwiToIWD_Click(object sender, RoutedEventArgs e)
        {
            if (TaskActive)
            {
                return;
            }

            buildIWD();
        }

        private void btnRunMap_Click(object sender, RoutedEventArgs e)
        {
            if (TaskActive)
            {
                return;
            }

            runGame();
        }

        private void refreshMapList_Click(object sender, RoutedEventArgs e)
        {
            fillMapList();
        }

        private void LstMapFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstMapFiles.SelectedIndex != -1)
            {
                Variables.selectedMap_Index = lstMapFiles.SelectedIndex;
                Variables.selectedMap_Name = StringType.FromObject(lstMapFiles.Items[Variables.selectedMap_Index]);

                saveUserSettings();
                btnRunMap.IsEnabled = true;
                btnGrid.IsEnabled = true;

                if (CheckSavedMap(Variables.selectedMap_Name))
                {
                    LoadSavedMap(Variables.selectedMap_Name);
                    return;
                }
            }
            else
            {
                btnRunMap.IsEnabled = false;
                btnGrid.IsEnabled = false;
            }

            resetCompileOptions();
        }

        private void ShaderGenClick(object sender, RoutedEventArgs e)
        {
            ShaderGenWindow shaderGenWindow = new ShaderGenWindow(this)
            {
                Owner = this
            };

            shaderGenWindow.Show();
        }

        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow()
            {
                Owner = this
            };

            settingsWindow.txtTreePath.Text = Variables.strTreePath;
            settingsWindow.copy_mapToUsermaps.IsChecked = Variables.copyToUsermaps;

            DimBox.Visibility = Visibility.Visible;
            settingsWindow.ShowDialog();
            DimBox.Visibility = Visibility.Hidden;
        }

        private void AboutClick(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow()
            {
                Owner = this
            };

            aboutWindow.VersionLabel.Content = String.Format("Version: {0}", Assembly.GetExecutingAssembly().GetName().Version);

            DimBox.Visibility = Visibility.Visible;
            aboutWindow.ShowDialog();
            DimBox.Visibility = Visibility.Hidden;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            saveUserSettings();
        }
    }
}
