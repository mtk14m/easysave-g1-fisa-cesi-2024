using EasySave_v1._0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave_v1._0.Packages
{
    internal class FileCopier
    {

        public async Task CopyFilesAsync(BackupJob backupJob)
        {
            try
            {
                // Obtenir la liste des fichiers à copier
                string[] sourceFiles = Directory.GetFiles(backupJob.SourceDirectory, "*", SearchOption.AllDirectories);
                int totalFiles = sourceFiles.Length;
                int copiedFiles = 0;

                // Démarrer le chronomètre pour le temps écoulé
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                foreach (string sourceFile in sourceFiles)
                {
                    string relativePath = Path.GetRelativePath(backupJob.SourceDirectory, sourceFile);
                    string targetFilePath = Path.Combine(backupJob.TargetDirectory, relativePath);

                    if (backupJob.Type.ToLower() == "full" || !File.Exists(targetFilePath))
                    {
                        // Copie complète ou le fichier n'existe pas dans le dossier cible
                        await Task.Run(() => File.Copy(sourceFile, targetFilePath, true));
                        copiedFiles++;
                    }
                    else if (backupJob.Type.ToLower() == "differential")
                    {
                        // Copie différentielle (si le fichier source est plus récent que le fichier cible)
                        DateTime sourceLastWriteTime = File.GetLastWriteTime(sourceFile);
                        DateTime targetLastWriteTime = File.GetLastWriteTime(targetFilePath);

                        if (sourceLastWriteTime > targetLastWriteTime)
                        {
                            await Task.Run(() => File.Copy(sourceFile, targetFilePath, true));
                            copiedFiles++;
                        }
                    }

                    // Calculer la progression
                    //backupJob.ProgressPercentage = (double)copiedFiles / totalFiles * 100;
                    int remainingFiles = totalFiles - copiedFiles;
                    TimeSpan elapsedTime = stopwatch.Elapsed;
                    backupJob.TimePerFile = TimeSpan.FromSeconds(elapsedTime.TotalSeconds / copiedFiles);
                    backupJob.RemainingTime = TimeSpan.FromSeconds(elapsedTime.TotalSeconds / copiedFiles * remainingFiles);

                    // Afficher la progression
                    Console.WriteLine($"Progress: {backupJob.ProgressPercentage:F2}% - Copied {copiedFiles}/{totalFiles} files - Time per file: {backupJob.TimePerFile:g} - Remaining time: {backupJob.RemainingTime:g}");

                    // Attente d'une seconde entre chaque copie (facultatif)
                    await Task.Delay(1000);
                }

                Console.WriteLine("File copy completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

    }
}

