using EasySave_v1._0.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EasySave_v1._1.Models
{
    internal class StateLogger
    {
        private readonly string logFilePath;

        public StateLogger(string logFilePath)
        {
            this.logFilePath = logFilePath;
        }

        public async Task LogStateAsync(BackupJob backupJob , string logType)
        {
            try
            {
                // Créer un objet de journal d'état
                var stateLogEntry = new StateLogEntry
                {
                    Name = backupJob.Name,
                    SourceFilePath = backupJob.SourceDirectory,
                    TargetFilePath = backupJob.TargetDirectory,
                    State = backupJob.JobState.ToString(),
                    TotalFilesToCopy = backupJob.TotalFiles,
                    TotalFilesSize = backupJob.CopiedFiles,
                    NbFilesLeftToDo = backupJob.RemainingFiles,
                    Progression = backupJob.ProgressPercentage
                };
/*
                // Sérialiser l'objet en JSON
                string jsonLogEntry = Newtonsoft.Json.JsonConvert.SerializeObject(stateLogEntry);

                // Écrire le JSON dans le fichier de journal de manière asynchrone
                await File.AppendAllTextAsync(logFilePath, jsonLogEntry + Environment.NewLine);*/

                if (logType == "json")
                {
                    // Sérialiser l'objet en JSON
                    string jsonLogEntry = Newtonsoft.Json.JsonConvert.SerializeObject(stateLogEntry);

                    // Écrire le JSON dans le fichier de journal
                    await File.AppendAllTextAsync($"{logFilePath}.json", jsonLogEntry + Environment.NewLine);

                }

                if (logType == "xml")
                {
                    // Sérialiser l'objet en XML
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(StateLogEntry));
                    using (StringWriter writer = new StringWriter())
                    {
                        xmlSerializer.Serialize(writer, stateLogEntry);

                        // Écrire le XML dans le fichier de journal
                        await File.AppendAllTextAsync($"{logFilePath}.xml", writer.ToString() + Environment.NewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging state: {ex.Message}");
            }
        }
    }

    // Modèle pour le journal d'état
    internal class StateLogEntry
    {
        public string Name { get; set; }
        public string SourceFilePath { get; set; }
        public string TargetFilePath { get; set; }
        public string State { get; set; }
        public int TotalFilesToCopy { get; set; }
        public int TotalFilesSize { get; set; }
        public int NbFilesLeftToDo { get; set; }
        public double Progression { get; set; }
    }
}
