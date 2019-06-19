using CodingMouse;
using HelloCSharp.Log;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace TestIMEI
{
    public partial class TestIMEIWindow : Form
    {
        private string _configFilePath = "TestIMEIConfig";
        private string _logPath = "TestIMEILog";
        private Dictionary<string, SerialPort> _portDictionary = new Dictionary<string, SerialPort>();
        private string[] _ports;
        private SerialPort _serialPort;
        private string _snNumber = "";
        private System.Timers.Timer _timer;

        /// <summary>
        /// Button控件
        /// </summary>
        /// <param name="flag"></param>
        private delegate void ButtonDele(bool flag);

        /// <summary>
        /// 释放串口
        /// </summary>
        private delegate void CloseIsOpenSerialPortDele();

        /// <summary>
        /// Label1控件
        /// </summary>
        /// <param name="flag"></param>
        private delegate void Label1Dele(bool flag);

        /// <summary>
        /// Label2控件
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="snNumber"></param>
        private delegate void Label2Dele(bool flag, string snNumber = "");

        /// <summary>
        /// 弹窗
        /// </summary>
        private delegate bool MessageBoxDele();

        /// <summary>
        /// 查询设备是否写入过SN
        /// </summary>
        private delegate void QueryDeviceSNDele();

        /// <summary>
        /// 查询设备是否通畅
        /// </summary>
        /// <param name="dictionary"></param>
        private delegate void QueryDeviceDele(Dictionary<string, SerialPort> dictionary);

        public TestIMEIWindow()
        {
            InitializeComponent();
            //生成本地日志
            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }
            _logPath += "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            FileStream fileStream = new FileStream(_logPath, FileMode.Append, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.Close();
            fileStream.Close();
            //生成外部文件的目录
            if (!Directory.Exists(_configFilePath))
            {
                Directory.CreateDirectory(_configFilePath);
            }
            _configFilePath += "\\IMEIConfig.txt";
            //获取所有串口名
            _ports = SerialPort.GetPortNames();
            Array.Sort(_ports);
            //定时器,在判断设备是否有连接后启动
            _timer = new System.Timers.Timer(500);
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerEvent);
            _timer.AutoReset = true;
            //程序启动时需要判断是否有设备连接
            FirstRunConnState();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            ButtonStateChanged(false);
            if (_serialPort != null)
            {
                if (_snNumber.Length < 8)
                {
                    MessageBox.Show("请检查该设备SN是否正确", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ButtonStateChanged(true);
                    return;
                }
                //一定要开启串口,因为每次写入完成都会关闭串口,尽量避免拨掉设备时抛出"资源占用"的异常
                _serialPort.Close();
                _serialPort.Open();
                //查询该设备写入
                WriterIMEI(ref _serialPort);
                Thread.Sleep(1000);
                //关闭所有串口,将控件上的SN置为空
                CloseIsOpenSerailPort();
                //在所有串口关闭以后再将按钮状态置为可用状态,避免
                ButtonStateChanged(true);
            }
            else
            {
                Logger.Instance.WriteLog("对象_serialPort为Null。。。");
                MessageBox.Show("写入失败!\r\n请与管理员联系...", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ButtonStateChanged(true);
            }
        }

        /// <summary>
        /// 按钮状态更新
        /// </summary>
        /// <param name="flag"></param>
        private void ButtonStateChanged(bool flag)
        {
            button1.Enabled = flag;
            button1.Invalidate();
        }

        /// <summary>
        /// 按钮状态的更新
        /// <param name="flag"></param>
        private void ButtonStateChangedByDele(bool flag)
        {
            //非UI线程访问该控件时
            if (button1.InvokeRequired)
            {
                button1.Invoke(new ButtonDele(ButtonStateChangedByDele), flag);
                return;
            }
            ButtonStateChanged(flag);
        }

        /// <summary>
        /// 释放串口(主线程调用才会起作用)
        /// </summary>
        private void CloseIsOpenSerailPort()
        {
            Dictionary<string, SerialPort>.ValueCollection values = _portDictionary.Values;
            foreach (SerialPort port in values)
            {
                if (port.IsOpen)
                {
                    port.Close();
                }
            }
            _portDictionary.Clear();
            if (_serialPort != null)
            {
                _serialPort.Close();
            }
        }

        /// <summary>
        /// 比较串口名数组内的元素是否一致
        /// </summary>
        /// <param name="array"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        private bool CompareComNameArray(string[] array, string[] array2)
        {
            IEnumerable<string> enums = from a in array join a2 in array2 on a equals a2 select a;
            return array.Length == array2.Length && enums.Count() == array.Length;
        }

        /// <summary>
        /// 查询设备是否写入过SN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceivedQuerySN(object sender, SerialDataReceivedEventArgs e)
        {
            //阻塞该线程,以防上一次的数据没有读完
            Thread.Sleep(20);
            SerialPort tempSerialPort = (SerialPort)sender;
            string portName = tempSerialPort.PortName;
            try
            {
                //读取缓冲区所有字节
                string tempStr = tempSerialPort.ReadExisting();
                //向设备写入AT+QCSN?命令后返回的内容
                if (tempStr.Contains("OK") && tempStr.IndexOf("AT+QCSN?") == 0 && tempStr.Contains("+QCSN:"))
                {
                    _snNumber = SubTwoStrContent(tempStr, "\"", "\"");
                    if (_snNumber.Length < 8)
                    {
                        Label1TextChangedByDele(false);
                        Label2StateChangedByDele(false);
                        ButtonStateChangedByDele(false);
                    }
                    else
                    {
                        _serialPort = tempSerialPort;
                        Label1TextChangedByDele(true);
                        Label2StateChangedByDele(true, _snNumber);
                        ButtonStateChangedByDele(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteException(ex, "在接收AT+QCSN?命令时发生异常,串口名:" + portName);
            }
        }

        /// <summary>
        /// 禁止写入
        /// </summary>
        private bool DeviceError()
        {
            MessageBox.Show("该设备COM口未较准,禁止写入!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        /// <summary>
        /// 程序启动时设备的连接状态
        /// </summary>
        private void FirstRunConnState()
        {
            //_serialPort不为null则说明有设备连接
            _portDictionary = TestDevice(_ports);
            Thread.Sleep(200);
            //程序首次运行完毕后再启动定时器
            _timer.Start();
        }

        /// <summary>
        /// 获取本机所有实例化过的串口
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, SerialPort> GetAllSerialPort()
        {
            string[] ports = SerialPort.GetPortNames();
            Dictionary<string, SerialPort> dictionary = new Dictionary<string, SerialPort>();
            foreach (string portName in ports)
            {
                //TODO:波特率暂时写死
                SerialPort serialPort = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
                try
                {
                    dictionary.Add(portName, serialPort);
                    serialPort.RtsEnable = true;
                    serialPort.DtrEnable = true;
                    serialPort.Handshake = Handshake.None;
                    serialPort.ReceivedBytesThreshold = 1;
                }
                catch (Exception ex)
                {
                    Logger.Instance.WriteException(ex, "实例化串口" + portName + "时发生异常");
                    serialPort.Close();
                }
            }
            return dictionary;
        }

        /// <summary>
        /// Label1内容更新
        /// </summary>
        /// <param name="flag"></param>
        private void Label1TextChanged(bool flag)
        {
            if (flag)
            {
                label1.Text = "设备已连接";
                label1.ForeColor = Color.Green;
            }
            else
            {
                label1.Text = "设备已断开";
                label1.ForeColor = Color.Red;
            }
            label1.Invalidate();
        }

        /// <summary>
        /// Label1内容更新
        /// </summary>
        /// <param name="flag"></param>
        private void Label1TextChangedByDele(bool flag)
        {
            //非UI线程访问该控件时
            if (label1.InvokeRequired)
            {
                label1.Invoke(new Label1Dele(Label1TextChangedByDele), flag);
                return;
            }
            Label1TextChanged(flag);
        }

        /// <summary>
        /// Label2内容更新
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="snNumber"></param>
        private void Label2StateChanged(bool flag, string snNumber = "")
        {
            label2.Text = snNumber;
            label2.Visible = flag;
            label2.Invalidate();
        }

        /// <summary>
        /// Label2内容更新
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="snNumber"></param>
        private void Label2StateChangedByDele(bool flag, string snNumber = "")
        {
            //非UI线程访问该控件时
            if (label2.InvokeRequired)
            {
                label2.Invoke(new Label2Dele(Label2StateChangedByDele), flag, snNumber);
                return;
            }
            Label2StateChanged(flag, snNumber);
        }

        /// <summary>
        /// 定时检测串口是否有变动
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnTimerEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            string[] tempPorts = SerialPort.GetPortNames();
            Array.Sort(tempPorts);
            //串口数量有变动
            if (_ports.Length != tempPorts.Length)
            {
                //首先释放所有串口,尽量避免后面的设备访问不了串口
                CloseIsOpenSerialPortDele closePort = new CloseIsOpenSerialPortDele(CloseIsOpenSerailPort);
                Invoke(closePort);
                //有设备连接
                if (tempPorts.Length > _ports.Length)
                {
                    _portDictionary = TestDevice(tempPorts);
                }
                //设备断开
                else
                {
                    //关闭设备的串口
                    if (_serialPort != null)
                    {
                        _serialPort.Close();
                    }
                    _snNumber = "";
                    //提示设备断开、不再显示SN、禁止写入
                    Label1Dele label1Dele = new Label1Dele(Label1TextChanged);
                    this.BeginInvoke(label1Dele, new object[] { false });
                    ButtonDele buttonDele = new ButtonDele(ButtonStateChanged);
                    this.BeginInvoke(buttonDele, new object[] { false });
                    Label2Dele label2Dele = new Label2Dele(Label2StateChangedByDele);
                    this.BeginInvoke(label2Dele, new object[] { false, _snNumber });
                }
                _ports = (string[])tempPorts.Clone();
            }
        }

        /// <summary>
        /// 替换该设备在本地日志的记录
        /// </summary>
        /// <param name="index"></param>
        /// <param name="content"></param>
        private void OverlayWriterLocalLog(ref int index, ref string content)
        {
            string[] arrayLines = File.ReadAllLines(_logPath);
            arrayLines[index - 1] = content;
            File.WriteAllLines(_logPath, arrayLines);
        }

        /// <summary>
        /// 测试串口是否通畅,写入的是串口总是返回查询内容的ATE1命令
        /// </summary>
        private Dictionary<string, SerialPort> TestDevice(string[] ports)
        {
            Dictionary<string, SerialPort> dictionary = new Dictionary<string, SerialPort>();
            foreach (string portName in ports)
            {
                //TODO:波特率暂时写死
                SerialPort serialPort = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
                try
                {
                    dictionary.Add(portName, serialPort);
                    serialPort.RtsEnable = true;
                    serialPort.DtrEnable = true;
                    serialPort.Handshake = Handshake.None;
                    serialPort.ReceivedBytesThreshold = 1;
                    //防止事件重复触发
                    serialPort.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedQuerySN);
                    serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedQuerySN);
                    serialPort.Open();
                    serialPort.Write("AT+QCSN?\r\n");
                    Thread.Sleep(50);
                    serialPort.Close();
                }
                catch (Exception ex)
                {
                    serialPort.Close();
                    Logger.Instance.WriteException(ex, "实例化串口时发生异常");
                }
            }
            return dictionary;
        }

        /// <summary>
        /// 向设备写入
        /// 为避免重复写入,对外部文件进行读一行删一行
        /// <param name="serialPort"></param>
        private void WriterIMEI(ref SerialPort serialPort)
        {
            try
            {
                //按行读取所有内容
                string[] arrayLines = File.ReadAllLines(_configFilePath, Encoding.UTF8);
                for (int i = 0; i < arrayLines.Length; i++)
                {
                    //从上往下获取第一个有内容的行
                    string[] tempContents = arrayLines[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempContents.Length < 1)
                    {
                        if (i == arrayLines.Length - 1)
                        {
                            MessageBox.Show("写入失败!\r\n请检查外部配置文件是否已读取完毕", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ButtonStateChanged(true);
                        }
                        continue;
                    }
                    else
                    {
                        //错误信息
                        string errorStr = "";
                        DataBaseClass dataBaseClass = new DataBaseClass();
                        string imei1 = "";
                        string imei2 = "";
                        string wifimac = "";
                        string btmac = "";
                        //要写入设备的内容
                        StringBuilder content = new StringBuilder();
                        //IMEI1
                        if (tempContents.Length > 0 && checkBox1.Checked)
                        {
                            imei1 = tempContents[0];
                            content.Append(_snNumber);
                            content.Append(" ");
                            content.Append(imei1);
                        }
                        //IMEI2
                        if (tempContents.Length > 1 && checkBox2.Checked)
                        {
                            imei2 = tempContents[1];
                            content.Append(" ");
                            content.Append(imei2);
                        }
                        //WIFI-MAC
                        if (tempContents.Length > 2 && checkBox3.Checked)
                        {
                            wifimac = tempContents[2];
                            wifimac = Regex.Replace(wifimac, ":", "");
                            content.Append(" ");
                            content.Append(wifimac);
                        }
                        //BT-MAC
                        if (tempContents.Length > 3 && checkBox4.Checked)
                        {
                            btmac = tempContents[3];
                            btmac = Regex.Replace(btmac, ":", "");
                            content.Append(" ");
                            content.Append(btmac);
                        }
                        if (!"".Equals(errorStr))
                        {
                        }
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append("Update workorder set ");
                        if (!"".Equals(imei1))
                        {
                            stringBuilder.Append("SIM_IMEI = '" + imei1 + "'");
                        }
                        if (!"".Equals(imei2))
                        {
                        }
                        if (!"".Equals(wifimac))
                        {
                            stringBuilder.Append(", WIFI_MAC = '" + wifimac + "'");
                        }
                        if (!"".Equals(btmac))
                        {
                            stringBuilder.Append(", BT_MAC = '" + btmac + "'");
                        }
                        stringBuilder.Append(" where IMEI1 = '" + _snNumber + "'");
                        int returnNum = dataBaseClass.RunCommand(stringBuilder.ToString());
                        if (returnNum < 1)
                        {
                            MessageBox.Show("录入系统失败,请与联系技术支持人员处理!");
                            ButtonStateChanged(true);
                            return;
                        }
                        Logger.Instance.WriteLog("数据库的IMEI命令写入完毕");
                        ////如果这台设备已经写入过IMEI、MAC直接覆盖,同样本地日志文件也要覆盖
                        //int tempIndex = ReadContentByLine(ref _snNumber);
                        //string tempStr = content.ToString();
                        //if (tempIndex > 0)
                        //{
                        //    OverlayWriterLocalLog(ref tempIndex, ref tempStr);
                        //    Thread.Sleep(200);
                        //}
                        ////没有则直接写入本地日志
                        //else
                        //{
                        //    WriterLocalLog(tempStr);
                        //}
                        //Logger.Instance.WriteLog("本地的IMEI命令写入完毕");
                        //该行记录清空
                        arrayLines[i] = "";
                        //写回外部文件
                        File.WriteAllLines(_configFilePath, arrayLines, Encoding.UTF8);
                        Thread.Sleep(200);
                        //显示在文本框
                        string content2 = " SN是" + _snNumber + "\tIMEI1是" + imei1 + "\tIMEI2是" + imei2 + "\tWIFI-MAC是" + wifimac + "\tBT-MAC是" + btmac;
                        TextBoxChanged(4, ref content2);
                        //最后进行设备的写入
                        if (!"".Equals(imei1))
                            serialPort.Write("at+egmr=1,7,\"" + imei1 + "\"\r\n");
                        if (!"".Equals(imei2))
                            serialPort.Write("at+egmr=1,10,\"" + imei2 + "\"\r\n");
                        if (!"".Equals(wifimac))
                            serialPort.Write("at+qnvw=4678,0,\"" + wifimac + "\"\r\n");
                        if (!"".Equals(btmac))
                            serialPort.Write("at+qnvw=447,0,\"" + btmac + "\"\r\n");
                        Logger.Instance.WriteLog("设备的IMEI命令写入完毕");
                        ButtonStateChanged(true);
                    }
                    break;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteException(ex, "写入IMEI命令时发生异常");
                MessageBox.Show("写入IMEI命令时发生异常", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ButtonStateChanged(true);
            }
        }

        /// <summary>
        /// 写入本地日志
        /// </summary>
        private void WriterLocalLog(string content)
        {
            FileStream fileStream = new FileStream(_logPath, FileMode.Append, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.WriteLine(content, Encoding.UTF8);
            streamWriter.Close();
            fileStream.Close();
        }

        /// <summary>
        /// 判断SN在日志中是否存在
        /// </summary>
        /// <param name="snNumber"></param>
        /// <returns></returns>
        private int ReadContentByLine(ref string snNumber)
        {
            FileStream fileStream = new FileStream(_logPath, FileMode.Open, FileAccess.Read);
            StreamReader streamReader = new StreamReader(fileStream);
            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            string strLine;
            int index = 0;
            while ((strLine = streamReader.ReadLine()) != null)
            {
                index++;
                string[] tempArray = strLine.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (tempArray[0].Equals(snNumber))
                {
                    break;
                }
            }
            fileStream.Close();
            streamReader.Close();
            return index;
        }

        /// <summary>
        /// 截取两个字符串之间的内容
        /// </summary>
        /// <param name="sourse"></param>
        /// <param name="startstr"></param>
        /// <param name="endstr"></param>
        /// <returns></returns>
        private string SubTwoStrContent(string sourse, string startStr, string endStr)
        {
            string result = string.Empty;
            int startIndex, endIndex;
            try
            {
                //开始字符首次出现的位置
                startIndex = sourse.IndexOf(startStr);
                if (startIndex == -1)
                {
                    return result;
                }
                //截取开始字符之后的所有字符
                string tmpStr = sourse.Substring(startIndex + startStr.Length);
                //结束字符首次出现的位置
                endIndex = tmpStr.IndexOf(endStr);
                if (endIndex == -1)
                {
                    return result;
                }
                //删除结束字符之后的所有字符
                result = tmpStr.Remove(endIndex);
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteException(ex, "截取字符串时发生异常");
            }
            return result;
        }

        /// <summary>
        /// 文本框内容更新
        /// </summary>
        /// <param name="param"></param>
        /// <param name="param2"></param>
        private void TextBoxChanged(int param, ref string param2)
        {
            switch (param)
            {
                case 0:
                    textBox1.Text += "执行中......\r\n";
                    break;
                case 1:
                    textBox1.Text += "写入成功!SN:" + param2 + "\r\n";
                    break;
                case 2:
                    textBox1.Text += "写入失败!原因:SN不一致!\r\n";
                    break;
                case 3:
                    textBox1.Text += "写入失败!原因:COM口未较准!\r\n";
                    break;
                case 4:
                    textBox1.Text += "写入成功!" + param2 + "\r\n";
                    break;
            }
            textBox1.ScrollToCaret();
            textBox1.Invalidate();
        }

        /// <summary>
        /// 关闭窗体时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestIMEIWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseIsOpenSerailPort();
        }
    }
}
