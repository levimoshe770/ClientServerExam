using Communicator;
using ControlMessages;
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
        public ClientHandler(CommConnection pCommConnection, UserManager pUserManager)
        {
            m_UserManager = pUserManager;
            m_Comm = pCommConnection;
            m_Comm.CommStatusEvent += OnCommStatusEvent;
            m_Comm.ReceiveEvent += OnReceiveMsgEvent;

            m_UserData = null;
            m_UserValidated = false;
        }

        public event dlgClientDisconnected OnClientDisconnected;

        private void OnReceiveMsgEvent(byte[] pBuffer)
        {
            string msgId = MessageConverter.GetMessageId(pBuffer);

            if (string.Compare(msgId, "ValidateUserMessage") == 0)
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
            else if (string.Compare(msgId, "GetFilesMsg") == 0)
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

        private UserManager m_UserManager;
        private CommConnection m_Comm;
        private UserData m_UserData;
        private bool m_UserValidated;
    }
}
