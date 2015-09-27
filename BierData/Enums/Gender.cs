using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Bierstrichler.Data.Enums
{
    /// <summary>
    /// Enum für das Geschlecht der Person
    /// </summary>
    [Serializable]
    public enum Gender
    {
        [Description("Männlich")]
        Male,
        [Description("Weiblich")]
        Female
    }
}
