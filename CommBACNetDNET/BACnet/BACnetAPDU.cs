using System;

namespace BACnet
{
    internal interface IBACnetAPDU
    {
        byte PDUType { get; }

        byte InvokeId { get; }

        bool HasServiceChoice { get; }

        byte ServiceChoice { get; }
    }

    internal class BACnetSimpleACK : IBACnetAPDU
    {
        // public 속성

        public bool HasServiceChoice
        {
            get { return true; }
        }

        // public BACnet 속성

        public byte PDUType
        {
            get { return m_PDUType; }
        }

        public byte InvokeId
        {
            get { return m_InvokeId; }
        }

        public byte ServiceChoice
        {
            get { return m_ServiceChoice; }
        }

        // 생성자

        public BACnetSimpleACK(byte[] packet, int startIdx, out int nextIdx)
        {
            int idx = startIdx;

            try
            {
                this.m_PDUType = packet[idx];
                idx++;

                this.m_InvokeId = packet[idx];
                idx++;

                this.m_ServiceChoice = packet[idx];
                idx++;
            }
            catch (IndexOutOfRangeException)
            {
                throw new BACnetPacketException("값을 읽는 중 패킷의 배열 범위를 벗어났습니다. (BACnetSimpleACK)");
            }

            nextIdx = idx;
        }

        // private BACnet 멤버변수

        private byte m_PDUType;

        private byte m_InvokeId;

        private byte m_ServiceChoice;
    }

    internal class BACnetErrorPDU : IBACnetAPDU
    {
        // public 속성

        public bool HasServiceChoice
        {
            get { return true; }
        }

        // public BACnet 속성

        public byte PDUType
        {
            get { return m_PDUType; }
        }

        public byte InvokeId
        {
            get { return m_InvokeId; }
        }

        public byte ServiceChoice
        {
            get { return m_ServiceChoice; }
        }

        public byte ErrorClass
        {
            get { return m_ErrorClass; }
        }

        public byte ErrorCode
        {
            get { return m_ErrorCode; }
        }

        // 생성자

        public BACnetErrorPDU(byte[] packet, int startIdx, out int nextIdx)
        {
            int idx = startIdx;

            try
            {
                this.m_PDUType = packet[idx];
                idx++;

                this.m_InvokeId = packet[idx];
                idx++;

                this.m_ServiceChoice = packet[idx];
                idx++;

                byte nowTag;

                nowTag = packet[idx];
                idx++;

                if (nowTag != 0x91)
                {
                    throw new BACnetPacketException("Error Class 의 Application Tag 값이 0x91 (Enumerated, L=1)이 아닌 0x" + nowTag.ToString("X2") + " 입니다.");
                }

                this.m_ErrorClass = packet[idx];
                idx++;

                nowTag = packet[idx];
                idx++;

                if (nowTag != 0x91)
                {
                    throw new BACnetPacketException("Error Code 의 Application Tag 값이 0x91 (Enumerated, L=1)이 아닌 0x" + nowTag.ToString("X2") + " 입니다.");
                }

                this.m_ErrorCode = packet[idx];
                idx++;
            }
            catch (IndexOutOfRangeException)
            {
                throw new BACnetPacketException("값을 읽는 중 패킷의 배열 범위를 벗어났습니다. (BACnetErrorPDU)");
            }

            nextIdx = idx;
        }

        // private BACnet 멤버변수

        private byte m_PDUType;

        private byte m_InvokeId;

        private byte m_ServiceChoice;

        private byte m_ErrorClass;

        private byte m_ErrorCode;
    }

    internal class BACnetRejectPDU : IBACnetAPDU
    {
        // public 속성

        public bool HasServiceChoice
        {
            get { return false; }
        }

        public byte ServiceChoice
        {
            get { return 0; }
        }

        // public BACnet 속성

        public byte PDUType
        {
            get { return m_PDUType; }
        }

        public byte InvokeId
        {
            get { return m_InvokeId; }
        }

        public byte RejectReason
        {
            get { return m_RejectReason; }
        }

        // 생성자

        public BACnetRejectPDU(byte[] packet, int startIdx, out int nextIdx)
        {
            int idx = startIdx;

            try
            {
                this.m_PDUType = packet[idx];
                idx++;

                this.m_InvokeId = packet[idx];
                idx++;

                this.m_RejectReason = packet[idx];
                idx++;
            }
            catch (IndexOutOfRangeException)
            {
                throw new BACnetPacketException("값을 읽는 중 패킷의 배열 범위를 벗어났습니다. (BACnetRejectPDU)");
            }

            nextIdx = idx;
        }

        // private BACnet 멤버변수

        private byte m_PDUType;

        private byte m_InvokeId;

        private byte m_RejectReason;
    }

    internal class BACnetComplexACK : IBACnetAPDU
    {
        // public 속성

        public bool HasServiceChoice
        {
            get { return true; }
        }

        public bool HasSegment
        {
            get { return m_HasSegment; }
        }

        // public BACnet 속성

        public byte PDUType
        {
            get { return m_PDUType; }
        }

        public byte InvokeId
        {
            get { return m_InvokeId; }
        }

        public byte SequenceNumber
        {
            get { return m_SequenceNumber; }
        }

        public byte ProposedWindowSize
        {
            get { return m_ProposedWindowSize; }
        }

        public byte ServiceChoice
        {
            get { return m_ServiceChoice; }
        }

        // 생성자

        public BACnetComplexACK(byte[] packet, int startIdx, out int nextIdx)
        {
            int idx = startIdx;

            try
            {
                this.m_PDUType = packet[idx];
                idx++;

                this.m_InvokeId = packet[idx];
                idx++;

                if ((this.m_PDUType & 0x08) > 0)
                {
                    this.m_HasSegment = true;

                    this.m_SequenceNumber = packet[idx];
                    idx++;

                    this.m_ProposedWindowSize = packet[idx];
                    idx++;
                }

                this.m_ServiceChoice = packet[idx];
                idx++;
            }
            catch (IndexOutOfRangeException)
            {
                throw new BACnetPacketException("값을 읽는 중 패킷의 배열 범위를 벗어났습니다. (BACnetComplexACK)");
            }

            nextIdx = idx;
        }

        // private 멤버변수

        private bool m_HasSegment = false;

        // private BACnet 멤버변수

        private byte m_PDUType;

        private byte m_InvokeId;

        private byte m_SequenceNumber;

        private byte m_ProposedWindowSize;

        private byte m_ServiceChoice;
    }
}
