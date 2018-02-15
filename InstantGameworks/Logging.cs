#define PrintLogToOutput
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace InstantGameworks
{
    class Logging
    {
        public static int LogCount = 0;
        public static string LogFile = null;
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

        public static List<Event?> FullEventLog = new List<Event?>();
        public static List<Error?> FullErrorLog = new List<Error?>();

        private static string FormatSmallDate(DateTime? date)
        {
            string newDate = date?.Hour + ":" + string.Format("{0:00}", date?.Minute) + ":" + string.Format("{0:00}", date?.Second) + "." + string.Format("{0:000}", date?.Millisecond);
            return newDate;
        }

        [Conditional("PrintLogToOutput")]
        private static void PrintToOutput(Event logEvent)
        {
            Console.WriteLine("[" + logEvent.LogMessageNumber + "] " + FormatSmallDate(logEvent.DateTime) + ": " + logEvent.Message);
        }

        [Conditional("PrintLogToOutput")]
        private static void PrintToOutput(Error logError)
        {
            Console.WriteLine("[" + logError.LogMessageNumber + "] " + FormatSmallDate(logError.DateTime) + ": " + logError.Exception + " at " + logError.TraceSource + " - " + logError.StackTrace);
        }

        public static void LogEvent(string message)
        {
            Event newEvent = new Event(DateTime.Now, message);
            PrintToOutput(newEvent);
            FullEventLog.Insert(LogCount - 1, newEvent);
        }

        public static void LogError(DateTime time, TraceSource source, Exception exception, StackTrace stack)
        {
            Error newError = new Error(time, source, exception, stack);
            PrintToOutput(newError);
            FullErrorLog.Insert(LogCount - 1, newError);
            WriteToFile();
        }

        private static void _displayLog()
        {
            Form newForm = new Form();
            newForm.Width = 600;
            newForm.Height = 600;
            newForm.Text = "Instant Gameworks Event Log";
            newForm.Icon = new System.Drawing.Icon(@"Extra\InstantGameworks.ico");
            newForm.Focus();
            MenuStrip tools = new MenuStrip();
            tools.Top = 0;
            tools.Parent = newForm;
            TextBox logDisplay = new TextBox();
            void updateHeight(object sender, EventArgs e)
            {
                logDisplay.Width = newForm.Width;
                logDisplay.Height = newForm.Height;
                logDisplay.Top = tools.Height;
            }
            updateHeight(null, EventArgs.Empty);
            newForm.SizeChanged += updateHeight;
            logDisplay.BackColor = System.Drawing.Color.White;
            logDisplay.Multiline = true;
            logDisplay.ScrollBars = ScrollBars.Vertical;
            logDisplay.ReadOnly = true;

            if (LogFile != null)
            {
                foreach (string line in File.ReadAllLines(LogFile))
                {
                    logDisplay.AppendText(line);
                    logDisplay.AppendText(Environment.NewLine);
                }
            }
            logDisplay.Parent = newForm;

            newForm.ShowDialog();
        }

        public static Thread DisplayLog()
        {
            Thread newThread = new Thread(_displayLog);
            newThread.Start();
            return newThread;
        }

        public static void WriteToFile()
        {
            DateTime now = DateTime.Now;
            string fileName = now.Year + "-" + now.Month + "-" + now.Day + "_" + now.Hour + "-" + string.Format("{0:00}", now.Minute) + "-" + string.Format("{0:00}", now.Second) + "-" + string.Format("{0:000}", now.Millisecond) + ".txt";
            LogFile = fileName;
            using (Stream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    writer.WriteLine("Instant Gameworks (c)  2018");
                    writer.WriteLine("Exit Time: " + now.ToShortDateString() + " " + FormatSmallDate(now));
                    writer.WriteLine("Event Count: " + LogCount);
                    writer.WriteLine();
                    writer.WriteLine("Start of log");
                    for (int logLength = 0; logLength < LogCount; logLength++)
                    {
                        if (FullEventLog.Count > logLength && FullEventLog[logLength] != null)
                        {
                            Event? current = FullEventLog[logLength];
                            writer.WriteLine("[" + current?.LogMessageNumber + "] " + FormatSmallDate(current?.DateTime) + ": " + current?.Message);
                        }
                        if (FullErrorLog.Count > logLength && FullErrorLog[logLength] != null)
                        {
                            Error? current = FullErrorLog[logLength];
                            writer.WriteLine("[" + current?.LogMessageNumber + "] " + FormatSmallDate(current?.DateTime) + ": " + current?.Exception + " at " + current?.TraceSource + " - " + current?.StackTrace);
                        }
                    }
                    writer.WriteLine("End of log");
                }
            }
        }
    }
}
