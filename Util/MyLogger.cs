using System;
using System.IO;
using System.Reflection;

namespace HelloCSharp.Log
{
    public enum LogType
    {
        All,
        Information,
        Debug,
        Success,
        Failure,
        Warning,
        Error
    }

    public class MyLogger
    {
        #region Instance
        private static object _logLock;

        private static MyLogger _logger;

        private MyLogger()
        {
        }

        public static MyLogger Instance
        {
            get
            {
                if (null == _logger)
                {
                    _logger = new MyLogger();
                    _logLock = new object();
                }
                return _logger;
            }
        }
        #endregion

        /// <summary>
        /// 生成Log日志
        /// </summary>
        /// <param name="logContent"></param>
        /// <param name="logType"></param>
        public void WriteLog(string logContent, LogType logType = LogType.Information)
        {
            try
            {
                Console.WriteLine(logContent);
                //当前exe文件执行的路径
                string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                //重定位到E盘目录下，上线时注释掉这句
                basePath = @"E:\MyLogs\HelloCSharp";
                string fileName = DateTime.Now.ToString("yyyy_MM_dd") + ".log";
                //创建目录
                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);
                //日志格式
                string[] logText = new string[] { DateTime.Now.ToString("hh:mm:ss") + ":" + logType.ToString() + ":" + logContent };
                lock (_logLock)
                {
                    File.AppendAllLines(basePath + "\\" + fileName, logText);
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 异常日志
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="specialText">特别说明</param>
        public void WriteException(Exception exception, string specialText = null)
        {
            if (exception != null)
            {
                Type exceptionType = exception.GetType();
                string text = string.Empty;
                text = "Exception: " + exceptionType.Name + Environment.NewLine;
                text += "               " + "Message: " + exception.Message + Environment.NewLine;
                text += "               " + "Source: " + exception.Source + Environment.NewLine;
                text += "               " + "StackTrace: " + exception.StackTrace + Environment.NewLine;
                if (!string.IsNullOrEmpty(specialText))
                {
                    specialText = "               " + specialText;
                    text = text + specialText + Environment.NewLine;
                }
                WriteLog(text, LogType.Error);
            }
        }

    }
}