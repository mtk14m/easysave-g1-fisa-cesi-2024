﻿using EasySave_v1._0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave_v1._1.Models
{
    internal class DailyLogger
    {
        private readonly string logFilePath;

        public DailyLogger(string logFilePath)
        {
            this.logFilePath = logFilePath;
        }

        public void LogDailyBackup(BackupJob backupJob)
        {
            try
            {
                // Créer un objet de journal quotidien
                var dailyLogEntry = new DailyLogEntry
                {
                    Name = backupJob.Name,
                    FileSource = backupJob.SourceDirectory,
                    FileTarget = backupJob.TargetDirectory,
                    FileSize = GetFileSize(backupJob.TargetDirectory),
                    FileTransferTime = GetFileTransferTime(backupJob.StartTime, DateTime.Now),
                    Time = DateTime.Now
                };

                // Sérialiser l'objet en JSON
                string jsonLogEntry = Newtonsoft.Json.JsonConvert.SerializeObject(dailyLogEntry);

                // Écrire le JSON dans le fichier de journal
                File.AppendAllText(logFilePath, jsonLogEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging daily backup: {ex.Message}");
            }
        }

        private long GetFileSize(string filePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                return fileInfo.Length;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting file size: {ex.Message}");
                return 0;
            }
        }

        private double GetFileTransferTime(DateTime startTime, DateTime endTime)
        {
            TimeSpan transferTime = endTime - startTime;
            return transferTime.TotalSeconds;
        }
    }

    // Modèle pour le journal quotidien
    internal class DailyLogEntry
    {
        public string Name { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public long FileSize { get; set; }
        public double FileTransferTime { get; set; }
        public DateTime Time { get; set; }
    }

}
