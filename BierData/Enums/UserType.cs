using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Data.Enums
{
    [Serializable]
    public enum UserType
    {
        [Description("Gast")]
        Gast,
        [Description("Korporierter")]
        Korpo,
        [Description("Bundesbruder")]
        Bundesbruder,
        [Description("Cartellbruder")]
        Cartellbruder,
        [Description("Spezial")]
        Special
    }
}
