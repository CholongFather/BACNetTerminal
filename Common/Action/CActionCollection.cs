using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Common
{
    [Serializable()]
    public class CActionCollection : Collection<CActionItems>    
    {
        private int m_nRooptime;
        private DateTime m_tActionTime;

        public int Rooptime
        {
            get { return m_nRooptime; }
            set { m_nRooptime = value; }
        }

        public DateTime ActionTime
        {
            get { return m_tActionTime; }
            set { m_tActionTime = value; }
        }

        public CActionCollection()
        {

        }
    }
}
