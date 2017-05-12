using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Xml.Serialization
{
    internal static class XmlRootAttributeExtensions
    {
        internal static bool GetIsNullableSpecified(this XmlRootAttribute xmlRootAtt)
        {
            return (bool)xmlRootAtt.GetType().GetProperty("IsNullableSpecified", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(xmlRootAtt);
        }

        internal static string GetKey(this XmlRootAttribute xmlRootAtt)
        {
            return (string)xmlRootAtt.GetType().GetProperty("Key", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(xmlRootAtt);
        }
    }
}
