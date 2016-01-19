﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace SerializationTypes
{
    public class TypeWithDateTimeStringProperty
    {
        public string DateTimeString;
        public DateTime CurrentDateTime;

        public TypeWithDateTimeStringProperty() { }
    }

    public class MyTypeA
    {
        public MyTypeB PropX { get; set; }
        public int PropY { get; set; }

        public MyTypeB[] P_Col_Array { get; set; }
    }

    [KnownType(typeof(MyTypeC))]
    [KnownType(typeof(MyTypeD))]
    public class MyTypeB
    {
        public char PropC { get; set; }
        public MyTypeA PropA { get; set; }
    }

    public class MyTypeC : MyTypeB
    {
        public bool PropB { get; set; }
    }

    public class MyTypeD : MyTypeB
    {
        public string PropS { get; set; }
        public int PropI { get; set; }
    }

    public class SimpleType
    {
        public string P1 { get; set; }
        public int P2 { get; set; }

        public static bool AreEqual(SimpleType x, SimpleType y)
        {
            if (x == null)
            {
                return y == null;
            }
            else if (y == null)
            {
                return x == null;
            }
            else
            {
                return (x.P1 == y.P1) && (x.P2 == y.P2);
            }
        }
    }

    public class TypeWithGetSetArrayMembers
    {
        public SimpleType[] F1;
        public int[] F2;

        public SimpleType[] P1 { get; set; }
        public int[] P2 { get; set; }
    }

    public class TypeWithGetOnlyArrayProperties
    {
        private SimpleType[] _p1 = new SimpleType[2];
        private int[] _p2 = new int[2];
        public SimpleType[] P1
        {
            get
            {
                return _p1;
            }
        }

        public int[] P2
        {
            get
            {
                return _p2;
            }
        }
    }

    public class TypeWithDictionaryGenericMembers
    {
        public Dictionary<string, int> F1;
        public IDictionary<string, int> F2;

        // read-write properties: strong type and interface type
        public Dictionary<string, int> P1 { get; set; }
        public IDictionary<string, int> P2 { get; set; }

        // read-only properties: strong type and interface type
        private Dictionary<bool, char> _ro1 = new Dictionary<bool, char>();
        public Dictionary<bool, char> RO1
        {
            get
            {
                return _ro1;
            }
        }

        private IDictionary<bool, char> _ro2 = new Dictionary<bool, char>();
        public IDictionary<bool, char> RO2
        {
            get
            {
                return _ro2;
            }
        }
    }

    public class MyDictionary : IDictionary
    {
        private Dictionary<object, object> _d = new Dictionary<object, object>();

        public void Add(object key, object value)
        {
            _d.Add(key, value);
        }

        public void Clear()
        {
            _d.Clear();
        }

        public bool Contains(object key)
        {
            return _d.ContainsKey(key);
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return ((IDictionary)_d).GetEnumerator();
        }

        public bool IsFixedSize
        {
            get { return ((IDictionary)_d).IsFixedSize; }
        }

        public bool IsReadOnly
        {
            get { return ((IDictionary)_d).IsReadOnly; }
        }

        public ICollection Keys
        {
            get { return _d.Keys; }
        }

        public void Remove(object key)
        {
            _d.Remove(key);
        }

        public ICollection Values
        {
            get { return _d.Values; }
        }

        public object this[object key]
        {
            get
            {
                return _d[key];
            }
            set
            {
                _d[key] = value;
            }
        }

        public void CopyTo(Array array, int index)
        {
            ((IDictionary)_d).CopyTo(array, index);
        }

        public int Count
        {
            get { return _d.Count; }
        }

        public bool IsSynchronized
        {
            get { return ((IDictionary)_d).IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return ((IDictionary)_d).SyncRoot; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_d).GetEnumerator();
        }
    }

    public struct ReadOnlyNonSerialziableProperty
    {
        public Uri Uri
        {
            get { return new Uri("http://www.microsoft.com"); }
        }
    }

    public struct StructNotSerializable
    {
        public int value;

        public override int GetHashCode()
        {
            return value;
        }
    }

    public class DictionaryWithVariousKeyValueTypes
    {
        public Dictionary<MyEnum, MyEnum> WithEnums;
        public Dictionary<StructNotSerializable, StructNotSerializable> WithStructs;
        public Dictionary<Nullable<Int16>, Nullable<Boolean>> WithNullables;

        public DictionaryWithVariousKeyValueTypes() { }

        public DictionaryWithVariousKeyValueTypes(bool init)
        {
            WithEnums = new Dictionary<MyEnum, MyEnum>();
            WithEnums.Add(MyEnum.Two, MyEnum.Three);
            WithEnums.Add(MyEnum.One, MyEnum.One);

            WithStructs = new Dictionary<StructNotSerializable, StructNotSerializable>();
            WithStructs.Add(new StructNotSerializable() { value = 10 }, new StructNotSerializable() { value = 12 });
            WithStructs.Add(new StructNotSerializable() { value = int.MaxValue }, new StructNotSerializable() { value = int.MinValue });

            WithNullables = new Dictionary<Nullable<Int16>, Nullable<Boolean>>();
            WithNullables.Add(Int16.MinValue, true);
            WithNullables.Add(0, false);
            WithNullables.Add(Int16.MaxValue, null);
        }
    }

    public class TypeWithDictionaryMembers
    {
        public MyDictionary F1;
        public IDictionary F2;

        // read-write properties: strong type and interface type
        public MyDictionary P1 { get; set; }
        public IDictionary P2 { get; set; }

        // read-only properties: strong type and interface type
        private MyDictionary _ro1 = new MyDictionary();
        public MyDictionary RO1
        {
            get
            {
                return _ro1;
            }
        }

        private IDictionary _ro2 = new MyDictionary();
        public IDictionary RO2
        {
            get
            {
                return _ro2;
            }
        }
    }

    public class TypeWithSimpleDictionaryMember
    {
        public Dictionary<string, int> F1;
    }

    public class TypeWithIDictionaryPropertyInitWithConcreteType
    {
        private IDictionary<string, string> _dictionaryProperty;

        public IDictionary<string, string> DictionaryProperty
        {
            get
            {
                return _dictionaryProperty;
            }
            set
            {
                _dictionaryProperty = value;
            }
        }

        public TypeWithIDictionaryPropertyInitWithConcreteType()
        {
            _dictionaryProperty = new Dictionary<string, string>();
        }
    }

    public class TypeWithListGenericMembers
    {
        public List<string> F1;
        public IList<string> F2;

        // read-write properties: strong type and interface type
        public List<int> P1 { get; set; }
        public IList<int> P2 { get; set; }

        // read-only properties: strong type and interface type
        private List<char> _ro1 = new List<char>();
        public List<char> RO1
        {
            get
            {
                return _ro1;
            }
        }

        private IList<char> _ro2 = new List<char>();
        public IList<char> RO2
        {
            get
            {
                return _ro2;
            }
        }
    }

    public class MyCollection<T> : ICollection<T>
    {
        private List<T> _items = new List<T>();

        public MyCollection()
        {
        }

        public MyCollection(params T[] values)
        {
            _items.AddRange(values);
        }

        public void Add(T item)
        {
            _items.Add(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((ICollection<T>)_items).IsReadOnly; }
        }

        public bool Remove(T item)
        {
            return _items.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((ICollection<T>)_items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }
    }

    public class TypeWithCollectionGenericMembers
    {
        public MyCollection<string> F1;
        public ICollection<string> F2;

        // read-write properties: strong type and interface type
        public MyCollection<string> P1 { get; set; }
        public ICollection<string> P2 { get; set; }

        // read-only properties: strong type and interface type
        private MyCollection<string> _ro1 = new MyCollection<string>();
        public MyCollection<string> RO1
        {
            get
            {
                return _ro1;
            }
        }

        private ICollection<string> _ro2 = new MyCollection<string>();
        public ICollection<string> RO2
        {
            get
            {
                return _ro2;
            }
        }
    }

    public class MyList : IList
    {
        private List<object> _items = new List<object>();

        public MyList()
        {
        }

        public MyList(params object[] values)
        {
            _items.AddRange(values);
        }

        public int Add(object value)
        {
            return ((IList)_items).Add(value);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            return _items.Contains(value);
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public bool IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public object this[int index]
        {
            get
            {
                return _items[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }
    }

    public class TypeWithListMembers
    {
        public MyList F1;
        public IList F2;

        // read-write properties: strong type and interface type
        public MyList P1 { get; set; }
        public IList P2 { get; set; }

        // read-only properties: strong type and interface type
        private MyList _ro1 = new MyList();
        public MyList RO1
        {
            get
            {
                return _ro1;
            }
        }

        private IList _ro2 = new MyList();
        public IList RO2
        {
            get
            {
                return _ro2;
            }
        }
    }

    public class MyEnumerable<T> : IEnumerable<T>
    {
        private List<T> _items = new List<T>();

        public MyEnumerable()
        {
        }

        public MyEnumerable(params T[] values)
        {
            _items.AddRange(values);
        }

        public void Add(T value)
        {
            _items.Add(value);
        }

        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }
    }

    public class TypeWithEnumerableGenericMembers
    {
        public MyEnumerable<string> F1;
        public IEnumerable<string> F2;

        // read-write properties: strong type and interface type
        public MyEnumerable<string> P1 { get; set; }
        public IEnumerable<string> P2 { get; set; }

        // read-only properties: strong type and interface type
        private MyEnumerable<string> _ro1 = new MyEnumerable<string>();
        public MyEnumerable<string> RO1
        {
            get
            {
                return _ro1;
            }
        }
    }

    public class MyCollection : ICollection
    {
        private List<object> _items = new List<object>();

        public MyCollection()
        {
        }

        public MyCollection(params object[] values)
        {
            _items.AddRange(values);
        }

        public object this[int index]
        {
            get
            {
                return _items[index];
            }
        }

        public void Add(object value)
        {
            _items.Add(value);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)_items).CopyTo(array, index);
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsSynchronized
        {
            get { return ((ICollection)_items).IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return ((ICollection)_items).SyncRoot; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((ICollection)_items).GetEnumerator();
        }
    }

    public class TypeWithCollectionMembers
    {
        public MyCollection F1;
        public ICollection F2;

        // read-write properties: strong type and interface type
        public MyCollection P1 { get; set; }
        public ICollection P2 { get; set; }

        // read-only properties: strong type and interface type
        private MyCollection _ro1 = new MyCollection();
        public MyCollection RO1
        {
            get
            {
                return _ro1;
            }
        }
    }

    public class MyEnumerable : IEnumerable
    {
        private List<object> _items = new List<object>();

        public MyEnumerable()
        {
        }

        public MyEnumerable(params object[] values)
        {
            _items.AddRange(values);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }

        public void Add(object value)
        {
            _items.Add(value);
        }

        public object this[int index]
        {
            get
            {
                return _items[index];
            }
        }

        public int Count
        {
            get
            {
                return _items.Count;
            }
        }
    }

    public class TypeWithEnumerableMembers
    {
        public MyEnumerable F1;
        public IEnumerable F2;

        // read-write properties: strong type and interface type
        public MyEnumerable P1 { get; set; }
        public IEnumerable P2 { get; set; }

        // read-only properties: strong type and interface type
        private MyEnumerable _ro1 = new MyEnumerable();
        public MyEnumerable RO1
        {
            get
            {
                return _ro1;
            }
        }
    }

    [DataContract]
    public class DCA_1
    {
        public string P1 { get; set; }
    }

    [DataContract(Name = "abc")]
    public class DCA_2
    {
        public string P1 { get; set; }
    }

    [DataContract(Namespace = "def")]
    public class DCA_3
    {
        public string P1 { get; set; }
    }

    [DataContract(IsReference = true)]
    public class DCA_4
    {
        public string P1 { get; set; }
    }

    [DataContract(Name = "abc", Namespace = "def", IsReference = false)]
    public class DCA_5
    {
        public string P1 { get; set; }
    }

    [DataContract]
    public class DMA_1
    {
        [DataMember]
        public string P1 { get; set; }

        [DataMember(Name = "xyz")]
        public int P2 { get; set; }

        [DataMember(Order = 100)]
        public bool Order100 { get; set; }

        [DataMember(Order = 2)]
        public bool P3 { get; set; }

        [DataMember(Order = int.MaxValue)]
        public bool OrderMaxValue { get; set; }

        [DataMember(IsRequired = true)]
        public char P4 { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public short P5 { get; set; }

        [DataMember]
        public MyDataContractClass04_1 MyDataMemberInAnotherNamespace { get; set; }
    }

    [DataContract(Namespace = "http://MyDataContractClass04_1.com/")]
    public class MyDataContractClass04_1
    {
        [DataMember]
        public string MyDataMember { get; set; }
    }

    [DataContract(IsReference = true)]
    [KnownType(typeof(CircularLinkDerived))]
    public class CircularLink
    {
        [DataMember]
        public CircularLink Link;
        [DataMember]
        public CircularLink RandomHangingLink;

        public CircularLink() { }
        public CircularLink(bool init)
        {
            Link = new CircularLink();
            Link.Link = new CircularLink();
            Link.Link.Link = this;

            RandomHangingLink = new CircularLink();
            RandomHangingLink.Link = new CircularLink();
            RandomHangingLink.Link.Link = new CircularLinkDerived();
            RandomHangingLink.Link.Link.Link = RandomHangingLink;
        }
    }

    [DataContract(IsReference = true)]
    [KnownType(typeof(CircularLink))]
    public class CircularLinkDerived : CircularLink
    {
        public CircularLinkDerived() { }
        public CircularLinkDerived(bool init) : base(init) { }
    }

    [DataContract]
    public class IDMA_1
    {
        [DataMember]
        public string MyDataMember { get; set; }

        [IgnoreDataMember]
        public string MyIgnoreDataMember { get; set; }

        public string MyUnsetDataMember { get; set; }
    }

    public enum MyEnum
    {
        [EnumMember]
        One,
        Two,
        [EnumMember]
        Three
    }

    public class TypeWithEnumMembers
    {
        public MyEnum F1;
        public MyEnum P1 { get; set; }
    }

    [DataContract]
    public struct DCStruct
    {
        [DataMember]
        public string Data;
        public DCStruct(bool init)
        {
            Data = "Data";
        }
    }

    [DataContract]
    public class DCClassWithEnumAndStruct
    {
        [DataMember]
        public DCStruct MyStruct;

        [DataMember]
        public MyEnum MyEnum1;

        public DCClassWithEnumAndStruct() { }
        public DCClassWithEnumAndStruct(bool init)
        {
            MyStruct = new DCStruct(init);
        }
    }

    public class BuiltInTypes
    {
        public byte[] ByteArray { get; set; }
    }

    [DataContract(IsReference = true)]
    [KnownType(typeof(SimpleBaseDerived))]
    public class SimpleBase
    {
        [DataMember]
        public string BaseData = String.Empty;

        public SimpleBase() { }
        public SimpleBase(bool init) { }
    }

    [DataContract(IsReference = true)]
    [KnownType(typeof(SimpleBaseDerived))]
    [KnownType(typeof(SimpleBaseDerived2))]
    public class GenericBase2<T, K>
        where T : new()
        where K : new()
    {
        [DataMember]
        public T genericData1;
        [DataMember]
        public K genericData2;

        public GenericBase2() { }
        public GenericBase2(bool init)
        {
            genericData1 = new T();
            genericData2 = new K();
        }
    }

    [DataContract(IsReference = true)]
    public class SimpleBaseDerived : SimpleBase
    {
        [DataMember]
        public string DerivedData = String.Empty;

        public SimpleBaseDerived() { }
        public SimpleBaseDerived(bool init) : base(init) { }
    }

    [DataContract(IsReference = true)]
    public class SimpleBaseDerived2 : SimpleBase
    {
        [DataMember]
        public string DerivedData = String.Empty;

        public SimpleBaseDerived2() { }
        public SimpleBaseDerived2(bool init) : base(init) { }
    }

    [DataContract(IsReference = true)]
    [KnownType(typeof(GenericBase<SimpleBaseContainer>))]
    [KnownType(typeof(SimpleBaseContainer))]
    public class GenericContainer
    {
        [DataMember]
        public object GenericData;

        public GenericContainer() { }
        public GenericContainer(bool init)
        {
            GenericData = new GenericBase<SimpleBaseContainer>(init);
        }
    }

    [DataContract(IsReference = true)]
    public class GenericBase<T> where T : new()
    {
        [DataMember]
        public object genericData;

        public GenericBase() { }
        public GenericBase(bool init)
        {
            genericData = new T();
        }
    }

    [DataContract(IsReference = true)]
    [KnownType(typeof(SimpleBaseDerived2))]
    public class SimpleBaseContainer
    {
        [DataMember]
        public SimpleBase Base1;
        [DataMember]
        public object Base2;

        public SimpleBaseContainer() { }
        public SimpleBaseContainer(bool init)
        {
            Base1 = new SimpleBaseDerived();
            Base2 = new SimpleBaseDerived2();
        }
    }

    public class TypeA
    {
        public string Name;
    }

    public class TypeB
    {
        public string Name;

        public static implicit operator TypeA(TypeB i)
        {
            return new TypeA { Name = i.Name };
        }

        public static implicit operator TypeB(TypeA i)
        {
            return new TypeB { Name = i.Name };
        }
    }

    public class TypeHasArrayOfASerializedAsB
    {
        public TypeA[] Items;

        public TypeHasArrayOfASerializedAsB() { }
        public TypeHasArrayOfASerializedAsB(bool init)
        {
            Items = new TypeA[]
            {
                new TypeA { Name = "typeAValue" },
                new TypeB { Name = "typeBValue" },
            };
        }
    }

    public class WithDuplicateNames
    {
        public DuplicateTypeNamesTest.ns1.ClassA ClassA1 { get; set; }
        public DuplicateTypeNamesTest.ns1.StructA StructA1 { get; set; }
        public DuplicateTypeNamesTest.ns1.EnumA EnumA1 { get; set; }
        public DuplicateTypeNamesTest.ns2.ClassA ClassA2;
        public DuplicateTypeNamesTest.ns2.StructA StructA2;
        public DuplicateTypeNamesTest.ns2.EnumA EnumA2;

        public WithDuplicateNames() { }
        public WithDuplicateNames(bool init)
        {
            ClassA1 = new DuplicateTypeNamesTest.ns1.ClassA() { Name = "Hello World! 漢 ñ" };
            StructA1 = new DuplicateTypeNamesTest.ns1.StructA() { Text = "" };
            EnumA1 = DuplicateTypeNamesTest.ns1.EnumA.two;
            ClassA2 = new DuplicateTypeNamesTest.ns2.ClassA() { Nombre = "" };
            StructA2 = new DuplicateTypeNamesTest.ns2.StructA() { Texto = "" };
            EnumA2 = DuplicateTypeNamesTest.ns2.EnumA.dos;
        }
    }

    public class WithXElement
    {
        public XElement e;

        public WithXElement() { }

        public WithXElement(bool init)
        {
            e = new XElement("ElementName1");
            e.SetAttributeValue(XName.Get("Attribute1"), "AttributeValue1");
            e.SetValue("Value1");
        }
    }

    public class WithXElementWithNestedXElement
    {
        public XElement e1;

        public WithXElementWithNestedXElement() { }

        public WithXElementWithNestedXElement(bool init)
        {
            e1 = new XElement("ElementName1");
            e1.SetAttributeValue(XName.Get("Attribute1"), "AttributeValue1");

            XElement e2 = new XElement("ElementName2");
            e2.SetAttributeValue(XName.Get("Attribute2"), "AttributeValue2");
            e2.SetValue("Value2");

            e1.Add(e2);
        }
    }

    public class WithArrayOfXElement
    {
        public XElement[] a;

        public WithArrayOfXElement() { }

        public WithArrayOfXElement(bool init)
        {
            string ns = "http://p.com/";

            a = new XElement[]
            {
                new XElement(XName.Get("item", ns), "item0"),
                new XElement(XName.Get("item", ns), "item1"),
                new XElement(XName.Get("item", ns), "item2"),
            };
        }
    }

    public class WithListOfXElement
    {
        public List<XElement> list;

        public WithListOfXElement() { }

        public WithListOfXElement(bool init)
        {
            string ns = "http://p.com/";

            list = new List<XElement>()
            {
                new XElement(XName.Get("item", ns), "item0"),
                new XElement(XName.Get("item", ns), "item1"),
                new XElement(XName.Get("item", ns), "item2"),
            };
        }
    }

    public class BaseType
    {
        public virtual string Name1 { get; set; }

        public string Name2 { get; set; }

        public string Name3 { get; set; }

        public string Name4 { get; set; }

        public string @Name5 { get; set; }
    }

    public class DerivedTypeWithDifferentOverrides : BaseType
    {
        public override string Name1 { get; set; }

        new public string Name2 { get; set; }

        new public string Name3 { get; set; }

        new internal string Name4 { get; set; }

        new public string Name5 { get; set; }
    }

    public class __TypeNameWithSpecialCharacters漢ñ
    {
        public string PropertyNameWithSpecialCharacters漢ñ { get; set; }
    }

    [DataContract]
    public class MyOtherType
    {
        [DataMember]
        public string Str;
    }

    [DataContract]
    public class MyType
    {
        [IgnoreDataMember]
        public bool OnSerializingMethodInvoked;

        [IgnoreDataMember]
        public bool OnSerializedMethodInvoked;

        [IgnoreDataMember]
        public bool OnDeserializingMethodInvoked;

        [IgnoreDataMember]
        public bool OnDeserializedMethodInvoked;

        [DataMember]
        public object Value;

        [OnSerializing()]
        private void OnSerializingMethod(StreamingContext context)
        {
            OnSerializingMethodInvoked = true;
        }

        [OnSerialized()]
        private void OnSerializedMethod(StreamingContext context)
        {
            OnSerializedMethodInvoked = true;
        }

        [OnDeserializing()]
        private void OnDeserializingMethod(StreamingContext context)
        {
            OnDeserializingMethodInvoked = true;
        }

        [OnDeserialized()]
        private void OnDeserializedMethod(StreamingContext context)
        {
            OnDeserializedMethodInvoked = true;
        }
    }

    public struct EnumerableStruct : IEnumerable<string>
    {
        private List<string> _values;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        public void Add(string value)
        {
            if (_values == null)
            {
                _values = new List<string>();
            }

            _values.Add(value);
        }
    }

    public class BaseClassWithSamePropertyName
    {
        [DataMember]
        public string StringProperty;

        [DataMember]
        public int IntProperty;

        [DataMember]
        public DateTime DateTimeProperty;

        [DataMember]
        public List<string> ListProperty;
    }

    public class DerivedClassWithSameProperty : BaseClassWithSamePropertyName
    {
        [DataMember]
        public new string StringProperty;

        [DataMember]
        public new int IntProperty;

        [DataMember]
        public new DateTime DateTimeProperty;

        [DataMember]
        public new List<string> ListProperty;
    }

    public class EnumerableCollection : IEnumerable<DateTime>
    {
        private List<DateTime> _values = new List<DateTime>();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator<DateTime> IEnumerable<DateTime>.GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        public void Add(DateTime value)
        {
            _values.Add(value);
        }
    }

    [DataContract(IsReference = false)]
    public class SimpleDC
    {
        [DataMember]
        public string Data;
        public SimpleDC() { }
        public SimpleDC(bool init)
        {
            Data = DateTime.MaxValue.ToString("T", CultureInfo.InvariantCulture);
        }
    }

    [DataContract(IsReference = false)]
    public class SimpleDCWithRef
    {
        [DataMember]
        public SimpleDC Data;

        [DataMember]
        public SimpleDC RefData;

        public SimpleDCWithRef() { }
        public SimpleDCWithRef(bool init)
        {
            Data = new SimpleDC(true);
            RefData = Data;
        }
    }

    [DataContract]
    public class ContainsLinkedList
    {
        [DataMember]
        public LinkedList<SimpleDCWithRef> Data;

        public ContainsLinkedList() { }
        public ContainsLinkedList(bool init)
        {
            this.Data = new LinkedList<SimpleDCWithRef>();
            SimpleDCWithRef d1 = new SimpleDCWithRef(true);
            SimpleDCWithRef d2 = new SimpleDCWithRef(true);
            d2.Data.Data = d1.RefData.Data;
            Data.AddLast(d1);
            Data.AddLast(d2);
            Data.AddLast(d2);
            Data.AddLast(d1);
            SimpleDCWithRef d3 = new SimpleDCWithRef(true);
            SimpleDCWithRef d4 = new SimpleDCWithRef(true);
            d4.Data = d3.RefData;
            Data.AddLast(d4);
            Data.AddLast(d3);
            SimpleDCWithRef d5 = new SimpleDCWithRef(true);
            SimpleDCWithRef d6 = new SimpleDCWithRef(true);
            SimpleDCWithRef d7 = new SimpleDCWithRef(true);
            d6.Data = d5.Data;
            d7.Data = d5.RefData;
            d7.RefData = d6.RefData;
            Data.AddLast(d7);
        }
    }

    [CollectionDataContract(Name = "SimpleCDC", ItemName = "Item")]
    public class SimpleCDC : ICollection<string>
    {
        private List<string> _data = new List<string>();
        public SimpleCDC() { }
        public SimpleCDC(bool init)
        {
            _data.Add("One");
            _data.Add("Two");
            _data.Add("Three");
        }

        #region ICollection<string> Members

        public void Add(string item)
        {
            _data.Add(item);
        }

        public void Clear()
        {
            _data.Clear();
        }

        public bool Contains(string item)
        {
            return _data.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            _data.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _data.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(string item)
        {
            return _data.Remove(item);
        }

        #endregion

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }
        #endregion
    }

    [XmlRoot(Namespace = "http://www.w3.org/2003/05/soap-envelope", ElementName = "Envelope")]
    public class TypeWithMemberWithXmlNamespaceDeclarationsAttribute
    {
        public string header;
        public string body;
        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces xmlns;
    }

    [XmlRoot(Namespace = "http://schemas.xmlsoap.org/ws/2005/04/discovery", IsNullable = false)]
    public class TypeWithXmlTextAttributeOnArray
    {
        [XmlText]
        public string[] Text;
    }

    [CollectionDataContract]
    public class MyDerivedCollection : LinkedList<string>
    {
        public MyDerivedCollection() { }
    }

    [DataContract]
    public class MyDerivedCollectionContainer
    {
        public MyDerivedCollectionContainer() { Items = new MyDerivedCollection(); }

        [DataMember]
        public MyDerivedCollection Items { get; set; }
    }

    [Flags]
    public enum EnumFlags
    {
        [EnumMember]
        One = 0x01,
        [EnumMember]
        Two = 0x02,
        [EnumMember]
        Three = 0x04,
        [EnumMember]
        Four = 0x08
    }

    public interface IBaseInterface
    {
        string ClassID { get; }

        string DisplayName { get; set; }

        string Id { get; set; }

        bool IsLoaded { get; set; }
    }

    [DataContract]
    public class ClassImplementsInterface : IBaseInterface
    {
        public virtual string ClassID { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public string Id { get; set; }

        public bool IsLoaded { get; set; }
    }

    #region XmlSerializer specific

    public class LocalReadingPosition
    {
        public string Ean { get; set; }
        public DateTime LastReadTime { get; set; }
        public int PageCount { get; set; }
        public string PageNumber { get; set; }
        public string PlatformOffset { get; set; }
    }

    public class WithStruct
    {
        public SomeStruct Some { get; set; }
    }

    public struct SomeStruct
    {
        public int A;
        public int B;
    }

    public class WithEnums
    {
        public IntEnum Int { get; set; }
        public ShortEnum Short { get; set; }
    }

    public class WithNullables
    {
        public IntEnum? Optional { get; set; }
        public IntEnum? Optionull { get; set; }
        public int? OptionalInt { get; set; }
        public Nullable<int> OptionullInt { get; set; }
        public SomeStruct? Struct1 { get; set; }
        public SomeStruct? Struct2 { get; set; }
    }

    public enum ByteEnum : byte
    {
        Option0, Option1, Option2
    }

    public enum SByteEnum : sbyte
    {
        Option0, Option1, Option2
    }

    public enum ShortEnum : short
    {
        Option0, Option1, Option2
    }

    public enum UShortEnum : ushort
    {
        Option0, Option1, Option2
    }

    public enum IntEnum
    {
        Option0, Option1, Option2
    }

    public enum UIntEnum : uint
    {
        Option0, Option1, Option2
    }

    public enum LongEnum : long
    {
        Option0, Option1, Option2
    }

    public enum ULongEnum : ulong
    {
        Option0, Option1, Option2
    }

    [XmlRoot(DataType = "XmlSerializerAttributes", ElementName = "AttributeTesting", IsNullable = false)]
    [XmlInclude(typeof(ItemChoiceType))]
    public class XmlSerializerAttributes
    {
        public XmlSerializerAttributes()
        {
            XmlElementProperty = 1;
            XmlAttributeProperty = 2;
            XmlArrayProperty = new string[] { "one", "two", "three" };
            EnumType = ItemChoiceType.Word;
            MyChoice = "String choice value";
            XmlIncludeProperty = ItemChoiceType.DecimalNumber;
            XmlEnumProperty = new ItemChoiceType[] { ItemChoiceType.DecimalNumber, ItemChoiceType.Number, ItemChoiceType.Word, ItemChoiceType.None };
            XmlTextProperty = "<xml>Hello XML</xml>";
            XmlNamespaceDeclarationsProperty = "XmlNamespaceDeclarationsPropertyValue";
        }

        [XmlElement(DataType = "int", ElementName = "XmlElementPropertyNode", Namespace = "http://element", Type = typeof(int))]
        public int XmlElementProperty { get; set; }

        [XmlAttribute(AttributeName = "XmlAttributeName")]
        public int XmlAttributeProperty { get; set; }

        [XmlArray(ElementName = "CustomXmlArrayProperty", Namespace = "http://mynamespace")]
        [XmlArrayItem(typeof(string))]
        public object[] XmlArrayProperty { get; set; }

        [XmlChoiceIdentifier("EnumType")]
        [XmlElement("Word", typeof(string))]
        [XmlElement("Number", typeof(int))]
        [XmlElement("DecimalNumber", typeof(double))]
        public object MyChoice;

        // Don't serialize this field. The EnumType field contains the enumeration value that corresponds to the MyChoice field value.
        [XmlIgnore]
        public ItemChoiceType EnumType;

        [XmlElement]
        public object XmlIncludeProperty;

        [XmlEnum("EnumProperty")]
        public ItemChoiceType[] XmlEnumProperty;

        [XmlText]
        public string XmlTextProperty;

        [XmlNamespaceDeclarations]
        public string XmlNamespaceDeclarationsProperty;
    }

    [XmlType(IncludeInSchema = false)]
    public enum ItemChoiceType
    {
        None,
        Word,
        Number,
        DecimalNumber
    }

    public class TypeWithAnyAttribute
    {
        public string Name;

        [XmlAttribute]
        public int IntProperty { get; set; }

        [XmlAnyAttribute]
        public XmlAttribute[] Attributes { get; set; }
    }

    public class KnownTypesThroughConstructor
    {
        public object EnumValue;

        public object SimpleTypeValue;
    }

    public class SimpleKnownTypeValue
    {
        public string StrProperty { get; set; }
    }

    public class ClassImplementingIXmlSerialiable : IXmlSerializable
    {
        public static bool WriteXmlInvoked = false;
        public static bool ReadXmlInvoked = false;

        public string StringValue { get; set; }
        private bool BoolValue { get; set; }

        public ClassImplementingIXmlSerialiable()
        {
            BoolValue = true;
        }

        public bool GetPrivateMember()
        {
            return BoolValue;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            ReadXmlInvoked = true;
            reader.MoveToContent();
            StringValue = reader.GetAttribute("StringValue");
            BoolValue = bool.Parse(reader.GetAttribute("BoolValue"));
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            WriteXmlInvoked = true;
            writer.WriteAttributeString("StringValue", StringValue);
            writer.WriteAttributeString("BoolValue", BoolValue.ToString());
        }
    }

    public class TypeWithNestedGenericClassImplementingIXmlSerialiable
    {
        // T can only be string
        public class NestedGenericClassImplementingIXmlSerialiable<T> : IXmlSerializable
        {
            public static bool WriteXmlInvoked = false;
            public static bool ReadXmlInvoked = false;

        public string StringValue { get; set; }
        private T GenericValue { get; set; }

        public NestedGenericClassImplementingIXmlSerialiable()
        {
            GenericValue = default(T);
        }

        public T GetPrivateMember()
        {
            return GenericValue;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            ReadXmlInvoked = true;
            reader.MoveToContent();
            StringValue = reader.GetAttribute("StringValue");
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            WriteXmlInvoked = true;
            writer.WriteAttributeString("StringValue", StringValue);
        }
        }
    }

    public class TypeWithPropertyNameSpecified
    {
        public string MyField;

        [XmlIgnore]
        public bool MyFieldSpecified;

        public int MyFieldIgnored;

        [XmlIgnore]
        public bool MyFieldIgnoredSpecified;
    }

    [XmlType(Namespace = ""), XmlRoot(Namespace = "", IsNullable = true)]
    public class TypeWithXmlSchemaFormAttribute
    {
        [XmlArray(Form = XmlSchemaForm.Unqualified)]
        public List<int> UnqualifiedSchemaFormListProperty { get; set; }

        [XmlArray(Form = XmlSchemaForm.None), XmlArrayItem("NoneParameter", Form = XmlSchemaForm.None, IsNullable = false)]
        public List<string> NoneSchemaFormListProperty { get; set; }

        [XmlArray(Form = XmlSchemaForm.Qualified), XmlArrayItem("QualifiedParameter", Form = XmlSchemaForm.Qualified, IsNullable = false)]
        public List<bool> QualifiedSchemaFormListProperty { get; set; }
    }

    [XmlType(TypeName = "MyXmlType")]
    public class TypeWithTypeNameInXmlTypeAttribute
    {
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public string XmlAttributeForm;
    }

    [XmlType(AnonymousType = true)]
    public class TypeWithSchemaFormInXmlAttribute
    {
        [XmlAttribute(Form = XmlSchemaForm.Qualified, Namespace = "http://test.com")]
        public string TestProperty;
    }


    #endregion

    public class UnspecifiedRootSerializationType
    {
        public int MyIntProperty { get; set; }

        public string MyStringProperty { get; set; }
    }

    [DataContract]
    internal class InternalType
    {
        public InternalType()
        {
            PrivateProperty = 100;
        }

        [DataMember]
        internal int InternalProperty { get; set; }

        [DataMember]
        private int PrivateProperty { get; set; }

        public int GetPrivatePropertyValue()
        {
            return PrivateProperty;
        }
    }

    [DataContract]
    public class TypeWithUriTypeProperty
    {
        [DataMember]
        public Uri ConfigUri
        {
            get;
            set;
        }
    }

    [DataContract]
    public class TypeWithDateTimeOffsetTypeProperty
    {
        [DataMember]
        public DateTimeOffset ModifiedTime
        {
            get;
            set;
        }
    }

    public class TypeWithCommonTypeProperties
    {
        public Guid Id { get; set; }
        public TimeSpan Ts { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TypeWithCommonTypeProperties)
            {
                TypeWithCommonTypeProperties other = (TypeWithCommonTypeProperties)obj;
                return (this.Id == other.Id && this.Ts == other.Ts);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() + Ts.GetHashCode();
        }
    }

    public class BaseClassForInvalidDerivedClass
    {
        public int Id;
    }

    public class InvalidDerivedClass : BaseClassForInvalidDerivedClass
    {
        public ICollection<int> Member1;
    }

    public class AnotherInvalidDerivedClass : BaseClassForInvalidDerivedClass
    {
        public ICollection<int> Member1;
    }

    public class TypeWithTypeProperty
    {
        public int Id { get; set; }
        public Type Type { get; set; }
        public string Name { get; set; }
    }

    [DataContract(Namespace = "SerializationTypes.GenericTypeWithPrivateSetter")]
    public class GenericTypeWithPrivateSetter<T>
    {
        public GenericTypeWithPrivateSetter()
        {
        }

        public GenericTypeWithPrivateSetter(string value)
        {
            PropertyWithPrivateSetter = value;
        }

        [DataMember]
        public string PropertyWithPrivateSetter { get; private set; }
    }

    public class TypeWithExplicitIEnumerableImplementation : IEnumerable
    {
        private List<string> _innerCollection = new List<string>();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _innerCollection.GetEnumerator();
        }
        public int Count { get { return _innerCollection.Count; } }
        public void Add(object item)
        {
            _innerCollection.Add((string)item);
        }
    }

    public class TypeWithNonPublicDefaultConstructor
    {
        private static string s_prefix;
        static TypeWithNonPublicDefaultConstructor()
        {
            s_prefix = "Mr. ";
        }

        private TypeWithNonPublicDefaultConstructor()
        {
            Name = s_prefix + "FooName";
        }
        public string Name { get; set; }
    }

    [XmlRoot("RootElement")]
    public class TypeWithMismatchBetweenAttributeAndPropertyType
    {
        private int _intValue = 120;

        [DefaultValue(true), XmlAttribute("IntValue")]
        public int IntValue
        {
            get
            {
                return _intValue;
            }
            set
            {
                _intValue = value;
            }
        }
    }

    public class Level
    {
        public string Name { get; set; }
        public int LevelNo { get; set; }
    }

    [KnownType(typeof(Dictionary<int, Level>))]
    public class TypeWithGenericDictionaryAsKnownType
    {
        [DataMember]
        public Dictionary<int, Level> Foo = new Dictionary<int, Level>();
    }

    public interface IArticle
    {
        string Title { get; set; }
        string Category { get; set; }
    }

    [DataContract]
    public class ArticleBase : IArticle
    {
        public ArticleBase() : this("Untitled", "Uncategorized") { }

        public ArticleBase(string title, string category)
        {
            _title = title;
            _category = category;
        }

        private string _title;

        [DataMember]
        public string Title { get { return _title; } set { _title = value; } }

        private string _category;

        [DataMember]
        public string Category { get { return _category; } set { _category = value; } }

        public override string ToString()
        {
            return Category + " - " + Title;
        }
    }

    public class NewsArticle : ArticleBase
    {
        public NewsArticle() : base("Untitled News", "News") { }
    }

    public class SummaryArticle : ArticleBase
    {
        public SummaryArticle() : base("Untitled Summary", "Summary") { }
    }

    [DataContract]
    [KnownType(typeof(ArticleBase))]
    [KnownType(typeof(NewsArticle))]
    [KnownType(typeof(SummaryArticle))]
    public class TypeWithKnownTypeAttributeAndInterfaceMember
    {
        [DataMember]
        public IArticle HeadLine { get; set; }
    }

    [DataContract]
    [KnownType(typeof(ArticleBase))]
    [KnownType(typeof(NewsArticle))]
    [KnownType(typeof(SummaryArticle))]
    public class TypeWithKnownTypeAttributeAndListOfInterfaceMember
    {
        [DataMember]
        public List<IArticle> Articles { get; set; }
    }

    // Comes from app: The Weather Channel. See bug 1101076 for details
    public class ServerSettings
    {
        public string DS2Root { get; set; }
        public string MetricConfigUrl { get; set; }
    }

    public class MyGenericList<T> : IList<T>
    {
        private List<T> _internalList = new List<T>();

        public int IndexOf(T item)
        {
            return _internalList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _internalList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _internalList.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return _internalList[index];
            }
            set
            {
                _internalList[index] = value;
            }
        }

        public void Add(T item)
        {
            _internalList.Add(item);
        }

        public void Clear()
        {
            _internalList.Clear();
        }

        public bool Contains(T item)
        {
            return _internalList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _internalList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _internalList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return _internalList.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }
    }

    public class TypeWithListPropertiesWithoutPublicSetters
    {
        private List<string> _anotherStringList = new List<string>();

        static TypeWithListPropertiesWithoutPublicSetters()
        {
            StaticProperty = "Static property should not be checked for public setter";
        }

        public TypeWithListPropertiesWithoutPublicSetters()
        {
            PropertyWithXmlElementAttribute = new List<string>();
            IntList = new MyGenericList<int>();
            StringList = new List<string>();
            PrivateIntListField = new List<int>();
            PublicIntListField = new List<int>();
            PublicIntListFieldWithXmlElementAttribute = new List<int>();
        }

        public static string StaticProperty { get; private set; }


        [XmlElement("PropWithXmlElementAttr")]
        public List<string> PropertyWithXmlElementAttribute { get; private set; }
        public MyGenericList<int> IntList { get; private set; }
        public List<string> StringList { get; private set; }
        public List<string> AnotherStringList { get { return _anotherStringList; } }

        private List<int> PrivateIntListField;
        public List<int> PublicIntListField;
        [XmlElement("FieldWithXmlElementAttr")]
        public List<int> PublicIntListFieldWithXmlElementAttribute;
    }

    public abstract class HighScoreManager<T> where T : HighScoreManager<T>.HighScoreBase
    {
        public abstract class HighScoreBase
        {
        }
    }

    public class HighScores : HighScoreManager<HighScores.BridgeGameHighScore>
    {
        public class BridgeGameHighScore : HighScoreManager<HighScores.BridgeGameHighScore>.HighScoreBase
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }

    public class TypeWithNestedPublicType
    {
        public TypeWithNestedPublicType()
        {
            Level = 10;
        }

        public uint Level { get; private set; }
        public class LevelData
        {
            public string Name { get; set; }
        }
    }

    public class PublicTypeWithNestedPublicTypeWithNestedPublicType
    {
        public PublicTypeWithNestedPublicTypeWithNestedPublicType()
        {
            Level = 10;
        }

        public uint Level { get; private set; }

        public class NestedPublicType
        {
            public class LevelData
            {
                public string Name { get; set; }
            }
        }
    }

    internal class InternalTypeWithNestedPublicType
    {
        public InternalTypeWithNestedPublicType()
        {
            Level = 10;
        }

        public uint Level { get; private set; }
        public class LevelData
        {
            public string Name { get; set; }
        }
    }

    internal class InternalTypeWithNestedPublicTypeWithNestedPublicType
    {
        public InternalTypeWithNestedPublicTypeWithNestedPublicType()
        {
            Level = 10;
        }

        public uint Level { get; private set; }

        public class NestedPublicType
        {
            public class LevelData
            {
                public string Name { get; set; }
            }
        }
    }

    [DataContract]
    public class BaseTypeWithDataMember
    {
        [DataMember]
        public TypeAsEmbeddedDataMember EmbeddedDataMember { get; set; }
    }

    [DataContract]
    public class DerivedTypeWithDataMemberInBaseType : BaseTypeWithDataMember
    {
    }

    [DataContract]
    public class TypeAsEmbeddedDataMember
    {
        [DataMember]
        public string Name { get; set; }
    }

    public class PocoBaseTypeWithDataMember
    {
        public PocoTypeAsEmbeddedDataMember EmbeddedDataMember { get; set; }
    }

    public class PocoDerivedTypeWithDataMemberInBaseType : PocoBaseTypeWithDataMember
    {
    }

    public class PocoTypeAsEmbeddedDataMember
    {
        public string Name { get; set; }
    }

    public class SpotlightDescription
    {
        public SpotlightDescription() { }
        public bool IsDynamic { get; set; }
        public string Value { get; set; }
    }

    public enum SlideEventType
    {
        None,
        LaunchURL,
        LaunchSection,
        LaunchVideo,
        LaunchImage
    }

    public class SerializableSlide
    {
        public SerializableSlide() { }
        public SpotlightDescription Description { get; set; }
        public string DisplayCondition { get; set; }
        public string EventData { get; set; }
        public SlideEventType EventType { get; set; }
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        public string Title { get; set; }
    }

    public class GenericTypeWithNestedGenerics<T>
    {
        public class InnerGeneric<U>
        {
            public T data1;
            public U data2;
        }
    }

    [DataContract]
    public class TypeWithXmlQualifiedName
    {
        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public XmlQualifiedName Value { get; set; }
    }

    public class TypeWithNoDefaultCtor
    {
        public TypeWithNoDefaultCtor(string id)
        {
            ID = id;
        }
        public string ID { get; set; }
    }

    public class TypeWithPropertyWithoutDefaultCtor
    {
        public TypeWithPropertyWithoutDefaultCtor()
        {
        }

        public string Name { get; set; }
        public TypeWithNoDefaultCtor MemberWithInvalidDataContract { get; set; }
    }


    [DataContract(Name = "DCWith.InName")]
    public class DataContractWithDotInName
    {
        [DataMember]
        public string Name { get; set; }
    }

    [DataContract(Name = "DCWith-InName")]
    public class DataContractWithMinusSignInName
    {
        [DataMember]
        public string Name { get; set; }
    }

    [DataContract(Name = "DCWith{}[]().,:;+-*/%&|^!~=<>?++--&&||<<>>==!=<=>=+=-=*=/=%=&=|=^=<<=>>=->InName")]
    public class DataContractWithOperatorsInName
    {
        [DataMember]
        public string Name { get; set; }
    }

    [DataContract(Name = "DCWith`@#$'\" 	InName")]
    public class DataContractWithOtherSymbolsInName
    {
        [DataMember]
        public string Name { get; set; }
    }

    [CollectionDataContract(Name = "MyHeaders", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect", ItemName = "MyHeader", KeyName = "MyKey", ValueName = "MyValue")]
    public sealed class CollectionDataContractWithCustomKeyName : Dictionary<int, int>
    {
        public CollectionDataContractWithCustomKeyName()
        {
        }
    }

    // Dictionary<int, int> is already used above so there will be a conflict.
    [CollectionDataContract(Name = "MyHeaders2", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect", ItemName = "MyHeader2", KeyName = "MyKey2", ValueName = "MyValue2")]
    public sealed class CollectionDataContractWithCustomKeyNameDuplicate : Dictionary<int, int>
    {
        public CollectionDataContractWithCustomKeyNameDuplicate()
        {
        }
    }

    public class CollectionWithoutDefaultConstructor : MyCollection<string>
    {
        internal CollectionWithoutDefaultConstructor(string name) : base()
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    public class TypeWithCollectionWithoutDefaultConstructor
    {
        public TypeWithCollectionWithoutDefaultConstructor()
        {
            _collectionWithoutDefaultConstructor = new CollectionWithoutDefaultConstructor("MyName");
        }

        CollectionWithoutDefaultConstructor _collectionWithoutDefaultConstructor;
        public CollectionWithoutDefaultConstructor CollectionProperty { get { return _collectionWithoutDefaultConstructor; } }
    }

    public class TypeMissingSerializationCodeBase
    {
    }

    public class TypeMissingSerializationCodeDerived : TypeMissingSerializationCodeBase
    {
        public string Name { get; set; }
    }
}

namespace DuplicateTypeNamesTest.ns1
{
    public class ClassA
    {
        public string Name;
    }

    public struct StructA
    {
        public string Text;
    }

    public enum EnumA
    {
        one, two, three,
    }
}

namespace DuplicateTypeNamesTest.ns2
{
    public class ClassA
    {
        public string Nombre;
    }

    public struct StructA
    {
        public string Texto;
    }

    public enum EnumA
    {
        uno, dos, tres,
    }
}

public class TypeWithoutPublicSetter
{
    public string Name { get; private set; }

    [XmlIgnore]
    public int Age { get; private set; }

    public Type MyType { get; private set; }

    public string ValidProperty { get; set; }

    public string PropertyWrapper
    {
        get
        {
            return ValidProperty;
        }
    }
}

[CompilerGenerated]
public class TypeWithCompilerGeneratedAttributeButWithoutPublicSetter
{
    [CompilerGenerated]
    public string Name { get; private set; }
}

public class TestableDerivedException : System.Exception
{
    public TestableDerivedException()
        : base()
    { }

    public TestableDerivedException(string message)
        : base(message)
    { }

    public TestableDerivedException(string message, Exception innerException)
        : base(message, innerException)
    { }

    public string TestProperty { get; set; }
}


public class TypeWithXmlElementProperty
{
    [XmlAnyElement]
    public XmlElement[] Elements;
}

public class TypeWithXmlDocumentProperty
{
    public XmlDocument Document;
}

public class TypeWithNonParameterlessConstructor
{
    public string StringProperty { get; set; }

    public TypeWithNonParameterlessConstructor(string value)
    {
        StringProperty = value;
    }
}

[DataContract]
public class AppEnvironment
{
    [DataMember(Name = "screen:orientation")]
    public string ScreenOrientation { get; set; }

    [DataMember(Name = "screen_dpi(x:y)")]
    public int ScreenDpi { get; set; }
}

#region Types for data contract surrogate tests

public class NonSerializablePerson
{
    public string Name { get; private set; }
    public int Age { get; private set; }

    public NonSerializablePerson(string name, int age)
    {
        this.Name = name;
        this.Age = age;
    }

    public override string ToString()
    {
        return string.Format("Person[Name={0},Age={1}]", this.Name, this.Age);
    }
}

public class Family
{
    public NonSerializablePerson[] Members;

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Family members:");
        foreach (var member in this.Members)
        {
            sb.AppendLine("  " + member);
        }

        return sb.ToString();
    }
}

[DataContract]
public class NonSerializablePersonSurrogate
{
    [DataMember(Name = "PersonName")]
    public string Name { get; set; }
    [DataMember(Name = "PersonAge")]
    public int Age { get; set; }
}

public class MyPersonSurrogateProvider : ISerializationSurrogateProvider
{
    public Type GetSurrogateType(Type type)
    {
        if (type == typeof(NonSerializablePerson))
        {
            return typeof(NonSerializablePersonSurrogate);
        }
        else
        {
            return type;
        }
    }

    public object GetDeserializedObject(object obj, Type targetType)
    {
        if (obj is NonSerializablePersonSurrogate)
        {
            NonSerializablePersonSurrogate person = (NonSerializablePersonSurrogate)obj;
            return new NonSerializablePerson(person.Name, person.Age);
        }

        return obj;
    }

    public object GetObjectToSerialize(object obj, Type targetType)
    {
        if (obj is NonSerializablePerson)
        {
            NonSerializablePerson nsp = (NonSerializablePerson)obj;
            NonSerializablePersonSurrogate serializablePerson = new NonSerializablePersonSurrogate
            {
                Name = nsp.Name,
                Age = nsp.Age,
            };

            return serializablePerson;
        }

        return obj;
    }
}

[DataContract]
class MyFileStream : IDisposable
{
    private FileStream stream;

    internal string Name
    {
        get
        {
            return this.stream.Name;
        }
    }

    internal MyFileStream(string fileName)
    {
        this.stream = new FileStream(
                            fileName,
                            FileMode.OpenOrCreate,
                            FileAccess.ReadWrite,
                            FileShare.ReadWrite);
    }

    internal void WriteLine(string line)
    {
        using (StreamWriter writer = new StreamWriter(this.stream))
        {
            writer.WriteLine(line);
        }
    }

    internal string ReadLine()
    {
        using (StreamReader reader = new StreamReader(this.stream))
        {
            return reader.ReadLine();
        }
    }

    public void Dispose()
    {
        this.stream.Dispose();
    }
}

[DataContract]
class MyFileStreamReference
{
    [DataMember]
    private string fileStreamName;

    private MyFileStreamReference(string fileStreamName)
    {
        this.fileStreamName = fileStreamName;
    }

    internal static MyFileStreamReference Create(MyFileStream myFileStream)
    {
        return new MyFileStreamReference(myFileStream.Name);
    }

    internal MyFileStream ToMyFileStream()
    {
        return new MyFileStream(fileStreamName);
    }
}

internal class MyFileStreamSurrogateProvider : ISerializationSurrogateProvider
{
    static MyFileStreamSurrogateProvider()
    {
        Singleton = new MyFileStreamSurrogateProvider();
    }

    internal static MyFileStreamSurrogateProvider Singleton { get; private set; }

    public Type GetSurrogateType(Type type)
    {
        if (type == typeof (MyFileStream))
        {
            return typeof (MyFileStreamReference);
        }

        return type;
    }

    public object GetObjectToSerialize(object obj, Type targetType)
    {
        if (obj == null)
        {
            return null;
        }
        MyFileStream myFileStream = obj as MyFileStream;
        if (null != myFileStream)
        {
            if (targetType != typeof (MyFileStreamReference))
            {
                throw new ArgumentException("Target type for serialization must be MyFileStream");
            }
            return MyFileStreamReference.Create(myFileStream);
        }

        return obj;
    }

    public object GetDeserializedObject(object obj, Type targetType)
    {
        if (obj == null)
        {
            return null;
        }
        MyFileStreamReference myFileStreamRef = obj as MyFileStreamReference;
        if (null != myFileStreamRef)
        {
            if (targetType != typeof (MyFileStream))
            {
                throw new ArgumentException("Target type for deserialization must be MyFileStream");
            }
            return myFileStreamRef.ToMyFileStream();
        }
        return obj;
    }
}

#endregion

public class TypeWithBinaryProperty
{
    [XmlElement(DataType = "hexBinary")]
    public byte[] BinaryHexContent { get; set; }
    [XmlElement(DataType = "base64Binary")]
    public byte[] Base64Content { get; set; }
}

public class TypeWithTimeSpanProperty
{
    public TimeSpan TimeSpanProperty;
}

public class TypeWithByteProperty
{
    public byte ByteProperty;
}

[DataContract(Name = "TypeWithIntAndStringProperty", Namespace = "")]
public class TypeWithIntAndStringProperty
{
    [DataMember]
    public int SampleInt { get; set; }

    [DataMember]
    public string SampleString { get; set; }
}

[DataContract(Name = "TypeWithTypeWithIntAndStringPropertyProperty", Namespace = "")]
public class TypeWithTypeWithIntAndStringPropertyProperty
{
    [DataMember]
    public TypeWithIntAndStringProperty ObjectProperty { get; set; }
}

[DataContract]
public class TypeWithCollectionInterfaceGetOnlyCollection
{
    List<string> items;

    [DataMember]
    public ICollection<string> Items
    {
        get
        {
            if (items == null)
            {
                items = new List<string>();
            }
            return this.items;
        }
    }

    public TypeWithCollectionInterfaceGetOnlyCollection() { }

    public TypeWithCollectionInterfaceGetOnlyCollection(List<string> items)
    {
        this.items = items;
    }
}

[DataContract]
public class TypeWithEnumerableInterfaceGetOnlyCollection
{
    List<string> items;

    [DataMember]
    public IEnumerable<string> Items
    {
        get
        {
            if (items == null)
            {
                items = new List<string>();
            }
            return this.items;
        }
    }

    public TypeWithEnumerableInterfaceGetOnlyCollection() { }

    public TypeWithEnumerableInterfaceGetOnlyCollection(List<string> items)
    {
        this.items = items;
    }
}

[CollectionDataContract]
public class RecursiveCollection : List<RecursiveCollection2>
{

}

[CollectionDataContract]
public class RecursiveCollection2 : List<RecursiveCollection3>
{

}

[CollectionDataContract]
public class RecursiveCollection3 : List<RecursiveCollection>
{

}
