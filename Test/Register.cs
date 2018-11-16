using HelloCSharp.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloCSharp.Test
{
    class Register
    {
        public Register()
        {
        }

        public Register(Dictionary<String, String> dictionary)
        {
            HttpWebResponse response = HttpPostUtil.HttpPost("", dictionary);
            String responseStr = HttpPostUtil.GetResponseStr(response);
            MessageBox.Show(responseStr);
        }
    }
}
