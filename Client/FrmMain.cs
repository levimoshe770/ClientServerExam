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
            m_ServerProxy.FileListArrived += OnFileListArrived;
        }

        private void OnConnectClick(object sender, EventArgs e)
        {
            ConnectData connectData = new ConnectData(m_ServerProxy);
            DialogResult res = connectData.ShowDialog();
            if (res == DialogResult.OK)
            {
                m_UserData = connectData.UserData;
                SetStateConnected();
            }
        }

        private void OnDisconnectClick(object sender, EventArgs e)
        {
            m_ServerProxy.Disconnect();
            SetStateDisconnected();
        }

        private void OnDownloadFileClick(object sender, EventArgs e)
        {

        }

        private void OnUploadFileClick(object sender, EventArgs e)
        {

        }

        private void OnFileRemoveClick(object sender, EventArgs e)
        {

        }

        #endregion

        #region Event handlers
        private void OnFileListArrived(List<string> pFiles)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new dlgFileListArrived(OnFileListArrived), pFiles);
            }
            else
            {
                pFiles.ForEach((file) => { lstFiles.Items.Add(file); });
            }
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
            toolStripStatusUser.Text = string.Format("Hello {0}", m_UserData.UserName);

            // Set tree view
            PopulateFileList();
        }

        private void SetStateDisconnected()
        {
            // Set disabled and state disconnected status
            toolStripStatusConnect.Text = "Disconnected";
            toolStripStatusConnect.ForeColor = Color.Red;
            disconnectToolStripMenuItem.Enabled = false;
            connectToolStripMenuItem.Enabled = true;
            toolStripStatusUser.Text = "No user";

            EmptyFileList();
        }

        private void EmptyFileList()
        {
            lstFiles.Items.Clear();
        }

        private void PopulateFileList()
        {
            m_ServerProxy.GetFiles();
        }

        #endregion

        #region Members

        private ServerProxy m_ServerProxy;
        private UserData m_UserData;


        #endregion

        #endregion

        
    }
}
