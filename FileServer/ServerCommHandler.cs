using Communicator;
using ControlMessages;
using MessageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer
{
    public class ServerCommHandler
    {
        public ServerCommHandler()
        {
            m_Comm = new CommServer(ServerConfig.Port, "FileServer");
            m_Comm.CommStatusEvent += OnCommStatus;
            m_Comm.ReceiveEvent += OnMessageReceive;

            // Create DB

        }

        private void OnMessageReceive(byte[] pBuffer)
        {
            string msgId = MessageConverter.GetMessageId(pBuffer);

            if (string.Compare(msgId, "CreateUserMessage") == 0)
            {
                CreateUserMessage msg = MessageConverter<CreateUserMessage>.RawMessageToObject(pBuffer);

                UserManager userManager = new UserManager();
                string homeFolder;
                bool status  = userManager.CreateUser(new UserData()
                {
                    UserName = msg.UserName,
                    Password = msg.Password,
                    HomePath = msg.HomePath
                }, out homeFolder);

                if (status)
                {
                    CreateUserMessage respondMsg = new CreateUserMessage()
                    {
                        UserName = msg.UserName,
                        Password = msg.Password,
                        HomePath = homeFolder
                    };

                    byte[] buffer = MessageConverter<CreateUserMessage>.ObjectToRawMessage(respondMsg);

                    m_Comm.Send(buffer);
                }
                
            }
        }

        private void OnCommStatus(CommStatusEn pCommStatus, string pCommId)
        {
            Logger.Logger.Log(string.Format("{0} {1}",
                pCommId,
                pCommStatus == CommStatusEn.Connected ? "Connected" : "Disconnected"
                ));
        }

        private ICommInterface m_Comm;
    }
}
