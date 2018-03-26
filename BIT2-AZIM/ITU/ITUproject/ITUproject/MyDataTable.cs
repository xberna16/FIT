using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITUproject
{
    public class MyDataTable : System.Data.DataTable
    {
        public MyDataTable()
        {
            Columns.Add("Typ",typeof(string));
            Columns.Add("Popis", typeof(string));
            Columns.Add("Zdroj", typeof(string));
            Columns.Add("Cas",typeof(DateTime));
        }
        public void Add(Udalost udalost)
        {
            System.Data.DataRow radek = NewRow();
            radek["Typ"] = udalost.Typ;
            radek["Popis"] = udalost.Popis;
            radek["Zdroj"] = udalost.Zdroj;
            radek["Cas"] = udalost.Cas;
            Rows.Add(radek);

        }
    }
}
