using ControlMessages;
using MessageHandler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

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
            m_ServerProxy.FileTransferStatus += OnFileTransferStatusUpdate;
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
            folderBrowserDialog1.ShowNewFolderButton = true;
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
            DialogResult res = folderBrowserDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                m_ServerProxy.DownloadFile(m_SelectedFile, folderBrowserDialog1.SelectedPath);
            }
        }

        private void OnUploadFileClick(object sender, EventArgs e)
        {
            //openFileDialog1.InitialDirectory = @"C:\";
            DialogResult res = openFileDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                m_ServerProxy.UploadFile(openFileDialog1.FileName);
            }
        }

        private void OnFileRemoveClick(object sender, EventArgs e)
        {
            m_ServerProxy.RemoveFile(m_SelectedFile);
        }

        private void OnFileListMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (lstFiles.SelectedIndex < 0)
                    return;

                m_SelectedFile = (string)lstFiles.Items[lstFiles.SelectedIndex];
            }
        }


        private void OnFileListMouseMove(object sender, MouseEventArgs e)
        {
            int idx = lstFiles.IndexFromPoint(e.X, e.Y);
            if (idx == ListBox.NoMatches)
            {
                lstFiles.SelectedIndex = -1;
                return;
            }

            lstFiles.SelectedIndex = idx;

        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void OnRefreshClick(object sender, EventArgs e)
        {
            lstFiles.Items.Clear();
            PopulateFileList();
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

        private void OnFileTransferStatusUpdate(int pNumOfChunksCompleted, int pTotalNumOfChunks, int pChunkSize)
        {
            if (m_FileTransferStatus == null)
            {
                m_FileTransferStatus = new FileTransferStatus();
                m_FileTransferStatus.Show();
            }

            m_FileTransferStatus.PercentageDone = (double)pNumOfChunksCompleted / pTotalNumOfChunks;
            m_FileTransferStatus.Refresh();

            if (pNumOfChunksCompleted == pTotalNumOfChunks)
            {
                m_FileTransferStatus.Close();
                m_FileTransferStatus = null;
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
        private string m_SelectedFile;
        private FileTransferStatus m_FileTransferStatus;

        #endregion

        #endregion

    }
}
