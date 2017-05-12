using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Xml.Serialization
{
    internal static class XmlArrayItemAttributeExtensions
    {
        internal static bool GetIsNullableSpecified(this XmlArrayItemAttribute xmlArrayItemAtt)
        {
            return (bool)xmlArrayItemAtt.GetType().GetProperty("IsNullableSpecified", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(xmlArrayItemAtt);
        }
    }
}
