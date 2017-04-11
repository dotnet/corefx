using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DesktopTestData
{
    [Serializable]
    public class SimpleResolver : DataContractResolver
    {
        public string defaultNS = "http://schemas.datacontract.org/2004/07/";

        TypeLibraryManager mgr = new TypeLibraryManager();

        public override bool TryResolveType(Type dcType, Type declaredType, DataContractResolver KTResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            System.Diagnostics.Debug.WriteLine("Resolving type {0}", dcType.Name);
            string resolvedTypeName = string.Empty;
            string resolvedNamespace = string.Empty;
            XmlDictionary dic = new XmlDictionary();

            if (mgr.AllTypesList.Contains(dcType))
            {
                resolvedTypeName = dcType.FullName + "***";
                resolvedNamespace = defaultNS + resolvedTypeName;
                typeName = dic.Add(resolvedTypeName);
                typeNamespace = dic.Add(resolvedNamespace);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Resolving type {0} using default KnownTypeResolver", dcType.FullName);
                KTResolver.TryResolveType(dcType, declaredType, null, out typeName, out typeNamespace);

            }
            if (typeName == null || typeNamespace == null)
            {
                System.Diagnostics.Debug.WriteLine("Resolving type {0} using default typename and assemblyName");
                XmlDictionary dictionary = new XmlDictionary();
                typeName = dictionary.Add(dcType.FullName);
                typeNamespace = dictionary.Add(dcType.Assembly.FullName);
            }
            return true;

        }

        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver KTResolver)
        {
            System.Diagnostics.Debug.WriteLine("Resolving Name {0}", typeName);
            TypeLibraryManager mgr = new TypeLibraryManager();
            string inputTypeName = typeName.Trim('*');
            Type result = null;
            if (null != mgr.AllTypesHashtable[inputTypeName])
            {
                result = (Type)mgr.AllTypesHashtable[inputTypeName];
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Resolving namespace {0} and type {1} using default KnownTypeResolver", typeNamespace, typeName);
                result = KTResolver.ResolveName(typeName, typeNamespace, declaredType, null);
            }
            if (null == result)
            {
                System.Diagnostics.Debug.WriteLine("Resolving namespace {0} and type {1} using default typeload", typeNamespace, typeName);
                result = Type.GetType(String.Format("{0}, {1}", typeName, typeNamespace));
            }
            return result;
        }
    }

        [Serializable]
    public class PrimitiveTypeResolver : DataContractResolver
    {
        public override bool TryResolveType(Type dcType, Type declaredType, DataContractResolver KTResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            Debug.WriteLine("Resolving type " + dcType.FullName);
            string resolvedTypeName = string.Empty;
            string resolvedNamespace = string.Empty;
            resolvedNamespace = DCRUtils.DefaultNS;
            switch (dcType.Name)
            {
                case "ObjectContainer":
                case "PrimitiveContainer":
                case "POCOObjectContainer":
                case "EnumStructContainer":
                case "EmptyDC":
                    {
                        resolvedTypeName = dcType.Name;
                    }
                    break;
                case "MyEnum1":
                    {
                        resolvedTypeName = "1munemy";
                    }
                    break;
                case "Seasons1":
                    {
                        resolvedTypeName = "1";
                    }
                    break;
                case "Seasons2":
                    {
                        resolvedTypeName = "2";
                    }
                    break;
                case "PublicDCStruct":
                    {
                        resolvedTypeName = "DcpublicStruct";
                    }
                    break;
                case "DateTimeOffset":
                    {
                        resolvedTypeName = "DTO";
                    }
                    break;
                case "Byte":
                case "Decimal":
                case "Double":
                case "Single":
                case "Int32":
                case "Int64":
                case "SByte":
                case "Int16":
                case "String":
                case "UInt16":
                case "UInt64":
                case "Char":
                case "Guid":
                case "XmlQualifiedName":
                case "TimeSpan":
                case "Array":
                case "Boolean":
                case "VT":
                case "NotSer":
                case "MyStruct":
                    {
                        resolvedTypeName = dcType.Name + "_foo";
                    }
                    break;
                default:
                    {
                        System.Diagnostics.Debug.WriteLine("Resolving type {0} using default KnownTypeResolver", dcType.FullName);
                        return KTResolver.TryResolveType(dcType, declaredType, null, out typeName, out typeNamespace);
                    }
            }
            XmlDictionary dic = new XmlDictionary();
            typeName = dic.Add(resolvedTypeName);
            typeNamespace = dic.Add(resolvedNamespace);
            return true;
        }

        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver KTResolver)
        {
            Debug.WriteLine("Resolving name " + typeName);
            switch (typeNamespace)
            {
                case "http://www.default.com":
                    {
                        switch (typeName)
                        {
                            case "1munemy":
                                {
                                    return typeof(MyEnum1);
                                }
                            case "DcpublicStruct":
                                {
                                    return typeof(PublicDCStruct);
                                }
                            case "DTO":
                                {
                                    return typeof(DateTimeOffset);
                                }
                            case "ObjectContainer":
                                {
                                    return typeof(ObjectContainer);
                                }
                            case "PrimitiveContainer":
                                {
                                    return typeof(PrimitiveContainer);
                                }
                            case "POCOObjectContainer":
                                {
                                    return typeof(POCOObjectContainer);
                                }
                            case "EnumStructContainer":
                                {
                                    return typeof(EnumStructContainer);
                                }
                            case "EmptyDC":
                                {
                                    return typeof(EmptyDC);
                                }
                            case "1":
                                {
                                    return typeof(Seasons1);
                                }
                            case "2":
                                {
                                    return typeof(Seasons2);
                                }
                            case "Byte_foo": { return typeof(System.Byte); }
                            case "Decimal_foo": { return typeof(System.Decimal); }
                            case "Double_foo": { return typeof(System.Double); }
                            case "Single_foo": { return typeof(System.Single); }
                            case "Int32_foo": { return typeof(System.Int32); }
                            case "Int64_foo": { return typeof(System.Int64); }
                            case "SByte_foo": { return typeof(System.SByte); }
                            case "Int16_foo": { return typeof(System.Int16); }
                            case "String_foo": { return typeof(System.String); }
                            case "UInt16_foo": { return typeof(System.UInt16); }
                            case "UInt64_foo": { return typeof(System.UInt64); }
                            case "Char_foo": { return typeof(System.Char); }
                            case "Guid_foo": { return typeof(System.Guid); }
                            case "XmlQualifiedName_foo": { return typeof(System.Xml.XmlQualifiedName); }
                            case "TimeSpan_foo": { return typeof(System.TimeSpan); }
                            case "Array_foo": { return typeof(System.Array); }
                            case "Boolean_foo": { return typeof(System.Boolean); }
                            case "VT_foo": { return typeof(VT); }
                            case "NotSer_foo": { return typeof(NotSer); }
                            case "MyStruct_foo": { return typeof(MyStruct); }
                            default: break;
                        }
                    }
                    break;
            }
            Debug.WriteLine("Resolving namespace {0} and type {1} using default KnownTypeResolver", typeNamespace, typeName);
            Type result = KTResolver.ResolveName(typeName, typeNamespace, declaredType, null);
            return result;
        }
    }

    [Serializable]
    public class GenericNullableResolver : DataContractResolver
    {
        public override bool TryResolveType(Type dcType, Type declaredType, DataContractResolver KTResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            TypeLibraryManager mgr = new TypeLibraryManager();
            System.Diagnostics.Debug.WriteLine("Resolving type " + dcType.FullName);
            string resolvedTypeName = string.Empty;
            string resolvedNamespace = DCRUtils.DefaultNS;
            if (dcType.Equals(typeof(ObjectContainer)))
            {
                resolvedTypeName = "ooo";
            }
            else if (dcType.Equals(typeof(POCOGenericsNullableContainer)))
            {
                resolvedTypeName = "1";
            }
            else if (dcType.Equals(typeof(CustomGeneric2<Guid, EmptyDC>)))
            {
                resolvedTypeName = "2";
            }
            else if (dcType.Equals(typeof(CustomGeneric2<BoxedPrim, EmptyDC>)))
            {
                resolvedTypeName = "3";
            }
            else if (dcType.Equals(typeof(Nullable<MyStruct>)))
            {
                resolvedTypeName = "4";
            }
            else if (dcType.Equals(typeof(CustomGeneric2<Person>)))
            {
                resolvedTypeName = "11";
            }
            else if (dcType.Equals(typeof(Seasons1))) { resolvedTypeName = "5"; }
            else if (dcType.Equals(typeof(Seasons2))) { resolvedTypeName = "6"; }
            else if (dcType.Equals(typeof(Person)))
            {
                resolvedTypeName = "7";
            }

            else if (dcType.Name.Contains("Drawing`3"))
            {
                System.Diagnostics.Debug.WriteLine("Resolving type Drawing'3");
                //if (dcType.FullName.Equals("CDF.Test.TestCases.Data.Serialization.DataContractResolverTest.Primitives.Drawing`3[[CDF.Test.TestCases.Data.Serialization.DataContractResolverTest.Primitives.Seasons1, CDF.Test.TestCases.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null],[CDF.Test.TestCases.Data.Serialization.DataContractResolverTest.Primitives.Seasons2, CDF.Test.TestCases.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null],[CDF.Test.TestCases.Data.Serialization.DataContractResolverTest.MiscSamples.CustomGeneric2`1[[CDF.Test.TestCases.Data.Serialization.DataContractResolverTest.Primitives.Person, CDF.Test.TestCases.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null]], CDF.Test.TestCases.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null]]"))
                if (dcType.FullName.Equals("Test.WCF.DCS.Drawing`3[[Test.WCF.DCS.Seasons1, Test.WCF.DCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[Test.WCF.DCS.Seasons2, Test.WCF.DCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[Test.WCF.DCS.MiscSamples.CustomGeneric2`1[[Test.WCF.DCS.Person, Test.WCF.DCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], Test.WCF.DCS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"))
                {
                    resolvedTypeName = "Drawing_Seasons1_Seasons2_CustomGeneric2`1_Person";
                    resolvedNamespace = "ĞģŹŵĚÒă";
                }
            }
            else if (mgr.FxPrimitivesInCollectionList.Contains(dcType))
            {
                SimpleResolver res = new SimpleResolver();
                return res.TryResolveType(dcType, declaredType, KTResolver, out typeName, out typeNamespace);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Resolving type {0} using default KnownTypeResolver", dcType.FullName);
                return KTResolver.TryResolveType(dcType, declaredType, null, out typeName, out typeNamespace);
            }
            System.Diagnostics.Debug.WriteLine("ResolvedTypeName =" + resolvedTypeName);
            System.Diagnostics.Debug.WriteLine("ResolvedNamespace =" + resolvedNamespace);
            XmlDictionary dic = new XmlDictionary();
            typeName = dic.Add(resolvedTypeName);
            typeNamespace = dic.Add(resolvedNamespace);
            return true;
        }

        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver KTResolver)
        {
            System.Diagnostics.Debug.WriteLine("Resolving name " + typeName);
            switch (typeNamespace)
            {
                case "http://www.default.com":
                    {
                        switch (typeName)
                        {
                            case "ooo":
                                {
                                    return typeof(ObjectContainer);
                                }
                            case "1":
                                {
                                    return typeof(POCOGenericsNullableContainer);
                                }
                            case "2":
                                {
                                    return typeof(CustomGeneric2<Guid, EmptyDC>);
                                }
                            case "3":
                                {
                                    return typeof(CustomGeneric2<BoxedPrim, EmptyDC>);
                                }
                            case "4":
                                {
                                    return typeof(Nullable<MyStruct>);
                                }
                            case "5":
                                {
                                    return typeof(Seasons1);
                                }
                            case "6":
                                {
                                    return typeof(Seasons2);
                                }
                            case "7":
                                {
                                    return typeof(Person);
                                }
                            case "11":
                                {
                                    return typeof(CustomGeneric2<Person>);
                                }
                        }
                    }
                    break;
            }
            if ((typeName == "Drawing_Seasons1_Seasons2_CustomGeneric2`1_Person") && (typeNamespace == "ĞģŹŵĚÒă"))
            {
                return typeof(Drawing<Seasons1, Seasons2, CustomGeneric2<Person>>);
            }
            else if (typeName.Contains("*"))
            {
                SimpleResolver res = new SimpleResolver();
                return res.ResolveName(typeName, typeNamespace, declaredType, KTResolver);
            }

            System.Diagnostics.Debug.WriteLine("Resolving namespace {0} and type {1} using default KnownTypeResolver", typeNamespace, typeName);
            //throw new TestCaseException("Cannot resolve typaname {0}", typeName);
            Type result = KTResolver.ResolveName(typeName, typeNamespace, declaredType, null);
            return result;
        }
    }

    // Serialization: Adds "***" to the full name of all Types in the dcrList.
    // Deserialization: If the Type name contains any *'s it removes the *'s, finds and returns the Type associated with that name.
    // Any Types not in the dcrList get handled by the KnownTypesResolver
    [Serializable]
    public class KTandDCRResolver : DataContractResolver
    {
        List<Type> dcrList;

        public KTandDCRResolver()
        {
            TypeLibraryManager mgr = new TypeLibraryManager();
            List<Type> dcrTypeList = DCRUtils.FilterAndMergeLists(new List<List<Type>> {
                mgr.IObjectRefTypeList,
                mgr.FxPrimitivesInCollectionList,
                mgr.SelfRefAndCyclesTypeList,
                mgr.CollectionsTypeList
            });
            dcrList = dcrTypeList;
        }

        public KTandDCRResolver(List<Type> aDCRList)
        {
            dcrList = aDCRList;
        }

        //Hashtable based on the dcrList to find the type during deserialization
        Hashtable dcrTypesHashTable = new Hashtable();
        public Hashtable DCRTypesHashtable
        {
            get
            {
                if (dcrTypesHashTable.Count == 0)
                {
                    dcrTypesHashTable = new Hashtable();
                    foreach (Type t in dcrList)
                    {
                        dcrTypesHashTable.Add(t.FullName, t);
                    }
                }
                return dcrTypesHashTable;
            }
        }

        public string defaultNS = "http://schemas.datacontract.org/2004/07/";

        //Serialization
        public override bool TryResolveType(Type dcType, Type declaredType, DataContractResolver KTResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            string resolvedTypeName = string.Empty;
            string resolvedNamespace = string.Empty;
            XmlDictionary dic = new XmlDictionary();
            if (dcrList.Contains(dcType))
            {
                resolvedTypeName = dcType.FullName + "***";
                resolvedNamespace = defaultNS + resolvedTypeName;
                typeName = dic.Add(resolvedTypeName);
                typeNamespace = dic.Add(resolvedNamespace);
            }
            else
            {
                KTResolver.TryResolveType(dcType, declaredType, null, out typeName, out typeNamespace);
            }
            if (typeName == null || typeNamespace == null)
            {
                XmlDictionary dictionary = new XmlDictionary();
                typeName = dictionary.Add(dcType.FullName);
                typeNamespace = dictionary.Add(dcType.Assembly.FullName);
            }
            return true;
        }

        //Deserialization
        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver KTResolver)
        {
            Type typeToDeserialize = null;
            if (typeName.Contains("*"))
            {
                string inputTypeName = typeName.Trim('*');
                string inputTypeNamespace = typeNamespace.Trim('*');
                //Find it in the hashtable
                typeToDeserialize = (Type)DCRTypesHashtable[inputTypeName];
            }
            else
            {
                typeToDeserialize = KTResolver.ResolveName(typeName, typeNamespace, declaredType, null);
            }
            if (typeToDeserialize == null)
            {
                typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, typeNamespace));
            }
            return typeToDeserialize;
        }
    }


}
