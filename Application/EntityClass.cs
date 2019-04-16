using Newtonsoft.Json;
using System.Collections.Generic;

namespace HelloCSharp
{
    public class EntityClass
    {
    }

    public class UserInfo
    {
        public int _id;
        public int _deleteFlag;
        public string _userName;
        public string _userPwd;
        public string _userPhone;
        public int _verifyCode;
        public string _registerTime;
        public int _rank;
    }

    public class EventEntity
    {
        public string listen;
        public ScriptEntity script;
    }

    public class ScriptEntity
    {
        public List<string> exec;
        public string type = "text/javascript";
    }

    public class ResponseEntity
    {
        public List<string> _strList;
    }

    public class URLEntity
    {
        public string raw;
        [Newtonsoft.Json.JsonIgnore()]
        public string protocol;
        public List<string> host;
        [Newtonsoft.Json.JsonIgnore()]
        public string port;
        public List<string> path;
    }

    public class BodyEntity
    {
        public string mode;
        public string raw;
    }

    public class HeaderEntity
    {
        public string key;
        public string value;
    }

    public class RequestEntity
    {
        public string method;
        public List<HeaderEntity> header;
        public BodyEntity body;
        public URLEntity url;
    }

    public class HttpEntity
    {
        public string name;
        [JsonProperty(PropertyName = "event")]
        public List<EventEntity> eventEntity;
        public RequestEntity request;
        public List<string> response;
    }

    public class InfoEntity
    {
        public string _postman_id;
        public string name;
        public string schema;
    }

    public class MyJsonClass
    {
        public InfoEntity info;
        public List<HttpEntity> item;
    }
}
