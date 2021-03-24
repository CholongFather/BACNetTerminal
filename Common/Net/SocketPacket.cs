using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Common.Net
{
    public class SocketPacket
    {
        public static readonly long MAX_BUFFER_SIZE = 100000;

        public System.Net.Sockets.Socket m_currentSocket = null;
        public int m_clientNumber;

        // Buffer to store the data sent by the client
        public byte[] dataBuffer = new byte[MAX_BUFFER_SIZE];

        // Constructor which takes a Socket and a client number
        public SocketPacket(System.Net.Sockets.Socket socket, int clientNumber)
        {
             this.m_currentSocket = socket;
             this.m_clientNumber = clientNumber;
        }

        public void Close()
        {
            if (m_currentSocket != null)
            {
                m_currentSocket.Close();
                m_currentSocket = null;
            }
        }

        /// <summary>
        /// IAsyncResult를 SocketPacket으로 타입 캐스팅
        /// </summary>
        public static SocketPacket convertPacket(IAsyncResult asynResult)
        {
            return (SocketPacket)asynResult.AsyncState;
        }
    }
}
