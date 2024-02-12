using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave_v1._0.Models
{
    internal class BackupJob
    {
        public string Name { get; set; } // Nom de la tâche de sauvegarde
        public string SourceDirectory { get; set; } // Répertoire source
        public string TargetDirectory { get; set; } // Répertoire cible
        public string Type { get; set; } // Type de sauvegarde (complète, différentielle, etc.)
        public DateTime LastRun { get; set; } // Date et heure de la dernière exécution
        public State JobState { get; set; } // État de la tâche de sauvegarde

        // Informations de progression de la sauvegarde
        public int TotalFiles { get; set; } // Nombre total de fichiers à copier
        public int CopiedFiles { get; set; } // Nombre de fichiers déjà copiés
        public int RemainingFiles { get; set; } // Nombre de fichiers restants à copier
        public TimeSpan ElapsedTime { get; set; } // Temps écoulé depuis le début de la copie
        public TimeSpan RemainingTime { get; set; } // Temps restant avant la fin de la copie
        public DateTime StartTime { get; set; } // Date et heure de début de la copie
        public TimeSpan TimePerFile { get; set; } // Temps moyen par fichier copié
        public double ProgressPercentage // Pourcentage de progression de la sauvegarde
        {
            get
            {
                if (TotalFiles == 0) return 100; // Éviter une division par zéro
                return (double)CopiedFiles / TotalFiles * 100;
            }
        }
    }


}

