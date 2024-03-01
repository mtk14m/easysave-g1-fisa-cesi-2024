using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Logique d'interaction pour login.xaml
    /// </summary>
    public partial class login : Page
    {
        public login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string serverIp = IP.Text; ; // Récupérer cette valeur depuis l'interface utilisateur
            int port = Port.Text; // Récupérer cette valeur depuis l'interface utilisateur
            RemoteViewModel remoteViewModel = new RemoteViewModel(serverIp, port);
        }
    }
}
