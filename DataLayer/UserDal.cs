using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControlMessages;

namespace DataLayer
{
    public class UserDal
    {
        public UserDal()
        {
            CreateUserTable();
        }

        public void InsertNewUser(UserData pUserData)
        {
            try
            {
                DataServices ds = new DataServices();

                string cmd = string.Format("INSERT INTO USERTBL(USERNAME, PASSWORD, HOMEFOLDER) VALUES ('{0}','{1}','{2}')",
                    pUserData.UserName,
                    pUserData.Password,
                    pUserData.HomePath
                    );

                ds.ExecuteNonQuery(cmd);

                Logger.Logger.Log("User {0} created", pUserData.UserName);
            }
            catch (Exception e)
            {
                Logger.Logger.Log("Failed to create user: {0}", e.Message);
                throw e;
            }
        }
        private void CreateUserTable()
        {
            try
            {
                const string userTbl = "USERTBL";

                DataServices ds = new DataServices();

                if (ds.TableExists(userTbl))
                    return;

                string cmd = string.Format("CREATE TABLE {0} ({1});",
                    userTbl,
                    "USERNAME VARCHAR(50), PASSWORD VARCHAR(50), HOMEFOLDER VARCHAR(100)"
                    );

                ds.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                Logger.Logger.Log("Failed to create user table: {0}", e.Message);
                throw e;
            }
        }

    }
}
