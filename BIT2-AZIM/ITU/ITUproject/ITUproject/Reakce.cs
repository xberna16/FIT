using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITUproject
{
    public class Reakce
    {
        public string Nazev { get; set; }
        public string Typ { get; set; }
        public string Popis { get; set; }
        public string Zdroj { get; set; }
        public string Cas { get; set; }

        public string Email { get; set; }
        public string TextEmailu { get; set; }

        public string Filtr { get; set; }

        public string CestaKProgramu { get; set; }

        public string TypNove { get; set; }
        public string PopisNove { get; set; }

        public Reakce(string nazev, string typ, string popis, string zdroj, string cas)
        {
            Nazev = nazev;
            Typ = typ;
            Popis = popis;
            Zdroj = zdroj;
            Cas = cas;
        }

        public void spachat()
        {
            if (Email != "")
                System.Windows.Forms.MessageBox.Show("Posilam mail na " + Email
                    + "s textem: " + TextEmailu);

            if (CestaKProgramu != "")
                System.Windows.Forms.MessageBox.Show("Spustim program v: " + CestaKProgramu);


            if (TypNove != "" && PopisNove != "")
                System.Windows.Forms.MessageBox.Show("Vyvolavam udalost: " + PopisNove
                    + " typu " + TypNove);  // TODO pridat dalsi udalost
        }
    }
}
