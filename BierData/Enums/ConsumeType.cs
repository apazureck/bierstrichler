using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Data.Enums
{
    [Serializable]
    public enum ConsumeType
    {
        [Description("Einkauf")]
        Purchase,
        [Description("Einzahlung")]
        Inpayment,
        [Description("Auszahlung")]
        Payout,
        [Description("Spezial")]
        SpecialPayment
    }
}
