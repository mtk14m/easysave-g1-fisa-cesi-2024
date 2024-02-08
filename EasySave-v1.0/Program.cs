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
            string language = "fr-fr";
            

            //Translator translator = new Translator();
            //translator.LoadTranslations("../../../Ressources/fr-fr.json");

            LanguageManager translator = new LanguageManager(language);

            // Création du contrôleur de sauvegarde et de la vue de sauvegarde
            BackupController backupController = new BackupController();
            BackupView backupView = new BackupView(backupController);

            //la langue
            

            // Menu principal
            while (true)
            {
                Console.WriteLine(translator.Translate("main_menu"));

                Console.WriteLine($"1. {translator.Translate("add_backup_job")}");
                Console.WriteLine($"2. {translator.Translate("list_backup_jobs")}");
                Console.WriteLine($"3. {translator.Translate("change_language")}");
                Console.WriteLine($"4. {translator.Translate("exit")}");

                Console.Write(translator.Translate("enter_choice"));
                string choice = Console.ReadLine();

                Console.WriteLine(language);

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
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine(translator.Translate("invalid_choice"));
                        break;
                }
        
            
           
            
            }

            void chLanguage()
            {

                language = (language == "en-us") ? "fr-fr" : "en-us"; // Toggle entre les deux langues
                translator.ChangeLanguage(language);
                translator.LoadTranslations(language);
                Console.WriteLine(language);
            }
            

        }

        

    }


}