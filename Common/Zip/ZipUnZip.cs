using System.Collections.Generic;
using System.Text;
using System;
using System.IO;

namespace Common
{
    public class ZipUnZip
    {
        public UInt32 ZipDataLen;
        public UInt32 UnZipDataLen;

        public byte[] Zip(byte[] value)
        {
            ZipDataLen = 0;
            byte[] byteArray = new byte[value.Length];
            //Prepare for compress
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.IO.Compression.GZipStream sw = new System.IO.Compression.GZipStream(ms,
                System.IO.Compression.CompressionMode.Compress);

            //Compress
            sw.Write(value, 0, value.Length);
            sw.Close();

            //Transform byte[] zip data to string
            byteArray = ms.GetBuffer();
            ms.Close();
            sw.Dispose();
            ms.Dispose();

            ZipDataLen = (uint)byteArray.Length;

            return byteArray;
        }

        public byte[] UnZip(byte[] value, int orglen)
        {
            UnZipDataLen = 0;
            //Prepare for decompress
            System.IO.MemoryStream ms = new System.IO.MemoryStream(value);
            System.IO.Compression.GZipStream sr = new System.IO.Compression.GZipStream(ms,
                System.IO.Compression.CompressionMode.Decompress);

            byte[] byteArray = new byte[orglen];

            //Decompress
            int rByte = sr.Read(byteArray, 0, orglen);

            sr.Close();
            ms.Close();
            sr.Dispose();
            ms.Dispose();

            UnZipDataLen = (uint)byteArray.Length;

            return byteArray;
        }
    }
}
