using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Data.Events
{
    public delegate void PersonChangedEventHandler(object sender, PersonEventArgs e);
    public delegate void ItemChangedEventHandler(object sender, ItemEventArgs e);
    public delegate void ItemChangedEvent(object sender, ChangeEventArgs e);
    public delegate void LogEntryEvent(object sender, LogEventArgs e);
}
