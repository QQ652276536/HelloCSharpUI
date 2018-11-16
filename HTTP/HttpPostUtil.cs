using HelloCSharp.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace HelloCSharp.HTTP
{
    class HttpPostUtil
    {
        public static HttpWebResponse HttpPost(String url, IDictionary<String, String> parameters)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                if (parameters == null || parameters.Count == 0)
                {
                    return null;
                }
                StringBuilder builder = new StringBuilder();
                bool appendFlag = false;
                foreach (String key in parameters.Keys)
                {
                    if (!appendFlag)
                    {
                        builder.AppendFormat("{0}={1}", key, parameters[key]);
                        appendFlag = true;
                    }
                    else
                    {
                        builder.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                }
                byte[] bytes = Encoding.UTF8.GetBytes(builder.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
                return request.GetResponse() as HttpWebResponse;
            }
            catch (WebException e)
            {
                Logger.Instance.WriteException(e);
                return null;
            }
        }

        public static String GetResponseStr(HttpWebResponse response)
        {
            if (response == null)
            {
                return "-1";
            }
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }

    }
}
