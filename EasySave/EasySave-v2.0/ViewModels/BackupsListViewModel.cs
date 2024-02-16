using EasySave_v2._0.Models;
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

namespace EasySave_v2._0.ViewModels
{
    public class BackupListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<BackupJob> _backupJobs;
        private List<BackupJob> backupJobs;

        private readonly string jsonPath = Path.Combine(Environment.CurrentDirectory, "../../../Ressources/backupjobs.json");

        public BackupListViewModel()
        {
            _backupJobs = new ObservableCollection<BackupJob>(); // Initialiser la collection BackupJobs
            LoadBackupJobsFromJson(); // Chargement des emplois de sauvegarde depuis le fichier JSON
            PageIndex = 0; // Initialiser PageIndex
            PageSize = 3; // Définir le nombre d'éléments par page
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

        public int TotalPages
        {
            get { return (int)Math.Ceiling((double)BackupJobs.Count / PageSize); }
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
                    BackupJobs = JsonSerializer.Deserialize<ObservableCollection<BackupJob>>(backupJobsJson) ?? new ObservableCollection<BackupJob>();
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
                string backupJobsJson = JsonSerializer.Serialize(BackupJobs, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(jsonPath, backupJobsJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing backup jobs to JSON: {ex.Message}");
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
                OnPropertyChanged(nameof(BackupJobs)); // Notifier la vue du changement dans BackupJobs
            }
        }

        public void NextPage()
        {
            if (PageIndex < TotalPages - 1)
            {
                PageIndex++;
                OnPropertyChanged(nameof(BackupJobs)); // Notifier la vue du changement dans BackupJobs
            }
        }

        //gestion de la visibilité des bouttons
        private bool _isBackupSelected;
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

        // Propriétés pour la visibilité des boutons en fonction de IsBackupSelected
        public Visibility IsStartButtonVisible => IsBackupSelected ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsPauseButtonVisible => IsBackupSelected ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsStopButtonVisible => IsBackupSelected ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsDeleteButtonVisible => IsBackupSelected ? Visibility.Visible : Visibility.Collapsed;

    }
}
