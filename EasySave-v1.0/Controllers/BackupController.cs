using EasySave_v1._0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EasySave_v1._0.Controllers
{
    internal class BackupController
    {
        private List<BackupJob> backupJobs;
        private readonly string jsonPath = Path.Combine(Environment.CurrentDirectory, "backupjobs.json");
        private const int MaxJobs = 5;

        public BackupController()
        {
            backupJobs = LoadBackupJobsFromJson();
        }

        public void AddBackupJob(BackupJob job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            ValidateBackupJob(job);

            backupJobs.Add(job);
            TrimBackupJobsIfNeeded();
            WriteBackupJobsToJson();
        }

        public List<BackupJob> GetAllBackupJobs()
        {
            return backupJobs;
        }

        private List<BackupJob> LoadBackupJobsFromJson()
        {
            try
            {
                if (File.Exists(jsonPath))
                {
                    string backupJobsJson = File.ReadAllText(jsonPath);
                    return JsonSerializer.Deserialize<List<BackupJob>>(backupJobsJson) ?? new List<BackupJob>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading backup jobs from JSON: {ex.Message}");
            }

            return new List<BackupJob>();
        }

        private void WriteBackupJobsToJson()
        {
            try
            {
                string backupJobsJson = JsonSerializer.Serialize(backupJobs, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(jsonPath, backupJobsJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing backup jobs to JSON: {ex.Message}");
            }
        }

        private void ValidateBackupJob(BackupJob job)
        {
            if (string.IsNullOrWhiteSpace(job.Name))
            {
                throw new ArgumentException("Backup job name cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(job.SourceDirectory))
            {
                throw new ArgumentException("Backup job source directory cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(job.TargetDirectory))
            {
                throw new ArgumentException("Backup job target directory cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(job.Type))
            {
                throw new ArgumentException("Backup job type cannot be empty.");
            }
        }

        private void TrimBackupJobsIfNeeded()
        {
            while (backupJobs.Count > MaxJobs)
            {
                backupJobs.RemoveAt(0);
            }
        }
    }
}
