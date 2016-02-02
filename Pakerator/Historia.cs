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
            string sql = "select GM_FSPOZ.ID, GM_FSPOZ.LP, GM_TOWARY.TYP, GM_TOWARY.SKROT, COALESCE(GM_TOWARY.SKROT2,'') as SKROT2, COALESCE(GM_TOWARY.KOD_KRESKOWY,'') as KOD_KRESKOWY, ";
            sql += "GM_TOWARY.NAZWA, GM_FSPOZ.ILOSC, 0 as SKANOWANE, COALESCE(GM_FSPOZ.ZNACZNIKI,'') as ZNACZNIKI, GM_FSPOZ.ID_TOWARU ";
            sql += "left join GM_FS on LOGSKAN.DOKUMENT_FS_ID=GM_FS.ID ";
            sql += "left join GM_TOWARY on LOGSKAN.TOWAR_ID=GM_TOWARY.ID_TOWARU";

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
                MessageBox.Show("Błąd przy wyoełnianiu listy hirtorią");
                throw;
            }
            fDataView.Table = fds.Tables["HIST"];
        }
    }
}
