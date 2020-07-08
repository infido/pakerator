using FirebirdSql.Data.FirebirdClient;
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
    public partial class DokumentyDlaMagazynu : Form
    {
        private ConnectionDB polaczenie;
        private FbDataAdapter fda;
        private DataSet fds;
        private DataView fDataView;
        private int mag1, mag2;
        private string mag1Kod, mag2Kod; 
        private string kk;
        private bool tylkoOstatnie7Dni;
        Dictionary<string, string> statusyZamowien;
        public DokumentyDlaMagazynu(ConnectionDB polacz, int magazyn1, string magazyn1skrot, int magazyn2, string magazyn2skrot, bool czyTylkoOstatnie7Dni)
        {
            InitializeComponent();
            polaczenie = polacz;
            mag1 = magazyn1;
            mag1Kod = magazyn1skrot;
            mag2 = magazyn2;
            mag2Kod = magazyn2skrot;
            tylkoOstatnie7Dni = czyTylkoOstatnie7Dni;
            statusyZamowien = new Dictionary<string, string>();
        }

        public string getKodDokumentuFromUser()
        {
            //wczytanie danych do okna
            string sql = "SELECT GM_FS.NUMER, GM_FS.NAZWA_SKROCONA_PLATNIKA, GM_FS.SYGNATURA, GM_FS.DATA_WYSTAWIENIA, GM_FSPOZ.ILOSC, GM_TOWARY.SKROT ";
            sql += " , -1 STAN_" + mag1Kod;
            if (mag2Kod != "" && mag1Kod!=mag2Kod)
                sql += " , -1 STAN_" + mag2Kod;
            sql += " , '-' STATUS, '' STATUS_ZAM ";
            sql += " from GM_FSPOZ ";
            sql += " join GM_FS on GM_FSPOZ.ID_GLOWKI=GM_FS.ID ";
            sql += " join GM_TOWARY ON GM_FSPOZ.ID_TOWARU=GM_TOWARY.ID ";
            sql += " left join GM_WZ on GM_WZ.ID_FS = GM_FS.ID ";
            sql += " left join GM_WZPOZ on GM_WZPOZ.ID_GLOWKI=GM_WZ.ID ";
            sql += " where ";
            sql += "  GM_FS.MAGAZYNOWY=0 AND GM_FS.FISKALNY=0 ";
            sql += " AND GM_WZPOZ.ILOSC_PO is null ";
            sql += " AND TYP='Towar' ";
            if (tylkoOstatnie7Dni)
            {
                sql += " AND GM_FS.DATA_WYSTAWIENIA>'" + DateTime.Now.AddDays(-8).ToShortDateString() + "' ";
            }
            sql += " AND GM_FS.MAGNUM=" + mag1 + ";";
            

            fda = new FbDataAdapter(sql, polaczenie.getConnection().ConnectionString);
            fds = new DataSet();
            fDataView = new DataView();
            fds.Tables.Add("SP");
            try
            {
                fda.Fill(fds.Tables["SP"]);
            }
            catch (Exception ex)
            {
                Pulpit.putLog(polaczenie, polaczenie.getCurrentUser(), "ERROR", "711 Błąd wykonania raportu listy dok sprzedaży do wyboru przez magazyn: " + ex.Message, "", "", "", 0, "", 0, "", mag1, "", 0, 0);
                throw;
            }
            fDataView.Table = fds.Tables["SP"];
            dataGridViewDokSP.DataSource = fDataView;

            setKolorowanieRekordow();
            setStatusZamOnGrid();

            setRowFilter();

            ShowDialog();

            return kk;
        }

        private void bAnuluj_Click(object sender, EventArgs e)
        {
            Visible = false;
            kk = "";
        }

        private void bWybierz_Click(object sender, EventArgs e)
        {
            Visible = false;
            kk = dataGridViewDokSP.CurrentRow.Cells["SYGNATURA"].Value.ToString();
        }

        private void setRowFilter()
        {
            if (cOK.Checked)
            {
                fDataView.RowFilter = "STATUS_ZAM LIKE '%OK%'";
            }

            if (cForMove.Checked && fDataView.RowFilter.Length > 0)
            {
                fDataView.RowFilter += " OR STATUS_ZAM LIKE '%DO%' ";
            }
            else if (cForMove.Checked)
            {
                fDataView.RowFilter += " STATUS_ZAM LIKE '%DO%' ";
            }

            if (cBRAK.Checked && fDataView.RowFilter.Length > 0)
            {
                fDataView.RowFilter += " OR STATUS_ZAM LIKE '%BRAK%' ";
            }
            else if (cBRAK.Checked)
            {
                fDataView.RowFilter = " STATUS_ZAM LIKE '%BRAK%' ";
            }
        }

        private void setKolorowanieRekordow()
        {
            string sym = "";
            string stat = "";
            DataGridViewRow rndx = null;
            foreach (DataGridViewRow row in dataGridViewDokSP.Rows)
            {
                if (row.Index == 0)
                {
                    //sym = row.Cells["NUMER"].Value.ToString();
                    rndx = row;
                }
                
                string sql = "SELECT sum(ILOSC) from GM_MAGAZYN join GM_TOWARY on ID_TOWAR=GM_TOWARY.ID_TOWARU ";
                sql += " where GM_TOWARY.SKROT='" + row.Cells["SKROT"].Value.ToString() + "' and GM_MAGAZYN.MAGNUM=" + mag1 + ";";

                FbCommand cdk = new FbCommand(sql, polaczenie.getConnection());
                try
                {
                    if (cdk.ExecuteScalar() != DBNull.Value)
                        row.Cells["STAN_" + mag1Kod].Value = Convert.ToDouble(cdk.ExecuteScalar());
                    else
                        row.Cells["STAN_" + mag1Kod].Value = 0;
                }
                catch (FbException ex)
                {
                    Pulpit.putLog(polaczenie, polaczenie.getCurrentUser(), "ERROR", "022 Bład zapytania o stany przy kolorowaniu rekordów w liście wyboru dok przez magazyn, Towar:" + row.Cells["SKROT"].Value.ToString() + System.Environment.NewLine + ex.Message , "","", row.Cells["NUMER"].Value.ToString(),0,"",0,mag1Kod,mag1,"",0,0);
                    row.Cells["STAN_" + mag1Kod].Value = -2;
                    row.Cells["STATUS"].Value = "BŁĄD";
                 }

                if ( ( Convert.ToDouble(row.Cells["STAN_" + mag1Kod].Value) - Convert.ToDouble(row.Cells["ILOSC"].Value))>=0)
                {
                    row.Cells["STATUS"].Value = "OK";
                }

                if (mag1!=mag2)
                {
                    sql = "SELECT sum(ILOSC) from GM_MAGAZYN join GM_TOWARY on ID_TOWAR=GM_TOWARY.ID_TOWARU ";
                    sql += " where GM_TOWARY.SKROT='" + row.Cells["SKROT"].Value.ToString() + "' and GM_MAGAZYN.MAGNUM=" + mag2 + ";";

                    cdk = new FbCommand(sql, polaczenie.getConnection());
                    try
                    {
                        if (cdk.ExecuteScalar() != DBNull.Value)
                            row.Cells["STAN_" + mag2Kod].Value = Convert.ToDouble(cdk.ExecuteScalar());
                        else
                            row.Cells["STAN_" + mag2Kod].Value = 0;
                    }
                    catch (FbException ex2)
                    {
                        Pulpit.putLog(polaczenie, polaczenie.getCurrentUser(), "ERROR", "022 Bład zapytania o stany przy kolorowaniu rekordów w liście wyboru dok przez magazyn, Towar:" + row.Cells["SKROT"].Value.ToString() + System.Environment.NewLine + ex2.Message, "", "", row.Cells["NUMER"].Value.ToString(), 0, "", 0, mag1Kod, mag1, "", 0, 0);
                        row.Cells["STAN_" + mag2Kod].Value = -2;
                        row.Cells["STATUS"].Value = "BŁĄD";
                    }

                }

                if ( mag1 != mag2 && !row.Cells["STATUS"].Value.ToString().Equals("OK") &&
                    (  (Convert.ToDouble(row.Cells["STAN_" + mag1Kod].Value) + Convert.ToDouble(row.Cells["STAN_" + mag2Kod].Value)) - Convert.ToDouble(row.Cells["ILOSC"].Value)) >= 0
                    )
                {
                    row.Cells["STATUS"].Value = "DO PRZESUNIĘCIA";
                }
                else if (mag1 != mag2 && !row.Cells["STATUS"].Value.ToString().Equals("OK") &&
                    ((Convert.ToDouble(row.Cells["STAN_" + mag1Kod].Value) + Convert.ToDouble(row.Cells["STAN_" + mag2Kod].Value)) - Convert.ToDouble(row.Cells["ILOSC"].Value)) < 0
                    )
                {
                    row.Cells["STATUS"].Value = "BRAK";
                }

                if (row.Cells["NUMER"].Value.ToString().Equals(sym))
                {
                    //to wiersz tego samego zamówienia
                    if (row.Cells["STATUS"].Value.ToString().Equals("DO PRZESUNIĘCIA") && stat.Equals("OK"))
                    {
                        stat = "DO PRZESUNIĘCIA";
                    }
                    else if (row.Cells["STATUS"].Value.ToString().Equals("BRAK") && stat.Equals("DO PRZESUNIĘCIA"))
                    {
                        stat = "BRAK";
                    }
                    else if (row.Cells["STATUS"].Value.ToString().Equals("BRAK") && stat.Equals("OK"))
                    {
                        stat = "BRAK";
                    }else if (stat == "")
                    {
                        stat = row.Cells["STATUS"].Value.ToString();
                    }
                }
                else
                {
                    sym = row.Cells["NUMER"].Value.ToString();
                    rndx = row;
                }
                setStatusToDict(row.Cells["NUMER"].Value.ToString(), row.Cells["STATUS"].Value.ToString());
            }
        }

        private void cOK_CheckedChanged(object sender, EventArgs e)
        {
            setRowFilter();
            dataGridViewDokSP.Refresh();
        }

        private void cForMove_CheckedChanged(object sender, EventArgs e)
        {
            setRowFilter();
            dataGridViewDokSP.Refresh();
        }

        private void cBRAK_CheckedChanged(object sender, EventArgs e)
        {
            setRowFilter();
            dataGridViewDokSP.Refresh();
        }

        private void setStatusToDict(string numer, string status)
        {
            if (!statusyZamowien.ContainsKey(numer))
            {
                statusyZamowien.Add(numer, status);
            }
            else if (statusyZamowien[numer].Equals("OK") && status.Equals("DO PRZESUNIĘCIA"))
            {
                statusyZamowien[numer] = "DO PRZESUNIĘCIA";
            }else if (statusyZamowien[numer].Equals("DO PRZESUNIĘCIA") && status.Equals("BRAK"))
            {
                statusyZamowien[numer] = "BRAK";
            }
            else if (statusyZamowien[numer].Equals("OK") && status.Equals("BRAK"))
            {
                statusyZamowien[numer] = "BRAK";
            }
        }

        private void setStatusZamOnGrid()
        {
            foreach (DataGridViewRow row in dataGridViewDokSP.Rows)
            {
                if (row.Cells["NUMER"].Value.ToString().Length > 0)
                    row.Cells["STATUS_ZAM"].Value = statusyZamowien[row.Cells["NUMER"].Value.ToString()];
            }
        }
    }

    public class DataGridViewGroup : DataGridView
    {
        protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs args)
        {
            // Call home to base
            base.OnCellFormatting(args);

            args.CellStyle.BackColor = IsStatuseForColoring(args.RowIndex, args.ColumnIndex);
            // First row always displays
            if (args.RowIndex == 0)
                return;

            if (IsRepeatedCellValue(args.RowIndex, args.ColumnIndex))
            {
                args.Value = string.Empty;
                args.FormattingApplied = true;
            }
        }

        private Color IsStatuseForColoring(int rowIndex, int colIndex)
        {
            if (colIndex > 3)
            {
                DataGridViewCell stateCell = Rows[rowIndex].Cells["STATUS"];
                switch (stateCell.Value.ToString())
                {
                    case "OK":
                        return Color.YellowGreen;
                    case "DO PRZESUNIĘCIA":
                        return Color.Yellow;
                    case "BRAK":
                        return Color.Orange;
                    default:
                        return Color.White;
                }
            }else if (colIndex <= 3)
            {
                DataGridViewCell stateZamCell = Rows[rowIndex].Cells["STATUS_ZAM"];
                switch (stateZamCell.Value.ToString())
                {
                    case "OK":
                        return Color.YellowGreen;
                    case "DO PRZESUNIĘCIA":
                        return Color.Yellow;
                    case "BRAK":
                        return Color.Orange;
                    default:
                        return Color.White;
                }
            }
            else
                return Color.White;
        }

        private bool IsRepeatedCellValue(int rowIndex, int colIndex)
        {
            DataGridViewCell currCell = Rows[rowIndex].Cells[colIndex];
            DataGridViewCell prevCell = Rows[rowIndex - 1].Cells[colIndex];

            if (
                ( colIndex<4
                &&
                  (
                    ( currCell.Value == prevCell.Value ) 
                    ||
                    ( currCell.Value != null && prevCell.Value != null && currCell.Value.ToString() == prevCell.Value.ToString() )
                  )
                )
               )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs args)
        {
            base.OnCellPainting(args);

            args.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;

            // Ignore column and row headers and first row
            if (args.RowIndex < 1 || args.ColumnIndex < 0)
                return;

            if (IsRepeatedCellValue(args.RowIndex, args.ColumnIndex))
            {
                args.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
            }
            else
            {
                args.AdvancedBorderStyle.Top = AdvancedCellBorderStyle.Top;
            }
        }
    }
}
