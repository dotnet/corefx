using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Collections;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace DesktopTestData
{
    [DataContract(IsReference = false)]
    public class BaseDC
    {
        [DataMember]
        public string data;
        [DataMember]
        public string data2;

        public BaseDC() { }

        public virtual string Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        public virtual string Data2
        {
            get
            {
                return data2;
            }
            set
            {
                data2 = value;
            }
        }

        public BaseDC(bool init)
        {
            data = DateTime.Now.ToLongTimeString();
            data2 = DateTime.Now.ToLongTimeString();
        }
    }

    [Serializable]
    [DataContract(IsReference = false)]
    public class BaseSerializable
    {
        [DataMember]
        public string data;
        [DataMember]
        public string data2;
        [DataMember]
        public string[] days;

        public BaseSerializable() { }

        public virtual string Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        public virtual string Data2
        {
            get
            {
                return data2;
            }
            set
            {
                data2 = value;
            }
        }

        public BaseSerializable(bool init)
        {
            data = DateTime.Now.ToLongTimeString();
            data2 = DateTime.Now.ToLongTimeString();
            days = new string[] { "Base1", "Base2", "Base3", "Base4", "Base5", "Base6", "Base7" };
        }
    }

    [DataContract(IsReference = false)]
    public class DerivedDC : BaseDC
    {
        [DataMember]
        public string data0;

        [DataMember]
        public string data1;

        [DataMember]
        public string data3;

        public DerivedDC() { }

        public override string Data
        {
            get
            {
                return data0;
            }
            set
            {
                data0 = value;
            }
        }

        public virtual string Data1
        {
            get
            {
                return data1;
            }
            set
            {
                data1 = value;
            }
        }

        public string Data3
        {
            get
            {
                return data3;
            }
            set
            {
                data3 = value;
            }
        }

        public DerivedDC(bool init)
            : base(init)
        {
            data0 = DateTime.Now.ToLongTimeString();
            data1 = DateTime.Now.ToLongTimeString();
            data3 = DateTime.Now.ToLongTimeString();
        }
    }

    [Serializable]
    public class DerivedSerializable : BaseSerializable
    {
        public string data0;
        public string data1;
        public string data3;

        public DerivedSerializable() { }

        public override string Data
        {
            get
            {
                return data0;
            }
            set
            {
                data0 = value;
            }
        }

        public virtual string Data1
        {
            get
            {
                return data1;
            }
            set
            {
                data1 = value;
            }
        }

        public string Data3
        {
            get
            {
                return data3;
            }
            set
            {
                data3 = value;
            }
        }

        public DerivedSerializable(bool init)
            : base(init)
        {
            data0 = DateTime.Now.ToLongTimeString();
            data1 = DateTime.Now.ToLongTimeString();
            data3 = DateTime.Now.ToLongTimeString();
        }
    }

    [DataContract(IsReference = false)]
    public class DerivedDCIsRefBaseSerializable : BaseSerializable
    {
        [DataMember]
        public string Data33;
    }

    [DataContract(IsReference = false)]
    public class DerivedDCBaseSerializable : BaseSerializable
    {
        [DataMember]
        public string Data33;
    }


    [DataContract(IsReference = false)]
    public class Derived2DC : DerivedDC
    {
        [DataMember]
        public string data11;
        [DataMember]
        public string data12;
        [DataMember]
        public string data4;

        public Derived2DC() { }

        public override string Data
        {
            get
            {
                return data11;
            }
            set
            {
                data11 = value;
            }
        }

        public override string Data1
        {
            get
            {
                return data12;
            }
            set
            {
                data12 = value;
            }
        }

        public string Data4
        {
            get
            {
                return data4;
            }
            set
            {
                data4 = value;
            }
        }

        public Derived2DC(bool init)
            : base(init)
        {
            data11 = DateTime.Now.ToLongTimeString();
            data12 = DateTime.Now.ToLongTimeString();
            data4 = DateTime.Now.ToLongTimeString();
        }
    }

    //POCO
    public class DerivedPOCOBaseDCISRef : BaseDC
    {
        public SimpleDCWithRef SimpleDCWithRefData;
        public SimpleDCWithRef RefData;

        public DerivedPOCOBaseDCISRef() { }
        public DerivedPOCOBaseDCISRef(bool init)
            : base(init)
        {
            SimpleDCWithRefData = new SimpleDCWithRef(true);
            RefData = SimpleDCWithRefData;
        }
    }

    [DataContract]
    public class BaseDCNoIsRef
    {
        [DataMember]
        string Data;

        public BaseDCNoIsRef()
        {
            Data = String.Empty;
        }
    }

    public class DerivedPOCOBaseDCNOISRef : BaseDCNoIsRef
    {
        public string Data22;
    }

    public class DerivedIXmlSerializable_POCOBaseDCNOISRef : DerivedPOCOBaseDCNOISRef, IXmlSerializable
    {
        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            XmlSchema schema = new XmlSchema();
            XmlSchemaElement element1 = new XmlSchemaElement();
            element1.Name = "Data";
            element1.SchemaType = XmlSchemaSimpleType.GetBuiltInComplexType(new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"));
            schema.Items.Add(element1);
            schema.Id = "Schema1";
            return schema;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlDictionaryReader xmlReader = XmlDictionaryReader.CreateDictionaryReader(reader);
            string data = xmlReader.ReadString();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlDictionaryWriter xmlWriter = XmlDictionaryWriter.CreateDictionaryWriter(writer);
            xmlWriter.WriteString(DateTime.MinValue.ToShortTimeString());
        }

        #endregion
    }

    [CollectionDataContract(IsReference = false)]
    public class DerivedCDCFromBaseDC : BaseDC, IList<string>
    {
        public string Data223 = String.Empty;

        List<string> internalData = new List<string>();
        #region IList<string> Members

        public int IndexOf(string item)
        {
            return internalData.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            internalData.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            internalData.RemoveAt(index);
        }

        public string this[int index]
        {
            get
            {
                return internalData[index];
            }
            set
            {
                internalData[index] = value;
            }
        }

        #endregion

        #region ICollection<string> Members

        public void Add(string item)
        {
            internalData.Add(item);
        }

        public void Clear()
        {

        }

        public bool Contains(string item)
        {
            return internalData.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {

        }

        public int Count
        {
            get { return internalData.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(string item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            return internalData.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return internalData.GetEnumerator();
        }

        #endregion

        public static DerivedCDCFromBaseDC CreateInstance()
        {
            DerivedCDCFromBaseDC list = new DerivedCDCFromBaseDC();
            list.Add("112");
            return list;
        }
    }

    [Serializable]
    public class Derived2Serializable : DerivedDC
    {
        public string data00;
        public string data122;
        public string data4;

        public Derived2Serializable() { }

        public override string Data
        {
            get
            {
                return data00;
            }
            set
            {
                data00 = value;
            }
        }

        public override string Data1
        {
            get
            {
                return data122;
            }
            set
            {
                data122 = value;
            }
        }

        public string Data4
        {
            get
            {
                return data4;
            }
            set
            {
                data4 = value;
            }
        }

        public Derived2Serializable(bool init)
            : base(init)
        {
            data00 = DateTime.Now.ToLongTimeString();
            data122 = DateTime.Now.ToLongTimeString();
            data4 = DateTime.Now.ToLongTimeString();
        }
    }

    [Serializable]
    public class Derived2SerializablePositive : DerivedDC
    {
        [OptionalField]
        public string data00;
        [OptionalField]
        public string data122;
        [OptionalField]
        public string data4;

        public Derived2SerializablePositive() { }

        public override string Data
        {
            get
            {
                return data00;
            }
            set
            {
                data00 = value;
            }
        }

        public override string Data1
        {
            get
            {
                return data122;
            }
            set
            {
                data122 = value;
            }
        }

        public string Data4
        {
            get
            {
                return data4;
            }
            set
            {
                data4 = value;
            }
        }

        public Derived2SerializablePositive(bool init)
            : base(init)
        {
            data00 = DateTime.MaxValue.ToLongTimeString();
            data122 = DateTime.MinValue.ToLongTimeString();
            data4 = DateTime.MinValue.ToLongTimeString();
        }
    }

    [DataContract(IsReference = false)]
    public class Derived2Derived2Serializable : Derived2Serializable
    {
        public Derived2Derived2Serializable() { }
        public Derived2Derived2Serializable(bool init) : base(init) { }
    }

    [DataContract(IsReference = false)]
    public class Derived3Derived2Serializable : Derived2Serializable
    {
        public Derived3Derived2Serializable() { }
        public Derived3Derived2Serializable(bool init) : base(init) { }
    }

    //POCO
    public class Derived31Derived2SerializablePOCO : Derived2Serializable
    {
        public SimpleDCWithRef SimpleDCWithRefData;
        public SimpleDCWithRef RefData;

        public Derived31Derived2SerializablePOCO() { }
        public Derived31Derived2SerializablePOCO(bool init)
            : base(init)
        {
            SimpleDCWithRefData = new SimpleDCWithRef(true);
            RefData = SimpleDCWithRefData;
        }
    }

    [DataContract]
    public class Derived4Derived2Serializable : Derived3Derived2Serializable
    {
        public Derived4Derived2Serializable() { }
        public Derived4Derived2Serializable(bool init) : base(init) { }
    }

    [Serializable]
    public class Derived5Derived2Serializable : Derived3Derived2Serializable
    {
        public Derived5Derived2Serializable() { }
        public Derived5Derived2Serializable(bool init) : base(init) { }
    }

    //POCO
    public class Derived6Derived2SerializablePOCO : Derived3Derived2Serializable
    {
        public SimpleDCWithRef SimpleDCWithRefData;
        public SimpleDCWithRef RefData;

        public Derived6Derived2SerializablePOCO() { }
        public Derived6Derived2SerializablePOCO(bool init)
            : base(init)
        {
            SimpleDCWithRefData = new SimpleDCWithRef(true);
            RefData = SimpleDCWithRefData;
        }
    }
}
