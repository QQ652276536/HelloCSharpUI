using HelloCSharp.Log;
using HelloCSharp.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloCSharp.UI
{
    public partial class MH1902 : Form
    {
        private readonly int[] BAUDRATE_ARRAY = new int[] { 115200, 57600, 56000, 38400, 19200, 9600, 4800, 2400, 1200 };
        private readonly int[] DATABIT_ARRAY = new int[] { 8, 7, 6, 5, 4 };
        private readonly byte[] STEP1 = MyConvertUtil.StringToBytes("7F");
        private readonly byte[] STEP2 = MyConvertUtil.StringToBytes("7C");

        private SerialPort _serialPort;
        private string _secretKeyPath = "", _updatePath = "", _portName = "";
        private string[] _portNameArray;
        private int _baudRate = 0, _dataBit = 8, _step = 1;
        private Thread _step1Thread, _step2Thread;
        private MyLogger _logger = MyLogger.Instance;

        private delegate void RichTextBoxDele(string str);

        public MH1902()
        {
            InitializeComponent();
            InitData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        private void RichTextBoxChangedByDele(string str)
        {
            //非UI线程访问控件时
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new RichTextBoxDele(RichTextBoxChangedByDele), str);
            }
            else
            {
                switch (_step)
                {
                    case 1:
                        _step = 2;
                        _step1Thread.Abort();
                        richTextBox1.AppendText("【Step1】收到：" + str + "\r\n");
                        richTextBox1.AppendText("【Step2】正在持续发送7C..." + "\r\n");
                        _step2Thread = new Thread(Step2);
                        _step2Thread.Start();
                        break;
                    case 2:
                        _step2Thread.Abort();
                        richTextBox1.AppendText("【Step2】收到：" + str + "\r\n");
                        break;
                    case 3:
                        richTextBox1.AppendText("【Step3】收到：" + str + "\r\n");
                        break;
                    case 4:
                        richTextBox1.AppendText("【Step4】收到：" + str + "\r\n");
                        break;
                    case 5:
                        richTextBox1.AppendText("【Step5】收到：" + str + "\r\n");
                        break;
                }
            }
        }

        /// <summary>
        /// 异步接收Com返回的内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceivedComData(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort tempSerialPort = (SerialPort)sender;
            if (tempSerialPort == null || !tempSerialPort.IsOpen)
            {
                return;
            }
            //读取缓冲区所有字节
            int len = tempSerialPort.BytesToRead;
            byte[] byteArray = new byte[len];
            tempSerialPort.Read(byteArray, 0, byteArray.Length);
            string str = MyConvertUtil.BytesToString(byteArray);
            if (string.IsNullOrEmpty(str))
            {
                return;
            }
            _logger.WriteLog("收到的数据（Hex）：" + str);
            Console.WriteLine("收到的数据（Hex）：" + str);
            RichTextBoxChangedByDele(str);
        }

        /// <summary>
        /// 建立连接过程的数据包，向芯片端持续发送7C
        /// </summary>
        private void Step2()
        {
            while (true && _serialPort.IsOpen)
            {
                _serialPort.Write(STEP2, 0, STEP2.Length);
            }
        }

        /// <summary>
        /// 建立连接过程的数据包，向芯片端持续发送7F表示需要下载
        /// </summary>
        private void Step1()
        {
            while (true && _serialPort.IsOpen)
            {
                _serialPort.Write(STEP1, 0, STEP1.Length);
            }
        }

        /// <summary>
        /// 初始化控件需要的数据
        /// </summary>
        private void InitData()
        {
            //串口Combox赋值
            _portNameArray = SerialPort.GetPortNames();
            DataTable dataTable1 = new DataTable();
            dataTable1.Columns.Add("value");
            foreach (string temp in _portNameArray)
            {
                DataRow dataRow = dataTable1.NewRow();
                dataRow[0] = temp;
                dataTable1.Rows.Add(dataRow);
            }
            if (dataTable1.Rows.Count > 0)
            {
                comboBox1.DataSource = dataTable1;
                comboBox1.ValueMember = "value";
            }
            DataTable dataTable2 = new DataTable();
            dataTable2.Columns.Add("value");
            foreach (int temp in BAUDRATE_ARRAY)
            {
                DataRow dataRow = dataTable2.NewRow();
                dataRow[0] = temp;
                dataTable2.Rows.Add(dataRow);
            }
            if (dataTable2.Rows.Count > 0)
            {
                comboBox2.DataSource = dataTable2;
                comboBox2.ValueMember = "value";
            }
            DataTable dataTable3 = new DataTable();
            dataTable3.Columns.Add("value");
            foreach (int temp in DATABIT_ARRAY)
            {
                DataRow dataRow = dataTable3.NewRow();
                dataRow[0] = temp;
                dataTable3.Rows.Add(dataRow);
            }
            if (dataTable3.Rows.Count > 0)
            {
                comboBox3.DataSource = dataTable3;
                comboBox3.ValueMember = "value";
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //是否允许选择多个文件
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "请选择密钥文件";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //返回文件的完整路径
                _secretKeyPath = openFileDialog.FileName;
                textBox5.Text = _secretKeyPath;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
        }

        private void button6_Click(object sender, EventArgs e)
        {
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //重置Step
            _step = 1;
            if (_step1Thread != null)
                _step1Thread.Abort();
            if (_step2Thread != null)
                _step2Thread.Abort();
            if (_serialPort != null)
                _serialPort.Close();
            richTextBox1.AppendText("已关闭：" + _portName + "\r\n");
            button3.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _portName = comboBox1.SelectedValue as string;
            _baudRate = Convert.ToInt32(comboBox2.SelectedValue as string);
            _dataBit = Convert.ToInt32(comboBox3.SelectedValue as string);
            _serialPort = new SerialPort(_portName, _baudRate, Parity.None, _dataBit, StopBits.One);
            try
            {
                if (!_serialPort.IsOpen)
                {
                    button3.Enabled = false;
                    richTextBox1.AppendText("已打开：" + _portName + "\r\n");
                    _serialPort.DataReceived += new SerialDataReceivedEventHandler(ReceivedComData);
                    _serialPort.Open();
                    richTextBox1.AppendText("正在连接..." + "\r\n");
                    _step = 1;
                    richTextBox1.AppendText("【Step1】正在持续发送7F..." + "\r\n");
                    _step1Thread = new Thread(Step1);
                    _step1Thread.Start();
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog(ex.ToString());
                Console.WriteLine(ex.ToString());
                string content = richTextBox1.Text.ToString();
                string exStr = ex.ToString();
                richTextBox1.AppendText(exStr + "\r\n");
                richTextBox1.Select(content.Length, exStr.Length);
                richTextBox1.SelectionColor = Color.Red;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //是否允许选择多个文件
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "请选择升级文件";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //返回文件的完整路径
                _updatePath = openFileDialog.FileName;
                textBox6.Text = _updatePath;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
