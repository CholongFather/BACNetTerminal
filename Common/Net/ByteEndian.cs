using System;

namespace Common.Net
{
    /// <summary>
    ///      [0] [1] [2] [3]
    ///     +---+---+---+---+
    ///     | 4 | 3 | 2 | 1 |
    ///     +---+---+---+---+
    ///  
    ///        +----+----+
    /// big    | 21 | 43 |
    ///        +----+----+
    ///     
    ///        +----+----+
    /// little | 43 | 21 |
    ///        +----+----+
    /// </summary>
    interface IByteEndian
    {
        UInt32 GetUint32(byte[] array, int begin);
        UInt32 GetUint24(byte[] array, int begin);
        UInt16 GetUint16(byte[] array, int begin);

        void SetUint32(byte[] array, int begin, UInt32 values);
        void SetUint24(byte[] array, int begin, UInt32 values);
        void SetUint16(byte[] array, int begin, UInt16 values);
    }

    /// <summary>
    /// 리틀 엔디언은 작은 단위의 바이트가 앞에 오는 방법
    /// </summary>
    class LittleEndianByte : IByteEndian
    {
        public UInt32 GetUint32(byte[] array, int begin)
        {
            if (array.Length < (begin + 4))
                throw new InvalidOperationException("배열이 작아 값을 읽지 못했습니다.");

            return (UInt32)(array[begin + 3] << 24 |
                       array[begin + 2] << 16 |
                       array[begin + 1] << 8 |
                       array[begin + 0]) & UInt32.MaxValue/*0xFFFFFFFF*/;
        }

        public UInt32 GetUint24(byte[] array, int begin)
        {
            if (array.Length < (begin + 3))
                throw new InvalidOperationException("배열이 작아 값을 읽지 못했습니다.");

            return (UInt32)(array[begin + 2] << 16 |
                       array[begin + 1] << 8 |
                       array[begin + 0]) & (UInt32.MaxValue >> 8)/*0xFFFFFFFF*/;
        }

        public UInt16 GetUint16(byte[] array, int begin)
        {
            if (array.Length < (begin + 2))
                throw new InvalidOperationException("배열이 작아 값을 읽지 못했습니다.");

            return (UInt16)(array[begin + 1] << 8 | array[begin + 0])
                /*& UInt16.MaxValue(UInt16)0xFFFF*/;
        }

        public void SetUint32(byte[] array, int begin, UInt32 values)
        {
            if (array.Length < (begin + 4))
                throw new InvalidOperationException("배열이 작아 값을 쓰지 못했습니다.");

            byte[] ordered = BitConverter.GetBytes(values);
            array[begin] = ordered[0];
            array[begin + 1] = ordered[1];
            array[begin + 2] = ordered[2];
            array[begin + 3] = ordered[3];
        }

        public void SetUint24(byte[] array, int begin, UInt32 values)
        {
            if (array.Length < (begin + 3))
                throw new InvalidOperationException("배열이 작아 값을 쓰지 못했습니다.");

            byte[] ordered = BitConverter.GetBytes(values);
            array[begin] = ordered[0];
            array[begin + 1] = ordered[1];
            array[begin + 2] = ordered[2];
        }

        public void SetUint16(byte[] array, int begin, UInt16 values)
        {
            if (array.Length < (begin + 2))
                throw new InvalidOperationException("배열이 작아 값을 쓰지 못했습니다.");

            byte[] ordered = BitConverter.GetBytes(values);
            array[begin] = ordered[0];
            array[begin + 1] = ordered[1];
        }
    }

    /// <summary>
    /// 빅 엔디언은 사람이 숫자를 쓰는 방법과 같이 큰 단위의 바이트가 앞에 오는 방법
    /// </summary>
    class BigEndianByte : IByteEndian
    {
        public UInt32 GetUint32(byte[] array, int begin)
        {
            if (array.Length < (begin + 4))
                throw new InvalidOperationException("배열이 작아 값을 읽지 못했습니다.");

            return (UInt32)(array[begin + 0] << 24 |
                       array[begin + 1] << 16 |
                       array[begin + 2] << 8 |
                       array[begin + 3]) & UInt32.MaxValue/*0xFFFFFFFF*/;
        }

        public UInt32 GetUint24(byte[] array, int begin)
        {
            if (array.Length < (begin + 3))
                throw new InvalidOperationException("배열이 작아 값을 읽지 못했습니다.");

            return (UInt32)(array[begin + 0] << 16 |
                       array[begin + 1] << 8 |
                       array[begin + 2]) & (UInt32.MaxValue >> 8)/*0xFFFFFFFF*/;
        }

        public UInt16 GetUint16(byte[] array, int begin)
        {
            if (array.Length < (begin + 2))
                throw new InvalidOperationException("배열이 작아 값을 읽지 못했습니다.");

            return (UInt16)(array[begin + 0] << 8 | array[begin + 1])
                /*& UInt16.MaxValue(UInt16)0xFFFF*/;
        }

        public void SetUint32(byte[] array, int begin, UInt32 values)
        {
            if (array.Length < (begin + 4))
                throw new InvalidOperationException("배열이 작아 값을 쓰지 못했습니다.");

            byte[] ordered = BitConverter.GetBytes(values);
            array[begin] = ordered[3];
            array[begin + 1] = ordered[2];
            array[begin + 2] = ordered[1];
            array[begin + 3] = ordered[0];
        }

        public void SetUint24(byte[] array, int begin, UInt32 values)
        {
            if (array.Length < (begin + 3))
                throw new InvalidOperationException("배열이 작아 값을 쓰지 못했습니다.");

            byte[] ordered = BitConverter.GetBytes(values);
            array[begin] = ordered[2];
            array[begin + 1] = ordered[1];
            array[begin + 2] = ordered[0];
        }

        public void SetUint16(byte[] array, int begin, UInt16 values)
        {
            if (array.Length < (begin + 2))
                throw new InvalidOperationException("배열이 작아 값을 쓰지 못했습니다.");

            byte[] ordered = BitConverter.GetBytes(values);
            array[begin] = ordered[1];
            array[begin + 1] = ordered[0];
        }
    }

    public static class ByteEndian
    {
        private static readonly bool onReverse;
        private static readonly IByteEndian mEndianByte;

        /// <summary>
        /// 정적 클래스의 생성자
        /// 헤더를 빅엔디안(big endian)으로 통일하기 위해 시스템의 엔디안(endian)을 검사
        /// </summary>
        static ByteEndian()
        {
            if (BitConverter.IsLittleEndian)
            {
                /// intel architecture
                onReverse = true;
                mEndianByte = new LittleEndianByte();
            }
            else
            {
                /// network or arm architecture
                onReverse = false;
                mEndianByte = new BigEndianByte();
            }
        }

        static public UInt32 GetUint32(byte[] array, int begin)
        {
            return mEndianByte.GetUint32(array, begin);
        }

        static public UInt32 GetUint24(byte[] array, int begin)
        {
            return mEndianByte.GetUint24(array, begin);
        }

        static public UInt16 GetUint16(byte[] array, int begin)
        {
            return mEndianByte.GetUint16(array, begin);
        }

        static public void SetUint32(byte[] array, int begin, UInt32 values)
        {
            mEndianByte.SetUint32(array, begin, values);
        }

        static public void SetUint24(byte[] array, int begin, UInt32 values)
        {
            mEndianByte.SetUint24(array, begin, values);
        }

        static public void SetUint16(byte[] array, int begin, UInt16 values)
        {
            mEndianByte.SetUint16(array, begin, values);
        }
    }
}
