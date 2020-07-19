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
            m_ServerProxy.DeleteUser(m_UserData.UserName);
            SetUserLoggedOut();
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

        private void OnUserLoginClick(object sender, EventArgs e)
        {
            UserLogin userLogin = new UserLogin(m_ServerProxy);
            DialogResult res = userLogin.ShowDialog();
            if (res == DialogResult.OK)
            {
                m_UserData = userLogin.UserData;
                SetUserLoggedIn(userLogin.UserData);
            }
        }

        private void OnUserLogoutClick(object sender, EventArgs e)
        {
            m_ServerProxy.LogoutUser(m_UserData);
            SetUserLoggedOut();
        }

        #endregion

        #region Methods

        private void SetStateConnected()
        {
            // Set enabled and state status in a clear place
            toolStripStatusConnect.Text = "Connected";
            toolStripStatusConnect.ForeColor = Color.Green;
            disconnectToolStripMenuItem.Enabled = true;
            connectToolStripMenuItem.Enabled = false;
            loginToolStripMenuItem.Enabled = true;
            createNewUserToolStripMenuItem.Enabled = true;
        }

        private void SetStateDisconnected()
        {
            // Set disabled and state disconnected status
            toolStripStatusConnect.Text = "Disconnected";
            toolStripStatusConnect.ForeColor = Color.Red;
            disconnectToolStripMenuItem.Enabled = false;
            connectToolStripMenuItem.Enabled = true;
            loginToolStripMenuItem.Enabled = false;
            createNewUserToolStripMenuItem.Enabled = false;
        }

        private void SetUserLoggedIn(UserData userData)
        {
            toolStripStatusUser.Text = string.Format("Hello {0}", userData.UserName);
            txtCurrentFolder.Text = userData.HomePath;
            loginToolStripMenuItem.Enabled = false;
            logoutToolStripMenuItem.Enabled = true;
            deleteUserToolStripMenuItem.Enabled = true;

            // Set tree view
            PopulateTree();

            // Set folder view
        }

        private void SetUserLoggedOut()
        {
            toolStripStatusUser.Text = string.Format("No user logged in");
            txtCurrentFolder.Text = "";
            loginToolStripMenuItem.Enabled = true;
            logoutToolStripMenuItem.Enabled = false;
            deleteUserToolStripMenuItem.Enabled = false;

            // Set tree view
            // Set folder view
        }

        private void PopulateTree()
        {
            m_ServerProxy.FolderListArrived += OnFolderListArrived;
            m_ServerProxy.GetFolders();
        }

        private void OnFolderListArrived(List<string> pFolders)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Members

        private ServerProxy m_ServerProxy;
        private UserData m_UserData;

        #endregion

        #endregion

        
    }
}
