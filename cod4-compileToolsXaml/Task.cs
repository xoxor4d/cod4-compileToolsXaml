using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Shell;
using System.Windows.Media.Imaging;
using System.IO;

namespace cod4_compileToolsXaml
{
    public partial class MainWindow
    {
        public static bool Task_FinishedPrints = false;
        public static bool TaskActive = false;
        public static Process execBatch;
        private long consoleTicksWhenLastFocus = DateTime.Now.Ticks;
        private DateTime consoleProcessStartTime;

        private void TimerTick(object sender, EventArgs e)
        {
            if (execBatch != null)
            {
                LauncherProcessTimeLabel.Visibility = Visibility.Visible;
                LauncherProcessTimeElapsedTextBox.Visibility = Visibility.Visible;
                LauncherProcessTimeElapsedTextBox.Content = (DateTime.Now - consoleProcessStartTime).ToString().Substring(0, 8);
            }
        }

        /// <summary>
        /// Task.cs \\ Compile Shader
        /// </summary>
        public bool compileShader(string shaderName, bool _ClearConsole = true)
        {
            if (TaskActive)
            {
                WriteConsole("ERROR: Thread still active!");

                Task_FinishedPrints = true;
                return false;
            }

            WriteConsole("\nCompiling Shader ....");
            WriteConsole("CONSOLESTATUS:COMPILE SHADER");

            Task_FinishedPrints = false;
            TaskActive = true;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += TimerTick;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();

            var compiler = new System.Diagnostics.ProcessStartInfo(Variables.strShaderBin + "shader_tool.exe");

            // StartInfo
            compiler.WorkingDirectory = Variables.strShaderBin;
            compiler.Arguments = shaderName;
            compiler.RedirectStandardOutput = true;
            compiler.RedirectStandardError = true;
            compiler.UseShellExecute = false;
            compiler.CreateNoWindow = true;

            //Dispatcher.Invoke(() =>
            //{
                execBatch = new System.Diagnostics.Process();
            //});
            
            execBatch.EnableRaisingEvents = true;
            execBatch.StartInfo = compiler;

            List<string> outputBuilder = new List<string>();
            List<string> errorBuilder = new List<string>();

            try
            {
                consoleProcessStartTime = DateTime.Now;

                // Event normal output
                execBatch.OutputDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        outputBuilder.Add(e.Data);
                    }
                };

                // Event error
                execBatch.ErrorDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        errorBuilder.Add(e.Data);
                    }
                };

                // Event exit
                execBatch.Exited += (s, e) =>
                {
                    SetCompilerCancelAbility(false);
                    TaskActive = false;
                    execBatch = null;
                };

                var isStarted = execBatch.Start();
                if (!isStarted)
                {
                    return false;
                }

                execBatch.BeginOutputReadLine();
                execBatch.BeginErrorReadLine();

                SetCompilerCancelAbility(true);

                Task.Run(() => 
                {
                    while(execBatch != null)
                    {
                        // run task till process exits
                    }

                    WriteConsole("\n");

                    for (int o = 0; o < outputBuilder.Count; o++)
                    {
                        WriteConsole(outputBuilder[o]);
                    }

                    for (int e = 0; e < errorBuilder.Count; e++)
                    {
                        WriteConsole(errorBuilder[e]);
                    }

                    Task_FinishedPrints = true;
                });

                return true;
            }

            catch
            {
                SetCompilerCancelAbility(false);
                TaskActive = false;
                Task_FinishedPrints = true;
                execBatch = null;

                return false;
            }
        }

        /// <summary>
        /// Task.cs \\ Create Compiler
        /// </summary>
        public void createCompileProcess(string compileOptions, string fileName, [Optional] string otherPath, [Optional] string workingDirPath, bool _ClearConsole = true)
        {
            if (TaskActive)
            {
                WriteConsole("ERROR: Thread still active!");
                return;
            }

            TaskActive = true;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += TimerTick;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();

            string customFilePath = Variables.strWorkingDir;
            string customWorkDir = Variables.strWorkingDir;

            // If other paths defined
            if (otherPath != null)
            {
                customFilePath = otherPath;
            }
                
            if (workingDirPath != null)
            {
                customWorkDir = workingDirPath;
            }

            string command = "/c ";

            var openBatch = new System.Diagnostics.ProcessStartInfo(customFilePath + "\\" + fileName + " ", command);

            // StartInfo
            openBatch.WorkingDirectory = customWorkDir;
            openBatch.Arguments = compileOptions;
            openBatch.RedirectStandardOutput = true;
            openBatch.RedirectStandardError = true;
            openBatch.UseShellExecute = false;
            openBatch.CreateNoWindow = true;

            execBatch = new System.Diagnostics.Process();

            execBatch.EnableRaisingEvents = true;
            execBatch.StartInfo = openBatch;

            try
            {
                consoleProcessStartTime = DateTime.Now;

                // Event output to console                    
                execBatch.OutputDataReceived += (sender, args) => WriteConsole(args.Data);
               
                // Event error
                execBatch.ErrorDataReceived += (sender, args) => WriteConsole(args.Data);
               
                // Event exit
                execBatch.Exited += (sender, args) =>
                {
                    SetCompilerCancelAbility(false);
                    TaskActive = false;
                    execBatch = null;
                };

                if (_ClearConsole)
                {
                    ClearConsole();
                }
                
                execBatch.Start();
                execBatch.BeginErrorReadLine();
                execBatch.BeginOutputReadLine();
                
                SetCompilerCancelAbility(true);
            }

            catch
            {
                SetCompilerCancelAbility(false);
                WriteConsole("ERROR: Can not find " + fileName + " in " + Variables.strWorkingDir);
                TaskActive = false;
                execBatch = null;
            }
        }

        /// <summary>
        /// Task.cs \\ Enable / Disable Compiler Cancel Button
        /// </summary>
        public void SetCompilerCancelAbility( bool state, [Optional] bool ignoreClick )
        {
            if (state)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    CancelCompilerButtonImage.Source = new BitmapImage(new Uri(@"/Resources/cancel.png", UriKind.Relative));

                    if (ignoreClick)
                    {
                        CancelCompilerButton.IsEnabled = false;
                    }
                    else
                    {
                        CancelCompilerButton.IsEnabled = true;
                    }
                }));
            }

            else
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    CancelCompilerButton.IsEnabled = false;
                    CancelCompilerButtonImage.Source = new BitmapImage(new Uri(@"/Resources/checked.png", UriKind.Relative));
                    label_cmpStatus.Content = "DONE";
                }));
            }
        }

        public void CancelCompiler_Click(object sender, RoutedEventArgs e)
        {
            if (!execBatch.HasExited)
            {
                execBatch.Kill();
                execBatch = null;
            }

            bool killedAny = false;

            Process[] processlist = Process.GetProcesses();
            foreach (Process theprocess in processlist)
            {
                if (theprocess.ProcessName == "cod4map" || theprocess.ProcessName == "cod4rad" || theprocess.ProcessName == "converter" || theprocess.ProcessName == "linker_pc")
                {
                    theprocess.Kill();
                    killedAny = true;
                }
            }

            if (killedAny)
            {
                SetCompilerCancelAbility(false);
                WriteConsole("ERROR: Canceled compilation!");
                TaskActive = false;
            }

            else
            {
                WriteConsole("WARNING: Nothing To Cancel!");
            }
        }

        private string[] stringErrorArray = 
        {
            "ERROR:",
            "******* leaked *******",
            "WROTE BSP LEAKFILE",
            "MAX_MAP_LIGHTBYTES",
            "compilation failed;",
            ": error "
        };

        private string[] stringWarningArray = 
        {
            "WARNING:",
            ":INFO:",
            "is missing",
            "Could not load file",
            "not found",
            "): warning"
        };

        private string[] stringGreenArray = 
        {
            ":GREEN:",
            "SUCCESS:",
            "DONE:",
            "Writing ",
            "writing ",
            "Finished "
        };

        private string[] fatalError = 
        {
            "Linker will now terminate",
            "UNRECOVERABLE",
            "(!)"
        };

        private string[] stringStatusArray = 
        {
            "CONSOLESTATUS:"
        };

        private void AppendColoredText( string text, string color )
        {
            BrushConverter bc = new BrushConverter();
            TextRange textRange = new TextRange(ConsoleBox2.Document.ContentEnd, ConsoleBox2.Document.ContentEnd);

            textRange.Text = text + "\n";

            try 
            {
                textRange.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertFromString(color));
            }
            catch (FormatException) 
            { }
        }

        /// <summary>
        /// Task.cs \\ Append to Console
        /// </summary>
        public void WriteConsole(string s)
        {
            if (s == null)
            {
                return;
            }
                
            long ticks = DateTime.Now.Ticks;
            bool doFocus = ticks - consoleTicksWhenLastFocus > 10000000L;

            if (doFocus)
            {
                consoleTicksWhenLastFocus = ticks;
            }

            ConsoleBox2.Dispatcher.Invoke(new Action(() =>
            {
                if (stringErrorArray.Any(s.Contains))
                {
                    AppendColoredText(s, "Red");
                }

                else if (stringWarningArray.Any(s.Contains))
                {
                    if (s.Contains(":INFO: "))
                    {
                        s = s.Replace(":INFO: ", "");
                    }
                        
                    AppendColoredText(s, "#FFDE8300"); //"Yellow");
                }

                else if (stringGreenArray.Any(s.Contains))
                {
                    if (s.Contains(":GREEN: "))
                    {
                        s = s.Replace(":GREEN: ", "");
                    }

                    AppendColoredText(s, "Green");
                }
                    
                else if (stringStatusArray.Any(s.Contains))
                {
                    s = s.Replace("CONSOLESTATUS:", "");
                    label_cmpStatus.Content = s;
                    s = "";
                }

                else
                {
                    AppendColoredText(s, "WhiteSmoke");
                }

                ConsoleBox2.ScrollToEnd();
            }));
        }

        /// <summary>
        /// Task.cs \\ Clear Console
        /// </summary>
        public void ClearConsole()
        {
            splash = false;

            ConsoleBox2.Document.FontSize = 11;
            ConsoleBox2.Document.TextAlignment = TextAlignment.Left;

            ConsoleBox2.SelectAll();
            ConsoleBox2.Selection.Text = "";
        }
    }
}
