using HelloCSharp.Log;
using HelloCSharp.Util;
using System;
using System.Data;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace HelloCSharp.UI
{
    public partial class Zm301TestWidnow : Form
    {
        /// <summary>
        /// 工号文本框委托
        /// </summary>
        /// <param name="str">文本内容</param>
        private delegate void WorkIdTxtDele(string str);

        /// <summary>
        /// 表箱文本框委托
        /// </summary>
        /// <param name="str">文本内容</param>
        private delegate void BoxTxtDele(string str);

        /// <summary>
        /// 位置文本框委托
        /// </summary>
        /// <param name="str">文本内容</param>
        private delegate void LatTxtDele(string str);

        /// <summary>
        /// 日志文本框委托
        /// </summary>
        /// <param name="str">文本内容</param>
        private delegate void LogTxtDele(string str);

        /// <summary>
        /// 开锁一，校验码已提前算好
        /// </summary>
        private const string OPEN_DOOR1 = "FE FE FE FE 68 22 23 01 56 34 00 68 10 01 34 E5 16";
        private readonly byte[] OPEN_DOOR1_BYTE = MyConvertUtil.HexStrToBytes(OPEN_DOOR1);

        /// <summary>
        /// 开锁二，校验码已提前算好
        /// </summary>
        private const string OPEN_DOOR2 = "FE FE FE FE 68 22 23 01 56 34 00 68 10 01 35 E6 16";
        private readonly byte[] OPEN_DOOR2_BYTE = MyConvertUtil.HexStrToBytes(OPEN_DOOR2);

        /// <summary>
        /// 开锁三，校验码已提前算好
        /// </summary>
        private const string OPEN_DOOR3 = "FE FE FE FE 68 22 23 01 56 34 00 68 10 01 36 E7 16";
        private readonly byte[] OPEN_DOOR3_BYTE = MyConvertUtil.HexStrToBytes(OPEN_DOOR3);

        /// <summary>
        /// 开全部锁，校验码已提前算好
        /// </summary>
        private const string OPEN_DOOR_ALL = "FE FE FE FE 68 22 23 01 56 34 00 68 10 01 37 E8 16";
        private readonly byte[] OPEN_DOOR_ALL_BYTE = MyConvertUtil.HexStrToBytes(OPEN_DOOR_ALL);

        /// <summary>
        /// 读取工号，校验码已提前算好
        /// </summary>
        private const string READ_WORK = "FE FE FE FE 68 22 23 01 56 34 00 68 0B 00 AB 16";
        private readonly byte[] READ_WORK_BYTE = MyConvertUtil.HexStrToBytes(READ_WORK);

        /// <summary>
        /// 读取表箱号，校验码已提前算好
        /// </summary>
        private const string READ_BOX = "FE FE FE FE 68 22 23 01 56 34 00 68 06 00 9E 16";
        private readonly byte[] READ_BOX_BYTE = MyConvertUtil.HexStrToBytes(READ_BOX);

        /// <summary>
        /// 读取GPS位置，校验码已提前算好
        /// </summary>
        private const string READ_GPS = "FE FE FE FE 68 22 23 01 56 34 00 68 0A 00 AA 16";
        private readonly byte[] READ_GPS_BYTE = MyConvertUtil.HexStrToBytes(READ_GPS);

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

        /// <summary>
        /// 工号的正则表达式
        /// </summary>
        private readonly Regex REGEX_WORK_ID = new Regex("[A-Z a-z 0-9]{8}");

        private MyLogger _logger = MyLogger.Instance;
        private SerialPort _serialPort;
        private Thread _thread;
        private string[] _portNameArray;
        private string _receivedStr = "";

        //串口名
        private string _portName = "";
        //波特率、数据位
        private int _rate = 1200, _data = 8;
        //校验位
        private Parity _parity = Parity.None;

        public Zm301TestWidnow()
        {
            InitializeComponent();
            InitData();
            InitView();
        }

        private void cbx_port_SelectedIndexChanged(object sender, EventArgs e)
        {
            _portName = _portNameArray[cbx_port.SelectedIndex];
            txt_log.AppendText("串口名称：" + _portName + "\r\n");
        }

        private void cbx_rate_SelectedIndexChanged(object sender, EventArgs e)
        {
            _rate = RATE_ARRAY[cbx_rate.SelectedIndex];
            txt_log.AppendText("波特率：" + _rate + "\r\n");
        }

        private void cbx_data_SelectedIndexChanged(object sender, EventArgs e)
        {
            _data = DATA_ARRAY[cbx_data.SelectedIndex];
            txt_log.AppendText("数据位：" + _data + "\r\n");
        }

        private void cbx_parity_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str = PARITY_ARRAY[cbx_parity.SelectedIndex];
            switch (str)
            {
                case "None": _parity = Parity.None; break;
                case "Odd": _parity = Parity.Odd; break;
                case "Even": _parity = Parity.Even; break;
                case "Mark": _parity = Parity.Mark; break;
                case "Space": _parity = Parity.Space; break;
            }
            txt_log.AppendText("校验位：" + _parity + "\r\n");
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
                    if (null == _serialPort)
                    {
                        _serialPort = new SerialPort(_portName, _rate, _parity, _data, StopBits.One);
                        _serialPort.DataReceived += new SerialDataReceivedEventHandler(ReceivedComData);
                    }
                    _serialPort.Open();
                    txt_log.AppendText("已打开：" + _portName + "\r\n");
                    EnableButton(true);
                    btn_open.Text = "关闭串口";
                }
                else
                {
                    _serialPort.Close();
                    txt_log.AppendText("已关闭：" + _portName + "\r\n");
                    EnableButton(false);
                    btn_open.Text = "打开串口";
                }
            }
            catch (Exception ex)
            {
                txt_log.AppendText("串口" + _portName + "打开/关闭失败，原因：" + ex.ToString() + "\r\n");
            }
        }

        /// <summary>
        /// 开锁一
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_lock1_Click(object sender, EventArgs e)
        {
            _serialPort.Write(OPEN_DOOR1_BYTE, 0, OPEN_DOOR1_BYTE.Length);
            txt_log.AppendText("发送开锁一指令：" + OPEN_DOOR1 + "\r\n");
        }

        /// <summary>
        /// 开锁二
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_lock2_Click(object sender, EventArgs e)
        {
            _serialPort.Write(OPEN_DOOR2_BYTE, 0, OPEN_DOOR2_BYTE.Length);
            txt_log.AppendText("发送开锁二指令：" + OPEN_DOOR2 + "\r\n");
        }

        /// <summary>
        /// 开锁三
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_lock3_Click(object sender, EventArgs e)
        {
            _serialPort.Write(OPEN_DOOR3_BYTE, 0, OPEN_DOOR3_BYTE.Length);
            txt_log.AppendText("发送开锁三指令：" + OPEN_DOOR3 + "\r\n");
        }

        /// <summary>
        /// 开锁全部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_lock_all_Click(object sender, EventArgs e)
        {
            _serialPort.Write(OPEN_DOOR_ALL_BYTE, 0, OPEN_DOOR_ALL_BYTE.Length);
            txt_log.AppendText("发送开全部锁指令：" + OPEN_DOOR_ALL + "\r\n");
        }

        /// <summary>
        /// 读取工号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_work_read_Click(object sender, EventArgs e)
        {
            _serialPort.Write(READ_WORK_BYTE, 0, READ_WORK_BYTE.Length);
            txt_log.AppendText("发送读取工号指令：" + READ_WORK + "\r\n");
        }

        /// <summary>
        /// 设置工号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_work_write_Click(object sender, EventArgs e)
        {
            string comStr = "68 53 01 80 01 01 00 68 0B 08";
            string input = txt_work_id.Text;
            bool flag = Regex.IsMatch(input, @"[A-Z a-z 0-9]{8}");
            if (flag)
            {
                String str = "";
                for (int i = 0; i < input.Length; i++)
                {
                    string temp = MyConvertUtil.CharAt(input, i);
                    temp = MyConvertUtil.StrToHexStr(temp);
                    if (temp.Length < 2)
                        temp = "0" + temp;
                    Print("遂字转换（Hex）：" + temp);
                    str += temp;
                }
                String[] strArray = MyConvertUtil.StrSplitInterval(str, 2);
                comStr += " " + strArray[0] + " " + strArray[1] + " " + strArray[2] + " " + strArray[3] + " " + strArray[4] + " " + strArray[5] + " " + strArray[6] + " " + strArray[7];
                string crcStr = MyConvertUtil.CalcZM301CRC(comStr);
                Print("计算出的校验码（Hex）：" + crcStr);
                comStr += " " + crcStr + " 16";
                byte[] comByte = MyConvertUtil.HexStrToBytes(comStr);
                _serialPort.Write(comByte, 0, comByte.Length);
                txt_log.AppendText("发送设置工号指令：" + comStr + "\r\n");
            }
        }

        /// <summary>
        /// 读取表箱号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_box_read_Click(object sender, EventArgs e)
        {
            _serialPort.Write(READ_BOX_BYTE, 0, READ_BOX_BYTE.Length);
            txt_log.AppendText("发送读取表箱号指令：" + READ_BOX + "\r\n");
        }

        /// <summary>
        /// 设置表箱号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_box_write_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 查询GPS位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_gps_Click(object sender, EventArgs e)
        {
            _serialPort.Write(READ_GPS_BYTE, 0, READ_GPS_BYTE.Length);
            txt_log.AppendText("发送查询GPS指令：" + READ_GPS + "\r\n");
        }

        /// <summary>
        /// 清空日志文本框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_clear_Click(object sender, EventArgs e)
        {
            txt_log.Clear();
        }

        /// <summary>
        /// 打印Log+控制台输出
        /// </summary>
        /// <param name="str"></param>
        private void Print(string str)
        {
            _logger.WriteLog(str);
            Console.WriteLine(str);
        }

        /// <summary>
        /// 修改日志文本框的内容
        /// </summary>
        /// <param name="str"></param>
        private void LogTxtChangedByDele(string str)
        {
            //非UI线程访问控件时
            if (txt_log.InvokeRequired)
            {
                txt_log.Invoke(new LogTxtDele(LogTxtChangedByDele), str);
            }
            else
            {
                txt_log.AppendText(str);
            }
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
        }

        /// <summary>
        /// 启用禁用按钮控件
        /// </summary>
        /// <param name="flag"></param>
        private void EnableButton(bool flag)
        {
            btn_lock1.Enabled = flag;
            btn_lock2.Enabled = flag;
            btn_lock3.Enabled = flag;
            btn_lock_all.Enabled = flag;
            btn_work_read.Enabled = flag;
            btn_work_write.Enabled = flag;
            btn_box_read.Enabled = flag;
            btn_box_write.Enabled = flag;
            btn_gps.Enabled = flag;
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
            {
                return;
            }
            //待读字节长度
            int byteLen = _serialPort.BytesToRead;
            byte[] byteArray = new byte[byteLen];
            _serialPort.Read(byteArray, 0, byteArray.Length);
            string str = MyConvertUtil.BytesToStr(byteArray);
            Print("本次读取字节：" + str + "，长度：" + byteLen);
            _receivedStr += str;
            //不能仅凭指令的开头和结尾来判断是否为一包完整数据，还需要验证校验码
            if (_receivedStr.StartsWith("68") && _receivedStr.EndsWith("16"))
            {
                Print("累计收到字节（Hex）：" + _receivedStr);
                //计算收到的数据的校验码的时候不包含最后现个字节
                string tempReceivedStr = _receivedStr.Substring(0, _receivedStr.Length - 4);
                Print("参与计算校验码的数据（Hex）：" + tempReceivedStr);
                //计算出的校验码
                string crcStr1 = MyConvertUtil.CalcZM301CRC(tempReceivedStr);
                Print("计算出的校验码（Hex）：" + crcStr1);
                string[] strArray = MyConvertUtil.StrSplitInterval(_receivedStr, 2);
                //收到的数据里的校验码
                string crcStr2 = strArray[strArray.Length - 2];
                Print("数据内容包含的校验码（Hex）：" + crcStr2);
                //比较校验码
                bool flag = crcStr1.Equals(crcStr2);
                Print("校验码是否正确：" + flag);
                if (flag)
                {
                    int len = _receivedStr.Length;
                    Print("收到完整的指令（Hex）：" + _receivedStr + "，数据长度：" + len);
                    _receivedStr = MyConvertUtil.StrAddChar(_receivedStr, 2, " ");
                    LogTxtChangedByDele("收到（Hex）：" + _receivedStr + "，数据长度：" + len + "\r\n");
                    //清空缓存
                    _receivedStr = "";
                }
            }
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
        /// 从数组中截取一部分成新的数组
        /// </summary>
        /// <param name="source">原数组</param>
        /// <param name="startIndex">原数组的起始位置</param>
        /// <param name="endIndex">原数组的截止位置</param>
        /// <returns></returns>
        public string[] SplitArray(string[] source, int startIndex, int endIndex)
        {
            try
            {
                string[] result = new string[endIndex - startIndex + 1];
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
        /// str[]排序
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
