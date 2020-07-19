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
        #region Constructor
        public ConnectData(ServerProxy pServerProxy)
        {
            InitializeComponent();

            m_ServerProxy = pServerProxy;
        }
        #endregion

        #region Private

        #region Winform event handlers

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

        #endregion

        #region Event handlers

        private void OnServerConnected(bool pStatus)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new dlgConnected(OnServerConnected), new object[] { pStatus });
            }
            else
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
        }

        #endregion

        #region Members

        private ServerProxy m_ServerProxy;

        #endregion

        #endregion
    }
}
