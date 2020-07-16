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
    public partial class UserLogin : Form
    {
        public UserLogin(ServerProxy pServerProxy)
        {
            InitializeComponent();

            m_ServerProxy = pServerProxy;
        }

        public UserData UserData { get { return m_UserData; } }

        private void OnLoginClick(object sender, EventArgs e)
        {
            m_UserData = new UserData()
            {
                UserName = txtUserName.Text,
                Password = txtPassword.Text
            };

            try
            {
                m_ServerProxy.UserLoggedIn += OnUserLoggedIn;
                m_ServerProxy.LoginUser(m_UserData);
            }
            catch
            {
                DialogResult = DialogResult.No;
                Close();
            }

        }

        private void OnLoginCancel(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void OnUserLoggedIn(bool pStatus)
        {
            if (pStatus)
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                DialogResult = DialogResult.None;
            }
            Close();
        }

        private UserData m_UserData;
        private ServerProxy m_ServerProxy;
    }
}
