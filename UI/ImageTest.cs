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
        /// 对时，为了方便测试，这里直接写死
        /// </summary>
        private string CMD_CHECK_TIME = "68 AA AA AA AA AA AA 68 07 06 00 84 3B 36 34 34 33 69 16";
        private byte[] CMD_CHECK_TIME_BYTE;

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
        private string CMD_EVENT2 = "68 AA AA AA AA AA AA 68 02 00 00 CE 16";
        private byte[] CMD_EVENT2_BYTE;

        /// <summary>
        /// 查询事件三，校验码已提前算好
        /// </summary>
        private string CMD_EVENT3 = "68 AA AA AA AA AA AA 68 03 00 00 CF 16";
        private byte[] CMD_EVENT3_BYTE;

        /// <summary>
        /// 查询事件四，校验码已提前算好
        /// </summary>
        private string CMD_EVENT4 = "68 AA AA AA AA AA AA 68 04 00 00 D0 16";
        private byte[] CMD_EVENT4_BYTE;

        /// <summary>
        /// 查询事件五，校验码已提前算好
        /// </summary>
        private string CMD_EVENT5 = "68 AA AA AA AA AA AA 68 05 00 00 D1 16";
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
        private string _receivedStr = "";
        //查询事件总数的线程
        private Thread _eventAllThread;
        //查询事件总数的线程的运行标识
        private bool _eventAllThreadFlag = true;
        //本机串口
        private string[] _portNameArray;
        //串口名
        private string _portName = "";
        //波特率、数据位
        private int _rate = 1200, _data = 8;
        //校验位
        private Parity _parity = Parity.None;
        //循环测试时当前执行到的步骤：0所有事件、1事件一、2事件二、3事件三、4事件四、5事件五
        private int _cycleTestStep = 0;
        //校验位
        private string _crc = "";
        //总包数、当前包数、总长度、每包大小
        private int _totalPkg = 0, _currPkg = 1, _totalLen = 0, _everyPkgSize = 0;
        //对时标识、文件头标识
        private bool _isCheckTime = false, _isFileHead = true;
        //图片数据（由包数据截取而来）
        private string _imgData = "";

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
            //循环查询事件总数，事件数大于零则一直循环查询对应的事件
            while (_eventAllThreadFlag && null != _serialPort && _serialPort.IsOpen)
            {
                //_logger.WriteLog("等待发送查询指令...");
                switch (_cycleTestStep)
                {
                    //查询所有事件（没有任何事件产生，则继续查询）
                    case 0:
                        _cycleTestStep = -1;
                        //先对时
                        if (!_isCheckTime)
                        {
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
                            string cmd = "68AAAAAAAAAA68070600" + hex_ss + hex_mm + hex_HH + hex_DD + hex_MM + hex_yy;
                            string crc = MyConvertUtil.CRC_Zistone_BLE(MyConvertUtil.StrAddChar(cmd, 2, " "));
                            cmd = MyConvertUtil.StrAddChar(cmd + crc + "16", 2, " ");
                            //byte[] timeCmdByte = MyConvertUtil.HexStrToBytes(cmd);
                            //_serialPort.Write(timeCmdByte, 0, timeCmdByte.Length);
                            _serialPort.Write(CMD_CHECK_TIME_BYTE, 0, CMD_CHECK_TIME_BYTE.Length);
                            //LogTxtChangedByDele("发送对时指令：" + cmd + "（20" + yy + "年" + MM + "月" + DD + "日" + HH + "时" + mm + "分" + ss + "秒）\n", Color.Black);
                            LogTxtChangedByDele("发送对时指令：" + CMD_CHECK_TIME + "\n", Color.Black);
                            Thread.Sleep(500);
                        }
                        //再查询
                        else
                        {
                            _serialPort.Write(CMD_EVENTALL_BYTE, 0, CMD_EVENTALL_BYTE.Length);
                            LogTxtChangedByDele("发送查询所有事件指令：" + CMD_EVENTALL + "\n", Color.Black);
                            Thread.Sleep(500);
                        }
                        break;

                    //查询事件四
                    case 4:
                        _cycleTestStep = -1;
                        //请求文件头
                        if (_isFileHead)
                        {
                            _serialPort.Write(CMD_EVENT4_BYTE, 0, CMD_EVENT4_BYTE.Length);
                            LogTxtChangedByDele("发送查询事件四指令（请求文件头）：" + CMD_EVENT4 + "\n", Color.Black);
                            Thread.Sleep(500);
                        }
                        //请求第n包数据
                        else
                        {
                            string hexCurrPkg = _currPkg.ToString().PadLeft(4, '0');
                            //当前包数的高位放到后面并添加空格
                            string[] tempCurrPkg = MyConvertUtil.StrSplitInterval(hexCurrPkg, 2);
                            int tempPkg0 = Convert.ToInt32(tempCurrPkg[0]) + 51;
                            int tempPkg1 = Convert.ToInt32(tempCurrPkg[1]) + 51;
                            string currPkg = tempPkg1.ToString("X") + " " + tempPkg0.ToString("X");
                            //计算校验码
                            string crc = MyConvertUtil.CRC_Zistone_BLE(CMD_PKG_FIXED + " " + currPkg);
                            //最终请求指令
                            string cmd = CMD_PKG_FIXED + " " + currPkg + " " + crc + " 16";
                            byte[] cmdBytes = MyConvertUtil.HexStrToBytes(cmd);
                            _serialPort.Write(cmdBytes, 0, cmdBytes.Length);
                            LogTxtChangedByDele("发送查询第" + _currPkg + "包指令：" + cmd + "\n", Color.Black);
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
            _logger.WriteLog(str);
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
            CMD_CHECK_TIME_BYTE = MyConvertUtil.HexStrToBytes(CMD_CHECK_TIME);
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
            cbx_rate.SelectedIndex = 8;
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
            cbx_parity.SelectedIndex = 0;
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
                string[] strArray = MyConvertUtil.StrSplitInterval(str, 2);
                string type = strArray[8];
                if ("80".Equals(type) || "81".Equals(type) || "82".Equals(type) || "83".Equals(type) || "84".Equals(type) || "85".Equals(type) || "87".Equals(type))
                {
                    int len = MyConvertUtil.HexStrToInt(strArray[10] + strArray[9]);
                    switch (type)
                    {
                        //对时响应
                        case "87":
                            _isCheckTime = true;
                            _cycleTestStep = 0;
                            LogTxtChangedByDele("收到对时响应\n", Color.Green);
                            break;
                        //收到的所有事件
                        case "80":
                            LogTxtChangedByDele("收到所有事件\n", Color.Green);
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
                                    LogTxtChangedByDele("有事件四产生\n", Color.Green);
                                }
                                else
                                {
                                    _cycleTestStep = 0;
                                }
                            }
                            break;
                        //收到的事件四
                        case "84":
                            LogTxtChangedByDele("收到事件四\n", Color.Green);
                            //第一包数据为文件头
                            if (_isFileHead)
                            {
                                LogTxtChangedByDele("该包为文件头\n", Color.Green);
                                _isFileHead = false;
                                //总包数
                                int totalPkg0 = Convert.ToInt32(strArray[22], 16) - 51;
                                int totalPkg1 = Convert.ToInt32(strArray[21], 16) - 51;
                                int totalPkg2 = Convert.ToInt32(strArray[20], 16) - 51;
                                int totalPkg3 = Convert.ToInt32(strArray[19], 16) - 51;
                                string totalPkg0Str = totalPkg0.ToString("X").PadLeft(2, '0').Replace("FFFFFF", "");
                                string totalPkg1Str = totalPkg1.ToString("X").PadLeft(2, '0').Replace("FFFFFF", "");
                                string totalPkg2Str = totalPkg2.ToString("X").PadLeft(2, '0').Replace("FFFFFF", "");
                                string totalPkg3Str = totalPkg3.ToString("X").PadLeft(2, '0').Replace("FFFFFF", "");
                                _totalPkg = Convert.ToInt32(totalPkg0Str + totalPkg1Str + totalPkg2Str + totalPkg3Str, 16);
                                //总长度
                                int totalLen0 = Convert.ToInt32(strArray[26], 16) - 51;
                                int totalLen1 = Convert.ToInt32(strArray[25], 16) - 51;
                                int totalLen2 = Convert.ToInt32(strArray[24], 16) - 51;
                                int totalLen3 = Convert.ToInt32(strArray[23], 16) - 51;
                                string totalLen0Str = totalLen0.ToString("X").PadLeft(2, '0').Replace("FFFFFF", "");
                                string totalLen1Str = totalLen1.ToString("X").PadLeft(2, '0').Replace("FFFFFF", "");
                                string totalLen2Str = totalLen2.ToString("X").PadLeft(2, '0').Replace("FFFFFF", "");
                                string totalLen3Str = totalLen3.ToString("X").PadLeft(2, '0').Replace("FFFFFF", "");
                                _totalLen = Convert.ToInt32(totalLen0Str + totalLen1Str + totalLen2Str + totalLen3Str, 16);
                                //校验位，所有数据累加和，最后做对比，如果对得上则说明正确
                                _crc = strArray[30] + strArray[29] + strArray[28] + strArray[27];
                                //每包大小
                                int everyPkgSize0 = Convert.ToInt32(strArray[32], 16) - 51;
                                int everyPkgSize1 = Convert.ToInt32(strArray[31], 16) - 51;
                                string everyPkgSize0Str = everyPkgSize0.ToString("X").PadLeft(2, '0');
                                string everyPkgSize1Str = everyPkgSize1.ToString("X").PadLeft(2, '0');
                                _everyPkgSize = Convert.ToInt32(everyPkgSize0Str + everyPkgSize1Str, 16);
                                LogTxtChangedByDele("总包数：" + _totalPkg + "，总长度：" + _totalLen + "，校验位：" + _crc + "\n", Color.Green);
                                LogTxtChangedByDele("即将开始请求第1包\n", Color.Green);
                                if (_totalPkg > 0)
                                {
                                    //修改标识，继续请求下一包
                                    _cycleTestStep = 4;
                                }
                            }
                            //这里才是数据包
                            else
                            {
                                if (_currPkg <= _totalPkg)
                                {
                                    LogTxtChangedByDele("收到第" + _currPkg + "包数据\n", Color.Green);
                                    //LogTxtChangedByDele("第" + _currPkg + "包数据：" + string.Join(" ", strArray) + "\n", Color.Black);
                                    //当前包
                                    int currPkg0 = Convert.ToInt32(strArray[22], 16) - 51;
                                    int currPkg1 = Convert.ToInt32(strArray[21], 16) - 51;
                                    string currPkg0Str = currPkg0.ToString("X").PadLeft(2, '0');
                                    string currPkg1Str = currPkg1.ToString("X").PadLeft(2, '0');
                                    _currPkg = Convert.ToInt32(currPkg0Str + currPkg1Str, 16) + 1;
                                    //去掉包数组前面的和最后的校验码、16
                                    string[] pkgDataArray = strArray.Skip(23).Take(strArray.Length - 25).ToArray();
                                    _imgData += string.Join("", pkgDataArray);
                                    _logger.WriteLog("该包的图片数据：" + _imgData);
                                    //修改标识，继续请求下一包
                                    _cycleTestStep = 4;
                                }
                                else
                                {
                                    //修改标识，停止读取图片数据
                                    _cycleTestStep = 4;
                                    _logger.WriteLog("完整的图片数据：" + _imgData);
                                    LogTxtChangedByDele("所有包数据收完，开始解析...", Color.Green);
                                    //每位都要减33（考虑有符号整数）
                                    string[] arrays = MyConvertUtil.StrAddChar(_imgData, 2, " ").Split(' ');
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
                                    string strs = string.Join("", arrays);
                                    byte[] bytes = MyConvertUtil.HexStrToBytes(strs);
                                    MemoryStream stream = new MemoryStream(bytes);
                                    Image image = Bitmap.FromStream(stream, true);
                                    Bitmap bitmap = new Bitmap(image);
                                    bitmap.Save("e:\\111.png");
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _cycleTestStep = -1;
                _receivedStr = "";
                _logger.WriteException(ex);
                LogTxtChangedByDele(ex.ToString() + "\r\n", Color.Red);
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

                _receivedStr += str;
                //判断是否为一包完整的命令
                if (_receivedStr.Length < 20 || !_receivedStr.StartsWith("68AAAAAAAAAAAA68") || !_receivedStr.EndsWith("16"))
                {
                    _logger.WriteLog("继续读取...");
                    return;
                }
                //显示收到的数据
                LogTxtChangedByDele("收到（Hex）：" + MyConvertUtil.StrAddChar(_receivedStr, 2, " ") + "\n", Color.Black);
                //解析数据
                Parse(_receivedStr);
                _receivedStr = "";
            }
            catch (Exception ex)
            {
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
            _receivedStr = "";
            _isFileHead = true;
            _totalPkg = 0;
            _currPkg = 1;
            _totalLen = 0;
            _everyPkgSize = 0;
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
            _rate = RATE_ARRAY[cbx_rate.SelectedIndex];
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
