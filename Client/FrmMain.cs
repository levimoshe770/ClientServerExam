using ControlMessages;
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
    public partial class frmMain : Form
    {
        #region Constructor
        public frmMain()
        {
            InitializeComponent();
        }
        #endregion

        #region Private

        #region WinForm event handlers
        
        private void OnLoad(object sender, EventArgs e)
        {
            m_ServerProxy = new ServerProxy();
        }

        private void OnConnectClick(object sender, EventArgs e)
        {
            ConnectData connectData = new ConnectData(m_ServerProxy);
            DialogResult res = connectData.ShowDialog();
            if (res == DialogResult.OK)
            {
                SetStateConnected();
            }
        }

        private void OnDisconnectClick(object sender, EventArgs e)
        {
            m_ServerProxy.Disconnect();
            SetStateDisconnected();
        }

        private void OnCreateNewUser(object sender, EventArgs e)
        {
            CreateUser createUser = new CreateUser(m_ServerProxy);
            DialogResult res = createUser.ShowDialog();
            if (res == DialogResult.OK)
            {
                // Set status to user created
                SetUserLoggedIn(createUser.UserData);
            }
        }

        private void OnDeleteUser(object sender, EventArgs e)
        {

        }

        private void OnNewFolderClick(object sender, EventArgs e)
        {

        }

        private void OnRemoveFolderClick(object sender, EventArgs e)
        {

        }

        private void OnDownloadFileClick(object sender, EventArgs e)
        {

        }

        private void OnUploadFileClick(object sender, EventArgs e)
        {

        }

        #endregion

        #region Methods

        private void SetStateConnected()
        {
            // Set enabled and state status in a clear place
            throw new NotImplementedException();
        }

        private void SetStateDisconnected()
        {
            // Set disabled and state disconnected status
            throw new NotImplementedException();
        }

        private void SetUserLoggedIn(UserData userData)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Members

        private ServerProxy m_ServerProxy;


        #endregion

        #endregion

        private void OnUserLoginClick(object sender, EventArgs e)
        {

        }
    }
}
