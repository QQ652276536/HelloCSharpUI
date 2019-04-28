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
        private string _paramFilePath = "E:\\TestIMEIConfig";
        bool _isFirstRunTimer = true;
        private int _overlayIndex = 0;
        private Dictionary<string, SerialPort> _portDictionary = new Dictionary<string, SerialPort>();
        private string[] _ports;
        private string _path = "E:\\TestIMEILog";
        private SerialPort _serialPort;
        private string _snNumber = "";
        private System.Timers.Timer _timer;

        public TestIMEIWindow()
        {
            InitializeComponent();
            //生成本地日志
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            _path += "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            FileStream fileStream = new FileStream(_path, FileMode.Append, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.Close();
            fileStream.Close();
            //生成外部文件的目录
            if (!Directory.Exists(_paramFilePath))
            {
                Directory.CreateDirectory(_paramFilePath);
            }
            _paramFilePath += "\\IMEIConfig(2).txt";
            //获取所有串口名
            _ports = SerialPort.GetPortNames();
            Array.Sort(_ports);
            //程序启动时需要判断是否有设备连接
            FirstRunConnState();
            //定时器
            _timer = new System.Timers.Timer(500);
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(CheckPorts);
            _timer.AutoReset = true;
            _timer.Start();
        }

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
        /// Label控件
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="content">Label2标签要显示的内容</param>
        private delegate void LabelDele(bool flag, string content = "");

        /// <summary>
        /// 弹窗
        /// </summary>
        private delegate bool MessageBoxDele();

        /// <summary>
        /// TextBox控件
        /// </summary>
        /// <param name="param">写入状态</param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        private delegate void TextBoxDele(int param, ref string param2, string param3 = null);

        /// <summary>
        /// 向设备写入
        /// </summary>
        private delegate void WriterDele();

        /// <summary>
        /// 询问是否覆盖
        /// </summary>
        /// <returns></returns>
        private bool AskIsOverlay()
        {
            DialogResult dr = MessageBox.Show("该设备XXX,是否覆盖?", "警告", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 测试串口是否通畅
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (_serialPort != null)
                {
                    _serialPort.Close();
                    //一定要开启串口,因为每次写入完成都会关闭串口,尽量避免拨掉设备时抛出"资源占用"的异常
                    _serialPort.Open();
                    //查询该设备写入
                    WriterIMEI();
                }
                else
                {
                    MessageBox.Show("写入失败!\r\n请与管理员联系...", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                _serialPort.Close();
                MessageBox.Show("写入失败!\r\n串口" + _serialPort.PortName + "正在使用中!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// 检测设备连接状态
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="content">Label2标签显示的内容</param>
        private void CheckDeviceState(bool flag, string content = "")
        {
            LabelDele labelDele = new LabelDele(LabelTextChanged);
            this.BeginInvoke(labelDele, new object[] { flag, content });
            ButtonDele buttonDele = new ButtonDele(ButtonStateChanged);
            this.BeginInvoke(buttonDele, new object[] { flag });
        }

        /// <summary>
        /// 定时检测串口是否有变动
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void CheckPorts(object source, System.Timers.ElapsedEventArgs e)
        {
            string[] tempPorts = SerialPort.GetPortNames();
            Array.Sort(tempPorts);
            if (_isFirstRunTimer)
            {
                //向设备写入AT+QCSN?命令
                WriterDele writerDele = new WriterDele(QueryAndWriterSN);
                BeginInvoke(writerDele);
                _isFirstRunTimer = false;
                return;
            }
            //串口数量有变动
            if (_ports.Length != tempPorts.Length)
            {
                //首先释放所有串口,尽量避免后面的设备访问不了串口
                CloseIsOpenSerialPortDele closePort = new CloseIsOpenSerialPortDele(CloseIsOpenSerailPort);
                Invoke(closePort);
                //有设备连接
                if (tempPorts.Length > _ports.Length)
                {
                    _portDictionary = TestDevice();
                    if (_serialPort != null)
                    {
                        CheckDeviceState(true, _snNumber);
                    }
                }
                //有设备断开
                else if (tempPorts.Length < _ports.Length)
                {
                    //关闭设备的串口
                    if (_serialPort != null)
                    {
                        _serialPort.Close();
                    }
                    //设备断开时禁止写入SN
                    CheckDeviceState(false, "");
                }
                //再释放一次所有串口,尽量避免后面的设备访问不了串口
                closePort = new CloseIsOpenSerialPortDele(CloseIsOpenSerailPort);
                Invoke(closePort);
                _ports = (string[])tempPorts.Clone();
            }
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
        /// 接收测试串口是否通畅返回的内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceivedTestCom(object sender, SerialDataReceivedEventArgs e)
        {
            //阻塞该线程,以防数据没有读完
            //Thread.Sleep(100);
            SerialPort tempSerialPort = (SerialPort)sender;
            string portName = tempSerialPort.PortName;
            //读取缓冲区所有字节
            string tempStr = tempSerialPort.ReadExisting();
            if (tempStr.Contains("") && tempStr.IndexOf("AT+QCSN?") == 0 && tempStr.Contains("+QCSN:"))
            {
                _snNumber = SubTwoStrContent(tempStr, "\"", "\"");
                LabelTextChangedByDele(true, _snNumber);
                //解绑串口之前的事件
                _serialPort.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedTestCom);
                //给串口绑定新的事件
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedWriterSN);
            }
            //向设备写入ate1命令后返回的内容
            else if (tempStr.Contains("OK"))
            {
                //获取测试通过的串口
                _serialPort = tempSerialPort;
                LabelTextChangedByDele(true);
                if (!_isFirstRunTimer)
                {
                    WriterDele writerDele = new WriterDele(QueryAndWriterSN);
                    BeginInvoke(writerDele);
                }
            }
        }

        /// <summary>
        /// 接收写入SN所返回的内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceivedWriterSN(object sender, SerialDataReceivedEventArgs e)
        {
            //阻塞该线程,以防数据没有读完
            Thread.Sleep(100);
            SerialPort tempSerialPort = (SerialPort)sender;
            string portName = tempSerialPort.PortName;
            //读取缓冲区所有字节
            string tempStr = tempSerialPort.ReadExisting();
            string snNumber = SubTwoStrContent(tempStr, "\"", "\"");
            //查询设备的SN
            if (tempStr.IndexOf("AT+QCSN?") == 0 && tempStr.Contains("+QCSN:"))
            {
                //_overlayIndex = ReadContentByLine(ref snNumber);
                ////该设备已经写过
                //if (_overlayIndex > 0)
                //{
                //    //询问是否覆盖再写入设备
                //    MessageBoxDele messageBoxDele = new MessageBoxDele(AskIsOverlay);
                //    if ((bool)this.Invoke(messageBoxDele))
                //    {
                //        WriterDele textDele = new WriterDele(WriterIMEI);
                //        BeginInvoke(textDele);
                //    }
                //}
                ////直接写入设备
                //else
                {
                    WriterDele textDele = new WriterDele(WriterIMEI);
                    BeginInvoke(textDele);
                }
            }
            //向设备写入SN后返回的内容
            else if (tempStr.IndexOf("AT+QCSN=") == 0)
            {
                //较验一致
                if (snNumber.Equals(_snNumber))
                {
                    //覆盖本地日志
                    if (_overlayIndex > 0)
                    {
                        OverlayWriterLocalLog(ref _overlayIndex, ref _snNumber);
                    }
                    //写入本地日志
                    else
                    {
                        WriterLocalLog(snNumber);
                    }
                    TextBoxChangedByDele(1, ref snNumber);
                }
                //较验不一致
                else
                {
                    TextBoxChangedByDele(2, ref snNumber);
                }
                CloseIsOpenSerialPortDele closePort = new CloseIsOpenSerialPortDele(CloseIsOpenSerailPort);
                BeginInvoke(closePort);
                _snNumber = null;
                _overlayIndex = 0;
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
            _portDictionary = TestDevice();
            Thread.Sleep(100);
            if (_serialPort != null)
            {
                LabelTextChanged(true, _snNumber);
                ButtonStateChanged(true);
            }
            //设备断开时禁止写入SN
            else
            {
                LabelTextChanged(false, "");
                ButtonStateChanged(false);
            }
        }

        /// <summary>
        /// Label内容更新
        /// </summary>
        /// <param name="flag"></param>
        private void LabelTextChanged(bool flag, string content = "")
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
            label2.Text = content;
            label2.Invalidate();
        }

        /// <summary>
        /// 标签内容更新
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="content"></param>
        private void LabelTextChangedByDele(bool flag, string content = "")
        {
            //非UI线程访问该控件时
            if (label1.InvokeRequired)
            {
                label1.Invoke(new LabelDele(LabelTextChangedByDele), flag, content);
                return;
            }
            else if (label2.InvokeRequired)
            {
                label2.Invoke(new LabelDele(LabelTextChangedByDele), flag, content);
            }
            LabelTextChanged(flag, content);
        }

        /// <summary>
        /// 覆盖设备原来在本地日志的SN
        /// </summary>
        /// <param name="content"></param>
        private void OverlayWriterLocalLog(ref int index, ref string content)
        {
            string[] arrayLines = File.ReadAllLines(_path);
            arrayLines[index - 1] = content;
            File.WriteAllLines(_path, arrayLines);
        }

        /// <summary>
        /// 查询设备的SN
        /// </summary>
        /// <param name="serialPort"></param>
        private void QueryAndWriterSN()
        {
            if (_serialPort == null)
            {
                return;
            }
            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
            }
            //PS:查询完SN后记得解绑这个事件
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedTestCom);
            _serialPort.Write("AT+QCSN?\r\n");
        }

        /// <summary>
        /// 测试串口是否通畅,写入的是串口总是返回查询内容的ATE1命令
        /// </summary>
        private Dictionary<string, SerialPort> TestDevice()
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
                    serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedTestCom);
                    serialPort.Open();
                    serialPort.Write("ate1\r\n");
                    Thread.Sleep(200);
                    serialPort.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedTestCom);
                    serialPort.Close();
                }
                catch (Exception e)
                {
                    serialPort.Close();
                    int error = -1;
                }
            }
            return dictionary;
        }

        /// <summary>
        /// 向设备写入
        /// 为避免重复写入,对外部文件进行读一行删一行
        /// </summary>
        private void WriterIMEI()
        {
            try
            {
                string[] arrayLines = File.ReadAllLines(_paramFilePath, Encoding.UTF8);
                string[] tempContents = arrayLines[0].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                string imei1 = "";
                string imei2 = "";
                string wifimac = "";
                string btmac = "";
                //写入设备
                //IMEI1
                if (tempContents.Length > 0 && checkBox1.Checked)
                {
                    imei1 = tempContents[0];
                    _serialPort.Write("at+egmr=1,7,\"" + imei1 + "\"\r\n");
                }
                //IMEI2
                if (tempContents.Length > 1 && checkBox2.Checked)
                {
                    imei2 = tempContents[1];
                    _serialPort.Write("at+egmr=1,10,\"" + imei2 + "\"\r\n");
                }
                //WIFI-MAC
                if (tempContents.Length > 2 && checkBox3.Checked)
                {
                    wifimac = tempContents[2];
                    wifimac = Regex.Replace(wifimac, ":", "");
                    _serialPort.Write("at+qnvw=4678,0,\"" + wifimac + "\"\r\n");
                }
                //BT-MAC
                if (tempContents.Length > 3 && checkBox4.Checked)
                {
                    btmac = tempContents[3];
                    btmac = Regex.Replace(btmac, ":", "");
                    _serialPort.Write("at+qnvw=447,0,\"" + btmac + "\"\r\n");
                }
                //写入本地日志
                string content = _snNumber + " " + imei1 + " " + imei2 + " " + wifimac + " " + btmac;
                WriterLocalLog(content);
                //清空该行
                arrayLines[0] = "\r\n";
                File.WriteAllLines(_path, arrayLines, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                int error = -1;
            }
        }

        /// <summary>
        /// 写入本地日志
        /// </summary>
        private void WriterLocalLog(string content)
        {
            FileStream fileStream = new FileStream(_path, FileMode.Append, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.WriteLine(content, Encoding.UTF8);
            streamWriter.Close();
            fileStream.Close();
        }

        /// <summary>
        /// 判断SN在文本中是否存在
        /// </summary>
        /// <param name="snNumber"></param>
        /// <returns></returns>
        private int ReadContentByLine(ref string snNumber)
        {
            FileStream fileStream = new FileStream(_path, FileMode.Open, FileAccess.Read);
            StreamReader streamReader = new StreamReader(fileStream);
            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            string strLine;
            int index = 0;
            while ((strLine = streamReader.ReadLine()) != null)
            {
                index++;
                if (strLine.Equals(snNumber))
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
            catch (Exception e)
            {
                int error = -1;
            }
            return result;
        }

        /// <summary>
        /// 文本框内容更新
        /// </summary>
        /// <param name="param"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        private void TextBoxChangedByDele(int param, ref string param2, string param3 = null)
        {
            //非UI线程访问该控件时
            if (textBox1.InvokeRequired)
            {
                //使用委托发出调用
                textBox1.Invoke(new TextBoxDele(TextBoxChangedByDele), param, param2, param3);
                return;
            }
            //使用委托发出的TextBoxChanged方法调用会跳到这来执行：在textBox1中显示
            switch (param)
            {
                case 0:
                    textBox1.Text += "执行中......\r\n";
                    textBox1.Select(textBox1.Text.Length, 0);
                    textBox1.ScrollToCaret();
                    break;
                case 1:
                    textBox1.Text += "写入成功!SN:" + param2 + "\r\n";
                    textBox1.ForeColor = Color.Green;
                    textBox1.Select(textBox1.Text.Length, 0);
                    textBox1.ScrollToCaret();
                    break;
                case 2:
                    textBox1.Text += "写入失败!原因:SN不一致!\r\n";
                    textBox1.ForeColor = Color.Red;
                    textBox1.Select(textBox1.Text.Length, 0);
                    textBox1.ScrollToCaret();
                    break;
                case 3:
                    textBox1.Text += "写入失败!原因:COM口未较准!\r\n";
                    textBox1.ForeColor = Color.Red;
                    textBox1.Select(textBox1.Text.Length, 0);
                    textBox1.ScrollToCaret();
                    break;
            }
            textBox1.Invalidate();
        }

    }
}
