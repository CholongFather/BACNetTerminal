using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Common
{
    public static class Log
    {
        //private static StringBuilder msg;
        private static string szFolder = "\\SystemLog";

        /// <summary>
        /// Data로그
        /// </summary>
        public static void log(string logMsg)
        {
            try
            {
                string currentdirectory = Directory.GetCurrentDirectory();
                string s_LogFilePath = currentdirectory + szFolder;

                if (!Directory.Exists(s_LogFilePath))
                {
                    Directory.CreateDirectory(s_LogFilePath);
                }

                string sFile = s_LogFilePath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";

                // This text is added only once to the file.
                if (!File.Exists(sFile))
                {
                    using (StreamWriter sw = File.CreateText(sFile))
                    { }
                }

                using (StreamWriter sw = File.AppendText(sFile))
                {
                    // Add some text to the file.
                    sw.Write(System.DateTime.Now.ToString("HH:mm:ss : "));
                    sw.WriteLine(logMsg);
                    sw.Close();
                }
            }
            catch (Exception PrintLogExecption)
            {
                //ExceptionLog(PrintLogExecption);
            }
        } //End Print_Log Method

        public static void log(Exception ex)
        {
            string logMsg=
                @"
                ------------------------------------------
                예외설명:" + ex.Message + @"
                예외스택:
                " + ex.StackTrace + @"
                예외소스:" + ex.Source + @"
                ------------------------------------------
                ";
            log(logMsg);
        }

        ///// <summary>
        ///// 1분데이터 로그
        ///// </summary>
        ///// <param name="analysisOneMinuteData"></param>
        ///// <param name="timerequest"></param>
        //public static void LogAnalysisOneMinuteData(ReceiveDataOneMinute analysisOneMinuteData, DateTime timerequest)
        //{
        //    StringBuilder s = new StringBuilder(string.Empty);

        //    s.Append("OBJECTID 갯수는 총 " + analysisOneMinuteData.ObjectIdCount.ToString() + "입니다\t\r\n");
        //    s.Append("요청시간 " + timerequest.Hour.ToString() + "시 " + timerequest.Minute.ToString() + "분\r\n");
        //    for (int j = 0; j < analysisOneMinuteData.ObjectIdCount; j++)
        //    {
        //        s.Append("ObjetctID:");
        //        s.Append(analysisOneMinuteData.objectid[j].ToString() + "\t");
        //        s.Append("ObjectValue:");
        //        s.Append(analysisOneMinuteData.objectvalue[j].ToString() + "\r\n");
        //    }
         
        //    Data_Log(s.ToString());
        //}

        ///// <summary>
        /////  예외로그
        ///// </summary>
        ///// <param name="e"></param>
        //public static void ExceptionLog(Exception e)
        //{
        //    msg = new StringBuilder(string.Empty);
        //    msg.Append("\r\n------------------------------------------\r\n");
        //    msg.Append("예외설명:");
        //    msg.Append(e.Message);
        //    msg.Append("\r\n예외스택:");
        //    msg.Append(e.StackTrace);
        //    msg.Append("\r\n예외소스:");
        //    msg.Append(e.Source);
        //    msg.Append("\r\n------------------------------------------\r\n");
        //    Execute_Log(msg.ToString());
        //}

        ///// <summary>
        ///// 실행로그
        ///// </summary>
        //public static void Execute_Log(string logMsg)
        //{
        //    try
        //    {
        //        string s_LogFilePath = string.Empty;
        //        string path = string.Empty;

        //        if (string.IsNullOrEmpty(ICUFacade.ICUNumber))
        //        {
        //            s_LogFilePath = Environment.CurrentDirectory + "\\" + "ICU_" + "LOG\\";
        //            path = @s_LogFilePath + "ICU_" + System.DateTime.Now.ToString("yyyyMMdd") + "_실행.log";
        //        }
        //        else
        //        {
        //            s_LogFilePath = Environment.CurrentDirectory + "\\" + ICUFacade.ICUNumber.ToString() + "번_ICU_" + "LOG\\";
        //            path = @s_LogFilePath + ICUFacade.ICUNumber.ToString() + "번_ICU_" + System.DateTime.Now.ToString("yyyyMMdd") + "_실행.log";
        //        }

        //        if (!Directory.Exists(s_LogFilePath))
        //        {
        //            Directory.CreateDirectory(s_LogFilePath);
        //        }

        //        //로그파일 저장
        //        // This text is added only once to the file.
        //        if (!File.Exists(path))
        //        {
        //            // Create a file to write to.
        //            using (StreamWriter sw = File.CreateText(path))
        //            { }
        //        }

        //        using (StreamWriter sw = File.AppendText(path))
        //        {
        //            // Add some text to the file.
        //            sw.Write(System.DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss "));
        //            sw.WriteLine(logMsg);
        //            sw.Close();
        //        }

        //    }
        //    catch (Exception PrintLogExecption)
        //    {
        //        ExceptionLog(PrintLogExecption);
        //    }
        //} //End Print_Log Method

    
        ///// <summary>
        ///// Data로그초기화
        ///// </summary>
        //public static void Init_Data_Log()
        //{
        //    try
        //    {
        //        string s_LogFilePath = string.Empty;
        //        string path = string.Empty;

        //        if (string.IsNullOrEmpty(ICUFacade.ICUNumber))
        //        {
        //            s_LogFilePath = Environment.CurrentDirectory + "\\" + "ICU_" + "LOG\\";
        //            path = @s_LogFilePath + "ICU_Temp_Data.log";
        //        }
        //        else
        //        {
        //            s_LogFilePath = Environment.CurrentDirectory + "\\" + ICUFacade.ICUNumber.ToString() + "번_ICU_" + "LOG\\";
        //            path = @s_LogFilePath + ICUFacade.ICUNumber.ToString() + "번_ICU_Temp_Data.log";
        //        }

        //        if (!Directory.Exists(s_LogFilePath))
        //        {
        //            return;
        //        }

        //        if (!File.Exists(path))
        //        {
        //            return;
        //        }

        //        File.WriteAllText(path, "");
                
        //    }
        //    catch (Exception PrintLogExecption)
        //    {
        //        ExceptionLog(PrintLogExecption);
        //    }
        //} //End Print_Log Method

        

    }
}
