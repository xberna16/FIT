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
    public partial class OknoReakce : Form
    {
       public bool editing, success = false;
        string filtr;
        public Reakce reakce1;
        public OknoReakce(string filter)
        {
            InitializeComponent();
            filtr = filter;
        }
        public OknoReakce(string filter, string titul, string typ, string zdroj, string popis, string email, string text, string path, string type, string popisNew)
        {
            InitializeComponent();
            nameTextBox.Text = titul;
            typeComboBox.Text = typ;
            sourceTextBox.Text = zdroj;
            descriptionTextBox.Text = popis;
            emailTextBox.Text = email;
            emailTextRichTextBox.Text = text;
            filePathTextBox.Text = path;
            NewTypeComboBox.Text = type;
            newDescriptionTextBox.Text = popisNew;
            filtr = filter;
            editing = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                filePathTextBox.Text = openFileDialog1.FileName;
        }

        private void OknoReakce_FormClosing(object sender, FormClosingEventArgs e)
        {
            reakce1 = new Reakce(nameTextBox.Text, typeComboBox.Text, descriptionTextBox.Text, sourceTextBox.Text, "");
            reakce1.Filtr = filtr;
            reakce1.Email = emailTextBox.Text;
            reakce1.TextEmailu = emailTextRichTextBox.Text;
            reakce1.CestaKProgramu = filePathTextBox.Text;
            reakce1.TypNove = NewTypeComboBox.Text;
            reakce1.PopisNove = newDescriptionTextBox.Text;

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            success = true;
            this.Close();
        }
    }
}
