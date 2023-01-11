using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KonzolniMeni;
using SkolaStranihJezikaSQL.DAO;

namespace SkolaStranihJezikaSQL.UI
{
    internal class GlavniMeni
    {
        public static void IspisMenija()
        {
            Meni glMeni = new Meni();
            glMeni.DodajOpciju(KursDAO.PrikazSvihKurseva, "Prikaz svih kurseva");
            glMeni.DodajOpciju(UcenikDAO.IspisivanjeSvihUcenika, "Prikaz svih ucenika");
            glMeni.DodajOpciju(UcenikDAO.UbacivanjeNovogUcenika, "Ubacivanje novog ucenika");
            glMeni.DodajOpciju(KursDAO.UpisivanjeUcenikaNaKurs, "Upisivanje ucenika na kurs");
            glMeni.DodajOpciju(KursDAO.PrikazKursevaPoZeljenomJeziku, "Prikaz kurseva po zeljenom jeziku");
            glMeni.Pokreni();
        }
    }
}
