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
using System.Text.RegularExpressions;
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
        private readonly Dictionary<int, string> DEVICEINFODICTIONARY = new Dictionary<int, string> {
            {1,"TAG_KEY_SUCCESS"},
            {2,"TAG_KEY_FAIL"},
            {3,"TAG_KEY_SN"},
            {4,"TAG_KEY_MODEL_ID"},
            {5,"TAG_KEY_DEVICE_ID"},
            {6,"TAG_KEY_DESK_KEY"},
            {7,"TAG_KEY_PIN_KEY"},
            {8,"TAG_KEY_TK_DESK"},
            {9,"TAG_KEY_TK_PIN"},
            {10,"TAG_KEY_SOFT_VER"},
            {11,"TAG_KEY_HARDWARE_VER"},
            {12,"TAG_KEY_SECURITY_1" },
            {31,"TAG_BOOT_MODE"}
        };

        private SerialPort _serialPort;
        //波特率、数据位、当前步骤、步骤1的数据长度、步骤2的数据长度、步骤3的数据长度、步骤4的数据长度、步骤5的数据长度、数据的长度（不包含最后两位校验码）
        private int _baudRate = 0, _dataBit = 8, _step = 1, _step1Len = 0, _step2Len = 0, _step3Len = 0, _step4Len = 0, _step5Len = 0, _dataLen = 0;
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
            InitView();
        }

        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitView()
        {
            EnableButton(false);
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

        private void EnableButton(bool flag)
        {
            button1.Enabled = flag;
            button2.Enabled = flag;
            button3.Enabled = flag;
            button4.Enabled = flag;
            button5.Enabled = flag;
            button6.Enabled = flag;
            button7.Enabled = flag;
            button8.Enabled = flag;
            button11.Enabled = flag;
            button12.Enabled = flag;
            button13.Enabled = flag;
            button14.Enabled = flag;
        }

        /// <summary>
        /// 开启/关闭串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button15_Click(object sender, EventArgs e)
        {
            if (button15.Text.ToString().Equals("打开串口"))
            {
                button15.Text = "关闭串口";
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
                EnableButton(true);
            }
            else
            {
                if (_serialPort != null)
                {
                    _serialPort.Close();
                    richTextBox1.AppendText("已关闭：" + _portName + "\r\n");
                }
                EnableButton(false);
            }
        }

        /// <summary>
        /// 【tabPage1】选择文件
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
        /// 【tabPage1】查询基本参数
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
        /// 【tabPage1】请求下载
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
        /// 【tabPage1】发送固件数据包
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
            string[] tempTotalLenArray = MyConvertUtil.StrAddChar(dataTotalLenHexStr, 2, " ").Split(' ');
            dataTotalLenHexStr = MyConvertUtil.SortStringArray(tempTotalLenArray, false);
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
            string[] everyPackageLenNotLastHexStrArray = MyConvertUtil.StrAddChar(everyPackageLenNotLastHexStr, 2, " ").Split(' ');
            everyPackageLenNotLastHexStr = MyConvertUtil.SortStringArray(everyPackageLenNotLastHexStrArray, false);
            //包数
            double packageNum = Math.Ceiling((double)fileLen / dataMax);
            //最后一个包的长度
            int surplusLen = fileLen % dataMax;
            //每次截取升级文件字节数组时的结束下标
            int splitArrayEndIndex = 0;
            for (int i = 0; i < packageNum; i++)
            {
                //当前块号，将高字节放在前面
                int blockNo = i + 1;
                string blockNoHexStr = blockNo.ToString("x4");
                string[] blockNoHexStrArray = MyConvertUtil.StrAddChar(blockNoHexStr, 2, " ").Split(' ');
                blockNoHexStr = MyConvertUtil.SortStringArray(blockNoHexStrArray, false);
                if (i != packageNum - 1)
                {
                    //包内容，不包含固件数据和校验码
                    string cmdx = "3F" + everyPackageLenNotLastHexStr + packageProperty + "21" + blockName + dataTotalLenHexStr + blockNoHexStr;
                    byte[] tempBytes1 = MyConvertUtil.HexStrToBytes(cmdx);
                    //要截取的数组的结束下标，不是长度
                    splitArrayEndIndex = (i + 1) * dataMax - 1;
                    //固件数据
                    byte[] tempBytes2 = MyConvertUtil.SplitArray(fileData, i * dataMax, splitArrayEndIndex);
                    string tempStr11111111111111111111111111111111111111111111111111111111 = MyConvertUtil.BytesToStr(tempBytes2);
                    //包内容，不包含校验码
                    byte[] tempBytes3 = MyConvertUtil.MergerArray(tempBytes1, tempBytes2);
                    string tempStr22222222222222222222222222222222222222222222222222222222 = MyConvertUtil.BytesToStr(tempBytes3);
                    //计算2位校验码，低字节在前
                    string crcStr = MyConvertUtil.CalcCRC(tempBytes3, true);
                    byte[] tempBytes4 = MyConvertUtil.HexStrToBytes(crcStr);
                    //完整的包内容
                    byte[] tempBytes5 = MyConvertUtil.MergerArray(tempBytes3, tempBytes4);
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
                    string[] tempArray = MyConvertUtil.StrAddChar(surplusLenHexStr, 2, " ").Split(' ');
                    surplusLenHexStr = MyConvertUtil.SortStringArray(tempArray, false);
                    //包内容，不包含固件数据和校验码
                    string cmdx = "3F" + surplusLenHexStr + packageProperty + "21" + blockName + dataTotalLenHexStr + blockNoHexStr;
                    byte[] tempBytes1 = MyConvertUtil.HexStrToBytes(cmdx);
                    //固件数据
                    byte[] tempBytes2 = MyConvertUtil.SplitArray(fileData, i * dataMax, splitArrayEndIndex + (int)surplusLen);
                    string tempStr11111111111111111111111111111111111111111111111111111111 = MyConvertUtil.BytesToStr(tempBytes2);
                    //包内容，不包含校验码
                    byte[] tempBytes3 = MyConvertUtil.MergerArray(tempBytes1, tempBytes2);
                    string tempStr22222222222222222222222222222222222222222222222222222222 = MyConvertUtil.BytesToStr(tempBytes3);
                    //计算2位校验码，低字节在前
                    string crcStr = MyConvertUtil.CalcCRC(tempBytes3, true);
                    byte[] tempBytes4 = MyConvertUtil.HexStrToBytes(crcStr);
                    //完整的包内容
                    byte[] tempBytes5 = MyConvertUtil.MergerArray(tempBytes3, tempBytes4);
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

        /// <summary>
        /// 清除RichTextBox1的内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        /// <summary>
        /// 【tabPage2】浏览
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 【tabPage2】加载配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            byte[] data = MyConvertUtil.HexStrToBytes("3F070000002039A9");
            Thread thread1 = new Thread(new ParameterizedThreadStart(WriteBytes));
            thread1.Start(data);
            richTextBox1.AppendText("已请求进入下载模式...\r\n");
        }

        /// <summary>
        /// 【tabPage2】编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            byte[] data = MyConvertUtil.HexStrToBytes("3F07000000017A9D");
            Thread thread = new Thread(new ParameterizedThreadStart(WriteBytes));
            thread.Start(data);
            richTextBox1.AppendText("查询POS机基本参数..." + "\r\n");
        }

        /// <summary>
        /// 【tabPage2】终止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 【tabPage2】下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 【tabPage2】连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 【tabPage2】选择本地升级文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 【tabPage2】签名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                richTextBox1.AppendText("收到：" + str);
                //switch (_step)
                //{
                //    case 1:
                //        _step1Thread.Abort();
                //        richTextBox1.AppendText("【Step1】收到：" + str + "\r\n");
                //        richTextBox1.AppendText("【Step2】正在持续发送7C..." + "\r\n");
                //        _step2Thread = new Thread(Step2);
                //        _step2Thread.Start();
                //        _step = 2;
                //        break;
                //    case 2:
                //        _step2Thread.Abort();
                //        richTextBox1.AppendText("【Step2】收到：" + str + "\r\n");
                //        if (_serialPort != null && _serialPort.IsOpen)
                //        {
                //            _serialPort.Write(STEP3, 0, STEP3.Length);
                //            richTextBox1.AppendText("【Step3】指令已发送..." + "\r\n");
                //        }
                //        _step = 3;
                //        break;
                //    case 3:
                //        richTextBox1.AppendText("【Step3】收到：" + str + "\r\n");
                //        break;
                //    case 4:
                //        richTextBox1.AppendText("【Step4】收到：" + str + "\r\n");
                //        break;
                //    case 5:
                //        richTextBox1.AppendText("【Step5】收到：" + str + "\r\n");
                //        break;
                //}
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
            _packageStr += str;
            _logger.WriteLog("收到的数据（Hex）：" + str + "，长度：" + byteLen);
            Console.WriteLine("收到的数据（Hex）：" + str + "，长度：" + byteLen);
            //每隔两位插入一个空格，用以分割成数组，方便使用下标进行判断
            string[] receivedDataArray = MyConvertUtil.StrAddChar(_packageStr, 2, ",").Split(',');
            string head = receivedDataArray[0];
            //1、必须是3F打头，否则就说明是上一包没读完的数据
            //2、下标为1和2的数据是数据长度（不包含最后两位校验码，高位在前），长度不一致说明这一包数据不完整
            //防止下标越界，同时避免该包连“数据长度”都没有
            if (head.Equals("3F") && _packageStr.Length >= 3)
            {
                //数据内容的长度
                _dataLen = MyConvertUtil.HexStrToInt(receivedDataArray[2] + receivedDataArray[1]);
                //完整包，命令头3F不算在内
                if ((_packageStr.Length - 2) / 2 == _dataLen)
                {
                    string type = receivedDataArray[5];
                    string result = receivedDataArray[6];
                    string txt = "";
                    switch (type)
                    {
                        //基本参数
                        case "01":
                            if (result.Equals("00"))
                            {
                                txt = "\r\n数据正确\r\n";
                                //解析的时候不算1个字节的命令头、2个字节的长度、2个字节的包属性、1个字节的命令码、1个字节的结果、2个字节的校验码
                                string[] data = MyConvertUtil.SplitArray(receivedDataArray, 7, receivedDataArray.Length - 1 - 2);
                                int t;
                                int l;
                                string v;
                                for (int i = 0; i < data.Length; i++)
                                {
                                    //T
                                    t = MyConvertUtil.HexStrToInt(data[i]);
                                    string tStr;
                                    DEVICEINFODICTIONARY.TryGetValue(t, out tStr);
                                    txt += tStr + " ";
                                    //L
                                    l = MyConvertUtil.HexStrToInt(data[++i]);
                                    txt += l + " ";
                                    //V
                                    string[] vArray = MyConvertUtil.SplitArray(data, i + 1, i + l);
                                    v = string.Join("", vArray);
                                    if (t == 3 || t == 5 || t == 10)
                                    {
                                        txt += MyConvertUtil.HexStrToStr(v) + "\r\n";
                                    }
                                    else
                                    {
                                        txt += MyConvertUtil.HexStrToInt(v) + "\r\n";
                                    }
                                    i += l;
                                }
                            }
                            else
                            {
                                txt += _packageStr + "\r\n数据有误\r\n";
                            }
                            break;
                        //请求下载
                        case "20":
                            if (result.Equals("00") || result.Equals("01"))
                            {
                                txt = _packageStr + "\r\n已进入下载模式！\r\n";
                            }
                            else
                            {
                                txt = _packageStr + "\r\n下载模式请求失败！\r\n";
                            }
                            break;
                        //固件升级包
                        case "21":
                            if (result.Equals("00"))
                            {
                                txt = _packageStr + "\r\n该包数据正确！\r\n";
                            }
                            else
                            {
                                txt = _packageStr + "\r\n该包数据错误！\r\n";
                            }
                            break;
                    }
                    //txt = Regex.Match(txt, "[A-Za-z0-9\u4e00-\u9fa5-]+").Value;
                    RichTextBoxChangedByDele(_packageStr + txt);
                    _logger.WriteLog(_packageStr + txt);
                    Console.WriteLine(_packageStr + txt);
                    _packageStr = "";
                }
                else
                {
                    _logger.WriteLog("该包数据不完整，已缓存，等待下一包数据...");
                    Console.WriteLine("该包数据不完整，已缓存，等待下一包数据...");
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
    }

}
