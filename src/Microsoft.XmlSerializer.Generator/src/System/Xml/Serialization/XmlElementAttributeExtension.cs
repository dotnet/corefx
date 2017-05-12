using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Xml.Serialization
{
    internal static class XmlElementAttributeExtension
    {
        internal static bool GetIsNullableSpecified(this XmlElementAttribute xmlElementAtt)
        {
            return (bool)xmlElementAtt.GetType().GetProperty("IsNullableSpecified", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(xmlElementAtt);
        }
    }
}
