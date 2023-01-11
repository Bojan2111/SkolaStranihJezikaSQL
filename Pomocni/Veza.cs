using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkolaStranihJezikaSQL.Pomocni
{
    internal class Veza
    {
        private static string nazivServera = ".\\SQLEXPRESS";
        private static string nazivBaze = "Skola";
        private static string korisnickoIme = "";
        private static string lozinka = "";
        private static bool integrisanaVeza = true;

        public delegate void SqlFunkcija(SqlConnection conn);
        
        public static void Uspostavljanje(SqlFunkcija funk)
        {
            string bezbednosnaProvera = (string.IsNullOrEmpty(Veza.korisnickoIme)) ? $"Integrated Security={integrisanaVeza}" : $"Username={Veza.korisnickoIme}; Password={Veza.lozinka}";
            SqlConnection conn = new SqlConnection($"Server ={Veza.nazivServera}; Database = {nazivBaze}; {bezbednosnaProvera};");
            try
            {
                conn.Open();

                funk(conn);
            }
            catch (Exception e)
            {
                Console.WriteLine("Desila se greska prilikom pristupa bazi\n" + e.Message);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
