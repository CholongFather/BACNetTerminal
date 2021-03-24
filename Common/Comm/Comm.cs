using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class NetHeader
    {
        public int HeaderLenght = 28;

        private byte[] m_Flag;                  // Msg Type 1:공유 메모리 전송 2:제어 명령
        private byte[] m_Seq;                   // 메시지 순번
        private byte[] m_StartPos;              // 공유 메모리 시작 번지
        private byte[] m_ReadCnt;               // 요청/전송 개수
        private byte[] m_ClientPos;             // 클라이언트 메모리 시작 위치
        private byte[] m_DataLen;               // 데이터 부분 길이
        private byte[] m_ComMode;               // 압축여부

        //생성자
        public NetHeader()
        {
            m_Flag = new byte[4];
            m_Seq = new byte[4];
            m_StartPos = new byte[4];
            m_ReadCnt = new byte[4];
            m_ClientPos = new byte[4];
            m_DataLen = new byte[4];
            m_ComMode = new byte[4];

            for (int i = 0; i < this.m_Seq.Length; i++)
                this.m_Seq[i] = 0x00;

            for (int i = 0; i < this.m_DataLen.Length; i++)
                this.m_DataLen[i] = 0x00;

            for (int i = 0; i < this.m_ComMode.Length; i++)
                this.m_ComMode[i] = 0x00;

            this.HeaderLenght = m_Flag.Length + m_Seq.Length + m_StartPos.Length + m_ReadCnt.Length + m_ClientPos.Length + m_DataLen.Length + m_ComMode.Length;

            headers = new byte[HeaderLenght];
        }

        public uint Flag
        {
            set { m_Flag = BitConverter.GetBytes(value); }
            get { return BitConverter.ToUInt32(m_Flag, 0); }
        }

        public uint Seq
        {
            set { m_Seq = BitConverter.GetBytes(value); }
            get { return BitConverter.ToUInt32(m_Seq, 0); }
        }

        public uint StartPos
        {
            set { m_StartPos = BitConverter.GetBytes(value); }
            get { return BitConverter.ToUInt32(m_StartPos, 0); }
        }

        public uint ReadCnt
        {
            set { m_ReadCnt = BitConverter.GetBytes(value); }
            get { return BitConverter.ToUInt32(m_ReadCnt, 0); }
        }

        public uint ClientPos
        {
            set { m_ClientPos = BitConverter.GetBytes(value); }
            get { return BitConverter.ToUInt32(m_ClientPos, 0); }
        }

        public uint DataLen
        {
            set { m_DataLen = BitConverter.GetBytes(value); }
            get { return BitConverter.ToUInt32(m_DataLen, 0); }
        }

        public uint ComMode
        {
            set { m_ComMode = BitConverter.GetBytes(value); }
            get { return BitConverter.ToUInt32(m_ComMode, 0); }
        }

        private byte[] headers;

        public byte[] Header()
        {
            ArrayList HeaderList = new ArrayList();
            HeaderList.Add(m_Flag);
            HeaderList.Add(m_Seq);
            HeaderList.Add(m_StartPos);
            HeaderList.Add(m_ReadCnt);
            HeaderList.Add(m_ClientPos);
            HeaderList.Add(m_DataLen);
            HeaderList.Add(m_ComMode);

            //배열을 초기화함
            int HeaderCount = 0;
            int totalcount = 0;

            //루프를 돌면서 내용을 추가한다.
            foreach (object obj in HeaderList)
            {
                //Byte 타입인지 확인한다.
                Type type = obj.GetType();
                if (type.ToString() == "System.Byte")
                {
                    headers[totalcount] = (Byte)obj;
                    totalcount++;
                    continue;
                }
                else if (type.ToString() == "System.Byte[]")
                {
                    byte[] b = (byte[])obj;
                    int ObjLength = b.Length;

                    for (HeaderCount = 0; HeaderCount < ObjLength; HeaderCount++)
                    {
                        headers[totalcount] = b[HeaderCount];
                        totalcount++;
                    }
                }
            }
            return headers;
        }
    }
}
