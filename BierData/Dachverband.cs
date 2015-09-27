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
    [Serializable]
    public class Dachverband
    {
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Uri zum Zirkel der Verbindung
        /// </summary>
        public string WappenRelativeImagePath { get; set; }

        /// <summary>
        /// Image Uri
        /// </summary>
        [IgnoreProperty]
        public Uri WappenImageUri
        {
            get
            {
                if (WappenRelativeImagePath != null)
                    return new Uri(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), WappenRelativeImagePath), UriKind.Absolute);
                else
                    return null;
            }
        }

        string Wahlspruch { set; get; }
    }
}
