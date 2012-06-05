using System;
using System.Configuration;
using System.Data.SQLite;
using System.Data;

namespace AccountBook.DAL.SQLiteImpl
{
    public static class SqliteHelper
    {
        private static SQLiteConnection _connection;

        private static readonly string ConnectionString;

        static SqliteHelper()
        {
            ConnectionString = string.Format("Data Source={0}", AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.ConnectionStrings["DBConnection"]);
        }

        /// <summary>
        /// 获得连接对象
        /// </summary>
        /// <returns></returns>
        public static SQLiteConnection Connection
        {
            get
            {
                return _connection ?? (_connection = new SQLiteConnection(ConnectionString));
            }
            set
            {
                _connection = value;
            }
        }

        /// <summary>
        /// 为数据库加密
        /// </summary>
        /// <param name="password">要为数据库设置的密码</param>
        public static void EncryptDataBase(string password)
        {
            Connection.Open();
            Connection.ChangePassword(password);
        }

        /// <summary>
        /// 清除数据库密码
        /// </summary>
        /// <param name="password">数据库当前密码</param>
        public static void DecryptDataBase(string password)
        {
            Connection.SetPassword(password);
            Connection.Open();
            Connection.ChangePassword(string.Empty);
        }

        /// <summary>
        /// 处理SQLiteCommand
        /// </summary>
        /// <param name="cmd">SQLiteCommand</param>
        /// <param name="cmdText">SQL语句</param>
        /// <param name="p">查询参数</param>
        private static void PrepareCommand(SQLiteCommand cmd, string cmdText, params object[] p)
        {
            if (p == null) return;

            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }

            cmd.Parameters.Clear();
            cmd.Connection = Connection;
            cmd.CommandText = cmdText;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 30;

            //foreach (var parm in p)
            //{
            //    cmd.Parameters.AddWithValue(string.Empty, parm);
            //}

            //modify by yuwang 2011-3-10
            foreach (var parm in p)
            {
                cmd.Parameters.AddWithValue(((SQLiteParameter)parm).ParameterName, ((SQLiteParameter)parm).Value);
            }

        }

        /// <summary>
        /// 执行SQL返回DataSet数据集
        /// </summary>
        /// <param name="cmdText">SQL语句</param>
        /// <param name="p">查询参数</param>
        /// <returns>DataSet数据集</returns>
        public static DataSet ExecuteDataset(string cmdText, params object[] p)
        {
            var ds = new DataSet();
            var command = new SQLiteCommand();
            lock (Connection)
            {
                PrepareCommand(command, cmdText, p);
                var da = new SQLiteDataAdapter(command);
                da.Fill(ds);
            }
            return ds;
        }

        /// <summary>
        /// 返回查询数据的首行数据
        /// </summary>
        /// <param name="cmdText">SQL语句</param>
        /// <param name="p">查询参数</param>
        /// <returns>返回首行数据</returns>
        public static DataRow ExecuteDataRow(string cmdText, params object[] p)
        {
            var ds = ExecuteDataset(cmdText, p);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            return null;
        }

        /// <summary>
        /// 执行Sql,返回受影响行数(需要开启SQLite数据库changes_count属性)
        /// </summary>
        /// <param name="cmdText">SQL</param>
        /// <param name="p">传入的参数</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(string cmdText, params object[] p)
        {
            var command = new SQLiteCommand();
            int result;
            lock (Connection)
            {
                PrepareCommand(command, cmdText, p);
                result = command.ExecuteNonQuery();
            }
            return result;
        }

        //[Obsolete("要同步使用 Connection 对象，该方法不可用。", true)]
        public static SQLiteDataReader ExecuteReader(string cmdText, params object[] p)
        {
            var command = new SQLiteCommand();
            try
            {
                PrepareCommand(command, cmdText, p);
                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }
            catch
            {
                Connection.Close();
                throw;
            }
        }

        /// <summary>
        /// 返回结果集中的第一行第一列，忽略其他行或列
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="p">传入的参数</param>
        /// <returns></returns>
        public static object ExecuteScalar(string cmdText, params object[] p)
        {
            var cmd = new SQLiteCommand();
            object obj;
            lock (Connection)
            {
                PrepareCommand(cmd, cmdText, p);
                obj = cmd.ExecuteScalar();
            }
            return obj;
        }

        /// <summary>
        /// 分页返回DataSet数据集
        /// </summary>
        /// <param name="recordCount">记录总数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="cmdText">查询数据SQL</param>
        /// <param name="countText">查询记录总数SQL</param>
        /// <param name="p">查询参数</param>
        /// <returns>数据DataSet</returns>
        public static DataSet ExecutePager(out int recordCount, int pageIndex, int pageSize, string cmdText, string countText, params object[] p)
        {
            object countResult = ExecuteScalar(countText, p);
            recordCount = countResult == null ? 0 : int.Parse(countResult.ToString());

            var ds = new DataSet();
            var command = new SQLiteCommand();
            lock (Connection)
            {
                PrepareCommand(command, cmdText, p);
                var da = new SQLiteDataAdapter(command);
                da.Fill(ds, (pageIndex - 1) * pageSize, pageSize, "result");
            }

            return ds;
        }

        /// <summary>
        /// 分页返回DataSet数据集
        /// </summary>
        /// <param name="recordCount">记录总数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="cmdText">查询数据SQL</param>
        /// <param name="p">查询参数</param>
        /// <returns>数据DataSet</returns>
        public static DataSet ExecutePager(out int recordCount, int pageIndex, int pageSize, string cmdText, params object[] p)
        {
            string countText = string.Format("SELECT COUNT(1) FROM ({0})", cmdText);
            return ExecutePager(out recordCount, pageIndex, pageSize, cmdText, countText, p);
        }

        /// <summary>
        /// 分页返回DataReader
        /// </summary>
        /// <param name="recordCount">记录总数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="cmdText">查询数据SQL</param>
        /// <param name="countText">查询记录总数SQL</param>
        /// <param name="p">查询参数</param>
        /// <returns>数据DataReader</returns>
        public static SQLiteDataReader ExecutePagerReader(out int recordCount, int pageIndex, int pageSize, string cmdText, string countText, params object[] p)
        {
            object countResult = ExecuteScalar(countText, p);
            recordCount = countResult == null ? 0 : int.Parse(countResult.ToString());

            lock (Connection)
            {
                cmdText = string.Format("{0} LIMIT {1},{2}", cmdText, pageIndex * pageSize, pageSize);
                return ExecuteReader(cmdText, p);
            }
        }

        /// <summary>
        /// 分页返回DataReader
        /// </summary>
        /// <param name="recordCount">记录总数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="cmdText">查询数据SQL</param>
        /// <param name="p">查询参数</param>
        /// <returns>数据DataReader</returns>
        public static SQLiteDataReader ExecutePagerReader(out int recordCount, int pageIndex, int pageSize, string cmdText, params object[] p)
        {
            string countText = string.Format("SELECT COUNT(1) FROM ({0})", cmdText);
            return ExecutePagerReader(out recordCount, pageIndex, pageSize, cmdText, countText, p);
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public static void CloseConnection()
        {
            _connection.Close();
        }
    }
}
