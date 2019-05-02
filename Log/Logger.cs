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

    public class Logger
    {
        #region Instance
        private static object logLock;

        private static Logger logger;

        private static string logFileName;
        private Logger()
        {
        }

        /// <summary>
        /// Logger instance
        /// </summary>
        public static Logger Instance
        {
            get
            {
                if (logger == null)
                {
                    logger = new Logger();
                    logLock = new object();
                    logFileName = Guid.NewGuid() + ".log";
                }
                return logger;
            }
        }
        #endregion

        /// <summary>
        /// Write log to log file
        /// </summary>
        /// <param name="logContent">Log content</param>
        /// <param name="logType">Log type</param>
        public void WriteLog(string logContent, LogType logType = LogType.Information, string fileName = null)
        {
            try
            {
                string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                basePath = @"E:\MyLogs";
                if (!Directory.Exists(basePath + "\\Log"))
                {
                    Directory.CreateDirectory(basePath + "\\Log");
                }

                string dataString = DateTime.Now.ToString("yyyy-MM-dd");
                if (!Directory.Exists(basePath + "\\Log\\" + dataString))
                {
                    Directory.CreateDirectory(basePath + "\\Log\\" + dataString);
                }

                string[] logText = new string[] { DateTime.Now.ToString("hh:mm:ss") + ": " + logType.ToString() + ": " + logContent };
                if (!string.IsNullOrEmpty(fileName))
                {
                    fileName = fileName + "_" + logFileName;
                }
                else
                {
                    fileName = logFileName;
                }

                lock (logLock)
                {
                    File.AppendAllLines(basePath + "\\Log\\" + dataString + "\\" + fileName, logText);
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Write exception to log file
        /// </summary>
        /// <param name="exception">Exception</param>
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