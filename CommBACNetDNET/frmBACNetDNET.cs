using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using BACnet;
using Common;
using DAC;
using System.Text;

namespace CommBACNetDNET
{
    public partial class frmBACNetDNET : Form
    {
        // delegate

        private delegate void UpdateRichEditCallback(string text);

        private delegate void SelectIFNameEditCallback(string text);

        // private 상수

        private const int WM_COPYDATA = 0x4A;

        private const int SLEEP_TIME_BETWEEN_VIEWLOG = 500;

        // public static 멤버변수

        public static IntPtr g_handle;

        // private 멤버변수

        private volatile bool m_break = false;

        private TagValue m_TagValue = new TagValue();

        private TagList tblTagList = new TagList(Program.conSQL);
        
        private InterfaceList tblInterface = new InterfaceList(Program.conSQL);

        private BACnetClient m_Client;

        private Queue<string> m_ViewLogQueue = new Queue<string>(50);

        // 생성자

        public frmBACNetDNET()
        {
            InitializeComponent();
        }

        // protected ovrride 메서드

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_COPYDATA:
                    try
                    {
                        COPYDATASTRUCT cds = new COPYDATASTRUCT();
                        cds = (COPYDATASTRUCT)m.GetLParam(cds.GetType());

                        if (m_Client != null)
                        {
                            m_Client.AddWriteData(cds.lpData);
                        }
                    }
                    catch (Exception se)
                    {
                        //예외정보를 기록한다.
                        Log.log(Program.applicationName + " : " + se.Message);
                    }

                    break;
            }

            base.WndProc(ref m);
        }

        // 이벤트

        private void frmBACnet_Load(object sender, EventArgs e)
        {
            Thread viewLogThread = new Thread(new ThreadStart(RepeatDisplayViewLog));
            viewLogThread.Name="DisplayViewLog";
            viewLogThread.IsBackground = true;
            viewLogThread.Start();

            this.m_Client = new BACnetClient(AppendToRichEditControl, SetValueToSharedMem, null);

            g_handle = this.Handle;
            this.Text = Program.applicationName;

            Thread DbConnectManager = new Thread(new ThreadStart(ConnectDB));
            DbConnectManager.Name = "DbConnect";
            DbConnectManager.IsBackground = true;
            DbConnectManager.Start();
        }

        private void frmBACnet_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_Client != null)
            {
                m_Client.EndClient();
            }

            Log.log(" End");

            Application.Exit();
        }

        private void frmBACnet_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_break == false)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        /// <summary>키 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmBACnet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.X)
            {
                // alt + x 종료. m_bBreak

                if (MessageBox.Show("종료 하시겠습니까?", "MemTrClient", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    m_break = true;
                    this.Close();
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
            }
            else if (e.Alt & e.KeyCode == Keys.F4)
            {
                e.Handled = true;
            }
        }

        /// <summary>Hide 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        /// <summary>제어명령 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtControl_Click(object sender, EventArgs e)
        {
            if (m_Client != null)
            {
                byte priority = ClsWritePacketData.DEFAULT_WRITE_PRIORITY;
                double value;
                bool hasValue = double.TryParse(txtVal.Text, out value);

                try
                {
                    m_Client.AddWriteData(Convert.ToInt32(txtTagID.Text), priority, hasValue, value);
                }
                catch (Exception ex)
                {
                    AppendToRichEditControl(ex.Message);
                }
            }
        }

        // private 메서드

        private void ConnectDB()
        {
            Log.log("Connect DB...");

            // 처음 로딩시 DB정보를 가져와야 한다....
            for (; ; )
            {
                if (DBConnect())
                {
                    break;
                }

                Thread.Sleep(3000);
            }

            // 통신 시작
            m_Client.StartClient();
        }

        private bool DBConnect()
        {
            bool chk = true;
            bool rtn = false;

            if (Program.szServerName.Trim().Length < 1) chk = false;
            if (Program.szDBName.Trim().Length < 1) chk = false;
            if (Program.szUserID.Trim().Length < 1) chk = false;
            if (Program.szPassword.Trim().Length < 1) chk = false;

            if (chk)
            {
                Program.conSQL.ServerName = Program.szServerName;
                Program.conSQL.DataBaseName = Program.szDBName;
                Program.conSQL.UserName = Program.szUserID;
                Program.conSQL.UserPassword = Program.szPassword;
                Program.conSQL.ConnectionOpen();

                if (Program.conSQL.ServerState == ConnectionState.Open)
                {
                    rtn = true;

                    List<ClsBACnetDevice> listDevice = GetInterfaceList2();
                    foreach (ClsBACnetDevice nowDevice in listDevice)
                    {
                        LoadTagList2(nowDevice);
                        m_Client.AddDevice(nowDevice);
                    }
                    Program.conSQL.ConnectionClose();
                }
                else
                {
                    rtn = false;
                }
            }

            return rtn;
        }

        /// <summary>
        /// 인터페이스 정보를 읽음
        /// </summary>
        /// <returns></returns>
        private List<ClsBACnetDevice> GetInterfaceList2()
        {
            var ifid = Program.applicationName.Substring(Program.applicationName.Length - 3, 3);
            var nIfId = Convert.ToInt32(ifid);
            var dsInterface = GetDeviceDataFromInterfaceAndTag(nIfId.ToString());
            List<ClsBACnetDevice> rtn = null;
            rtn = new List<ClsBACnetDevice>();

            try
            {
                var idxDevice = 0;

                Log.log(string.Format("IF Device CNT [{0}]", dsInterface.Tables[0].Rows.Count));

                if (dsInterface.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow nowRow in dsInterface.Tables[0].Rows)
                    {
                        var nowIfId = String.Format("{0},{1},{2}", nowRow["If_Id"], nowRow["Tag_objid4"], nowRow["Tag_objid"]);
                        Log.log(string.Format("IF ID [{0}]", nowIfId));
                    
                        IPAddress nowIPAddress;
                        if (IPAddress.TryParse(nowRow["If_Ipaddress"].ToString(), out nowIPAddress) != true)
                        {
                            AppendToRichEditControl("ERROR : NO IPAddress : IfId= " + nowIfId + ", IPAddress= " + nowRow["If_Ipaddress"].ToString());
                            Log.log("ERROR : NO IPAddress : IfId= " + nowIfId + ", IPAddress= " + nowRow["If_Ipaddress"].ToString());
                            continue;
                        }
                    
                        var strDNET = nowRow["Tag_objid4"].ToString().Trim();
                        var strDADR = nowRow["Tag_objid"].ToString().Trim(); // 동일한 BACnet IP로 장치가 여러개 일 때 각 장치의 주소
                        if (String.IsNullOrEmpty(strDNET) != true & String.IsNullOrEmpty(strDADR) != true)
                        {
                            ushort nowNET;
                            if (ushort.TryParse(strDNET, out nowNET) != true)
                            {
                                AppendToRichEditControl("ERROR : NO DNET : IfId= " + nowIfId + ", Tag_objid4= " + strDNET);
                                Log.log("ERROR : NO DNET : IfId= " + nowIfId + ", Tag_objid4= " + strDNET);
                                continue;
                            }

                            var arrayStrADR = strDADR.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            var listADR = new List<byte>();

                            foreach (var nowStrADR in arrayStrADR)
                            {
                                try
                                {
                                    var adr = byte.Parse(nowStrADR, System.Globalization.NumberStyles.HexNumber);
                                    listADR.Add(adr);
                                }
                                catch
                                {
                                    listADR.Clear();
                                    break;
                                }
                            }

                            if (listADR.Count > 0)
                            {
                                var nowADR = listADR.ToArray();

                                ClsBACnetDevice nowDevice = new ClsBACnetDevice(idxDevice, nowIfId, nowIPAddress, nowNET, nowADR);
                                rtn.Add(nowDevice);
                                idxDevice++;
                            }
                            else
                            {
                                AppendToRichEditControl("ERROR : Wrong DADR String : IfId= " + nowIfId + ", If_Value2= " + strDADR);
                                Log.log("ERROR : Wrong DADR String : IfId= " + nowIfId + ", If_Value2= " + strDADR);
                                continue;
                            }
                            
                        }
                        else
                        {
                            ClsBACnetDevice nowDevice = new ClsBACnetDevice(idxDevice, nowIfId, nowIPAddress);
                            rtn.Add(nowDevice);
                            idxDevice++;
                        }
                    }
                }
                else
                {
                    AppendToRichEditControl("ERROR : NO INTERFACE " + ifid);
                    Log.log("ERROR : NO INTERFACE " + ifid);
                }

                string msgTItle = "단위 시스템(BACnet) : Device 갯수 " + idxDevice.ToString();
                if (InvokeRequired)
                {
                    lblTitle.BeginInvoke(new SelectIFNameEditCallback(SelectIFNameEdit), msgTItle);
                }
                else
                {
                    SelectIFNameEdit(msgTItle);
                }
            }
            catch (Exception ex)
            {
                Log.log(Program.applicationName + " : " + ex.Message);
            }

            return rtn;
        }

        /// <summary>
        /// 배열에 넣어준다
        /// </summary>
        /// <param name="_value"></param>
        /// <returns></returns>
        private byte[] getDADR(string _value)
        {
            byte[] bArray = new byte[6];
            
            if (_value.Length > 2)
            {
                for (int i = 0; i < 6; i++)
                {
                    Byte.TryParse(_value.Substring(i * 2, 2), out bArray[i]);
                }
            }
            else
            {
                Byte.TryParse(_value, out bArray[0]);
            }

            return bArray;
        }

        /// <summary>
        /// 각 장치의 주소를 얻음
        /// </summary>
        /// <param name="ifId"></param>
        /// <returns></returns>
        private DataSet GetDeviceDataFromInterfaceAndTag(string ifId)
        {
            string strSQL = string.Format(
                @"SELECT *
                FROM (
                SELECT DISTINCT 
	                I.If_Id, I.If_Ipaddress, 
	                T.Tag_objid4, T.Tag_objid
                FROM InterfaceList AS I
                JOIN TagList AS T
                ON I.If_Id = T.If_Id
                WHERE I.If_Id = '{0}'
                ) AS S
                ORDER BY LEN(Tag_objid4), Tag_objid4, LEN(Tag_objid), Tag_objid", ifId);

            DataSet rtn = null;
            try
            {
                rtn = Program.conSQL.ExecuteDataset(strSQL);
                rtn.Tables[0].TableName = "InterfaceList";
            }
            catch (Exception ex)
            {
                Log.log(ex);
            }

            if (rtn == null)
            {
                rtn = new DataSet();
                rtn.Tables.Add("InterfaceList");
            }

            return rtn;
        }

        
        private void LoadTagList2(ClsBACnetDevice device)
        {
            string[] arrayDeviceCode = device.IfId.Split(new char[] { ',' }, 3);
            if (arrayDeviceCode.Length < 3)
            {
                string[] newArrayCode = new string[3] { "", "", "" };
                for (int idx = 0; idx < Math.Min(arrayDeviceCode.Length, newArrayCode.Length); idx++)
                {
                    newArrayCode[idx] = arrayDeviceCode[idx] ?? "";
                }
                arrayDeviceCode = newArrayCode;
            }

            DataSet dsData = GetTagList2(arrayDeviceCode[0], arrayDeviceCode[1], arrayDeviceCode[2]);

            foreach (DataRow nowRow in dsData.Tables["TagList"].Rows)
            {
                int nowTagId = Convert.ToInt32(nowRow["Tag_Id"].ToString());

                int nowObjInstance;
                string strObjInstance = nowRow["Tag_objid2"].ToString();
                if (int.TryParse(strObjInstance, out nowObjInstance) != true)
                {
                    AppendToRichEditControl("ERROR (Obj Instance) : TagId= " + nowTagId.ToString() + ", Tag_objid2= " + strObjInstance);
                    continue;
                }

                string strTagType1 = nowRow["Tag_Type"].ToString();
                string strTagType2 = nowRow["Tag_objid3"].ToString();

                BACnetObjectType nowObjType;
                if (strTagType2 == "1")
                {
                    // Value 타입
                    switch (strTagType1)
                    {
                        case "01":
                        case "02":
                            nowObjType = BACnetObjectType.AV;
                            break;
                        case "03":
                        case "04":
                            nowObjType = BACnetObjectType.BV;
                            break;
                        case "07":
                        case "08":
                            nowObjType = BACnetObjectType.MSV;
                            break;
                        default:
                            AppendToRichEditControl("ERROR (Obj Type) : TagId= " + nowTagId.ToString() + ", Tag_Type= " + strTagType1);
                            continue;
                    }
                }
                else
                {
                    switch (strTagType1)
                    {
                        case "01":
                            nowObjType = BACnetObjectType.AI;
                            break;
                        case "02":
                            nowObjType = BACnetObjectType.AO;
                            break;
                        case "03":
                            nowObjType = BACnetObjectType.BI;
                            break;
                        case "04":
                            nowObjType = BACnetObjectType.BO;
                            break;
                        case "07":
                            nowObjType = BACnetObjectType.MSI;
                            break;
                        case "08":
                            nowObjType = BACnetObjectType.MSO;
                            break;
                        default:
                            AppendToRichEditControl("ERROR (Obj Type) : TagId= " + nowTagId.ToString() + ", Tag_Type= " + strTagType1);
                            continue;
                    }
                }

                ClsTagItem nowTagItem = new ClsTagItem(nowTagId, nowObjType, nowObjInstance, device);
                device.Items.Add(nowTagItem);
            }
        }

        private DataSet GetTagList2(string ifId, string net, string adr)
        {
            string strSQL =
                @"SELECT Tag_Id, Tag_Type, Tag_objid3, Tag_objid2
                FROM TagList
                WHERE If_Id = '" + ifId + @"' AND Tag_objid4 = '" + net + @"' AND Tag_objid = '" + adr + @"'
                ORDER BY LEN(Tag_Id), Tag_Id
                ";
            DataSet rtn = null;
            try
            {
                rtn = Program.conSQL.ExecuteDataset(strSQL);
                rtn.Tables[0].TableName = "TagList";
            }
            catch (Exception ex)
            {
                Log.log(ex);
            }

            if (rtn == null)
            {
                rtn = new DataSet();
                rtn.Tables.Add("TagList");
            }

            return rtn;
        }

        private void SelectIFNameEdit(string msg)
        {
            this.lblTitle.Text = msg;
        }

        private void SetValueToSharedMem(int tagId, double value)
        {
            bool isSuccess = m_TagValue.WriteMemByTagId(tagId, value);
            //AppendToRichEditControl("SetValue : Tag_Id= " + tagId.ToString() + ", Value= " + value.ToString("#,##0.###"));
        }

        // 로그

        private void RepeatDisplayViewLog()
        {
            while (this.m_break != true)
            {
                int msgCount = 0;
                
                try
                {
                    msgCount = m_ViewLogQueue.Count;
                }
                catch
                {
                }

                if (msgCount > 0)
                {
                    StringBuilder sbViewLog = new StringBuilder(160 * msgCount + 160);

                    try
                    {
                        for (int cnt = 0; cnt < msgCount; cnt++)
                        {
                            sbViewLog.Append(m_ViewLogQueue.Dequeue());
                        }
                    }
                    catch
                    {
                    }

                    try
                    {
                        if (sbViewLog.Length > 0)
                        {
                            if (this.InvokeRequired)
                            {
                                richTextBox.BeginInvoke(new UpdateRichEditCallback(OnUpdateRichEdit), sbViewLog.ToString());
                            }
                            else
                            {
                                // This is the main thread which created this control, hence update it
                                // directly 
                                OnUpdateRichEdit(sbViewLog.ToString());
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                Thread.Sleep(SLEEP_TIME_BETWEEN_VIEWLOG);
            }
        }

        private void AppendToRichEditControl(string msg)
        {
            string viewMsg = DateTime.Now.ToString("HH:mm:ss.fff") + " : " + msg + "\r\n";
            
            m_ViewLogQueue.Enqueue(viewMsg);
        }

        private void OnUpdateRichEdit(string msg)
        {
            if (richTextBox.Text.Length > 50000)
            {
                richTextBox.Clear();
            }

            richTextBox.AppendText(msg);
            richTextBox.ScrollToCaret();
        }

        // private static 메서드

        private static DataSet GetIfIdInfoXml(string XmlFile)
        {
            DataSet dsData = new DataSet();

            try
            {
                dsData.ReadXml(XmlFile);
            }
            catch (Exception ex)
            {
                Log.log(ex);
            }

            return dsData;
        }

        // 내부 구조체

        private struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }
    }
}