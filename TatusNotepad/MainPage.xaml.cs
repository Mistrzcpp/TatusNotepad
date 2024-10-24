using Microsoft.Maui.Controls;
using System.Globalization;
using System;
using Microsoft.Maui.Controls;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Platform;
using Microsoft.Maui;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using System.Data.SQLite;

namespace TatusNotepad
{

    public partial class MainPage : ContentPage
    {
        public List<Zlecenie>? Zlecenia { get; set; }
        public MainPage()
        {
            InitializeComponent();
            try
            {
                string sciezka = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                sciezka = Path.Combine(sciezka, "TatusNotepad");
                if(!Directory.Exists(sciezka))
                {
                    Directory.CreateDirectory(sciezka);
                    sciezka = Path.Combine(sciezka, "TatusNotepad.db");
                    SQLiteConnection.CreateFile(sciezka);
                    BazaDanych.UtworzBazeDanych();
                }
                string filtr = ZwrocFiltry().Substring(4);
                Sortowanie.SelectedItem = "Status";
                Zlecenia = BazaDanych.PobierzZlecenia($"SELECT * FROM zlecenie WHERE {filtr}");
                PickerDataDo.Date = DateTime.Now;
                Sortuj();
                BindingContext = this;
                PickerDataOd.MaximumDate = DateTime.Now;
                PickerDataDo.MaximumDate = DateTime.Now;
            }
            catch (Exception ex)
            {
                File.WriteAllText("log.txt", ex.ToString());
                throw ex;
            }
        }
        public Zlecenie PobierzZlecenie(int id)
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
        public async void OtworzEdytor(object sender, EventArgs e)
        {
            try
            {
                var id = (int)((TappedEventArgs)e).Parameter;
                var zlecenie = PobierzZlecenie(id);
                var NoweOkno = new Window(new Edytor(zlecenie, this));
                NoweOkno.Width = 1187;
                Application.Current.OpenWindow(NoweOkno);
                
            }
            catch (Exception ex)
            {
                var NoweOkno = new Window(new Edytor(this));
                NoweOkno.Width = 1187;
                Application.Current.OpenWindow(NoweOkno);
            }
            
        }
        public void ZmienStatus(object sender, EventArgs e)
        {
            var button = sender as Button;
            var id = (int)button.CommandParameter;
            foreach (var zlecenie in Zlecenia)
            {
                if(zlecenie.id == id)
                {
                    if (zlecenie.status == "Przyjęte")
                        zlecenie.status = "Zamówione";
                    else if (zlecenie.status == "Zamówione")
                        zlecenie.status = "Powiadomiono";
                    else if (zlecenie.status == "Powiadomiono")
                        zlecenie.status = "Zrealizowane";
                    else if (zlecenie.status == "Zrealizowane")
                        zlecenie.status = "Przyjęte";

                    BazaDanych.AktualizujZlecenie(zlecenie);
                }
            }
            KolekcjaZlecen.ItemsSource = null;
            KolekcjaZlecen.ItemsSource = Zlecenia;
        }
        private void WyszukajPoTekscie(object sender, TextChangedEventArgs e)
        {
            string haslo = e.NewTextValue.Trim().Replace("'", string.Empty);
            Wyszukaj(haslo);
        }
        private void WyszukajPoPrzycisku(object sender, EventArgs e)
        {
            string haslo = Wyszukiwarka.Text.Trim().Replace("'", string.Empty);
            Wyszukaj(haslo);
        }
        private void WyszukajPoFiltrze(object sender, EventArgs e)
        {
            string haslo = "";
            Wyszukaj(haslo);
        }
        private void Wyszukaj(string haslo)
        {
            string filtry = ZwrocFiltry();
            if (haslo.StartsWith('#') && haslo.Length > 1 && !string.IsNullOrEmpty(haslo))
            {
                haslo = haslo.Substring(1);
                Zlecenia = BazaDanych.PobierzZlecenia($"SELECT * FROM zlecenie WHERE (id LIKE '%{haslo}%') {filtry}");
            }
            else
            {
                haslo = "%" + haslo + "%";
                Zlecenia = BazaDanych.PobierzZlecenia(
                    "SELECT z.*, " +
                        "(SELECT GROUP_CONCAT(p.nazwa, ' ') FROM produkt p WHERE p.id_zlecenia = z.id) AS produkty, " +
                        "(SELECT GROUP_CONCAT(pl.tytul, ' ') FROM plik pl WHERE pl.id_zlecenia = z.id) AS pliki " +
                    "FROM zlecenie z " +
                    "WHERE (" +
                        (HasloToId(haslo) ? $"id = {haslo} OR" : "") +
                        $"nazwa LIKE '{haslo}' " +
                        $"OR telefon LIKE '{haslo}' " +
                        $"OR uwagi LIKE '{haslo}' " +
                        $"OR data LIKE '{haslo}' " +
                        $"OR status LIKE '{haslo}' " +
                        $"OR email LIKE '{haslo}' " +
                        $"OR produkty LIKE '{haslo}' " +
                        $"OR pliki LIKE '{haslo}')" +
                        $"{filtry}");
            }
            Sortuj();
            KolekcjaZlecen.ItemsSource = null;
            KolekcjaZlecen.ItemsSource = Zlecenia;
        }
        private bool HasloToId(string haslo)
        {
            return !string.IsNullOrEmpty(haslo) && int.TryParse(haslo, out _);
        }
        private void PokazFiltry(object sender, EventArgs e)
        {
            Filtry.IsVisible = !Filtry.IsVisible;
        }
        private string ZwrocFiltry()
        {
            var filtry = new List<string>();
            if (Przyjete.IsChecked)
                filtry.Add("status='Przyjęte'");
            if (Zamowione.IsChecked)
                filtry.Add("status='Zamówione'");
            if (Powiadomiono.IsChecked)
                filtry.Add("status='Powiadomiono'");
            if (Zrealizowane.IsChecked)
                filtry.Add("status='Zrealizowane'");

            string dataSzablon = "";
            if(!string.IsNullOrEmpty(FiltrRok.Text) || !string.IsNullOrEmpty(FiltrMiesiac.Text) || !string.IsNullOrEmpty(FiltrDzien.Text))
            {
                string rok = FiltrRok.Text;
                string miesiac = FiltrMiesiac.Text;
                string dzien = FiltrDzien.Text;
                if (!string.IsNullOrEmpty(miesiac) && miesiac.Length == 1)
                    miesiac = "0" + miesiac;
                if (!string.IsNullOrEmpty(dzien) && dzien.Length == 1)
                    dzien = "0" + dzien;

                if (string.IsNullOrEmpty(rok))
                    dataSzablon += "____-";
                else
                    dataSzablon += rok + "-";
                if(string.IsNullOrEmpty(miesiac))
                    dataSzablon += "__-";
                else
                    dataSzablon += miesiac + "-";
                if (string.IsNullOrEmpty(dzien))
                    dataSzablon += "__";
                else
                    dataSzablon += dzien;
                dataSzablon = "data LIKE '" + dataSzablon + "'";
            }
            else
            {
                string dataOd = PickerDataOd.Date.ToString("yyyy-MM-dd");
                string dataDo = PickerDataDo.Date.ToString("yyyy-MM-dd");

                dataSzablon = "data >='" + dataOd + "' AND data <='" + dataDo + "'";
            }
            
            

            if (filtry.Count > 0 && dataSzablon != "")
                return "AND (" + string.Join(" OR ", filtry) + $") AND ({dataSzablon})";
            else if(filtry.Count > 0 && dataSzablon == "")
                return "AND (" + string.Join(" OR ", filtry) + ")";
            else if (filtry.Count==0 && dataSzablon != "")
                return $"AND ({dataSzablon})";
            else
                return "";
        }
        private void Sortuj()
        {
            if (Sortowanie.SelectedItem == null)
                return;

            if (Sortowanie.SelectedItem.ToString() == "Status")
            {
                var z = new List<Zlecenie>(Zlecenia).OrderBy(z =>
                {
                    switch (z.status)
                    {
                        case "Przyjęte":
                            return 1;
                        case "Zamówione":
                            return 2;
                        case "Powiadomiono":
                            return 3;
                        case "Zrealizowane":
                            return 4;
                        default:
                            return 5;
                    }
                }).ThenByDescending(z => z.data).ToList();
                Zlecenia = z;
            }
            else if(Sortowanie.SelectedItem.ToString() == "Od najstarszych")
            {
                var z = new List<Zlecenie>(Zlecenia).OrderBy(z => z.data).ToList();
                Zlecenia = z;
            }
            else if (Sortowanie.SelectedItem.ToString() == "Od najnowszych")
            {
                var z = new List<Zlecenie>(Zlecenia).OrderByDescending(z => z.data).ToList();
                Zlecenia = z;
            }
        }
        private void WybierzStatus(object sender, TappedEventArgs e)
        {
            string status = e.Parameter.ToString();
            if (status == "Przyjęte")
                Przyjete.IsChecked = !Przyjete.IsChecked;
            else if (status == "Zamówione")
                Zamowione.IsChecked = !Zamowione.IsChecked;
            else if (status == "Powiadomiono")
                Powiadomiono.IsChecked = !Powiadomiono.IsChecked;
            else if (status == "Zrealizowane")
                Zrealizowane.IsChecked = !Zrealizowane.IsChecked;
        }
        private void KonkretnaData(object sender, EventArgs e)
        {
            PickerDataOd.Date = DateTime.Parse("01/01/2024");
            PickerDataDo.Date = DateTime.Now;
            WyszukajPoFiltrze(sender, e);
        }
        private void DataOdDo(object sender, EventArgs e)
        {
            FiltrRok.Text = string.Empty;
            FiltrMiesiac.Text = string.Empty;
            FiltrDzien.Text = string.Empty;
            WyszukajPoFiltrze(sender, e);
        }
    }
    
}
