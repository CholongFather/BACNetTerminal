using System;

namespace BACnet
{
    internal class PacketWriteRequest
    {
        // public 속성
        public int InvokeID
        {
            get { return Convert.ToInt32(m_InvokeID); }
        }

        // public 메서드
        public byte[] GetPacket()
        {
            if ((m_Control & 0x20) != 0)
            {
                m_Length = (UInt16)(10 + 2 + 1 + m_DADDR.Length + 1 + 9 + m_Value.Length + 3);
            }
            else
            {
                m_Length = (UInt16)(10 + 9 + m_Value.Length + 3);
            }

            byte[] rtn = new byte[m_Length];

            rtn[0] = m_Type;
            rtn[1] = m_Function;
            rtn[2] = (byte)((m_Length & 0xff00) >> 8);
            rtn[3] = (byte)((m_Length & 0x00ff));
            rtn[4] = m_Version;

            int idxPoint;
            if ((m_Control & 0x20) == 0)
            {
                rtn[5] = m_Control;

                rtn[6] = m_PDUType;
                rtn[7] = m_MaxAPDUSize;
                rtn[8] = m_InvokeID;
                rtn[9] = m_ServiceChoice;

                idxPoint = 10;
            }
            else
            {
                rtn[5] = m_Control;
                rtn[6] = (byte)((m_DNET & 0xff00) >> 8);
                rtn[7] = (byte)((m_DNET & 0x00ff));
                rtn[8] = m_DLEN;
                for (int idx = 0; idx < m_DLEN; idx++)
                {
                    rtn[9 + idx] = m_DADDR[idx];
                }
                rtn[9 + m_DLEN] = m_HopCount;

                rtn[10 + m_DLEN] = m_PDUType;
                rtn[11 + m_DLEN] = m_MaxAPDUSize;
                rtn[12 + m_DLEN] = m_InvokeID;
                rtn[13 + m_DLEN] = m_ServiceChoice;

                idxPoint = 10 + 2 + 1 + m_DLEN + 1;
            }

            rtn[idxPoint + 0] = (byte)0x0c; // 0C SD Context Tag 0 (Object Identifer, L=4) 0b0000 1100
            rtn[idxPoint + 1] = (byte)((m_ObjectIdentifier & 0xff000000) >> (8 * 3)); //objid H
            rtn[idxPoint + 2] = (byte)((m_ObjectIdentifier & 0x00ff0000) >> (8 * 2)); //objid
            rtn[idxPoint + 3] = (byte)((m_ObjectIdentifier & 0x0000ff00) >> (8 * 1)); //objid
            rtn[idxPoint + 4] = (byte)((m_ObjectIdentifier & 0x000000ff) >> (8 * 0)); //objid L
            rtn[idxPoint + 5] = (byte)0x19; // 19 SD Context 1 (Property, L=1) 0b 0001 1001
            rtn[idxPoint + 6] = m_PropertyID;
            rtn[idxPoint + 7] = (byte)0x3E; // 3E Opening Context Tag 3 (List of Property Value) 0b 0011 1110
            idxPoint += 8;

            rtn[idxPoint] = m_DataType; // Application Tag 0x44 (Real, L=4), 0x91 (Enumerated, L=1), 0x22 (unsigned int, L=2), 0x21 (unsigned int, L=1)
            idxPoint++;

            foreach (byte nowByte in m_Value)
            {
                rtn[idxPoint] = nowByte;
                idxPoint++;
            }

            rtn[idxPoint + 0] = (byte)0x3f; // 3F Closing Context Tag 3 (List of Property Value) 0b 0011 1111
            rtn[idxPoint + 1] = (byte)0x49; // 49 SD Context 4 (Priority, L=1) 0b 0100 1001
            rtn[idxPoint + 2] = m_Priority; // Priority

            return rtn;
        }

        // 생성자

        public PacketWriteRequest(byte invokeID, UInt32 objectIdentifier, byte dataType, double value, byte priority)
        {
            this.m_InvokeID = invokeID;

            this.m_ObjectIdentifier = objectIdentifier;
            this.m_DataType = dataType;

            if (this.m_DataType == 0x91)
            {
                //(Enumerated, L=1)

                if (value == 0)
                {
                    this.m_Value = new byte[] { 0 };
                }
                else
                {
                    this.m_Value = new byte[] { 1 };
                }
            }
            else if (this.m_DataType == 0x44)
            {
                //(Real, L=4)
                byte[] nowData = BitConverter.GetBytes(Convert.ToSingle(value));
                byte[] bacnetData = new byte[] { nowData[3], nowData[2], nowData[1], nowData[0] };
                this.m_Value = bacnetData;
            }
            else if (this.m_DataType == 0x21)
            {
                //(unsigned int, L=1)
                this.m_Value = new byte[] { Convert.ToByte(value) };
            }
            else if (this.m_DataType == 0x22)
            {
                //(unsigned int, L=2)
                byte[] nowData = BitConverter.GetBytes(Convert.ToUInt16(value));
                byte[] bacnetData = new byte[] { nowData[1], nowData[0] };
                this.m_Value = bacnetData;
            }
            else if (this.m_DataType == 0x00)
            {
                //(Null)
                this.m_Value = new byte[0];
            }
            else
            {
                throw new BACnetPacketException("Application Tag 값이 예상하지 못한 0x" + this.m_DataType.ToString("X2") + " 입니다.");
            }

            m_Priority = priority;
        }

        public PacketWriteRequest(byte invokeID, UInt16 dnet, byte[] daddr, UInt32 objectIdentifier, byte dataType, double value, byte priority)
            : this(invokeID, objectIdentifier, dataType, value, priority)
        {
            m_Control = (byte)(0x04 | 0x20);
            m_DNET = dnet;
            m_DLEN = (byte)daddr.Length;
            m_DADDR = new byte[m_DLEN];
            Buffer.BlockCopy(daddr, 0, m_DADDR, 0, m_DADDR.Length);
        }

        // private 멤버변수

        // BVLLheader : Type(1byte), Function(1byte), Length(2byte:H->L)
        byte m_Type = (byte)0x81;   // Type(1byte) : 0x81 고정
        byte m_Function = (byte)0x0a;   // Function(1byte) : 0x0a Unicast, 0x0b broadcast
        UInt16 m_Length;    // Length(2byte:H->L)

        // BACnet NETheader : Version(1byte), Control(1byte)
        byte m_Version = (byte)0x01;    // Version(1byte) : 0x01 고정
        byte m_Control = (byte)0x04;    // Control(1byte) : 0x04 (if there is an answering message), 0x24 (if there is an answering message + Destination Specifier)

        UInt16 m_DNET;
        byte m_DLEN;
        byte[] m_DADDR;
        byte m_HopCount = 255;

        byte m_PDUType = (byte)0;   //00 PDU Type=0 (BACnet-Confirmed-Request-PDU, SEG=0, MOR=0, SA=0)  //30 PDU Type=3 (BACnet-ComplexACK-PDU, SEG=0, MOR=0)
        byte m_MaxAPDUSize = (byte)0x04;    //04 Maximum APDU Size Accepted=1024 octets

        byte m_InvokeID = (byte)0;
        byte m_ServiceChoice = (byte)0x0f; // 0x0f = 15 = WriteProperty - Request

        UInt32 m_ObjectIdentifier;

        byte m_PropertyID = (byte)0x55; // 0x55 = 85 = PRESENT_VALUE
        //byte m_PropertyID = (byte)0x1c; // 0x1c = 28 = Description

        byte m_DataType;

        byte[] m_Value;

        byte m_Priority;
    }
}
