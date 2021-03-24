using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace WooriSI.Common
{
    [Serializable()]
    public class SystemAlarm
    {
        public static ArrayList arrSysAlarm = new ArrayList();
        public strAlarm sAlarm;

        public int GetAlarmCount()
        {
            return arrSysAlarm.Count;
        }

        public void PutAlarm(int Tagid, Double Val, string Page)
        {
            sAlarm = new strAlarm();
            sAlarm.m_Tagid = Tagid;
            sAlarm.m_Value = Val;
            sAlarm.m_GoPage = Page;
            sAlarm.m_AlarmTime = DateTime.Now;
            arrSysAlarm.Add(sAlarm);
        }

        public void DelAlarm(int Tagid)
        {
            for (int i = 0; i < arrSysAlarm.Count; i++)
            {
                sAlarm = (strAlarm)arrSysAlarm[i];
                if (sAlarm.m_Tagid == Tagid)
                {
                    arrSysAlarm.RemoveAt(i);
                    break;
                }
            }
        }

        public strAlarm GetAlarmArray(int Tagid)
        {
            for (int i = 0; i < arrSysAlarm.Count; i++)
            {
                sAlarm = (strAlarm)arrSysAlarm[i];
                if (sAlarm.m_Tagid == Tagid)
                {
                    return sAlarm;
                }
            }

            return sAlarm;
        }

    }

    public struct strAlarm
    {
        public int m_Tagid;
        public double m_Value;
        public DateTime m_AlarmTime;
        public string m_GoPage;
    }
}
