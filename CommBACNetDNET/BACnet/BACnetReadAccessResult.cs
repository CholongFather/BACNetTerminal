using System;
using System.Collections.Generic;

namespace BACnet
{
    /// <summary>ReadPropertyMultiple - Result
    /// </summary>
    internal class BACnetReadAccessResult
    {
        // public 속성

        public bool HasError
        {
            get { return m_HasError; }
        }

        // public BACnet 속성

        public UInt32 ObjectIdentifier
        {
            get { return m_ObjectIdentifier; }
        }

        public byte PropertyID
        {
            get { return m_PropertyID; }
        }

        public double Value
        {
            get { return m_Value; }
        }

        public double?[] Values
        {
            get { return m_Values; }
        }

        public byte ErrorClass
        {
            get { return m_ErrorClass; }
        }

        public byte ErrorCode
        {
            get { return m_ErrorCode; }
        }

        // public static 메서드

        #region 패킷 샘플
        // 0C 00 10 00 09 1E 29 55 4E 44 40 0E 14 7B 4F 1F 
        // 0C 00 12 00 05 1E 29 55 4E 44 41 11 99 9A 4F 1F
        // 0C 00 12 00 06 1E 29 55 4E 44 42 20 00 00 4F 1F 
        // 0C 00 12 00 07 1E 29 55 4E 44 44 83 E0 00 4F 1F
        // 0C 00 12 00 08 1E 29 55 4E 44 3F 78 51 EC 4F 1F 
        // 0C 00 12 00 09 1E 29 55 4E 44 3F 4F 5C 29 4F 1F
        // 0C 01 08 00 02 1E 29 55 4E 91 00 4F 1F 
        // 0C 01 0A 00 02 1E 29 55 4E 91 00 4F 1F
        // 0C 01 0C 00 02 1E 29 55 4E 91 00 4F 1F 
        #endregion
        #region 패킷 샘플(priority-array. priority 8에 Real 100.0)
        // 0C 00 40 00 01 
        // 1E 29 57 
        // 4E 
        // 00 00 00 00 
        // 00 00 00 44 42 C8 00 00 
        // 00 00 00 00 
        // 00 00 00 00 
        // 4F 
        // 1F        
        #endregion

        // 수정 2015-02-06 // 기존 PRESENT_VALUE 외에 priority-array 처리 기능 추가

        public static BACnetReadAccessResult GetResult(byte[] packet, int startIndex, out int index)
        {
            BACnetReadAccessResult rtn = new BACnetReadAccessResult();

            int idx = startIndex;

            byte nowTag;
            byte[] nowData;

            nowTag = ReadTag(packet, ref idx);

            if (nowTag != 0x0c)
            {
                throw new BACnetPacketException("SD Context Tag 0 (Object Identifer, L=4) Tag값이 0x0C 가 아닌 0x" + nowTag.ToString("X2") + " 입니다.");
            }

            nowData = ReadValue(packet, ref idx, 4);
            rtn.m_ObjectIdentifier = Convert.ToUInt32((nowData[0] * 0x01000000) + (nowData[1] * 0x00010000) + (nowData[2] * 0x00000100) + (nowData[3] * 0x00000001));

            nowTag = ReadTag(packet, ref idx);

            if (nowTag != 0x1e)
            {
                throw new BACnetPacketException("PD Opening Tag 1 (List Of Result) Tag 값이 0x1E 가 아닌 0x" + nowTag.ToString("X2") + " 입니다.");
            }

            nowTag = ReadTag(packet, ref idx);

            if (nowTag != 0x29)
            {
                throw new BACnetPacketException("SD Context Tag 2 (Property Identifier, L=1) Tag 값이 0x29 가 아닌 0x" + nowTag.ToString("X2") + " 입니다.");
            }

            nowData = ReadValue(packet, ref idx, 1);
            rtn.m_PropertyID = nowData[0];

            if (rtn.m_PropertyID == 0x55)
            {
                // 0x55 = 85 = PRESENT_VALUE
            }
            else if (rtn.m_PropertyID == 0x57)
            {
                // 0x57 = 87 = priority-array
            }
            else
            {
                throw new BACnetPacketException("PropertyID 값이 0x55 (PRESENT_VALUE), 0x57 (priority-array)가 아닌 0x" + rtn.m_PropertyID.ToString("X2") + " 입니다.");
            }

            nowTag = ReadTag(packet, ref idx);
            if (nowTag == 0x4e)
            {
                // Property Array Index (Optional) 는 없는 것으로 가정

                rtn.m_HasError = false;

                List<double?> listValues = new List<double?>(16);
                int cntValues = 1;

                if (rtn.m_PropertyID == 0x55)
                {
                    // 0x55 = 85 = PRESENT_VALUE
                    cntValues = 1;
                }
                else if (rtn.m_PropertyID == 0x57)
                {
                    // 0x57 = 87 = priority-array
                    cntValues = 16;
                }

                for (int idxValues = 0; idxValues < cntValues; idxValues++)
                {
                    nowTag = ReadTag(packet, ref idx);
                    if (nowTag == 0x00)
                    {
                        // NULL
                        listValues.Add(null);
                    }
                    else if (nowTag == 0x91)
                    {
                        //(Enumerated, L=1)

                        nowData = ReadValue(packet, ref idx, 1);
                        listValues.Add((double)nowData[0]);
                    }
                    else if (nowTag == 0x44)
                    {
                        //(Real, L=4)
                        nowData = ReadValue(packet, ref idx, 4);
                        byte[] tempData = new byte[] { nowData[3], nowData[2], nowData[1], nowData[0] };
                        listValues.Add(Convert.ToDouble(BitConverter.ToSingle(tempData, 0)));
                    }
                    else if (nowTag == 0x21)
                    {
                        //(unsigned int, L=1)
                        nowData = ReadValue(packet, ref idx, 1);
                        listValues.Add(Convert.ToInt32(nowData[0]));
                    }
                    else if (nowTag == 0x22)
                    {
                        //(unsigned int, L=2)
                        nowData = ReadValue(packet, ref idx, 2);
                        byte[] tempData = new byte[] { nowData[1], nowData[0] };
                        listValues.Add(Convert.ToInt32(BitConverter.ToUInt16(tempData, 0)));
                    }
                    else
                    {
                        throw new BACnetPacketException("Application Tag 값이 0x91 (Enumerated, L=1), 0x44 (Real, L=4), 0x21 (unsigned int, L=1), 0x22 (unsigned int, L=2)가 아닌 0x" + nowTag.ToString("X2") + " 입니다.");
                    }
                }

                if (rtn.m_PropertyID == 0x55)
                {
                    // 0x55 = 85 = PRESENT_VALUE
                    rtn.m_Value = (double)listValues[0];
                }
                else if (rtn.m_PropertyID == 0x57)
                {
                    // 0x57 = 87 = priority-array
                    for (int idxValues = 0; idxValues < Math.Min(listValues.Count, rtn.m_Values.Length); idxValues++)
                    {
                        rtn.m_Values[idxValues] = listValues[idxValues];
                    }
                }

                nowTag = ReadTag(packet, ref idx);
                if (nowTag == 0x4f)
                {
                    // PD Closing Tag 4 (Property Value)
                }
                else
                {
                    throw new BACnetPacketException("PD Closing Tag 4 (Property Value) Tag 값이 0x4F 가 아닌 0x" + nowTag.ToString("X2") + " 입니다.");
                }
            }
            else if (nowTag == 0x5e)
            {
                rtn.m_HasError = true;

                nowTag = ReadTag(packet, ref idx);
                if (nowTag == 0x91)
                {
                    nowData = ReadValue(packet, ref idx, 1);
                    rtn.m_ErrorClass = nowData[0];
                }
                else
                {
                    throw new BACnetPacketException("Error Class 의 Application Tag 값이 0x91 (Enumerated, L=1)이 아닌 0x" + nowTag.ToString("X2") + " 입니다.");
                }

                nowTag = ReadTag(packet, ref idx);
                if (nowTag == 0x91)
                {
                    nowData = ReadValue(packet, ref idx, 1);
                    rtn.m_ErrorCode = nowData[0];
                }
                else
                {
                    throw new BACnetPacketException("Error Code 의 Application Tag 값이 0x91 (Enumerated, L=1)이 아닌 0x" + nowTag.ToString("X2") + " 입니다.");
                }

                nowTag = ReadTag(packet, ref idx);
                if (nowTag == 0x5f)
                {
                    // PD Closing Tag 5 (Property Access Error)
                }
                else
                {
                    throw new BACnetPacketException("PD Closing Tag 5 (Property Access Error) Tag 값이 0x5F 가 아닌 0x" + nowTag.ToString("X2") + " 입니다.");
                }
            }
            else
            {
                throw new BACnetPacketException("PD Opening Tag 값이 0x4E (Property Value), 0x5E (Property Access Error)가 아닌 0x" + nowTag.ToString("X2") + " 입니다.");
            }

            nowTag = ReadTag(packet, ref idx);
            if (nowTag == 0x1f)
            {

            }
            else
            {
                throw new BACnetPacketException("PD Closing Tag 1 (List Of Result) Tag 값이 0x1F 가 아닌 0x" + nowTag.ToString("X2") + " 입니다.");
            }

            index = idx;

            return rtn;
        }

        // private static 메서드

        private static byte ReadTag(byte[] packet, ref int index)
        {
            byte rtn = 0;

            try
            {
                rtn = packet[index];
            }
            catch (IndexOutOfRangeException)
            {
                throw new BACnetPacketException("Tag를 읽는 중 패킷의 배열 범위를 벗어났습니다. (BACnetReadAccessResult)");
            }

            index++;
            return rtn;
        }

        private static byte[] ReadValue(byte[] packet, ref int index, int count)
        {
            byte[] rtn = new byte[count];

            try
            {
                for (int nowIdx = 0; nowIdx < count; nowIdx++)
                {
                    rtn[nowIdx] = packet[index + nowIdx];
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new BACnetPacketException("값을 읽는 중 패킷의 배열 범위를 벗어났습니다. (BACnetReadAccessResult)");
            }

            index += count;
            return rtn;
        }

        // private 멤버변수

        bool m_HasError = false;

        // private BACnet 멤버변수

        UInt32 m_ObjectIdentifier;

        byte m_PropertyID = (byte)0x55;

        double?[] m_Values = new double?[16];

        double m_Value;

        byte m_ErrorClass;

        byte m_ErrorCode;
    }
}