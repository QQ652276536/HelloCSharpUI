using HelloCSharp.UI;
using HelloCSharp.Util;
using System;
using System.Windows.Forms;
using TestIMEI;
using TestTools;

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

            //Application.Run(new LoginAndRegisterWindow());
            //Application.Run(new MainWindow());
            //Application.Run(new FileAWindow());
            //Application.Run(new FileBWindow());

            //Application.Run(new TestCOMWindow());
            //Application.Run(new TestIMEIWindow());
            Application.Run(new MH1902());
        }

    }
}
