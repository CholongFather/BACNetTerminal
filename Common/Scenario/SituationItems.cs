using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    [Serializable()] 
    public class SituationItems
    {
        //시나리오코드
        private string m_Scenario_Code;

        //상황코드
        private string m_Situation_Code;

        //상황명
        private string m_Situation_Name;

        //원복여부
        private string m_Situation_RestoreYN;

        //포인트ID
        private string m_Tag_Id;

        //동작타입
        private string m_Tag_Type;

        //동작값
        private double m_Tag_Value;

        //동작조건
        private string m_Tag_Condition;

        //동작시간
        private int m_Delay_Time;

        //원복포인트ID
        private string m_RestoreTag_Id;

        //원복동작타입
        private string m_RestoreTag_Type;

        //원복동작값
        private double m_RestoreTag_Value;

        //원복동작조건
        private string m_RestoreTag_Condition;

        //원복동작시간
        private int m_RestoreDelay_Time;

        //사용여부
        private string m_UseYN; 

        private ActionCollection m_Action = new ActionCollection();

        //설정값 초과된 갯수
        private int m_ValueCount;

        //복구설정값 초과된 갯수
        private int m_RestoreValueCount;

        //현재값 저장 
        private string m_ValueTemp;

        //복구값 저장
        private string m_RestoreValueTemp;

        //복구상태
        private bool m_RestoreState = false;

        public SituationItems()
        {

        }

        public ActionCollection Actions
        {
            get { return m_Action; }
            set { m_Action = value; }
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

        public string Name
        {
            get { return m_Situation_Name; }
            set { m_Situation_Name = value; }
        }

        public string RestoreYN
        {
            get { return m_Situation_RestoreYN; }
            set { m_Situation_RestoreYN = value; }
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

        public double TagValue
        {
            get { return m_Tag_Value; }
            set { m_Tag_Value = value; }
        }

        public string TagCondition
        {
            get { return m_Tag_Condition; }
            set { m_Tag_Condition = value; }
        }

        public int DelayTime
        {
            get { return m_Delay_Time; }
            set { m_Delay_Time = value; }
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

        public string RestoreTagCondition
        {
            get { return m_RestoreTag_Condition; }
            set { m_RestoreTag_Condition = value; }
        }

        public int RestoreDelayTime
        {
            get { return m_RestoreDelay_Time; }
            set { m_RestoreDelay_Time = value; }
        }

        public string UseYN
        {
            get { return m_UseYN; }
            set { m_UseYN = value; }
        }

        public int ValueCount
        {
            get { return m_ValueCount; }
            set { m_ValueCount = value; }
        }

        public int RestoreValueCount
        {
            get { return m_RestoreValueCount; }
            set { m_RestoreValueCount = value; }
        }

        public string ValueTemp
        {
            get { return m_ValueTemp; }
            set { m_ValueTemp = value; }
        }

        public string RestoreValueTemp
        {
            get { return m_RestoreValueTemp; }
            set { m_RestoreValueTemp = value; }
        }

        public bool RestoreState
        {
            get { return m_RestoreState; }
            set { m_RestoreState = value; }
        }
    }
}
