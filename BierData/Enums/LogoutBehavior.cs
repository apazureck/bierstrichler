using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Data.Enums
{
    [Serializable]
    public enum LogoutBehavior
    {
        [Description("Nie")]
        Never,
        [Description("Nach 10 Minuten")]
        TenMinutes,
        [Description("Nach 30 Minuten")]
        ThirtyMinutes,
        [Description("Nach einer Stunde")]
        OneHour,
        [Description("Nach drei Stunden")]
        ThreeHours,
        [Description("Am Nächsten Morgen um 8 Uhr")]
        TheNextMorning
    }
}
