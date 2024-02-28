using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;
namespace EasySave_v2._0.Packages
{
    public class TranslationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Assurez-vous que value est une chaîne représentant une clé de traduction
            if (value is string key)
            {
                // Utilisez LanguageManager pour traduire la clé
                var languageManager = new LanguageManager(); // Créez une instance de LanguageManager
                return languageManager.Translate(key);
            }

            // Si la valeur n'est pas une chaîne, retournez-la telle quelle
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
