using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using CompanyAPI.Domain;

namespace CompanyAPI.Services
{
    public static class LogHelper
    {
        private const string DateTimeStandardFormat = "MM/dd/yyyy hh:mm:ss:fff tt";
        private const string DateTimeMilitaryFormat = "MM/dd/yyyy HH:mm:ss:fff";        

        /// <summary>
        /// Returns the log file path.
        /// </summary>
        /// <returns></returns>
        public static string GetLogFilePath()
        {
            string appDir = Directory.GetCurrentDirectory();
            string appName = Assembly.GetExecutingAssembly().GetName().Name;
            string fileName = string.Concat(appName, ".log");
            return Path.Combine(appDir, fileName);
        }

        /// <summary>
        /// Writes the message to the log file. Optionally prepend timestamp.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="timeFormat"></param>
        /// <param name="timeZone"></param>
        public static void Write(string message, TimeFormat timeFormat = TimeFormat.Standard, Domain.TimeZone timeZone = Domain.TimeZone.Local)
        {
            WriteToLog(message, timeFormat, timeZone, false);
        }

        /// <summary>
        /// Writes the message, followed by the current line terminator, to the log file. Optionally prepend timestamp.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="timeFormat"></param>
        /// <param name="timeZone"></param>
        public static void WriteLine(string message, TimeFormat timeFormat = TimeFormat.Standard, Domain.TimeZone timeZone = Domain.TimeZone.Local)
        {
            WriteToLog(message, timeFormat, timeZone, true);
        }

        private static void WriteToLog(string message, TimeFormat timeFormat, Domain.TimeZone timeZone, bool newline)
        {
            try
            {
                var sb = new StringBuilder();
                DateTime dtNow = timeZone == Domain.TimeZone.Utc ? DateTime.Now.ToUniversalTime() : DateTime.Now;

                if (timeFormat == TimeFormat.Standard)
                    sb.Append(dtNow.ToString(DateTimeStandardFormat) + " ");
                else if (timeFormat == TimeFormat.Military)
                    sb.Append(dtNow.ToString(DateTimeMilitaryFormat) + " ");
                else if (timeFormat == TimeFormat.UnixTime)
                    sb.Append(UnixTimeHelper.ToUnixTime(DateTime.Now) + " ");

                sb.Append(message + (newline ? "\n" : ""));
                //if (Debugger.IsAttached) Console.WriteLine(sb.ToString());

                string filePath = GetLogFilePath();
                using (StreamWriter writer = File.AppendText(filePath))
                {
                    writer.Write(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}