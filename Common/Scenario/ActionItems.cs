using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    [Serializable()]
    public class ActionItems
    {
        //시나리오 코드
        private string m_Scenario_Code;
        //상황코드
        private string m_Situation_Code;
        //액션코드
        private string m_Action_Code;


        //액션명
        private string m_Action_Name;

        //액션타입
        private string m_Action_Type;

        //액션순서
        private int m_Action_Order;

        //액션 원복 순서
        private int m_Action_RestoreOrder;

        //사용여부
        private string m_UseYN;

        //지연시간
        private int m_Delay_Time;

        private ActivityCollection m_Activity = new ActivityCollection();

        public ActionItems()
        {

        }

        public ActivityCollection Activitys
        {
            get { return m_Activity; }
            set { m_Activity = value; }
        }

        public string ScenarioCode
        {
            get { return m_Scenario_Code; }
            set { m_Scenario_Code = value; }
        }

        public string SituationCode
        {
            get { return m_Situation_Code; }
            set { m_Situation_Code = value; }
        }

        public string ActionCode
        {
            get { return m_Action_Code; }
            set { m_Action_Code = value; }
        }

        public string Name
        {
            get { return m_Action_Name; }
            set { m_Action_Name = value; }
        }

        public string Type
        {
            get { return m_Action_Type; }
            set { m_Action_Type = value; }
        }

        public int Order
        {
            get { return m_Action_Order; }
            set { m_Action_Order = value; }
        }

        public int RestoreOrder
        {
            get { return m_Action_RestoreOrder; }
            set { m_Action_RestoreOrder = value; }
        }

        public string UseYN
        {
            get { return m_UseYN; }
            set { m_UseYN = value; }
        }

        public int DelayTime
        {
            get { return m_Delay_Time; }
            set { m_Delay_Time = value; }
        }

    }
}
