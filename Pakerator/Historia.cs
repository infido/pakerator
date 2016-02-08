using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;

namespace Pakerator
{
    public partial class Historia : Form
    {
        private ConnectionDB polaczenie;
        private FbDataAdapter fda;
        private DataSet fds;
        private DataView fDataView;

        public Historia(ConnectionDB polaczenie)
        {
            InitializeComponent();
            this.polaczenie = polaczenie;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bSzukaj_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Refresh();
            
            string sql = "SELECT LOGSKAN.PRACOWNIK, LOGSKAN.KODKRESKOWY, LOGSKAN.LIST_PRZEWOZOWY, gm_FS.NUMER, GM_TOWARY.SKROT, GM_TOWARY.NAZWA, ";
            sql += "LOGSKAN.KOMUNIKAT, LOGSKAN.OPERACJA, LOGSKAN.MAGAZYN_NAZWA, LOGSKAN.KONTRAHENT, LOGSKAN.IP, LOGSKAN.HOST, LOGSKAN.UTWORZONO";
            sql += " from LOGSKAN ";
            sql += "left join GM_FS on LOGSKAN.DOKUMENT_FS_ID=GM_FS.ID ";
            sql += "left join GM_TOWARY on LOGSKAN.TOWAR_ID=GM_TOWARY.ID_TOWARU ";
            if (tDokument.Text.Length != 0 || tUser.Text.Length != 0 || tList.Text.Length != 0 || tKontrahent.Text.Length != 0)
            {
                sql += " where ";

                if (tDokument.Text.Length != 0)
                {
                    sql += " GM_FS.NUMER like '%" + tDokument.Text +"%' " ;
                    //Dodanie obsługi pozostałych dokumentów
                    //MM i Zlecenia wewnętrznego
                }

                //string test = sql.Substring(sql.Length - 7);
                if (tUser.Text.Length != 0 && sql.Substring(sql.Length - 7).Equals(" where "))
                {
                    sql += " LOGSKAN.PRACOWNIK like '%" + tUser.Text + "%' ";
                }
                else if (tUser.Text.Length != 0)
                {
                    sql += " AND LOGSKAN.PRACOWNIK like '%" + tUser.Text + "%' "; 
                }

                if (tList.Text.Length != 0 && sql.Substring(sql.Length - 7).Equals(" where "))
                {
                    sql += " LOGSKAN.LIST_PRZEWOZOWY like '%" + tList.Text + "%' ";
                }
                else if (tList.Text.Length != 0)
                {
                    sql += " AND LOGSKAN.LIST_PRZEWOZOWY like '%" + tList.Text + "%' ";
                }

                if (tKontrahent.Text.Length != 0 && sql.Substring(sql.Length - 7).Equals(" where "))
                {
                    sql += " LOGSKAN.KONTRAHENT like '%" + tKontrahent.Text + "%' ";
                }
                else if (tKontrahent.Text.Length != 0)
                {
                    sql += " AND LOGSKAN.KONTRAHENT like '%" + tKontrahent.Text + "%' ";
                }
            }

            fda = new FbDataAdapter(sql, polaczenie.getConnection().ConnectionString);
            fds = new DataSet();
            fDataView = new DataView();
            fds.Tables.Add("HIST");
            try
            {
                fda.Fill(fds.Tables["HIST"]);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd przy wyoełnianiu listy hirtorią: " + ex.Message);
                throw;
            }
            fDataView.Table = fds.Tables["HIST"];
            dataGridView1.DataSource = fDataView;
        }
    }
}
