using Communicator;
using ControlMessages;
using FileTransfer;
using MessageHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer
{
    public delegate void dlgClientDisconnected(string pId);

    public class ClientHandler
    {
        #region Constructor
        public ClientHandler(CommConnection pCommConnection, UserManager pUserManager)
        {
            m_UserManager = pUserManager;
            m_Comm = pCommConnection;
            m_Comm.CommStatusEvent += OnCommStatusEvent;
            m_Comm.ReceiveEvent += OnReceiveMsgEvent;

            m_UserData = null;
            m_UserValidated = false;
        }
        #endregion

        #region Public

        #region Events

        public event dlgClientDisconnected OnClientDisconnected;

        #endregion

        #endregion

        #region Private

        #region Comm Event handlers

        private void OnReceiveMsgEvent(byte[] pBuffer)
        {
            string msgId = MessageConverter.GetMessageId(pBuffer);

            if (string.Compare(msgId, "ValidateUserMessage") == 0)
            {
                HandleValidateUserMessage(pBuffer);
            }
            else if (string.Compare(msgId, "GetFilesMsg") == 0)
            {
                HandleGetFilesMsg();
            }
            else if (string.Compare(msgId, "TransferFileMsg") == 0)
            {
                HandleRequestFileMsg(pBuffer);
            }
            else if (string.Compare(msgId, "FileTransferMsg") == 0 ||
                         string.Compare(msgId, "BlobMsg") == 0 ||
                         string.Compare(msgId, "FileTransferComplete") == 0)
            {
                if (string.Compare(msgId, "FileTransferMsg") == 0)
                {
                    m_FileTransferHandler = new FileTransferHandler(m_Comm);
                }

                m_FileTransferHandler.ReceiveFileFromOtherSide(msgId, pBuffer, m_UserData.HomePath);
            }
            else if (string.Compare(msgId, "RemoveFileMsg") == 0)
            {
                RemoveFileMsg msg = MessageConverter<RemoveFileMsg>.RawMessageToObject(pBuffer);

                string fileName = string.Format(@"{0}\{1}", m_UserData.HomePath, msg.FileName);

                File.Delete(fileName);
            }
        }

        private void OnCommStatusEvent(CommStatusEn pCommStatus, string pCommId)
        {
            if (pCommStatus == CommStatusEn.Disconnected)
                OnClientDisconnected?.Invoke(pCommId);

            Logger.Logger.Log("Client {0} {1}",
                pCommId,
                pCommStatus == CommStatusEn.Connected ? "connected" : "disconnected"
                );
        }

        #endregion

        #region Methods

        private void HandleRequestFileMsg(byte[] pBuffer)
        {
            RequestFileMsg msg = MessageConverter<RequestFileMsg>.RawMessageToObject(pBuffer);

            string fileName = string.Format(@"{0}\{1}", m_UserData.HomePath, msg.File);

            FileTransferHandler handler = new FileTransferHandler(m_Comm);

            handler.SendFileToTheOtherSide(fileName);
        }

        private void HandleGetFilesMsg()
        {
            if (!m_UserValidated)
            {
                Logger.Logger.Log("User is not validated for file retrieval");
            }
            else
            {
                string[] files = Directory.GetFiles(m_UserData.HomePath);

                FileListBegin beginMsg = new FileListBegin();
                byte[] responseBuffer = MessageConverter<FileListBegin>.ObjectToRawMessage(beginMsg);

                m_Comm.Send(responseBuffer);

                foreach (string file in files)
                {
                    FileMsg fileMsg = new FileMsg()
                    {
                        FileName = Path.GetFileName(file)
                    };

                    responseBuffer = MessageConverter<FileMsg>.ObjectToRawMessage(fileMsg);
                    m_Comm.Send(responseBuffer);
                }

                FileListEnd fileListEndMsg = new FileListEnd();
                responseBuffer = MessageConverter<FileListEnd>.ObjectToRawMessage(fileListEndMsg);

                m_Comm.Send(responseBuffer);

            }
        }

        private void HandleValidateUserMessage(byte[] pBuffer)
        {
            ValidateUserMessage msg = MessageConverter<ValidateUserMessage>.RawMessageToObject(pBuffer);

            bool status = m_UserManager.ValidateUser(msg.UserName, msg.Password);

            UserValidationStatus response = new UserValidationStatus()
            {
                Validated = status
            };

            if (status)
            {
                m_UserData = m_UserManager[msg.UserName];
                m_UserValidated = true;
            }

            byte[] responseBuffer = MessageConverter<UserValidationStatus>.ObjectToRawMessage(response);

            m_Comm.Send(responseBuffer);
        }

        #endregion

        #region Members

        private UserManager m_UserManager;
        private CommConnection m_Comm;
        private UserData m_UserData;
        private bool m_UserValidated;

        private FileTransferHandler m_FileTransferHandler;

        #endregion


        #endregion
    }
}
