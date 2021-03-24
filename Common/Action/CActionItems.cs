using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Security;

namespace Common
{
    [Serializable()] 
    public class CActionItems
    {
        private string m_SnA_id;
        private string m_Tagid;
        private ActionAType m_AType;
        private ActionCType m_CType;
        private Double m_Value;
        private int m_nActionTime;
        public DateTime m_tAcTime;
        private string m_RunAppPath;
        private string m_Arguments;

        public CActionItems()
        {

        }

        public CActionItems(string SnA_id, ActionCType CType, ActionAType AType, Double Value, string Tagid, int ActionTime, string RunAppPath, string Arguments)
        {
            m_SnA_id = SnA_id;
            m_CType = CType;
            m_AType = AType;
            m_Value = Value;
            m_Tagid = Tagid;
            m_nActionTime = ActionTime;
            m_RunAppPath = RunAppPath;
            m_Arguments = Arguments;
        }

        public string SnA_id
        {
            get { return m_SnA_id; }
            set { m_SnA_id = value; }
        }

        public ActionCType CType
        {
            get { return m_CType; }
            set { m_CType = value; }
        }

        public ActionAType AType
        {
            get { return m_AType; }
            set { m_AType = value; }
        }

        public Double Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        public string Tagid
        {
            get { return m_Tagid; }
            set { m_Tagid = value; }
        }

        public DateTime AcTime
        {
            get { return m_tAcTime; }
            set { m_tAcTime = value; }
        }

        public int ActionTime
        {
            get { return m_nActionTime; }
            set { m_nActionTime = value; }
        }

        public string RunAppPath
        {
            get { return m_RunAppPath; }
            set { m_RunAppPath = value; }
        }

        public string Arguments
        {
            get { return m_Arguments; }
            set { m_Arguments = value; }
        }        
    }

    public enum ActionCType
    {
        Equal,
        Bigger,
        Lower
    }

    public enum ActionAType
    {
        Compare,
        DeviceOutput,
        TagValOutput,
        RunApp,
        Others
    }
}
