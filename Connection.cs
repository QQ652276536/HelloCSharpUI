using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace OnlyEatNotWash
{
    class Connection
    {
        public static String URL = "server=101.132.102.203;user=root;database=test;port=3306;password=root;";

        public void StartConnection()
        {
            MySqlConnection connection = new MySqlConnection(URL);
            connection.Open();
            int id = 111;
            String name = "A";
            String sqlStr = String.Format(@"insert into test1(id,name) values ('{0}','{1}')", id, name);
            MySqlCommand command = new MySqlCommand(sqlStr,connection);
            if (command.ExecuteNonQuery() > 0)
            {
                Console.WriteLine("数据插入成功!");
            }
            Console.ReadLine();
            connection.Close();
        }
    }
}
