using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Common;
using DAC;

namespace CommBACNetDNET
{
    static class Program
    {
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string className, string windowName);

        [DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("User32.dll")]
        public static extern void BringWindowToTop(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern void SetForegroundWindow(IntPtr hWnd);

        public static SQLHelper conSQL = new SQLHelper();

        public static ConfigMgr PjtBuilder;

        public static frmBACNetDNET fClient;

        public static string PjtName = "";

        public static string PjtPath = "";

        public static string szServerName, szDBName, szUserID, szPassword;

        public static string applicationName;

        public static string g_currentdirectory;

        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Thread.Sleep(100);

            IntPtr Handle = (IntPtr)FindWindow(null, "EngineClient");

            string nowExcuteName = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
            if (nowExcuteName.EndsWith(".vshost"))
            {
                nowExcuteName = nowExcuteName.Substring(0, nowExcuteName.Length - ".vshost".Length);
            }
            applicationName = nowExcuteName;

            bool createdAsNew = false;
            System.Threading.Mutex mutex = new System.Threading.Mutex(true, applicationName, out createdAsNew);

            if (createdAsNew)
            {
                g_currentdirectory = Directory.GetCurrentDirectory();
                PjtBuilder = new ConfigMgr(g_currentdirectory + "\\" + "Config.xml");

                // Config.xml 에서 프로젝트 정보를 읽어온다.
                PjtName = Program.PjtBuilder.GetConfigInfo("Project", "Name", "")[0];
                PjtPath = Program.PjtBuilder.GetConfigInfo("Project", "Path", "")[0];

                szServerName = Program.PjtBuilder.GetConfigInfo("DataInfo", "ServerName", "")[0];
                szDBName = Program.PjtBuilder.GetConfigInfo("DataInfo", "DBName", "")[0];
                szUserID = Program.PjtBuilder.GetConfigInfo("DataInfo", "UserID", "")[0];
                szPassword = Program.PjtBuilder.GetConfigInfo("DataInfo", "UserPwd", "")[0];

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                fClient = new frmBACNetDNET();
                Application.Run(fClient);
            }
            else
            {
                IntPtr ptr = IntPtr.Zero;
                ptr = FindWindow(null, applicationName);

                if (ptr != IntPtr.Zero)
                {
                    BringWindowToTop(ptr);
                    ShowWindow(ptr, 5);
                    SetForegroundWindow(ptr);
                }

                Application.Exit();
            }
        }
    }
}