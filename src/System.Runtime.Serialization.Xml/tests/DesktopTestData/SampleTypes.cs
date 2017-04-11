using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DesktopTestData
{
    // Not using KnownType attribute.
    [DataContract]
    public class CircularLinkOne
    {
        [DataMember]
        public CircularLinkOne Link;

        [DataMember]
        public CircularLinkOne RandomHangingLink;

        public CircularLinkOne() { }

        public CircularLinkOne(bool init)
        {
            Link = new CircularLinkOne();
            Link.Link = new CircularLinkOne();
            Link.Link.Link = this;

            RandomHangingLink = new CircularLinkOne();
            RandomHangingLink.Link = new CircularLinkOne();
            RandomHangingLink.Link.Link = new CircularLinkOneDerived();
            RandomHangingLink.Link.Link.Link = RandomHangingLink;
        }
    }

    // Not using KnownType attribute.
    [DataContract]
    public class CircularLinkOneDerived : CircularLinkOne
    {
        public CircularLinkOneDerived() { }
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
    public class CircularLinkDerived : CircularLink
    {
        public CircularLinkDerived() { }
        public CircularLinkDerived(bool inti) : base() { }
    }


    #region KT Methods
    [DataContract(IsReference = true)]
    [KnownType("GetKT")]
    public class KT1Base
    {
        [DataMember]
        public KT1Base BData;

        public KT1Base()
        { }
        public KT1Base(bool init)
        {
            BData = new KT1Derived();
        }

        private static Type[] GetKT()
        {
            return new Type[] { typeof(KT1Derived) };
        }

        public override bool Equals(object obj)
        {
            KT1Base kt1 = obj as KT1Base;
            if (kt1 == null) return false;
            if (this.BData == null || kt1.BData == null)
            {
                if (this.BData == null && kt1.BData == null) { return true; }
                else { return false; }
            }
            else
            {
                return this.BData.Equals(kt1.BData);
            }

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


    [DataContract(IsReference = true)]
    public class KT1Derived : KT1Base
    {
        [DataMember]
        public string DData;

        public KT1Derived()
        {
            this.DData = DateTimeOffset.Now.ToString();
        }

        public override bool Equals(object obj)
        {
            KT1Derived kt1 = obj as KT1Derived;
            if (kt1 == null) return false;
            if (this.DData != kt1.DData) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


    [DataContract(IsReference = true)]
    [KnownType("GetKT")]
    public class KT2Base
    {
        [DataMember]
        public KT2Base BData;

        public KT2Base()
        { }
        public KT2Base(bool init)
        {
            BData = new KT2Derived();
        }

        public static Type[] GetKT()
        {
            return new Type[] { typeof(KT2Derived) };
        }

        public override bool Equals(object obj)
        {
            KT2Base kt1 = obj as KT2Base;
            if (kt1 == null) return false;
            if (this.BData == null || kt1.BData == null)
            {
                if (this.BData == null && kt1.BData == null) { return true; }
                else { return false; }
            }
            else
            {
                return this.BData.Equals(kt1.BData);
            }

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


    [DataContract(IsReference = true)]
    [KnownType("GetKT")]
    public class KT3BaseKTMReturnsPrivateType
    {
        [DataMember]
        public KT3BaseKTMReturnsPrivateType BData;

        public KT3BaseKTMReturnsPrivateType()
        { }
        public KT3BaseKTMReturnsPrivateType(bool init)
        {
            BData = new KT3DerivedPrivate();
        }

        public static Type[] GetKT()
        {
            return new Type[] { typeof(KT3DerivedPrivate) };
        }

        public override bool Equals(object obj)
        {
            KT3BaseKTMReturnsPrivateType kt1 = obj as KT3BaseKTMReturnsPrivateType;
            if (kt1 == null) return false;
            if (this.BData == null || kt1.BData == null)
            {
                if (this.BData == null && kt1.BData == null) { return true; }
                else { return false; }
            }
            else
            {
                return this.BData.Equals(kt1.BData);
            }

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [DataContract(IsReference = true)]
    class KT3DerivedPrivate : KT3BaseKTMReturnsPrivateType
    {
        [DataMember]
        public string DData;

        public KT3DerivedPrivate()
        {
            this.DData = DateTimeOffset.Now.ToString();
        }

        public override bool Equals(object obj)
        {
            KT3DerivedPrivate kt1 = obj as KT3DerivedPrivate;
            if (kt1 == null) return false;
            if (this.DData != kt1.DData) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


    [DataContract(IsReference = true)]
    public class KT2Derived : KT2Base
    {
        [DataMember]
        public string DData;

        public KT2Derived()
        {
            this.DData = DateTimeOffset.Now.ToString();
        }

        public override bool Equals(object obj)
        {
            KT2Derived kt1 = obj as KT2Derived;
            if (kt1 == null) return false;
            if (this.DData != kt1.DData) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    #endregion

    [CollectionDataContract(IsReference = true)]
    [KnownType(typeof(PublicDC))]
    [KnownType(typeof(PublicDCClassPublicDM))]
    [KnownType(typeof(PublicDCClassPrivateDM_DerivedDCClassPublic))]
    public class CB1 : CollectionBase
    {
        public CB1() { }

        public CB1(bool init)
        {
            this.List.Add(new PublicDC());
            this.List.Add(new PublicDCClassPublicDM());
            this.List.Add(new PublicDCClassPrivateDM_DerivedDCClassPublic());
        }
    }

    [DataContract(IsReference = true)]
    [KnownType(typeof(PublicDC))]
    [KnownType(typeof(PublicDCClassPublicDM))]
    public class ArrayListWithCDCFilledPublicTypes
    {
        [DataMember]
        public ArrayList List = new ArrayList();

        public ArrayListWithCDCFilledPublicTypes() { }

        public ArrayListWithCDCFilledPublicTypes(bool init)
        {
            this.List.Add(new PublicDC());
            this.List.Add(new PublicDCClassPublicDM(true));
        }
    }

    [DataContract(IsReference = true)]
    [KnownType(typeof(PublicDC))]
    [KnownType(typeof(PublicDCClassPublicDM))]
    [KnownType(typeof(PublicDCClassPrivateDM_DerivedDCClassPublic))]
    [KnownType(typeof(PrivateDCClassPublicDM_DerivedDCClassPrivate))]
    [KnownType(typeof(PrivateDCClassPrivateDM))]
    [KnownType(typeof(PrivateCallBackSample_IDeserializationCallback))]
    [KnownType(typeof(PrivateCallBackSample_OnDeserialized))]
    [KnownType(typeof(PrivateCallBackSample_OnSerialized))]
    [KnownType(typeof(PrivateDCStruct))]
    [KnownType(typeof(PrivateDefaultCtorIXmlSerializables))]
    [KnownType(typeof(PrivateIXmlSerializables))]
    [KnownType(typeof(Derived_Override_Prop_GetPrivate_Private))]
    [KnownType(typeof(DerivedFromPriC))]
    public class ArrayListWithCDCFilledWithMixedTypes
    {

        [DataMember]
        public ArrayList List = new ArrayList();
        public ArrayListWithCDCFilledWithMixedTypes() { }

        public ArrayListWithCDCFilledWithMixedTypes(bool init)
        {
            this.List.Add(new PublicDC());
            this.List.Add(new PublicDCClassPublicDM(true));
            this.List.Add(new PublicDCClassPrivateDM_DerivedDCClassPublic());

            this.List.Add(new PrivateDCClassPublicDM_DerivedDCClassPrivate());
            this.List.Add(new PrivateDCClassPrivateDM(true));
            this.List.Add(new PrivateCallBackSample_IDeserializationCallback());
            this.List.Add(new PrivateCallBackSample_OnDeserialized());
            this.List.Add(new PrivateCallBackSample_OnSerialized());
            this.List.Add(new PrivateDCStruct(true));
            this.List.Add(new PrivateDefaultCtorIXmlSerializables(true));
            this.List.Add(new PrivateIXmlSerializables());
            //this.List.Add(new DCExplicitInterfaceIObjRefReturnsPrivate());
            this.List.Add(new Derived_Override_Prop_GetPrivate_Private(true));
            this.List.Add(new DerivedFromPriC(100));
        }
    }

    [CollectionDataContract(IsReference = true)]
    [KnownType(typeof(PublicDC))]
    [KnownType(typeof(PublicDCClassPublicDM))]
    [KnownType(typeof(PublicDCClassPrivateDM_DerivedDCClassPublic))]
    public class CollectionBaseWithCDCFilledPublicTypes : CollectionBase
    {
        public CollectionBaseWithCDCFilledPublicTypes() { }

        public CollectionBaseWithCDCFilledPublicTypes(bool init)
        {
            this.List.Add(new PublicDC());
            this.List.Add(new PublicDCClassPublicDM(true));
        }
    }

    [CollectionDataContract(IsReference = true)]
    [KnownType(typeof(PublicDC))]
    [KnownType(typeof(PublicDCClassPublicDM))]
    [KnownType(typeof(PublicDCClassPrivateDM_DerivedDCClassPublic))]
    [KnownType(typeof(PrivateDCClassPublicDM_DerivedDCClassPrivate))]
    [KnownType(typeof(PrivateDCClassPrivateDM))]
    [KnownType(typeof(PrivateCallBackSample_IDeserializationCallback))]
    [KnownType(typeof(PrivateCallBackSample_OnDeserialized))]
    [KnownType(typeof(PrivateCallBackSample_OnSerialized))]
    [KnownType(typeof(PrivateDCStruct))]
    [KnownType(typeof(PrivateDefaultCtorIXmlSerializables))]
    [KnownType(typeof(PrivateIXmlSerializables))]
    [KnownType(typeof(Derived_Override_Prop_GetPrivate_Private))]
    [KnownType(typeof(DerivedFromPriC))]
    public class CollectionBaseWithCDCFilledWithMixedTypes : CollectionBase
    {
        public CollectionBaseWithCDCFilledWithMixedTypes() { }

        public CollectionBaseWithCDCFilledWithMixedTypes(bool init)
        {
            this.List.Add(new PublicDC());
            this.List.Add(new PublicDCClassPublicDM(true));
            this.List.Add(new PublicDCClassPrivateDM_DerivedDCClassPublic());

            this.List.Add(new PrivateDCClassPublicDM_DerivedDCClassPrivate());
            this.List.Add(new PrivateDCClassPrivateDM(true));
            this.List.Add(new PrivateCallBackSample_IDeserializationCallback());
            this.List.Add(new PrivateCallBackSample_OnDeserialized());
            this.List.Add(new PrivateCallBackSample_OnSerialized());
            this.List.Add(new PrivateDCStruct(true));
            this.List.Add(new PrivateDefaultCtorIXmlSerializables(true));
            this.List.Add(new PrivateIXmlSerializables());
            this.List.Add(new Derived_Override_Prop_GetPrivate_Private(true));
            this.List.Add(new DerivedFromPriC(100));
        }
    }

    [DataContract(IsReference = true)]
    [KnownType(typeof(PublicDC))]
    [KnownType(typeof(PublicDCClassPublicDM))]
    [KnownType(typeof(PublicDCClassPrivateDM_DerivedDCClassPublic))]
    [KnownType(typeof(Guid))]
    [KnownType(typeof(AllTypes))]
    [KnownType(typeof(DateTimeOffset))]
    public class DCHashtableContainerPublic
    {
        [DataMember]
        public Hashtable List = new Hashtable();

        public DCHashtableContainerPublic() { }
        public DCHashtableContainerPublic(bool init)
        {
            this.List.Add(Guid.NewGuid(), new PublicDC());
            this.List.Add(Guid.NewGuid(), new PublicDCClassPublicDM(true));
            this.List.Add(int.MaxValue, int.MinValue);
            this.List.Add("null", null);
            this.List.Add(DateTime.Now, DateTime.MaxValue);
            this.List.Add(DateTimeOffset.MaxValue, DateTimeOffset.Now);
            this.List.Add(Guid.NewGuid(), new AllTypes());
        }
    }


    [DataContract(IsReference = true)]
    [KnownType(typeof(PublicDC))]
    [KnownType(typeof(PublicDCClassPublicDM))]
    [KnownType(typeof(PublicDCClassPrivateDM_DerivedDCClassPublic))]
    [KnownType(typeof(PrivateDCClassPublicDM_DerivedDCClassPrivate))]
    [KnownType(typeof(PrivateDCClassPrivateDM))]
    [KnownType(typeof(PrivateCallBackSample_IDeserializationCallback))]
    [KnownType(typeof(PrivateCallBackSample_OnDeserialized))]
    [KnownType(typeof(PrivateCallBackSample_OnSerialized))]
    [KnownType(typeof(PrivateDCStruct))]
    [KnownType(typeof(PrivateDefaultCtorIXmlSerializables))]
    [KnownType(typeof(PrivateIXmlSerializables))]
    [KnownType(typeof(Derived_Override_Prop_GetPrivate_Private))]
    [KnownType(typeof(DerivedFromPriC))]
    [KnownType(typeof(Guid))]
    [KnownType(typeof(AllTypes))]
    [KnownType(typeof(DateTimeOffset))]
    public class DCHashtableContainerMixedTypes
    {
        [DataMember]
        public Hashtable List = new Hashtable();

        public DCHashtableContainerMixedTypes() { }
        public DCHashtableContainerMixedTypes(bool init)
        {
            this.List.Add(Guid.NewGuid(), new PublicDC());
            this.List.Add(Guid.NewGuid(), new PublicDCClassPublicDM(true));
            this.List.Add(Guid.NewGuid(), new PublicDCClassPrivateDM_DerivedDCClassPublic());

            this.List.Add(Guid.NewGuid(), new PrivateDCClassPublicDM_DerivedDCClassPrivate());
            this.List.Add(Guid.NewGuid(), new PrivateDCClassPrivateDM(true));
            this.List.Add(Guid.NewGuid(), new PrivateCallBackSample_IDeserializationCallback());
            this.List.Add(Guid.NewGuid(), new PrivateCallBackSample_OnDeserialized());
            this.List.Add(Guid.NewGuid(), new PrivateCallBackSample_OnSerialized());
            this.List.Add(Guid.NewGuid(), new PrivateDCStruct(true));
            this.List.Add(Guid.NewGuid(), new PrivateDefaultCtorIXmlSerializables(true));
            this.List.Add(Guid.NewGuid(), new PrivateIXmlSerializables());
            //this.List.Add(Guid.NewGuid(), new DCExplicitInterfaceIObjRefReturnsPrivate());
            this.List.Add(Guid.NewGuid(), new Derived_Override_Prop_GetPrivate_Private(true));
            this.List.Add(Guid.NewGuid(), new DerivedFromPriC(100));

            this.List.Add(String.Empty, String.Empty);
            this.List.Add("null", null);
            this.List.Add(double.MaxValue, double.MinValue);

            this.List.Add(DateTime.Now, DateTime.MaxValue);
            this.List.Add(DateTimeOffset.MaxValue, DateTimeOffset.Now);
            this.List.Add(Guid.NewGuid(), new AllTypes());

        }
    }


    #region Custom Generics

    #region Generic Type Private

    [DataContract(IsReference = true)]
    public class CustomGenericContainerPrivateType1
    {
        [DataMember]
        CustomGeneric1<PrivateDC> data1 = new CustomGeneric1<PrivateDC>();

    }


    [DataContract(IsReference = true)]
    public class CustomGenericContainerPrivateType2
    {
        [DataMember]
        CustomGeneric2<PrivateDC, PrivateDC> data1 = new CustomGeneric2<PrivateDC, PrivateDC>();

    }

    [DataContract(IsReference = true)]
    public class CustomGenericContainerPrivateType3
    {
        [DataMember]
        CustomGeneric2<PublicDC, PublicDCClassPublicDM_DerivedDCClassPrivate> data1 = new CustomGeneric2<PublicDC, PublicDCClassPublicDM_DerivedDCClassPrivate>();

    }

    [DataContract(IsReference = true)]
    public class CustomGenericContainerPrivateType4
    {
        [DataMember]
        CustomGeneric2<PublicDC, PublicDCClassPrivateDM> data1 = new CustomGeneric2<PublicDC, PublicDCClassPrivateDM>();

    }


    [DataContract(IsReference = true)]
    public class CustomGenericContainerPublicType1
    {
        [DataMember]
        public CustomGeneric1<PublicDC> data1 = new CustomGeneric1<PublicDC>();

    }


    [DataContract(IsReference = true)]
    public class CustomGenericContainerPublicType2
    {
        [DataMember]
        public CustomGeneric2<PublicDC, PublicDC> data1 = new CustomGeneric2<PublicDC, PublicDC>();

    }

    [DataContract(IsReference = true)]
    public class CustomGenericContainerPublicType3
    {
        [DataMember]
        public CustomGeneric2<PublicDC, PublicDCClassPublicDM_DerivedDCClassPublic> data1 = new CustomGeneric2<PublicDC, PublicDCClassPublicDM_DerivedDCClassPublic>();

    }

    [DataContract(IsReference = true)]
    public class CustomGenericContainerPublicType4
    {
        [DataMember]
        public CustomGeneric2<PublicDC, PublicDCClassPublicDM> data1 = new CustomGeneric2<PublicDC, PublicDCClassPublicDM>();

    }



    [DataContract(IsReference = true)]
    public class CustomGeneric1<T> where T : new()
    {
        [DataMember]
        public T t = new T();
    }

    [DataContract(IsReference = true)]
    public class CustomGeneric2<T, K>
        where T : new()
        where K : new()
    {
        [DataMember]
        public T t = new T();

        [DataMember]
        public K k = new K();
    }


    #endregion

    #region Generics with KnownTypes


    [DataContract(IsReference = true)]
    [KnownType(typeof(GenericBase<SimpleBaseContainer>))]
    [KnownType(typeof(SimpleBaseContainer))]
    public class GenericContainer
    {
        [DataMember]
        public object GenericData;
        public GenericContainer()
        {
        }
        public GenericContainer(bool init)
        {
            GenericData = new GenericBase<SimpleBaseContainer>();
        }
    }

    [DataContract(IsReference = true)]
    public class GenericBase<T> where T : new()
    {
        [DataMember]
        public object genericData = new T();
    }

    [DataContract(IsReference = true)]
    public class GenericBase2<T, K>
        where T : new()
        where K : new()
    {
        [DataMember]
        public T genericData1 = new T();

        [DataMember]
        public K genericData2 = new K();
    }

    [DataContract(IsReference = true)]
    [KnownType(typeof(SimpleBaseDerived))]
    public class SimpleBase
    {
        [DataMember]
        public string BaseData = String.Empty;
    }

    [DataContract(IsReference = true)]
    public class SimpleBaseDerived : SimpleBase
    {
        [DataMember]
        public string DerivedData = String.Empty;
    }

    [DataContract(IsReference = true)]
    public class SimpleBaseDerived2 : SimpleBase
    {
        [DataMember]
        public string DerivedData = String.Empty;
    }

    [DataContract(IsReference = true)]
    [KnownType(typeof(SimpleBaseDerived2))]
    public class SimpleBaseContainer
    {
        [DataMember]
        public SimpleBase Base1;

        [DataMember]
        public object Base2;

        public SimpleBaseContainer()
        { }
        public SimpleBaseContainer(bool init)
        {
            Base1 = new SimpleBaseDerived();
            Base2 = new SimpleBaseDerived2();
        }
    }

    #endregion

    #endregion


    [DataContract(IsReference = true)]
    [KnownType(typeof(PrivateDC))]
    public class DCListPrivateTContainer2
    {
        [DataMember]
        public List<object> ListData = new List<object>();
        public DCListPrivateTContainer2() { ListData.Add(new PrivateDC()); }
    }


    [DataContract(IsReference = true)]
    public class DCListPrivateTContainer
    {
        [DataMember]
        List<PrivateDC> ListData = new List<PrivateDC>();
        public DCListPrivateTContainer() { ListData.Add(new PrivateDC()); }
    }

    [DataContract(IsReference = true)]
    public class DCListPublicTContainer
    {
        [DataMember]
        public List<PublicDC> ListData = new List<PublicDC>();
        public DCListPublicTContainer() { ListData.Add(new PublicDC()); }
    }

    [DataContract(IsReference = true)]
    [KnownType(typeof(PrivateDC))]
    [KnownType(typeof(PublicDC))]
    public class DCListMixedTContainer
    {
        [DataMember]
        List<object> ListData = new List<object>();
        public DCListMixedTContainer()
        {
            ListData.Add(new PublicDC());
            ListData.Add(new PrivateDC());
        }
    }

    #region IList

    [DataContract(IsReference = true)]
    class SamplePrivateListImplicitWithDC : IList
    {
        List<object> internalList = new List<object>();

        public SamplePrivateListImplicitWithDC() { }

        public SamplePrivateListImplicitWithDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }

        #region IList Members

        public int Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }

        public void Clear()
        {
            this.internalList.Clear();
        }

        public bool Contains(object value)
        {
            return this.internalList.Contains(value);
        }

        public int IndexOf(object value)
        {
            return this.internalList.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            this.internalList.Insert(index, value);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            this.internalList.Remove(value);
        }

        public void RemoveAt(int index)
        {
            this.internalList.Remove(index);
        }

        public object this[int index]
        {
            get
            {
                return this.internalList[index];
            }
            set
            {
                this.internalList[index] = value;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            //this.internalList.CopyTo(array, index);
        }

        public int Count
        {
            get { return this.internalList.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(Name = "SampleListImplicitWithDC")]
    [KnownType(typeof(SimpleDCWithRef))]
    public class SampleListImplicitWithDC : IList
    {
        List<object> internalList = new List<object>();

        public SampleListImplicitWithDC() { }

        public SampleListImplicitWithDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
            this.internalList.Add(new SimpleDCWithRef(true));
            //this.internalList.Add(this.internalList[7]);
        }

        #region IList Members

        public int Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }

        public void Clear()
        {
            this.internalList.Clear();
        }

        public bool Contains(object value)
        {
            return this.internalList.Contains(value);
        }

        public int IndexOf(object value)
        {
            return this.internalList.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            this.internalList.Insert(index, value);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            this.internalList.Remove(value);
        }

        public void RemoveAt(int index)
        {
            this.internalList.Remove(index);
        }

        public object this[int index]
        {
            get
            {
                return this.internalList[index];
            }
            set
            {
                this.internalList[index] = value;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            //this.internalList.CopyTo(array, index);
        }

        public int Count
        {
            get { return this.internalList.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(Name = "SampleListImplicitWithoutDC")]
    public class SampleListImplicitWithoutDC : IList
    {
        List<object> internalList = new List<object>();

        public SampleListImplicitWithoutDC() { }

        public SampleListImplicitWithoutDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }

        #region IList Members

        public int Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }

        public void Clear()
        {
            this.internalList.Clear();
        }

        public bool Contains(object value)
        {
            return this.internalList.Contains(value);
        }

        public int IndexOf(object value)
        {
            return this.internalList.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            this.internalList.Insert(index, value);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            this.internalList.Remove(value);
        }

        public void RemoveAt(int index)
        {
            this.internalList.Remove(index);
        }

        public object this[int index]
        {
            get
            {
                return this.internalList[index];
            }
            set
            {
                this.internalList[index] = value;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            //this.internalList.CopyTo(array, index);
        }

        public int Count
        {
            get { return this.internalList.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SampleListImplicitWithCDC1", Namespace = "Test", ItemName = "Item")]
    public class SampleListImplicitWithCDC : IList
    {
        List<object> internalList = new List<object>();

        public SampleListImplicitWithCDC() { }

        public SampleListImplicitWithCDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }

        #region IList Members

        public int Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }

        public void Clear()
        {
            this.internalList.Clear();
        }

        public bool Contains(object value)
        {
            return this.internalList.Contains(value);
        }

        public int IndexOf(object value)
        {
            return this.internalList.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            this.internalList.Insert(index, value);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            this.internalList.Remove(value);
        }

        public void RemoveAt(int index)
        {
            this.internalList.Remove(index);
        }

        public object this[int index]
        {
            get
            {
                return this.internalList[index];
            }
            set
            {
                this.internalList[index] = value;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            //this.internalList.CopyTo(array, index);
        }

        public int Count
        {
            get { return this.internalList.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [DataContract(IsReference = true)]
    public class SampleListExplicitWithDC : IList
    {
        List<object> internalList = new List<object>();
        public SampleListExplicitWithDC() { }
        public SampleListExplicitWithDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }
        #region IList Members

        int IList.Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }

        void IList.Clear()
        {
            this.internalList.Clear();
        }

        bool IList.Contains(object value)
        {
            return this.internalList.Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return this.internalList.IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            this.internalList.Insert(index, value);
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        void IList.Remove(object value)
        {
            this.internalList.Remove(value);
        }

        void IList.RemoveAt(int index)
        {
            this.internalList.Remove(index);
        }

        object IList.this[int index]
        {
            get
            {
                return this.internalList[index];
            }
            set
            {
                this.internalList[index] = value;
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            //this.internalList.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return this.internalList.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(Name = "SampleListExplicitWithoutDC")]
    public class SampleListExplicitWithoutDC : IList
    {
        List<object> internalList = new List<object>();
        public SampleListExplicitWithoutDC() { }

        public SampleListExplicitWithoutDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }
        #region IList Members

        int IList.Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }

        void IList.Clear()
        {
            this.internalList.Clear();
        }

        bool IList.Contains(object value)
        {
            return this.internalList.Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return this.internalList.IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            this.internalList.Insert(index, value);
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        void IList.Remove(object value)
        {
            this.internalList.Remove(value);
        }

        void IList.RemoveAt(int index)
        {
            this.internalList.Remove(index);
        }

        object IList.this[int index]
        {
            get
            {
                return this.internalList[index];
            }
            set
            {
                this.internalList[index] = value;
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            //this.internalList.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return this.internalList.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SampleListExplicitWithCDC2", Namespace = "Test", ItemName = "Item")]
    public class SampleListExplicitWithCDC : IList
    {
        List<object> internalList = new List<object>();
        public SampleListExplicitWithCDC() { }
        public SampleListExplicitWithCDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }
        #region IList Members

        int IList.Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }

        void IList.Clear()
        {
            this.internalList.Clear();
        }

        bool IList.Contains(object value)
        {
            return this.internalList.Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return this.internalList.IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            this.internalList.Insert(index, value);
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        void IList.Remove(object value)
        {
            this.internalList.Remove(value);
        }

        void IList.RemoveAt(int index)
        {
            this.internalList.Remove(index);
        }

        object IList.this[int index]
        {
            get
            {
                return this.internalList[index];
            }
            set
            {
                this.internalList[index] = value;
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            //this.internalList.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return this.internalList.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SampleListExplicitWithCDCContainsPrivateDC", Namespace = "Test", ItemName = "Item")]
    [KnownType(typeof(PrivateDC))]
    public class SampleListExplicitWithCDCContainsPrivateDC : IList
    {
        List<object> internalList = new List<object>();
        public SampleListExplicitWithCDCContainsPrivateDC() { }
        public SampleListExplicitWithCDCContainsPrivateDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
            this.internalList.Add(new PrivateDC());
        }
        #region IList Members

        int IList.Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }

        void IList.Clear()
        {
            this.internalList.Clear();
        }

        bool IList.Contains(object value)
        {
            return this.internalList.Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return this.internalList.IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            this.internalList.Insert(index, value);
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        void IList.Remove(object value)
        {
            this.internalList.Remove(value);
        }

        void IList.RemoveAt(int index)
        {
            this.internalList.Remove(index);
        }

        object IList.this[int index]
        {
            get
            {
                return this.internalList[index];
            }
            set
            {
                this.internalList[index] = value;
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            //this.internalList.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return this.internalList.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SamplePrivateListExplicitWithCDC", Namespace = "Test", ItemName = "Item")]
    class SamplePrivateListExplicitWithCDC : IList
    {
        List<object> internalList = new List<object>();

        public SamplePrivateListExplicitWithCDC() { }
        public SamplePrivateListExplicitWithCDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }
        #region IList Members

        int IList.Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }

        void IList.Clear()
        {
            this.internalList.Clear();
        }

        bool IList.Contains(object value)
        {
            return this.internalList.Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return this.internalList.IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            this.internalList.Insert(index, value);
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        void IList.Remove(object value)
        {
            this.internalList.Remove(value);
        }

        void IList.RemoveAt(int index)
        {
            this.internalList.Remove(index);
        }

        object IList.this[int index]
        {
            get
            {
                return this.internalList[index];
            }
            set
            {
                this.internalList[index] = value;
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            //this.internalList.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return this.internalList.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    #endregion

    #region IList<T>

    [DataContract(IsReference = true)]
    class SamplePrivateListTImplicitWithDC : IList<DC>
    {
        List<DC> internalList = new List<DC>();
        public SamplePrivateListTImplicitWithDC() { }

        public SamplePrivateListTImplicitWithDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }

        #region IList<DC> Members

        public int IndexOf(DC item)
        {
            return this.internalList.IndexOf(item);
        }

        public void Insert(int index, DC item)
        {
            this.internalList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.internalList.RemoveAt(index);
        }

        public DC this[int index]
        {
            get
            {
                return this.internalList[index];
            }
            set
            {
                this.internalList[index] = value;
            }
        }

        #endregion

        #region ICollection<DC> Members

        public void Add(DC item)
        {
            this.internalList.Add(item);
        }

        public void Clear()
        {
            this.internalList.Clear();
        }

        public bool Contains(DC item)
        {
            return this.internalList.Contains(item);
        }

        public void CopyTo(DC[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.internalList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(DC item)
        {
            return this.internalList.Remove(item); ;
        }

        #endregion

        #region IEnumerable<DC> Members

        public IEnumerator<DC> GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [DataContract(IsReference = true)]
    public class SampleListTImplicitWithDC : IList<DC>
    {
        List<DC> internalList = new List<DC>();
        public SampleListTImplicitWithDC() { }

        public SampleListTImplicitWithDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }
        #region IList<DC> Members

        public int IndexOf(DC item)
        {
            return this.internalList.IndexOf(item);
        }

        public void Insert(int index, DC item)
        {
            this.internalList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.internalList.RemoveAt(index);
        }

        public DC this[int index]
        {
            get
            {
                return this.internalList[index];
            }
            set
            {
                this.internalList[index] = value;
            }
        }

        #endregion

        #region ICollection<DC> Members

        public void Add(DC item)
        {
            this.internalList.Add(item);
        }

        public void Clear()
        {
            this.internalList.Clear();
        }

        public bool Contains(DC item)
        {
            return this.internalList.Contains(item);
        }

        public void CopyTo(DC[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.internalList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(DC item)
        {
            return this.internalList.Remove(item); ;
        }

        #endregion

        #region IEnumerable<DC> Members

        public IEnumerator<DC> GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(Name = "SampleListTImplicitWithoutDC")]
    public class SampleListTImplicitWithoutDC : IList<DC>
    {
        List<DC> internalList = new List<DC>();
        public SampleListTImplicitWithoutDC() { }
        public SampleListTImplicitWithoutDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }
        #region IList<DC> Members

        public int IndexOf(DC item)
        {
            return this.internalList.IndexOf(item);
        }

        public void Insert(int index, DC item)
        {
            this.internalList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.internalList.RemoveAt(index);
        }

        public DC this[int index]
        {
            get
            {
                return this.internalList[index];
            }
            set
            {
                this.internalList[index] = value;
            }
        }

        #endregion

        #region ICollection<DC> Members

        public void Add(DC item)
        {
            this.internalList.Add(item);
        }

        public void Clear()
        {
            this.internalList.Clear();
        }

        public bool Contains(DC item)
        {
            return this.internalList.Contains(item);
        }

        public void CopyTo(DC[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.internalList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(DC item)
        {
            return this.internalList.Remove(item); ;
        }

        #endregion

        #region IEnumerable<DC> Members

        public IEnumerator<DC> GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SampleListTImplicitWithCDC", Namespace = "Test", ItemName = "Item")]
    public class SampleListTImplicitWithCDC : IList<DC>
    {
        List<DC> internalList = new List<DC>();
        public SampleListTImplicitWithCDC() { }
        public SampleListTImplicitWithCDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }
        #region IList<DC> Members

        public int IndexOf(DC item)
        {
            return this.internalList.IndexOf(item);
        }

        public void Insert(int index, DC item)
        {
            this.internalList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.internalList.RemoveAt(index);
        }

        public DC this[int index]
        {
            get
            {
                return this.internalList[index];
            }
            set
            {
                this.internalList[index] = value;
            }
        }

        #endregion

        #region ICollection<DC> Members

        public void Add(DC item)
        {
            this.internalList.Add(item);
        }

        public void Clear()
        {
            this.internalList.Clear();
        }

        public bool Contains(DC item)
        {
            return this.internalList.Contains(item);
        }

        public void CopyTo(DC[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.internalList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(DC item)
        {
            return this.internalList.Remove(item); ;
        }

        #endregion

        #region IEnumerable<DC> Members

        public IEnumerator<DC> GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [DataContract(IsReference = true)]
    public class SampleListTExplicitWithDC : IList<DC>
    {
        List<DC> internalList = new List<DC>();
        public SampleListTExplicitWithDC() { }
        public SampleListTExplicitWithDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }
        #region IList<DC> Members

        int IList<DC>.IndexOf(DC item)
        {
            return this.internalList.IndexOf(item);
        }

        void IList<DC>.Insert(int index, DC item)
        {
            this.internalList.Insert(index, item);
        }

        void IList<DC>.RemoveAt(int index)
        {
            this.internalList.RemoveAt(index);
        }

        DC IList<DC>.this[int index]
        {
            get
            {
                return this.internalList[index];
            }
            set
            {
                this.internalList[index] = value;
            }
        }

        #endregion

        #region ICollection<DC> Members

        void ICollection<DC>.Add(DC item)
        {
            this.internalList.Add(item);
        }

        void ICollection<DC>.Clear()
        {
            this.internalList.Clear();
        }

        bool ICollection<DC>.Contains(DC item)
        {
            return this.internalList.Contains(item);
        }

        void ICollection<DC>.CopyTo(DC[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        int ICollection<DC>.Count
        {
            get { return this.internalList.Count; }
        }

        bool ICollection<DC>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<DC>.Remove(DC item)
        {
            return this.internalList.Remove(item); ;
        }

        #endregion

        #region IEnumerable<DC> Members

        IEnumerator<DC> IEnumerable<DC>.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(Name = "SampleListTExplicitWithoutDC")]
    public class SampleListTExplicitWithoutDC : IList<DC>
    {
        List<DC> internalList = new List<DC>();
        public SampleListTExplicitWithoutDC() { }
        public SampleListTExplicitWithoutDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }
        #region IList<DC> Members

        int IList<DC>.IndexOf(DC item)
        {
            return this.internalList.IndexOf(item);
        }

        void IList<DC>.Insert(int index, DC item)
        {
            this.internalList.Insert(index, item);
        }

        void IList<DC>.RemoveAt(int index)
        {
            this.internalList.RemoveAt(index);
        }

        DC IList<DC>.this[int index]
        {
            get
            {
                return this.internalList[index];
            }
            set
            {
                this.internalList[index] = value;
            }
        }

        #endregion

        #region ICollection<DC> Members

        void ICollection<DC>.Add(DC item)
        {
            this.internalList.Add(item);
        }

        void ICollection<DC>.Clear()
        {
            this.internalList.Clear();
        }

        bool ICollection<DC>.Contains(DC item)
        {
            return this.internalList.Contains(item);
        }

        void ICollection<DC>.CopyTo(DC[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        int ICollection<DC>.Count
        {
            get { return this.internalList.Count; }
        }

        bool ICollection<DC>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<DC>.Remove(DC item)
        {
            return this.internalList.Remove(item); ;
        }

        #endregion

        #region IEnumerable<DC> Members

        IEnumerator<DC> IEnumerable<DC>.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SampleListTExplicitWithCDC", Namespace = "Test", ItemName = "Item")]
    public class SampleListTExplicitWithCDC : IList<DC>
    {
        List<DC> internalList = new List<DC>();
        public SampleListTExplicitWithCDC() { }
        public SampleListTExplicitWithCDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }
        #region IList<DC> Members

        int IList<DC>.IndexOf(DC item)
        {
            return this.internalList.IndexOf(item);
        }

        void IList<DC>.Insert(int index, DC item)
        {
            this.internalList.Insert(index, item);
        }

        void IList<DC>.RemoveAt(int index)
        {
            this.internalList.RemoveAt(index);
        }

        DC IList<DC>.this[int index]
        {
            get
            {
                return this.internalList[index];
            }
            set
            {
                this.internalList[index] = value;
            }
        }

        #endregion

        #region ICollection<DC> Members

        void ICollection<DC>.Add(DC item)
        {
            this.internalList.Add(item);
        }

        void ICollection<DC>.Clear()
        {
            this.internalList.Clear();
        }

        bool ICollection<DC>.Contains(DC item)
        {
            return this.internalList.Contains(item);
        }

        void ICollection<DC>.CopyTo(DC[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        int ICollection<DC>.Count
        {
            get { return this.internalList.Count; }
        }

        bool ICollection<DC>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<DC>.Remove(DC item)
        {
            return this.internalList.Remove(item); ;
        }

        #endregion

        #region IEnumerable<DC> Members

        IEnumerator<DC> IEnumerable<DC>.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SampleListTExplicitWithCDCContainsPublicDCClassPrivateDM", Namespace = "Test", ItemName = "Item")]
    public class SampleListTExplicitWithCDCContainsPublicDCClassPrivateDM : IList<PublicDCClassPrivateDM>
    {
        List<PublicDCClassPrivateDM> internalList = new List<PublicDCClassPrivateDM>();
        public SampleListTExplicitWithCDCContainsPublicDCClassPrivateDM() { }
        public SampleListTExplicitWithCDCContainsPublicDCClassPrivateDM(bool init)
        {
            PublicDCClassPrivateDM dc1 = new PublicDCClassPrivateDM();
            this.internalList.Add(dc1);
            this.internalList.Add(new PublicDCClassPrivateDM());
            this.internalList.Add(dc1);
        }
        #region IList<PublicDCClassPrivateDM> Members

        int IList<PublicDCClassPrivateDM>.IndexOf(PublicDCClassPrivateDM item)
        {
            return this.internalList.IndexOf(item);
        }

        void IList<PublicDCClassPrivateDM>.Insert(int index, PublicDCClassPrivateDM item)
        {
            this.internalList.Insert(index, item);
        }

        void IList<PublicDCClassPrivateDM>.RemoveAt(int index)
        {
            this.internalList.RemoveAt(index);
        }

        PublicDCClassPrivateDM IList<PublicDCClassPrivateDM>.this[int index]
        {
            get
            {
                return this.internalList[index];
            }
            set
            {
                this.internalList[index] = value;
            }
        }

        #endregion

        #region ICollection<PublicDCClassPrivateDM> Members

        void ICollection<PublicDCClassPrivateDM>.Add(PublicDCClassPrivateDM item)
        {
            this.internalList.Add(item);
        }

        void ICollection<PublicDCClassPrivateDM>.Clear()
        {
            this.internalList.Clear();
        }

        bool ICollection<PublicDCClassPrivateDM>.Contains(PublicDCClassPrivateDM item)
        {
            return this.internalList.Contains(item);
        }

        void ICollection<PublicDCClassPrivateDM>.CopyTo(PublicDCClassPrivateDM[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        int ICollection<PublicDCClassPrivateDM>.Count
        {
            get { return this.internalList.Count; }
        }

        bool ICollection<PublicDCClassPrivateDM>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<PublicDCClassPrivateDM>.Remove(PublicDCClassPrivateDM item)
        {
            return this.internalList.Remove(item); ;
        }

        #endregion

        #region IEnumerable<PublicDCClassPrivateDM> Members

        IEnumerator<PublicDCClassPrivateDM> IEnumerable<PublicDCClassPrivateDM>.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SampleListTExplicitWithCDCContainsPrivateDC", Namespace = "Test", ItemName = "Item")]
    public class SampleListTExplicitWithCDCContainsPrivateDC : IList<PrivateDC>
    {
        List<PrivateDC> internalList = new List<PrivateDC>();
        public SampleListTExplicitWithCDCContainsPrivateDC() { }
        public SampleListTExplicitWithCDCContainsPrivateDC(bool init)
        {
            PrivateDC dc1 = new PrivateDC();
            this.internalList.Add(dc1);
            this.internalList.Add(new PrivateDC());
            this.internalList.Add(dc1);
        }
        #region IList<PrivateDC> Members

        int IList<PrivateDC>.IndexOf(PrivateDC item)
        {
            return this.internalList.IndexOf(item);
        }

        void IList<PrivateDC>.Insert(int index, PrivateDC item)
        {
            this.internalList.Insert(index, item);
        }

        void IList<PrivateDC>.RemoveAt(int index)
        {
            this.internalList.RemoveAt(index);
        }

        PrivateDC IList<PrivateDC>.this[int index]
        {
            get
            {
                return this.internalList[index];
            }
            set
            {
                this.internalList[index] = value;
            }
        }

        #endregion

        #region ICollection<PrivateDC> Members

        void ICollection<PrivateDC>.Add(PrivateDC item)
        {
            this.internalList.Add(item);
        }

        void ICollection<PrivateDC>.Clear()
        {
            this.internalList.Clear();
        }

        bool ICollection<PrivateDC>.Contains(PrivateDC item)
        {
            return this.internalList.Contains(item);
        }

        void ICollection<PrivateDC>.CopyTo(PrivateDC[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        int ICollection<PrivateDC>.Count
        {
            get { return this.internalList.Count; }
        }

        bool ICollection<PrivateDC>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<PrivateDC>.Remove(PrivateDC item)
        {
            return this.internalList.Remove(item); ;
        }

        #endregion

        #region IEnumerable<PrivateDC> Members

        IEnumerator<PrivateDC> IEnumerable<PrivateDC>.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SamplePrivateListTExplicitWithCDC", Namespace = "Test", ItemName = "Item")]
    class SamplePrivateListTExplicitWithCDC : IList<DC>
    {
        List<DC> internalList = new List<DC>();
        public SamplePrivateListTExplicitWithCDC() { }
        public SamplePrivateListTExplicitWithCDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }
        #region IList<DC> Members

        int IList<DC>.IndexOf(DC item)
        {
            return this.internalList.IndexOf(item);
        }

        void IList<DC>.Insert(int index, DC item)
        {
            this.internalList.Insert(index, item);
        }

        void IList<DC>.RemoveAt(int index)
        {
            this.internalList.RemoveAt(index);
        }

        DC IList<DC>.this[int index]
        {
            get
            {
                return this.internalList[index];
            }
            set
            {
                this.internalList[index] = value;
            }
        }

        #endregion

        #region ICollection<DC> Members

        void ICollection<DC>.Add(DC item)
        {
            this.internalList.Add(item);
        }

        void ICollection<DC>.Clear()
        {
            this.internalList.Clear();
        }

        bool ICollection<DC>.Contains(DC item)
        {
            return this.internalList.Contains(item);
        }

        void ICollection<DC>.CopyTo(DC[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        int ICollection<DC>.Count
        {
            get { return this.internalList.Count; }
        }

        bool ICollection<DC>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<DC>.Remove(DC item)
        {
            return this.internalList.Remove(item); ;
        }

        #endregion

        #region IEnumerable<DC> Members

        IEnumerator<DC> IEnumerable<DC>.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    #endregion

    #region ICollection<T>

    [DataContract(IsReference = true)]
    class SamplePrivateICollectionTImplicitWithDC : ICollection<DC>
    {
        List<DC> internalList = new List<DC>();
        public SamplePrivateICollectionTImplicitWithDC() { }

        public SamplePrivateICollectionTImplicitWithDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }



        #region ICollection<DC> Members

        public void Add(DC item)
        {
            this.internalList.Add(item);
        }

        public void Clear()
        {
            this.internalList.Clear();
        }

        public bool Contains(DC item)
        {
            return this.internalList.Contains(item);
        }

        public void CopyTo(DC[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.internalList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(DC item)
        {
            return this.internalList.Remove(item); ;
        }

        #endregion

        #region IEnumerable<DC> Members

        public IEnumerator<DC> GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [DataContract(IsReference = true)]
    public class SampleICollectionTImplicitWithDC : ICollection<DC>
    {
        List<DC> internalList = new List<DC>();
        public SampleICollectionTImplicitWithDC() { }

        public SampleICollectionTImplicitWithDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }


        #region ICollection<DC> Members

        public void Add(DC item)
        {
            this.internalList.Add(item);
        }

        public void Clear()
        {
            this.internalList.Clear();
        }

        public bool Contains(DC item)
        {
            return this.internalList.Contains(item);
        }

        public void CopyTo(DC[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.internalList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(DC item)
        {
            return this.internalList.Remove(item); ;
        }

        #endregion

        #region IEnumerable<DC> Members

        public IEnumerator<DC> GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(Name = "SampleICollectionTImplicitWithoutDC")]
    public class SampleICollectionTImplicitWithoutDC : ICollection<DC>
    {
        List<DC> internalList = new List<DC>();
        public SampleICollectionTImplicitWithoutDC() { }
        public SampleICollectionTImplicitWithoutDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }


        #region ICollection<DC> Members

        public void Add(DC item)
        {
            this.internalList.Add(item);
        }

        public void Clear()
        {
            this.internalList.Clear();
        }

        public bool Contains(DC item)
        {
            return this.internalList.Contains(item);
        }

        public void CopyTo(DC[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.internalList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(DC item)
        {
            return this.internalList.Remove(item); ;
        }

        #endregion

        #region IEnumerable<DC> Members

        public IEnumerator<DC> GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SampleICollectionTImplicitWithCDC", Namespace = "Test", ItemName = "Item")]
    public class SampleICollectionTImplicitWithCDC : ICollection<DC>
    {
        List<DC> internalList = new List<DC>();
        public SampleICollectionTImplicitWithCDC() { }
        public SampleICollectionTImplicitWithCDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }

        #region ICollection<DC> Members

        public void Add(DC item)
        {
            this.internalList.Add(item);
        }

        public void Clear()
        {
            this.internalList.Clear();
        }

        public bool Contains(DC item)
        {
            return this.internalList.Contains(item);
        }

        public void CopyTo(DC[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.internalList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(DC item)
        {
            return this.internalList.Remove(item); ;
        }

        #endregion

        #region IEnumerable<DC> Members

        public IEnumerator<DC> GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [DataContract(IsReference = true)]
    public class SampleICollectionTExplicitWithDC : ICollection<DC>
    {
        List<DC> internalList = new List<DC>();
        public SampleICollectionTExplicitWithDC() { }
        public SampleICollectionTExplicitWithDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }


        #region ICollection<DC> Members

        void ICollection<DC>.Add(DC item)
        {
            this.internalList.Add(item);
        }

        void ICollection<DC>.Clear()
        {
            this.internalList.Clear();
        }

        bool ICollection<DC>.Contains(DC item)
        {
            return this.internalList.Contains(item);
        }

        void ICollection<DC>.CopyTo(DC[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        int ICollection<DC>.Count
        {
            get { return this.internalList.Count; }
        }

        bool ICollection<DC>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<DC>.Remove(DC item)
        {
            return this.internalList.Remove(item); ;
        }

        #endregion

        #region IEnumerable<DC> Members

        IEnumerator<DC> IEnumerable<DC>.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(Name = "SampleICollectionTExplicitWithoutDC")]
    public class SampleICollectionTExplicitWithoutDC : ICollection<DC>
    {
        List<DC> internalList = new List<DC>();
        public SampleICollectionTExplicitWithoutDC() { }
        public SampleICollectionTExplicitWithoutDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }


        #region ICollection<DC> Members

        void ICollection<DC>.Add(DC item)
        {
            this.internalList.Add(item);
        }

        void ICollection<DC>.Clear()
        {
            this.internalList.Clear();
        }

        bool ICollection<DC>.Contains(DC item)
        {
            return this.internalList.Contains(item);
        }

        void ICollection<DC>.CopyTo(DC[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        int ICollection<DC>.Count
        {
            get { return this.internalList.Count; }
        }

        bool ICollection<DC>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<DC>.Remove(DC item)
        {
            return this.internalList.Remove(item); ;
        }

        #endregion

        #region IEnumerable<DC> Members

        IEnumerator<DC> IEnumerable<DC>.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SampleICollectionTExplicitWithCDC", Namespace = "Test", ItemName = "Item")]
    public class SampleICollectionTExplicitWithCDC : ICollection<DC>
    {
        List<DC> internalList = new List<DC>();
        public SampleICollectionTExplicitWithCDC() { }
        public SampleICollectionTExplicitWithCDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }

        #region ICollection<DC> Members

        void ICollection<DC>.Add(DC item)
        {
            this.internalList.Add(item);
        }

        void ICollection<DC>.Clear()
        {
            this.internalList.Clear();
        }

        bool ICollection<DC>.Contains(DC item)
        {
            return this.internalList.Contains(item);
        }

        void ICollection<DC>.CopyTo(DC[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        int ICollection<DC>.Count
        {
            get { return this.internalList.Count; }
        }

        bool ICollection<DC>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<DC>.Remove(DC item)
        {
            return this.internalList.Remove(item); ;
        }

        #endregion

        #region IEnumerable<DC> Members

        IEnumerator<DC> IEnumerable<DC>.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SampleICollectionTExplicitWithCDCContainsPrivateDC", Namespace = "Test", ItemName = "Item")]
    public class SampleICollectionTExplicitWithCDCContainsPrivateDC : ICollection<PrivateDC>
    {
        List<PrivateDC> internalList = new List<PrivateDC>();
        public SampleICollectionTExplicitWithCDCContainsPrivateDC() { }
        public SampleICollectionTExplicitWithCDCContainsPrivateDC(bool init)
        {
            PrivateDC dc1 = new PrivateDC();
            this.internalList.Add(dc1);
            this.internalList.Add(new PrivateDC());
            this.internalList.Add(dc1);
        }

        #region ICollection<PrivateDC> Members

        void ICollection<PrivateDC>.Add(PrivateDC item)
        {
            this.internalList.Add(item);
        }

        void ICollection<PrivateDC>.Clear()
        {
            this.internalList.Clear();
        }

        bool ICollection<PrivateDC>.Contains(PrivateDC item)
        {
            return this.internalList.Contains(item);
        }

        void ICollection<PrivateDC>.CopyTo(PrivateDC[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        int ICollection<PrivateDC>.Count
        {
            get { return this.internalList.Count; }
        }

        bool ICollection<PrivateDC>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<PrivateDC>.Remove(PrivateDC item)
        {
            return this.internalList.Remove(item); ;
        }

        #endregion

        #region IEnumerable<PrivateDC> Members

        IEnumerator<PrivateDC> IEnumerable<PrivateDC>.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SamplePrivateICollectionTExplicitWithCDC", Namespace = "Test", ItemName = "Item")]
    class SamplePrivateICollectionTExplicitWithCDC : ICollection<DC>
    {
        List<DC> internalList = new List<DC>();
        public SamplePrivateICollectionTExplicitWithCDC() { }
        public SamplePrivateICollectionTExplicitWithCDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }

        #region ICollection<DC> Members

        void ICollection<DC>.Add(DC item)
        {
            this.internalList.Add(item);
        }

        void ICollection<DC>.Clear()
        {
            this.internalList.Clear();
        }

        bool ICollection<DC>.Contains(DC item)
        {
            return this.internalList.Contains(item);
        }

        void ICollection<DC>.CopyTo(DC[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        int ICollection<DC>.Count
        {
            get { return this.internalList.Count; }
        }

        bool ICollection<DC>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<DC>.Remove(DC item)
        {
            return this.internalList.Remove(item); ;
        }

        #endregion

        #region IEnumerable<DC> Members

        IEnumerator<DC> IEnumerable<DC>.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    #endregion

    #region IEnumerable<T>

    [DataContract(IsReference = true)]
    class SamplePrivateIEnumerableTImplicitWithDC : IEnumerable<DC>
    {
        List<DC> internalList = new List<DC>();
        public SamplePrivateIEnumerableTImplicitWithDC() { }

        public SamplePrivateIEnumerableTImplicitWithDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }


        #region IEnumerable<DC> Members

        public IEnumerator<DC> GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [DataContract(IsReference = true)]
    public class SampleIEnumerableTImplicitWithDC : IEnumerable<DC>
    {
        List<DC> internalList = new List<DC>();
        public SampleIEnumerableTImplicitWithDC() { }

        public SampleIEnumerableTImplicitWithDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }

        #region IEnumerable<DC> Members

        public IEnumerator<DC> GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(Name = "SampleIEnumerableTImplicitWithoutDC")]
    public class SampleIEnumerableTImplicitWithoutDC : IEnumerable<DC>
    {
        List<DC> internalList = new List<DC>();
        public SampleIEnumerableTImplicitWithoutDC() { }
        public SampleIEnumerableTImplicitWithoutDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }

        public void Add(DC dc)
        {
            internalList.Add(dc);
        }

        #region IEnumerable<DC> Members

        public IEnumerator<DC> GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SampleIEnumerableTImplicitWithCDC", Namespace = "Test", ItemName = "Item")]
    public class SampleIEnumerableTImplicitWithCDC : IEnumerable<DC>
    {
        List<DC> internalList = new List<DC>();
        public SampleIEnumerableTImplicitWithCDC() { }
        public SampleIEnumerableTImplicitWithCDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }

        public void Add(DC item)
        {
            this.internalList.Add(item);
        }

        #region IEnumerable<DC> Members

        public IEnumerator<DC> GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [DataContract(IsReference = true)]
    public class SampleIEnumerableTExplicitWithDC : IEnumerable<DC>
    {
        List<DC> internalList = new List<DC>();
        public SampleIEnumerableTExplicitWithDC() { }
        public SampleIEnumerableTExplicitWithDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }

        #region IEnumerable<DC> Members

        IEnumerator<DC> IEnumerable<DC>.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(Name = "SampleIEnumerableTExplicitWithoutDC")]
    public class SampleIEnumerableTExplicitWithoutDC : IEnumerable<DC>
    {
        List<DC> internalList = new List<DC>();
        public SampleIEnumerableTExplicitWithoutDC() { }
        public SampleIEnumerableTExplicitWithoutDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }

        public void Add(DC dc)
        {
            internalList.Add(dc);
        }

        #region IEnumerable<DC> Members

        IEnumerator<DC> IEnumerable<DC>.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SampleIEnumerableTExplicitWithCDC", Namespace = "Test", ItemName = "Item")]
    public class SampleIEnumerableTExplicitWithCDC : IEnumerable<DC>
    {
        List<DC> internalList = new List<DC>();
        public SampleIEnumerableTExplicitWithCDC() { }
        public SampleIEnumerableTExplicitWithCDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }


        public void Add(DC dc)
        {
            internalList.Add(dc);
        }

        #region IEnumerable<DC> Members

        IEnumerator<DC> IEnumerable<DC>.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SampleIEnumerableTExplicitWithCDCContainsPrivateDC", Namespace = "Test", ItemName = "Item")]
    public class SampleIEnumerableTExplicitWithCDCContainsPrivateDC : IEnumerable<PrivateDC>
    {
        List<PrivateDC> internalList = new List<PrivateDC>();
        public SampleIEnumerableTExplicitWithCDCContainsPrivateDC() { }
        public SampleIEnumerableTExplicitWithCDCContainsPrivateDC(bool init)
        {
            PrivateDC dc1 = new PrivateDC();
            this.internalList.Add(dc1);
            this.internalList.Add(new PrivateDC());
            this.internalList.Add(dc1);
        }


        public void Add(object PrivateDC)
        {
            internalList.Add((PrivateDC)PrivateDC);
        }

        #region IEnumerable<PrivateDC> Members

        IEnumerator<PrivateDC> IEnumerable<PrivateDC>.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SamplePrivateIEnumerableTExplicitWithCDC", Namespace = "Test", ItemName = "Item")]
    class SamplePrivateIEnumerableTExplicitWithCDC : IEnumerable<DC>
    {
        List<DC> internalList = new List<DC>();
        public SamplePrivateIEnumerableTExplicitWithCDC() { }
        public SamplePrivateIEnumerableTExplicitWithCDC(bool init)
        {
            DC dc1 = new DC();
            this.internalList.Add(dc1);
            this.internalList.Add(new DC());
            this.internalList.Add(dc1);
        }

        public void Add(DC dc)
        {
            internalList.Add(dc);
        }

        #region IEnumerable<DC> Members

        IEnumerator<DC> IEnumerable<DC>.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    #endregion

    #region ICollection

    [DataContract(IsReference = true)]
    public class SampleICollectionImplicitWithDC : ICollection
    {
        List<object> internalList = new List<object>();

        public SampleICollectionImplicitWithDC() { }

        public SampleICollectionImplicitWithDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }
        public int Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }


        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            //this.internalList.CopyTo(array, index);
        }

        public int Count
        {
            get { return this.internalList.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion



        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [DataContract(IsReference = true)]
    class SamplePrivateICollectionImplicitWithDC : ICollection
    {
        List<object> internalList = new List<object>();

        public SamplePrivateICollectionImplicitWithDC() { }

        public SamplePrivateICollectionImplicitWithDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }
        public int Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }


        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            //this.internalList.CopyTo(array, index);
        }

        public int Count
        {
            get { return this.internalList.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion



        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(Name = "SampleICollectionImplicitWithoutDC")]
    public class SampleICollectionImplicitWithoutDC : ICollection
    {
        List<object> internalList = new List<object>();

        public SampleICollectionImplicitWithoutDC() { }

        public SampleICollectionImplicitWithoutDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }
        public int Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }


        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            //this.internalList.CopyTo(array, index);
        }

        public int Count
        {
            get { return this.internalList.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SampleListImplicitWithCDC2", Namespace = "Test", ItemName = "Item")]
    public class SampleICollectionImplicitWithCDC : ICollection
    {
        List<object> internalList = new List<object>();

        public SampleICollectionImplicitWithCDC() { }

        public SampleICollectionImplicitWithCDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }
        public int Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }


        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            //this.internalList.CopyTo(array, index);
        }

        public int Count
        {
            get { return this.internalList.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [DataContract(IsReference = true)]
    public class SampleICollectionExplicitWithDC : ICollection
    {
        List<object> internalList = new List<object>();
        public SampleICollectionExplicitWithDC() { }
        public SampleICollectionExplicitWithDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }
        public int Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }


        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            //this.internalList.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return this.internalList.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(Name = "SampleICollectionExplicitWithoutDC")]
    public class SampleICollectionExplicitWithoutDC : ICollection
    {
        List<object> internalList = new List<object>();
        public SampleICollectionExplicitWithoutDC() { }

        public SampleICollectionExplicitWithoutDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }

        public int Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }



        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            //this.internalList.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return this.internalList.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SampleListExplicitWithCDC1", Namespace = "Test", ItemName = "Item")]
    public class SampleICollectionExplicitWithCDC : ICollection
    {
        List<object> internalList = new List<object>();
        public SampleICollectionExplicitWithCDC() { }
        public SampleICollectionExplicitWithCDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }


        public int Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }


        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            //this.internalList.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return this.internalList.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SampleICollectionExplicitWithCDCContainsPrivateDC", Namespace = "Test", ItemName = "Item")]
    [KnownType(typeof(PrivateDC))]
    public class SampleICollectionExplicitWithCDCContainsPrivateDC : ICollection
    {
        List<object> internalList = new List<object>();
        public SampleICollectionExplicitWithCDCContainsPrivateDC()
        {
        }

        public SampleICollectionExplicitWithCDCContainsPrivateDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
            this.internalList.Add(new PrivateDC());
        }


        public int Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }


        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            //this.internalList.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return this.internalList.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SamplePrivateICollectionExplicitWithCDC", Namespace = "Test", ItemName = "Item")]
    class SamplePrivateICollectionExplicitWithCDC : ICollection
    {
        List<object> internalList = new List<object>();
        public SamplePrivateICollectionExplicitWithCDC() { }
        public SamplePrivateICollectionExplicitWithCDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }


        public int Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }


        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            //this.internalList.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return this.internalList.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }


    #endregion

    #region IEnumerable

    [DataContract(IsReference = true)]
    public class SampleIEnumerableImplicitWithDC : IEnumerable
    {
        List<object> internalList = new List<object>();

        public SampleIEnumerableImplicitWithDC() { }

        public SampleIEnumerableImplicitWithDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }
        public int Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [DataContract(IsReference = true)]
    class SamplePrivateIEnumerableImplicitWithDC : IEnumerable
    {
        List<object> internalList = new List<object>();

        public SamplePrivateIEnumerableImplicitWithDC() { }

        public SamplePrivateIEnumerableImplicitWithDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }
        public int Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(Name = "SampleIEnumerableImplicitWithoutDC")]
    public class SampleIEnumerableImplicitWithoutDC : IEnumerable
    {
        List<object> internalList = new List<object>();

        public SampleIEnumerableImplicitWithoutDC() { }

        public SampleIEnumerableImplicitWithoutDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }
        public int Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SampleListImplicitWithCDC3", Namespace = "Test", ItemName = "Item")]
    public class SampleIEnumerableImplicitWithCDC : IEnumerable
    {
        List<object> internalList = new List<object>();

        public SampleIEnumerableImplicitWithCDC() { }

        public SampleIEnumerableImplicitWithCDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }

        public int Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }



        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [DataContract(IsReference = true)]
    public class SampleIEnumerableExplicitWithDC : IEnumerable
    {
        List<object> internalList = new List<object>();
        public SampleIEnumerableExplicitWithDC() { }
        public SampleIEnumerableExplicitWithDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }

        public void Add(object value)
        {
            this.internalList.Add(value);
        }


        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(Name = "SampleIEnumerableExplicitWithoutDC")]
    public class SampleIEnumerableExplicitWithoutDC : IEnumerable
    {
        List<object> internalList = new List<object>();
        public SampleIEnumerableExplicitWithoutDC() { }

        public SampleIEnumerableExplicitWithoutDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }

        public int Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }




        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SampleIEnumerableExplicitWithCDC", Namespace = "Test", ItemName = "Item")]
    public class SampleIEnumerableExplicitWithCDC : IEnumerable
    {
        List<object> internalList = new List<object>();
        public SampleIEnumerableExplicitWithCDC() { }
        public SampleIEnumerableExplicitWithCDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }


        public int Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }




        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, Name = "SampleIEnumerableExplicitWithCDCContainsPrivateDC", Namespace = "Test", ItemName = "Item")]
    [KnownType(typeof(PrivateDC))]
    public class SampleIEnumerableExplicitWithCDCContainsPrivateDC : IEnumerable
    {
        List<object> internalList = new List<object>();
        public SampleIEnumerableExplicitWithCDCContainsPrivateDC() { }
        public SampleIEnumerableExplicitWithCDCContainsPrivateDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
            this.internalList.Add(new PrivateDC());
        }


        public int Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }




        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }


    [CollectionDataContract(IsReference = true, Name = "SamplePrivateIEnumerableExplicitWithCDC", Namespace = "Test", ItemName = "Item")]
    class SamplePrivateIEnumerableExplicitWithCDC : IEnumerable
    {
        List<object> internalList = new List<object>();
        public SamplePrivateIEnumerableExplicitWithCDC() { }
        public SamplePrivateIEnumerableExplicitWithCDC(bool init)
        {
            this.internalList.Add(DateTime.Now);
            this.internalList.Add(TimeSpan.MaxValue);
            this.internalList.Add(String.Empty);
            this.internalList.Add(Double.MaxValue);
            this.internalList.Add(Double.NegativeInfinity);
            this.internalList.Add(Guid.NewGuid());
        }


        public int Add(object value)
        {
            this.internalList.Add(value);
            return this.internalList.Count;
        }




        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        #endregion
    }


    #endregion

    #region IDictionary

    [CollectionDataContract(IsReference = true, ItemName = "DictItem", KeyName = "DictKey", Name = "MyIDictionary4", Namespace = "MyDictNS1", ValueName = "DictValue")]
    [KnownType(typeof(PublicDC))]
    public class MyIDictionaryContainsPublicDC : IDictionary
    {
        Dictionary<object, object> data = new Dictionary<object, object>();

        public MyIDictionaryContainsPublicDC()
        {
        }
        public MyIDictionaryContainsPublicDC(bool init)
        {
            data.Add(new PublicDC(), new PublicDC());
            data.Add(new PublicDC(), new PublicDC());
        }

        #region IDictionary Members

        public void Add(object key, object value)
        {
            data.Add(key, value);
        }

        public void Clear()
        {
            data.Clear();
        }

        public bool Contains(object key)
        {
            return data.ContainsKey(key);
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return data.GetEnumerator();
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public ICollection Keys
        {
            get { return data.Keys; }
        }

        public void Remove(object key)
        {
            data.Remove(key);
        }

        public ICollection Values
        {
            get { return data.Keys; }
        }

        public object this[object key]
        {
            get
            {
                return data[key];
            }
            set
            {
                data[key] = value; ;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            throw new Exception("TEST EXCEPTION!!!!: CopyTo method or operation is not implemented.");
        }

        public int Count
        {
            get { return data.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, ItemName = "DictItem", KeyName = "DictKey", Name = "MyIDictionary2", Namespace = "MyDictNS1", ValueName = "DictValue")]
    [KnownType(typeof(PublicDC))]
    public class MyIDictionaryContainsPublicDCExplicit : IDictionary
    {
        Dictionary<object, object> data = new Dictionary<object, object>();

        public MyIDictionaryContainsPublicDCExplicit()
        {
        }

        public MyIDictionaryContainsPublicDCExplicit(bool init)
        {
            data.Add(new PublicDC(), new PublicDC());
            data.Add(new PublicDC(), new PublicDC());
        }

        #region IDictionary Members

        void IDictionary.Add(object key, object value)
        {
            data.Add(key, value);
        }

        void IDictionary.Clear()
        {
            data.Clear();
        }

        bool IDictionary.Contains(object key)
        {
            return data.ContainsKey(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        bool IDictionary.IsFixedSize
        {
            get { return false; }
        }

        bool IDictionary.IsReadOnly
        {
            get { return true; }
        }

        ICollection IDictionary.Keys
        {
            get { return data.Keys; }
        }

        void IDictionary.Remove(object key)
        {
            data.Remove(key);
        }

        ICollection IDictionary.Values
        {
            get { return data.Keys; }
        }

        object IDictionary.this[object key]
        {
            get
            {
                return data[key];
            }
            set
            {
                data[key] = value; ;
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            throw new Exception("TEST EXCEPTION!!!!: CopyTo method or operation is not implemented.");
        }

        int ICollection.Count
        {
            get { return data.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, ItemName = "DictItem", KeyName = "DictKey", Name = "MyIDictionary3", Namespace = "MyDictNS2", ValueName = "DictValue")]
    [KnownType(typeof(PrivateDC))]
    [KnownType(typeof(PublicDCClassPrivateDM))]
    public class MyIDictionaryContainsPrivateDC : IDictionary
    {
        Dictionary<object, object> data = new Dictionary<object, object>();

        public MyIDictionaryContainsPrivateDC()
        { }
        public MyIDictionaryContainsPrivateDC(bool init)
        {
            data.Add(new PrivateDC(), new PrivateDC());
            data.Add(new PrivateDC(), new PrivateDC());
            data.Add(new PublicDCClassPrivateDM(), new PublicDCClassPrivateDM());
        }

        #region IDictionary Members

        public void Add(object key, object value)
        {
            data.Add(key, value);
        }

        public void Clear()
        {
            data.Clear();
        }

        public bool Contains(object key)
        {
            return data.ContainsKey(key);
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return data.GetEnumerator();
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public ICollection Keys
        {
            get { return data.Keys; }
        }

        public void Remove(object key)
        {
            data.Remove(key);
        }

        public ICollection Values
        {
            get { return data.Keys; }
        }

        public object this[object key]
        {
            get
            {
                return data[key];
            }
            set
            {
                data[key] = value; ;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            throw new Exception("TEST EXCEPTION!!!!: CopyTo method or operation is not implemented.");
        }

        public int Count
        {
            get { return data.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(Name = "MyGenericIDictionaryKVContainsPublicDC")]
    public class MyGenericIDictionaryKVContainsPublicDC : IDictionary<PublicDC, PublicDC>
    {
        Dictionary<PublicDC, PublicDC> data = new Dictionary<PublicDC, PublicDC>();

        public MyGenericIDictionaryKVContainsPublicDC()
        { }
        public MyGenericIDictionaryKVContainsPublicDC(bool init)
        {
            data.Add(new PublicDC(), new PublicDC());
            data.Add(new PublicDC(), new PublicDC());
        }

        #region IDictionary Members

        public void Add(PublicDC key, PublicDC value)
        {
            data.Add(key, value);
        }

        public void Clear()
        {
            data.Clear();
        }

        public bool Contains(PublicDC key)
        {
            return data.ContainsKey(key);
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return data.GetEnumerator();
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public ICollection Keys
        {
            get { return data.Keys; }
        }

        public void Remove(PublicDC key)
        {
            data.Remove(key);
        }

        public ICollection Values
        {
            get { return data.Keys; }
        }

        public PublicDC this[PublicDC key]
        {
            get
            {
                return data[key];
            }
            set
            {
                data[key] = value; ;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            throw new Exception("TEST EXCEPTION!!!!: CopyTo method or operation is not implemented.");
        }

        public int Count
        {
            get { return data.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        #endregion

        #region IDictionary<PublicDC,PublicDC> Members


        public bool ContainsKey(PublicDC key)
        {
            return data.ContainsKey(key);
        }

        ICollection<PublicDC> IDictionary<PublicDC, PublicDC>.Keys
        {
            get { return data.Keys; }
        }

        bool IDictionary<PublicDC, PublicDC>.Remove(PublicDC key)
        {
            return data.Remove(key);
        }

        public bool TryGetValue(PublicDC key, out PublicDC value)
        {
            return data.TryGetValue(key, out value);
        }

        ICollection<PublicDC> IDictionary<PublicDC, PublicDC>.Values
        {
            get { return data.Values; }
        }

        PublicDC IDictionary<PublicDC, PublicDC>.this[PublicDC key]
        {
            get
            {
                return data[key];
            }
            set
            {
                data[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<PublicDC,PublicDC>> Members

        public void Add(KeyValuePair<PublicDC, PublicDC> item)
        {
            data.Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<PublicDC, PublicDC> item)
        {
            return data.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<PublicDC, PublicDC>[] array, int arrayIndex)
        {
            throw new Exception("TEST EXCEPTION!!: CopyTO: method or operation is not implemented.");
        }

        public bool Remove(KeyValuePair<PublicDC, PublicDC> item)
        {
            return data.Remove(item.Key);
        }

        #endregion

        #region IEnumerable<KeyValuePair<PublicDC,PublicDC>> Members

        IEnumerator<KeyValuePair<PublicDC, PublicDC>> IEnumerable<KeyValuePair<PublicDC, PublicDC>>.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(Name = "MyGenericIDictionaryKVContainsPublicDCExplicit")]
    public class MyGenericIDictionaryKVContainsPublicDCExplicit : IDictionary<PublicDC, PublicDC>
    {
        Dictionary<PublicDC, PublicDC> data = new Dictionary<PublicDC, PublicDC>();

        public MyGenericIDictionaryKVContainsPublicDCExplicit()
        {
        }
        public MyGenericIDictionaryKVContainsPublicDCExplicit(bool init)
        {
            data.Add(new PublicDC(), new PublicDC());
            data.Add(new PublicDC(), new PublicDC());
        }
        #region IDictionary Members

        void IDictionary<PublicDC, PublicDC>.Add(PublicDC key, PublicDC value)
        {
            data.Add(key, value);
        }

        void ICollection<KeyValuePair<PublicDC, PublicDC>>.Clear()
        {
            data.Clear();
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public ICollection Keys
        {
            get { return data.Keys; }
        }

        public void Remove(PublicDC key)
        {
            data.Remove(key);
        }

        public ICollection Values
        {
            get { return data.Keys; }
        }

        public PublicDC this[PublicDC key]
        {
            get
            {
                return data[key];
            }
            set
            {
                data[key] = value; ;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            throw new Exception("TEST EXCEPTION!!!!: CopyTo method or operation is not implemented.");
        }

        int ICollection<KeyValuePair<PublicDC, PublicDC>>.Count
        {
            get { return data.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        #endregion

        #region IDictionary<PublicDC,PublicDC> Members


        bool IDictionary<PublicDC, PublicDC>.ContainsKey(PublicDC key)
        {
            return data.ContainsKey(key);
        }

        ICollection<PublicDC> IDictionary<PublicDC, PublicDC>.Keys
        {
            get { return data.Keys; }
        }

        bool IDictionary<PublicDC, PublicDC>.Remove(PublicDC key)
        {
            return data.Remove(key);
        }

        bool IDictionary<PublicDC, PublicDC>.TryGetValue(PublicDC key, out PublicDC value)
        {
            return data.TryGetValue(key, out value);
        }

        ICollection<PublicDC> IDictionary<PublicDC, PublicDC>.Values
        {
            get { return data.Values; }
        }

        PublicDC IDictionary<PublicDC, PublicDC>.this[PublicDC key]
        {
            get
            {
                return data[key];
            }
            set
            {
                data[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<PublicDC,PublicDC>> Members

        void ICollection<KeyValuePair<PublicDC, PublicDC>>.Add(KeyValuePair<PublicDC, PublicDC> item)
        {
            data.Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<PublicDC, PublicDC>>.Contains(KeyValuePair<PublicDC, PublicDC> item)
        {
            return data.ContainsKey(item.Key);
        }

        void ICollection<KeyValuePair<PublicDC, PublicDC>>.CopyTo(KeyValuePair<PublicDC, PublicDC>[] array, int arrayIndex)
        {
            throw new Exception("TEST EXCEPTION!!: CopyTO: method or operation is not implemented.");
        }

        bool ICollection<KeyValuePair<PublicDC, PublicDC>>.Remove(KeyValuePair<PublicDC, PublicDC> item)
        {
            return data.Remove(item.Key);
        }

        #endregion

        #region IEnumerable<KeyValuePair<PublicDC,PublicDC>> Members

        IEnumerator<KeyValuePair<PublicDC, PublicDC>> IEnumerable<KeyValuePair<PublicDC, PublicDC>>.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, ItemName = "DictItem", KeyName = "DictKey", Name = "MyGenericIDictionaryKVContainsPrivateDC", Namespace = "MyDictNS", ValueName = "DictValue")]
    [KnownType(typeof(PrivateDC))]
    public class MyGenericIDictionaryKVContainsPrivateDC : IDictionary<Object, Object>
    {
        Dictionary<Object, Object> data = new Dictionary<Object, Object>();

        public MyGenericIDictionaryKVContainsPrivateDC()
        { }
        public MyGenericIDictionaryKVContainsPrivateDC(bool init)
        {
            data.Add(new PrivateDC(), new PrivateDC());
            data.Add(new PrivateDC(), new PrivateDC());
        }
        #region IDictionary Members

        public void Add(Object key, Object value)
        {
            data.Add(key, value);
        }

        public void Clear()
        {
            data.Clear();
        }

        public bool Contains(Object key)
        {
            return data.ContainsKey(key);
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return data.GetEnumerator();
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public ICollection Keys
        {
            get { return data.Keys; }
        }

        public void Remove(Object key)
        {
            data.Remove(key);
        }

        public ICollection Values
        {
            get { return data.Keys; }
        }

        public Object this[Object key]
        {
            get
            {
                return data[key];
            }
            set
            {
                data[key] = value; ;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            throw new Exception("TEST EXCEPTION!!!!: CopyTo method or operation is not implemented.");
        }

        public int Count
        {
            get { return data.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        #endregion

        #region IDictionary<PublicDC,PublicDC> Members


        public bool ContainsKey(Object key)
        {
            return data.ContainsKey(key);
        }

        ICollection<Object> IDictionary<Object, Object>.Keys
        {
            get { return data.Keys; }
        }

        bool IDictionary<Object, Object>.Remove(Object key)
        {
            return data.Remove(key);
        }

        public bool TryGetValue(Object key, out Object value)
        {
            return data.TryGetValue(key, out value);
        }

        ICollection<Object> IDictionary<Object, Object>.Values
        {
            get { return data.Values; }
        }

        Object IDictionary<Object, Object>.this[Object key]
        {
            get
            {
                return data[key];
            }
            set
            {
                data[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<PublicDC,PublicDC>> Members

        public void Add(KeyValuePair<Object, Object> item)
        {
            data.Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<Object, Object> item)
        {
            return data.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<Object, Object>[] array, int arrayIndex)
        {
            throw new Exception("TEST EXCEPTION!!: CopyTO: method or operation is not implemented.");
        }

        public bool Remove(KeyValuePair<Object, Object> item)
        {
            return data.Remove(item.Key);
        }

        #endregion

        #region IEnumerable<KeyValuePair<PublicDC,PublicDC>> Members

        IEnumerator<KeyValuePair<Object, Object>> IEnumerable<KeyValuePair<Object, Object>>.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        #endregion
    }


    [CollectionDataContract(IsReference = true, ItemName = "DictItem", KeyName = "DictKey", Name = "MyIDictionaryContainsPublicDC", Namespace = "MyDictNS", ValueName = "DictValue")]
    [KnownType(typeof(PublicDC))]
    class MyPrivateIDictionaryContainsPublicDC : IDictionary
    {
        Dictionary<object, object> data = new Dictionary<object, object>();

        public MyPrivateIDictionaryContainsPublicDC()
        { }
        public MyPrivateIDictionaryContainsPublicDC(bool init)
        {
            data.Add(new PublicDC(), new PublicDC());
            data.Add(new PublicDC(), new PublicDC());
        }

        #region IDictionary Members

        public void Add(object key, object value)
        {
            data.Add(key, value);
        }

        public void Clear()
        {
            data.Clear();
        }

        public bool Contains(object key)
        {
            return data.ContainsKey(key);
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return data.GetEnumerator();
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public ICollection Keys
        {
            get { return data.Keys; }
        }

        public void Remove(object key)
        {
            data.Remove(key);
        }

        public ICollection Values
        {
            get { return data.Keys; }
        }

        public object this[object key]
        {
            get
            {
                return data[key];
            }
            set
            {
                data[key] = value; ;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            throw new Exception("TEST EXCEPTION!!!!: CopyTo method or operation is not implemented.");
        }

        public int Count
        {
            get { return data.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        #endregion
    }

    #endregion

    #region IDictionary<K,V>

    [CollectionDataContract(Name = "MyPrivateGenericIDictionaryKVContainsPublicDC")]
    class MyPrivateGenericIDictionaryKVContainsPublicDC : IDictionary<PublicDC, PublicDC>
    {
        Dictionary<PublicDC, PublicDC> data = new Dictionary<PublicDC, PublicDC>();

        public MyPrivateGenericIDictionaryKVContainsPublicDC()
        {
        }
        public MyPrivateGenericIDictionaryKVContainsPublicDC(bool init)
        {
            PublicDC dc = new PublicDC();
            dc.Data = "hi";
            data.Add(new PublicDC(), dc);
            data.Add(new PublicDC(), new PublicDC());
            data.Add(dc, new PublicDC());
        }
        #region IDictionary Members

        public void Add(PublicDC key, PublicDC value)
        {
            data.Add(key, value);
        }

        public void Clear()
        {
            data.Clear();
        }

        public bool Contains(PublicDC key)
        {
            return data.ContainsKey(key);
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return data.GetEnumerator();
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public ICollection Keys
        {
            get { return data.Keys; }
        }

        public void Remove(PublicDC key)
        {
            data.Remove(key);
        }

        public ICollection Values
        {
            get { return data.Keys; }
        }

        public PublicDC this[PublicDC key]
        {
            get
            {
                return data[key];
            }
            set
            {
                data[key] = value; ;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            throw new Exception("TEST EXCEPTION!!!!: CopyTo method or operation is not implemented.");
        }

        public int Count
        {
            get { return data.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        #endregion

        #region IDictionary<PublicDC,PublicDC> Members


        public bool ContainsKey(PublicDC key)
        {
            return data.ContainsKey(key);
        }

        ICollection<PublicDC> IDictionary<PublicDC, PublicDC>.Keys
        {
            get { return data.Keys; }
        }

        bool IDictionary<PublicDC, PublicDC>.Remove(PublicDC key)
        {
            return data.Remove(key);
        }

        public bool TryGetValue(PublicDC key, out PublicDC value)
        {
            return data.TryGetValue(key, out value);
        }

        ICollection<PublicDC> IDictionary<PublicDC, PublicDC>.Values
        {
            get { return data.Values; }
        }

        PublicDC IDictionary<PublicDC, PublicDC>.this[PublicDC key]
        {
            get
            {
                return data[key];
            }
            set
            {
                data[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<PublicDC,PublicDC>> Members

        public void Add(KeyValuePair<PublicDC, PublicDC> item)
        {
            data.Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<PublicDC, PublicDC> item)
        {
            return data.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<PublicDC, PublicDC>[] array, int arrayIndex)
        {
            throw new Exception("TEST EXCEPTION!!: CopyTO: method or operation is not implemented.");
        }

        public bool Remove(KeyValuePair<PublicDC, PublicDC> item)
        {
            return data.Remove(item.Key);
        }

        #endregion

        #region IEnumerable<KeyValuePair<PublicDC,PublicDC>> Members

        IEnumerator<KeyValuePair<PublicDC, PublicDC>> IEnumerable<KeyValuePair<PublicDC, PublicDC>>.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        #endregion
    }

    [CollectionDataContract(IsReference = true, ItemName = "DictItem", KeyName = "DictKey", Name = "MyPrivateGenericIDictionaryKVContainsPrivateDC", Namespace = "MyDictNS", ValueName = "DictValue")]
    [KnownType(typeof(PrivateDC))]
    class MyPrivateGenericIDictionaryKVContainsPrivateDC : IDictionary<Object, Object>
    {
        Dictionary<Object, Object> data = new Dictionary<Object, Object>();

        public MyPrivateGenericIDictionaryKVContainsPrivateDC()
        {

        }

        public MyPrivateGenericIDictionaryKVContainsPrivateDC(bool init)
        {
            PrivateDC dc = new PrivateDC();

            data.Add(new PrivateDC(), dc);
            data.Add(dc, new PrivateDC());
        }
        #region IDictionary Members

        public void Add(Object key, Object value)
        {
            data.Add(key, value);
        }

        public void Clear()
        {
            data.Clear();
        }

        public bool Contains(Object key)
        {
            return data.ContainsKey(key);
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return data.GetEnumerator();
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public ICollection Keys
        {
            get { return data.Keys; }
        }

        public void Remove(Object key)
        {
            data.Remove(key);
        }

        public ICollection Values
        {
            get { return data.Keys; }
        }

        public Object this[Object key]
        {
            get
            {
                return data[key];
            }
            set
            {
                data[key] = value; ;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            throw new Exception("TEST EXCEPTION!!!!: CopyTo method or operation is not implemented.");
        }

        public int Count
        {
            get { return data.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        #endregion

        #region IDictionary<PublicDC,PublicDC> Members


        public bool ContainsKey(Object key)
        {
            return data.ContainsKey(key);
        }

        ICollection<Object> IDictionary<Object, Object>.Keys
        {
            get { return data.Keys; }
        }

        bool IDictionary<Object, Object>.Remove(Object key)
        {
            return data.Remove(key);
        }

        public bool TryGetValue(Object key, out Object value)
        {
            return data.TryGetValue(key, out value);
        }

        ICollection<Object> IDictionary<Object, Object>.Values
        {
            get { return data.Values; }
        }

        Object IDictionary<Object, Object>.this[Object key]
        {
            get
            {
                return data[key];
            }
            set
            {
                data[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<PublicDC,PublicDC>> Members

        public void Add(KeyValuePair<Object, Object> item)
        {
            data.Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<Object, Object> item)
        {
            return data.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<Object, Object>[] array, int arrayIndex)
        {
            throw new Exception("TEST EXCEPTION!!: CopyTO: method or operation is not implemented.");
        }

        public bool Remove(KeyValuePair<Object, Object> item)
        {
            return data.Remove(item.Key);
        }

        #endregion

        #region IEnumerable<KeyValuePair<PublicDC,PublicDC>> Members

        IEnumerator<KeyValuePair<Object, Object>> IEnumerable<KeyValuePair<Object, Object>>.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        #endregion
    }

    #region Dictionary containers

    [DataContract(IsReference = true)]
    public class DCDictionaryPrivateKTContainer
    {
        [DataMember]
        Dictionary<PrivateDC, PrivateDC> DictData = new Dictionary<PrivateDC, PrivateDC>();
        public DCDictionaryPrivateKTContainer() { DictData.Add(new PrivateDC(), new PrivateDC()); }
    }

    [DataContract(IsReference = true)]
    public class DCDictionaryPublicKTContainer
    {
        [DataMember]
        public Dictionary<PublicDC, PublicDC> DictData = new Dictionary<PublicDC, PublicDC>();
        public DCDictionaryPublicKTContainer() { DictData.Add(new PublicDC(), new PublicDC()); }
    }

    [DataContract(IsReference = true)]
    [KnownType(typeof(PrivateDC))]
    [KnownType(typeof(PublicDC))]
    [KnownType(typeof(PublicDCDerivedPublic))]
    [KnownType(typeof(PublicDCDerivedPrivate))]
    public class DCDictionaryMixedKTContainer1
    {
        [DataMember]
        public Dictionary<object, object> DictData = new Dictionary<object, object>();

        public DCDictionaryMixedKTContainer1()
        {
            DictData.Add(new PublicDC(), new PublicDC());
            DictData.Add(new PrivateDC(), new PublicDC());
            DictData.Add(new PublicDC(), new PrivateDC());
            DictData.Add(new PrivateDC(), new PrivateDC());

            DictData.Add(new PublicDCDerivedPublic(), new PublicDCDerivedPublic());
            DictData.Add(new PublicDCDerivedPrivate(), new PublicDCDerivedPublic());
            DictData.Add(new PublicDCDerivedPublic(), new PublicDCDerivedPrivate());
            DictData.Add(new PublicDCDerivedPrivate(), new PublicDCDerivedPrivate());

            DictData.Add(new PublicDC(), new PublicDCDerivedPublic());
            DictData.Add(new PublicDC(), new PublicDCDerivedPrivate());
            DictData.Add(new PrivateDC(), new PublicDCDerivedPublic());
            DictData.Add(new PrivateDC(), new PublicDCDerivedPrivate());

            DictData.Add(new PublicDCDerivedPublic(), new PublicDC());
            DictData.Add(new PublicDCDerivedPrivate(), new PublicDC());
            DictData.Add(new PublicDCDerivedPublic(), new PrivateDC());
            DictData.Add(new PublicDCDerivedPrivate(), new PrivateDC());
        }

        public static DCDictionaryMixedKTContainer1 CreateInstance()
        {
            DCDictionaryMixedKTContainer1 dict = new DCDictionaryMixedKTContainer1();
            return dict;
        }
    }

    [DataContract(IsReference = true)]
    public class DCDictionaryMixedKTContainer2
    {
        [DataMember]
        Dictionary<PublicDC, PrivateDC> DictData = new Dictionary<PublicDC, PrivateDC>();
        public DCDictionaryMixedKTContainer2() { DictData.Add(new PublicDC(), new PrivateDC()); }
    }

    [DataContract(IsReference = true)]
    public class DCDictionaryMixedKTContainer3
    {
        [DataMember]
        Dictionary<PrivateDC, PublicDC> DictData = new Dictionary<PrivateDC, PublicDC>();
        public DCDictionaryMixedKTContainer3() { DictData.Add(new PrivateDC(), new PublicDC()); }
    }

    [DataContract(IsReference = true)]
    public class DCDictionaryMixedKTContainer4
    {
        [DataMember]
        public Dictionary<PublicDCDerivedPublic, PublicDC> DictData = new Dictionary<PublicDCDerivedPublic, PublicDC>();
        public DCDictionaryMixedKTContainer4() { DictData.Add(new PublicDCDerivedPublic(), new PublicDC()); }
    }

    [DataContract(IsReference = true)]
    public class DCDictionaryMixedKTContainer5
    {
        [DataMember]
        Dictionary<PublicDCDerivedPrivate, PublicDC> DictData = new Dictionary<PublicDCDerivedPrivate, PublicDC>();
        public DCDictionaryMixedKTContainer5() { DictData.Add(new PublicDCDerivedPrivate(), new PublicDC()); }
    }

    [DataContract(IsReference = true)]
    public class DCDictionaryMixedKTContainer6
    {
        [DataMember]
        Dictionary<PublicDCDerivedPublic, PrivateDC> DictData = new Dictionary<PublicDCDerivedPublic, PrivateDC>();
        public DCDictionaryMixedKTContainer6() { DictData.Add(new PublicDCDerivedPublic(), new PrivateDC()); }
    }

    [DataContract(IsReference = true)]
    public class DCDictionaryMixedKTContainer7
    {
        [DataMember]
        Dictionary<PublicDCDerivedPrivate, PrivateDC> DictData = new Dictionary<PublicDCDerivedPrivate, PrivateDC>();
        public DCDictionaryMixedKTContainer7() { DictData.Add(new PublicDCDerivedPrivate(), new PrivateDC()); }
    }


    [DataContract(IsReference = true)]
    public class DCDictionaryMixedKTContainer8
    {
        [DataMember]
        public Dictionary<PublicDCDerivedPublic, PublicDCDerivedPublic> DictData = new Dictionary<PublicDCDerivedPublic, PublicDCDerivedPublic>();
        public DCDictionaryMixedKTContainer8() { DictData.Add(new PublicDCDerivedPublic(), new PublicDCDerivedPublic()); }
    }

    [DataContract(IsReference = true)]
    public class DCDictionaryMixedKTContainer9
    {
        [DataMember]
        Dictionary<PublicDCDerivedPrivate, PublicDCDerivedPrivate> DictData = new Dictionary<PublicDCDerivedPrivate, PublicDCDerivedPrivate>();
        public DCDictionaryMixedKTContainer9() { DictData.Add(new PublicDCDerivedPrivate(), new PublicDCDerivedPrivate()); }
    }

    [DataContract(IsReference = true)]
    public class DCDictionaryMixedKTContainer10
    {
        [DataMember]
        Dictionary<PublicDCDerivedPublic, PublicDCDerivedPrivate> DictData = new Dictionary<PublicDCDerivedPublic, PublicDCDerivedPrivate>();
        public DCDictionaryMixedKTContainer10() { DictData.Add(new PublicDCDerivedPublic(), new PublicDCDerivedPrivate()); }
    }

    [DataContract(IsReference = true)]
    public class DCDictionaryMixedKTContainer11
    {
        [DataMember]
        Dictionary<PublicDCDerivedPrivate, PublicDCDerivedPublic> DictData = new Dictionary<PublicDCDerivedPrivate, PublicDCDerivedPublic>();
        public DCDictionaryMixedKTContainer11() { DictData.Add(new PublicDCDerivedPrivate(), new PublicDCDerivedPublic()); }
    }

    [DataContract(IsReference = true)]
    public class DCDictionaryMixedKTContainer12
    {
        [DataMember]
        Dictionary<PublicDCClassPrivateDM, PublicDCClassPublicDM_DerivedDCClassPublicContainsPrivateDM> DictData = new Dictionary<PublicDCClassPrivateDM, PublicDCClassPublicDM_DerivedDCClassPublicContainsPrivateDM>();
        public DCDictionaryMixedKTContainer12() { DictData.Add(new PublicDCClassPrivateDM(), new PublicDCClassPublicDM_DerivedDCClassPublicContainsPrivateDM()); }
    }

    [DataContract(IsReference = true)]
    public class DCDictionaryMixedKTContainer13
    {
        [DataMember]
        Dictionary<KT1Base, KT2Base> DictData = new Dictionary<KT1Base, KT2Base>();
        public DCDictionaryMixedKTContainer13() { DictData.Add(new KT1Base(), new KT2Base()); }
    }

    [DataContract(IsReference = true)]
    public class DCDictionaryMixedKTContainer14
    {
        //[DataMember]
        Dictionary<PrivateIXmlSerializables, PrivateDefaultCtorIXmlSerializables> DictData = new Dictionary<PrivateIXmlSerializables, PrivateDefaultCtorIXmlSerializables>();
        public DCDictionaryMixedKTContainer14() { DictData.Add(new PrivateIXmlSerializables(), new PrivateDefaultCtorIXmlSerializables(true)); }
    }
    #endregion

    [DataContract(IsReference = true)]
    class PrivateDC
    {
        [DataMember]
        public string Data = Guid.NewGuid().ToString();

        public override bool Equals(object obj)
        {
            PrivateDC other = obj as PrivateDC;
            if (other == null) return false;
            if (String.IsNullOrEmpty(other.Data) && String.IsNullOrEmpty(Data)) { return true; }
            return other.Data.Equals(Data);
        }
        public override int GetHashCode()
        {
            return Data.GetHashCode();
        }
    }

    [DataContract(IsReference = true)]
    public class PublicDC
    {
        [DataMember]
        [IgnoreMember]
        public string Data = Guid.NewGuid().ToString();

        public override bool Equals(object obj)
        {
            PublicDC other = obj as PublicDC;
            if (other == null) return false;
            if (String.IsNullOrEmpty(other.Data) && String.IsNullOrEmpty(Data)) { return true; }
            return other.Data.Equals(Data);
        }
        public override int GetHashCode()
        {
            return Data.GetHashCode();
        }
    }

    [DataContract(IsReference = true)]
    class PublicDCDerivedPrivate : PublicDC
    {

    }

    [DataContract(IsReference = true)]
    public class PublicDCDerivedPublic : PublicDC
    {

    }

    #endregion

    [DataContract(IsReference = true)]
    public class DC
    {
        [DataMember]
        public string Data = DateTime.Now.ToLongDateString();

        [DataMember]
        public DC Next;
    }

    [DataContract(IsReference = true)]
    public class DCWithReadOnlyField
    {
        [DataMember]
        public readonly string Data;
    }

    #region IXmlSerializables

    public class IReadWriteXmlWriteBinHex_EqualityDefined : IXmlSerializable
    {
        byte[] bits = System.Text.Encoding.UTF8.GetBytes("hello world");

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public virtual void ReadXml(System.Xml.XmlReader reader)
        {
            byte[] readBits = new byte[bits.Length];
            reader.Read();
            //reader.ReadToDescendant("WriteBinHex");
            int readLen = reader.ReadContentAsBinHex(readBits, 0, readBits.Length);

            if (bits.Length != readLen)
            {
                throw new Exception("Test Code Exception: read content length didnt match expected in IReadWriteXmlWriteBinHex.ReadXml. Read: " + readLen + " Original: " + bits.Length);
            }

            for (int i = 0; i < readLen; i++)
            {
                if (readBits[i] != bits[i])
                {
                    throw new Exception("Test Code Exception: read xml content is not correct in IReadWriteXmlWriteBinHex.ReadXml. Read: " + readBits[i] + " Original: " + bits[i]);

                }
            }

        }

        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            byte[] bits = System.Text.Encoding.UTF8.GetBytes("hello world");
            //writer.WriteStartElement("WriteBinHex");
            writer.WriteBinHex(bits, 0, bits.Length); //writes hello world again
            //writer.WriteBase64(bits, 0, bits.Length);
            //writer.WriteEndElement(); // WriteBinHex
        }

        public override bool Equals(object obj)
        {
            IReadWriteXmlWriteBinHex_EqualityDefined other = obj as IReadWriteXmlWriteBinHex_EqualityDefined;
            if (other == null) { return false; }
            for (int i = 0; i < bits.Length; i++)
            {
                if (other.bits[i] != bits[i])
                {
                    return false;

                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return bits.GetHashCode();
        }
    }

    class PrivateIXmlSerializables : IXmlSerializable
    {
        byte[] bits = System.Text.Encoding.UTF8.GetBytes("hello world");

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public virtual void ReadXml(System.Xml.XmlReader reader)
        {
            byte[] readBits = new byte[bits.Length];
            reader.Read();
            int readLen = reader.ReadContentAsBinHex(readBits, 0, readBits.Length);

            if (bits.Length != readLen)
            {
                throw new Exception("Test Code Exception: read content length didnt match expected in IReadWriteXmlWriteBinHex.ReadXml. Read: " + readLen + " Original: " + bits.Length);
            }

            for (int i = 0; i < readLen; i++)
            {
                if (readBits[i] != bits[i])
                {
                    throw new Exception("Test Code Exception: read xml content is not correct in IReadWriteXmlWriteBinHex.ReadXml. Read: " + readBits[i] + " Original: " + bits[i]);

                }
            }

        }

        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            byte[] bits = System.Text.Encoding.UTF8.GetBytes("hello world");
            //writer.WriteStartElement("WriteBinHex");
            writer.WriteBinHex(bits, 0, bits.Length); //writes hello world again
            //writer.WriteBase64(bits, 0, bits.Length);
            //writer.WriteEndElement(); // WriteBinHex
        }


    }

    public class PrivateDefaultCtorIXmlSerializables : IXmlSerializable
    {
        private PrivateDefaultCtorIXmlSerializables() { }
        public PrivateDefaultCtorIXmlSerializables(bool init) { }

        byte[] bits = System.Text.Encoding.UTF8.GetBytes("hello world");

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public virtual void ReadXml(System.Xml.XmlReader reader)
        {
            byte[] readBits = new byte[bits.Length];
            reader.Read();
            int readLen = reader.ReadContentAsBinHex(readBits, 0, readBits.Length);

            if (bits.Length != readLen)
            {
                throw new Exception("Test Code Exception: read content length didnt match expected in IReadWriteXmlWriteBinHex.ReadXml. Read: " + readLen + " Original: " + bits.Length);
            }

            for (int i = 0; i < readLen; i++)
            {
                if (readBits[i] != bits[i])
                {
                    throw new Exception("Test Code Exception: read xml content is not correct in IReadWriteXmlWriteBinHex.ReadXml. Read: " + readBits[i] + " Original: " + bits[i]);

                }
            }

        }

        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            byte[] bits = System.Text.Encoding.UTF8.GetBytes("hello world");
            //writer.WriteStartElement("WriteBinHex");
            writer.WriteBinHex(bits, 0, bits.Length); //writes hello world again
            //writer.WriteBase64(bits, 0, bits.Length);
            //writer.WriteEndElement(); // WriteBinHex
        }
    }

    [XmlSchemaProvider("MySchema")]
    public class PublicIXmlSerializablesWithPublicSchemaProvider : IXmlSerializable
    {
        byte[] bits = System.Text.Encoding.UTF8.GetBytes("hello world");

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        // This is the method named by the XmlSchemaProviderAttribute applied to the type.
        public static XmlQualifiedName MySchema(XmlSchemaSet xs)
        {
            return new XmlQualifiedName("MyData", "MyNameSpace");
        }

        public virtual void ReadXml(System.Xml.XmlReader reader)
        {
            byte[] readBits = new byte[bits.Length];
            reader.Read();
            int readLen = reader.ReadContentAsBinHex(readBits, 0, readBits.Length);

            if (bits.Length != readLen)
            {
                throw new Exception("Test Code Exception: read content length didnt match expected in IReadWriteXmlWriteBinHex.ReadXml. Read: " + readLen + " Original: " + bits.Length);
            }

            for (int i = 0; i < readLen; i++)
            {
                if (readBits[i] != bits[i])
                {
                    throw new Exception("Test Code Exception: read xml content is not correct in IReadWriteXmlWriteBinHex.ReadXml. Read: " + readBits[i] + " Original: " + bits[i]);

                }
            }

        }

        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            byte[] bits = System.Text.Encoding.UTF8.GetBytes("hello world");
            //writer.WriteStartElement("WriteBinHex");
            writer.WriteBinHex(bits, 0, bits.Length); //writes hello world again
            //writer.WriteBase64(bits, 0, bits.Length);
            //writer.WriteEndElement(); // WriteBinHex
        }


    }


    [XmlSchemaProvider("MySchema")]
    public class PublicExplicitIXmlSerializablesWithPublicSchemaProvider : IXmlSerializable
    {
        byte[] bits = System.Text.Encoding.UTF8.GetBytes("hello world");

        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }

        // This is the method named by the XmlSchemaProviderAttribute applied to the type.
        public static XmlQualifiedName MySchema(XmlSchemaSet xs)
        {
            return new XmlQualifiedName("MyData2", "MyNameSpace");
        }

        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            byte[] readBits = new byte[bits.Length];
            reader.Read();
            int readLen = reader.ReadContentAsBinHex(readBits, 0, readBits.Length);

            if (bits.Length != readLen)
            {
                throw new Exception("Test Code Exception: read content length didnt match expected in IReadWriteXmlWriteBinHex.ReadXml. Read: " + readLen + " Original: " + bits.Length);
            }

            for (int i = 0; i < readLen; i++)
            {
                if (readBits[i] != bits[i])
                {
                    throw new Exception("Test Code Exception: read xml content is not correct in IReadWriteXmlWriteBinHex.ReadXml. Read: " + readBits[i] + " Original: " + bits[i]);

                }
            }

        }

        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            byte[] bits = System.Text.Encoding.UTF8.GetBytes("hello world");
            //writer.WriteStartElement("WriteBinHex");
            writer.WriteBinHex(bits, 0, bits.Length); //writes hello world again
            //writer.WriteBase64(bits, 0, bits.Length);
            //writer.WriteEndElement(); // WriteBinHex
        }


    }

    [XmlSchemaProvider("MySchema")]
    public class PublicIXmlSerializablesWithPrivateSchemaProvider : IXmlSerializable
    {
        byte[] bits = System.Text.Encoding.UTF8.GetBytes("hello world");

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        // This is the method named by the XmlSchemaProviderAttribute applied to the type.
        static XmlQualifiedName MySchema(XmlSchemaSet xs)
        {
            return new XmlQualifiedName("MyData3", "MyNameSpace");
        }

        public virtual void ReadXml(System.Xml.XmlReader reader)
        {
            byte[] readBits = new byte[bits.Length];
            reader.Read();
            int readLen = reader.ReadContentAsBinHex(readBits, 0, readBits.Length);

            if (bits.Length != readLen)
            {
                throw new Exception("Test Code Exception: read content length didnt match expected in IReadWriteXmlWriteBinHex.ReadXml. Read: " + readLen + " Original: " + bits.Length);
            }

            for (int i = 0; i < readLen; i++)
            {
                if (readBits[i] != bits[i])
                {
                    throw new Exception("Test Code Exception: read xml content is not correct in IReadWriteXmlWriteBinHex.ReadXml. Read: " + readBits[i] + " Original: " + bits[i]);

                }
            }

        }

        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            byte[] bits = System.Text.Encoding.UTF8.GetBytes("hello world");
            //writer.WriteStartElement("WriteBinHex");
            writer.WriteBinHex(bits, 0, bits.Length); //writes hello world again
            //writer.WriteBase64(bits, 0, bits.Length);
            //writer.WriteEndElement(); // WriteBinHex
        }


    }

    #endregion

    #region DC and DM access test sampels

    [DataContract(IsReference = true)]
    class PrivateDCClassPublicDM
    {
        [DataMember]
        public string Data = "Data";

        public PrivateDCClassPublicDM() { }
        public PrivateDCClassPublicDM(bool init) { Data = "No change"; }
    }

    [DataContract(IsReference = true)]
    class PrivateDCClassPrivateDM
    {
        [DataMember]
        private string Data;

        public PrivateDCClassPrivateDM() { Data = String.Empty; }
        public PrivateDCClassPrivateDM(bool init) { Data = "No change"; }
    }

    [DataContract(IsReference = true)]
    class PrivateDCClassInternalDM
    {
        [DataMember]
        internal string Data;

        public PrivateDCClassInternalDM() { }
        public PrivateDCClassInternalDM(bool init) { Data = "No change"; }
    }

    [DataContract(IsReference = true)]
    class PrivateDCClassMixedDM
    {
        [DataMember]
        public string Data1;

        [DataMember]
        private string Data2 = String.Empty;

        [DataMember]
        internal string Data3 = String.Empty;

        public PrivateDCClassMixedDM() { }
        public PrivateDCClassMixedDM(bool init) { Data1 = "No change"; }
    }

    [DataContract(IsReference = true)]
    public class PublicDCClassPublicDM
    {
        [DataMember]
        public string Data;

        public PublicDCClassPublicDM() { }
        public PublicDCClassPublicDM(bool init) { Data = "No change"; }
    }

    [DataContract(IsReference = true)]
    public class PublicDCClassPrivateDM
    {
        [DataMember]
        private string Data;

        public PublicDCClassPrivateDM() { Data = String.Empty; }
        public PublicDCClassPrivateDM(bool init) { Data = "No change"; }
    }

    [DataContract(IsReference = true)]
    public class PublicDCClassInternalDM
    {
        [DataMember]
        internal string Data;

        public PublicDCClassInternalDM() { }
        public PublicDCClassInternalDM(bool init) { Data = "No change"; }
    }

    [DataContract(IsReference = true)]
    public class PublicDCClassMixedDM
    {
        [DataMember]
        public string Data1 = String.Empty;

        [DataMember]
        private string Data2 = String.Empty;

        [DataMember]
        internal string Data3 = String.Empty;

        public PublicDCClassMixedDM() { }
        public PublicDCClassMixedDM(bool init) { Data1 = "No change"; }
    }



    [DataContract(IsReference = true)]
    class PublicDCClassPublicDM_DerivedDCClassPrivate : PublicDCClassPublicDM
    {
    }

    [DataContract(IsReference = true)]
    public class PublicDCClassPublicDM_DerivedDCClassPublic : PublicDCClassPublicDM
    {
    }

    [DataContract(IsReference = true)]
    class PublicDCClassPrivateDM_DerivedDCClassPrivate : PublicDCClassPrivateDM
    {
    }

    [DataContract(IsReference = true)]
    public class PublicDCClassPrivateDM_DerivedDCClassPublic : PublicDCClassPrivateDM
    {
    }

    [DataContract(IsReference = true)]
    class PrivateDCClassPublicDM_DerivedDCClassPrivate : PrivateDCClassPublicDM
    {
    }

    /** following not allowed  - cant compiler
    [DataContract(IsReference=true)]
    public class PrivateDCClassPublicDM_DerivedDCClassPublic : PrivateDCClassPublicDM
    {
    }
    */

    [DataContract(IsReference = true)]
    public class PublicDCClassPublicDM_DerivedDCClassPublicContainsPrivateDM : PublicDCClassPublicDM
    {
        [DataMember]
        string DerivedData1;

        [DataMember]
        public string DerivedData2;

        public PublicDCClassPublicDM_DerivedDCClassPublicContainsPrivateDM()
        {
            DerivedData1 = String.Empty;
        }
    }

    #endregion

    #region property based access samples

    [DataContract(IsReference = true)]
    public class Prop_PublicDCClassPublicDM_PublicDCClassPrivateDM
    {
        PublicDCClassPrivateDM _data;
        [DataMember]
        public PublicDCClassPrivateDM Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public Prop_PublicDCClassPublicDM_PublicDCClassPrivateDM() { }
        public Prop_PublicDCClassPublicDM_PublicDCClassPrivateDM(bool init) { Data = new PublicDCClassPrivateDM(); }
    }

    [DataContract(IsReference = true)]
    public class Prop_SetPrivate_PublicDCClassPublicDM_PublicDCClassPrivateDM
    {
        internal PublicDCClassPrivateDM _data;
        [DataMember]
        public virtual PublicDCClassPrivateDM Data
        {
            get { return _data; }
            private set { _data = value; }
        }

        public Prop_SetPrivate_PublicDCClassPublicDM_PublicDCClassPrivateDM() { }
        public Prop_SetPrivate_PublicDCClassPublicDM_PublicDCClassPrivateDM(bool init) { Data = new PublicDCClassPrivateDM(); }
    }

    [DataContract(IsReference = true)]
    public class Prop_GetPrivate_PublicDCClassPublicDM_PublicDCClassPrivateDM
    {
        internal PublicDCClassPrivateDM _data;
        [DataMember]
        public PublicDCClassPrivateDM Data
        {
            private get { return _data; }
            set { _data = value; }
        }

        public Prop_GetPrivate_PublicDCClassPublicDM_PublicDCClassPrivateDM() { }
        public Prop_GetPrivate_PublicDCClassPublicDM_PublicDCClassPrivateDM(bool init) { Data = new PublicDCClassPrivateDM(); }
    }

    [DataContract(IsReference = true)]
    class Prop_PrivateDCClassPublicDM
    {
        string _data;

        [DataMember]
        public string Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public Prop_PrivateDCClassPublicDM() { }
        public Prop_PrivateDCClassPublicDM(bool init) { Data = "No change"; }
    }

    [DataContract(IsReference = true)]
    class Prop_PrivateDCClassPrivateDM
    {
        string _data;
        [DataMember]
        private string Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public Prop_PrivateDCClassPrivateDM() { }
        public Prop_PrivateDCClassPrivateDM(bool init) { Data = "No change"; }
    }

    [DataContract(IsReference = true)]
    class Prop_PrivateDCClassInternalDM
    {
        string _data;
        [DataMember]
        internal string Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public Prop_PrivateDCClassInternalDM() { }
        public Prop_PrivateDCClassInternalDM(bool init) { Data = "No change"; }
    }

    [DataContract(IsReference = true)]
    class Prop_PrivateDCClassMixedDM
    {
        string _data1;
        string _data2;
        string _data3;

        [DataMember]
        public string Data1
        {
            get { return _data1; }
            set { _data1 = value; }
        }

        [DataMember]
        private string Data2
        {
            get { return _data2; }
            set { _data2 = value; }
        }

        [DataMember]
        internal string Data3
        {
            get { return _data3; }
            set { _data3 = value; }
        }

        public Prop_PrivateDCClassMixedDM() { }
        public Prop_PrivateDCClassMixedDM(bool init) { Data1 = "No change"; }
    }

    [DataContract(IsReference = true)]
    public class Prop_PublicDCClassPublicDM
    {
        string _data;
        [DataMember]
        public string Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public Prop_PublicDCClassPublicDM() { }
        public Prop_PublicDCClassPublicDM(bool init) { Data = "No change"; }
    }

    [DataContract(IsReference = true)]
    public class Prop_PublicDCClassPrivateDM
    {
        string _data;
        [DataMember]
        private string Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public Prop_PublicDCClassPrivateDM() { }
        public Prop_PublicDCClassPrivateDM(bool init) { Data = "No change"; }
    }

    [DataContract(IsReference = true)]
    public class Prop_PublicDCClassInternalDM
    {
        string _data;
        [DataMember]
        internal string Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public Prop_PublicDCClassInternalDM() { }
        public Prop_PublicDCClassInternalDM(bool init) { Data = "No change"; }
    }

    [DataContract(IsReference = true)]
    public class Prop_PublicDCClassMixedDM
    {
        string _data1;
        string _data2;
        string _data3;

        [DataMember]
        public string Data1
        {
            get { return _data1; }
            set { _data1 = value; }
        }

        [DataMember]
        private string Data2
        {
            get { return _data2; }
            set { _data2 = value; }
        }

        [DataMember]
        internal string Data3
        {
            get { return _data3; }
            set { _data3 = value; }
        }


        public Prop_PublicDCClassMixedDM() { }
        public Prop_PublicDCClassMixedDM(bool init) { Data1 = "No change"; }
    }

    [DataContract(IsReference = true)]
    class Prop_PublicDCClassPublicDM_DerivedDCClassPrivate : Prop_PublicDCClassPublicDM
    {
    }

    [DataContract(IsReference = true)]
    public class Prop_PublicDCClassPublicDM_DerivedDCClassPublic : Prop_PublicDCClassPublicDM
    {
    }

    [DataContract(IsReference = true)]
    class Prop_PublicDCClassPrivateDM_DerivedDCClassPrivate : Prop_PublicDCClassPrivateDM
    {
    }

    [DataContract(IsReference = true)]
    public class Prop_PublicDCClassPrivateDM_DerivedDCClassPublic : Prop_PublicDCClassPrivateDM
    {
    }

    [DataContract(IsReference = true)]
    class Prop_PrivateDCClassPublicDM_DerivedDCClassPrivate : Prop_PrivateDCClassPublicDM
    {
    }

    /** following not allowed  - cant compile
    [DataContract(IsReference=true)]
    public class Prop_PrivateDCClassPublicDM_DerivedDCClassPublic : Prop_PrivateDCClassPublicDM
    {
    }
    */

    [DataContract(IsReference = true)]
    public class Prop_PublicDCClassPublicDM_DerivedDCClassPublicContainsPrivateDM : Prop_PublicDCClassPublicDM
    {
        [DataMember]
        string DerivedData1 = String.Empty;

        [DataMember]
        public string DerivedData2;
    }


    [DataContract(IsReference = true)]
    public class Prop_SetPrivate_PublicDCClassPublicDM
    {
        internal string _data;
        [DataMember]
        public virtual string Data
        {
            get { return _data; }
            private set { _data = value; }
        }

        public Prop_SetPrivate_PublicDCClassPublicDM() { }
        public Prop_SetPrivate_PublicDCClassPublicDM(bool init) { Data = "No change"; }
    }

    [DataContract(IsReference = true)]
    public class Prop_GetPrivate_PublicDCClassPublicDM
    {
        internal string _data;
        [DataMember]
        public string Data
        {
            private get { return _data; }
            set { _data = value; }
        }

        public Prop_GetPrivate_PublicDCClassPublicDM() { }
        public Prop_GetPrivate_PublicDCClassPublicDM(bool init) { Data = "No change"; }
    }

    [DataContract(IsReference = true)]
    public class Derived_Override_Prop_All_Public : Prop_SetPrivate_PublicDCClassPublicDM
    {
        [DataMember]
        public new string Data
        {
            get { return _data; }
            private set { _data = value; }
        }

        public Derived_Override_Prop_All_Public() { }
        public Derived_Override_Prop_All_Public(bool init) { Data = "No change"; }
    }

    [DataContract(IsReference = true)]
    public class Derived_Override_Prop_Private : Prop_SetPrivate_PublicDCClassPublicDM
    {
        [DataMember]
        public new string Data
        {
            private get { return _data; }
            set { _data = value; }
        }

        public Derived_Override_Prop_Private() { }
        public Derived_Override_Prop_Private(bool init) { Data = "No change"; }
    }

    [DataContract(IsReference = true)]
    public class Derived_Override_Prop_GetPrivate_All_Public : Prop_GetPrivate_PublicDCClassPublicDM
    {
        [DataMember]
        public new string Data
        {
            get { return _data; }
            private set { _data = value; }
        }

        public Derived_Override_Prop_GetPrivate_All_Public() { }
        public Derived_Override_Prop_GetPrivate_All_Public(bool init) { Data = "No change"; }
    }

    [DataContract(IsReference = true)]
    public class Derived_Override_Prop_GetPrivate_Private : Prop_GetPrivate_PublicDCClassPublicDM
    {
        [DataMember]
        public new string Data
        {
            private get { return _data; }
            set { _data = value; }
        }

        public Derived_Override_Prop_GetPrivate_Private() { }
        public Derived_Override_Prop_GetPrivate_Private(bool init) { Data = "No change"; }
    }


    #endregion

    #region Versioned roundtrips

    [DataContract(Name = "DC1_Version")]
    public class DC1_Version1
    {
    }

    [DataContract(Name = "DC1_Version2")]
    class DC1_Version2
    {
    }

    [DataContract(Name = "DC2_Version1")]
    public class DC2_Version1
    {
        [DataMember]
        public string Data;
    }

    [DataContract(Name = "DC2_Version2")]
    class DC2_Version2
    {
        [DataMember]
        public string Data = String.Empty;
    }

    [DataContract(Name = "DC2_Version3")]
    class DC2_Version3
    {
        [DataMember]
        string Data = String.Empty;
    }

    [DataContract(Name = "DC2_Version4")]
    public class DC2_Version4
    {
        [DataMember]
        string Data = String.Empty;
    }

    [DataContract(Name = "DC2_Version5")]
    public class DC2_Version5
    {
        string _data;

        [DataMember]
        public string Data
        {
            get { return _data; }
            private set { _data = value; }
        }
    }

    [DataContract(Name = "DC3_Version1")]
    public class DC3_Version1
    {
        [DataMember]
        public string Data1;
    }

    [DataContract(Name = "DC3_Version2")]
    public class DC3_Version2 : IExtensibleDataObject
    {
        ExtensionDataObject _extensionData;

        #region IExtensibleDataObject Members

        public ExtensionDataObject ExtensionData
        {
            get
            {
                return _extensionData;
            }
            set
            {
                _extensionData = value;
            }
        }

        #endregion
    }

    [DataContract(Name = "DC3_Version3")]
    public class DC3_Version3 : IExtensibleDataObject
    {
        ExtensionDataObject _extensionData;

        #region IExtensibleDataObject Members

        ExtensionDataObject IExtensibleDataObject.ExtensionData
        {
            get
            {
                return _extensionData;
            }
            set
            {
                _extensionData = value;
            }
        }

        #endregion
    }

    #endregion

    #region Callbacks

    [DataContract(IsReference = true)]
    public class CallBackSample_OnSerializing_Public
    {
        [DataMember]
        public string Data;

        [OnSerializing]
        public void OnSerializing(System.Runtime.Serialization.StreamingContext context) { }

    }

    [DataContract(IsReference = true)]
    public class CallBackSample_OnSerialized_Public
    {
        [DataMember]
        public string Data;

        [OnSerialized]
        public void OnSerialized(System.Runtime.Serialization.StreamingContext context) { }

    }


    [DataContract(IsReference = true)]
    public class CallBackSample_OnDeserializing_Public
    {
        [DataMember]
        public string Data;

        [OnDeserializing]
        public void OnDeserializing(System.Runtime.Serialization.StreamingContext context) { }

    }

    [DataContract(IsReference = true)]
    public class CallBackSample_OnDeserialized_Public
    {
        [DataMember]
        public string Data;

        [OnDeserialized]
        public void OnDeserialized(System.Runtime.Serialization.StreamingContext context) { }

    }


    [DataContract(IsReference = true)]
    public class CallBackSample_OnSerializing
    {
        [DataMember]
        public string Data;

        [OnSerializing]
        void OnSerializing(System.Runtime.Serialization.StreamingContext context) { }

    }

    [DataContract(IsReference = true)]
    public class CallBackSample_OnSerialized
    {
        [DataMember]
        public string Data;

        [OnSerialized]
        internal void OnSerialized(System.Runtime.Serialization.StreamingContext context) { }

    }


    [DataContract(IsReference = true)]
    public class CallBackSample_OnDeserializing
    {
        [DataMember]
        public string Data;

        [OnDeserializing]
        private void OnDeserializing(System.Runtime.Serialization.StreamingContext context) { }

    }

    [DataContract(IsReference = true)]
    public class CallBackSample_OnDeserialized
    {
        [DataMember]
        public string Data;

        [OnDeserialized]
        protected internal void OnDeserialized(System.Runtime.Serialization.StreamingContext context) { }
    }



    [DataContract(IsReference = true)]
    public class CallBackSample_IDeserializationCallback : IDeserializationCallback
    {
        [DataMember]
        public string Data;


        #region IDeserializationCallback Members

        public void OnDeserialization(object sender)
        {

        }

        #endregion
    }

    [DataContract(IsReference = true)]
    public class CallBackSample_IDeserializationCallback_Explicit : IDeserializationCallback
    {
        [DataMember]
        public string Data;

        #region IDeserializationCallback Members

        void IDeserializationCallback.OnDeserialization(object sender)
        {

        }

        #endregion
    }


    #region in private types

    [DataContract(IsReference = true)]
    class PrivateCallBackSample_OnSerializing_Public
    {
        [DataMember]
        public string Data = "Data";

        [OnSerializing]
        public void OnSerializing(System.Runtime.Serialization.StreamingContext context) { }

    }

    [DataContract(IsReference = true)]
    class PrivateCallBackSample_OnSerialized_Public
    {
        [DataMember]
        public string Data = "Data";

        [OnSerialized]
        public void OnSerialized(System.Runtime.Serialization.StreamingContext context) { }

    }


    [DataContract(IsReference = true)]
    class PrivateCallBackSample_OnDeserializing_Public
    {
        [DataMember]
        public string Data = "Data";

        [OnDeserializing]
        public void OnDeserializing(System.Runtime.Serialization.StreamingContext context) { }

    }

    [DataContract(IsReference = true)]
    class PrivateCallBackSample_OnDeserialized_Public
    {
        [DataMember]
        public string Data = "Data";

        [OnDeserialized]
        public void OnDeserialized(System.Runtime.Serialization.StreamingContext context) { }

    }


    [DataContract(IsReference = true)]
    class PrivateCallBackSample_OnSerializing
    {
        [DataMember]
        public string Data = "Data";

        [OnSerializing]
        void OnSerializing(System.Runtime.Serialization.StreamingContext context) { }

    }

    [DataContract(IsReference = true)]
    class PrivateCallBackSample_OnSerialized
    {
        [DataMember]
        public string Data = "Data";

        [OnSerialized]
        internal void OnSerialized(System.Runtime.Serialization.StreamingContext context) { }

    }


    [DataContract(IsReference = true)]
    class PrivateCallBackSample_OnDeserializing
    {
        [DataMember]
        public string Data = "Data";

        [OnDeserializing]
        private void OnDeserializing(System.Runtime.Serialization.StreamingContext context) { }

    }

    [DataContract(IsReference = true)]
    class PrivateCallBackSample_OnDeserialized
    {
        [DataMember]
        public string Data = "Data";

        [OnDeserialized]
        protected internal void OnDeserialized(System.Runtime.Serialization.StreamingContext context) { }
    }

    [DataContract(IsReference = true)]
    class PrivateCallBackSample_IDeserializationCallback : IDeserializationCallback
    {
        [DataMember]
        public string Data = "Data";


        #region IDeserializationCallback Members

        public void OnDeserialization(object sender)
        {

        }

        #endregion
    }

    [DataContract(IsReference = true)]
    class PrivateCallBackSample_IDeserializationCallback_Explicit : IDeserializationCallback
    {
        [DataMember]
        public string Data = "Data";

        #region IDeserializationCallback Members

        void IDeserializationCallback.OnDeserialization(object sender)
        {

        }

        #endregion
    }

    [DataContract(IsReference = true)]
    public class CallBackSample_OnDeserialized_Private_Base
    {
        [DataMember]
        public string Data = DateTime.Now.ToLongTimeString();

        [OnDeserialized]
        void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            Console.WriteLine("Base");
        }

    }

    [DataContract(IsReference = true)]
    public class CallBackSample_OnDeserialized_Public_Derived : CallBackSample_OnDeserialized_Private_Base
    {
        [DataMember]
        public string Data2 = DateTime.Now.ToLongTimeString();

        public CallBackSample_OnDeserialized_Public_Derived() { }
        [OnDeserialized]
        public void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            Console.WriteLine("Derived");
        }

    }

    #endregion

    #endregion

    #region collection Datacontract

    [CollectionDataContract(IsReference = true)]
    public class CDC_Possitive : IList<string>
    {
        List<string> innerList = new List<string>();

        #region IList<string> Members

        public int IndexOf(string item)
        {
            return innerList.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            innerList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            innerList.RemoveAt(index);
        }

        public string this[int index]
        {
            get
            {
                return innerList[index];
            }
            set
            {
                innerList[index] = value;
            }
        }

        #endregion

        #region ICollection<string> Members

        public void Add(string item)
        {
            innerList.Add(item);
        }

        public void Clear()
        {
            innerList.Clear();
        }

        public bool Contains(string item)
        {
            return innerList.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            innerList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return innerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(string item)
        {
            return innerList.Remove(item);
        }

        #endregion

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        #endregion

        public static CDC_Possitive CreateInstance()
        {
            CDC_Possitive list = new CDC_Possitive();
            list.Add("112");
            return list;
        }
    }


    [CollectionDataContract(IsReference = true)]
    public class CDC_PrivateAdd : IEnumerable<string>
    {
        List<string> innerList = new List<string>();

        #region IList<string> Members

        public int IndexOf(string item)
        {
            return innerList.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            innerList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            innerList.RemoveAt(index);
        }

        public string this[int index]
        {
            get
            {
                return innerList[index];
            }
            set
            {
                innerList[index] = value;
            }
        }

        #endregion

        #region ICollection<string> Members

        void Add(string item)
        {
            innerList.Add(item.ToString());
        }

        public void Clear()
        {
            innerList.Clear();
        }

        public bool Contains(string item)
        {
            return innerList.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            innerList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return innerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(string item)
        {
            return innerList.Remove(item);
        }

        #endregion

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        #endregion


        public static CDC_PrivateAdd CreateInstance()
        {
            CDC_PrivateAdd list = new CDC_PrivateAdd();
            list.Insert(0, "222323");
            list.Insert(1, "222323");
            return list;
        }
    }


    [CollectionDataContract(IsReference = true)]
    public class CDC_PrivateDefaultCtor : IList<string>
    {
        List<string> innerList = new List<string>();

        private CDC_PrivateDefaultCtor()
        {
        }

        public CDC_PrivateDefaultCtor(bool init)
        {
        }

        #region IList<string> Members

        public int IndexOf(string item)
        {
            return innerList.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            innerList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            innerList.RemoveAt(index);
        }

        public string this[int index]
        {
            get
            {
                return innerList[index];
            }
            set
            {
                innerList[index] = value;
            }
        }

        #endregion

        #region ICollection<string> Members

        public void Add(string item)
        {
            innerList.Add(item);
        }

        public void Clear()
        {
            innerList.Clear();
        }

        public bool Contains(string item)
        {
            return innerList.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            innerList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return innerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(string item)
        {
            return innerList.Remove(item);
        }

        #endregion

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        #endregion
    }


    [CollectionDataContract(IsReference = true)]
    class CDC_Private : IList<string>
    {
        List<string> innerList = new List<string>();

        #region IList<string> Members

        public int IndexOf(string item)
        {
            return innerList.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            innerList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            innerList.RemoveAt(index);
        }

        public string this[int index]
        {
            get
            {
                return innerList[index];
            }
            set
            {
                innerList[index] = value;
            }
        }

        #endregion

        #region ICollection<string> Members

        public void Add(string item)
        {
            innerList.Add(item);
        }

        public void Clear()
        {
            innerList.Clear();
        }

        public bool Contains(string item)
        {
            return innerList.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            innerList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return innerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(string item)
        {
            return innerList.Remove(item);
        }

        #endregion

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        #endregion
    }


    [CollectionDataContract(IsReference = true)]
    public class Base_Possitive_VirtualAdd : IEnumerable<String>
    {
        List<string> innerList = new List<string>();

        #region IList<string> Members

        public int IndexOf(string item)
        {
            return innerList.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            innerList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            innerList.RemoveAt(index);
        }

        public string this[int index]
        {
            get
            {
                return innerList[index];
            }
            set
            {
                innerList[index] = value;
            }
        }

        #endregion

        #region ICollection<string> Members

        public virtual void Add(String item)
        {
            innerList.Add(item.ToString());
        }

        public void Clear()
        {
            innerList.Clear();
        }

        public bool Contains(string item)
        {
            return innerList.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            innerList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return innerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(string item)
        {
            return innerList.Remove(item);
        }

        #endregion

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        #endregion

        public static Base_Possitive_VirtualAdd CreateInstance()
        {
            Base_Possitive_VirtualAdd list = new Base_Possitive_VirtualAdd();
            list.Insert(0, "222323");
            list.Insert(1, "222323");
            return list;
        }
    }

    [CollectionDataContract(IsReference = true)]
    public class CDC_NewAddToPrivate : Base_Possitive_VirtualAdd
    {
        private new void Add(String item)
        {
            base.Add(item.ToString());
        }

        public new static CDC_NewAddToPrivate CreateInstance()
        {
            CDC_NewAddToPrivate list = new CDC_NewAddToPrivate();
            list.Insert(0, "223213");
            list.Insert(0, "223213");
            return list;
        }
    }

    #endregion

    #region surrogates

    public class NonDCPerson
    {
        public string Name = "jeff";
        public int Age = 20;

        public NonDCPerson() { }

        public NonDCPerson(PersonSurrogated nonDCPerson)
        {
            this.Name = nonDCPerson.Name;
            this.Age = nonDCPerson.Age;
        }
    }

    [DataContract(IsReference = true)]
    public class PersonSurrogated
    {
        [DataMember]
        public string Name = "Jeffery";

        [DataMember]
        public int Age = 30;

        public PersonSurrogated() { }

        public PersonSurrogated(NonDCPerson nonDCPerson)
        {
            this.Name = nonDCPerson.Name;
            this.Age = nonDCPerson.Age;
        }
    }
    /*
    public class DCSurrogate : IDataContractSurrogate
    {
        #region IDataContractSurrogate Members

        public object GetCustomDataToExport(Type clrType, Type dataContractType)
        {
            throw new NotImplementedException();
        }

        public object GetCustomDataToExport(System.Reflection.MemberInfo memberInfo, Type dataContractType)
        {
            throw new NotImplementedException();
        }

        public Type GetDataContractType(Type type)
        {
            if (typeof(NonDCPerson).IsAssignableFrom(type))
            {
                return typeof(PersonSurrogated);
            }
            return type;
        }

        public object GetDeserializedObject(object obj, Type memberType)
        {
            if (obj is PersonSurrogated)
            {
                return new NonDCPerson((PersonSurrogated)obj);
            }
            return obj;
        }

        public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
        {

        }

        public object GetObjectToSerialize(object obj, Type membertype)
        {
            if (obj is NonDCPerson)
            {
                return new PersonSurrogated((NonDCPerson)obj);
            }
            return obj;
        }

        public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            throw new NotImplementedException();
        }

        public System.CodeDom.CodeTypeDeclaration ProcessImportedType(System.CodeDom.CodeTypeDeclaration typeDeclaration, System.CodeDom.CodeCompileUnit compileUnit)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    */
    public class SerSurrogate : ISerializationSurrogate
    {
        #region ISerializationSurrogate Members

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            if (obj is NonDCPerson)
            {
                info.AddValue("data", new PersonSurrogated(obj as NonDCPerson));
            }
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            if (obj is PersonSurrogated)
            {
                return new NonDCPerson(obj as PersonSurrogated);
            }
            else if (obj is NonDCPerson)
            {
                return new NonDCPerson(info.GetValue("data", typeof(PersonSurrogated)) as PersonSurrogated);
            }
            else
            {
                return obj;
            }

        }

        #endregion
    }

    #region explicit implementations
    /*
    public class DCSurrogateExplicit : IDataContractSurrogate
    {
        #region IDataContractSurrogate Members

        object IDataContractSurrogate.GetCustomDataToExport(Type clrType, Type dataContractType)
        {
            throw new NotImplementedException();
        }

        object IDataContractSurrogate.GetCustomDataToExport(System.Reflection.MemberInfo memberInfo, Type dataContractType)
        {
            throw new NotImplementedException();
        }

        Type IDataContractSurrogate.GetDataContractType(Type type)
        {
            if (typeof(NonDCPerson).IsAssignableFrom(type))
            {
                return typeof(PersonSurrogated);
            }
            return type;
        }

        object IDataContractSurrogate.GetDeserializedObject(object obj, Type memberType)
        {
            if (obj is PersonSurrogated)
            {
                return new NonDCPerson((PersonSurrogated)obj);
            }
            return obj;
        }

        void IDataContractSurrogate.GetKnownCustomDataTypes(Collection<Type> customDataTypes)
        {

        }

        object IDataContractSurrogate.GetObjectToSerialize(object obj, Type membertype)
        {
            if (obj is NonDCPerson)
            {
                return new PersonSurrogated((NonDCPerson)obj);
            }
            return obj;
        }

        Type IDataContractSurrogate.GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            throw new NotImplementedException();
        }

        System.CodeDom.CodeTypeDeclaration IDataContractSurrogate.ProcessImportedType(System.CodeDom.CodeTypeDeclaration typeDeclaration, System.CodeDom.CodeCompileUnit compileUnit)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    */
    public class SerSurrogateExplicit : ISerializationSurrogate
    {
        #region ISerializationSurrogate Members

        void ISerializationSurrogate.GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            if (obj is NonDCPerson)
            {
                info.AddValue("data", new PersonSurrogated(obj as NonDCPerson));
            }
        }

        object ISerializationSurrogate.SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            if (obj is PersonSurrogated)
            {
                return new NonDCPerson(obj as PersonSurrogated);
            }
            else if (obj is NonDCPerson)
            {
                return new NonDCPerson(info.GetValue("data", typeof(PersonSurrogated)) as PersonSurrogated);
            }
            else
            {
                return obj;
            }

        }

        #endregion
    }

    #endregion

    #region private surrogate type explicit implementations
    /*
    class PrivateDCSurrogate : IDataContractSurrogate
    {
        #region IDataContractSurrogate Members

        public object GetCustomDataToExport(Type clrType, Type dataContractType)
        {
            throw new NotImplementedException();
        }

        public object GetCustomDataToExport(System.Reflection.MemberInfo memberInfo, Type dataContractType)
        {
            throw new NotImplementedException();
        }

        public Type GetDataContractType(Type type)
        {
            if (typeof(NonDCPerson).IsAssignableFrom(type))
            {
                return typeof(PersonSurrogated);
            }
            return type;
        }

        public object GetDeserializedObject(object obj, Type memberType)
        {
            if (obj is PersonSurrogated)
            {
                return new NonDCPerson((PersonSurrogated)obj);
            }
            return obj;
        }

        public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
        {

        }

        public object GetObjectToSerialize(object obj, Type membertype)
        {
            if (obj is NonDCPerson)
            {
                return new PersonSurrogated((NonDCPerson)obj);
            }
            return obj;
        }

        public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            throw new NotImplementedException();
        }

        public System.CodeDom.CodeTypeDeclaration ProcessImportedType(System.CodeDom.CodeTypeDeclaration typeDeclaration, System.CodeDom.CodeCompileUnit compileUnit)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    */
    class PrivateSerSurrogate : ISerializationSurrogate
    {
        #region ISerializationSurrogate Members

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            if (obj is NonDCPerson)
            {
                info.AddValue("data", new PersonSurrogated(obj as NonDCPerson));
            }
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            if (obj is PersonSurrogated)
            {
                return new NonDCPerson(obj as PersonSurrogated);
            }
            else if (obj is NonDCPerson)
            {
                return new NonDCPerson(info.GetValue("data", typeof(PersonSurrogated)) as PersonSurrogated);
            }
            else
            {
                return obj;
            }

        }

        #endregion
    }

    /*
    class PrivateDCSurrogateExplicit : IDataContractSurrogate
    {
        #region IDataContractSurrogate Members

        object IDataContractSurrogate.GetCustomDataToExport(Type clrType, Type dataContractType)
        {
            throw new NotImplementedException();
        }

        object IDataContractSurrogate.GetCustomDataToExport(System.Reflection.MemberInfo memberInfo, Type dataContractType)
        {
            throw new NotImplementedException();
        }

        Type IDataContractSurrogate.GetDataContractType(Type type)
        {
            if (typeof(NonDCPerson).IsAssignableFrom(type))
            {
                return typeof(PersonSurrogated);
            }
            return type;
        }

        object IDataContractSurrogate.GetDeserializedObject(object obj, Type memberType)
        {
            if (obj is PersonSurrogated)
            {
                return new NonDCPerson((PersonSurrogated)obj);
            }
            return obj;
        }

        void IDataContractSurrogate.GetKnownCustomDataTypes(Collection<Type> customDataTypes)
        {

        }

        object IDataContractSurrogate.GetObjectToSerialize(object obj, Type membertype)
        {
            if (obj is NonDCPerson)
            {
                return new PersonSurrogated((NonDCPerson)obj);
            }
            return obj;
        }

        Type IDataContractSurrogate.GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            throw new NotImplementedException();
        }

        System.CodeDom.CodeTypeDeclaration IDataContractSurrogate.ProcessImportedType(System.CodeDom.CodeTypeDeclaration typeDeclaration, System.CodeDom.CodeCompileUnit compileUnit)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    */
    class PrivateSerSurrogateExplicit : ISerializationSurrogate
    {
        #region ISerializationSurrogate Members

        void ISerializationSurrogate.GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            if (obj is NonDCPerson)
            {
                info.AddValue("data", new PersonSurrogated(obj as NonDCPerson));
            }
        }

        object ISerializationSurrogate.SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            if (obj is PersonSurrogated)
            {
                return new NonDCPerson(obj as PersonSurrogated);
            }
            else if (obj is NonDCPerson)
            {
                return new NonDCPerson(info.GetValue("data", typeof(PersonSurrogated)) as PersonSurrogated);
            }
            else
            {
                return obj;
            }

        }

        #endregion
    }

    #endregion

    #region private dc
    /*
    public class DCSurrogateReturnPrivate : IDataContractSurrogate
    {
        #region IDataContractSurrogate Members

        public object GetCustomDataToExport(Type clrType, Type dataContractType)
        {
            throw new NotImplementedException();
        }

        public object GetCustomDataToExport(System.Reflection.MemberInfo memberInfo, Type dataContractType)
        {
            throw new NotImplementedException();
        }

        public Type GetDataContractType(Type type)
        {
            if (typeof(NonDCPerson).IsAssignableFrom(type))
            {
                return typeof(PrivateDC);
            }
            return type;
        }

        public object GetDeserializedObject(object obj, Type memberType)
        {
            if (obj is PrivateDC)
            {
                return new NonDCPerson();
            }
            return obj;
        }

        public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
        {

        }

        public object GetObjectToSerialize(object obj, Type membertype)
        {
            if (obj is NonDCPerson)
            {
                return new PrivateDC();
            }
            return obj;
        }

        public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            throw new NotImplementedException();
        }

        public System.CodeDom.CodeTypeDeclaration ProcessImportedType(System.CodeDom.CodeTypeDeclaration typeDeclaration, System.CodeDom.CodeCompileUnit compileUnit)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    */
    public class SerSurrogateReturnPrivate : ISerializationSurrogate
    {
        #region ISerializationSurrogate Members

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            if (obj is NonDCPerson)
            {
                info.AddValue("data", new PrivateDC());
            }
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            if (obj is PrivateDC)
            {
                return new NonDCPerson();
            }
            else
            {
                return obj;
            }

        }

        #endregion
    }
    #endregion

    #endregion

    #region Nullable<T>

    [DataContract(IsReference = true)]
    public class NullableContainerContainsValue
    {
        [DataMember]
        public Nullable<PublicDCStruct> Data = new PublicDCStruct(true);

        public NullableContainerContainsValue()
        {
        }
    }

    [DataContract(IsReference = true)]
    public class NullableContainerContainsNull
    {
        [DataMember]
        public Nullable<PublicDCStruct> Data = null;

        public NullableContainerContainsNull()
        {
        }
    }

    [DataContract(IsReference = true)]
    class PrivateNullableContainerContainsValue
    {
        [DataMember]
        public Nullable<PublicDCStruct> Data = new PublicDCStruct(true);

        public PrivateNullableContainerContainsValue()
        {
        }
    }

    [DataContract(IsReference = true)]
    class PrivateNullableContainerContainsNull
    {
        [DataMember]
        public Nullable<PublicDCStruct> Data = null;

        public PrivateNullableContainerContainsNull()
        {
        }
    }

    [DataContract]//(IsReference=true)]
    public struct PublicDCStruct
    {
        [DataMember]
        public string Data;
        public PublicDCStruct(bool init)
        {
            Data = "Data";
        }
    }

    #region contains private struct

    [DataContract(IsReference = true)]
    [KnownType(typeof(Nullable<PrivateDCStruct>))]
    public class NullablePrivateContainerContainsValue
    {
        [DataMember]
        public object Data = new Nullable<PrivateDCStruct>(new PrivateDCStruct(true));

        public NullablePrivateContainerContainsValue()
        {
        }
    }

    [DataContract(IsReference = true)]
    [KnownType(typeof(Nullable<PrivateDCStruct>))]
    public class NullablePrivateContainerContainsNull
    {
        [DataMember]
        public object Data = new Nullable<PrivateDCStruct>(new PrivateDCStruct(true));

        public NullablePrivateContainerContainsNull()
        {
        }
    }

    [DataContract(IsReference = true)]
    class PrivateNullablePrivateContainerContainsValue
    {
        [DataMember]
        public Nullable<PrivateDCStruct> Data = new PrivateDCStruct(true);

        public PrivateNullablePrivateContainerContainsValue()
        {
        }
    }

    [DataContract(IsReference = true)]
    class PrivateNullablePrivateContainerContainsNull
    {
        [DataMember]
        public Nullable<PrivateDCStruct> Data = null;

        public PrivateNullablePrivateContainerContainsNull()
        {
        }
    }


    [DataContract]//(IsReference=true)]
    struct PrivateDCStruct
    {
        [DataMember]
        public int Data;

        public PrivateDCStruct(bool init)
        {
            Data = int.MaxValue;
        }

    }

    #endregion

    #region contains public dc containing private dm
    [DataContract(IsReference = true)]
    public class NullablePrivateDataInDMContainerContainsValue
    {
        [DataMember]
        public Nullable<PublicDCStructContainsPrivateDataInDM> Data = new PublicDCStructContainsPrivateDataInDM(true);

        public NullablePrivateDataInDMContainerContainsValue()
        {
        }
    }

    [DataContract(IsReference = true)]
    [KnownType(typeof(Nullable<PublicDCStructContainsPrivateDataInDM>))]
    public class NullablePrivateDataInDMContainerContainsNull
    {
        [DataMember]
        public object Data = new Nullable<PublicDCStructContainsPrivateDataInDM>(new PublicDCStructContainsPrivateDataInDM(true));

        public NullablePrivateDataInDMContainerContainsNull()
        {
        }
    }

    [DataContract(IsReference = true)]
    class PrivateNullablePrivateDataInDMContainerContainsValue
    {
        [DataMember]
        public Nullable<PublicDCStructContainsPrivateDataInDM> Data = new PublicDCStructContainsPrivateDataInDM(true);

        public PrivateNullablePrivateDataInDMContainerContainsValue()
        {
        }
    }

    [DataContract(IsReference = true)]
    class PrivateNullablePrivateDataInDMContainerContainsNull
    {
        [DataMember]
        public Nullable<PublicDCStructContainsPrivateDataInDM> Data = null;

        public PrivateNullablePrivateDataInDMContainerContainsNull()
        {
        }
    }

    [DataContract]//(IsReference=true)]
    public struct PublicDCStructContainsPrivateDataInDM
    {
        [DataMember]
        public PublicDCClassPrivateDM Data;

        public PublicDCStructContainsPrivateDataInDM(bool init)
        {
            Data = new PublicDCClassPrivateDM(true);
        }
    }

    #endregion

    #endregion

    #region Dataset Container

    [DataContract(IsReference = true)]
    public class DCPublicDatasetPublic
    {
        [DataMember]
        public DataSet dataSet;

        [DataMember]
        public DataSet dataSet2;

        [DataMember]
        public DataTable dataTable;

        [DataMember]
        public DataTable dataTable2;

        public DCPublicDatasetPublic()
        {
        }
        public DCPublicDatasetPublic(bool init)
        {
            dataSet = new DataSet("MyData");
            dataTable = new DataTable("MyTable");
            DataColumn dc1 = new DataColumn("Data", typeof(string));
            dataTable.Columns.Add(dc1);
            DataRow row1 = dataTable.NewRow();
            row1[dc1] = "10";
            dataTable.Rows.Add(row1);
            dataSet.Tables.Add(dataTable);
            dataSet2 = dataSet;
            dataTable2 = dataTable;
        }
    }


    [DataContract(IsReference = true)]
    public class DCPublicDatasetPrivate
    {
        [DataMember]
        DataSet dataSet;

        [DataMember]
        public DataTable dataTable;

        public DCPublicDatasetPrivate()
        {
        }
        public DCPublicDatasetPrivate(bool init)
        {
            dataSet = new DataSet("MyData");
            dataTable = new DataTable("MyTable");
            DataColumn dc1 = new DataColumn("Data", typeof(string));
            dataTable.Columns.Add(dc1);
            DataRow row1 = dataTable.NewRow();
            row1[dc1] = "20";
            dataTable.Rows.Add(row1);
            dataSet.Tables.Add(dataTable);
        }
    }

#pragma warning disable 0659
    [Serializable]
    public class SerPublicDatasetPublic
    {
        public DataSet dataSet;

        public DataTable dataTable;

        public SerPublicDatasetPublic()
        {
        }
        public SerPublicDatasetPublic(bool init)
        {
            dataSet = new DataSet("MyData");
            DataTable dt = new DataTable("MyTable");
            DataColumn dc1 = new DataColumn("Data", typeof(string));
            dt.Columns.Add(dc1);
            DataRow row1 = dt.NewRow();
            row1[dc1] = "Testing";
            dt.Rows.Add(row1);
            dataSet.Tables.Add(dt);

            dataTable = new DataTable("MyTable");
            DataColumn dc2 = new DataColumn("Data", typeof(string));
            dataTable.Columns.Add(dc2);
            DataRow row2 = dataTable.NewRow();
            row2[dc2] = "Testing";
            dataTable.Rows.Add(row2);
        }
        public override bool Equals(object obj)
        {
            SerPublicDatasetPublic result = (SerPublicDatasetPublic)obj;

            if (Util.CompareDataSets(this.dataSet, result.dataSet))
            {
                if (Util.CompareDataTable(this.dataTable, result.dataTable))
                {
                    return true;
                }
            }
            return false;
        }
    }

    [Serializable]
    public class SerPublicDatasetPrivate
    {
        DataSet dataSet;

        [DataMember]
        public DataTable dataTable;

        public SerPublicDatasetPrivate()
        { }
        public SerPublicDatasetPrivate(bool init)
        {
            dataSet = new DataSet("MyData");


            DataTable dt = new DataTable("MyTable");
            DataColumn dc1 = new DataColumn("Data", typeof(string));
            dt.Columns.Add(dc1);
            DataRow row1 = dt.NewRow();
            row1[dc1] = "Testing";
            dt.Rows.Add(row1);
            dataSet.Tables.Add(dt);

            dataTable = new DataTable("MyTable");
            DataColumn dc2 = new DataColumn("Data", typeof(string));
            dataTable.Columns.Add(dc2);
            DataRow row2 = dataTable.NewRow();
            row2[dc2] = "Testing";
            dataTable.Rows.Add(row2);

        }
        public override bool Equals(object obj)
        {
            SerPublicDatasetPrivate result = (SerPublicDatasetPrivate)obj;

            if (Util.CompareDataSets(this.dataSet, result.dataSet))
            {
                if (Util.CompareDataTable(this.dataTable, result.dataTable))
                {
                    return true;
                }
            }
            return false;
        }




    }
#pragma warning restore 0659

    [DataContract(IsReference = true)]
    class DCPrivateDatasetPublic
    {
        [DataMember]
        public DataSet dataSet;

        [DataMember]
        public DataTable dataTable;

        public DCPrivateDatasetPublic()
        { }
        public DCPrivateDatasetPublic(bool init)
        {
            dataSet = new DataSet("MyData");
            dataTable = new DataTable("MyTable");
            DataColumn dc1 = new DataColumn("Data", typeof(string));
            dataTable.Columns.Add(dc1);
            DataRow row1 = dataTable.NewRow();
            row1[dc1] = DateTime.Now.ToLongTimeString();
            dataTable.Rows.Add(row1);
            dataSet.Tables.Add(dataTable);
        }
    }

    [DataContract(IsReference = true)]
    class DCPrivateDatasetPrivate
    {
        [DataMember]
        DataSet dataSet;

        [DataMember]
        public DataTable dataTable;

        public DCPrivateDatasetPrivate()
        {
        }

        public DCPrivateDatasetPrivate(bool init)
        {

            dataSet = new DataSet("MyData");
            dataTable = new DataTable("MyTable");
            DataColumn dc1 = new DataColumn("Data", typeof(string));
            dataTable.Columns.Add(dc1);
            DataRow row1 = dataTable.NewRow();
            row1[dc1] = DateTime.Now.ToLongTimeString();
            dataTable.Rows.Add(row1);
            dataSet.Tables.Add(dataTable);
        }
    }

    [Serializable]
    class SerPrivateDatasetPublic
    {
        public DataSet dataSet;

        [DataMember]
        public DataTable dataTable;

        public SerPrivateDatasetPublic()
        {
        }

        public SerPrivateDatasetPublic(bool init)
        {
            dataSet = new DataSet("MyData");
            dataTable = new DataTable("MyTable");
            DataColumn dc1 = new DataColumn("Data", typeof(string));
            dataTable.Columns.Add(dc1);
            DataRow row1 = dataTable.NewRow();
            row1[dc1] = DateTime.Now.ToLongTimeString();
            dataTable.Rows.Add(row1);
            dataSet.Tables.Add(dataTable);
        }
    }


    [Serializable]
    class SerPrivateDatasetPrivate
    {
        DataSet dataSet;

        [DataMember]
        public DataTable dataTable;

        public SerPrivateDatasetPrivate()
        {
        }

        public SerPrivateDatasetPrivate(bool init)
        {
            dataSet = new DataSet("MyData");
            dataTable = new DataTable("MyTable");
            DataColumn dc1 = new DataColumn("Data", typeof(string));
            dataTable.Columns.Add(dc1);
            DataRow row1 = dataTable.NewRow();
            row1[dc1] = DateTime.Now.ToLongTimeString();
            dataTable.Rows.Add(row1);
            dataSet.Tables.Add(dataTable);
        }
    }

    #endregion

    [System.Runtime.Serialization.DataContract(IsReference = true)]
    public class CustomGeneric2<T> where T : new()
    {
        [System.Runtime.Serialization.DataMember]
        public string Data = "dada";

        public T t = new T();
    }

    [DataContract(IsReference = true)]
    [KnownType(typeof(DateTimeOffset))]
    [KnownType(typeof(MyEnum1))]
    public class DTOContainer
    {
        [DataMember]
        [IgnoreMember]
        public Enum enumBase1 = MyEnum1.red;

        [DataMember]
        public Array array1 = new object[] { new object(), DateTimeOffset.Now, new object() };

        [DataMember]
        public ValueType valType = DateTimeOffset.Now;

        [DataMember]
        public Nullable<DateTimeOffset> nDTO = DateTimeOffset.MaxValue;

        [DataMember]
        public List<DateTimeOffset> lDTO = new List<DateTimeOffset>();

        [DataMember]
        public Dictionary<DateTimeOffset, DateTimeOffset> dictDTO = new Dictionary<DateTimeOffset, DateTimeOffset>();

        [DataMember]
        public DateTimeOffset[] arrayDTO;

        public DTOContainer() { }
        public DTOContainer(bool init)
        {
            lDTO.Add(DateTimeOffset.MaxValue);
            lDTO.Add(DateTimeOffset.MinValue);
            dictDTO.Add(DateTimeOffset.Now, DateTimeOffset.MaxValue);
            dictDTO.Add(DateTimeOffset.MinValue, DateTimeOffset.Now);

            arrayDTO = new DateTimeOffset[] { DateTimeOffset.Now, DateTimeOffset.UtcNow };
        }
    }

}
/*******************
namespace Test.WCF.DCS.Delegates
{
    using MiscSamples;
    using System.Runtime.Serialization;

    public delegate void PublicVoidDelegate();
    public delegate void PublicVoidDelegateContainsPublicDC(PublicDC publicDC);
    delegate void PublicVoidDelegateContainsPrivateDC(PrivateDC privateDC);
    public delegate void PublicVoidDelegateContainsPublicDCClassPrivateDM(PublicDCClassPrivateDM privateDC);
    delegate void PublicVoidDelegateContainsPrivateDerived(PublicDCDerivedPrivate privateDC);
    delegate void PublicVoidDelegateContainsMixed(PrivateDC privateDC, PublicDC publicDC, PublicDCClassPrivateDM privateDM, PublicDCDerivedPrivate derivedPrivate);

    delegate void PrivateVoidDelegate();

    delegate PrivateDC PrivateDCDelegate();
    public delegate PublicDCClassPrivateDM PublicPublicDCClassPrivateDMDelegate();

    delegate PrivateDC PrivateDCDelegateContainsMixed(PrivateDC privateDC, PublicDC publicDC, PublicDCClassPrivateDM privateDM, PublicDCDerivedPrivate derivedPrivate);

    [DataContract(IsReference = true)]
    public class DelegateContainer_PublicVoidDelegate
    {
        [DataMember]
        public PublicVoidDelegate publicDelegate;

        public DelegateContainer_PublicVoidDelegate() { }

        public DelegateContainer_PublicVoidDelegate(bool init)
        {
            publicDelegate = new PublicVoidDelegate(PrivateMethod);
        }

        public void PublicMethod() { }

        void PrivateMethod() { }

    }
}
*/