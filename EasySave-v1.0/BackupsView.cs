using EasySave_v1._0;
using EasySaveUtilities.Models;
using System;

namespace EasySave.BackupView
{
    public class BackupView
    {
        private BackupsAppController appController;

        internal BackupView(BackupsAppController backupsAppController)
        {
            appController = backupsAppController;
        }

        public void ShowMenu()
        {
            Console.WriteLine("Welcome to EasySave!");
            Console.WriteLine("1. Add Backup Job");
            Console.WriteLine("2. List Backup Jobs");
            Console.WriteLine("3. Change language");
            Console.WriteLine("4. Exit");
        }

        public void AddBackup()
        {
            Console.WriteLine("Adding Backup:");
            Console.Write("Enter Name: ");
            string name = Console.ReadLine();
            Console.Write("Enter Source Directory: ");
            string sourceDirectory = Console.ReadLine();
            Console.Write("Enter Target Directory: ");
            string targetDirectory = Console.ReadLine();
            Console.Write("Enter Type (Full or Differential): ");
            string type = Console.ReadLine();

            appController.AddBackup(new Backup
            {
                Name = name,
                SourceDirectory = sourceDirectory,
                TargetDirectory = targetDirectory,
                Type = type
            });

            Console.WriteLine("Backup Job added successfully.");
        }

        public void ListBackups()
        {
            var backups = appController.GetAllBackups();

            if (backups.Count == 0)
            {
                Console.WriteLine("No backup jobs found.");
            }
            else
            {
                Console.WriteLine("Backup Jobs:");
                foreach (var backup in backups)
                {
                    Console.WriteLine($"Name: {backup.Name}, Source Directory: {backup.SourceDirectory}, Target Directory: {backup.TargetDirectory}, Type: {backup.Type}");
                }
            }
        }
    }
}