using ControlMessages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class CreateUser : Form
    {
        public CreateUser(ServerProxy pServerProxy)
        {
            InitializeComponent();

            m_ServerProxy = pServerProxy;
        }

        public UserData UserData
        { 
            get { return m_UserData; }
        }

        private void OnCreateClick(object sender, EventArgs e)
        {
            if (txtPassword.Text != txtPassword.Text)
            {
                MessageBox.Show("Passwords do not match");
                return;
            }

            try
            {
                m_UserData = new UserData()
                {
                    UserName = txtUserName.Text,
                    Password = txtPassword.Text
                };
                m_ServerProxy.UserCreated += OnUserCreated;
                m_ServerProxy.CreateNewUser(m_UserData);
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

        private void OnUserCreated(string pHomePath)
        {
            if (pHomePath != "err")
            {
                m_UserData.HomePath = pHomePath;
                DialogResult = DialogResult.OK;
            }
            else
            {
                DialogResult = DialogResult.No;
            }

            Close();
        }

        private ServerProxy m_ServerProxy;
        private UserData m_UserData;
    }
}
