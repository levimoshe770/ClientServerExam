using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using ControlMessages;

namespace Client
{
    public delegate void dlgConnected(bool pStatus);
    public delegate void dlgUserLoggedIn(bool pStatus);
    public delegate void dlgUserCreated(string pHomePath);

    public class ServerProxy
    {
        public void Connect (string pHost, int pPort)
        {

        }

        public void Disconnect()
        {

        }

        public string CreateNewUser(UserData pUser)
        {
            throw new NotImplementedException();
        }

        public void DeleteUser(string pUserName)
        {

        }

        public void CreateFolder(string pPath)
        {

        }

        public void RemoveFolder(string pPath)
        {

        }

        public void LoginUser(UserData pUserData)
        {

        }

        public void DownloadFile(string pRemotePath, string pLocalPath)
        {

        }

        public void UploadFile(string pLocalPath, string pRemotePath)
        {

        }

        public event dlgConnected ServerConnected;
        public event dlgUserLoggedIn UserLoggedIn;
        public event dlgUserCreated UserCreated;
    }
}
