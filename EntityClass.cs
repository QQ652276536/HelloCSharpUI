using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloCSharp
{
    public class EntityClass
    {
    }

    public class UserInfo
    {
        #region
        public int _id;
        public int _deleteFlag;
        public String _userName;
        public String _userPwd;
        public String _userPhone;
        public int _verifyCode;
        public String _registerTime;
        public int _rank;
        #endregion
    }

    public class ResponseEntity
    {
        #region
        public List<String> _strList;
        #endregion
    }

    public class URLEntity
    {
        #region
        public String raw;
        public String protocol;
        public List<String> host;
        public String port;
        public List<String> path;
        #endregion
    }

    public class BodyEntity
    {
        #region
        public String mode;
        public String raw;
        #endregion
    }

    public class HeaderEntity
    {
        #region
        public String key;
        public String value;
        #endregion
    }

    public class RequestEntity
    {
        #region
        public String method;
        public List<HeaderEntity> header;
        public BodyEntity body;
        public URLEntity url;
        #endregion
    }

    public class HttpEntity
    {
        #region
        public String name;
        public RequestEntity request;
        public List<String> response;
        #endregion
    }

    public class InfoEntity
    {
        #region
        public String _postman_id;
        public String name;
        public String schema;
        #endregion
    }

    public class MyJsonClass
    {
        #region
        public InfoEntity info;
        public List<HttpEntity> item;
        #endregion
    }
}
