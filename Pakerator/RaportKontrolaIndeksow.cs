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
    public partial class RaportKontrolaIndeksow : Form
    {
        private ConnectionDB polaczenie;
        private FbDataAdapter fda;
        private DataSet fds;
        private DataView fDataView;
        private int mag1, mag2;
        public RaportKontrolaIndeksow(ConnectionDB polacz, int magazyn1, int magazyn2)
        {
            InitializeComponent();
            polaczenie = polacz;
            mag1 = magazyn1;
            mag2 = magazyn2;
            Show();
        }

        private void bClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void getData()
        {
            string sql = "SELECT GM_TOWARY.SKROT, GM_FS.NUMER, GM_FS.DATA_WYSTAWIENIA, GM_FS.NAZWA_SKROCONA_PLATNIKA, GM_FS.SYGNATURA, GM_FSPOZ.ILOSC ";
            sql += " from GM_FSPOZ ";
            sql += " join GM_FS on GM_FSPOZ.ID_GLOWKI=GM_FS.ID ";
            sql += " join GM_TOWARY ON GM_FSPOZ.ID_TOWARU=GM_TOWARY.ID ";
            sql += " where ";
            sql += "  GM_FS.MAGAZYNOWY=0 AND GM_FS.FISKALNY=0 ";
            sql += " AND (GM_TOWARY.SKROT like '" + tKodDoZnalezienia.Text + "' OR GM_TOWARY.SKROT2 like'" + tKodDoZnalezienia.Text + "' OR GM_TOWARY.KOD_KRESKOWY like '" + tKodDoZnalezienia.Text + "' ) ";
            if (mag1==mag2 || mag2==0)
            {
                sql += " AND GM_FS.MAGNUM=" + mag1 + ";";
            }
            else
            {
                sql += " AND (GM_FS.MAGNUM=" + mag1 + " OR GM_FS.MAGNUM=" + mag2 +"); ";
            }


            fda = new FbDataAdapter(sql, polaczenie.getConnection().ConnectionString);
            fds = new DataSet();
            fDataView = new DataView();
            fds.Tables.Add("POZ");
            try
            {
                fda.Fill(fds.Tables["POZ"]);
            }
            catch (Exception ex)
            {
                Pulpit.putLog(polaczenie, polaczenie.getCurrentUser(), "ERROR", "710 Błąd wykonania raportu 701 kontroli towarów na dok sprzedaży, wyświetlono rekordów: " + ex.Message, tKodDoZnalezienia.Text, "", "", 0, "", 0, "", mag1, "", 0, 0);
                throw;
            }
            fDataView.Table = fds.Tables["POZ"];
            dataGridView1.DataSource = fDataView;
        }

        private void tKodDoZnalezienia_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Return))
                bFind.PerformClick();
        }

        private void bFind_Click(object sender, EventArgs e)
        {
            getData();
            Pulpit.putLog(polaczenie, polaczenie.getCurrentUser(), "REPORT", "701 Wykonanie raportu kontroli towarów na dok sprzedaży, wyświetlono rekordów " + dataGridView1.Rows.Count , tKodDoZnalezienia.Text, "", "", 0, "", 0, "", mag1, "", 0, 0);
        }
    }
}
