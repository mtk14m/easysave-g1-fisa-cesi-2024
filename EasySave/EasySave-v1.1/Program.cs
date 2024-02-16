using EasySave_v1._0.Controllers;
using EasySave_v1._0.Packages;
using EasySave_v1._0.Views;
using System;


namespace EasySave
{
    class Program
    {
        //static string language = "fr-fr";
        static void Main(string[] args)
        {
            // Initialisation du traducteur

            LanguageManager translator = new LanguageManager();

            // Création du contrôleur de sauvegarde et de la vue de sauvegarde
            BackupController backupController = new BackupController(translator);
            BackupView backupView = new BackupView(backupController);

            // Menu principal
            while (true)
            {
                translator.LoadTranslations();
                Console.WriteLine(translator.Translate("main_menu"));

                Console.WriteLine($"1. {translator.Translate("add_backup_job")}");
                Console.WriteLine($"2. {translator.Translate("list_backup_jobs")}");
                Console.WriteLine($"3. {translator.Translate("change_language")}");
                Console.WriteLine($"4. {translator.Translate("change_log_type")}");
                Console.WriteLine($"5. {translator.Translate("exit")}");

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
                        chLanguage();
                        break;
                    case "4":
                        backupView.ChangeLogType();
                        break;
                    case "5":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine(translator.Translate("invalid_choice"));
                        break;
                }
            
            }

            void chLanguage()
            {
                translator.ChangeLanguage();
                translator.LoadTranslations();
            }
            

        }

        

    }


}