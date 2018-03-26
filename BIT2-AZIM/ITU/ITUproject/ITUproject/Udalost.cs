using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITUproject
{
    public class Udalost
    {
        public string Typ { get; set; }
        public string Popis { get; set; }
        public string Zdroj { get; set; }
        public DateTime Cas { get; set; }

        public Udalost( string typ, string popis, string zdroj, DateTime cas)
        {
            Typ = typ;
            Popis = popis;
            Zdroj = zdroj;
            Cas = cas;
        }
    }
}
