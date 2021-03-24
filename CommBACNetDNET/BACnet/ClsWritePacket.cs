using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace BACnet
{
    internal interface IRequestPacketData
    {
        void Send(UdpClient udpClient);
    }

    internal class ClsWritePacketData : IRequestPacketData
    {
        public const byte MAX_INVOKE_ID = 255;

        public const byte DEFAULT_WRITE_PRIORITY = 8;

        // public 속성

        public int TagId
        {
            get { return m_Item.TagId; }
        }

        public DateTime OrderTime
        {
            get { return this.m_OrderTime; }
        }

        public int ReorderCount
        {
            get { return this.m_ReorderCount; }
        }

        public DateTime FirstOrderTime
        {
            get { return this.m_FirstOrderTime; }
        }

        public DateTime RequestTime
        {
            get { return this.m_RequestTime; }
        }

        public ClsBACnetDevice Device
        {
            get { return this.m_Device; }
        }

        public ClsTagItem Item
        {
            get { return this.m_Item; }
        }

        public bool HasValue
        {
            get { return this.m_HasValue; }
        }

        public double Value
        {
            get { return this.m_Value; }
        }

        public byte Priority
        {
            get { return this.m_Priority; }
        }

        // public 메서드

        public void Send(UdpClient udpClient)
        {
            if (udpClient == null)
            {
                throw new ArgumentNullException("UdpClient 가 null 입니다", "udpClient");
            }
            else
            {
                byte[] sendPacket = MakePacket();

                udpClient.Send(sendPacket, sendPacket.Length, m_Device.RemoteEP);

                m_RequestTime = DateTime.Now;
            }
        }

        public bool CheckReceiceData(byte invokeId, IPEndPoint remoteEP, bool hasSNet, ushort snet, byte[] sadr)
        {
            if (m_InvokeId != invokeId)
            {
                return false;
            }
            else
            {
                return m_Device.CheckDevice(remoteEP, hasSNet, snet, sadr);
            }
        }

        // 생성자

        public ClsWritePacketData(byte invokeId, ClsTagItem tagItem, byte orderPriority, bool hasValue, double value)
        {
            m_OrderTime = DateTime.Now;
            m_ReorderCount = 0;
            m_FirstOrderTime = m_OrderTime;

            m_InvokeId = invokeId;
            m_Item = tagItem;
            m_Device = tagItem.Device;

            m_Priority = orderPriority;
            m_HasValue = hasValue;
            m_Value = value;
        }

        public ClsWritePacketData(byte invokeId, ClsWritePacketData delayedOrder, bool hasValue, double value)
            : this(invokeId, delayedOrder.m_Item, delayedOrder.m_Priority, hasValue, value)
        {
            m_ReorderCount = delayedOrder.m_ReorderCount + 1;
            m_FirstOrderTime = delayedOrder.m_FirstOrderTime;
        }

        // private 멤버변수

        private readonly DateTime m_OrderTime;

        private readonly int m_ReorderCount = 0;

        private readonly DateTime m_FirstOrderTime;

        private DateTime m_RequestTime = DateTime.MinValue;

        private readonly byte m_InvokeId;

        private readonly ClsBACnetDevice m_Device;

        private readonly ClsTagItem m_Item;

        private readonly bool m_HasValue = true;

        private readonly double m_Value;

        private readonly byte m_Priority;

        // private 메서드

        private byte[] MakePacket()
        {
            byte dataType;
            double readValue;

            if (this.m_HasValue)
            {
                dataType = m_Item.ValueTypeTag;
                readValue = (m_Value - m_Item.BIAS) / m_Item.GAIN;
            }
            else
            {
                dataType = 0x00;
                readValue = 0;
            }

            PacketWriteRequest packet;

            if (m_Device.HasNET)
            {
                packet = new PacketWriteRequest(m_InvokeId, m_Device.NET, m_Device.ADR, m_Item.ObjectIdentifier, dataType, readValue, m_Priority);
            }
            else
            {
                packet = new PacketWriteRequest(m_InvokeId, m_Item.ObjectIdentifier, dataType, readValue, m_Priority);
            }

            return packet.GetPacket();
        }
    }
}

