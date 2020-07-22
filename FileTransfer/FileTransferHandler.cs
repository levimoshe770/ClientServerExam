using Communicator;
using ControlMessages;
using MessageHandler;
using System;
using System.IO;
using System.Threading;

namespace FileTransfer
{
    public delegate void dlgFileTransferStatus(int pNumOfChunksCompleted, int pTotalNumOfChunks, int pChunkSize);

    public class FileTransferHandler
    {
        #region Constructor
        public FileTransferHandler(ICommInterface pComm)
        {
            m_Comm = pComm;
            m_Complete = false;
        }
        #endregion

        #region Public

        #region Methods

        public void SendFileToTheOtherSide(string pFileName)
        {
            ThreadPool.QueueUserWorkItem(SendFileToTheOtherSideTh, (object)pFileName);
        }

        public void ReceiveFileFromOtherSide(string pMsgId, byte[] pBuffer, string pLocalPath)
        {
            try
            {
                if (string.Compare(pMsgId, "FileTransferMsg") == 0)
                {
                    m_Start = DateTime.Now;

                    FileTransferMsg msg = MessageConverter<FileTransferMsg>.RawMessageToObject(pBuffer);

                    HandleFileTransferMsg(msg, pLocalPath);
                }
                else if (string.Compare(pMsgId, "BlobMsg") == 0)
                {
                    BlobMsg msg = MessageConverter<BlobMsg>.RawMessageToObject(pBuffer);

                    HandleBlobMessage(msg);
                }
                else if (string.Compare(pMsgId, "FileTransferComplete") == 0)
                {
                    m_Complete = true;

                    m_InFileStream.Close();
                    m_InFileStream.Dispose();
                    m_InFileStream = null;

                    m_End = DateTime.Now;

                    Logger.Logger.Log("Time to receive file is {0}", m_End - m_Start);
                }
            }
            catch (Exception e)
            {
                Logger.Logger.Log("Send file: {0} - {1}", e.Message, e.ToString());
            }

        }

        #endregion

        #region Events

        public event dlgFileTransferStatus FileTransferStatusEvent;

        #endregion

        #endregion

        #region Private

        private void SendFileToTheOtherSideTh(object pFileName)
        {
            string fileName = (string)pFileName;

            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                long size = fileInfo.Length;

                long numOfChunks = size / ChunkSize + (size % ChunkSize > 0 ? 1 : 0);

                DateTime start = DateTime.Now;

                SendFileTransferMsg(fileName, size, numOfChunks);

                using (FileStream r = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    for (int i = 0; i < numOfChunks; i++)
                    {
                        long chunkSize = Math.Min(ChunkSize, size - i * ChunkSize);
                        byte[] chunk = new byte[chunkSize];
                        int read = r.Read(chunk, 0, (int)chunkSize);

                        SendBlobMessage(chunk);

                        FileTransferStatusEvent?.Invoke(i + 1, (int)numOfChunks, (int)chunkSize);

                        Logger.Logger.Log("Sending chunk {0} out of {1}. Size: {2}", i + 1, numOfChunks, chunkSize);
                    }
                }

                SendFileTransferComplete();

                DateTime end = DateTime.Now;
                Logger.Logger.Log("Time to send file of size {0} is {1}", size, end - start);
            }
            catch (Exception e)
            {
                Logger.Logger.Log("Send file: {0} - {1}", e.Message, e.ToString());
            }
        }


        #region Methods

        private void SendFileTransferMsg(string pFileName, long size, long numOfChunks)
        {
            FileTransferMsg transferMsg = new FileTransferMsg()
            {
                Name = Path.GetFileName(pFileName),
                Size = size,
                NumOfChunks = numOfChunks,
                ChunkSize = ChunkSize
            };

            byte[] buffer = MessageConverter<FileTransferMsg>.ObjectToRawMessage(transferMsg);

            m_Comm.Send(buffer);
        }

        private void SendBlobMessage(byte[] chunk)
        {
            BlobMsg msg = new BlobMsg()
            {
                Blob = chunk
            };

            byte[] buffer = MessageConverter<BlobMsg>.ObjectToRawMessage(msg);

            m_Comm.Send(buffer);
        }

        private void SendFileTransferComplete()
        {
            FileTransferComplete msg = new FileTransferComplete();
            m_Comm.Send(MessageConverter<FileTransferComplete>.ObjectToRawMessage(msg));
        }

        private void HandleFileTransferMsg(FileTransferMsg pMsg, string pLocalPath)
        {
            m_FileName = string.Format(@"{0}\{1}", pLocalPath, pMsg.Name);
            m_Size = pMsg.Size;
            m_NumOfChunks = pMsg.NumOfChunks;
            m_ChunkSize = pMsg.ChunkSize;
            m_NumOfChunksReceived = 0;

            m_InFileStream = File.Open(m_FileName, FileMode.Append, FileAccess.Write, FileShare.None);

            Logger.Logger.Log("Getting ready to receive file {0}. Num of chunks {1}, Size {2}", m_FileName, m_NumOfChunks, m_Size);
        }

        private void HandleBlobMessage(BlobMsg msg)
        {
            m_InFileStream.Write(msg.Blob, 0, msg.Blob.Length);

            m_NumOfChunksReceived++;

            FileTransferStatusEvent?.Invoke(m_NumOfChunksReceived, (int)m_NumOfChunks, (int)m_ChunkSize);

            Logger.Logger.Log("Received chunk {0} out of {1}", m_NumOfChunksReceived, m_NumOfChunks);
        }

        #endregion

        #region Members

        private ICommInterface m_Comm;

        private string m_FileName;
        private long m_Size;
        private long m_NumOfChunks;
        private long m_ChunkSize;
        private int m_NumOfChunksReceived;
        private bool m_Complete;
        private FileStream m_InFileStream;

        private DateTime m_Start;
        private DateTime m_End;

        #endregion

        #region Constants

        private const int ChunkSize = 1024 * 1024; // 32K

        #endregion

        #endregion

    }
}
