using HelloCSharp.Log;
using HelloCSharp.Util;
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

namespace HelloCSharp.UI
{
    public partial class MH1902 : Form
    {
        private readonly int[] BAUDRATE_ARRAY = new int[] { 115200, 57600, 56000, 38400, 19200,
            9600, 4800, 2400, 1200 };
        private readonly int[] DATABIT_ARRAY = new int[] { 8, 7, 6, 5, 4 };
        private readonly byte[] STEP1 = MyConvertUtil.HexStrToBytes("7F7F7F7F7F7F7F7F7F7F");
        private readonly byte[] STEP2 = MyConvertUtil.HexStrToBytes("7C7C7C7C7C7C7C7C7C7C");
        private readonly byte[] STEP3 = MyConvertUtil.HexStrToBytes("01330600030046DB");

        private SerialPort _serialPort;
        //波特率、数据位、当前步骤、步骤1的数据长度、步骤2的数据长度、步骤3的数据长度、步骤4的数据长度、步骤5的数据长度、数据的长度（不包含最后两位校验码）
        private int _baudRate = 0, _dataBit = 8, _step = 1, _step1Len = 0, _step2Len = 0, _step3Len = 0, _step4Len = 0, _step5Len = 0, _dataLen = 0, _count = 0;
        //密钥文件路径、升级文件路径、串口名、本包内容
        private string _secretKeyPath = "", _portName = "", _packageStr = "";
        private Thread _step1Thread, _step2Thread;
        private MyLogger _logger = MyLogger.Instance;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">文本内容</param>
        private delegate void RichTextBoxDele(string str);

        public MH1902()
        {
            InitializeComponent();
            InitData();
        }

        /// <summary>
        /// 初始化控件需要的数据
        /// </summary>
        private void InitData()
        {
            //串口Combox赋值
            string[] portNameArray = SerialPort.GetPortNames();
            DataTable dataTable1 = new DataTable();
            dataTable1.Columns.Add("value");
            foreach (string temp in portNameArray)
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

        private void button15_Click(object sender, EventArgs e)
        {
            _portName = comboBox1.SelectedValue as string;
            _baudRate = Convert.ToInt32(comboBox2.SelectedValue as string);
            _dataBit = Convert.ToInt32(comboBox3.SelectedValue as string);
            if (_serialPort == null)
            {
                _serialPort = new SerialPort(_portName, _baudRate, Parity.None, _dataBit, StopBits.One);
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(ReceivedComData);
                richTextBox1.AppendText("已打开：" + _portName + "\r\n");
                _serialPort.Open();
            }
            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
            }
        }

        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button14_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //是否允许选择多个文件
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "请选择升级文件";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //返回文件的完整路径
                textBox7.Text = openFileDialog.FileName;
            }
        }

        /// <summary>
        /// 查询基本参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            byte[] data = MyConvertUtil.HexStrToBytes("3F07000000017A9D");
            Thread thread = new Thread(new ParameterizedThreadStart(WriteBytes));
            thread.Start(data);
            richTextBox1.AppendText("查询POS机基本参数..." + "\r\n");

        }

        /// <summary>
        /// 请求下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            byte[] data = MyConvertUtil.HexStrToBytes("3F070000002039A9");
            Thread thread1 = new Thread(new ParameterizedThreadStart(WriteBytes));
            thread1.Start(data);
            richTextBox1.AppendText("已请求进入下载模式...\r\n");
        }

        /// <summary>
        /// 发送本地文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            int fileLen = 0;
            byte[] fileData;
            try
            {
                fileData = File.ReadAllBytes(textBox6.Text.ToString());
                fileLen = fileData.Length;
                string fileStr1 = MyConvertUtil.BytesToStr(fileData);
                int a = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //每包固定的长度（除包头外）=包长度（2字节）+包属性（2字节）+命令码（1字节）+固件名字（16字节）+固件总长度（3字节）+当前块号（2字节）+校验码（2字节）=28字节
            int everyPackageFixedLen = 28;
            //包属性
            string packageProperty = "4012";
            //固件名字，将高字节放在前面
            string blockName = MyConvertUtil.StrToHexStr("kernel.bin").PadRight(32, '0');
            //固件总长度，将高字节放在前面
            string dataTotalLenHexStr = fileLen.ToString("x6");
            string[] tempTotalLenArray = MyConvertUtil.StrAddCharacter(dataTotalLenHexStr, 2, " ").Split(' ');
            dataTotalLenHexStr = SortStringArray(tempTotalLenArray, false);
            //将固件数据分包，每包的长度
            int dataMax = 900;
            if (fileLen < dataMax)
            {
                MessageBox.Show("该升级包小于分包");
                return;
            }
            //包长度，不包含包头=固件数据分包+每包固定长度，将高字节放在前面
            int everyPackageLenNotLast = dataMax + everyPackageFixedLen;
            string everyPackageLenNotLastHexStr = everyPackageLenNotLast.ToString("x4");
            string[] everyPackageLenNotLastHexStrArray = MyConvertUtil.StrAddCharacter(everyPackageLenNotLastHexStr, 2, " ").Split(' ');
            everyPackageLenNotLastHexStr = SortStringArray(everyPackageLenNotLastHexStrArray, false);
            //包数
            double packageNum = Math.Ceiling((double)fileLen / dataMax);
            //最后一个包的长度
            int surplusLen = fileLen % dataMax;
            //每次截取升级文件字节数组时的结束下标
            int splitArrayEndIndex = 0;
            for (int i = 0; i < packageNum; i++)
            {
                _count++;
                //当前块号，将高字节放在前面
                int blockNo = i + 1;
                string blockNoHexStr = blockNo.ToString("x4");
                string[] blockNoHexStrArray = MyConvertUtil.StrAddCharacter(blockNoHexStr, 2, " ").Split(' ');
                blockNoHexStr = SortStringArray(blockNoHexStrArray, false);
                if (i != packageNum - 1)
                {
                    //包内容，不包含固件数据和校验码
                    string cmdx = "3F" + everyPackageLenNotLastHexStr + packageProperty + "21" + blockName + dataTotalLenHexStr + blockNoHexStr;
                    byte[] tempBytes1 = MyConvertUtil.HexStrToBytes(cmdx);
                    //要截取的数组的结束下标，不是长度
                    splitArrayEndIndex = (i + 1) * dataMax - 1;
                    //固件数据
                    byte[] tempBytes2 = SplitArray(fileData, i * dataMax, splitArrayEndIndex);
                    string tempStr11111111111111111111111111111111111111111111111111111111 = MyConvertUtil.BytesToStr(tempBytes2);
                    //包内容，不包含校验码
                    byte[] tempBytes3 = MergerArray(tempBytes1, tempBytes2);
                    string tempStr22222222222222222222222222222222222222222222222222222222 = MyConvertUtil.BytesToStr(tempBytes3);
                    //计算2位校验码，低字节在前
                    string crcStr = CalculateCRC(tempBytes3, true);
                    byte[] tempBytes4 = MyConvertUtil.HexStrToBytes(crcStr);
                    //完整的包内容
                    byte[] tempBytes5 = MergerArray(tempBytes3, tempBytes4);
                    string tempStr33333333333333333333333333333333333333333333333333333333 = MyConvertUtil.BytesToStr(tempBytes5);
                    new Thread(new ParameterizedThreadStart(WriteBytes)).Start(tempBytes5);
                    richTextBox1.AppendText("发送第" + blockNo + "包数据..." + "\r\n");
                    //设置光标的位置到文本尾   
                    richTextBox1.Select(richTextBox1.TextLength, 0);
                    //滚动到控件光标处   
                    richTextBox1.ScrollToCaret();
                }
                else
                {
                    if (surplusLen == 0)
                    {
                        return;
                    }
                    //高字节在前
                    string surplusLenHexStr = ((int)surplusLen).ToString("x4");
                    string[] tempArray = MyConvertUtil.StrAddCharacter(surplusLenHexStr, 2, " ").Split(' ');
                    surplusLenHexStr = SortStringArray(tempArray, false);
                    //包内容，不包含固件数据和校验码
                    string cmdx = "3F" + surplusLenHexStr + packageProperty + "21" + blockName + dataTotalLenHexStr + blockNoHexStr;
                    byte[] tempBytes1 = MyConvertUtil.HexStrToBytes(cmdx);
                    //固件数据
                    byte[] tempBytes2 = SplitArray(fileData, i * dataMax, splitArrayEndIndex + (int)surplusLen);
                    string tempStr11111111111111111111111111111111111111111111111111111111 = MyConvertUtil.BytesToStr(tempBytes2);
                    //包内容，不包含校验码
                    byte[] tempBytes3 = MergerArray(tempBytes1, tempBytes2);
                    string tempStr22222222222222222222222222222222222222222222222222222222 = MyConvertUtil.BytesToStr(tempBytes3);
                    //计算2位校验码，低字节在前
                    string crcStr = CalculateCRC(tempBytes3, true);
                    byte[] tempBytes4 = MyConvertUtil.HexStrToBytes(crcStr);
                    //完整的包内容
                    byte[] tempBytes5 = MergerArray(tempBytes3, tempBytes4);
                    string tempStr33333333333333333333333333333333333333333333333333333333 = MyConvertUtil.BytesToStr(tempBytes5);
                    new Thread(new ParameterizedThreadStart(WriteBytes)).Start(tempBytes5);
                    richTextBox1.AppendText("发送第" + (i + 1) + "包数据..." + "\r\n");
                    //设置光标的位置到文本尾   
                    richTextBox1.Select(richTextBox1.TextLength, 0);
                    //滚动到控件光标处   
                    richTextBox1.ScrollToCaret();
                }
                Thread.Sleep(500);
            }
            richTextBox1.AppendText("升级包发送完毕!\r\n");
            button1.Enabled = true;
        }

        private void button10_Click(object sender, EventArgs e)
        {
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
            byte[] data = MyConvertUtil.HexStrToBytes("3F070000002039A9");
            Thread thread1 = new Thread(new ParameterizedThreadStart(WriteBytes));
            thread1.Start(data);
            richTextBox1.AppendText("已请求进入下载模式...\r\n");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            byte[] data = MyConvertUtil.HexStrToBytes("3F07000000017A9D");
            Thread thread = new Thread(new ParameterizedThreadStart(WriteBytes));
            thread.Start(data);
            richTextBox1.AppendText("查询POS机基本参数..." + "\r\n");
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
                textBox6.Text = openFileDialog.FileName;
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
                        _step1Thread.Abort();
                        richTextBox1.AppendText("【Step1】收到：" + str + "\r\n");
                        richTextBox1.AppendText("【Step2】正在持续发送7C..." + "\r\n");
                        _step2Thread = new Thread(Step2);
                        _step2Thread.Start();
                        _step = 2;
                        break;
                    case 2:
                        _step2Thread.Abort();
                        richTextBox1.AppendText("【Step2】收到：" + str + "\r\n");
                        if (_serialPort != null && _serialPort.IsOpen)
                        {
                            _serialPort.Write(STEP3, 0, STEP3.Length);
                            richTextBox1.AppendText("【Step3】指令已发送..." + "\r\n");
                        }
                        _step = 3;
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
        /// 注意，收到数据就会触发该方法，每次收到的数据不一定是完整的，需要校对
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
            //本次收到的数据长度，只是用于按字节读取
            int byteLen = tempSerialPort.BytesToRead;
            byte[] byteArray = new byte[byteLen];
            tempSerialPort.Read(byteArray, 0, byteArray.Length);
            string str = MyConvertUtil.BytesToStr(byteArray);
            if (string.IsNullOrEmpty(str))
            {
                return;
            }
            _logger.WriteLog(_count + "收到的数据（Hex）：" + str + "，长度：" + byteLen);
            Console.WriteLine(_count + "收到的数据（Hex）：" + str + "，长度：" + byteLen);
            _packageStr += str;
            //每隔两位插入一个空格，用以分割成数组，方便使用下标进行判断
            string[] receivedDataArray = MyConvertUtil.StrAddCharacter(_packageStr, 2, ",").Split(',');
            //1、必须是AA打头，否则就说明是上一包没读完的数据
            //2、下标为2和3的数据是数据长度（不包含最后两位校验码，低位在前），长度不一致说明这一包数据不完整
            //防止下标越界，同时避免本次包连“数据长度”都没有
            if (_packageStr.StartsWith("AA") && _packageStr.Length >= 4)
            {
                //数据内容的长度
                _dataLen = MyConvertUtil.HexStrToInt(receivedDataArray[3] + receivedDataArray[2]);
                //完整包
                if (_packageStr.Length == _dataLen + 2)
                {
                    RichTextBoxChangedByDele(_packageStr);
                    _packageStr = "";
                }
                else
                {
                    _logger.WriteLog("本次包数据不完整");
                    Console.WriteLine("本次包数据不完整");
                }
            }
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

        private void WriteBytes(object obj)
        {
            byte[] data = obj as byte[];
            if (_serialPort.IsOpen)
            {
                _serialPort.Write(data, 0, data.Length);
                _logger.WriteLog("已发送数据...");
                Console.WriteLine("已发送数据...");
            }
        }

        /// <summary>
        /// 计算2位校验码
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="isLowInBefore">低字节在前/后</param>
        /// <returns></returns>
        private string CalculateCRC(byte[] byteArray, bool isLowInBefore)
        {
            int crc = 0;
            foreach (byte tempByte in byteArray)
            {
                // 0x80 = 128
                for (int i = 0x80; i != 0; i /= 2)
                {
                    crc *= 2;
                    // 0x10000 = 65536
                    if ((crc & 0x10000) != 0)
                        // 0x11021 = 69665
                        crc ^= 0x11021;
                    if ((tempByte & i) != 0)
                        // 0x1021 = 4129
                        crc ^= 0x1021;
                }
            }
            string crcStr = crc.ToString("x2");
            string[] tempArray = MyConvertUtil.StrAddCharacter(crcStr, 2, " ").Split(' ');
            if (tempArray.Length == 1)
            {
                return "00" + tempArray[0];
            }
            if (isLowInBefore)
            {
                if (MyConvertUtil.HexStrToInt(tempArray[0]) > MyConvertUtil.HexStrToInt(tempArray[1]))
                {
                    string temp = tempArray[1];
                    tempArray[1] = tempArray[0];
                    tempArray[0] = temp;
                }
            }
            else
            {
                if (MyConvertUtil.HexStrToInt(tempArray[0]) < MyConvertUtil.HexStrToInt(tempArray[1]))
                {
                    string temp = tempArray[1];
                    tempArray[1] = tempArray[0];
                    tempArray[0] = temp;
                }
            }
            //补零
            if (tempArray[0].Length < 2)
                tempArray[0] = "0" + tempArray[0];
            if (tempArray[1].Length < 2)
                tempArray[1] = "0" + tempArray[1];
            return tempArray[0] + tempArray[1];
        }

        /// <summary>
        /// 从数组中截取一部分成新的数组
        /// </summary>
        /// <param name="source">原数组</param>
        /// <param name="startIndex">原数组的起始位置</param>
        /// <param name="endIndex">原数组的截止位置</param>
        /// <returns></returns>
        public byte[] SplitArray(byte[] source, int startIndex, int endIndex)
        {
            try
            {
                byte[] result = new byte[endIndex - startIndex + 1];
                for (int i = 0; i <= endIndex - startIndex; i++)
                    result[i] = source[i + startIndex];
                return result;
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 合并数组
        /// </summary>
        /// <param name="first">第一个数组</param>
        /// <param name="second">第二个数组</param>
        /// <returns></returns>
        public byte[] MergerArray(byte[] first, byte[] second)
        {
            byte[] result = new byte[first.Length + second.Length];
            first.CopyTo(result, 0);
            second.CopyTo(result, first.Length);
            return result;
        }

        /// <summary>
        /// Str的前面/后面补零
        /// </summary>
        /// <param name="input"></param>
        /// <param name="length"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        private string SupplementZero(string input, int length, bool flag)
        {
            string zero = "";
            for (int i = 0; i < length - input.Length; i++)
            {
                zero += "0";
            }
            if (flag)
                return zero + input;
            else
                return input + zero;
        }

        /// <summary>
        /// Str[]排序
        /// </summary>
        /// <param name="array"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        private string SortStringArray(string[] array, bool flag)
        {
            //从小到大
            if (flag)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    for (int j = 0; j < array.Length; j++)
                    {
                        if (MyConvertUtil.HexStrToInt(array[i]) < MyConvertUtil.HexStrToInt(array[j]))
                        {
                            string temp = array[i];
                            array[i] = array[j];
                            array[j] = temp;
                        }
                    }
                }
            }
            //从大到小
            else
            {
                for (int i = 0; i < array.Length; i++)
                {
                    for (int j = 0; j < array.Length; j++)
                    {
                        if (MyConvertUtil.HexStrToInt(array[i]) > MyConvertUtil.HexStrToInt(array[j]))
                        {
                            string temp = array[i];
                            array[i] = array[j];
                            array[j] = temp;
                        }
                    }
                }
            }
            string result = "";
            foreach (string tempStr in array)
            {
                result += tempStr;
            }
            return result;
        }
    }
}
