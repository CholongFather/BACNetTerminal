using System;
using System.Collections.Generic;
using System.Text;

namespace BACnet
{
    internal class ClsTagItem
    {
        // public 속성

        public int TagId
        {
            get { return m_TagId; }
        }

        public BACnetObjectType ObjectType
        {
            get { return m_ObjectType; }
        }

        public int ObjectInstance
        {
            get { return m_ObjectInstance; }
        }

        public uint ObjectIdentifier
        {
            get { return m_ObjectIdentifier; }
        }

        public bool IsWriteable
        {
            get { return m_IsWriteable; }
        }

        public byte ValueTypeTag
        {
            get { return m_ValueTypeTag; }
        }

        public double GAIN
        {
            get { return m_GAIN; }
        }

        public double BIAS
        {
            get { return m_BIAS; }
        }

        public ClsBACnetDevice Device
        {
            get
            {
                ClsBACnetDevice rtn = (ClsBACnetDevice)m_Device.Target;

                if (rtn != null)
                {
                    return rtn;
                }
                else
                {
                    throw new ObjectDisposedException("ClsBACnetDevice", "제거된 디바이스입니다");
                }
            }
        }

        // public 메서드

        public bool CheckTagItem(int tagId)
        {
            if (this.TagId == tagId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // public static 메서드

        public static ClsTagItem FindTagObject(IEnumerable<ClsTagItem> tagItems, int tagId)
        {
            ClsTagItem rtn = null;

            foreach (ClsTagItem nowItem in tagItems)
            {
                if (nowItem.CheckTagItem(tagId))
                {
                    rtn = nowItem;
                    break;
                }
            }

            return rtn;
        }

        // 생성자

        public ClsTagItem(int tagId, BACnetObjectType objectType, int objectInstance, ClsBACnetDevice device)
            : this(tagId, objectType, objectInstance, device, 1, 0)
        {
        }

        public ClsTagItem(int tagId, BACnetObjectType objectType, int objectInstance, ClsBACnetDevice device, double gain, double bias)
        {
            this.m_TagId = tagId;

            if (Enum.IsDefined(typeof(BACnetObjectType), objectType) != true)
            {
                throw new ArgumentException("BACnet Object의 ObjectType 이 처리 가능한 타입이 아닙니다", "objectType");
            }
            else if (objectInstance < 0 | objectInstance >= 0x400000)
            {
                throw new ArgumentException("BACnet Object의 ObjectInstance 가 범위를 벗어났습니다", "objectInstance");
            }
            else if (gain == 0)
            {
                throw new ArgumentException("GAIN 이 0 입니다", "gain");
            }
            else
            {
                this.m_ObjectType = objectType;
                this.m_ObjectInstance = objectInstance;
                this.m_ObjectIdentifier = (Convert.ToUInt32(this.m_ObjectType) << 22) | Convert.ToUInt32(objectInstance);

                #region ObjectType 에 따라, IsWriteable, ValueTypeTag 설정

                switch (objectType)
                {
                    case BACnetObjectType.AI:
                        this.m_IsWriteable = false;
                        this.m_ValueTypeTag = 0x44;
                        break;
                    case BACnetObjectType.BI:
                        this.m_IsWriteable = false;
                        this.m_ValueTypeTag = 0x91;
                        break;
                    case BACnetObjectType.MSI:
                        this.m_IsWriteable = false;
                        this.m_ValueTypeTag = 0x21;
                        break;
                    case BACnetObjectType.AO:
                        this.m_IsWriteable = true;
                        this.m_ValueTypeTag = 0x44;
                        break;
                    case BACnetObjectType.AV:
                        this.m_IsWriteable = true;
                        this.m_ValueTypeTag = 0x44;
                        break;
                    case BACnetObjectType.BO:
                        this.m_IsWriteable = true;
                        this.m_ValueTypeTag = 0x91;
                        break;
                    case BACnetObjectType.BV:
                        this.m_IsWriteable = true;
                        this.m_ValueTypeTag = 0x91;
                        break;
                    case BACnetObjectType.MSO:
                        this.m_IsWriteable = true;
                        this.m_ValueTypeTag = 0x21;
                        break;
                    case BACnetObjectType.MSV:
                        this.m_IsWriteable = true;
                        this.m_ValueTypeTag = 0x21;
                        break;
                }

                #endregion

                this.m_Device = new WeakReference(device);

                this.m_GAIN = gain;
                this.m_BIAS = bias;
            }
        }

        // private 멤버변수

        private int m_TagId;

        private BACnetObjectType m_ObjectType;

        private int m_ObjectInstance;

        private uint m_ObjectIdentifier;

        private bool m_IsWriteable = false;

        private byte m_ValueTypeTag;

        private double m_GAIN = 1;

        private double m_BIAS = 0;

        private WeakReference m_Device;
    }
}
