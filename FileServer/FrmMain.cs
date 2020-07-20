using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Communicator;
using DataLayer;
using CryptoManager;
using System.Diagnostics;

namespace FileServer
{
    public partial class FrmMain : Form
    {
        #region Constructor
        
        public FrmMain()
        {
            InitializeComponent();

            m_Clients = new Dictionary<string, ClientHandler>();
            m_UserManager = new UserManager();
        }

        #endregion

        #region Private

        #region Winform event handlers

        private void OnChooseFolderClick(object sender, EventArgs e)
        {
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
            DialogResult res = folderBrowserDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                txtFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void OnUserCreate(object sender, EventArgs e)
        {
            CreateUser createUser = new CreateUser(m_UserManager);
            DialogResult res = createUser.ShowDialog();
            if (res == DialogResult.OK)
            {
                PopulateUserGrid();
            }
        }

        private void OnUserRemove(object sender, EventArgs e)
        {
            string userName = (string)dgUsers.Rows[m_RowSelected].Cells["USERNAME"].Value;
            m_UserManager.RemoveUser(userName);

            PopulateUserGrid();
        }

        private void OnUserBlock(object sender, EventArgs e)
        {

        }

        private void OnStartServer(object sender, EventArgs e)
        {
            int port;
            if (int.TryParse(txtPort.Text, out port))
            {
                ServerConfig.Port = port;
                ServerConfig.HomeFolder = txtFolder.Text;

                m_CommServer = new CommServer(port);
                m_CommServer.ClientConnected += OnClientConnected;

                dgUsers.Enabled = true;

                SetListenerOpened();

                return;
            }

            MessageBox.Show("Invalid port. Should be int");
        }

        private void OnAdminLogin(object sender, EventArgs e)
        {
            bool valid = m_UserManager.ValidateUser(
                txtUserName.Text,
                CryptoHandler.HashEncode(txtPassword.Text)
            );

            if (valid)
            {
                // Set admin logged in
                SetAdminLoggedIn();
            }
            else
            {
                MessageBox.Show("User validation failed!");
            }
        }

        private void OnUserChangePassword(object sender, EventArgs e)
        {
            string userName = (string)dgUsers.Rows[m_RowSelected].Cells["USERNAME"].Value;

            ChangePassword changePassword = new ChangePassword(m_UserManager, userName);
            DialogResult res = changePassword.ShowDialog();
            if (res == DialogResult.OK)
            {
                MessageBox.Show("Changed");
            }
        }

        private void OnGridMouseDown(object sender, MouseEventArgs e)
        {
            if (dgUsers.Rows.Count == 0)
                return;

            if (e.Button == MouseButtons.Right)
            {
                DataGridView.HitTestInfo hit = dgUsers.HitTest(e.X, e.Y);
                dgUsers.ClearSelection();
                dgUsers.Rows[hit.RowIndex].Selected = true;

                m_RowSelected = hit.RowIndex;
            }
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        #endregion

        #region Event handlers

        private void OnClientConnected(CommConnection pClient)
        {
            Logger.Logger.Log("Client {0} connected", pClient.Id);
            ClientHandler handler = new ClientHandler(pClient, m_UserManager);
            handler.OnClientDisconnected += OnClientDisconnected;
            if (m_Clients.ContainsKey(pClient.Id))
            {
                m_Clients[pClient.Id] = handler;
            }
            else
            {
                m_Clients.Add(pClient.Id, handler);
            }
        }

        private void OnClientDisconnected(string pId)
        {
            m_Clients[pId].OnClientDisconnected -= OnClientDisconnected;
            m_Clients.Remove(pId);
        }

        #endregion

        #region Methods
        private void SetAdminLoggedIn()
        {
            txtPassword.Enabled = false;
            txtUserName.Enabled = false;
            btnLogin.Enabled = false;

            txtPort.Enabled = true;
            txtFolder.Enabled = true;
            btnChooseFolder.Enabled = true;
            btnOpenListener.Enabled = true;

            Logger.Logger.Log("Admin logged in");

            PopulateUserGrid();
        }

        private void PopulateUserGrid()
        {
            UserDal userDal = new UserDal();

            DataTable dt = userDal.GetAllUsers();

            dgUsers.DataSource = dt;

            dt.Dispose();
        }

        private void SetListenerOpened()
        {
            txtPort.Enabled = false;
            txtFolder.Enabled = false;
            btnOpenListener.Enabled = false;
        }



        #endregion

        #region Members

        private CommServer m_CommServer;
        private Dictionary<string, ClientHandler> m_Clients;
        private int m_RowSelected;
        private readonly UserManager m_UserManager;


        #endregion

        #endregion

    }
}
