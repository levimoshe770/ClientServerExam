using DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileServer
{
    public partial class ChangePassword : Form
    {
        public ChangePassword(UserManager pUserManager, string pUserName)
        {
            InitializeComponent();

            m_UserManager = pUserManager;
            m_UserName = pUserName;
        }

        private void OnOkClick(object sender, EventArgs e)
        {
            if (string.Compare(txtNewPassword.Text, txtConfirmPassword.Text) != 0)
            {
                DialogResult = DialogResult.No;
                MessageBox.Show("Passwords don't match");
                return;
            }

            bool userChanged = m_UserManager.ChangePassword(m_UserName, txtOldPassword.Text, txtNewPassword.Text);

            DialogResult = userChanged ? DialogResult.OK : DialogResult.No;

            Close();
           
        }

        private void OnCancelClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private UserManager m_UserManager;
        private string m_UserName;
    }
}
