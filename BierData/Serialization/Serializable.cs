using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Data.Serialization
{
    /// <summary>
    /// Base class for serialization.
    /// </summary>
    public abstract class Serializable : ISerializable, ICloneable
    {
        public Serializable()
        {

        }

        public Serializable(SerializationInfo info, StreamingContext context)
        {
            GetProperties(info, context);
        }
        /// <summary>
        /// Adds the current object to the file using all public properties which are not ignored.
        /// </summary>
        /// <param name="fileStream">File stream to add the object to.</param>
        public virtual void AddToFile(System.IO.FileStream fileStream)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fileStream, this);
        }

        /// <summary>
        /// This Method will add all serialization information for public properties of the current class. The method will ignore all Properties holding the <see cref="IgnorePropertyAttribute"/>
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo to populate with data.</param>
        /// <param name="context">The destination <see cref="System.Runtime.Serialization.StreamingContext"/> for this serialization.</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            PropertyInfo[] pia = this.GetType().GetProperties();
            // Fügt alle properties hinzu, die nicht auf der Ingorelist stehen.
            foreach (PropertyInfo pi in pia)
                try
                {
                    if (!Attribute.IsDefined(pi, typeof(IgnorePropertyAttribute)))
                        info.AddValue(pi.Name, pi.GetValue(this));
                }
                catch { }

        }

        /// <summary>
        /// Gets the object data from the serializationinfo via reflection.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo to populate with data.</param>
        /// <param name="context">The destination <see cref="System.Runtime.Serialization.StreamingContext"/> for this serialization.</param>
        protected virtual void GetProperties(SerializationInfo info, StreamingContext context)
        {
            PropertyInfo[] pia = this.GetType().GetProperties();
            // Fügt alle properties hinzu, die nicht auf der Ingorelist stehen.
            foreach (PropertyInfo pi in pia)
                if (!Attribute.IsDefined(pi, typeof(IgnorePropertyAttribute)))
                    pi.SetValue(this, info.GetValue(pi.Name, pi.PropertyType));
        }

        /// <summary>
        /// Implemented ICloneable
        /// </summary>
        /// <returns>Clone of the current object.</returns>
        public abstract object Clone();

        /// <summary>
        /// Kopiert alle übereinstimmenden Properties von <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Object to get the values from</param>
        protected virtual void GetProperties(Serializable source)
        {
            PropertyInfo[] pia = source.GetType().GetProperties();
            // Fügt alle properties hinzu, die nicht auf der Ingorelist stehen.
            foreach (PropertyInfo pi in pia)
                if (!Attribute.IsDefined(pi, typeof(IgnorePropertyAttribute)))
                    try
                    {
                        pi.SetValue(this, pi.GetValue(source));
                    }
                    catch { }
        }
    }
}
