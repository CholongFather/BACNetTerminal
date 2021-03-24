using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace BACnet
{
    internal class ClsMultiReadPacketData : IRequestPacketData
    {
        public const byte PROPERTY_PRESENT_VALUE = 0x55;
        public const byte PROPERTY_PRIORITY_ARRAY = 0x57;
        
        public const byte MAX_INVOKE_ID = 255;

        public const int REQUEST_MAX_OBJECT_COUNT = 50;

        // public 속성

        public byte InvokeId
        {
            get { return m_InvokeId; }
        }

        public IPEndPoint RemoteEP
        {
            get { return m_RemoteEP; }
        }

        public ClsBACnetDevice Device
        {
            get { return m_Device; }
        }

        public List<ClsRequestItemData> Items
        {
            get { return m_Items; }
        }

        // public 메서드

        public bool AddTag(ClsTagItem tagItem)
        {
            return this.AddTag(tagItem, PROPERTY_PRESENT_VALUE);
        }

        public bool AddTag(ClsTagItem tagItem, byte propertyid)
        {
            bool rtn = false;

            if (tagItem == null)
            {
                throw new ArgumentNullException("TagItem 이 null 입니다", "tagItem");
            }
            //else if (this.m_Items.Count > REQUEST_MAX_OBJECT_COUNT)
            //{
            //    throw new ArgumentException("최대 Object 갯수를 초과했습니다", "tagItem");
            //}
            else if (propertyid != PROPERTY_PRESENT_VALUE & propertyid != PROPERTY_PRIORITY_ARRAY)
            {
                throw new ArgumentException("처리하지 않는 propertyid 입니다", "propertyid");
            }
            else if (this.m_RequsetPacket != null)
            {
                throw new ArgumentException("RequestPacket 이 만든 후에는 추가할 수 없습니다", "tagItem");
            }
            else
            {
                if (propertyid == PROPERTY_PRESENT_VALUE
                    & this.m_ResponsePacketSizeEstimates + PacketMultiReadRequest.RESPONSE_PACKET_PRESENT_VALUE_SIZE <= PacketMultiReadRequest.PACKET_MAX_SIZE)
                {
                    this.m_ResponsePacketSizeEstimates += PacketMultiReadRequest.RESPONSE_PACKET_PRESENT_VALUE_SIZE;
                    this.m_Items.Add(new ClsRequestItemData(tagItem, propertyid));
                    rtn = true;
                }
                else if (propertyid == PROPERTY_PRIORITY_ARRAY
                   & this.m_ResponsePacketSizeEstimates + PacketMultiReadRequest.RESPONSE_PACKET_PRIORITY_ARRAY_SIZE <= PacketMultiReadRequest.PACKET_MAX_SIZE)
                {
                    this.m_ResponsePacketSizeEstimates += PacketMultiReadRequest.RESPONSE_PACKET_PRIORITY_ARRAY_SIZE;
                    this.m_Items.Add(new ClsRequestItemData(tagItem, propertyid));
                    rtn = true;
                }
                else
                {
                    rtn = false;
                }
            }

            return rtn;
        }

        public void Send(UdpClient udpClient)
        {
            if (udpClient == null)
            {
                throw new ArgumentNullException("UdpClient 가 null 입니다", "udpClient");
            }
            else if (this.m_RequsetPacket == null)
            {
                throw new Exception("Request Packet 이 준비되지 않았습니다");
            }
            else
            {
                udpClient.Send(m_RequsetPacket, m_RequsetPacket.Length, m_RemoteEP);
            }
        }

        public void MakeRequestPacket()
        {
            if (this.m_RequsetPacket != null)
            {
                throw new Exception("Request Packet 이 이미 있습니다");
            }
            else if (this.m_Items.Count == 0)
            {
                throw new Exception("Item Count 가 0 입니다");
            }
            else
            {
                PacketMultiReadRequest nowPacket;
                if (this.m_Device.HasNET)
                {
                    nowPacket = new PacketMultiReadRequest(this.m_InvokeId, m_Device.NET, this.m_Device.ADR);
                }
                else
                {
                    nowPacket = new PacketMultiReadRequest(this.m_InvokeId);
                }

                foreach (ClsRequestItemData nowItem in this.m_Items)
                {
                    nowPacket.AddObjectID(nowItem.ObjectIdentifier, nowItem.PropertyId);
                }

                this.m_RequsetPacket = nowPacket.GetPacket();
            }
        }

        public bool CheckReceiceData(byte invokeId, IList<BACnetReadAccessResult> listResult, IPEndPoint remoteEP, bool hasSNet, ushort snet, byte[] sadr)
        {
            if (m_InvokeId != invokeId)
            {
                return false;
            }

            if (IPEndPoint.Equals(m_RemoteEP, remoteEP) != true)
            {
                return false;
            }

            if (m_Device.HasNET != hasSNet)
            {
                return false;
            }

            if (m_Device.HasNET == true)
            {
                if (sadr == null) sadr = new byte[0];

                if (m_Device.NET != snet)
                {
                    return false;
                }

                if (m_Device.ADR.Length != sadr.Length)
                {
                    return false;
                }

                for (int idx = 0; idx < sadr.Length; idx++)
                {
                    if (m_Device.ADR[idx] != sadr[idx])
                    {
                        return false;
                    }
                }
            }

            if (listResult == null)
            {
                listResult = new BACnetReadAccessResult[0];
            }

            if (m_Items.Count != listResult.Count)
            {
                return false;
            }

            for (int idx = 0; idx < listResult.Count; idx++)
            {
                if (m_Items[idx].ObjectIdentifier != listResult[idx].ObjectIdentifier)
                {
                    return false;
                }
            }

            return true;
        }

        // public static 메서드

        public static ClsMultiReadPacketData FindReadPacketData(IEnumerable<ClsMultiReadPacketData> listData, IPEndPoint remoteEP, BACnetReceive data)
        {
            ClsMultiReadPacketData rtn = null;

            BACnetComplexACK complexACK = data.APDU as BACnetComplexACK;
            if (data.HasBVLCI & data.HasNPCI & data.IsInterestingAPDU & data.HasReadAccessResult & complexACK != null & complexACK.ServiceChoice == (byte)BACnetService.ReadPropertyMultiple)
            {
                foreach (ClsMultiReadPacketData nowData in listData)
                {
                    NetworkProtocolControlInformation nowNPCI = data.NPCI;
                    if (nowData.CheckReceiceData(data.APDU.InvokeId, data.Results, remoteEP, nowNPCI.HasSNET, nowNPCI.SNET, nowNPCI.SADR))
                    {
                        rtn = nowData;
                        break;
                    }
                }
            }

            return rtn;
        }

        // 생성자

        public ClsMultiReadPacketData(byte invokeId, ClsBACnetDevice device)
        {
            if (invokeId > MAX_INVOKE_ID)
            {
                throw new ArgumentException("InvokeId 가 사용가능한 범위를 벗어났습니다", "invokeId");
            }
            else if (device == null)
            {
                throw new ArgumentNullException("Device 가 null 입니다", "device");
            }
            else
            {
                this.m_InvokeId = invokeId;
                this.m_RemoteEP = device.RemoteEP;
                this.m_Device = device;

                this.m_Items = new List<ClsRequestItemData>(50);
            }
        }

        // private 멤버변수

        private byte m_InvokeId;

        private IPEndPoint m_RemoteEP;

        private byte[] m_RequsetPacket;

        private ClsBACnetDevice m_Device;

        private List<ClsRequestItemData> m_Items;

        private int m_ResponsePacketSizeEstimates = PacketMultiReadRequest.RESPONSE_PACKET_BASE_SIZE;
    }

    class ClsRequestItemData
    {
        // public 속성

        public int TagId
        {
            get { return this.m_TagItem.TagId; }
        }

        public UInt32 ObjectIdentifier
        {
            get { return this.m_TagItem.ObjectIdentifier; }
        }

        public byte PropertyId
        {
            get { return this.m_PropertyId; }
        }

        public ClsTagItem TagItem
        {
            get { return this.m_TagItem; }
        }

        // 생성자

        public ClsRequestItemData(ClsTagItem tagItem, byte propertyid)
        {
            this.m_TagItem = tagItem;
            this.m_PropertyId = propertyid;
        }

        // private 멤버변수

        private ClsTagItem m_TagItem;

        private byte m_PropertyId;
    }
}
