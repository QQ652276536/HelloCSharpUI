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
            Application.Run(new MainWindow());

            if (1 > 0)
            {
                return;
            }
            string str = "3F4300801001000B01650A1520797A70746573745F76312E315F32303035333000030AFFFFFFFFFFFFFFFFFFFF04016C0510FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE868";
            string[] strs = MyConvertUtil.StrAddCharacter(str, 2, ",").Split(',');
            string[] data = SplitArray(strs, 7, strs.Length - 1 - 2);
            string t;
            int l;
            string v;
            string txt = "";
            for (int i = 0; i < data.Length; i++)
            {
                //T
                t = data[i];
                string tStr;
                //DEVICEINFODICTIONARY.TryGetValue(MyConvertUtil.HexStrToInt(t), out tStr);
                txt += t + " ";
                //L
                l = MyConvertUtil.HexStrToInt(data[++i]);
                txt += l + " ";
                //V
                string[] vArray = SplitArray(data, i + 1, i + l);
                v = string.Join("", vArray);
                txt += MyConvertUtil.HexStrToStr(v) + "\r\n";
                i += l;
            }
            Console.WriteLine(txt);
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
