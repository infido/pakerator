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
        private int lastCount;

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
            lPodsumowanie.Text = "Wykonuję zapytanie do bazy danych";
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Refresh();
            
            string sql = "SELECT LOGSKAN.UTWORZONO, LOGSKAN.PRACOWNIK, LOGSKAN.OPERACJA, LOGSKAN.KODKRESKOWY, LOGSKAN.LIST_PRZEWOZOWY, gm_FS.NUMER as FAKTURA, gm_MM.NUMER as MM, GM_TOWARY.SKROT, GM_TOWARY.NAZWA, ";
            sql += "LOGSKAN.KOMUNIKAT, LOGSKAN.MAGAZYN_NAZWA, LOGSKAN.KONTRAHENT, LOGSKAN.IP, LOGSKAN.HOST ";
            sql += " from LOGSKAN ";
            sql += "left join GM_FS on LOGSKAN.DOKUMENT_FS_ID=GM_FS.ID ";
            sql += "left join GM_MM on LOGSKAN.DOKUMENT_MM_ID=GM_MM.ID ";
            sql += "left join GM_TOWARY on LOGSKAN.TOWAR_ID=GM_TOWARY.ID_TOWARU ";
            if (tDokument.Text.Length != 0 || tUser.Text.Length != 0 || tList.Text.Length != 0 || tKontrahent.Text.Length != 0
                 || tKK.Text.Length != 0 || tOperacja.Text.Length != 0 || !rbDataWszystko.Checked)
            {
                sql += " where ";

                if (tDokument.Text.Length != 0)
                {
                    sql += " (GM_FS.NUMER like '%" + tDokument.Text + "%' OR GM_MM.NUMER like '%" + tDokument.Text + "%' OR LOGSKAN.LIST_PRZEWOZOWY like '%" + tDokument.Text + "%') ";
                    //Dodanie obsługi pozostałych dokumentów
                    //Zlecenia wewnętrznego
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

                if (tKK.Text.Length != 0 && sql.Substring(sql.Length - 7).Equals(" where "))
                {
                    sql += " LOGSKAN.KODKRESKOWY like '%" + tKK.Text + "%' ";
                }
                else if (tKK.Text.Length != 0)
                {
                    sql += " AND LOGSKAN.KODKRESKOWY like '%" + tKK.Text + "%' ";
                }

                if (tOperacja.Text.Length != 0 && sql.Substring(sql.Length - 7).Equals(" where "))
                {
                    sql += " LOGSKAN.OPERACJA like '%" + tOperacja.Text + "%' ";
                }
                else if (tOperacja.Text.Length != 0)
                {
                    sql += " AND LOGSKAN.OPERACJA like '%" + tOperacja.Text + "%' ";
                }

                #region filtrowanie zakresu danych po datatch
                if (rbDataDzisiaj.Checked)
                {
                    if (sql.Substring(sql.Length - 7).Equals(" where "))
                    {
                        sql += " LOGSKAN.UTWORZONO >= '" + DateTime.Now.ToShortDateString() + "' ";
                    }
                    else
                    {
                        sql += " AND LOGSKAN.UTWORZONO >='" + DateTime.Now.ToShortDateString() + "' ";
                    } 
                }
                else if (rbDataWczoraj.Checked)
                {
                    if (sql.Substring(sql.Length - 7).Equals(" where "))
                    {
                        sql += " LOGSKAN.UTWORZONO BETWEEN '" + DateTime.Now.AddDays(-1).ToShortDateString() + "' AND '" + DateTime.Now.ToShortDateString() + "' ";
                    }
                    else
                    {
                        sql += " AND LOGSKAN.UTWORZONO '" + DateTime.Now.AddDays(-1).ToShortDateString() + "' AND '" + DateTime.Now.ToShortDateString() + "' ";
                    }
                }else if (rbData7Dni.Checked)
                {
                    if (sql.Substring(sql.Length - 7).Equals(" where "))
                    {
                        sql += " LOGSKAN.UTWORZONO >= '" + DateTime.Now.AddDays(-7).ToShortDateString() + "' ";
                    }
                    else
                    {
                        sql += " AND LOGSKAN.UTWORZONO >='" + DateTime.Now.AddDays(-7).ToShortDateString() + "' ";
                    }
                }
                else if (rbData14Dni.Checked)
                {
                    if (sql.Substring(sql.Length - 7).Equals(" where "))
                    {
                        sql += " LOGSKAN.UTWORZONO >= '" + DateTime.Now.AddDays(-14).ToShortDateString() + "' ";
                    }
                    else
                    {
                        sql += " AND LOGSKAN.UTWORZONO >='" + DateTime.Now.AddDays(-14).ToShortDateString() + "' ";
                    }
                }
                else if (rbData31Dni.Checked)
                {
                    if (sql.Substring(sql.Length - 7).Equals(" where "))
                    {
                        sql += " LOGSKAN.UTWORZONO >= '" + DateTime.Now.AddDays(-31).ToShortDateString() + "' ";
                    }
                    else
                    {
                        sql += " AND LOGSKAN.UTWORZONO >='" + DateTime.Now.AddDays(-31).ToShortDateString() + "' ";
                    }
                }
                else if (rbData90Dni.Checked)
                {
                    if (sql.Substring(sql.Length - 7).Equals(" where "))
                    {
                        sql += " LOGSKAN.UTWORZONO >= '" + DateTime.Now.AddDays(-90).ToShortDateString() + "' ";
                    }
                    else
                    {
                        sql += " AND LOGSKAN.UTWORZONO >='" + DateTime.Now.AddDays(-90).ToShortDateString() + "' ";
                    }
                }
                #endregion

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
            lPodsumowanie.Text = "Wykonano zapytanie i wczytano " + dataGridView1.Rows.Count +" rekordów, poprzednio " + lastCount;
            lastCount = dataGridView1.Rows.Count;
        }

        private void bPrzewinDoKonca_Click(object sender, EventArgs e)
        {
            dataGridView1.CurrentCell = dataGridView1[0, dataGridView1.Rows.Count - 1];
        }
    }
}
