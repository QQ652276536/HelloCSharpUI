using HelloCSharp.Log;
using HelloCSharp.Util;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace HelloCSharp.UI
{
    public partial class ImageTest : Form
    {
        /// <summary>
        /// 日志文本框委托
        /// </summary>
        /// <param name="str"></param>
        /// <param name="color"></param>
        private delegate void LogTxtDele(string str, Color color);

        /// <summary>
        /// 查询所有事件，校验码已提前算好
        /// </summary>
        private string CMD_EVENTALL = "68 AA AA AA AA AA AA 68 00 00 00 CC 16";
        private byte[] CMD_EVENTALL_BYTE;

        /// <summary>
        /// 查询事件一，校验码已提前算好
        /// </summary>
        private string CMD_EVENT1 = "68 AA AA AA AA AA AA 68 01 00 00 CD 16";
        private byte[] CMD_EVENT1_BYTE;

        /// <summary>
        /// 查询事件二，校验码已提前算好
        /// </summary>
        private string CMD_EVENT2 = "68 AA AA AA AA AA AA 68 01 00 00 CE 16";
        private byte[] CMD_EVENT2_BYTE;

        /// <summary>
        /// 查询事件三，校验码已提前算好
        /// </summary>
        private string CMD_EVENT3 = "68 AA AA AA AA AA AA 68 01 00 00 CF 16";
        private byte[] CMD_EVENT3_BYTE;

        /// <summary>
        /// 查询事件四，校验码已提前算好
        /// </summary>
        private string CMD_EVENT4 = "68 AA AA AA AA AA AA 68 01 00 00 D0 16";
        private byte[] CMD_EVENT4_BYTE;

        /// <summary>
        /// 查询事件五，校验码已提前算好
        /// </summary>
        private string CMD_EVENT5 = "68 AA AA AA AA AA AA 68 01 00 00 D1 16";
        private byte[] CMD_EVENT5_BYTE;

        /// <summary>
        /// 请求包数据的固定部分
        /// </summary>
        private string CMD_PKG_FIXED = "68 AA AA AA AA AA AA 68 04 0C 00 33 33 33 33 33 33 33 33 33 33";

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
        private readonly int TIME_OUT = 3 * 1000;

        private MyLogger _logger = MyLogger.Instance;
        //用于获取当前毫秒，Ticks为纳秒，转换为毫秒需要除以10000，转换为秒需要除以10000000
        private DateTime DATETIME = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        //用于获取当前时间
        private DateTime _nowTime = DateTime.Now;
        //串口
        private SerialPort _serialPort;
        //查询事件总数的线程
        private Thread _eventAllThread;
        //查询事件总数的线程的运行标识
        private bool _eventAllThreadFlag = true;
        //本机串口
        private string[] _portNameArray;
        private string _receivedStr = "";
        //串口名
        private string _portName = "";
        //波特率、数据位
        private int _rate = 1200, _data = 8;
        //校验位
        private Parity _parity = Parity.None;
        //循环测试时当前执行到的步骤：0所有事件、1事件一、2事件二、3事件三、4事件四、5事件五
        private int _cycleTestStep = 0;
        private bool[] _eventFlag = { true, false, false, false, false };
        //校验位
        private string _crc = "";
        //总包数、当前包数、总长度、每包大小
        private int _totalPkg = 0, _currPkg = 1, _totalLen = 0, _everyPkgSize = 0;
        private bool _isFileHead = true;
        //图片数据
        private string _pkgData = "";

        public ImageTest()
        {
            InitializeComponent();
            InitData();
            EnableBtn(false);
        }

        /// <summary>
        /// 循环测试
        /// </summary>
        private void CycleTest()
        {
            //循环流程：先发一次对时，再发一次设置工号（如果有填写工号），然后循环查询事件总数，事件数大于零则一直循环查询对应的事件
            while (_eventAllThreadFlag && null != _serialPort && _serialPort.IsOpen)
            {
                //_logger.WriteLog("等待发送查询指令...");
                switch (_cycleTestStep)
                {
                    //查询所有事件
                    case 0:
                        if (_eventFlag[0])
                        {
                            _cycleTestStep = -1;
                            _serialPort.Write(CMD_EVENTALL_BYTE, 0, CMD_EVENTALL_BYTE.Length);
                            LogTxtChangedByDele("发送查询所有事件指令：" + CMD_EVENTALL + "\n", Color.Black);
                            Thread.Sleep(500);
                        }
                        break;
                    //查询事件一
                    case 1:
                        if (_eventFlag[1])
                        {
                            _cycleTestStep = -1;
                            _serialPort.Write(CMD_EVENT1_BYTE, 0, CMD_EVENT1_BYTE.Length);
                            LogTxtChangedByDele("发送查询事件一指令：" + CMD_EVENT1 + "\n", Color.Black);
                            Thread.Sleep(500);
                        }
                        break;
                    //查询事件二
                    case 2:
                        if (_eventFlag[2])
                        {
                            _cycleTestStep = -1;
                            _serialPort.Write(CMD_EVENT2_BYTE, 0, CMD_EVENT2_BYTE.Length);
                            LogTxtChangedByDele("发送查询事件二指令：" + CMD_EVENT2 + "\n", Color.Black);
                            Thread.Sleep(500);
                        }
                        break;
                    //查询事件三
                    case 3:
                        if (_eventFlag[3])
                        {
                            _cycleTestStep = -1;
                            _serialPort.Write(CMD_EVENT3_BYTE, 0, CMD_EVENT3_BYTE.Length);
                            LogTxtChangedByDele("发送查询事件三指令：" + CMD_EVENT3 + "\n", Color.Black);
                            Thread.Sleep(500);
                        }
                        break;
                    //查询事件四
                    case 4:
                        if (_eventFlag[4])
                        {
                            _cycleTestStep = -1;
                            //请求文件头
                            if (_isFileHead)
                            {
                                _serialPort.Write(CMD_EVENT4_BYTE, 0, CMD_EVENT4_BYTE.Length);
                                LogTxtChangedByDele("发送查询事件四指令：" + CMD_EVENT4 + "\n", Color.Black);
                                Thread.Sleep(500);
                            }
                            //请求第n包数据
                            else
                            {
                                string currPkg = MyConvertUtil.IntToHexStr(_currPkg + "");
                                currPkg = MyConvertUtil.AddZero(currPkg, 4, true);
                                //当前包数的高位放到后面并添加空格
                                string[] tempCurrPkg = MyConvertUtil.StrSplitInterval(currPkg, 2);
                                currPkg = MyConvertUtil.StrAddChar(tempCurrPkg[1] + currPkg[0], 2, " ");
                                //计算校验码
                                string crc = MyConvertUtil.CRC_Zistone_BLE(CMD_PKG_FIXED + " " + currPkg);
                                //最终请求指令
                                string cmd = CMD_PKG_FIXED + " " + currPkg + " " + crc + " 16";
                                byte[] cmdBytes = MyConvertUtil.HexStrToBytes(cmd);
                                _serialPort.Write(cmdBytes, 0, cmdBytes.Length);
                                LogTxtChangedByDele("发送查询第" + _currPkg + "包指令：" + cmd + "\n", Color.Black);
                                Thread.Sleep(500);
                            }
                        }
                        break;
                    //查询事件五
                    case 5:
                        if (_eventFlag[5])
                        {
                            _cycleTestStep = -1;
                            _serialPort.Write(CMD_EVENT5_BYTE, 0, CMD_EVENT5_BYTE.Length);
                            LogTxtChangedByDele("发送查询事件五指令：" + CMD_EVENT5 + "\n", Color.Black);
                            Thread.Sleep(500);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 修改日志文本框的内容
        /// </summary>
        /// <param name="str"></param>
        private void LogTxtChangedByDele(string str, Color color)
        {
            //非UI线程访问控件时
            if (txt_log1.InvokeRequired)
            {
                txt_log1.Invoke(new LogTxtDele(LogTxtChangedByDele), str, color);
            }
            else
            {
                txt_log1.SelectionColor = color;
                txt_log1.AppendText(str);
                //设置光标的位置到文本尾
                txt_log1.Select(txt_log1.TextLength, 0);
                //滚动到控件光标处  
                txt_log1.ScrollToCaret();
            }
        }

        /// <summary>
        /// 初始化控件需要的数据
        /// </summary>
        private void InitData()
        {
            CMD_EVENTALL_BYTE = MyConvertUtil.HexStrToBytes(CMD_EVENTALL);
            CMD_EVENT1_BYTE = MyConvertUtil.HexStrToBytes(CMD_EVENT1);
            CMD_EVENT2_BYTE = MyConvertUtil.HexStrToBytes(CMD_EVENT2);
            CMD_EVENT3_BYTE = MyConvertUtil.HexStrToBytes(CMD_EVENT3);
            CMD_EVENT4_BYTE = MyConvertUtil.HexStrToBytes(CMD_EVENT4);
            CMD_EVENT5_BYTE = MyConvertUtil.HexStrToBytes(CMD_EVENT5);
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
            cbx_parity.SelectedIndex = 2;
        }

        /// <summary>
        /// 启用禁用按钮控件
        /// </summary>
        /// <param name="flag"></param>
        private void EnableBtn(bool flag)
        {
            btn_start.Enabled = flag;
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
                //每次收到数据时都将事件重置
                _eventFlag[0] = false;
                _eventFlag[1] = false;
                _eventFlag[2] = false;
                _eventFlag[3] = false;
                _eventFlag[4] = false;
                string[] strArray = str.Split(' ');
                string type = strArray[8];
                if ("80".Equals(type) || "81".Equals(type) || "82".Equals(type) || "83".Equals(type) || "84".Equals(type) || "85".Equals(type))
                {
                    int len = MyConvertUtil.HexStrToInt(strArray[10] + strArray[9]);
                    switch (type)
                    {
                        //收到的所有事件
                        case "80":
                            _eventFlag[0] = false;
                            LogTxtChangedByDele("收到所有事件\n", Color.Green);
                            //产生了事件一
                            if (len >= 3)
                            {
                                int event00 = Convert.ToInt32(strArray[13], 16) - 51;
                                int event01 = Convert.ToInt32(strArray[12], 16) - 51;
                                int event02 = Convert.ToInt32(strArray[11], 16) - 51;
                                //有事件一产生
                                if (event00 + event01 + event02 > 0)
                                {
                                    _cycleTestStep = 1;
                                    _eventFlag[1] = true;
                                    LogTxtChangedByDele("有事件一产生\n", Color.Green);
                                }
                            }
                            //产生了事件二
                            if (len >= 6)
                            {
                                int event10 = Convert.ToInt32(strArray[16], 16) - 51;
                                int event11 = Convert.ToInt32(strArray[15], 16) - 51;
                                int event12 = Convert.ToInt32(strArray[14], 16) - 51;
                                //有事件二产生
                                if (event10 + event11 + event12 > 0)
                                {
                                    _cycleTestStep = 2;
                                    _eventFlag[2] = true;
                                    LogTxtChangedByDele("有事件二产生\n", Color.Green);
                                }
                            }
                            //产生了事件三
                            if (len >= 9)
                            {
                                int event20 = Convert.ToInt32(strArray[19], 16) - 51;
                                int event21 = Convert.ToInt32(strArray[18], 16) - 51;
                                int event22 = Convert.ToInt32(strArray[17], 16) - 51;
                                //有事件三产生
                                if (event20 + event21 + event22 > 0)
                                {
                                    _cycleTestStep = 3;
                                    _eventFlag[3] = true;
                                    LogTxtChangedByDele("有事件三产生\n", Color.Green);
                                }
                            }
                            //产生了事件四
                            if (len >= 12)
                            {
                                int event30 = Convert.ToInt32(strArray[22], 16) - 51;
                                int event31 = Convert.ToInt32(strArray[21], 16) - 51;
                                int event32 = Convert.ToInt32(strArray[20], 16) - 51;
                                //有事件四产生
                                if (event30 + event31 + event32 > 0)
                                {
                                    _cycleTestStep = 4;
                                    _eventFlag[4] = true;
                                    LogTxtChangedByDele("有事件四产生\n", Color.Green);
                                }
                            }
                            //产生了事件五
                            if (len >= 15)
                            {
                                int event40 = Convert.ToInt32(strArray[25], 16) - 51;
                                int event41 = Convert.ToInt32(strArray[24], 16) - 51;
                                int event42 = Convert.ToInt32(strArray[23], 16) - 51;
                                //有事件五产生
                                if (event40 + event41 + event42 > 0)
                                {
                                    _cycleTestStep = 5;
                                    _eventFlag[5] = true;
                                    LogTxtChangedByDele("有事件五产生\n", Color.Green);
                                }
                            }
                            break;
                        //收到的事件一
                        case "81":
                            _eventFlag[1] = false;
                            LogTxtChangedByDele("收到事件一\r\n", Color.Green);
                            break;
                        //收到的事件二
                        case "82":
                            _eventFlag[2] = false;
                            LogTxtChangedByDele("收到事件二\r\n", Color.Green);
                            break;
                        //收到的事件三
                        case "83":
                            _eventFlag[3] = false;
                            LogTxtChangedByDele("收到事件三\r\n", Color.Green);
                            break;
                        //收到的事件四
                        case "84":
                            _eventFlag[4] = false;
                            LogTxtChangedByDele("收到事件四\r\n", Color.Green);
                            //第一包数据为文件头
                            if (_isFileHead)
                            {
                                _isFileHead = false;
                                //总包数
                                int totalPkg0 = Convert.ToInt32(strArray[22], 16) - 51;
                                int totalPkg1 = Convert.ToInt32(strArray[21], 16) - 51;
                                int totalPkg2 = Convert.ToInt32(strArray[20], 16) - 51;
                                int totalPkg3 = Convert.ToInt32(strArray[19], 16) - 51;
                                String totalPkgStr = MyConvertUtil.HexStrToStr(totalPkg0 + totalPkg1 + totalPkg2 + totalPkg3 + "");
                                _totalPkg = Convert.ToInt32(totalPkgStr);
                                //总长度
                                int totalLen0 = Convert.ToInt32(strArray[26], 16) - 51;
                                int totalLen1 = Convert.ToInt32(strArray[25], 16) - 51;
                                int totalLen2 = Convert.ToInt32(strArray[24], 16) - 51;
                                int totalLen3 = Convert.ToInt32(strArray[23], 16) - 51;
                                String totalLenStr = MyConvertUtil.HexStrToStr(totalLen0 + totalLen1 + totalLen2 + totalLen3 + "");
                                _totalLen = Convert.ToInt32(totalLenStr);
                                //校验位，所有数据累加和，最后做对比，如果对得上则说明正确
                                _crc = strArray[30] + strArray[29] + strArray[28] + strArray[27];
                                //每包大小
                                int everyPkgSize0 = Convert.ToInt32(strArray[32], 16) - 51;
                                int everyPkgSize1 = Convert.ToInt32(strArray[31], 16) - 51;
                                String everyPkgSizeStr = MyConvertUtil.HexStrToStr(totalPkg0 + totalPkg1 + totalPkg2 + totalPkg3 + "");
                                _everyPkgSize = Convert.ToInt32(everyPkgSizeStr);
                                LogTxtChangedByDele("即将开始请求第一包\n", Color.Green);
                                if (_totalPkg > 0)
                                {
                                    //修改标识，继续请求下一包
                                    _eventFlag[4] = true;
                                }
                            }
                            //这里才是数据包
                            else
                            {
                                if (_currPkg < _totalPkg)
                                {
                                    //修改标识，继续请求下一包
                                    _eventFlag[4] = true;
                                    LogTxtChangedByDele("收到第" + _currPkg + "包数据\n", Color.Green);
                                    //当前包
                                    int currPkg0 = Convert.ToInt32(strArray[22], 16) - 51;
                                    int currPkg1 = Convert.ToInt32(strArray[21], 16) - 51;
                                    int currPkg = Convert.ToInt32(MyConvertUtil.HexStrToStr(currPkg0 + currPkg1 + ""));
                                    _currPkg = currPkg;
                                    //去掉包数组前面的和最后的校验码、16
                                    string[] pkgDataArray = strArray.Skip(2).Take(strArray.Length - 23 - 2).ToArray();
                                    string pkgData = string.Join(" ", pkgDataArray);
                                    _logger.WriteLog("包数据：" + pkgData);
                                    _pkgData += pkgData;
                                }
                                else
                                {
                                    //修改标识，停止请求下一包
                                    _eventFlag[4] = false;
                                    //每位都要减33（考虑有符号整数）
                                    string[] arrays = _pkgData.Split(' ');
                                    for (int i = 0; i < arrays.Length; i++)
                                    {
                                        int num = Convert.ToInt32(arrays[i], 16) - 51;
                                        if (num < 0)
                                        {
                                            arrays[i] = (128 + num | 128).ToString("X").PadLeft(2, '0');
                                        }
                                        else
                                        {
                                            arrays[i] = num.ToString("X").PadLeft(2, '0');
                                        }
                                    }
                                    _pkgData = string.Join("", arrays);
                                    //string转stream
                                    MemoryStream stream = new MemoryStream();
                                    StreamWriter writer = new StreamWriter(stream);
                                    writer.Write(_pkgData);
                                    writer.Flush();
                                    stream.Position = 0;
                                    Bitmap img = new Bitmap(stream);
                                    img.Save("e:\\1111.jpg");
                                    img.Dispose();
                                }
                            }
                            break;
                        //收到的事件五
                        case "85":
                            _eventFlag[5] = false;
                            LogTxtChangedByDele("收到事件五\r\n", Color.Green);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteException(ex);
                LogTxtChangedByDele(ex.ToString() + "\r\n", Color.Red);
            }
            finally
            {
                _isFileHead = true;
                _totalPkg = 0;
                _currPkg = 1;
                _totalLen = 0;
                _everyPkgSize = 0;
                _pkgData = "";
                _receivedStr = "";
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
                string str = MyConvertUtil.BytesToStr(byteArray).ToUpper();

                //C#串口偶尔会莫名其妙的发3F下来，网上说和校验位有关，但是改了校验位就和硬件无法通信，所以暂时这样处理
                str = str.Replace("3F3F", "");

                _logger.WriteLog("读取：" + str + "，长度：" + str.Length);
                _receivedStr += str;
                _logger.WriteLog("累计读取：" + _receivedStr + "，长度：" + _receivedStr.Length);
                //目前最短的数据内容的长度是21个字节
                //一条完整的命令至少包含两个68
                if (_receivedStr.Length < 20 || !_receivedStr.Contains("68 AA AA AA AA AA AA 68") || !_receivedStr.Contains("16"))
                {
                    _logger.WriteLog("继续读取...");
                    return;
                }
                _receivedStr = MyConvertUtil.StrAddChar(_receivedStr, 2, " ");
                //显示收到的数据
                LogTxtChangedByDele("收到（Hex）：" + _receivedStr + "\n", Color.Black);
                //解析数据
                Parse(_receivedStr);
            }
            catch (Exception ex)
            {
                //清空缓存
                _receivedStr = "";
                _logger.WriteException(ex);
                LogTxtChangedByDele(ex.ToString() + "\r\n", Color.Red);
            }
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            _cycleTestStep = -1;
            _eventAllThreadFlag = false;
            _eventAllThread = null;
        }

        /// <summary>
        /// 解析图片数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_start_click(object sender, EventArgs e)
        {
            _isFileHead = true;
            _totalPkg = 0;
            _currPkg = 1;
            _totalLen = 0;
            _everyPkgSize = 0;
            _pkgData = "";
            _receivedStr = "";
            if ("开始".Equals(btn_start.Text))
            {
                _cycleTestStep = 0;
                btn_start.Text = "停止";
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
            }
            else
            {
                btn_start.Text = "开始";
                _eventAllThreadFlag = false;
                _eventAllThread = null;
            }
        }

        /// <summary>
        /// 开启/关闭串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_open_click(object sender, EventArgs e)
        {
            _isFileHead = true;
            _totalPkg = 0;
            _currPkg = 1;
            _totalLen = 0;
            _everyPkgSize = 0;
            _pkgData = "";
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
                }
                else
                {
                    //关闭循环测试的线程
                    _eventAllThreadFlag = false;
                    _eventAllThread = null;
                    //关闭串口
                    _serialPort.Close();
                    _serialPort = null;
                    LogTxtChangedByDele("已关闭：" + _portName + "\r\n", Color.Black);
                    EnableBtn(false);
                    btn_open.Text = "打开串口";
                    btn_start.Text = "开始";
                }
                _cycleTestStep = -1;
            }
            catch (Exception ex)
            {
                _logger.WriteException(ex);
                LogTxtChangedByDele("串口" + _portName + "打开/关闭失败，原因：" + ex.ToString() + "\r\n", Color.Red);
            }
        }

        /// <summary>
        /// 清空日志文本框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_clear_Click(object sender, EventArgs e)
        {
            txt_log1.Clear();
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
