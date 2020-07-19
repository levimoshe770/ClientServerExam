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
        #region Constructor

        public CreateUser(ServerProxy pServerProxy)
        {
            InitializeComponent();

            m_ServerProxy = pServerProxy;
        }

        #endregion

        #region Public

        #region Properties

        public UserData UserData
        { 
            get { return m_UserData; }
        }

        #endregion

        #endregion

        #region Private

        #region Winform event handlers

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
                    Password = txtPassword.Text,
                    HomePath = ""
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

        #endregion

        #region Event handlers

        private void OnUserCreated(string pHomePath)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new dlgUserCreated(OnUserCreated), new object[] { pHomePath });
            }
            else
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
        }

        #endregion

        #region Members

        private ServerProxy m_ServerProxy;
        private UserData m_UserData;

        #endregion

        #endregion
    }
}
