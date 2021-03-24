using System;

namespace BACnet
{
    /// <summary>BVLCI 클래스, BACnet Virtual Link Layer 의 Control Information
    /// </summary>
    internal class BACnetVirtualLinkControlInformation
    {
        // public 속성

        public bool HasOriginatingDevice
        {
            get { return m_HasOriginatingDevice; }
        }

        // public BACnet 속성

        public byte Type
        {
            get { return m_Type; }
        }

        public BACnetFunction Function
        {
            get { return m_Function; }
        }

        public UInt16 Length
        {
            get { return m_Length; }
        }

        public byte[] OriginatingDeviceIpAddress
        {
            get
            {
                if (m_OriginatingDeviceIpAddress == null)
                {
                    return new byte[0];
                }
                else
                {
                    byte[] rtn = new byte[m_OriginatingDeviceIpAddress.Length];
                    Array.Copy(m_OriginatingDeviceIpAddress, rtn, m_OriginatingDeviceIpAddress.Length);

                    return rtn;
                }
            }
        }

        public UInt16 OriginatingDevicePort
        {
            get { return m_OriginatingDevicePort; }
        }

        // 생성자

        public BACnetVirtualLinkControlInformation(byte[] packet, int startIdx, out int nextIdx)
        {
            int idx = startIdx;

            try
            {
                this.m_Type = packet[idx];
                idx++;

                this.m_Function = (BACnetFunction)packet[idx];
                idx++;

                this.m_Length = Convert.ToUInt16((int)packet[idx] * 0x0100 + (int)packet[idx + 1]);
                idx += 2;

                if (this.m_Function == BACnetFunction.ForwardedNPDU)
                {
                    m_HasOriginatingDevice = true;
                    m_OriginatingDeviceIpAddress = new byte[4];

                    m_OriginatingDeviceIpAddress[0] = packet[idx];
                    idx++;

                    m_OriginatingDeviceIpAddress[1] = packet[idx];
                    idx++;

                    m_OriginatingDeviceIpAddress[2] = packet[idx];
                    idx++;

                    m_OriginatingDeviceIpAddress[3] = packet[idx];
                    idx++;

                    this.m_OriginatingDevicePort = Convert.ToUInt16((int)packet[idx] * 0x0100 + (int)packet[idx + 1]);
                    idx += 2;
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new BACnetPacketException("값을 읽는 중 패킷의 배열 범위를 벗어났습니다. (NetworkProtocolControlInformation)");
            }

            nextIdx = idx;
        }

        // private 멤버변수

        private bool m_HasOriginatingDevice = false;

        // private BACnet 멤버변수

        private byte m_Type = (byte)0x81;   // Type(1byte) : 0x81 고정

        private BACnetFunction m_Function;   // Function(1byte) : 0x0a Unicast, 0x0b broadcast

        private UInt16 m_Length;    // Length(2byte:H->L)

        byte[] m_OriginatingDeviceIpAddress;

        ushort m_OriginatingDevicePort;
    }
}
