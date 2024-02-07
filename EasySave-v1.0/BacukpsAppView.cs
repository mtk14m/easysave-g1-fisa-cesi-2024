using EasySave.BackupView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave_v1._0
{
         class Program
        {
            static void Main(string[] args)
            {
                var backupAppController = new BackupsAppController();
                var backupView = new BackupView(backupAppController);

                while (true)
                {
                    backupView.ShowMenu();

                    Console.Write("Enter your choice: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            backupView.AddBackup();
                            break;
                        case "2":
                            backupView.ListBackups();
                            break;
                        case "3":
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }

           //Console.WriteLine("Hello Bonjour");
            }
        }
    
}
