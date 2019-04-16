using HelloCSharp.HTTP;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;

namespace HelloCSharp
{
    class Register
    {
        public const string _URL = "http://localhost:8080/HelloJavaServer/Register";
        public Register()
        {
        }

        public Register(string param)
        {
            HttpWebResponse response = HttpPostUtil.HttpPost(_URL, param);
            string responseStr = HttpPostUtil.GetResponseStr(response);
            MessageBox.Show(responseStr);
        }

        public Register(Dictionary<string, string> dictionary)
        {
            HttpWebResponse response = HttpPostUtil.HttpPost(_URL, dictionary);
            string responseStr = HttpPostUtil.GetResponseStr(response);
            MessageBox.Show(responseStr);
        }
    }
}
