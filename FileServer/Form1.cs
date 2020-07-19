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

namespace FileServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void OnOpenListenerClick(object sender, MouseEventArgs e)
        {
            int port;
            if (int.TryParse(txtPort.Text, out port))
            {
                ServerConfig.Port = port;
                ServerConfig.HomeFolder = txtFolder.Text;

                m_ServerCommHandler = new ServerCommHandler();

                return;
            }

            MessageBox.Show("Invalid port. Should be int");
        }

        private void OnChooseFolderClick(object sender, EventArgs e)
        {
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
            DialogResult res = folderBrowserDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                txtFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private ServerCommHandler m_ServerCommHandler;
    }
}
