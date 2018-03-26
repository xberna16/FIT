using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ITUproject
{
    public partial class Form1 : Form
    {
        MyDataTable udalosti = new MyDataTable();
        BindingSource bs = new BindingSource();
        BindingSource reakce = new BindingSource();

        public Form1()
        {
            InitializeComponent();
            bs.DataSource = udalosti.DefaultView;
            eventDataGridView.DataSource = bs;
            reactionsDataGridView.DataSource = reakce;
            eventDataGridView.DefaultCellStyle.Format = "tt hh:mm:ss dd/MM/yyyy";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (new Random().Next(0, 8))
            {
                case 0:
                   
                    udalosti.Add(new Udalost("Chyba", "Připojení se nezdařilo", "Windows Updater", DateTime.Now));
                    break;
                case 1:
                    udalosti.Add(new Udalost("Chyba", "Služba nebyla spuštěna", "Windows Defender", DateTime.Now));
                    break;
                case 2:
                    udalosti.Add(new Udalost("Informace", "Řídící příkaz byl odeslán", "Service Control Manager", DateTime.Now));
                    break;
                case 3:
                    udalosti.Add(new Udalost("Varování", "Nepodařilo se synchronizovat čas", "W32Time", DateTime.Now));
                    break;
            }
            
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OknoReakce r1 = new OknoReakce(bs.Filter);
            r1.ShowDialog();
            if (!r1.success)
                return;
            Reakce reakce1 = r1.reakce1;
            reakce.Add(reakce1);
            zmizet();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filter="";
            if (typeComboBox.Text != "")
            {
                filter = "Typ LIKE '" + typeComboBox.Text + "' AND ";
            }
            if (descriptionTextBox.Text != "")
            { 
                filter += "Popis LIKE '%" + descriptionTextBox.Text + "%' AND ";
            }
            if (sourceTextBox.Text != "")
            {
                filter += "Zdroj LIKE '" + sourceTextBox.Text + "' AND ";
            }
            if (filter.Length > 0)
                bs.Filter = filter.Substring(0, filter.Length - 5);
            else
                bs.Filter = filter;

            eventDataGridView.ResetBindings();
        }

        private void reactionsDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (reactionsDataGridView.Columns[e.ColumnIndex].Name == "Smazat")
            {
                reakce.RemoveAt(e.RowIndex);
                return;
            }
            Reakce edited = (Reakce) reakce[e.RowIndex];
            OknoReakce r1 = new OknoReakce(edited.Filtr, edited.Nazev, edited.Typ,edited.Zdroj, edited.Popis, edited.Email, edited.TextEmailu,edited.CestaKProgramu,edited.TypNove,edited.PopisNove);
            r1.ShowDialog();
            if (!r1.success)
                return;
            Reakce reakce1 = r1.reakce1;
            reakce.RemoveAt(e.RowIndex);
            reakce.Insert(e.RowIndex,reakce1);
            zmizet();
        }
        private void zmizet()
        {

            if (reactionsDataGridView.RowCount > 0)
            {
                bool hasDelete = false;
                foreach (DataGridViewColumn column in reactionsDataGridView.Columns)
                {
                    if (column.Name != "Nazev" && column.Name != "Smazat")
                        column.Visible = false;

                    if (column.Name == "Smazat")
                        hasDelete = true;
                }
                if (!hasDelete)
                {
                    DataGridViewButtonColumn col = new DataGridViewButtonColumn();
                    col.Name = "Smazat";
                    col.HeaderText = "Smazat";
                    col.Text = "Smazat";
                    col.UseColumnTextForButtonValue = true;
                    reactionsDataGridView.Columns.Add(col);
                }
            }
        }
    }
}
