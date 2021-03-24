using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true)]

namespace Common
{
    public class TagValue
    {
        // public const 및 static readonly 멤버변수 

        public static readonly IntPtr InvalidHandleValue = new IntPtr(-1);
        public const UInt32 FILE_MAP_WRITE = 2;
        public const UInt32 FILE_MAP_READ = 4;
        public const UInt32 PAGE_READWRITE = 0x04;

        // extern 메서드

        [DllImport("Kernel32", CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateFileMapping(IntPtr hFile,
            IntPtr pAttributes, UInt32 flProtect,
            UInt32 dwMaximumSizeHigh, UInt32 dwMaximumSizeLow, String pName);

        [DllImport("Kernel32", CharSet = CharSet.Unicode)]
        public static extern IntPtr OpenFileMapping(UInt32 dwDesiredAccess,
            Boolean bInheritHandle, String name);

        [DllImport("Kernel32", CharSet = CharSet.Unicode)]
        public static extern Boolean CloseHandle(IntPtr handle);

        [DllImport("Kernel32", CharSet = CharSet.Unicode)]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject,
            UInt32 dwDesiredAccess,
            UInt32 dwFileOffsetHigh, UInt32 dwFileOffsetLow,
            UInt32 dwNumberOfBytesToMap);

        [DllImport("Kernel32", CharSet = CharSet.Unicode)]
        public static extern Boolean UnmapViewOfFile(IntPtr address);

        [DllImport("Kernel32", CharSet = CharSet.Unicode)]
        public static extern IntPtr OpenMutex(UInt32 dwDesiredAccess, bool bInheritHandle, String pName);

        [DllImport("Kernel32", CharSet = CharSet.Unicode)]
        public static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

        [DllImport("Kernel32", CharSet = CharSet.Unicode)]
        public static extern bool ReleaseMutex(IntPtr hHandle);

        // public 멤버변수

        public IntPtr m_hFileMapR = IntPtr.Zero;
        public IntPtr m_hFileMap = IntPtr.Zero;
        public IntPtr hShmMutex = IntPtr.Zero;

        // private 멤버변수

        // 각각의 프로그램 정보및 watchdog 정보 저장
        private int n_MemStartPos = 1000;

        // public 메서드

        public double ReadMemByTagId(int TagId)
        {
            double retVal = -88888;

            if (m_hFileMapR == IntPtr.Zero)
            {
                m_hFileMapR = OpenFileMapping(FILE_MAP_READ, true, "SharedMemory");
            }

            if (m_hFileMapR == IntPtr.Zero)
            {
                return -99999;
            }

            if (hShmMutex == IntPtr.Zero)
            {
                hShmMutex = OpenMutex((UInt32)(0x000F0000L | 0x00100000L | 0x0001), false, "SharedMemory-MTX");
            }

            if (hShmMutex != IntPtr.Zero)
            {
                try
                {
                    if (WaitForSingleObject(hShmMutex, 100) == 0)
                    {
                        IntPtr address = MapViewOfFile(m_hFileMapR, FILE_MAP_READ, 0, 0, 0);
                        if (address != IntPtr.Zero)
                        {
                            try
                            {
                                long nStartPos = TagId * 8 + n_MemStartPos;
                                Byte[] buffer = new Byte[8];

                                unsafe
                                {
                                    for (Int32 x = 0; x < buffer.Length; x++)
                                    {
                                        buffer[x] = *((Byte*)address + x + nStartPos);
                                    }
                                }

                                retVal = BitConverter.ToDouble(buffer, 0);
                            }
                            finally
                            {
                                UnmapViewOfFile(address);
                            }
                        }
                    }
                }
                finally
                {
                    ReleaseMutex(hShmMutex);
                }
            }

            return retVal;
        }

        public byte[] ReadBuffByMem(int nStartPos, int ReadCnt)
        {
            Byte[] rtn = null;

            try
            {
                if (m_hFileMapR == IntPtr.Zero)
                {
                    m_hFileMapR = OpenFileMapping(FILE_MAP_READ, true, "SharedMemory");
                }

                if (m_hFileMapR == IntPtr.Zero)
                {
                    return null;
                }

                if (hShmMutex == IntPtr.Zero)
                {
                    hShmMutex = OpenMutex((UInt32)(0x000F0000L | 0x00100000L | 0x0001), false, "SharedMemory-MTX");
                }

                if (hShmMutex != IntPtr.Zero)
                {
                    try
                    {
                        if (WaitForSingleObject(hShmMutex, 100) == 0)
                        {
                            IntPtr address = MapViewOfFile(m_hFileMapR, FILE_MAP_READ, 0, 0, 0);
                            if (address != IntPtr.Zero)
                            {
                                try
                                {
                                    byte[] buffer = new Byte[ReadCnt];
                                    unsafe
                                    {
                                        for (Int32 x = 0; x < buffer.Length; x++)
                                        {
                                            buffer[x] = *((Byte*)address + nStartPos + x);
                                        }
                                    }
                                    rtn = buffer;
                                }
                                finally
                                {
                                    UnmapViewOfFile(address);
                                }
                            }
                        }
                    }
                    finally
                    {
                        ReleaseMutex(hShmMutex);
                    }
                }

            }
            catch (Exception e)
            {
                Log.log("ReadBuffByMem " + e.Message);
            }

            return rtn;
        }

        public bool WriteMemByTagId(int TagId, double Val)
        {
            bool rtn = false;

            if (m_hFileMap == IntPtr.Zero)
            {
                m_hFileMap = OpenFileMapping(FILE_MAP_WRITE, true, "SharedMemory");
            }

            if (m_hFileMap == IntPtr.Zero)
            {
                return false;
            }

            if (hShmMutex == IntPtr.Zero)
            {
                hShmMutex = OpenMutex((UInt32)(0x000F0000L | 0x00100000L | 0x0001), false, "SharedMemory-MTX");
            }

            if (hShmMutex != IntPtr.Zero)
            {
                try
                {
                    if (WaitForSingleObject(hShmMutex, 100) == 0)
                    {
                        IntPtr address = MapViewOfFile(m_hFileMap, FILE_MAP_WRITE, 0, 0, 0);
                        if (address != IntPtr.Zero)
                        {
                            try
                            {
                                long nStartPos = TagId * 8 + n_MemStartPos;
                                Byte[] buffer = BitConverter.GetBytes(Val);

                                unsafe
                                {
                                    for (Int32 x = 0; x < buffer.Length; x++)
                                    {
                                        *((Byte*)address + nStartPos + x) = buffer[x];
                                    }
                                }

                                rtn = true;
                            }
                            finally
                            {
                                UnmapViewOfFile(address);
                            }
                        }
                    }
                }
                finally
                {
                    ReleaseMutex(hShmMutex);
                }
            }

            return rtn;
        }

        public bool WriteMemByBuff(int nStartPos, byte[] buff, int Cnt)
        {
            bool rtn = false;

            if (m_hFileMap == IntPtr.Zero)
            {
                m_hFileMap = OpenFileMapping(FILE_MAP_WRITE, true, "SharedMemory");
            }

            if (m_hFileMap == IntPtr.Zero)
            {
                return false;
            }

            if (hShmMutex == IntPtr.Zero)
            {
                hShmMutex = OpenMutex((UInt32)(0x000F0000L | 0x00100000L | 0x0001), false, "SharedMemory-MTX");
            }

            if (hShmMutex != IntPtr.Zero)
            {
                try
                {
                    if (WaitForSingleObject(hShmMutex, 100) == 0)
                    {
                        IntPtr address = MapViewOfFile(m_hFileMap, FILE_MAP_WRITE, 0, 0, 0);
                        if (address != IntPtr.Zero)
                        {
                            try
                            {
                                unsafe
                                {
                                    for (Int32 x = 0; x < Cnt; x++)
                                    {
                                        *((Byte*)address + nStartPos + x) = buff[x];
                                    }
                                }

                                rtn = true;
                            }
                            finally
                            {
                                UnmapViewOfFile(address);
                            }
                        }
                    }
                }
                finally
                {
                    ReleaseMutex(hShmMutex);
                }
            }

            return rtn;
        }

        public bool WriteWatchdogByPid(int ProgramNo, int nVal)
        {
            bool rtn = false;

            if (m_hFileMap == IntPtr.Zero)
            {
                m_hFileMap = OpenFileMapping(FILE_MAP_WRITE, true, "SharedMemory");
            }

            if (m_hFileMap == IntPtr.Zero)
            {
                return false;
            }

            if (hShmMutex == IntPtr.Zero)
            {
                hShmMutex = OpenMutex((UInt32)(0x000F0000L | 0x00100000L | 0x0001), false, "SharedMemory-MTX");
            }

            if (hShmMutex != IntPtr.Zero)
            {
                try
                {
                    if (WaitForSingleObject(hShmMutex, 100) == 0)
                    {
                        IntPtr address = MapViewOfFile(m_hFileMap, FILE_MAP_WRITE, 0, 0, 0);
                        if (address != IntPtr.Zero)
                        {
                            try
                            {
                                long nStartPos = ProgramNo * 5;
                                byte[] buffer = BitConverter.GetBytes(nVal);

                                unsafe
                                {
                                    for (Int32 x = 0; x < buffer.Length; x++)
                                    {
                                        *((Byte*)address + nStartPos + x) = buffer[x];
                                    }
                                }

                                rtn = true;
                            }
                            finally
                            {
                                UnmapViewOfFile(address);
                            }
                        }
                    }
                }
                finally
                {
                    ReleaseMutex(hShmMutex);
                }
            }

            return rtn;
        }

        public int ReadWatchdogByPid(int ProgramNo)
        {
            // TODO : 실패시 반환값 확인 필요. (OpenFileMapping 실패시 -1, 그외 실패시 0 으로 되어있었음)
            int rtn = 0;

            if (m_hFileMapR == IntPtr.Zero)
            {
                m_hFileMapR = OpenFileMapping(FILE_MAP_READ, true, "SharedMemory");
            }

            if (m_hFileMapR == IntPtr.Zero)
            {
                return -1;
            }

            if (hShmMutex == IntPtr.Zero)
            {
                hShmMutex = OpenMutex((UInt32)(0x000F0000L | 0x00100000L | 0x0001), false, "SharedMemory-MTX");
            }

            if (hShmMutex != IntPtr.Zero)
            {
                try
                {
                    if (WaitForSingleObject(hShmMutex, 100) == 0)
                    {
                        IntPtr address = MapViewOfFile(m_hFileMapR, FILE_MAP_READ, 0, 0, 0);
                        if (address != IntPtr.Zero)
                        {
                            try
                            {
                                long StartPos = ProgramNo * 5;
                                Byte[] buffer = new Byte[4];

                                unsafe
                                {
                                    for (Int32 x = 0; x < buffer.Length; x++)
                                    {
                                        buffer[x] = *((Byte*)address + x + StartPos);
                                    }
                                }

                                int retVal = BitConverter.ToInt32(buffer, 0);
                                rtn = retVal;
                            }
                            finally
                            {
                                UnmapViewOfFile(address);
                            }
                        }
                    }
                }
                finally
                {
                    ReleaseMutex(hShmMutex);
                }
            }

            return rtn;
        }

        public bool WriteWatchdogByPid(int ProgramNo, int nHandle, byte bStatus)
        {
            bool rtn = false;

            if (m_hFileMap == IntPtr.Zero)
            {
                m_hFileMap = OpenFileMapping(FILE_MAP_WRITE, true, "SharedMemory");
            }

            if (m_hFileMap == IntPtr.Zero)
            {
                return false;
            }

            if (hShmMutex == IntPtr.Zero)
            {
                hShmMutex = OpenMutex((UInt32)(0x000F0000L | 0x00100000L | 0x0001), false, "SharedMemory-MTX");
            }

            if (hShmMutex != IntPtr.Zero)
            {
                try
                {
                    if (WaitForSingleObject(hShmMutex, 100) == 0)
                    {
                        IntPtr address = MapViewOfFile(m_hFileMap, FILE_MAP_WRITE, 0, 0, 0);
                        if (address != IntPtr.Zero)
                        {
                            try
                            {
                                long nStartPos = ProgramNo * 5;
                                byte[] buffer = BitConverter.GetBytes(nHandle);

                                unsafe
                                {
                                    for (Int32 x = 0; x < buffer.Length; x++)
                                    {
                                        *((Byte*)address + nStartPos + x) = buffer[x];
                                    }

                                    *((Byte*)address + nStartPos + buffer.Length) = bStatus;
                                }

                                rtn = true;
                            }
                            finally
                            {
                                UnmapViewOfFile(address);
                            }
                        }
                    }
                }
                finally
                {
                    ReleaseMutex(hShmMutex);
                }
            }

            return rtn;
        }

        public int ReadWatchdogByPid(int ProgramNo, ref byte bStatus)
        {
            // TODO : 실패시 반환값 확인 필요. (OpenFileMapping 실패시 -1, 그외 실패시 0 으로 되어있었음)
            int rtn = 0;

            if (m_hFileMapR == IntPtr.Zero)
            {
                m_hFileMapR = OpenFileMapping(FILE_MAP_READ, true, "SharedMemory");
            }

            if (m_hFileMapR == IntPtr.Zero)
            {
                return -1;
            }

            if (hShmMutex == IntPtr.Zero)
            {
                hShmMutex = OpenMutex((UInt32)(0x000F0000L | 0x00100000L | 0x0001), false, "SharedMemory-MTX");
            }

            if (hShmMutex != IntPtr.Zero)
            {
                try
                {
                    if (WaitForSingleObject(hShmMutex, 100) == 0)
                    {
                        IntPtr address = MapViewOfFile(m_hFileMapR, FILE_MAP_READ, 0, 0, 0);
                        if (address != IntPtr.Zero)
                        {
                            try
                            {
                                long nStartPos = ProgramNo * 5;
                                Byte[] buffer = new Byte[4];

                                unsafe
                                {
                                    for (Int32 x = 0; x < buffer.Length; x++)
                                    {
                                        buffer[x] = *((Byte*)address + x + nStartPos);
                                    }
                                    bStatus = *((Byte*)address + buffer.Length + nStartPos);
                                }

                                int retVal = BitConverter.ToInt32(buffer, 0);
                                rtn = retVal;
                            }
                            finally
                            {
                                UnmapViewOfFile(address);
                            }
                        }
                    }
                }
                finally
                {
                    ReleaseMutex(hShmMutex);
                }
            }

            return rtn;
        }
    }
}
