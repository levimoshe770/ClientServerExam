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
using System.IO;

namespace FileServer
{
    public partial class CreateUser : Form
    {
        #region Constructor

        public CreateUser(UserManager pUserManager)
        {
            InitializeComponent();

            m_UserManager = pUserManager;
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
                string userPath = string.Format(@"{0}\{1}", ServerConfig.HomeFolder, txtUserName.Text);
                m_UserData = new UserData()
                {
                    UserName = txtUserName.Text,
                    Password = txtPassword.Text,
                    HomePath = userPath,
                    UserRole = "Regular"

                };

                bool userCreated = m_UserManager.CreateUser(m_UserData);

                // Create user's folder
                if (!Directory.Exists(userPath))
                {
                    Directory.CreateDirectory(userPath);
                }

                if (userCreated)
                    DialogResult = DialogResult.OK;
                else
                    DialogResult = DialogResult.No;

                Close();
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

        #endregion

        #region Members

        private UserData m_UserData;
        private UserManager m_UserManager;

        #endregion

        #endregion
    }
}
