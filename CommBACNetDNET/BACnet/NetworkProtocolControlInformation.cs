using System;

namespace BACnet
{
    internal class NetworkProtocolControlInformation
    {
        // public 속성

        public bool HasDNET
        {
            get { return this.m_HasDNET; }
        }

        public bool HasSNET
        {
            get { return this.m_HasSNET; }
        }

        // public BACnet 속성

        /// <summary>Verseron (Always 0x01)
        /// </summary>
        public byte Version
        {
            get { return m_Version; }
        }

        /// <summary><pre>NPCI Control Octet.
        /// Bit 7 (& 0x80) : Network Layer Message Or APDU, 
        /// Bit 6 : Reserved,
        /// Bit 5 (& 0x20) : Destination Specifier,
        /// Bit 4 : Reserved,
        /// Bit 3 (& 0x08) : Source Specifier,
        /// Bit 2 (& 0x04) : Expecting reply
        /// Bit 1, 2 (& 0x03) : Priority(11=LifeSatety, 10=CriticalEquip, 01=Urgent, 00=Normal)
        /// </pre>
        /// </summary>
        public byte Control
        {
            get { return m_Control; }
        }

        public UInt16 DNET
        {
            get { return m_DNET; }
        }

        public byte DLEN
        {
            get { return m_DLEN; }
        }

        public byte[] DADR
        {
            get
            {
                if (m_DADR == null)
                {
                    return new byte[0];
                }
                else
                {
                    byte[] rtn = new byte[m_DADR.Length];
                    Array.Copy(m_DADR, rtn, m_DADR.Length);

                    return rtn;
                }
            }
        }

        public UInt16 SNET
        {
            get { return m_SNET; }
        }

        public byte SLEN
        {
            get { return m_SLEN; }
        }

        public byte[] SADR
        {
            get
            {
                if (m_SADR == null)
                {
                    return new byte[0];
                }
                else
                {
                    byte[] rtn = new byte[m_SADR.Length];
                    Array.Copy(m_SADR, rtn, m_SADR.Length);

                    return rtn;
                }
            }
        }

        public byte HopCount
        {
            get { return m_HopCount; }
        }

        // 생성자

        public NetworkProtocolControlInformation(byte[] packet, int startIdx, out int nextIdx)
        {
            int idx = startIdx;

            try
            {
                this.m_Version = packet[idx];
                idx++;

                this.m_Control = packet[idx];
                idx++;

                if ((this.m_Control & 0x20) > 0)
                {
                    this.m_HasDNET = true;
                }

                if ((this.m_Control & 0x08) > 0)
                {
                    this.m_HasSNET = true;
                }

                if (this.m_HasDNET)
                {
                    //DNET DLEN DADR present
                    this.m_DNET = Convert.ToUInt16((int)packet[idx] * 0x0100 + (int)packet[idx + 1]);
                    idx += 2;

                    this.m_DLEN = packet[idx];
                    idx++;

                    this.m_DADR = new byte[this.m_DLEN];
                    for (int addrIdx = 0; addrIdx < this.m_DLEN; addrIdx++)
                    {
                        this.m_DADR[addrIdx] = packet[idx + addrIdx];
                    }
                    idx += this.m_DLEN;
                }

                if (this.m_HasSNET)
                {
                    //SNET SLEN SADR present
                    this.m_SNET = Convert.ToUInt16((int)packet[idx] * 0x0100 + (int)packet[idx + 1]);
                    idx += 2;

                    this.m_SLEN = packet[idx];
                    idx++;

                    this.m_SADR = new byte[this.m_SLEN];
                    for (int addrIdx = 0; addrIdx < this.m_SLEN; addrIdx++)
                    {
                        this.m_SADR[addrIdx] = packet[idx + addrIdx];
                    }
                    idx += this.m_SLEN;
                }

                if (this.m_HasDNET)
                {
                    //Hop Count
                    this.m_HopCount = packet[idx];
                    idx++;
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new BACnetPacketException("값을 읽는 중 패킷의 배열 범위를 벗어났습니다. (NetworkProtocolControlInformation)");
            }

            nextIdx = idx;
        }

        // private 멤버변수

        private bool m_HasSNET = false;

        private bool m_HasDNET = false;

        // private BACnet 멤버변수

        /// <summary>Version(1byte) : 0x01 고정
        /// </summary>
        private byte m_Version = (byte)0x01;

        /// <summary><pre>NPCI Control Octet.
        /// Bit 7 (& 0x80) : Network Layer Message Or APDU, 
        /// Bit 6 : Reserved,
        /// Bit 5 (& 0x20) : Destination Specifier,
        /// Bit 4 : Reserved,
        /// Bit 3 (& 0x08) : Source Specifier,
        /// Bit 2 (& 0x04) : Expecting reply
        /// Bit 1, 2 (& 0x03) : Priority(11=LifeSatety, 10=CriticalEquip, 01=Urgent, 00=Normal)
        /// </pre>
        /// </summary>
        private byte m_Control = (byte)0x00;

        private UInt16 m_DNET;

        private byte m_DLEN;

        private byte[] m_DADR;

        private UInt16 m_SNET;

        private byte m_SLEN;

        private byte[] m_SADR;

        private byte m_HopCount = 255;
    }
}
