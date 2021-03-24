using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    [Serializable()] 
    public class ActivityItems
    {
        //시나리오코드
        private string m_Scenario_Code;
        //상황코드
        private string m_Situation_Code;
        //액션코드
        private string m_Action_Code;
        //액티비티코드
        private string m_Activity_Code;


        //포인트ID
        private string m_Tag_Id;

        //동작타입
        private string m_Tag_Type;

        //동작타입
        private string m_RestoreYN;

        //동작값
        private double m_Tag_Value;


        //원복포인트ID
        private string m_RestoreTag_Id;

        //원복동작타입
        private string m_RestoreTag_Type;

        //원복동작값
        private double m_RestoreTag_Value;

        //사용여부
        private string m_UseYN;

        public ActivityItems()
        {

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

        public string ActivityCode
        {
            get { return m_Activity_Code; }
            set { m_Activity_Code = value; }
        }

        public string Tagid
        {
            get { return m_Tag_Id; }
            set { m_Tag_Id = value; }
        }

        public string TagType
        {
            get { return m_Tag_Type; }
            set { m_Tag_Type = value; }
        }

        public string RestoreYN
        {
            get { return m_RestoreYN; }
            set { m_RestoreYN = value; }
        }

        public double TagValue
        {
            get { return m_Tag_Value; }
            set { m_Tag_Value = value; }
        }

        public string RestoreTagid
        {
            get { return m_RestoreTag_Id; }
            set { m_RestoreTag_Id = value; }
        }

        public string RestoreTagType
        {
            get { return m_RestoreTag_Type; }
            set { m_RestoreTag_Type = value; }
        }

        public double RestoreTagValue
        {
            get { return m_RestoreTag_Value; }
            set { m_RestoreTag_Value = value; }
        }

        public string UseYN
        {
            get { return m_UseYN; }
            set { m_UseYN = value; }
        }
    }
}
