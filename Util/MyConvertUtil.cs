using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloCSharp.Util
{
    class MyConvertUtil
    {
        private MyConvertUtil()
        {
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
        /// str转byte[]
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] StringToBytes(string input)
        {
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
        public static string BytesToString(byte[] input)
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
