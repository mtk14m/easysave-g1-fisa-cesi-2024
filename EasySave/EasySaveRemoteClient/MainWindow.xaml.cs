using System.Diagnostics;
using System.Runtime;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasySaveRemoteClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            frameContent.Navigate(new Uri("login.xaml", UriKind.Relative));
            rdShow.IsChecked = true;
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {

            foreach (Process process in Process.GetProcesses())
            {
                if (process.ProcessName == "EasySave-v2.0")
                {
                    process.Kill(); // Terminer le processus
                }
            }

            Close(); // Fermer l'application
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void pageNavigation(object sender, RoutedEventArgs e)
        {
            /* if(sender == rdMenu)
             {
                 frameContent.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));

             }*/
            if (sender == rdShow)
            {
                frameContent.Navigate(new Uri("login.xaml", UriKind.Relative));

            }
            if (sender == rdAdd)
            {
                frameContent.Navigate(new Uri("manage.xaml", UriKind.Relative));

            }

        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}