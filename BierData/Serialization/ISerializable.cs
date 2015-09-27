using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Data.Serialization
{
    public interface ISerializable : System.Runtime.Serialization.ISerializable
    {
        /// <summary>
        /// Saves the object to a file the object.
        /// </summary>
        /// <param name="fileStream">FileStream to save the object to</param>
        void AddToFile(FileStream fileStream);
    }
}
