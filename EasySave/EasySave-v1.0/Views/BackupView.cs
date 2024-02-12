using EasySave_v1._0.Controllers;
using EasySave_v1._0.Models;
using EasySave_v1._0.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EasySave_v1._0.Views
{
    internal class BackupView
    {
        private BackupController controller;

        // Initialisation du traducteur
        Translator translator = new Translator();

        public BackupView(BackupController backupController)
        {
            controller = backupController;
        }

        public void ShowMenu()
        {
            Console.WriteLine("Welcome to EasySave!");
            Console.WriteLine("1. Add Backup Job");
            Console.WriteLine("2. List Backup Jobs");
            Console.WriteLine("3. Exit");
        }
        public void AddBackupJob()
        {
            Console.WriteLine("Adding Backup Job:");
            Console.Write("Enter Name: ");
            string name = Console.ReadLine();
            Console.Write("Enter Source Directory: ");
            string sourceDirectory = Console.ReadLine();
            Console.Write("Enter Target Directory: ");
            string targetDirectory = Console.ReadLine();
            Console.Write("Enter Type (Complet or Differential): ");
            string type = Console.ReadLine();

            controller.AddBackupJob(new BackupJob
            {
                Name = name,
                SourceDirectory = sourceDirectory,
                TargetDirectory = targetDirectory,
                Type = type
            });

            Console.WriteLine("Backup Job added successfully.");
        }

        public void ListBackupJobs()
        {
            var backupJobs = controller.GetAllBackupJobs();

            if (backupJobs.Count == 0)
            {
                Console.WriteLine("No backup jobs found.");
            }
            else
            {
                Console.WriteLine("Backup Jobs:");
                for (int i = 0; i < backupJobs.Count; i++)
                {
                    var job = backupJobs[i];
                    Console.ForegroundColor = i % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.Gray;
                    Console.WriteLine($" {i + 1}. Name: {job.Name}");
                    Console.WriteLine($"    Source Directory: {job.SourceDirectory}");
                    Console.WriteLine($"    Target Directory: {job.TargetDirectory}");
                    Console.WriteLine($"    Type: {job.Type}");
                    Console.WriteLine($"    Files Copied: {job.CopiedFiles}");
                    Console.WriteLine($"    Time Taken: {job.ElapsedTime}");
                    Console.WriteLine();
                }
                Console.ResetColor();
            }
        }

    }

}