using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Common.Save
{
    [Serializable()] 
    public class Citems
    {
        private string m_Name;
        private ControlType m_ConType;
        private int m_Left;
        private int m_Top;
        private int m_Width;
        private int m_Height;
        private Color m_ForeColor;
        private Color m_BackColor;
        private Image m_BackgroundImage;
        private ImageLayout m_BackgroundImageLayout;
        private string m_Value;

        private Font m_Font;
        private BorderStyle m_BorderStyle;
        private System.Drawing.ContentAlignment m_ValueAlign;

        private int m_ChildIndex;

        private CAniCollection m_aitems = new CAniCollection();

        private string m_PLink = "";

        private string m_Kind_Code ="";
        private string m_tag_id = "";
        private string m_tag_name = "";
        private string m_tag_desc = "";
        private string m_tag_unit = "";
        private string m_tag_type = "";
        private string m_sif_id = "";
        private string m_tag_ttype = "";
        private string m_tag_objid = "";
        private string m_tag_objid2 = "";
        private string m_tag_objid3 = "";
        private string m_tag_objid4 = "";
        private string m_tag_value1 = "";
        private string m_tag_value2 = "";
        private string m_tag_value3 = "";

        private string m_tag_lo = "";
        private string m_tag_lolo = "";
        private string m_tag_hi = "";
        private string m_tag_hihi = "";

        private string m_tag_on = "";
        private string m_tag_off = "";

        private string m_tag_limithi = "";
        private string m_tag_limitlo = "";

        private string m_tag_addr = "";
        private string m_tag_initval = "";

        private string m_tag_use_cos = "";
        private string m_tag_use_trend = "";

        private bool m_tag_is_single = false;
        private string m_tag_single_value = "";

        // 2016.10.19. Add Park
        private string m_sConn = "";
        private string m_sServerIP = ".";
        private string m_sDatabase = "";
        private string m_sUser = "";
        private string m_sPassword = "";
        private string m_sTableName = "";
        private string m_sQuery = "";
        private string m_sDateField = "";
        private string m_sDateFormat = "";
        private string m_sOrderby = "";
        private ConnectType m_CType = ConnectType.Table;
        private SearchType m_SType = SearchType.Day;

        private ActionModeType m_ActType =  ActionModeType.Custom;

        private bool m_protect = false;

        public ControlType ConType
        {
            get { return m_ConType; }
            set { m_ConType = value; }
        }

        public ActionModeType ActType
        {
            get { return m_ActType; }
            set { m_ActType = value; }
        }

        public int ChildIndex
        {
            get { return m_ChildIndex; }
            set { m_ChildIndex = value; }
        }

        public Citems()
        {

        }

        public Citems(BaseControl Bcontrol)
        {
            m_Name = Bcontrol.Name;
            m_ConType = Bcontrol.ConType;

            m_Value = Bcontrol.Value;
            m_Font = Bcontrol.Font;
            m_BorderStyle = Bcontrol.BorderStyle;
            m_ValueAlign = Bcontrol.ValueAlign;
            
            m_Left = Bcontrol.Left;
            m_Top = Bcontrol.Top;
            m_Width = Bcontrol.Width;
            m_Height = Bcontrol.Height;
            m_ForeColor = Bcontrol.ForeColor;
            m_BackColor = Bcontrol.BackColor;
            m_BackgroundImage = Bcontrol.BackgroundImage;
            m_BackgroundImageLayout = Bcontrol.BackgroundImageLayout;

            m_aitems = Bcontrol.AItems;
            m_PLink = Bcontrol.PageLink;

            m_Kind_Code = Bcontrol.Kind_Code;
            m_tag_id = Bcontrol.tag_id;
            m_tag_name = Bcontrol.tag_name;
            m_tag_desc = Bcontrol.tag_desc;
            m_tag_unit = Bcontrol.tag_unit;
            m_tag_type = Bcontrol.tag_type;
            m_sif_id = Bcontrol.sif_id;
            m_tag_ttype = Bcontrol.tag_ttype;
            m_tag_objid = Bcontrol.tag_objid;
            m_tag_objid2 = Bcontrol.tag_objid2;
            m_tag_objid3 = Bcontrol.tag_objid3;
            m_tag_objid4 = Bcontrol.tag_objid4;
            m_tag_value1 = Bcontrol.tag_value1;
            m_tag_value2 = Bcontrol.tag_value2;
            m_tag_value3 = Bcontrol.tag_value3;
            m_ActType = Bcontrol.ActType;

            m_tag_lo = Bcontrol.Lo;
            m_tag_lolo = Bcontrol.Lolo;
            m_tag_hi = Bcontrol.Hi;
            m_tag_hihi = Bcontrol.Hihi;
            m_tag_on = Bcontrol.On;
            m_tag_off = Bcontrol.Off;
            m_tag_limithi = Bcontrol.LimitHi;
            m_tag_limitlo = Bcontrol.LimitLo;
            m_tag_addr = Bcontrol.Addr;
            m_tag_initval = Bcontrol.tag_initval;

            m_tag_use_cos = Bcontrol.tag_use_cos;
            m_tag_use_trend = Bcontrol.tag_use_trend;

            m_tag_is_single = Bcontrol.tag_is_single;
            m_tag_single_value = Bcontrol.tag_single_value;

            m_protect = Bcontrol.Protect;

            // 2016.10.19. Add Park
            m_sConn = Bcontrol.sConn;
            m_sServerIP = Bcontrol.sServerIP;
            m_sDatabase = Bcontrol.sDatabase;
            m_sUser = Bcontrol.sUser;
            m_sPassword = Bcontrol.sPassword;
            m_sTableName = Bcontrol.sTableName;
            m_sQuery = Bcontrol.sQuery;
            m_sDateField = Bcontrol.sDateField;
            m_sDateFormat = Bcontrol.sDateFormat;
            m_sOrderby = Bcontrol.sOrderby;
            m_CType = Bcontrol.CType;
            m_SType = Bcontrol.SType;
         }

        public void GetControl(BaseControl Bcontrol)
        {
            Bcontrol.Name = m_Name;

            Bcontrol.Value = m_Value;
            Bcontrol.Font = m_Font;
            Bcontrol.BorderStyle = m_BorderStyle;
            Bcontrol.ValueAlign = m_ValueAlign;

            Bcontrol.Left = m_Left;
            Bcontrol.Top = m_Top;
            Bcontrol.Width = m_Width;
            Bcontrol.Height = m_Height;
            Bcontrol.ForeColor = m_ForeColor;
            Bcontrol.BackColor = m_BackColor;
            Bcontrol.BackgroundImage = m_BackgroundImage;
            Bcontrol.BackgroundImageLayout = m_BackgroundImageLayout;

            Bcontrol.AItems     = m_aitems;
            Bcontrol.PageLink   = m_PLink;

            Bcontrol.Kind_Code  = m_Kind_Code;
            Bcontrol.tag_id     = m_tag_id;
            Bcontrol.tag_name   = m_tag_name;
            Bcontrol.tag_desc   = m_tag_desc;
            Bcontrol.tag_unit   = m_tag_unit;
            Bcontrol.tag_type   = m_tag_type;
            Bcontrol.sif_id     = m_sif_id;
            Bcontrol.tag_ttype  = m_tag_ttype;
            Bcontrol.tag_objid  = m_tag_objid;
            Bcontrol.tag_objid2 = m_tag_objid2;
            Bcontrol.tag_objid3 = m_tag_objid3;
            Bcontrol.tag_objid4 = m_tag_objid4;
            Bcontrol.tag_value1 = m_tag_value1;
            Bcontrol.tag_value2 = m_tag_value2;
            Bcontrol.tag_value3 = m_tag_value3;

            Bcontrol.ActType = m_ActType;

            Bcontrol.Lo         = m_tag_lo;
            Bcontrol.Lolo       = m_tag_lolo;
            Bcontrol.Hi         = m_tag_hi;
            Bcontrol.Hihi       = m_tag_hihi;
            Bcontrol.On         = m_tag_on;
            Bcontrol.Off        = m_tag_off;
            Bcontrol.LimitHi    = m_tag_limithi;
            Bcontrol.LimitLo    = m_tag_limitlo;
            Bcontrol.Addr       = m_tag_addr;
            Bcontrol.tag_initval = m_tag_initval;

            Bcontrol.tag_use_cos = m_tag_use_cos;
            Bcontrol.tag_use_trend = m_tag_use_trend;

            Bcontrol.tag_is_single = m_tag_is_single;
            Bcontrol.tag_single_value = m_tag_single_value;

            Bcontrol.Protect = m_protect;

            // 2016.10.19. Add Park
            Bcontrol.sServerIP = m_sServerIP;
            Bcontrol.sDatabase = m_sDatabase;
            Bcontrol.sUser = m_sUser;
            Bcontrol.sPassword = m_sPassword;
            Bcontrol.sTableName = m_sTableName;
            Bcontrol.sQuery = m_sQuery;
            Bcontrol.sDateField = m_sDateField;
            Bcontrol.sDateFormat = m_sDateFormat;
            Bcontrol.sOrderby = m_sOrderby;
            Bcontrol.CType = m_CType;
            Bcontrol.SType = m_SType;
        }

    }

    public enum ActionModeType
    {
        AnalogHiLo,
        AnalogPercent,
        DigitalText,
        DigitalImage,
        Custom
    }
    
}
