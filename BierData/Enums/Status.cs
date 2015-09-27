using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Data.Enums
{
    [Serializable]
    public enum Status
    {
        [Description("Unbekannt")]
        Unknown,
        [Description("Gast")]
        Gast,
        [Description("Verkehrsgast")]
        Verkehrsgast,
        [Description("Zeitweiliges Mitglied")]
        ZMer,
        [Description("Fuchs")]
        Fux,
        [Description("Bursch")]
        Bursch,
        [Description("Philister")]
        Philister,
        [Description("Bandphilister")]
        Bandphilister,
        [Description("Ehrenmitglied")]
        Ehrenmitglied
    }
}
