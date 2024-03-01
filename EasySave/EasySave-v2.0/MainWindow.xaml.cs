using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasySave_v2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            
            bool isNewInstance;
            Mutex mutex = new Mutex(true, "EasySave-v2.0", out isNewInstance);

            if (!isNewInstance)
            {
                MessageBox.Show("L'application est déjà en cours d'exécution.");
                Application.Current.Shutdown(); // Fermer cette instance de l'application
            }

            InitializeComponent();

            frameContent.Navigate(new Uri("/Pages/BackupsListPage.xaml", UriKind.Relative));
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
                frameContent.Navigate(new Uri("/Pages/BackupsListPage.xaml", UriKind.Relative));

            }
            if (sender == rdAdd)
            {
                frameContent.Navigate(new Uri("/Pages/AddBackupJobPage.xaml", UriKind.Relative));

            }
            if (sender == rdSettings)
            {
                frameContent.Navigate(new Uri("/Pages/SettingsPage.xaml", UriKind.Relative));

            }

        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        

    }
}