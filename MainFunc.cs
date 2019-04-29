using HelloCSharp.UI;
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

            Application.Run(new LoginAndRegisterWindow());

            //Application.Run(new TestCOMWindow());
            //Application.Run(new TestIMEIWindow());

        }

    }
}
