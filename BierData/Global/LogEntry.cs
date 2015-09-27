using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bierstrichler.Data.Global
{
    [Serializable]
    public class LogEntry : Serialization.Serializable
    {
        public LogEntry() { }
        public LogEntry(string line)
        {
            throw new NotImplementedException();
        }

        public LogEntry(LogEntry origin)
        {
            GetProperties(origin);
        }

        public LogEntry(LogLevel logLevel, string message, DateTime date)
        {
            Level = logLevel;
            Message = message;
            Date = date;
        }

        public LogLevel Level { get; private set; }
        public string Message { get; private set; }
        public DateTime Date { get; private set; }

        public override object Clone()
        {
            return new LogEntry(this);
        }

        public override string ToString()
        {
            return DateTime.Now.ToString() + " " + LogLevelTag(Level) + Message;
        }

        /// <summary>
        /// Schreibt an den Anfang der Nachricht das LogLevel.
        /// </summary>
        /// <param name="logLevel">Debug, Information, Warning, Error</param>
        /// <returns></returns>
        private static string LogLevelTag(LogLevel logLevel)
        {
            return Enum.GetName(typeof(LogLevel), logLevel) + ": ";
        }
    }
}
