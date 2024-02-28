using EasySave_v2._0.Models;
using EasySave_v2._0.Pages;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasySave_v2._0.Packages
{
    internal class FileCopier
    {
        private LanguageManager translator;
        private Configuration configData;

        public double ProgressPercentage { get; private set; }
        public int RemainingFiles { get; private set; }
        public TimeSpan ElapsedTime { get; private set; }
        public TimeSpan TimePerFile { get; private set; }
        public TimeSpan RemainingTime { get; private set; }

        public FileCopier(LanguageManager languageManager, Configuration config)
        {
            translator = languageManager;
            configData = config;
        }

        public void CopyFiles(BackupJob backupJob)
        {
            Task.Run(() =>
            {
                string[] files = Directory.GetFiles(backupJob.SourceDirectory, "*.*", SearchOption.AllDirectories);
                int totalFiles = files.Length;
                int copiedFiles = 0;

                var priorityFiles = files.Where(file => configData.ExtensionsWithPriority.Contains(Path.GetExtension(file)));

                List<Task> copyTasks = new List<Task>();

                foreach (string file in priorityFiles)
                {
                    Task copyTask = Task.Run(() =>
                    {
                        string targetFilePath = file.Replace(backupJob.SourceDirectory, backupJob.TargetDirectory);

                        if (configData.ExtensionsToEncrypt.Contains(Path.GetExtension(file)))
                        {
                            EncryptFile(file);
                        }

                        File.Copy(file, targetFilePath, true);

                        Interlocked.Increment(ref copiedFiles);
                        backupJob.UpdateProgress(copiedFiles, totalFiles, DateTime.Now);
                    });

                    copyTasks.Add(copyTask);
                }

                var nonPriorityFiles = files.Except(priorityFiles);

                foreach (string file in nonPriorityFiles)
                {
                    Task copyTask = Task.Run(() =>
                    {
                        string targetFilePath = file.Replace(backupJob.SourceDirectory, backupJob.TargetDirectory);
                        if (configData.ExtensionsToEncrypt.Contains(Path.GetExtension(file)))
                        {
                            EncryptFile(file);
                        }
                        File.Copy(file, targetFilePath, true);

                        Interlocked.Increment(ref copiedFiles);
                        backupJob.UpdateProgress(copiedFiles, totalFiles, DateTime.Now);

                    });
                    copyTasks.Add(copyTask);
                }

                Task.WaitAll(copyTasks.ToArray());

                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Le backup '{backupJob.Name}' est terminé.", "Backup Terminé", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            });
        }

        //Chiffrer les fichiers
        private void EncryptFile(string filePath)
        {
            try
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                byte[] key = Encoding.UTF8.GetBytes("YourEncryptionKey"); // Clé de chiffrement XOR

                // Chiffrer chaque octet du fichier avec la clé
                for (int i = 0; i < fileBytes.Length; i++)
                {
                    fileBytes[i] = (byte)(fileBytes[i] ^ key[i % key.Length]);
                }

                // Écrire les octets chiffrés dans le fichier
                File.WriteAllBytes(filePath, fileBytes);
            }
            catch (Exception ex)
            {
                // Gérer l'exception si nécessaire
                MessageBox.Show($"Erreur lors du chiffrement du fichier : {ex.Message}");
            }
        }

        // Méthodes d'accès (getters) pour les informations de progression
        public double GetProgressPercentage()
        {
            return ProgressPercentage;
        }

        public int GetRemainingFiles()
        {
            return RemainingFiles;
        }

        public TimeSpan GetElapsedTime()
        {
            return ElapsedTime;
        }

        public TimeSpan GetTimePerFile()
        {
            return TimePerFile;
        }

        public TimeSpan GetRemainingTime()
        {
            return RemainingTime;
        }
    }
}
