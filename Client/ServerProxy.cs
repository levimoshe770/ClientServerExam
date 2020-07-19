using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using Communicator;
using ControlMessages;
using MessageHandler;

namespace Client
{
    public delegate void dlgConnected(bool pStatus);
    public delegate void dlgUserLoggedIn(bool pStatus);
    public delegate void dlgUserCreated(string pHomePath);
    public delegate void dlgFolderList(List<string> pFolders);

    public class ServerProxy
    {
        #region Public

        #region Methods

        public void Connect(string pHost, int pPort)
        {
            m_Comm = new CommClient(pHost, pPort, "FileClient");
            m_Comm.CommStatusEvent += OnCommStatusEvent;
            m_Comm.ReceiveEvent += OnMessageReceived;
        }

        public void Disconnect()
        {
            m_Comm.Close();
        }

        public void CreateNewUser(UserData pUser)
        {
            CreateUserMessage msg = new CreateUserMessage() 
            { 
                UserName = pUser.UserName,
                Password = pUser.Password,
                HomePath = pUser.HomePath
            };

            byte[] buffer = MessageConverter<CreateUserMessage>.ObjectToRawMessage(msg);

            m_Comm.Send(buffer);
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

        public void GetFolders()
        {
            
        }

        public List<string> GetFiles(string pPath)
        {
            return new List<string>();
        }

        public void LogoutUser(UserData m_UserData)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Events

        public event dlgConnected ServerConnected;
        public event dlgUserLoggedIn UserLoggedIn;
        public event dlgUserCreated UserCreated;
        public event dlgFolderList FolderListArrived;

        #endregion

        #endregion

        #region Private

        #region Event handlers

        #region Comm handlers
        
        private void OnMessageReceived(byte[] pBuffer)
        {
            string msgId = MessageConverter.GetMessageId(pBuffer);

            if (string.Compare(msgId, "CreateUserMessage") == 0)
            {
                CreateUserMessage msg = MessageConverter<CreateUserMessage>.RawMessageToObject(pBuffer);

                UserCreated?.Invoke(msg.HomePath);
            }
        }

        private void OnCommStatusEvent(CommStatusEn pCommStatus, string pCommId)
        {
            ServerConnected?.Invoke(pCommStatus == CommStatusEn.Connected);
        }

        #endregion

        #endregion

        #region Members

        private ICommInterface m_Comm;

        #endregion

        #endregion
    }
}
