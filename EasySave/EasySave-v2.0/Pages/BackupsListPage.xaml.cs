using EasySave_v2._0.Models;
using EasySave_v2._0.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace EasySave_v2._0.Pages
{
    /// <summary>
    /// Interaction logic for BackupsListPage.xaml
    /// </summary>
    public partial class BackupsListPage : Page
    {
        private BackupListViewModel viewModel;
        private CollectionViewSource collectionViewSource;

        public BackupsListPage()
        {
            InitializeComponent();

            // Initialiser le ViewModel avant de le définir comme DataContext
            viewModel = new BackupListViewModel();
            DataContext = viewModel;

            // Charger les backups après l'initialisation du ViewModel
            viewModel.LoadBackupJobsFromJson();

            // Créer une CollectionViewSource avec la liste des backups
            collectionViewSource = new CollectionViewSource();
            collectionViewSource.Source = viewModel.BackupJobs;

            // Appliquer un filtre pour la pagination
            collectionViewSource.Filter += (sender, e) =>
            {
                // Créer une liste indexée à partir de la vue de la source de collection
                var indexedList = collectionViewSource.View.Cast<object>().ToList();

                // Obtenir l'index de l'élément dans la liste indexée
                var index = indexedList.IndexOf(e.Item);

                // Vérifier si l'index de l'élément se trouve dans la plage de la page actuelle
                e.Accepted = index >= viewModel.PageSize * viewModel.PageIndex && index < viewModel.PageSize * (viewModel.PageIndex +1);
            };

            // Utiliser la CollectionViewSource pour la pagination
            backupsListBox.ItemsSource = collectionViewSource.View;
        }

        private void AddBackup_Click(object sender, RoutedEventArgs e)
        {
            // Implémenter la logique pour ajouter une sauvegarde (backup)
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            // Implémenter la logique pour démarrer les sauvegardes sélectionnées
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            // Implémenter la logique pour mettre en pause les sauvegardes sélectionnées
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            // Implémenter la logique pour arrêter les sauvegardes sélectionnées
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // Implémenter la logique pour supprimer les sauvegardes sélectionnées
        }

        // Logique GetLog

        // Logique pour la pagination
        private void pagination(object sender, RoutedEventArgs e)
        {
            if(sender == btnNext)
            {
                viewModel.NextPage();
            }
            else if(sender == btnPrevious) { 
            
                viewModel.PreviousPage();
            }
            collectionViewSource.View.Refresh();
            backupsListBox.ItemsSource = collectionViewSource.View; 

        }

        //gestion des click sur les boutons

        private void btnClicked(object sender, RoutedEventArgs e)
        {
            //TODO
        }
    }
}
