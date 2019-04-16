using HelloCSharp.HTTP;
using System.Net;

namespace HelloCSharp
{
    class GetSms
    {
        public const string URL = "http://localhost:8080/HelloJavaServer/SMS";
        public string _code;

        public GetSms()
        {
        }

        public GetSms(string phoneNumber)
        {
            HttpWebResponse response = HttpPostUtil.HttpPost(URL, phoneNumber);
            _code = HttpPostUtil.GetResponseStr(response);
        }
    }
}
