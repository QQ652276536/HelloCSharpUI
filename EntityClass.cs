using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HelloCSharp
{
    public class EntityClass
    {
    }

    #region
    public class UserInfo
    {
        public int _id;
        public int _deleteFlag;
        public String _userName;
        public String _userPwd;
        public String _userPhone;
        public int _verifyCode;
        public String _registerTime;
        public int _rank;
    }
    #endregion

    #region
    public class EventEntity
    {
        public String listen;
        public ScriptEntity script;
    }

    public class ScriptEntity
    {
        public List<String> exec;
        public String type = "text/javascript";
    }

    public class ResponseEntity
    {
        public List<String> _strList;
    }

    public class URLEntity
    {
        public String raw;
        [Newtonsoft.Json.JsonIgnore()]
        public String protocol;
        public List<String> host;
        [Newtonsoft.Json.JsonIgnore()]
        public String port;
        public List<String> path;
    }

    public class BodyEntity
    {
        public String mode;
        public String raw;
    }

    public class HeaderEntity
    {
        public String key;
        public String value;
    }

    public class RequestEntity
    {
        public String method;
        public List<HeaderEntity> header;
        public BodyEntity body;
        public URLEntity url;
    }

    public class HttpEntity
    {
        public String name;
        [JsonProperty(PropertyName = "event")]
        public List<EventEntity> eventEntity;
        public RequestEntity request;
        public List<String> response;
    }

    public class InfoEntity
    {
        public String _postman_id;
        public String name;
        public String schema;
    }

    public class MyJsonClass
    {
        public InfoEntity info;
        public List<HttpEntity> item;
    }
    #endregion
}
