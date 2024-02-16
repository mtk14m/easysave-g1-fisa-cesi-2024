using EasySave_v1._0.Models;
using EasySave_v1._0.Packages;
using EasySave_v1._1.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace EasySave_v1._0.Controllers
{
    internal class BackupController
    {
        private List<BackupJob> backupJobs;
        private readonly string jsonPath = Path.Combine(Environment.CurrentDirectory, "backupjobs.json");
        private const int MaxJobs = 5;

        public static string logType ="json";

        // Injecter LanguageManager dans FileCopier
        private LanguageManager languageManager;
        private FileCopier fileCopier;

        private DailyLogger dailyLogger = new DailyLogger(Path.Combine(Environment.CurrentDirectory, "../../../Logs", "daily_log"));
        private StateLogger stateLogger = new StateLogger(Path.Combine(Environment.CurrentDirectory, "../../../Logs", "state_log"));

        public BackupController(LanguageManager manager)
        {
            backupJobs = LoadBackupJobsFromJson();
            languageManager = manager;
            fileCopier = new FileCopier(languageManager); // Injecter languageManager dans FileCopier
        }

        public void AddBackupJob(BackupJob job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            //ValidateBackupJob(job).Wait();

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

        private async Task ValidateBackupJob(BackupJob job)
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

            // lancer la copie
            await fileCopier.CopyFilesAsync(job);

            // Logger pour l'état
            await stateLogger.LogStateAsync(job, logType);

            // Logger quotidien
             dailyLogger.LogDailyBackup(job, logType);
        }

        public void ExecuteBackup(string backupRange)
        {
            string[] backupRanges = backupRange.Split(';');

            foreach (string range in backupRanges)
            {
                string[] indices = range.Split('-');

                if (indices.Length == 1) // Plage discrète
                {
                    if (!int.TryParse(indices[0].Trim(), out int index))
                    {
                        Console.WriteLine($"Invalid backup index: {indices[0]}");
                        continue;
                    }

                    if (index < 1 || index > backupJobs.Count)
                    {
                        Console.WriteLine($"Backup index out of range: {index}");
                        continue;
                    }

                    try
                    {
                        BackupJob backupJob = backupJobs[index - 1]; 
                        ValidateBackupJob(backupJob).Wait();
                        Console.WriteLine($"Backup job {index} executed successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error executing backup job {index}: {ex.Message}");
                    }
                }
                else if (indices.Length == 2) // Plage continue
                {
                    if (!int.TryParse(indices[0].Trim(), out int startIndex) || !int.TryParse(indices[1].Trim(), out int endIndex))
                    {
                        Console.WriteLine($"Invalid backup range: {range}");
                        continue;
                    }

                    if (startIndex < 1 || endIndex < 1 || startIndex > backupJobs.Count || endIndex > backupJobs.Count || startIndex > endIndex)
                    {
                        Console.WriteLine($"Invalid backup range: {range}");
                        continue;
                    }

                    for (int i = startIndex; i <= endIndex; i++)
                    {
                        try
                        {
                            BackupJob backupJob = backupJobs[i - 1]; 
                            ValidateBackupJob(backupJob).Wait();
                            Console.WriteLine($"Backup job {i} executed successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error executing backup job {i}: {ex.Message}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid backup range: {range}");
                }
            }
        }

        public void ChangeLogType(int typeChoose)
        {
            if (typeChoose == 0) logType = "json";

            if (typeChoose == 1) logType = "xml";

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
