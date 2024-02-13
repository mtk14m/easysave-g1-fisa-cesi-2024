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
            InitializeComponent();

            //Pour avoir la MainPage en page principale
            navframe.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
        }

        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if(sender == sidebar)
            {
                var selected = sidebar.SelectedItem as NavButton;
                navframe.Navigate(selected?.Navlink);
            }

        }

        private void logo_Click(object sender, RoutedEventArgs e)
        {
            
            navframe.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
            sidebar.SelectedIndex = -1;
        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            
            navframe.Navigate(new Uri("/Pages/SettingsPage.xaml", UriKind.Relative));
            sidebar.SelectedItem = null;
        }
    }
}