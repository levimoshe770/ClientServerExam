using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;

namespace Communicator
{
    public class CommClient : ICommInterface
    {
        #region Constructor

        public CommClient (string pRemoteHost, int pPort, string pId)
        {
            m_RemoteHost = pRemoteHost;
            m_Port = pPort;
            m_Id = pId;

            m_Connector = new Thread(Connector);
            m_Connector.Name = "TCP Client Connector";
            m_Connector.Start();
        }

        #endregion

        #region ICommInterface

        public void Send(byte[] pBuffer)
        {
            if (m_Connected)
            {
                m_TcpReaderWriter.Send(pBuffer);
            }
            else
            {
                Logger.Logger.Log("Attempt to send buffer to non connected recepient");
            }
        }

        public void Close()
        {
            m_TcpReaderWriter.Abort();
        }

        public event ReceiveDlg ReceiveEvent;
        public event CommStatusDlg CommStatusEvent;

        #endregion

        #region Private methods

        private void Connector()
        {
            while (true)
            {
                try
                {
                    m_TcpClient = new TcpClient();

                    if (string.Compare(m_RemoteHost, "localhost") == 0)
                    {
                        m_RemoteHost = Dns.GetHostName();
                    }

                    IPAddress iPAddress =
                        (from ip in Dns.GetHostEntry(m_RemoteHost).AddressList
                         where ip.AddressFamily == AddressFamily.InterNetwork
                         select ip).First();
                    IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, m_Port);
                    m_TcpClient.Connect(iPEndPoint);
                    if (m_TcpClient.Connected)
                    {
                        m_TcpReaderWriter = new TcpReaderWriter(m_TcpClient, m_Id);
                        m_TcpReaderWriter.ReceiveEvent += OnReceiveEvent;
                        m_TcpReaderWriter.TcpReaderWriterAbortedEvent += OnTcpReaderWriterAbortEvent;
                        m_TcpReaderWriter.ConnectedIdSetEvent += OnConnectedIdSet;
                        m_Connected = true;
                        RaiseConnectionStatus(true);
                        Thread.Sleep(100);
                        break;
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                catch (SocketException e)
                {
                    //Logger.LoggerSingle.ExceptionLog(e);
                    Thread.Sleep(100);
                }
                catch (Exception e)
                {
                    Logger.Logger.Log(e.Message);
                    Thread.Sleep(100);
                }
            }
        }

        private void OnConnectedIdSet(uint pId, string pConnectedId)
        {
            m_ConnectedId = pConnectedId;
        }

        private void OnTcpReaderWriterAbortEvent(AbortReasonEn pPreason, uint pId)
        {
            m_Connected = false;
            m_ConnectedId = "";

            RaiseConnectionStatus(false);

            if (m_Connector == null)
            {
                m_Connector = new Thread(Connector);
                m_Connector.Start();
                return;
            }

            if (m_Connector.ThreadState != ThreadState.Running)
            {
                m_Connector = new Thread(Connector);
                m_Connector.Start();
            }
        }

        private void RaiseConnectionStatus(bool pStatus)
        {
            CommStatusEvent?.Invoke(pStatus ? CommStatusEn.Connected : CommStatusEn.Disconnected, m_ConnectedId);
        }

        private void OnReceiveEvent(byte[] pBytes, uint pId)
        {
            RaiseReceiveEvent(pBytes);
        }

        private void RaiseReceiveEvent(byte[] pBytes)
        {
            ReceiveEvent?.Invoke(pBytes);
        }

        #endregion

        #region Private members

        private TcpReaderWriter m_TcpReaderWriter;
        private TcpClient m_TcpClient;
        private Thread m_Connector;
        private string m_RemoteHost;
        private readonly int m_Port;
        private readonly string m_Id;
        private bool m_Connected;
        private string m_ConnectedId;

        #endregion
    }
}
