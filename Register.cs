using HelloCSharp.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloCSharp
{
    class Register
    {
        public const String _URL = "http://localhost:8080/HelloJavaServer/Register";
        public Register()
        {
        }

        public Register(String param)
        {
            HttpWebResponse response = HttpPostUtil.HttpPost(_URL, param);
            String responseStr = HttpPostUtil.GetResponseStr(response);
            MessageBox.Show(responseStr);
        }

        public Register(Dictionary<String, String> dictionary)
        {
            HttpWebResponse response = HttpPostUtil.HttpPost(_URL, dictionary);
            String responseStr = HttpPostUtil.GetResponseStr(response);
            MessageBox.Show(responseStr);
        }
    }
}
