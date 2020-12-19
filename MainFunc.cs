using HelloCSharp.UI;
using HelloCSharp.Util;
using System;
using System.Linq;
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

        }
    }
}
