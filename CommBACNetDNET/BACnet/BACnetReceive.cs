using System.Collections.Generic;
using System;

namespace BACnet
{
    internal class BACnetReceive
    {
        // public 속성

        public bool IsPacketError
        {
            get { return m_IsPacketError; }
        }

        public bool HasBVLCI
        {
            get { return m_HasBVLCI; }
        }

        public bool IsUnicastNPDU
        {
            get { return m_IsUnicastNPDU; }
        }

        public bool HasNPCI
        {
            get { return m_HasNPCI; }
        }

        public bool IsNetworkLayerMessage
        {
            get { return m_IsNetworkLayerMessage; }
        }

        public bool IsInterestingAPDU
        {
            get { return m_IsInterestingAPDU; }
        }

        public BACnetPDUType PDUType
        {
            get { return m_PDUType; }
        }

        public bool HasSegment
        {
            get { return m_HasSegment; }
        }

        public bool HasServiceChoice
        {
            get { return m_HasServiceChoice; }
        }

        public BACnetService ServiceChoice
        {
            get { return m_ServiceChoice; }
        }

        public bool HasReadAccessResult
        {
            get { return m_HasReadAccessResult; }
        }

        // public BACnet 속성

        public BACnetVirtualLinkControlInformation BVLCI
        {
            get { return m_BVLCI; }
        }

        public NetworkProtocolControlInformation NPCI
        {
            get { return m_NPCI; }
        }

        public IBACnetAPDU APDU
        {
            get { return m_APDU; }
        }

        /// <summary>BACnet-ComplexACK-PDU - ReadPropertyMutiple ServiceChoice - 결과
        /// </summary>
        public BACnetReadAccessResult[] Results
        {
            get
            {
                if (m_Results == null)
                {
                    return new BACnetReadAccessResult[0];
                }
                else
                {
                    return m_Results.ToArray();
                }
            }
        }

        // public static 메서드

        public static BACnetReceive ReadPacket(byte[] packet, int startIdx, out int nextIdx)
        {
            BACnetReceive rtn = new BACnetReceive();

            int idx = startIdx;

            rtn.m_BVLCI = new BACnetVirtualLinkControlInformation(packet, idx, out idx);

            if (rtn.m_BVLCI.Type != 0x81)
            {
                rtn.m_IsPacketError = true;
                throw new BACnetPacketException("BVLCI Type 값이 0x81 이 아닙니다");
            }
            rtn.m_HasBVLCI = true;

            nextIdx = startIdx + rtn.m_BVLCI.Length;
            if (rtn.m_BVLCI.Function != BACnetFunction.OriginalUnicastNPDU)
            {
                // Unicast 아님
                rtn.m_IsUnicastNPDU = false;
                return rtn;
            }
            else
            {
                rtn.m_IsUnicastNPDU = true;
            }

            rtn.m_NPCI = new NetworkProtocolControlInformation(packet, idx, out idx);
            rtn.m_HasNPCI = true;

            if ((rtn.m_NPCI.Control & 0x80) > 0)
            {
                // Network Layer Message
                rtn.m_IsNetworkLayerMessage = true;
                return rtn;
            }

            try
            {
                rtn.m_PDUType = (BACnetPDUType)(packet[idx] & 0xf0);
            }
            catch (IndexOutOfRangeException)
            {
                throw new BACnetPacketException("PDUType 을 읽는 중 패킷의 배열 범위를 벗어났습니다.");
            }

            switch (rtn.m_PDUType)
            {
                default:
                case BACnetPDUType.BACnetComfirmedRequestPDU:
                case BACnetPDUType.BACnetUnconfirmedRequestPDU:
                case BACnetPDUType.SegmentACK:
                case BACnetPDUType.AbortPDU:
                    rtn.m_IsInterestingAPDU = false;
                    return rtn;
                case BACnetPDUType.BACnetSimpleACKPDU:
                    {
                        BACnetSimpleACK simpleACK = new BACnetSimpleACK(packet, idx, out idx);
                        rtn.m_APDU = simpleACK;
                        rtn.m_IsInterestingAPDU = true;
                        rtn.m_HasServiceChoice = true;
                        rtn.m_ServiceChoice = (BACnetService)simpleACK.ServiceChoice;
                    }
                    break;
                case BACnetPDUType.BACnetComplexACKPDU:
                    {
                        BACnetComplexACK complexACK = new BACnetComplexACK(packet, idx, out idx);
                        rtn.m_APDU = complexACK;
                        rtn.m_IsInterestingAPDU = true;
                        rtn.m_HasServiceChoice = true;
                        rtn.m_ServiceChoice = (BACnetService)complexACK.ServiceChoice;
                    }
                    break;
                case BACnetPDUType.ErrorPDU:
                    {
                        BACnetErrorPDU errorPDU = new BACnetErrorPDU(packet, idx, out idx);
                        rtn.m_APDU = errorPDU;
                        rtn.m_IsInterestingAPDU = true;
                        rtn.m_HasServiceChoice = true;
                        rtn.m_ServiceChoice = (BACnetService)errorPDU.ServiceChoice;
                    }
                    break;
                case BACnetPDUType.RejectPDU:
                    {
                        BACnetRejectPDU rejectPDU = new BACnetRejectPDU(packet, idx, out idx);
                        rtn.m_APDU = rejectPDU;
                        rtn.m_IsInterestingAPDU = true;
                        rtn.m_HasServiceChoice = false;
                    }
                    break;
            }

            if (rtn.m_PDUType == BACnetPDUType.BACnetComplexACKPDU)
            {
                if ((rtn.m_APDU.PDUType & 0x08) > 0)
                {
                    // Has Segment
                    // 현재, 분할 응답에 대한 처리는 되어있지 않음
                    rtn.m_HasSegment = true;
                }

                rtn.m_Results = new List<BACnetReadAccessResult>(20);
                rtn.m_HasReadAccessResult = true;

                try
                {
                    while (idx < nextIdx)
                    {
                        BACnetReadAccessResult nowResult = BACnetReadAccessResult.GetResult(packet, idx, out idx);
                        rtn.m_Results.Add(nowResult);
                    }
                }
                catch
                {
                    rtn.m_IsPacketError = true;
                    throw;
                }
            }

            if (idx != nextIdx)
            {
                throw new BACnetPacketException("패킷 내용의 길이가 BVLCI 의 Length 와 일치하지 않습니다.");
            }

            return rtn;
        }

        // private 멤버변수

        private bool m_IsPacketError = false;

        private bool m_HasBVLCI = false;

        private bool m_IsUnicastNPDU = false;

        private bool m_HasNPCI = false;

        private bool m_IsNetworkLayerMessage = false;

        private bool m_IsInterestingAPDU = false;

        private BACnetPDUType m_PDUType = (byte)0x00;

        private bool m_HasSegment = false;

        private bool m_HasServiceChoice = false;

        private BACnetService m_ServiceChoice = (byte)0x00;

        private bool m_HasReadAccessResult = false;

        // private BACnet 멤버변수

        private BACnetVirtualLinkControlInformation m_BVLCI;

        private NetworkProtocolControlInformation m_NPCI;

        private IBACnetAPDU m_APDU;

        private List<BACnetReadAccessResult> m_Results;
    }
}