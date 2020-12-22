using HelloCSharp.Log;
using HelloCSharp.Util;
using System;
using System.Data;
using System.Drawing;
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
        /// <param name="str"></param>
        /// <param name="color"></param>
        private delegate void LogTxtDele(string str, Color color);

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
        /// 查询事件总数，校验码已提前算好，如果有设置蓝牙名称则需要重新生成指令并计算校验码
        /// </summary>
        private string READ_EVENT_ALL = "FE FE FE FE 68 22 23 01 56 34 00 68 00 00 A0 16";
        private byte[] READ_EVENT_ALL_BYTE;

        /// <summary>
        /// 查询事件_开锁，校验码已提前算好，如果有设置蓝牙名称则需要重新生成指令并计算校验码
        /// </summary>
        private string READ_EVENT_ALL_OPENLOCK = "FE FE FE FE 68 22 23 01 56 34 00 68 01 00 A1 16";
        private byte[] READ_EVENT_ALL_OPENLOCK_BYTE;

        /// <summary>
        /// 查询事件_关锁，校验码已提前算好，如果有设置蓝牙名称则需要重新生成指令并计算校验码
        /// </summary>
        private string READ_EVENT_ALL_CLOSELOCK = "FE FE FE FE 68 22 23 01 56 34 00 68 02 00 A2 16";
        private byte[] READ_EVENT_ALL_CLOSELOCK_BYTE;

        /// <summary>
        /// 查询事件_开门，校验码已提前算好，如果有设置蓝牙名称则需要重新生成指令并计算校验码
        /// </summary>
        private string READ_EVENT_ALL_OPENDOOR = "FE FE FE FE 68 22 23 01 56 34 00 68 03 00 A3 16";
        private byte[] READ_EVENT_ALL_OPENDOOR_BYTE;

        /// <summary>
        /// 查询事件_关门，校验码已提前算好，如果有设置蓝牙名称则需要重新生成指令并计算校验码
        /// </summary>
        private string READ_EVENT_ALL_CLOSEDOOR = "FE FE FE FE 68 22 23 01 56 34 00 68 04 00 A4 16";
        private byte[] READ_EVENT_ALL_CLOSEDOOR_BYTE;

        /// <summary>
        /// 查询事件_窃电，校验码已提前算好，如果有设置蓝牙名称则需要重新生成指令并计算校验码
        /// </summary>
        private string READ_EVENT_ALL_STEAL = "FE FE FE FE 68 22 23 01 56 34 00 68 05 00 A5 16";
        private byte[] READ_EVENT_ALL_STEAL_BYTE;

        /// <summary>
        /// 查询事件_振动，校验码已提前算好，如果有设置蓝牙名称则需要重新生成指令并计算校验码
        /// </summary>
        private string READ_EVENT_ALL_VIBRATE = "FE FE FE FE 68 22 23 01 56 34 00 68 09 00 A9 16";
        private byte[] READ_EVENT_ALL_VIBRATE_BYTE;

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
        /// 读取超时时间
        /// </summary>
        private readonly int TIME_OUT = 2 * 1000;

        private MyLogger _logger = MyLogger.Instance;
        //用于获取当前毫秒，Ticks为纳秒，转换为毫秒需要除以10000，转换为秒需要除以10000000
        private DateTime DATETIME = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        //用于获取当前时间
        private DateTime _nowTime = DateTime.Now;
        //串口
        private SerialPort _serialPort;
        //查询事件总数的线程、读取超时线程
        private Thread _eventAllThread, _timeOutThread;
        //查询事件总数的线程的运行标识、判断是否读取超时的线程的运行标识
        private bool _eventAllThreadFlag = true, _timeOutThreadFlag = true;
        //这些标识只能手动置为0（停止循环测试），什么时候置为1（开始循环测试）由硬件返回参数决定：是否查询开锁事件、是否查询关锁事件、是否查询开门事件、是否查询关门事件、是否查询窃电事件、是否查询振动事件
        private int _openLock = 0, _closeLock = 0, _openDoor = 0, _closeDoor = 0, _steal = 0, _vibrate = 0;
        //本机串口
        private string[] _portNameArray;
        private string _receivedStr = "";
        private int _tabSelectedIndex = 0;
        //串口名
        private string _portName = "";
        //波特率、数据位
        private int _rate = 1200, _data = 8;
        //校验位
        private Parity _parity = Parity.None;
        //记录当前指令发送时间
        private long _currentMillis = 0;
        //记录具体操作
        private string _operation = "";
        //蓝牙名称，如果设置了每次发送指令的时候都是要包含进去的
        private string _hexName = "";
        //工号，如果设置了每次循环测试都要发送一次“设置工号”
        private string _hexWorkId = "";
        //循环测试时当前执行到的步骤：0对时、1设置工号、2查询事件总数、3开锁事件、4关锁事件、5开门事件、6关门事件、7窃电事件、8振动事件
        private int _cycleTestStep = 0;

        public Zm301TestWidnow()
        {
            InitializeComponent();
            InitData();
            EnableBtn(false);
        }

        /// <summary>
        /// 监听串口响应
        /// </summary>
        private void TimeOutForThread()
        {
            while (_timeOutThreadFlag && null != _serialPort && _serialPort.IsOpen)
            {
                long tempCurrentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
                //发送指令后再判断是否读取超时
                if (_currentMillis == 0)
                    continue;
                bool timeOutFlag = tempCurrentMillis - _currentMillis > TIME_OUT;
                //一、操作类型不为空、记录当前毫秒值不为零，则说明有发送指令的动作
                //二、接收数据的缓存为空则说明未收到任何数据
                //三、当前毫秒值与记录的毫秒值之差大于指定时间则表示超时
                if (!"".Equals(_operation) && "".Equals(_receivedStr) && _currentMillis > 0 && timeOutFlag)
                {
                    switch (_operation)
                    {
                        case "开锁一": LogTxtChangedByDele("开锁一超时\r\n", Color.Red); break;
                        case "开锁二": LogTxtChangedByDele("开锁二超时\r\n", Color.Red); break;
                        case "开锁三": LogTxtChangedByDele("开锁三超时\r\n", Color.Red); break;
                        case "开锁全部": LogTxtChangedByDele("开锁全部超时\r\n", Color.Red); break;
                        case "读取工号": LogTxtChangedByDele("读取工号超时\r\n", Color.Red); break;
                        case "设置工号": LogTxtChangedByDele("设置工号超时\r\n", Color.Red); break;
                        case "读取表箱号": LogTxtChangedByDele("读取表箱号超时\r\n", Color.Red); break;
                        case "设置表箱号": LogTxtChangedByDele("设置表箱号超时\r\n", Color.Red); break;
                        case "查询GPS位置": LogTxtChangedByDele("查询GPS位置超时\r\n", Color.Red); break;
                        case "对时": LogTxtChangedByDele("对时超时\r\n", Color.Red); break;
                        case "查询事件总数": LogTxtChangedByDele("查询事件总数超时\r\n", Color.Red); break;
                        case "循环测试_开锁事件": LogTxtChangedByDele("查询开锁事件超时\r\n", Color.Red); break;
                        case "循环测试_关锁事件": LogTxtChangedByDele("查询关锁事件超时\r\n", Color.Red); break;
                        case "循环测试_开门事件": LogTxtChangedByDele("查询开门事件超时\r\n", Color.Red); break;
                        case "循环测试_关门事件": LogTxtChangedByDele("查询关门事件超时\r\n", Color.Red); break;
                        case "循环测试_窃电事件": LogTxtChangedByDele("查询窃电事件超时\r\n", Color.Red); break;
                        case "循环测试_振动事件": LogTxtChangedByDele("查询振动事件超时\r\n", Color.Red); break;
                    }
                    //重置记录的状态
                    _currentMillis = 0;
                    _operation = "";
                }
            }
        }

        /// <summary>
        /// 循环测试_振动事件查询
        /// </summary>
        private void CycleTest_Vibrate()
        {
            while (_vibrate == 1 && _eventAllThreadFlag && null != _serialPort && _serialPort.IsOpen)
            {
                //等待判断超时的线程重置记录再进行下一次测试
                if (_currentMillis > 0)
                    continue;
                _operation = "循环测试_振动事件";
                _serialPort.Write(READ_EVENT_ALL_VIBRATE_BYTE, 0, READ_EVENT_ALL_VIBRATE_BYTE.Length);
                _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
                LogTxtChangedByDele("发送查询振动事件指令：" + READ_EVENT_ALL_VIBRATE + "\r\n", Color.Black);
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// 循环测试_窃电事件查询
        /// </summary>
        private void CycleTest_Steal()
        {
            while (_steal == 1 && _eventAllThreadFlag && null != _serialPort && _serialPort.IsOpen)
            {
                //等待判断超时的线程重置记录再进行下一次测试
                if (_currentMillis > 0)
                    continue;
                _operation = "循环测试_窃电事件";
                _serialPort.Write(READ_EVENT_ALL_STEAL_BYTE, 0, READ_EVENT_ALL_STEAL_BYTE.Length);
                _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
                LogTxtChangedByDele("发送查询窃电事件指令：" + READ_EVENT_ALL_STEAL + "\r\n", Color.Black);
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// 循环测试_关门事件查询
        /// </summary>
        private void CycleTest_CloseDoor()
        {
            while (_closeDoor == 1 && _eventAllThreadFlag && null != _serialPort && _serialPort.IsOpen)
            {
                //等待判断超时的线程重置记录再进行下一次测试
                if (_currentMillis > 0)
                    continue;
                _operation = "循环测试_关门事件";
                _serialPort.Write(READ_EVENT_ALL_CLOSEDOOR_BYTE, 0, READ_EVENT_ALL_CLOSEDOOR_BYTE.Length);
                _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
                LogTxtChangedByDele("发送查询关门事件指令：" + READ_EVENT_ALL_CLOSEDOOR + "\r\n", Color.Black);
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// 循环测试_开门事件查询
        /// </summary>
        private void CycleTest_OpenDoor()
        {
            while (_openDoor == 1 && _eventAllThreadFlag && null != _serialPort && _serialPort.IsOpen)
            {
                //等待判断超时的线程重置记录再进行下一次测试
                if (_currentMillis > 0)
                    continue;
                _operation = "循环测试_开门事件";
                _serialPort.Write(READ_EVENT_ALL_OPENDOOR_BYTE, 0, READ_EVENT_ALL_OPENDOOR_BYTE.Length);
                _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
                LogTxtChangedByDele("发送查询开门事件指令：" + READ_EVENT_ALL_OPENDOOR + "\r\n", Color.Black);
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// 循环测试_关锁事件查询
        /// </summary>
        private void CycleTest_CloseLock()
        {
            while (_closeLock == 1 && _eventAllThreadFlag && null != _serialPort && _serialPort.IsOpen)
            {
                //等待判断超时的线程重置记录再进行下一次测试
                if (_currentMillis > 0)
                    continue;
                _operation = "循环测试_关锁事件";
                _serialPort.Write(READ_EVENT_ALL_CLOSELOCK_BYTE, 0, READ_EVENT_ALL_CLOSELOCK_BYTE.Length);
                _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
                LogTxtChangedByDele("发送查询关锁事件指令：" + READ_EVENT_ALL_CLOSELOCK + "\r\n", Color.Black);
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// 循环测试_开锁事件查询
        /// </summary>
        private void CycleTest_OpenLock()
        {
            while (_openLock == 1 && _eventAllThreadFlag && null != _serialPort && _serialPort.IsOpen)
            {
                //等待判断超时的线程重置记录再进行下一次测试
                if (_currentMillis > 0)
                    continue;
                _operation = "循环测试_开锁事件";
                _serialPort.Write(READ_EVENT_ALL_OPENLOCK_BYTE, 0, READ_EVENT_ALL_OPENLOCK_BYTE.Length);
                _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
                LogTxtChangedByDele("发送查询开锁事件指令：" + READ_EVENT_ALL_OPENLOCK + "\r\n", Color.Black);
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// 循环测试
        /// </summary>
        private void CycleTest()
        {
            //循环流程：先发一次对时，再发一次设置工号（如果有填写工号），然后循环查询事件总数，事件数大于零则一直循环查询对应的事件
            while (_eventAllThreadFlag && null != _serialPort && _serialPort.IsOpen)
            {
                //等待判断超时的线程重置记录再进行下一次测试
                if (_currentMillis > 0)
                    continue;
                switch (_cycleTestStep)
                {
                    //对时，FE FE FE FE 68 XX XX XX XX XX 00 68 07 06 秒 分 时 日 月 年 校验码 16
                    case 0:
                        int yy = DateTime.Now.Year % 100;
                        int MM = DateTime.Now.Month;
                        int DD = DateTime.Now.Day;
                        int HH = DateTime.Now.Hour;
                        int mm = DateTime.Now.Minute;
                        int ss = DateTime.Now.Second;
                        int yy2 = Convert.ToInt32(yy + "", 16) + 51;
                        int MM2 = Convert.ToInt32(MM + "", 16) + 51;
                        int DD2 = Convert.ToInt32(DD + "", 16) + 51;
                        int HH2 = Convert.ToInt32(HH + "", 16) + 51;
                        int mm2 = Convert.ToInt32(mm + "", 16) + 51;
                        int ss2 = Convert.ToInt32(ss + "", 16) + 51;
                        string hex_yy = yy2.ToString("X");
                        string hex_MM = MM2.ToString("X");
                        string hex_DD = DD2.ToString("X");
                        string hex_HH = HH2.ToString("X");
                        string hex_mm = mm2.ToString("X");
                        string hex_ss = ss2.ToString("X");
                        _operation = "对时";
                        if (!"".Equals(_hexName))
                        {
                            string timeCmd = "68" + _hexName + "00680706" + hex_ss + hex_mm + hex_HH + hex_DD + hex_MM + hex_yy;
                            string timeCrc = MyConvertUtil.CalcZM301CRC(timeCmd);
                            timeCmd = "FEFEFEFE" + timeCmd + timeCrc + "16";
                            byte[] timeCmdByte = MyConvertUtil.HexStrToBytes(timeCmd);
                            _serialPort.Write(timeCmdByte, 0, timeCmdByte.Length);
                            _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
                            LogTxtChangedByDele("发送对时指令：" + MyConvertUtil.StrAddChar(timeCmd, 2, " ") + "（20" + yy + "年" + MM + "月" + DD + "日" + HH + "时" + mm + "分" + ss + "秒）\r\n", Color.Black);
                        }
                        else
                        {
                            string timeCmd = "68222301563400680706" + hex_ss + hex_mm + hex_HH + hex_DD + hex_MM + hex_yy;
                            string timeCrc = MyConvertUtil.CalcZM301CRC(timeCmd);
                            timeCmd = "FEFEFEFE" + timeCmd + timeCrc + "16";
                            byte[] timeCmdByte = MyConvertUtil.HexStrToBytes(timeCmd);
                            _serialPort.Write(timeCmdByte, 0, timeCmdByte.Length);
                            _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
                            LogTxtChangedByDele("发送对时指令：" + MyConvertUtil.StrAddChar(timeCmd, 2, " ") + "（20" + yy + "年" + MM + "月" + DD + "日" + HH + "时" + mm + "分" + ss + "秒）\r\n", Color.Black);
                        }
                        Thread.Sleep(1 * 1000);
                        _cycleTestStep = 1;
                        break;
                    //设置工号，调用设置工号的按钮事件即可
                    case 1:
                        if (!"".Equals(_hexWorkId))
                        {
                            _operation = "设置工号";
                            btn_work_write.PerformClick();
                            _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
                            Thread.Sleep(1 * 1000);
                        }
                        _cycleTestStep = 2;
                        break;
                    //查询事件总数
                    case 2:
                        _operation = "查询事件总数";
                        _serialPort.Write(READ_EVENT_ALL_BYTE, 0, READ_EVENT_ALL_BYTE.Length);
                        _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
                        LogTxtChangedByDele("发送查询事件总数指令：" + READ_EVENT_ALL + "\r\n", Color.Black);
                        Thread.Sleep(1 * 1000);
                        _cycleTestStep = 3;
                        break;
                    //开锁事件
                    case 3:
                        if (_openLock == 1)
                        {
                            _operation = "循环测试_开锁事件";
                            _serialPort.Write(READ_EVENT_ALL_OPENLOCK_BYTE, 0, READ_EVENT_ALL_OPENLOCK_BYTE.Length);
                            _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
                            LogTxtChangedByDele("发送查询开锁事件指令：" + READ_EVENT_ALL_OPENLOCK + "\r\n", Color.Black);
                            Thread.Sleep(500);
                        }
                        _cycleTestStep = 4;
                        break;
                    //关锁事件
                    case 4:
                        if (_closeLock == 1)
                        {
                            _operation = "循环测试_关锁事件";
                            _serialPort.Write(READ_EVENT_ALL_CLOSELOCK_BYTE, 0, READ_EVENT_ALL_CLOSELOCK_BYTE.Length);
                            _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
                            LogTxtChangedByDele("发送查询关锁事件指令：" + READ_EVENT_ALL_CLOSELOCK + "\r\n", Color.Black);
                            Thread.Sleep(500);
                        }
                        _cycleTestStep = 5;
                        break;
                    //开门事件
                    case 5:
                        if (_openDoor == 1)
                        {
                            _operation = "循环测试_开门事件";
                            _serialPort.Write(READ_EVENT_ALL_OPENDOOR_BYTE, 0, READ_EVENT_ALL_OPENDOOR_BYTE.Length);
                            _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
                            LogTxtChangedByDele("发送查询开门事件指令：" + READ_EVENT_ALL_OPENDOOR + "\r\n", Color.Black);
                            Thread.Sleep(500);
                        }
                        _cycleTestStep = 6;
                        break;
                    //关门事件
                    case 6:
                        if (_closeDoor == 1)
                        {
                            _operation = "循环测试_关门事件";
                            _serialPort.Write(READ_EVENT_ALL_CLOSEDOOR_BYTE, 0, READ_EVENT_ALL_CLOSEDOOR_BYTE.Length);
                            _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
                            LogTxtChangedByDele("发送查询关门事件指令：" + READ_EVENT_ALL_CLOSEDOOR + "\r\n", Color.Black);
                            Thread.Sleep(500);
                        }
                        _cycleTestStep = 7;
                        break;
                    //窃电事件
                    case 7:
                        if (_steal == 1)
                        {
                            _operation = "循环测试_窃电事件";
                            _serialPort.Write(READ_EVENT_ALL_STEAL_BYTE, 0, READ_EVENT_ALL_STEAL_BYTE.Length);
                            _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
                            LogTxtChangedByDele("发送查询窃电事件指令：" + READ_EVENT_ALL_STEAL + "\r\n", Color.Black);
                            Thread.Sleep(500);
                        }
                        _cycleTestStep = 8;
                        break;
                    //振动事件
                    case 8:
                        if (_vibrate == 1)
                        {
                            _operation = "循环测试_振动事件";
                            _serialPort.Write(READ_EVENT_ALL_VIBRATE_BYTE, 0, READ_EVENT_ALL_VIBRATE_BYTE.Length);
                            _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
                            LogTxtChangedByDele("发送查询振动事件指令：" + READ_EVENT_ALL_VIBRATE + "\r\n", Color.Black);
                            Thread.Sleep(500);
                        }
                        _cycleTestStep = 2;
                        break;
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
        private void LogTxtChangedByDele(string str, Color color)
        {
            Print(str);
            //判断给哪个Tab下的日志文本框添加内容
            switch (_tabSelectedIndex)
            {
                case 0:
                    //非UI线程访问控件时
                    if (txt_log.InvokeRequired)
                    {
                        txt_log.Invoke(new LogTxtDele(LogTxtChangedByDele), str, color);
                    }
                    else
                    {
                        txt_log.SelectionColor = color;
                        txt_log.AppendText(str);
                        //设置光标的位置到文本尾
                        txt_log.Select(txt_log.TextLength, 0);
                        //滚动到控件光标处  
                        txt_log.ScrollToCaret();
                    }
                    break;
                case 1:
                    //非UI线程访问控件时
                    if (txt_log2.InvokeRequired)
                    {
                        txt_log2.Invoke(new LogTxtDele(LogTxtChangedByDele), str, color);
                    }
                    else
                    {
                        txt_log2.SelectionColor = color;
                        txt_log2.AppendText(str);
                        //设置光标的位置到文本尾
                        txt_log2.Select(txt_log2.TextLength, 0);
                        //滚动到控件光标处  
                        txt_log2.ScrollToCaret();
                    }
                    break;
            }
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
            READ_EVENT_ALL_OPENLOCK_BYTE = MyConvertUtil.HexStrToBytes(READ_EVENT_ALL_OPENLOCK);
            READ_EVENT_ALL_CLOSELOCK_BYTE = MyConvertUtil.HexStrToBytes(READ_EVENT_ALL_CLOSELOCK);
            READ_EVENT_ALL_OPENDOOR_BYTE = MyConvertUtil.HexStrToBytes(READ_EVENT_ALL_OPENDOOR);
            READ_EVENT_ALL_CLOSEDOOR_BYTE = MyConvertUtil.HexStrToBytes(READ_EVENT_ALL_CLOSEDOOR);
            READ_EVENT_ALL_STEAL_BYTE = MyConvertUtil.HexStrToBytes(READ_EVENT_ALL_STEAL);
            READ_EVENT_ALL_VIBRATE_BYTE = MyConvertUtil.HexStrToBytes(READ_EVENT_ALL_VIBRATE);
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
        private void EnableBtn(bool flag)
        {
            btn_lock1.Enabled = flag;
            btn_lock2.Enabled = flag;
            btn_lock3.Enabled = flag;
            btn_lock_all.Enabled = flag;
            btn_work_read.Enabled = flag;
            btn_box_read.Enabled = flag;
            btn_gps.Enabled = flag;
            btn_cycle_start.Enabled = flag;
            //btn_name_write.Enabled = flag;
            //btn_work_write.Enabled = flag;
            //btn_box_write.Enabled = flag;
            //打开串口后禁止更改参数
            if (flag)
            {
                cbx_port.Enabled = false;
                cbx_rate.Enabled = false;
                cbx_data.Enabled = false;
                cbx_parity.Enabled = false;
            }
            else
            {
                cbx_port.Enabled = true;
                cbx_rate.Enabled = true;
                cbx_data.Enabled = true;
                cbx_parity.Enabled = true;
            }
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="str"></param>
        private void Parse(string str)
        {
            try
            {
                string[] strArray = str.Split(' ');
                string type = strArray[8];
                //循环测试里的事件数据域
                string cycleEventHexIndex1 = "";
                string cycleEventHexIndex2 = "";
                string cycleEventIndex = cycleEventHexIndex1 + cycleEventHexIndex2;
                int eventIndex = 0;
                string eventHex_yy = "";
                string eventHex_MM = "";
                string eventHex_DD = "";
                string eventHex_HH = "";
                string eventHex_mm = "";
                string eventHex_ss = "";
                string eventHexTime = "";
                if ("81".Equals(type) || "82".Equals(type) || "83".Equals(type) || "84".Equals(type) || "85".Equals(type) || "89".Equals(type))
                {
                    cycleEventHexIndex1 = (Convert.ToInt32(strArray[11], 16) - 51).ToString("X");
                    cycleEventHexIndex2 = (Convert.ToInt32(strArray[10], 16) - 51).ToString("X");
                    cycleEventIndex = cycleEventHexIndex1 + cycleEventHexIndex2;
                    eventIndex = Convert.ToInt32(cycleEventIndex, 16);
                    eventHex_yy = (Convert.ToInt32(strArray[17], 16) - 51).ToString("X");
                    eventHex_MM = (Convert.ToInt32(strArray[16], 16) - 51).ToString("X");
                    eventHex_DD = (Convert.ToInt32(strArray[15], 16) - 51).ToString("X");
                    eventHex_HH = (Convert.ToInt32(strArray[14], 16) - 51).ToString("X");
                    eventHex_mm = (Convert.ToInt32(strArray[13], 16) - 51).ToString("X");
                    eventHex_ss = (Convert.ToInt32(strArray[12], 16) - 51).ToString("X");
                    eventHexTime = "20" + eventHex_yy + "年" + eventHex_MM + "月" + eventHex_DD + "日" + eventHex_HH + "时" + eventHex_mm + "分" + eventHex_ss + "秒";
                }
                int len = MyConvertUtil.HexStrToInt(strArray[9]);
                switch (type)
                {
                    //开锁事件
                    case "81":
                        LogTxtChangedByDele("开锁事件[" + eventIndex + "]触发，时间：" + eventHexTime + "\r\n", Color.Green);
                        break;
                    //关锁事件
                    case "82":
                        LogTxtChangedByDele("关锁事件[" + eventIndex + "]触发，时间：" + eventHexTime + "\r\n", Color.Green);
                        break;
                    //开门事件
                    case "83":
                        LogTxtChangedByDele("开门事件[" + eventIndex + "]触发，时间：" + eventHexTime + "\r\n", Color.Green);
                        break;
                    //关门事件
                    case "84":
                        LogTxtChangedByDele("关门事件[" + eventIndex + "]触发，时间：" + eventHexTime + "\r\n", Color.Green);
                        break;
                    //窃电事件
                    case "85":
                        LogTxtChangedByDele("窃电事件[" + eventIndex + "]触发，时间：" + eventHexTime + "\r\n", Color.Green);
                        break;
                    //振动事件
                    case "89":
                        LogTxtChangedByDele("振动事件[" + eventIndex + "]触发，时间：" + eventHexTime + "\r\n", Color.Green);
                        break;
                    //对时失败
                    case "D4":
                    case "C6":
                    case "C7":
                        LogTxtChangedByDele("对时失败\r\n", Color.Red);
                        break;
                    //对时成功
                    case "87":
                        //设置对时返回的是版本号
                        int ver1 = Convert.ToInt32(strArray[10], 16) - 51;
                        string hexVer1 = MyConvertUtil.HexStrToStr(ver1.ToString("X"));
                        int ver2 = Convert.ToInt32(strArray[11], 16) - 51;
                        string hexVer2 = MyConvertUtil.HexStrToStr(ver2.ToString("X"));
                        int ver3 = Convert.ToInt32(strArray[12], 16) - 51;
                        string hexVer3 = MyConvertUtil.HexStrToStr(ver3.ToString("X"));
                        int ver4 = Convert.ToInt32(strArray[13], 16) - 51;
                        string hexVer4 = MyConvertUtil.HexStrToStr(ver4.ToString("X"));
                        LogTxtChangedByDele("对时成功，版本号：" + hexVer1 + hexVer2 + "." + hexVer3 + "." + hexVer4 + "\r\n", Color.Green);
                        break;
                    //事件总数
                    case "80":
                        {
                            //0表示没有，1表示有
                            //开锁事件
                            _openLock = Convert.ToInt32(strArray[10], 16) - 51;
                            //开锁事件次数
                            int openLockCount = (Convert.ToInt32(strArray[12], 16) - 51) + (Convert.ToInt32(strArray[11], 16) - 51);
                            //关锁事件
                            _closeLock = Convert.ToInt32(strArray[13], 16) - 51;
                            //关锁事件次数
                            int closeLockCount = (Convert.ToInt32(strArray[15], 16) - 51) + (Convert.ToInt32(strArray[14], 16) - 51);
                            //开门事件
                            _openDoor = Convert.ToInt32(strArray[16], 16) - 51;
                            //开门事件次数
                            int openDoorCount = (Convert.ToInt32(strArray[18], 16) - 51) + (Convert.ToInt32(strArray[17], 16) - 51);
                            //关门事件
                            _closeDoor = Convert.ToInt32(strArray[19], 16) - 51;
                            //关门事件次数
                            int closeDoorCount = (Convert.ToInt32(strArray[21], 16) - 51) + (Convert.ToInt32(strArray[20], 16) - 51);
                            //窃电事件
                            _steal = Convert.ToInt32(strArray[22], 16) - 51;
                            //窃电事件次数
                            int stealCount = (Convert.ToInt32(strArray[24], 16) - 51) + (Convert.ToInt32(strArray[23], 16) - 51);
                            //振动事件
                            _vibrate = Convert.ToInt32(strArray[25], 16) - 51;
                            //振动事件次数
                            int vibrateCount = (Convert.ToInt32(strArray[27], 16) - 51) + (Convert.ToInt32(strArray[26], 16) - 51);
                            LogTxtChangedByDele("事件总数，开锁事件：" + _openLock + "，次数：" + openLockCount + (_openLock > 0 ? "（开始循环查询开锁事件）" : "") +
                                 "\r\n事件总数，关锁事件：" + _closeLock + "，次数：" + closeLockCount + (_closeLock > 0 ? "（开始循环查询关锁事件）" : "") +
                                 "\r\n事件总数，开门事件：" + _openDoor + "，次数：" + openDoorCount + (_openDoor > 0 ? "（开始循环查询开门事件）" : "") +
                                 "\r\n事件总数，关门事件：" + _closeDoor + "，次数：" + closeDoorCount + (_closeDoor > 0 ? "（开始循环查询关门事件）" : "") +
                                 "\r\n事件总数，窃电事件：" + _steal + "，次数：" + stealCount + (_steal > 0 ? "（开始循环查询窃电事件）" : "") +
                                 "\r\n事件总数，振动事件：" + _vibrate + "，次数：" + vibrateCount + (_vibrate > 0 ? "（开始循环查询振动事件）" : "") +
                                 "\r\n", Color.Green);
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
                                        LogTxtChangedByDele("开锁一成功，状态码：" + result + "\r\n", Color.Green);
                                    else
                                        LogTxtChangedByDele("开锁一失败，状态码：" + result + "\r\n", Color.Red);
                                    break;
                                case "开锁二":
                                    if ("00".Equals(result))
                                        LogTxtChangedByDele("开锁二成功，状态码：" + result + "\r\n", Color.Green);
                                    else
                                        LogTxtChangedByDele("开锁二失败，状态码：" + result + "\r\n", Color.Red);
                                    break;
                                case "开锁三":
                                    if ("00".Equals(result))
                                        LogTxtChangedByDele("开锁三成功，状态码：" + result + "\r\n", Color.Green);
                                    else
                                        LogTxtChangedByDele("开锁三失败，状态码：" + result + "\r\n", Color.Red);
                                    break;
                                case "开锁全部":
                                    if ("00".Equals(result))
                                        LogTxtChangedByDele("开锁全部成功，状态码：" + result + "\r\n", Color.Green);
                                    else
                                        LogTxtChangedByDele("开锁全部失败，状态码：" + result + "\r\n", Color.Red);
                                    break;
                            }
                        }
                        break;
                    //工号
                    case "8B":
                        {
                            switch (_operation)
                            {
                                case "读取工号":
                                    int workId1 = Convert.ToInt32(strArray[10], 16) - 51;
                                    int workId2 = Convert.ToInt32(strArray[11], 16) - 51;
                                    int workId3 = Convert.ToInt32(strArray[12], 16) - 51;
                                    int workId4 = Convert.ToInt32(strArray[13], 16) - 51;
                                    int workId5 = Convert.ToInt32(strArray[14], 16) - 51;
                                    int workId6 = Convert.ToInt32(strArray[15], 16) - 51;
                                    int workId7 = Convert.ToInt32(strArray[16], 16) - 51;
                                    int workId8 = Convert.ToInt32(strArray[17], 16) - 51;
                                    string hexWorkId1 = workId1.ToString("X");
                                    string hexWorkId2 = workId2.ToString("X");
                                    string hexWorkId3 = workId3.ToString("X");
                                    string hexWorkId4 = workId4.ToString("X");
                                    string hexWorkId5 = workId5.ToString("X");
                                    string hexWorkId6 = workId6.ToString("X");
                                    string hexWorkId7 = workId7.ToString("X");
                                    string hexWorkId8 = workId8.ToString("X");
                                    string workId = MyConvertUtil.HexStrToStr(hexWorkId1 + hexWorkId2 + hexWorkId3 + hexWorkId4 + hexWorkId5 + hexWorkId6 + hexWorkId7 + hexWorkId8);
                                    WorkIdTxtChangedByDele(workId);
                                    LogTxtChangedByDele("读取工号成功：" + workId + "\r\n", Color.Green);
                                    break;
                                case "设置工号":
                                    if ("00".Equals(strArray[9]))
                                        LogTxtChangedByDele("设置工号成功，状态码：" + strArray[9] + "\r\n", Color.Green);
                                    else
                                        LogTxtChangedByDele("设置工号失败，状态码：" + strArray[9] + "\r\n", Color.Red);
                                    break;
                            }
                        }
                        break;
                    //表箱号
                    case "86":
                        //int boxId1 = Convert.ToInt32(strArray[], 16) - 51;
                        //int boxId2 = Convert.ToInt32(strArray[], 16) - 51;
                        //int boxId3 = Convert.ToInt32(strArray[], 16) - 51;
                        //int boxId4 = Convert.ToInt32(strArray[], 16) - 51;
                        //int boxId5 = Convert.ToInt32(strArray[], 16) - 51;
                        //int boxId6 = Convert.ToInt32(strArray[], 16) - 51;
                        //string hexBoxId1 = boxId1.ToString("X");
                        //string hexBoxId2 = boxId2.ToString("X");
                        //string hexBoxId3 = boxId3.ToString("X");
                        //string hexBoxId4 = boxId4.ToString("X");
                        //string hexBoxId5 = boxId5.ToString("X");
                        //string hexBoxId6 = boxId6.ToString("X");
                        //string boxId = hexBoxId1 + hexBoxId2 + hexBoxId3 + hexBoxId4 + hexBoxId5 + hexBoxId6;
                        string boxId = "";
                        switch (_operation)
                        {
                            case "读取表箱号":
                                BoxIdTxtChangedByDele(boxId);
                                LogTxtChangedByDele("读取表箱号成功：" + boxId + "\r\n", Color.Green);
                                break;
                            case "设置表箱号":
                                LogTxtChangedByDele("设置表箱号成功：" + boxId + "\r\n", Color.Green);
                                break;
                        }
                        break;
                    //读取GPS位置
                    case "8A":
                        {
                            //状态信息：00未定位，1已定位
                            int state = Convert.ToInt32(strArray[10], 16) - 51;
                            LogTxtChangedByDele("GPS位置，状态信息：" + (state == 1 ? "已定位" : "未定位") + "\r\n", Color.Green);
                            //时间
                            string hex_yy = (Convert.ToInt32(strArray[16], 16) - 51).ToString("X");
                            string hex_MM = (Convert.ToInt32(strArray[15], 16) - 51).ToString("X");
                            string hex_DD = (Convert.ToInt32(strArray[14], 16) - 51).ToString("X");
                            string hex_HH = (Convert.ToInt32(strArray[13], 16) - 51).ToString("X");
                            string hex_mm = (Convert.ToInt32(strArray[12], 16) - 51).ToString("X");
                            string hex_ss = (Convert.ToInt32(strArray[11], 16) - 51).ToString("X");
                            string hexTime = "20" + hex_yy + "年" + hex_MM + "月" + hex_DD + "日" + hex_HH + "时" + hex_mm + "分" + hex_ss + "秒";
                            LogTxtChangedByDele("GPS位置，时间：" + hexTime + "\r\n", Color.Green);
                            //经度，因为需要转换成10进制，所以要判断负数情况
                            string hexLat1 = (Convert.ToInt32(strArray[20], 16) - 51).ToString("X").Replace("FF", "");
                            string hexLat2 = (Convert.ToInt32(strArray[19], 16) - 51).ToString("X").Replace("FF", "");
                            string hexLat3 = (Convert.ToInt32(strArray[18], 16) - 51).ToString("X").Replace("FF", "");
                            string hexLat4 = (Convert.ToInt32(strArray[17], 16) - 51).ToString("X").Replace("FF", "");
                            double latNum = Convert.ToDouble(Convert.ToInt32(hexLat1 + hexLat2 + hexLat3 + hexLat4, 16)) / 1000000;
                            LogTxtChangedByDele("GPS位置，经度：" + latNum + "\r\n", Color.Green);
                            //纬度，因为需要转换成10进制，所以要判断负数情况
                            string hexLot1 = (Convert.ToInt32(strArray[24], 16) - 51).ToString("X").Replace("FF", "");
                            string hexLot2 = (Convert.ToInt32(strArray[23], 16) - 51).ToString("X").Replace("FF", "");
                            string hexLot3 = (Convert.ToInt32(strArray[22], 16) - 51).ToString("X").Replace("FF", "");
                            string hexLot4 = (Convert.ToInt32(strArray[21], 16) - 51).ToString("X").Replace("FF", "");
                            double lotNum = Convert.ToDouble(Convert.ToInt32(hexLot1 + hexLot2 + hexLot3 + hexLot4, 16)) / 1000000;
                            LogTxtChangedByDele("GPS位置，纬度：" + lotNum + "\r\n", Color.Green);
                            //海拔
                            string height1 = (Convert.ToInt32(strArray[26], 16) - 51).ToString("X");
                            string height2 = (Convert.ToInt32(strArray[25], 16) - 51).ToString("X");
                            string heightStr = height1 + height2;
                            int height = Convert.ToInt32(heightStr, 16);
                            LogTxtChangedByDele("GPS位置，海拔：" + height + "米" + "\r\n", Color.Green);
                            //速度
                            string speed1 = (Convert.ToInt32(strArray[28], 16) - 51).ToString("X");
                            string speed2 = (Convert.ToInt32(strArray[27], 16) - 51).ToString("X");
                            string speedStr = speed1 + speed2;
                            int speed = Convert.ToInt32(speedStr, 16);
                            LogTxtChangedByDele("GPS位置，速度：" + speed + "km/h\r\n", Color.Green);
                            //方向
                            string dir1 = (Convert.ToInt32(strArray[30], 16) - 51).ToString("X");
                            string dir2 = (Convert.ToInt32(strArray[29], 16) - 51).ToString("X");
                            string dirStr = dir1 + dir2;
                            int dir = Convert.ToInt32(dirStr, 16);
                            LogTxtChangedByDele("GPS位置，方向：" + dir + "\r\n", Color.Green);
                            //温度
                            string temperature = (Convert.ToInt32(strArray[31], 16) - 51).ToString("X");
                            LogTxtChangedByDele("GPS位置，温度：" + temperature + "℃\r\n", Color.Green);
                            string parseStr = hexTime + "，经度：" + latNum + "，纬度：" + lotNum + "，海拔：" + height + "米，速度" + speed + "km/h，方向：" + dir + "，温度：" + temperature + "℃";
                            GpsTxtChangedByDele(parseStr);

                        }
                        break;
                }
                //重置记录的状态
                _currentMillis = 0;
                _operation = "";
            }
            catch (Exception e)
            {
                LogTxtChangedByDele(e.ToString() + "\r\n", Color.Red);
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
            try
            {
                //待读字节长度
                int byteLen = _serialPort.BytesToRead;
                byte[] byteArray = new byte[byteLen];
                _serialPort.Read(byteArray, 0, byteArray.Length);
                string str = MyConvertUtil.BytesToStr(byteArray);
                Print("本次读取字节：" + str + "，长度：" + byteLen);
                //过滤掉开头的干扰数据（缓存为空则说明收到的是第一个字节，收到的完整数据必须是以68开头）
                if ("".Equals(_receivedStr) && !str.StartsWith("68"))
                {
                    Print("继续读取，收到的完整数据必须是以68开头（Hex）：" + _receivedStr);
                    return;
                }
                _receivedStr += str;
                Print("累计收到字节（Hex）：" + _receivedStr);
                //过滤掉结尾的干扰数据（收到的完整数据必须是以16结尾）
                int endStrIndex = _receivedStr.LastIndexOf("16");
                //目前最短的数据是12个字节
                if (endStrIndex < 11)
                {
                    Print("继续读取，收到的完整数据必须是以16结尾（Hex）：" + _receivedStr);
                    return;
                }
                //开头+结尾+校验码=完整数据
                if (_receivedStr.StartsWith("68") && _receivedStr.EndsWith("16"))
                {
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
                        //显示收到的数据
                        LogTxtChangedByDele("收到（Hex）：" + _receivedStr + "，数据长度：" + len + "\r\n", Color.Black);
                        //解析数据
                        Parse(_receivedStr);
                    }
                }
                //清空缓存
                _receivedStr = "";
            }
            catch (Exception ex)
            {
                //清空缓存
                _receivedStr = "";
                LogTxtChangedByDele(ex.ToString() + "\r\n", Color.Red);
            }
        }

        private void txt_name_TextChanged(object sender, EventArgs e)
        {
            string input = txt_name.Text.ToString();
            bool flag = Regex.IsMatch(input, @"[A-Z a-z 0-9]{5}");
            if (flag)
                btn_name_write.Enabled = true;
            else
                btn_name_write.Enabled = false;
        }

        private void txt_work_id_TextChanged(object sender, EventArgs e)
        {
            string input = txt_work_id.Text.ToString();
            bool flag = Regex.IsMatch(input, @"[A-Z a-z 0-9]{8}");
            if (flag)
                btn_work_write.Enabled = true;
            else
                btn_work_write.Enabled = false;
        }

        private void txt_box_id_TextChanged(object sender, EventArgs e)
        {
            string input = txt_box_id.Text.ToString();
            bool flag = Regex.IsMatch(input, @"[A-Z a-z 0-9]{6}");
            if (flag)
                btn_box_write.Enabled = true;
            else
                btn_box_write.Enabled = false;
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            _tabSelectedIndex = tabControl.SelectedIndex;
            //切换至单项测试时停止循环测试
            _eventAllThreadFlag = false;
            _eventAllThread = null;
        }

        /// <summary>
        /// 预设蓝牙名称
        /// 蓝牙名称不属于数据域，不需要+33
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_name_write_Click(object sender, EventArgs e)
        {
            string input = txt_name.Text.ToString();
            _hexName = "";
            //蓝牙名称
            for (int i = 0; i < input.Length; i++)
            {
                string temp = MyConvertUtil.CharAt(input, i);
                temp = MyConvertUtil.StrToHexStr(temp);
                if (temp.Length < 2)
                    temp = "0" + temp;
                Print("设置蓝牙名称，遂字转换（Hex）：" + temp);
                _hexName += temp;
            }

            //重新生成查询开锁事件指令并计算校验码
            //READ_EVENT_ALL_OPENLOCK = "FE FE FE FE 68 22 23 01 56 34 00 68 01 00 A1 16";
            string readEventOpenLockCmd = "68" + _hexName + "00680100";
            string readEventOpenLockCrc = MyConvertUtil.CalcZM301CRC(readEventOpenLockCmd);
            READ_EVENT_ALL_OPENLOCK = "FEFEFEFE" + readEventOpenLockCmd + readEventOpenLockCrc + "16";
            //添加空格
            READ_EVENT_ALL_OPENLOCK = MyConvertUtil.StrAddChar(READ_EVENT_ALL_OPENLOCK, 2, " ");
            READ_EVENT_ALL_OPENLOCK_BYTE = MyConvertUtil.HexStrToBytes(READ_EVENT_ALL_OPENLOCK);

            //重新生成查询关锁事件指令并计算校验码
            //READ_EVENT_ALL_CLOSELOCK = "FE FE FE FE 68 22 23 01 56 34 00 68 02 00 A2 16";
            string readEventCloseLockCmd = "68" + _hexName + "00680200";
            string readEventCloseLockCrc = MyConvertUtil.CalcZM301CRC(readEventCloseLockCmd);
            READ_EVENT_ALL_CLOSELOCK = "FEFEFEFE" + readEventCloseLockCmd + readEventCloseLockCrc + "16";
            //添加空格
            READ_EVENT_ALL_CLOSELOCK = MyConvertUtil.StrAddChar(READ_EVENT_ALL_CLOSELOCK, 2, " ");
            READ_EVENT_ALL_CLOSELOCK_BYTE = MyConvertUtil.HexStrToBytes(READ_EVENT_ALL_CLOSELOCK);

            //重新生成查询开门事件指令并计算校验码
            //READ_EVENT_ALL_OPENDOOR = "FE FE FE FE 68 22 23 01 56 34 00 68 03 00 A3 16";
            string readEventOpenDoorCmd = "68" + _hexName + "00680300";
            string readEventOpenDoorCrc = MyConvertUtil.CalcZM301CRC(readEventOpenDoorCmd);
            READ_EVENT_ALL_OPENDOOR = "FEFEFEFE" + readEventOpenDoorCmd + readEventOpenDoorCrc + "16";
            //添加空格
            READ_EVENT_ALL_OPENDOOR = MyConvertUtil.StrAddChar(READ_EVENT_ALL_OPENDOOR, 2, " ");
            READ_EVENT_ALL_OPENDOOR_BYTE = MyConvertUtil.HexStrToBytes(READ_EVENT_ALL_OPENDOOR);

            //重新生成查询关门事件指令并计算校验码
            //READ_EVENT_ALL_CLOSEDOOR = "FE FE FE FE 68 22 23 01 56 34 00 68 04 00 A4 16";
            string readEventCloseDoorCmd = "68" + _hexName + "00680400";
            string readEventCloseDoorCrc = MyConvertUtil.CalcZM301CRC(readEventCloseDoorCmd);
            READ_EVENT_ALL_CLOSEDOOR = "FEFEFEFE" + readEventCloseDoorCmd + readEventCloseDoorCrc + "16";
            //添加空格
            READ_EVENT_ALL_CLOSEDOOR = MyConvertUtil.StrAddChar(READ_EVENT_ALL_CLOSEDOOR, 2, " ");
            READ_EVENT_ALL_CLOSEDOOR_BYTE = MyConvertUtil.HexStrToBytes(READ_EVENT_ALL_CLOSEDOOR);

            //重新生成查询窃电事件指令并计算校验码
            //READ_EVENT_ALL_STEAL = "FE FE FE FE 68 22 23 01 56 34 00 68 05 00 A5 16";
            string readEventStealCmd = "68" + _hexName + "00680500";
            string readEventStealCrc = MyConvertUtil.CalcZM301CRC(readEventStealCmd);
            READ_EVENT_ALL_STEAL = "FEFEFEFE" + readEventStealCmd + readEventStealCrc + "16";
            //添加空格
            READ_EVENT_ALL_STEAL = MyConvertUtil.StrAddChar(READ_EVENT_ALL_STEAL, 2, " ");
            READ_EVENT_ALL_STEAL_BYTE = MyConvertUtil.HexStrToBytes(READ_EVENT_ALL_STEAL);

            //重新生成查询振动事件指令并计算校验码
            //READ_EVENT_ALL_VIBRATE = "FE FE FE FE 68 22 23 01 56 34 00 68 09 00 A9 16";
            string readEventVibrateCmd = "68" + _hexName + "00680900";
            string readEventVibrateCrc = MyConvertUtil.CalcZM301CRC(readEventVibrateCmd);
            READ_EVENT_ALL_VIBRATE = "FEFEFEFE" + readEventVibrateCmd + readEventVibrateCrc + "16";
            //添加空格
            READ_EVENT_ALL_VIBRATE = MyConvertUtil.StrAddChar(READ_EVENT_ALL_VIBRATE, 2, " ");
            READ_EVENT_ALL_VIBRATE_BYTE = MyConvertUtil.HexStrToBytes(READ_EVENT_ALL_VIBRATE);

            //重新生成查询事件总数指令并计算校验码
            //READ_EVENT_ALL = "FE FE FE FE 68 22 23 01 56 34 00 68 00 00 A0 16";
            string readEventAllCmd = "68" + _hexName + "00680000";
            string readEventAllCrc = MyConvertUtil.CalcZM301CRC(readEventAllCmd);
            READ_EVENT_ALL = "FEFEFEFE" + readEventAllCmd + readEventAllCrc + "16";
            //添加空格
            READ_EVENT_ALL = MyConvertUtil.StrAddChar(READ_EVENT_ALL, 2, " ");
            READ_EVENT_ALL_BYTE = MyConvertUtil.HexStrToBytes(READ_EVENT_ALL);

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

            LogTxtChangedByDele("蓝牙名称即将设置为" + input + "，发送任意指令即生效\r\n", Color.Blue);
        }

        /// <summary>
        /// 开始/停止循环测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_cycle_start_Click(object sender, EventArgs e)
        {
            //清空缓存
            _receivedStr = "";
            if ("开始".Equals(btn_cycle_start.Text))
            {
                if (null == _eventAllThread)
                {
                    _eventAllThreadFlag = true;
                    _eventAllThread = new Thread(CycleTest);
                    _eventAllThread.Start();
                }
                else
                {
                    _eventAllThreadFlag = false;
                    _eventAllThread = null;
                    _eventAllThread = new Thread(CycleTest);
                    _eventAllThread.Start();
                }
                btn_cycle_start.Text = "停止";
            }
            else
            {
                _openLock = 0;
                _closeLock = 0;
                _openDoor = 0;
                _closeDoor = 0;
                _steal = 0;
                _vibrate = 0;
                _eventAllThreadFlag = false;
                _eventAllThread = null;
                btn_cycle_start.Text = "开始";
            }
            //重置记录的状态
            _currentMillis = 0;
            _operation = "";
            _cycleTestStep = 0;
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
                    //打开串口
                    if (null == _serialPort)
                    {
                        _serialPort = new SerialPort(_portName, _rate, _parity, _data, StopBits.One);
                        _serialPort.DataReceived += new SerialDataReceivedEventHandler(ReceivedComData);
                    }
                    _serialPort.Open();
                    LogTxtChangedByDele("已打开：" + _portName + "\r\n", Color.Black);
                    EnableBtn(true);
                    btn_open.Text = "关闭串口";
                    //开启读取超时的线程，里面对串口有作判断，必须放在串口操作之后
                    if (null == _timeOutThread)
                    {
                        _timeOutThreadFlag = true;
                        _timeOutThread = new Thread(TimeOutForThread);
                        _timeOutThread.Start();
                    }
                    else
                    {
                        _timeOutThreadFlag = false;
                        _timeOutThread = null;
                        _timeOutThreadFlag = true;
                        _timeOutThread = new Thread(TimeOutForThread);
                        _timeOutThread.Start();
                    }
                }
                else
                {
                    //关闭读取超时的线程
                    _timeOutThreadFlag = false;
                    _timeOutThread = null;
                    //关闭循环测试的线程
                    _eventAllThreadFlag = false;
                    _eventAllThread = null;
                    //关闭串口
                    _serialPort.Close();
                    _serialPort = null;
                    LogTxtChangedByDele("已关闭：" + _portName + "\r\n", Color.Black);
                    EnableBtn(false);
                    btn_open.Text = "打开串口";
                }
                //重置记录的状态
                _currentMillis = 0;
                _operation = "";
                _cycleTestStep = 0;
            }
            catch (Exception ex)
            {
                LogTxtChangedByDele("串口" + _portName + "打开/关闭失败，原因：" + ex.ToString() + "\r\n", Color.Red);
            }
        }

        /// <summary>
        /// 开锁一
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_lock1_Click(object sender, EventArgs e)
        {
            //清空缓存
            _receivedStr = "";
            _operation = "开锁一";
            _serialPort.Write(OPEN_DOOR1_BYTE, 0, OPEN_DOOR1_BYTE.Length);
            _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
            LogTxtChangedByDele("发送开锁一指令：" + OPEN_DOOR1 + "\r\n", Color.Black);
        }

        /// <summary>
        /// 开锁二
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_lock2_Click(object sender, EventArgs e)
        {
            //清空缓存
            _receivedStr = "";
            _operation = "开锁二";
            _serialPort.Write(OPEN_DOOR2_BYTE, 0, OPEN_DOOR2_BYTE.Length);
            _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
            LogTxtChangedByDele("发送开锁二指令：" + OPEN_DOOR2 + "\r\n", Color.Black);
        }

        /// <summary>
        /// 开锁三
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_lock3_Click(object sender, EventArgs e)
        {
            //清空缓存
            _receivedStr = "";
            _operation = "开锁三";
            _serialPort.Write(OPEN_DOOR3_BYTE, 0, OPEN_DOOR3_BYTE.Length);
            _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
            LogTxtChangedByDele("发送开锁三指令：" + OPEN_DOOR3 + "\r\n", Color.Black);
        }

        /// <summary>
        /// 开锁全部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_lock_all_Click(object sender, EventArgs e)
        {
            //清空缓存
            _receivedStr = "";
            _operation = "开锁全部";
            _serialPort.Write(OPEN_DOOR_ALL_BYTE, 0, OPEN_DOOR_ALL_BYTE.Length);
            _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
            LogTxtChangedByDele("发送开全部锁指令：" + OPEN_DOOR_ALL + "\r\n", Color.Black);
        }

        /// <summary>
        /// 读取工号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_work_read_Click(object sender, EventArgs e)
        {
            //清空缓存
            _receivedStr = "";
            _operation = "读取工号";
            _serialPort.Write(READ_WORK_BYTE, 0, READ_WORK_BYTE.Length);
            _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
            LogTxtChangedByDele("发送读取工号指令：" + READ_WORK + "\r\n", Color.Black);
        }

        /// <summary>
        /// 设置工号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_work_write_Click(object sender, EventArgs e)
        {
            //清空缓存
            _receivedStr = "";
            string input = txt_work_id.Text;
            string cmdStr = "68222301563400680B08";
            //有设置蓝牙名称
            if (!"".Equals(_hexName))
                cmdStr = "68" + _hexName + "00680B08";
            _hexWorkId = "";
            //工号+33
            for (int i = 0; i < input.Length; i++)
            {
                string temp = MyConvertUtil.CharAt(input, i);
                temp = MyConvertUtil.StrToHexStr(temp);
                int tempValue = Convert.ToInt32(temp, 16) + 51;
                temp = tempValue.ToString("X");
                if (temp.Length < 2)
                    temp = "0" + temp;
                Print("设置工号，遂字转换（Hex）：" + temp);
                _hexWorkId += temp;
            }
            string[] strArray = MyConvertUtil.StrSplitInterval(_hexWorkId, 2);
            cmdStr += strArray[0] + strArray[1] + strArray[2] + strArray[3] + strArray[4] + strArray[5] + strArray[6] + strArray[7];
            string crcStr = MyConvertUtil.CalcZM301CRC(cmdStr);
            Print("计算出的校验码（Hex）：" + crcStr);
            cmdStr += crcStr + "16";
            cmdStr = "FEFEFEFE" + cmdStr;
            byte[] comByte = MyConvertUtil.HexStrToBytes(cmdStr);
            _operation = "设置工号";
            _serialPort.Write(comByte, 0, comByte.Length);
            _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
            LogTxtChangedByDele("发送设置工号指令：" + MyConvertUtil.StrAddChar(cmdStr, 2, " ") + "\r\n", Color.Black);
        }

        /// <summary>
        /// 读取表箱号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_box_read_Click(object sender, EventArgs e)
        {
            //清空缓存
            _receivedStr = "";
            _operation = "读取表箱号";
            _serialPort.Write(READ_BOX_BYTE, 0, READ_BOX_BYTE.Length);
            _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
            LogTxtChangedByDele("发送读取表箱号指令：" + READ_BOX + "\r\n", Color.Black);
        }

        /// <summary>
        /// 设置表箱号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_box_write_Click(object sender, EventArgs e)
        {
            //清空缓存
            _receivedStr = "";
            string input = txt_box_id.Text;
            string cmdStr = "68222301563400680606";
            //有设置蓝牙名称
            if (!"".Equals(_hexName))
                cmdStr = "68" + _hexName + "00680606";
            string str = "";
            //表箱号+33
            for (int i = 0; i < input.Length; i++)
            {
                string temp = MyConvertUtil.CharAt(input, i);
                temp = MyConvertUtil.StrToHexStr(temp);
                int tempValue = Convert.ToInt32(temp, 16) + 51;
                temp = tempValue.ToString("X");
                if (temp.Length < 2)
                    temp = "0" + temp;
                Print("设置表箱号，遂字转换（Hex）：" + temp);
                str += temp;
            }
            string[] strArray = MyConvertUtil.StrSplitInterval(str, 2);
            cmdStr += strArray[0] + strArray[1] + strArray[2] + strArray[3] + strArray[4] + strArray[5];
            string crcStr = MyConvertUtil.CalcZM301CRC(cmdStr);
            Print("计算出的校验码（Hex）：" + crcStr);
            cmdStr += crcStr + "16";
            cmdStr = "FEFEFEFE" + cmdStr;
            byte[] comByte = MyConvertUtil.HexStrToBytes(cmdStr);
            _operation = "设置表箱号";
            _serialPort.Write(comByte, 0, comByte.Length);
            _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
            LogTxtChangedByDele("发送设置表箱号指令：" + MyConvertUtil.StrAddChar(cmdStr, 2, " ") + "\r\n", Color.Black);
        }

        /// <summary>
        /// 查询GPS位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_gps_Click(object sender, EventArgs e)
        {
            //清空缓存
            _receivedStr = "";
            _operation = "查询GPS位置";
            _serialPort.Write(READ_GPS_BYTE, 0, READ_GPS_BYTE.Length);
            _currentMillis = (DateTime.Now.Ticks - DATETIME.Ticks) / 10000;
            LogTxtChangedByDele("发送查询GPS指令：" + READ_GPS + "\r\n", Color.Black);
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

        private void cbx_port_SelectedIndexChanged(object sender, EventArgs e)
        {
            _portName = _portNameArray[cbx_port.SelectedIndex];
            LogTxtChangedByDele("串口名称：" + _portName + "\r\n", Color.Black);
        }

        private void cbx_rate_SelectedIndexChanged(object sender, EventArgs e)
        {
            LogTxtChangedByDele("波特率：" + _rate + "\r\n", Color.Black);
        }

        private void cbx_data_SelectedIndexChanged(object sender, EventArgs e)
        {
            _data = DATA_ARRAY[cbx_data.SelectedIndex];
            LogTxtChangedByDele("数据位：" + _data + "\r\n", Color.Black);
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
            LogTxtChangedByDele("校验位：" + _parity + "\r\n", Color.Black);
        }
    }

}
