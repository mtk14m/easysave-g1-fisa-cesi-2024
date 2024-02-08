using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave_v1._0.Packages
{
    internal class FileCopier
    {

        public async Task CopyFilesAsync(string sourceDirectory, string targetDirectory)
        {
            try
            {
                // Obtenir la liste des fichiers à copier
                string[] sourceFiles = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);
                int totalFiles = sourceFiles.Length;
                int copiedFiles = 0;

                // Démarrer le chronomètre pour le temps écoulé
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                foreach (string sourceFile in sourceFiles)
                {
                    string relativePath = Path.GetRelativePath(sourceDirectory, sourceFile);
                    string targetFilePath = Path.Combine(targetDirectory, relativePath);

                    // Copier le fichier
                    await Task.Run(() => File.Copy(sourceFile, targetFilePath, true));
                    copiedFiles++;

                    // Calculer la progression
                    int remainingFiles = totalFiles - copiedFiles;
                    TimeSpan elapsedTime = stopwatch.Elapsed;
                    TimeSpan remainingTime = TimeSpan.FromSeconds(elapsedTime.TotalSeconds / copiedFiles * remainingFiles);

                    // Afficher la progression
                    Console.WriteLine($"Copied {copiedFiles}/{totalFiles} files. Remaining time: {remainingTime}");

                    // Attente d'une seconde entre chaque copie (facultatif)
                    await Task.Delay(1000);
                }

                Console.WriteLine("File copy completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
      
            }
    }   }
}
