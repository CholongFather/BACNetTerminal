using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Common
{
    [Serializable()] 
    public class CTagItems
    {
        private string m_Value;
        private string m_OldValue;

        private string m_id = "";
        private string m_name = "";
        private string m_desc = "";
        private string m_code = "";
        private string m_if_id = "";
        private string m_type = "";
        private string m_addr = "";
        private string m_unit = "";
        private string m_initval = "";
        private string m_lolo = "";
        private string m_lo = "";
        private string m_hi = "";
        private string m_hihi = "";

        private string m_eng_lo = "";
        private string m_eng_hi = "";
        private string m_limit_lo = "";
        private string m_limit_hi = "";

        private string m_use_alarm = "";
        private string m_on_msg = "";
        private string m_off_msg = "";

        private string m_relay_time = "";
        private string m_di_alarm_type = "";

        private string m_objid = "";
        private string m_objid2 = "";
        private string m_objid3 = "";
        private string m_objid4 = "";
        private string m_format = "";
        private string m_value2 = "";
        private string m_value3 = "";

        private string m_use_cos = "";
        private string m_use_trend = "";

        private bool m_tag_is_single = false;
        private string m_tag_single_value = "";

        private int m_Status = 3;       

        public CTagItems()
        {

        }

        public double Value
        {
            get { return Convert.ToDouble( m_Value ); }
            set {
                m_OldValue = m_Value;
                m_Value = value.ToString(); 
            }
        }

        public double OldValue
        {
            get { return Convert.ToDouble(m_OldValue); }
            set
            {
                m_OldValue = value.ToString();
            }
        }

        public string Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public string Desc
        {
            get { return m_desc; }
            set { m_desc = value; }
        }

        public string Code
        {
            get { return m_code; }
            set { m_code = value; }
        }

        public string If_id
        {
            get { return m_if_id; }
            set { m_if_id = value; }
        }

        public string Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public string Addr
        {
            get { return m_addr; }
            set { m_addr = value; }
        }

        public string Unit
        {
            get { return m_unit; }
            set { m_unit = value; }
        }

        public int Initval
        {
            get { return Convert.ToInt32( m_initval ); }
            set { m_initval = value.ToString(); }
        }

        public int Lolo
        {
            get { return Convert.ToInt32(m_lolo); }
            set { m_lolo = value.ToString(); }
        }

        public int Lo
        {
            get { return Convert.ToInt32(m_lo); }
            set { m_lo = value.ToString(); }
        }

        public int Hi
        {
            get { return Convert.ToInt32(m_hi); }
            set { m_hi = value.ToString(); }
        }

        public int Hihi
        {
            get { return Convert.ToInt32(m_hihi); }
            set { m_hihi = value.ToString(); }
        }

        public int Eng_lo
        {
            get { return Convert.ToInt32(m_eng_lo); }
            set { m_eng_lo = value.ToString(); }
        }

        public int Eng_hi
        {
            get { return Convert.ToInt32(m_eng_hi); }
            set { m_eng_hi = value.ToString(); }
        }

        public int Limit_lo
        {
            get { return Convert.ToInt32(m_limit_lo); }
            set { m_limit_lo = value.ToString(); }
        }

        public int Limit_hi
        {
            get { return Convert.ToInt32(m_limit_hi); }
            set { m_limit_hi = value.ToString(); }
        }


        public string Use_alarm
        {
            get { return m_use_alarm; }
            set { m_use_alarm = value; }
        }

        public string On_msg
        {
            get { return m_on_msg; }
            set { m_on_msg = value; }
        }

        public string Off_msg
        {
            get { return m_off_msg; }
            set { m_off_msg = value; }
        }

        public string Relay_time
        {
            get { return m_relay_time; }
            set { m_relay_time = value; }
        }

        public string Di_alarm_type
        {
            get { return m_di_alarm_type; }
            set { m_di_alarm_type = value; }
        }

        public string Objid
        {
            get { return m_objid; }
            set { m_objid = value; }
        }

        public string Objid2
        {
            get { return m_objid2; }
            set { m_objid2 = value; }
        }

        public string Objid3
        {
            get { return m_objid3; }
            set { m_objid3 = value; }
        }

        public string Objid4
        {
            get { return m_objid4; }
            set { m_objid4 = value; }
        }

        public string Format
        {
            get { return m_format; }
            set { m_format = value; }
        }

        public string Value2
        {
            get { return m_value2; }
            set { m_value2 = value; }
        }

        public string Value3
        {
            get { return m_value3; }
            set { m_value3 = value; }
        }

        public string Use_cos
        {
            get { return m_use_cos; }
            set { m_use_cos = value; }
        }

        public string Use_trend
        {
            get { return m_use_trend; }
            set { m_use_trend = value; }
        }

        public bool Tag_is_single
        {
            get { return m_tag_is_single; }
            set { m_tag_is_single = value; }
        }

        public string Tag_single_value
        {
            get { return m_tag_single_value; }
            set { m_tag_single_value = value; }
        }

        public int Status
        {
            get { return m_Status; }
            set { m_Status = value; }
        }
        
    }
}
