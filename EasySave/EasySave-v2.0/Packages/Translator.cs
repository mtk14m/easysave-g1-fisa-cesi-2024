using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EasySave_v2._0.Packages
{
    internal class Translator
    {

        private Dictionary<string, string> translations = new Dictionary<string, string>();

        public void LoadTranslations(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                if (translations == null)
                {
                    Console.WriteLine("Failed to load translations. Using default translations.");
                    translations = new Dictionary<string, string>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading translations: {ex.Message}");
            }
        }

        public string Translate(string key)
        {
            if (translations.ContainsKey(key))
            {
                return translations[key];
            }
            else
            {
                return $"Translation not found for key: {key}";
            }
        }
    }

}
