using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Common.Net
{
    public enum SocketStatus { CONNECTED, NON_CONNECTED, NO_PORT, NO_DEST_ADDR }
    
    public struct AsyncClntArg
    {
        public Socket clntSocket;
        //public IPEndPoint ipEndPoint;

        public static AsyncClntArg makeArg(ref TcpClient client)
        {
            AsyncClntArg arg = new AsyncClntArg();
            arg.clntSocket = client.Client;
            //arg.ipEndPoint = new IPEndPoint(arg.clntSocket.)

            return arg;
        }

        public static AsyncClntArg makeArg(ref UdpClient client)
        {
            AsyncClntArg arg = new AsyncClntArg();
            arg.clntSocket = client.Client;
            //arg.ipEndPoint = new IPEndPoint(arg.clntSocket.)

            return arg;
        }

        public static void parse(IAsyncResult asynResult, out UdpClient client)
        {
            client = (UdpClient) asynResult.AsyncState;
        }

        public static void parse(IAsyncResult asynResult, out TcpClient client)
        {
            client = (TcpClient) asynResult.AsyncState;
        }
    };

    public class Client
    {
        public static SocketStatus ConnectToServer(out TcpClient client, IPEndPoint endPoint)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client = new TcpClient();
            client.Client = socket;

            try
            {
                socket.Connect(endPoint);
            }
            catch (Exception)
            {
                socket.Close();
                socket = null;
            }

            if (socket != null)
                return SocketStatus.CONNECTED;

            return SocketStatus.NON_CONNECTED;
        }

        public static SocketStatus ConnectToServer(out UdpClient client, Int32 port)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            /// C#에서는 UDP 소켓의 클라이언트 자신의 종점(End Point)를 설정해야 함
            socket.Bind(new IPEndPoint(IPAddress.Any, port));

            client = new UdpClient();
            client.Client = socket;

            return SocketStatus.CONNECTED;
        }
    }
}
