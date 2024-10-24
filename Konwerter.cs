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
                if (value is int)
                {
                    if ((int)value == 1)
                        return "W realizacji";
                    else if ((int)value == 2)
                        return "Powiadomiono";
                    else
                        return "Zrealizowano";
                }
                else
                    return "Brak";
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
                if (value is int)
                {
                    if ((int)value == 1)
                        return "#c94200";
                    else if ((int)value == 2)
                        return "#0087c9";
                    else
                        return "#87c900";
                }
                else
                    return "#c90087";
            }

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
