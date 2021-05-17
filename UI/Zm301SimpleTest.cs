using HelloCSharp.Log;
using HelloCSharp.Util;
using System;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace HelloCSharp.UI
{
    public partial class Zm301SimpleTest : Form
    {

        /// <summary>
        /// 所有Label委托
        /// </summary>
        /// <param name="str"></param>
        /// <param name="color"></param>
        private delegate void LabDele(Label lab, string str, Color color);

        /// <summary>
        /// 查询命令，校验码已提前算好
        /// </summary>
        private string READ_CMD = "FE FE FE FE 68 22 23 01 56 34 00 68 F1 00 91 16";
        private byte[] READ_CMD_BYTE;

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

        private MyLogger _logger = MyLogger.Instance;
        private SerialPort _serialPort;
        //本机串口
        private string[] _portNameArray;
        //串口名
        private string _portName = "";
        private string _receivedStr = "";
        //发送命令的线程
        private Thread _sendCmdThread;
        //发送命令的的线程的运行标识
        private bool _sendCmdThreadFlag = true;
        private bool _cycleTest = false;

        public Zm301SimpleTest()
        {
            InitializeComponent();
            READ_CMD_BYTE = MyConvertUtil.HexStrToBytes(READ_CMD);
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
            cbx_parity.SelectedIndex = 2;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="str"></param>
        private void Parse(string str)
        {
            string[] strArray = str.Split(' ');
            string type = strArray[8];
            int len = MyConvertUtil.HexStrToInt(strArray[9]);
            switch (type)
            {
                case "B1":
                    int sensor = Convert.ToInt32(strArray[10], 16) - 51;
                    if (sensor > 0)
                        LabChangedByDele(lab_sensor, "成功", Color.Green);
                    else
                        LabChangedByDele(lab_sensor, "失败", Color.Red);
                    int doorState = Convert.ToInt32(strArray[11], 16) - 51;
                    if (doorState == 1)
                        LabChangedByDele(lab_door, "开", Color.Green);
                    else
                        LabChangedByDele(lab_door, "关", Color.Red);
                    int openLock = Convert.ToInt32(strArray[12], 16) - 51;
                    if (openLock == 1)
                        LabChangedByDele(lab_lock, "成功", Color.Green);
                    else
                        LabChangedByDele(lab_lock, "失败", Color.Red);
                    int gpsState = Convert.ToInt32(strArray[13], 16) - 51;
                    if (gpsState == 1)
                        LabChangedByDele(lab_gps, "定位", Color.Green);
                    else
                        LabChangedByDele(lab_gps, "未定位", Color.Red);
                    int elec1 = (Convert.ToInt32(strArray[14], 16) - 51);
                    int elec2 = (Convert.ToInt32(strArray[15], 16) - 51);
                    //判断负数
                    string hexElec = elec1.ToString("X").Replace("FF", "") + elec2.ToString("X").Replace("FF", ""); ;
                    double elec = Convert.ToUInt32(hexElec, 16) / 1000.00;
                    if (elec >= 3.3 && elec <= 4.2)
                    {
                        LabChangedByDele(lab_elec, elec + "V", Color.Green);
                    }
                    else
                    {
                        LabChangedByDele(lab_elec, elec + "V（低电压）", Color.Red);
                    }
                    int ver1 = Convert.ToInt32(strArray[16], 16) - 51;
                    string hexVer1 = MyConvertUtil.HexStrToStr(ver1.ToString("X"));
                    int ver2 = Convert.ToInt32(strArray[17], 16) - 51;
                    string hexVer2 = MyConvertUtil.HexStrToStr(ver2.ToString("X"));
                    int ver3 = Convert.ToInt32(strArray[18], 16) - 51;
                    string hexVer3 = MyConvertUtil.HexStrToStr(ver3.ToString("X"));
                    int ver4 = Convert.ToInt32(strArray[19], 16) - 51;
                    string hexVer4 = MyConvertUtil.HexStrToStr(ver4.ToString("X"));
                    LabChangedByDele(lab_ver, hexVer1 + hexVer2 + hexVer3 + hexVer4, Color.Green);
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
            try
            {
                //待读字节长度
                int byteLen = _serialPort.BytesToRead;
                byte[] byteArray = new byte[byteLen];
                _serialPort.Read(byteArray, 0, byteArray.Length);
                string str = MyConvertUtil.BytesToStr(byteArray);

                //C#串口偶尔会莫名其妙的发3F下来，网上说和校验位有关，但是改了校验位就和硬件无法通信，所以暂时这样处理
                str = str.Replace("3F3F", "");

                _logger.WriteLog("本次读取字节：" + str + "，长度：" + byteLen);
                _receivedStr += str;
                _logger.WriteLog("累计收到字节（Hex）：" + _receivedStr);
                int beginIndex = _receivedStr.IndexOf("68");
                if (beginIndex > 0)
                    _receivedStr = _receivedStr.Substring(beginIndex);
                int endIndex = _receivedStr.LastIndexOf("16");
                if (endIndex > 0 && _receivedStr.Length >= 44)
                    _receivedStr = _receivedStr.Substring(0, endIndex + 2);
                //开头+结尾+校验码=完整数据
                if (_receivedStr.StartsWith("68") && _receivedStr.EndsWith("16"))
                {
                    //计算收到的数据的校验码的时候不包含最后两个字节（校验码和结尾的16）
                    string tempReceivedStr = _receivedStr.Substring(0, _receivedStr.Length - 4);
                    _logger.WriteLog("参与计算校验码的数据（Hex）：" + tempReceivedStr);
                    //计算出的校验码
                    string crcStr1 = MyConvertUtil.CalcZM301CRC(tempReceivedStr);
                    _logger.WriteLog("计算出的校验码（Hex）：" + crcStr1);
                    string[] strArray = MyConvertUtil.StrSplitInterval(_receivedStr, 2);
                    //收到的数据里的校验码
                    string crcStr2 = strArray[strArray.Length - 2];
                    _logger.WriteLog("数据内容包含的校验码（Hex）：" + crcStr2);
                    //比较校验码
                    bool flag = crcStr1.Equals(crcStr2);
                    _logger.WriteLog("校验码是否正确：" + flag);
                    if (flag)
                    {
                        int len = _receivedStr.Length;
                        _logger.WriteLog("收到完整的指令（Hex）：" + _receivedStr + "，长度：" + len);
                        _receivedStr = MyConvertUtil.StrAddChar(_receivedStr, 2, " ");
                        //解析数据
                        Parse(_receivedStr);
                        //已收到完整数据，清空缓存
                        _receivedStr = "";
                        _logger.WriteLog("已收到完整数据，清空缓存！");
                    }
                    else
                    {
                        _logger.WriteLog("校验码不正确，继续读取...");
                        //收到的数据错误，清空缓存
                        if (_receivedStr.Length > 68)
                            _receivedStr = "";
                    }
                }
            }
            catch (Exception ex)
            {
                //清空缓存
                _receivedStr = "";
                _logger.WriteException(ex);
            }
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        private void SendCmdForThread()
        {
            do
            {
                _serialPort.Write(READ_CMD_BYTE, 0, READ_CMD_BYTE.Length);
                Thread.Sleep(3 * 1000);
            }
            while (_cycleTest && _sendCmdThreadFlag && null != _serialPort && _serialPort.IsOpen);
        }

        /// <summary>
        /// 修改所有Label标签的内容
        /// </summary>
        /// <param name="lab"></param>
        /// <param name="str"></param>
        /// <param name="color"></param>
        private void LabChangedByDele(Label lab, string str, Color color)
        {
            //非UI线程访问控件时
            if (lab.InvokeRequired)
            {
                lab.Invoke(new LabDele(LabChangedByDele), lab, str, color);
            }
            else
            {
                _logger.WriteLog(str);
                lab.Text = str;
                lab.ForeColor = color;
            }
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
                    lab_ver.Text = "null";
                    lab_sensor.Text = "null";
                    lab_door.Text = "null";
                    lab_lock.Text = "null";
                    lab_gps.Text = "null";
                    lab_elec.Text = "null";
                    lab_ver.ForeColor = Color.Black;
                    lab_sensor.ForeColor = Color.Black;
                    lab_door.ForeColor = Color.Black;
                    lab_lock.ForeColor = Color.Black;
                    lab_gps.ForeColor = Color.Black;
                    lab_elec.ForeColor = Color.Black;
                    //打开串口
                    if (null == _serialPort)
                    {
                        _serialPort = new SerialPort(_portName, 115200, Parity.Even, 8, StopBits.One);
                        _serialPort.DataReceived += new SerialDataReceivedEventHandler(ReceivedComData);
                    }
                    _serialPort.Open();
                    btn_open.Text = "关闭串口";
                    cbx_port.Enabled = false;
                    chk_cycle.Enabled = false;
                    //开启读取超时的线程，里面对串口有作判断，必须放在串口操作之后
                    if (null == _sendCmdThread)
                    {
                        _sendCmdThreadFlag = true;
                        _sendCmdThread = new Thread(SendCmdForThread);
                        _sendCmdThread.Start();
                    }
                    else
                    {
                        _sendCmdThreadFlag = false;
                        _sendCmdThread = null;
                        _sendCmdThreadFlag = true;
                        _sendCmdThread = new Thread(SendCmdForThread);
                        _sendCmdThread.Start();
                    }
                }
                else
                {
                    //关闭读取超时的线程
                    _sendCmdThreadFlag = false;
                    _sendCmdThread = null;
                    //关闭串口
                    _serialPort.Close();
                    _serialPort = null;
                    btn_open.Text = "打开串口";
                    cbx_port.Enabled = true;
                    chk_cycle.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                _logger.WriteException(ex);
            }
        }

        private void cbx_port_SelectedIndexChanged(object sender, EventArgs e)
        {
            _portName = _portNameArray[cbx_port.SelectedIndex];
        }
        
        private void chk_cycle_CheckedChanged(object sender, EventArgs e)
        {
            _cycleTest = chk_cycle.Checked;
        }
    }
}
