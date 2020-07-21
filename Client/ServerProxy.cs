using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Communicator;
using ControlMessages;
using FileTransfer;
using MessageHandler;

namespace Client
{
    public delegate void dlgConnected(bool pStatus);
    public delegate void dlgFileListArrived(List<string> pFiles);

    public class ServerProxy
    {
        #region Public

        #region Methods

        public void Connect(string pHost, int pPort, string pUserName, string pPassword)
        {
            m_UserName = pUserName;
            m_Password = pPassword;

            m_Comm = new CommClient(pHost, pPort, "FileClient");
            m_Comm.CommStatusEvent += OnCommStatusEvent;
            m_Comm.ReceiveEvent += OnMessageReceived;
        }

        public void Disconnect()
        {
            m_Comm.Close();
        }

        public void DownloadFile(string pFileName, string pLocalPath)
        {
            m_Localpath = pLocalPath;

            RequestFileMsg msg = new RequestFileMsg()
            {
                File = pFileName
            };

            byte[] buffer = MessageConverter<RequestFileMsg>.ObjectToRawMessage(msg);

            m_Comm.Send(buffer);
        }

        public void UploadFile(string pFileName)
        {
            FileTransferHandler fh = new FileTransferHandler(m_Comm);
            fh.FileTransferStatusEvent += OnFileTransferStatusUpdated;

            fh.SendFileToTheOtherSide(pFileName);
        }

        internal void RemoveFile(string pSelectedFile)
        {
            RemoveFileMsg msg = new RemoveFileMsg()
            {
                FileName = pSelectedFile
            };

            byte[] buffer = MessageConverter<RemoveFileMsg>.ObjectToRawMessage(msg);
            m_Comm.Send(buffer);
        }

        public void GetFiles()
        {
            GetFilesMsg msg = new GetFilesMsg();

            byte[] buffer = MessageConverter<GetFilesMsg>.ObjectToRawMessage(msg);

            m_Comm.Send(buffer);
        }

        #endregion

        #region Events

        public event dlgConnected ServerConnected;
        public event dlgFileListArrived FileListArrived;
        public event dlgFileTransferStatus FileTransferStatus;

        #endregion

        #endregion

        #region Private

        #region Event handlers

        #region Comm handlers
        
        private void OnMessageReceived(byte[] pBuffer)
        {
            try
            {
                string msgId = MessageConverter.GetMessageId(pBuffer);

                if (string.Compare(msgId, "UserValidationStatus") == 0)
                {
                    UserValidationStatus msg = MessageConverter<UserValidationStatus>.RawMessageToObject(pBuffer);

                    if (msg.Validated)
                    {
                        Logger.Logger.Log("Connection successful");
                        ServerConnected?.Invoke(true);
                    }
                    else
                    {
                        Logger.Logger.Log("User is invalid");
                        ServerConnected?.Invoke(false);
                    }
                }
                else if (string.Compare(msgId, "FileListBegin") == 0)
                {
                    m_FileList = new List<string>();
                }
                else if (string.Compare(msgId, "FileMsg") == 0)
                {
                    FileMsg msg = MessageConverter<FileMsg>.RawMessageToObject(pBuffer);

                    m_FileList.Add(msg.FileName);
                }
                else if (string.Compare(msgId, "FileListEnd") == 0)
                {
                    FileListArrived?.Invoke(m_FileList);
                }
                else if (string.Compare(msgId, "FileTransferMsg") == 0 ||
                         string.Compare(msgId, "BlobMsg") == 0 ||
                         string.Compare(msgId, "FileTransferComplete") == 0)
                {
                    if (string.Compare(msgId, "FileTransferMsg") == 0)
                    {
                        m_FileTransferHandler = new FileTransferHandler(m_Comm);
                        m_FileTransferHandler.FileTransferStatusEvent += OnFileTransferStatusUpdated;
                    }

                    m_FileTransferHandler.ReceiveFileFromOtherSide(msgId, pBuffer, m_Localpath);
                }
            }
            catch (Exception e)
            {
                Logger.Logger.Log("Handle Message {0},{1}", e.Message,e.StackTrace[1].ToString());
            }

        }
        
        private void OnCommStatusEvent(CommStatusEn pCommStatus, string pCommId)
        {
            if (pCommStatus == CommStatusEn.Connected)
            {
                // Send validation message
                ValidateUserMessage msg = new ValidateUserMessage()
                {
                    UserName = m_UserName,
                    Password = m_Password
                };

                byte[] buffer = MessageConverter<ValidateUserMessage>.ObjectToRawMessage(msg);

                m_Comm.Send(buffer);
            }
        }

        #endregion

        #region General event handlers

        private void OnFileTransferStatusUpdated(int pNumOfChunksCompleted, int pTotalNumOfChunks, int pChunkSize)
        {
            FileTransferStatus?.Invoke(pNumOfChunksCompleted, pTotalNumOfChunks, pChunkSize);
        }

        #endregion

        #endregion

        #region Members

        private ICommInterface m_Comm;
        private string m_UserName;
        private string m_Password;
        private List<string> m_FileList;
        private FileTransferHandler m_FileTransferHandler;
        private string m_Localpath;

        #endregion

        #endregion
    }
}
