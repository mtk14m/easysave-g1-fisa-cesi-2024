using EasySave_v1._0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave_v1._0.Controllers
{
    internal class BackupController
    {
        private List<BackupJob> backupJobs;

        public BackupController()
        {
            backupJobs = new List<BackupJob>();
        }

        public void AddBackupJob(BackupJob job)
        {
            backupJobs.Add(job);
        }

        public List<BackupJob> GetAllBackupJobs()
        {
            return backupJobs;
        }

    }
}
