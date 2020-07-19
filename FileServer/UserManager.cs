using ControlMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using System.Data.Entity.Core.Common.CommandTrees;

namespace FileServer
{
    public class UserManager
    {
        public bool CreateUser(UserData pUserData, out string pHomePath)
        {
            pHomePath = string.Empty;
            try
            {
                pHomePath = string.Format(@"{0}\{1}", ServerConfig.HomeFolder, pUserData.UserName);
                UserDal dal = new UserDal();
                dal.InsertNewUser(new UserData()
                {
                    UserName = pUserData.UserName,
                    Password = pUserData.Password,
                    HomePath = pHomePath
                });

                return true;
            }
            catch (Exception e)
            {
                Logger.Logger.Log("Exception in create user dal {0}", e.Message);
            }

            return false;
        }

        //private 
    }
}
