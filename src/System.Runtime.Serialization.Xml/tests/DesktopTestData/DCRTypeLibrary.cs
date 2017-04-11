using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DesktopTestData
{
    [DataContract]
    public class ObjectContainer
    {
        [DataMember]
        private object data;

        [DataMember]
        private object data2;

        public ObjectContainer(object input)
        {
            data = input;
            data2 = data;
        }

        public object Data
        {
            get { return data; }
        }

        public object Data2
        {
            get { return data2; }
        }
    }

    [DataContract(Name = "EmptyDCType", Namespace = "http://www.Default.com")]
    public class EmptyDCType
    {
    }

    public class TypeNotFound { };

    [KnownType(typeof(EmptyDCType))]
    public class POCOObjectContainer
    {
        public POCOObjectContainer() { data = new EmptyDCType(); }

        public object data;

        [IgnoreDataMember]
        public object NonSerializedData;

        public POCOObjectContainer(object input)
        {
            data = input;
        }
    }

    public class TypeLibraryManager
    {
        List<Type> primitiveTypeList = new List<Type>();        // Also known as EnumsStructList
        List<Type> collectionTypeList = new List<Type>();
        List<Type> selfRefAndCyclesTypeList = new List<Type>();
        List<Type> iobjectRefTypeList = new List<Type>();
        List<Type> sampleTypeList = new List<Type>();
        List<Type> isReferenceTypeList = new List<Type>();
        List<Type> allTypesList = new List<Type>();
        List<Type> fxPrimitivesInCollectionList = new List<Type>();

        Hashtable allTypeHashTable = new Hashtable();

        public Hashtable AllTypesHashtable
        {
            get
            {
                if (allTypeHashTable.Count == 0)
                {
                    allTypeHashTable = new Hashtable();
                    foreach (Type t in AllTypesList)
                    {
                        allTypeHashTable.Add(t.FullName, t);
                    }
                }
                return allTypeHashTable;
            }
        }

        #region List

        public List<Type> FxPrimitivesInCollectionList
        {
            get
            {
                if (fxPrimitivesInCollectionList.Count == 0)
                {
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Boolean>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Byte>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Byte[]>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Char>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.DateTime>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.DBNull>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Decimal>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Double>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Guid>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Int16>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Int32>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Int64>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Nullable<System.Boolean>>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Nullable<System.Byte>>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Nullable<System.Char>>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Nullable<System.DateTime>>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Nullable<System.Decimal>>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Nullable<System.Double>>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Nullable<System.Guid>>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Nullable<System.Int16>>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Nullable<System.Int32>>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Nullable<System.Int64>>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Nullable<System.SByte>>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Nullable<System.Single>>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Nullable<System.TimeSpan>>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Nullable<System.UInt16>>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Nullable<System.UInt32>>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Nullable<System.UInt64>>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.SByte>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Single>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.String>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.TimeSpan>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.UInt16>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.UInt32>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.UInt64>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Xml.XmlElement>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Xml.XmlNode[]>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.DateTimeOffset>));
                    fxPrimitivesInCollectionList.Add(typeof(List<System.Nullable<System.DateTimeOffset>>));
                }
                return fxPrimitivesInCollectionList;
            }
        }

        public List<Type> AllTypesList
        {
            get
            {
                if (allTypesList.Count == 0)
                {
                    allTypesList.AddRange(this.FxPrimitivesInCollectionList);
                    allTypesList.AddRange(this.EnumsStructList);
                    allTypesList.AddRange(this.SelfRefAndCyclesTypeList);
                    allTypesList.AddRange(this.IsReferenceTypeList);
                    allTypesList.AddRange(this.IObjectRefTypeList);
                    allTypesList.AddRange(this.CollectionsTypeList);
                    allTypesList.AddRange(this.SampleTypeList);
                }
                return allTypesList;
            }
        }

        public List<Type> IsReferenceTypeList
        {
            get
            {
                if (isReferenceTypeList.Count == 0)
                {
                    isReferenceTypeList.Add(typeof(TestInheritence9));
                    isReferenceTypeList.Add(typeof(SimpleDC));
                    isReferenceTypeList.Add(typeof(SimpleDCWithSimpleDMRef));
                    isReferenceTypeList.Add(typeof(SimpleDCWithRef));
                    isReferenceTypeList.Add(typeof(ContainsSimpleDCWithRef));
                    isReferenceTypeList.Add(typeof(SimpleDCWithIsRequiredFalse));
                    isReferenceTypeList.Add(typeof(Mixed1));
                    isReferenceTypeList.Add(typeof(SerIser));
                    isReferenceTypeList.Add(typeof(DCVersioned1));
                    isReferenceTypeList.Add(typeof(DCVersioned2));
                    isReferenceTypeList.Add(typeof(DCVersionedContainer1));
                    isReferenceTypeList.Add(typeof(DCVersionedContainerVersion1));
                    isReferenceTypeList.Add(typeof(DCVersionedContainerVersion2));
                    isReferenceTypeList.Add(typeof(DCVersionedContainerVersion3));
                    isReferenceTypeList.Add(typeof(BaseDC));
                    isReferenceTypeList.Add(typeof(BaseSerializable));
                    isReferenceTypeList.Add(typeof(DerivedDC));
                    isReferenceTypeList.Add(typeof(DerivedSerializable));
                    isReferenceTypeList.Add(typeof(DerivedDCIsRefBaseSerializable));
                    isReferenceTypeList.Add(typeof(DerivedDCBaseSerializable));
                    isReferenceTypeList.Add(typeof(Derived2DC));
                    isReferenceTypeList.Add(typeof(BaseDCNoIsRef));
                    isReferenceTypeList.Add(typeof(DerivedPOCOBaseDCNOISRef));
                    isReferenceTypeList.Add(typeof(DerivedIXmlSerializable_POCOBaseDCNOISRef));
                    isReferenceTypeList.Add(typeof(DerivedCDCFromBaseDC));
                    isReferenceTypeList.Add(typeof(Derived2Serializable));
                    isReferenceTypeList.Add(typeof(Derived2SerializablePositive));
                    isReferenceTypeList.Add(typeof(Derived2Derived2Serializable));
                    isReferenceTypeList.Add(typeof(Derived3Derived2Serializable));
                    isReferenceTypeList.Add(typeof(Derived31Derived2SerializablePOCO));
                    isReferenceTypeList.Add(typeof(Derived4Derived2Serializable));
                    isReferenceTypeList.Add(typeof(Derived5Derived2Serializable));
                    isReferenceTypeList.Add(typeof(Derived6Derived2SerializablePOCO));
                    isReferenceTypeList.Add(typeof(BaseWithIsRefTrue));
                    isReferenceTypeList.Add(typeof(DerivedNoIsRef));
                    isReferenceTypeList.Add(typeof(DerivedNoIsRef2));
                    isReferenceTypeList.Add(typeof(DerivedNoIsRef3));
                    isReferenceTypeList.Add(typeof(DerivedNoIsRef4));
                    isReferenceTypeList.Add(typeof(DerivedNoIsRef5));
                    isReferenceTypeList.Add(typeof(DerivedNoIsRefWithIsRefTrue6));
                    isReferenceTypeList.Add(typeof(DerivedWithIsRefFalse));
                    isReferenceTypeList.Add(typeof(DerivedWithIsRefFalse2));
                    isReferenceTypeList.Add(typeof(DerivedWithIsRefFalse3));
                    isReferenceTypeList.Add(typeof(DerivedWithIsRefFalse4));
                    isReferenceTypeList.Add(typeof(DerivedWithIsRefFalse5));
                    isReferenceTypeList.Add(typeof(DerivedWithIsRefTrue6));
                    isReferenceTypeList.Add(typeof(DerivedWithIsRefTrueExplicit));
                    isReferenceTypeList.Add(typeof(DerivedWithIsRefTrueExplicit2));
                    isReferenceTypeList.Add(typeof(BaseNoIsRef));
                    isReferenceTypeList.Add(typeof(DerivedWithIsRefFalseExplicit));
                    isReferenceTypeList.Add(typeof(TestInheritence));
                    isReferenceTypeList.Add(typeof(TestInheritence91));
                    isReferenceTypeList.Add(typeof(TestInheritence5));
                    isReferenceTypeList.Add(typeof(TestInheritence10));
                    isReferenceTypeList.Add(typeof(TestInheritence2));
                    isReferenceTypeList.Add(typeof(TestInheritence11));
                    isReferenceTypeList.Add(typeof(TestInheritence3));
                    isReferenceTypeList.Add(typeof(TestInheritence16));
                    isReferenceTypeList.Add(typeof(TestInheritence4));
                    isReferenceTypeList.Add(typeof(TestInheritence12));
                    isReferenceTypeList.Add(typeof(TestInheritence6));
                    isReferenceTypeList.Add(typeof(TestInheritence7));
                    isReferenceTypeList.Add(typeof(TestInheritence14));
                    isReferenceTypeList.Add(typeof(TestInheritence8));
                }
                return isReferenceTypeList;
            }
        }

        public List<Type> SelfRefAndCyclesTypeList
        {
            get
            {
                if (selfRefAndCyclesTypeList.Count == 0)
                {
                    selfRefAndCyclesTypeList.Add(typeof(SelfRef1));
                    selfRefAndCyclesTypeList.Add(typeof(SelfRef1DoubleDM));
                    selfRefAndCyclesTypeList.Add(typeof(SelfRef2));
                    selfRefAndCyclesTypeList.Add(typeof(SelfRef3));
                    selfRefAndCyclesTypeList.Add(typeof(Cyclic1));
                    selfRefAndCyclesTypeList.Add(typeof(Cyclic2));
                    selfRefAndCyclesTypeList.Add(typeof(CyclicA));
                    selfRefAndCyclesTypeList.Add(typeof(CyclicB));
                    selfRefAndCyclesTypeList.Add(typeof(CyclicC));
                    selfRefAndCyclesTypeList.Add(typeof(CyclicD));
                    selfRefAndCyclesTypeList.Add(typeof(CyclicABCD1));
                    selfRefAndCyclesTypeList.Add(typeof(CyclicABCD2));
                    selfRefAndCyclesTypeList.Add(typeof(CyclicABCD3));
                    selfRefAndCyclesTypeList.Add(typeof(CyclicABCD4));
                    selfRefAndCyclesTypeList.Add(typeof(CyclicABCD5));
                    selfRefAndCyclesTypeList.Add(typeof(CyclicABCD6));
                    selfRefAndCyclesTypeList.Add(typeof(CyclicABCD7));
                    selfRefAndCyclesTypeList.Add(typeof(CyclicABCD8));
                    selfRefAndCyclesTypeList.Add(typeof(CyclicABCDNoCycles));
                    selfRefAndCyclesTypeList.Add(typeof(A1));
                    selfRefAndCyclesTypeList.Add(typeof(B1));
                    selfRefAndCyclesTypeList.Add(typeof(C1));
                    selfRefAndCyclesTypeList.Add(typeof(BB1));
                    selfRefAndCyclesTypeList.Add(typeof(BBB1));
                }
                return selfRefAndCyclesTypeList;
            }
        }

        public List<Type> IObjectRefTypeList
        {
            get
            {
                if (iobjectRefTypeList.Count == 0)
                {
                    iobjectRefTypeList.Add(typeof(DCExplicitInterfaceIObjRef));
                    iobjectRefTypeList.Add(typeof(DCIObjRef));
                    iobjectRefTypeList.Add(typeof(SerExplicitInterfaceIObjRefReturnsPrivate));
                    iobjectRefTypeList.Add(typeof(SerIObjRefReturnsPrivate));
                    iobjectRefTypeList.Add(typeof(DCExplicitInterfaceIObjRefReturnsPrivate));
                    iobjectRefTypeList.Add(typeof(DCIObjRefReturnsPrivate));
                }
                return iobjectRefTypeList;
            }
        }

        public List<Type> EnumsStructList
        {
            get
            {
                if (primitiveTypeList.Count == 0)
                {
                    primitiveTypeList.Add(typeof(Person));
                    primitiveTypeList.Add(typeof(CharClass));
                    primitiveTypeList.Add(typeof(AllTypes));
                    primitiveTypeList.Add(typeof(AllTypes2));
                    primitiveTypeList.Add(typeof(DictContainer));
                    primitiveTypeList.Add(typeof(ListContainer));
                    primitiveTypeList.Add(typeof(ArrayContainer));
                    primitiveTypeList.Add(typeof(EnumContainer1));
                    primitiveTypeList.Add(typeof(EnumContainer2));
                    primitiveTypeList.Add(typeof(EnumContainer3));
                    primitiveTypeList.Add(typeof(WithStatic));
                    primitiveTypeList.Add(typeof(DerivedFromPriC));
                    primitiveTypeList.Add(typeof(EmptyDC));
                    primitiveTypeList.Add(typeof(Base));
                    primitiveTypeList.Add(typeof(Derived));
                    primitiveTypeList.Add(typeof(list));
                    primitiveTypeList.Add(typeof(Arrays));
                    primitiveTypeList.Add(typeof(Array3));
                    primitiveTypeList.Add(typeof(Properties));
                    primitiveTypeList.Add(typeof(HaveNS));
                    primitiveTypeList.Add(typeof(OutClass));
                    primitiveTypeList.Add(typeof(Temp));
                    primitiveTypeList.Add(typeof(Array22));
                    primitiveTypeList.Add(typeof(Person2));
                    primitiveTypeList.Add(typeof(BoxedPrim));
                    primitiveTypeList.Add(typeof(MyEnum));
                    primitiveTypeList.Add(typeof(MyPrivateEnum1));
                    primitiveTypeList.Add(typeof(MyPrivateEnum2));
                    primitiveTypeList.Add(typeof(MyPrivateEnum3));
                    primitiveTypeList.Add(typeof(MyEnum1));
                    primitiveTypeList.Add(typeof(MyEnum2));
                    primitiveTypeList.Add(typeof(MyEnum3));
                    primitiveTypeList.Add(typeof(MyEnum4));
                    primitiveTypeList.Add(typeof(MyEnum7));
                    primitiveTypeList.Add(typeof(MyEnum8));
                    primitiveTypeList.Add(typeof(SeasonsEnumContainer));
                }
                return primitiveTypeList;
            }
        }

        public List<Type> CollectionsTypeList
        {
            get
            {
                if (collectionTypeList.Count == 0)
                {
                    collectionTypeList = new List<Type>();
                    collectionTypeList.Add(typeof(ContainsLinkedList));
                    collectionTypeList.Add(typeof(SimpleCDC));
                    collectionTypeList.Add(typeof(SimpleCDC2));
                    collectionTypeList.Add(typeof(ContainsSimpleCDC));
                    collectionTypeList.Add(typeof(DMInCollection1));
                    collectionTypeList.Add(typeof(DMInCollection2));
                    collectionTypeList.Add(typeof(DMInDict1));
                    collectionTypeList.Add(typeof(DMWithRefInCollection1));
                    collectionTypeList.Add(typeof(DMWithRefInCollection2));
                    collectionTypeList.Add(typeof(DMWithRefInDict1));
                }
                return collectionTypeList;
            }
        }

        public List<Type> SampleTypeList
        {
            get
            {
                if (sampleTypeList.Count == 0)
                {
                    sampleTypeList = new List<Type>();
                    sampleTypeList.Add(typeof(TypeNotFound));
                    sampleTypeList.Add(typeof(EmptyDCType));
                    sampleTypeList.Add(typeof(ObjectContainer));
                    sampleTypeList.Add(typeof(POCOObjectContainer));
                    sampleTypeList.Add(typeof(CircularLink));
                    sampleTypeList.Add(typeof(CircularLinkDerived));
                    sampleTypeList.Add(typeof(KT1Base));
                    sampleTypeList.Add(typeof(KT1Derived));
                    sampleTypeList.Add(typeof(KT2Base));
                    sampleTypeList.Add(typeof(KT3BaseKTMReturnsPrivateType));
                    sampleTypeList.Add(typeof(KT2Derived));
                    sampleTypeList.Add(typeof(CB1));
                    sampleTypeList.Add(typeof(ArrayListWithCDCFilledPublicTypes));
                    sampleTypeList.Add(typeof(ArrayListWithCDCFilledWithMixedTypes));
                    sampleTypeList.Add(typeof(CollectionBaseWithCDCFilledPublicTypes));
                    sampleTypeList.Add(typeof(CollectionBaseWithCDCFilledWithMixedTypes));
                    sampleTypeList.Add(typeof(DCHashtableContainerPublic));
                    sampleTypeList.Add(typeof(DCHashtableContainerMixedTypes));
                    sampleTypeList.Add(typeof(CustomGenericContainerPrivateType1));
                    sampleTypeList.Add(typeof(CustomGenericContainerPrivateType2));
                    sampleTypeList.Add(typeof(CustomGenericContainerPrivateType3));
                    sampleTypeList.Add(typeof(CustomGenericContainerPrivateType4));
                    sampleTypeList.Add(typeof(CustomGenericContainerPublicType1));
                    sampleTypeList.Add(typeof(CustomGenericContainerPublicType2));
                    sampleTypeList.Add(typeof(CustomGenericContainerPublicType3));
                    sampleTypeList.Add(typeof(CustomGenericContainerPublicType4));
                    sampleTypeList.Add(typeof(CustomGeneric1<KT1Base>));
                    sampleTypeList.Add(typeof(CustomGeneric2<KT1Base, NonDCPerson>));
                    sampleTypeList.Add(typeof(GenericContainer));
                    sampleTypeList.Add(typeof(GenericBase<NonDCPerson>));
                    sampleTypeList.Add(typeof(GenericBase2<KT1Base, NonDCPerson>));
                    sampleTypeList.Add(typeof(SimpleBase));
                    sampleTypeList.Add(typeof(SimpleBaseDerived));
                    sampleTypeList.Add(typeof(SimpleBaseDerived2));
                    sampleTypeList.Add(typeof(SimpleBaseContainer));
                    sampleTypeList.Add(typeof(DCListPrivateTContainer2));
                    sampleTypeList.Add(typeof(DCListPrivateTContainer));
                    sampleTypeList.Add(typeof(DCListPublicTContainer));
                    sampleTypeList.Add(typeof(DCListMixedTContainer));
                    sampleTypeList.Add(typeof(SampleListImplicitWithDC));
                    sampleTypeList.Add(typeof(SampleListImplicitWithoutDC));
                    sampleTypeList.Add(typeof(SampleListImplicitWithCDC));
                    sampleTypeList.Add(typeof(SampleListExplicitWithDC));
                    sampleTypeList.Add(typeof(SampleListExplicitWithoutDC));
                    sampleTypeList.Add(typeof(SampleListExplicitWithCDC));
                    sampleTypeList.Add(typeof(SampleListExplicitWithCDCContainsPrivateDC));
                    sampleTypeList.Add(typeof(SampleListTImplicitWithDC));
                    sampleTypeList.Add(typeof(SampleListTImplicitWithoutDC));
                    sampleTypeList.Add(typeof(SampleListTImplicitWithCDC));
                    sampleTypeList.Add(typeof(SampleListTExplicitWithDC));
                    sampleTypeList.Add(typeof(SampleListTExplicitWithoutDC));
                    sampleTypeList.Add(typeof(SampleListTExplicitWithCDC));
                    sampleTypeList.Add(typeof(SampleListTExplicitWithCDCContainsPublicDCClassPrivateDM));
                    sampleTypeList.Add(typeof(SampleListTExplicitWithCDCContainsPrivateDC));
                    sampleTypeList.Add(typeof(SampleICollectionTImplicitWithDC));
                    sampleTypeList.Add(typeof(SampleICollectionTImplicitWithoutDC));
                    sampleTypeList.Add(typeof(SampleICollectionTImplicitWithCDC));
                    sampleTypeList.Add(typeof(SampleICollectionTExplicitWithDC));
                    sampleTypeList.Add(typeof(SampleICollectionTExplicitWithoutDC));
                    sampleTypeList.Add(typeof(SampleICollectionTExplicitWithCDC));
                    sampleTypeList.Add(typeof(SampleICollectionTExplicitWithCDCContainsPrivateDC));
                    sampleTypeList.Add(typeof(SampleIEnumerableTImplicitWithDC));
                    sampleTypeList.Add(typeof(SampleIEnumerableTImplicitWithoutDC));
                    sampleTypeList.Add(typeof(SampleIEnumerableTImplicitWithCDC));
                    sampleTypeList.Add(typeof(SampleIEnumerableTExplicitWithDC));
                    sampleTypeList.Add(typeof(SampleIEnumerableTExplicitWithoutDC));
                    sampleTypeList.Add(typeof(SampleIEnumerableTExplicitWithCDC));
                    sampleTypeList.Add(typeof(SampleIEnumerableTExplicitWithCDCContainsPrivateDC));
                    sampleTypeList.Add(typeof(SampleICollectionImplicitWithDC));
                    sampleTypeList.Add(typeof(SampleICollectionImplicitWithoutDC));
                    sampleTypeList.Add(typeof(SampleICollectionImplicitWithCDC));
                    sampleTypeList.Add(typeof(SampleICollectionExplicitWithDC));
                    sampleTypeList.Add(typeof(SampleICollectionExplicitWithoutDC));
                    sampleTypeList.Add(typeof(SampleICollectionExplicitWithCDC));
                    sampleTypeList.Add(typeof(SampleICollectionExplicitWithCDCContainsPrivateDC));
                    sampleTypeList.Add(typeof(SampleIEnumerableImplicitWithDC));
                    sampleTypeList.Add(typeof(SampleIEnumerableImplicitWithoutDC));
                    sampleTypeList.Add(typeof(SampleIEnumerableImplicitWithCDC));
                    sampleTypeList.Add(typeof(SampleIEnumerableExplicitWithDC));
                    sampleTypeList.Add(typeof(SampleIEnumerableExplicitWithoutDC));
                    sampleTypeList.Add(typeof(SampleIEnumerableExplicitWithCDC));
                    sampleTypeList.Add(typeof(SampleIEnumerableExplicitWithCDCContainsPrivateDC));
                    sampleTypeList.Add(typeof(MyIDictionaryContainsPublicDC));
                    sampleTypeList.Add(typeof(MyIDictionaryContainsPublicDCExplicit));
                    sampleTypeList.Add(typeof(MyIDictionaryContainsPrivateDC));
                    sampleTypeList.Add(typeof(MyGenericIDictionaryKVContainsPublicDC));
                    sampleTypeList.Add(typeof(MyGenericIDictionaryKVContainsPublicDCExplicit));
                    sampleTypeList.Add(typeof(MyGenericIDictionaryKVContainsPrivateDC));
                    sampleTypeList.Add(typeof(DCDictionaryPrivateKTContainer));
                    sampleTypeList.Add(typeof(DCDictionaryPublicKTContainer));
                    sampleTypeList.Add(typeof(DCDictionaryMixedKTContainer1));
                    sampleTypeList.Add(typeof(DCDictionaryMixedKTContainer2));
                    sampleTypeList.Add(typeof(DCDictionaryMixedKTContainer3));
                    sampleTypeList.Add(typeof(DCDictionaryMixedKTContainer4));
                    sampleTypeList.Add(typeof(DCDictionaryMixedKTContainer5));
                    sampleTypeList.Add(typeof(DCDictionaryMixedKTContainer6));
                    sampleTypeList.Add(typeof(DCDictionaryMixedKTContainer7));
                    sampleTypeList.Add(typeof(DCDictionaryMixedKTContainer8));
                    sampleTypeList.Add(typeof(DCDictionaryMixedKTContainer9));
                    sampleTypeList.Add(typeof(DCDictionaryMixedKTContainer10));
                    sampleTypeList.Add(typeof(DCDictionaryMixedKTContainer11));
                    sampleTypeList.Add(typeof(DCDictionaryMixedKTContainer12));
                    sampleTypeList.Add(typeof(DCDictionaryMixedKTContainer13));
                    sampleTypeList.Add(typeof(DCDictionaryMixedKTContainer14));
                    sampleTypeList.Add(typeof(PublicDC));
                    sampleTypeList.Add(typeof(PublicDCDerivedPublic));
                    sampleTypeList.Add(typeof(DC));
                    sampleTypeList.Add(typeof(DCWithReadOnlyField));
                    sampleTypeList.Add(typeof(IReadWriteXmlWriteBinHex_EqualityDefined));
                    sampleTypeList.Add(typeof(PrivateDefaultCtorIXmlSerializables));
                    sampleTypeList.Add(typeof(PublicIXmlSerializablesWithPublicSchemaProvider));
                    sampleTypeList.Add(typeof(PublicExplicitIXmlSerializablesWithPublicSchemaProvider));
                    sampleTypeList.Add(typeof(PublicIXmlSerializablesWithPrivateSchemaProvider));
                    sampleTypeList.Add(typeof(PublicDCClassPublicDM));
                    sampleTypeList.Add(typeof(PublicDCClassPrivateDM));
                    sampleTypeList.Add(typeof(PublicDCClassInternalDM));
                    sampleTypeList.Add(typeof(PublicDCClassMixedDM));
                    sampleTypeList.Add(typeof(PublicDCClassPublicDM_DerivedDCClassPublic));
                    sampleTypeList.Add(typeof(PublicDCClassPrivateDM_DerivedDCClassPublic));
                    sampleTypeList.Add(typeof(PublicDCClassPublicDM_DerivedDCClassPublicContainsPrivateDM));
                    sampleTypeList.Add(typeof(Prop_PublicDCClassPublicDM_PublicDCClassPrivateDM));
                    sampleTypeList.Add(typeof(Prop_SetPrivate_PublicDCClassPublicDM_PublicDCClassPrivateDM));
                    sampleTypeList.Add(typeof(Prop_GetPrivate_PublicDCClassPublicDM_PublicDCClassPrivateDM));
                    sampleTypeList.Add(typeof(Prop_PublicDCClassPublicDM));
                    sampleTypeList.Add(typeof(Prop_PublicDCClassPrivateDM));
                    sampleTypeList.Add(typeof(Prop_PublicDCClassInternalDM));
                    sampleTypeList.Add(typeof(Prop_PublicDCClassMixedDM));
                    sampleTypeList.Add(typeof(Prop_PublicDCClassPublicDM_DerivedDCClassPublic));
                    sampleTypeList.Add(typeof(Prop_PublicDCClassPrivateDM_DerivedDCClassPublic));
                    sampleTypeList.Add(typeof(Prop_PublicDCClassPublicDM_DerivedDCClassPublicContainsPrivateDM));
                    sampleTypeList.Add(typeof(Prop_SetPrivate_PublicDCClassPublicDM));
                    sampleTypeList.Add(typeof(Prop_GetPrivate_PublicDCClassPublicDM));
                    sampleTypeList.Add(typeof(Derived_Override_Prop_All_Public));
                    sampleTypeList.Add(typeof(Derived_Override_Prop_Private));
                    sampleTypeList.Add(typeof(Derived_Override_Prop_GetPrivate_All_Public));
                    sampleTypeList.Add(typeof(Derived_Override_Prop_GetPrivate_Private));
                    sampleTypeList.Add(typeof(DC1_Version1));
                    sampleTypeList.Add(typeof(DC2_Version1));
                    sampleTypeList.Add(typeof(DC2_Version4));
                    sampleTypeList.Add(typeof(DC2_Version5));
                    sampleTypeList.Add(typeof(DC3_Version1));
                    sampleTypeList.Add(typeof(DC3_Version2));
                    sampleTypeList.Add(typeof(DC3_Version3));
                    sampleTypeList.Add(typeof(CallBackSample_OnSerializing_Public));
                    sampleTypeList.Add(typeof(CallBackSample_OnSerialized_Public));
                    sampleTypeList.Add(typeof(CallBackSample_OnDeserializing_Public));
                    sampleTypeList.Add(typeof(CallBackSample_OnDeserialized_Public));
                    sampleTypeList.Add(typeof(CallBackSample_OnSerializing));
                    sampleTypeList.Add(typeof(CallBackSample_OnSerialized));
                    sampleTypeList.Add(typeof(CallBackSample_OnDeserializing));
                    sampleTypeList.Add(typeof(CallBackSample_OnDeserialized));
                    sampleTypeList.Add(typeof(CallBackSample_IDeserializationCallback));
                    sampleTypeList.Add(typeof(CallBackSample_IDeserializationCallback_Explicit));
                    sampleTypeList.Add(typeof(CallBackSample_OnDeserialized_Private_Base));
                    sampleTypeList.Add(typeof(CallBackSample_OnDeserialized_Public_Derived));
                    sampleTypeList.Add(typeof(CDC_Possitive));
                    sampleTypeList.Add(typeof(CDC_PrivateAdd));
                    sampleTypeList.Add(typeof(Base_Possitive_VirtualAdd));
                    sampleTypeList.Add(typeof(CDC_NewAddToPrivate));
                    sampleTypeList.Add(typeof(CDC_PrivateDefaultCtor));
                    sampleTypeList.Add(typeof(NonDCPerson));
                    sampleTypeList.Add(typeof(PersonSurrogated));
                    //sampleTypeList.Add(typeof(DCSurrogate));
                    sampleTypeList.Add(typeof(SerSurrogate));
                    //sampleTypeList.Add(typeof(DCSurrogateExplicit));
                    sampleTypeList.Add(typeof(SerSurrogateExplicit));
                    //sampleTypeList.Add(typeof(DCSurrogateReturnPrivate));
                    sampleTypeList.Add(typeof(SerSurrogateReturnPrivate));
                    sampleTypeList.Add(typeof(NullableContainerContainsValue));
                    sampleTypeList.Add(typeof(NullableContainerContainsNull));
                    sampleTypeList.Add(typeof(NullablePrivateContainerContainsValue));
                    sampleTypeList.Add(typeof(NullablePrivateContainerContainsNull));
                    sampleTypeList.Add(typeof(NullablePrivateDataInDMContainerContainsValue));
                    sampleTypeList.Add(typeof(NullablePrivateDataInDMContainerContainsNull));
                    sampleTypeList.Add(typeof(DCPublicDatasetPublic));
                    sampleTypeList.Add(typeof(DCPublicDatasetPrivate));
                    sampleTypeList.Add(typeof(SerPublicDatasetPublic));
                    sampleTypeList.Add(typeof(SerPublicDatasetPrivate));
                    sampleTypeList.Add(typeof(CustomGeneric2<NonDCPerson>));
                    sampleTypeList.Add(typeof(DTOContainer));
                }
                return sampleTypeList;
            }
        }
        #endregion
    }


}
