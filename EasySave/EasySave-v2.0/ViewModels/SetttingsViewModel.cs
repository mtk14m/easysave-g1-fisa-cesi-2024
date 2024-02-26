using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using EasySave_v2._0.Models;

namespace EasySave_v2._0.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public SettingsViewModel()
        {
            LoadConfig();
        }

        private string _language;
        internal string Language
        {
            get { return _language; }
            set
            {
                if (_language != value)
                {
                    _language = value;
                    OnPropertyChanged(nameof(Language));
                }
            }
        }

        private string _dailyLogPath;
        public string DailyLogPath
        {
            get { return _dailyLogPath; }
            set
            {
                if (_dailyLogPath != value)
                {
                    _dailyLogPath = value;
                    OnPropertyChanged(nameof(DailyLogPath));
                }
            }
        }

        private string _stateLogPath;
        public string StateLogPath
        {
            get { return _stateLogPath; }
            set
            {
                if (_stateLogPath != value)
                {
                    _stateLogPath = value;
                    OnPropertyChanged(nameof(StateLogPath));
                }
            }
        }

        private string _extensionsToEncrypt;
        public string ExtensionsToEncrypt
        {
            get { return _extensionsToEncrypt; }
            set
            {
                if (_extensionsToEncrypt != value)
                {
                    _extensionsToEncrypt = value;
                    OnPropertyChanged(nameof(ExtensionsToEncrypt));
                }
            }
        }

        private string _extensionsWithPriority;
        public string ExtensionsWithPriority
        {
            get { return _extensionsWithPriority; }
            set
            {
                if (_extensionsWithPriority != value)
                {
                    _extensionsWithPriority = value;
                    OnPropertyChanged(nameof(ExtensionsWithPriority));
                }
            }
        }

        private int _fileSizeLimit;
        public int FileSizeLimit
        {
            get { return _fileSizeLimit; }
            set
            {
                if (_fileSizeLimit != value)
                {
                    _fileSizeLimit = value;
                    OnPropertyChanged(nameof(FileSizeLimit));
                }
            }
        }

        private string _configFilePath;
        public string ConfigFilePath
        {
            get { return _configFilePath; }
            set
            {
                if (_configFilePath != value)
                {
                    _configFilePath = value;
                    OnPropertyChanged(nameof(ConfigFilePath));
                }
            }
        }

        private string _logType;
        public string LogType
        {
            get { return _logType; }
            set
            {
                if (_logType != value)
                {
                    _logType = value;
                    OnPropertyChanged(nameof(LogType));
                }
            }
        }

        //Lire le fichier de configuration
        public void LoadConfig()
        {
            ConfigFilePath ="../../../Ressources/";
            try
            {
                string configFileName = "config.json";
                string configFilePath = Path.Combine(ConfigFilePath, configFileName);

                if (File.Exists(configFilePath))
                {
                    string jsonConfig = File.ReadAllText(configFilePath);
                    Configuration config = JsonConvert.DeserializeObject<Configuration>(jsonConfig);

                    // Mettre à jour les propriétés du ViewModel avec les valeurs de la configuration chargée
                    Language = config.Language;
                    DailyLogPath = config.DailyLogPath;
                    StateLogPath = config.StateLogPath;
                    ExtensionsToEncrypt = config.ExtensionsToEncrypt;
                    ExtensionsWithPriority = config.ExtensionsWithPriority;
                    FileSizeLimit = config.FileSizeLimit;
                    LogType = config.LogType;
                }
                else
                {
                    MessageBox.Show("Le fichier de configuration n'existe pas.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite lors de la lecture de la configuration : {ex.Message}");
            }
        }

        public void SaveConfig()
        {
            try
            {
                Configuration config = new Configuration
                {
                    Language = Language,
                    DailyLogPath = DailyLogPath,
                    StateLogPath = StateLogPath,
                    ExtensionsToEncrypt = ExtensionsToEncrypt,
                    ExtensionsWithPriority = ExtensionsWithPriority,
                    FileSizeLimit = FileSizeLimit,
                    ConfigFilePath = ConfigFilePath,
                    LogType = LogType
                };

                string configFileName = "config.json";
                string configFilePath = Path.Combine(ConfigFilePath, configFileName);

                string jsonConfig = JsonConvert.SerializeObject(config, Formatting.Indented);

                File.WriteAllText(configFilePath, jsonConfig);

                MessageBox.Show("Configuration appliquée avec succès et enregistrée dans config.json.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite lors de l'enregistrement de la configuration : {ex.Message}");
            }
        }

        public void ResetConfig()
        {
            try
            {
                string defaultConfigFilePath = "../../../Ressources/config-base.json";
                if (File.Exists(defaultConfigFilePath))
                {
                    string jsonConfig = File.ReadAllText(defaultConfigFilePath);
                    Configuration defaultConfig = JsonConvert.DeserializeObject<Configuration>(jsonConfig);

                    Language = defaultConfig.Language;
                    DailyLogPath = defaultConfig.DailyLogPath;
                    StateLogPath = defaultConfig.StateLogPath;
                    ExtensionsToEncrypt = defaultConfig.ExtensionsToEncrypt;
                    ExtensionsWithPriority = defaultConfig.ExtensionsWithPriority;
                    FileSizeLimit = defaultConfig.FileSizeLimit;
                    ConfigFilePath = defaultConfig.ConfigFilePath;
                    LogType = defaultConfig.LogType;

                    // Je reenregistre la configuration par défaut
                    SaveConfig();

                    //MessageBox.Show("Configuration restaurée avec succès.");
                }
                else
                {
                    MessageBox.Show("Le fichier de configuration par défaut est introuvable.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite lors de la restauration de la configuration : {ex.Message}");
            }
        }

        //INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
