using System;
using System.Collections.Generic;

namespace EasySave_v2._0.Packages
{
    internal class LanguageManager
    {
        private static string currentLanguage = "fr-fr";
        private Translator translator;

        public LanguageManager()
        {
            translator = new Translator();
            LoadTranslations(); // Chargement des traductions
        }

        // Charge les traductions à partir du fichier JSON correspondant à la langue actuelle
        internal void LoadTranslations()
        {
            string filePath = $"../../../Ressources/{currentLanguage}.json";
            try
            {
                translator.LoadTranslations(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading translations: {ex.Message}");
            }
        }

        // Change la langue et recharge les traductions
        public void ChangeLanguage()
        {
            currentLanguage = (currentLanguage == "en-us") ? "fr-fr" : "en-us"; // Toggle entre les deux langues
            LoadTranslations();
            Console.WriteLine($"Language changed to {(currentLanguage == "en-us" ? "English" : "French")}");
        }

        // Permet de traduire une phrase avec le Translator
        public string Translate(string key)
        {
            return translator.Translate(key);
        }
    }
}
