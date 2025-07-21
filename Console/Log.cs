#if UNITY_EDITOR
using System;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.Framework.Console
{
    [Serializable]
    public class Log
    {
        public string message;
        public string[] logTags;
        public string stackTrace;
        public string time;
        public bool select;
        public LogType logType;
        public Rect rect;

        private static StackTrace s_stacktrace;

        public Log(string message, string stackTrace, string[] logTags, string time, bool select, LogType logType)
        {
            this.message = message;
            this.logTags = logTags;
            this.stackTrace = stackTrace;
            this.time = time;
            this.select = select;
            this.logType = logType;
        }

        public static void Message(object msg, params object[] formatValues)
        {
            s_stacktrace = new StackTrace(true);
            SendMessage(msg, new[] { "Custom" }, LogType.Log, formatValues);
        }

        public static void Message(object msg, string logTag, LogType logType = LogType.Log,
            params object[] formatValues)
        {
            s_stacktrace = new StackTrace(true);
            SendMessage(msg, new[] { logTag }, logType, formatValues);
        }

        public static void Message(object msg, string[] logTags, LogType logType = LogType.Log,
            params object[] formatValues)
        {
            s_stacktrace = new StackTrace(true);
            SendMessage(msg, logTags, logType, formatValues);
        }

        private static void SendMessage(object msg, string[] logTags, LogType logType, params object[] formatValues)
        {
            if (formatValues.Length > 0)
            {
                msg = string.Format(msg.ToString(), formatValues);
            }

            var time = $"[{DateTime.Now.TimeOfDay:hh\\:mm\\:ss}] ";
            var lastFrame = s_stacktrace.GetFrame(1);

            var objectName = lastFrame.GetMethod().ReflectedType!.Name; // ! = Removed possible null warning
            var methodName = lastFrame.GetMethod().Name;

            const string sendMessageMethod = "CustomConsole.Log:SendMessage(object,string[],LogType,object[])\n";
            var logCaller = $"{objectName}:{methodName}()";

            var fileName = lastFrame.GetFileName();
            var lineNumber = lastFrame.GetFileLineNumber();

            string filePath = string.Empty;
            if (!string.IsNullOrEmpty(fileName))
            {
                var startIndex = fileName.IndexOf("Assets", StringComparison.Ordinal);
                var substring = fileName[startIndex..];
                filePath = substring.Replace(@"\", "/");
            }

            // TODO: Smaller text size 
            var hyperlink = sendMessageMethod + logCaller + $" (at <a href=\"{filePath}\" line=\"{lineNumber}\">{filePath}:{lineNumber}</a>)\n";
            var customLog = new Log(msg.ToString(), hyperlink, logTags, time, false, logType);
            CustomConsole.LogMessage(customLog);
        }
    }
}
#endif