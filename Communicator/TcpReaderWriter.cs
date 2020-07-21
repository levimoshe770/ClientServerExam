using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Logger;

namespace Communicator
{
    internal enum AbortReasonEn
    {
        SocketAborted,
        UserClosed
    }

    internal class TcpReaderWriter
    {
        #region Delegate

        public delegate void ReceiveInternalDlg(byte[] pBytes, uint pId);

        public delegate void ConnectedIdSetDlg(uint pId, string pConnectedId);

        public delegate void TcpReaderWriterAbortedDlg(AbortReasonEn pReason, uint pId);

        #endregion

        #region Constructor

        public TcpReaderWriter(TcpClient pTcpClient, string pId)
        {
            m_TcpClient = pTcpClient;
            m_Id = s_Counter++;

            SendId(pId);

            m_Receiver = new Thread(Receiver);
            m_Receiver.Name = string.Format("TcpReceiver {0}", m_Id);
            m_Receiver.Start();
        }

        #endregion

        #region Public

        #region Properties

        public uint Id
        {
            get { return m_Id; }
        }

        public string ConnectedId
        {
            get { return m_ConnectedId; }
        }

        #endregion

        #region Methods

        public void Send(byte[] pBuffer)
        {
            Send(pBuffer, DataMsg);
        }

        public void Abort()
        {
            try
            {
                byte[] buffer = new byte[3];
                const ushort size = 0;

                NetworkStream networkStream = m_TcpClient.GetStream();
                Buffer.BlockCopy(BitConverter.GetBytes(size), 0, buffer, 0, 2);
                buffer[2] = AbortMsg;

                networkStream.Write(buffer, 0, size + 3);
            }
            catch (Exception e)
            {
                Logger.Logger.Log(string.Format("TcpReadWrite aborted. Exception log: {0}", e));
                Logger.Logger.Log(e.Message);
                RaiseTcpReaderWriterAbortedEvent(AbortReasonEn.SocketAborted);
            }
        }

        #endregion

        #region Events

        public event ReceiveInternalDlg ReceiveEvent;
        public event ConnectedIdSetDlg ConnectedIdSetEvent;
        public event TcpReaderWriterAbortedDlg TcpReaderWriterAbortedEvent;

        #endregion

        #endregion

        #region Private

        #region Methods

        private void Send(byte[] pBuffer, byte pControl)
        {
            try
            {
                byte[] buffer = new byte[MaxBufferSize + 5];
                UInt32 size = (UInt32)pBuffer.Length;

                NetworkStream networkStream = m_TcpClient.GetStream();

                Array.Copy(BitConverter.GetBytes(size), buffer, 4);
                buffer[4] = pControl;
                Array.Copy(pBuffer, 0, buffer, 5, size);

                networkStream.Write(buffer, 0, (int)size + 5);

            }
            catch (Exception e)
            {
                Logger.Logger.Log(string.Format("TcpReadWrite aborted. Exception log: {0}", e));
                Logger.Logger.Log(e.Message);
                RaiseTcpReaderWriterAbortedEvent(AbortReasonEn.SocketAborted);
            }
        }

        private void SendId(string pId)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(pId);

            Send(buffer, IdMsg);
        }

        private void Receiver()
        {
            try
            {
                while (true)
                {
                    byte[] bytes = ReadPacket();

                    if (bytes.Length > 0)
                        RaiseReceiveEvent(bytes);
                }
            }
            catch (Exception e)
            {
                Logger.Logger.Log("Receiver {0}, StackTrace {1}, Inner: {2}", e.Message, e.StackTrace, e.InnerException);

                RaiseTcpReaderWriterAbortedEvent(AbortReasonEn.SocketAborted);
            }
        }

        private void RaiseReceiveEvent(byte[] pBytes)
        {
            ReceiveEvent?.Invoke(pBytes, m_Id);
        }

        private void RaiseTcpReaderWriterAbortedEvent(AbortReasonEn pReason)
        {
            TcpReaderWriterAbortedEvent?.Invoke(pReason, Id);
        }

        private void RaiseConnectedIdSet(uint pId, string pConnectedId)
        {
            ConnectedIdSetEvent?.Invoke(pId, pConnectedId);
        }

        /// <summary>
        /// Reads a full packet from the stream, and blocks until it is fully received. First two bytes of a packet
        /// determine it's size
        /// </summary>
        /// <returns></returns>
        private byte[] ReadPacket()
        {
            NetworkStream stream = m_TcpClient.GetStream();
            byte[] buffer = new byte[MaxBufferSize];
            byte[] sizeBuffer = new byte[4];

            // Read packet size
            stream.Read(sizeBuffer, 0, 4);
            Int32 size = BitConverter.ToInt32(sizeBuffer, 0);
            if (size < 0)
            {
                throw new Exception("Mismatch in incoming stream. Received negative size");
            }

            // Read control byte
            byte controlByte = (byte) stream.ReadByte();
            if (controlByte == AbortMsg)
            {
                RaiseTcpReaderWriterAbortedEvent(AbortReasonEn.UserClosed);
            } 
            else if (controlByte == IdMsg)
            {
                if (size > 0)
                {
                    stream.Read(buffer, 0, (int)size);
                    byte[] res = new byte[size];
                    Array.Copy(buffer, res, size);
                    m_ConnectedId = Encoding.ASCII.GetString(res);

                    RaiseConnectedIdSet(m_Id, m_ConnectedId);

                    return new byte[]{};
                }
            }

            if (size > 0)
            {
                //Logger.Logger.Log("Size: {0}", size);
                // Read packet -- Block read until all the bytes are received

                int totalRead = 0;

                while (totalRead != size)
                {
                    int read = stream.Read(buffer, totalRead, size - totalRead);
                    totalRead += read;
                }

                byte[] res = new byte[size];

                Array.Copy(buffer, res, size);

                return res;
            }

            return new byte[]{};
        }

        #endregion

        #region Members

        private readonly TcpClient m_TcpClient;
        private readonly Thread m_Receiver;
        private readonly uint m_Id;
        private string m_ConnectedId = "";

        private static uint s_Counter;

        #endregion

        #region Const

        private const uint MaxBufferSize = 2048 * 1024;

        private const byte AbortMsg = 1;
        private const byte DataMsg = 0;
        private const byte IdMsg = 2;

        #endregion

        #endregion
    }
}
