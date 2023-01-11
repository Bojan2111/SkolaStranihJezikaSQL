using SkolaStranihJezikaSQL.Pomocni;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkolaStranihJezikaSQL.DAO
{
    internal class KursDAO
    {
        public static void PrikazSvihKurseva()
        {
            Veza.Uspostavljanje(UcitavanjeKurseva);
        }
        public static void UpisivanjeUcenikaNaKurs()
        {
            Veza.Uspostavljanje(UpisNaKurs);
        }
        public static void PrikazKursevaPoZeljenomJeziku()
        {
            Veza.Uspostavljanje(IspisKursaPoJeziku);
        }
        public static void UcitavanjeKurseva(SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand("select * from kursevi", conn);
            SqlDataReader rdr = cmd.ExecuteReader();

            Console.WriteLine($"Prikaz svih kurseva:\n" +
                    $"{new string('=', 33)}\n");
            while (rdr.Read())
            {
                int id = int.Parse(rdr["id"].ToString());
                string naziv = rdr["naziv"].ToString();
                int brojUcenika = int.Parse(rdr["broj_ucenika"].ToString());
                bool aktivan = (bool)rdr["aktivan"];
                string aktivanStr = aktivan ? "Da" : "Ne";
                string jezik = rdr["jezik"].ToString();

                Console.WriteLine($"[ID: {id}] - {naziv}\n" +
                    $"Broj ucenika: {brojUcenika}\n" +
                    $"Aktivan: {aktivanStr}\n" +
                    $"Strani jezik: {jezik}\n" +
                    $"{new string('-', 33)}\n");
            }
            rdr.Close();
            Console.WriteLine($"{new string('=', 33)}\n");
        }
        public static void DostupniKursevi(SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand("select id, naziv from kursevi where broj_ucenika < 8 and aktivan=1", conn);
            SqlDataReader rdr = cmd.ExecuteReader();

            Console.WriteLine("Prikaz dostupnih kurseva:\n");
            while (rdr.Read())
            {
                int id = int.Parse(rdr["id"].ToString());
                string naziv = rdr["naziv"].ToString();

                Console.WriteLine($"[ID: {id}] - {naziv}");
            }
            rdr.Close();
        }
        public static bool ProveraUcenikaNaKursu(SqlConnection conn, int idUcenika, int idKursa)
        {
            string provera = $"select id_kursa from kursevi_ucenika where id_ucenika = {idUcenika}";
            SqlCommand cmd = new SqlCommand(provera, conn);
            SqlDataReader rdr = cmd.ExecuteReader();

            // Jedina lista koju sam koristio u ovoj verziji jer ne mogu doci do adekvatnog koda za SQL query
            List<int> listaKurseva = new List<int>();
            while (rdr.Read())
            {
                int id = int.Parse(rdr["id_kursa"].ToString());

                listaKurseva.Add(id);
            }
            rdr.Close();
            return listaKurseva.Contains(idKursa);
        }
        public static void UpisNaKurs(SqlConnection conn)
        {
            UcenikDAO.IspisSvihUcenika(conn);
            Console.WriteLine("\nUnesite ID ucenika kojeg zelite ubaciti u kurs:");
            int idUcenika = int.Parse(Console.ReadLine());

            DostupniKursevi(conn);
            Console.WriteLine("Unesite ID kursa u koji zelite ubaciti ucenika:");
            int idKursa = int.Parse(Console.ReadLine());

            // provera da li ucenik vec pohadja odabrani kurs.
            if (!ProveraUcenikaNaKursu(conn, idUcenika, idKursa))
            {
                // update broja ucenika - prvo ova transakcija jer je u bazi vezana za constraint 'provera_broja_ucenika'
                // drugim recima, prekinuce sve naredne transakcije u ovoj funkciji i zatvoriti vezu sa bazom.
                string readString = $"update kursevi set broj_ucenika = ISNULL(broj_ucenika + 1, 1) where id={idKursa}";
                SqlCommand cmdUpdate = new SqlCommand(readString, conn);
                cmdUpdate.ExecuteNonQuery();

                // unos u medjutabelu
                string insertString = "INSERT INTO kursevi_ucenika " +
                    "(id_ucenika, id_kursa) " +
                    "VALUES (@id_ucenika, @id_kursa);";
                SqlCommand cmd = new SqlCommand(insertString, conn);
                cmd.Parameters.AddWithValue("@id_ucenika", idUcenika);
                cmd.Parameters.AddWithValue("@id_kursa", idKursa);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Ucenik uspesno ubacen na kurs!");
            }
            else
            {
                Console.WriteLine("Ucenik vec pohadja izabrani kurs.");
            }
        }
        public static void IspisKursaPoJeziku(SqlConnection conn)
        {
            Console.WriteLine("Unesite strani jezik za ispis kurseva:");
            string zeljeniJezik = Console.ReadLine();
            SqlCommand cmd = new SqlCommand($"select * from kursevi where jezik='{zeljeniJezik}'", conn);
            SqlDataReader rdr = cmd.ExecuteReader();

            Console.WriteLine($"Prikaz svih kurseva gde se uci {zeljeniJezik} jezik:\n" +
                    $"{new string('=', 33)}\n");
            while (rdr.Read())
            {
                int id = int.Parse(rdr["id"].ToString());
                string naziv = rdr["naziv"].ToString();
                int brojUcenika = int.Parse(rdr["broj_ucenika"].ToString());
                bool aktivan = (bool)rdr["aktivan"];
                string aktivanStr = aktivan ? "Da" : "Ne";
                string jezik = rdr["jezik"].ToString();

                Console.WriteLine($"[ID: {id}] - {naziv}\n" +
                    $"Broj ucenika: {brojUcenika}\n" +
                    $"Aktivan: {aktivanStr}\n" +
                    $"Strani jezik: {jezik}\n" +
                    $"{new string('-', 33)}\n");
            }
            rdr.Close();
            Console.WriteLine($"{new string('=', 33)}\n");
        }
        /* Omogućiti korisniku aplikacije ispis svih kurseva na kojima se uči željeni strani
         *  jezik.
         * 2) Izmeniti sistem tako da maksimalan broj učenika na kursu nije osam, već se može
         *  menjati za svaki kurs pojedinačno.
         * 3) Napraviti jedinični test koji proverava funkcionalnost metoda za ubacivanje
         *  postojećeg učenika na željeni kurs (osnovna funkcionalnost br. 3). Ukoliko je
         *  neophodno, izmeniti sistem tako da testiranje ove funkcinalnosti može biti
         *  nezavisno od drugih delova sistema.
         * 4) Omogućiti „izvoz“ (export) podataka o jednom kursu u .PDF datoteku. Dakle,
         *  korisnik aplikacije može da odabere jedan kurs, pri čemu aplikacija treba da
         *  podatke o tom kursu, zajedno sa učenicima koji ga slušaju sačuva u .PDF
         *  datoteku
         */
    }
}
