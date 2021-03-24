//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Data;
//using System.Data.SqlClient;
//using System.Windows.Forms;
//using Microsoft.VisualBasic.Devices;
//using MySql.Data.MySqlClient;
//using Common;

//김남훈 MYSQL 사용시 Mysql.Data 버전에 맞게 가져오세요.
//namespace DAC
//{
//    public class MySQLHelper
//    {

//        private MySqlConnection conSQL = new MySqlConnection();

//        private string m_connString = "";

//        private string m_svrName = "";
//        private string m_dbName = "";
//        private string m_usrName = "";
//        private string m_usrPass = "";

//        /// <summary>
//        /// 생성자
//        /// </summary>
//        public MySQLHelper()
//        {

//        }

//        /// <summary>
//        /// 생성자
//        /// </summary>
//        /// <param name="ConnectionString">연결문자열</param>
//        public MySQLHelper(string ConnectionString)
//        {
//            m_connString = ConnectionString;
//            //conSQL.ConnectionString = m_connString;
//        }

//        public ConnectionState ServerState
//        {
//            get { return conSQL.State; }
//        }

//        public string ServerName
//        {
//            get { return m_svrName; }
//            set { m_svrName = value; SetConnString(); }
//        }

//        public string DataBaseName
//        {
//            get { return m_dbName; }
//            set { m_dbName = value; SetConnString(); }
//        }

//        public string UserName
//        {
//            get { return m_usrName; }
//            set { m_usrName = value; SetConnString(); }
//        }

//        public string UserPassword
//        {
//            get { return m_usrPass; }
//            set
//            {
//                m_usrPass = value;
//                SetConnString();
//            }
//        }

//        #region ▶ AttachParameters : Command에 파라미터 추가

//        /// <summary>
//        /// OracleCommand에 OracleParameter를 추가합니다.<br/>
//        /// 추가시 Input 또는 Output 파라미터의 값이 null일 경우 DbNull 값을 지정합니다.
//        /// </summary>
//        /// <param name="command">파라미터가 추가될 OracleCommand입니다.</param>
//        /// <param name="param">OracleCommand에 추가할 OracleParameter 배열입니다.</param>
//        private void AttachParameters(MySqlCommand command, MySqlParameter[] parameter)
//        {
//            try
//            {
//                foreach (MySqlParameter p in parameter)
//                {
//                    if (p != null)
//                    {
//                        if ((p.Direction == ParameterDirection.InputOutput
//                            || p.Direction == ParameterDirection.Input)
//                            && (p.Value == null))
//                            p.Value = DBNull.Value;
//                        command.Parameters.Add(p);
//                    }
//                }
//            }
//            catch (MySqlException ex)
//            {
//                //string errorMessage = ExceptionMgr.GetErrorMessage(ex, NAMESPACE, CLASSNAME, "AttachParameters");
//                throw new Exception("\r\n\r\n▶▶▶[ DataService ]◀◀◀ \r\n\r\n" + ex.Message);
//                //throw new Exception("AttachParameters(SqlCommand command, SqlParameter[] parameter) - " + ex.Message);
//            }
//        }

//        #endregion

//        public bool ConnectionOpen()
//        {
//            bool rtn = false;

//            try
//            {
//                if (m_connString.Length > 0)
//                {
//                    if (conSQL.State == ConnectionState.Open)
//                    {
//                        conSQL.Close();
//                    }

//                    conSQL.Open();

//                    rtn = true;
//                }
//            }
//            catch (Exception sqlex)
//            {
//                Log.log(sqlex.Message);
//                //throw new Exception(sqlex.Message );
//                rtn = false;
//            }

//            return rtn;
//        }

//        public void ConnectionClose()
//        {
//            try
//            {
//                if (conSQL.State != ConnectionState.Closed)
//                {
//                    conSQL.Close();
//                }

//            }
//            catch (MySqlException sqlex)
//            {
//                Log.log(sqlex.Message);
//                //throw new Exception(sqlex.Message);
//            }
//        }

//        private void SetConnString()
//        {
//            m_connString = "Initial Catalog=" + m_dbName + ";Data Source=" + m_svrName + ";User ID=" + m_usrName + ";Password=" + m_usrPass + ";";
//            conSQL.ConnectionString = m_connString;
//        }


//        public int ExecuteNonQuery(string strSQL)
//        {

//            ConnectionOpen();
//            MySqlCommand cmd = new MySqlCommand(strSQL, conSQL);

//            int obj = 0;
//            try
//            {

//                obj = cmd.ExecuteNonQuery();
//            }
//            catch (MySqlException sqlex)
//            {
//                Log.log(sqlex.Message);
//                //throw new Exception(sqlex.Message);
//            }
//            finally
//            {
//                ConnectionClose();
//            }
//            return obj;

//        }


//        public Object ExecuteScalar(string strSQL)
//        {
//            ConnectionOpen();
//            MySqlCommand cmd = new MySqlCommand(strSQL, conSQL);

//            Object obj = new object();
//            try
//            {
//                obj = cmd.ExecuteScalar();
//            }
//            catch (MySqlException sqlex)
//            {
//                Log.log(sqlex.Message);
//                //throw new Exception(sqlex.Message);
//            }
//            finally
//            {
//                ConnectionClose();
//            }

//            return obj;

//        }


//        public DataSet ExecuteDataset(string strSQL)
//        {
//            ConnectionOpen();

//            MySqlCommand cmd = new MySqlCommand(strSQL, conSQL);

//            MySqlDataAdapter adt = new MySqlDataAdapter(cmd);

//            ConnectionClose();

//            DataSet dsData = new DataSet();

//            try
//            {
//                adt.Fill(dsData);
//            }
//            catch (Exception sqlex)
//            {
//                Log.log(sqlex.Message);
//                //throw new Exception(sqlex.Message);
//            }
//            finally
//            {
//                ConnectionClose();
//            }

//            return dsData;

//        }

//        public DataSet ExecuteDataset(string strSQL, string TableName)
//        {
//            ConnectionOpen();

//            MySqlCommand cmd = new MySqlCommand(strSQL, conSQL);

//            MySqlDataAdapter adt = new MySqlDataAdapter(cmd);

//            DataSet dsData = new DataSet();

//            try
//            {
//                adt.Fill(dsData, TableName);
//            }
//            catch (MySqlException sqlex)
//            {
//                Log.log(sqlex.Message);
//                //throw new Exception(sqlex.Message);
//            }
//            finally
//            {
//                ConnectionClose();
//            }

//            return dsData;

//        }


//        public bool UpdateTable(DataTable dsData, string strSQL)
//        {
//            bool rtn = false;
//            //string strSQL;
//            //strSQL = "Select eqi_code, eqc_code, eql_code, eqi_name, eqi_unit, eqi_note from StandardCompItems";
//            //CommandBuilder사용시 제약조건(인용) 
//            //1. Select 명령을 DataAdapter 생성자에서 또는 오로지 한 테이블로부터 
//            //레코드를 검색하는 DataAdapter의 SelectCommand 매개변수로부터 지정해야한다. 
//            //2. Select 명령내에서 적어도 기본 키나 고유 칼럼을 필수 칼럼으로 지정해야한다. 

//            //DS를 표현하는 DataGrid에서 값을 변경하거나 추가/삭제를 하는 모든 행위가 
//            //물리DB에 적용됨 
//            ConnectionOpen();

//            MySqlDataAdapter sqlDa = new MySqlDataAdapter();

//            sqlDa.SelectCommand = new MySqlCommand(strSQL, conSQL);
//            MySqlCommandBuilder cb = new MySqlCommandBuilder(sqlDa);

//            DataTable dsChanges;

//            try
//            {

//                dsChanges = dsData.GetChanges();

//                if ((dsChanges != null))
//                {

//                    sqlDa.Update(dsData);

//                    dsData.AcceptChanges();

//                }
//                rtn = true;
//            }
//            catch (DBConcurrencyException ex)
//            {
//                rtn = false;
//                //Log.log(ex.Message);
//                Log.log(ex);
//            }
//            catch (MySqlException sqlex)
//            {
//                rtn = false;
//                Log.log(sqlex.Message);
//                //throw new Exception(sqlex.Message);
//            }
//            finally
//            {
//                ConnectionClose();
//            }

//            return rtn;


//        }

//        /// <summary>
//        /// 비트랜잭션, 저장 프로시저를 실행하고 DataSet을 반환합니다.<br/>
//        /// (저장 프로시저명, 파라미터 사용)
//        /// </summary>
//        /// <param name="spName">저장 프로시저명입니다.</param>
//        /// <param name="parameter">파라미터 배열명입니다.</param>
//        /// <returns>System.Data.DataSet 형식으로 DataSet이면 성공, null이면 실패입니다.</returns>
//        public DataSet ExecuteDataset(string spName, MySqlParameter[] parameter)
//        {
//            DataSet returnDataset = null;
//            MySqlDataAdapter adapter = null;
//            try
//            {
//                ConnectionOpen();
//                adapter = new MySqlDataAdapter(spName, conSQL);
//                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
//                if (parameter != null)
//                    AttachParameters(adapter.SelectCommand, parameter);
//                returnDataset = new DataSet();
//                adapter.Fill(returnDataset);
//            }
//            catch (MySqlException ex)
//            {
//                returnDataset = null;
//                Log.log(ex.Message);
//                //throw new Exception("\r\n\r\n▶▶▶[ DataService ]◀◀◀ \r\n\r\n" + ex.Message);                
//            }
//            catch (Exception ex)
//            {
//                returnDataset = null;
//                Log.log(ex.Message);
//                //throw new Exception("\r\n\r\n▶▶▶[ DataService ]◀◀◀ \r\n\r\n" + ex.Message);
//            }
//            finally
//            {
//                if (adapter != null) { adapter.SelectCommand.Parameters.Clear(); adapter.Dispose(); }
//                ConnectionClose();
//            }

//            return returnDataset;
//        }

//        /// <summary>
//        /// 트랜잭션, 저장 프로시저를 실행하고 영향받은 레코드수를 반환합니다.<br/>
//        /// (저장 프로시저명, 파라미터 사용)
//        /// </summary>
//        /// <param name="spName">저장 프로시저명입니다.</param>
//        /// <param name="parameter">파라미터 배열명입니다.</param>
//        /// <param name="parameterCountName">레코드수 저장 Output 파라미터명입니다.</param>
//        /// <returns>영향받은 레코드수입니다.</returns>
//        /// <remarks>
//        /// Sql에서 저장 프로시저가 정상 처리되었을 경우 영향받은 레코드수가 아닌 -1이 반환됩니다.<br/>
//        /// 영향받은 레코드수를 반환받기 위해서는 저장 프로시저에 Output 파라미터를 사용하고,<br/>
//        /// Query문 실행 후 Output 파라미터에 영향받은 레코드수(0 또는 SQL%ROWCOUNT)를 지정해주어야 합니다.
//        /// </remarks>
//        public int ExecuteNonQuery(string spName, MySqlParameter[] parameter)
//        {
//            int recordAffected = 0;
//            MySqlCommand command = null;
//            try
//            {
//                ConnectionOpen();
//                command = new MySqlCommand(spName, conSQL);
//                command.CommandType = CommandType.StoredProcedure;
//                if (parameter != null)
//                    AttachParameters(command, parameter);
//                recordAffected = command.ExecuteNonQuery();

//            }
//            catch (MySqlException ex)
//            {
//                recordAffected = 0;
//                Log.log(ex.Message);
//                //throw new Exception("\r\n\r\n▶▶▶[ DataService ]◀◀◀ \r\n\r\n" + ex.Message);
//            }
//            catch (Exception ex)
//            {
//                recordAffected = 0;
//                Log.log(ex.Message);
//                //throw new Exception("\r\n\r\n▶▶▶[ DataService ]◀◀◀ \r\n\r\n" + ex.Message);
//            }
//            finally
//            {
//                if (command != null) { command.Parameters.Clear(); command.Dispose(); }
//                ConnectionClose();
//            }
//            return recordAffected;
//        }


//        /// <summary>
//        /// 비트랜잭션, 저장 프로시저에 의한 단일 값을 검색합니다.<br/>
//        /// (저장 프로시저명, 파라미터 사용)
//        /// </summary>
//        /// <param name="spName">저장 프로시저명입니다.</param>
//        /// <param name="parameter">OracleParameter 배열입니다.</param>
//        /// <returns>object 형식으로 object이면 성공, null이면 실패입니다.</returns>
//        public object ExecuteScalar(string spName, MySqlParameter[] parameter)
//        {
//            MySqlCommand command = null;
//            object returnObj = null;
//            try
//            {
//                ConnectionOpen();
//                command = new MySqlCommand(spName, conSQL);
//                command.CommandType = CommandType.StoredProcedure;
//                if (parameter != null)
//                    AttachParameters(command, parameter);
//                returnObj = command.ExecuteScalar();
//            }
//            catch (MySqlException ex)
//            {
//                returnObj = null;
//                Log.log(ex.Message);
//                //throw new Exception("\r\n\r\n▶▶▶[ DataService ]◀◀◀ \r\n\r\n" + ex.Message);
//            }
//            catch (Exception ex)
//            {
//                returnObj = null;
//                Log.log(ex.Message);
//                //throw new Exception("\r\n\r\n▶▶▶[ DataService ]◀◀◀ \r\n\r\n" + ex.Message);
//            }
//            finally
//            {
//                if (command != null) { command.Parameters.Clear(); command.Dispose(); }
//                ConnectionClose();
//            }
//            return returnObj;
//        }

//    }
//}
