using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;

namespace DesktopTestData
{
    [Serializable]
    public class PrimitiveTypeResolver : DataContractResolver
    {
        private readonly static string DefaultNS = "http://www.default.com";
        private readonly static Dictionary<string, Type> Types = new Dictionary<string, Type>();

        public override bool TryResolveType(Type dcType, Type declaredType, DataContractResolver KTResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            string resolvedTypeName = string.Empty;
            string resolvedNamespace = string.Empty;
            resolvedNamespace = DefaultNS;
            resolvedTypeName = dcType.Name + "_foo";
            Types[resolvedTypeName] = dcType;
            XmlDictionary dic = new XmlDictionary();
            typeName = dic.Add(resolvedTypeName);
            typeNamespace = dic.Add(resolvedNamespace);
            return true;
        }

        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver KTResolver)
        {
            return Types[typeName];
        }
    }
}

