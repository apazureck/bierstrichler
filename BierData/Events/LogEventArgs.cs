using Bierstrichler.Data.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bierstrichler.Data.Events
{
    public class LogEventArgs : EventArgs
    {
        public LogEntry Entry { get; set; }

        public LogEventArgs(LogEntry e = null)
        {
            if (e == null)
                Entry = new LogEntry();
            else
                Entry = e;
        }
    }
}
