using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloCSharp.SQLServer
{
    class SQLServerConnectionTool
    {
        private string m_address = @"Data Source=192.168.0.124,1433;Initial Catalog=wande;Persist Security Info=True;User ID=sa;Password=asdf1234";

        public SQLServerConnectionTool()
        {
        }

        /// <summary>
        /// 增删改操作
        /// </summary>
        /// <returns></returns>
        public int AddDelUpdate(string sqlStr)
        {
            SqlConnection sqlConnection = new SqlConnection(m_address);
            SqlCommand sqlCommand = new SqlCommand
            {
                Connection = sqlConnection,
                CommandText = sqlStr,
                CommandType = CommandType.Text
            };
            int result = sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
            return result;
        }

        /// <summary>
        /// 执行查询操作
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <returns></returns>
        public int Search(string sqlStr)
        {
            SqlConnection sqlConnection = new SqlConnection(m_address);
            SqlCommand sqlCommand = new SqlCommand
            {
                Connection = sqlConnection,
                CommandText = sqlStr,
                CommandType = CommandType.Text
            };
            int result = 0;
            SqlDataReader dataReader = sqlCommand.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    result++;
                }
            }
            Console.Write("共有" + result.ToString() + "条数据");
            sqlConnection.Close();
            return result;
        }
    }

}
