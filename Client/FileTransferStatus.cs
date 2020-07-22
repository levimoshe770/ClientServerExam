using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class FileTransferStatus : Form
    {
        public FileTransferStatus()
        {
            InitializeComponent();
        }

        public double PercentageDone {
            get
            {
                return m_PercentageDone;
            }
            set
            {
                m_PercentageDone = value;
                pgComplete.Value = (int)(m_PercentageDone * 100.0);
                lblPctg.Text = string.Format("{0:.#}%", m_PercentageDone * 100.0);
            } 
        }

        private void OnLoad(object sender, EventArgs e)
        {
            pgComplete.Value = 0;
            lblPctg.Text = "0%";
            BringToFront();
        }

        private double m_PercentageDone;
    }
}
