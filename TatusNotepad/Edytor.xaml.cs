using System.Diagnostics;

namespace TatusNotepad
{
	public partial class Edytor : ContentPage
	{
		public Zlecenie zlecenie { get; set; }
		public List<Produkt> produkty { get; set; }
		public List<Plik> pliki { get; set; }
		private bool nowy;
		private bool naPewnoUsun;
		private MainPage MainPage;
		public Edytor(Zlecenie zlecenie, MainPage mainPage)
		{
			InitializeComponent();
			MainPage = mainPage;
			this.zlecenie = zlecenie;
			this.Title = zlecenie.nazwa;
			Nazwa.Text = zlecenie.nazwa;
			Telefon.Text = zlecenie.telefon;
			Email.Text = zlecenie.email;
			Uwagi.Text = zlecenie.uwagi;
			if (zlecenie.produkty != null)
				produkty = zlecenie.produkty;
			else
				produkty = new List<Produkt>();

			if (zlecenie.pliki != null)
				pliki = zlecenie.pliki;
			else
				pliki = new List<Plik>();

			nowy = false;
			naPewnoUsun = false;
			BindingContext = this;
		}
		public Edytor(MainPage mainPage)
		{
			InitializeComponent();
			MainPage = mainPage;
			this.zlecenie = new Zlecenie();
			produkty = new List<Produkt>();
			pliki = new List<Plik>();
			nowy = true;
			naPewnoUsun = false;
			BindingContext = this;
		}
		public void UsunProdukt(object sender, EventArgs e)
		{
			string nazwa = (string)((TappedEventArgs)e).Parameter;
			produkty.RemoveAll(p => p.nazwa == nazwa);
            ZmienPrzyciskZapisz(sender, e);
            KolekcjaProdukty.ItemsSource = null;
            KolekcjaProdukty.ItemsSource = produkty;
		}
		public void DodajProdukt(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(NazwaProduktu.Text) || !string.IsNullOrWhiteSpace(Indeks.Text))
			{
				Produkt produkt = new Produkt();
				produkt.nazwa = NazwaProduktu.Text;
				produkt.indeks = Indeks.Text;
				produkt.id_zlecenia = zlecenie.id;
				produkty.Add(produkt);
				NazwaProduktu.Text = null;
				Indeks.Text = null;
                ZmienPrzyciskZapisz(sender, e);
                KolekcjaProdukty.ItemsSource = null;
                KolekcjaProdukty.ItemsSource = produkty;
			}

		}
		private void Zapisz(object sender, EventArgs e)
		{
			if (nowy)
			{
				zlecenie.id = null;
				zlecenie.data = DateTime.Now.ToString("yyyy-MM-dd");
				zlecenie.status = "Przyjête";
			}
			zlecenie.nazwa = Nazwa.Text;
			zlecenie.telefon = Telefon.Text;
			zlecenie.email = Email.Text;
			zlecenie.uwagi = Uwagi.Text;
			zlecenie.produkty = produkty;
			zlecenie.pliki = pliki;
			int? idZlecenia;
			if (nowy)
			{
				BazaDanych.DodajZlecenie(zlecenie);
                idZlecenia = BazaDanych.OstatnieId();
				nowy = !nowy;
            }
			else
			{
				BazaDanych.AktualizujZlecenie(zlecenie);
				idZlecenia = zlecenie.id;
			}

			foreach(var plik in zlecenie.pliki)
			{

                string aplikacjaFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                aplikacjaFolder = Path.Combine(aplikacjaFolder, "TatusNotepad");
                var folder = Path.Combine(aplikacjaFolder, idZlecenia.ToString());
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                string plikSciezka = Path.Combine(folder, plik.tytul);
				if(!File.Exists(plikSciezka))
					File.Copy(plik.sciezka, plikSciezka, true);
            }
			this.PrzyciskZapisz.Text = "Zapisano";
		}
		private void UsunZlecenie(object sender, EventArgs e)
		{
			if (!naPewnoUsun)
			{
				Usun.Text = "Czy na pewno chcesz usun¹æ zlecenie?";
				naPewnoUsun = true;
			}
			else
			{
				BazaDanych.UsunZlecenie(zlecenie);
				Application.Current.CloseWindow(this.Window);
            }
        }
		async void UpuszczonoPlik(object sender, DropEventArgs e) { }
		async public void DolaczPlik(object sender, EventArgs e)
		{
			var wskazanyPlik = await FilePicker.PickAsync();
			var plik = new Plik();
			if(wskazanyPlik != null)
			{
				plik.tytul = wskazanyPlik.FileName;
				plik.sciezka = wskazanyPlik.FullPath;
				plik.id_plik = null;
				plik.id_zlecenia = null;
				pliki.Add(plik);
            }
			zlecenie.pliki = pliki;
			ZmienPrzyciskZapisz(sender, e);
            KolekcjaPlikow.ItemsSource = null;
            KolekcjaPlikow.ItemsSource = pliki;
        }
		public void UsunPlik(object sender, TappedEventArgs e)
		{
			string tytul = (string)e.Parameter;
			var plikiNowe = new List<Plik>(pliki);
			foreach(var plik in pliki)
			{
				if(plik.tytul == tytul)
				{
					var sciezka = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    sciezka = Path.Combine(sciezka, "TatusNotepad");
                    sciezka = Path.Combine(sciezka, plik.id_zlecenia.ToString());
					var sciezkaPlik = Path.Combine(sciezka, plik.tytul);
					File.Delete(sciezkaPlik);
					plikiNowe.Remove(plik);
					if (!Directory.GetFiles(sciezka).Any())
					{
						Directory.Delete(sciezka);
					}
				}
            }
			pliki = plikiNowe;
            ZmienPrzyciskZapisz(sender, e);
            KolekcjaPlikow.ItemsSource = null;
            KolekcjaPlikow.ItemsSource = pliki;
		}
		private void ZmienPrzyciskZapisz(object sender, EventArgs e) 
		{
			if (PrzyciskZapisz.Text == "Zapisano")
				PrzyciskZapisz.Text = "Zapisz";
        }
		private void OtworzPlik(object sender, TappedEventArgs e)
		{
			string tytul = e.Parameter.ToString();
			string sciezka = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            sciezka = Path.Combine(sciezka, "TatusNotepad");
            sciezka = Path.Combine(sciezka, zlecenie.id.ToString());
			sciezka = Path.Combine(sciezka, tytul);
            Process.Start(new ProcessStartInfo(sciezka) { UseShellExecute = true });
        }
        private void OtworzFolder(object sender, TappedEventArgs e)
        {
            string tytul = e.Parameter.ToString();
			string sciezka = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            sciezka = Path.Combine(sciezka, "TatusNotepad");
            sciezka = Path.Combine(sciezka, zlecenie.id.ToString());
            Process.Start("explorer.exe", sciezka);
        }

		
    }
}