using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pakerator
{
    public partial class Raport : Form
    {
        int kolumnaFiltr = 0;
        string filtr = "";
        public Raport()
        {
            InitializeComponent();
        }

        public Raport(DataSet dane)
        {
            InitializeComponent();
            dataGridViewRaport.AutoGenerateColumns = true;
            dataGridViewRaport.DataSource = dane.Tables["TAB"];

            lWynikRaportu.Text = "Raport wyświetlił " + dane.Tables["TAB"].Rows.Count + " wierszy";
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridViewRaport_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewRaport.CurrentCell.ColumnIndex!=kolumnaFiltr)
            {
                filtr = "";
                kolumnaFiltr = dataGridViewRaport.CurrentCell.ColumnIndex;
            }
        }

        private void dataGridViewRaport_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                Text = "Raport";
                filtr = "";
                foreach (DataGridViewRow row in dataGridViewRaport.Rows)
                {
                    row.Visible = true;
                }
            }
            else
            {
                filtr += e.KeyChar;
                Text = "Raport filtr=" + filtr;
            }

            if (filtr.Length>0)
            {
                List<DataGridViewRow> li = new List<DataGridViewRow>();
                foreach (DataGridViewRow row in dataGridViewRaport.Rows)
                {
                    if (row.Visible)
                    {
                        if (row.Cells[kolumnaFiltr].Value.ToString().Contains(filtr.ToString()))
                        {
                            row.Visible = true;
                            //dataGridViewRaport.Rows[row.Index].Cells[kolumnaFiltr].Selected=true;
                            dataGridViewRaport.CurrentCell = row.Cells[kolumnaFiltr];
                        }
                        else
                            li.Add(row);
                    }
                }

                if (li.Count > 0)
                {
                    foreach (DataGridViewRow item in li)
                    {
                        try
                        {
                            item.Visible = false;
                        }
                        catch (Exception)
                        {
                            //MessageBox.Show("Brak rekordów spełniających kryteria", "Brak danych");
                            Text = "Raport filtr=" + filtr;
                            dataGridViewRaport.CurrentCell = null;
                            item.Visible = false;
                        }
                    }
                }
            }
        }
    }
}
