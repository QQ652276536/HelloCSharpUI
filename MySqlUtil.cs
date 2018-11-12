using System;
using MySql.Data.MySqlClient;

namespace OnlyEatNotWash
{
    class MySqlUtil
    {
        public static String _URL = "server=101.132.102.203;user=root;database=test;port=3306;password=root;";
        public MySqlConnection _connection = null;

        public MySqlUtil()
        {
            StartConnection();
        }

        private void StartConnection()
        {
            try
            {
                _connection = new MySqlConnection(_URL);
                _connection.Open();
                Logger.Instance.WriteLog("Connection is open...");
                int id = 111;
                String name = "AAA";
                String sqlStr = String.Format(@"insert into test1(id,name) values ('{0}','{1}')", id, name);
                MySqlCommand command = new MySqlCommand(sqlStr, _connection);
                if (command.ExecuteNonQuery() > 0)
                {
                    Console.WriteLine("数据插入成功!");
                }
            }
            catch (MySqlException e)
            {
                Logger.Instance.WriteException(e);
            }
            finally
            {
                _connection.Close();
                Logger.Instance.WriteLog("Connection is close...");
            }
        }
    }
}
