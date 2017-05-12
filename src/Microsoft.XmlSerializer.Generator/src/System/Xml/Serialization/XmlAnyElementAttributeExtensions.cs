using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Xml.Serialization
{
    internal static class XmlAnyElementAttributeExtensions
    {
        internal static bool GetNamespaceSpecified(this XmlAnyElementAttribute xmlAnyElementAtt)
        {
            return (bool)xmlAnyElementAtt.GetType().GetProperty("NamespaceSpecified", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(xmlAnyElementAtt);
        }
    }
}
