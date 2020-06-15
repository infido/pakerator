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
        public Raport()
        {
            InitializeComponent();
        }

        public Raport(DataSet dane)
        {
            InitializeComponent();
            dataGridViewRaport.AutoGenerateColumns = true;
            dataGridViewRaport.DataSource = dane.Tables["TAB"];
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
