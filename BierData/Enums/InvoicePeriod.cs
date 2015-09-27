using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Data.Enums
{
    /// <summary>
    /// Enum zum Versenden der Rechnung
    /// </summary>
    public enum InvoicePeriod
    {
        [Description("Wöchentlich")]
        Weekly = 7,
        [Description("Monatlich")]
        Monthly = 1,
        [Description("Vierteljährlich")]
        Quarterly = 3,
        [Description("Jedes Trimester")]
        Trimester = 4,
        [Description("Jedes Semester")]
        Semester = 6,
        [Description("Jährlich")]
        Annual = 12
    }
}
