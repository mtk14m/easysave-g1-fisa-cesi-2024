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
            Thread backupThread = new Thread(() =>
            {
                string[] files = Directory.GetFiles(backupJob.SourceDirectory, "*.*", SearchOption.AllDirectories);
                int totalFiles = files.Length;
                int copiedFiles = 0;
                backupJob.JobState = State.Active;

                var priorityFiles = files.Where(file => configData.ExtensionsWithPriority.Contains(Path.GetExtension(file))).ToList();
                var nonPriorityFiles = files.Except(priorityFiles).ToList();

                int maxThreads = 5; // Nombre maximal de threads pouvant s'exécuter simultanément
                Semaphore semaphore = new Semaphore(maxThreads, maxThreads); // Utilisation d'un sémaphore pour contrôler les threads

                List<Thread> priorityThreads = new List<Thread>(); // Liste pour stocker les threads prioritaires
                List<Thread> nonPriorityThreads = new List<Thread>(); // Liste pour stocker les threads non prioritaires

                // Traitement des fichiers prioritaires
                foreach (string file in priorityFiles)
                {
                    while (backupJob.IsPaused())
                    {
                        Thread.Sleep(1000);
                        if (backupJob.IsStopped())
                        {
                            backupJob.JobState = State.Stopped;
                            return;
                        }
                        if (backupJob.IsRunning())
                        {
                            backupJob.JobState = State.Paused;
                            return;
                        }
                    }
                    semaphore.WaitOne(); // Attendre qu'une place se libère dans le sémaphore
                    Thread priorityThread = new Thread(() =>
                    {
                        try
                        {
                            string targetFilePath = file.Replace(backupJob.SourceDirectory, backupJob.TargetDirectory);

                            if (configData.ExtensionsToEncrypt.Contains(Path.GetExtension(file)))
                            {
                                EncryptFile(file);
                            }

                            File.Copy(file, targetFilePath, true);

                            Interlocked.Increment(ref copiedFiles);
                            backupJob.UpdateProgress(copiedFiles, totalFiles, DateTime.Now);
                        }
                        finally
                        {
                            semaphore.Release(); // Libérer une place dans le sémaphore après que le thread ait terminé son travail
                        }
                    });

                    priorityThreads.Add(priorityThread); // Ajouter le thread à la liste
                    priorityThread.Start();
                }

                // Traitement des fichiers non prioritaires
                foreach (string file in nonPriorityFiles)
                {
                    while (backupJob.IsPaused())
                    {
                        Thread.Sleep(1000);
                        if (backupJob.IsStopped())
                        {
                            backupJob.JobState = State.Stopped;
                            return;
                        }
                        if (backupJob.IsRunning())
                        {
                            backupJob.JobState = State.Paused;
                            return;
                        }
                    }
                    semaphore.WaitOne();
                    Thread nonPriorityThread = new Thread(() =>
                    {
                        try
                        {
                            string targetFilePath = file.Replace(backupJob.SourceDirectory, backupJob.TargetDirectory);
                            if (configData.ExtensionsToEncrypt.Contains(Path.GetExtension(file)))
                            {
                                EncryptFile(file);
                            }
                            else
                            {
                                File.Copy(file, targetFilePath, true);
                            }

                            Interlocked.Increment(ref copiedFiles);
                            backupJob.UpdateProgress(copiedFiles, totalFiles, DateTime.Now);
                        }
                        finally
                        {
                            semaphore.Release(); // Libérer une place dans le sémaphore après que le thread ait terminé son travail
                        }
                    });

                    nonPriorityThreads.Add(nonPriorityThread); // Ajouter le thread à la liste
                    nonPriorityThread.Start();
                }

                // Attendre la fin de tous les threads
                foreach (Thread thread in priorityThreads.Concat(nonPriorityThreads))
                {
                    thread.Join();
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Le backup '{backupJob.Name}' est terminé.", "Backup Terminé", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            });

            backupJob.JobState = State.Completed;
            backupThread.Start();
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
