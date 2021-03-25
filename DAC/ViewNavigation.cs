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
        /// TagLevel ������
        /// </summary>
        /// <param name="agent"></param>
        public ViewNavigation(SQLHelper agent)
        {
            _agent = agent;
        }

        /// <summary>
        ///���̺� InterfaceList(���̺�� ����)�� ������ SELECT
        /// </summary>
        /// <returns>���̺� InterfaceList(���̺�� ����)�� ����� DataSet ���� ��ȯ</returns>
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
        ///���̺� InterfaceList(���̺�� ����)�� ������ ���ó��
        /// <param name="fparas">FMParameters �Ķ��Ÿ �ݷ����Դϴ�</param>
        /// </summary>
        /// <returns>���̺� InterfaceList(���̺�� ����)�� ���ó�� ����� true/false�� ��ȯ</returns>
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
                    //ComUtil.LogInsert("InterfaceList", "", "InterfaceListTx", "InsertDB", keystr, cnt, "����", HttpContext.Current.Session["User_ID"].ToString());
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
        ///���̺� InterfaceList(���̺�� ����)�� ������ ����ó��
        /// <param name="fparas">FMParameters �Ķ��Ÿ �ݷ����Դϴ�</param>
        /// </summary>
        /// <returns>���̺� InterfaceList(���̺�� ����)�� ����ó�� ����� true/false�� ��ȯ</returns>
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
                    //ComUtil.LogUpdate("InterfaceList", "", "InterfaceListTx", "UpdateDB", keystr, cnt, "����", HttpContext.Current.Session["User_ID"].ToString());
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
        ///���̺� InterfaceList(���̺�� ����)�� ������ ����ó��
        /// </summary>
        /// <param name="If_Id"></param>
        /// <returns>���̺� InterfaceList(���̺�� ����)�� ��������� true/false�� ��ȯ</returns>
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
                    //ComUtil.LogDelete("InterfaceList", "", "InterfaceListTx", "DeleteDB", fparas.GetKeyString(), cnt, "����", HttpContext.Current.Session["User_ID"].ToString());
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
