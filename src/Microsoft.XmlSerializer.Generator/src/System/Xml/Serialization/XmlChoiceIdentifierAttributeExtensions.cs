using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Xml.Serialization
{
    internal static class XmlChoiceIdentifierAttributeExtensions
    {
        internal static MemberInfo GetMemberInfo(this XmlChoiceIdentifierAttribute xmlChoiceIdentifierAtt)
        {
            return (MemberInfo)xmlChoiceIdentifierAtt.GetType().GetProperty("MemberInfo", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(xmlChoiceIdentifierAtt);
        }

        internal static void SetMemberInfo(this XmlChoiceIdentifierAttribute xmlChoiceIdentifierAtt, MemberInfo memberInfo)
        {
            xmlChoiceIdentifierAtt.GetType().GetProperty("MemberInfo", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(xmlChoiceIdentifierAtt, memberInfo);
        }
    }
}
