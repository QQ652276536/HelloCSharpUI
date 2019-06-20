using System;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace HelloCSharp
{
    public class DataBaseClass
    {
        private static readonly string SERVER = "127.0.0.1";
        private static readonly string UID = "sa";
        private static readonly string PWD = ".";
        private static readonly string DATABASE = "wande";

        public static string str = "server=" + SERVER + ";uid=" + UID + ";pwd=" + PWD + ";database=" + DATABASE;

        public DataSet GetDataSetDatas(string sql)
        {

            DataSet ds = new DataSet();
            try
            {

                SqlConnection conn = new SqlConnection(str);
                conn.Open();


                SqlDataAdapter odbcda = new SqlDataAdapter(sql, conn);
                odbcda.Fill(ds);
                conn.Close();
                odbcda.Dispose();
                conn.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ds;

        }
        public DataSet GetDataSetDatas(string commandString, string tableName)
        {

            DataSet dataSet = new DataSet();
            try
            {
                SqlConnection conn = new SqlConnection(str);
                conn.Open();

                SqlDataAdapter sqlDA = new SqlDataAdapter(commandString, conn);

                SqlCommandBuilder ss = new SqlCommandBuilder(sqlDA);

                if (tableName != "")
                {
                    sqlDA.Fill(dataSet, tableName);

                }
                else
                {
                    sqlDA.Fill(dataSet);
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return dataSet;
            //MessageBox.Show("8");
        }
        /* 执行查询数据库操作是否有记录　*/
        /// <summary>
        /// 功能：执行查询数据库操作是否有记录
        /// </summary>
        /// <param name=”strSql”></param>
        public static bool ExecuteRead(string strSql)
        {
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            SqlCommand cm = new SqlCommand();
            cm.CommandText = strSql;
            cm.Connection = conn;
            try
            {
                SqlDataReader dr = cm.ExecuteReader();
                if (dr.Read())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                cm.Dispose();
                conn.Close();
            }

        }
        /* 执行更新删除插入数据库操作,成功则返回true　*/
        /// <summary>   
        /// 功能：执行更新删除插入数据库操作,成功则返回true   
        /// </summary>   
        /// <param name="strSql"></param>   
        /// <returns></returns>   
        public static bool ExecuteSql(string strSql)
        {
            bool flag = false;
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            //操作   
            SqlCommand cm = new SqlCommand();
            cm.CommandText = strSql;
            try
            {
                cm.Connection = conn;
                cm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
            finally
            {
                conn.Close();
                flag = true;
            }
            return flag;

        }
        // 返回Sql语句执行结果的第一行第一列
        public static string readData(string commandString)
        {
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            string result;
            SqlDataReader Dr;
            try
            {
                SqlCommand cmd = new SqlCommand(commandString, conn);
                Dr = cmd.ExecuteReader();
                if (Dr.Read())
                {
                    result = Dr[0].ToString();
                    Dr.Close();
                }
                else
                {
                    result = "";
                    Dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);


            }
            conn.Close();
            return result;
        }

        public int RunCommand(string commandString)
        {
            int runCount = 0;
            try
            {



                // 执行 SQL 命令
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(commandString, conn);

                    runCount = cmd.ExecuteNonQuery();

                    conn.Close();
                    conn.Dispose();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "操作数据失败 RunCommand",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            return runCount;

        }

    }

}
