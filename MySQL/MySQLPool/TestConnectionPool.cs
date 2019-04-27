using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloCSharp.MySQL.MySQLPool
{
    public class TestConnectionPool
    {
        public string _url = "server=101.132.102.203;user=root;database=springproject;port=3306;password=root;";
                
        public void Test()
        {
            ConnectionPool connectionPool = new ConnectionPool(_url);
            connectionPool.CreatePool();
            DateTime beforeTime = System.DateTime.Now;
            for (int i = 0; i < 50; i++)
            {
                MySqlConnection tempConnection = connectionPool.GetCanUseConnection();
                TestConnectionDB(tempConnection);
                connectionPool.PutInConnection(tempConnection);
            }
            DateTime afterTime = System.DateTime.Now;
            Console.WriteLine("使用连接池共耗时:" + afterTime.Subtract(beforeTime).TotalMilliseconds / 1000 +"秒");
            beforeTime = System.DateTime.Now;
            for (int i = 0; i < 50; i++)
            {
                MySqlConnection tempConnectoin = new MySqlConnection(_url);
                tempConnectoin.Open();
                TestConnectionDB(tempConnectoin);
                tempConnectoin.Close();
            }
            afterTime = System.DateTime.Now;
            Console.WriteLine("不使用连接池共耗时:" + afterTime.Subtract(beforeTime).TotalMilliseconds / 1000 + "秒");
            Console.ReadKey();
        }

        public void TestConnectionDB(MySqlConnection connection)
        {
            try
            {
                string selectSql = "select * from userinfo where userName = ''";
                MySqlCommand command = new MySqlCommand(selectSql, connection);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine(reader.GetString("id"));
                    Console.WriteLine(reader.GetString("deleteFlag"));
                    Console.WriteLine(reader.GetString("userName"));
                    Console.WriteLine(reader.GetString("userPwd"));
                    Console.WriteLine(reader.GetString("userPhone"));
                    Console.WriteLine(reader.GetString("verifyCode"));
                    Console.WriteLine(reader.GetString("registerTime"));
                    Console.WriteLine(reader.GetString("rank"));
                    Console.WriteLine(reader.GetString("email"));
                }
                reader.Close();
            }
            catch (Exception e)
            {
            }
        }
    }
}
