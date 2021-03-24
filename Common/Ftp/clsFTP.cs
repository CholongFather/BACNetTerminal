using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;
using Microsoft.VisualBasic; 

namespace Common
{
    /// <summary>
    /// FTP 클래스
    /// </summary>
    public class clsFTP
    {
        #region "Main Class Variable Declarations" 
        private string m_sRemoteHost; 
        private string m_sRemotePath; 
        private string m_sRemoteUser; 
        private string m_sRemotePassword; 
        private string m_sMess; 
        private Int32 m_iRemotePort; 
        private Int32 m_iBytes; 
        private Socket m_objClientSocket; 
        
        private Int32 m_iRetValue; 
        private bool m_bLoggedIn; 
        // Change to loggedIn 
        private string m_sMes; 
        private string m_sReply; 
        
        /// <summary>
        /// Set the size of the packet that is used to read and 
        /// write data to the FTP Server to the spcified size below.
        /// </summary>
        public const int BLOCK_SIZE = 4096; 
        private byte[] m_aBuffer = new byte[BLOCK_SIZE + 1]; 
        private Encoding ASCII = Encoding.ASCII; 
        
        // General variables 
        private string m_sMessageString; 
        #endregion 
        
        #region "Class Constructors" 

        /// <summary>
        /// Main class constructor. 
        /// </summary>
        public clsFTP() 
        { 
            m_sRemoteHost = "microsoft"; 
            m_sRemotePath = "."; 
            m_sRemoteUser = "anonymous"; 
            m_sRemotePassword = ""; 
            m_sMessageString = ""; 
            m_iRemotePort = 21; 
            m_bLoggedIn = false; 
        } 
        
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="FTPConnectionString">연결문자열</param>
        public clsFTP(string FTPConnectionString) 
        { 
            SetFTPValue(FTPConnectionString); 
        } 
        
        /// <summary>
        /// Parametized constructor. 
        /// </summary>
        /// <param name="sRemoteHost">서버주소</param>
        /// <param name="sRemotePath">경로</param>
        /// <param name="sRemoteUser">사용자</param>
        /// <param name="sRemotePassword">비밀번호</param>
        /// <param name="iRemotePort">접속포트</param>
        public clsFTP(string sRemoteHost, string sRemotePath, string sRemoteUser, string sRemotePassword, Int32 iRemotePort) 
        { 
            m_sRemoteHost = sRemoteHost; 
            m_sRemotePath = sRemotePath; 
            m_sRemoteUser = sRemoteUser; 
            m_sRemotePassword = sRemotePassword; 
            m_sMessageString = ""; 
            m_iRemotePort = iRemotePort; 
            m_bLoggedIn = false; 
        } 
        #endregion 
        
        #region "Public Properties" 
        
        /// <summary>
        /// FTP_CONNECTION_STATES
        /// </summary>
        public enum FTP_CONNECTION_STATES 
        { 
            FTP_CONNECTION_RESOLVING_HOST, 
            FTP_CONNECTION_HOST_RESOLVED, 
            FTP_CONNECTION_CONNECTED, 
            FTP_CONNECTION_AUTHENTICATION, 
            FTP_USER_LOGGED, 
            FTP_ESTABLISHING_DATA_CONNECTION, 
            FTP_DATA_CONNECTION_ESTABLISHED, 
            FTP_RETRIEVING_DIRECTORY_INFO, 
            FTP_DIRECTORY_INFO_COMPLETED, 
            FTP_TRANSFER_STARTING, 
            FTP_TRANSFER_COMLETED 
        } 
        
        public event StateChangedEventHandler StateChanged; 
        public delegate void StateChangedEventHandler(FTP_CONNECTION_STATES state); 
        public event FileTransferProgressEventHandler FileTransferProgress; 
        public delegate void FileTransferProgressEventHandler(long lCurrentBytes, long lTotalBytes); 
        
        /// <summary>
        /// Set/Get the name of the FTP Server. 
        /// </summary>
        public string RemoteHost { 
            get { return m_sRemoteHost; } 
            set { m_sRemoteHost = value; } 
        } 
        
        /// <summary>
        /// Set/Get the FTP Port Number. 
        /// </summary>
        public Int32 RemotePort { 
            get { return m_iRemotePort; } 
            set { m_iRemotePort = value; } 
        } 
                
        /// <summary>
        /// Set/Get the remote path. 
        /// </summary>
        public string RemotePath { 
            get { return m_sRemotePath; } 
            set { m_sRemotePath = value; } 
        }         

        /// <summary>
        /// Set the remote password. 
        /// </summary>
        public string RemotePassword { 
            get { return m_sRemotePassword; } 
            set { m_sRemotePassword = value; } 
        } 
        
        /// <summary>
        /// Set/Get the remote user. 
        /// </summary>
        public string RemoteUser { 
            get { return m_sRemoteUser; } 
            set { m_sRemoteUser = value; } 
        }         

        /// <summary>
        /// Set the class messagestring. 
        /// </summary>
        public string MessageString { 
            get { return m_sMessageString; } 
            set { m_sMessageString = value; } 
        } 
        
        #endregion 
        
        #region "Public Subs and Functions" 

        /// <summary>
        /// 파일의 존재여부확인
        /// </summary>
        /// <param name="fname">파일명</param>
        /// <returns>결과값</returns>
        public bool GetFileExists(string fname) 
        { 
            string[] mess = null; 
            int i = 0; 
            
            mess = GetFileList(""); 
            
            for (i = 0; i <= mess.Length - 1; i++) { 
                //MsgBox("mess:" & mess(i) & "-" & fname) 
                if (mess[i].Trim() == fname.Trim()) { 
                    //MsgBox("같음") 
                    //GetFileExists = true; 
                    return true; 
                } 
            } 
            return false;              
        } 

        /// <summary>
        /// Return a list of files within a string() array from the file system. 
        /// </summary>
        /// <param name="sMask">마스크</param>
        /// <returns>결과</returns>
        public string[] GetFileList(string sMask) 
        { 
            Socket cSocket = default(Socket); 
            Int32 bytes = default(Int32); 
            char seperator = ControlChars.Lf; 
            string[] mess = null; 
            
            m_sMes = ""; 
            if ((!(m_bLoggedIn))) { 
                Login(); 
            } 
            
            cSocket = CreateDataSocket(); 
            SendCommand("NLST " + sMask); 
            
            if ((!(m_iRetValue == 150 | m_iRetValue == 125))) { 
                MessageString = m_sReply; 
                throw new IOException(m_sReply.Substring(4)); 
            } 
            
            m_sMes = ""; 
            while ((true)) { 
                //m_aBuffer.Clear(m_aBuffer, 0, m_aBuffer.Length);
                m_aBuffer.Initialize();
                bytes = cSocket.Receive(m_aBuffer, m_aBuffer.Length, 0); 
                m_sMes += ASCII.GetString(m_aBuffer, 0, bytes); 
                
                if ((bytes < m_aBuffer.Length)) { 
                    break; // TODO: might not be correct. Was : Exit Do 
                } 
            } 
            
            mess = m_sMes.Split(seperator); 
            cSocket.Close(); 
            ReadReply(); 
            
            if ((m_iRetValue != 226)) { 
                MessageString = m_sReply; 
                throw new IOException(m_sReply.Substring(4)); 
            } 
            
            return mess; 
        }         

        /// <summary>
        /// Get the size of the file on the FTP Server. 
        /// </summary>
        /// <param name="sFileName">파일명</param>
        /// <returns>파일크기</returns>
        public long GetFileSize(string sFileName) 
        { 
            long size = 0; 
            
            if ((!(m_bLoggedIn))) { 
                Login(); 
            } 
            
            SendCommand("SIZE " + sFileName); 
            size = 0; 
            
            if ((m_iRetValue == 213)) { 
                size = Int64.Parse(m_sReply.Substring(4)); 
            } 
            else { 
                MessageString = m_sReply; 
                throw new IOException(m_sReply.Substring(4)); 
            } 
            
            return size; 
        }         

        /// <summary>
        /// Log into the FTP Server. 
        /// </summary>
        /// <returns></returns>
        public bool Login() 
        { 
            m_objClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 
            IPEndPoint ep = new IPEndPoint(Dns.GetHostEntry(m_sRemoteHost).AddressList[0], m_iRemotePort); 
            
            try { 
                m_objClientSocket.Connect(ep); 
            } 
            catch (Exception ex) { 
                MessageString = m_sReply; 
                throw new IOException("Couldn't connect to remote server" + ex.Message ); 
            } 
            
            ReadReply(); 
            if ((m_iRetValue != 220)) { 
                CloseConnection(); 
                MessageString = m_sReply; 
                throw new IOException(m_sReply.Substring(4)); 
            } 
            
            SendCommand("USER " + m_sRemoteUser); 
            if ((!(m_iRetValue == 331 | m_iRetValue == 230))) { 
                Cleanup(); 
                MessageString = m_sReply; 
                throw new IOException(m_sReply.Substring(4)); 
            } 
            
            if ((m_iRetValue != 230)) { 
                SendCommand("PASS " + m_sRemotePassword); 
                if ((!(m_iRetValue == 230 | m_iRetValue == 202))) { 
                    Cleanup(); 
                    MessageString = m_sReply; 
                    throw new IOException(m_sReply.Substring(4)); 
                } 
            } 
            
            m_bLoggedIn = true; 
            ChangeDirectory(m_sRemotePath); 
            
            // Return the end result. 
            return m_bLoggedIn; 
        } 
                
        /// <summary>
        /// If the value of mode is true, set binary mode for downloads. Else, set Ascii mode. 
        /// </summary>
        /// <param name="bMode">모드값</param>
        public void SetBinaryMode(bool bMode) 
        {             
            if ((bMode)) { 
                SendCommand("TYPE I"); 
            } 
            else { 
                SendCommand("TYPE A"); 
            } 
            
            if ((m_iRetValue != 200)) { 
                MessageString = m_sReply; 
                throw new IOException(m_sReply.Substring(4)); 
            } 
        }         

        /// <summary>
        /// Download a file to the Assembly's local directory, 
        /// keeping the same file name. 
        /// </summary>
        /// <param name="sFileName">파일명</param>
        public void DownloadFile(string sFileName) 
        { 
            DownloadFile(sFileName, "", false); 
        }

        /// <summary> 
        /// Download a remote file to the Assembly's local 
        /// directory, keeping the same file name, and set 
        /// the resume flag. 
        /// </summary>
        public void DownloadFile(string sFileName, bool bResume) 
        { 
            DownloadFile(sFileName, "", bResume); 
        }

        /// <summary>
        /// Download a remote file to a local file name which can 
        /// include a path. The local file name will be created or 
        /// overwritten, but the path must exist. 
        /// </summary>
        public void DownloadFile(string sFileName, string sLocalFileName) 
        { 
            DownloadFile(sFileName, sLocalFileName, false); 
        }

        /// <summary> 
        // Download a remote file to a local file name which can 
        // include a path, and set the resume flag. The local file 
        // name will be created or overwritten, but the path must 
        // exist. 
        /// </summary>
        public void DownloadFile(string sFileName, string sLocalFileName, bool bResume) 
        { 
            FtpWebRequest reqFTP;
            try
            {
                FileStream outputStream = new FileStream(sLocalFileName, FileMode.Create);

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + m_sRemoteHost + "/" + sFileName));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(m_sRemoteUser, m_sRemotePassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];

                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }

                ftpStream.Close();
                outputStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Log.log("Ftp Download fail!! " + ex.Message);
            }

            if (StateChanged != null)
            {
                StateChanged(FTP_CONNECTION_STATES.FTP_TRANSFER_COMLETED);
            }
        } 
        
        // Upload a file. 
        public void UploadFile(string sFileName) 
        { 
            UploadFile(sFileName, "", false); 
        } 
        
        // Upload a file and set the resume flag. 
        public void UploadFile(string sFileName, string dFileName, bool bResume) 
        { 
            Socket cSocket = default(Socket); 
            long offset = 0; 
            FileStream input = default(FileStream); 
            bool bFileNotFound = false; 
            
            //추가부분 
            long sum = 0; 
            long size = 0; 
            sum = 0; 
            
            if ((!(m_bLoggedIn))) { 
                Login(); 
            } 
            
            cSocket = CreateDataSocket(); 
            offset = 0; 
            
            if ((bResume)) { 
                try { 
                    SetBinaryMode(true); 
                    offset = GetFileSize(sFileName); 
                } 
                catch (Exception ex) { 
                    offset = 0; 
                } 
            } 
            
            if ((offset > 0)) { 
                SendCommand("REST " + offset); 
                if ((m_iRetValue != 350)) { 
                    //throw new IOException(reply.Substring(4)); 
                    //Remote server may not support resuming. 
                    offset = 0; 
                } 
            }             
            
            if (dFileName.Trim().Length > 0) { 
                SendCommand("STOR " + dFileName); 
            } 
            else { 
                SendCommand("STOR " + Path.GetFileName(sFileName)); 
            } 
            
            if ((!(m_iRetValue == 125 | m_iRetValue == 150))) { 
                MessageString = m_sReply; 
                throw new IOException(m_sReply.Substring(4)); 
            } 
            
            // Check to see if the file exists before the upload. 
            bFileNotFound = false; 
            if ((File.Exists(sFileName))) 
            { 
                // Open input stream to read source file 
                input = new FileStream(sFileName, FileMode.Open); 
                
                //추가부분 
                size = input.Length; 
                // 
                if ((offset != 0)) { 
                    input.Seek(offset, SeekOrigin.Begin); 
                } 
                
                // Upload the file 
                m_iBytes = input.Read(m_aBuffer, 0, m_aBuffer.Length); 
                while ((m_iBytes > 0)) { 
                    
                    cSocket.Send(m_aBuffer, m_iBytes, 0); 
                    m_iBytes = input.Read(m_aBuffer, 0, m_aBuffer.Length); 
                    
                    //추가부분 
                    sum = sum + m_iBytes; 
                    if (FileTransferProgress != null) { 
                        FileTransferProgress(sum, size); 
                    } 
                } 
                
                input.Close(); 
            } 
            else { 
                bFileNotFound = true; 
            } 
            
            if ((cSocket.Connected)) { 
                cSocket.Close(); 
            } 
            
            // No point in reading the return value if the file was 
            // not found. 
            if ((bFileNotFound)) { 
                MessageString = m_sReply; 
                throw new IOException("The file: " + sFileName + " was not found. Can not upload the file to the FTP Site."); 
            } 
            
            ReadReply(); 
            if ((!(m_iRetValue == 226 | m_iRetValue == 250))) { 
                MessageString = m_sReply; 
                throw new IOException(m_sReply.Substring(4)); 
            } 
            
            //추가부분 
            if (StateChanged != null) { 
                StateChanged(FTP_CONNECTION_STATES.FTP_TRANSFER_COMLETED); 
            } 
        } 
        
        // 
        // Delete a file from the remote FTP server. 
        public bool DeleteFile(string sFileName) 
        { 
            bool bResult = false; 
            
            bResult = true; 
            if ((!(m_bLoggedIn))) { 
                Login(); 
            } 
            
            SendCommand("DELE " + sFileName); 
            if ((m_iRetValue != 250)) { 
                bResult = false; 
                MessageString = m_sReply; 
            } 
            
            // Return the final result. 
            return bResult; 
        } 
        
        // 
        // Rename a file on the remote FTP server. 
        public bool RenameFile(string sOldFileName, string sNewFileName) 
        { 
            bool bResult = false; 
            
            bResult = true; 
            if ((!(m_bLoggedIn))) { 
                Login(); 
            } 
            
            SendCommand("RNFR " + sOldFileName); 
            if ((m_iRetValue != 350)) { 
                MessageString = m_sReply; 
                throw new IOException(m_sReply.Substring(4)); 
            } 
            
            // known problem 
            // rnto will not take care of existing file. 
            // i.e. It will overwrite if newFileName exist 
            SendCommand("RNTO " + sNewFileName); 
            if ((m_iRetValue != 250)) { 
                MessageString = m_sReply; 
                throw new IOException(m_sReply.Substring(4)); 
            } 
            
            return bResult; 
        } 
        
        // 
        // Create a directory on the remote FTP server. 
        public bool CreateDirectory(string sDirName) 
        { 
            bool bResult = false; 
            
            bResult = true; 
            if ((!(m_bLoggedIn))) { 
                Login(); 
            } 
            
            SendCommand("MKD " + sDirName); 
            if ((m_iRetValue != 257)) { 
                bResult = false; 
                MessageString = m_sReply; 
            } 
            
            // Return the final result. 
            return bResult; 
        } 
        
        // 
        // Delete a directory on the remote FTP server. 
        public bool RemoveDirectory(string sDirName) 
        { 
            bool bResult = false; 
            
            bResult = true; 
            if ((!(m_bLoggedIn))) { 
                Login(); 
            } 
            
            SendCommand("RMD " + sDirName); 
            if ((m_iRetValue != 250)) { 
                bResult = false; 
                MessageString = m_sReply; 
            } 
            
            // Return the final result. 
            return bResult; 
        } 
        
        // 
        // Change the current working directory on the remote FTP 
        // server. 
        public bool ChangeDirectory(string sDirName) 
        { 
            bool bResult = false; 
            
            bResult = true; 
            if ((sDirName.Equals("."))) {
                return bResult; 
            } 
            
            if ((!(m_bLoggedIn))) { 
                Login(); 
            } 
            
            SendCommand("CWD " + sDirName); 
            if ((m_iRetValue != 250)) { 
                bResult = false; 
                MessageString = m_sReply; 
            } 
            
            this.m_sRemotePath = sDirName; 
            
            // Return the final result. 
            return bResult; 
        } 
        
        // 
        // Close the FTP connection. 
        public void CloseConnection() 
        { 
            if (((m_objClientSocket != null))) { 
                SendCommand("QUIT"); 
            } 
            
            Cleanup(); 
        } 
        
        private void SetFTPValue(string FTPConnectionString) 
        { 
            string[] ftpvalue = null; 
            int i = 0; 
            
            ftpvalue = FTPConnectionString.Split(new char[] {';'}); 
            
            for (i = 0; i <= ftpvalue.Length  - 1; i++) { 
                string ftitle = null; 
                string fvalue = null; 
                int myPos = 0; 
                myPos = ftpvalue[i].IndexOf("="); 
                ftitle = Strings.Mid(ftpvalue[i], 1, myPos - 1); 
                fvalue = Strings.Mid(ftpvalue[i], myPos + 1, Strings.Len(ftpvalue[i]) - myPos); 
                switch (ftitle) { 
                    case "Server": 
                        m_sRemoteHost = fvalue; 
                        break; 
                    case "UID": 
                        m_sRemoteUser = fvalue; 
                        break; 
                    case "PWD": 
                        m_sRemotePassword = fvalue; 
                        break; 
                    case "Port": 
                        m_iRemotePort = Convert.ToInt32(fvalue); 
                        break; 
                } 
            } 
            
            m_sRemotePath = "."; 
            m_sMessageString = ""; 
            m_bLoggedIn = false;             
        } 
        
        #endregion 
        
        #region "Private Subs and Functions" 
        // 
        // Read the reply from the FTP Server 
        private void ReadReply() 
        { 
            m_sMes = ""; 
            m_sReply = ReadLine(false); 
            m_iRetValue = Int32.Parse(m_sReply.Substring(0, 3)); 
        } 
        
        // 
        // Clean up some variables. 
        private void Cleanup() 
        { 
            if ((m_objClientSocket != null)) { 
                m_objClientSocket.Close(); 
                m_objClientSocket = null; 
            } 
            
            m_bLoggedIn = false; 
        } 
        
        // 
        // Read a line from the server. 
        private string ReadLine(bool bClearMes)
        { 
            char seperator = ControlChars.Lf; 
            string[] mess = null; 
            
            if ((bClearMes)) { 
                m_sMes = ""; 
            } 
            while ((true)) { 
                //m_aBuffer.Clear(m_aBuffer, 0, BLOCK_SIZE);
                m_aBuffer.Initialize();
                m_iBytes = m_objClientSocket.Receive(m_aBuffer, m_aBuffer.Length, 0); 
                m_sMes += ASCII.GetString(m_aBuffer, 0, m_iBytes); 
                if ((m_iBytes < m_aBuffer.Length)) { 
                    break; // TODO: might not be correct. Was : Exit Do 
                } 
            } 
            
            mess = m_sMes.Split(seperator); 
            if ((m_sMes.Length > 2)) { 
                m_sMes = mess[mess.Length - 2]; 
            } 
            else { 
                m_sMes = mess[0]; 
            } 
            
            if ((!(m_sMes.Substring(3, 1).Equals(" ")))) { 
                return ReadLine(true); 
            } 
            
            return m_sMes; 
        } 
        
        // 
        // Send a command to the FTP Server. 
        private void SendCommand(string sCommand) 
        { 
            sCommand = sCommand + ControlChars.CrLf; 
            byte[] cmdbytes = ASCII.GetBytes(sCommand); 
            
            m_objClientSocket.Send(cmdbytes, cmdbytes.Length, 0); 
            ReadReply(); 
        } 
        
        // 
        // Create a Data socket. 
        private Socket CreateDataSocket() 
        { 
            Int32 index1 = default(Int32); 
            Int32 index2 = default(Int32); 
            Int32 len = default(Int32); 
            Int32 partCount = default(Int32); 
            Int32 i = default(Int32); 
            Int32 port = default(Int32); 
            string ipData = null; 
            string buf = null; 
            string ipAddress = null; 
            Int32[] parts = new Int32[7]; 
            char ch = '\0'; 
            Socket s = default(Socket); 
            IPEndPoint ep = default(IPEndPoint); 
            
            SendCommand("PASV"); 
            if ((m_iRetValue != 227)) { 
                MessageString = m_sReply; 
                throw new IOException(m_sReply.Substring(4)); 
            } 
            
            index1 = m_sReply.IndexOf("("); 
            index2 = m_sReply.IndexOf(")"); 
            ipData = m_sReply.Substring(index1 + 1, index2 - index1 - 1); 
            
            len = ipData.Length; 
            partCount = 0; 
            buf = ""; 
            
            for (i = 0; (i<=(len - 1) && partCount <= 6); i++) { 
                ch = char.Parse(ipData.Substring(i, 1)); 
                if ((char.IsDigit(ch))) { 
                    buf += ch; 
                } 
                else if ((ch != ',')) { 
                    MessageString = m_sReply; 
                    throw new IOException("Malformed PASV reply: " + m_sReply); 
                } 
                
                if (((ch == ',') || (i + 1 == len))) { 
                    try { 
                        parts[partCount] = Int32.Parse(buf); 
                        partCount += 1; 
                        buf = ""; 
                    } 
                    catch (Exception ex) { 
                        MessageString = m_sReply; 
                        throw new IOException("Malformed PASV reply: " + m_sReply); 
                    } 
                } 
            } 
            
            ipAddress = parts[0] + "." + parts[1] + "." + parts[2] + "." + parts[3]; 

            // multiply the number by 2 to the power of 8. 
            port = parts[4] * Convert.ToInt32(Math.Pow(2, 8)); 
            
            // Determine the data port number. 
            port = port + parts[5]; 
            
            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 
            ep = new IPEndPoint(Dns.GetHostEntry(ipAddress).AddressList[0], port); 
            
            try { 
                s.Connect(ep); 
            } 
            catch (Exception ex) { 
                MessageString = m_sReply; 
                throw new IOException("Can't connect to remote server"); 
            } 
            
            return s; 
        } 
        
        #endregion 
        
        public bool FTPConnect() 
        { 
            try { 
                if ((Login())) { 
                    
                    return true; 
                } 
                else { 
                    return false; 
                } 
            } 
            catch (Exception ex) {
                return false; 
            } 
        } 


    }
}
