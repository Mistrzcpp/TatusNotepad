using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Resources;
using Microsoft.Maui.ApplicationModel.Communication;
using static System.Net.Mime.MediaTypeNames;

namespace TatusNotepad
{
    public class BazaDanych
    {
        static public List<Zlecenie> PobierzZlecenia(string query)
        {
            List<Zlecenie> Zlecenia = new List<Zlecenie>();
            string sciezka = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            sciezka = Path.Combine(sciezka, @"TatusNotepad\\TatusNotepad.db");
            SQLiteConnection polaczenie = new SQLiteConnection($"Data Source={sciezka};Version=3;");
            polaczenie.Open();

            SQLiteCommand polecenie = new SQLiteCommand(query, polaczenie);
            SQLiteDataReader dane = polecenie.ExecuteReader();
            while (dane.Read())
            {
                Zlecenie zlecenie = new Zlecenie
                {
                    id = dane.GetInt32(0),
                    nazwa = GetDbString(dane[1]),
                    telefon = GetDbString(dane[2]),
                    uwagi = GetDbString(dane[3]),
                    data = GetDbString(dane[4]),
                    status = dane.GetString(5),
                    email = GetDbString(dane[6]),
                };
                Zlecenia.Add(zlecenie);
            }

            foreach (var zlecenie in Zlecenia)
            {
                List<Produkt> Produkty = new List<Produkt>();
                query = $"SELECT * FROM produkt WHERE id_zlecenia={zlecenie.id}";
                polecenie = new SQLiteCommand(query, polaczenie);
                dane = polecenie.ExecuteReader();
                while (dane.Read())
                {
                    Produkt produkt = new Produkt()
                    {
                        id_produkt = dane.GetInt32(0),
                        indeks = GetDbString(dane[1]),
                        nazwa = GetDbString(dane[2]),
                        id_zlecenia = dane.GetInt32(3)
                    };
                    Produkty.Add(produkt);
                }
                zlecenie.produkty = Produkty;
            }
            foreach(var zlecenie in Zlecenia)
            {
                List<Plik> Pliki = new List<Plik>();
                query = $"SELECT * FROM plik WHERE id_zlecenia='{zlecenie.id}'";
                polecenie = new SQLiteCommand(query, polaczenie);
                dane = polecenie.ExecuteReader();
                while (dane.Read())
                {
                    var plik = new Plik()
                    {
                        id_plik = dane.GetInt32(0),
                        tytul = GetDbString(dane[1]),
                        id_zlecenia = dane.GetInt32(2)
                    };
                    Pliki.Add(plik);
                }
                zlecenie.pliki = Pliki;
            }
            polaczenie.Close();
            return Zlecenia;
        }
        static private string? GetDbString(object value)
        {
            return value is null ? null : value.ToString();
        }
        static public void DodajZlecenie(Zlecenie zlecenie)
        {
            string sciezka = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            sciezka = Path.Combine(sciezka, @"TatusNotepad\\TatusNotepad.db");
            SQLiteConnection polaczenie = new SQLiteConnection($"Data Source={sciezka};Version=3;");
            polaczenie.Open();

            string query = $"INSERT INTO zlecenie VALUES (NULL, '{zlecenie.nazwa}', '{zlecenie.telefon}', '{zlecenie.uwagi}', '{zlecenie.data}', '{zlecenie.status}', '{zlecenie.email}')";
            SQLiteCommand polecenie = new SQLiteCommand(query, polaczenie);
            polecenie.ExecuteNonQuery();

            query = $"SELECT id FROM zlecenie ORDER BY id DESC LIMIT 1";
            polecenie = new SQLiteCommand(query, polaczenie);
            SQLiteDataReader dane = polecenie.ExecuteReader();
            dane.Read();
            zlecenie.id = dane.GetInt32(0);

            foreach (var produkt in zlecenie.produkty)
            {
                query = $"INSERT INTO produkt VALUES (NULL, '{produkt.indeks}', '{produkt.nazwa}', '{zlecenie.id}')";
                polecenie = new SQLiteCommand(query, polaczenie);
                polecenie.ExecuteNonQuery();
            }
            foreach(var plik in zlecenie.pliki)
            {
                query = $"INSERT INTO plik VALUES (NULL, '{plik.tytul}', '{zlecenie.id}')";
                polecenie = new SQLiteCommand(query, polaczenie);
                polecenie.ExecuteNonQuery();
            }

            polaczenie.Close();
        }
        static public void AktualizujZlecenie(Zlecenie zlecenie)
        {
            string sciezka = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            sciezka = Path.Combine(sciezka, @"TatusNotepad\\TatusNotepad.db");
            SQLiteConnection polaczenie = new SQLiteConnection($"Data Source={sciezka};Version=3;");
            polaczenie.Open();

            string query = $"UPDATE zlecenie SET nazwa='{zlecenie.nazwa}', " +
                $"telefon='{zlecenie.telefon}', " +
                $"uwagi='{zlecenie.uwagi}', " +
                $"data='{zlecenie.data}', " +
                $"status='{zlecenie.status}'," +
                $"email='{zlecenie.email}'" +
                $"WHERE id={zlecenie.id}";
            SQLiteCommand polecenie = new SQLiteCommand(query, polaczenie);
            polecenie.ExecuteNonQuery();

            query = $"DELETE FROM produkt WHERE id_zlecenia='{zlecenie.id}'";
            polecenie = new SQLiteCommand(query, polaczenie);
            polecenie.ExecuteNonQuery();

            foreach (var produkt in zlecenie.produkty)
            {
                query = $"INSERT INTO produkt VALUES (NULL, '{produkt.indeks}', '{produkt.nazwa}', '{zlecenie.id}')";
                polecenie = new SQLiteCommand(query, polaczenie);
                polecenie.ExecuteNonQuery();
            }

            query = $"DELETE FROM plik WHERE id_zlecenia={zlecenie.id}";
            polecenie = new SQLiteCommand(query, polaczenie);
            polecenie.ExecuteNonQuery();

            foreach (var plik in zlecenie.pliki)
            {
                query = $"INSERT INTO plik VALUES (NULL, '{plik.tytul}', '{zlecenie.id}')";
                polecenie = new SQLiteCommand(query, polaczenie);
                polecenie.ExecuteNonQuery();
            }
            polaczenie.Close();
        }
        static public void UsunZlecenie(Zlecenie zlecenie)
        {
            string sciezka = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            sciezka = Path.Combine(sciezka, @"TatusNotepad\\TatusNotepad.db");
            SQLiteConnection polaczenie = new SQLiteConnection($"Data Source={sciezka};Version=3;");
            polaczenie.Open();
            string query = $"DELETE FROM zlecenie WHERE id='{zlecenie.id}'";
            SQLiteCommand polecenie = new SQLiteCommand(query, polaczenie);
            polecenie.ExecuteNonQuery();
            query = $"DELETE FROM produkt WHERE id_zlecenia='{zlecenie.id}'";
            polecenie = new SQLiteCommand(query, polaczenie);
            polecenie.ExecuteNonQuery();
            query = $"DELETE FROM plik WHERE id_zlecenia='{zlecenie.id}'";
            polecenie = new SQLiteCommand(query, polaczenie);
            polecenie.ExecuteNonQuery();
            polaczenie.Close();
        }
        static public int OstatnieId()
        {
            string sciezka = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            sciezka = Path.Combine(sciezka, @"TatusNotepad\\TatusNotepad.db");
            SQLiteConnection polaczenie = new SQLiteConnection($"Data Source={sciezka};Version=3;");
            polaczenie.Open();
            string query = "SELECT id FROM zlecenie ORDER BY id DESC LIMIT 1";
            SQLiteCommand polecenie = new SQLiteCommand(query, polaczenie);
            SQLiteDataReader dane = polecenie.ExecuteReader();
            dane.Read();
            int id = dane.GetInt32(0);
            polaczenie.Close();
            return id;
        }
        static public void UtworzBazeDanych()
        {
            string sciezka = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            sciezka = Path.Combine(sciezka, @"TatusNotepad\\TatusNotepad.db");
            SQLiteConnection polaczenie = new SQLiteConnection($"Data Source={sciezka};Version=3;");
            polaczenie.Open();

            string query = @"CREATE TABLE 'zlecenie'(
                'id'  INTEGER, " +
                "'nazwa'    TEXT, " +
                "'telefon' TEXT, " +
                "'uwagi'   TEXT, " +
                "'data'    TEXT, " +
                "'status'  TEXT, " +
                "'email' TEXT, " +
                "PRIMARY KEY(id AUTOINCREMENT))";
            SQLiteCommand polecenie = new SQLiteCommand(query, polaczenie);
            polecenie.ExecuteNonQuery();

            query = @"CREATE TABLE 'plik'( " +
                "'id_plik'	INTEGER NOT NULL UNIQUE, " +
                "'tytul'	TEXT NOT NULL, " +
                "'id_zlecenia'	INTEGER NOT NULL, " +
                "PRIMARY KEY(id_plik AUTOINCREMENT), " +
                "FOREIGN KEY(id_zlecenia) REFERENCES zlecenie(id) ON DELETE CASCADE )";
            polecenie = new SQLiteCommand(query, polaczenie);
            polecenie.ExecuteNonQuery();

            query = @"CREATE TABLE 'produkt'( " +
                "'id_produkt'	INTEGER NOT NULL UNIQUE, " +
                "'indeks'	TEXT, " +
                "'nazwa'	TEXT, " +
                "'id_zlecenia'	INTEGER NOT NULL, " +
                "PRIMARY KEY(id_produkt AUTOINCREMENT), " +
                "FOREIGN KEY(id_zlecenia) REFERENCES zlecenie(id) ON DELETE CASCADE )";
            polecenie = new SQLiteCommand(query, polaczenie);
            polecenie.ExecuteNonQuery();

            polaczenie.Close();
        }
    }
}
