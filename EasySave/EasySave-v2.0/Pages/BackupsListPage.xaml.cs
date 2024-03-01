using EasySave_v2._0.Models;
using EasySave_v2._0.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            //gerer la visibilité des boutons
            btnStart.IsEnabled = false;
            btnStart.Visibility = Visibility.Collapsed;
            btnDelete.IsEnabled = false;
            btnDelete.Visibility = Visibility.Collapsed;
            btnStop.IsEnabled = false;
            btnStop.Visibility = Visibility.Collapsed;
            btnPause.IsEnabled = false;
            btnPause.Visibility = Visibility.Collapsed;

            // Initialiser le ViewModel avant de le définir comme DataContext
            viewModel = new BackupListViewModel();
            DataContext = viewModel;

            SettingsViewModel settingsViewModel = new SettingsViewModel();
            settingsViewModel.LoadConfig();

            //config variables
            string dailyLogPath = settingsViewModel.DailyLogPath;
            string stateLogPath = settingsViewModel.StateLogPath;
            string configFilePath = settingsViewModel.ConfigFilePath;
            string language = settingsViewModel.Language;
            string logType = settingsViewModel.LogType;
            string extensionsToEncrypt = settingsViewModel.ExtensionsToEncrypt;
            //string priority = settingsViewModel.Priority;


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


            //gerer les JobState
            /*foreach (BackupJob backupJob in viewModel.BackupJobs)
            {
                backupJob.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "JobState")
                    {
                        BackupJob job = (BackupJob)sender;
                        job.ChangeState();
                        if (job.JobState == State.Stopped)
                        {
                            UpdateConsole($"Le backup {job.Name} a été arrêté.", ConsoleColor.Red);
                        }
                        else if (job.JobState == State.Paused)
                        {
                            UpdateConsole($"Le backup {job.Name} a été mis en pause.", ConsoleColor.Yellow);
                        }
                        else if (job.JobState == State.Active)
                        {
                            UpdateConsole($"Le backup {job.Name} a été démarré.", ConsoleColor.Green);
                        }
                    }
                };
            }*/
        }


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
                    foreach (int index in selectedIndexes)
                    {
                        // Assurez-vous que l'index est valide
                        if (index >= 0 && index < viewModel.BackupJobs.Count)
                        {
                            BackupJob backupJob = viewModel.BackupJobs[index];
                            backupJob.Play(); 
                            backupJob.ChangeState();
                        }
                    }
                    decocher(selectedIndexes);
                }

            }


            //supprimer les backups
            if (sender == btnDelete)
            {
                if (DataContext is BackupListViewModel viewModel)
                {
                    List<int> selectedIndexes = viewModel.GetSelectedIndexes(backupsListBox);
                    viewModel.DeleteBackups(selectedIndexes);
                    decocher(selectedIndexes);
                    
                    RefreshBindings();
                }
            }

            //stopper les backups
            if (sender == btnStop)
            {
                if (DataContext is BackupListViewModel viewModel)
                {
                    List<int> selectedIndexes = viewModel.GetSelectedIndexes(backupsListBox);

                    foreach (int index in selectedIndexes)
                    {
                        // Assurez-vous que l'index est valide
                        if (index >= 0 && index < viewModel.BackupJobs.Count)
                        {
                            BackupJob backupJob = viewModel.BackupJobs[index];
                            backupJob.Cancel(); // Annuler le backup
                            backupJob.ChangeState();
                        }
                    }
                    decocher(selectedIndexes);
                }
            }

            //mettre en pause les backups
            if (sender == btnPause)
            {
                if (DataContext is BackupListViewModel viewModel)
                {
                    List<int> selectedIndexes = viewModel.GetSelectedIndexes(backupsListBox);

                    foreach (int index in selectedIndexes)
                    {
                        
                        if (index >= 0 && index < viewModel.BackupJobs.Count)
                        {
                            BackupJob backupJob = viewModel.BackupJobs[index];
                            backupJob.Pause(); 
                            backupJob.ChangeState();
                        }
                    }
                    decocher(selectedIndexes);
                }
            }
        }

        private void decocher (List<int> selectedIndexes)
        {
            foreach (int index in selectedIndexes)
            {
                if (index >= 0 && index < backupsListBox.Items.Count)
                {
                    ListBoxItem listBoxItem = (ListBoxItem)backupsListBox.ItemContainerGenerator.ContainerFromIndex(index);
                    if (listBoxItem != null)
                    {
                        CheckBox checkBox = FindVisualChild<CheckBox>(listBoxItem);
                        if (checkBox != null)
                        {
                            checkBox.IsChecked = false;
                        }
                    }
                }
            }

            selectedIndexes.Clear();
        }

        //gestion des checked
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (DataContext is BackupListViewModel viewModel)
            {
                
                List<int> selectedIndexes = viewModel.GetSelectedIndexes(backupsListBox);
               if(selectedIndexes.Count > 0)
                {
                    btnStart.IsEnabled = true;
                    btnStart.Visibility = Visibility.Visible;
                    btnDelete.IsEnabled = true;
                    btnDelete.Visibility = Visibility.Visible;
                    btnStop.IsEnabled = true;
                    btnStop.Visibility = Visibility.Visible;
                    btnPause.IsEnabled = true;
                    btnPause.Visibility = Visibility.Visible;
                }
                else
                {
                    btnStart.IsEnabled = false;
                    btnStart.Visibility = Visibility.Collapsed;
                    btnDelete.IsEnabled = false;
                    btnDelete.Visibility = Visibility.Collapsed;
                    btnStop.IsEnabled = false;
                    btnStop.Visibility = Visibility.Collapsed;
                    btnPause.IsEnabled = false;
                    btnPause.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (DataContext is BackupListViewModel viewModel)
            {
                List<int> selectedIndexes = viewModel.GetSelectedIndexes(backupsListBox);
                if (selectedIndexes.Count > 0)
                {
                    btnStart.IsEnabled = true;
                    btnStart.Visibility = Visibility.Visible;
                    btnDelete.IsEnabled = true;
                    btnDelete.Visibility = Visibility.Visible;
                    btnStop.IsEnabled = true;
                    btnStop.Visibility = Visibility.Visible;
                    btnPause.IsEnabled = true;
                    btnPause.Visibility = Visibility.Visible;
                }
                else
                {
                    btnStart.IsEnabled = false;
                    btnStart.Visibility = Visibility.Collapsed;
                    btnDelete.IsEnabled = false;
                    btnDelete.Visibility = Visibility.Collapsed;
                    btnStop.IsEnabled = false;
                    btnStop.Visibility = Visibility.Collapsed;
                    btnPause.IsEnabled = false;
                    btnPause.Visibility = Visibility.Collapsed;
                }
            }
        }
        //fiind visual child
        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }

        //Visualiser les logs
        private void UpdateConsole(string message, ConsoleColor color)
        {

            Console.ForegroundColor = color;


            consoleTextBox.AppendText($"{message}\n");


            Console.ResetColor();
        }


        internal void DisplayInformationInConsole(BackupJob backupJob)
        {
            StringBuilder consoleMessage = new StringBuilder();

            // Construire le message à afficher en utilisant les propriétés de l'objet BackupJob
            consoleMessage.AppendLine($"Nom du Backup: {backupJob.Name}");
            consoleMessage.AppendLine($"Fichier Transféré: {backupJob.SourceDirectory}");
            consoleMessage.AppendLine($"Fichiers Transférés: {backupJob.CopiedFiles}");
            consoleMessage.AppendLine($"Fichiers Totals: {backupJob.TotalFiles}");

            UpdateConsole(consoleMessage.ToString(), ConsoleColor.Yellow);
        }
    }
}
