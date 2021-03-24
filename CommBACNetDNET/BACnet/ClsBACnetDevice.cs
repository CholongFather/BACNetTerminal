using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace BACnet
{
    internal class ClsBACnetDevice
    {
        // public 속성

        public int Index
        {
            get { return m_Index; }
        }

        public string IfId
        {
            get { return m_IfId ?? ""; }
        }

        public IPEndPoint RemoteEP
        {
            get { return m_RemoteEP; }
        }

        public bool HasNET
        {
            get { return m_HasNET; }
        }

        public ushort NET
        {
            get { return m_NET; }
        }

        public byte LEN
        {
            get { return m_LEN; }
        }

        public byte[] ADR
        {
            get
            {
                if (m_ADR == null)
                {
                    return new byte[0];
                }
                else
                {
                    byte[] rtn = new byte[m_ADR.Length];
                    m_ADR.CopyTo(rtn, 0);
                    return rtn;
                }
            }
        }

        public List<ClsTagItem> Items
        {
            get { return m_Items; }
        }

        // public 메서드

        public bool CheckDevice(IPEndPoint findEP, bool hasNet, ushort findNet, byte[] findAdr)
        {
            if (IPEndPoint.Equals(this.m_RemoteEP, findEP) != true)
            {
                return false;
            }

            if (this.m_HasNET != hasNet)
            {
                return false;
            }

            if (this.m_HasNET == true)
            {
                findAdr = new byte[0];
                byte[] devAdr = this.m_ADR ?? new byte[0];
                if (this.m_NET != findNet | this.m_ADR.Length != findAdr.Length)
                {
                    return false;
                }

                for (int idx = 0; idx < findAdr.Length; idx++)
                {
                    if (devAdr[idx] != findAdr[idx])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void SetItems(IEnumerable<ClsTagItem> tagItems)
        {
            this.m_Items.Clear();
            foreach (ClsTagItem nowItem in tagItems)
            {
                this.m_Items.Add(nowItem);
            }
        }

        // public static 메서드

        public static bool FindIndexOfDevice(IList<ClsBACnetDevice> devices, ref int findIndex, IPEndPoint findEP, bool hasNet, ushort findNet, byte[] findAddr)
        {
            for (int idx = findIndex; idx < devices.Count; idx++)
            {
                if (devices[idx].CheckDevice(findEP, hasNet, findNet, findAddr))
                {
                    findIndex = idx;
                    return true;
                }
            }

            return false;
        }

        // 생성자

        public ClsBACnetDevice(int index, string ifId, IPEndPoint deviceEP)
        {
            this.m_Index = index;
            this.m_IfId = ifId;
            this.m_RemoteEP = deviceEP;
            this.m_HasNET = false;
            m_Items = new List<ClsTagItem>(20);
        }

        public ClsBACnetDevice(int index, string ifId, IPAddress deviceIP)
            : this(index, ifId, new IPEndPoint(deviceIP, 0xBAC0))
        {

        }

        public ClsBACnetDevice(int index, string ifId, IPEndPoint deviceEP, ushort deviceNet, byte[] deviceAdr)
            : this(index, ifId, deviceEP)
        {
            this.m_HasNET = true;

            if (deviceNet == 0 | deviceNet == 0xffff)
            {
                throw new ArgumentException("BACnet 디바이스의 Network Number 가 0 이거나 0xffff 입니다", "deviceNet");
            }
            else if (deviceAdr == null)
            {
                throw new ArgumentNullException("BACnet 디바이스의 Address 가 null 입니다", "deviceAdr");
            }
            else if (deviceAdr.Length == 0)
            {
                throw new ArgumentException("BACnet 디바이스의 Address 길이가 0 입니다", "deviceAdr");
            }
            else if (deviceAdr.Length > 18)
            {
                throw new ArgumentException("BACnet 디바이스의 Address 길이가 18 보다 큽니다", "deviceAdr");
            }
            else
            {
                this.m_NET = deviceNet;
                this.m_LEN = Convert.ToByte(deviceAdr.Length);
                this.m_ADR = deviceAdr;
            }
        }

        public ClsBACnetDevice(int index, string ifId, IPAddress deviceIP, ushort deviceNet, byte[] deviceAdr)
            : this(index, ifId, new IPEndPoint(deviceIP, 0xBAC0), deviceNet, deviceAdr)
        {

        }

        // private 멤버변수

        private readonly int m_Index;

        private readonly string m_IfId;

        private readonly IPEndPoint m_RemoteEP = null;

        private readonly bool m_HasNET = false;

        private readonly ushort m_NET = 0;

        private readonly byte m_LEN = 0;

        private readonly byte[] m_ADR = null;

        private List<ClsTagItem> m_Items;
    }
}