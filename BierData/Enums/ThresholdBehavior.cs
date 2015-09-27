using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Bierstrichler.Data.Enums
{
    /// <summary>
    /// Zeigt an, welche Aktion durchgeführt werden soll, wenn der gewünschte Mindestbedarf unterschritten wurde.
    /// </summary>
    [Serializable]
    public enum ThresholdBehavior
    {
        /// <summary>
        /// Nichts durchführen
        /// </summary>
        [Description("Keine Aktion durchführen")]
        None,
        /// <summary>
        /// Sende Email an das angegebene Konto
        /// </summary>
        [Description("Email senden")]
        SendEmail,
        /// <summary>
        /// Sende SMS an die angegebene Nummer
        /// </summary>
        //[Description("SMS senden")]
        //SendSMS,
        /// <summary>
        /// Lade das Konto automatisch auf den gewünschten Autoload Betrag auf
        /// </summary>
        //[Description("Automatisch aufladen")]
        //AutoLoad,
        /// <summary>
        /// Rechnet einmal im Monat automatisch über z.B. PP ab.
        /// </summary>
        //[Description("Monatsrechnung")]
        //Bill
    }
}
