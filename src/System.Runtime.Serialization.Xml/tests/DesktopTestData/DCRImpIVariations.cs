using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

namespace DesktopTestData
{
    /// <summary>
    /// Default implementation which resolves the unknowntype and calls default KT logic if it cannot.
    /// </summary>
    public abstract class AbstractDefaultDCR : DataContractResolver
    {
        public override bool TryResolveType(Type dcType, Type declaredType, DataContractResolver KTResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            System.Diagnostics.Debug.WriteLine("Resolving type {0}", dcType.Name);
            string resolvedTypeName = string.Empty;
            string resolvedNamespace = string.Empty;
            DCRUtils.ResolveType(dcType, out resolvedTypeName, out resolvedNamespace);

            if ((!resolvedTypeName.Equals(DCRUtils.NoValue)) && (!resolvedNamespace.Equals(DCRUtils.NoValue)))
            {
                XmlDictionary dic = new XmlDictionary();
                typeName = dic.Add(resolvedTypeName);
                typeNamespace = dic.Add(resolvedNamespace);
                return true;
            }
            else
            {
                //call default KT logic
                return KTResolver.TryResolveType(dcType, declaredType, null, out typeName, out typeNamespace);
            }
        }
        
        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver KTResolver)
        {
            System.Diagnostics.Debug.WriteLine("Resolving Name {0}", typeName);
            Type returnType = DCRUtils.ResolveName(typeName, typeNamespace);
            if (!returnType.Equals(typeof(TypeNotFound)))
            {
                return returnType;
            }
            else
            {
                //Call default KT logic
                return KTResolver.ResolveName(typeName, typeNamespace, declaredType, null);
            }
        }
    }

    public class PublicDerivedDCR_1 : AbstractDefaultDCR
    {
        public override bool TryResolveType(Type dcType, Type declaredType, DataContractResolver KTResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            return base.TryResolveType(dcType, declaredType, KTResolver, out typeName, out typeNamespace);
        }


        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver KTResolver)
        {
            return base.ResolveName(typeName, typeNamespace, declaredType, KTResolver);
        }
    }

    /// <summary>
    /// Set KTResolver to null in ResolveName
    /// </summary>
    public class WorkAddress2 : Address
    {
        public WorkAddress2()
        {
        }
        public WorkAddress2(string c, string s, int z, string su)
            : base(c, s, z)
        {
            Suite = su;
        }

        [DataMember]
        public string Suite;

    }

    [DataContract]
    public class Blocks
    {
        public Blocks(string s)
        {
            color = s;
        }

        [DataMember]
        public string color;
    }

    [DataContract]
    public class TestPerson
    {
        public TestPerson()
        {
            nickName = "tuffy";
        }

        [DataMember]
        public string nickName;
    }

    /// <summary>
    /// Resolved by DCR "DCR_NegativeScenarios"
    /// ResolveName returns  null type and ResolveType valid out params 
    /// </summary>
    [DataContract]
    public class Person1
    {
        public object address;

        public Person1(string variation)
        {
            age = 10;
            name = "Tintin";
            address = new Address("rd", "wa", 90012);
        }

        public Person1()
        {
        }

        [DataMember]
        public int age;

        [DataMember]
        public string name;
    }

    [DataContract]
    public class Person2 : Person1
    {
        [DataMember]
        public Guid Uid;

        [DataMember]
        public XmlQualifiedName[] XQAArray;

        [DataMember]
        public object anyData;

        public Person2()
        {
            Uid = Guid.NewGuid();
            XQAArray = new XmlQualifiedName[] { new XmlQualifiedName("Name1", "http://www.PlayForFun.com"), new XmlQualifiedName("Name2", "http://www.FunPlay.com") };
            anyData = new Kid();
        }

    }

    /// <summary>
    /// Valid resolution by DCR
    /// </summary>
    public class Kid : Person1
    {
        [DataMember]
        public object FavoriteToy;

        public Kid()
        {
            FavoriteToy = new Blocks("Orange");
            age = 3;
        }
    }

    [DataContract]
    public class Address
    {
        public Address()
        {
        }

        public Address(string c, string s, int z)
        {
            City = c;
            State = s;
            ZipCode = z;
        }

        [DataMember]
        public string City;

        [DataMember]
        public string State;

        [DataMember]
        public int ZipCode;
    }

    class PrivateDerivedDCR : AbstractDefaultDCR
    {
    }

    public class PublicDerivedDCR_2 : AbstractDefaultDCR
    {
    }

    public class DCR_ReturnNull_1 : DataContractResolver
    {
        public override bool TryResolveType(Type dcType, Type declaredType, DataContractResolver KTResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            System.Diagnostics.Debug.WriteLine("Resolving type {0}", dcType.Name);
            XmlDictionary dic = new XmlDictionary();
            typeName = dic.Add(null);
            typeNamespace = dic.Add(null);
            return false;
        }


        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver KTResolver)
        {
            System.Diagnostics.Debug.WriteLine("Resolving Name {0}", typeName);
            return null;
        }
    }

    public class DCR_ResolveTypeToEmpty : DCR_ReturnNull_1
    {
        public override bool TryResolveType(Type dcType, Type declaredType, DataContractResolver KTResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            System.Diagnostics.Debug.WriteLine("Resolving type {0}", dcType.Name);
            XmlDictionary dic = new XmlDictionary();
            typeName = dic.Add(String.Empty);
            typeNamespace = dic.Add(String.Empty);
            return true;
        }

    }

    public class DCR_ReturnNull_2 : DCR_ReturnNull_1
    {
        public override bool TryResolveType(Type dcType, Type declaredType, DataContractResolver KTResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            System.Diagnostics.Debug.WriteLine("Resolving type {0}", dcType.Name);
            typeName = null;
            typeNamespace = null;
            return false;
        }
    }

    public class DCRThrowArgumentException : DataContractResolver
    {
        public override bool TryResolveType(Type dcType, Type declaredType, DataContractResolver KTResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            System.Diagnostics.Debug.WriteLine("Resolving type {0}", dcType.Name);
            throw new ArgumentException("DCR threw ArgumentExp");
        }


        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver KTResolver)
        {
            System.Diagnostics.Debug.WriteLine("Resolving Name {0}", typeName);
            throw new ArgumentException("DCR threw ArgumentExp");
        }
    }
    public class DCRThrowInvalidOpExp : DataContractResolver
    {
        public override bool TryResolveType(Type dcType, Type declaredType, DataContractResolver KTResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            System.Diagnostics.Debug.WriteLine("Resolving type {0}", dcType.Name);
            throw new InvalidOperationException("DCR threw InvalidOPExp");

        }


        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver KTResolver)
        {
            System.Diagnostics.Debug.WriteLine("Resolving Name {0}", typeName);
            throw new InvalidOperationException("DCR threw InvalidOPExp");
        }
    }

    public class DCR_NegativeScenarios : DataContractResolver
    {
        public override bool TryResolveType(Type dcType, Type declaredType, DataContractResolver KTResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            System.Diagnostics.Debug.WriteLine("Resolving type {0}", dcType.Name);

            XmlDictionary dic = new XmlDictionary();
            switch (dcType.Name)
            {
                case "Person1":
                    {
                        //ResolveName returns  null type and ResolveType valid out params 
                        typeName = dic.Add("TestPerson1");
                        typeNamespace = dic.Add("http://foo.com");
                    }
                    break;

                case "Person2":
                    {
                        //ResolveName returns valid Type and ResolveType returns invalid out params
                        //typeName = dic.Add("Person2#$$$$!&*"); //CSDMain: 78965
                        typeName = dic.Add("Person2Person2");
                        typeNamespace = dic.Add("http://fooPerson2.com");
                    }
                    break;

                case "Kid":
                    {
                        //valid resolution during ser and deser
                        typeName = dic.Add("Kid123");
                        typeNamespace = dic.Add("http://www.Kidsplay.com");
                    }
                    break;

                case "Address":
                    {
                        typeName = dic.Add("SomeAddress");
                        typeNamespace = dic.Add("http://foo.com");
                    }
                    break;
                case "WorkAddress":
                    {
                        //Set  KnownTypeResolver to null
                        typeName = null;
                        typeNamespace = null;
                        KTResolver = null;
                        return false;
                    }
                case "WorkAddress2":
                    {
                        //Set  KnownTypeResolver to null in ResolveName
                        typeName = dic.Add("WorkAddress2");
                        typeNamespace = dic.Add("http://tempuri.org");
                    }
                    break;
                default:
                    {
                        KTResolver.TryResolveType(dcType, declaredType, null, out typeName, out typeNamespace);
                    }
                    break;
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
            Type returnType = null;
            switch (typeNamespace)
            {
                case "http://foo.com":
                    {
                        if (typeName.Equals("TestPerson1"))
                        {
                            returnType = null;
                            return returnType;
                        }
                        else if (typeName.Equals("SomeAddress"))
                        {
                            returnType = typeof(Address);
                        }
                    }
                    break;

                case "http://fooPerson2.com":
                    {
                        if (typeName.Equals("Person2Person2"))
                        {
                            returnType = typeof(Kid);
                        }
                    }
                    break;
                case "http://www.Kidsplay.com":
                    {
                        if (typeName.Equals("Kid123"))
                        {
                            returnType = typeof(Kid);
                        }
                    }
                    break;
                case "http://tempuri.org":
                    {
                        //Set KTResolver to null
                        if (typeName.Equals("WorkAddress2"))
                        {
                            returnType = typeof(WorkAddress2);
                            KTResolver = null;
                            return returnType;
                        }
                    }
                    break;
                default:
                    {
                        returnType = KTResolver.ResolveName(typeNamespace, typeNamespace, declaredType, null);
                    }
                    break;
            }
            if (null == returnType)
            {
                System.Diagnostics.Debug.WriteLine("Resolving namespace {0} and type {1} using default typeload", typeNamespace, typeName);
                returnType = Type.GetType(String.Format("{0}, {1}", typeName, typeNamespace));
            }
            return returnType;
        }
    }

    public class DCR_ReturnNull_3 : DCR_ReturnNull_1
    {
        public override bool TryResolveType(Type dcType, Type declaredType, DataContractResolver KTResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            System.Diagnostics.Debug.WriteLine("Resolving type {0}", dcType.Name);
            XmlDictionary dic = new XmlDictionary();
            typeName = dic.Add("EmptyDCType");
            typeNamespace = dic.Add("Default");
            return true;
        }
    }

}
