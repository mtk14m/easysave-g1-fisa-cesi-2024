using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave_v1._0.Packages
{
    internal class LanguageManager
    {
        private Translator translator;
        private string currentLanguage;

        public LanguageManager()
        {
            translator = new Translator();
            currentLanguage = "en-us"; // Par défaut, la langue est l'anglais
            LoadTranslations(); // Chargement des traductions
        }

        // Charge les traductions à partir du fichier JSON correspondant à la langue actuelle
        private void LoadTranslations()
        {
            string filePath = $"Ressources/{currentLanguage}.json";
            try
            {
                string json = File.ReadAllText(filePath);
                translator.LoadTranslations(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading translations: {ex.Message}");
            }
        }

        // Change la langue et recharge les traductions
        public void ChangeLanguage(string language)
        {
            if (language == currentLanguage)
            {
                Console.WriteLine("Language is already set to " + (language == "en-us" ? "English" : "French"));
                return;
            }

            currentLanguage = language;
            LoadTranslations();
            Console.WriteLine($"Language changed to {(language == "en" ? "English" : "French")}");
        }

        // Permet de traduire une phrase avec le Translator
        public string Translate(string key)
        {
            return translator.Translate(key);
        }
    }
}
