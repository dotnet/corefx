using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Xml.Serialization
{
    internal static class SoapAttributesExtensions
    {
        internal static SoapAttributeFlags GetSoapFlags(this SoapAttributes soapAtt)
        {
            return (SoapAttributeFlags)soapAtt.GetType().GetProperty("SoapFlags", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(soapAtt);
        }
    }

    internal enum SoapAttributeFlags
    {
        Enum = 0x1,
        Type = 0x2,
        Element = 0x4,
        Attribute = 0x8,
    }
}
