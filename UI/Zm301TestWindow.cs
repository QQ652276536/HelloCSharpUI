using HelloCSharp.Log;
using HelloCSharp.Util;
using System;
using System.Data;
using System.IO.Ports;
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

        private readonly int[] RATE_ARRAY = new int[] { 115200, 57600, 56000, 38400, 19200,
            9600, 4800, 2400, 1200 };
        private readonly int[] DATA_ARRAY = new int[] { 8, 7, 6, 5, 4 };
        private readonly string[] PARITY_ARRAY = new string[] { "None", "Odd", "Even", "Mark", "Space" };

        private MyLogger _logger = MyLogger.Instance;
        private SerialPort _serialPort;
        private Thread _thread;
        private string[] _portNameArray;

        //串口名
        private string _portName = "";
        //波特率、数据位
        private int _rate = 115200, _data = 8;
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
            txt_log.AppendText("串口名称：" + _portName + "\n");
        }

        private void cbx_rate_SelectedIndexChanged(object sender, EventArgs e)
        {
            _rate = RATE_ARRAY[cbx_rate.SelectedIndex];
            txt_log.AppendText("波特率：" + _rate + "\n");
        }

        private void cbx_data_SelectedIndexChanged(object sender, EventArgs e)
        {
            _data = DATA_ARRAY[cbx_data.SelectedIndex];
            txt_log.AppendText("数据位：" + _data + "\n");
        }

        private void cbx_parity_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str = PARITY_ARRAY[cbx_data.SelectedIndex];
            switch (str)
            {
                case "None": _parity = Parity.None; break;
                case "Odd": _parity = Parity.Odd; break;
                case "Even": _parity = Parity.Even; break;
                case "Mark": _parity = Parity.Mark; break;
                case "Space": _parity = Parity.Space; break;
            }
            txt_log.AppendText("校验位：" + _data + "\n");
        }

        /// <summary>
        /// 开启/关闭串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_open_Click(object sender, EventArgs e)
        {
            if (btn_open.Text.ToString().Equals("打开串口"))
            {
                btn_open.Text = "关闭串口";
                if (_serialPort == null)
                {
                    _serialPort = new SerialPort(_portName, _rate, _parity, _data, StopBits.None);
                    _serialPort.DataReceived += new SerialDataReceivedEventHandler(ReceivedComData);
                    txt_log.AppendText("已打开：" + _portName + "\n");
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
                    txt_log.AppendText("已关闭：" + _portName + "\n");
                }
                EnableButton(false);
            }
        }

        /// <summary>
        /// 开锁一
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_lock1_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 开锁二
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_lock2_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 开锁三
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_lock3_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 开锁全部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_lock_all_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 读取工号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_work_read_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 设置工号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_work_write_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 读取表箱号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_box_read_Click(object sender, EventArgs e)
        {

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
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="obj"></param>
        private void WriteBytes(object obj)
        {
            byte[] data = obj as byte[];
            if (_serialPort.IsOpen)
            {
                _serialPort.Write(data, 0, data.Length);
                _logger.WriteLog("已发送数据...");
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
