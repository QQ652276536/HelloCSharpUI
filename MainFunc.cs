using HelloCSharp.UI;
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
            //MySqlUtil mySqlUtil = new MySqlUtil();
            //EmailWindow window = new EmailWindow();
            Application.Run(new RegisterWindow());
        }

    }
}
