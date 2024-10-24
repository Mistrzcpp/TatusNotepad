using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TatusNotepad
{
    public class Zlecenie
    {
        public int? id { get; set; }
        public string? nazwa { get; set; }
        public string? telefon { get; set; }
        public string? email { get; set; }
        public string? uwagi { get; set; }
        public string? data { get; set; }
        public string status { get; set; }
        public List<Produkt>? produkty {  get; set; }
        public List<Plik>? pliki { get; set; }
    }
}
