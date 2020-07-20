using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Logger;

namespace Communicator
{
    public delegate void dlgClientConnected(CommConnection pClient);

    public class CommServer
    {
        private class CommClientInternal
        {
            public TcpReaderWriter TcpReaderWriter { get; set; }
            public string Id { get; set; }
        }

        #region Constructor

        public CommServer(int pPort)
        {
            try 
            {
                IPAddress ipAddress =
                    (from ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList
                     where ip.AddressFamily == AddressFamily.InterNetwork
                     select ip).First();
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, pPort);
                m_Server = new TcpListener(ipEndPoint);

                m_ListeningThread = new Thread(Listener);
                m_ListeningThread.Name = "TCP Listening ";
                m_ListeningThread.Start();
            }
            catch (Exception e)
            {
                Logger.Logger.Log("Failed to open server socket: {0}", e.Message);
            }
        }

        #endregion

        #region Public

        #region Events

        public event dlgClientConnected ClientConnected;

        #endregion

        #endregion

        #region Private

        #region Listener thread methods

        private void Listener()
        {
            try
            {
                m_Server.Start();

                while (true)
                {
                    TcpClient newClient = m_Server.AcceptTcpClient();
                    ClientConnected?.Invoke(new CommConnection(newClient, Id));
                }
            }
            catch (Exception e)
            {
                Logger.Logger.Log(e.Message);
                throw;
            }
            
        }

        #endregion

        #region Members

        private readonly TcpListener m_Server;
        private readonly Thread m_ListeningThread;

        #endregion

        #region Constants
        private const string Id = "FileServer";
        #endregion

        #endregion
    }
}
