using HelloCSharp.MySQL;
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
            MySqlUtil mySqlUtil = new MySqlUtil();
            Application.Run(new LoginAndRegisterWindow());
            //Application.Run(new MainForm());
            //Application.Run(new FileB());
        }

    }
}
