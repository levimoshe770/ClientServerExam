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
    public partial class ConnectData : Form
    {
        public ConnectData(ServerProxy pServerProxy)
        {
            InitializeComponent();

            m_ServerProxy = pServerProxy;
        }

        private void OnConnectClick(object sender, EventArgs e)
        {
            int port;
            bool isNumber = int.TryParse(txtPort.Text, out port);

            if (!isNumber)
            {
                MessageBox.Show("Port should be a number above 2000");
                return;
            }

            try
            {
                m_ServerProxy.ServerConnected += OnServerConnected;
                m_ServerProxy.Connect(txtHost.Text, port);
            }
            catch
            {
                DialogResult = DialogResult.No;
                Close();
            }
        }

        private void OnCancelClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void OnServerConnected(bool pStatus)
        {
            if (pStatus)
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                DialogResult = DialogResult.No;
            }

            Close();
        }

        private ServerProxy m_ServerProxy;

    }
}
