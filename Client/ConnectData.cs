using ControlMessages;
using CryptoManager;
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

        public UserData UserData { get { return m_UserData; } }

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
                m_UserData = new UserData()
                {
                    UserName = txtUserName.Text,
                    Password = txtPassword.Text
                };

                m_ServerProxy.ServerConnected += OnServerConnected;
                m_ServerProxy.Connect(
                    txtHost.Text, 
                    port,
                    txtUserName.Text,
                    CryptoHandler.HashEncode(txtPassword.Text)
                );
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
                    MessageBox.Show("Connection failed");
                }

                Close();
            }
        }

        #endregion

        #region Members

        private ServerProxy m_ServerProxy;
        private UserData m_UserData;

        #endregion

        #endregion
    }
}
