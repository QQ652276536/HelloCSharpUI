using System;
using System.Text;

namespace HelloCSharp.Util
{

    class MyConvertUtil
    {
        private MyConvertUtil()
        {
        }

        /// <summary>
        /// 计算2位校验码
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="isLowInBefore">低字节在前/后</param>
        /// <returns></returns>
        public static string CalculateCRC(byte[] byteArray, bool isLowInBefore)
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
        public static byte[] SplitArray(byte[] source, int startIndex, int endIndex)
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
        public static string[] SplitArray(string[] source, int startIndex, int endIndex)
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
        public static byte[] MergerArray(byte[] first, byte[] second)
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
        public static string SupplementZero(string input, int length, bool flag)
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
        public static string SortStringArray(string[] array, bool flag)
        {
            //从小到大
            if (flag)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    for (int j = 0; j < array.Length; j++)
                    {
                        if (HexStrToInt(array[i]) < HexStrToInt(array[j]))
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
                        if (HexStrToInt(array[i]) > HexStrToInt(array[j]))
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

        /// <summary>
        /// HexStr转Int
        /// </summary>
        /// <param name="hextr"></param>
        /// <returns></returns>
        public static int HexStrToInt(string hexStr)
        {
            return Int32.Parse(hexStr, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        /// <summary>
        /// 每隔n个字符插入一个字符
        /// </summary>
        /// <param name="input"></param>
        /// <param name="interval"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        public static string StrAddCharacter(string input, int interval, string character)
        {
            for (int i = interval; i < input.Length; i += interval + 1)
                input = input.Insert(i, character);
            return input;
        }

        public static string CharAt(string input, int index)
        {
            if ((index >= input.Length) || (index < 0))
                return "";
            return input.Substring(index, 1);
        }

        /// <summary>
        /// HexStr转Str
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string HexStrToStr(string input)
        {
            StringBuilder stringBuilder = new StringBuilder();
            while (input.Length > 0)
            {
                stringBuilder.Append(Convert.ToChar(Convert.ToUInt32(input.Substring(0, 2), 16)).ToString());
                input = input.Substring(2);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// str转hexStr
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StrToHexStr(string input)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char chr in input)
            {
                stringBuilder.Append(String.Format("{0:X2}", Convert.ToInt32(chr)));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// HexStr转byte[]
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] HexStrToBytes(string input)
        {
            input = input.Replace(" ", "");
            int len = input.Length;
            if (len % 2 != 0)
            {
                throw new Exception("输入的字符串长度有误，必须是偶数。");
            }
            byte[] bytes = new byte[len / 2];
            for (int i = 0; i < len / 2; i++)
            {
                if (!byte.TryParse(input.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber, null, out bytes[i]))
                {
                    throw new Exception(string.Format("在位置{0}处的字符无法转换为16进制字节", i * 2 + 1));
                }
            }
            return bytes;
        }

        /// <summary>
        /// byte[]转str
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string BytesToStr(byte[] input)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in input)
            {
                stringBuilder.Append(b.ToString("X2"));
            }
            return stringBuilder.ToString();
        }
    }

}
