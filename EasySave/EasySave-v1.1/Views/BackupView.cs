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
        private LanguageManager translator;

        public BackupView(BackupController backupController)
        {
            controller = backupController;
            translator = new LanguageManager();
            translator.LoadTranslations();
        }

        public void ShowMenu()
        {
            translator.LoadTranslations();
            Console.WriteLine($"{translator.Translate("main_menu")}");
            Console.WriteLine($"1. {translator.Translate("add_backup_job")}");
            Console.WriteLine($"2. {translator.Translate("list_backup_jobs")}");
            Console.WriteLine($"3. {translator.Translate("exit")}");

        }
        public void AddBackupJob()
        {
            translator.LoadTranslations();
            Console.WriteLine($"{translator.Translate("add_backup_job_prompt")}");
            Console.Write($"{translator.Translate("enter_name_prompt")}");
            string name = Console.ReadLine();
            Console.Write($"{translator.Translate("enter_source_directory_prompt")}");
            string sourceDirectory = Console.ReadLine();
            Console.Write($"{translator.Translate("enter_target_directory_prompt")}");
            string targetDirectory = Console.ReadLine();
            Console.Write($"{translator.Translate("enter_type_prompt")}");
            string type = Console.ReadLine();

            controller.AddBackupJob(new BackupJob
            {
                Name = name,
                SourceDirectory = sourceDirectory,
                TargetDirectory = targetDirectory,
                Type = type
            });

            Console.WriteLine($"{translator.Translate("backup_job_added_successfully_message")}");
        }

        public void ListBackupJobs()
        {
            translator.LoadTranslations();
            var backupJobs = controller.GetAllBackupJobs();

            if (backupJobs.Count == 0)
            {
                Console.WriteLine($"{translator.Translate("no_backup_jobs_found_message")}");
            }
            else
            {
                Console.WriteLine($"{translator.Translate("backup_jobs_header")}");
                for (int i = 0; i < backupJobs.Count; i++)
                {
                    var job = backupJobs[i];
                    Console.ForegroundColor = i % 2 == 0 ? ConsoleColor.DarkGray : ConsoleColor.Gray;
                    Console.WriteLine($" {i + 1}. {translator.Translate("enter_name_prompt")}: {job.Name}");
                    Console.WriteLine($"    {translator.Translate("enter_source_directory_prompt")}: {job.SourceDirectory}");
                    Console.WriteLine($"    {translator.Translate("enter_target_directory_prompt")}: {job.TargetDirectory}");
                    Console.WriteLine($"    {translator.Translate("enter_type_prompt")}: {job.Type}");
                    Console.WriteLine();
                }
                Console.ResetColor();

                Console.WriteLine($"{translator.Translate("enter_choice")}");
                Console.Write("> ");
                string choice = Console.ReadLine();

                controller.ExecuteBackup(choice);
            }
        }

        //changer le type de log
        public void ChangeLogType()
        {
            Console.WriteLine($" {translator.Translate("EnterLogType")}");
            string input = Console.ReadLine();

            if (int.TryParse(input, out int typeChoose))
            {
                if (typeChoose == 0)
                {
                    controller.ChangeLogType(typeChoose);
                    Console.WriteLine($"{translator.Translate("JSONType")}");
                }
                else if (typeChoose == 1)
                {
                    controller.ChangeLogType(typeChoose);
                    Console.WriteLine($"{translator.Translate("LogTypeSetTo")}");
                }
                else
                {
                    Console.WriteLine($"{translator.Translate("InvalidLogType")}");
                }
            }
            else
            {
                Console.WriteLine($"{translator.Translate("InvalidLogTypeInput")}");
            }
        }

    }
}
