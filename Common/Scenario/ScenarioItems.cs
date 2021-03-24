using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    [Serializable()] 
    public class ScenarioItems
    {
        //시나리오코드
        private string m_Scenario_Code;

        //시나리오명
        private string m_Scenario_Name;

        //시나리오 타입
        private string m_Scenario_Type;

        //시나리오 설명
        private string m_Scenario_Note;

        //시나리오 상태
        private string m_Scenario_Status;

        //시나리오 사용여부
        private string m_UseYN;

        private SituationCollection m_Situation = new SituationCollection();


        public ScenarioItems()
        {

        }

        public SituationCollection Situations
        {
            get { return m_Situation; }
            set { m_Situation = value; }
        }

        public string ScenarioCode
        {
            get { return m_Scenario_Code; }
            set { m_Scenario_Code = value; }
        }

        public string Name
        {
            get { return m_Scenario_Name; }
            set { m_Scenario_Name = value; }
        }

        public string Type
        {
            get { return m_Scenario_Type; }
            set { m_Scenario_Type = value; }
        }

        public string Note
        {
            get { return m_Scenario_Note; }
            set { m_Scenario_Note = value; }
        }

        public string Status
        {
            get { return m_Scenario_Status; }
            set { m_Scenario_Status = value; }
        }

        public string UseYN
        {
            get { return m_UseYN; }
            set { m_UseYN = value; }
        }

    }
}
