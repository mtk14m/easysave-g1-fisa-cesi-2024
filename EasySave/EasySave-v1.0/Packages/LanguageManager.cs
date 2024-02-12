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
        //private string currentLanguage;

        public LanguageManager(string currentLanguage = "fr-fr")
        {
            translator = new Translator();
            LoadTranslations(currentLanguage); // Chargement des traductions
        }

        // Charge les traductions à partir du fichier JSON correspondant à la langue actuelle
        internal void LoadTranslations(string currentLanguage)
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
        public void ChangeLanguage(string currentLanguage)
        {
            //currentLanguage = (currentLanguage == "en-us") ? "fr-fr" : "en-us"; // Toggle entre les deux langues
            LoadTranslations(currentLanguage);
            Console.WriteLine($"Language changed to {(currentLanguage == "en-us" ? "English" : "French")}");
        }

        // Permet de traduire une phrase avec le Translator
        public string Translate(string key)
        {
            return translator.Translate(key);
        }
    }
}
