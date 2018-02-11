#define PrintLogToOutput
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace InstantGameworks
{
    class Logging
    {
        public static int LogCount = 0;
        public interface ILogMessage
        {
            DateTime DateTime { get; }
            int LogMessageNumber { get; }
        }
        public struct Event : ILogMessage
        {
            public DateTime DateTime { get; }
            public string Message { get; }
            public int LogMessageNumber { get; }
            public Event(DateTime dateTime, string message)
            {
                DateTime = dateTime;
                Message = message;
                LogMessageNumber = ++LogCount;
            }
        }
        public struct Error : ILogMessage
        {
            public DateTime DateTime { get; }
            public TraceSource TraceSource { get; }
            public Exception Exception { get; }
            public StackTrace StackTrace { get; }
            public int LogMessageNumber { get; }
            public Error(DateTime dateTime, TraceSource traceSource, Exception exception, StackTrace stackTrace)
            {
                DateTime = dateTime;
                TraceSource = traceSource;
                Exception = exception;
                StackTrace = stackTrace;
                LogMessageNumber = ++LogCount;
            }
        }

        public static List<ILogMessage> FullApplicationLog = new List<ILogMessage>();

        [Conditional("PrintLogToOutput")]
        private static void PrintToOutput(Event logEvent)
        {
            Console.WriteLine("[" + logEvent.LogMessageNumber + "] " + logEvent.DateTime.ToShortTimeString() + ": " + logEvent.Message);
        }

        [Conditional("PrintLogToOutput")]
        private static void PrintToOutput(Error logError)
        {
            Console.WriteLine("[" + logError.LogMessageNumber + "] " + logError.DateTime.ToShortTimeString() + ": " + logError.Exception + " at " + logError.TraceSource + " - " + logError.StackTrace);
        }

        public static void LogEvent(string message)
        {
            Event newEvent = new Event(DateTime.Now, message);
            PrintToOutput(newEvent);
            FullApplicationLog.Add(newEvent);
        }

        public static void LogError(DateTime time, TraceSource source, Exception exception, StackTrace stack)
        {
            Error newError = new Error(time, source, exception, stack);
            PrintToOutput(newError);
            FullApplicationLog.Add(newError);
            WriteToFile();
        }

        public static void WriteToFile()
        {
            DateTime now = DateTime.Now;
            FileStream fileStream = new FileStream(now.ToString() + ".txt", FileMode.Create, FileAccess.Write, FileShare.None);
            StreamWriter writer = new StreamWriter(fileStream);
            writer.WriteLine("Instant Gameworks Error Log");
            writer.WriteLine("Time: " + now.ToString());
            writer.WriteLine("Event Count: " + FullApplicationLog.Count);
            foreach (ILogMessage message in FullApplicationLog)
            {
            }
        }
    }
}
