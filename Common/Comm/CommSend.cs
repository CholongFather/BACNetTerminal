using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;

namespace Common
{   
    public class CommSend
    {
        public int HeaderLenght = 56;

        private byte[] m_Flag;                  // Msg Type 2:제어 명령
        private byte[] m_Seq;                   // 메시지 순번
        private byte[] m_Tagid;                 // Tag id
        private byte[] m_Value;                 // value
        private byte[] m_User;                  // user id
        private byte[] m_msg;                   // 성공 여부 0:fail 1:success
        private byte[] m_Kind;                  // 프로그램분류
        private byte[] m_Kind_Len;              // 프로그램명 길이
                
        //생성자
        public CommSend()
        {            
            m_Flag = new byte[4];
            m_Seq = new byte[4];
            m_Tagid = new byte[4];
            m_Value = new byte[8];
            m_msg = new byte[4];
            m_User = new byte[32];
            m_Kind = new byte[32];
            m_Kind_Len = new byte[4]; 
            
            for (int i = 0; i < this.m_Seq.Length; i++)
                this.m_Seq[i] = 0x00;

            for (int i = 0; i < this.m_msg.Length; i++)
                this.m_msg[i] = 0x00;

            this.HeaderLenght = m_Flag.Length + m_Seq.Length + m_Tagid.Length + m_Value.Length + m_User.Length + m_msg.Length + m_Kind.Length + m_Kind_Len.Length;

            headers = new byte[HeaderLenght];
        }

        public int Flag
        {
            set{m_Flag = BitConverter.GetBytes(value);}
            get{return BitConverter.ToInt32(m_Flag, 0);}
        }

        public int Seq
        {
            set { m_Seq = BitConverter.GetBytes(value); }
            get { return BitConverter.ToInt32(m_Seq, 0); }
        }

        public int Tagid
        {
            set { m_Tagid = BitConverter.GetBytes(value); }
            get { return BitConverter.ToInt32(m_Tagid, 0); }
        }

        public double Value
        {
            set { m_Value = BitConverter.GetBytes(value); }
            get { return BitConverter.ToDouble(m_Value, 0); }
        }

        public string User
        {   
            set { m_User = Encoding.Default.GetBytes(value.ToString()); }
            get { return Encoding.Default.GetString(m_User); }
        }

        public int Msg
        {
            set { m_msg = BitConverter.GetBytes(value); }
            get { return BitConverter.ToInt32(m_msg, 0); }
        }

        public string kind
        {
            set { m_Kind = Encoding.Default.GetBytes(value.ToString()); }
            get { return Encoding.Default.GetString(m_Kind); }
        }

        public int Kind_Lenth
        {
            set { m_Kind_Len = BitConverter.GetBytes(value); }
            get { return BitConverter.ToInt32(m_Kind_Len, 0); }
        }
                
        private byte[] headers;

        public byte[] Header()
        {
            ArrayList HeaderList = new ArrayList();
            HeaderList.Add(m_Flag);
            HeaderList.Add(m_Seq);
            HeaderList.Add(m_Tagid);
            HeaderList.Add(m_Value);
            HeaderList.Add(m_msg);
            HeaderList.Add(m_Kind_Len);
            HeaderList.Add(m_User);
            HeaderList.Add(m_Kind);
            
            
            //배열을 초기화함
            int HeaderCount = 0;
            int totalcount = 0;

            //루프를 돌면서 내용을 추가한다.
            foreach (object obj in HeaderList)
            {
                //Byte 타입인지 확인한다.
                Type type = obj.GetType();
                if (type.ToString() == "System.Byte")
                {
                    headers[totalcount] = (Byte)obj;
                    totalcount++;
                    continue;
                }
                else if (type.ToString() == "System.Byte[]")
                {
                    byte[] b = (byte[])obj;
                    int ObjLength = b.Length;

                    for (HeaderCount = 0; HeaderCount < ObjLength; HeaderCount++)
                    {
                        headers[totalcount] = b[HeaderCount];
                        totalcount++;
                    }
                }
            }
            return headers;
        }

        private AsyncCallback m_pfnCallBack;
        private Socket m_clientSocket;
        private IAsyncResult m_result;
        private bool m_Send = false;

        public bool SendData(CommSend comm, string sIP, string sPort, ref bool bRtn, ref int nTagid)
        {
            bool bret = false;
            if (ConnectServerSchedule(sIP, sPort))
            {
                try
                {
                    byte[] Sdata = new byte[256];
                    Sdata = comm.Header();

                    NetworkStream networkStream = new NetworkStream(m_clientSocket);
                    networkStream.Write(Sdata, 0, comm.HeaderLenght);
                    networkStream.Flush();
                    ReceiveData(ref bRtn, ref nTagid);
                    
                }
                catch (SocketException se)
                {
                    Log.log("Viewer : " + se.Message);
                    CloseClient();
                    return bret;
                }
            }
            else
            {                
                return bret;
            }

            return bret;
        }

        public bool SendData(CommSend comm, string sIP, string sPort)
        {
            bool bret = false;
            if (ConnectServer(sIP, sPort))
            {
                try
                {
                    byte[] Sdata = new byte[256];
                    Sdata = comm.Header();

                    NetworkStream networkStream = new NetworkStream(m_clientSocket);
                    networkStream.Write(Sdata, 0, comm.HeaderLenght);
                    networkStream.Flush();

                    bret = true;

                    DateTime tComtime = new DateTime();
                    tComtime = DateTime.Now;

                    while (m_Send == false)
                    {
                        int ret = DateTime.Compare(tComtime, DateTime.Now);
                        if (ret < -3)
                        {
                            // time out
                            bret = false;
                            break;
                        }
                    }

                    //CloseClient();
                }
                catch (SocketException se)
                {
                    Log.log("Viewer : " + se.Message);
                    CloseClient();
                    return bret;
                }
            }
            else
            {
                return bret;
            }

            return bret;
        }

        private bool ConnectServerSchedule(string szIp, string szPort)
        {
            try
            {
                m_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(szIp), Convert.ToInt16(szPort));

                m_clientSocket.Connect(ipEnd);

                if (m_clientSocket.Connected)
                {
                    //WaitForData();
                }
            }
            catch (SocketException se)
            {
                Log.log("Viewer : " + se.Message);
                return false;
            }

            return true;
        }

        private bool ConnectServer(string szIp, string szPort)
        {
            try
            {
                m_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(szIp), Convert.ToInt16(szPort));

                m_clientSocket.Connect(ipEnd);

                if (m_clientSocket.Connected)
                {
                    WaitForData();
                }
            }
            catch (SocketException se)
            {
                Log.log("Viewer : " + se.Message);
                return false;
            }

            return true;
        }

        private void CloseClient()
        {
            if (m_clientSocket != null)
            {
                m_clientSocket.Close();
                m_clientSocket = null;
            }
        }

        public class SocketPacket
        {
            public System.Net.Sockets.Socket thisSocket;
            public byte[] dataBuffer = new byte[8192];
        }

        public void ReceiveData(ref bool bRtn, ref int nTagid)
        {
            try
            {

                byte[] Packet = new byte[50];
                int nReceived = 0;

                if (m_clientSocket.Connected)
                {
                    nReceived = m_clientSocket.Receive(Packet);

                    if (Packet.Length == 0)
                    {
                        CloseClient();
                        bRtn = false;
                    }
                    byte[] value = new byte[32];

                    value[0] = Packet[0];
                    value[1] = Packet[1];
                    value[2] = Packet[2];
                    value[3] = Packet[3];
                    Flag = BitConverter.ToInt32(value, 0);

                    value[0] = Packet[4];
                    value[1] = Packet[5];
                    value[2] = Packet[6];
                    value[3] = Packet[7];
                    Seq = BitConverter.ToInt32(value, 0);

                    value[0] = Packet[8];
                    value[1] = Packet[9];
                    value[2] = Packet[10];
                    value[3] = Packet[11];
                    Tagid = BitConverter.ToInt32(value, 0);

                    nTagid = Tagid;

                    value[0] = Packet[12];
                    value[1] = Packet[13];
                    value[2] = Packet[14];
                    value[3] = Packet[15];
                    value[4] = Packet[16];
                    value[5] = Packet[17];
                    value[6] = Packet[18];
                    value[7] = Packet[19];
                    Value = BitConverter.ToDouble(value, 0);

                    value[0] = Packet[20];
                    value[1] = Packet[21];
                    value[2] = Packet[22];
                    value[3] = Packet[23];
                    Msg = BitConverter.ToInt32(value, 0);

                    if(Msg == 1)
                        bRtn = true;
                   
                }


            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\nReceiveData: Socket has been closed\n");
            }
            catch (SocketException se)
            {
                Log.log("Common : " + se.Message);
                //MessageBox.Show(se.Message);
            }

        }

        public void OnDataReceived(IAsyncResult asyn)
        {
            try
            {
                SocketPacket theSockId = (SocketPacket)asyn.AsyncState;
                int iRx = theSockId.thisSocket.EndReceive(asyn);

                byte[] value = new byte[32];

                value[0] = theSockId.dataBuffer[0];
                value[1] = theSockId.dataBuffer[1];
                value[2] = theSockId.dataBuffer[2];
                value[3] = theSockId.dataBuffer[3];
                Flag = BitConverter.ToInt32(value, 0);

                value[0] = theSockId.dataBuffer[4];
                value[1] = theSockId.dataBuffer[5];
                value[2] = theSockId.dataBuffer[6];
                value[3] = theSockId.dataBuffer[7];
                Seq = BitConverter.ToInt32(value, 0);

                value[0] = theSockId.dataBuffer[8];
                value[1] = theSockId.dataBuffer[9];
                value[2] = theSockId.dataBuffer[10];
                value[3] = theSockId.dataBuffer[11];
                Tagid = BitConverter.ToInt32(value, 0);

                value[0] = theSockId.dataBuffer[12];
                value[1] = theSockId.dataBuffer[13];
                value[2] = theSockId.dataBuffer[14];
                value[3] = theSockId.dataBuffer[15];
                value[4] = theSockId.dataBuffer[16];
                value[5] = theSockId.dataBuffer[17];
                value[6] = theSockId.dataBuffer[18];
                value[7] = theSockId.dataBuffer[19];
                Value = BitConverter.ToDouble(value, 0);

                value[0] = theSockId.dataBuffer[20];
                value[1] = theSockId.dataBuffer[21];
                value[2] = theSockId.dataBuffer[22];
                value[3] = theSockId.dataBuffer[23];
                Msg = BitConverter.ToInt32(value, 0);

                m_Send = true;

                Log.log("Viewer : Control ID:" + Tagid.ToString() + " val(" + Value.ToString("0.00") + ") ret:" + Msg.ToString());

            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\nOnDataReceived: Socket has been closed\n");
            }
            catch (SocketException se)
            {
                Log.log("Viewer : " + se.Message);
                //MessageBox.Show(se.Message);
            }

            m_Send = true;

        }

        public void WaitForData()
        {
            try
            {
                if (m_pfnCallBack == null)
                {
                    m_pfnCallBack = new AsyncCallback(OnDataReceived);
                }
                SocketPacket theSocPkt = new SocketPacket();
                theSocPkt.thisSocket = m_clientSocket;
                // Start listening to the data asynchronously
                m_result = m_clientSocket.BeginReceive(theSocPkt.dataBuffer,
                                                        0, theSocPkt.dataBuffer.Length,
                                                        SocketFlags.None,
                                                        m_pfnCallBack,
                                                        theSocPkt);
            }
            catch (SocketException se)
            {
                Log.log("Viewer : " + se.Message);
            }
        }
    }
}
