using EasySave_v2._0.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EasySave_v2._0.Packages
{
    internal class FileCopier
    {
        private LanguageManager translator;

        public FileCopier(LanguageManager languageManager)
        {
            translator = languageManager;
        }

        public async Task CopyFilesAsync(BackupJob backupJob)
        {
            try
            {
                // Vérifier si les répertoires sources et cibles existent
                if (!Directory.Exists(backupJob.SourceDirectory))
                {
                    throw new DirectoryNotFoundException($"{translator.Translate("source_directory_not_found_message")}: {backupJob.SourceDirectory}");
                }

                if (!Directory.Exists(backupJob.TargetDirectory))
                {
                    throw new DirectoryNotFoundException($"{translator.Translate("target_directory_not_found_message")}: {backupJob.TargetDirectory}");
                }

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
                    double progressPercentage = (double)copiedFiles / totalFiles * 100;
                    int remainingFiles = totalFiles - copiedFiles;
                    TimeSpan elapsedTime = stopwatch.Elapsed;
                    TimeSpan timePerFile = TimeSpan.FromSeconds(elapsedTime.TotalSeconds / copiedFiles);
                    TimeSpan remainingTime = TimeSpan.FromSeconds(elapsedTime.TotalSeconds / copiedFiles * remainingFiles);


                    //Copie des informations dans l'objet
                    backupJob.ElapsedTime = elapsedTime;
                    backupJob.CopiedFiles = copiedFiles;
                    backupJob.TotalFiles = copiedFiles;
                    backupJob.JobState = State.Completed;
                    backupJob.RemainingTime = remainingTime;
                    


                    // Afficher la progression
                    Console.WriteLine($"{translator.Translate("progress_message")}: {progressPercentage:F2}% - {translator.Translate("copied_files_message")}: {copiedFiles}/{totalFiles} - {translator.Translate("time_per_file_message")}: {timePerFile:g} - {translator.Translate("remaining_time_message")}: {remainingTime:g}");

                    // Attente d'une seconde entre chaque copie (facultatif)
                    await Task.Delay(1000);
                }

                Console.WriteLine(translator.Translate("file_copy_completed_message"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{translator.Translate("error_occurred_message")}: {ex.Message}");
            }
        }
    }
}
