using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NWJ.DBUtility
{
    public class SqlHelper
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public SqlHelper() { }


        /// <summary>
        /// 数据分页
        /// 适用2005以上数据库
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="orderType">排序类型</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="start">开始行</param>
        /// <param name="end">结束行</param>
        /// <param name="count">输出条数</param>
        /// <returns>DataTable</returns>
        public DataTable GetPageList(string sql, string orderField, string orderType, int start, int pageCount, out int count)
        {
            count = 0;
            int end = start + pageCount;
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("Select * From (Select ROW_NUMBER() Over (Order By " + orderField + " " + orderType + "");
                sb.Append(") As rowNum, * From (" + sql + ") As T ) As N Where rowNum > " + start + " And rowNum <= " + end + "");
                count = Convert.ToInt32(GetObject("Select Count(0) From (" + sql + ") As t"));
                return this.GetDataTable(sb.ToString());
            }
            catch (Exception e)
            {
                count = 0;
                return null;
            }
        }

        public DataTable GetPageList(string sql, string orderField, string orderType, int start, int pageCount, out int count, SqlParameter[] m_param)
        {
            count = 0;
            int end = start + pageCount;
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("Select * From (Select ROW_NUMBER() Over (Order By " + orderField + " " + orderType + "");
                sb.Append(") As rowNum, * From (" + sql + ") As T ) As N Where rowNum > " + start + " And rowNum <= " + end + "");
                count = Convert.ToInt32(GetObject("Select COUNT(0) From (" + sql + ") As t"));
                return this.GetDataTable(sb.ToString(), m_param);
            }
            catch (Exception e)
            {
                count = 0;
                return null;
            }
        }

        /// <summary>
        /// DataTable分页
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="PageIndex">页索引,注意：从1开始</param>
        /// <param name="PageSize">每页大小</param>
        /// <returns>分好页的DataTable数据</returns>     第1页        每页10条
        public DataTable GetPagedTable(DataTable dt, int PageIndex, int PageSize, out int count)
        {
            if (PageIndex == 0)
            {
                count = dt.Rows.Count;
                return dt;
            }
            DataTable newdt = dt.Copy();
            newdt.Clear();
            int rowbegin = (PageIndex - 1) * PageSize;
            int rowend = PageIndex * PageSize;

            if (rowbegin >= dt.Rows.Count)
            {
                count = dt.Rows.Count;
                return newdt;
            }

            if (rowend > dt.Rows.Count)
            { rowend = dt.Rows.Count; }
            for (int i = rowbegin; i <= rowend - 1; i++)
            {
                DataRow newdr = newdt.NewRow();
                DataRow dr = dt.Rows[i];
                foreach (DataColumn column in dt.Columns)
                {
                    newdr[column.ColumnName] = dr[column.ColumnName];
                }
                newdt.Rows.Add(newdr);
            }
            count = dt.Rows.Count;
            return newdt;
        }

        public DataTable BindDataTable(string tableName, string Where)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select * from " + tableName + " Where 1=1");
            sb.Append(Where);
            return this.GetDataTable(sb.ToString());
        }

        /// <summary>
        /// 根据唯一ID获取Hashtable
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="pkName">字段主键</param>
        /// <param name="pkVal">字段值</param>
        /// <returns></returns>
        public Hashtable GetHashtableById(string tableName, string pkName, string pkVal)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select * From ").Append(tableName).Append(" Where ").Append(pkName).Append("=@ID");
            DataTable dt = this.GetDataTable(sb.ToString(), new SqlParameter[] { new SqlParameter("@ID", pkVal) });
            return DataTableToHashtable(dt);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="pkName">字段主键</param>
        /// <param name="pkVal">字段值</param>
        /// <returns></returns>
        public int DeleteData(string tableName, string pkName, string pkVal)
        {
            StringBuilder sb = new StringBuilder("Delete From " + tableName + " Where " + pkName + " = @ID");
            return Convert.ToInt32(this.ExecuteBySql(sb.ToString(), new SqlParameter[] { new SqlParameter("@ID", pkVal) }));
        }

        /// <summary>
        /// 对象参数转换
        /// </summary>
        /// <param name="ht"></param>
        /// <returns></returns>
        private SqlParameter[] GetParameter(Hashtable ht)
        {
            SqlParameter[] _params = new SqlParameter[ht.Count];
            int i = 0;
            foreach (string key in ht.Keys)
            {
                _params[i] = new SqlParameter("@" + key, ht[key]);
                i++;
            }
            return _params;
        }

        /// <summary>
        /// 数据表转Hashtable
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        private Hashtable DataTableToHashtable(DataTable dt)
        {
            Hashtable ht = new Hashtable();
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string key = dt.Columns[i].ColumnName;
                    ht[key] = dr[key];
                }
            }
            return ht;
        }

        /// 单表查询结果转换成泛型集合
        /// </summary>
        /// <typeparam name="T">泛型集合类型</typeparam>
        /// <param name="dt">查询结果DataTable</param>
        /// <returns>以实体类为元素的泛型集合</returns>
        public IList<T> ConvertToList<T>(DataTable dt) where T : new()
        {
            // 定义集合
            List<T> ts = new List<T>();

            // 获得此模型的类型
            Type type = typeof(T);
            //定义一个临时变量
            string tempName = string.Empty;
            //遍历DataTable中所有的数据行 
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性
                PropertyInfo[] propertys = t.GetType().GetProperties();
                //遍历该对象的所有属性
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;//将属性名称赋值给临时变量  
                    //检查DataTable是否包含此列（列名==对象的属性名）    
                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter  
                        if (!pi.CanWrite) continue;//该属性不可写，直接跳出  
                        //取值  
                        object value = dr[tempName];
                        //如果非空，则赋给对象的属性  
                        if (value != DBNull.Value)
                            pi.SetValue(t, value, null);
                    }
                }
                //对象添加到泛型集合中
                ts.Add(t);
            }

            return ts;
        }

        #region 执行sql带 Hashtable参数的SQL语句
        /// <summary>
        /// 获取数据集(DataTable) 带Hashtable条件
        /// </summary>
        public DataTable GetDataTable(string sql, Hashtable ht)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(sql).Append(" Where 1 = 1 ");
            foreach (string key in ht.Keys)
            {
                sb.Append(" And ").Append(key).Append("=").Append("@" + key);
            }
            SqlParameter[] _params = this.GetParameter(ht);

            return this.GetDataTable(sb.ToString(), _params);
        }

        /// <summary>
        /// 通过Hashtable插入数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="ht">Hashtable</param>
        /// <returns>int</returns>
        public int InsertByHashtable(string tableName, Hashtable ht)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" Insert Into ");
            sb.Append(tableName);
            sb.Append("(");
            StringBuilder sp = new StringBuilder();
            StringBuilder sb_prame = new StringBuilder();
            foreach (string key in ht.Keys)
            {
                //防止传入带ID的主键
                if (key.ToUpper() != "ID")
                {
                    sb_prame.Append("," + key);
                    sp.Append(",@" + key);
                }
            }
            sb.Append(sb_prame.ToString().Substring(1, sb_prame.ToString().Length - 1) + ") Values (");
            sb.Append(sp.ToString().Substring(1, sp.ToString().Length - 1) + ")");
            object _object = this.ExecuteBySql(sb.ToString(), this.GetParameter(ht));
            return (_object == DBNull.Value) ? 0 : Convert.ToInt32(_object);
        }

        /// <summary>
        /// 判断是否存在该记录
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="pkName">主键字段</param>
        /// <param name="pkVal">主键字段值</param>
        /// <param name="ht">Hashtable</param>
        /// <returns></returns>
        public bool IsExist(string tableName, string pkName, string pkVal, Hashtable ht)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select Count(1) From ").Append(tableName).Append(" Where 1 = 1 ");
            foreach (string key in ht.Keys)
            {
                sb.Append(" And ").Append(key).Append("=").Append("@" + key);
            }
            if (!String.IsNullOrEmpty(pkVal))
            {
                sb.Append(" And ").Append(pkName).Append("!=").Append("@" + pkName);
                ht[pkName] = pkVal;
            }
            SqlParameter[] _params = this.GetParameter(ht);
            return Convert.ToInt32(this.GetObject(sb.ToString(), _params)) >= 1 ? false : true;
        }

        /// <summary>
        /// 通过Hashtable插入数据返回主键（针对整型主键返回）
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="ht">Hashtable</param>
        /// <returns></returns>
        public int InsertByHashtableReturnPkVal(string tableName, Hashtable ht)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" Declare @ReturnValue int Insert Into ");
            sb.Append(tableName);
            sb.Append("(");
            StringBuilder sp = new StringBuilder();
            StringBuilder sb_prame = new StringBuilder();
            foreach (string key in ht.Keys)
            {
                //防止传入带ID的主键
                if (key.ToUpper() != "ID")
                {
                    sb_prame.Append("," + key);
                    sp.Append(",@" + key);
                }
            }
            sb.Append(sb_prame.ToString().Substring(1, sb_prame.ToString().Length - 1) + ") Values (");
            sb.Append(sp.ToString().Substring(1, sp.ToString().Length - 1) + ") Set @ReturnValue=SCOPE_IDENTITY() Select @ReturnValue");
            object _object = this.GetObject(sb.ToString(), this.GetParameter(ht));
            return (_object == DBNull.Value) ? 0 : Convert.ToInt32(_object);
        }
        /// <summary>
        /// 通过Hashtable修改数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="pkName">字段主键</param>
        /// <param name="pkValue"></param>
        /// <param name="ht">Hashtable</param>
        /// <returns>int</returns>
        public int UpdateByHashtable(string tableName, string pkName, string pkValue, Hashtable ht)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" Update ");
            sb.Append(tableName);
            sb.Append(" Set ");
            bool isFirstValue = true;
            foreach (string key in ht.Keys)
            {
                //如果是主键 不赋值
                if (key.ToUpper() == pkName.ToUpper())
                {
                    continue;
                }
                if (isFirstValue)
                {
                    isFirstValue = false;
                    sb.Append(key);
                    sb.Append("=");
                    sb.Append("@" + key);
                }
                else
                {
                    sb.Append("," + key);
                    sb.Append("=");
                    sb.Append("@" + key);
                }
            }
            sb.Append(" Where ").Append(pkName).Append("=").Append("@" + pkName);
            ht[pkName] = pkValue;
            SqlParameter[] _params = this.GetParameter(ht);
            object _object = this.ExecuteBySql(sb.ToString(), _params);
            return (_object == DBNull.Value) ? 0 : Convert.ToInt32(_object);
        }

        #endregion

        #region  执行简单SQL语句
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public object ExecuteBySql(string sql)
        {
            int n = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        try
                        {
                            connection.Open();
                            int rows = cmd.ExecuteNonQuery();
                            connection.Close();
                            return rows;
                        }
                        catch (System.Data.SqlClient.SqlException e)
                        {
                            connection.Close();
                        }
                    }
                }

            }
            catch (Exception e)
            {
            }
            return n;
        }

        /// <summary>
        /// 获取执行sql返回值
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public object GetObject(string sql)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        try
                        {
                            connection.Open();
                            cmd.CommandTimeout = 180;
                            object obj = cmd.ExecuteScalar();
                            connection.Close();
                            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                            {
                                return null;
                            }
                            else
                            {
                                return obj;
                            }
                        }
                        catch (System.Data.SqlClient.SqlException e)
                        {
                            connection.Close();
                            throw new Exception(e.Message);
                        }
                    }
                }

                //Database db = DatabaseFactory.CreateDatabase(connectionString);
                //DbCommand cmd = db.GetSqlStringCommand(sql);
                //cmd.CommandTimeout = 30;
                //this.AddInParameter(db, cmd, param);
                //return db.ExecuteScalar(cmd);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataTable(string sql)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        connection.Open();
                        SqlDataAdapter command = new SqlDataAdapter(sql, connection);
                        command.Fill(ds, "ds");
                        connection.Close();
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds.Tables[0];
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #endregion

        #region 执行带参数的SQL语句
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteBySql(string SQLString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        //cmd.CommandTimeout = 180;
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        connection.Close();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException E)
                    {
                        throw new Exception(E.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 获取执行sql返回值
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="param">带参数</param>
        /// <returns></returns>
        public object GetObject(string sql, params SqlParameter[] cmdParms)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        try
                        {
                            PrepareCommand(cmd, connection, null, sql, cmdParms);
                            object obj = cmd.ExecuteScalar();
                            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                            {
                                return null;
                            }
                            else
                            {
                                return obj;
                            }
                        }
                        catch (System.Data.SqlClient.SqlException e)
                        {
                            connection.Close();
                            throw new Exception(e.Message);
                        }
                    }
                }

                //Database db = DatabaseFactory.CreateDatabase(connectionString);
                //DbCommand cmd = db.GetSqlStringCommand(sql);
                //cmd.CommandTimeout = 30;
                //this.AddInParameter(db, cmd, param);
                //return db.ExecuteScalar(cmd);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataTable(string sql, params SqlParameter[] cmdParms)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    PrepareCommand(cmd, connection, null, sql, cmdParms);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        try
                        {
                            da.Fill(ds, "ds");
                            cmd.Parameters.Clear();
                        }
                        catch (System.Data.SqlClient.SqlException e)
                        {
                        }
                        return ds.Tables[0];
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="param">构造参数</param>
        /// <returns>DataSet</returns>
        public DataSet GetDataSet(string sql, params SqlParameter[] param)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    PrepareCommand(cmd, connection, null, sql, param);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        try
                        {
                            da.Fill(ds, "ds");
                            cmd.Parameters.Clear();
                        }
                        catch (System.Data.SqlClient.SqlException ex)
                        {
                        }
                        return ds;
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandTimeout = 180;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {


                foreach (SqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }
        #endregion

        #region 存储过程操作

        /// <summary>
        /// 执行存储过程  (使用该方法切记要手工关闭SqlDataReader和连接)
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlDataReader returnReader;
            connection.Open();
            SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.CommandType = CommandType.StoredProcedure;
            returnReader = command.ExecuteReader();
            //Connection.Close(); 不能在此关闭，否则，返回的对象将无法使用            
            return returnReader;

        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>结果中第一行第一列</returns>
        public string RunProc(string storedProcName, IDataParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string StrValue;
                connection.Open();
                SqlCommand cmd;
                cmd = BuildQueryCommand(connection, storedProcName, parameters);
                StrValue = cmd.ExecuteScalar().ToString();
                connection.Close();
                return StrValue;
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataTable结果中的表名</param>
        /// <returns>DataTable</returns>
        public DataTable RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet, tableName);
                connection.Close();
                return dataSet.Tables[0];
            }
        }
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, int pageIndex, int pageSize)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                int firstPage = pageIndex * pageSize;
                DataSet dataSet = new DataSet();
                connection.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet, firstPage, pageSize, "ds");
                connection.Close();
                return dataSet;
            }
        }

        public DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName, int Times)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.SelectCommand.CommandTimeout = Times;
                sqlDA.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }

        /// <summary>
        /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand</returns>
        public static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
            {
                if (parameter != null)
                {
                    // 检查未分配值的输出参数,将其分配以DBNull.Value.
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }

        /// <summary>
        /// 执行存储过程，返回影响的行数		
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                int result;
                connection.Open();
                SqlCommand command = BuildIntCommand(connection, storedProcName, parameters);
                rowsAffected = command.ExecuteNonQuery();
                result = (int)command.Parameters["ReturnValue"].Value;
                //Connection.Close();
                return result;
            }
        }
        /// <summary>
        /// 执行存储过程，返回影响的行数		
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public int RunProcedures(string storedProcName, IDataParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                int result;
                connection.Open();
                SqlCommand command = BuildIntCommand(connection, storedProcName, parameters);
                result = command.ExecuteNonQuery();
                //result = (int)command.Parameters["ReturnValue"].Value;
                //Connection.Close();
                return result;
            }
        }
        /// <summary>
        /// 创建 SqlCommand 对象实例(用来返回一个整数值)	
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand 对象实例</returns>
        public static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new SqlParameter("ReturnValue",
                SqlDbType.Int, 4, ParameterDirection.ReturnValue,
                false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>结果中第一行第一列</returns>
        public string RunSql(string query)
        {
            string str;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        str = (cmd.ExecuteScalar().ToString() == "") ? "" : cmd.ExecuteScalar().ToString();
                        return str;
                    }
                    catch (System.Data.SqlClient.SqlException E)
                    {
                        connection.Close();
                        throw new Exception(E.Message);
                    }
                }
            }
        }

        #endregion

    }
}
