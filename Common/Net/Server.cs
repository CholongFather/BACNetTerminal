using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common;
using Common.Net;

namespace Common.Net
{
    /// <summary>
    /// 클라이언트의 접속을 기다리는 클래스
    /// 클라이언트가 접속하면 SocketPacket 인스턴스를 인자로 IReceiveData.OnDataReceived() 메소드 호출
    /// </summary>
    public class Server
    {
        public static readonly int LISTEN_CLIENT = 4;
        public static readonly int RECEIVE_TIMEOUT = 1000 * 60; // 1 minute

        private readonly int port;
        private readonly ProtocolType protocolType;
        private AsyncCallback onReceivedData = ar => { /** default */ };

        private Socket m_mainSocket;

        private ArrayList m_workerSocketList = ArrayList.Synchronized(new ArrayList());

        private int m_clientCount = 0;

        /// <summary>
        /// 현재 실행기가 실행되는 서버의 IP를 가져온다.
        /// </summary>
        public static string GetLocalIP()
        {
            string sHostName = Dns.GetHostName();
            IPHostEntry iphostentry = Dns.GetHostEntry(sHostName);

            string sIP = "";
            foreach (IPAddress ipAddr in iphostentry.AddressList)
            {
                if (ipAddr.AddressFamily == AddressFamily.InterNetwork)
                {
                    sIP = ipAddr.ToString();
                    break;
                }
            }

            return sIP;
        }

        public Server(int port, ProtocolType type)
        {
            this.port = port;
            this.protocolType = type;
        }

        public Server(int port, ProtocolType type, AsyncCallback onReceiveData)
            : this(port, type)
        {
            this.onReceivedData = onReceiveData;
        }

        public bool StartServer()
        {
            try
            {
                m_mainSocket = new Socket(AddressFamily.InterNetwork,
                                            SocketType.Stream,
                                            this.protocolType);

                IPEndPoint ipLocal = new IPEndPoint(IPAddress.Any, this.port);

                m_mainSocket.Bind(ipLocal);

                // Start listening...
                m_mainSocket.Listen(LISTEN_CLIENT);

                m_mainSocket.ReceiveTimeout = RECEIVE_TIMEOUT;
                
                return true;
            }
            catch (SocketException se)
            {
                Log.log("[Scenario ACCE] [" + Convert.ToString(se.ErrorCode) + "] " + se.Message);

                if (m_mainSocket != null)
                {
                    m_mainSocket.Close();
                    m_mainSocket = null;
                }

                return false;
            }
        }

        public int WaitForClient(byte[] buffer)
        {
            Socket clntSock = m_mainSocket.Accept();

            return clntSock.Receive(buffer);
        }

        public void WaitForClientAsync()
        {       
            /// 클라이언트 기다리기
            m_mainSocket.BeginAccept(new AsyncCallback(OnClientConnect), m_mainSocket);
        }

        public void CloseSocket()
        {
            if (m_mainSocket != null)
            {
                m_mainSocket.Close();
            }

            Socket workerSocket = null;

            for (int i = 0; i < m_workerSocketList.Count; i++)
            {
                workerSocket = (Socket)m_workerSocketList[i];
                if (workerSocket != null)
                {
                    workerSocket.Close();
                    workerSocket = null;
                }
            }
        }

        /// <summary>
        /// This is the call back function, which will be invoked when a client is connected
        /// </summary>
        /// <param name="asyn"></param>
        private void OnClientConnect(IAsyncResult asyn)
        {
            try
            {
                Socket workerSocket = m_mainSocket.EndAccept(asyn);
                Interlocked.Increment(ref m_clientCount);
                m_workerSocketList.Add(workerSocket); //client가 접속할때마다 list추가
                WaitForData(workerSocket, m_clientCount);

                /// 클라이언트 기다리기
                m_mainSocket.BeginAccept(new AsyncCallback(OnClientConnect), m_mainSocket);
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\n OnClientConnection: Socket has been closed\n");
            }
            catch (SocketException se)
            {
                Log.log("[Scenario CONN] [" + Convert.ToString(se.ErrorCode) + "] " + se.Message);
            }
            catch (Exception ex)
            {
#if D_SCEN_MONITOR
                Log.log(ex.Message);
#endif
            }
        }

        /// <summary>
        /// Start waiting for data from the client
        /// </summary>
        /// <param name="soc"></param>
        /// <param name="clientNumber"></param>
        private void WaitForData(Socket soc, int clientNumber)
        {
            try
            {
                //if (pfnWorkerCallBack == null)
                //{
                //    pfnWorkerCallBack = new AsyncCallback(this.receiveData.OnDataReceived);
                //}
                
                SocketPacket theSocPkt = new SocketPacket(soc, clientNumber);

                soc.BeginReceive(theSocPkt.dataBuffer, 0,
                    theSocPkt.dataBuffer.Length,
                    SocketFlags.None,
                    /*pfnWorkerCallBack*/this.onReceivedData,
                    theSocPkt);
            }
            catch (SocketException se)
            {
                Log.log("[Scenario WAIT] [" + Convert.ToString(se.ErrorCode) + "] " + se.Message);
                //Log.log("AlarmServer " + se.Message);
            }
        }
    }
}
