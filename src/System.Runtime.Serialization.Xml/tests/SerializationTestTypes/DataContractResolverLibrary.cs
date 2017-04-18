using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;

namespace SerializationTestTypes
{
    [Serializable]
    public class PrimitiveTypeResolver : DataContractResolver
    {
        private readonly static string s_defaultNS = "http://www.default.com";
        private readonly static Dictionary<string, Type> s_types = new Dictionary<string, Type>();

        public override bool TryResolveType(Type dcType, Type declaredType, DataContractResolver KTResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            string resolvedTypeName = string.Empty;
            string resolvedNamespace = string.Empty;
            resolvedNamespace = s_defaultNS;
            resolvedTypeName = dcType.Name + "_foo";
            s_types[resolvedTypeName] = dcType;
            XmlDictionary dic = new XmlDictionary();
            typeName = dic.Add(resolvedTypeName);
            typeNamespace = dic.Add(resolvedNamespace);
            return true;
        }

        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver KTResolver)
        {
            return s_types[typeName];
        }
    }
}

