using HelloCSharp.UI;
using HelloCSharp.Util;
using System;
using System.Windows.Forms;

namespace HelloCSharp
{
    class MainFunc
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Zm301TestWidnow());

            //Console.WriteLine((Convert.ToInt32("F2", 16) - 33).ToString("x"));

            //string testStr = "68 53 01 80 01 01 00 68 8A 16 33 36 7B 49 46 45 53 33 33 33 33 33 33 33 33 6E 33 33 33 33 33 47";
            //testStr = testStr.Replace(" ", "");
            //string[] strs = MyConvertUtil.StrSplitInterval(testStr, 2);
            //string tempStr = "";
            //foreach (string temp in strs)
            //{
            //    Console.WriteLine("分割：" + temp);
            //    tempStr += temp;
            //}
            //Console.WriteLine("分割后还原：" + tempStr.Equals(testStr));
            //Console.WriteLine("---------------------------------------------------------------------------");
            //Console.WriteLine("校验码：" + MyConvertUtil.CalcZM301CRC(testStr));
        }

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
    }
}
