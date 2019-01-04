using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloCSharp.MySQL.MySQLPool
{
    public class ConnectionInfo
    {
        //数据库连接
        private MySqlConnection _connection;
        //连接是否在使用
        private bool _useFlag;

        public ConnectionInfo(ref MySqlConnection connection)
        {
            _connection = connection;
        }

        public MySqlConnection GetConnection()
        {
            return _connection;
        }

        public void SetConnectoin(ref MySqlConnection connection)
        {
            this._connection = connection;
        }

        public bool GetUseFlag()
        {
            return _useFlag;
        }

        public void SetUseFlag(bool useFlag)
        {
            this._useFlag = useFlag;
        }
    }
}
