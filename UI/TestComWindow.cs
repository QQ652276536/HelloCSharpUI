using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestCOM
{
    public partial class TestComForm : Form
    {
        private int _number = 0;
        private Dictionary<string, SerialPort> _portDirectory;
        private string[] _ports;
        private string _path = "E:\\TestComLog";
        private string _snNumber;
        private string[] _timerPorts;
        private System.Timers.Timer _timer;

        /// <summary>
        /// 在非UI线程对控件进行操作的委托
        /// </summary>
        /// <param name="param"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        private delegate void ShowDataDelegate(int param, ref string param2, string param3 = null);

        /// <summary>
        /// 向设备写入SN号码的委托
        /// </summary>
        /// <param name="serialPort"></param>
        private delegate void WriterDele(ref SerialPort serialPort);

        public TestComForm()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            _portDirectory = new Dictionary<string, SerialPort>();
            _path += "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            //获取所有串口名
            _ports = SerialPort.GetPortNames();
            //实例化Timer类,设置间隔时间为毫秒
            _timer = new System.Timers.Timer(500);
            //到达时间的时候执行事件
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(CheckPorts);
            //设置一直执行
            _timer.AutoReset = true;
        }

        private void CheckPorts(object source, System.Timers.ElapsedEventArgs e)
        {
            _timerPorts = SerialPort.GetPortNames();
            if (_ports.Length != _timerPorts.Length)
            {
                _ports = (string[])_timerPorts.Clone();
                Array.Sort(_ports);
                //如果只有两个串口时总是会报"端口被占用"的异常,这里先用这个LowFunc回避一下,以后弄懂了再解决吧
                if (_timerPorts.Length > 2)
                {
                    FirstWriterSN();
                }
            }
        }

        /// <summary>
        /// 开始检测是否有设备连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("开始"))
            {
                button1.Text = "停止";
                _timer.Start();
            }
            else
            {
                button1.Text = "开始";
                _timer.Stop();
            }
        }

        /// <summary>
        /// 开始写入SN号码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            FirstWriterSN();
        }

        /// <summary>
        /// 向串口发送一条查询SN号码的命令
        /// </summary>
        /// <param name="serialPort"></param>
        private void CheckIsExsit(ref SerialPort serialPort)
        {
            //清理残余的缓冲区
            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
            serialPort.Encoding = Encoding.ASCII;
            //发送数据
            serialPort.Write("AT+QCSN?\r\n");
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
            //SN查询命令的返回内容
            if (tempStr.Contains("AT+QCSN?"))
            {
                string snNumber = SubTwoStrContent(tempStr, "\"", "\"");
                bool tempFlag = ReadContentByLine(ref snNumber);
                if (tempFlag)
                {
                    string tempParam = "";
                    TextBoxChanged(3, ref tempParam);
                    //确认覆盖
                    if ("Exist".Equals(tempParam))
                    {
                        WriterSN(tempSerialPort);
                    }
                }
            }
            //写入成功后的返回内容
            else if (tempStr.Contains("MLA"))
            {
                string snNumber = SubTwoStrContent(tempStr, "\"", "\"");
                if (snNumber.Equals(_snNumber))
                {
                    string tempParam = "";
                    TextBoxChanged(1, ref tempParam, snNumber);
                    //写入成功则关闭所有串口,防止下次串口访问失败
                    Dictionary<string, SerialPort>.ValueCollection values = _portDirectory.Values;
                    foreach (SerialPort port in values)
                    {
                        port.Close();
                    }
                    _portDirectory.Clear();
                }
                else
                {
                    string tempParam = "";
                    TextBoxChanged(2, ref tempParam);
                }
            }
            //测试串口是否通畅的返回内容
            else if (tempStr.Contains("OK"))
            {
                //写入之前先通过命令查询串口返回的SN号码,并和本地文本作比较,如果相同则提示是否覆盖
                CheckIsExsit(ref tempSerialPort);
            }
        }

        /// <summary>
        /// 写入的是串口总是返回查询内容的ATE1命令,这里用作测试串口是否通畅
        /// </summary>
        private void FirstWriterSN()
        {
            foreach (String portName in _ports)
            {
                //TODO:波特率暂时写死
                SerialPort serialPort = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
                try
                {
                    if (!portName.Equals("COM1"))
                    {
                        _portDirectory.Add(portName, serialPort);
                        serialPort.RtsEnable = true;
                        serialPort.DtrEnable = true;
                        serialPort.Handshake = Handshake.None;
                        serialPort.ReceivedBytesThreshold = 1;
                        serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                        serialPort.Open();
                        //清理残余的缓冲区
                        serialPort.DiscardInBuffer();
                        serialPort.DiscardOutBuffer();
                        serialPort.Encoding = Encoding.ASCII;
                        //发送数据,用于测试是否连通
                        serialPort.Write("ate1\r\n");
                        Thread.Sleep(100);
                    }
                }
                //超时处理
                catch (Exception ex)
                {
                    _portDirectory.Remove(portName);
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
        /// 判断写入的SN在文本中是否存在
        /// </summary>
        /// <returns></returns>
        private bool ReadContentByLine(ref string snNumber)
        {
            FileStream fs = new FileStream(_path, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            sr.BaseStream.Seek(0, SeekOrigin.Begin);
            string strLine;
            while ((strLine = sr.ReadLine()) != null)
            {
                if (strLine.Equals(snNumber))
                {
                    return true;
                }
            }
            fs.Close();
            sr.Close();
            return false;
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
        /// 以委托的方式
        /// </summary>
        /// <param name="param"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        private void TextBoxChanged(int param, ref string param2, string param3 = null)
        {
            // 当跨线程刷新UI界面时
            if (textBox1.InvokeRequired)
            {
                // 使用委托发出ShowData调用
                textBox1.Invoke(new ShowDataDelegate(TextBoxChanged), param, param2, param3);
                return;
            }
            // 使用委托发出的TextBoxChanged方法调用会跳到这来执行：在textBox1中显示
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
