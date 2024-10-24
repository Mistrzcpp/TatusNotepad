using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TatusNotepad
{
    public class Konwerter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter as string == "Napis")
            {
                return value as string;
            }
            else if (parameter as string == "CzasTrwania")
            {
                if (value != null)
                {
                    DateTime dzisiaj = DateTime.Today;
                    DateTime dataUtworzenia = DateTime.Parse((string)value);
                    TimeSpan roznica = dzisiaj - dataUtworzenia;
                    return "Dni: " + (int)roznica.TotalDays;
                }
                return null;

            }
            else if (parameter as string == "Produkty")
            {
                if (value is List<Produkt> produkty && produkty.Any())
                {
                    Produkt produkt = produkty.First();
                    return produkt.nazwa;
                }
                else
                    return null;
            }
            else if (parameter as string == "DataEdytor")
            {
                if(value is string data)
                {
                    DateTime dzisiaj = DateTime.Today;
                    DateTime dataUtworzenia = DateTime.Parse((string)value);
                    TimeSpan roznica = dzisiaj - dataUtworzenia;
                    string dni = ((int)roznica.TotalDays).ToString();
                    string napis = "Utworzono dnia: " + data + '\n';
                    napis += "Upłyneło dni: " + dni;
                    return napis;
                }
                return null;
            }
            else
            {
                if (value as string == "Przyjęte") 
                    return "#c94200";
                else if (value as string == "Zamówione")
                    return "#C9A700";
                else if (value as string == "Powiadomiono")
                    return "#0087c9";
                else if (value as string == "Zrealizowane")
                    return "#87c900";
                else
                    return "#ffffff";
            }

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
