using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlClient
{
    /// <summary>
    /// sqlserver数据库操作通用类
    /// </summary>
    public class SqlHelper
    {
        //数据库连接字符串
        public static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultDBConnString"].ConnectionString;

        /// <summary>
        /// 数据库备份文件存放路径
        /// </summary>
        public static readonly string backupPath = ConfigurationManager.AppSettings["backupPath"];

        /// <summary>
        /// 数据库名称
        /// </summary>
        public static readonly string databaseName = ConfigurationManager.AppSettings["databaseName"];

        /// <summary>
        /// 存储过程返回值名称
        /// </summary>
        public static readonly string RETURNVALUE = "RETURNVALUE";


        /// <summary>
        /// 获取数据库连接对象，并打开连接
        /// </summary>
        /// <returns>数据库连接对象</returns>
        private static SqlConnection GetConnection()
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            return conn;
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        /// <param name="conn">数据库连接对象</param>
        private static void CloseConnection(SqlConnection conn)
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 备份数据库
        /// </summary>
        /// <returns></returns>
        public static int BackUp()
        {
            string filename = DateTime.Now.ToString("yyyyMMddHHmmss");
            string sql = "backup database " + databaseName + " to disk='" + backupPath + "/" + filename + ".bak' ";
            return ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 执行一条无参数的非查询sql语句并返回受影响的行数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(string sql)
        {
            SqlConnection conn = GetConnection(); ;
            int val;
            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                val = cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //错误处理
                SystemLog.WriteLog(e.ToString());
                val = -1;
            }
            finally
            {
                CloseConnection(conn);

            }
            return val;
        }

        /// <summary>
        /// 执行一条带参数的非查询sql语句并返回受影响的行数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数数组</param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(string sql, SqlParameter[] param)
        {
            SqlConnection conn = GetConnection();
            int val;
            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                foreach (SqlParameter parameter in param)
                {
                    cmd.Parameters.Add(parameter);
                }
                val = cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //错误处理
                SystemLog.WriteLog(e.ToString());
                val = -1;
            }
            finally
            {
                CloseConnection(conn);
            }
            return val;
        }

        /// <summary>
        /// 执行一条带参数的非查询sql语句并返回受影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string sql, List<SqlParameter> param)
        {
            SqlConnection conn = GetConnection();
            int val;
            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                foreach (SqlParameter parameter in param)
                {
                    cmd.Parameters.Add(parameter);
                }
                val = cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //错误处理
                SystemLog.WriteLog(e.ToString());
                val = -1;
            }
            finally
            {
                CloseConnection(conn);
            }
            return val;
        }

        /// <summary>
        /// 执行多条sql语句，实现事务
        /// </summary>
        /// <param name="sql">sql语句数组</param>
        /// <param name="param">参数数组</param>
        /// <returns></returns>
        public static int ExecuteTransaction(string[] sql, SqlParameter[][] param)
        {
            SqlConnection conn = GetConnection();
            SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
            SqlCommand cmd = conn.CreateCommand();
            int val = 0;
            try
            {
                cmd.Transaction = trans;
                for (int i = 0; i < sql.Length; i++)
                {
                    cmd.CommandText = sql[i];
                    foreach (SqlParameter parameter in param[i])
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    val += cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
                trans.Commit();
            }
            catch (Exception e)
            {
                try
                {
                    trans.Rollback();//事务回滚
                    SystemLog.WriteLog(e.ToString());
                    val = -1;
                }
                catch (Exception e1)
                {
                    //错误处理
                    SystemLog.WriteLog(e.ToString());
                    val = -1;
                }
            }
            finally
            {
                CloseConnection(conn);
            }
            return val;
        }

        /// <summary>
        /// 执行多条sql语句，实现事务
        /// </summary>
        /// <param name="sql">sql语句数组</param>
        /// <param name="param">参数数组</param>
        /// <returns></returns>
        public static int ExecuteTransaction1(List<string> sql, List<SqlParameter[]> param)
        {
            SqlConnection conn = GetConnection();
            SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
            SqlCommand cmd = conn.CreateCommand();
            int val = 0;
            try
            {
                cmd.Transaction = trans;
                for (int i = 0; i < sql.Count; i++)
                {
                    cmd.CommandText = sql[i];
                    foreach (SqlParameter parameter in param[i])
                    {
                        cmd.Parameters.Add(parameter);
                    }
                    val += cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
                trans.Commit();
            }
            catch (Exception e)
            {
                try
                {
                    trans.Rollback();//事务回滚
                    SystemLog.WriteLog(e.ToString());
                    val = -1;
                }
                catch (Exception e1)
                {
                    //错误处理
                    SystemLog.WriteLog(e.ToString());
                    val = -1;
                }
            }
            finally
            {
                CloseConnection(conn);
            }
            return val;
        }

        /// <summary>
        /// 执行多条sql语句，实现事务
        /// </summary>
        /// <param name="sql">sql语句数组</param>
        /// <returns></returns>
        public static int ExecuteTransaction1(List<string> sql)
        {
            SqlConnection conn = GetConnection();
            SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
            SqlCommand cmd = conn.CreateCommand();
            int val = 0;
            try
            {
                cmd.Transaction = trans;
                for (int i = 0; i < sql.Count; i++)
                {
                    cmd.CommandText = sql[i];
                    val += cmd.ExecuteNonQuery();
                }
                trans.Commit();
            }
            catch (Exception e)
            {
                try
                {
                    trans.Rollback();//事务回滚
                    SystemLog.WriteLog(e.ToString());
                    val = -1;
                }
                catch (Exception e1)
                {
                    //错误处理
                    SystemLog.WriteLog(e.ToString());
                    val = -1;
                }
            }
            finally
            {
                CloseConnection(conn);
            }
            return val;
        }

        /// <summary>
        /// 执行一条无参数的查询sql语句并返回SqlDataReader对象（请在使用完SqlDataReader对象后及时关闭该对象，否则数据库连接将不能正常关闭）
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回SqlDataReader对象</returns>
        public static SqlDataReader ExecuteReader(string sql)
        {
            SqlConnection conn = GetConnection(); ;
            SqlDataReader dr;
            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception e)
            {
                //错误处理
                SystemLog.WriteLog(e.ToString());
                dr = null;
                CloseConnection(conn);
            }
            return dr;
        }

        /// <summary>
        /// 执行一条带参数的查询sql语句并返回SqlDataReader对象（请在使用完SqlDataReader对象后及时关闭该对象，否则数据库连接将不能正常关闭）
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数数组</param>
        /// <returns>返回SqlDataReader对象</returns>
        public static SqlDataReader ExecuteReader(string sql, SqlParameter[] param)
        {
            SqlConnection conn = GetConnection();
            SqlDataReader dr;
            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                foreach (SqlParameter parameter in param)
                {
                    cmd.Parameters.Add(parameter);
                }
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception e)
            {
                //错误处理
                SystemLog.WriteLog(e.ToString());
                dr = null;
                CloseConnection(conn);
            }
            return dr;
        }

        /// <summary>
        /// 执行一条无参数的查询sql语句并返回第一行第一列的值
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回第一行第一列的值</returns>
        public static object ExecuteScalar(string sql)
        {
            SqlConnection conn = GetConnection();
            object val;
            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                val = cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                //错误处理
                SystemLog.WriteLog(e.ToString());
                val = null;
            }
            finally
            {
                CloseConnection(conn);

            }
            return val;
        }

        /// <summary>
        /// 执行一条带参数的查询sql语句并返回第一行第一列的值
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数数组</param>
        /// <returns>返回第一行第一列的值</returns>
        public static object ExecuteScalar(string sql, SqlParameter[] param)
        {
            SqlConnection conn = GetConnection();
            object val;
            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                foreach (SqlParameter parameter in param)
                {
                    cmd.Parameters.Add(parameter);
                }
                val = cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                //错误处理
                SystemLog.WriteLog(e.ToString());
                val = null;
            }
            finally
            {
                CloseConnection(conn);
            }
            return val;
        }

        /// <summary>
        /// 执行一条带参数的查询sql语句并返回第一行第一列的值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string sql, List<SqlParameter> param)
        {
            SqlConnection conn = GetConnection();
            object val;
            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                foreach (SqlParameter parameter in param)
                {
                    cmd.Parameters.Add(parameter);
                }
                val = cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                //错误处理
                SystemLog.WriteLog(e.ToString());
                val = null;
            }
            finally
            {
                CloseConnection(conn);
            }
            return val;
        }

        /// <summary>
        /// 执行一条不带参数的查询sql语句并返回DataSet结果集
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataSet ExecuteQuery(string sql)
        {
            SqlConnection conn = GetConnection();
            try
            {
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                CloseConnection(conn);
                return ds;
            }
            catch (Exception e)
            {
                //错误处理
                CloseConnection(conn);
                SystemLog.WriteLog(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// 执行一条带参数的查询sql语句并返回DataSet结果集
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DataSet ExecuteQuery(string sql, SqlParameter[] param)
        {
            SqlConnection conn = GetConnection();
            try
            {
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand(sql, conn);
                foreach (SqlParameter parameter in param)
                {
                    cmd.Parameters.Add(parameter);
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                CloseConnection(conn);
                return ds;
            }
            catch (Exception e)
            {
                //错误处理
                CloseConnection(conn);
                SystemLog.WriteLog(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// 执行一条带参数的查询sql语句并返回DataSet结果集
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DataSet ExecuteQuery(string sql, List<SqlParameter> param)
        {
            SqlConnection conn = GetConnection();
            try
            {
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand(sql, conn);
                foreach (SqlParameter parameter in param)
                {
                    cmd.Parameters.Add(parameter);
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                CloseConnection(conn);
                return ds;
            }
            catch (Exception e)
            {
                //错误处理
                CloseConnection(conn);
                SystemLog.WriteLog(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// 执行一个不带参数的存储过程返回受影响的行数
        /// </summary>
        /// <param name="procedureName"></param>
        /// <returns></returns>
        public static int ExecuteProcedure(string procedureName)
        {
            SqlConnection conn = GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(procedureName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                int i = cmd.ExecuteNonQuery();
                return i;
            }
            catch (Exception e)
            {
                //错误处理
                CloseConnection(conn);
                SystemLog.WriteLog(e.ToString());
                return 0;
            }
        }

        /// <summary>
        /// 执行一个带参数的存储过程返回受影响的行数
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static int ExecuteProcedure(string procedureName, SqlParameter[] param)
        {
            SqlConnection conn = GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(procedureName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter parameter in param)
                {
                    cmd.Parameters.Add(parameter);
                }
                int i = cmd.ExecuteNonQuery();
                return i;
            }
            catch (Exception e)
            {
                //错误处理
                CloseConnection(conn);
                SystemLog.WriteLog(e.ToString());
                return 0;
            }
        }

        /// <summary>
        /// 执行一个带参数的存储过程返回存储过程的返回值
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static int ExecuteProcedure1(string procedureName, SqlParameter[] param)
        {
            SqlConnection conn = GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(procedureName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter parameter in param)
                {
                    cmd.Parameters.Add(parameter);
                }
                cmd.Parameters.Add(new SqlParameter(RETURNVALUE, SqlDbType.Int, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
                cmd.ExecuteNonQuery();
                return (int)cmd.Parameters[RETURNVALUE].Value;
            }
            catch (Exception e)
            {
                //错误处理
                CloseConnection(conn);
                SystemLog.WriteLog(e.ToString());
                return 0;
            }
        }

        /// <summary>
        /// 执行一个不带参数的存储过程返回存储过程的返回值
        /// </summary>
        /// <param name="procedureName"></param>
        /// <returns></returns>
        public static int ExecuteProcedure1(string procedureName)
        {
            SqlConnection conn = GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(procedureName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter(RETURNVALUE, SqlDbType.Int, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
                cmd.ExecuteNonQuery();
                return (int)cmd.Parameters[RETURNVALUE].Value;
            }
            catch (Exception e)
            {
                //错误处理
                CloseConnection(conn);
                SystemLog.WriteLog(e.ToString());
                return 0;
            }
        }

        /// <summary>
        /// 执行带参数的存储过程，返回dataset
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DataSet ExecuteProcedure2(string procedureName, SqlParameter[] param)
        {
            SqlConnection conn = GetConnection();
            try
            {
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand(procedureName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter parameter in param)
                {
                    cmd.Parameters.Add(parameter);
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                CloseConnection(conn);
                return ds;
            }
            catch (Exception e)
            {
                //错误处理
                CloseConnection(conn);
                SystemLog.WriteLog(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// 执行不带参数的存储过程，返回dataset
        /// </summary>
        /// <param name="procedureName"></param>
        /// <returns></returns>
        public static DataSet ExecuteProcedure2(string procedureName)
        {
            SqlConnection conn = GetConnection();
            try
            {
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand(procedureName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                CloseConnection(conn);
                return ds;
            }
            catch (Exception e)
            {
                //错误处理
                CloseConnection(conn);
                SystemLog.WriteLog(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// 生成存储过程参数
        /// </summary>
        /// <param name="ParamName">存储过程名称</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数大小</param>
        /// <param name="Direction">参数方向</param>
        /// <param name="Value">参数值</param>
        /// <returns>新的 parameter 对象</returns>
        public SqlParameter CreateParam(string paramName, SqlDbType dbType, Int32 size, ParameterDirection direction, object value)
        {
            SqlParameter param;
            if (size > 0)
            {
                param = new SqlParameter(paramName, dbType, size);
            }
            else
            {
                ///当参数大小为0时，不使用该参数大小值
                param = new SqlParameter(paramName, dbType);
            }
            ///创建输出类型的参数
            param.Direction = direction;
            param.Value = value;
            ///返回创建的参数
            return param;
        }
    }
}
