using EasySave_v2._0.Models;
using EasySave_v2._0.Packages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace EasySave_v2._0.ViewModels
{
    public class BackupListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<BackupJob> _backupJobs;
        private List<BackupJob> backupJobs;

        private readonly string jsonPath = Path.Combine(Environment.CurrentDirectory, "../../../Ressources/backupjobs.json");

        private LanguageManager languageManager;
        private FileCopier fileCopier;

        private DailyLogger dailyLogger = new DailyLogger(Path.Combine(Environment.CurrentDirectory, "../../../Logs", "daily_log.json"));
        private StateLogger stateLogger = new StateLogger(Path.Combine(Environment.CurrentDirectory, "../../../Logs", "state_log.json"));

        // Déclaration de la minuterie
        private DispatcherTimer timer;

        public BackupListViewModel()
        {
            //load config
            SettingsViewModel settingsViewModel = new SettingsViewModel();
            settingsViewModel.LoadConfig();

            

            ////////////////////
            _backupJobs = new ObservableCollection<BackupJob>(); // Initialiser la collection BackupJobs
            LoadBackupJobsFromJson(); // Chargement des emplois de sauvegarde depuis le fichier JSON
            PageIndex = 0; // Initialiser PageIndex
            PageSize = 10; // Définir le nombre d'éléments par page
            fileCopier = new FileCopier(languageManager,settingsViewModel.ReturnSettings());

            // Initialisation de la minuterie
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Intervalle de rafraîchissement toutes les secondes
            timer.Tick += Timer_Tick;
            timer.Start();

            
        }

        // Gestionnaire d'événement de la minuterie
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Mettre à jour les propriétés liées à la vue
            OnPropertyChanged(nameof(Progress));
            OnPropertyChanged(nameof(BackupName));
            OnPropertyChanged(nameof(TransferredFileName));
            OnPropertyChanged(nameof(TransferredFilesCount));
            OnPropertyChanged(nameof(TotalFilesCount));
        }

        internal ObservableCollection<BackupJob> BackupJobs
        {
            get { return _backupJobs; }
            set
            {
                _backupJobs = value;
                OnPropertyChanged(nameof(BackupJobs));
            }
        }

        private int _pageIndex;
        public int PageIndex
        {
            get { return _pageIndex; }
            set
            {
                _pageIndex = value;
                OnPropertyChanged(nameof(PageIndex));

                // Rafraîchir la vue après avoir modifié PageIndex
                OnPropertyChanged(nameof(TotalPages));
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        public int PageSize { get; private set; }

        private int _totalPages;
        public int TotalPages
        {
            get { return _totalPages; }
            set
            {
                if (_totalPages != value)
                {
                    _totalPages = value;
                    OnPropertyChanged(nameof(TotalPages));
                }
            }
        }

        public int CurrentPage
        {
            get { return PageIndex + 1; }
        }

        public void LoadBackupJobsFromJson()
        {
            try
            {
                if (File.Exists(jsonPath))
                {
                    string backupJobsJson = File.ReadAllText(jsonPath);
                    var allBackupJobs = JsonSerializer.Deserialize<ObservableCollection<BackupJob>>(backupJobsJson) ?? new ObservableCollection<BackupJob>();

                    // Calculez le nombre total de pages en fonction du nombre total de jobs et de la taille de la page
                    TotalPages = (int)Math.Ceiling((double)allBackupJobs.Count / PageSize);

                    // Calculez l'index de départ et le nombre d'éléments à charger pour la page actuelle
                    int startIndex = PageIndex * PageSize;
                    int count = Math.Min(PageSize, allBackupJobs.Count - startIndex);

                    // Chargez uniquement les éléments de la page actuelle
                    BackupJobs.Clear(); // Effacer les anciens éléments
                    foreach (var job in allBackupJobs.Skip(startIndex).Take(count))
                    {
                        BackupJobs.Add(job); // Ajouter les éléments de la page actuelle
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading backup jobs from JSON: {ex.Message}");
            }
        }

        public void WriteBackupJobsToJson()
        {
            try
            {
                List<BackupJob> existingBackupJobs = new List<BackupJob>();
                if (File.Exists(jsonPath))
                {
                    string existingBackupJobsJson = File.ReadAllText(jsonPath);
                    existingBackupJobs = JsonSerializer.Deserialize<List<BackupJob>>(existingBackupJobsJson) ?? new List<BackupJob>();
                }

                existingBackupJobs.AddRange(_backupJobs);

                string backupJobsJson = JsonSerializer.Serialize(existingBackupJobs, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(jsonPath, backupJobsJson);
            }
            catch (Exception ex)
            {
                // Gérer l'exception si nécessaire
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void PreviousPage()
        {
            if (PageIndex > 0)
            {
                PageIndex--;
                LoadBackupJobsFromJson();
                OnPropertyChanged(nameof(BackupJobs));
            }
        }

        public void NextPage()
        {
            if (PageIndex < TotalPages - 1)
            {
                PageIndex++;
                LoadBackupJobsFromJson();
                OnPropertyChanged(nameof(BackupJobs));
            }
        }

        private bool _isBackupSelected = false;
        public bool IsBackupSelected
        {
            get { return _isBackupSelected; }
            set
            {
                _isBackupSelected = value;
                OnPropertyChanged(nameof(IsBackupSelected));

                // Rafraîchir la visibilité des boutons
                OnPropertyChanged(nameof(IsStartButtonVisible));
                OnPropertyChanged(nameof(IsPauseButtonVisible));
                OnPropertyChanged(nameof(IsStopButtonVisible));
                OnPropertyChanged(nameof(IsDeleteButtonVisible));
            }
        }

        public Visibility IsStartButtonVisible => IsBackupSelected ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsPauseButtonVisible => IsBackupSelected ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsStopButtonVisible => IsBackupSelected ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsDeleteButtonVisible => IsBackupSelected ? Visibility.Visible : Visibility.Collapsed;

        internal List<int> GetSelectedIndexes(ListView backupsListView)
        {
            List<int> selectedIndexes = new List<int>();

            // Parcourir chaque élément de la liste
            for (int i = 0; i < backupsListView.Items.Count; i++)
            {
                ListViewItem listViewItem = backupsListView.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                if (listViewItem != null)
                {
                    // Rechercher la case à cocher dans l'élément de la liste
                    CheckBox checkBox = FindVisualChild<CheckBox>(listViewItem);
                    if (checkBox != null && checkBox.IsChecked == true)
                    {
                        // Ajouter l'index de l'élément sélectionné à la liste
                        selectedIndexes.Add(i);
                    }
                }
            }

            return selectedIndexes;
        }

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

        public void ExecuteBackups(List<int> selectedIndexes)
        {
            foreach (int index in selectedIndexes)
            {
                if (index < 0 || index >= _backupJobs.Count)
                {
                    continue;
                }

                try
                {
                    BackupJob backupJob = _backupJobs[index];
                    Thread backupThread = new Thread(() =>
                    {
                        ValidateBackupJob(backupJob).GetAwaiter().GetResult();
                    });
                    backupThread.Start();
                }
                catch (Exception ex)
                {
                    // Gérer les exceptions ici
                }
            }
        }


        public void DeleteBackups(List<int> selectedIndexes)
        {
            if (selectedIndexes == null || selectedIndexes.Count == 0)
            {
                return;
            }

            try
            {
                // Supprimer les éléments sélectionnés en ordre décroissant pour éviter les erreurs d'index
                selectedIndexes.Sort();
                selectedIndexes.Reverse();

                List<BackupJob> existingBackupJobs = new List<BackupJob>();
                if (File.Exists(jsonPath))
                {
                    string existingBackupJobsJson = File.ReadAllText(jsonPath);
                    existingBackupJobs = JsonSerializer.Deserialize<List<BackupJob>>(existingBackupJobsJson) ?? new List<BackupJob>();
                }

                foreach (int selectedIndex in selectedIndexes)
                {
                    // Calculer l'index de l'élément à supprimer dans la liste complète
                    int indexToDelete = selectedIndex;

                    if (indexToDelete >= 0 && indexToDelete < existingBackupJobs.Count)
                    {
                        existingBackupJobs.RemoveAt(indexToDelete);
                    }
                }

                string backupJobsJson = JsonSerializer.Serialize(existingBackupJobs, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(jsonPath, backupJobsJson);

                // Mettre à jour l'affichage dans l'image (UI)
                // Supprimer les backups de la liste _backupJobs
                foreach (int selectedIndex in selectedIndexes)
                {
                    int indexToDelete = selectedIndex;

                    if (indexToDelete >= 0 && indexToDelete < _backupJobs.Count)
                    {
                        _backupJobs.RemoveAt(indexToDelete);
                    }
                }
            }
            catch (Exception ex)
            {
                // Gérer l'exception si nécessaire
            }
        }





        //Stopper la sauvegarde
        internal void StopBackup(BackupJob job)
        {
            job.Cancel();
        }

        //Pause de la sauvegarde
        internal void PauseBackup(BackupJob job)
        {
            job.Pause();
        }

        //Changer l'état de la sauvegarde avec la methode changeState de BackupJob
        internal void ChangeState(BackupJob job)
        {
            job.ChangeState();
        }



        private async Task ValidateBackupJob(BackupJob job)
        {
            try
            {
                fileCopier.CopyFiles(job);

                //Mettre à jour les informations de progression de la sauvegarde
                Progress = fileCopier.ProgressPercentage;
                BackupName = job.Name;
                TransferredFileName = job.Name;
                TransferredFilesCount = fileCopier.RemainingFiles;
                TotalFilesCount = fileCopier.RemainingFiles;

                //Les logs
                stateLogger.LogState(job);
                dailyLogger.LogDailyBackup(job);
                job.JobState = State.Completed;
            }
            catch (Exception ex)
            {
                job.JobState = State.Stopped;
                MessageBox.Show($"Erreur lors de la sauvegarde : {ex.Message}");
            }
        }

        public void CreateAndSaveBackupJob(string name, string source, string destination, string type)
        {
            // Créer un nouveau travail de sauvegarde avec les valeurs fournies
            BackupJob newBackupJob = new BackupJob
            {
                Name = name,
                SourceDirectory = source,
                TargetDirectory = destination,
                Type = type,
                JobState = State.Ready
            };
            _backupJobs.Add(newBackupJob);

            WriteBackupJobsToJson();
        }

        // Mettre à jour les informations
        private double progress;
        public double Progress
        {
            get { return progress; }
            set
            {
                if (progress != value)
                {
                    progress = value;
                    OnPropertyChanged(nameof(Progress));
                }
            }
        } 
        private string _backupName;
        public string BackupName
        {
            get { return _backupName; }
            set
            {
                if (_backupName != value)
                {
                    _backupName = value;
                    OnPropertyChanged(nameof(BackupName));
                }
            }
        }

        private string _transferredFileName;
        public string TransferredFileName
        {
            get { return _transferredFileName; }
            set
            {
                if (_transferredFileName != value)
                {
                    _transferredFileName = value;
                    OnPropertyChanged(nameof(TransferredFileName));
                }
            }
        }

        private int _transferredFilesCount;
        public int TransferredFilesCount
        {
            get { return _transferredFilesCount; }
            set
            {
                if (_transferredFilesCount != value)
                {
                    _transferredFilesCount = value;
                    OnPropertyChanged(nameof(TransferredFilesCount));
                }
            }
        }

        private int _totalFilesCount;
        public int TotalFilesCount
        {
            get { return _totalFilesCount; }
            set
            {
                if (_totalFilesCount != value)
                {
                    _totalFilesCount = value;
                    OnPropertyChanged(nameof(TotalFilesCount));
                }
            }
        }



        ///server de backup distants

        private BackupServer backupServer = new BackupServer();

        public async Task StartBackupServerAsync()
        {
            await Task.Run(() => backupServer.Start());
        }
    }
}
