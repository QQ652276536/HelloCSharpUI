using HelloCSharp.MySQL;
using HelloCSharp.MySQL.MySQLPool;
using HelloCSharp.UI;
using System;
using System.IO.Ports;
using System.Windows.Forms;
using TestCOM;
using TestIMEI;

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

            //Application.Run(new TestComWindow());
            Application.Run(new TestIMEIWindow());

        }

    }
}
