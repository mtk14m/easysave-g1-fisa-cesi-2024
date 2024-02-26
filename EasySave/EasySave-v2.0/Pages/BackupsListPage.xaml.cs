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
        //private List<int> selectedIndexes;
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

        // Logique GetLog

        // Logique pour la pagination
        private void pagination(object sender, RoutedEventArgs e)
        {
            if (sender == btnNext)
            {
                viewModel.NextPage();
                RefreshBindings();
            }
            else if (sender == btnPrevious)
            {
                viewModel.PreviousPage();
                RefreshBindings();
            }

            collectionViewSource.View.Refresh(); // Rafraîchir la vue après avoir changé de page
        }

        //Refreshh
        private void RefreshBindings()
        {
            backupsListBox.ItemsSource = null;
            backupsListBox.ItemsSource = viewModel.BackupJobs;
        }


        //gestion des click sur les boutons

        private void btnClicked(object sender, RoutedEventArgs e)
        {
            //exécuter le backup
            if (sender == btnStart)
            {
                if (DataContext is BackupListViewModel viewModel)
                {
                    List<int> selectedIndexes = viewModel.GetSelectedIndexes(backupsListBox);
                    viewModel.ExecuteBackups(selectedIndexes);
                }
                for (int i = 0; i< backupsListBox.Items.Count; i++)
                {

                }
            }

            //supprimer les backups
            if (sender == btnDelete)
            {
                if (DataContext is BackupListViewModel viewModel)
                {
                    List<int> selectedIndexes = viewModel.GetSelectedIndexes(backupsListBox);
                    viewModel.DeleteBackups(selectedIndexes);
                }
            }
        }

        //gestion des checked
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (DataContext is BackupListViewModel viewModel)
            {
                
                List<int> selectedIndexes = viewModel.GetSelectedIndexes(backupsListBox);
                foreach (int index in selectedIndexes)
                {
                    Console.WriteLine($"Element at index {index} is selected.");
                }
            }

            //gerer la visibilité des boutons

        }


    }
}
