using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EasySave_v2._0.Models
{
    internal class BackupJob : INotifyPropertyChanged
    {
        private double progressPercentage;

        public string Name { get; set; }
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
        public string Type { get; set; }
        public DateTime LastRun { get; set; }
        public State JobState { get; set; }

        // Informations de progression
        public int TotalFiles { get; set; }
        public int CopiedFiles { get; set; }
        public int RemainingFiles { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public TimeSpan RemainingTime { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan TimePerFile { get; set; }

        public double ProgressPercentage
        {
            get => progressPercentage;
            set
            {
                if (value != progressPercentage)
                {
                    progressPercentage = value;
                    OnPropertyChanged(nameof(ProgressPercentage));
                }
            }
        }

        // Méthode pour mettre à jour les informations de progression
        public void UpdateProgress(int copiedFiles, int totalFiles, DateTime startTime)
        {
            CopiedFiles = copiedFiles;
            TotalFiles = totalFiles;
            RemainingFiles = totalFiles - copiedFiles;
            ElapsedTime = DateTime.Now - startTime;

            if (TotalFiles > 0)
            {
                TimePerFile = TimeSpan.FromTicks(ElapsedTime.Ticks / TotalFiles);
                RemainingTime = TimeSpan.FromTicks(RemainingFiles * TimePerFile.Ticks);
                ProgressPercentage = Math.Round((double)copiedFiles / totalFiles * 100);
            }
            else
            {
                TimePerFile = TimeSpan.Zero;
                RemainingTime = TimeSpan.Zero;
                ProgressPercentage = 100; // Si TotalFiles est 0, la progression est complète
            }
        }

        // Implementation de INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
