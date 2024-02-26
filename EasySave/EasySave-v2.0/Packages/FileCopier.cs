using EasySave_v2._0.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace EasySave_v2._0.Packages
{
    internal class FileCopier
    {
        private LanguageManager translator;

        public double ProgressPercentage { get; private set; }
        public int RemainingFiles { get; private set; }
        public TimeSpan ElapsedTime { get; private set; }
        public TimeSpan TimePerFile { get; private set; }
        public TimeSpan RemainingTime { get; private set; }

        public FileCopier(LanguageManager languageManager)
        {
            translator = languageManager;
        }

        public void CopyFiles(BackupJob backupJob)
        {
            Task.Run(() =>
            {
                try
                {
                    // Vérifier si les répertoires sources et cibles existent
                    if (!Directory.Exists(backupJob.SourceDirectory))
                    {
                        throw new DirectoryNotFoundException(translator.Translate("source_directory_not_found_message"));
                    }

                    if (!Directory.Exists(backupJob.TargetDirectory))
                    {
                        throw new DirectoryNotFoundException(translator.Translate("target_directory_not_found_message"));
                    }

                    // Obtenir la liste des fichiers à copier
                    string[] sourceFiles = Directory.GetFiles(backupJob.SourceDirectory, "*", SearchOption.AllDirectories);
                    int totalFiles = sourceFiles.Length;
                    int copiedFiles = 0;

                    // Démarrer le chronomètre pour le temps écoulé
                    var stopwatch = Stopwatch.StartNew();

                    foreach (string sourceFile in sourceFiles)
                    {
                        string relativePath = Path.GetRelativePath(backupJob.SourceDirectory, sourceFile);
                        string targetFilePath = Path.Combine(backupJob.TargetDirectory, relativePath);

                        if (backupJob.Type.ToLower() == "full" || !File.Exists(targetFilePath))
                        {
                            // Copie complète ou le fichier n'existe pas dans le dossier cible
                            File.Copy(sourceFile, targetFilePath, true);
                            copiedFiles++;
                        }
                        else if (backupJob.Type.ToLower() == "differential")
                        {
                            // Copie différentielle (si le fichier source est plus récent que le fichier cible)
                            DateTime sourceLastWriteTime = File.GetLastWriteTime(sourceFile);
                            DateTime targetLastWriteTime = File.GetLastWriteTime(targetFilePath);

                            if (sourceLastWriteTime > targetLastWriteTime)
                            {
                                File.Copy(sourceFile, targetFilePath, true);
                                copiedFiles++;
                            }
                        }

                        // Mettre à jour les informations de progression
                        UpdateProgressInfo(copiedFiles, totalFiles, stopwatch.Elapsed);

                        // Attente d'une seconde entre chaque copie (facultatif)
                        System.Threading.Thread.Sleep(1000);
                    }

                    MessageBox.Show("Backup terminé.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    // Gérer l'exception si nécessaire
                }
            });
        }

        private async Task UpdateProgressInfo(int copiedFiles, int totalFiles, TimeSpan elapsedTime)
        {
            await Task.Run(() =>
            {
                ProgressPercentage = (double)copiedFiles / totalFiles * 100;
                RemainingFiles = totalFiles - copiedFiles;
                ElapsedTime = elapsedTime;
                TimePerFile = TimeSpan.FromSeconds(elapsedTime.TotalSeconds / copiedFiles);
                RemainingTime = TimeSpan.FromSeconds(elapsedTime.TotalSeconds / copiedFiles * RemainingFiles);
            });
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
