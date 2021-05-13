using HelloCSharp.UI;
using HelloCSharp.Util;
using System;
using System.Linq;
using System.Threading;
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

            Console.WriteLine(string.Join("", MyConvertUtil.StrSplitInterval("F0FF", 2)));
            string[] strs = { "一", "二", "三", "四", "五" };
            string[] pkgData = strs.Skip(2).Take(strs.Length - 3).ToArray();

            //每位都要减33（考虑有符号整数）
            string[] arrays = { "32", "33", "34" };
            for (int i = 0; i < arrays.Length; i++)
            {
                int num = Convert.ToInt32(arrays[i], 16) - 51;
                if (num < 0)
                {
                    arrays[i] = (128 + num | 128).ToString("X").PadLeft(2, '0');
                }
                else
                {
                    arrays[i] = num.ToString("X").PadLeft(2, '0');
                }
            }

            //防止程序被重复启动
            bool isRunning = false;
            Mutex mutex = new Mutex(true, System.Diagnostics.Process.GetCurrentProcess().ProcessName, out isRunning);
            if (!isRunning)
            {
                Environment.Exit(1);
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Main());
            Application.Run(new ImageTest());
        }
    }
}
