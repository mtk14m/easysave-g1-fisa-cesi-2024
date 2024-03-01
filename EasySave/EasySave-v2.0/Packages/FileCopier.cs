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
            bool isAnyLogicielMetierRunning = false;

            Thread backupThread = new Thread(() =>
            {
                string[] files = Directory.GetFiles(backupJob.SourceDirectory, "*.*", SearchOption.AllDirectories);
                int totalFiles = files.Length;
                int copiedFiles = 0;
                backupJob.JobState = State.Active;
                string logicielMetierString = configData.LogicielMetier;
                string[] logicielMetierList = logicielMetierString.Split(',');
                var priorityFiles = files.Where(file => configData.ExtensionsWithPriority.Contains(Path.GetExtension(file))).ToList();
                var nonPriorityFiles = files.Except(priorityFiles).ToList();

                int maxThreads = 5;
                Semaphore semaphore = new Semaphore(maxThreads, maxThreads);

                List<Thread> priorityThreads = new List<Thread>();
                List<Thread> nonPriorityThreads = new List<Thread>();

                foreach (string file in priorityFiles)
                {
                    foreach (string processName in logicielMetierList)
                    {
                        Process[] processes = Process.GetProcessesByName(processName);
                        foreach (Process process in processes)
                        {
                            MessageBox.Show($"Le logiciel métier {process.ProcessName} est en cours d'exécution. Veuillez le fermer pour continuer le backup.");
                            process.WaitForExit();
                        }
                    }
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
                    Thread priorityThread = new Thread(() =>
                    {
                        try
                        {
                            string targetFilePath = file.Replace(backupJob.SourceDirectory, backupJob.TargetDirectory);

                            if (configData.ExtensionsToEncrypt.Contains(Path.GetExtension(file)))
                            {
                                EncryptFile(file, backupJob.TargetDirectory);
                            }

                            File.Copy(file, targetFilePath, true);

                            Interlocked.Increment(ref copiedFiles);
                            backupJob.UpdateProgress(copiedFiles, totalFiles, DateTime.Now, backupJob.JobState);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    });

                    priorityThreads.Add(priorityThread);
                    priorityThread.Start();
                }

                foreach (string file in nonPriorityFiles)
                {
                    foreach (string processName in logicielMetierList)
                    {
                        Process[] processes = Process.GetProcessesByName(processName);
                        foreach (Process process in processes)
                        {
                            MessageBox.Show($"Le logiciel métier {process.ProcessName} est en cours d'exécution. Veuillez le fermer pour continuer le backup.");
                            process.WaitForExit();
                        }
                    }
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
                                EncryptFile(file, backupJob.TargetDirectory);
                            }
                            else
                            {
                                File.Copy(file, targetFilePath, true);
                            }

                            Interlocked.Increment(ref copiedFiles);
                            backupJob.UpdateProgress(copiedFiles, totalFiles, DateTime.Now, backupJob.JobState);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    });

                    nonPriorityThreads.Add(nonPriorityThread);
                    nonPriorityThread.Start();
                }

                foreach (Thread thread in priorityThreads.Concat(nonPriorityThreads))
                {
                    thread.Join();
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    backupJob.finished();
                    MessageBox.Show($"Le backup '{backupJob.Name}' est terminé.", "Backup Terminé", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            });
            backupThread.Start();
        }


        private Mutex fileMutex = new Mutex(); // Mutex pour verrouiller l'accès au fichier

        private void EncryptFile(string sourceFilePath, string targetDirectory)
        {
            try
            {
                fileMutex.WaitOne(); // Attendre pour obtenir le verrou

                byte[] fileBytes = File.ReadAllBytes(sourceFilePath);
                byte[] key = Encoding.UTF8.GetBytes("YourEncryptionKey");

                for (int i = 0; i < fileBytes.Length; i++)
                {
                    fileBytes[i] = (byte)(fileBytes[i] ^ key[i % key.Length]);
                }

                string encryptedFileName = Path.GetFileName(sourceFilePath) + ".enc";
                string encryptedFilePath = Path.Combine(targetDirectory, encryptedFileName);

                // Utiliser le même mutex pour verrouiller l'accès à l'écriture du fichier chiffré
                fileMutex.WaitOne(); // Attendre pour obtenir le verrou
                File.WriteAllBytes(encryptedFilePath, fileBytes);
                fileMutex.ReleaseMutex(); // Libérer le verrou

                MessageBox.Show($"Le fichier chiffré a été enregistré dans le dossier cible : {encryptedFilePath}", "Chiffrement Terminé", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chiffrement du fichier : {ex.Message}");
            }
            finally
            {
                fileMutex.ReleaseMutex(); // Libérer le verrou
            }
        }

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
