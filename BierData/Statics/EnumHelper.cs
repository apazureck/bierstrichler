using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Functional
{
    public static class EnumHelper
    {
        public static string GetDescription(Enum value)
        {
            MemberInfo[] mi = value.GetType().GetMember(value.ToString());
            DescriptionAttribute da = (DescriptionAttribute)mi[0].GetCustomAttribute(typeof(DescriptionAttribute), false);
            return da.Description;
        }

        public static Enum GetValueOfDescription(string description, Type enumType)
        {
            MemberInfo[] mis = enumType.GetMembers(BindingFlags.GetField | BindingFlags.Public | BindingFlags.Static);
            List<string> descriptions = new List<string>(mis.Length);
            foreach (MemberInfo mi in mis)
                descriptions.Add(((DescriptionAttribute)mi.GetCustomAttribute(typeof(DescriptionAttribute), false)).Description);
            return (Enum)Enum.ToObject(enumType, descriptions.IndexOf(description));
        }
    }
}
