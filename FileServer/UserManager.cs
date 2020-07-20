using ControlMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using CryptoManager;
using System.Data.Entity.Core.Common.CommandTrees;

namespace FileServer
{
    public class UserManager
    {
        #region Constructor
        public UserManager()
        {
            // Create admin user if necessary
            CreateAdminUser();
        }
        #endregion

        #region Public

        public UserData this[string pUserName]
        {
            get
            {
                UserDal dal = new UserDal();

                return dal.GetUserData(pUserName);
            }
        }

        #region Methods

        public bool CreateUser(UserData pUserData)
        {
            try
            {
                UserDal dal = new UserDal();
                dal.InsertNewUser(new UserData()
                {
                    UserName = pUserData.UserName,
                    Password = CryptoHandler.HashEncode(pUserData.Password),
                    HomePath = pUserData.HomePath,
                    UserRole = pUserData.UserRole
                });

                return true;
            }
            catch (Exception e)
            {
                Logger.Logger.Log("Exception in create user dal {0}", e.Message);
            }

            return false;
        }

        public bool ValidateUser(string pUserName, string pEncodedPassword)
        {
            try
            {
                UserDal dal = new UserDal();

                UserData userData = dal.GetUserData(pUserName);
                if (userData == null)
                    return false;

                if (string.Compare(userData.Password, pEncodedPassword) != 0)
                    return false;

                return true;
            }
            catch (Exception e)
            {
                Logger.Logger.Log("User {0} validation failed. {1}", pUserName, e.Message);
            }

            return false;

        }

        internal void RemoveUser(string pUserName)
        {
            try
            {
                UserDal dal = new UserDal();
                dal.RemoveUser(pUserName);
            }
            catch(Exception e)
            {
                Logger.Logger.Log("Failed to remove user {0}", e.Message);
            }
        }

        internal bool ChangePassword(string pUserName, string pOldPassword, string pNewPassword)
        {
            try
            {
                UserDal dal = new UserDal();

                UserData userData = dal.GetUserData(pUserName);

                if (string.Compare(userData.Password, CryptoHandler.HashEncode(pOldPassword)) != 0)
                {
                    Logger.Logger.Log("Invalid old password");
                    return false;
                }

                dal.ChangePassword(pUserName, CryptoHandler.HashEncode(pNewPassword));
                Logger.Logger.Log("Changed password to user {0}", pUserName);

                return true;
            }
            catch(Exception e)
            {
                Logger.Logger.Log("Failed to change password {0}", e.Message);
            }

            return false;
        }

        #endregion

        #endregion

        #region Private

        #region Methods

        private void CreateAdminUser()
        {
            const string adminName = "admin";
            const string adminPassword = "admin";

            UserDal dal = new UserDal();
            if (!dal.UserExists(adminName))
            {
                // Create default admin user
                string passwdEncoded = CryptoHandler.HashEncode(adminPassword);

                dal.InsertNewUser(new UserData()
                {
                    UserName = adminName,
                    Password = passwdEncoded,
                    HomePath = @"C:\",
                    UserRole = "Administrator"
                });
            }

        }

        #endregion

        #endregion

    }
}
