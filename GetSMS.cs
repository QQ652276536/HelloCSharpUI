using HelloCSharp.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HelloCSharp
{
    class GetSms
    {
        public const String URL = "http://localhost:8080/HelloJavaServer/SMS";
        public String _code;

        public GetSms()
        {
        }

        public GetSms(String phoneNumber)
        {
            HttpWebResponse response = HttpPostUtil.HttpPost(URL, phoneNumber);
            _code = HttpPostUtil.GetResponseStr(response);
        }
    }
}
