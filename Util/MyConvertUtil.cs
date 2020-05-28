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
        /// 字符串转字节数组
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
        /// 字节数组转字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string BytesToString(byte[] input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in input)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }

    }
}
