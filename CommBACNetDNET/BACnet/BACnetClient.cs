using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;

namespace BACnet
{
    internal class BACnetClient
    {
        // detegate

        internal delegate void SetValueDelegate(int tagId, double value);

        internal delegate void SetPriorityArrayDelegate(int tagId, double?[] values);

        internal delegate void UpdateRichEditCallback(string msg);

        // private 상수

        /// <summary><pre>최대 연속 Write 횟수.
        /// 남은 Write 필요 개수가 지정한 개수 이상일 때, 최대 연속 Write 횟수만큼 Write 한 후, 1회 Read 요청한다</pre>
        /// </summary>
        private const ushort MAX_CONTINOUS_WRITE_COUNT = 10;

        /// <summary><pre>Write 요청과 Read 요청 비율을 결정하는 남은 Write 필요 개수.
        /// 이 개수보다 많으면, 최대 연속 Write 횟수만큼 Write 한 후, Read 한번 한다.
        /// 이 개수보다 적으면, 연속 Write 횟수도 그 비율만큼 줄어든다</pre> 
        /// </summary>
        private const ushort NUMBER_DETERMINE_WRITE_READ_RATE = 100;

        private const string LOCAL_IP_ADDRESS = "0.0.0.0";

#if PORT_FIX
        private const int PORT_LOCAL = 47808;
#else
        private const int PORT_LOCAL = 0;
#endif

#if SLOW_COMM
        //private const int TIMEOUT_SECOND_WAIT_ACK_OF_WRITE = 750;  
        private const int TIMEOUT_SECOND_WAIT_ACK_OF_WRITE = 300;  

        //private const int MIN_SLEEPTIME_AFTER_REQUEST = 500;    
        private const int MIN_SLEEPTIME_AFTER_REQUEST = 200;    
        
        //private const int MILLISECOND_READ_CYCLE = 30 * 1000;    
        private const int MILLISECOND_READ_CYCLE = 10 * 1000;   
#else
        private const int TIMEOUT_SECOND_WAIT_ACK_OF_WRITE = 100;

        private const int MIN_SLEEPTIME_AFTER_REQUEST = 50;

        //private const int MILLISECOND_READ_CYCLE = 1000;
        private const int MILLISECOND_READ_CYCLE = 5 * 1000;
#endif

        // public 속성

        public bool IsWorking
        {
            get { return m_IsWorking; }
        }

        /// <summary>Request 후 Thread.Sleep 시간(밀리세컨드)
        /// </summary>
        [DefaultValue(MIN_SLEEPTIME_AFTER_REQUEST)]
        public int SleepTimeAfterRequest
        {
            get { return m_SleepTimeAfterRequest; }
            set
            {
                if (value < MIN_SLEEPTIME_AFTER_REQUEST)
                {
                    m_SleepTimeAfterRequest = MIN_SLEEPTIME_AFTER_REQUEST;
                }
                else
                {
                    m_SleepTimeAfterRequest = value;
                }
            }
        }

        // public 메서드

        public void AddDevice(ClsBACnetDevice device)
        {
            m_Devices.Add(device);
            foreach (ClsTagItem nowTagItem in device.Items)
            {
                m_Tags.Add(nowTagItem.TagId, nowTagItem);
            }
        }

        public void AddWriteData(string wndMsgData)
        {
            if (m_IsWorking != true)
            {
                AppendToRichEditControl("During the starting up or shutting down the program, write control dose not work.");
                return;
            }

            string[] arrayMsg = wndMsgData.Split(':');

            if (arrayMsg.Length < 2)
            {
                AppendToRichEditControl("ControlMessage Error = " + wndMsgData);
            }
            else
            {
                int tagId;
                if (int.TryParse(arrayMsg[0], out tagId) != true)
                {
                    AppendToRichEditControl("ControlMessage Error(tagId) = " + wndMsgData);
                }
                else if (arrayMsg.Length==2)
                {
                    double value;
                    if (double.TryParse(arrayMsg[1], out value) != true)
                    {
                        AppendToRichEditControl("ControlMessage Error(value) = " + wndMsgData);
                    }
                    else
                    {
                        AddWriteData(tagId, ClsWritePacketData.DEFAULT_WRITE_PRIORITY, true, value);
                    }
                }
                else if (arrayMsg.Length >= 3)
                {
                    // 우선권 및 명령 클리어 기능

                    double value;
                    bool hasValue = double.TryParse(arrayMsg[1], out value);

                    byte priority = 8;
                    byte.TryParse(arrayMsg[2], out priority);
                    if (priority < 1 | priority > 16)
                    {
                        AppendToRichEditControl("ControlMessage Error(priority) = " + wndMsgData);
                    }
                    else
                    {
                        AddWriteData(tagId, priority, hasValue, value);
                    }
                }
            }
        }

        public void AddWriteData(int tagId, byte priority, bool hasValue, double value)
        {
            Thread threadAddWriteData = new Thread(new ParameterizedThreadStart(AddWriteDataBackground));
            threadAddWriteData.IsBackground = true;
            threadAddWriteData.Start(new object[] { tagId, priority, hasValue, value });

            m_CommandDone.Set();
        }

        public void StartClient()
        {
            lock (m_SyncWorking)
            {
                m_IsExit = false;

                if (m_IsWorking != true)
                {
                    while (SetUdpClient() != true & m_IsExit != true)
                    {
                        Thread.Sleep(1000);
                    }

                    if (m_IsExit != true)
                    {
                        try
                        {
                            if (m_RequestThread != null && m_RequestThread.IsAlive == true)
                            {
                                m_RequestThread.Interrupt();
                                if (m_RequestThread.Join(100) != true)
                                {
                                    m_RequestThread.Abort();
                                }
                                AppendToRichEditControl("Request Thread is reseted.");
                            }
                        }
                        catch (Exception ex)
                        {
                            AppendToRichEditControl("Error (Request Thread Reset) : " + ex.Message);
                        }

                        try
                        {
                            if (m_ReceiveThread != null && m_ReceiveThread.IsAlive == true)
                            {
                                m_ReceiveThread.Interrupt();
                                if (m_ReceiveThread.Join(100) != true)
                                {
                                    m_ReceiveThread.Abort();
                                }
                                AppendToRichEditControl("Receive Thread is reseted.");
                            }
                        }
                        catch (Exception ex)
                        {
                            AppendToRichEditControl("Error (Receive Thread Reset) : " + ex.Message);
                        }

                        SetReadMultipleData();

                        m_ReceiveThread = new Thread(new ThreadStart(RepeatReceive));
                        m_ReceiveThread.IsBackground = true;
                        m_ReceiveThread.Start();

                        m_RequestThread = new Thread(new ThreadStart(RepeatRequest));
                        m_RequestThread.IsBackground = true;
                        m_RequestThread.Start();

                        m_IsWorking = true;
                    }
                }
                else
                {
                    AppendToRichEditControl("Work is already in progress.");
                }
            }
        }

        public void EndClient()
        {
            lock (m_SyncWorking)
            {
                this.m_IsExit = true;

                if (m_IsWorking)
                {
                    try
                    {
                        if (m_RequestThread.Join(100) != true)
                        {
                            m_RequestThread.Interrupt();

                            if (m_RequestThread.Join(100) != true)
                            {
                                m_RequestThread.Abort();
                            }
                        }
                        AppendToRichEditControl("Request EndClient");
                    }
                    catch (Exception ex)
                    {
                        AppendToRichEditControl("ERROR : " + ex.Message);
                    }

                    try
                    {
                        if (m_ReceiveThread.Join(100) != true)
                        {
                            m_ReceiveThread.Interrupt();

                            if (m_ReceiveThread.Join(100) != true)
                            {
                                m_ReceiveThread.Abort();
                            }
                        }
                        AppendToRichEditControl("Receive EndClient");
                    }
                    catch (Exception ex)
                    {
                        AppendToRichEditControl("ERROR : " + ex.Message);
                    }
                }

                SocketClose();
            }
        }

        // 생성자 

        public BACnetClient(UpdateRichEditCallback logMethod, SetValueDelegate setValueMethod, SetPriorityArrayDelegate setPriorityMethod)
        {
            if (logMethod == null)
            {
                m_AppendToRichEditControl = new UpdateRichEditCallback(delegate(string msg) { });
            }
            else
            {
                m_AppendToRichEditControl = new UpdateRichEditCallback(logMethod);
            }

            if (setValueMethod == null)
            {
                m_SetValue = new SetValueDelegate(delegate(int tagId, double value) { });
            }
            else
            {
                m_SetValue = new SetValueDelegate(setValueMethod);
            }

            if (setPriorityMethod == null)
            {
                m_SetPriorityArray = new SetPriorityArrayDelegate(delegate(int tagId, double?[] values) { });
            }
            else
            {
                m_SetPriorityArray = new SetPriorityArrayDelegate(setPriorityMethod);
            }

            m_Devices = new List<ClsBACnetDevice>();
            m_Tags = new Dictionary<int, ClsTagItem>();
            m_ClsReadData = new List<ClsMultiReadPacketData>();
            m_WritePackets = new List<ClsWritePacketData>();
            m_WaitACKWritePackets = new List<ClsWritePacketData>();

            this.m_CommandDone = new AutoResetEvent(false);

            m_SyncWorking = new object();
        }

        // private readonly 멤버변수

        private readonly UpdateRichEditCallback m_AppendToRichEditControl;

        private readonly SetValueDelegate m_SetValue;

        private readonly SetPriorityArrayDelegate m_SetPriorityArray;

        // private volatile 멤버변수

        private volatile byte m_LastWriteInvokeId = ClsWritePacketData.MAX_INVOKE_ID;

        private volatile bool m_IsExit = false;

        private volatile bool m_NeedRebind = false;

        // private 멤버변수

        private UdpClient m_Client;

        private bool m_IsWorking = false;

        private object m_SyncWorking;

        private Thread m_RequestThread;

        private Thread m_ReceiveThread;

        /// <summary>Request 후 Thread.Sleep 시간(밀리세컨드)
        /// </summary>
        private int m_SleepTimeAfterRequest = MIN_SLEEPTIME_AFTER_REQUEST;

        // 정보

        private readonly List<ClsBACnetDevice> m_Devices;

        private readonly Dictionary<int, ClsTagItem> m_Tags;

        // ReadPropertyMultiple

        private readonly List<ClsMultiReadPacketData> m_ClsReadData;

        // WriteProperty

        private readonly AutoResetEvent m_CommandDone;

        private readonly List<ClsWritePacketData> m_WritePackets;

        private readonly List<ClsWritePacketData> m_WaitACKWritePackets;

        // private 메서드

        private void SetReadMultipleData()
        {
            lock (m_ClsReadData)
            {
                m_ClsReadData.Clear();

                byte invokeId = ClsMultiReadPacketData.MAX_INVOKE_ID;

                foreach (ClsBACnetDevice nowDevice in m_Devices)
                {
                    // TODO : PROPERTY_PRIORITY_ARRAY 제외

                    //bool needPriorityArray = false;
                    int idxPoint = 0;
                    while (idxPoint< nowDevice.Items.Count)
                    {
                        invokeId = GetReadInvokeId(invokeId, 1);
                        ClsMultiReadPacketData nowPacketData = new ClsMultiReadPacketData(invokeId, nowDevice);

                        //if (needPriorityArray == true)
                        //{
                        //    try
                        //    {
                        //        nowPacketData.AddTag(nowDevice.Items[idxPoint], ClsMultiReadPacketData.PROPERTY_PRIORITY_ARRAY);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        AppendToRichEditControl("Error (SetReadMultipleData.1) : " + ex.Message);
                        //    }
                        //    idxPoint++;
                        //    needPriorityArray = false;
                        //}

                        while (idxPoint < nowDevice.Items.Count)
                        {
                            if (nowPacketData.AddTag(nowDevice.Items[idxPoint]) == true)
                            {
                                //if (nowDevice.Items[idxPoint].IsWriteable == true)
                                //{
                                //    if (nowPacketData.AddTag(nowDevice.Items[idxPoint], ClsMultiReadPacketData.PROPERTY_PRIORITY_ARRAY) == true)
                                //    {
                                //        idxPoint++;
                                //    }
                                //    else
                                //    {
                                //        needPriorityArray = true;
                                //        break;
                                //    }
                                //}
                                //else
                                {
                                    idxPoint++;
                                }
                            }
                            else
                            {
                                break;
                            }

                        }
                        nowPacketData.MakeRequestPacket();

                        m_ClsReadData.Add(nowPacketData);
                    }
                }
            }
        }

        private bool SetUdpClient()
        {
            if (m_Client == null || m_Client.Client == null)
            {
                bool rtn = true;

                UdpClient newClient = new UdpClient();
                newClient.ExclusiveAddressUse = false;
                newClient.EnableBroadcast = true;
                newClient.Ttl = 100;

                try
                {
                    newClient.Client.Bind(new IPEndPoint(IPAddress.Parse(LOCAL_IP_ADDRESS), PORT_LOCAL));
                }
                catch (ArgumentException ex)
                {
                    AppendToRichEditControl("ERROR (bind " + PORT_LOCAL.ToString() + ") : " + ex.Message);
                    rtn = false;
                }
                catch (SocketException ex)
                {
                    AppendToRichEditControl("ERROR (bind " + PORT_LOCAL.ToString() + ") : " + ex.Message);
                    rtn = false;
                }
                catch (ObjectDisposedException ex)
                {
                    AppendToRichEditControl("ERROR (bind " + PORT_LOCAL.ToString() + ") : " + ex.Message);
                    rtn = false;
                }
                catch (SecurityException ex)
                {
                    AppendToRichEditControl("ERROR (bind " + PORT_LOCAL.ToString() + ") : " + ex.Message);
                    rtn = false;
                }

                if (rtn)
                {
                    m_Client = newClient;
                    AppendToRichEditControl("UDP " + PORT_LOCAL.ToString() + " BIND!!");
                    return true;
                }
                else
                {
                    try
                    {
                        newClient.Close();
                    }
                    catch
                    {
                    }

                    return false;
                }
            }
            else
            {
                AppendToRichEditControl("To initialize the socket.");

                SocketClose();
                m_Client = null;

                return false;
            }
        }

        private void SocketClose()
        {
            try
            {
                UdpClient nowClient = m_Client;
                if (nowClient != null)
                {
                    nowClient.Close();
                    AppendToRichEditControl("Closed the socket");
                }
            }
            catch (SocketException iex)
            {
                AppendToRichEditControl("Failed to close the socket : " + iex.Message);
            }
        }

        private void AppendToRichEditControl(string msg)
        {
            try
            {
                m_AppendToRichEditControl(msg);
            }
            catch (Exception ex)
            {
                // 로그 중 예외
            }
        }

        private void SetValue(int tagId, double value)
        {
            try
            {
                m_SetValue(tagId, value);
            }
            catch (Exception ex)
            {
                AppendToRichEditControl("ERROR (SetValue) : " + ex.Message);
            }
        }

        private void SetPriorityArray(int tagId, double?[] values)
        {
            try
            {
                m_SetPriorityArray(tagId, values);
            }
            catch (Exception ex)
            {
                AppendToRichEditControl("ERROR (SetPriorityArray) : " + ex.Message);
            }
        }

        private void RepeatRequest()
        {
            int cntReadCycle = 0;

            int idxMultiReadRequest = 0;

            int cntWriteRequest = 0;

            bool isWaitNextReadTime = false;
            int readWaitMilisecond = 0;

            Stopwatch timeCheck = new Stopwatch();
            Stopwatch timeWaitCheck = new Stopwatch();
            timeCheck.Start();

            while (this.m_IsExit != true)
            {
                try
                {
                    if (m_NeedRebind)
                    {
                        while (SetUdpClient() != true & m_IsExit != true)
                        {
                            Thread.Sleep(1000);
                        }

                        if (m_IsExit == true)
                        {
                            break;
                        }
                        else
                        {
                            AppendToRichEditControl("Socket Re-bind !");
                            m_NeedRebind = false;
                        }
                    }
                    
                    // Request 처리

                    int cntWriteNeed = m_WritePackets.Count;

                    if ((isWaitNextReadTime == true & cntWriteNeed > 0) |
                        (cntWriteNeed > 0 & cntWriteRequest <= MAX_CONTINOUS_WRITE_COUNT & cntWriteRequest <= cntWriteNeed * MAX_CONTINOUS_WRITE_COUNT / NUMBER_DETERMINE_WRITE_READ_RATE))
                    {
                        cntWriteRequest++;
                        // Write Request 처리

                        #region 제어명령

                        ClsWritePacketData nowWriteData = null;
                        lock (m_WritePackets)
                        {
                            if (m_WritePackets.Count > 0)
                            {
                                nowWriteData = m_WritePackets[0];
                                m_WritePackets.RemoveAt(0);
                            }
                        }

                        if (nowWriteData != null)
                        {
                            m_NeedRebind = SendWritePacket(nowWriteData);

                            lock (m_WaitACKWritePackets)
                            {
                                m_WaitACKWritePackets.Add(nowWriteData);
                            }

                            Thread.Sleep(m_SleepTimeAfterRequest);
                        }
                        else
                        {
                            AppendToRichEditControl("Abnormal processing : Review the code to remove write data");
                        }

                        #endregion
                    }
                    else if (isWaitNextReadTime != true)
                    {
                        cntWriteRequest = 0;
                        // Read Request

                        if (idxMultiReadRequest < m_ClsReadData.Count)
                        {
                            #region 읽기 요청

                            if (idxMultiReadRequest == 0)
                            {
#if SLOW_COMM
                                if (cntReadCycle % 10 == 0)
                                {
                                    AppendToRichEditControl("Request : idx= 0, cnt= " + cntReadCycle.ToString("d3"));
                                }
#else
                                if (cntReadCycle % 50 == 0)
                                {
                                    AppendToRichEditControl("Request : idx= 0, cnt= " + cntReadCycle.ToString("d3"));
                                }
#endif
                                cntReadCycle = (cntReadCycle + 1) % 1000;
                            }

                            ClsMultiReadPacketData nowReadData = m_ClsReadData[idxMultiReadRequest];

                            m_NeedRebind = SendWritePacket(nowReadData);

                            idxMultiReadRequest = (idxMultiReadRequest + 1) % m_ClsReadData.Count;

                            Thread.Sleep(m_SleepTimeAfterRequest);

                            #endregion
                        }
                        else
                        {
                            AppendToRichEditControl("Abnormal processing : Not Read Point");
                            idxMultiReadRequest = 0;
                        }

                        if (idxMultiReadRequest == 0 & isWaitNextReadTime != true)
                        {
                            timeCheck.Stop();
                            long cycleTime = timeCheck.ElapsedMilliseconds;

                            if (cycleTime < MILLISECOND_READ_CYCLE)
                            {
                                readWaitMilisecond = Convert.ToInt32(MILLISECOND_READ_CYCLE - cycleTime);
                                isWaitNextReadTime = true;
                                timeWaitCheck.Reset();
                                timeWaitCheck.Start();
                            }
                        }
                    }

                    if (isWaitNextReadTime)
                    {
                        if (readWaitMilisecond > 0)
                        {
                            this.m_CommandDone.Reset();

                            if (m_WritePackets.Count == 0)
                            {
                                long waitTime = timeWaitCheck.ElapsedMilliseconds;
                                if (waitTime > int.MaxValue)
                                {
                                    readWaitMilisecond = 0;
                                }
                                else
                                {
                                    readWaitMilisecond -= Convert.ToInt32(waitTime);
                                }

                                if (readWaitMilisecond <= 0)
                                {
                                    readWaitMilisecond = 0;
                                    isWaitNextReadTime = false;
                                    timeWaitCheck.Stop();

                                    timeCheck.Reset();
                                    timeCheck.Start();
                                }
                                else
                                {
                                    if (this.m_CommandDone.WaitOne(readWaitMilisecond) != true)
                                    {
                                        readWaitMilisecond = 0;
                                        isWaitNextReadTime = false;
                                        timeWaitCheck.Stop();

                                        timeCheck.Reset();
                                        timeCheck.Start();
                                    }
                                }
                            }
                        }
                        else
                        {
                            readWaitMilisecond = 0;
                            isWaitNextReadTime = false;
                            timeWaitCheck.Stop();

                            timeCheck.Reset();
                            timeCheck.Start();
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppendToRichEditControl("ERROR(Request) : " + ex.Message);
                }

                Thread.Sleep(1);
            }
        }

        private void RepeatReceive()
        {
            while (this.m_IsExit != true)
            {
                while (m_Client == null | m_NeedRebind == true)
                {
                    // UdpClient 가 설정될 때까지 대기
                    Thread.Sleep(m_SleepTimeAfterRequest * 5);
                }

                // Receive 처리

                IPEndPoint remoteEP;
                byte[] packet;

                m_NeedRebind = ReceivePacket(out packet, out remoteEP);

                if (packet != null)
                {
                    try
                    {
                        int readIdx = 0;
                        while (readIdx < packet.Length)
                        {
                            int idxStartPacket = readIdx;

                            BACnetReceive data = null;
                            try
                            {
                                data = BACnetReceive.ReadPacket(packet, readIdx, out readIdx);
                            }
                            catch (BACnetPacketException ex)
                            {
                                // 잘못된 패킷
                                AppendToRichEditControl(ex.Message + "\r\n" + GetStringFromPacket(packet, idxStartPacket, packet.Length));
                                break;
                            }

                            if (data == null)
                            {
                                break;
                            }
                            else if (data.HasBVLCI & data.HasNPCI & data.IsInterestingAPDU & data.HasServiceChoice)
                            {
                                if (data.ServiceChoice == BACnetService.ReadPropertyMultiple)
                                {
                                    if (data.PDUType == BACnetPDUType.BACnetComplexACKPDU)
                                    {
                                        // 정상적인 응답
                                        AnalysisComplexACKOfReadMultiple(data, remoteEP);
                                    }
                                    else if (data.PDUType == BACnetPDUType.ErrorPDU)
                                    {
                                        // 전체 실패
                                        AnalysisShortADPUOfReadMultiple(data, remoteEP);
                                    }
                                    else if (data.PDUType == BACnetPDUType.BACnetSimpleACKPDU)
                                    {
                                        // 비정상 응답
                                        AnalysisShortADPUOfReadMultiple(data, remoteEP);
                                    }
                                    else
                                    {
                                        // 그외 PDU Type (디버깅을 위해)
                                        string strDevice = GetDeviceStringFromReceive(data, remoteEP);
                                        AppendToRichEditControl("Unexpected APDU : PDUType= " + data.PDUType.ToString() + ", " + strDevice + "\r\n" + GetStringFromPacket(packet, idxStartPacket, readIdx));
                                    }
                                }
                                else if (data.ServiceChoice == BACnetService.WriteProperty)
                                {
                                    if (data.PDUType == BACnetPDUType.BACnetComplexACKPDU)
                                    {
                                        // 비정상 응답
                                        string strDevice = GetDeviceStringFromReceive(data, remoteEP);
                                        AppendToRichEditControl("Unexpected Serviced ComplexACK : " + strDevice + ", Service= " + (data.ServiceChoice).ToString());
                                    }
                                    else if (data.PDUType == BACnetPDUType.ErrorPDU)
                                    {
                                        // Write 실패
                                        AnalysisShortADPUOfWrite(data, remoteEP);
                                    }
                                    else if (data.PDUType == BACnetPDUType.BACnetSimpleACKPDU)
                                    {
                                        // Write 성공
                                        AnalysisShortADPUOfWrite(data, remoteEP);
                                    }
                                    else
                                    {
                                        // 그외 PDU Type (디버깅을 위해)
                                        string strDevice = GetDeviceStringFromReceive(data, remoteEP);
                                        AppendToRichEditControl("Unexpected APDU : PDUType= " + data.PDUType.ToString() + ", " + strDevice + "\r\n" + GetStringFromPacket(packet, idxStartPacket, readIdx));
                                    }
                                }
                                else
                                {
                                    // 기대하지 않은 ServiceChoice
                                    string strDevice = GetDeviceStringFromReceive(data, remoteEP);
                                    AppendToRichEditControl("Unexpected ServiceChoice : " + strDevice + ", ServiceChoice= " + data.ServiceChoice.ToString() + "\r\n" + GetStringFromPacket(packet, idxStartPacket, readIdx));
                                }
                            }
                            else if (data.HasBVLCI & data.HasNPCI & data.IsInterestingAPDU & data.PDUType == BACnetPDUType.RejectPDU)
                            {
                                // Reject PDU
                                BACnetRejectPDU rejectPDU = (BACnetRejectPDU)data.APDU;

                                string strDevice = GetDeviceStringFromReceive(data, remoteEP);
                                AppendToRichEditControl("Reject PDU : " + strDevice + ", Reject Reason= " + ((BACnetRejectReason)rejectPDU.RejectReason).ToString());
                            }
                            else if (data.IsPacketError)
                            {
                                // 잘못된 패킷
                                AppendToRichEditControl("Wrong Packet : " + "\r\n" + GetStringFromPacket(packet));
                                break;
                            }
                            else if (data.HasBVLCI & data.IsUnicastNPDU & data.IsNetworkLayerMessage != true)
                            {
                                // UnicastNPDU 이고, 네트워크 계층 메시지도 아닌 패킷
                                AppendToRichEditControl("Unexpected Packet : " + "\r\n" + GetStringFromPacket(packet, idxStartPacket, readIdx));
                            }
                            else
                            {
                                // 관심 대상이 아닌 패킷
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // 예상하지 못한 예외(발생시 디버깅할 것)
                        AppendToRichEditControl("Unhandled ERROR : " + ex.Message + "\r\n" + GetStringFromPacket(packet));
                    }
                }

                Thread.Sleep(1);
            }
        }

        /// <summary>Request Packet Send(예외 발생시 로그 표시하고, 소켓 재설정 여부 반환)
        /// </summary>
        /// <param name="requestData">Request Packet의 데이터 클래스</param>
        /// <returns>UdpClient 다시 설정 필요 여부</returns>
        private bool SendWritePacket(IRequestPacketData requestData)
        {
            bool needRebind = false;

            try
            {
                requestData.Send(m_Client);
            }
            catch (ArgumentNullException ex)
            {
                AppendToRichEditControl("ERROR (ArgumentNullException) : Program debugging is required. : " + ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                AppendToRichEditControl("ERROR (ObjectDisposedException) : " + ex.Message);
                needRebind = true;
            }
            catch (InvalidOperationException ex)
            {
                AppendToRichEditControl("ERROR (InvalidOperationException) : Program debugging is required. : " + ex.Message);
            }
            catch (SocketException ex)
            {
                AppendToRichEditControl("ERROR (SocketException) : " + ex.Message);

                SocketClose();
                needRebind = true;
            }
            return needRebind;
        }

        /// <summary>Receive Packet(예외 발생시 로그 표시하고, 소켓 재설정 여부 반환)
        /// </summary>
        /// <param name="packet">받은 패킷 byte 배열</param>
        /// <param name="remoteEP">원격 IPEndPoint</param>
        /// <returns>UdpClient 다시 설정 필요 여부</returns>
        private bool ReceivePacket(out byte[] packet, out IPEndPoint remoteEP)
        {
            bool needRebind = false;

            remoteEP = new IPEndPoint(IPAddress.Any, PORT_LOCAL);
            packet = null;

            try
            {
                packet = m_Client.Receive(ref remoteEP);
            }
            catch (ObjectDisposedException ex)
            {
                AppendToRichEditControl("ERROR (ObjectDisposedException) : " + ex.Message);
                needRebind = true;
            }
            catch (SocketException ex)
            {
                AppendToRichEditControl("ERROR (SocketException) : " + ex.Message);

                if (ex.SocketErrorCode != SocketError.ConnectionReset)
                {
                    SocketClose();
                    needRebind = true;
                }
                else
                {
                    packet = null;
                }
            }

            return needRebind;
        }

        // TODO : 임시, property로 PRESENT_VALUE 대신, priority-array 를 쓰도록 수정
        private void AnalysisComplexACKOfReadMultiple(BACnetReceive data, IPEndPoint remoteEP)
        {
            ClsMultiReadPacketData nowReadPacketData = ClsMultiReadPacketData.FindReadPacketData(m_ClsReadData, remoteEP, data);

            string strDevice = GetDeviceStringFromReceive(data, remoteEP);

            if (nowReadPacketData == null)
            {
                // 일치하는 Request 데이터 없음

                StringBuilder sbObjects = new StringBuilder(data.Results.Length * 10);
                foreach (BACnetReadAccessResult nowResult in data.Results)
                {
                    sbObjects.Append(nowResult.ObjectIdentifier);
                    sbObjects.Append(", ");
                }

                AppendToRichEditControl("Request Not Found (ReadPropertyMultiple) : " + strDevice + ", Objects= " + sbObjects.ToString());
            }
            else
            {
                // 정보용 // 임시 // 에러메시지 아님
                //AppendToRichEditControl("Complex-ACK-PDU - ReadPropertyMultiple : " + strDevice +", ObjectCount= " + data.Results.Length.ToString());

                StringBuilder sbError = new StringBuilder(50);
                bool hasError = false;

                for (int idxObject = 0; idxObject < data.Results.Length; idxObject++)
                {
                    BACnetReadAccessResult nowResult = data.Results[idxObject];
                    ClsTagItem nowTagItem = nowReadPacketData.Items[idxObject].TagItem;

                    if (nowResult.HasError)
                    {
                        sbError.AppendLine();
                        sbError.Append("    ERROR : ");
                        sbError.Append(", ObjectIdentifier= " + nowResult.ObjectIdentifier.ToString());
                        sbError.Append(", Error= " + ((BACnetErrorClass)nowResult.ErrorClass).ToString());
                        sbError.Append(", " + ((BACnetErrorCode)nowResult.ErrorCode).ToString());

                    }
                    else if (nowResult.PropertyID == 0x55)
                    {
                        double adjustValue;
                        adjustValue = Convert.ToDouble(nowResult.Value) * nowTagItem.GAIN + nowTagItem.BIAS;

                        // 값 입력
                        SetValue(nowTagItem.TagId, adjustValue);
                    }
                    else if (nowResult.PropertyID == 0x57)
                    {
                        // 0x57 = 87 = priority-array

                        double?[] adjustValue = new double?[nowResult.Values.Length];
                        for (int idxValues = 0; idxValues < nowResult.Values.Length; idxValues++)
                        {
                            if (nowResult.Values[idxValues] != null)
                            {
                                adjustValue[idxValues] = Convert.ToDouble((double)nowResult.Values[idxValues]) * nowTagItem.GAIN + nowTagItem.BIAS;
                            }
                        }

                        // 값 입력
                        SetPriorityArray(nowTagItem.TagId, adjustValue);
                    }
                    else
                    {
                        // unhandled PropertyID
                    }
                }

                if (hasError)
                {
                    AppendToRichEditControl("Complex-ACK-PDU - Error (ReadPropertyMultiple) : " + strDevice + sbError.ToString());
                }
            }
        }

        private void AnalysisShortADPUOfReadMultiple(BACnetReceive data, IPEndPoint remoteEP)
        {
            byte invokeId = data.APDU.InvokeId;

            List<ClsMultiReadPacketData> listGuess = new List<ClsMultiReadPacketData>(5);

            for (int idxRequest = invokeId; idxRequest < m_ClsReadData.Count; idxRequest += (ClsMultiReadPacketData.MAX_INVOKE_ID + 1))
            {
                ClsMultiReadPacketData nowData = m_ClsReadData[idxRequest];

                if (nowData.Device.CheckDevice(remoteEP, data.NPCI.HasSNET, data.NPCI.SNET, data.NPCI.SADR))
                {
                    listGuess.Add(nowData);
                }
            }

            StringBuilder sbRequest = new StringBuilder(100);
            foreach (ClsMultiReadPacketData nowData in listGuess)
            {
                sbRequest.Append("Objects= ");
                foreach (ClsRequestItemData nowTagItem in nowData.Items)
                {
                    sbRequest.Append(nowTagItem.ObjectIdentifier);
                    sbRequest.Append(", ");
                }
                sbRequest.AppendLine();
            }

            if (data.PDUType == BACnetPDUType.ErrorPDU)
            {
                BACnetErrorPDU nowErrorPDU = (BACnetErrorPDU)data.APDU;

                string strDevice = GetDeviceStringFromReceive(data, remoteEP);
                string strError = ", Error= " + ((BACnetErrorClass)nowErrorPDU.ErrorClass).ToString() + ", " + ((BACnetErrorCode)nowErrorPDU.ErrorCode).ToString();
                AppendToRichEditControl("ErrorPDU (ReadPropertyMultiple) : " + strDevice + strError + "\r\nOne of a list of: \r\n" + sbRequest.ToString());
            }
            else if (data.PDUType == BACnetPDUType.BACnetSimpleACKPDU)
            {
                BACnetSimpleACK nowSimplePDU = (BACnetSimpleACK)data.APDU;

                string strDevice = GetDeviceStringFromReceive(data, remoteEP);
                AppendToRichEditControl("Unexpected SimpleACK (ReadPropertyMultiple) : " + strDevice + "\r\nOne of a list of: \r\n" + sbRequest.ToString());
            }
        }

        private void AnalysisShortADPUOfWrite(BACnetReceive data, IPEndPoint remoteEP)
        {
            byte invokeId = data.APDU.InvokeId;

            List<ClsWritePacketData> listGuess = new List<ClsWritePacketData>(5);
            List<ClsWritePacketData> listRemoveTimeOut = new List<ClsWritePacketData>(5);
            ClsWritePacketData bestGuess = null;
            ClsWritePacketData anotherGuess = null;

            DateTime nowTime = DateTime.Now;
            DateTime removeTime = nowTime.AddSeconds(-TIMEOUT_SECOND_WAIT_ACK_OF_WRITE);
            lock (m_WaitACKWritePackets)
            {
                foreach (ClsWritePacketData nowData in m_WaitACKWritePackets)
                {
                    if (nowData.CheckReceiceData(invokeId, remoteEP, data.NPCI.HasSNET, data.NPCI.SNET, data.NPCI.SADR))
                    {
                        listGuess.Add(nowData);

                        if (bestGuess == null)
                        {
                            if (nowData.RequestTime > removeTime)
                            {
                                bestGuess = nowData;
                            }
                            else if (anotherGuess == null || nowData.RequestTime > anotherGuess.RequestTime)
                            {
                                anotherGuess = nowData;
                            }
                        }
                        else if (nowData.RequestTime > removeTime & nowData.RequestTime < bestGuess.RequestTime)
                        {
                            bestGuess = nowData;
                        }
                    }

                    if (nowData.RequestTime <= removeTime)
                    {
                        listRemoveTimeOut.Add(nowData);
                    }
                }

                foreach (ClsWritePacketData removeItem in listRemoveTimeOut)
                {
                    m_WaitACKWritePackets.Remove(removeItem);
                }

                if (bestGuess != null)
                {
                    m_WaitACKWritePackets.Remove(bestGuess);
                }
            }

            if (listRemoveTimeOut.Count > 0 & anotherGuess == null)
            {
                AppendToRichEditControl("Timeout(" + TIMEOUT_SECOND_WAIT_ACK_OF_WRITE + ") Wait ACK : " + listRemoveTimeOut.Count.ToString() + " packets");
            }
            else if (listRemoveTimeOut.Count > 1 & anotherGuess != null)
            {
                int deleyedSecond = Convert.ToInt32((anotherGuess.RequestTime - nowTime).TotalSeconds);
                AppendToRichEditControl("Timeout(" + TIMEOUT_SECOND_WAIT_ACK_OF_WRITE + ") Wait ACK : " + (listRemoveTimeOut.Count - 1).ToString() + " packets remove, And Response is deleyed for " + deleyedSecond.ToString() + "seconds");
            }
            else if (listRemoveTimeOut.Count == 1 & anotherGuess != null)
            {
                // 요청에 대한 응답이 일정 시간 경과 후 도착한 듯함 
                int deleyedSecond = Convert.ToInt32((anotherGuess.RequestTime - nowTime).TotalSeconds);
                AppendToRichEditControl("Response is deleyed for " + deleyedSecond.ToString() + "seconds");
            }

            StringBuilder sbRequest = new StringBuilder(listGuess.Count * 40 + 20);
            if (listGuess.Count == 0)
            {
                sbRequest.Append("Not Found Matching Request Packets");
            }
            else if (listGuess.Count == 1)
            {
                ClsWritePacketData nowData = listGuess[0];
                if (nowData.ReorderCount > 0)
                {
                    sbRequest.Append("FirstOrderTime= ");
                    sbRequest.AppendFormat("{0:mm:ss.fff}", nowData.FirstOrderTime);
                    sbRequest.Append(", ReorderCount= ");
                    sbRequest.Append(nowData.ReorderCount);
                    sbRequest.Append(", OrderTime= ");
                }
                else
                {
                    sbRequest.Append("OrderTime= ");
                }
                sbRequest.AppendFormat("{0:mm:ss.fff}", nowData.OrderTime);
                sbRequest.Append(", RequestTime= ");
                sbRequest.AppendFormat("{0:mm:ss.fff}", nowData.RequestTime);
                sbRequest.Append(", Object= ");
                sbRequest.Append(nowData.Item.ObjectIdentifier);
                
                if (nowData.HasValue)
                {
                    sbRequest.Append(", Value= ");
                    sbRequest.Append(nowData.Value);
                    sbRequest.Append(", Priority= ");
                    sbRequest.Append(nowData.Priority);
                }
                else
                {
                    sbRequest.Append(", Value= Null, Priority= ");
                    sbRequest.Append(nowData.Priority);
                }
            }
            else if (listGuess.Count > 1)
            {
                sbRequest.Append("One of a list of: ");
                foreach (ClsWritePacketData nowData in listGuess)
                {
                    sbRequest.AppendLine();
                    if (nowData.ReorderCount > 0)
                    {
                        sbRequest.Append("FirstOrderTime= ");
                        sbRequest.AppendFormat("{0:mm:ss.fff}", nowData.FirstOrderTime);
                        sbRequest.Append(", ReorderCount= ");
                        sbRequest.Append(nowData.ReorderCount);
                        sbRequest.Append(", OrderTime= ");
                    }
                    else
                    {
                        sbRequest.Append("OrderTime= ");
                    }
                    sbRequest.AppendFormat("{0:mm:ss.fff}", nowData.OrderTime);
                    sbRequest.Append(", RequestTime= ");
                    sbRequest.AppendFormat("{0:mm:ss.fff}", nowData.RequestTime);
                    sbRequest.Append(", Object= ");
                    sbRequest.Append(nowData.Item.ObjectIdentifier);

                    if (nowData.HasValue)
                    {
                        sbRequest.Append(", Value= ");
                        sbRequest.Append(nowData.Value);
                        sbRequest.Append(", Priority= ");
                        sbRequest.Append(nowData.Priority);
                    }
                    else
                    {
                        sbRequest.Append(", Value= Null, Priority= ");
                        sbRequest.Append(nowData.Priority);
                    }
                }
            }

            if (data.PDUType == BACnetPDUType.ErrorPDU)
            {
                BACnetErrorPDU nowErrorPDU = (BACnetErrorPDU)data.APDU;

                string strDevice = GetDeviceStringFromReceive(data, remoteEP);
                string strError = ", Error= " + ((BACnetErrorClass)nowErrorPDU.ErrorClass).ToString() + ", " + ((BACnetErrorCode)nowErrorPDU.ErrorCode).ToString();
                AppendToRichEditControl("ErrorPDU (WriteProperty) : " + strDevice + strError + ", " + sbRequest.ToString());
            }
            else if (data.PDUType == BACnetPDUType.BACnetSimpleACKPDU)
            {
                BACnetSimpleACK nowSimplePDU = (BACnetSimpleACK)data.APDU;

                string strDevice = GetDeviceStringFromReceive(data, remoteEP);
                AppendToRichEditControl("Successful SimpleACK (WriteProperty) : " + strDevice + ", " + sbRequest.ToString());
            }
        }

        /// <summary>AddWriteData 스레드 용 메서드
        /// </summary>
        /// <param name="cnt">new object[] { tagId, priority, hasValue, value }</param>
        private void AddWriteDataBackground(object obj)
        {
            int tagId;
            byte priority;
            bool hasValue;
            double value;

            try
            {
                object[] arrayObj = (object[])obj;
                tagId = (int)arrayObj[0];
                priority = (byte)arrayObj[1];
                hasValue = (bool)arrayObj[2];
                value = (double)arrayObj[3];
            }
            catch (Exception ex)
            {
                AppendToRichEditControl("CHECK CALLER !!! (AddWriteDataBackground) : " + ex.Message);
                return;
            }

            if (m_IsWorking != true)
            {
                AppendToRichEditControl("During the starting up or shutting down the program, write control dose not work.");
                return;
            }

            if (m_Tags.ContainsKey(tagId))
            {
                ClsTagItem nowTagItem = m_Tags[tagId];

                if (nowTagItem.IsWriteable)
                {
                    lock (m_WritePackets)
                    {
                        byte nowInvokeId = GetWriteInvokeId(this.m_LastWriteInvokeId, 1);
                        m_LastWriteInvokeId = nowInvokeId;

                        int idxOld = -1;
                        ClsWritePacketData oldWriteData = null;
                        for (int idx = 0; idx < m_WritePackets.Count; idx++)
                        {
                            if (m_WritePackets[idx].TagId == tagId & m_WritePackets[idx].Priority == priority)
                            {
                                idxOld = idx;
                                oldWriteData = m_WritePackets[idx];
                                break;
                            }
                        }

                        if (idxOld < 0)
                        {
                            ClsWritePacketData nowWriteData = new ClsWritePacketData(nowInvokeId, nowTagItem, priority, hasValue, value);
                            m_WritePackets.Add(nowWriteData);
                        }
                        else
                        {
                            ClsWritePacketData nowWriteData = new ClsWritePacketData(nowInvokeId, oldWriteData, hasValue, value);
                            m_WritePackets.RemoveAt(idxOld);
                            m_WritePackets.Insert(idxOld, nowWriteData);
                        }
                    }
                }
                else
                {
                    AppendToRichEditControl("Tag ObjectType Not Writable = " + tagId.ToString() + ", " + nowTagItem.ObjectType.ToString());
                }
            }
            else
            {
                AppendToRichEditControl("Not Found Tagid = " + tagId.ToString());
            }

        }

        // private static 메서드

        private static byte GetReadInvokeId(int idx)
        {
            int value = idx % ((int)ClsMultiReadPacketData.MAX_INVOKE_ID + 1);
            if (value < 0)
            {
                value += ((int)ClsMultiReadPacketData.MAX_INVOKE_ID + 1);
            }

            return Convert.ToByte(value);
        }

        private static byte GetReadInvokeId(byte invokeId, byte addValue)
        {
            return GetReadInvokeId(Convert.ToInt32(invokeId) + Convert.ToInt32(addValue));
        }

        private static byte GetWriteInvokeId(byte invokeId, byte addValue)
        {
            int value = Convert.ToInt32(invokeId) + Convert.ToInt32(addValue);
            value %= ((int)ClsWritePacketData.MAX_INVOKE_ID + 1);

            return Convert.ToByte(value);
        }

        private static string GetDeviceString(byte invokeId, ClsBACnetDevice device)
        {
            string viewMsg = "Id= " + invokeId.ToString();
            viewMsg += ", Dev= " + device.RemoteEP.Address.ToString();
            if (device.HasNET)
            {
                viewMsg += ", SNET= " + device.NET.ToString() + ", SLEN= " + device.LEN.ToString();
                if (device.ADR.Length > 0)
                {
                    viewMsg += ", SADR= 0x";
                }
                foreach (byte nowAdr in device.ADR)
                {
                    viewMsg += nowAdr.ToString("X2");
                }
            }

            return viewMsg;
        }

        private static string GetDeviceStringFromReceive(BACnetReceive data, IPEndPoint remoteEP)
        {
            string viewMsg = "Id= " + data.APDU.InvokeId.ToString();
            viewMsg += ", Dev= " + remoteEP.Address.ToString();
            if (data.NPCI.HasSNET)
            {
                viewMsg += ", SNET= " + data.NPCI.SNET.ToString() + ", SLEN= " + data.NPCI.SLEN.ToString();
                if (data.NPCI.SADR.Length > 0)
                {
                    viewMsg += ", SADR= 0x";
                }
                foreach (byte nowAdr in data.NPCI.SADR)
                {
                    viewMsg += nowAdr.ToString("X2");
                }
            }

            return viewMsg;
        }

        private static string GetStringFromPacket(byte[] packet)
        {
            return GetStringFromPacket(packet, 0, packet.Length);
        }

        private static string GetStringFromPacket(byte[] packet, int startIdx, int nextIdx)
        {
            StringBuilder sb = new StringBuilder(Math.Max(nextIdx - startIdx, 10) * 3);

            for (int idx = startIdx; idx < nextIdx; idx++)
            {
                sb.AppendFormat("{0:X2} ", packet[idx]);
            }

            return sb.ToString();
        }
    }
}