using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Logger;

namespace Communicator
{
    public class CommServer : ICommInterface
    {
        private class CommClientInternal
        {
            public TcpReaderWriter TcpReaderWriter { get; set; }
            public string Id { get; set; }
        }

        #region Constructor

        public CommServer(int pPort, string pId)
        {
            m_Id = pId;
            m_Server = new TcpListener(IPAddress.Parse("127.0.0.1"),pPort);

            m_ListeningThread = new Thread(Listener);
            m_ListeningThread.Name = "TCP Listening " + pId;
            m_ListeningThread.Start();
        }

        #endregion

        #region ICommInterface

        public int OpenConnections
        {
            get 
            { 
                if (m_Client != null) return 1;
                return 0;
            }
        }

        public void Send(byte[] pBuffer)
        {
            m_Client.TcpReaderWriter.Send(pBuffer);
        }

        public void Close()
        {
            m_Client.TcpReaderWriter.Abort();
        }

        public event ReceiveDlg ReceiveEvent;

        public event CommStatusDlg CommStatusEvent;

        #endregion

        #region Private methods

        //private void HandleTcpClientException(Exception pException, TcpReaderWriter pTcpReaderWriter)
        //{
        //    ATLogger.AtLogger.sLogger.Critical(pTcpReaderWriter.Id + pException.ToString());
        //    throw pException;
        //}

        private void Listener()
        {
            try
            {
                m_Server.Start();

                while (true)
                {
                    TcpClient newClient = m_Server.AcceptTcpClient();
                    CommClientInternal client = new CommClientInternal
                                                    {
                                                        TcpReaderWriter = new TcpReaderWriter(newClient, m_Id),
                                                        Id = ""
                                                    };
                    client.TcpReaderWriter.TcpReaderWriterAbortedEvent += OnTcpReaderWriterAborted;
                    client.TcpReaderWriter.ReceiveEvent += OnMsgReceive;
                    client.TcpReaderWriter.ConnectedIdSetEvent += OnConnectedIdSet;

                    m_Client = client;
                }
            }
            catch (Exception e)
            {
                Logger.Logger.Log(e.Message);
                throw;
            }
            
        }

        private void OnConnectedIdSet(uint pId, string pConnectedid)
        {
            m_Client.Id = pConnectedid;
 
            RaiseCommStatusEvent(CommStatusEn.Connected, pConnectedid);
       }

        private void OnTcpReaderWriterAborted(AbortReasonEn pReason, uint pId)
        {
            RaiseCommStatusEvent(CommStatusEn.Disconnected, m_Client.Id);
        }

        private void OnMsgReceive(byte[] pBytes, uint pId)
        {
            RaiseReceiveEvent(pBytes);
        }

        private void RaiseReceiveEvent(byte[] pBytes)
        {
            ReceiveEvent?.Invoke(pBytes);
        }

        private void RaiseCommStatusEvent(CommStatusEn pCommStatus, string pCommId)
        {
            CommStatusEvent?.Invoke(pCommStatus, pCommId);
        }

        #endregion

        #region Members

        private readonly TcpListener m_Server;
        private CommClientInternal m_Client;
        private readonly Thread m_ListeningThread;
        //private string m_ConnectedId;
        private readonly string m_Id;
        
        #endregion
    }
}
