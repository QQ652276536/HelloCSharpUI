using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

/*串口开启状态下拨出设备,设备的连接状态无法正确响应,且再插入设备会抛出"资源占用"的异常*/
namespace TestCOM
{
    public partial class TestComWindow : Form
    {
        private int _number = 0;
        private Dictionary<string, SerialPort> _portDictionary;
        private string[] _ports;
        private string _path = "E:\\TestComLog";
        private SerialPort _serialPort;
        private string _snNumber;
        private string[] _timerPorts;
        private System.Timers.Timer _timer;

        public TestComWindow()
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
            _portDictionary = new Dictionary<string, SerialPort>();
            //获取所有串口名
            _ports = SerialPort.GetPortNames();
            Array.Sort(_ports);
            //实例化Timer类,设置间隔时间为毫秒
            _timer = new System.Timers.Timer(100);
            //到达时间的时候执行事件
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(CheckPorts);
            //设置启动后是否一直执行
            _timer.AutoReset = true;
            //程序启动即执行
            _timer.Start();
            //程序启动时需要判断是否有设备连接
            FirstRunConnState();
        }

        /// <summary>
        /// Button控件的委托
        /// </summary>
        /// <param name="index">从1开始</param>
        /// <param name="flag"></param>
        private delegate void ButtonDele(int index, bool flag, string txt = null);

        /// <summary>
        /// Label控件的委托
        /// </summary>
        /// <param name="index">从1开始</param>
        /// <param name="flag"></param>
        private delegate void LabelDele(int index, bool flag);

        /// <summary>
        /// TextBox控件的委托
        /// </summary>
        /// <param name="param">写入状态</param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        private delegate void TextBoxDele(int param, ref string param2, string param3 = null);

        /// <summary>
        /// 向设备写入SN号码的委托
        /// </summary>
        /// <param name="serialPort"></param>
        private delegate void WriterDele(ref SerialPort serialPort);

        /// <summary>
        /// 定时检测串口是否有变动
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void CheckPorts(object source, System.Timers.ElapsedEventArgs e)
        {
            _timerPorts = SerialPort.GetPortNames();
            Array.Sort(_timerPorts);
            if (!CompareComNameArray(_ports, _timerPorts))
            {
                //有新设备连接
                if (_timerPorts.Length > 2)
                {
                    CheckConnAndStartState(1, true);
                }
                //设备断开连接
                else
                {
                    //设备断开时禁止打开串口和写入SN
                    CheckConnAndStartState(1, false);
                    CheckConnAndStartState(2, false);
                    //设备断开时清除所有串口相关的数据
                    Dictionary<string, SerialPort>.ValueCollection values = _portDictionary.Values;
                    foreach (SerialPort tempPort in values)
                    {
                        tempPort.Close();
                    }
                    _serialPort = null;
                }
                //更新当前串口名数组
                _ports = (string[])_timerPorts.Clone();
            }
        }

        /// <summary>
        /// 测试串口是否通畅
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("开启串口"))
            {
                //串口开启成功才改变按钮状态
                TestSN();
            }
            //关闭所有串口
            else
            {
                Dictionary<string, SerialPort>.ValueCollection values = _portDictionary.Values;
                foreach (SerialPort tempPort in values)
                {
                    tempPort.Close();
                }
                _portDictionary.Clear();
                _serialPort = null;
                ButtonStateChanged(1, true, "开启串口");
                LabelTextChanged(2, false);
            }
        }

        /// <summary>
        /// 开始写入SN号码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (_serialPort != null)
            {
                //写入之前先通过命令查询串口返回的SN号码,并和本地文本作比较,如果相同则提示是否覆盖
                QuerySN(ref _serialPort);
            }
            else
            {
                MessageBox.Show("写入失败!请与管理员联系...", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 按钮状态更新
        /// </summary>
        /// <param name="index">从1开始</param>
        /// <param name="flag"></param>
        /// <param name="txt"></param>
        private void ButtonStateChanged(int index, bool flag, string txt = null)
        {
            switch (index)
            {
                case 1:
                    {
                        button1.Enabled = flag;
                        if (txt != null)
                        {
                            button1.Text = txt;
                        }
                        button1.Invalidate();
                        break;
                    }
                case 2:
                    {
                        button2.Enabled = flag;
                        if (txt != null)
                        {
                            button2.Text = txt;
                        }
                        button2.Invalidate();
                        break;
                    }
            }
        }

        /// <summary>
        /// 按钮状态的更新
        /// <param name="index">从1开始</param>
        /// <param name="flag"></param>
        private void ButtonStateChangedByDele(int index, bool flag, string txt = null)
        {
            //非UI线程访问该控件时
            if (button1.InvokeRequired)
            {
                button1.Invoke(new ButtonDele(ButtonStateChangedByDele), index, flag, txt);
                return;
            }
            else if (button2.InvokeRequired)
            {
                button2.Invoke(new ButtonDele(ButtonStateChangedByDele), index, flag, txt);
                return;
            }
            ButtonStateChanged(index, flag, txt);
        }

        /// <summary>
        /// 检测设备连接状态和串口开启状态
        /// </summary>
        /// <param name="index">从1开始</param>
        /// <param name="flag"></param>
        private void CheckConnAndStartState(int index, bool flag)
        {
            LabelDele labelDele = new LabelDele(LabelTextChanged);
            switch (index)
            {
                case 1:
                    this.BeginInvoke(labelDele, new object[] { 1, flag });
                    break;
                case 2:
                    this.BeginInvoke(labelDele, new object[] { 2, flag });
                    break;
            }
        }

        /// <summary>
        /// 向串口发送一条查询SN号码的命令
        /// </summary>
        /// <param name="serialPort"></param>
        private void CheckSNIsExsit(ref SerialPort serialPort)
        {
            //发送数据
            serialPort.Write("AT+QCSN?\r\n");
        }

        /// <summary>
        /// 比较串品名数组内的元素是否一致
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
        /// 接收查询设备SN号码返回的内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceivedQueryCom(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort tempSerialPort = (SerialPort)sender;
            string portName = tempSerialPort.PortName;
            //读取缓冲区所有字节
            string tempStr = tempSerialPort.ReadExisting();
            if (tempStr.Contains("AT+QCSN?"))
            {
                string snNumber = SubTwoStrContent(tempStr, "\"", "\"");
                int index = ReadContentByLine(ref snNumber);
                if (index > 0)
                {
                    string tempParam = "";
                    TextBoxChangedByDele(3, ref tempParam);
                    //确认覆盖
                    if ("Exist".Equals(tempParam))
                    {
                        WriterSN(tempSerialPort);
                    }
                }
            }
        }

        /// <summary>
        /// 接收测试串口是否通畅返回的内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceivedTestCom(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort tempSerialPort = (SerialPort)sender;
            string portName = tempSerialPort.PortName;
            //读取缓冲区所有字节
            string tempStr = tempSerialPort.ReadExisting();
            if (tempStr.Contains("OK"))
            {
                _serialPort = tempSerialPort;
                //如果串口是通的则改变按钮状态
                ButtonStateChangedByDele(1, true, "关闭串口");
                //如果串口是通的则允许执行写入SN操作
                ButtonStateChangedByDele(2, true);
                LabelTextChangedByDele(2, true);
            }
        }

        /// <summary>
        /// 接收串口返回的内容
        /// 辅助线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort tempSerialPort = (SerialPort)sender;
            string portName = tempSerialPort.PortName;
            //读取缓冲区所有字节
            string tempStr = tempSerialPort.ReadExisting();
            //查询SN号码返回的内容
            if (tempStr.Contains("AT+QCSN?"))
            {
            }
            //写入成功后的返回内容
            else if (tempStr.Contains("MLA"))
            {
                string snNumber = SubTwoStrContent(tempStr, "\"", "\"");
                if (snNumber.Equals(_snNumber))
                {
                    string tempParam = "";
                    TextBoxChangedByDele(1, ref tempParam, snNumber);
                    //写入成功则关闭所有串口,防止下次串口访问失败
                    Dictionary<string, SerialPort>.ValueCollection values = _portDictionary.Values;
                    foreach (SerialPort port in values)
                    {
                        port.Close();
                    }
                    _portDictionary.Clear();
                }
                else
                {
                    string tempParam = "";
                    TextBoxChangedByDele(2, ref tempParam);
                }
            }
            //测试串口是否通畅的返回内容
            else if (tempStr.Contains("OK"))
            {
                //写入之前先通过命令查询串口返回的SN号码,并和本地文本作比较,如果相同则提示是否覆盖
                CheckSNIsExsit(ref tempSerialPort);
            }
        }

        /// <summary>
        /// 程序启动时设备的连接状态
        /// </summary>
        private void FirstRunConnState()
        {
            //如果串口数在2个以上则表示有设备连接,就先这样判断吧
            if (_ports.Contains("COM1") && _ports.Length > 2)
            {
                LabelTextChanged(1, true);
            }
            //设备断开时禁止开启串口和写入SN
            else
            {
                LabelTextChanged(1, false);
                LabelTextChanged(2, false);
            }
        }

        /// <summary>
        /// Label内容更新
        /// </summary>
        /// <param name="flag"></param>
        private void LabelTextChanged(int index, bool flag)
        {
            switch (index)
            {
                //设备是否连接
                case 1:
                    {
                        if
                            (flag)
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
                        break;
                    }
                //串口是否开启
                case 2:
                    {
                        if (flag)
                        {
                            label2.Text = "串口已开启";
                            label2.ForeColor = Color.Green;
                        }
                        else
                        {
                            label2.Text = "串口已关闭";
                            label2.ForeColor = Color.Red;
                        }
                        label2.Invalidate();
                        break;
                    }
            }
        }

        /// <summary>
        /// 标签内容更新
        /// </summary>
        /// <param name="index">从1开始</param>
        /// <param name="flag"></param>
        private void LabelTextChangedByDele(int index, bool flag)
        {
            //非UI线程访问该控件时
            if (label1.InvokeRequired)
            {
                label1.Invoke(new LabelDele(LabelTextChangedByDele), index, flag);
                return;
            }
            else if (label2.InvokeRequired)
            {
                label2.Invoke(new LabelDele(LabelTextChangedByDele), index, flag);
                return;
            }
            LabelTextChanged(index, flag);
        }

        /// <summary>
        /// 查询设备的SN号码
        /// </summary>
        /// <param name="serialPort"></param>
        private void QuerySN(ref SerialPort serialPort)
        {
            serialPort.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedTestCom);
            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedQueryCom);
            serialPort.Write("AT+QCSN?\r\n");
        }

        /// <summary>
        /// 测试串口是否通畅,写入的是串口总是返回查询内容的ATE1命令
        /// </summary>
        private void TestSN()
        {
            foreach (string portName in _ports)
            {
                try
                {
                    //TODO:波特率暂时写死
                    SerialPort serialPort = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
                    if (!portName.Equals("COM1"))
                    {
                        _portDictionary.Add(portName, serialPort);
                        serialPort.RtsEnable = true;
                        serialPort.DtrEnable = true;
                        serialPort.Handshake = Handshake.None;
                        serialPort.ReceivedBytesThreshold = 1;
                        serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedTestCom);
                        serialPort.Open();
                        serialPort.Write("ate1\r\n");
                    }
                }
                catch (Exception e)
                {
                    int error = -1;
                }
            }
        }

        /// <summary>
        /// 向设备写入SN号码
        /// </summary>
        /// <param name="param"></param>
        private void WriterSN(object param)
        {
            WriterDele textDele = new WriterDele(WriterSN);
            this.BeginInvoke(textDele, new Object[] { param as SerialPort });
        }

        /// <summary>
        /// 向设备写入SN号码
        /// </summary>
        /// <param name="serialPort"></param>
        private void WriterSN(ref SerialPort serialPort)
        {
            string content;
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
            tickStr = tickStr.Substring(tickStr.Length - 3);
            tickStr += (++_number).ToString("D3");
            content += tickStr;
            //清理残余的缓冲区
            //serialPort.DiscardInBuffer();
            //serialPort.DiscardOutBuffer();
            serialPort.Encoding = Encoding.ASCII;
            //发送命令之前留痕,用作串口返回时的较验
            _snNumber = content;
            //发送写入命令
            serialPort.Write("AT+QCSN=\"" + content + "\"\r\n");
            //写入本地文本
            FileStream fileStream = new FileStream(_path, FileMode.Append, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.WriteLine(content);
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
            FileStream fs = new FileStream(_path, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            sr.BaseStream.Seek(0, SeekOrigin.Begin);
            string strLine;
            int index = 0;
            while ((strLine = sr.ReadLine()) != null)
            {
                index++;
                if (strLine.Equals(snNumber))
                {
                    return index;
                }
            }
            fs.Close();
            sr.Close();
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
                    textBox1.Text += "写入成功!SN号码:" + param2 + "\r\n";
                    textBox1.Select(textBox1.Text.Length, 0);
                    textBox1.ScrollToCaret();
                    break;
                case 2:
                    textBox1.Text += "写入失败!\r\n原因:SN号码不一致!\r\n";
                    textBox1.Select(textBox1.Text.Length, 0);
                    textBox1.ScrollToCaret();
                    MessageBox.Show("写入失败!\r\n原因:SN号码不一致!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case 3:
                    DialogResult dr = MessageBox.Show("该设备已经存在SN号码,是否覆盖?", "警告", MessageBoxButtons.OKCancel);
                    if (dr == DialogResult.OK)
                    {
                        param2 = "Exist";
                    }
                    else if (dr == DialogResult.Cancel)
                    {
                        param2 = "NoExist";
                    }
                    break;
            }
            textBox1.Invalidate();
        }

    }
}
