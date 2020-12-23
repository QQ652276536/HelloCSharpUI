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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Zm301SimpleTest());

            string str = "000003F3F68!@#$%16^&*68_+160000";
            str = "681612";
            //int beginIndex = str.IndexOf("68");
            //if (beginIndex > 0)
            //    str = str.Substring(beginIndex);
            //int endIndex = str.LastIndexOf("16");
            //if (endIndex > 0)
            //    str = str.Substring(0, endIndex + 2);
            //Console.WriteLine(str);
        }
    }
}
