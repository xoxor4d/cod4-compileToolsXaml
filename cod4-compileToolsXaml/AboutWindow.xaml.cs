using System.Diagnostics;
using System.Windows;

namespace cod4_compileToolsXaml
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow() 
        {
            InitializeComponent();
        }

        private void HomePageButtonClick(object sender, RoutedEventArgs e) 
        {
            Process.Start("https://github.com/xoxor4d/cod4-compileToolsXaml/");
        }

        private void BouncepatchButtonClick(object sender, RoutedEventArgs e) 
        {
            Process.Start("https://xoxor4d.github.io/");
        }
    }
}
