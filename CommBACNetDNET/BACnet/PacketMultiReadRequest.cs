using System;
using System.Collections.Generic;
using System.Text;

namespace BACnet
{
    internal class PacketMultiReadRequest
    {
        public const byte PROPERTY_PRESENT_VALUE = 0x55;
        public const byte PROPERTY_PRIORITY_ARRAY = 0x57;

        //public const int PACKET_MAX_SIZE = 1476;
        public const int PACKET_MAX_SIZE = 478;

        public const int RESPONSE_PACKET_BASE_SIZE = 30;

        public const int RESPONSE_PACKET_PRESENT_VALUE_SIZE = 16;

        public const int RESPONSE_PACKET_PRIORITY_ARRAY_SIZE = 27 + (4 * 16);   // 91

        // public 속성
        public int InvokeID
        {
            get { return Convert.ToInt32(m_InvokeID); }
        }

        // public 메서드
        public void AddObjectID(UInt32 objid)
        {
            m_QuePoint.Enqueue(new StructObjectIdAndProperty(objid, PROPERTY_PRIORITY_ARRAY));
        }

        public void AddObjectID(UInt32 objid, byte propertyid)
        {
            m_QuePoint.Enqueue(new StructObjectIdAndProperty(objid, propertyid));
        }

        /// <summary>
        /// 
        ///
        ///                                                함수
        ///                                                  |
        ///                                           패킷ID |
        ///                                               |  |  
        ///                           장치주소(여러개의 장치가 있을 때)
        ///           network ctl        |                |  |      
        ///       len      |             |                |  |
        ///        |       |             |                |  |
        /// # BACnet 장치에 하나의 IP가 있을 때 값 읽기
        /// 81 0A 0011 01 04                         0003 9B 0C 0C00000007 1955
        /// 
        /// # 동일한 IP로 여러개의 BACnet 장치가 있을 때 값 읽기                            
        /// 81 0A 001B 01 24 0002 06 000000001409 FF 0003 9B 0C 0C00000007 1955
        /// 
        /// # 동일한 IP로 여러개의 BACnet 장치가 있을 때 여러개 값 읽기
        /// 81 0A 002F 01 24 0002 06 000000001409 FF 0003 9E 0E 0C00000000 1E09551F 0C00000002 1E09551F 0C00000003 1E09551F
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] GetPacket()
        {
            if ((m_Control & 0x20) != 0)
            {
                m_Length = (UInt16)(10 + 2 + 1 + m_DADDR.Length + 1 + m_QuePoint.Count * 9);
            }
            else
            {
                m_Length = (UInt16)(10 + m_QuePoint.Count * 9);
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
                    rtn[9 + idx] = m_DADDR[idx];

                rtn[9 + m_DLEN] = m_HopCount;
                rtn[10 + m_DLEN] = m_PDUType;
                rtn[11 + m_DLEN] = m_MaxAPDUSize;
                rtn[12 + m_DLEN] = m_InvokeID;
                rtn[13 + m_DLEN] = m_ServiceChoice;

                idxPoint = 10 + 2 + 1 + m_DLEN + 1;
            }

            while (m_QuePoint.Count > 0)
            {
                StructObjectIdAndProperty nowPoint = m_QuePoint.Dequeue();

                rtn[idxPoint + 0] = (byte)0x0c; //0C SD Context Tag 0 (Object Identifer, L=4) 0b0000 1100
                rtn[idxPoint + 1] = (byte)((nowPoint.ObjectIdentifier & 0xff000000) >> (8 * 3)); //objid H
                rtn[idxPoint + 2] = (byte)((nowPoint.ObjectIdentifier & 0x00ff0000) >> (8 * 2)); //objid
                rtn[idxPoint + 3] = (byte)((nowPoint.ObjectIdentifier & 0x0000ff00) >> (8 * 1)); //objid
                rtn[idxPoint + 4] = (byte)((nowPoint.ObjectIdentifier & 0x000000ff) >> (8 * 0)); //objid L
                rtn[idxPoint + 5] = (byte)0x1e; //1E PD Opening Tag 1 (List Of Property References) 0b0000 1100
                rtn[idxPoint + 6] = (byte)0x09; //09 SD Context Tag 0 (Property Identifier, L=1)
                rtn[idxPoint + 7] = nowPoint.PropertyId;
                rtn[idxPoint + 8] = (byte)0x1f; //1F PD Closing Tag 1 (List Of Property References)

                idxPoint += 9;
            }

            return rtn;
        }

        // 생성자

        public PacketMultiReadRequest(byte invokeID)
        {
            m_InvokeID = invokeID;
        }

        public PacketMultiReadRequest(byte invokeID, UInt16 dnet, byte[] daddr)
        {
            m_InvokeID = invokeID;
            m_Control = (byte)(0x04 | 0x20);
            m_DNET = dnet;
            m_DLEN = (byte)daddr.Length;
            m_DADDR = new byte[m_DLEN];
            Buffer.BlockCopy(daddr, 0, m_DADDR, 0, m_DADDR.Length);
        }

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
        //byte m_MaxAPDUSize = (byte)0x04;    //04 Maximum APDU Size Accepted=1024 octets
        byte m_MaxAPDUSize = (byte)0x05;    //05 Maximum APDU Size Accepted=1476 octets

        byte m_InvokeID = (byte)0;
        byte m_ServiceChoice = (byte)0x0e; // 0x0e = 14 = ReadPropertyMultiple - Request

        Queue<StructObjectIdAndProperty> m_QuePoint = new Queue<StructObjectIdAndProperty>();
    }

    struct StructObjectIdAndProperty
    {
        // public 속성

        public UInt32 ObjectIdentifier
        {
            get { return this.m_ObjectIdentifier; }
        }

        public byte PropertyId
        {
            get { return this.m_PropertyId; }
        }

        // 생성자

        public StructObjectIdAndProperty(UInt32 objid, byte property)
        {
            this.m_ObjectIdentifier = objid;
            this.m_PropertyId = property;
        }

        // private 멤버변수

        private UInt32 m_ObjectIdentifier;

        private byte m_PropertyId;
    }
}
