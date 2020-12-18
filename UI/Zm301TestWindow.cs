using HelloCSharp.Log;
using HelloCSharp.Util;
using System;
using System.Data;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace HelloCSharp.UI
{
    public partial class Zm301TestWidnow : Form
    {
        /// <summary>
        /// 工号文本框委托
        /// </summary>
        /// <param name="str">文本内容</param>
        private delegate void WorkIdTxtDele(string str);

        /// <summary>
        /// 表箱号文本框委托
        /// </summary>
        /// <param name="str">文本内容</param>
        private delegate void BoxIdTxtDele(string str);

        /// <summary>
        /// 位置文本框委托
        /// </summary>
        /// <param name="str">文本内容</param>
        private delegate void GpsTxtDele(string str);

        /// <summary>
        /// 日志文本框委托
        /// </summary>
        /// <param name="str">文本内容</param>
        private delegate void LogTxtDele(string str);

        /// <summary>
        /// 开锁一，校验码已提前算好，如果有设置蓝牙名称则需要重新生成指令并计算校验码
        /// </summary>
        private string OPEN_DOOR1 = "FE FE FE FE 68 22 23 01 56 34 00 68 10 01 34 E5 16";
        private byte[] OPEN_DOOR1_BYTE;

        /// <summary>
        /// 开锁二，校验码已提前算好，如果有设置蓝牙名称则需要重新生成指令并计算校验码
        /// </summary>
        private string OPEN_DOOR2 = "FE FE FE FE 68 22 23 01 56 34 00 68 10 01 35 E6 16";
        private byte[] OPEN_DOOR2_BYTE;

        /// <summary>
        /// 开锁三，校验码已提前算好，如果有设置蓝牙名称则需要重新生成指令并计算校验码
        /// </summary>
        private string OPEN_DOOR3 = "FE FE FE FE 68 22 23 01 56 34 00 68 10 01 36 E7 16";
        private byte[] OPEN_DOOR3_BYTE;

        /// <summary>
        /// 开全部锁，校验码已提前算好，如果有设置蓝牙名称则需要重新生成指令并计算校验码
        /// </summary>
        private string OPEN_DOOR_ALL = "FE FE FE FE 68 22 23 01 56 34 00 68 10 01 3A EB 16";
        private byte[] OPEN_DOOR_ALL_BYTE;

        /// <summary>
        /// 读取工号，校验码已提前算好，如果有设置蓝牙名称则需要重新生成指令并计算校验码
        /// </summary>
        private string READ_WORK = "FE FE FE FE 68 22 23 01 56 34 00 68 0B 00 AB 16";
        private byte[] READ_WORK_BYTE;

        /// <summary>
        /// 读取表箱号，校验码已提前算好，如果有设置蓝牙名称则需要重新生成指令并计算校验码
        /// </summary>
        private string READ_BOX = "FE FE FE FE 68 22 23 01 56 34 00 68 06 00 A6 16";
        private byte[] READ_BOX_BYTE;

        /// <summary>
        /// 读取GPS位置，校验码已提前算好，如果有设置蓝牙名称则需要重新生成指令并计算校验码
        /// </summary>
        private string READ_GPS = "FE FE FE FE 68 22 23 01 56 34 00 68 0A 00 AA 16";
        private byte[] READ_GPS_BYTE;

        /// <summary>
        /// 查询事件总次数，校验码已提前算好，如果有设置蓝牙名称则需要重新生成指令并计算校验码
        /// </summary>
        private string READ_EVENT_ALL = "FE FE FE FE 68 22 23 01 56 34 00 68 00 00 A0 16";
        private byte[] READ_EVENT_ALL_BYTE;

        /// <summary>
        /// 波特率
        /// </summary>
        private readonly int[] RATE_ARRAY = new int[] { 1200, 2400, 4800, 9600, 19200, 38400, 56000, 57600, 115200 };

        /// <summary>
        /// 数据位
        /// </summary>
        private readonly int[] DATA_ARRAY = new int[] { 8, 7, 6, 5, 4 };

        /// <summary>
        /// 校验位
        /// </summary>
        private readonly string[] PARITY_ARRAY = new string[] { "None", "Odd", "Even", "Mark", "Space" };

        /// <summary>
        /// 工号的正则表达式
        /// </summary>
        private readonly Regex REGEX_WORK_ID = new Regex("[A-Z a-z 0-9]{8}");

        private MyLogger _logger = MyLogger.Instance;
        private SerialPort _serialPort;
        //查询事件总次数的线程、查询开锁事件的线程、查询关锁事件的线程、查询开门事件的线程、查询关门事件的线程、查询窃电事件的线程、查询振动事件的线程
        private Thread _eventAllThread, _openLockThread, _closeLockThread, _openDoorThread, _closeDoorThread, _stealThread, _vibrateThread;
        private bool _eventAllThreadFlag = true, _openLockThreadFlag = true, _closeLockThreadFlag = true, _openDoorThreadFlag = true, _closeDoorThreadFlag = true, _stealThreadFlag = true, _vibrateThreadFlag = true;
        //查询开锁事件、查询关锁事件、查询开门事件、查询关门事件、查询窃电事件、查询振动事件
        private int _openLock = 0, _closeLock = 0, _openDoor = 0, _closeDoor = 0, _steal = 0, _vibrate = 0;
        private string[] _portNameArray;
        private string _receivedStr = "";
        //标示具体操作
        private string _operation = "";
        private int _tabSelectedIndex = 0;
        //蓝牙名称，如果设置了每次发送指令的时候都是要包含进去的
        private string _hexName = "";

        //串口名
        private string _portName = "";
        //波特率、数据位
        private int _rate = 1200, _data = 8;
        //校验位
        private Parity _parity = Parity.None;

        public Zm301TestWidnow()
        {
            InitializeComponent();
            InitData();
            InitView();
        }

        /// <summary>
        /// 循环测试_振动事件查询
        /// </summary>
        private void CycleTest_Vibrate()
        {
            while (_vibrateThreadFlag && _eventAllThreadFlag && null != _serialPort && _serialPort.IsOpen)
            {
                LogTxtChangedByDele("循环测试_振动事件查询\r\n");
            }
            Thread.Sleep(100);
        }

        /// <summary>
        /// 循环测试_窃电事件查询
        /// </summary>
        private void CycleTest_Steal()
        {
            while (_stealThreadFlag && _eventAllThreadFlag && null != _serialPort && _serialPort.IsOpen)
            {
                LogTxtChangedByDele("循环测试_窃电事件查询\r\n");
            }
            Thread.Sleep(100);
        }

        /// <summary>
        /// 循环测试_关门事件查询
        /// </summary>
        private void CycleTest_CloseDoor()
        {
            while (_closeDoorThreadFlag && _eventAllThreadFlag && null != _serialPort && _serialPort.IsOpen)
            {
                LogTxtChangedByDele("循环测试_关门事件查询\r\n");
            }
            Thread.Sleep(100);
        }

        /// <summary>
        /// 循环测试_开门事件查询
        /// </summary>
        private void CycleTest_OpenDoor()
        {
            while (_openDoorThreadFlag && _eventAllThreadFlag && null != _serialPort && _serialPort.IsOpen)
            {
                LogTxtChangedByDele("循环测试_开门事件查询\r\n");
            }
            Thread.Sleep(100);
        }

        /// <summary>
        /// 循环测试_关锁事件查询
        /// </summary>
        private void CycleTest_CloseLock()
        {
            while (_closeLockThreadFlag && _eventAllThreadFlag && null != _serialPort && _serialPort.IsOpen)
            {
                LogTxtChangedByDele("循环测试_关锁事件查询\r\n");
            }
            Thread.Sleep(100);
        }

        /// <summary>
        /// 循环测试_开锁事件查询
        /// </summary>
        private void CycleTest_OpenLock()
        {
            while (_openLockThreadFlag && _eventAllThreadFlag && null != _serialPort && _serialPort.IsOpen)
            {
                LogTxtChangedByDele("循环测试_开锁事件查询\r\n");
            }
            Thread.Sleep(100);
        }

        /// <summary>
        /// 循环测试
        /// </summary>
        private void CycleTest()
        {
            while (_eventAllThreadFlag && null != _serialPort && _serialPort.IsOpen)
            {
                _serialPort.Write(READ_EVENT_ALL_BYTE, 0, READ_EVENT_ALL_BYTE.Length);
                LogTxtChangedByDele("发送查询事件总数指令：" + READ_EVENT_ALL + "\r\n");
                Thread.Sleep(1 * 1000);
                //开启循环测试开锁事件查询
                if (_openLock == 1)
                {
                    if (null == _openLockThread)
                    {
                        _openLockThread = new Thread(CycleTest_OpenLock);
                        _openLockThread.Start();
                    }
                }
                //开启循环测试关锁事件查询
                if (_closeLock == 1)
                {
                    if (null == _closeLockThread)
                    {
                        _closeLockThread = new Thread(CycleTest_CloseLock);
                        _closeLockThread.Start();
                    }
                }
                //开启循环测试开门事件查询
                if (_openDoor == 1)
                {
                    if (null == _openDoorThread)
                    {
                        _openDoorThread = new Thread(CycleTest_OpenDoor);
                        _openDoorThread.Start();
                    }
                }
                //开启循环测试关门事件查询
                if (_closeDoor == 1)
                {
                    if (null == _closeDoorThread)
                    {
                        _closeDoorThread = new Thread(CycleTest_CloseDoor);
                        _closeDoorThread.Start();
                    }
                }
                //开启循环测试窃电事件查询
                if (_steal == 1)
                {
                    if (null == _stealThread)
                    {
                        _stealThread = new Thread(CycleTest_Steal);
                        _stealThread.Start();
                    }
                }
                //开启循环测试振动事件查询
                if (_vibrate == 1)
                {
                    if (null == _vibrateThread)
                    {
                        _vibrateThread = new Thread(CycleTest_Vibrate);
                        _vibrateThread.Start();
                    }
                }
            }
        }

        /// <summary>
        /// 打印Log+控制台输出
        /// </summary>
        /// <param name="str"></param>
        private void Print(string str)
        {
            _logger.WriteLog(str);
            Console.WriteLine(str);
        }

        /// <summary>
        /// 修改GPS文本框内容
        /// </summary>
        /// <param name="str"></param>
        private void GpsTxtChangedByDele(string str)
        {
            //非UI线程访问控件时
            if (txt_gps.InvokeRequired)
                txt_gps.Invoke(new GpsTxtDele(GpsTxtChangedByDele), str);
            else
                txt_gps.Text = str;
        }


        /// <summary>
        /// 修改表箱号文本框内容
        /// </summary>
        /// <param name="str"></param>
        private void BoxIdTxtChangedByDele(string str)
        {
            //非UI线程访问控件时
            if (txt_box_id.InvokeRequired)
                txt_box_id.Invoke(new BoxIdTxtDele(BoxIdTxtChangedByDele), str);
            else
                txt_box_id.Text = str;
        }

        /// <summary>
        /// 修改工号文本框的内容
        /// </summary>
        /// <param name="str"></param>
        private void WorkIdTxtChangedByDele(string str)
        {
            //非UI线程访问控件时
            if (txt_work_id.InvokeRequired)
                txt_work_id.Invoke(new WorkIdTxtDele(WorkIdTxtChangedByDele), str);
            else
                txt_work_id.Text = str;
        }

        /// <summary>
        /// 修改日志文本框的内容
        /// </summary>
        /// <param name="str"></param>
        private void LogTxtChangedByDele(string str)
        {
            Print(str);
            //判断给哪个Tab下的日志文本框添加内容
            switch (_tabSelectedIndex)
            {
                case 0:
                    //非UI线程访问控件时
                    if (txt_log.InvokeRequired)
                        txt_log.Invoke(new LogTxtDele(LogTxtChangedByDele), str);
                    else
                        txt_log.AppendText(str);
                    break;
                case 1:
                    //非UI线程访问控件时
                    if (txt_log2.InvokeRequired)
                        txt_log2.Invoke(new LogTxtDele(LogTxtChangedByDele), str);
                    else
                        txt_log2.AppendText(str);
                    break;
            }
        }

        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitView()
        {
            EnableButton(false);
        }

        /// <summary>
        /// 初始化控件需要的数据
        /// </summary>
        private void InitData()
        {
            OPEN_DOOR1_BYTE = MyConvertUtil.HexStrToBytes(OPEN_DOOR1);
            OPEN_DOOR2_BYTE = MyConvertUtil.HexStrToBytes(OPEN_DOOR2);
            OPEN_DOOR3_BYTE = MyConvertUtil.HexStrToBytes(OPEN_DOOR3);
            OPEN_DOOR_ALL_BYTE = MyConvertUtil.HexStrToBytes(OPEN_DOOR_ALL);
            READ_WORK_BYTE = MyConvertUtil.HexStrToBytes(READ_WORK);
            READ_BOX_BYTE = MyConvertUtil.HexStrToBytes(READ_BOX);
            READ_GPS_BYTE = MyConvertUtil.HexStrToBytes(READ_GPS);
            READ_EVENT_ALL_BYTE = MyConvertUtil.HexStrToBytes(READ_EVENT_ALL);
            //串口Combox赋值
            _portNameArray = SerialPort.GetPortNames();
            DataTable dataPoarName = new DataTable();
            dataPoarName.Columns.Add("value");
            foreach (string temp in _portNameArray)
            {
                DataRow dataRow = dataPoarName.NewRow();
                dataRow[0] = temp;
                dataPoarName.Rows.Add(dataRow);
            }
            if (dataPoarName.Rows.Count > 0)
            {
                cbx_port.DataSource = dataPoarName;
                cbx_port.ValueMember = "value";
            }
            //波特率Combox赋值
            DataTable dataRate = new DataTable();
            dataRate.Columns.Add("value");
            foreach (int temp in RATE_ARRAY)
            {
                DataRow dataRow = dataRate.NewRow();
                dataRow[0] = temp;
                dataRate.Rows.Add(dataRow);
            }
            if (dataRate.Rows.Count > 0)
            {
                cbx_rate.DataSource = dataRate;
                cbx_rate.ValueMember = "value";
            }
            //数据位Combox赋值
            DataTable dataData = new DataTable();
            dataData.Columns.Add("value");
            foreach (int temp in DATA_ARRAY)
            {
                DataRow dataRow = dataData.NewRow();
                dataRow[0] = temp;
                dataData.Rows.Add(dataRow);
            }
            if (dataData.Rows.Count > 0)
            {
                cbx_data.DataSource = dataData;
                cbx_data.ValueMember = "value";
            }
            //校验位Combox赋值
            DataTable dataParity = new DataTable();
            dataParity.Columns.Add("value");
            foreach (string temp in PARITY_ARRAY)
            {
                DataRow dataRow = dataParity.NewRow();
                dataRow[0] = temp;
                dataParity.Rows.Add(dataRow);
            }
            if (dataParity.Rows.Count > 0)
            {
                cbx_parity.DataSource = dataParity;
                cbx_parity.ValueMember = "value";
            }
        }

        /// <summary>
        /// 启用禁用按钮控件
        /// </summary>
        /// <param name="flag"></param>
        private void EnableButton(bool flag)
        {
            btn_lock1.Enabled = flag;
            btn_lock2.Enabled = flag;
            btn_lock3.Enabled = flag;
            btn_lock_all.Enabled = flag;
            btn_work_read.Enabled = flag;
            btn_work_write.Enabled = flag;
            btn_box_read.Enabled = flag;
            btn_box_write.Enabled = flag;
            btn_gps.Enabled = flag;
            btn_cycle_start.Enabled = flag;
            btn_name_write.Enabled = flag;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="str"></param>
        private void Parse(string str)
        {
            string[] strArray = str.Split(' ');
            string type = strArray[8];
            switch (type)
            {
                //事件总数
                case "80":
                    {
                        int len = MyConvertUtil.HexStrToInt(strArray[9]);
                        //0表示没有，1表示有
                        //开锁事件
                        _openLock = Convert.ToInt32(strArray[10]) - 33;
                        //开锁事件次数
                        int openLockCount = Convert.ToInt32(strArray[12]) + Convert.ToInt32(strArray[11]);
                        //关锁事件
                        _closeLock = Convert.ToInt32(strArray[13]) - 33;
                        //关锁事件次数
                        int closeLockCount = Convert.ToInt32(strArray[15]) + Convert.ToInt32(strArray[14]);
                        //开门事件
                        _openDoor = Convert.ToInt32(strArray[16]) - 33;
                        //开门事件次数
                        int openDoorCount = Convert.ToInt32(strArray[18]) + Convert.ToInt32(strArray[17]);
                        //关门事件
                        _closeDoor = Convert.ToInt32(strArray[19]) - 33;
                        //关门事件次数
                        int closeDoorCount = Convert.ToInt32(strArray[21]) + Convert.ToInt32(strArray[20]);
                        //窃电事件
                        _steal = Convert.ToInt32(strArray[22]) - 33;
                        //窃电事件次数
                        int stealCount = Convert.ToInt32(strArray[24]) + Convert.ToInt32(strArray[23]);
                        //振动事件
                        _vibrate = Convert.ToInt32(strArray[25]) - 33;
                        //振动事件次数
                        int vibrateCount = Convert.ToInt32(strArray[27]) + Convert.ToInt32(strArray[26]);
                    }
                    break;
                //开锁
                case "90":
                    {
                        string result = strArray[9];
                        switch (_operation)
                        {
                            case "开锁一":
                                if ("00".Equals(result))
                                    LogTxtChangedByDele("开锁一成功：" + result + "\r\n");
                                else
                                    LogTxtChangedByDele("开锁一失败：" + result + "\r\n");
                                break;
                            case "开锁二":
                                if ("00".Equals(result))
                                    LogTxtChangedByDele("开锁二成功：" + result + "\r\n");
                                else
                                    LogTxtChangedByDele("开锁二失败：" + result + "\r\n");
                                break;
                            case "开锁三":
                                if ("00".Equals(result))
                                    LogTxtChangedByDele("开锁三成功：" + result + "\r\n");
                                else
                                    LogTxtChangedByDele("开锁三失败：" + result + "\r\n");
                                break;
                            case "开锁全部":
                                if ("00".Equals(result))
                                    LogTxtChangedByDele("开锁全部成功：" + result + "\r\n");
                                else
                                    LogTxtChangedByDele("开锁全部失败：" + result + "\r\n");
                                break;
                        }
                    }
                    break;
                //工号
                case "8B":
                    {
                        int len = MyConvertUtil.HexStrToInt(strArray[9]);
                        string hexWorkId = strArray[10] + strArray[11] + strArray[12] + strArray[13] + strArray[14] + strArray[15] + strArray[16] + strArray[17];
                        string workId = MyConvertUtil.HexStrToStr(hexWorkId);
                        switch (_operation)
                        {
                            case "读取工号":
                                WorkIdTxtChangedByDele(workId);
                                LogTxtChangedByDele("读取工号成功：" + workId + "\r\n");
                                break;
                            case "设置工号":
                                LogTxtChangedByDele("设置工号成功：" + workId + "\r\n");
                                break;
                        }
                    }
                    break;
                //表箱号
                case "86":
                    string hexBoxId = strArray[1] + strArray[2] + strArray[3] + strArray[4] + strArray[5] + strArray[6];
                    switch (_operation)
                    {
                        case "读取表箱号":
                            BoxIdTxtChangedByDele(hexBoxId);
                            LogTxtChangedByDele("读取表箱号成功：" + hexBoxId + "\r\n");
                            break;
                        case "设置表箱号":
                            LogTxtChangedByDele("设置表箱号成功：" + hexBoxId + "\r\n");
                            break;
                    }
                    break;
                //读取GPS位置
                case "8A":
                    {
                        int len = MyConvertUtil.HexStrToInt(strArray[9]);
                        //状态信息：00未定位，1已定位
                        int state = Convert.ToInt32(strArray[10]) - 33;
                        if (1 == state)
                        {
                            //时间
                            int ss = Convert.ToInt32(strArray[11]) - 33;
                            int mm = Convert.ToInt32(strArray[12]) - 33;
                            int HH = Convert.ToInt32(strArray[13]) - 33;
                            int dd = Convert.ToInt32(strArray[14]) - 33;
                            int MM = Convert.ToInt32(strArray[15]) - 33;
                            int yy = Convert.ToInt32(strArray[16]) - 33;
                            string timeStr = yy + "年" + MM + "月" + dd + "日" + HH + "时" + mm + "分" + ss + "秒";
                            //经纬度
                            int lat1 = Convert.ToInt32(strArray[17]) - 33;
                            int lat2 = Convert.ToInt32(strArray[18]) - 33;
                            int lat3 = Convert.ToInt32(strArray[19]) - 33;
                            int lat4 = Convert.ToInt32(strArray[20]) - 33;
                            string latStr = "" + lat1 + lat2 + lat3 + lat4;
                            int lat = Convert.ToInt32(latStr) * 1000000;
                            int lot1 = Convert.ToInt32(strArray[21]) - 33;
                            int lot2 = Convert.ToInt32(strArray[22]) - 33;
                            int lot3 = Convert.ToInt32(strArray[23]) - 33;
                            int lot4 = Convert.ToInt32(strArray[24]) - 33;
                            string lotStr = "" + lot1 + lot2 + lot3 + lot4;
                            int lot = Convert.ToInt32(lotStr) * 1000000;
                            //海拔
                            int height1 = Convert.ToInt32(strArray[25]) - 33;
                            int height2 = Convert.ToInt32(strArray[26]) - 33;
                            string heightStr = "" + height1 + height2;
                            int height = Convert.ToInt32(heightStr);
                            //速度
                            int speed1 = Convert.ToInt32(strArray[27]) - 33;
                            int speed2 = Convert.ToInt32(strArray[28]) - 33;
                            string speedStr = "" + speed1 + speed2;
                            int speed = Convert.ToInt32(speedStr);
                            //方向
                            int dir1 = Convert.ToInt32(strArray[29]) - 33;
                            int dir2 = Convert.ToInt32(strArray[30]) - 33;
                            string dirStr = "" + dir1 + dir2;
                            int dir = Convert.ToInt32(dirStr);
                            //温度
                            int temperature = Convert.ToInt32(strArray[31]) - 33;
                            string parseStr = timeStr + "   经度：" + lat + "   纬度：" + lot + "  海拔：" + height + "米   速度" + speed + "   方向：" + dir + "   温度：" + temperature;
                            GpsTxtChangedByDele(parseStr);
                            LogTxtChangedByDele(parseStr + "\r\n");
                        }
                        else
                        {
                            GpsTxtChangedByDele("未定位");
                            LogTxtChangedByDele("未定位\r\n");
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 异步接收
        /// 注意，收到数据就会触发该方法，每次收到的数据不一定是完整的，需要校对
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceivedComData(object sender, SerialDataReceivedEventArgs e)
        {
            if (null == _serialPort || !_serialPort.IsOpen)
                return;
            //待读字节长度
            int byteLen = _serialPort.BytesToRead;
            byte[] byteArray = new byte[byteLen];
            _serialPort.Read(byteArray, 0, byteArray.Length);
            string str = MyConvertUtil.BytesToStr(byteArray);
            Print("本次读取字节：" + str + "，长度：" + byteLen);
            _receivedStr += str;
            //不能仅凭指令的开头和结尾来判断是否为一包完整数据，还需要验证校验码
            if (_receivedStr.StartsWith("68") && _receivedStr.EndsWith("16"))
            {
                Print("累计收到字节（Hex）：" + _receivedStr);
                //计算收到的数据的校验码的时候不包含最后现个字节
                string tempReceivedStr = _receivedStr.Substring(0, _receivedStr.Length - 4);
                Print("参与计算校验码的数据（Hex）：" + tempReceivedStr);
                //计算出的校验码
                string crcStr1 = MyConvertUtil.CalcZM301CRC(tempReceivedStr);
                Print("计算出的校验码（Hex）：" + crcStr1);
                string[] strArray = MyConvertUtil.StrSplitInterval(_receivedStr, 2);
                //收到的数据里的校验码
                string crcStr2 = strArray[strArray.Length - 2];
                Print("数据内容包含的校验码（Hex）：" + crcStr2);
                //比较校验码
                bool flag = crcStr1.Equals(crcStr2);
                Print("校验码是否正确：" + flag);
                if (flag)
                {
                    int len = _receivedStr.Length;
                    Print("收到完整的指令（Hex）：" + _receivedStr + "，数据长度：" + len);
                    _receivedStr = MyConvertUtil.StrAddChar(_receivedStr, 2, " ");
                    //显示数据
                    LogTxtChangedByDele("收到（Hex）：" + _receivedStr + "，数据长度：" + len + "\r\n");
                    //解析数据
                    Parse(_receivedStr);
                    //清空缓存
                    _receivedStr = "";
                }
            }
        }

        private void txt_name_TextChanged(object sender, EventArgs e)
        {
        }

        private void txt_work_id_TextChanged(object sender, EventArgs e)
        {
        }

        private void txt_box_id_TextChanged(object sender, EventArgs e)
        {
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            _tabSelectedIndex = tabControl.SelectedIndex;
        }

        private void btn_name_write_Click(object sender, EventArgs e)
        {
            //TODO:校验五位蓝牙名称
            string name = txt_name.Text.ToString();
            _hexName = MyConvertUtil.StrToHexStr(name);

            //重新生成开锁一指令并计算校验码
            //OPEN_DOOR1 = "FE FE FE FE 68 22 23 01 56 34 00 68 10 01 34 E5 16";
            string openLock1Cmd = "68" + _hexName + "0068100134";
            string openLock1Crc = MyConvertUtil.CalcZM301CRC(openLock1Cmd);
            OPEN_DOOR1 = "FEFEFEFE" + openLock1Cmd + openLock1Crc + "16";
            //添加空格
            OPEN_DOOR1 = MyConvertUtil.StrAddChar(OPEN_DOOR1, 2, " ");
            OPEN_DOOR1_BYTE = MyConvertUtil.HexStrToBytes(OPEN_DOOR1);

            //重新生成开锁二指令并计算校验码
            //OPEN_DOOR2 = "FE FE FE FE 68 22 23 01 56 34 00 68 10 01 35 E6 16";
            string openLock2Cmd = "68" + _hexName + "0068100135";
            string openLock2Crc = MyConvertUtil.CalcZM301CRC(openLock2Cmd);
            OPEN_DOOR2 = "FEFEFEFE" + openLock2Cmd + openLock2Crc + "16";
            //添加空格
            OPEN_DOOR2 = MyConvertUtil.StrAddChar(OPEN_DOOR2, 2, " ");
            OPEN_DOOR2_BYTE = MyConvertUtil.HexStrToBytes(OPEN_DOOR2);

            //重新生成开锁三指令并计算校验码
            //OPEN_DOOR3 = "FE FE FE FE 68 22 23 01 56 34 00 68 10 01 36 E7 16";
            string openLock3Cmd = "68" + _hexName + "0068100136";
            string openLock3Crc = MyConvertUtil.CalcZM301CRC(openLock3Cmd);
            OPEN_DOOR3 = "FEFEFEFE" + openLock3Cmd + openLock3Crc + "16";
            //添加空格
            OPEN_DOOR3 = MyConvertUtil.StrAddChar(OPEN_DOOR3, 2, " ");
            OPEN_DOOR3_BYTE = MyConvertUtil.HexStrToBytes(OPEN_DOOR3);

            //重新生成开全部锁指令并计算校验码
            //OPEN_DOOR_ALL = "FE FE FE FE 68 22 23 01 56 34 00 68 10 01 3A EB 16";
            string openLockAllCmd = "68" + _hexName + "006810013A";
            string openLockAllCrc = MyConvertUtil.CalcZM301CRC(openLockAllCmd);
            OPEN_DOOR_ALL = "FEFEFEFE" + openLockAllCmd + openLockAllCrc + "16";
            //添加空格
            OPEN_DOOR_ALL = MyConvertUtil.StrAddChar(OPEN_DOOR_ALL, 2, " ");
            OPEN_DOOR_ALL_BYTE = MyConvertUtil.HexStrToBytes(OPEN_DOOR_ALL);

            //重新生成读取工号指令并计算校验码
            //READ_WORK = "FE FE FE FE 68 22 23 01 56 34 00 68 0B 00 AB 16";
            string readWorkCmd = "68" + _hexName + "00680B00";
            string readWorkCrc = MyConvertUtil.CalcZM301CRC(readWorkCmd);
            READ_WORK = "FEFEFEFE" + readWorkCmd + readWorkCrc + "16";
            //添加空格
            READ_WORK = MyConvertUtil.StrAddChar(READ_WORK, 2, " ");
            READ_WORK_BYTE = MyConvertUtil.HexStrToBytes(READ_WORK);

            //重新生成读取表箱号指令并计算校验码
            //READ_BOX = "FE FE FE FE 68 22 23 01 56 34 00 68 06 00 A6 16";
            string readBoxCmd = "68" + _hexName + "00680600";
            string readBoxCrc = MyConvertUtil.CalcZM301CRC(readBoxCmd);
            READ_BOX = "FEFEFEFE" + readBoxCmd + readBoxCrc + "16";
            //添加空格
            READ_BOX = MyConvertUtil.StrAddChar(READ_BOX, 2, " ");
            READ_BOX_BYTE = MyConvertUtil.HexStrToBytes(READ_BOX);

            //需要重新生成读取GPS位置指令并计算校验码
            //READ_GPS = "FE FE FE FE 68 22 23 01 56 34 00 68 0A 00 AA 16";
            string readGpsCmd = "68" + _hexName + "00680A00";
            string readGpsCrc = MyConvertUtil.CalcZM301CRC(readGpsCmd);
            READ_GPS = "FEFEFEFE" + readGpsCmd + readGpsCrc + "16";
            //添加空格
            READ_GPS = MyConvertUtil.StrAddChar(READ_GPS, 2, " ");
            READ_GPS_BYTE = MyConvertUtil.HexStrToBytes(READ_GPS);

            //重新生成查询事件总数指令并计算校验码
            //READ_EVENT_ALL = "FE FE FE FE 68 22 23 01 56 34 00 68 00 00 A0 16";
            string readEventAllCmd = "68" + _hexName + " 00680000";
            string readEventAllCrc = MyConvertUtil.CalcZM301CRC(readEventAllCmd);
            READ_EVENT_ALL = "FEFEFEFE" + readEventAllCmd + readEventAllCrc + "16";
            //添加空格
            READ_EVENT_ALL = MyConvertUtil.StrAddChar(READ_EVENT_ALL, 2, " ");
            READ_EVENT_ALL_BYTE = MyConvertUtil.HexStrToBytes(READ_EVENT_ALL);
        }

        private void btn_cycle_start_Click(object sender, EventArgs e)
        {
            if ("开始".Equals(btn_cycle_start.Text))
            {
                if (null == _eventAllThread)
                {
                    _eventAllThread = new Thread(CycleTest);
                    _eventAllThread.Start();
                }
                else
                {
                    _eventAllThreadFlag = false;
                    _eventAllThread.Abort();
                    _eventAllThread = null;
                    _eventAllThread = new Thread(CycleTest);
                    _eventAllThread.Start();
                }
                btn_cycle_start.Text = "停止";
            }
            else
            {
                _eventAllThreadFlag = false;
                _openLockThreadFlag = false;
                _closeLockThreadFlag = false;
                _openDoorThreadFlag = false;
                _closeDoorThreadFlag = false;
                _stealThreadFlag = false;
                _vibrateThreadFlag = false;
                if (null != _openLockThread)
                    _openLockThread.Abort();
                if (null != _closeLockThread)
                    _closeLockThread.Abort();
                if (null != _openDoorThread)
                    _openDoorThread.Abort();
                if (null != _closeDoorThread)
                    _closeDoorThread.Abort();
                if (null != _stealThread)
                    _stealThread.Abort();
                if (null != _vibrateThread)
                    _vibrateThread.Abort();
                if (null != _eventAllThread)
                    _eventAllThread.Abort();
                btn_cycle_start.Text = "开始";
            }
        }

        private void cbx_port_SelectedIndexChanged(object sender, EventArgs e)
        {
            _portName = _portNameArray[cbx_port.SelectedIndex];
            LogTxtChangedByDele("串口名称：" + _portName + "\r\n");
        }

        private void cbx_rate_SelectedIndexChanged(object sender, EventArgs e)
        {
            LogTxtChangedByDele("波特率：" + _rate + "\r\n");
        }

        private void cbx_data_SelectedIndexChanged(object sender, EventArgs e)
        {
            _data = DATA_ARRAY[cbx_data.SelectedIndex];
            LogTxtChangedByDele("数据位：" + _data + "\r\n");
        }

        private void cbx_parity_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str = PARITY_ARRAY[cbx_parity.SelectedIndex];
            switch (str)
            {
                case "None": _parity = Parity.None; break;
                case "Odd": _parity = Parity.Odd; break;
                case "Even": _parity = Parity.Even; break;
                case "Mark": _parity = Parity.Mark; break;
                case "Space": _parity = Parity.Space; break;
            }
            LogTxtChangedByDele("校验位：" + _parity + "\r\n");
        }

        /// <summary>
        /// 开启/关闭串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_open_Click(object sender, EventArgs e)
        {
            //清空缓存
            _receivedStr = "";
            try
            {
                if ("打开串口".Equals(btn_open.Text.ToString()))
                {
                    if (null == _serialPort)
                    {
                        _serialPort = new SerialPort(_portName, _rate, _parity, _data, StopBits.One);
                        _serialPort.DataReceived += new SerialDataReceivedEventHandler(ReceivedComData);
                    }
                    _serialPort.Open();
                    LogTxtChangedByDele("已打开：" + _portName + "\r\n");
                    EnableButton(true);
                    btn_open.Text = "关闭串口";
                }
                else
                {
                    _serialPort.Close();
                    LogTxtChangedByDele("已关闭：" + _portName + "\r\n");
                    EnableButton(false);
                    btn_open.Text = "打开串口";
                }
            }
            catch (Exception ex)
            {
                LogTxtChangedByDele("串口" + _portName + "打开/关闭失败，原因：" + ex.ToString() + "\r\n");
            }
        }

        /// <summary>
        /// 开锁一
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_lock1_Click(object sender, EventArgs e)
        {
            _serialPort.Write(OPEN_DOOR1_BYTE, 0, OPEN_DOOR1_BYTE.Length);
            _operation = "开锁一";
            LogTxtChangedByDele("发送开锁一指令：" + OPEN_DOOR1 + "\r\n");
        }

        /// <summary>
        /// 开锁二
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_lock2_Click(object sender, EventArgs e)
        {
            _serialPort.Write(OPEN_DOOR2_BYTE, 0, OPEN_DOOR2_BYTE.Length);
            _operation = "开锁二";
            LogTxtChangedByDele("发送开锁二指令：" + OPEN_DOOR2 + "\r\n");
        }

        /// <summary>
        /// 开锁三
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_lock3_Click(object sender, EventArgs e)
        {
            _serialPort.Write(OPEN_DOOR3_BYTE, 0, OPEN_DOOR3_BYTE.Length);
            _operation = "开锁三";
            LogTxtChangedByDele("发送开锁三指令：" + OPEN_DOOR3 + "\r\n");
        }

        /// <summary>
        /// 开锁全部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_lock_all_Click(object sender, EventArgs e)
        {
            _serialPort.Write(OPEN_DOOR_ALL_BYTE, 0, OPEN_DOOR_ALL_BYTE.Length);
            _operation = "开锁全部";
            LogTxtChangedByDele("发送开全部锁指令：" + OPEN_DOOR_ALL + "\r\n");
        }

        /// <summary>
        /// 读取工号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_work_read_Click(object sender, EventArgs e)
        {
            _serialPort.Write(READ_WORK_BYTE, 0, READ_WORK_BYTE.Length);
            _operation = "读取工号";
            LogTxtChangedByDele("发送读取工号指令：" + READ_WORK + "\r\n");
        }

        /// <summary>
        /// 设置工号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_work_write_Click(object sender, EventArgs e)
        {
            string input = txt_work_id.Text;
            bool flag = Regex.IsMatch(input, @"[A-Z a-z 0-9]{8}");
            if (flag)
            {
                string cmdStr = "68222301563400680B08";
                //有设置蓝牙名称
                if (!"".Equals(_hexName))
                    cmdStr = "68" + _hexName + "00680B08";
                String str = "";
                for (int i = 0; i < input.Length; i++)
                {
                    string temp = MyConvertUtil.CharAt(input, i);
                    temp = MyConvertUtil.StrToHexStr(temp);
                    if (temp.Length < 2)
                        temp = "0" + temp;
                    Print("遂字转换（Hex）：" + temp);
                    str += temp;
                }
                String[] strArray = MyConvertUtil.StrSplitInterval(str, 2);
                cmdStr += strArray[0] + strArray[1] + strArray[2] + strArray[3] + strArray[4] + strArray[5] + strArray[6] + strArray[7];
                string crcStr = MyConvertUtil.CalcZM301CRC(cmdStr);
                Print("计算出的校验码（Hex）：" + crcStr);
                cmdStr += crcStr + " 16";
                cmdStr = "FEFEFEFE" + cmdStr;
                byte[] comByte = MyConvertUtil.HexStrToBytes(cmdStr);
                _serialPort.Write(comByte, 0, comByte.Length);
                _operation = "设置工号";
                LogTxtChangedByDele("发送设置工号指令：" + MyConvertUtil.StrAddChar(cmdStr, 2, " ") + "\r\n");
            }
        }

        /// <summary>
        /// 读取表箱号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_box_read_Click(object sender, EventArgs e)
        {
            _serialPort.Write(READ_BOX_BYTE, 0, READ_BOX_BYTE.Length);
            _operation = "读取表箱号";
            LogTxtChangedByDele("发送读取表箱号指令：" + READ_BOX + "\r\n");
        }

        /// <summary>
        /// 设置表箱号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_box_write_Click(object sender, EventArgs e)
        {
            string input = txt_box_id.Text;
            bool flag = Regex.IsMatch(input, @"[A-Z a-z 0-9]{6}");
            if (flag)
            {
                string cmdStr = "68222301563400680606";
                //有设置蓝牙名称
                if (!"".Equals(_hexName))
                    cmdStr = "68" + _hexName + "00680606";
                String str = "";
                for (int i = 0; i < input.Length; i++)
                {
                    string temp = MyConvertUtil.CharAt(input, i);
                    temp = MyConvertUtil.StrToHexStr(temp);
                    if (temp.Length < 2)
                        temp = "0" + temp;
                    Print("遂字转换（Hex）：" + temp);
                    str += temp;
                }
                String[] strArray = MyConvertUtil.StrSplitInterval(str, 2);
                cmdStr += strArray[0] + strArray[1] + strArray[2] + strArray[3] + strArray[4] + strArray[5];
                string crcStr = MyConvertUtil.CalcZM301CRC(cmdStr);
                Print("计算出的校验码（Hex）：" + crcStr);
                cmdStr += crcStr + "16";
                cmdStr = "FEFEFEFE" + cmdStr;
                byte[] comByte = MyConvertUtil.HexStrToBytes(cmdStr);
                _serialPort.Write(comByte, 0, comByte.Length);
                _operation = "设置表箱号";
                LogTxtChangedByDele("发送设置表箱号指令：" + MyConvertUtil.StrAddChar(cmdStr, 2, " ") + "\r\n");
            }
        }

        /// <summary>
        /// 查询GPS位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_gps_Click(object sender, EventArgs e)
        {
            _serialPort.Write(READ_GPS_BYTE, 0, READ_GPS_BYTE.Length);
            LogTxtChangedByDele("发送查询GPS指令：" + READ_GPS + "\r\n");
        }

        /// <summary>
        /// 清空日志文本框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_clear2_Click(object sender, EventArgs e)
        {
            txt_log2.Clear();
        }

        /// <summary>
        /// 清空日志文本框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_clear_Click(object sender, EventArgs e)
        {
            txt_log.Clear();
        }
    }

}
