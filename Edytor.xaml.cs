namespace TatusNotepad;

public partial class Edytor : ContentPage
{
	public Zlecenie zlecenie { get; set; }
	public List<Produkt> produkty { get; set; }
	public List<Plik> pliki { get; set; }
	public Edytor(Zlecenie zlecenie)
	{
		InitializeComponent();
		this.zlecenie = zlecenie;
		Nazwa.Text = zlecenie.nazwa;
		Telefon.Text = zlecenie.telefon;
		Uwagi.Text = zlecenie.uwagi;
		produkty = zlecenie.produkty;
		pliki = zlecenie.pliki;
        BindingContext = this;
    }
	public Edytor()
	{
		InitializeComponent();
	}
	public void UsunProdukt(object sender, EventArgs e)
	{
		int id = (int)((TappedEventArgs)e).Parameter;
		produkty.RemoveAll(p => p.id_produkt == id);
		KolekcjaProdukty.ItemsSource = null;
		KolekcjaProdukty.ItemsSource = produkty;
	}
	public void DodajProdukt(object sender, EventArgs e)
	{
		if(!string.IsNullOrWhiteSpace(NazwaProduktu.Text) || !string.IsNullOrWhiteSpace(Indeks.Text)){
			Produkt produkt = new Produkt();
			produkt.nazwa = NazwaProduktu.Text;
			produkt.indeks = Indeks.Text;
			produkt.id_zlecenia = zlecenie.id;
			produkty.Add(produkt);
			NazwaProduktu.Text = null;
			Indeks.Text = null;
			KolekcjaProdukty.ItemsSource = null;
			KolekcjaProdukty.ItemsSource = produkty;
		}
		
    }
    private void Zapisz(object sender, EventArgs e)
    {
		zlecenie.nazwa = Nazwa.Text;
		zlecenie.telefon = Telefon.Text;
		zlecenie.uwagi = Uwagi.Text;
		zlecenie.produkty = produkty;
		zlecenie.pliki = pliki;
    }
}