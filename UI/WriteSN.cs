﻿using HelloCSharp.Log;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HelloCSharp.UI
{
    public partial class WriteSN : Form
    {
        private int _overlayIndex = 0;
        private Dictionary<string, SerialPort> _portDictionary = new Dictionary<string, SerialPort>();
        private string[] _ports;
        private string _path = "TestComLog";
        private SerialPort _serialPort;
        private string _snNumber;
        private System.Timers.Timer _timer;

        public WriteSN()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
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
            //获取所有串口名
            _ports = SerialPort.GetPortNames();
            Array.Sort(_ports);
            //定时器
            _timer = new System.Timers.Timer(500);
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerEvent);
            _timer.AutoReset = true;
            //程序启动时需要判断是否有设备连接
            FirstRunConnState();
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
        private delegate void LabelDele(bool flag);

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
        /// 向设备写入SN
        /// </summary>
        private delegate void WriterDele();

        /// <summary>
        /// 询问是否覆盖SN
        /// </summary>
        /// <returns></returns>
        private bool AskIsOverlay()
        {
            DialogResult dr = MessageBox.Show("该设备已经存在SN,是否覆盖?", "警告", MessageBoxButtons.OKCancel);
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
                    ButtonStateChanged(false);
                    //一定要开启串口,因为每次写入完成都会关闭串口,尽量避免拨掉设备时抛出"资源占用"的异常
                    _serialPort.Close();
                    _serialPort.Open();
                    //验证是否符合写入条件(目前是查看返回的内容是否以0P和08结尾)
                    //_serialPort.Write("at+egmr=0,5\r\n");
                    _serialPort.Write("AT+QCSN?\r\n");
                    Thread.Sleep(1000);
                    ButtonStateChanged(true);
                }
                else
                {
                    MessageBox.Show("写入失败!\r\n请与管理员联系...", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ButtonStateChanged(true);
                }
            }
            catch (Exception ex)
            {
                _serialPort.Close();
                MessageBox.Show("写入失败!\r\n串口" + _serialPort.PortName + "正在使用中,请重新插入设备或重新程序!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// 检测设备连接状态
        /// </summary>
        /// <param name="flag"></param>
        private void CheckConnStartState(bool flag)
        {
            LabelDele labelDele = new LabelDele(LabelTextChanged);
            this.BeginInvoke(labelDele, new object[] { flag });
            ButtonDele buttonDele = new ButtonDele(ButtonStateChanged);
            this.BeginInvoke(buttonDele, new object[] { flag });
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
        /// 生成SN
        /// </summary>
        /// <param name="content"></param>
        private void CreateSNNumber(ref string content)
        {
            //公司名
            content = "MLA";
            //产品缩写
            string selectItem = comboBox1.SelectedItem.ToString();
            switch (selectItem)
            {
                case "T1-Lite-L":
                    content += "T1A";
                    break;
                case "T1-Lite-W":
                    content += "T1B";
                    break;
                case "T-L":
                    content += "T1C";
                    break;
                case "T-W":
                    content += "T1D";
                    break;
            }
            //年,4位
            content += DateTime.Now.Year.ToString();
            //月,1位
            if ("10".Equals(DateTime.Now.Month.ToString()))
            {
                content += "A";
            }
            else if ("11".Equals(DateTime.Now.Month.ToString()))
            {
                content += "B";
            }
            else if ("12".Equals(DateTime.Now.Month.ToString()))
            {
                content += "C";
            }
            else
            {
                content += DateTime.Now.Month.ToString();
            }
            //日,2位
            content += DateTime.Now.Day.ToString("D2");
            //顺序号,6位
            long currentTicks1 = DateTime.Now.Ticks;
            long currentTicks2 = (currentTicks1 - new DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks) / 10000;
            string tickStr = currentTicks2.ToString();
            tickStr = tickStr.Substring(tickStr.Length - 6);
            content += tickStr;
        }

        /// <summary>
        /// 接收写入命令后返回的内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceivedTestCom(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort tempSerialPort = (SerialPort)sender;
            string portName = tempSerialPort.PortName;
            //读取缓冲区所有字节
            string tempStr = tempSerialPort.ReadExisting();
            //向设备写入ate1命令后返回的内容
            if (tempStr.Contains("ate1") && tempStr.Contains("OK"))
            {
                //获取测试通过的串口
                _serialPort = tempSerialPort;
                //解绑串口之前的事件
                _serialPort.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedTestCom);
                //给串口绑定新的事件
                //_serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedWriter);
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedWriter_NotCheckSerialPort);
                LabelTextChangedByDele(true);
            }
        }

        /// <summary>
        /// 接收写入命令后返回的内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceivedWriter(object sender, SerialDataReceivedEventArgs e)
        {
            //阻塞该线程,以防上一次的数据没有读完
            Thread.Sleep(100);
            SerialPort tempSerialPort = (SerialPort)sender;
            string portName = tempSerialPort.PortName;
            //读取缓冲区所有字节
            string tempStr = tempSerialPort.ReadExisting();
            string snNumber = SubTwoStrContent(tempStr, "\"", "\"");
            if (tempStr.Contains("at+egmr=0,5") && tempStr.Contains("+EGMR:") || tempStr.Contains("at+egmr=0,5") && tempStr.Contains("ERROR"))
            {
                //验证设备是否符合写入条件(是否以0P和08结尾)
                if (snNumber.EndsWith("0P") || snNumber.EndsWith("08"))
                {
                    WriterDele writerDele = new WriterDele(QueryAndWriterSN);
                    BeginInvoke(writerDele);
                }
                //禁止写入
                else
                {
                    TextBoxChangedByDele(3, ref snNumber);
                    MessageBoxDele messageBoxDele = new MessageBoxDele(DeviceError);
                    BeginInvoke(messageBoxDele);
                    ButtonStateChangedByDele(true);
                    return;
                }
            }
            else
            {
                //查询设备的SN所返回的内容
                if (tempStr.IndexOf("AT+QCSN?") == 0 && tempStr.Contains("+QCSN:"))
                {
                    //截取出来的SN有内容就说明之前写过SN,不论写入SN的格式
                    if (snNumber != null && snNumber.Length > 0)
                    {
                        //询问是否覆盖再写入设备
                        MessageBoxDele messageBoxDele = new MessageBoxDele(AskIsOverlay);
                        if ((bool)this.Invoke(messageBoxDele))
                        {
                            WriterDele textDele = new WriterDele(WriterSN);
                            BeginInvoke(textDele);
                        }
                        //关闭所有串口,将控件上的SN置为空
                        else
                        {
                            ButtonStateChangedByDele(true);
                            CloseIsOpenSerialPortDele closePort = new CloseIsOpenSerialPortDele(CloseIsOpenSerailPort);
                            BeginInvoke(closePort);
                            _snNumber = null;
                            _overlayIndex = 0;
                        }
                    }
                    //直接写入设备
                    else
                    {
                        WriterDele textDele = new WriterDele(WriterSN);
                        BeginInvoke(textDele);
                    }
                }
                //向设备写入SN后返回的内容
                else if (tempStr.IndexOf("AT+QCSN=") == 0)
                {
                    _overlayIndex = ReadContentByLine(ref _snNumber);
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
                        _overlayIndex = 0;
                    }
                    ButtonStateChangedByDele(true);
                    //关闭所有串口,将控件上的SN置为空
                    CloseIsOpenSerialPortDele closePort = new CloseIsOpenSerialPortDele(CloseIsOpenSerailPort);
                    BeginInvoke(closePort);
                    _snNumber = null;
                    _overlayIndex = 0;
                }
            }
        }

        /// <summary>
        /// 不验证设备的串口参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceivedWriter_NotCheckSerialPort(object sender, SerialDataReceivedEventArgs e)
        {
            //阻塞该线程,以防上一次的数据没有读完
            Thread.Sleep(100);
            SerialPort tempSerialPort = (SerialPort)sender;
            string portName = tempSerialPort.PortName;
            //读取缓冲区所有字节
            string tempStr = tempSerialPort.ReadExisting();
            string snNumber = SubTwoStrContent(tempStr, "\"", "\"");
            //查询设备的SN所返回的内容
            if (tempStr.IndexOf("AT+QCSN?") == 0 && tempStr.Contains("+QCSN:") || tempStr.IndexOf("AT+QCSN?") == 0 && tempStr.Contains("ERROR"))
            {
                //截取出来的SN有内容就说明之前写过SN,不论写入SN的格式
                if (snNumber != null && snNumber.Length > 0)
                {
                    //询问是否覆盖再写入设备
                    MessageBoxDele messageBoxDele = new MessageBoxDele(AskIsOverlay);
                    if ((bool)this.Invoke(messageBoxDele))
                    {
                        WriterDele textDele = new WriterDele(WriterSN);
                        BeginInvoke(textDele);
                    }
                    //关闭所有串口,将控件上的SN置为空
                    else
                    {
                        ButtonStateChangedByDele(true);
                        CloseIsOpenSerialPortDele closePort = new CloseIsOpenSerialPortDele(CloseIsOpenSerailPort);
                        BeginInvoke(closePort);
                        _snNumber = null;
                        _overlayIndex = 0;
                    }
                }
                //直接写入设备
                else
                {
                    WriterDele textDele = new WriterDele(WriterSN);
                    BeginInvoke(textDele);
                }
            }
            //向设备写入SN后返回的内容
            else if (tempStr.IndexOf("AT+QCSN=") == 0)
            {
                _overlayIndex = ReadContentByLine(ref _snNumber);
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
                    _overlayIndex = 0;
                }
                ButtonStateChangedByDele(true);
                //关闭所有串口,将控件上的SN置为空
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
            _portDictionary = TestDevice(_ports);
            Thread.Sleep(100);
            if (_serialPort != null)
            {
                LabelTextChanged(true);
                ButtonStateChanged(true);
            }
            //设备断开时禁止写入SN
            else
            {
                LabelTextChanged(false);
                ButtonStateChanged(false);
            }
            //程序首次运行完毕后再启动定时器
            _timer.Start();
        }

        /// <summary>
        /// Label内容更新
        /// </summary>
        /// <param name="flag"></param>
        private void LabelTextChanged(bool flag)
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
        /// 标签内容更新
        /// </summary>
        /// <param name="flag"></param>
        private void LabelTextChangedByDele(bool flag)
        {
            //非UI线程访问该控件时
            if (label1.InvokeRequired)
            {
                label1.Invoke(new LabelDele(LabelTextChangedByDele), flag);
                return;
            }
            LabelTextChanged(flag);
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
                    if (_serialPort != null)
                    {
                        CheckConnStartState(true);
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
                    CheckConnStartState(false);
                }
                _ports = (string[])tempPorts.Clone();
            }
        }

        /// <summary>
        /// 覆盖设备原来在本地日志的SN
        /// </summary>
        /// <param name="index"></param>
        /// <param name="content"></param>
        private void OverlayWriterLocalLog(ref int index, ref string content)
        {
            try
            {
                string[] arrayLines = File.ReadAllLines(_path);
                arrayLines[index - 1] = content;
                File.WriteAllLines(_path, arrayLines);
            }
            catch (Exception ex)
            {
                MyLogger.Instance.WriteException(ex, "覆盖本地日志时发生IO异常");
            }
        }

        /// <summary>
        /// 查询设备的SN
        /// </summary>
        /// <param name="serialPort"></param>
        private void QueryAndWriterSN()
        {
            _serialPort.Write("AT+QCSN?\r\n");
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
                    serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedTestCom);
                    serialPort.Open();
                    serialPort.Write("ate1\r\n");
                    Thread.Sleep(100);
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
        /// 向设备写入SN
        /// </summary>
        private void WriterSN()
        {
            string content = "";
            CreateSNNumber(ref content);
            //发送命令之前留痕,用作SN写入成功返回时较验
            _snNumber = content;
            _serialPort.Write("AT+QCSN=\"" + content + "\"\r\n");
        }

        /// <summary>
        /// 将SN写入本地日志
        /// </summary>
        private void WriterLocalLog(string content)
        {
            FileStream fileStream = new FileStream(_path, FileMode.Append, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.WriteLine(content);
            streamWriter.Flush();
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
                    return index;
                }
            }
            streamReader.Close();
            fileStream.Close();
            return 0;
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

        /// <summary>
        /// 关闭窗体时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestCOMWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseIsOpenSerailPort();
        }

        private void 串口设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 数据库配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 日志路径ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

    }
}
