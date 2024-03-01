using System;
using System.ComponentModel;
using System.IO;

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
        private State jobState;
        // Informations de progression
        public int TotalFiles { get; set; }
        public int CopiedFiles { get; set; }
        public int RemainingFiles { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public TimeSpan RemainingTime { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan TimePerFile { get; set; }

        public State JobState
        {
            get => jobState;
            set
            {
                if (value != jobState)
                {
                    jobState = value;
                    OnPropertyChanged(nameof(JobState));
                }
            }
        }

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

        public void UpdateProgress(int copiedFiles, int totalFiles, DateTime startTime, State jobState)
        {
            JobState = jobState;

            if (jobState == State.Active)
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
                    ProgressPercentage = 100;
                }
            }
            else if (jobState == State.Stopped)
            {
                CopiedFiles = 0;
                TotalFiles = 0;
                RemainingFiles = 0;
                ElapsedTime = TimeSpan.Zero;
                TimePerFile = TimeSpan.Zero;
                RemainingTime = TimeSpan.Zero;
                ProgressPercentage = 0;
            }
        }


        public void Stop()
        {
            JobState = State.Stopped;
        }

        private volatile bool isPaused;
        private volatile bool isStopped;
        private volatile bool isRunning;

        public void Pause()
        {
            isPaused = true;
        }

        public void Resume()
        {
            isPaused = false;
            isRunning = true;
            ChangeState();
        }

        public void Cancel()
        {
            ProgressPercentage = 0;
            isStopped = true;
            isRunning = false;
            ChangeState();
        }

        public bool IsPaused()
        {
            ChangeState();
            return isPaused;
        }

        public void finished()
        {
            ProgressPercentage = 100;
            isRunning = false;
            isPaused = false;
            isStopped = false;
            JobState = State.Completed;
            ChangeState();
        }
        public void ChangeState()
        {
            if (isPaused)
            {
                JobState = State.Paused;
            }
            else if (isStopped)
            {
                JobState = State.Stopped;
            }
            else if (isRunning)
            {
                JobState = State.Active;
            }
        }

        public bool IsStopped()
        {
            ChangeState();
            return isStopped;
        }

        public bool IsRunning()
        {
            ChangeState();
            return isRunning;
        }

        public void Play()
        {
            if (isPaused)
            {
                isPaused = false;
                isStopped = false;
                isRunning = true;
                UpdateProgress(CopiedFiles, TotalFiles, StartTime, JobState);
            }
            else
            {
                isRunning = true;
                isPaused = false;
                isStopped = false;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
