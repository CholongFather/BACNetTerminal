using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace WooriSI.DAC
{
    public class ViewNavigation
    {
        private SQLHelper _agent;

        /// <summary>
        /// TagLevel 생성자
        /// </summary>
        /// <param name="agent"></param>
        public ViewNavigation(SQLHelper agent)
        {
            _agent = agent;
        }

        /// <summary>
        ///테이블 InterfaceList(테이블명 없음)의 데이터 SELECT
        /// </summary>
        /// <returns>테이블 InterfaceList(테이블명 없음)의 결과를 DataSet 으로 반환</returns>
        public DataSet GetData()
        {
            DataSet rtn = new DataSet();
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("SELECT * FROM InterfaceList ");
                sb.Append(" order by Page_Number ");
                rtn = _agent.ExecuteDataset(sb.ToString());
                rtn.Tables[0].TableName = "InterfaceList";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rtn;
        }


        /// <summary>
        ///테이블 InterfaceList(테이블명 없음)의 데이터 등록처리
        /// <param name="fparas">FMParameters 파라메타 콜렉션입니다</param>
        /// </summary>
        /// <returns>테이블 InterfaceList(테이블명 없음)의 등록처리 결과를 true/false로 반환</returns>
        public bool InsertDB(FMParameters fparas)
        {
            bool rtn;
            int cnt;

            try
            {
                cnt = _agent.ExecuteNonQuery("sp_ViewAction_insert", fparas.Parameters);
                if (cnt > 0)
                {
                    rtn = true;
                    string keystr = "";
                    keystr = keystr + fparas.Parameter("@If_Id").Value.ToString() + "/";
                    //ComUtil.LogInsert("InterfaceList", "", "InterfaceListTx", "InsertDB", keystr, cnt, "성공", HttpContext.Current.Session["User_ID"].ToString());
                }
                else
                {
                    rtn = false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rtn;
        }

        /// <summary>
        ///테이블 InterfaceList(테이블명 없음)의 데이터 수정처리
        /// <param name="fparas">FMParameters 파라메타 콜렉션입니다</param>
        /// </summary>
        /// <returns>테이블 InterfaceList(테이블명 없음)의 수정처리 결과를 true/false로 반환</returns>
        public bool UpdateDB(FMParameters fparas)
        {
            bool rtn;
            int cnt;

            try
            {
                cnt = _agent.ExecuteNonQuery("sp_ViewAction_update", fparas.Parameters);
                if (cnt > 0)
                {
                    rtn = true;
                    string keystr = "";
                    keystr = keystr + fparas.Parameter("@If_Id").Value.ToString() + "/";
                    //ComUtil.LogUpdate("InterfaceList", "", "InterfaceListTx", "UpdateDB", keystr, cnt, "성공", HttpContext.Current.Session["User_ID"].ToString());
                }
                else
                {
                    rtn = false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rtn;
        }

        /// <summary>
        ///테이블 InterfaceList(테이블명 없음)의 데이터 삭제처리
        /// </summary>
        /// <param name="If_Id"></param>
        /// <returns>테이블 InterfaceList(테이블명 없음)의 삭제결과를 true/false로 반환</returns>
        public bool DeleteDB(string nNumber)
        {
            bool rtn;
            int cnt;
            FMParameters fparas = new FMParameters();

            try
            {
                fparas.AddParameter("@Page_Number", nNumber, 20, ParameterDirection.Input);
                cnt = _agent.ExecuteNonQuery("sp_ViewAction_delete", fparas.Parameters);
                if (cnt > 0)
                {
                    rtn = true;
                    //ComUtil.LogDelete("InterfaceList", "", "InterfaceListTx", "DeleteDB", fparas.GetKeyString(), cnt, "성공", HttpContext.Current.Session["User_ID"].ToString());
                }
                else
                {
                    rtn = false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rtn;
        }
    }
}
