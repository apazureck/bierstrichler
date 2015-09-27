using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Bierstrichler.Data
{
    [Serializable]
    public class Address
    {
        public Address() 
        {
            Street = "";
            ZipCode = "";
            City = "";
            Country = "";
        }

        public Address(string addressString) : this()
        {
            FromString(addressString);
        }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        /// <summary>
        /// Converts a Address from a in the form of "Street, ZipCode City, Country"
        /// </summary>
        /// <param name="addressString"></param>
        public void FromString(string addressString)
        {
            string[] splitAddress = addressString.Split(',');
            try { Street = splitAddress[0].Trim(); } catch { }
            try { ZipCode = Regex.Match(splitAddress[1], @"\d{5}").Value; } catch { }
            try { City = Regex.Match(splitAddress[1], @"\b[\p{L}]+$\b").Value; } catch { }
            try { Country = splitAddress[2].Trim(); } catch { }
        }

        public override string ToString()
        {
            string ret = "";
            bool noStreet = Street.Equals("");
            bool noZip = ZipCode.Equals("");
            bool noCity = City.Equals("");
            bool noCountry = Country.Equals("");

            if (!noStreet)
                ret += Street + (noZip && noCity && noCountry ? "" : ", ");
            if (!noZip)
                ret += ZipCode + (noCity ? (noCountry ? "" : ", ") : " ");
            if (!noCity)
                ret += City + (noCountry ? "" : ", ");
            if (!noCountry)
                ret += Country;
            return ret;
        }
    }
}
