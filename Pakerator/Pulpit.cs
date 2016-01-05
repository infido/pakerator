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
    public partial class Pulpit : Form
    {
        public ConnectionDB polaczenie;
        private FbDataAdapter fda;
        private Login logowanie;
        
        public Pulpit()
        {
            InitializeComponent();
            Text = "Pakerator " + Application.ProductVersion;
            polaczenie = new ConnectionDB();
            logowanie = new Login(polaczenie);
            logowanie.ShowDialog();
        }

        private void Pulpit_Load(object sender, EventArgs e)
        {
            
        }

        private void Pulpit_FormClosed(object sender, FormClosedEventArgs e)
        {
            polaczenie.Close();
        }
    }
}
