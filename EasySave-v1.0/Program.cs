using EasySave_v1._0.Controllers;
using EasySave_v1._0.Packages;
using EasySave_v1._0.Views;
using System;


namespace EasySave
{
    class Program
    {  
        static void Main(string[] args)
        {
            // Initialisation du traducteur
            Translator translator = new Translator();
            translator.LoadTranslations("translations.json");

            // Création du contrôleur de sauvegarde et de la vue de sauvegarde
            BackupController backupController = new BackupController();
            BackupView backupView = new BackupView(backupController);

            // Menu principal
            while (true)
            {
                Console.WriteLine(translator.Translate("main_menu"));

                Console.WriteLine($"1. {translator.Translate("add_backup_job")}");
                Console.WriteLine($"2. {translator.Translate("list_backup_jobs")}");
                Console.WriteLine($"3. {translator.Translate("exit")}");

                Console.Write(translator.Translate("enter_choice"));
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        backupView.AddBackupJob();
                        break;
                    case "2":
                        backupView.ListBackupJobs();
                        break;
                    case "3":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine(translator.Translate("invalid_choice"));
                        break;
                }
        
            
            
            }
    }   }
}