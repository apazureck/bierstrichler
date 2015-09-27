using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Data.Global
{
    public enum LogLevel
    {
        [Description("Error")]
        Error,
        [Description("Warning")]
        Warning,
        [Description("Information")]
        Information,
        [Description("Debug")]
        Debug
    }
}
