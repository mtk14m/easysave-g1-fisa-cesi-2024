using System;
using System.Collections.Generic;
using EasySaveUtilities.Models;

namespace EasySave_v1._0
{
    class BackupsAppController
    {
        private List<Backup> backups;

        public BackupsAppController()
        {
            backups = new List<Backup>();
        }

        public void AddBackup(Backup job)
        {
            
            backups.Add(job);
        }

        public List<Backup> GetAllBackups()
        {
            return backups;
        }
        public List<Backup> GetBackups() {return backups;}
    }
}