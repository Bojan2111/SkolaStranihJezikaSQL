using SkolaStranihJezikaSQL.Pomocni;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkolaStranihJezikaSQL.DAO
{
    internal class UcenikDAO
    {
        // Funkcije koje pozivaju vezu sa bazom, a kao argument uzimaju funkciju u cijem se telu
        // odvija query za zadatu vrstu transakcije.
        public static void UbacivanjeNovogUcenika()
        {
            Veza.Uspostavljanje(UbacivanjeUcenika);
        }
        public static void IspisivanjeSvihUcenika()
        {
            Veza.Uspostavljanje(IspisSvihUcenika);
        }

        // Funkcije sa detaljima o transakciji - ispis, unos, provera...
        public static void UbacivanjeUcenika(SqlConnection conn)
        {
            Console.WriteLine("Unesite ime ucenika:");
            string ime = Console.ReadLine();
            Console.WriteLine("Unesite prezime ucenika:");
            string prezime = Console.ReadLine();

            string insertString = "INSERT INTO ucenici " +
                "(ime, prezime) " +
                "VALUES (@ime, @prezime);";
            SqlCommand cmd = new SqlCommand(insertString, conn);
            cmd.Parameters.AddWithValue("@ime", ime);
            cmd.Parameters.AddWithValue("@prezime", prezime);
            cmd.ExecuteNonQuery();
            Console.WriteLine($"\nUcenik {ime} {prezime} uspesno dodat!");
        }
        public static void IspisSvihUcenika(SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand("select * from ucenici", conn);
            SqlDataReader rdr = cmd.ExecuteReader();

            Console.WriteLine($"Prikaz svih ucenika:\n" +
                    $"{new string('=', 33)}\n");
            while (rdr.Read())
            {
                int id = int.Parse(rdr["id"].ToString());
                string ime = rdr["ime"].ToString();
                string prezime = rdr["prezime"].ToString();

                Console.WriteLine($"[ID: {id}] - {ime} {prezime}");
            }
            rdr.Close();
            Console.WriteLine($"{new string('=', 33)}\n");
        }
    }
}
