using Communicator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Communicator
{
    public class CommConnection : ICommInterface
    {
        #region Constructor
        public CommConnection(TcpClient pTcpClient, string pId)
        {
            m_Id = pId;
            m_ReaderWriter = new TcpReaderWriter(pTcpClient, pId);
            m_ReaderWriter.TcpReaderWriterAbortedEvent += OnTcpReaderWriterAborted;
            m_ReaderWriter.ReceiveEvent += OnMsgReceived;
            m_ReaderWriter.ConnectedIdSetEvent += OnConnectedIdSet;
        }
        #endregion

        #region Public

        #region Properties
        public string Id { get { return m_Id; } }
        #endregion

        #region ICommInterface

        public void Close()
        {
            m_ReaderWriter.Abort();
        }

        public void Send(byte[] pBuffer)
        {
            m_ReaderWriter.Send(pBuffer);
        }

        public event ReceiveDlg ReceiveEvent;
        public event CommStatusDlg CommStatusEvent;

        #endregion

        #endregion

        #region Private

        #region Event handlers
        private void OnConnectedIdSet(uint pId, string pConnectedId)
        {
            CommStatusEvent?.Invoke(CommStatusEn.Connected, pConnectedId);
        }

        private void OnMsgReceived(byte[] pBytes, uint pId)
        {
            ReceiveEvent?.Invoke(pBytes);
        }

        private void OnTcpReaderWriterAborted(AbortReasonEn pReason, uint pId)
        {
            Logger.Logger.Log("Connection {0} aborted. Reason: {1}", pId, pReason);
            CommStatusEvent?.Invoke(CommStatusEn.Disconnected, m_Id);
        }
        #endregion

        #region Members
        private TcpReaderWriter m_ReaderWriter;
        private string m_Id;
        #endregion

        #endregion
    }
}
