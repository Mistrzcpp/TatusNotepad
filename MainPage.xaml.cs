using System.Globalization;
using System;
using Microsoft.Maui.Controls;
using System.Runtime.CompilerServices;

namespace TatusNotepad
{
    public partial class MainPage : ContentPage
    {
        public List<Zlecenie> Zlecenia { get; set; }
        public MainPage()
        {
            InitializeComponent();
            Zlecenia = new List<Zlecenie>
            {
                new Zlecenie {id=1, data="2024-09-20",  nazwa="Imie Nazwiko", telefon="123 456 789"},
                new Zlecenie {id=2, data="2024-08-30", nazwa="Adam Nowak",  status=1},
                new Zlecenie {id=3, nazwa="Adam Nowak", telefon="89 533 25 03",  status=2},
                new Zlecenie {id=4, nazwa="Adam Nowak",  status=3},
                new Zlecenie
                {
                    id=5,
                    nazwa="Janusz Adamski",
                    telefon="987654321",
                    uwagi="Uwaga",
                    data="2024-09-01",
                    status=1,
                    produkty = new List<Produkt>
                    {
                        new Produkt {id_produkt=1, indeks="wier_ryo", nazwa="Wiertartka ryobi", id_zlecenia=1},
                        new Produkt {id_produkt=2, indeks="lat_pro", nazwa="Latarka ProLine", id_zlecenia=1}
                    },
                    pliki = new List<Plik>
                    {
                        new Plik {id_plik=1, sciezka="./resources/plik.pdf", tytul="plik.pdf", id_zlecenia=1}
                    }
                }
            };
            BindingContext = this;
        }
        public Zlecenie DajZlecenie(int id)
        {
            foreach (var zlecenie in Zlecenia)
            {
                if (zlecenie.id == id)
                {
                    return zlecenie;
                }
            }
            return null;
        }
        private async void OtworzEdytor(object sender, EventArgs e)
        {
            try
            {
                var id = (int)((TappedEventArgs)e).Parameter;
                var zlecenie = DajZlecenie(id);
                var NoweOkno = new Window(new Edytor(zlecenie));
                NoweOkno.Width = 1187;
                Application.Current.OpenWindow(NoweOkno);
            }
            catch (Exception ex)
            {
                var NoweOkno = new Window(new Edytor());
                NoweOkno.Width = 1187;
                Application.Current.OpenWindow(NoweOkno);
            }
        }
    }
}
