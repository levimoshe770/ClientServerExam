using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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

                string cmd = string.Format("INSERT INTO USERTBL(USERNAME, PASSWORD, HOMEFOLDER, USERROLE) VALUES ('{0}','{1}','{2}', '{3}')",
                    pUserData.UserName,
                    pUserData.Password,
                    pUserData.HomePath,
                    pUserData.UserRole
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

        public bool UserExists(string pUserName)
        {
            try
            {
                DataServices ds = new DataServices();

                string cmd = string.Format("SELECT COUNT(1) FROM USERTBL WHERE USERNAME = '{0}'", pUserName);

                SQLiteDataReader dr = ds.ExecuteReader(cmd);
                int cnt = 0;
                while (dr.Read())
                {
                    cnt = dr.GetInt32(0);
                }

                dr.Close();
                dr.Dispose();
                ds.CloseConnection();

                return cnt > 0;
            }
            catch(Exception e)
            {
                Logger.Logger.Log("Failed to check if user exist: {0}", e.Message);
                throw e;
            }
        }

        public void RemoveUser(string pUserName)
        {
            try
            {
                DataServices ds = new DataServices();

                string cmd = string.Format("DELETE FROM USERTBL WHERE USERNAME = '{0}'",
                    pUserName);

                ds.ExecuteNonQuery(cmd);

                Logger.Logger.Log("User {0} removed", pUserName);
            }
            catch(Exception e)
            {
                Logger.Logger.Log("Failed to remove user {0} - {1}", pUserName, e.Message);
                throw e;
            }
        }

        public void ChangePassword(string pUserName, string pNewPassword)
        {
            try
            {
                DataServices ds = new DataServices();

                string cmd = string.Format("UPDATE USERTBL SET PASSWORD = '{0}' WHERE USERNAME = '{1}' ",
                    pNewPassword, pUserName
                    );

                ds.ExecuteNonQuery(cmd);

                Logger.Logger.Log("Password changed");
            }
            catch(Exception e)
            {
                Logger.Logger.Log("Failed to change password {0}", e.Message);
                throw e;
            }
        }

        public UserData GetUserData(string pUserName)
        {
            UserData res = null;

            try
            {
                DataServices ds = new DataServices();

                string cmdFrmt = "SELECT USERNAME, PASSWORD, HOMEFOLDER, USERROLE " +
                                 "FROM USERTBL " +
                                 "WHERE USERNAME = '{0}'";
                string cmd = string.Format(cmdFrmt, pUserName);

                SQLiteDataReader dr = ds.ExecuteReader(cmd);

                dr.Read();

                res = new UserData()
                {
                    UserName = pUserName,
                    Password = dr.GetString(1),
                    HomePath = dr.GetString(2),
                    UserRole = dr.GetString(3)
                };

                dr.Close();
                dr.Dispose();
                ds.CloseConnection();

                return res;
            }
            catch(Exception e)
            {
                Logger.Logger.Log("Failed to get user data {0}", e.Message);
                throw e;
            }
        }

        public DataTable GetAllUsers()
        {
            try
            {
                DataServices ds = new DataServices();

                string cmd =
                    "SELECT USERNAME, USERROLE, HOMEFOLDER " +
                    "FROM USERTBL ";

                SQLiteDataAdapter da = ds.ExecuteAdapter(cmd);

                DataTable res = new DataTable();
                da.Fill(res);

                da.Dispose();
                ds.CloseConnection();

                return res;

            }
            catch (Exception e)
            {
                Logger.Logger.Log("Error retrieving users {0}", e.Message);
                return null;
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
                    "USERNAME VARCHAR(50), PASSWORD VARCHAR(50), HOMEFOLDER VARCHAR(100), USERROLE VARCHAR(50)"
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
