using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HelloCSharp.MySQL.MySQLPool
{
    public class ConnectionPool
    {
        private string _url = "";
        //连接池初始连接数
        private int _initConnections = 10;
        //连接池自动增加数
        private int _autoAddSize = 5;
        //最大连接数
        private int _maxConnections = 50;
        //
        private List<ConnectionInfo> _connectionInfoList;

        public ConnectionPool(string url)
        {
            this._url = url;
        }

        public int GetInitConnections()
        {
            return _initConnections;
        }

        public void SetInitConnections(int num)
        {
            this._initConnections = num;
        }

        public int GetAutoAddSize()
        {
            return _autoAddSize;
        }

        public void SetAutoAddSize(int num)
        {
            this._autoAddSize = num;
        }

        public int GetMaxConnections()
        {
            return _maxConnections;
        }

        public void SetMaxConnections(int num)
        {
            this._maxConnections = num;
        }

        public void CreatePool()
        {
            if (string.IsNullOrEmpty(_url))
            {
                return;
            }
            _connectionInfoList = new List<ConnectionInfo>();
            CreateConnections(_initConnections);
        }

        /// <summary>
        /// 创建指定数目的数据库连接
        /// </summary>
        /// <param name="num"></param>
        public void CreateConnections(int num)
        {
            for (int i = 0; i < num; i++)
            {
                //不能超过连接池的最大连接数
                if (_maxConnections > 0 && _connectionInfoList.Count() >= _maxConnections)
                {
                    break;
                }
                MySqlConnection tempConnection = CreateNewConnection();
                _connectionInfoList.Add(new ConnectionInfo(ref tempConnection));
            }
        }

        /// <summary>
        /// 创建一个新的连接
        /// </summary>
        /// <returns></returns>
        public MySqlConnection CreateNewConnection()
        {
            MySqlConnection connection = new MySqlConnection(_url);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// 获取一个可用的数据库连接，如果当前没有可用的数据库连接则不能创建新的连接
        /// </summary>
        /// <returns></returns>
        public MySqlConnection GetCanUseConnection()
        {
            if (_connectionInfoList == null)
            {
                return null;
            }
            //从连接池里获取一个可用的数据库连接
            MySqlConnection canUseConnection = GetCanUseConnectoinByPool();
            //如果当前没有可使用的连接即所有连接都在使用中
            while (canUseConnection == null)
            {
                Thread.Sleep(500);
                //重新获取连接，直到获取到可有的连接
                canUseConnection = GetCanUseConnectoinByPool();
            }
            return canUseConnection;
        }

        /// <summary>
        /// 从连接池里获取一个可用的数据库连接
        /// </summary>
        /// <returns></returns>
        public MySqlConnection GetCanUseConnectoinByPool()
        {
            MySqlConnection connection = null;
            foreach (ConnectionInfo tempInfo in _connectionInfoList)
            {
                if (tempInfo.GetUseFlag())
                {
                    continue;
                }
                connection = tempInfo.GetConnection();
                tempInfo.SetUseFlag(true);
                //测试此连接是否可用
                if (!TestUsableConnection(ref connection))
                {
                    try
                    {
                        connection = CreateNewConnection();
                    }
                    catch (Exception e)
                    {
                    }
                    tempInfo.SetConnectoin(ref connection);
                }
                break;
            }
            if (connection == null)
            {
                CreateConnections(_autoAddSize);
                connection = GetCanUseConnectoinByPool();
            }
            return connection;
        }

        /// <summary>
        /// 将一个数据库连接放回连接池中，并将状态置为闲置
        /// </summary>
        /// <param name="connection"></param>
        public void PutInConnection(MySqlConnection connection)
        {
            //确保连接池存在
            if (_connectionInfoList == null)
            {
                return;
            }
            foreach (ConnectionInfo tempInfo in _connectionInfoList)
            {
                if (tempInfo.GetConnection() == connection)
                {
                    tempInfo.SetUseFlag(false);
                    break;
                }
            }
        }

        /// <summary>
        /// 测试一个连接是否可用，如果不可用则关闭
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public bool TestUsableConnection(ref MySqlConnection connection)
        {
            try
            {
                string selectStr = "select * from phonenumber_copy where name = ''";
                MySqlCommand command = new MySqlCommand(selectStr, connection);
                
                MySqlDataReader dataReader = command.ExecuteReader();
                dataReader.Close();
                return true;
            }
            catch
            {
                CloseConnection(connection);
                return false;
            }
        }

        /// <summary>
        /// 关闭一个数据库连接
        /// </summary>
        /// <param name="connection"></param>
        public void CloseConnection(MySqlConnection connection)
        {
            try
            {
                connection.Close();
            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// 关闭连接池中所有的连接并清空
        /// </summary>
        public void CloseConnectionPool()
        {
            //确保连接池已存在
            if (_connectionInfoList == null)
            {
                return;
            }
            foreach (ConnectionInfo tempInfo in _connectionInfoList)
            {
                //如果该连接正在使用，等待5秒后关闭
                if (tempInfo.GetUseFlag())
                {
                    Thread.Sleep(5000);
                }
                CloseConnection(tempInfo.GetConnection());
                _connectionInfoList.Remove(tempInfo);
            }
            _connectionInfoList.Clear();
            _connectionInfoList = null;
        }

        /// <summary>
        /// 刷新连接池中所有连接
        /// </summary>
        public void RefreshConnectionList()
        {
            //确保连接池存在
            if (_connectionInfoList == null)
            {
                return;
            }
            foreach (ConnectionInfo tempInfo in _connectionInfoList)
            {
                if (tempInfo.GetUseFlag())
                {
                    Thread.Sleep(1000);
                }
                //关闭这个连接
                CloseConnection(tempInfo.GetConnection());
                //创建一个新的连接
                MySqlConnection tempConnection = CreateNewConnection();
                //替换
                tempInfo.SetConnectoin(ref tempConnection);
                tempInfo.SetUseFlag(false);
            }
        }

    }
}
