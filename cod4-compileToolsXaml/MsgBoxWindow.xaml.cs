using System.Diagnostics;
using System.Windows;

namespace cod4_compileToolsXaml
{
    public partial class MsgBoxWindow : Window
    {
        public string msgbox_msg { get; set; }

        /// <summary>
        /// MsgBoxWindow.xaml.cs \\
        /// </summary>
        public MsgBoxWindow(string msgbox_msg, bool okOnly = false)
        {
            InitializeComponent();
            msgTextbox.Text = msgbox_msg;

            if (okOnly)
            {
                CancelButton.IsEnabled = false;
                CancelButton.Visibility = Visibility.Hidden;
                OkButton.Width = 370;
            }

            else
            {
                OkButton.Width = 182;
                CancelButton.IsEnabled = true;
                CancelButton.Visibility = Visibility.Visible;
            }
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
    }
}
