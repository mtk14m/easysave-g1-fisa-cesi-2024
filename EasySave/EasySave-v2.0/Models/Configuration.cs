using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave_v2._0.Models
{
    public class Configuration
    {
        public string Language { get; set; }
        public string DailyLogPath { get; set; }
        public string StateLogPath { get; set; }
        public string ExtensionsToEncrypt { get; set; }
        public string ExtensionsWithPriority { get; set; }
        public int FileSizeLimit { get; set; }
        public string ConfigFilePath { get; set; }
        public string LogType { get; set; }
        public string LogicielMetier { get; set; }
    }
}

