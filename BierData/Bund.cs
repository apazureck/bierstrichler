using Bierstrichler.Data.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Bierstrichler.Data
{
    /// <summary>
    /// Klasse enthält Informationen zu einer Verindung
    /// </summary>
    [Serializable]
    public class Bund
    {
        /// <summary>
        /// Bezeichnung der Verbindung (z.B. KDSTV)
        /// </summary>
        [XmlAttribute]
        public string Bezeichner { get; set; }
        /// <summary>
        /// Name der Verbindung
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }
        /// <summary>
        /// Stadt der Verbindung
        /// </summary>
        [XmlAttribute]
        public string Stadt { get; set; }
        /// <summary>
        /// Dachverband der Verbindung
        /// </summary>
        public Dachverband Dachverband { get; set; }
        /// <summary>
        /// Farben der Verbindung
        /// </summary>
        [XmlAttribute]
        public string Farben { get; set; }
        /// <summary>
        /// Wahlspruch der Verbindung
        /// </summary>
        [XmlAttribute]
        public string Wahlspruch { get; set; }
        /// <summary>
        /// Relativer Pfad zum Bild des Gegenstands
        /// </summary>
        public string ZirkelRelativeImagePath { get; set; }

        /// <summary>
        /// Image Uri
        /// </summary>
        [IgnoreProperty]
        public Uri ZirkelImageUri
        {
            get
            {
                if (ZirkelRelativeImagePath != null)
                    return new Uri(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), ZirkelRelativeImagePath), UriKind.Absolute);
                else
                    return null;
            }
        }
        DateTime Gruendungsdatum { get; set; }
    }
}
