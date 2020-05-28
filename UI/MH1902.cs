using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloCSharp.UI
{
    public partial class MH1902 : Form
    {
        private SerialPort _serialPort;
        private string _secretKeyPath = "", _updatePath = "", _portName = "";
        private string[] _portNameArray;
        private int _baudRate = 0, _dataBit = 8;
        private readonly int[] BAUDRATE_ARRAY = new int[] { 115200, 57600, 56000, 38400, 19200, 9600, 4800, 2400, 1200 };
        private readonly int[] DATABIT_ARRAY = new int[] { 8, 7, 6, 5, 4 };
        private readonly string STEP1 = "7F7F7F7F7F7F7F7F7F7F";
        private readonly string STEP2 = "7C7C7C7C7C7C7C7C7C7C";

        public MH1902()
        {
            InitializeComponent();
            InitData();
        }

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

        private void button1_Click(object sender, EventArgs e)
        {

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

        private void button3_Click(object sender, EventArgs e)
        {
            _portName = comboBox1.SelectedValue as string;
            _baudRate = Convert.ToInt32(comboBox2.SelectedValue as string);
            _dataBit = Convert.ToInt32(comboBox3.SelectedValue as string);
            _serialPort = new SerialPort(_portName, _baudRate, Parity.None, _dataBit, StopBits.One);
            _serialPort.Open();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (_serialPort != null)
                _serialPort.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

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

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 建立连接过程的数据包，向芯片端发送10个7C
        /// </summary>
        /// <returns></returns>
        private string Step2()
        {
            richTextBox1.AppendText("\r\n--------------------【Step2】向芯片端发送10个7C--------------------");
            _serialPort.Write(STEP2);
            string receiveStr = _serialPort.ReadExisting();
            richTextBox1.AppendText("\r\n--------------------【Step2】收到芯片端发送过来的数据--------------------");
            richTextBox1.AppendText(receiveStr);
            return "";
        }

        /// <summary>
        /// 建立连接过程的数据包，向芯片端发送10个7F表示需要下载
        /// </summary>
        /// <returns></returns>
        private String Step1()
        {
            richTextBox1.AppendText("--------------------【Step1】向芯片端发送10个7F表示需要下载--------------------");
            _serialPort.Write(STEP1);
            string receiveStr = _serialPort.ReadExisting();
            richTextBox1.AppendText("\r\n--------------------【Step1】收到芯片端发送过来的数据--------------------");
            richTextBox1.AppendText(receiveStr);
            return "";
        }
    }
}
