[assembly:System.Security.AllowPartiallyTrustedCallers()]
[assembly:System.Security.SecurityTransparent()]
[assembly:System.Security.SecurityRules(System.Security.SecurityRuleSet.Level1)]
[assembly:System.Xml.Serialization.XmlSerializerVersionAttribute(ParentAssemblyId=@"579ae18f-2fe1-4e91-a75c-7fb07d000783,", Version=@"1.0.0.0")]
namespace Microsoft.Xml.Serialization.GeneratedAssembly {
//#pragma warning disable
    public class XmlSerializationWriter1 : System.Xml.Serialization.XmlSerializationWriter {

        public void Write94_TypeWithXmlElementProperty(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithXmlElementProperty", @"");
                return;
            }
            TopLevelElement();
            Write2_TypeWithXmlElementProperty(@"TypeWithXmlElementProperty", @"", ((global::TypeWithXmlElementProperty)o), true, false);
        }

        public void Write95_TypeWithXmlDocumentProperty(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithXmlDocumentProperty", @"");
                return;
            }
            TopLevelElement();
            Write3_TypeWithXmlDocumentProperty(@"TypeWithXmlDocumentProperty", @"", ((global::TypeWithXmlDocumentProperty)o), true, false);
        }

        public void Write96_TypeWithBinaryProperty(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithBinaryProperty", @"");
                return;
            }
            TopLevelElement();
            Write4_TypeWithBinaryProperty(@"TypeWithBinaryProperty", @"", ((global::TypeWithBinaryProperty)o), true, false);
        }

        public void Write97_TypeWithTimeSpanProperty(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithTimeSpanProperty", @"");
                return;
            }
            TopLevelElement();
            Write5_TypeWithTimeSpanProperty(@"TypeWithTimeSpanProperty", @"", ((global::TypeWithTimeSpanProperty)o), true, false);
        }

        public void Write98_Item(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithDefaultTimeSpanProperty", @"");
                return;
            }
            TopLevelElement();
            Write6_Item(@"TypeWithDefaultTimeSpanProperty", @"", ((global::TypeWithDefaultTimeSpanProperty)o), true, false);
        }

        public void Write99_TypeWithByteProperty(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithByteProperty", @"");
                return;
            }
            TopLevelElement();
            Write7_TypeWithByteProperty(@"TypeWithByteProperty", @"", ((global::TypeWithByteProperty)o), true, false);
        }

        public void Write100_TypeWithXmlNodeArrayProperty(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithXmlNodeArrayProperty", @"");
                return;
            }
            TopLevelElement();
            Write8_TypeWithXmlNodeArrayProperty(@"TypeWithXmlNodeArrayProperty", @"", ((global::TypeWithXmlNodeArrayProperty)o), true, false);
        }

        public void Write101_Animal(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"Animal", @"");
                return;
            }
            TopLevelElement();
            Write9_Animal(@"Animal", @"", ((global::Animal)o), true, false);
        }

        public void Write102_Dog(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"Dog", @"");
                return;
            }
            TopLevelElement();
            Write11_Dog(@"Dog", @"", ((global::Dog)o), true, false);
        }

        public void Write103_DogBreed(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteEmptyTag(@"DogBreed", @"");
                return;
            }
            WriteElementString(@"DogBreed", @"", Write10_DogBreed(((global::DogBreed)o)));
        }

        public void Write104_Group(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"Group", @"");
                return;
            }
            TopLevelElement();
            Write13_Group(@"Group", @"", ((global::Group)o), true, false);
        }

        public void Write105_Vehicle(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"Vehicle", @"");
                return;
            }
            TopLevelElement();
            Write12_Vehicle(@"Vehicle", @"", ((global::Vehicle)o), true, false);
        }

        public void Write106_Employee(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"Employee", @"");
                return;
            }
            TopLevelElement();
            Write14_Employee(@"Employee", @"", ((global::Employee)o), true, false);
        }

        public void Write107_BaseClass(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"BaseClass", @"");
                return;
            }
            TopLevelElement();
            Write16_BaseClass(@"BaseClass", @"", ((global::BaseClass)o), true, false);
        }

        public void Write108_DerivedClass(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"DerivedClass", @"");
                return;
            }
            TopLevelElement();
            Write15_DerivedClass(@"DerivedClass", @"", ((global::DerivedClass)o), true, false);
        }

        public void Write109_PurchaseOrder(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteEmptyTag(@"PurchaseOrder", @"http://www.contoso1.com");
                return;
            }
            TopLevelElement();
            Write19_PurchaseOrder(@"PurchaseOrder", @"http://www.contoso1.com", ((global::PurchaseOrder)o), false, false);
        }

        public void Write110_Address(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"Address", @"");
                return;
            }
            TopLevelElement();
            Write20_Address(@"Address", @"", ((global::Address)o), true, false);
        }

        public void Write111_OrderedItem(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"OrderedItem", @"");
                return;
            }
            TopLevelElement();
            Write21_OrderedItem(@"OrderedItem", @"", ((global::OrderedItem)o), true, false);
        }

        public void Write112_AliasedTestType(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"AliasedTestType", @"");
                return;
            }
            TopLevelElement();
            Write22_AliasedTestType(@"AliasedTestType", @"", ((global::AliasedTestType)o), true, false);
        }

        public void Write113_BaseClass1(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"BaseClass1", @"");
                return;
            }
            TopLevelElement();
            Write23_BaseClass1(@"BaseClass1", @"", ((global::BaseClass1)o), true, false);
        }

        public void Write114_DerivedClass1(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"DerivedClass1", @"");
                return;
            }
            TopLevelElement();
            Write24_DerivedClass1(@"DerivedClass1", @"", ((global::DerivedClass1)o), true, false);
        }

        public void Write115_ArrayOfDateTime(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"ArrayOfDateTime", @"");
                return;
            }
            TopLevelElement();
            {
                global::MyCollection1 a = (global::MyCollection1)((global::MyCollection1)o);
                if ((object)(a) == null) {
                    WriteNullTagLiteral(@"ArrayOfDateTime", @"");
                }
                else {
                    WriteStartElement(@"ArrayOfDateTime", @"", null, false);
                    System.Collections.IEnumerator e = ((System.Collections.Generic.IEnumerable<global::System.DateTime>)a).GetEnumerator();
                    if (e != null)
                    while (e.MoveNext()) {
                        global::System.DateTime ai = (global::System.DateTime)e.Current;
                        WriteElementStringRaw(@"dateTime", @"", FromDateTime(((global::System.DateTime)ai)));
                    }
                    WriteEndElement();
                }
            }
        }

        public void Write116_Orchestra(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"Orchestra", @"");
                return;
            }
            TopLevelElement();
            Write26_Orchestra(@"Orchestra", @"", ((global::Orchestra)o), true, false);
        }

        public void Write117_Instrument(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"Instrument", @"");
                return;
            }
            TopLevelElement();
            Write25_Instrument(@"Instrument", @"", ((global::Instrument)o), true, false);
        }

        public void Write118_Brass(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"Brass", @"");
                return;
            }
            TopLevelElement();
            Write27_Brass(@"Brass", @"", ((global::Brass)o), true, false);
        }

        public void Write119_Trumpet(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"Trumpet", @"");
                return;
            }
            TopLevelElement();
            Write28_Trumpet(@"Trumpet", @"", ((global::Trumpet)o), true, false);
        }

        public void Write120_Pet(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"Pet", @"");
                return;
            }
            TopLevelElement();
            Write29_Pet(@"Pet", @"", ((global::Pet)o), true, false);
        }

        public void Write121_TypeWithDateTimeStringProperty(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithDateTimeStringProperty", @"");
                return;
            }
            TopLevelElement();
            Write30_TypeWithDateTimeStringProperty(@"TypeWithDateTimeStringProperty", @"", ((global::SerializationTypes.TypeWithDateTimeStringProperty)o), true, false);
        }

        public void Write122_SimpleType(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"SimpleType", @"");
                return;
            }
            TopLevelElement();
            Write31_SimpleType(@"SimpleType", @"", ((global::SerializationTypes.SimpleType)o), true, false);
        }

        public void Write123_TypeWithGetSetArrayMembers(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithGetSetArrayMembers", @"");
                return;
            }
            TopLevelElement();
            Write32_TypeWithGetSetArrayMembers(@"TypeWithGetSetArrayMembers", @"", ((global::SerializationTypes.TypeWithGetSetArrayMembers)o), true, false);
        }

        public void Write124_TypeWithGetOnlyArrayProperties(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithGetOnlyArrayProperties", @"");
                return;
            }
            TopLevelElement();
            Write33_TypeWithGetOnlyArrayProperties(@"TypeWithGetOnlyArrayProperties", @"", ((global::SerializationTypes.TypeWithGetOnlyArrayProperties)o), true, false);
        }

        public void Write125_StructNotSerializable(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteEmptyTag(@"StructNotSerializable", @"");
                return;
            }
            Write34_StructNotSerializable(@"StructNotSerializable", @"", ((global::SerializationTypes.StructNotSerializable)o), false);
        }

        public void Write126_TypeWithMyCollectionField(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithMyCollectionField", @"");
                return;
            }
            TopLevelElement();
            Write35_TypeWithMyCollectionField(@"TypeWithMyCollectionField", @"", ((global::SerializationTypes.TypeWithMyCollectionField)o), true, false);
        }

        public void Write127_Item(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithReadOnlyMyCollectionProperty", @"");
                return;
            }
            TopLevelElement();
            Write36_Item(@"TypeWithReadOnlyMyCollectionProperty", @"", ((global::SerializationTypes.TypeWithReadOnlyMyCollectionProperty)o), true, false);
        }

        public void Write128_ArrayOfAnyType(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"ArrayOfAnyType", @"");
                return;
            }
            TopLevelElement();
            {
                global::SerializationTypes.MyList a = (global::SerializationTypes.MyList)((global::SerializationTypes.MyList)o);
                if ((object)(a) == null) {
                    WriteNullTagLiteral(@"ArrayOfAnyType", @"");
                }
                else {
                    WriteStartElement(@"ArrayOfAnyType", @"", null, false);
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++) {
                        Write1_Object(@"anyType", @"", ((global::System.Object)a[ia]), true, false);
                    }
                    WriteEndElement();
                }
            }
        }

        public void Write129_MyEnum(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteEmptyTag(@"MyEnum", @"");
                return;
            }
            WriteElementString(@"MyEnum", @"", Write37_MyEnum(((global::SerializationTypes.MyEnum)o)));
        }

        public void Write130_TypeWithEnumMembers(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithEnumMembers", @"");
                return;
            }
            TopLevelElement();
            Write38_TypeWithEnumMembers(@"TypeWithEnumMembers", @"", ((global::SerializationTypes.TypeWithEnumMembers)o), true, false);
        }

        public void Write131_DCStruct(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteEmptyTag(@"DCStruct", @"");
                return;
            }
            Write39_DCStruct(@"DCStruct", @"", ((global::SerializationTypes.DCStruct)o), false);
        }

        public void Write132_DCClassWithEnumAndStruct(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"DCClassWithEnumAndStruct", @"");
                return;
            }
            TopLevelElement();
            Write40_DCClassWithEnumAndStruct(@"DCClassWithEnumAndStruct", @"", ((global::SerializationTypes.DCClassWithEnumAndStruct)o), true, false);
        }

        public void Write133_BuiltInTypes(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"BuiltInTypes", @"");
                return;
            }
            TopLevelElement();
            Write41_BuiltInTypes(@"BuiltInTypes", @"", ((global::SerializationTypes.BuiltInTypes)o), true, false);
        }

        public void Write134_TypeA(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeA", @"");
                return;
            }
            TopLevelElement();
            Write42_TypeA(@"TypeA", @"", ((global::SerializationTypes.TypeA)o), true, false);
        }

        public void Write135_TypeB(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeB", @"");
                return;
            }
            TopLevelElement();
            Write43_TypeB(@"TypeB", @"", ((global::SerializationTypes.TypeB)o), true, false);
        }

        public void Write136_TypeHasArrayOfASerializedAsB(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeHasArrayOfASerializedAsB", @"");
                return;
            }
            TopLevelElement();
            Write44_TypeHasArrayOfASerializedAsB(@"TypeHasArrayOfASerializedAsB", @"", ((global::SerializationTypes.TypeHasArrayOfASerializedAsB)o), true, false);
        }

        public void Write137_WithXElement(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"WithXElement", @"");
                return;
            }
            TopLevelElement();
            Write45_WithXElement(@"WithXElement", @"", ((global::SerializationTypes.WithXElement)o), true, false);
        }

        public void Write138_WithXElementWithNestedXElement(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"WithXElementWithNestedXElement", @"");
                return;
            }
            TopLevelElement();
            Write46_WithXElementWithNestedXElement(@"WithXElementWithNestedXElement", @"", ((global::SerializationTypes.WithXElementWithNestedXElement)o), true, false);
        }

        public void Write139_WithArrayOfXElement(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"WithArrayOfXElement", @"");
                return;
            }
            TopLevelElement();
            Write47_WithArrayOfXElement(@"WithArrayOfXElement", @"", ((global::SerializationTypes.WithArrayOfXElement)o), true, false);
        }

        public void Write140_WithListOfXElement(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"WithListOfXElement", @"");
                return;
            }
            TopLevelElement();
            Write48_WithListOfXElement(@"WithListOfXElement", @"", ((global::SerializationTypes.WithListOfXElement)o), true, false);
        }

        public void Write141_Item(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"__TypeNameWithSpecialCharacters漢ñ", @"");
                return;
            }
            TopLevelElement();
            Write49_Item(@"__TypeNameWithSpecialCharacters漢ñ", @"", ((global::SerializationTypes.@__TypeNameWithSpecialCharacters漢ñ)o), true, false);
        }

        public void Write142_BaseClassWithSamePropertyName(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"BaseClassWithSamePropertyName", @"");
                return;
            }
            TopLevelElement();
            Write50_BaseClassWithSamePropertyName(@"BaseClassWithSamePropertyName", @"", ((global::SerializationTypes.BaseClassWithSamePropertyName)o), true, false);
        }

        public void Write143_DerivedClassWithSameProperty(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"DerivedClassWithSameProperty", @"");
                return;
            }
            TopLevelElement();
            Write51_DerivedClassWithSameProperty(@"DerivedClassWithSameProperty", @"", ((global::SerializationTypes.DerivedClassWithSameProperty)o), true, false);
        }

        public void Write144_DerivedClassWithSameProperty2(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"DerivedClassWithSameProperty2", @"");
                return;
            }
            TopLevelElement();
            Write52_DerivedClassWithSameProperty2(@"DerivedClassWithSameProperty2", @"", ((global::SerializationTypes.DerivedClassWithSameProperty2)o), true, false);
        }

        public void Write145_Item(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithDateTimePropertyAsXmlTime", @"");
                return;
            }
            TopLevelElement();
            Write53_Item(@"TypeWithDateTimePropertyAsXmlTime", @"", ((global::SerializationTypes.TypeWithDateTimePropertyAsXmlTime)o), true, false);
        }

        public void Write146_TypeWithByteArrayAsXmlText(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithByteArrayAsXmlText", @"");
                return;
            }
            TopLevelElement();
            Write54_TypeWithByteArrayAsXmlText(@"TypeWithByteArrayAsXmlText", @"", ((global::SerializationTypes.TypeWithByteArrayAsXmlText)o), true, false);
        }

        public void Write147_SimpleDC(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"SimpleDC", @"");
                return;
            }
            TopLevelElement();
            Write55_SimpleDC(@"SimpleDC", @"", ((global::SerializationTypes.SimpleDC)o), true, false);
        }

        public void Write148_Item(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteEmptyTag(@"TypeWithXmlTextAttributeOnArray", @"http://schemas.xmlsoap.org/ws/2005/04/discovery");
                return;
            }
            TopLevelElement();
            Write56_Item(@"TypeWithXmlTextAttributeOnArray", @"http://schemas.xmlsoap.org/ws/2005/04/discovery", ((global::SerializationTypes.TypeWithXmlTextAttributeOnArray)o), false, false);
        }

        public void Write149_EnumFlags(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteEmptyTag(@"EnumFlags", @"");
                return;
            }
            WriteElementString(@"EnumFlags", @"", Write57_EnumFlags(((global::SerializationTypes.EnumFlags)o)));
        }

        public void Write150_ClassImplementsInterface(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"ClassImplementsInterface", @"");
                return;
            }
            TopLevelElement();
            Write58_ClassImplementsInterface(@"ClassImplementsInterface", @"", ((global::SerializationTypes.ClassImplementsInterface)o), true, false);
        }

        public void Write151_WithStruct(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"WithStruct", @"");
                return;
            }
            TopLevelElement();
            Write60_WithStruct(@"WithStruct", @"", ((global::SerializationTypes.WithStruct)o), true, false);
        }

        public void Write152_SomeStruct(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteEmptyTag(@"SomeStruct", @"");
                return;
            }
            Write59_SomeStruct(@"SomeStruct", @"", ((global::SerializationTypes.SomeStruct)o), false);
        }

        public void Write153_WithEnums(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"WithEnums", @"");
                return;
            }
            TopLevelElement();
            Write63_WithEnums(@"WithEnums", @"", ((global::SerializationTypes.WithEnums)o), true, false);
        }

        public void Write154_WithNullables(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"WithNullables", @"");
                return;
            }
            TopLevelElement();
            Write64_WithNullables(@"WithNullables", @"", ((global::SerializationTypes.WithNullables)o), true, false);
        }

        public void Write155_ByteEnum(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteEmptyTag(@"ByteEnum", @"");
                return;
            }
            WriteElementString(@"ByteEnum", @"", Write65_ByteEnum(((global::SerializationTypes.ByteEnum)o)));
        }

        public void Write156_SByteEnum(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteEmptyTag(@"SByteEnum", @"");
                return;
            }
            WriteElementString(@"SByteEnum", @"", Write66_SByteEnum(((global::SerializationTypes.SByteEnum)o)));
        }

        public void Write157_ShortEnum(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteEmptyTag(@"ShortEnum", @"");
                return;
            }
            WriteElementString(@"ShortEnum", @"", Write62_ShortEnum(((global::SerializationTypes.ShortEnum)o)));
        }

        public void Write158_IntEnum(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteEmptyTag(@"IntEnum", @"");
                return;
            }
            WriteElementString(@"IntEnum", @"", Write61_IntEnum(((global::SerializationTypes.IntEnum)o)));
        }

        public void Write159_UIntEnum(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteEmptyTag(@"UIntEnum", @"");
                return;
            }
            WriteElementString(@"UIntEnum", @"", Write67_UIntEnum(((global::SerializationTypes.UIntEnum)o)));
        }

        public void Write160_LongEnum(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteEmptyTag(@"LongEnum", @"");
                return;
            }
            WriteElementString(@"LongEnum", @"", Write68_LongEnum(((global::SerializationTypes.LongEnum)o)));
        }

        public void Write161_ULongEnum(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteEmptyTag(@"ULongEnum", @"");
                return;
            }
            WriteElementString(@"ULongEnum", @"", Write69_ULongEnum(((global::SerializationTypes.ULongEnum)o)));
        }

        public void Write162_AttributeTesting(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteEmptyTag(@"AttributeTesting", @"");
                return;
            }
            TopLevelElement();
            Write71_XmlSerializerAttributes(@"AttributeTesting", @"", ((global::SerializationTypes.XmlSerializerAttributes)o), false, false);
        }

        public void Write163_ItemChoiceType(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteEmptyTag(@"ItemChoiceType", @"");
                return;
            }
            WriteElementString(@"ItemChoiceType", @"", Write70_ItemChoiceType(((global::SerializationTypes.ItemChoiceType)o)));
        }

        public void Write164_TypeWithAnyAttribute(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithAnyAttribute", @"");
                return;
            }
            TopLevelElement();
            Write72_TypeWithAnyAttribute(@"TypeWithAnyAttribute", @"", ((global::SerializationTypes.TypeWithAnyAttribute)o), true, false);
        }

        public void Write165_KnownTypesThroughConstructor(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"KnownTypesThroughConstructor", @"");
                return;
            }
            TopLevelElement();
            Write73_KnownTypesThroughConstructor(@"KnownTypesThroughConstructor", @"", ((global::SerializationTypes.KnownTypesThroughConstructor)o), true, false);
        }

        public void Write166_SimpleKnownTypeValue(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"SimpleKnownTypeValue", @"");
                return;
            }
            TopLevelElement();
            Write74_SimpleKnownTypeValue(@"SimpleKnownTypeValue", @"", ((global::SerializationTypes.SimpleKnownTypeValue)o), true, false);
        }

        public void Write167_Item(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"ClassImplementingIXmlSerialiable", @"");
                return;
            }
            TopLevelElement();
            WriteSerializable((System.Xml.Serialization.IXmlSerializable)((global::SerializationTypes.ClassImplementingIXmlSerialiable)o), @"ClassImplementingIXmlSerialiable", @"", true, true);
        }

        public void Write168_TypeWithPropertyNameSpecified(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithPropertyNameSpecified", @"");
                return;
            }
            TopLevelElement();
            Write75_TypeWithPropertyNameSpecified(@"TypeWithPropertyNameSpecified", @"", ((global::SerializationTypes.TypeWithPropertyNameSpecified)o), true, false);
        }

        public void Write169_TypeWithXmlSchemaFormAttribute(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithXmlSchemaFormAttribute", @"");
                return;
            }
            TopLevelElement();
            Write76_TypeWithXmlSchemaFormAttribute(@"TypeWithXmlSchemaFormAttribute", @"", ((global::SerializationTypes.TypeWithXmlSchemaFormAttribute)o), true, false);
        }

        public void Write170_MyXmlType(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"MyXmlType", @"");
                return;
            }
            TopLevelElement();
            Write77_Item(@"MyXmlType", @"", ((global::SerializationTypes.TypeWithTypeNameInXmlTypeAttribute)o), true, false);
        }

        public void Write171_Item(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithSchemaFormInXmlAttribute", @"");
                return;
            }
            TopLevelElement();
            Write78_Item(@"TypeWithSchemaFormInXmlAttribute", @"", ((global::SerializationTypes.TypeWithSchemaFormInXmlAttribute)o), true, false);
        }

        public void Write172_Item(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithNonPublicDefaultConstructor", @"");
                return;
            }
            TopLevelElement();
            Write79_Item(@"TypeWithNonPublicDefaultConstructor", @"", ((global::SerializationTypes.TypeWithNonPublicDefaultConstructor)o), true, false);
        }

        public void Write173_ServerSettings(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"ServerSettings", @"");
                return;
            }
            TopLevelElement();
            Write80_ServerSettings(@"ServerSettings", @"", ((global::SerializationTypes.ServerSettings)o), true, false);
        }

        public void Write174_TypeWithXmlQualifiedName(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithXmlQualifiedName", @"");
                return;
            }
            TopLevelElement();
            Write81_TypeWithXmlQualifiedName(@"TypeWithXmlQualifiedName", @"", ((global::SerializationTypes.TypeWithXmlQualifiedName)o), true, false);
        }

        public void Write175_TypeWith2DArrayProperty2(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWith2DArrayProperty2", @"");
                return;
            }
            TopLevelElement();
            Write82_TypeWith2DArrayProperty2(@"TypeWith2DArrayProperty2", @"", ((global::SerializationTypes.TypeWith2DArrayProperty2)o), true, false);
        }

        public void Write176_Item(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithPropertiesHavingDefaultValue", @"");
                return;
            }
            TopLevelElement();
            Write83_Item(@"TypeWithPropertiesHavingDefaultValue", @"", ((global::SerializationTypes.TypeWithPropertiesHavingDefaultValue)o), true, false);
        }

        public void Write177_Item(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithEnumPropertyHavingDefaultValue", @"");
                return;
            }
            TopLevelElement();
            Write84_Item(@"TypeWithEnumPropertyHavingDefaultValue", @"", ((global::SerializationTypes.TypeWithEnumPropertyHavingDefaultValue)o), true, false);
        }

        public void Write178_Item(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithEnumFlagPropertyHavingDefaultValue", @"");
                return;
            }
            TopLevelElement();
            Write85_Item(@"TypeWithEnumFlagPropertyHavingDefaultValue", @"", ((global::SerializationTypes.TypeWithEnumFlagPropertyHavingDefaultValue)o), true, false);
        }

        public void Write179_TypeWithShouldSerializeMethod(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithShouldSerializeMethod", @"");
                return;
            }
            TopLevelElement();
            Write86_TypeWithShouldSerializeMethod(@"TypeWithShouldSerializeMethod", @"", ((global::SerializationTypes.TypeWithShouldSerializeMethod)o), true, false);
        }

        public void Write180_Item(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"KnownTypesThroughConstructorWithArrayProperties", @"");
                return;
            }
            TopLevelElement();
            Write87_Item(@"KnownTypesThroughConstructorWithArrayProperties", @"", ((global::SerializationTypes.KnownTypesThroughConstructorWithArrayProperties)o), true, false);
        }

        public void Write181_Item(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"KnownTypesThroughConstructorWithValue", @"");
                return;
            }
            TopLevelElement();
            Write88_Item(@"KnownTypesThroughConstructorWithValue", @"", ((global::SerializationTypes.KnownTypesThroughConstructorWithValue)o), true, false);
        }

        public void Write182_Item(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithTypesHavingCustomFormatter", @"");
                return;
            }
            TopLevelElement();
            Write89_Item(@"TypeWithTypesHavingCustomFormatter", @"", ((global::SerializationTypes.TypeWithTypesHavingCustomFormatter)o), true, false);
        }

        public void Write183_Item(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithArrayPropertyHavingChoice", @"");
                return;
            }
            TopLevelElement();
            Write91_Item(@"TypeWithArrayPropertyHavingChoice", @"", ((global::SerializationTypes.TypeWithArrayPropertyHavingChoice)o), true, false);
        }

        public void Write184_MoreChoices(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteEmptyTag(@"MoreChoices", @"");
                return;
            }
            WriteElementString(@"MoreChoices", @"", Write90_MoreChoices(((global::SerializationTypes.MoreChoices)o)));
        }

        public void Write185_TypeWithFieldsOrdered(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"TypeWithFieldsOrdered", @"");
                return;
            }
            TopLevelElement();
            Write92_TypeWithFieldsOrdered(@"TypeWithFieldsOrdered", @"", ((global::SerializationTypes.TypeWithFieldsOrdered)o), true, false);
        }

        public void Write186_Person(object o) {
            WriteStartDocument();
            if (o == null) {
                WriteNullTagLiteral(@"Person", @"");
                return;
            }
            TopLevelElement();
            Write93_Person(@"Person", @"", ((global::Outer.Person)o), true, false);
        }

        void Write93_Person(string n, string ns, global::Outer.Person o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::Outer.Person)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"Person", @"");
            WriteElementString(@"FirstName", @"", ((global::System.String)o.@FirstName));
            WriteElementString(@"MiddleName", @"", ((global::System.String)o.@MiddleName));
            WriteElementString(@"LastName", @"", ((global::System.String)o.@LastName));
            WriteEndElement(o);
        }

        void Write92_TypeWithFieldsOrdered(string n, string ns, global::SerializationTypes.TypeWithFieldsOrdered o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithFieldsOrdered)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithFieldsOrdered", @"");
            WriteElementStringRaw(@"IntField1", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@IntField1)));
            WriteElementStringRaw(@"IntField2", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@IntField2)));
            WriteElementString(@"StringField2", @"", ((global::System.String)o.@StringField2));
            WriteElementString(@"StringField1", @"", ((global::System.String)o.@StringField1));
            WriteEndElement(o);
        }

        string Write90_MoreChoices(global::SerializationTypes.MoreChoices v) {
            string s = null;
            switch (v) {
                case global::SerializationTypes.MoreChoices.@None: s = @"None"; break;
                case global::SerializationTypes.MoreChoices.@Item: s = @"Item"; break;
                case global::SerializationTypes.MoreChoices.@Amount: s = @"Amount"; break;
                default: throw CreateInvalidEnumValueException(((System.Int64)v).ToString(System.Globalization.CultureInfo.InvariantCulture), @"SerializationTypes.MoreChoices");
            }
            return s;
        }

        void Write91_Item(string n, string ns, global::SerializationTypes.TypeWithArrayPropertyHavingChoice o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithArrayPropertyHavingChoice)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithArrayPropertyHavingChoice", @"");
            {
                global::System.Object[] a = (global::System.Object[])o.@ManyChoices;
                if (a != null) {
                    global::SerializationTypes.MoreChoices[] c = (global::SerializationTypes.MoreChoices[])o.@ChoiceArray;
                    if (c == null || c.Length < a.Length) {
                        throw CreateInvalidChoiceIdentifierValueException(@"SerializationTypes.MoreChoices", @"ChoiceArray");}
                    for (int ia = 0; ia < a.Length; ia++) {
                        global::System.Object ai = (global::System.Object)a[ia];
                        global::SerializationTypes.MoreChoices ci = (global::SerializationTypes.MoreChoices)c[ia];
                        {
                            if (ci == SerializationTypes.MoreChoices.@Item && ((object)(ai) != null)) {
                                if (((object)ai) != null && !(ai is global::System.String)) throw CreateMismatchChoiceException(@"System.String", @"ChoiceArray", @"SerializationTypes.MoreChoices.@Item");
                                WriteElementString(@"Item", @"", ((global::System.String)ai));
                            }
                            else if (ci == SerializationTypes.MoreChoices.@Amount && ((object)(ai) != null)) {
                                if (((object)ai) != null && !(ai is global::System.Int32)) throw CreateMismatchChoiceException(@"System.Int32", @"ChoiceArray", @"SerializationTypes.MoreChoices.@Amount");
                                WriteElementStringRaw(@"Amount", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)ai)));
                            }
                            else  if ((object)(ai) != null){
                                throw CreateUnknownTypeException(ai);
                            }
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        void Write89_Item(string n, string ns, global::SerializationTypes.TypeWithTypesHavingCustomFormatter o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithTypesHavingCustomFormatter)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithTypesHavingCustomFormatter", @"");
            WriteElementStringRaw(@"DateTimeContent", @"", FromDateTime(((global::System.DateTime)o.@DateTimeContent)));
            WriteElementQualifiedName(@"QNameContent", @"", ((global::System.Xml.XmlQualifiedName)o.@QNameContent));
            WriteElementStringRaw(@"DateContent", @"", FromDate(((global::System.DateTime)o.@DateContent)));
            WriteElementString(@"NameContent", @"", FromXmlName(((global::System.String)o.@NameContent)));
            WriteElementString(@"NCNameContent", @"", FromXmlNCName(((global::System.String)o.@NCNameContent)));
            WriteElementString(@"NMTOKENContent", @"", FromXmlNmToken(((global::System.String)o.@NMTOKENContent)));
            WriteElementString(@"NMTOKENSContent", @"", FromXmlNmTokens(((global::System.String)o.@NMTOKENSContent)));
            WriteElementStringRaw(@"Base64BinaryContent", @"", FromByteArrayBase64(((global::System.Byte[])o.@Base64BinaryContent)));
            WriteElementStringRaw(@"HexBinaryContent", @"", FromByteArrayHex(((global::System.Byte[])o.@HexBinaryContent)));
            WriteEndElement(o);
        }

        void Write88_Item(string n, string ns, global::SerializationTypes.KnownTypesThroughConstructorWithValue o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.KnownTypesThroughConstructorWithValue)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"KnownTypesThroughConstructorWithValue", @"");
            Write1_Object(@"Value", @"", ((global::System.Object)o.@Value), false, false);
            WriteEndElement(o);
        }

        void Write1_Object(string n, string ns, global::System.Object o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::System.Object)) {
                }
                else if (t == typeof(global::Outer.Person)) {
                    Write93_Person(n, ns,(global::Outer.Person)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWithFieldsOrdered)) {
                    Write92_TypeWithFieldsOrdered(n, ns,(global::SerializationTypes.TypeWithFieldsOrdered)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWithArrayPropertyHavingChoice)) {
                    Write91_Item(n, ns,(global::SerializationTypes.TypeWithArrayPropertyHavingChoice)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWithTypesHavingCustomFormatter)) {
                    Write89_Item(n, ns,(global::SerializationTypes.TypeWithTypesHavingCustomFormatter)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.KnownTypesThroughConstructorWithValue)) {
                    Write88_Item(n, ns,(global::SerializationTypes.KnownTypesThroughConstructorWithValue)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.KnownTypesThroughConstructorWithArrayProperties)) {
                    Write87_Item(n, ns,(global::SerializationTypes.KnownTypesThroughConstructorWithArrayProperties)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWithShouldSerializeMethod)) {
                    Write86_TypeWithShouldSerializeMethod(n, ns,(global::SerializationTypes.TypeWithShouldSerializeMethod)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWithEnumFlagPropertyHavingDefaultValue)) {
                    Write85_Item(n, ns,(global::SerializationTypes.TypeWithEnumFlagPropertyHavingDefaultValue)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWithEnumPropertyHavingDefaultValue)) {
                    Write84_Item(n, ns,(global::SerializationTypes.TypeWithEnumPropertyHavingDefaultValue)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWithPropertiesHavingDefaultValue)) {
                    Write83_Item(n, ns,(global::SerializationTypes.TypeWithPropertiesHavingDefaultValue)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWith2DArrayProperty2)) {
                    Write82_TypeWith2DArrayProperty2(n, ns,(global::SerializationTypes.TypeWith2DArrayProperty2)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWithXmlQualifiedName)) {
                    Write81_TypeWithXmlQualifiedName(n, ns,(global::SerializationTypes.TypeWithXmlQualifiedName)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.ServerSettings)) {
                    Write80_ServerSettings(n, ns,(global::SerializationTypes.ServerSettings)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWithNonPublicDefaultConstructor)) {
                    Write79_Item(n, ns,(global::SerializationTypes.TypeWithNonPublicDefaultConstructor)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWithTypeNameInXmlTypeAttribute)) {
                    Write77_Item(n, ns,(global::SerializationTypes.TypeWithTypeNameInXmlTypeAttribute)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWithXmlSchemaFormAttribute)) {
                    Write76_TypeWithXmlSchemaFormAttribute(n, ns,(global::SerializationTypes.TypeWithXmlSchemaFormAttribute)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWithPropertyNameSpecified)) {
                    Write75_TypeWithPropertyNameSpecified(n, ns,(global::SerializationTypes.TypeWithPropertyNameSpecified)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.SimpleKnownTypeValue)) {
                    Write74_SimpleKnownTypeValue(n, ns,(global::SerializationTypes.SimpleKnownTypeValue)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.KnownTypesThroughConstructor)) {
                    Write73_KnownTypesThroughConstructor(n, ns,(global::SerializationTypes.KnownTypesThroughConstructor)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWithAnyAttribute)) {
                    Write72_TypeWithAnyAttribute(n, ns,(global::SerializationTypes.TypeWithAnyAttribute)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.XmlSerializerAttributes)) {
                    Write71_XmlSerializerAttributes(n, ns,(global::SerializationTypes.XmlSerializerAttributes)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.WithNullables)) {
                    Write64_WithNullables(n, ns,(global::SerializationTypes.WithNullables)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.WithEnums)) {
                    Write63_WithEnums(n, ns,(global::SerializationTypes.WithEnums)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.WithStruct)) {
                    Write60_WithStruct(n, ns,(global::SerializationTypes.WithStruct)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.SomeStruct)) {
                    Write59_SomeStruct(n, ns,(global::SerializationTypes.SomeStruct)o, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.ClassImplementsInterface)) {
                    Write58_ClassImplementsInterface(n, ns,(global::SerializationTypes.ClassImplementsInterface)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWithXmlTextAttributeOnArray)) {
                    Write56_Item(n, ns,(global::SerializationTypes.TypeWithXmlTextAttributeOnArray)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.SimpleDC)) {
                    Write55_SimpleDC(n, ns,(global::SerializationTypes.SimpleDC)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWithByteArrayAsXmlText)) {
                    Write54_TypeWithByteArrayAsXmlText(n, ns,(global::SerializationTypes.TypeWithByteArrayAsXmlText)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWithDateTimePropertyAsXmlTime)) {
                    Write53_Item(n, ns,(global::SerializationTypes.TypeWithDateTimePropertyAsXmlTime)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.BaseClassWithSamePropertyName)) {
                    Write50_BaseClassWithSamePropertyName(n, ns,(global::SerializationTypes.BaseClassWithSamePropertyName)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.DerivedClassWithSameProperty)) {
                    Write51_DerivedClassWithSameProperty(n, ns,(global::SerializationTypes.DerivedClassWithSameProperty)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.DerivedClassWithSameProperty2)) {
                    Write52_DerivedClassWithSameProperty2(n, ns,(global::SerializationTypes.DerivedClassWithSameProperty2)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.@__TypeNameWithSpecialCharacters漢ñ)) {
                    Write49_Item(n, ns,(global::SerializationTypes.@__TypeNameWithSpecialCharacters漢ñ)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.WithListOfXElement)) {
                    Write48_WithListOfXElement(n, ns,(global::SerializationTypes.WithListOfXElement)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.WithArrayOfXElement)) {
                    Write47_WithArrayOfXElement(n, ns,(global::SerializationTypes.WithArrayOfXElement)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.WithXElementWithNestedXElement)) {
                    Write46_WithXElementWithNestedXElement(n, ns,(global::SerializationTypes.WithXElementWithNestedXElement)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.WithXElement)) {
                    Write45_WithXElement(n, ns,(global::SerializationTypes.WithXElement)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeHasArrayOfASerializedAsB)) {
                    Write44_TypeHasArrayOfASerializedAsB(n, ns,(global::SerializationTypes.TypeHasArrayOfASerializedAsB)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeB)) {
                    Write43_TypeB(n, ns,(global::SerializationTypes.TypeB)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeA)) {
                    Write42_TypeA(n, ns,(global::SerializationTypes.TypeA)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.BuiltInTypes)) {
                    Write41_BuiltInTypes(n, ns,(global::SerializationTypes.BuiltInTypes)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.DCClassWithEnumAndStruct)) {
                    Write40_DCClassWithEnumAndStruct(n, ns,(global::SerializationTypes.DCClassWithEnumAndStruct)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.DCStruct)) {
                    Write39_DCStruct(n, ns,(global::SerializationTypes.DCStruct)o, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWithEnumMembers)) {
                    Write38_TypeWithEnumMembers(n, ns,(global::SerializationTypes.TypeWithEnumMembers)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWithReadOnlyMyCollectionProperty)) {
                    Write36_Item(n, ns,(global::SerializationTypes.TypeWithReadOnlyMyCollectionProperty)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWithMyCollectionField)) {
                    Write35_TypeWithMyCollectionField(n, ns,(global::SerializationTypes.TypeWithMyCollectionField)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.StructNotSerializable)) {
                    Write34_StructNotSerializable(n, ns,(global::SerializationTypes.StructNotSerializable)o, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWithGetOnlyArrayProperties)) {
                    Write33_TypeWithGetOnlyArrayProperties(n, ns,(global::SerializationTypes.TypeWithGetOnlyArrayProperties)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWithGetSetArrayMembers)) {
                    Write32_TypeWithGetSetArrayMembers(n, ns,(global::SerializationTypes.TypeWithGetSetArrayMembers)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.SimpleType)) {
                    Write31_SimpleType(n, ns,(global::SerializationTypes.SimpleType)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeWithDateTimeStringProperty)) {
                    Write30_TypeWithDateTimeStringProperty(n, ns,(global::SerializationTypes.TypeWithDateTimeStringProperty)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::Pet)) {
                    Write29_Pet(n, ns,(global::Pet)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::Orchestra)) {
                    Write26_Orchestra(n, ns,(global::Orchestra)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::Instrument)) {
                    Write25_Instrument(n, ns,(global::Instrument)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::Brass)) {
                    Write27_Brass(n, ns,(global::Brass)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::Trumpet)) {
                    Write28_Trumpet(n, ns,(global::Trumpet)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::BaseClass1)) {
                    Write23_BaseClass1(n, ns,(global::BaseClass1)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::DerivedClass1)) {
                    Write24_DerivedClass1(n, ns,(global::DerivedClass1)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::AliasedTestType)) {
                    Write22_AliasedTestType(n, ns,(global::AliasedTestType)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::OrderedItem)) {
                    Write21_OrderedItem(n, ns,(global::OrderedItem)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::Address)) {
                    Write20_Address(n, ns,(global::Address)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::PurchaseOrder)) {
                    Write19_PurchaseOrder(n, ns,(global::PurchaseOrder)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::OrderedItem)) {
                    Write18_OrderedItem(n, ns,(global::OrderedItem)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::Address)) {
                    Write17_Address(n, ns,(global::Address)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::BaseClass)) {
                    Write16_BaseClass(n, ns,(global::BaseClass)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::DerivedClass)) {
                    Write15_DerivedClass(n, ns,(global::DerivedClass)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::Employee)) {
                    Write14_Employee(n, ns,(global::Employee)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::Group)) {
                    Write13_Group(n, ns,(global::Group)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::Vehicle)) {
                    Write12_Vehicle(n, ns,(global::Vehicle)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::Animal)) {
                    Write9_Animal(n, ns,(global::Animal)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::Dog)) {
                    Write11_Dog(n, ns,(global::Dog)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::TypeWithXmlNodeArrayProperty)) {
                    Write8_TypeWithXmlNodeArrayProperty(n, ns,(global::TypeWithXmlNodeArrayProperty)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::TypeWithByteProperty)) {
                    Write7_TypeWithByteProperty(n, ns,(global::TypeWithByteProperty)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::TypeWithDefaultTimeSpanProperty)) {
                    Write6_Item(n, ns,(global::TypeWithDefaultTimeSpanProperty)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::TypeWithTimeSpanProperty)) {
                    Write5_TypeWithTimeSpanProperty(n, ns,(global::TypeWithTimeSpanProperty)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::TypeWithBinaryProperty)) {
                    Write4_TypeWithBinaryProperty(n, ns,(global::TypeWithBinaryProperty)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::TypeWithXmlDocumentProperty)) {
                    Write3_TypeWithXmlDocumentProperty(n, ns,(global::TypeWithXmlDocumentProperty)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::TypeWithXmlElementProperty)) {
                    Write2_TypeWithXmlElementProperty(n, ns,(global::TypeWithXmlElementProperty)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::DogBreed)) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"DogBreed", @"");
                    Writer.WriteString(Write10_DogBreed((global::DogBreed)o));
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::OrderedItem[])) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"ArrayOfOrderedItem", @"http://www.contoso1.com");
                    {
                        global::OrderedItem[] a = (global::OrderedItem[])o;
                        if (a != null) {
                            for (int ia = 0; ia < a.Length; ia++) {
                                Write18_OrderedItem(@"OrderedItem", @"http://www.contoso1.com", ((global::OrderedItem)a[ia]), true, false);
                            }
                        }
                    }
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::System.Collections.Generic.List<global::System.Int32>)) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"ArrayOfInt", @"");
                    {
                        global::System.Collections.Generic.List<global::System.Int32> a = (global::System.Collections.Generic.List<global::System.Int32>)o;
                        if (a != null) {
                            for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++) {
                                WriteElementStringRaw(@"int", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)a[ia])));
                            }
                        }
                    }
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::System.Collections.Generic.List<global::System.String>)) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"ArrayOfString", @"");
                    {
                        global::System.Collections.Generic.List<global::System.String> a = (global::System.Collections.Generic.List<global::System.String>)o;
                        if (a != null) {
                            for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++) {
                                WriteNullableStringLiteral(@"string", @"", ((global::System.String)a[ia]));
                            }
                        }
                    }
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::System.Collections.Generic.List<global::System.Double>)) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"ArrayOfDouble", @"");
                    {
                        global::System.Collections.Generic.List<global::System.Double> a = (global::System.Collections.Generic.List<global::System.Double>)o;
                        if (a != null) {
                            for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++) {
                                WriteElementStringRaw(@"double", @"", System.Xml.XmlConvert.ToString((global::System.Double)((global::System.Double)a[ia])));
                            }
                        }
                    }
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::MyCollection1)) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"ArrayOfDateTime", @"");
                    {
                        global::MyCollection1 a = (global::MyCollection1)o;
                        if (a != null) {
                            System.Collections.IEnumerator e = ((System.Collections.Generic.IEnumerable<global::System.DateTime>)a).GetEnumerator();
                            if (e != null)
                            while (e.MoveNext()) {
                                global::System.DateTime ai = (global::System.DateTime)e.Current;
                                WriteElementStringRaw(@"dateTime", @"", FromDateTime(((global::System.DateTime)ai)));
                            }
                        }
                    }
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::Instrument[])) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"ArrayOfInstrument", @"");
                    {
                        global::Instrument[] a = (global::Instrument[])o;
                        if (a != null) {
                            for (int ia = 0; ia < a.Length; ia++) {
                                Write25_Instrument(@"Instrument", @"", ((global::Instrument)a[ia]), true, false);
                            }
                        }
                    }
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::SerializationTypes.SimpleType[])) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"ArrayOfSimpleType", @"");
                    {
                        global::SerializationTypes.SimpleType[] a = (global::SerializationTypes.SimpleType[])o;
                        if (a != null) {
                            for (int ia = 0; ia < a.Length; ia++) {
                                Write31_SimpleType(@"SimpleType", @"", ((global::SerializationTypes.SimpleType)a[ia]), true, false);
                            }
                        }
                    }
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::SerializationTypes.MyList)) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"ArrayOfAnyType", @"");
                    {
                        global::SerializationTypes.MyList a = (global::SerializationTypes.MyList)o;
                        if (a != null) {
                            for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++) {
                                Write1_Object(@"anyType", @"", ((global::System.Object)a[ia]), true, false);
                            }
                        }
                    }
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::SerializationTypes.MyEnum)) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"MyEnum", @"");
                    Writer.WriteString(Write37_MyEnum((global::SerializationTypes.MyEnum)o));
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::SerializationTypes.TypeA[])) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"ArrayOfTypeA", @"");
                    {
                        global::SerializationTypes.TypeA[] a = (global::SerializationTypes.TypeA[])o;
                        if (a != null) {
                            for (int ia = 0; ia < a.Length; ia++) {
                                Write42_TypeA(@"TypeA", @"", ((global::SerializationTypes.TypeA)a[ia]), true, false);
                            }
                        }
                    }
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::System.Xml.Linq.XElement[])) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"ArrayOfXElement", @"");
                    {
                        global::System.Xml.Linq.XElement[] a = (global::System.Xml.Linq.XElement[])o;
                        if (a != null) {
                            for (int ia = 0; ia < a.Length; ia++) {
                                WriteSerializable((System.Xml.Serialization.IXmlSerializable)((global::System.Xml.Linq.XElement)a[ia]), @"XElement", @"", true, true);
                            }
                        }
                    }
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::SerializationTypes.EnumFlags)) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"EnumFlags", @"");
                    Writer.WriteString(Write57_EnumFlags((global::SerializationTypes.EnumFlags)o));
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::SerializationTypes.IntEnum)) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"IntEnum", @"");
                    Writer.WriteString(Write61_IntEnum((global::SerializationTypes.IntEnum)o));
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::SerializationTypes.ShortEnum)) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"ShortEnum", @"");
                    Writer.WriteString(Write62_ShortEnum((global::SerializationTypes.ShortEnum)o));
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::SerializationTypes.ByteEnum)) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"ByteEnum", @"");
                    Writer.WriteString(Write65_ByteEnum((global::SerializationTypes.ByteEnum)o));
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::SerializationTypes.SByteEnum)) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"SByteEnum", @"");
                    Writer.WriteString(Write66_SByteEnum((global::SerializationTypes.SByteEnum)o));
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::SerializationTypes.UIntEnum)) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"UIntEnum", @"");
                    Writer.WriteString(Write67_UIntEnum((global::SerializationTypes.UIntEnum)o));
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::SerializationTypes.LongEnum)) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"LongEnum", @"");
                    Writer.WriteString(Write68_LongEnum((global::SerializationTypes.LongEnum)o));
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::SerializationTypes.ULongEnum)) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"ULongEnum", @"");
                    Writer.WriteString(Write69_ULongEnum((global::SerializationTypes.ULongEnum)o));
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::SerializationTypes.ItemChoiceType)) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"ItemChoiceType", @"");
                    Writer.WriteString(Write70_ItemChoiceType((global::SerializationTypes.ItemChoiceType)o));
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::SerializationTypes.ItemChoiceType[])) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"ArrayOfItemChoiceType", @"");
                    {
                        global::SerializationTypes.ItemChoiceType[] a = (global::SerializationTypes.ItemChoiceType[])o;
                        if (a != null) {
                            for (int ia = 0; ia < a.Length; ia++) {
                                WriteElementString(@"ItemChoiceType", @"", Write70_ItemChoiceType(((global::SerializationTypes.ItemChoiceType)a[ia])));
                            }
                        }
                    }
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::System.Object[])) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"ArrayOfString", @"http://mynamespace");
                    {
                        global::System.Object[] a = (global::System.Object[])o;
                        if (a != null) {
                            for (int ia = 0; ia < a.Length; ia++) {
                                WriteNullableStringLiteral(@"string", @"http://mynamespace", ((global::System.String)a[ia]));
                            }
                        }
                    }
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::System.Collections.Generic.List<global::System.String>)) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"ArrayOfString1", @"");
                    {
                        global::System.Collections.Generic.List<global::System.String> a = (global::System.Collections.Generic.List<global::System.String>)o;
                        if (a != null) {
                            for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++) {
                                WriteElementString(@"NoneParameter", @"", ((global::System.String)a[ia]));
                            }
                        }
                    }
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::System.Collections.Generic.List<global::System.Boolean>)) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"ArrayOfBoolean", @"");
                    {
                        global::System.Collections.Generic.List<global::System.Boolean> a = (global::System.Collections.Generic.List<global::System.Boolean>)o;
                        if (a != null) {
                            for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++) {
                                WriteElementStringRaw(@"QualifiedParameter", @"", System.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)a[ia])));
                            }
                        }
                    }
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::SerializationTypes.SimpleType[][])) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"ArrayOfArrayOfSimpleType", @"");
                    {
                        global::SerializationTypes.SimpleType[] a = (global::SerializationTypes.SimpleType[])((global::SerializationTypes.SimpleType[])o);
                        if (a != null){
                            WriteStartElement(@"SimpleType", @"", null, false);
                            for (int ia = 0; ia < a.Length; ia++) {
                                Write31_SimpleType(@"SimpleType", @"", ((global::SerializationTypes.SimpleType)a[ia]), true, false);
                            }
                            WriteEndElement();
                        }
                    }
                    Writer.WriteEndElement();
                    return;
                }
                else if (t == typeof(global::SerializationTypes.MoreChoices)) {
                    Writer.WriteStartElement(n, ns);
                    WriteXsiType(@"MoreChoices", @"");
                    Writer.WriteString(Write90_MoreChoices((global::SerializationTypes.MoreChoices)o));
                    Writer.WriteEndElement();
                    return;
                }
                else {
                    WriteTypedPrimitive(n, ns, o, true);
                    return;
                }
            }
            WriteStartElement(n, ns, o, false, null);
            WriteEndElement(o);
        }

        void Write31_SimpleType(string n, string ns, global::SerializationTypes.SimpleType o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.SimpleType)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"SimpleType", @"");
            WriteElementString(@"P1", @"", ((global::System.String)o.@P1));
            WriteElementStringRaw(@"P2", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@P2)));
            WriteEndElement(o);
        }

        string Write70_ItemChoiceType(global::SerializationTypes.ItemChoiceType v) {
            string s = null;
            switch (v) {
                case global::SerializationTypes.ItemChoiceType.@None: s = @"None"; break;
                case global::SerializationTypes.ItemChoiceType.@Word: s = @"Word"; break;
                case global::SerializationTypes.ItemChoiceType.@Number: s = @"Number"; break;
                case global::SerializationTypes.ItemChoiceType.@DecimalNumber: s = @"DecimalNumber"; break;
                default: throw CreateInvalidEnumValueException(((System.Int64)v).ToString(System.Globalization.CultureInfo.InvariantCulture), @"SerializationTypes.ItemChoiceType");
            }
            return s;
        }

        string Write69_ULongEnum(global::SerializationTypes.ULongEnum v) {
            string s = null;
            switch (v) {
                case global::SerializationTypes.ULongEnum.@Option0: s = @"Option0"; break;
                case global::SerializationTypes.ULongEnum.@Option1: s = @"Option1"; break;
                case global::SerializationTypes.ULongEnum.@Option2: s = @"Option2"; break;
                default: throw CreateInvalidEnumValueException(((System.Int64)v).ToString(System.Globalization.CultureInfo.InvariantCulture), @"SerializationTypes.ULongEnum");
            }
            return s;
        }

        string Write68_LongEnum(global::SerializationTypes.LongEnum v) {
            string s = null;
            switch (v) {
                case global::SerializationTypes.LongEnum.@Option0: s = @"Option0"; break;
                case global::SerializationTypes.LongEnum.@Option1: s = @"Option1"; break;
                case global::SerializationTypes.LongEnum.@Option2: s = @"Option2"; break;
                default: throw CreateInvalidEnumValueException(((System.Int64)v).ToString(System.Globalization.CultureInfo.InvariantCulture), @"SerializationTypes.LongEnum");
            }
            return s;
        }

        string Write67_UIntEnum(global::SerializationTypes.UIntEnum v) {
            string s = null;
            switch (v) {
                case global::SerializationTypes.UIntEnum.@Option0: s = @"Option0"; break;
                case global::SerializationTypes.UIntEnum.@Option1: s = @"Option1"; break;
                case global::SerializationTypes.UIntEnum.@Option2: s = @"Option2"; break;
                default: throw CreateInvalidEnumValueException(((System.Int64)v).ToString(System.Globalization.CultureInfo.InvariantCulture), @"SerializationTypes.UIntEnum");
            }
            return s;
        }

        string Write66_SByteEnum(global::SerializationTypes.SByteEnum v) {
            string s = null;
            switch (v) {
                case global::SerializationTypes.SByteEnum.@Option0: s = @"Option0"; break;
                case global::SerializationTypes.SByteEnum.@Option1: s = @"Option1"; break;
                case global::SerializationTypes.SByteEnum.@Option2: s = @"Option2"; break;
                default: throw CreateInvalidEnumValueException(((System.Int64)v).ToString(System.Globalization.CultureInfo.InvariantCulture), @"SerializationTypes.SByteEnum");
            }
            return s;
        }

        string Write65_ByteEnum(global::SerializationTypes.ByteEnum v) {
            string s = null;
            switch (v) {
                case global::SerializationTypes.ByteEnum.@Option0: s = @"Option0"; break;
                case global::SerializationTypes.ByteEnum.@Option1: s = @"Option1"; break;
                case global::SerializationTypes.ByteEnum.@Option2: s = @"Option2"; break;
                default: throw CreateInvalidEnumValueException(((System.Int64)v).ToString(System.Globalization.CultureInfo.InvariantCulture), @"SerializationTypes.ByteEnum");
            }
            return s;
        }

        string Write62_ShortEnum(global::SerializationTypes.ShortEnum v) {
            string s = null;
            switch (v) {
                case global::SerializationTypes.ShortEnum.@Option0: s = @"Option0"; break;
                case global::SerializationTypes.ShortEnum.@Option1: s = @"Option1"; break;
                case global::SerializationTypes.ShortEnum.@Option2: s = @"Option2"; break;
                default: throw CreateInvalidEnumValueException(((System.Int64)v).ToString(System.Globalization.CultureInfo.InvariantCulture), @"SerializationTypes.ShortEnum");
            }
            return s;
        }

        string Write61_IntEnum(global::SerializationTypes.IntEnum v) {
            string s = null;
            switch (v) {
                case global::SerializationTypes.IntEnum.@Option0: s = @"Option0"; break;
                case global::SerializationTypes.IntEnum.@Option1: s = @"Option1"; break;
                case global::SerializationTypes.IntEnum.@Option2: s = @"Option2"; break;
                default: throw CreateInvalidEnumValueException(((System.Int64)v).ToString(System.Globalization.CultureInfo.InvariantCulture), @"SerializationTypes.IntEnum");
            }
            return s;
        }

        string Write57_EnumFlags(global::SerializationTypes.EnumFlags v) {
            string s = null;
            switch (v) {
                case global::SerializationTypes.EnumFlags.@One: s = @"One"; break;
                case global::SerializationTypes.EnumFlags.@Two: s = @"Two"; break;
                case global::SerializationTypes.EnumFlags.@Three: s = @"Three"; break;
                case global::SerializationTypes.EnumFlags.@Four: s = @"Four"; break;
                default: s = FromEnum(((System.Int64)v), new string[] {@"One", 
                    @"Two", 
                    @"Three", 
                    @"Four"}, new System.Int64[] {(long)global::SerializationTypes.EnumFlags.@One, 
                    (long)global::SerializationTypes.EnumFlags.@Two, 
                    (long)global::SerializationTypes.EnumFlags.@Three, 
                    (long)global::SerializationTypes.EnumFlags.@Four}, @"SerializationTypes.EnumFlags"); break;
            }
            return s;
        }

        void Write42_TypeA(string n, string ns, global::SerializationTypes.TypeA o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeA)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeA", @"");
            WriteElementString(@"Name", @"", ((global::System.String)o.@Name));
            WriteEndElement(o);
        }

        string Write37_MyEnum(global::SerializationTypes.MyEnum v) {
            string s = null;
            switch (v) {
                case global::SerializationTypes.MyEnum.@One: s = @"One"; break;
                case global::SerializationTypes.MyEnum.@Two: s = @"Two"; break;
                case global::SerializationTypes.MyEnum.@Three: s = @"Three"; break;
                default: throw CreateInvalidEnumValueException(((System.Int64)v).ToString(System.Globalization.CultureInfo.InvariantCulture), @"SerializationTypes.MyEnum");
            }
            return s;
        }

        void Write25_Instrument(string n, string ns, global::Instrument o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::Instrument)) {
                }
                else if (t == typeof(global::Brass)) {
                    Write27_Brass(n, ns,(global::Brass)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::Trumpet)) {
                    Write28_Trumpet(n, ns,(global::Trumpet)o, isNullable, true);
                    return;
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"Instrument", @"");
            WriteElementString(@"Name", @"", ((global::System.String)o.@Name));
            WriteEndElement(o);
        }

        void Write28_Trumpet(string n, string ns, global::Trumpet o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::Trumpet)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"Trumpet", @"");
            WriteElementString(@"Name", @"", ((global::System.String)o.@Name));
            WriteElementStringRaw(@"IsValved", @"", System.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@IsValved)));
            WriteElementString(@"Modulation", @"", FromChar(((global::System.Char)o.@Modulation)));
            WriteEndElement(o);
        }

        void Write27_Brass(string n, string ns, global::Brass o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::Brass)) {
                }
                else if (t == typeof(global::Trumpet)) {
                    Write28_Trumpet(n, ns,(global::Trumpet)o, isNullable, true);
                    return;
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"Brass", @"");
            WriteElementString(@"Name", @"", ((global::System.String)o.@Name));
            WriteElementStringRaw(@"IsValved", @"", System.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@IsValved)));
            WriteEndElement(o);
        }

        void Write18_OrderedItem(string n, string ns, global::OrderedItem o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::OrderedItem)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"OrderedItem", @"http://www.contoso1.com");
            WriteElementString(@"ItemName", @"http://www.contoso1.com", ((global::System.String)o.@ItemName));
            WriteElementString(@"Description", @"http://www.contoso1.com", ((global::System.String)o.@Description));
            WriteElementStringRaw(@"UnitPrice", @"http://www.contoso1.com", System.Xml.XmlConvert.ToString((global::System.Decimal)((global::System.Decimal)o.@UnitPrice)));
            WriteElementStringRaw(@"Quantity", @"http://www.contoso1.com", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@Quantity)));
            WriteElementStringRaw(@"LineTotal", @"http://www.contoso1.com", System.Xml.XmlConvert.ToString((global::System.Decimal)((global::System.Decimal)o.@LineTotal)));
            WriteEndElement(o);
        }

        string Write10_DogBreed(global::DogBreed v) {
            string s = null;
            switch (v) {
                case global::DogBreed.@GermanShepherd: s = @"GermanShepherd"; break;
                case global::DogBreed.@LabradorRetriever: s = @"LabradorRetriever"; break;
                default: throw CreateInvalidEnumValueException(((System.Int64)v).ToString(System.Globalization.CultureInfo.InvariantCulture), @"DogBreed");
            }
            return s;
        }

        void Write2_TypeWithXmlElementProperty(string n, string ns, global::TypeWithXmlElementProperty o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::TypeWithXmlElementProperty)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithXmlElementProperty", @"");
            {
                global::System.Xml.XmlElement[] a = (global::System.Xml.XmlElement[])o.@Elements;
                if (a != null) {
                    for (int ia = 0; ia < a.Length; ia++) {
                        if ((a[ia]) is System.Xml.XmlNode || a[ia] == null) {
                            WriteElementLiteral((System.Xml.XmlNode)a[ia], @"", null, false, true);
                        }
                        else {
                            throw CreateInvalidAnyTypeException(a[ia]);
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        void Write3_TypeWithXmlDocumentProperty(string n, string ns, global::TypeWithXmlDocumentProperty o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::TypeWithXmlDocumentProperty)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithXmlDocumentProperty", @"");
            if ((((global::System.Xml.XmlDocument)o.@Document)) is System.Xml.XmlNode || ((global::System.Xml.XmlDocument)o.@Document) == null) {
                WriteElementLiteral((System.Xml.XmlNode)((global::System.Xml.XmlDocument)o.@Document), @"Document", @"", false, false);
            }
            else {
                throw CreateInvalidAnyTypeException(((global::System.Xml.XmlDocument)o.@Document));
            }
            WriteEndElement(o);
        }

        void Write4_TypeWithBinaryProperty(string n, string ns, global::TypeWithBinaryProperty o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::TypeWithBinaryProperty)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithBinaryProperty", @"");
            WriteElementStringRaw(@"BinaryHexContent", @"", FromByteArrayHex(((global::System.Byte[])o.@BinaryHexContent)));
            WriteElementStringRaw(@"Base64Content", @"", FromByteArrayBase64(((global::System.Byte[])o.@Base64Content)));
            WriteEndElement(o);
        }

        void Write5_TypeWithTimeSpanProperty(string n, string ns, global::TypeWithTimeSpanProperty o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::TypeWithTimeSpanProperty)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithTimeSpanProperty", @"");
            WriteElementStringRaw(@"TimeSpanProperty", @"", System.Xml.XmlConvert.ToString((global::System.TimeSpan)((global::System.TimeSpan)o.@TimeSpanProperty)));
            WriteEndElement(o);
        }

        void Write6_Item(string n, string ns, global::TypeWithDefaultTimeSpanProperty o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::TypeWithDefaultTimeSpanProperty)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithDefaultTimeSpanProperty", @"");
            if (((global::System.TimeSpan)o.@TimeSpanProperty) !=  new System.TimeSpan(600000000)) {
                WriteElementStringRaw(@"TimeSpanProperty", @"", System.Xml.XmlConvert.ToString((global::System.TimeSpan)((global::System.TimeSpan)o.@TimeSpanProperty)));
            }
            if (((global::System.TimeSpan)o.@TimeSpanProperty2) !=  new System.TimeSpan(10000000)) {
                WriteElementStringRaw(@"TimeSpanProperty2", @"", System.Xml.XmlConvert.ToString((global::System.TimeSpan)((global::System.TimeSpan)o.@TimeSpanProperty2)));
            }
            WriteEndElement(o);
        }

        void Write7_TypeWithByteProperty(string n, string ns, global::TypeWithByteProperty o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::TypeWithByteProperty)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithByteProperty", @"");
            WriteElementStringRaw(@"ByteProperty", @"", System.Xml.XmlConvert.ToString((global::System.Byte)((global::System.Byte)o.@ByteProperty)));
            WriteEndElement(o);
        }

        void Write8_TypeWithXmlNodeArrayProperty(string n, string ns, global::TypeWithXmlNodeArrayProperty o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::TypeWithXmlNodeArrayProperty)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithXmlNodeArrayProperty", @"");
            {
                global::System.Xml.XmlNode[] a = (global::System.Xml.XmlNode[])o.@CDATA;
                if (a != null) {
                    for (int ia = 0; ia < a.Length; ia++) {
                        if ((object)(a[ia]) != null){
                            ((global::System.Xml.XmlNode)a[ia]).WriteTo(Writer);
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        void Write11_Dog(string n, string ns, global::Dog o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::Dog)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"Dog", @"");
            WriteElementStringRaw(@"Age", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@Age)));
            WriteElementString(@"Name", @"", ((global::System.String)o.@Name));
            WriteElementString(@"Breed", @"", Write10_DogBreed(((global::DogBreed)o.@Breed)));
            WriteEndElement(o);
        }

        void Write9_Animal(string n, string ns, global::Animal o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::Animal)) {
                }
                else if (t == typeof(global::Dog)) {
                    Write11_Dog(n, ns,(global::Dog)o, isNullable, true);
                    return;
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"Animal", @"");
            WriteElementStringRaw(@"Age", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@Age)));
            WriteElementString(@"Name", @"", ((global::System.String)o.@Name));
            WriteEndElement(o);
        }

        void Write12_Vehicle(string n, string ns, global::Vehicle o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::Vehicle)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"Vehicle", @"");
            WriteElementString(@"LicenseNumber", @"", ((global::System.String)o.@LicenseNumber));
            WriteEndElement(o);
        }

        void Write13_Group(string n, string ns, global::Group o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::Group)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"Group", @"");
            WriteElementString(@"GroupName", @"", ((global::System.String)o.@GroupName));
            Write12_Vehicle(@"GroupVehicle", @"", ((global::Vehicle)o.@GroupVehicle), false, false);
            WriteEndElement(o);
        }

        void Write14_Employee(string n, string ns, global::Employee o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::Employee)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"Employee", @"");
            WriteElementString(@"EmployeeName", @"", ((global::System.String)o.@EmployeeName));
            WriteEndElement(o);
        }

        void Write15_DerivedClass(string n, string ns, global::DerivedClass o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::DerivedClass)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"DerivedClass", @"");
            WriteElementString(@"Value", @"", ((global::System.String)o.@Value));
            WriteElementString(@"value", @"", ((global::System.String)o.@value));
            WriteEndElement(o);
        }

        void Write16_BaseClass(string n, string ns, global::BaseClass o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::BaseClass)) {
                }
                else if (t == typeof(global::DerivedClass)) {
                    Write15_DerivedClass(n, ns,(global::DerivedClass)o, isNullable, true);
                    return;
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"BaseClass", @"");
            WriteElementString(@"Value", @"", ((global::System.String)o.@Value));
            WriteElementString(@"value", @"", ((global::System.String)o.@value));
            WriteEndElement(o);
        }

        void Write17_Address(string n, string ns, global::Address o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::Address)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"Address", @"http://www.contoso1.com");
            WriteAttribute(@"Name", @"", ((global::System.String)o.@Name));
            WriteElementString(@"Line1", @"http://www.contoso1.com", ((global::System.String)o.@Line1));
            WriteElementString(@"City", @"http://www.contoso1.com", ((global::System.String)o.@City));
            WriteElementString(@"State", @"http://www.contoso1.com", ((global::System.String)o.@State));
            WriteElementString(@"Zip", @"http://www.contoso1.com", ((global::System.String)o.@Zip));
            WriteEndElement(o);
        }

        void Write19_PurchaseOrder(string n, string ns, global::PurchaseOrder o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::PurchaseOrder)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"PurchaseOrder", @"http://www.contoso1.com");
            Write17_Address(@"ShipTo", @"http://www.contoso1.com", ((global::Address)o.@ShipTo), false, false);
            WriteElementString(@"OrderDate", @"http://www.contoso1.com", ((global::System.String)o.@OrderDate));
            {
                global::OrderedItem[] a = (global::OrderedItem[])((global::OrderedItem[])o.@OrderedItems);
                if (a != null){
                    WriteStartElement(@"Items", @"http://www.contoso1.com", null, false);
                    for (int ia = 0; ia < a.Length; ia++) {
                        Write18_OrderedItem(@"OrderedItem", @"http://www.contoso1.com", ((global::OrderedItem)a[ia]), true, false);
                    }
                    WriteEndElement();
                }
            }
            WriteElementStringRaw(@"SubTotal", @"http://www.contoso1.com", System.Xml.XmlConvert.ToString((global::System.Decimal)((global::System.Decimal)o.@SubTotal)));
            WriteElementStringRaw(@"ShipCost", @"http://www.contoso1.com", System.Xml.XmlConvert.ToString((global::System.Decimal)((global::System.Decimal)o.@ShipCost)));
            WriteElementStringRaw(@"TotalCost", @"http://www.contoso1.com", System.Xml.XmlConvert.ToString((global::System.Decimal)((global::System.Decimal)o.@TotalCost)));
            WriteEndElement(o);
        }

        void Write20_Address(string n, string ns, global::Address o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::Address)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"Address", @"");
            WriteAttribute(@"Name", @"", ((global::System.String)o.@Name));
            WriteElementString(@"Line1", @"", ((global::System.String)o.@Line1));
            WriteElementString(@"City", @"", ((global::System.String)o.@City));
            WriteElementString(@"State", @"", ((global::System.String)o.@State));
            WriteElementString(@"Zip", @"", ((global::System.String)o.@Zip));
            WriteEndElement(o);
        }

        void Write21_OrderedItem(string n, string ns, global::OrderedItem o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::OrderedItem)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"OrderedItem", @"");
            WriteElementString(@"ItemName", @"", ((global::System.String)o.@ItemName));
            WriteElementString(@"Description", @"", ((global::System.String)o.@Description));
            WriteElementStringRaw(@"UnitPrice", @"", System.Xml.XmlConvert.ToString((global::System.Decimal)((global::System.Decimal)o.@UnitPrice)));
            WriteElementStringRaw(@"Quantity", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@Quantity)));
            WriteElementStringRaw(@"LineTotal", @"", System.Xml.XmlConvert.ToString((global::System.Decimal)((global::System.Decimal)o.@LineTotal)));
            WriteEndElement(o);
        }

        void Write22_AliasedTestType(string n, string ns, global::AliasedTestType o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::AliasedTestType)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"AliasedTestType", @"");
            if ((object)(o.@Aliased) != null){
                if (o.@Aliased is global::System.Collections.Generic.List<global::System.Int32>) {
                    {
                        global::System.Collections.Generic.List<global::System.Int32> a = (global::System.Collections.Generic.List<global::System.Int32>)((global::System.Collections.Generic.List<global::System.Int32>)o.@Aliased);
                        if (a != null){
                            WriteStartElement(@"X", @"", null, false);
                            for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++) {
                                WriteElementStringRaw(@"int", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)a[ia])));
                            }
                            WriteEndElement();
                        }
                    }
                }
                else if (o.@Aliased is global::System.Collections.Generic.List<global::System.String>) {
                    {
                        global::System.Collections.Generic.List<global::System.String> a = (global::System.Collections.Generic.List<global::System.String>)((global::System.Collections.Generic.List<global::System.String>)o.@Aliased);
                        if (a != null){
                            WriteStartElement(@"Y", @"", null, false);
                            for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++) {
                                WriteNullableStringLiteral(@"string", @"", ((global::System.String)a[ia]));
                            }
                            WriteEndElement();
                        }
                    }
                }
                else if (o.@Aliased is global::System.Collections.Generic.List<global::System.Double>) {
                    {
                        global::System.Collections.Generic.List<global::System.Double> a = (global::System.Collections.Generic.List<global::System.Double>)((global::System.Collections.Generic.List<global::System.Double>)o.@Aliased);
                        if (a != null){
                            WriteStartElement(@"Z", @"", null, false);
                            for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++) {
                                WriteElementStringRaw(@"double", @"", System.Xml.XmlConvert.ToString((global::System.Double)((global::System.Double)a[ia])));
                            }
                            WriteEndElement();
                        }
                    }
                }
                else  if ((object)(o.@Aliased) != null){
                    throw CreateUnknownTypeException(o.@Aliased);
                }
            }
            WriteEndElement(o);
        }

        void Write24_DerivedClass1(string n, string ns, global::DerivedClass1 o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::DerivedClass1)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"DerivedClass1", @"");
            {
                global::MyCollection1 a = (global::MyCollection1)o.@Prop;
                if (a != null) {
                    System.Collections.IEnumerator e = ((System.Collections.Generic.IEnumerable<global::System.DateTime>)a).GetEnumerator();
                    if (e != null)
                    while (e.MoveNext()) {
                        global::System.DateTime ai = (global::System.DateTime)e.Current;
                        WriteElementStringRaw(@"Prop", @"", FromDateTime(((global::System.DateTime)ai)));
                    }
                }
            }
            WriteEndElement(o);
        }

        void Write23_BaseClass1(string n, string ns, global::BaseClass1 o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::BaseClass1)) {
                }
                else if (t == typeof(global::DerivedClass1)) {
                    Write24_DerivedClass1(n, ns,(global::DerivedClass1)o, isNullable, true);
                    return;
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"BaseClass1", @"");
            {
                global::MyCollection1 a = (global::MyCollection1)o.@Prop;
                if (a != null) {
                    System.Collections.IEnumerator e = ((System.Collections.Generic.IEnumerable<global::System.DateTime>)a).GetEnumerator();
                    if (e != null)
                    while (e.MoveNext()) {
                        global::System.DateTime ai = (global::System.DateTime)e.Current;
                        WriteElementStringRaw(@"Prop", @"", FromDateTime(((global::System.DateTime)ai)));
                    }
                }
            }
            WriteEndElement(o);
        }

        void Write26_Orchestra(string n, string ns, global::Orchestra o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::Orchestra)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"Orchestra", @"");
            {
                global::Instrument[] a = (global::Instrument[])((global::Instrument[])o.@Instruments);
                if (a != null){
                    WriteStartElement(@"Instruments", @"", null, false);
                    for (int ia = 0; ia < a.Length; ia++) {
                        Write25_Instrument(@"Instrument", @"", ((global::Instrument)a[ia]), true, false);
                    }
                    WriteEndElement();
                }
            }
            WriteEndElement(o);
        }

        void Write29_Pet(string n, string ns, global::Pet o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::Pet)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"Pet", @"");
            if (((global::System.String)o.@Animal) != @"Dog") {
                WriteElementString(@"Animal", @"", ((global::System.String)o.@Animal));
            }
            WriteElementString(@"Comment2", @"", ((global::System.String)o.@Comment2));
            WriteEndElement(o);
        }

        void Write30_TypeWithDateTimeStringProperty(string n, string ns, global::SerializationTypes.TypeWithDateTimeStringProperty o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithDateTimeStringProperty)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithDateTimeStringProperty", @"");
            WriteElementString(@"DateTimeString", @"", ((global::System.String)o.@DateTimeString));
            WriteElementStringRaw(@"CurrentDateTime", @"", FromDateTime(((global::System.DateTime)o.@CurrentDateTime)));
            WriteEndElement(o);
        }

        void Write32_TypeWithGetSetArrayMembers(string n, string ns, global::SerializationTypes.TypeWithGetSetArrayMembers o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithGetSetArrayMembers)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithGetSetArrayMembers", @"");
            {
                global::SerializationTypes.SimpleType[] a = (global::SerializationTypes.SimpleType[])((global::SerializationTypes.SimpleType[])o.@F1);
                if (a != null){
                    WriteStartElement(@"F1", @"", null, false);
                    for (int ia = 0; ia < a.Length; ia++) {
                        Write31_SimpleType(@"SimpleType", @"", ((global::SerializationTypes.SimpleType)a[ia]), true, false);
                    }
                    WriteEndElement();
                }
            }
            {
                global::System.Int32[] a = (global::System.Int32[])((global::System.Int32[])o.@F2);
                if (a != null){
                    WriteStartElement(@"F2", @"", null, false);
                    for (int ia = 0; ia < a.Length; ia++) {
                        WriteElementStringRaw(@"int", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)a[ia])));
                    }
                    WriteEndElement();
                }
            }
            {
                global::SerializationTypes.SimpleType[] a = (global::SerializationTypes.SimpleType[])((global::SerializationTypes.SimpleType[])o.@P1);
                if (a != null){
                    WriteStartElement(@"P1", @"", null, false);
                    for (int ia = 0; ia < a.Length; ia++) {
                        Write31_SimpleType(@"SimpleType", @"", ((global::SerializationTypes.SimpleType)a[ia]), true, false);
                    }
                    WriteEndElement();
                }
            }
            {
                global::System.Int32[] a = (global::System.Int32[])((global::System.Int32[])o.@P2);
                if (a != null){
                    WriteStartElement(@"P2", @"", null, false);
                    for (int ia = 0; ia < a.Length; ia++) {
                        WriteElementStringRaw(@"int", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)a[ia])));
                    }
                    WriteEndElement();
                }
            }
            WriteEndElement(o);
        }

        void Write33_TypeWithGetOnlyArrayProperties(string n, string ns, global::SerializationTypes.TypeWithGetOnlyArrayProperties o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithGetOnlyArrayProperties)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithGetOnlyArrayProperties", @"");
            WriteEndElement(o);
        }

        void Write34_StructNotSerializable(string n, string ns, global::SerializationTypes.StructNotSerializable o, bool needType) {
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.StructNotSerializable)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"StructNotSerializable", @"");
            WriteElementStringRaw(@"value", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@value)));
            WriteEndElement(o);
        }

        void Write35_TypeWithMyCollectionField(string n, string ns, global::SerializationTypes.TypeWithMyCollectionField o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithMyCollectionField)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithMyCollectionField", @"");
            {
                global::SerializationTypes.MyCollection<global::System.String> a = (global::SerializationTypes.MyCollection<global::System.String>)((global::SerializationTypes.MyCollection<global::System.String>)o.@Collection);
                if (a != null){
                    WriteStartElement(@"Collection", @"", null, false);
                    System.Collections.IEnumerator e = a.@GetEnumerator();
                    if (e != null)
                    while (e.MoveNext()) {
                        global::System.String ai = (global::System.String)e.Current;
                        WriteNullableStringLiteral(@"string", @"", ((global::System.String)ai));
                    }
                    WriteEndElement();
                }
            }
            WriteEndElement(o);
        }

        void Write36_Item(string n, string ns, global::SerializationTypes.TypeWithReadOnlyMyCollectionProperty o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithReadOnlyMyCollectionProperty)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithReadOnlyMyCollectionProperty", @"");
            {
                global::SerializationTypes.MyCollection<global::System.String> a = (global::SerializationTypes.MyCollection<global::System.String>)((global::SerializationTypes.MyCollection<global::System.String>)o.@Collection);
                if (a != null){
                    WriteStartElement(@"Collection", @"", null, false);
                    System.Collections.IEnumerator e = a.@GetEnumerator();
                    if (e != null)
                    while (e.MoveNext()) {
                        global::System.String ai = (global::System.String)e.Current;
                        WriteNullableStringLiteral(@"string", @"", ((global::System.String)ai));
                    }
                    WriteEndElement();
                }
            }
            WriteEndElement(o);
        }

        void Write38_TypeWithEnumMembers(string n, string ns, global::SerializationTypes.TypeWithEnumMembers o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithEnumMembers)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithEnumMembers", @"");
            WriteElementString(@"F1", @"", Write37_MyEnum(((global::SerializationTypes.MyEnum)o.@F1)));
            WriteElementString(@"P1", @"", Write37_MyEnum(((global::SerializationTypes.MyEnum)o.@P1)));
            WriteEndElement(o);
        }

        void Write39_DCStruct(string n, string ns, global::SerializationTypes.DCStruct o, bool needType) {
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.DCStruct)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"DCStruct", @"");
            WriteElementString(@"Data", @"", ((global::System.String)o.@Data));
            WriteEndElement(o);
        }

        void Write40_DCClassWithEnumAndStruct(string n, string ns, global::SerializationTypes.DCClassWithEnumAndStruct o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.DCClassWithEnumAndStruct)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"DCClassWithEnumAndStruct", @"");
            Write39_DCStruct(@"MyStruct", @"", ((global::SerializationTypes.DCStruct)o.@MyStruct), false);
            WriteElementString(@"MyEnum1", @"", Write37_MyEnum(((global::SerializationTypes.MyEnum)o.@MyEnum1)));
            WriteEndElement(o);
        }

        void Write41_BuiltInTypes(string n, string ns, global::SerializationTypes.BuiltInTypes o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.BuiltInTypes)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"BuiltInTypes", @"");
            WriteElementStringRaw(@"ByteArray", @"", FromByteArrayBase64(((global::System.Byte[])o.@ByteArray)));
            WriteEndElement(o);
        }

        void Write43_TypeB(string n, string ns, global::SerializationTypes.TypeB o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeB)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeB", @"");
            WriteElementString(@"Name", @"", ((global::System.String)o.@Name));
            WriteEndElement(o);
        }

        void Write44_TypeHasArrayOfASerializedAsB(string n, string ns, global::SerializationTypes.TypeHasArrayOfASerializedAsB o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeHasArrayOfASerializedAsB)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeHasArrayOfASerializedAsB", @"");
            {
                global::SerializationTypes.TypeA[] a = (global::SerializationTypes.TypeA[])((global::SerializationTypes.TypeA[])o.@Items);
                if (a != null){
                    WriteStartElement(@"Items", @"", null, false);
                    for (int ia = 0; ia < a.Length; ia++) {
                        Write42_TypeA(@"TypeA", @"", ((global::SerializationTypes.TypeA)a[ia]), true, false);
                    }
                    WriteEndElement();
                }
            }
            WriteEndElement(o);
        }

        void Write45_WithXElement(string n, string ns, global::SerializationTypes.WithXElement o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.WithXElement)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"WithXElement", @"");
            WriteSerializable((System.Xml.Serialization.IXmlSerializable)((global::System.Xml.Linq.XElement)o.@e), @"e", @"", false, true);
            WriteEndElement(o);
        }

        void Write46_WithXElementWithNestedXElement(string n, string ns, global::SerializationTypes.WithXElementWithNestedXElement o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.WithXElementWithNestedXElement)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"WithXElementWithNestedXElement", @"");
            WriteSerializable((System.Xml.Serialization.IXmlSerializable)((global::System.Xml.Linq.XElement)o.@e1), @"e1", @"", false, true);
            WriteEndElement(o);
        }

        void Write47_WithArrayOfXElement(string n, string ns, global::SerializationTypes.WithArrayOfXElement o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.WithArrayOfXElement)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"WithArrayOfXElement", @"");
            {
                global::System.Xml.Linq.XElement[] a = (global::System.Xml.Linq.XElement[])((global::System.Xml.Linq.XElement[])o.@a);
                if (a != null){
                    WriteStartElement(@"a", @"", null, false);
                    for (int ia = 0; ia < a.Length; ia++) {
                        WriteSerializable((System.Xml.Serialization.IXmlSerializable)((global::System.Xml.Linq.XElement)a[ia]), @"XElement", @"", true, true);
                    }
                    WriteEndElement();
                }
            }
            WriteEndElement(o);
        }

        void Write48_WithListOfXElement(string n, string ns, global::SerializationTypes.WithListOfXElement o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.WithListOfXElement)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"WithListOfXElement", @"");
            {
                global::System.Collections.Generic.List<global::System.Xml.Linq.XElement> a = (global::System.Collections.Generic.List<global::System.Xml.Linq.XElement>)((global::System.Collections.Generic.List<global::System.Xml.Linq.XElement>)o.@list);
                if (a != null){
                    WriteStartElement(@"list", @"", null, false);
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++) {
                        WriteSerializable((System.Xml.Serialization.IXmlSerializable)((global::System.Xml.Linq.XElement)a[ia]), @"XElement", @"", true, true);
                    }
                    WriteEndElement();
                }
            }
            WriteEndElement(o);
        }

        void Write49_Item(string n, string ns, global::SerializationTypes.@__TypeNameWithSpecialCharacters漢ñ o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.@__TypeNameWithSpecialCharacters漢ñ)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"__TypeNameWithSpecialCharacters漢ñ", @"");
            WriteElementString(@"PropertyNameWithSpecialCharacters漢ñ", @"", ((global::System.String)o.@PropertyNameWithSpecialCharacters漢ñ));
            WriteEndElement(o);
        }

        void Write52_DerivedClassWithSameProperty2(string n, string ns, global::SerializationTypes.DerivedClassWithSameProperty2 o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.DerivedClassWithSameProperty2)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"DerivedClassWithSameProperty2", @"");
            WriteElementString(@"StringProperty", @"", ((global::System.String)o.@StringProperty));
            WriteElementStringRaw(@"IntProperty", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@IntProperty)));
            WriteElementStringRaw(@"DateTimeProperty", @"", FromDateTime(((global::System.DateTime)o.@DateTimeProperty)));
            {
                global::System.Collections.Generic.List<global::System.String> a = (global::System.Collections.Generic.List<global::System.String>)((global::System.Collections.Generic.List<global::System.String>)o.@ListProperty);
                if (a != null){
                    WriteStartElement(@"ListProperty", @"", null, false);
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++) {
                        WriteNullableStringLiteral(@"string", @"", ((global::System.String)a[ia]));
                    }
                    WriteEndElement();
                }
            }
            WriteEndElement(o);
        }

        void Write51_DerivedClassWithSameProperty(string n, string ns, global::SerializationTypes.DerivedClassWithSameProperty o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.DerivedClassWithSameProperty)) {
                }
                else if (t == typeof(global::SerializationTypes.DerivedClassWithSameProperty2)) {
                    Write52_DerivedClassWithSameProperty2(n, ns,(global::SerializationTypes.DerivedClassWithSameProperty2)o, isNullable, true);
                    return;
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"DerivedClassWithSameProperty", @"");
            WriteElementString(@"StringProperty", @"", ((global::System.String)o.@StringProperty));
            WriteElementStringRaw(@"IntProperty", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@IntProperty)));
            WriteElementStringRaw(@"DateTimeProperty", @"", FromDateTime(((global::System.DateTime)o.@DateTimeProperty)));
            {
                global::System.Collections.Generic.List<global::System.String> a = (global::System.Collections.Generic.List<global::System.String>)((global::System.Collections.Generic.List<global::System.String>)o.@ListProperty);
                if (a != null){
                    WriteStartElement(@"ListProperty", @"", null, false);
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++) {
                        WriteNullableStringLiteral(@"string", @"", ((global::System.String)a[ia]));
                    }
                    WriteEndElement();
                }
            }
            WriteEndElement(o);
        }

        void Write50_BaseClassWithSamePropertyName(string n, string ns, global::SerializationTypes.BaseClassWithSamePropertyName o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.BaseClassWithSamePropertyName)) {
                }
                else if (t == typeof(global::SerializationTypes.DerivedClassWithSameProperty)) {
                    Write51_DerivedClassWithSameProperty(n, ns,(global::SerializationTypes.DerivedClassWithSameProperty)o, isNullable, true);
                    return;
                }
                else if (t == typeof(global::SerializationTypes.DerivedClassWithSameProperty2)) {
                    Write52_DerivedClassWithSameProperty2(n, ns,(global::SerializationTypes.DerivedClassWithSameProperty2)o, isNullable, true);
                    return;
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"BaseClassWithSamePropertyName", @"");
            WriteElementString(@"StringProperty", @"", ((global::System.String)o.@StringProperty));
            WriteElementStringRaw(@"IntProperty", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@IntProperty)));
            WriteElementStringRaw(@"DateTimeProperty", @"", FromDateTime(((global::System.DateTime)o.@DateTimeProperty)));
            {
                global::System.Collections.Generic.List<global::System.String> a = (global::System.Collections.Generic.List<global::System.String>)((global::System.Collections.Generic.List<global::System.String>)o.@ListProperty);
                if (a != null){
                    WriteStartElement(@"ListProperty", @"", null, false);
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++) {
                        WriteNullableStringLiteral(@"string", @"", ((global::System.String)a[ia]));
                    }
                    WriteEndElement();
                }
            }
            WriteEndElement(o);
        }

        void Write53_Item(string n, string ns, global::SerializationTypes.TypeWithDateTimePropertyAsXmlTime o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithDateTimePropertyAsXmlTime)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithDateTimePropertyAsXmlTime", @"");
            {
                WriteValue(FromTime(((global::System.DateTime)o.@Value)));
            }
            WriteEndElement(o);
        }

        void Write54_TypeWithByteArrayAsXmlText(string n, string ns, global::SerializationTypes.TypeWithByteArrayAsXmlText o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithByteArrayAsXmlText)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithByteArrayAsXmlText", @"");
            if ((object)(o.@Value) != null){
                WriteValue(FromByteArrayBase64(((global::System.Byte[])o.@Value)));
            }
            WriteEndElement(o);
        }

        void Write55_SimpleDC(string n, string ns, global::SerializationTypes.SimpleDC o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.SimpleDC)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"SimpleDC", @"");
            WriteElementString(@"Data", @"", ((global::System.String)o.@Data));
            WriteEndElement(o);
        }

        void Write56_Item(string n, string ns, global::SerializationTypes.TypeWithXmlTextAttributeOnArray o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithXmlTextAttributeOnArray)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithXmlTextAttributeOnArray", @"http://schemas.xmlsoap.org/ws/2005/04/discovery");
            {
                global::System.String[] a = (global::System.String[])o.@Text;
                if (a != null) {
                    for (int ia = 0; ia < a.Length; ia++) {
                        if ((object)(a[ia]) != null){
                            WriteValue(((global::System.String)a[ia]));
                        }
                    }
                }
            }
            WriteEndElement(o);
        }

        void Write58_ClassImplementsInterface(string n, string ns, global::SerializationTypes.ClassImplementsInterface o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.ClassImplementsInterface)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"ClassImplementsInterface", @"");
            WriteElementString(@"ClassID", @"", ((global::System.String)o.@ClassID));
            WriteElementString(@"DisplayName", @"", ((global::System.String)o.@DisplayName));
            WriteElementString(@"Id", @"", ((global::System.String)o.@Id));
            WriteElementStringRaw(@"IsLoaded", @"", System.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)o.@IsLoaded)));
            WriteEndElement(o);
        }

        void Write59_SomeStruct(string n, string ns, global::SerializationTypes.SomeStruct o, bool needType) {
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.SomeStruct)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"SomeStruct", @"");
            WriteElementStringRaw(@"A", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@A)));
            WriteElementStringRaw(@"B", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@B)));
            WriteEndElement(o);
        }

        void Write60_WithStruct(string n, string ns, global::SerializationTypes.WithStruct o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.WithStruct)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"WithStruct", @"");
            Write59_SomeStruct(@"Some", @"", ((global::SerializationTypes.SomeStruct)o.@Some), false);
            WriteEndElement(o);
        }

        void Write63_WithEnums(string n, string ns, global::SerializationTypes.WithEnums o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.WithEnums)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"WithEnums", @"");
            WriteElementString(@"Int", @"", Write61_IntEnum(((global::SerializationTypes.IntEnum)o.@Int)));
            WriteElementString(@"Short", @"", Write62_ShortEnum(((global::SerializationTypes.ShortEnum)o.@Short)));
            WriteEndElement(o);
        }

        void Write64_WithNullables(string n, string ns, global::SerializationTypes.WithNullables o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.WithNullables)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"WithNullables", @"");
            if (o.@Optional != null) {
                WriteElementString(@"Optional", @"", Write61_IntEnum(((global::SerializationTypes.IntEnum)o.@Optional)));
            }
            else {
                WriteNullTagLiteral(@"Optional", @"");
            }
            if (o.@Optionull != null) {
                WriteElementString(@"Optionull", @"", Write61_IntEnum(((global::SerializationTypes.IntEnum)o.@Optionull)));
            }
            else {
                WriteNullTagLiteral(@"Optionull", @"");
            }
            if (o.@OptionalInt != null) {
                WriteNullableStringLiteralRaw(@"OptionalInt", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@OptionalInt)));
            }
            else {
                WriteNullTagLiteral(@"OptionalInt", @"");
            }
            if (o.@OptionullInt != null) {
                WriteNullableStringLiteralRaw(@"OptionullInt", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@OptionullInt)));
            }
            else {
                WriteNullTagLiteral(@"OptionullInt", @"");
            }
            if (o.@Struct1 != null) {
                Write59_SomeStruct(@"Struct1", @"", ((global::SerializationTypes.SomeStruct)o.@Struct1), false);
            }
            else {
                WriteNullTagLiteral(@"Struct1", @"");
            }
            if (o.@Struct2 != null) {
                Write59_SomeStruct(@"Struct2", @"", ((global::SerializationTypes.SomeStruct)o.@Struct2), false);
            }
            else {
                WriteNullTagLiteral(@"Struct2", @"");
            }
            WriteEndElement(o);
        }

        void Write71_XmlSerializerAttributes(string n, string ns, global::SerializationTypes.XmlSerializerAttributes o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.XmlSerializerAttributes)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"XmlSerializerAttributes", @"");
            WriteAttribute(@"XmlAttributeName", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@XmlAttributeProperty)));
            {
                if (o.@EnumType == SerializationTypes.ItemChoiceType.@Word && ((object)(o.@MyChoice) != null)) {
                    if (((object)o.@MyChoice) != null && !(o.@MyChoice is global::System.String)) throw CreateMismatchChoiceException(@"System.String", @"EnumType", @"SerializationTypes.ItemChoiceType.@Word");
                    WriteElementString(@"Word", @"", ((global::System.String)o.@MyChoice));
                }
                else if (o.@EnumType == SerializationTypes.ItemChoiceType.@Number && ((object)(o.@MyChoice) != null)) {
                    if (((object)o.@MyChoice) != null && !(o.@MyChoice is global::System.Int32)) throw CreateMismatchChoiceException(@"System.Int32", @"EnumType", @"SerializationTypes.ItemChoiceType.@Number");
                    WriteElementStringRaw(@"Number", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@MyChoice)));
                }
                else if (o.@EnumType == SerializationTypes.ItemChoiceType.@DecimalNumber && ((object)(o.@MyChoice) != null)) {
                    if (((object)o.@MyChoice) != null && !(o.@MyChoice is global::System.Double)) throw CreateMismatchChoiceException(@"System.Double", @"EnumType", @"SerializationTypes.ItemChoiceType.@DecimalNumber");
                    WriteElementStringRaw(@"DecimalNumber", @"", System.Xml.XmlConvert.ToString((global::System.Double)((global::System.Double)o.@MyChoice)));
                }
                else  if ((object)(o.@MyChoice) != null){
                    throw CreateUnknownTypeException(o.@MyChoice);
                }
            }
            Write1_Object(@"XmlIncludeProperty", @"", ((global::System.Object)o.@XmlIncludeProperty), false, false);
            {
                global::SerializationTypes.ItemChoiceType[] a = (global::SerializationTypes.ItemChoiceType[])((global::SerializationTypes.ItemChoiceType[])o.@XmlEnumProperty);
                if (a != null){
                    WriteStartElement(@"XmlEnumProperty", @"", null, false);
                    for (int ia = 0; ia < a.Length; ia++) {
                        WriteElementString(@"ItemChoiceType", @"", Write70_ItemChoiceType(((global::SerializationTypes.ItemChoiceType)a[ia])));
                    }
                    WriteEndElement();
                }
            }
            if ((object)(o.@XmlTextProperty) != null){
                WriteValue(((global::System.String)o.@XmlTextProperty));
            }
            WriteElementString(@"XmlNamespaceDeclarationsProperty", @"", ((global::System.String)o.@XmlNamespaceDeclarationsProperty));
            WriteElementStringRaw(@"XmlElementPropertyNode", @"http://element", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@XmlElementProperty)));
            {
                global::System.Object[] a = (global::System.Object[])((global::System.Object[])o.@XmlArrayProperty);
                if (a != null){
                    WriteStartElement(@"CustomXmlArrayProperty", @"http://mynamespace", null, false);
                    for (int ia = 0; ia < a.Length; ia++) {
                        WriteNullableStringLiteral(@"string", @"http://mynamespace", ((global::System.String)a[ia]));
                    }
                    WriteEndElement();
                }
            }
            WriteEndElement(o);
        }

        void Write72_TypeWithAnyAttribute(string n, string ns, global::SerializationTypes.TypeWithAnyAttribute o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithAnyAttribute)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithAnyAttribute", @"");
            WriteAttribute(@"IntProperty", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@IntProperty)));
            {
                global::System.Xml.XmlAttribute[] a = (global::System.Xml.XmlAttribute[])o.@Attributes;
                if (a != null) {
                    for (int i = 0; i < a.Length; i++) {
                        global::System.Xml.XmlAttribute ai = (global::System.Xml.XmlAttribute)a[i];
                        WriteXmlAttribute(ai, o);
                    }
                }
            }
            WriteElementString(@"Name", @"", ((global::System.String)o.@Name));
            WriteEndElement(o);
        }

        void Write73_KnownTypesThroughConstructor(string n, string ns, global::SerializationTypes.KnownTypesThroughConstructor o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.KnownTypesThroughConstructor)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"KnownTypesThroughConstructor", @"");
            Write1_Object(@"EnumValue", @"", ((global::System.Object)o.@EnumValue), false, false);
            Write1_Object(@"SimpleTypeValue", @"", ((global::System.Object)o.@SimpleTypeValue), false, false);
            WriteEndElement(o);
        }

        void Write74_SimpleKnownTypeValue(string n, string ns, global::SerializationTypes.SimpleKnownTypeValue o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.SimpleKnownTypeValue)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"SimpleKnownTypeValue", @"");
            WriteElementString(@"StrProperty", @"", ((global::System.String)o.@StrProperty));
            WriteEndElement(o);
        }

        void Write75_TypeWithPropertyNameSpecified(string n, string ns, global::SerializationTypes.TypeWithPropertyNameSpecified o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithPropertyNameSpecified)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithPropertyNameSpecified", @"");
            if (o.@MyFieldSpecified) {
                WriteElementString(@"MyField", @"", ((global::System.String)o.@MyField));
            }
            if (o.@MyFieldIgnoredSpecified) {
                WriteElementStringRaw(@"MyFieldIgnored", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@MyFieldIgnored)));
            }
            WriteEndElement(o);
        }

        void Write76_TypeWithXmlSchemaFormAttribute(string n, string ns, global::SerializationTypes.TypeWithXmlSchemaFormAttribute o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithXmlSchemaFormAttribute)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithXmlSchemaFormAttribute", @"");
            {
                global::System.Collections.Generic.List<global::System.Int32> a = (global::System.Collections.Generic.List<global::System.Int32>)((global::System.Collections.Generic.List<global::System.Int32>)o.@UnqualifiedSchemaFormListProperty);
                if (a != null){
                    WriteStartElement(@"UnqualifiedSchemaFormListProperty", @"", null, false);
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++) {
                        WriteElementStringRaw(@"int", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)a[ia])));
                    }
                    WriteEndElement();
                }
            }
            {
                global::System.Collections.Generic.List<global::System.String> a = (global::System.Collections.Generic.List<global::System.String>)((global::System.Collections.Generic.List<global::System.String>)o.@NoneSchemaFormListProperty);
                if (a != null){
                    WriteStartElement(@"NoneSchemaFormListProperty", @"", null, false);
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++) {
                        WriteElementString(@"NoneParameter", @"", ((global::System.String)a[ia]));
                    }
                    WriteEndElement();
                }
            }
            {
                global::System.Collections.Generic.List<global::System.Boolean> a = (global::System.Collections.Generic.List<global::System.Boolean>)((global::System.Collections.Generic.List<global::System.Boolean>)o.@QualifiedSchemaFormListProperty);
                if (a != null){
                    WriteStartElement(@"QualifiedSchemaFormListProperty", @"", null, false);
                    for (int ia = 0; ia < ((System.Collections.ICollection)a).Count; ia++) {
                        WriteElementStringRaw(@"QualifiedParameter", @"", System.Xml.XmlConvert.ToString((global::System.Boolean)((global::System.Boolean)a[ia])));
                    }
                    WriteEndElement();
                }
            }
            WriteEndElement(o);
        }

        void Write77_Item(string n, string ns, global::SerializationTypes.TypeWithTypeNameInXmlTypeAttribute o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithTypeNameInXmlTypeAttribute)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"MyXmlType", @"");
            WriteAttribute(@"XmlAttributeForm", @"", ((global::System.String)o.@XmlAttributeForm));
            WriteEndElement(o);
        }

        void Write79_Item(string n, string ns, global::SerializationTypes.TypeWithNonPublicDefaultConstructor o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithNonPublicDefaultConstructor)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithNonPublicDefaultConstructor", @"");
            WriteElementString(@"Name", @"", ((global::System.String)o.@Name));
            WriteEndElement(o);
        }

        void Write80_ServerSettings(string n, string ns, global::SerializationTypes.ServerSettings o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.ServerSettings)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"ServerSettings", @"");
            WriteElementString(@"DS2Root", @"", ((global::System.String)o.@DS2Root));
            WriteElementString(@"MetricConfigUrl", @"", ((global::System.String)o.@MetricConfigUrl));
            WriteEndElement(o);
        }

        void Write81_TypeWithXmlQualifiedName(string n, string ns, global::SerializationTypes.TypeWithXmlQualifiedName o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithXmlQualifiedName)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithXmlQualifiedName", @"");
            WriteElementQualifiedName(@"Value", @"", ((global::System.Xml.XmlQualifiedName)o.@Value));
            WriteEndElement(o);
        }

        void Write82_TypeWith2DArrayProperty2(string n, string ns, global::SerializationTypes.TypeWith2DArrayProperty2 o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWith2DArrayProperty2)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWith2DArrayProperty2", @"");
            {
                global::SerializationTypes.SimpleType[][] a = (global::SerializationTypes.SimpleType[][])((global::SerializationTypes.SimpleType[][])o.@TwoDArrayOfSimpleType);
                if (a != null){
                    WriteStartElement(@"TwoDArrayOfSimpleType", @"", null, false);
                    for (int ia = 0; ia < a.Length; ia++) {
                        {
                            global::SerializationTypes.SimpleType[] aa = (global::SerializationTypes.SimpleType[])((global::SerializationTypes.SimpleType[])a[ia]);
                            if (aa != null){
                                WriteStartElement(@"SimpleType", @"", null, false);
                                for (int iaa = 0; iaa < aa.Length; iaa++) {
                                    Write31_SimpleType(@"SimpleType", @"", ((global::SerializationTypes.SimpleType)aa[iaa]), true, false);
                                }
                                WriteEndElement();
                            }
                        }
                    }
                    WriteEndElement();
                }
            }
            WriteEndElement(o);
        }

        void Write83_Item(string n, string ns, global::SerializationTypes.TypeWithPropertiesHavingDefaultValue o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithPropertiesHavingDefaultValue)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithPropertiesHavingDefaultValue", @"");
            if ((((global::System.String)o.@EmptyStringProperty) != null) && (((global::System.String)o.@EmptyStringProperty).Length != 0)) {
                WriteElementString(@"EmptyStringProperty", @"", ((global::System.String)o.@EmptyStringProperty));
            }
            if (((global::System.String)o.@StringProperty) != @"DefaultString") {
                WriteElementString(@"StringProperty", @"", ((global::System.String)o.@StringProperty));
            }
            if (((global::System.Int32)o.@IntProperty) != 11) {
                WriteElementStringRaw(@"IntProperty", @"", System.Xml.XmlConvert.ToString((global::System.Int32)((global::System.Int32)o.@IntProperty)));
            }
            WriteElementString(@"CharProperty", @"", FromChar(((global::System.Char)o.@CharProperty)));
            WriteEndElement(o);
        }

        void Write84_Item(string n, string ns, global::SerializationTypes.TypeWithEnumPropertyHavingDefaultValue o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithEnumPropertyHavingDefaultValue)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithEnumPropertyHavingDefaultValue", @"");
            if (((global::SerializationTypes.IntEnum)o.@EnumProperty) != global::SerializationTypes.IntEnum.@Option1) {
                WriteElementString(@"EnumProperty", @"", Write61_IntEnum(((global::SerializationTypes.IntEnum)o.@EnumProperty)));
            }
            WriteEndElement(o);
        }

        void Write85_Item(string n, string ns, global::SerializationTypes.TypeWithEnumFlagPropertyHavingDefaultValue o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithEnumFlagPropertyHavingDefaultValue)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithEnumFlagPropertyHavingDefaultValue", @"");
            if (((global::SerializationTypes.EnumFlags)o.@EnumProperty) != (global::SerializationTypes.EnumFlags.@One | 
            global::SerializationTypes.EnumFlags.@Four)) {
                WriteElementString(@"EnumProperty", @"", Write57_EnumFlags(((global::SerializationTypes.EnumFlags)o.@EnumProperty)));
            }
            WriteEndElement(o);
        }

        void Write86_TypeWithShouldSerializeMethod(string n, string ns, global::SerializationTypes.TypeWithShouldSerializeMethod o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithShouldSerializeMethod)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"TypeWithShouldSerializeMethod", @"");
            if (o.@ShouldSerializeFoo()) {
                WriteElementString(@"Foo", @"", ((global::System.String)o.@Foo));
            }
            WriteEndElement(o);
        }

        void Write87_Item(string n, string ns, global::SerializationTypes.KnownTypesThroughConstructorWithArrayProperties o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.KnownTypesThroughConstructorWithArrayProperties)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(@"KnownTypesThroughConstructorWithArrayProperties", @"");
            Write1_Object(@"StringArrayValue", @"", ((global::System.Object)o.@StringArrayValue), false, false);
            Write1_Object(@"IntArrayValue", @"", ((global::System.Object)o.@IntArrayValue), false, false);
            WriteEndElement(o);
        }

        void Write78_Item(string n, string ns, global::SerializationTypes.TypeWithSchemaFormInXmlAttribute o, bool isNullable, bool needType) {
            if ((object)o == null) {
                if (isNullable) WriteNullTagLiteral(n, ns);
                return;
            }
            if (!needType) {
                System.Type t = o.GetType();
                if (t == typeof(global::SerializationTypes.TypeWithSchemaFormInXmlAttribute)) {
                }
                else {
                    throw CreateUnknownTypeException(o);
                }
            }
            WriteStartElement(n, ns, o, false, null);
            if (needType) WriteXsiType(null, @"");
            WriteAttribute(@"TestProperty", @"http://test.com", ((global::System.String)o.@TestProperty));
            WriteEndElement(o);
        }

        protected override void InitCallbacks() {
        }
    }

    public class XmlSerializationReader1 : System.Xml.Serialization.XmlSerializationReader {

        public object Read97_TypeWithXmlElementProperty() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id1_TypeWithXmlElementProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read2_TypeWithXmlElementProperty(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithXmlElementProperty");
            }
            return (object)o;
        }

        public object Read98_TypeWithXmlDocumentProperty() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id3_TypeWithXmlDocumentProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read3_TypeWithXmlDocumentProperty(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithXmlDocumentProperty");
            }
            return (object)o;
        }

        public object Read99_TypeWithBinaryProperty() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id4_TypeWithBinaryProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read4_TypeWithBinaryProperty(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithBinaryProperty");
            }
            return (object)o;
        }

        public object Read100_TypeWithTimeSpanProperty() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id5_TypeWithTimeSpanProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read5_TypeWithTimeSpanProperty(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithTimeSpanProperty");
            }
            return (object)o;
        }

        public object Read101_Item() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id6_Item && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read6_Item(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithDefaultTimeSpanProperty");
            }
            return (object)o;
        }

        public object Read102_TypeWithByteProperty() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id7_TypeWithByteProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read7_TypeWithByteProperty(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithByteProperty");
            }
            return (object)o;
        }

        public object Read103_TypeWithXmlNodeArrayProperty() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id8_TypeWithXmlNodeArrayProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read8_TypeWithXmlNodeArrayProperty(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithXmlNodeArrayProperty");
            }
            return (object)o;
        }

        public object Read104_Animal() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id9_Animal && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read9_Animal(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":Animal");
            }
            return (object)o;
        }

        public object Read105_Dog() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id10_Dog && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read11_Dog(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":Dog");
            }
            return (object)o;
        }

        public object Read106_DogBreed() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id11_DogBreed && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    {
                        o = Read10_DogBreed(Reader.ReadElementString());
                    }
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":DogBreed");
            }
            return (object)o;
        }

        public object Read107_Group() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id12_Group && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read13_Group(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":Group");
            }
            return (object)o;
        }

        public object Read108_Vehicle() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id13_Vehicle && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read12_Vehicle(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":Vehicle");
            }
            return (object)o;
        }

        public object Read109_Employee() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id14_Employee && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read14_Employee(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":Employee");
            }
            return (object)o;
        }

        public object Read110_BaseClass() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id15_BaseClass && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read16_BaseClass(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":BaseClass");
            }
            return (object)o;
        }

        public object Read111_DerivedClass() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id16_DerivedClass && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read15_DerivedClass(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":DerivedClass");
            }
            return (object)o;
        }

        public object Read112_PurchaseOrder() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id17_PurchaseOrder && (object) Reader.NamespaceURI == (object)id18_httpwwwcontoso1com)) {
                    o = Read19_PurchaseOrder(false, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @"http://www.contoso1.com:PurchaseOrder");
            }
            return (object)o;
        }

        public object Read113_Address() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id19_Address && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read20_Address(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":Address");
            }
            return (object)o;
        }

        public object Read114_OrderedItem() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id20_OrderedItem && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read21_OrderedItem(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":OrderedItem");
            }
            return (object)o;
        }

        public object Read115_AliasedTestType() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id21_AliasedTestType && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read22_AliasedTestType(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":AliasedTestType");
            }
            return (object)o;
        }

        public object Read116_BaseClass1() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id22_BaseClass1 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read23_BaseClass1(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":BaseClass1");
            }
            return (object)o;
        }

        public object Read117_DerivedClass1() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id23_DerivedClass1 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read24_DerivedClass1(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":DerivedClass1");
            }
            return (object)o;
        }

        public object Read118_ArrayOfDateTime() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id24_ArrayOfDateTime && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    if (!ReadNull()) {
                        if ((object)(o) == null) o = new global::MyCollection1();
                        global::MyCollection1 a_0_0 = (global::MyCollection1)o;
                        if ((Reader.IsEmptyElement)) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            int whileIterations0 = 0;
                            int readerCount0 = ReaderCount;
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id25_dateTime && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        {
                                            a_0_0.Add(ToDateTime(Reader.ReadElementString()));
                                        }
                                    }
                                    else {
                                        UnknownNode(null, @":dateTime");
                                    }
                                }
                                else {
                                    UnknownNode(null, @":dateTime");
                                }
                                Reader.MoveToContent();
                                CheckReaderCount(ref whileIterations0, ref readerCount0);
                            }
                        ReadEndElement();
                        }
                    }
                    else {
                        if ((object)(o) == null) o = new global::MyCollection1();
                        global::MyCollection1 a_0_0 = (global::MyCollection1)o;
                    }
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":ArrayOfDateTime");
            }
            return (object)o;
        }

        public object Read119_Orchestra() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id26_Orchestra && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read26_Orchestra(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":Orchestra");
            }
            return (object)o;
        }

        public object Read120_Instrument() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id27_Instrument && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read25_Instrument(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":Instrument");
            }
            return (object)o;
        }

        public object Read121_Brass() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id28_Brass && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read27_Brass(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":Brass");
            }
            return (object)o;
        }

        public object Read122_Trumpet() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id29_Trumpet && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read28_Trumpet(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":Trumpet");
            }
            return (object)o;
        }

        public object Read123_Pet() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id30_Pet && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read29_Pet(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":Pet");
            }
            return (object)o;
        }

        public object Read124_TypeWithDateTimeStringProperty() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id31_TypeWithDateTimeStringProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read30_TypeWithDateTimeStringProperty(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithDateTimeStringProperty");
            }
            return (object)o;
        }

        public object Read125_SimpleType() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id32_SimpleType && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read31_SimpleType(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":SimpleType");
            }
            return (object)o;
        }

        public object Read126_TypeWithGetSetArrayMembers() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id33_TypeWithGetSetArrayMembers && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read32_TypeWithGetSetArrayMembers(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithGetSetArrayMembers");
            }
            return (object)o;
        }

        public object Read127_TypeWithGetOnlyArrayProperties() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id34_TypeWithGetOnlyArrayProperties && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read33_TypeWithGetOnlyArrayProperties(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithGetOnlyArrayProperties");
            }
            return (object)o;
        }

        public object Read128_StructNotSerializable() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id35_StructNotSerializable && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read34_StructNotSerializable(true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":StructNotSerializable");
            }
            return (object)o;
        }

        public object Read129_TypeWithMyCollectionField() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id36_TypeWithMyCollectionField && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read35_TypeWithMyCollectionField(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithMyCollectionField");
            }
            return (object)o;
        }

        public object Read130_Item() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id37_Item && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read36_Item(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithReadOnlyMyCollectionProperty");
            }
            return (object)o;
        }

        public object Read131_ArrayOfAnyType() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id38_ArrayOfAnyType && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    if (!ReadNull()) {
                        if ((object)(o) == null) o = new global::SerializationTypes.MyList();
                        global::SerializationTypes.MyList a_0_0 = (global::SerializationTypes.MyList)o;
                        if ((Reader.IsEmptyElement)) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            int whileIterations1 = 0;
                            int readerCount1 = ReaderCount;
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id39_anyType && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        if ((object)(a_0_0) == null) Reader.Skip(); else a_0_0.Add(Read1_Object(true, true));
                                    }
                                    else {
                                        UnknownNode(null, @":anyType");
                                    }
                                }
                                else {
                                    UnknownNode(null, @":anyType");
                                }
                                Reader.MoveToContent();
                                CheckReaderCount(ref whileIterations1, ref readerCount1);
                            }
                        ReadEndElement();
                        }
                    }
                    else {
                        if ((object)(o) == null) o = new global::SerializationTypes.MyList();
                        global::SerializationTypes.MyList a_0_0 = (global::SerializationTypes.MyList)o;
                    }
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":ArrayOfAnyType");
            }
            return (object)o;
        }

        public object Read132_MyEnum() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id40_MyEnum && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    {
                        o = Read37_MyEnum(Reader.ReadElementString());
                    }
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":MyEnum");
            }
            return (object)o;
        }

        public object Read133_TypeWithEnumMembers() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id41_TypeWithEnumMembers && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read38_TypeWithEnumMembers(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithEnumMembers");
            }
            return (object)o;
        }

        public object Read134_DCStruct() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id42_DCStruct && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read39_DCStruct(true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":DCStruct");
            }
            return (object)o;
        }

        public object Read135_DCClassWithEnumAndStruct() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id43_DCClassWithEnumAndStruct && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read40_DCClassWithEnumAndStruct(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":DCClassWithEnumAndStruct");
            }
            return (object)o;
        }

        public object Read136_BuiltInTypes() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id44_BuiltInTypes && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read41_BuiltInTypes(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":BuiltInTypes");
            }
            return (object)o;
        }

        public object Read137_TypeA() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id45_TypeA && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read42_TypeA(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeA");
            }
            return (object)o;
        }

        public object Read138_TypeB() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id46_TypeB && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read43_TypeB(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeB");
            }
            return (object)o;
        }

        public object Read139_TypeHasArrayOfASerializedAsB() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id47_TypeHasArrayOfASerializedAsB && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read44_TypeHasArrayOfASerializedAsB(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeHasArrayOfASerializedAsB");
            }
            return (object)o;
        }

        public object Read140_WithXElement() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id48_WithXElement && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read45_WithXElement(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":WithXElement");
            }
            return (object)o;
        }

        public object Read141_WithXElementWithNestedXElement() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id49_WithXElementWithNestedXElement && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read46_WithXElementWithNestedXElement(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":WithXElementWithNestedXElement");
            }
            return (object)o;
        }

        public object Read142_WithArrayOfXElement() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id50_WithArrayOfXElement && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read47_WithArrayOfXElement(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":WithArrayOfXElement");
            }
            return (object)o;
        }

        public object Read143_WithListOfXElement() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id51_WithListOfXElement && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read48_WithListOfXElement(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":WithListOfXElement");
            }
            return (object)o;
        }

        public object Read144_Item() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id52_Item && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read49_Item(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":__TypeNameWithSpecialCharacters漢ñ");
            }
            return (object)o;
        }

        public object Read145_BaseClassWithSamePropertyName() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id53_BaseClassWithSamePropertyName && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read50_BaseClassWithSamePropertyName(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":BaseClassWithSamePropertyName");
            }
            return (object)o;
        }

        public object Read146_DerivedClassWithSameProperty() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id54_DerivedClassWithSameProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read51_DerivedClassWithSameProperty(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":DerivedClassWithSameProperty");
            }
            return (object)o;
        }

        public object Read147_DerivedClassWithSameProperty2() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id55_DerivedClassWithSameProperty2 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read52_DerivedClassWithSameProperty2(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":DerivedClassWithSameProperty2");
            }
            return (object)o;
        }

        public object Read148_Item() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id56_Item && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read53_Item(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithDateTimePropertyAsXmlTime");
            }
            return (object)o;
        }

        public object Read149_TypeWithByteArrayAsXmlText() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id57_TypeWithByteArrayAsXmlText && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read54_TypeWithByteArrayAsXmlText(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithByteArrayAsXmlText");
            }
            return (object)o;
        }

        public object Read150_SimpleDC() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id58_SimpleDC && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read55_SimpleDC(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":SimpleDC");
            }
            return (object)o;
        }

        public object Read151_Item() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id59_Item && (object) Reader.NamespaceURI == (object)id60_Item)) {
                    o = Read56_Item(false, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @"http://schemas.xmlsoap.org/ws/2005/04/discovery:TypeWithXmlTextAttributeOnArray");
            }
            return (object)o;
        }

        public object Read152_EnumFlags() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id61_EnumFlags && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    {
                        o = Read57_EnumFlags(Reader.ReadElementString());
                    }
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":EnumFlags");
            }
            return (object)o;
        }

        public object Read153_ClassImplementsInterface() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id62_ClassImplementsInterface && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read58_ClassImplementsInterface(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":ClassImplementsInterface");
            }
            return (object)o;
        }

        public object Read154_WithStruct() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id63_WithStruct && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read60_WithStruct(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":WithStruct");
            }
            return (object)o;
        }

        public object Read155_SomeStruct() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id64_SomeStruct && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read59_SomeStruct(true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":SomeStruct");
            }
            return (object)o;
        }

        public object Read156_WithEnums() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id65_WithEnums && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read63_WithEnums(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":WithEnums");
            }
            return (object)o;
        }

        public object Read157_WithNullables() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id66_WithNullables && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read67_WithNullables(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":WithNullables");
            }
            return (object)o;
        }

        public object Read158_ByteEnum() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id67_ByteEnum && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    {
                        o = Read68_ByteEnum(Reader.ReadElementString());
                    }
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":ByteEnum");
            }
            return (object)o;
        }

        public object Read159_SByteEnum() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id68_SByteEnum && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    {
                        o = Read69_SByteEnum(Reader.ReadElementString());
                    }
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":SByteEnum");
            }
            return (object)o;
        }

        public object Read160_ShortEnum() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id69_ShortEnum && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    {
                        o = Read62_ShortEnum(Reader.ReadElementString());
                    }
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":ShortEnum");
            }
            return (object)o;
        }

        public object Read161_IntEnum() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id70_IntEnum && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    {
                        o = Read61_IntEnum(Reader.ReadElementString());
                    }
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":IntEnum");
            }
            return (object)o;
        }

        public object Read162_UIntEnum() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id71_UIntEnum && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    {
                        o = Read70_UIntEnum(Reader.ReadElementString());
                    }
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":UIntEnum");
            }
            return (object)o;
        }

        public object Read163_LongEnum() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id72_LongEnum && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    {
                        o = Read71_LongEnum(Reader.ReadElementString());
                    }
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":LongEnum");
            }
            return (object)o;
        }

        public object Read164_ULongEnum() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id73_ULongEnum && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    {
                        o = Read72_ULongEnum(Reader.ReadElementString());
                    }
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":ULongEnum");
            }
            return (object)o;
        }

        public object Read165_AttributeTesting() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id74_AttributeTesting && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read74_XmlSerializerAttributes(false, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":AttributeTesting");
            }
            return (object)o;
        }

        public object Read166_ItemChoiceType() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id75_ItemChoiceType && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    {
                        o = Read73_ItemChoiceType(Reader.ReadElementString());
                    }
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":ItemChoiceType");
            }
            return (object)o;
        }

        public object Read167_TypeWithAnyAttribute() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id76_TypeWithAnyAttribute && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read75_TypeWithAnyAttribute(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithAnyAttribute");
            }
            return (object)o;
        }

        public object Read168_KnownTypesThroughConstructor() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id77_KnownTypesThroughConstructor && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read76_KnownTypesThroughConstructor(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":KnownTypesThroughConstructor");
            }
            return (object)o;
        }

        public object Read169_SimpleKnownTypeValue() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id78_SimpleKnownTypeValue && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read77_SimpleKnownTypeValue(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":SimpleKnownTypeValue");
            }
            return (object)o;
        }

        public object Read170_Item() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id79_Item && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = (global::SerializationTypes.ClassImplementingIXmlSerialiable)ReadSerializable(( System.Xml.Serialization.IXmlSerializable)new global::SerializationTypes.ClassImplementingIXmlSerialiable());
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":ClassImplementingIXmlSerialiable");
            }
            return (object)o;
        }

        public object Read171_TypeWithPropertyNameSpecified() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id80_TypeWithPropertyNameSpecified && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read78_TypeWithPropertyNameSpecified(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithPropertyNameSpecified");
            }
            return (object)o;
        }

        public object Read172_TypeWithXmlSchemaFormAttribute() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id81_TypeWithXmlSchemaFormAttribute && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read79_TypeWithXmlSchemaFormAttribute(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithXmlSchemaFormAttribute");
            }
            return (object)o;
        }

        public object Read173_MyXmlType() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id82_MyXmlType && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read80_Item(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":MyXmlType");
            }
            return (object)o;
        }

        public object Read174_Item() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id83_Item && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read81_Item(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithSchemaFormInXmlAttribute");
            }
            return (object)o;
        }

        public object Read175_Item() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id84_Item && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read82_Item(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithNonPublicDefaultConstructor");
            }
            return (object)o;
        }

        public object Read176_ServerSettings() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id85_ServerSettings && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read83_ServerSettings(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":ServerSettings");
            }
            return (object)o;
        }

        public object Read177_TypeWithXmlQualifiedName() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id86_TypeWithXmlQualifiedName && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read84_TypeWithXmlQualifiedName(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithXmlQualifiedName");
            }
            return (object)o;
        }

        public object Read178_TypeWith2DArrayProperty2() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id87_TypeWith2DArrayProperty2 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read85_TypeWith2DArrayProperty2(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWith2DArrayProperty2");
            }
            return (object)o;
        }

        public object Read179_Item() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id88_Item && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read86_Item(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithPropertiesHavingDefaultValue");
            }
            return (object)o;
        }

        public object Read180_Item() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id89_Item && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read87_Item(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithEnumPropertyHavingDefaultValue");
            }
            return (object)o;
        }

        public object Read181_Item() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id90_Item && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read88_Item(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithEnumFlagPropertyHavingDefaultValue");
            }
            return (object)o;
        }

        public object Read182_TypeWithShouldSerializeMethod() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id91_TypeWithShouldSerializeMethod && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read89_TypeWithShouldSerializeMethod(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithShouldSerializeMethod");
            }
            return (object)o;
        }

        public object Read183_Item() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id92_Item && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read90_Item(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":KnownTypesThroughConstructorWithArrayProperties");
            }
            return (object)o;
        }

        public object Read184_Item() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id93_Item && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read91_Item(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":KnownTypesThroughConstructorWithValue");
            }
            return (object)o;
        }

        public object Read185_Item() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id94_Item && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read92_Item(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithTypesHavingCustomFormatter");
            }
            return (object)o;
        }

        public object Read186_Item() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id95_Item && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read94_Item(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithArrayPropertyHavingChoice");
            }
            return (object)o;
        }

        public object Read187_MoreChoices() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id96_MoreChoices && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    {
                        o = Read93_MoreChoices(Reader.ReadElementString());
                    }
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":MoreChoices");
            }
            return (object)o;
        }

        public object Read188_TypeWithFieldsOrdered() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id97_TypeWithFieldsOrdered && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read95_TypeWithFieldsOrdered(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":TypeWithFieldsOrdered");
            }
            return (object)o;
        }

        public object Read189_Person() {
            object o = null;
            Reader.MoveToContent();
            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                if (((object) Reader.LocalName == (object)id98_Person && (object) Reader.NamespaceURI == (object)id2_Item)) {
                    o = Read96_Person(true, true);
                }
                else {
                    throw CreateUnknownNodeException();
                }
            }
            else {
                UnknownNode(null, @":Person");
            }
            return (object)o;
        }

        global::Outer.Person Read96_Person(bool isNullable, bool checkType) {
            System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType) {
            if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id98_Person && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
            }
            else
                throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::Outer.Person o;
            o = new global::Outer.Person();
            bool[] paramsRead = new bool[3];
            while (Reader.MoveToNextAttribute()) {
                if (!IsXmlnsAttribute(Reader.Name)) {
                    UnknownNode((object)o);
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement) {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations2 = 0;
            int readerCount2 = ReaderCount;
            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                    if (!paramsRead[0] && ((object) Reader.LocalName == (object)id99_FirstName && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        {
                            o.@FirstName = Reader.ReadElementString();
                        }
                        paramsRead[0] = true;
                    }
                    else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id100_MiddleName && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        {
                            o.@MiddleName = Reader.ReadElementString();
                        }
                        paramsRead[1] = true;
                    }
                    else if (!paramsRead[2] && ((object) Reader.LocalName == (object)id101_LastName && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        {
                            o.@LastName = Reader.ReadElementString();
                        }
                        paramsRead[2] = true;
                    }
                    else {
                        UnknownNode((object)o, @":FirstName, :MiddleName, :LastName");
                    }
                }
                else {
                    UnknownNode((object)o, @":FirstName, :MiddleName, :LastName");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations2, ref readerCount2);
            }
            ReadEndElement();
            return o;
        }

        global::SerializationTypes.TypeWithFieldsOrdered Read95_TypeWithFieldsOrdered(bool isNullable, bool checkType) {
            System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType) {
            if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id97_TypeWithFieldsOrdered && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
            }
            else
                throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::SerializationTypes.TypeWithFieldsOrdered o;
            o = new global::SerializationTypes.TypeWithFieldsOrdered();
            bool[] paramsRead = new bool[4];
            while (Reader.MoveToNextAttribute()) {
                if (!IsXmlnsAttribute(Reader.Name)) {
                    UnknownNode((object)o);
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement) {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            int state = 0;
            Reader.MoveToContent();
            int whileIterations3 = 0;
            int readerCount3 = ReaderCount;
            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                    switch (state) {
                    case 0:
                        if (((object) Reader.LocalName == (object)id102_IntField1 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@IntField1 = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                            }
                        }
                        state = 1;
                        break;
                    case 1:
                        if (((object) Reader.LocalName == (object)id103_IntField2 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@IntField2 = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                            }
                        }
                        state = 2;
                        break;
                    case 2:
                        if (((object) Reader.LocalName == (object)id104_StringField2 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@StringField2 = Reader.ReadElementString();
                            }
                        }
                        state = 3;
                        break;
                    case 3:
                        if (((object) Reader.LocalName == (object)id105_StringField1 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@StringField1 = Reader.ReadElementString();
                            }
                        }
                        state = 4;
                        break;
                    default:
                        UnknownNode((object)o, null);
                        break;
                    }
                }
                else {
                    UnknownNode((object)o, null);
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations3, ref readerCount3);
            }
            ReadEndElement();
            return o;
        }

        global::SerializationTypes.MoreChoices Read93_MoreChoices(string s) {
            switch (s) {
                case @"None": return global::SerializationTypes.MoreChoices.@None;
                case @"Item": return global::SerializationTypes.MoreChoices.@Item;
                case @"Amount": return global::SerializationTypes.MoreChoices.@Amount;
                default: throw CreateUnknownConstantException(s, typeof(global::SerializationTypes.MoreChoices));
            }
        }

        global::SerializationTypes.TypeWithArrayPropertyHavingChoice Read94_Item(bool isNullable, bool checkType) {
            System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType) {
            if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id95_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
            }
            else
                throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::SerializationTypes.TypeWithArrayPropertyHavingChoice o;
            o = new global::SerializationTypes.TypeWithArrayPropertyHavingChoice();
            global::System.Object[] a_0 = null;
            int ca_0 = 0;
            global::SerializationTypes.MoreChoices[] choice_a_0 = null;
            int cchoice_a_0 = 0;
            bool[] paramsRead = new bool[1];
            while (Reader.MoveToNextAttribute()) {
                if (!IsXmlnsAttribute(Reader.Name)) {
                    UnknownNode((object)o);
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement) {
                Reader.Skip();
                o.@ManyChoices = (global::System.Object[])ShrinkArray(a_0, ca_0, typeof(global::System.Object), true);
                o.@ChoiceArray = (global::SerializationTypes.MoreChoices[])ShrinkArray(choice_a_0, cchoice_a_0, typeof(global::SerializationTypes.MoreChoices), true);
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations4 = 0;
            int readerCount4 = ReaderCount;
            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                    if (((object) Reader.LocalName == (object)id106_Item && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        {
                            a_0 = (global::System.Object[])EnsureArrayIndex(a_0, ca_0, typeof(global::System.Object));a_0[ca_0++] = Reader.ReadElementString();
                        }
                        choice_a_0 = (global::SerializationTypes.MoreChoices[])EnsureArrayIndex(choice_a_0, cchoice_a_0, typeof(global::SerializationTypes.MoreChoices));choice_a_0[cchoice_a_0++] = global::SerializationTypes.MoreChoices.@Item;
                    }
                    else if (((object) Reader.LocalName == (object)id107_Amount && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        {
                            a_0 = (global::System.Object[])EnsureArrayIndex(a_0, ca_0, typeof(global::System.Object));a_0[ca_0++] = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                        }
                        choice_a_0 = (global::SerializationTypes.MoreChoices[])EnsureArrayIndex(choice_a_0, cchoice_a_0, typeof(global::SerializationTypes.MoreChoices));choice_a_0[cchoice_a_0++] = global::SerializationTypes.MoreChoices.@Amount;
                    }
                    else {
                        UnknownNode((object)o, @":Item, :Amount");
                    }
                }
                else {
                    UnknownNode((object)o, @":Item, :Amount");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations4, ref readerCount4);
            }
            o.@ManyChoices = (global::System.Object[])ShrinkArray(a_0, ca_0, typeof(global::System.Object), true);
            o.@ChoiceArray = (global::SerializationTypes.MoreChoices[])ShrinkArray(choice_a_0, cchoice_a_0, typeof(global::SerializationTypes.MoreChoices), true);
            ReadEndElement();
            return o;
        }

        global::SerializationTypes.TypeWithTypesHavingCustomFormatter Read92_Item(bool isNullable, bool checkType) {
            System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType) {
            if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id94_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
            }
            else
                throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::SerializationTypes.TypeWithTypesHavingCustomFormatter o;
            o = new global::SerializationTypes.TypeWithTypesHavingCustomFormatter();
            bool[] paramsRead = new bool[9];
            while (Reader.MoveToNextAttribute()) {
                if (!IsXmlnsAttribute(Reader.Name)) {
                    UnknownNode((object)o);
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement) {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations5 = 0;
            int readerCount5 = ReaderCount;
            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                    if (!paramsRead[0] && ((object) Reader.LocalName == (object)id108_DateTimeContent && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        {
                            o.@DateTimeContent = ToDateTime(Reader.ReadElementString());
                        }
                        paramsRead[0] = true;
                    }
                    else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id109_QNameContent && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        {
                            o.@QNameContent = ReadElementQualifiedName();
                        }
                        paramsRead[1] = true;
                    }
                    else if (!paramsRead[2] && ((object) Reader.LocalName == (object)id110_DateContent && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        {
                            o.@DateContent = ToDate(Reader.ReadElementString());
                        }
                        paramsRead[2] = true;
                    }
                    else if (!paramsRead[3] && ((object) Reader.LocalName == (object)id111_NameContent && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        {
                            o.@NameContent = ToXmlName(Reader.ReadElementString());
                        }
                        paramsRead[3] = true;
                    }
                    else if (!paramsRead[4] && ((object) Reader.LocalName == (object)id112_NCNameContent && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        {
                            o.@NCNameContent = ToXmlNCName(Reader.ReadElementString());
                        }
                        paramsRead[4] = true;
                    }
                    else if (!paramsRead[5] && ((object) Reader.LocalName == (object)id113_NMTOKENContent && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        {
                            o.@NMTOKENContent = ToXmlNmToken(Reader.ReadElementString());
                        }
                        paramsRead[5] = true;
                    }
                    else if (!paramsRead[6] && ((object) Reader.LocalName == (object)id114_NMTOKENSContent && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        {
                            o.@NMTOKENSContent = ToXmlNmTokens(Reader.ReadElementString());
                        }
                        paramsRead[6] = true;
                    }
                    else if (!paramsRead[7] && ((object) Reader.LocalName == (object)id115_Base64BinaryContent && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        {
                            o.@Base64BinaryContent = ToByteArrayBase64(false);
                        }
                        paramsRead[7] = true;
                    }
                    else if (!paramsRead[8] && ((object) Reader.LocalName == (object)id116_HexBinaryContent && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        {
                            o.@HexBinaryContent = ToByteArrayHex(false);
                        }
                        paramsRead[8] = true;
                    }
                    else {
                        UnknownNode((object)o, @":DateTimeContent, :QNameContent, :DateContent, :NameContent, :NCNameContent, :NMTOKENContent, :NMTOKENSContent, :Base64BinaryContent, :HexBinaryContent");
                    }
                }
                else {
                    UnknownNode((object)o, @":DateTimeContent, :QNameContent, :DateContent, :NameContent, :NCNameContent, :NMTOKENContent, :NMTOKENSContent, :Base64BinaryContent, :HexBinaryContent");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations5, ref readerCount5);
            }
            ReadEndElement();
            return o;
        }

        global::SerializationTypes.KnownTypesThroughConstructorWithValue Read91_Item(bool isNullable, bool checkType) {
            System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType) {
            if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id93_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
            }
            else
                throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
            }
            if (isNull) return null;
            global::SerializationTypes.KnownTypesThroughConstructorWithValue o;
            o = new global::SerializationTypes.KnownTypesThroughConstructorWithValue();
            bool[] paramsRead = new bool[1];
            while (Reader.MoveToNextAttribute()) {
                if (!IsXmlnsAttribute(Reader.Name)) {
                    UnknownNode((object)o);
                }
            }
            Reader.MoveToElement();
            if (Reader.IsEmptyElement) {
                Reader.Skip();
                return o;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int whileIterations6 = 0;
            int readerCount6 = ReaderCount;
            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                    if (!paramsRead[0] && ((object) Reader.LocalName == (object)id117_Value && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        o.@Value = Read1_Object(false, true);
                        paramsRead[0] = true;
                    }
                    else {
                        UnknownNode((object)o, @":Value");
                    }
                }
                else {
                    UnknownNode((object)o, @":Value");
                }
                Reader.MoveToContent();
                CheckReaderCount(ref whileIterations6, ref readerCount6);
            }
            ReadEndElement();
            return o;
        }

        global::System.Object Read1_Object(bool isNullable, bool checkType) {
            System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
            bool isNull = false;
            if (isNullable) isNull = ReadNull();
            if (checkType) {
                if (isNull) {
                    if (xsiType != null) return (global::System.Object)ReadTypedNull(xsiType);
                    else return null;
                }
                if (xsiType == null) {
                    return ReadTypedPrimitive(new System.Xml.XmlQualifiedName("anyType", "http://www.w3.org/2001/XMLSchema"));
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id98_Person && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read96_Person(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id97_TypeWithFieldsOrdered && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read95_TypeWithFieldsOrdered(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id95_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read94_Item(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id94_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read92_Item(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id93_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read91_Item(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id92_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read90_Item(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id91_TypeWithShouldSerializeMethod && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read89_TypeWithShouldSerializeMethod(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id90_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read88_Item(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id89_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read87_Item(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id88_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read86_Item(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id87_TypeWith2DArrayProperty2 && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read85_TypeWith2DArrayProperty2(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id86_TypeWithXmlQualifiedName && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read84_TypeWithXmlQualifiedName(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id85_ServerSettings && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read83_ServerSettings(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id84_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read82_Item(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id82_MyXmlType && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read80_Item(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id81_TypeWithXmlSchemaFormAttribute && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read79_TypeWithXmlSchemaFormAttribute(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id80_TypeWithPropertyNameSpecified && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read78_TypeWithPropertyNameSpecified(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id78_SimpleKnownTypeValue && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read77_SimpleKnownTypeValue(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id77_KnownTypesThroughConstructor && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read76_KnownTypesThroughConstructor(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id76_TypeWithAnyAttribute && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read75_TypeWithAnyAttribute(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id118_XmlSerializerAttributes && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read74_XmlSerializerAttributes(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id66_WithNullables && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read67_WithNullables(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id65_WithEnums && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read63_WithEnums(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id63_WithStruct && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read60_WithStruct(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id64_SomeStruct && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read59_SomeStruct(false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id62_ClassImplementsInterface && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read58_ClassImplementsInterface(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id59_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id60_Item))
                    return Read56_Item(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id58_SimpleDC && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read55_SimpleDC(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id57_TypeWithByteArrayAsXmlText && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read54_TypeWithByteArrayAsXmlText(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id56_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read53_Item(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id53_BaseClassWithSamePropertyName && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read50_BaseClassWithSamePropertyName(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id54_DerivedClassWithSameProperty && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read51_DerivedClassWithSameProperty(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id55_DerivedClassWithSameProperty2 && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read52_DerivedClassWithSameProperty2(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id52_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read49_Item(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id51_WithListOfXElement && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read48_WithListOfXElement(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id50_WithArrayOfXElement && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read47_WithArrayOfXElement(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id49_WithXElementWithNestedXElement && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read46_WithXElementWithNestedXElement(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id48_WithXElement && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read45_WithXElement(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id47_TypeHasArrayOfASerializedAsB && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read44_TypeHasArrayOfASerializedAsB(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id46_TypeB && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read43_TypeB(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id45_TypeA && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read42_TypeA(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id44_BuiltInTypes && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read41_BuiltInTypes(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id43_DCClassWithEnumAndStruct && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read40_DCClassWithEnumAndStruct(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id42_DCStruct && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read39_DCStruct(false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id41_TypeWithEnumMembers && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read38_TypeWithEnumMembers(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id37_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read36_Item(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id36_TypeWithMyCollectionField && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read35_TypeWithMyCollectionField(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id35_StructNotSerializable && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read34_StructNotSerializable(false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id34_TypeWithGetOnlyArrayProperties && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read33_TypeWithGetOnlyArrayProperties(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id33_TypeWithGetSetArrayMembers && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read32_TypeWithGetSetArrayMembers(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id32_SimpleType && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read31_SimpleType(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id31_TypeWithDateTimeStringProperty && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read30_TypeWithDateTimeStringProperty(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id30_Pet && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read29_Pet(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id26_Orchestra && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read26_Orchestra(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id27_Instrument && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read25_Instrument(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id28_Brass && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read27_Brass(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id29_Trumpet && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read28_Trumpet(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id22_BaseClass1 && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read23_BaseClass1(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id23_DerivedClass1 && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read24_DerivedClass1(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id21_AliasedTestType && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read22_AliasedTestType(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id20_OrderedItem && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read21_OrderedItem(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id19_Address && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read20_Address(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id17_PurchaseOrder && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id18_httpwwwcontoso1com))
                    return Read19_PurchaseOrder(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id20_OrderedItem && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id18_httpwwwcontoso1com))
                    return Read18_OrderedItem(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id19_Address && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id18_httpwwwcontoso1com))
                    return Read17_Address(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id15_BaseClass && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read16_BaseClass(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id16_DerivedClass && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read15_DerivedClass(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id14_Employee && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read14_Employee(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id12_Group && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read13_Group(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id13_Vehicle && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read12_Vehicle(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id9_Animal && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read9_Animal(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id10_Dog && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read11_Dog(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id8_TypeWithXmlNodeArrayProperty && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read8_TypeWithXmlNodeArrayProperty(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id7_TypeWithByteProperty && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read7_TypeWithByteProperty(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id6_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read6_Item(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id5_TypeWithTimeSpanProperty && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read5_TypeWithTimeSpanProperty(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id4_TypeWithBinaryProperty && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read4_TypeWithBinaryProperty(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id3_TypeWithXmlDocumentProperty && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read3_TypeWithXmlDocumentProperty(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id1_TypeWithXmlElementProperty && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read2_TypeWithXmlElementProperty(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id11_DogBreed && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    Reader.ReadStartElement();
                    object e = Read10_DogBreed(CollapseWhitespace(Reader.ReadString()));
                    ReadEndElement();
                    return e;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id119_ArrayOfOrderedItem && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id18_httpwwwcontoso1com)) {
                    global::OrderedItem[] a = null;
                    if (!ReadNull()) {
                        global::OrderedItem[] z_0_0 = null;
                        int cz_0_0 = 0;
                        if ((Reader.IsEmptyElement)) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            int whileIterations7 = 0;
                            int readerCount7 = ReaderCount;
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id20_OrderedItem && (object) Reader.NamespaceURI == (object)id18_httpwwwcontoso1com)) {
                                        z_0_0 = (global::OrderedItem[])EnsureArrayIndex(z_0_0, cz_0_0, typeof(global::OrderedItem));z_0_0[cz_0_0++] = Read18_OrderedItem(true, true);
                                    }
                                    else {
                                        UnknownNode(null, @"http://www.contoso1.com:OrderedItem");
                                    }
                                }
                                else {
                                    UnknownNode(null, @"http://www.contoso1.com:OrderedItem");
                                }
                                Reader.MoveToContent();
                                CheckReaderCount(ref whileIterations7, ref readerCount7);
                            }
                        ReadEndElement();
                        }
                        a = (global::OrderedItem[])ShrinkArray(z_0_0, cz_0_0, typeof(global::OrderedItem), false);
                    }
                    return a;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id120_ArrayOfInt && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    global::System.Collections.Generic.List<global::System.Int32> a = null;
                    if (!ReadNull()) {
                        if ((object)(a) == null) a = new global::System.Collections.Generic.List<global::System.Int32>();
                        global::System.Collections.Generic.List<global::System.Int32> z_0_0 = (global::System.Collections.Generic.List<global::System.Int32>)a;
                        if ((Reader.IsEmptyElement)) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            int whileIterations8 = 0;
                            int readerCount8 = ReaderCount;
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id121_int && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        {
                                            z_0_0.Add(System.Xml.XmlConvert.ToInt32(Reader.ReadElementString()));
                                        }
                                    }
                                    else {
                                        UnknownNode(null, @":int");
                                    }
                                }
                                else {
                                    UnknownNode(null, @":int");
                                }
                                Reader.MoveToContent();
                                CheckReaderCount(ref whileIterations8, ref readerCount8);
                            }
                        ReadEndElement();
                        }
                    }
                    return a;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id122_ArrayOfString && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    global::System.Collections.Generic.List<global::System.String> a = null;
                    if (!ReadNull()) {
                        if ((object)(a) == null) a = new global::System.Collections.Generic.List<global::System.String>();
                        global::System.Collections.Generic.List<global::System.String> z_0_0 = (global::System.Collections.Generic.List<global::System.String>)a;
                        if ((Reader.IsEmptyElement)) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            int whileIterations9 = 0;
                            int readerCount9 = ReaderCount;
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id123_string && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        if (ReadNull()) {
                                            z_0_0.Add(null);
                                        }
                                        else {
                                            z_0_0.Add(Reader.ReadElementString());
                                        }
                                    }
                                    else {
                                        UnknownNode(null, @":string");
                                    }
                                }
                                else {
                                    UnknownNode(null, @":string");
                                }
                                Reader.MoveToContent();
                                CheckReaderCount(ref whileIterations9, ref readerCount9);
                            }
                        ReadEndElement();
                        }
                    }
                    return a;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id124_ArrayOfDouble && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    global::System.Collections.Generic.List<global::System.Double> a = null;
                    if (!ReadNull()) {
                        if ((object)(a) == null) a = new global::System.Collections.Generic.List<global::System.Double>();
                        global::System.Collections.Generic.List<global::System.Double> z_0_0 = (global::System.Collections.Generic.List<global::System.Double>)a;
                        if ((Reader.IsEmptyElement)) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            int whileIterations10 = 0;
                            int readerCount10 = ReaderCount;
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id125_double && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        {
                                            z_0_0.Add(System.Xml.XmlConvert.ToDouble(Reader.ReadElementString()));
                                        }
                                    }
                                    else {
                                        UnknownNode(null, @":double");
                                    }
                                }
                                else {
                                    UnknownNode(null, @":double");
                                }
                                Reader.MoveToContent();
                                CheckReaderCount(ref whileIterations10, ref readerCount10);
                            }
                        ReadEndElement();
                        }
                    }
                    return a;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id24_ArrayOfDateTime && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    global::MyCollection1 a = null;
                    if (!ReadNull()) {
                        if ((object)(a) == null) a = new global::MyCollection1();
                        global::MyCollection1 z_0_0 = (global::MyCollection1)a;
                        if ((Reader.IsEmptyElement)) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            int whileIterations11 = 0;
                            int readerCount11 = ReaderCount;
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id25_dateTime && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        {
                                            z_0_0.Add(ToDateTime(Reader.ReadElementString()));
                                        }
                                    }
                                    else {
                                        UnknownNode(null, @":dateTime");
                                    }
                                }
                                else {
                                    UnknownNode(null, @":dateTime");
                                }
                                Reader.MoveToContent();
                                CheckReaderCount(ref whileIterations11, ref readerCount11);
                            }
                        ReadEndElement();
                        }
                    }
                    return a;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id126_ArrayOfInstrument && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    global::Instrument[] a = null;
                    if (!ReadNull()) {
                        global::Instrument[] z_0_0 = null;
                        int cz_0_0 = 0;
                        if ((Reader.IsEmptyElement)) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            int whileIterations12 = 0;
                            int readerCount12 = ReaderCount;
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id27_Instrument && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        z_0_0 = (global::Instrument[])EnsureArrayIndex(z_0_0, cz_0_0, typeof(global::Instrument));z_0_0[cz_0_0++] = Read25_Instrument(true, true);
                                    }
                                    else {
                                        UnknownNode(null, @":Instrument");
                                    }
                                }
                                else {
                                    UnknownNode(null, @":Instrument");
                                }
                                Reader.MoveToContent();
                                CheckReaderCount(ref whileIterations12, ref readerCount12);
                            }
                        ReadEndElement();
                        }
                        a = (global::Instrument[])ShrinkArray(z_0_0, cz_0_0, typeof(global::Instrument), false);
                    }
                    return a;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id127_ArrayOfSimpleType && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    global::SerializationTypes.SimpleType[] a = null;
                    if (!ReadNull()) {
                        global::SerializationTypes.SimpleType[] z_0_0 = null;
                        int cz_0_0 = 0;
                        if ((Reader.IsEmptyElement)) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            int whileIterations13 = 0;
                            int readerCount13 = ReaderCount;
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id32_SimpleType && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        z_0_0 = (global::SerializationTypes.SimpleType[])EnsureArrayIndex(z_0_0, cz_0_0, typeof(global::SerializationTypes.SimpleType));z_0_0[cz_0_0++] = Read31_SimpleType(true, true);
                                    }
                                    else {
                                        UnknownNode(null, @":SimpleType");
                                    }
                                }
                                else {
                                    UnknownNode(null, @":SimpleType");
                                }
                                Reader.MoveToContent();
                                CheckReaderCount(ref whileIterations13, ref readerCount13);
                            }
                        ReadEndElement();
                        }
                        a = (global::SerializationTypes.SimpleType[])ShrinkArray(z_0_0, cz_0_0, typeof(global::SerializationTypes.SimpleType), false);
                    }
                    return a;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id38_ArrayOfAnyType && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    global::SerializationTypes.MyList a = null;
                    if (!ReadNull()) {
                        if ((object)(a) == null) a = new global::SerializationTypes.MyList();
                        global::SerializationTypes.MyList z_0_0 = (global::SerializationTypes.MyList)a;
                        if ((Reader.IsEmptyElement)) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            int whileIterations14 = 0;
                            int readerCount14 = ReaderCount;
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id39_anyType && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        if ((object)(z_0_0) == null) Reader.Skip(); else z_0_0.Add(Read1_Object(true, true));
                                    }
                                    else {
                                        UnknownNode(null, @":anyType");
                                    }
                                }
                                else {
                                    UnknownNode(null, @":anyType");
                                }
                                Reader.MoveToContent();
                                CheckReaderCount(ref whileIterations14, ref readerCount14);
                            }
                        ReadEndElement();
                        }
                    }
                    return a;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id40_MyEnum && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    Reader.ReadStartElement();
                    object e = Read37_MyEnum(CollapseWhitespace(Reader.ReadString()));
                    ReadEndElement();
                    return e;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id128_ArrayOfTypeA && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    global::SerializationTypes.TypeA[] a = null;
                    if (!ReadNull()) {
                        global::SerializationTypes.TypeA[] z_0_0 = null;
                        int cz_0_0 = 0;
                        if ((Reader.IsEmptyElement)) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            int whileIterations15 = 0;
                            int readerCount15 = ReaderCount;
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id45_TypeA && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        z_0_0 = (global::SerializationTypes.TypeA[])EnsureArrayIndex(z_0_0, cz_0_0, typeof(global::SerializationTypes.TypeA));z_0_0[cz_0_0++] = Read42_TypeA(true, true);
                                    }
                                    else {
                                        UnknownNode(null, @":TypeA");
                                    }
                                }
                                else {
                                    UnknownNode(null, @":TypeA");
                                }
                                Reader.MoveToContent();
                                CheckReaderCount(ref whileIterations15, ref readerCount15);
                            }
                        ReadEndElement();
                        }
                        a = (global::SerializationTypes.TypeA[])ShrinkArray(z_0_0, cz_0_0, typeof(global::SerializationTypes.TypeA), false);
                    }
                    return a;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id129_ArrayOfXElement && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    global::System.Xml.Linq.XElement[] a = null;
                    if (!ReadNull()) {
                        global::System.Xml.Linq.XElement[] z_0_0 = null;
                        int cz_0_0 = 0;
                        if ((Reader.IsEmptyElement)) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            int whileIterations16 = 0;
                            int readerCount16 = ReaderCount;
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id130_XElement && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        z_0_0 = (global::System.Xml.Linq.XElement[])EnsureArrayIndex(z_0_0, cz_0_0, typeof(global::System.Xml.Linq.XElement));z_0_0[cz_0_0++] = (global::System.Xml.Linq.XElement)ReadSerializable(( System.Xml.Serialization.IXmlSerializable)System.Activator.CreateInstance(typeof(global::System.Xml.Linq.XElement), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.NonPublic, null, new object[0], null), true
                                        );
                                    }
                                    else {
                                        UnknownNode(null, @":XElement");
                                    }
                                }
                                else {
                                    UnknownNode(null, @":XElement");
                                }
                                Reader.MoveToContent();
                                CheckReaderCount(ref whileIterations16, ref readerCount16);
                            }
                        ReadEndElement();
                        }
                        a = (global::System.Xml.Linq.XElement[])ShrinkArray(z_0_0, cz_0_0, typeof(global::System.Xml.Linq.XElement), false);
                    }
                    return a;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id61_EnumFlags && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    Reader.ReadStartElement();
                    object e = Read57_EnumFlags(CollapseWhitespace(Reader.ReadString()));
                    ReadEndElement();
                    return e;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id70_IntEnum && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    Reader.ReadStartElement();
                    object e = Read61_IntEnum(CollapseWhitespace(Reader.ReadString()));
                    ReadEndElement();
                    return e;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id69_ShortEnum && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    Reader.ReadStartElement();
                    object e = Read62_ShortEnum(CollapseWhitespace(Reader.ReadString()));
                    ReadEndElement();
                    return e;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id67_ByteEnum && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    Reader.ReadStartElement();
                    object e = Read68_ByteEnum(CollapseWhitespace(Reader.ReadString()));
                    ReadEndElement();
                    return e;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id68_SByteEnum && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    Reader.ReadStartElement();
                    object e = Read69_SByteEnum(CollapseWhitespace(Reader.ReadString()));
                    ReadEndElement();
                    return e;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id71_UIntEnum && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    Reader.ReadStartElement();
                    object e = Read70_UIntEnum(CollapseWhitespace(Reader.ReadString()));
                    ReadEndElement();
                    return e;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id72_LongEnum && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    Reader.ReadStartElement();
                    object e = Read71_LongEnum(CollapseWhitespace(Reader.ReadString()));
                    ReadEndElement();
                    return e;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id73_ULongEnum && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    Reader.ReadStartElement();
                    object e = Read72_ULongEnum(CollapseWhitespace(Reader.ReadString()));
                    ReadEndElement();
                    return e;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id75_ItemChoiceType && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    Reader.ReadStartElement();
                    object e = Read73_ItemChoiceType(CollapseWhitespace(Reader.ReadString()));
                    ReadEndElement();
                    return e;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id131_ArrayOfItemChoiceType && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    global::SerializationTypes.ItemChoiceType[] a = null;
                    if (!ReadNull()) {
                        global::SerializationTypes.ItemChoiceType[] z_0_0 = null;
                        int cz_0_0 = 0;
                        if ((Reader.IsEmptyElement)) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            int whileIterations17 = 0;
                            int readerCount17 = ReaderCount;
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id75_ItemChoiceType && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        {
                                            z_0_0 = (global::SerializationTypes.ItemChoiceType[])EnsureArrayIndex(z_0_0, cz_0_0, typeof(global::SerializationTypes.ItemChoiceType));z_0_0[cz_0_0++] = Read73_ItemChoiceType(Reader.ReadElementString());
                                        }
                                    }
                                    else {
                                        UnknownNode(null, @":ItemChoiceType");
                                    }
                                }
                                else {
                                    UnknownNode(null, @":ItemChoiceType");
                                }
                                Reader.MoveToContent();
                                CheckReaderCount(ref whileIterations17, ref readerCount17);
                            }
                        ReadEndElement();
                        }
                        a = (global::SerializationTypes.ItemChoiceType[])ShrinkArray(z_0_0, cz_0_0, typeof(global::SerializationTypes.ItemChoiceType), false);
                    }
                    return a;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id122_ArrayOfString && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id132_httpmynamespace)) {
                    global::System.Object[] a = null;
                    if (!ReadNull()) {
                        global::System.Object[] z_0_0 = null;
                        int cz_0_0 = 0;
                        if ((Reader.IsEmptyElement)) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            int whileIterations18 = 0;
                            int readerCount18 = ReaderCount;
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id123_string && (object) Reader.NamespaceURI == (object)id132_httpmynamespace)) {
                                        if (ReadNull()) {
                                            z_0_0 = (global::System.Object[])EnsureArrayIndex(z_0_0, cz_0_0, typeof(global::System.Object));z_0_0[cz_0_0++] = null;
                                        }
                                        else {
                                            z_0_0 = (global::System.Object[])EnsureArrayIndex(z_0_0, cz_0_0, typeof(global::System.Object));z_0_0[cz_0_0++] = Reader.ReadElementString();
                                        }
                                    }
                                    else {
                                        UnknownNode(null, @"http://mynamespace:string");
                                    }
                                }
                                else {
                                    UnknownNode(null, @"http://mynamespace:string");
                                }
                                Reader.MoveToContent();
                                CheckReaderCount(ref whileIterations18, ref readerCount18);
                            }
                        ReadEndElement();
                        }
                        a = (global::System.Object[])ShrinkArray(z_0_0, cz_0_0, typeof(global::System.Object), false);
                    }
                    return a;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id133_ArrayOfString1 && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    global::System.Collections.Generic.List<global::System.String> a = null;
                    if (!ReadNull()) {
                        if ((object)(a) == null) a = new global::System.Collections.Generic.List<global::System.String>();
                        global::System.Collections.Generic.List<global::System.String> z_0_0 = (global::System.Collections.Generic.List<global::System.String>)a;
                        if ((Reader.IsEmptyElement)) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            int whileIterations19 = 0;
                            int readerCount19 = ReaderCount;
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id134_NoneParameter && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        {
                                            z_0_0.Add(Reader.ReadElementString());
                                        }
                                    }
                                    else {
                                        UnknownNode(null, @":NoneParameter");
                                    }
                                }
                                else {
                                    UnknownNode(null, @":NoneParameter");
                                }
                                Reader.MoveToContent();
                                CheckReaderCount(ref whileIterations19, ref readerCount19);
                            }
                        ReadEndElement();
                        }
                    }
                    return a;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id135_ArrayOfBoolean && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    global::System.Collections.Generic.List<global::System.Boolean> a = null;
                    if (!ReadNull()) {
                        if ((object)(a) == null) a = new global::System.Collections.Generic.List<global::System.Boolean>();
                        global::System.Collections.Generic.List<global::System.Boolean> z_0_0 = (global::System.Collections.Generic.List<global::System.Boolean>)a;
                        if ((Reader.IsEmptyElement)) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            int whileIterations20 = 0;
                            int readerCount20 = ReaderCount;
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id136_QualifiedParameter && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        {
                                            z_0_0.Add(System.Xml.XmlConvert.ToBoolean(Reader.ReadElementString()));
                                        }
                                    }
                                    else {
                                        UnknownNode(null, @":QualifiedParameter");
                                    }
                                }
                                else {
                                    UnknownNode(null, @":QualifiedParameter");
                                }
                                Reader.MoveToContent();
                                CheckReaderCount(ref whileIterations20, ref readerCount20);
                            }
                        ReadEndElement();
                        }
                    }
                    return a;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id137_ArrayOfArrayOfSimpleType && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    global::SerializationTypes.SimpleType[][] a = null;
                    if (!ReadNull()) {
                        global::SerializationTypes.SimpleType[][] z_0_0 = null;
                        int cz_0_0 = 0;
                        if ((Reader.IsEmptyElement)) {
                            Reader.Skip();
                        }
                        else {
                            Reader.ReadStartElement();
                            Reader.MoveToContent();
                            int whileIterations21 = 0;
                            int readerCount21 = ReaderCount;
                            while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                    if (((object) Reader.LocalName == (object)id32_SimpleType && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                        if (!ReadNull()) {
                                            global::SerializationTypes.SimpleType[] z_0_0_0 = null;
                                            int cz_0_0_0 = 0;
                                            if ((Reader.IsEmptyElement)) {
                                                Reader.Skip();
                                            }
                                            else {
                                                Reader.ReadStartElement();
                                                Reader.MoveToContent();
                                                int whileIterations22 = 0;
                                                int readerCount22 = ReaderCount;
                                                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                                        if (((object) Reader.LocalName == (object)id32_SimpleType && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                            z_0_0_0 = (global::SerializationTypes.SimpleType[])EnsureArrayIndex(z_0_0_0, cz_0_0_0, typeof(global::SerializationTypes.SimpleType));z_0_0_0[cz_0_0_0++] = Read31_SimpleType(true, true);
                                                        }
                                                        else {
                                                            UnknownNode(null, @":SimpleType");
                                                        }
                                                    }
                                                    else {
                                                        UnknownNode(null, @":SimpleType");
                                                    }
                                                    Reader.MoveToContent();
                                                    CheckReaderCount(ref whileIterations22, ref readerCount22);
                                                }
                                            ReadEndElement();
                                            }
                                            z_0_0 = (global::SerializationTypes.SimpleType[][])EnsureArrayIndex(z_0_0, cz_0_0, typeof(global::SerializationTypes.SimpleType[]));z_0_0[cz_0_0++] = (global::SerializationTypes.SimpleType[])ShrinkArray(z_0_0_0, cz_0_0_0, typeof(global::SerializationTypes.SimpleType), false);
                                        }
                                    }
                                    else {
                                        UnknownNode(null, @":SimpleType");
                                    }
                                }
                                else {
                                    UnknownNode(null, @":SimpleType");
                                }
                                Reader.MoveToContent();
                                CheckReaderCount(ref whileIterations21, ref readerCount21);
                            }
                        ReadEndElement();
                        }
                        a = (global::SerializationTypes.SimpleType[][])ShrinkArray(z_0_0, cz_0_0, typeof(global::SerializationTypes.SimpleType[]), false);
                    }
                    return a;
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id96_MoreChoices && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                    Reader.ReadStartElement();
                    object e = Read93_MoreChoices(CollapseWhitespace(Reader.ReadString()));
                    ReadEndElement();
                    return e;
                }
                else
                    return ReadTypedPrimitive((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::System.Object o;
                o = new global::System.Object();
                bool[] paramsRead = new bool[0];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations23 = 0;
                int readerCount23 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        UnknownNode((object)o, @"");
                    }
                    else {
                        UnknownNode((object)o, @"");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations23, ref readerCount23);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.SimpleType Read31_SimpleType(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id32_SimpleType && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.SimpleType o;
                o = new global::SerializationTypes.SimpleType();
                bool[] paramsRead = new bool[2];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations24 = 0;
                int readerCount24 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id138_P1 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@P1 = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id139_P2 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@P2 = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                            }
                            paramsRead[1] = true;
                        }
                        else {
                            UnknownNode((object)o, @":P1, :P2");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":P1, :P2");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations24, ref readerCount24);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.ItemChoiceType Read73_ItemChoiceType(string s) {
                switch (s) {
                    case @"None": return global::SerializationTypes.ItemChoiceType.@None;
                    case @"Word": return global::SerializationTypes.ItemChoiceType.@Word;
                    case @"Number": return global::SerializationTypes.ItemChoiceType.@Number;
                    case @"DecimalNumber": return global::SerializationTypes.ItemChoiceType.@DecimalNumber;
                    default: throw CreateUnknownConstantException(s, typeof(global::SerializationTypes.ItemChoiceType));
                }
            }

            global::SerializationTypes.ULongEnum Read72_ULongEnum(string s) {
                switch (s) {
                    case @"Option0": return global::SerializationTypes.ULongEnum.@Option0;
                    case @"Option1": return global::SerializationTypes.ULongEnum.@Option1;
                    case @"Option2": return global::SerializationTypes.ULongEnum.@Option2;
                    default: throw CreateUnknownConstantException(s, typeof(global::SerializationTypes.ULongEnum));
                }
            }

            global::SerializationTypes.LongEnum Read71_LongEnum(string s) {
                switch (s) {
                    case @"Option0": return global::SerializationTypes.LongEnum.@Option0;
                    case @"Option1": return global::SerializationTypes.LongEnum.@Option1;
                    case @"Option2": return global::SerializationTypes.LongEnum.@Option2;
                    default: throw CreateUnknownConstantException(s, typeof(global::SerializationTypes.LongEnum));
                }
            }

            global::SerializationTypes.UIntEnum Read70_UIntEnum(string s) {
                switch (s) {
                    case @"Option0": return global::SerializationTypes.UIntEnum.@Option0;
                    case @"Option1": return global::SerializationTypes.UIntEnum.@Option1;
                    case @"Option2": return global::SerializationTypes.UIntEnum.@Option2;
                    default: throw CreateUnknownConstantException(s, typeof(global::SerializationTypes.UIntEnum));
                }
            }

            global::SerializationTypes.SByteEnum Read69_SByteEnum(string s) {
                switch (s) {
                    case @"Option0": return global::SerializationTypes.SByteEnum.@Option0;
                    case @"Option1": return global::SerializationTypes.SByteEnum.@Option1;
                    case @"Option2": return global::SerializationTypes.SByteEnum.@Option2;
                    default: throw CreateUnknownConstantException(s, typeof(global::SerializationTypes.SByteEnum));
                }
            }

            global::SerializationTypes.ByteEnum Read68_ByteEnum(string s) {
                switch (s) {
                    case @"Option0": return global::SerializationTypes.ByteEnum.@Option0;
                    case @"Option1": return global::SerializationTypes.ByteEnum.@Option1;
                    case @"Option2": return global::SerializationTypes.ByteEnum.@Option2;
                    default: throw CreateUnknownConstantException(s, typeof(global::SerializationTypes.ByteEnum));
                }
            }

            global::SerializationTypes.ShortEnum Read62_ShortEnum(string s) {
                switch (s) {
                    case @"Option0": return global::SerializationTypes.ShortEnum.@Option0;
                    case @"Option1": return global::SerializationTypes.ShortEnum.@Option1;
                    case @"Option2": return global::SerializationTypes.ShortEnum.@Option2;
                    default: throw CreateUnknownConstantException(s, typeof(global::SerializationTypes.ShortEnum));
                }
            }

            global::SerializationTypes.IntEnum Read61_IntEnum(string s) {
                switch (s) {
                    case @"Option0": return global::SerializationTypes.IntEnum.@Option0;
                    case @"Option1": return global::SerializationTypes.IntEnum.@Option1;
                    case @"Option2": return global::SerializationTypes.IntEnum.@Option2;
                    default: throw CreateUnknownConstantException(s, typeof(global::SerializationTypes.IntEnum));
                }
            }

            System.Collections.Hashtable _EnumFlagsValues;

            internal System.Collections.Hashtable EnumFlagsValues {
                get {
                    if ((object)_EnumFlagsValues == null) {
                        System.Collections.Hashtable h = new System.Collections.Hashtable();
                        h.Add(@"One", (long)global::SerializationTypes.EnumFlags.@One);
                        h.Add(@"Two", (long)global::SerializationTypes.EnumFlags.@Two);
                        h.Add(@"Three", (long)global::SerializationTypes.EnumFlags.@Three);
                        h.Add(@"Four", (long)global::SerializationTypes.EnumFlags.@Four);
                        _EnumFlagsValues = h;
                    }
                    return _EnumFlagsValues;
                }
            }

            global::SerializationTypes.EnumFlags Read57_EnumFlags(string s) {
                return (global::SerializationTypes.EnumFlags)ToEnum(s, EnumFlagsValues, @"global::SerializationTypes.EnumFlags");
            }

            global::SerializationTypes.TypeA Read42_TypeA(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id45_TypeA && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeA o;
                o = new global::SerializationTypes.TypeA();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations25 = 0;
                int readerCount25 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id140_Name && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Name = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":Name");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Name");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations25, ref readerCount25);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.MyEnum Read37_MyEnum(string s) {
                switch (s) {
                    case @"One": return global::SerializationTypes.MyEnum.@One;
                    case @"Two": return global::SerializationTypes.MyEnum.@Two;
                    case @"Three": return global::SerializationTypes.MyEnum.@Three;
                    default: throw CreateUnknownConstantException(s, typeof(global::SerializationTypes.MyEnum));
                }
            }

            global::Instrument Read25_Instrument(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id27_Instrument && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id28_Brass && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read27_Brass(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id29_Trumpet && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read28_Trumpet(isNullable, false);
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::Instrument o;
                o = new global::Instrument();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations26 = 0;
                int readerCount26 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id140_Name && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Name = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":Name");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Name");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations26, ref readerCount26);
                }
                ReadEndElement();
                return o;
            }

            global::Trumpet Read28_Trumpet(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id29_Trumpet && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::Trumpet o;
                o = new global::Trumpet();
                bool[] paramsRead = new bool[3];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations27 = 0;
                int readerCount27 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id140_Name && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Name = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id141_IsValved && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@IsValved = System.Xml.XmlConvert.ToBoolean(Reader.ReadElementString());
                            }
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && ((object) Reader.LocalName == (object)id142_Modulation && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Modulation = ToChar(Reader.ReadElementString());
                            }
                            paramsRead[2] = true;
                        }
                        else {
                            UnknownNode((object)o, @":Name, :IsValved, :Modulation");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Name, :IsValved, :Modulation");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations27, ref readerCount27);
                }
                ReadEndElement();
                return o;
            }

            global::Brass Read27_Brass(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id28_Brass && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id29_Trumpet && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read28_Trumpet(isNullable, false);
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::Brass o;
                o = new global::Brass();
                bool[] paramsRead = new bool[2];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations28 = 0;
                int readerCount28 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id140_Name && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Name = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id141_IsValved && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@IsValved = System.Xml.XmlConvert.ToBoolean(Reader.ReadElementString());
                            }
                            paramsRead[1] = true;
                        }
                        else {
                            UnknownNode((object)o, @":Name, :IsValved");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Name, :IsValved");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations28, ref readerCount28);
                }
                ReadEndElement();
                return o;
            }

            global::OrderedItem Read18_OrderedItem(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id20_OrderedItem && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id18_httpwwwcontoso1com)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::OrderedItem o;
                o = new global::OrderedItem();
                bool[] paramsRead = new bool[5];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations29 = 0;
                int readerCount29 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id143_ItemName && (object) Reader.NamespaceURI == (object)id18_httpwwwcontoso1com)) {
                            {
                                o.@ItemName = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id144_Description && (object) Reader.NamespaceURI == (object)id18_httpwwwcontoso1com)) {
                            {
                                o.@Description = Reader.ReadElementString();
                            }
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && ((object) Reader.LocalName == (object)id145_UnitPrice && (object) Reader.NamespaceURI == (object)id18_httpwwwcontoso1com)) {
                            {
                                o.@UnitPrice = System.Xml.XmlConvert.ToDecimal(Reader.ReadElementString());
                            }
                            paramsRead[2] = true;
                        }
                        else if (!paramsRead[3] && ((object) Reader.LocalName == (object)id146_Quantity && (object) Reader.NamespaceURI == (object)id18_httpwwwcontoso1com)) {
                            {
                                o.@Quantity = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                            }
                            paramsRead[3] = true;
                        }
                        else if (!paramsRead[4] && ((object) Reader.LocalName == (object)id147_LineTotal && (object) Reader.NamespaceURI == (object)id18_httpwwwcontoso1com)) {
                            {
                                o.@LineTotal = System.Xml.XmlConvert.ToDecimal(Reader.ReadElementString());
                            }
                            paramsRead[4] = true;
                        }
                        else {
                            UnknownNode((object)o, @"http://www.contoso1.com:ItemName, http://www.contoso1.com:Description, http://www.contoso1.com:UnitPrice, http://www.contoso1.com:Quantity, http://www.contoso1.com:LineTotal");
                        }
                    }
                    else {
                        UnknownNode((object)o, @"http://www.contoso1.com:ItemName, http://www.contoso1.com:Description, http://www.contoso1.com:UnitPrice, http://www.contoso1.com:Quantity, http://www.contoso1.com:LineTotal");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations29, ref readerCount29);
                }
                ReadEndElement();
                return o;
            }

            global::DogBreed Read10_DogBreed(string s) {
                switch (s) {
                    case @"GermanShepherd": return global::DogBreed.@GermanShepherd;
                    case @"LabradorRetriever": return global::DogBreed.@LabradorRetriever;
                    default: throw CreateUnknownConstantException(s, typeof(global::DogBreed));
                }
            }

            global::TypeWithXmlElementProperty Read2_TypeWithXmlElementProperty(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id1_TypeWithXmlElementProperty && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::TypeWithXmlElementProperty o;
                o = new global::TypeWithXmlElementProperty();
                global::System.Xml.XmlElement[] a_0 = null;
                int ca_0 = 0;
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    o.@Elements = (global::System.Xml.XmlElement[])ShrinkArray(a_0, ca_0, typeof(global::System.Xml.XmlElement), true);
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations30 = 0;
                int readerCount30 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        a_0 = (global::System.Xml.XmlElement[])EnsureArrayIndex(a_0, ca_0, typeof(global::System.Xml.XmlElement));a_0[ca_0++] = (global::System.Xml.XmlElement)ReadXmlNode(false);
                    }
                    else {
                        UnknownNode((object)o, @"");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations30, ref readerCount30);
                }
                o.@Elements = (global::System.Xml.XmlElement[])ShrinkArray(a_0, ca_0, typeof(global::System.Xml.XmlElement), true);
                ReadEndElement();
                return o;
            }

            global::TypeWithXmlDocumentProperty Read3_TypeWithXmlDocumentProperty(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id3_TypeWithXmlDocumentProperty && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::TypeWithXmlDocumentProperty o;
                o = new global::TypeWithXmlDocumentProperty();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations31 = 0;
                int readerCount31 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id148_Document && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            o.@Document = (global::System.Xml.XmlDocument)ReadXmlDocument(true);
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":Document");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Document");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations31, ref readerCount31);
                }
                ReadEndElement();
                return o;
            }

            global::TypeWithBinaryProperty Read4_TypeWithBinaryProperty(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id4_TypeWithBinaryProperty && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::TypeWithBinaryProperty o;
                o = new global::TypeWithBinaryProperty();
                bool[] paramsRead = new bool[2];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations32 = 0;
                int readerCount32 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id149_BinaryHexContent && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@BinaryHexContent = ToByteArrayHex(false);
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id150_Base64Content && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Base64Content = ToByteArrayBase64(false);
                            }
                            paramsRead[1] = true;
                        }
                        else {
                            UnknownNode((object)o, @":BinaryHexContent, :Base64Content");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":BinaryHexContent, :Base64Content");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations32, ref readerCount32);
                }
                ReadEndElement();
                return o;
            }

            global::TypeWithTimeSpanProperty Read5_TypeWithTimeSpanProperty(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id5_TypeWithTimeSpanProperty && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::TypeWithTimeSpanProperty o;
                o = new global::TypeWithTimeSpanProperty();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations33 = 0;
                int readerCount33 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id151_TimeSpanProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@TimeSpanProperty = System.Xml.XmlConvert.ToTimeSpan(Reader.ReadElementString());
                            }
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":TimeSpanProperty");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":TimeSpanProperty");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations33, ref readerCount33);
                }
                ReadEndElement();
                return o;
            }

            global::TypeWithDefaultTimeSpanProperty Read6_Item(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id6_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::TypeWithDefaultTimeSpanProperty o;
                o = new global::TypeWithDefaultTimeSpanProperty();
                bool[] paramsRead = new bool[2];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations34 = 0;
                int readerCount34 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id151_TimeSpanProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (Reader.IsEmptyElement) {
                                Reader.Skip();
                            }
                            else {
                                o.@TimeSpanProperty = System.Xml.XmlConvert.ToTimeSpan(Reader.ReadElementString());
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id152_TimeSpanProperty2 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (Reader.IsEmptyElement) {
                                Reader.Skip();
                            }
                            else {
                                o.@TimeSpanProperty2 = System.Xml.XmlConvert.ToTimeSpan(Reader.ReadElementString());
                            }
                            paramsRead[1] = true;
                        }
                        else {
                            UnknownNode((object)o, @":TimeSpanProperty, :TimeSpanProperty2");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":TimeSpanProperty, :TimeSpanProperty2");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations34, ref readerCount34);
                }
                ReadEndElement();
                return o;
            }

            global::TypeWithByteProperty Read7_TypeWithByteProperty(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id7_TypeWithByteProperty && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::TypeWithByteProperty o;
                o = new global::TypeWithByteProperty();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations35 = 0;
                int readerCount35 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id153_ByteProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@ByteProperty = System.Xml.XmlConvert.ToByte(Reader.ReadElementString());
                            }
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":ByteProperty");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":ByteProperty");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations35, ref readerCount35);
                }
                ReadEndElement();
                return o;
            }

            global::TypeWithXmlNodeArrayProperty Read8_TypeWithXmlNodeArrayProperty(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id8_TypeWithXmlNodeArrayProperty && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::TypeWithXmlNodeArrayProperty o;
                o = new global::TypeWithXmlNodeArrayProperty();
                global::System.Xml.XmlNode[] a_0 = null;
                int ca_0 = 0;
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    o.@CDATA = (global::System.Xml.XmlNode[])ShrinkArray(a_0, ca_0, typeof(global::System.Xml.XmlNode), true);
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations36 = 0;
                int readerCount36 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                string tmp = null;
                if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        UnknownNode((object)o, @"");
                    }
                    else if (Reader.NodeType == System.Xml.XmlNodeType.Text || 
                    Reader.NodeType == System.Xml.XmlNodeType.CDATA || 
                    Reader.NodeType == System.Xml.XmlNodeType.Whitespace || 
                    Reader.NodeType == System.Xml.XmlNodeType.SignificantWhitespace) {
                        a_0 = (global::System.Xml.XmlNode[])EnsureArrayIndex(a_0, ca_0, typeof(global::System.Xml.XmlNode));a_0[ca_0++] = (global::System.Xml.XmlNode)Document.CreateTextNode(Reader.ReadString());
                    }
                    else {
                        UnknownNode((object)o, @"");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations36, ref readerCount36);
                }
                o.@CDATA = (global::System.Xml.XmlNode[])ShrinkArray(a_0, ca_0, typeof(global::System.Xml.XmlNode), true);
                ReadEndElement();
                return o;
            }

            global::Dog Read11_Dog(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id10_Dog && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::Dog o;
                o = new global::Dog();
                bool[] paramsRead = new bool[3];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations37 = 0;
                int readerCount37 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id154_Age && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Age = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id140_Name && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Name = Reader.ReadElementString();
                            }
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && ((object) Reader.LocalName == (object)id155_Breed && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Breed = Read10_DogBreed(Reader.ReadElementString());
                            }
                            paramsRead[2] = true;
                        }
                        else {
                            UnknownNode((object)o, @":Age, :Name, :Breed");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Age, :Name, :Breed");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations37, ref readerCount37);
                }
                ReadEndElement();
                return o;
            }

            global::Animal Read9_Animal(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id9_Animal && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id10_Dog && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read11_Dog(isNullable, false);
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::Animal o;
                o = new global::Animal();
                bool[] paramsRead = new bool[2];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations38 = 0;
                int readerCount38 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id154_Age && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Age = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id140_Name && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Name = Reader.ReadElementString();
                            }
                            paramsRead[1] = true;
                        }
                        else {
                            UnknownNode((object)o, @":Age, :Name");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Age, :Name");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations38, ref readerCount38);
                }
                ReadEndElement();
                return o;
            }

            global::Vehicle Read12_Vehicle(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id13_Vehicle && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::Vehicle o;
                o = new global::Vehicle();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations39 = 0;
                int readerCount39 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id156_LicenseNumber && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@LicenseNumber = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":LicenseNumber");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":LicenseNumber");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations39, ref readerCount39);
                }
                ReadEndElement();
                return o;
            }

            global::Group Read13_Group(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id12_Group && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::Group o;
                o = new global::Group();
                bool[] paramsRead = new bool[2];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations40 = 0;
                int readerCount40 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id157_GroupName && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@GroupName = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id158_GroupVehicle && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            o.@GroupVehicle = Read12_Vehicle(false, true);
                            paramsRead[1] = true;
                        }
                        else {
                            UnknownNode((object)o, @":GroupName, :GroupVehicle");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":GroupName, :GroupVehicle");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations40, ref readerCount40);
                }
                ReadEndElement();
                return o;
            }

            global::Employee Read14_Employee(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id14_Employee && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::Employee o;
                o = new global::Employee();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations41 = 0;
                int readerCount41 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id159_EmployeeName && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@EmployeeName = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":EmployeeName");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":EmployeeName");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations41, ref readerCount41);
                }
                ReadEndElement();
                return o;
            }

            global::DerivedClass Read15_DerivedClass(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id16_DerivedClass && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::DerivedClass o;
                o = new global::DerivedClass();
                bool[] paramsRead = new bool[2];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations42 = 0;
                int readerCount42 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id117_Value && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Value = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id160_value && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@value = Reader.ReadElementString();
                            }
                            paramsRead[1] = true;
                        }
                        else {
                            UnknownNode((object)o, @":Value, :value");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Value, :value");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations42, ref readerCount42);
                }
                ReadEndElement();
                return o;
            }

            global::BaseClass Read16_BaseClass(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id15_BaseClass && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id16_DerivedClass && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read15_DerivedClass(isNullable, false);
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::BaseClass o;
                o = new global::BaseClass();
                bool[] paramsRead = new bool[2];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations43 = 0;
                int readerCount43 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id117_Value && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Value = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id160_value && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@value = Reader.ReadElementString();
                            }
                            paramsRead[1] = true;
                        }
                        else {
                            UnknownNode((object)o, @":Value, :value");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Value, :value");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations43, ref readerCount43);
                }
                ReadEndElement();
                return o;
            }

            global::Address Read17_Address(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id19_Address && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id18_httpwwwcontoso1com)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::Address o;
                o = new global::Address();
                bool[] paramsRead = new bool[5];
                while (Reader.MoveToNextAttribute()) {
                    if (!paramsRead[0] && ((object) Reader.LocalName == (object)id140_Name && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        o.@Name = Reader.Value;
                        paramsRead[0] = true;
                    }
                    else if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o, @":Name");
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations44 = 0;
                int readerCount44 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[1] && ((object) Reader.LocalName == (object)id161_Line1 && (object) Reader.NamespaceURI == (object)id18_httpwwwcontoso1com)) {
                            {
                                o.@Line1 = Reader.ReadElementString();
                            }
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && ((object) Reader.LocalName == (object)id162_City && (object) Reader.NamespaceURI == (object)id18_httpwwwcontoso1com)) {
                            {
                                o.@City = Reader.ReadElementString();
                            }
                            paramsRead[2] = true;
                        }
                        else if (!paramsRead[3] && ((object) Reader.LocalName == (object)id163_State && (object) Reader.NamespaceURI == (object)id18_httpwwwcontoso1com)) {
                            {
                                o.@State = Reader.ReadElementString();
                            }
                            paramsRead[3] = true;
                        }
                        else if (!paramsRead[4] && ((object) Reader.LocalName == (object)id164_Zip && (object) Reader.NamespaceURI == (object)id18_httpwwwcontoso1com)) {
                            {
                                o.@Zip = Reader.ReadElementString();
                            }
                            paramsRead[4] = true;
                        }
                        else {
                            UnknownNode((object)o, @"http://www.contoso1.com:Line1, http://www.contoso1.com:City, http://www.contoso1.com:State, http://www.contoso1.com:Zip");
                        }
                    }
                    else {
                        UnknownNode((object)o, @"http://www.contoso1.com:Line1, http://www.contoso1.com:City, http://www.contoso1.com:State, http://www.contoso1.com:Zip");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations44, ref readerCount44);
                }
                ReadEndElement();
                return o;
            }

            global::PurchaseOrder Read19_PurchaseOrder(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id17_PurchaseOrder && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id18_httpwwwcontoso1com)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::PurchaseOrder o;
                o = new global::PurchaseOrder();
                global::OrderedItem[] a_2 = null;
                int ca_2 = 0;
                bool[] paramsRead = new bool[6];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations45 = 0;
                int readerCount45 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id165_ShipTo && (object) Reader.NamespaceURI == (object)id18_httpwwwcontoso1com)) {
                            o.@ShipTo = Read17_Address(false, true);
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id166_OrderDate && (object) Reader.NamespaceURI == (object)id18_httpwwwcontoso1com)) {
                            {
                                o.@OrderDate = Reader.ReadElementString();
                            }
                            paramsRead[1] = true;
                        }
                        else if (((object) Reader.LocalName == (object)id167_Items && (object) Reader.NamespaceURI == (object)id18_httpwwwcontoso1com)) {
                            if (!ReadNull()) {
                                global::OrderedItem[] a_2_0 = null;
                                int ca_2_0 = 0;
                                if ((Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations46 = 0;
                                    int readerCount46 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id20_OrderedItem && (object) Reader.NamespaceURI == (object)id18_httpwwwcontoso1com)) {
                                                a_2_0 = (global::OrderedItem[])EnsureArrayIndex(a_2_0, ca_2_0, typeof(global::OrderedItem));a_2_0[ca_2_0++] = Read18_OrderedItem(true, true);
                                            }
                                            else {
                                                UnknownNode(null, @"http://www.contoso1.com:OrderedItem");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @"http://www.contoso1.com:OrderedItem");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations46, ref readerCount46);
                                    }
                                ReadEndElement();
                                }
                                o.@OrderedItems = (global::OrderedItem[])ShrinkArray(a_2_0, ca_2_0, typeof(global::OrderedItem), false);
                            }
                        }
                        else if (!paramsRead[3] && ((object) Reader.LocalName == (object)id168_SubTotal && (object) Reader.NamespaceURI == (object)id18_httpwwwcontoso1com)) {
                            {
                                o.@SubTotal = System.Xml.XmlConvert.ToDecimal(Reader.ReadElementString());
                            }
                            paramsRead[3] = true;
                        }
                        else if (!paramsRead[4] && ((object) Reader.LocalName == (object)id169_ShipCost && (object) Reader.NamespaceURI == (object)id18_httpwwwcontoso1com)) {
                            {
                                o.@ShipCost = System.Xml.XmlConvert.ToDecimal(Reader.ReadElementString());
                            }
                            paramsRead[4] = true;
                        }
                        else if (!paramsRead[5] && ((object) Reader.LocalName == (object)id170_TotalCost && (object) Reader.NamespaceURI == (object)id18_httpwwwcontoso1com)) {
                            {
                                o.@TotalCost = System.Xml.XmlConvert.ToDecimal(Reader.ReadElementString());
                            }
                            paramsRead[5] = true;
                        }
                        else {
                            UnknownNode((object)o, @"http://www.contoso1.com:ShipTo, http://www.contoso1.com:OrderDate, http://www.contoso1.com:Items, http://www.contoso1.com:SubTotal, http://www.contoso1.com:ShipCost, http://www.contoso1.com:TotalCost");
                        }
                    }
                    else {
                        UnknownNode((object)o, @"http://www.contoso1.com:ShipTo, http://www.contoso1.com:OrderDate, http://www.contoso1.com:Items, http://www.contoso1.com:SubTotal, http://www.contoso1.com:ShipCost, http://www.contoso1.com:TotalCost");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations45, ref readerCount45);
                }
                ReadEndElement();
                return o;
            }

            global::Address Read20_Address(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id19_Address && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::Address o;
                o = new global::Address();
                bool[] paramsRead = new bool[5];
                while (Reader.MoveToNextAttribute()) {
                    if (!paramsRead[0] && ((object) Reader.LocalName == (object)id140_Name && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        o.@Name = Reader.Value;
                        paramsRead[0] = true;
                    }
                    else if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o, @":Name");
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations47 = 0;
                int readerCount47 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[1] && ((object) Reader.LocalName == (object)id161_Line1 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Line1 = Reader.ReadElementString();
                            }
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && ((object) Reader.LocalName == (object)id162_City && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@City = Reader.ReadElementString();
                            }
                            paramsRead[2] = true;
                        }
                        else if (!paramsRead[3] && ((object) Reader.LocalName == (object)id163_State && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@State = Reader.ReadElementString();
                            }
                            paramsRead[3] = true;
                        }
                        else if (!paramsRead[4] && ((object) Reader.LocalName == (object)id164_Zip && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Zip = Reader.ReadElementString();
                            }
                            paramsRead[4] = true;
                        }
                        else {
                            UnknownNode((object)o, @":Line1, :City, :State, :Zip");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Line1, :City, :State, :Zip");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations47, ref readerCount47);
                }
                ReadEndElement();
                return o;
            }

            global::OrderedItem Read21_OrderedItem(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id20_OrderedItem && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::OrderedItem o;
                o = new global::OrderedItem();
                bool[] paramsRead = new bool[5];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations48 = 0;
                int readerCount48 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id143_ItemName && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@ItemName = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id144_Description && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Description = Reader.ReadElementString();
                            }
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && ((object) Reader.LocalName == (object)id145_UnitPrice && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@UnitPrice = System.Xml.XmlConvert.ToDecimal(Reader.ReadElementString());
                            }
                            paramsRead[2] = true;
                        }
                        else if (!paramsRead[3] && ((object) Reader.LocalName == (object)id146_Quantity && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Quantity = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                            }
                            paramsRead[3] = true;
                        }
                        else if (!paramsRead[4] && ((object) Reader.LocalName == (object)id147_LineTotal && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@LineTotal = System.Xml.XmlConvert.ToDecimal(Reader.ReadElementString());
                            }
                            paramsRead[4] = true;
                        }
                        else {
                            UnknownNode((object)o, @":ItemName, :Description, :UnitPrice, :Quantity, :LineTotal");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":ItemName, :Description, :UnitPrice, :Quantity, :LineTotal");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations48, ref readerCount48);
                }
                ReadEndElement();
                return o;
            }

            global::AliasedTestType Read22_AliasedTestType(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id21_AliasedTestType && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::AliasedTestType o;
                o = new global::AliasedTestType();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations49 = 0;
                int readerCount49 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id171_X && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (!ReadNull()) {
                                if ((object)(o.@Aliased) == null) o.@Aliased = new global::System.Collections.Generic.List<global::System.Int32>();
                                global::System.Collections.Generic.List<global::System.Int32> a_0_0 = (global::System.Collections.Generic.List<global::System.Int32>)o.@Aliased;
                                if ((Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations50 = 0;
                                    int readerCount50 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id121_int && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                {
                                                    a_0_0.Add(System.Xml.XmlConvert.ToInt32(Reader.ReadElementString()));
                                                }
                                            }
                                            else {
                                                UnknownNode(null, @":int");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":int");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations50, ref readerCount50);
                                    }
                                ReadEndElement();
                                }
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[0] && ((object) Reader.LocalName == (object)id172_Y && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (!ReadNull()) {
                                if ((object)(o.@Aliased) == null) o.@Aliased = new global::System.Collections.Generic.List<global::System.String>();
                                global::System.Collections.Generic.List<global::System.String> a_0_0 = (global::System.Collections.Generic.List<global::System.String>)o.@Aliased;
                                if ((Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations51 = 0;
                                    int readerCount51 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id123_string && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                if (ReadNull()) {
                                                    a_0_0.Add(null);
                                                }
                                                else {
                                                    a_0_0.Add(Reader.ReadElementString());
                                                }
                                            }
                                            else {
                                                UnknownNode(null, @":string");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":string");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations51, ref readerCount51);
                                    }
                                ReadEndElement();
                                }
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[0] && ((object) Reader.LocalName == (object)id173_Z && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (!ReadNull()) {
                                if ((object)(o.@Aliased) == null) o.@Aliased = new global::System.Collections.Generic.List<global::System.Double>();
                                global::System.Collections.Generic.List<global::System.Double> a_0_0 = (global::System.Collections.Generic.List<global::System.Double>)o.@Aliased;
                                if ((Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations52 = 0;
                                    int readerCount52 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id125_double && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                {
                                                    a_0_0.Add(System.Xml.XmlConvert.ToDouble(Reader.ReadElementString()));
                                                }
                                            }
                                            else {
                                                UnknownNode(null, @":double");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":double");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations52, ref readerCount52);
                                    }
                                ReadEndElement();
                                }
                            }
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":X, :Y, :Z");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":X, :Y, :Z");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations49, ref readerCount49);
                }
                ReadEndElement();
                return o;
            }

            global::DerivedClass1 Read24_DerivedClass1(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id23_DerivedClass1 && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::DerivedClass1 o;
                o = new global::DerivedClass1();
                if ((object)(o.@Prop) == null) o.@Prop = new global::MyCollection1();
                global::MyCollection1 a_0 = (global::MyCollection1)o.@Prop;
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations53 = 0;
                int readerCount53 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (((object) Reader.LocalName == (object)id174_Prop && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                a_0.Add(ToDateTime(Reader.ReadElementString()));
                            }
                        }
                        else {
                            UnknownNode((object)o, @":Prop");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Prop");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations53, ref readerCount53);
                }
                ReadEndElement();
                return o;
            }

            global::BaseClass1 Read23_BaseClass1(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id22_BaseClass1 && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id23_DerivedClass1 && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read24_DerivedClass1(isNullable, false);
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::BaseClass1 o;
                o = new global::BaseClass1();
                if ((object)(o.@Prop) == null) o.@Prop = new global::MyCollection1();
                global::MyCollection1 a_0 = (global::MyCollection1)o.@Prop;
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations54 = 0;
                int readerCount54 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (((object) Reader.LocalName == (object)id174_Prop && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                a_0.Add(ToDateTime(Reader.ReadElementString()));
                            }
                        }
                        else {
                            UnknownNode((object)o, @":Prop");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Prop");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations54, ref readerCount54);
                }
                ReadEndElement();
                return o;
            }

            global::Orchestra Read26_Orchestra(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id26_Orchestra && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::Orchestra o;
                o = new global::Orchestra();
                global::Instrument[] a_0 = null;
                int ca_0 = 0;
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations55 = 0;
                int readerCount55 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (((object) Reader.LocalName == (object)id175_Instruments && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (!ReadNull()) {
                                global::Instrument[] a_0_0 = null;
                                int ca_0_0 = 0;
                                if ((Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations56 = 0;
                                    int readerCount56 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id27_Instrument && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                a_0_0 = (global::Instrument[])EnsureArrayIndex(a_0_0, ca_0_0, typeof(global::Instrument));a_0_0[ca_0_0++] = Read25_Instrument(true, true);
                                            }
                                            else {
                                                UnknownNode(null, @":Instrument");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":Instrument");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations56, ref readerCount56);
                                    }
                                ReadEndElement();
                                }
                                o.@Instruments = (global::Instrument[])ShrinkArray(a_0_0, ca_0_0, typeof(global::Instrument), false);
                            }
                        }
                        else {
                            UnknownNode((object)o, @":Instruments");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Instruments");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations55, ref readerCount55);
                }
                ReadEndElement();
                return o;
            }

            global::Pet Read29_Pet(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id30_Pet && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::Pet o;
                o = new global::Pet();
                bool[] paramsRead = new bool[2];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations57 = 0;
                int readerCount57 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id9_Animal && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Animal = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id176_Comment2 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Comment2 = Reader.ReadElementString();
                            }
                            paramsRead[1] = true;
                        }
                        else {
                            UnknownNode((object)o, @":Animal, :Comment2");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Animal, :Comment2");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations57, ref readerCount57);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeWithDateTimeStringProperty Read30_TypeWithDateTimeStringProperty(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id31_TypeWithDateTimeStringProperty && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeWithDateTimeStringProperty o;
                o = new global::SerializationTypes.TypeWithDateTimeStringProperty();
                bool[] paramsRead = new bool[2];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations58 = 0;
                int readerCount58 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id177_DateTimeString && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@DateTimeString = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id178_CurrentDateTime && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@CurrentDateTime = ToDateTime(Reader.ReadElementString());
                            }
                            paramsRead[1] = true;
                        }
                        else {
                            UnknownNode((object)o, @":DateTimeString, :CurrentDateTime");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":DateTimeString, :CurrentDateTime");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations58, ref readerCount58);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeWithGetSetArrayMembers Read32_TypeWithGetSetArrayMembers(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id33_TypeWithGetSetArrayMembers && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeWithGetSetArrayMembers o;
                o = new global::SerializationTypes.TypeWithGetSetArrayMembers();
                global::SerializationTypes.SimpleType[] a_0 = null;
                int ca_0 = 0;
                global::System.Int32[] a_1 = null;
                int ca_1 = 0;
                global::SerializationTypes.SimpleType[] a_2 = null;
                int ca_2 = 0;
                global::System.Int32[] a_3 = null;
                int ca_3 = 0;
                bool[] paramsRead = new bool[4];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations59 = 0;
                int readerCount59 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (((object) Reader.LocalName == (object)id179_F1 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (!ReadNull()) {
                                global::SerializationTypes.SimpleType[] a_0_0 = null;
                                int ca_0_0 = 0;
                                if ((Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations60 = 0;
                                    int readerCount60 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id32_SimpleType && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                a_0_0 = (global::SerializationTypes.SimpleType[])EnsureArrayIndex(a_0_0, ca_0_0, typeof(global::SerializationTypes.SimpleType));a_0_0[ca_0_0++] = Read31_SimpleType(true, true);
                                            }
                                            else {
                                                UnknownNode(null, @":SimpleType");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":SimpleType");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations60, ref readerCount60);
                                    }
                                ReadEndElement();
                                }
                                o.@F1 = (global::SerializationTypes.SimpleType[])ShrinkArray(a_0_0, ca_0_0, typeof(global::SerializationTypes.SimpleType), false);
                            }
                        }
                        else if (((object) Reader.LocalName == (object)id180_F2 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (!ReadNull()) {
                                global::System.Int32[] a_1_0 = null;
                                int ca_1_0 = 0;
                                if ((Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations61 = 0;
                                    int readerCount61 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id121_int && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                {
                                                    a_1_0 = (global::System.Int32[])EnsureArrayIndex(a_1_0, ca_1_0, typeof(global::System.Int32));a_1_0[ca_1_0++] = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                                                }
                                            }
                                            else {
                                                UnknownNode(null, @":int");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":int");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations61, ref readerCount61);
                                    }
                                ReadEndElement();
                                }
                                o.@F2 = (global::System.Int32[])ShrinkArray(a_1_0, ca_1_0, typeof(global::System.Int32), false);
                            }
                        }
                        else if (((object) Reader.LocalName == (object)id138_P1 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (!ReadNull()) {
                                global::SerializationTypes.SimpleType[] a_2_0 = null;
                                int ca_2_0 = 0;
                                if ((Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations62 = 0;
                                    int readerCount62 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id32_SimpleType && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                a_2_0 = (global::SerializationTypes.SimpleType[])EnsureArrayIndex(a_2_0, ca_2_0, typeof(global::SerializationTypes.SimpleType));a_2_0[ca_2_0++] = Read31_SimpleType(true, true);
                                            }
                                            else {
                                                UnknownNode(null, @":SimpleType");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":SimpleType");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations62, ref readerCount62);
                                    }
                                ReadEndElement();
                                }
                                o.@P1 = (global::SerializationTypes.SimpleType[])ShrinkArray(a_2_0, ca_2_0, typeof(global::SerializationTypes.SimpleType), false);
                            }
                        }
                        else if (((object) Reader.LocalName == (object)id139_P2 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (!ReadNull()) {
                                global::System.Int32[] a_3_0 = null;
                                int ca_3_0 = 0;
                                if ((Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations63 = 0;
                                    int readerCount63 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id121_int && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                {
                                                    a_3_0 = (global::System.Int32[])EnsureArrayIndex(a_3_0, ca_3_0, typeof(global::System.Int32));a_3_0[ca_3_0++] = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                                                }
                                            }
                                            else {
                                                UnknownNode(null, @":int");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":int");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations63, ref readerCount63);
                                    }
                                ReadEndElement();
                                }
                                o.@P2 = (global::System.Int32[])ShrinkArray(a_3_0, ca_3_0, typeof(global::System.Int32), false);
                            }
                        }
                        else {
                            UnknownNode((object)o, @":F1, :F2, :P1, :P2");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":F1, :F2, :P1, :P2");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations59, ref readerCount59);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeWithGetOnlyArrayProperties Read33_TypeWithGetOnlyArrayProperties(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id34_TypeWithGetOnlyArrayProperties && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeWithGetOnlyArrayProperties o;
                o = new global::SerializationTypes.TypeWithGetOnlyArrayProperties();
                bool[] paramsRead = new bool[0];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations64 = 0;
                int readerCount64 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        UnknownNode((object)o, @"");
                    }
                    else {
                        UnknownNode((object)o, @"");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations64, ref readerCount64);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.StructNotSerializable Read34_StructNotSerializable(bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id35_StructNotSerializable && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                global::SerializationTypes.StructNotSerializable o;
                try {
                    o = (global::SerializationTypes.StructNotSerializable)System.Activator.CreateInstance(typeof(global::SerializationTypes.StructNotSerializable), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.NonPublic, null, new object[0], null);
                }
                catch (System.MissingMethodException) {
                    throw CreateInaccessibleConstructorException(@"global::SerializationTypes.StructNotSerializable");
                }
                catch (System.Security.SecurityException) {
                    throw CreateCtorHasSecurityException(@"global::SerializationTypes.StructNotSerializable");
                }
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations65 = 0;
                int readerCount65 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id160_value && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@value = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                            }
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":value");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":value");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations65, ref readerCount65);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeWithMyCollectionField Read35_TypeWithMyCollectionField(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id36_TypeWithMyCollectionField && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeWithMyCollectionField o;
                o = new global::SerializationTypes.TypeWithMyCollectionField();
                if ((object)(o.@Collection) == null) o.@Collection = new global::SerializationTypes.MyCollection<global::System.String>();
                global::SerializationTypes.MyCollection<global::System.String> a_0 = (global::SerializationTypes.MyCollection<global::System.String>)o.@Collection;
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations66 = 0;
                int readerCount66 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (((object) Reader.LocalName == (object)id181_Collection && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (!ReadNull()) {
                                if ((object)(o.@Collection) == null) o.@Collection = new global::SerializationTypes.MyCollection<global::System.String>();
                                global::SerializationTypes.MyCollection<global::System.String> a_0_0 = (global::SerializationTypes.MyCollection<global::System.String>)o.@Collection;
                                if ((Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations67 = 0;
                                    int readerCount67 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id123_string && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                if (ReadNull()) {
                                                    a_0_0.Add(null);
                                                }
                                                else {
                                                    a_0_0.Add(Reader.ReadElementString());
                                                }
                                            }
                                            else {
                                                UnknownNode(null, @":string");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":string");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations67, ref readerCount67);
                                    }
                                ReadEndElement();
                                }
                            }
                        }
                        else {
                            UnknownNode((object)o, @":Collection");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Collection");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations66, ref readerCount66);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeWithReadOnlyMyCollectionProperty Read36_Item(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id37_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeWithReadOnlyMyCollectionProperty o;
                o = new global::SerializationTypes.TypeWithReadOnlyMyCollectionProperty();
                global::SerializationTypes.MyCollection<global::System.String> a_0 = (global::SerializationTypes.MyCollection<global::System.String>)o.@Collection;
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations68 = 0;
                int readerCount68 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (((object) Reader.LocalName == (object)id181_Collection && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (!ReadNull()) {
                                global::SerializationTypes.MyCollection<global::System.String> a_0_0 = (global::SerializationTypes.MyCollection<global::System.String>)o.@Collection;
                                if (((object)(a_0_0) == null) || (Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations69 = 0;
                                    int readerCount69 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id123_string && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                if (ReadNull()) {
                                                    a_0_0.Add(null);
                                                }
                                                else {
                                                    a_0_0.Add(Reader.ReadElementString());
                                                }
                                            }
                                            else {
                                                UnknownNode(null, @":string");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":string");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations69, ref readerCount69);
                                    }
                                ReadEndElement();
                                }
                            }
                        }
                        else {
                            UnknownNode((object)o, @":Collection");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Collection");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations68, ref readerCount68);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeWithEnumMembers Read38_TypeWithEnumMembers(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id41_TypeWithEnumMembers && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeWithEnumMembers o;
                o = new global::SerializationTypes.TypeWithEnumMembers();
                bool[] paramsRead = new bool[2];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations70 = 0;
                int readerCount70 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id179_F1 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@F1 = Read37_MyEnum(Reader.ReadElementString());
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id138_P1 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@P1 = Read37_MyEnum(Reader.ReadElementString());
                            }
                            paramsRead[1] = true;
                        }
                        else {
                            UnknownNode((object)o, @":F1, :P1");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":F1, :P1");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations70, ref readerCount70);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.DCStruct Read39_DCStruct(bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id42_DCStruct && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                global::SerializationTypes.DCStruct o;
                try {
                    o = (global::SerializationTypes.DCStruct)System.Activator.CreateInstance(typeof(global::SerializationTypes.DCStruct), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.NonPublic, null, new object[0], null);
                }
                catch (System.MissingMethodException) {
                    throw CreateInaccessibleConstructorException(@"global::SerializationTypes.DCStruct");
                }
                catch (System.Security.SecurityException) {
                    throw CreateCtorHasSecurityException(@"global::SerializationTypes.DCStruct");
                }
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations71 = 0;
                int readerCount71 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id182_Data && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Data = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":Data");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Data");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations71, ref readerCount71);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.DCClassWithEnumAndStruct Read40_DCClassWithEnumAndStruct(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id43_DCClassWithEnumAndStruct && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.DCClassWithEnumAndStruct o;
                o = new global::SerializationTypes.DCClassWithEnumAndStruct();
                bool[] paramsRead = new bool[2];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations72 = 0;
                int readerCount72 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id183_MyStruct && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            o.@MyStruct = Read39_DCStruct(true);
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id184_MyEnum1 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@MyEnum1 = Read37_MyEnum(Reader.ReadElementString());
                            }
                            paramsRead[1] = true;
                        }
                        else {
                            UnknownNode((object)o, @":MyStruct, :MyEnum1");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":MyStruct, :MyEnum1");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations72, ref readerCount72);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.BuiltInTypes Read41_BuiltInTypes(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id44_BuiltInTypes && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.BuiltInTypes o;
                o = new global::SerializationTypes.BuiltInTypes();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations73 = 0;
                int readerCount73 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id185_ByteArray && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@ByteArray = ToByteArrayBase64(false);
                            }
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":ByteArray");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":ByteArray");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations73, ref readerCount73);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeB Read43_TypeB(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id46_TypeB && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeB o;
                o = new global::SerializationTypes.TypeB();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations74 = 0;
                int readerCount74 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id140_Name && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Name = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":Name");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Name");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations74, ref readerCount74);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeHasArrayOfASerializedAsB Read44_TypeHasArrayOfASerializedAsB(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id47_TypeHasArrayOfASerializedAsB && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeHasArrayOfASerializedAsB o;
                o = new global::SerializationTypes.TypeHasArrayOfASerializedAsB();
                global::SerializationTypes.TypeA[] a_0 = null;
                int ca_0 = 0;
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations75 = 0;
                int readerCount75 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (((object) Reader.LocalName == (object)id167_Items && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (!ReadNull()) {
                                global::SerializationTypes.TypeA[] a_0_0 = null;
                                int ca_0_0 = 0;
                                if ((Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations76 = 0;
                                    int readerCount76 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id45_TypeA && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                a_0_0 = (global::SerializationTypes.TypeA[])EnsureArrayIndex(a_0_0, ca_0_0, typeof(global::SerializationTypes.TypeA));a_0_0[ca_0_0++] = Read42_TypeA(true, true);
                                            }
                                            else {
                                                UnknownNode(null, @":TypeA");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":TypeA");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations76, ref readerCount76);
                                    }
                                ReadEndElement();
                                }
                                o.@Items = (global::SerializationTypes.TypeA[])ShrinkArray(a_0_0, ca_0_0, typeof(global::SerializationTypes.TypeA), false);
                            }
                        }
                        else {
                            UnknownNode((object)o, @":Items");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Items");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations75, ref readerCount75);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.WithXElement Read45_WithXElement(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id48_WithXElement && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.WithXElement o;
                o = new global::SerializationTypes.WithXElement();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations77 = 0;
                int readerCount77 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id186_e && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            o.@e = (global::System.Xml.Linq.XElement)ReadSerializable(( System.Xml.Serialization.IXmlSerializable)System.Activator.CreateInstance(typeof(global::System.Xml.Linq.XElement), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.NonPublic, null, new object[0], null), true
                            );
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":e");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":e");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations77, ref readerCount77);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.WithXElementWithNestedXElement Read46_WithXElementWithNestedXElement(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id49_WithXElementWithNestedXElement && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.WithXElementWithNestedXElement o;
                o = new global::SerializationTypes.WithXElementWithNestedXElement();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations78 = 0;
                int readerCount78 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id187_e1 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            o.@e1 = (global::System.Xml.Linq.XElement)ReadSerializable(( System.Xml.Serialization.IXmlSerializable)System.Activator.CreateInstance(typeof(global::System.Xml.Linq.XElement), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.NonPublic, null, new object[0], null), true
                            );
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":e1");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":e1");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations78, ref readerCount78);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.WithArrayOfXElement Read47_WithArrayOfXElement(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id50_WithArrayOfXElement && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.WithArrayOfXElement o;
                o = new global::SerializationTypes.WithArrayOfXElement();
                global::System.Xml.Linq.XElement[] a_0 = null;
                int ca_0 = 0;
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations79 = 0;
                int readerCount79 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (((object) Reader.LocalName == (object)id188_a && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (!ReadNull()) {
                                global::System.Xml.Linq.XElement[] a_0_0 = null;
                                int ca_0_0 = 0;
                                if ((Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations80 = 0;
                                    int readerCount80 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id130_XElement && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                a_0_0 = (global::System.Xml.Linq.XElement[])EnsureArrayIndex(a_0_0, ca_0_0, typeof(global::System.Xml.Linq.XElement));a_0_0[ca_0_0++] = (global::System.Xml.Linq.XElement)ReadSerializable(( System.Xml.Serialization.IXmlSerializable)System.Activator.CreateInstance(typeof(global::System.Xml.Linq.XElement), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.NonPublic, null, new object[0], null), true
                                                );
                                            }
                                            else {
                                                UnknownNode(null, @":XElement");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":XElement");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations80, ref readerCount80);
                                    }
                                ReadEndElement();
                                }
                                o.@a = (global::System.Xml.Linq.XElement[])ShrinkArray(a_0_0, ca_0_0, typeof(global::System.Xml.Linq.XElement), false);
                            }
                        }
                        else {
                            UnknownNode((object)o, @":a");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":a");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations79, ref readerCount79);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.WithListOfXElement Read48_WithListOfXElement(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id51_WithListOfXElement && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.WithListOfXElement o;
                o = new global::SerializationTypes.WithListOfXElement();
                if ((object)(o.@list) == null) o.@list = new global::System.Collections.Generic.List<global::System.Xml.Linq.XElement>();
                global::System.Collections.Generic.List<global::System.Xml.Linq.XElement> a_0 = (global::System.Collections.Generic.List<global::System.Xml.Linq.XElement>)o.@list;
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations81 = 0;
                int readerCount81 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (((object) Reader.LocalName == (object)id189_list && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (!ReadNull()) {
                                if ((object)(o.@list) == null) o.@list = new global::System.Collections.Generic.List<global::System.Xml.Linq.XElement>();
                                global::System.Collections.Generic.List<global::System.Xml.Linq.XElement> a_0_0 = (global::System.Collections.Generic.List<global::System.Xml.Linq.XElement>)o.@list;
                                if ((Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations82 = 0;
                                    int readerCount82 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id130_XElement && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                a_0_0.Add((global::System.Xml.Linq.XElement)ReadSerializable(( System.Xml.Serialization.IXmlSerializable)System.Activator.CreateInstance(typeof(global::System.Xml.Linq.XElement), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.NonPublic, null, new object[0], null), true
                                                ));
                                            }
                                            else {
                                                UnknownNode(null, @":XElement");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":XElement");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations82, ref readerCount82);
                                    }
                                ReadEndElement();
                                }
                            }
                        }
                        else {
                            UnknownNode((object)o, @":list");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":list");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations81, ref readerCount81);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.@__TypeNameWithSpecialCharacters漢ñ Read49_Item(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id52_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.@__TypeNameWithSpecialCharacters漢ñ o;
                o = new global::SerializationTypes.@__TypeNameWithSpecialCharacters漢ñ();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations83 = 0;
                int readerCount83 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id190_Item && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@PropertyNameWithSpecialCharacters漢ñ = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":PropertyNameWithSpecialCharacters漢ñ");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":PropertyNameWithSpecialCharacters漢ñ");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations83, ref readerCount83);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.DerivedClassWithSameProperty2 Read52_DerivedClassWithSameProperty2(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id55_DerivedClassWithSameProperty2 && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.DerivedClassWithSameProperty2 o;
                o = new global::SerializationTypes.DerivedClassWithSameProperty2();
                if ((object)(o.@ListProperty) == null) o.@ListProperty = new global::System.Collections.Generic.List<global::System.String>();
                global::System.Collections.Generic.List<global::System.String> a_3 = (global::System.Collections.Generic.List<global::System.String>)o.@ListProperty;
                bool[] paramsRead = new bool[4];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations84 = 0;
                int readerCount84 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id191_StringProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@StringProperty = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id192_IntProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@IntProperty = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                            }
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && ((object) Reader.LocalName == (object)id193_DateTimeProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@DateTimeProperty = ToDateTime(Reader.ReadElementString());
                            }
                            paramsRead[2] = true;
                        }
                        else if (((object) Reader.LocalName == (object)id194_ListProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (!ReadNull()) {
                                if ((object)(o.@ListProperty) == null) o.@ListProperty = new global::System.Collections.Generic.List<global::System.String>();
                                global::System.Collections.Generic.List<global::System.String> a_3_0 = (global::System.Collections.Generic.List<global::System.String>)o.@ListProperty;
                                if ((Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations85 = 0;
                                    int readerCount85 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id123_string && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                if (ReadNull()) {
                                                    a_3_0.Add(null);
                                                }
                                                else {
                                                    a_3_0.Add(Reader.ReadElementString());
                                                }
                                            }
                                            else {
                                                UnknownNode(null, @":string");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":string");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations85, ref readerCount85);
                                    }
                                ReadEndElement();
                                }
                            }
                        }
                        else {
                            UnknownNode((object)o, @":StringProperty, :IntProperty, :DateTimeProperty, :ListProperty");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":StringProperty, :IntProperty, :DateTimeProperty, :ListProperty");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations84, ref readerCount84);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.DerivedClassWithSameProperty Read51_DerivedClassWithSameProperty(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id54_DerivedClassWithSameProperty && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id55_DerivedClassWithSameProperty2 && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read52_DerivedClassWithSameProperty2(isNullable, false);
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.DerivedClassWithSameProperty o;
                o = new global::SerializationTypes.DerivedClassWithSameProperty();
                if ((object)(o.@ListProperty) == null) o.@ListProperty = new global::System.Collections.Generic.List<global::System.String>();
                global::System.Collections.Generic.List<global::System.String> a_3 = (global::System.Collections.Generic.List<global::System.String>)o.@ListProperty;
                bool[] paramsRead = new bool[4];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations86 = 0;
                int readerCount86 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id191_StringProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@StringProperty = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id192_IntProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@IntProperty = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                            }
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && ((object) Reader.LocalName == (object)id193_DateTimeProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@DateTimeProperty = ToDateTime(Reader.ReadElementString());
                            }
                            paramsRead[2] = true;
                        }
                        else if (((object) Reader.LocalName == (object)id194_ListProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (!ReadNull()) {
                                if ((object)(o.@ListProperty) == null) o.@ListProperty = new global::System.Collections.Generic.List<global::System.String>();
                                global::System.Collections.Generic.List<global::System.String> a_3_0 = (global::System.Collections.Generic.List<global::System.String>)o.@ListProperty;
                                if ((Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations87 = 0;
                                    int readerCount87 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id123_string && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                if (ReadNull()) {
                                                    a_3_0.Add(null);
                                                }
                                                else {
                                                    a_3_0.Add(Reader.ReadElementString());
                                                }
                                            }
                                            else {
                                                UnknownNode(null, @":string");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":string");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations87, ref readerCount87);
                                    }
                                ReadEndElement();
                                }
                            }
                        }
                        else {
                            UnknownNode((object)o, @":StringProperty, :IntProperty, :DateTimeProperty, :ListProperty");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":StringProperty, :IntProperty, :DateTimeProperty, :ListProperty");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations86, ref readerCount86);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.BaseClassWithSamePropertyName Read50_BaseClassWithSamePropertyName(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id53_BaseClassWithSamePropertyName && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id54_DerivedClassWithSameProperty && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read51_DerivedClassWithSameProperty(isNullable, false);
                else if (((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id55_DerivedClassWithSameProperty2 && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item))
                    return Read52_DerivedClassWithSameProperty2(isNullable, false);
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.BaseClassWithSamePropertyName o;
                o = new global::SerializationTypes.BaseClassWithSamePropertyName();
                if ((object)(o.@ListProperty) == null) o.@ListProperty = new global::System.Collections.Generic.List<global::System.String>();
                global::System.Collections.Generic.List<global::System.String> a_3 = (global::System.Collections.Generic.List<global::System.String>)o.@ListProperty;
                bool[] paramsRead = new bool[4];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations88 = 0;
                int readerCount88 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id191_StringProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@StringProperty = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id192_IntProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@IntProperty = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                            }
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && ((object) Reader.LocalName == (object)id193_DateTimeProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@DateTimeProperty = ToDateTime(Reader.ReadElementString());
                            }
                            paramsRead[2] = true;
                        }
                        else if (((object) Reader.LocalName == (object)id194_ListProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (!ReadNull()) {
                                if ((object)(o.@ListProperty) == null) o.@ListProperty = new global::System.Collections.Generic.List<global::System.String>();
                                global::System.Collections.Generic.List<global::System.String> a_3_0 = (global::System.Collections.Generic.List<global::System.String>)o.@ListProperty;
                                if ((Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations89 = 0;
                                    int readerCount89 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id123_string && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                if (ReadNull()) {
                                                    a_3_0.Add(null);
                                                }
                                                else {
                                                    a_3_0.Add(Reader.ReadElementString());
                                                }
                                            }
                                            else {
                                                UnknownNode(null, @":string");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":string");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations89, ref readerCount89);
                                    }
                                ReadEndElement();
                                }
                            }
                        }
                        else {
                            UnknownNode((object)o, @":StringProperty, :IntProperty, :DateTimeProperty, :ListProperty");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":StringProperty, :IntProperty, :DateTimeProperty, :ListProperty");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations88, ref readerCount88);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeWithDateTimePropertyAsXmlTime Read53_Item(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id56_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeWithDateTimePropertyAsXmlTime o;
                o = new global::SerializationTypes.TypeWithDateTimePropertyAsXmlTime();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations90 = 0;
                int readerCount90 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    string tmp = null;
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        UnknownNode((object)o, @"");
                    }
                    else if (Reader.NodeType == System.Xml.XmlNodeType.Text || 
                    Reader.NodeType == System.Xml.XmlNodeType.CDATA || 
                    Reader.NodeType == System.Xml.XmlNodeType.Whitespace || 
                    Reader.NodeType == System.Xml.XmlNodeType.SignificantWhitespace) {
                        o.@Value = ToTime(Reader.ReadString());
                    }
                    else {
                        UnknownNode((object)o, @"");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations90, ref readerCount90);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeWithByteArrayAsXmlText Read54_TypeWithByteArrayAsXmlText(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id57_TypeWithByteArrayAsXmlText && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeWithByteArrayAsXmlText o;
                o = new global::SerializationTypes.TypeWithByteArrayAsXmlText();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations91 = 0;
                int readerCount91 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    string tmp = null;
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        UnknownNode((object)o, @"");
                    }
                    else if (Reader.NodeType == System.Xml.XmlNodeType.Text || 
                    Reader.NodeType == System.Xml.XmlNodeType.CDATA || 
                    Reader.NodeType == System.Xml.XmlNodeType.Whitespace || 
                    Reader.NodeType == System.Xml.XmlNodeType.SignificantWhitespace) {
                        o.@Value = ToByteArrayBase64(Reader.ReadString());
                    }
                    else {
                        UnknownNode((object)o, @"");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations91, ref readerCount91);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.SimpleDC Read55_SimpleDC(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id58_SimpleDC && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.SimpleDC o;
                o = new global::SerializationTypes.SimpleDC();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations92 = 0;
                int readerCount92 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id182_Data && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Data = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":Data");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Data");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations92, ref readerCount92);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeWithXmlTextAttributeOnArray Read56_Item(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id59_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id60_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeWithXmlTextAttributeOnArray o;
                o = new global::SerializationTypes.TypeWithXmlTextAttributeOnArray();
                global::System.String[] a_0 = null;
                int ca_0 = 0;
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    o.@Text = (global::System.String[])ShrinkArray(a_0, ca_0, typeof(global::System.String), true);
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations93 = 0;
                int readerCount93 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    string tmp = null;
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        UnknownNode((object)o, @"");
                    }
                    else if (Reader.NodeType == System.Xml.XmlNodeType.Text || 
                    Reader.NodeType == System.Xml.XmlNodeType.CDATA || 
                    Reader.NodeType == System.Xml.XmlNodeType.Whitespace || 
                    Reader.NodeType == System.Xml.XmlNodeType.SignificantWhitespace) {
                        a_0 = (global::System.String[])EnsureArrayIndex(a_0, ca_0, typeof(global::System.String));a_0[ca_0++] = Reader.ReadString();
                    }
                    else {
                        UnknownNode((object)o, @"");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations93, ref readerCount93);
                }
                o.@Text = (global::System.String[])ShrinkArray(a_0, ca_0, typeof(global::System.String), true);
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.ClassImplementsInterface Read58_ClassImplementsInterface(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id62_ClassImplementsInterface && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.ClassImplementsInterface o;
                o = new global::SerializationTypes.ClassImplementsInterface();
                bool[] paramsRead = new bool[4];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations94 = 0;
                int readerCount94 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id195_ClassID && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@ClassID = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id196_DisplayName && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@DisplayName = Reader.ReadElementString();
                            }
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && ((object) Reader.LocalName == (object)id197_Id && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Id = Reader.ReadElementString();
                            }
                            paramsRead[2] = true;
                        }
                        else if (!paramsRead[3] && ((object) Reader.LocalName == (object)id198_IsLoaded && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@IsLoaded = System.Xml.XmlConvert.ToBoolean(Reader.ReadElementString());
                            }
                            paramsRead[3] = true;
                        }
                        else {
                            UnknownNode((object)o, @":ClassID, :DisplayName, :Id, :IsLoaded");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":ClassID, :DisplayName, :Id, :IsLoaded");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations94, ref readerCount94);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.SomeStruct Read59_SomeStruct(bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id64_SomeStruct && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                global::SerializationTypes.SomeStruct o;
                try {
                    o = (global::SerializationTypes.SomeStruct)System.Activator.CreateInstance(typeof(global::SerializationTypes.SomeStruct), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.NonPublic, null, new object[0], null);
                }
                catch (System.MissingMethodException) {
                    throw CreateInaccessibleConstructorException(@"global::SerializationTypes.SomeStruct");
                }
                catch (System.Security.SecurityException) {
                    throw CreateCtorHasSecurityException(@"global::SerializationTypes.SomeStruct");
                }
                bool[] paramsRead = new bool[2];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations95 = 0;
                int readerCount95 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id199_A && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@A = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id200_B && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@B = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                            }
                            paramsRead[1] = true;
                        }
                        else {
                            UnknownNode((object)o, @":A, :B");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":A, :B");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations95, ref readerCount95);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.WithStruct Read60_WithStruct(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id63_WithStruct && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.WithStruct o;
                o = new global::SerializationTypes.WithStruct();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations96 = 0;
                int readerCount96 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id201_Some && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            o.@Some = Read59_SomeStruct(true);
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":Some");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Some");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations96, ref readerCount96);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.WithEnums Read63_WithEnums(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id65_WithEnums && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.WithEnums o;
                o = new global::SerializationTypes.WithEnums();
                bool[] paramsRead = new bool[2];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations97 = 0;
                int readerCount97 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id202_Int && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Int = Read61_IntEnum(Reader.ReadElementString());
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id203_Short && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Short = Read62_ShortEnum(Reader.ReadElementString());
                            }
                            paramsRead[1] = true;
                        }
                        else {
                            UnknownNode((object)o, @":Int, :Short");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Int, :Short");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations97, ref readerCount97);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.WithNullables Read67_WithNullables(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id66_WithNullables && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.WithNullables o;
                o = new global::SerializationTypes.WithNullables();
                bool[] paramsRead = new bool[6];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations98 = 0;
                int readerCount98 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id204_Optional && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            o.@Optional = Read64_NullableOfIntEnum(true);
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id205_Optionull && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            o.@Optionull = Read64_NullableOfIntEnum(true);
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && ((object) Reader.LocalName == (object)id206_OptionalInt && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            o.@OptionalInt = Read65_NullableOfInt32(true);
                            paramsRead[2] = true;
                        }
                        else if (!paramsRead[3] && ((object) Reader.LocalName == (object)id207_OptionullInt && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            o.@OptionullInt = Read65_NullableOfInt32(true);
                            paramsRead[3] = true;
                        }
                        else if (!paramsRead[4] && ((object) Reader.LocalName == (object)id208_Struct1 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            o.@Struct1 = Read66_NullableOfSomeStruct(true);
                            paramsRead[4] = true;
                        }
                        else if (!paramsRead[5] && ((object) Reader.LocalName == (object)id209_Struct2 && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            o.@Struct2 = Read66_NullableOfSomeStruct(true);
                            paramsRead[5] = true;
                        }
                        else {
                            UnknownNode((object)o, @":Optional, :Optionull, :OptionalInt, :OptionullInt, :Struct1, :Struct2");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Optional, :Optionull, :OptionalInt, :OptionullInt, :Struct1, :Struct2");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations98, ref readerCount98);
                }
                ReadEndElement();
                return o;
            }

            global::System.Nullable<global::SerializationTypes.SomeStruct> Read66_NullableOfSomeStruct(bool checkType) {
                global::System.Nullable<global::SerializationTypes.SomeStruct> o = default(global::System.Nullable<global::SerializationTypes.SomeStruct>);
                if (ReadNull())
                    return o;
                o = Read59_SomeStruct(true);
                return o;
            }

            global::System.Nullable<global::System.Int32> Read65_NullableOfInt32(bool checkType) {
                global::System.Nullable<global::System.Int32> o = default(global::System.Nullable<global::System.Int32>);
                if (ReadNull())
                    return o;
                {
                    o = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                }
                return o;
            }

            global::System.Nullable<global::SerializationTypes.IntEnum> Read64_NullableOfIntEnum(bool checkType) {
                global::System.Nullable<global::SerializationTypes.IntEnum> o = default(global::System.Nullable<global::SerializationTypes.IntEnum>);
                if (ReadNull())
                    return o;
                {
                    o = Read61_IntEnum(Reader.ReadElementString());
                }
                return o;
            }

            global::SerializationTypes.XmlSerializerAttributes Read74_XmlSerializerAttributes(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id118_XmlSerializerAttributes && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.XmlSerializerAttributes o;
                o = new global::SerializationTypes.XmlSerializerAttributes();
                global::SerializationTypes.ItemChoiceType[] a_2 = null;
                int ca_2 = 0;
                global::System.Object[] a_7 = null;
                int ca_7 = 0;
                bool[] paramsRead = new bool[8];
                while (Reader.MoveToNextAttribute()) {
                    if (!paramsRead[6] && ((object) Reader.LocalName == (object)id210_XmlAttributeName && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        o.@XmlAttributeProperty = System.Xml.XmlConvert.ToInt32(Reader.Value);
                        paramsRead[6] = true;
                    }
                    else if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o, @":XmlAttributeName");
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations99 = 0;
                int readerCount99 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    string tmp = null;
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id211_Word && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@MyChoice = Reader.ReadElementString();
                            }
                            o.@EnumType = global::SerializationTypes.ItemChoiceType.@Word;
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[0] && ((object) Reader.LocalName == (object)id212_Number && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@MyChoice = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                            }
                            o.@EnumType = global::SerializationTypes.ItemChoiceType.@Number;
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[0] && ((object) Reader.LocalName == (object)id213_DecimalNumber && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@MyChoice = System.Xml.XmlConvert.ToDouble(Reader.ReadElementString());
                            }
                            o.@EnumType = global::SerializationTypes.ItemChoiceType.@DecimalNumber;
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id214_XmlIncludeProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            o.@XmlIncludeProperty = Read1_Object(false, true);
                            paramsRead[1] = true;
                        }
                        else if (((object) Reader.LocalName == (object)id215_XmlEnumProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (!ReadNull()) {
                                global::SerializationTypes.ItemChoiceType[] a_2_0 = null;
                                int ca_2_0 = 0;
                                if ((Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations100 = 0;
                                    int readerCount100 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id75_ItemChoiceType && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                {
                                                    a_2_0 = (global::SerializationTypes.ItemChoiceType[])EnsureArrayIndex(a_2_0, ca_2_0, typeof(global::SerializationTypes.ItemChoiceType));a_2_0[ca_2_0++] = Read73_ItemChoiceType(Reader.ReadElementString());
                                                }
                                            }
                                            else {
                                                UnknownNode(null, @":ItemChoiceType");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":ItemChoiceType");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations100, ref readerCount100);
                                    }
                                ReadEndElement();
                                }
                                o.@XmlEnumProperty = (global::SerializationTypes.ItemChoiceType[])ShrinkArray(a_2_0, ca_2_0, typeof(global::SerializationTypes.ItemChoiceType), false);
                            }
                        }
                        else if (!paramsRead[4] && ((object) Reader.LocalName == (object)id216_Item && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@XmlNamespaceDeclarationsProperty = Reader.ReadElementString();
                            }
                            paramsRead[4] = true;
                        }
                        else if (!paramsRead[5] && ((object) Reader.LocalName == (object)id217_XmlElementPropertyNode && (object) Reader.NamespaceURI == (object)id218_httpelement)) {
                            {
                                o.@XmlElementProperty = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                            }
                            paramsRead[5] = true;
                        }
                        else if (((object) Reader.LocalName == (object)id219_CustomXmlArrayProperty && (object) Reader.NamespaceURI == (object)id132_httpmynamespace)) {
                            if (!ReadNull()) {
                                global::System.Object[] a_7_0 = null;
                                int ca_7_0 = 0;
                                if ((Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations101 = 0;
                                    int readerCount101 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id123_string && (object) Reader.NamespaceURI == (object)id132_httpmynamespace)) {
                                                if (ReadNull()) {
                                                    a_7_0 = (global::System.Object[])EnsureArrayIndex(a_7_0, ca_7_0, typeof(global::System.Object));a_7_0[ca_7_0++] = null;
                                                }
                                                else {
                                                    a_7_0 = (global::System.Object[])EnsureArrayIndex(a_7_0, ca_7_0, typeof(global::System.Object));a_7_0[ca_7_0++] = Reader.ReadElementString();
                                                }
                                            }
                                            else {
                                                UnknownNode(null, @"http://mynamespace:string");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @"http://mynamespace:string");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations101, ref readerCount101);
                                    }
                                ReadEndElement();
                                }
                                o.@XmlArrayProperty = (global::System.Object[])ShrinkArray(a_7_0, ca_7_0, typeof(global::System.Object), false);
                            }
                        }
                        else {
                            UnknownNode((object)o, @":Word, :Number, :DecimalNumber, :XmlIncludeProperty, :XmlEnumProperty, :XmlNamespaceDeclarationsProperty, http://element:XmlElementPropertyNode, http://mynamespace:CustomXmlArrayProperty");
                        }
                    }
                    else if (Reader.NodeType == System.Xml.XmlNodeType.Text || 
                    Reader.NodeType == System.Xml.XmlNodeType.CDATA || 
                    Reader.NodeType == System.Xml.XmlNodeType.Whitespace || 
                    Reader.NodeType == System.Xml.XmlNodeType.SignificantWhitespace) {
                        tmp = ReadString(tmp, false);
                        o.@XmlTextProperty = tmp;
                    }
                    else {
                        UnknownNode((object)o, @":Word, :Number, :DecimalNumber, :XmlIncludeProperty, :XmlEnumProperty, :XmlNamespaceDeclarationsProperty, http://element:XmlElementPropertyNode, http://mynamespace:CustomXmlArrayProperty");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations99, ref readerCount99);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeWithAnyAttribute Read75_TypeWithAnyAttribute(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id76_TypeWithAnyAttribute && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeWithAnyAttribute o;
                o = new global::SerializationTypes.TypeWithAnyAttribute();
                global::System.Xml.XmlAttribute[] a_2 = null;
                int ca_2 = 0;
                bool[] paramsRead = new bool[3];
                while (Reader.MoveToNextAttribute()) {
                    if (!paramsRead[1] && ((object) Reader.LocalName == (object)id192_IntProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        o.@IntProperty = System.Xml.XmlConvert.ToInt32(Reader.Value);
                        paramsRead[1] = true;
                    }
                    else if (!IsXmlnsAttribute(Reader.Name)) {
                        System.Xml.XmlAttribute attr = (System.Xml.XmlAttribute) Document.ReadNode(Reader);
                        ParseWsdlArrayType(attr);
                        a_2 = (global::System.Xml.XmlAttribute[])EnsureArrayIndex(a_2, ca_2, typeof(global::System.Xml.XmlAttribute));a_2[ca_2++] = attr;
                    }
                }
                o.@Attributes = (global::System.Xml.XmlAttribute[])ShrinkArray(a_2, ca_2, typeof(global::System.Xml.XmlAttribute), true);
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    o.@Attributes = (global::System.Xml.XmlAttribute[])ShrinkArray(a_2, ca_2, typeof(global::System.Xml.XmlAttribute), true);
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations102 = 0;
                int readerCount102 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id140_Name && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Name = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":Name");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Name");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations102, ref readerCount102);
                }
                o.@Attributes = (global::System.Xml.XmlAttribute[])ShrinkArray(a_2, ca_2, typeof(global::System.Xml.XmlAttribute), true);
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.KnownTypesThroughConstructor Read76_KnownTypesThroughConstructor(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id77_KnownTypesThroughConstructor && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.KnownTypesThroughConstructor o;
                o = new global::SerializationTypes.KnownTypesThroughConstructor();
                bool[] paramsRead = new bool[2];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations103 = 0;
                int readerCount103 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id220_EnumValue && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            o.@EnumValue = Read1_Object(false, true);
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id221_SimpleTypeValue && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            o.@SimpleTypeValue = Read1_Object(false, true);
                            paramsRead[1] = true;
                        }
                        else {
                            UnknownNode((object)o, @":EnumValue, :SimpleTypeValue");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":EnumValue, :SimpleTypeValue");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations103, ref readerCount103);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.SimpleKnownTypeValue Read77_SimpleKnownTypeValue(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id78_SimpleKnownTypeValue && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.SimpleKnownTypeValue o;
                o = new global::SerializationTypes.SimpleKnownTypeValue();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations104 = 0;
                int readerCount104 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id222_StrProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@StrProperty = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":StrProperty");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":StrProperty");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations104, ref readerCount104);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeWithPropertyNameSpecified Read78_TypeWithPropertyNameSpecified(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id80_TypeWithPropertyNameSpecified && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeWithPropertyNameSpecified o;
                o = new global::SerializationTypes.TypeWithPropertyNameSpecified();
                bool[] paramsRead = new bool[2];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations105 = 0;
                int readerCount105 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id223_MyField && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            o.@MyFieldSpecified = true;
                            {
                                o.@MyField = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id224_MyFieldIgnored && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            o.@MyFieldIgnoredSpecified = true;
                            {
                                o.@MyFieldIgnored = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                            }
                            paramsRead[1] = true;
                        }
                        else {
                            UnknownNode((object)o, @":MyField, :MyFieldIgnored");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":MyField, :MyFieldIgnored");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations105, ref readerCount105);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeWithXmlSchemaFormAttribute Read79_TypeWithXmlSchemaFormAttribute(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id81_TypeWithXmlSchemaFormAttribute && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeWithXmlSchemaFormAttribute o;
                o = new global::SerializationTypes.TypeWithXmlSchemaFormAttribute();
                if ((object)(o.@UnqualifiedSchemaFormListProperty) == null) o.@UnqualifiedSchemaFormListProperty = new global::System.Collections.Generic.List<global::System.Int32>();
                global::System.Collections.Generic.List<global::System.Int32> a_0 = (global::System.Collections.Generic.List<global::System.Int32>)o.@UnqualifiedSchemaFormListProperty;
                if ((object)(o.@NoneSchemaFormListProperty) == null) o.@NoneSchemaFormListProperty = new global::System.Collections.Generic.List<global::System.String>();
                global::System.Collections.Generic.List<global::System.String> a_1 = (global::System.Collections.Generic.List<global::System.String>)o.@NoneSchemaFormListProperty;
                if ((object)(o.@QualifiedSchemaFormListProperty) == null) o.@QualifiedSchemaFormListProperty = new global::System.Collections.Generic.List<global::System.Boolean>();
                global::System.Collections.Generic.List<global::System.Boolean> a_2 = (global::System.Collections.Generic.List<global::System.Boolean>)o.@QualifiedSchemaFormListProperty;
                bool[] paramsRead = new bool[3];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations106 = 0;
                int readerCount106 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (((object) Reader.LocalName == (object)id225_Item && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (!ReadNull()) {
                                if ((object)(o.@UnqualifiedSchemaFormListProperty) == null) o.@UnqualifiedSchemaFormListProperty = new global::System.Collections.Generic.List<global::System.Int32>();
                                global::System.Collections.Generic.List<global::System.Int32> a_0_0 = (global::System.Collections.Generic.List<global::System.Int32>)o.@UnqualifiedSchemaFormListProperty;
                                if ((Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations107 = 0;
                                    int readerCount107 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id121_int && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                {
                                                    a_0_0.Add(System.Xml.XmlConvert.ToInt32(Reader.ReadElementString()));
                                                }
                                            }
                                            else {
                                                UnknownNode(null, @":int");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":int");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations107, ref readerCount107);
                                    }
                                ReadEndElement();
                                }
                            }
                        }
                        else if (((object) Reader.LocalName == (object)id226_NoneSchemaFormListProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (!ReadNull()) {
                                if ((object)(o.@NoneSchemaFormListProperty) == null) o.@NoneSchemaFormListProperty = new global::System.Collections.Generic.List<global::System.String>();
                                global::System.Collections.Generic.List<global::System.String> a_1_0 = (global::System.Collections.Generic.List<global::System.String>)o.@NoneSchemaFormListProperty;
                                if ((Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations108 = 0;
                                    int readerCount108 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id134_NoneParameter && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                {
                                                    a_1_0.Add(Reader.ReadElementString());
                                                }
                                            }
                                            else {
                                                UnknownNode(null, @":NoneParameter");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":NoneParameter");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations108, ref readerCount108);
                                    }
                                ReadEndElement();
                                }
                            }
                        }
                        else if (((object) Reader.LocalName == (object)id227_Item && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (!ReadNull()) {
                                if ((object)(o.@QualifiedSchemaFormListProperty) == null) o.@QualifiedSchemaFormListProperty = new global::System.Collections.Generic.List<global::System.Boolean>();
                                global::System.Collections.Generic.List<global::System.Boolean> a_2_0 = (global::System.Collections.Generic.List<global::System.Boolean>)o.@QualifiedSchemaFormListProperty;
                                if ((Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations109 = 0;
                                    int readerCount109 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id136_QualifiedParameter && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                {
                                                    a_2_0.Add(System.Xml.XmlConvert.ToBoolean(Reader.ReadElementString()));
                                                }
                                            }
                                            else {
                                                UnknownNode(null, @":QualifiedParameter");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":QualifiedParameter");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations109, ref readerCount109);
                                    }
                                ReadEndElement();
                                }
                            }
                        }
                        else {
                            UnknownNode((object)o, @":UnqualifiedSchemaFormListProperty, :NoneSchemaFormListProperty, :QualifiedSchemaFormListProperty");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":UnqualifiedSchemaFormListProperty, :NoneSchemaFormListProperty, :QualifiedSchemaFormListProperty");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations106, ref readerCount106);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeWithTypeNameInXmlTypeAttribute Read80_Item(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id82_MyXmlType && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeWithTypeNameInXmlTypeAttribute o;
                o = new global::SerializationTypes.TypeWithTypeNameInXmlTypeAttribute();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!paramsRead[0] && ((object) Reader.LocalName == (object)id228_XmlAttributeForm && (object) Reader.NamespaceURI == (object)id2_Item)) {
                        o.@XmlAttributeForm = Reader.Value;
                        paramsRead[0] = true;
                    }
                    else if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o, @":XmlAttributeForm");
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations110 = 0;
                int readerCount110 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        UnknownNode((object)o, @"");
                    }
                    else {
                        UnknownNode((object)o, @"");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations110, ref readerCount110);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeWithNonPublicDefaultConstructor Read82_Item(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id84_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeWithNonPublicDefaultConstructor o;
                try {
                    o = (global::SerializationTypes.TypeWithNonPublicDefaultConstructor)System.Activator.CreateInstance(typeof(global::SerializationTypes.TypeWithNonPublicDefaultConstructor), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.NonPublic, null, new object[0], null);
                }
                catch (System.MissingMethodException) {
                    throw CreateInaccessibleConstructorException(@"global::SerializationTypes.TypeWithNonPublicDefaultConstructor");
                }
                catch (System.Security.SecurityException) {
                    throw CreateCtorHasSecurityException(@"global::SerializationTypes.TypeWithNonPublicDefaultConstructor");
                }
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations111 = 0;
                int readerCount111 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id140_Name && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Name = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":Name");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Name");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations111, ref readerCount111);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.ServerSettings Read83_ServerSettings(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id85_ServerSettings && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.ServerSettings o;
                o = new global::SerializationTypes.ServerSettings();
                bool[] paramsRead = new bool[2];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations112 = 0;
                int readerCount112 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id229_DS2Root && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@DS2Root = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id230_MetricConfigUrl && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@MetricConfigUrl = Reader.ReadElementString();
                            }
                            paramsRead[1] = true;
                        }
                        else {
                            UnknownNode((object)o, @":DS2Root, :MetricConfigUrl");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":DS2Root, :MetricConfigUrl");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations112, ref readerCount112);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeWithXmlQualifiedName Read84_TypeWithXmlQualifiedName(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id86_TypeWithXmlQualifiedName && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeWithXmlQualifiedName o;
                o = new global::SerializationTypes.TypeWithXmlQualifiedName();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations113 = 0;
                int readerCount113 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id117_Value && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Value = ReadElementQualifiedName();
                            }
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":Value");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Value");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations113, ref readerCount113);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeWith2DArrayProperty2 Read85_TypeWith2DArrayProperty2(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id87_TypeWith2DArrayProperty2 && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeWith2DArrayProperty2 o;
                o = new global::SerializationTypes.TypeWith2DArrayProperty2();
                global::SerializationTypes.SimpleType[][] a_0 = null;
                int ca_0 = 0;
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations114 = 0;
                int readerCount114 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (((object) Reader.LocalName == (object)id231_TwoDArrayOfSimpleType && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (!ReadNull()) {
                                global::SerializationTypes.SimpleType[][] a_0_0 = null;
                                int ca_0_0 = 0;
                                if ((Reader.IsEmptyElement)) {
                                    Reader.Skip();
                                }
                                else {
                                    Reader.ReadStartElement();
                                    Reader.MoveToContent();
                                    int whileIterations115 = 0;
                                    int readerCount115 = ReaderCount;
                                    while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                        if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                            if (((object) Reader.LocalName == (object)id32_SimpleType && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                if (!ReadNull()) {
                                                    global::SerializationTypes.SimpleType[] a_0_0_0 = null;
                                                    int ca_0_0_0 = 0;
                                                    if ((Reader.IsEmptyElement)) {
                                                        Reader.Skip();
                                                    }
                                                    else {
                                                        Reader.ReadStartElement();
                                                        Reader.MoveToContent();
                                                        int whileIterations116 = 0;
                                                        int readerCount116 = ReaderCount;
                                                        while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                                                            if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                                                                if (((object) Reader.LocalName == (object)id32_SimpleType && (object) Reader.NamespaceURI == (object)id2_Item)) {
                                                                    a_0_0_0 = (global::SerializationTypes.SimpleType[])EnsureArrayIndex(a_0_0_0, ca_0_0_0, typeof(global::SerializationTypes.SimpleType));a_0_0_0[ca_0_0_0++] = Read31_SimpleType(true, true);
                                                                }
                                                                else {
                                                                    UnknownNode(null, @":SimpleType");
                                                                }
                                                            }
                                                            else {
                                                                UnknownNode(null, @":SimpleType");
                                                            }
                                                            Reader.MoveToContent();
                                                            CheckReaderCount(ref whileIterations116, ref readerCount116);
                                                        }
                                                    ReadEndElement();
                                                    }
                                                    a_0_0 = (global::SerializationTypes.SimpleType[][])EnsureArrayIndex(a_0_0, ca_0_0, typeof(global::SerializationTypes.SimpleType[]));a_0_0[ca_0_0++] = (global::SerializationTypes.SimpleType[])ShrinkArray(a_0_0_0, ca_0_0_0, typeof(global::SerializationTypes.SimpleType), false);
                                                }
                                            }
                                            else {
                                                UnknownNode(null, @":SimpleType");
                                            }
                                        }
                                        else {
                                            UnknownNode(null, @":SimpleType");
                                        }
                                        Reader.MoveToContent();
                                        CheckReaderCount(ref whileIterations115, ref readerCount115);
                                    }
                                ReadEndElement();
                                }
                                o.@TwoDArrayOfSimpleType = (global::SerializationTypes.SimpleType[][])ShrinkArray(a_0_0, ca_0_0, typeof(global::SerializationTypes.SimpleType[]), false);
                            }
                        }
                        else {
                            UnknownNode((object)o, @":TwoDArrayOfSimpleType");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":TwoDArrayOfSimpleType");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations114, ref readerCount114);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeWithPropertiesHavingDefaultValue Read86_Item(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id88_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeWithPropertiesHavingDefaultValue o;
                o = new global::SerializationTypes.TypeWithPropertiesHavingDefaultValue();
                bool[] paramsRead = new bool[4];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations117 = 0;
                int readerCount117 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id232_EmptyStringProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@EmptyStringProperty = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id191_StringProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@StringProperty = Reader.ReadElementString();
                            }
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && ((object) Reader.LocalName == (object)id192_IntProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (Reader.IsEmptyElement) {
                                Reader.Skip();
                            }
                            else {
                                o.@IntProperty = System.Xml.XmlConvert.ToInt32(Reader.ReadElementString());
                            }
                            paramsRead[2] = true;
                        }
                        else if (!paramsRead[3] && ((object) Reader.LocalName == (object)id233_CharProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (Reader.IsEmptyElement) {
                                Reader.Skip();
                            }
                            else {
                                o.@CharProperty = ToChar(Reader.ReadElementString());
                            }
                            paramsRead[3] = true;
                        }
                        else {
                            UnknownNode((object)o, @":EmptyStringProperty, :StringProperty, :IntProperty, :CharProperty");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":EmptyStringProperty, :StringProperty, :IntProperty, :CharProperty");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations117, ref readerCount117);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeWithEnumPropertyHavingDefaultValue Read87_Item(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id89_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeWithEnumPropertyHavingDefaultValue o;
                o = new global::SerializationTypes.TypeWithEnumPropertyHavingDefaultValue();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations118 = 0;
                int readerCount118 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id234_EnumProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (Reader.IsEmptyElement) {
                                Reader.Skip();
                            }
                            else {
                                o.@EnumProperty = Read61_IntEnum(Reader.ReadElementString());
                            }
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":EnumProperty");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":EnumProperty");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations118, ref readerCount118);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeWithEnumFlagPropertyHavingDefaultValue Read88_Item(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id90_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeWithEnumFlagPropertyHavingDefaultValue o;
                o = new global::SerializationTypes.TypeWithEnumFlagPropertyHavingDefaultValue();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations119 = 0;
                int readerCount119 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id234_EnumProperty && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            if (Reader.IsEmptyElement) {
                                Reader.Skip();
                            }
                            else {
                                o.@EnumProperty = Read57_EnumFlags(Reader.ReadElementString());
                            }
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":EnumProperty");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":EnumProperty");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations119, ref readerCount119);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeWithShouldSerializeMethod Read89_TypeWithShouldSerializeMethod(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id91_TypeWithShouldSerializeMethod && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeWithShouldSerializeMethod o;
                o = new global::SerializationTypes.TypeWithShouldSerializeMethod();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations120 = 0;
                int readerCount120 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id235_Foo && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            {
                                o.@Foo = Reader.ReadElementString();
                            }
                            paramsRead[0] = true;
                        }
                        else {
                            UnknownNode((object)o, @":Foo");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":Foo");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations120, ref readerCount120);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.KnownTypesThroughConstructorWithArrayProperties Read90_Item(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id92_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.KnownTypesThroughConstructorWithArrayProperties o;
                o = new global::SerializationTypes.KnownTypesThroughConstructorWithArrayProperties();
                bool[] paramsRead = new bool[2];
                while (Reader.MoveToNextAttribute()) {
                    if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o);
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations121 = 0;
                int readerCount121 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        if (!paramsRead[0] && ((object) Reader.LocalName == (object)id236_StringArrayValue && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            o.@StringArrayValue = Read1_Object(false, true);
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && ((object) Reader.LocalName == (object)id237_IntArrayValue && (object) Reader.NamespaceURI == (object)id2_Item)) {
                            o.@IntArrayValue = Read1_Object(false, true);
                            paramsRead[1] = true;
                        }
                        else {
                            UnknownNode((object)o, @":StringArrayValue, :IntArrayValue");
                        }
                    }
                    else {
                        UnknownNode((object)o, @":StringArrayValue, :IntArrayValue");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations121, ref readerCount121);
                }
                ReadEndElement();
                return o;
            }

            global::SerializationTypes.TypeWithSchemaFormInXmlAttribute Read81_Item(bool isNullable, bool checkType) {
                System.Xml.XmlQualifiedName xsiType = checkType ? GetXsiType() : null;
                bool isNull = false;
                if (isNullable) isNull = ReadNull();
                if (checkType) {
                if (xsiType == null || ((object) ((System.Xml.XmlQualifiedName)xsiType).Name == (object)id2_Item && (object) ((System.Xml.XmlQualifiedName)xsiType).Namespace == (object)id2_Item)) {
                }
                else
                    throw CreateUnknownTypeException((System.Xml.XmlQualifiedName)xsiType);
                }
                if (isNull) return null;
                global::SerializationTypes.TypeWithSchemaFormInXmlAttribute o;
                o = new global::SerializationTypes.TypeWithSchemaFormInXmlAttribute();
                bool[] paramsRead = new bool[1];
                while (Reader.MoveToNextAttribute()) {
                    if (!paramsRead[0] && ((object) Reader.LocalName == (object)id238_TestProperty && (object) Reader.NamespaceURI == (object)id239_httptestcom)) {
                        o.@TestProperty = Reader.Value;
                        paramsRead[0] = true;
                    }
                    else if (!IsXmlnsAttribute(Reader.Name)) {
                        UnknownNode((object)o, @"http://test.com:TestProperty");
                    }
                }
                Reader.MoveToElement();
                if (Reader.IsEmptyElement) {
                    Reader.Skip();
                    return o;
                }
                Reader.ReadStartElement();
                Reader.MoveToContent();
                int whileIterations122 = 0;
                int readerCount122 = ReaderCount;
                while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.NodeType != System.Xml.XmlNodeType.None) {
                    if (Reader.NodeType == System.Xml.XmlNodeType.Element) {
                        UnknownNode((object)o, @"");
                    }
                    else {
                        UnknownNode((object)o, @"");
                    }
                    Reader.MoveToContent();
                    CheckReaderCount(ref whileIterations122, ref readerCount122);
                }
                ReadEndElement();
                return o;
            }

            protected override void InitCallbacks() {
            }

            string id3_TypeWithXmlDocumentProperty;
            string id51_WithListOfXElement;
            string id89_Item;
            string id153_ByteProperty;
            string id5_TypeWithTimeSpanProperty;
            string id131_ArrayOfItemChoiceType;
            string id21_AliasedTestType;
            string id194_ListProperty;
            string id73_ULongEnum;
            string id188_a;
            string id230_MetricConfigUrl;
            string id87_TypeWith2DArrayProperty2;
            string id61_EnumFlags;
            string id14_Employee;
            string id162_City;
            string id173_Z;
            string id145_UnitPrice;
            string id204_Optional;
            string id175_Instruments;
            string id48_WithXElement;
            string id62_ClassImplementsInterface;
            string id67_ByteEnum;
            string id142_Modulation;
            string id16_DerivedClass;
            string id218_httpelement;
            string id47_TypeHasArrayOfASerializedAsB;
            string id81_TypeWithXmlSchemaFormAttribute;
            string id163_State;
            string id103_IntField2;
            string id91_TypeWithShouldSerializeMethod;
            string id78_SimpleKnownTypeValue;
            string id65_WithEnums;
            string id99_FirstName;
            string id102_IntField1;
            string id185_ByteArray;
            string id52_Item;
            string id77_KnownTypesThroughConstructor;
            string id116_HexBinaryContent;
            string id196_DisplayName;
            string id108_DateTimeContent;
            string id74_AttributeTesting;
            string id193_DateTimeProperty;
            string id55_DerivedClassWithSameProperty2;
            string id212_Number;
            string id228_XmlAttributeForm;
            string id94_Item;
            string id222_StrProperty;
            string id106_Item;
            string id208_Struct1;
            string id176_Comment2;
            string id198_IsLoaded;
            string id166_OrderDate;
            string id38_ArrayOfAnyType;
            string id93_Item;
            string id239_httptestcom;
            string id4_TypeWithBinaryProperty;
            string id118_XmlSerializerAttributes;
            string id97_TypeWithFieldsOrdered;
            string id223_MyField;
            string id161_Line1;
            string id147_LineTotal;
            string id167_Items;
            string id236_StringArrayValue;
            string id11_DogBreed;
            string id138_P1;
            string id76_TypeWithAnyAttribute;
            string id90_Item;
            string id197_Id;
            string id177_DateTimeString;
            string id134_NoneParameter;
            string id49_WithXElementWithNestedXElement;
            string id165_ShipTo;
            string id151_TimeSpanProperty;
            string id23_DerivedClass1;
            string id195_ClassID;
            string id18_httpwwwcontoso1com;
            string id169_ShipCost;
            string id226_NoneSchemaFormListProperty;
            string id220_EnumValue;
            string id144_Description;
            string id100_MiddleName;
            string id71_UIntEnum;
            string id130_XElement;
            string id112_NCNameContent;
            string id2_Item;
            string id171_X;
            string id182_Data;
            string id98_Person;
            string id15_BaseClass;
            string id168_SubTotal;
            string id211_Word;
            string id199_A;
            string id143_ItemName;
            string id95_Item;
            string id28_Brass;
            string id219_CustomXmlArrayProperty;
            string id180_F2;
            string id39_anyType;
            string id181_Collection;
            string id20_OrderedItem;
            string id178_CurrentDateTime;
            string id137_ArrayOfArrayOfSimpleType;
            string id150_Base64Content;
            string id80_TypeWithPropertyNameSpecified;
            string id84_Item;
            string id41_TypeWithEnumMembers;
            string id40_MyEnum;
            string id59_Item;
            string id63_WithStruct;
            string id136_QualifiedParameter;
            string id64_SomeStruct;
            string id200_B;
            string id133_ArrayOfString1;
            string id85_ServerSettings;
            string id156_LicenseNumber;
            string id26_Orchestra;
            string id126_ArrayOfInstrument;
            string id221_SimpleTypeValue;
            string id120_ArrayOfInt;
            string id111_NameContent;
            string id217_XmlElementPropertyNode;
            string id1_TypeWithXmlElementProperty;
            string id58_SimpleDC;
            string id207_OptionullInt;
            string id35_StructNotSerializable;
            string id205_Optionull;
            string id152_TimeSpanProperty2;
            string id184_MyEnum1;
            string id117_Value;
            string id232_EmptyStringProperty;
            string id96_MoreChoices;
            string id53_BaseClassWithSamePropertyName;
            string id110_DateContent;
            string id42_DCStruct;
            string id72_LongEnum;
            string id75_ItemChoiceType;
            string id159_EmployeeName;
            string id122_ArrayOfString;
            string id170_TotalCost;
            string id139_P2;
            string id129_ArrayOfXElement;
            string id6_Item;
            string id114_NMTOKENSContent;
            string id121_int;
            string id8_TypeWithXmlNodeArrayProperty;
            string id12_Group;
            string id141_IsValved;
            string id216_Item;
            string id155_Breed;
            string id179_F1;
            string id124_ArrayOfDouble;
            string id210_XmlAttributeName;
            string id107_Amount;
            string id237_IntArrayValue;
            string id233_CharProperty;
            string id164_Zip;
            string id70_IntEnum;
            string id231_TwoDArrayOfSimpleType;
            string id229_DS2Root;
            string id172_Y;
            string id202_Int;
            string id113_NMTOKENContent;
            string id50_WithArrayOfXElement;
            string id132_httpmynamespace;
            string id128_ArrayOfTypeA;
            string id9_Animal;
            string id146_Quantity;
            string id183_MyStruct;
            string id83_Item;
            string id43_DCClassWithEnumAndStruct;
            string id203_Short;
            string id215_XmlEnumProperty;
            string id154_Age;
            string id148_Document;
            string id54_DerivedClassWithSameProperty;
            string id45_TypeA;
            string id25_dateTime;
            string id56_Item;
            string id109_QNameContent;
            string id192_IntProperty;
            string id119_ArrayOfOrderedItem;
            string id187_e1;
            string id33_TypeWithGetSetArrayMembers;
            string id60_Item;
            string id86_TypeWithXmlQualifiedName;
            string id214_XmlIncludeProperty;
            string id127_ArrayOfSimpleType;
            string id234_EnumProperty;
            string id160_value;
            string id27_Instrument;
            string id115_Base64BinaryContent;
            string id82_MyXmlType;
            string id125_double;
            string id190_Item;
            string id24_ArrayOfDateTime;
            string id238_TestProperty;
            string id22_BaseClass1;
            string id79_Item;
            string id225_Item;
            string id104_StringField2;
            string id92_Item;
            string id44_BuiltInTypes;
            string id29_Trumpet;
            string id158_GroupVehicle;
            string id46_TypeB;
            string id66_WithNullables;
            string id37_Item;
            string id30_Pet;
            string id88_Item;
            string id186_e;
            string id31_TypeWithDateTimeStringProperty;
            string id209_Struct2;
            string id36_TypeWithMyCollectionField;
            string id69_ShortEnum;
            string id101_LastName;
            string id235_Foo;
            string id68_SByteEnum;
            string id57_TypeWithByteArrayAsXmlText;
            string id191_StringProperty;
            string id227_Item;
            string id213_DecimalNumber;
            string id10_Dog;
            string id140_Name;
            string id135_ArrayOfBoolean;
            string id201_Some;
            string id32_SimpleType;
            string id123_string;
            string id19_Address;
            string id157_GroupName;
            string id206_OptionalInt;
            string id105_StringField1;
            string id13_Vehicle;
            string id7_TypeWithByteProperty;
            string id224_MyFieldIgnored;
            string id174_Prop;
            string id149_BinaryHexContent;
            string id34_TypeWithGetOnlyArrayProperties;
            string id17_PurchaseOrder;
            string id189_list;

            protected override void InitIDs() {
                id3_TypeWithXmlDocumentProperty = Reader.NameTable.Add(@"TypeWithXmlDocumentProperty");
                id51_WithListOfXElement = Reader.NameTable.Add(@"WithListOfXElement");
                id89_Item = Reader.NameTable.Add(@"TypeWithEnumPropertyHavingDefaultValue");
                id153_ByteProperty = Reader.NameTable.Add(@"ByteProperty");
                id5_TypeWithTimeSpanProperty = Reader.NameTable.Add(@"TypeWithTimeSpanProperty");
                id131_ArrayOfItemChoiceType = Reader.NameTable.Add(@"ArrayOfItemChoiceType");
                id21_AliasedTestType = Reader.NameTable.Add(@"AliasedTestType");
                id194_ListProperty = Reader.NameTable.Add(@"ListProperty");
                id73_ULongEnum = Reader.NameTable.Add(@"ULongEnum");
                id188_a = Reader.NameTable.Add(@"a");
                id230_MetricConfigUrl = Reader.NameTable.Add(@"MetricConfigUrl");
                id87_TypeWith2DArrayProperty2 = Reader.NameTable.Add(@"TypeWith2DArrayProperty2");
                id61_EnumFlags = Reader.NameTable.Add(@"EnumFlags");
                id14_Employee = Reader.NameTable.Add(@"Employee");
                id162_City = Reader.NameTable.Add(@"City");
                id173_Z = Reader.NameTable.Add(@"Z");
                id145_UnitPrice = Reader.NameTable.Add(@"UnitPrice");
                id204_Optional = Reader.NameTable.Add(@"Optional");
                id175_Instruments = Reader.NameTable.Add(@"Instruments");
                id48_WithXElement = Reader.NameTable.Add(@"WithXElement");
                id62_ClassImplementsInterface = Reader.NameTable.Add(@"ClassImplementsInterface");
                id67_ByteEnum = Reader.NameTable.Add(@"ByteEnum");
                id142_Modulation = Reader.NameTable.Add(@"Modulation");
                id16_DerivedClass = Reader.NameTable.Add(@"DerivedClass");
                id218_httpelement = Reader.NameTable.Add(@"http://element");
                id47_TypeHasArrayOfASerializedAsB = Reader.NameTable.Add(@"TypeHasArrayOfASerializedAsB");
                id81_TypeWithXmlSchemaFormAttribute = Reader.NameTable.Add(@"TypeWithXmlSchemaFormAttribute");
                id163_State = Reader.NameTable.Add(@"State");
                id103_IntField2 = Reader.NameTable.Add(@"IntField2");
                id91_TypeWithShouldSerializeMethod = Reader.NameTable.Add(@"TypeWithShouldSerializeMethod");
                id78_SimpleKnownTypeValue = Reader.NameTable.Add(@"SimpleKnownTypeValue");
                id65_WithEnums = Reader.NameTable.Add(@"WithEnums");
                id99_FirstName = Reader.NameTable.Add(@"FirstName");
                id102_IntField1 = Reader.NameTable.Add(@"IntField1");
                id185_ByteArray = Reader.NameTable.Add(@"ByteArray");
                id52_Item = Reader.NameTable.Add(@"__TypeNameWithSpecialCharacters漢ñ");
                id77_KnownTypesThroughConstructor = Reader.NameTable.Add(@"KnownTypesThroughConstructor");
                id116_HexBinaryContent = Reader.NameTable.Add(@"HexBinaryContent");
                id196_DisplayName = Reader.NameTable.Add(@"DisplayName");
                id108_DateTimeContent = Reader.NameTable.Add(@"DateTimeContent");
                id74_AttributeTesting = Reader.NameTable.Add(@"AttributeTesting");
                id193_DateTimeProperty = Reader.NameTable.Add(@"DateTimeProperty");
                id55_DerivedClassWithSameProperty2 = Reader.NameTable.Add(@"DerivedClassWithSameProperty2");
                id212_Number = Reader.NameTable.Add(@"Number");
                id228_XmlAttributeForm = Reader.NameTable.Add(@"XmlAttributeForm");
                id94_Item = Reader.NameTable.Add(@"TypeWithTypesHavingCustomFormatter");
                id222_StrProperty = Reader.NameTable.Add(@"StrProperty");
                id106_Item = Reader.NameTable.Add(@"Item");
                id208_Struct1 = Reader.NameTable.Add(@"Struct1");
                id176_Comment2 = Reader.NameTable.Add(@"Comment2");
                id198_IsLoaded = Reader.NameTable.Add(@"IsLoaded");
                id166_OrderDate = Reader.NameTable.Add(@"OrderDate");
                id38_ArrayOfAnyType = Reader.NameTable.Add(@"ArrayOfAnyType");
                id93_Item = Reader.NameTable.Add(@"KnownTypesThroughConstructorWithValue");
                id239_httptestcom = Reader.NameTable.Add(@"http://test.com");
                id4_TypeWithBinaryProperty = Reader.NameTable.Add(@"TypeWithBinaryProperty");
                id118_XmlSerializerAttributes = Reader.NameTable.Add(@"XmlSerializerAttributes");
                id97_TypeWithFieldsOrdered = Reader.NameTable.Add(@"TypeWithFieldsOrdered");
                id223_MyField = Reader.NameTable.Add(@"MyField");
                id161_Line1 = Reader.NameTable.Add(@"Line1");
                id147_LineTotal = Reader.NameTable.Add(@"LineTotal");
                id167_Items = Reader.NameTable.Add(@"Items");
                id236_StringArrayValue = Reader.NameTable.Add(@"StringArrayValue");
                id11_DogBreed = Reader.NameTable.Add(@"DogBreed");
                id138_P1 = Reader.NameTable.Add(@"P1");
                id76_TypeWithAnyAttribute = Reader.NameTable.Add(@"TypeWithAnyAttribute");
                id90_Item = Reader.NameTable.Add(@"TypeWithEnumFlagPropertyHavingDefaultValue");
                id197_Id = Reader.NameTable.Add(@"Id");
                id177_DateTimeString = Reader.NameTable.Add(@"DateTimeString");
                id134_NoneParameter = Reader.NameTable.Add(@"NoneParameter");
                id49_WithXElementWithNestedXElement = Reader.NameTable.Add(@"WithXElementWithNestedXElement");
                id165_ShipTo = Reader.NameTable.Add(@"ShipTo");
                id151_TimeSpanProperty = Reader.NameTable.Add(@"TimeSpanProperty");
                id23_DerivedClass1 = Reader.NameTable.Add(@"DerivedClass1");
                id195_ClassID = Reader.NameTable.Add(@"ClassID");
                id18_httpwwwcontoso1com = Reader.NameTable.Add(@"http://www.contoso1.com");
                id169_ShipCost = Reader.NameTable.Add(@"ShipCost");
                id226_NoneSchemaFormListProperty = Reader.NameTable.Add(@"NoneSchemaFormListProperty");
                id220_EnumValue = Reader.NameTable.Add(@"EnumValue");
                id144_Description = Reader.NameTable.Add(@"Description");
                id100_MiddleName = Reader.NameTable.Add(@"MiddleName");
                id71_UIntEnum = Reader.NameTable.Add(@"UIntEnum");
                id130_XElement = Reader.NameTable.Add(@"XElement");
                id112_NCNameContent = Reader.NameTable.Add(@"NCNameContent");
                id2_Item = Reader.NameTable.Add(@"");
                id171_X = Reader.NameTable.Add(@"X");
                id182_Data = Reader.NameTable.Add(@"Data");
                id98_Person = Reader.NameTable.Add(@"Person");
                id15_BaseClass = Reader.NameTable.Add(@"BaseClass");
                id168_SubTotal = Reader.NameTable.Add(@"SubTotal");
                id211_Word = Reader.NameTable.Add(@"Word");
                id199_A = Reader.NameTable.Add(@"A");
                id143_ItemName = Reader.NameTable.Add(@"ItemName");
                id95_Item = Reader.NameTable.Add(@"TypeWithArrayPropertyHavingChoice");
                id28_Brass = Reader.NameTable.Add(@"Brass");
                id219_CustomXmlArrayProperty = Reader.NameTable.Add(@"CustomXmlArrayProperty");
                id180_F2 = Reader.NameTable.Add(@"F2");
                id39_anyType = Reader.NameTable.Add(@"anyType");
                id181_Collection = Reader.NameTable.Add(@"Collection");
                id20_OrderedItem = Reader.NameTable.Add(@"OrderedItem");
                id178_CurrentDateTime = Reader.NameTable.Add(@"CurrentDateTime");
                id137_ArrayOfArrayOfSimpleType = Reader.NameTable.Add(@"ArrayOfArrayOfSimpleType");
                id150_Base64Content = Reader.NameTable.Add(@"Base64Content");
                id80_TypeWithPropertyNameSpecified = Reader.NameTable.Add(@"TypeWithPropertyNameSpecified");
                id84_Item = Reader.NameTable.Add(@"TypeWithNonPublicDefaultConstructor");
                id41_TypeWithEnumMembers = Reader.NameTable.Add(@"TypeWithEnumMembers");
                id40_MyEnum = Reader.NameTable.Add(@"MyEnum");
                id59_Item = Reader.NameTable.Add(@"TypeWithXmlTextAttributeOnArray");
                id63_WithStruct = Reader.NameTable.Add(@"WithStruct");
                id136_QualifiedParameter = Reader.NameTable.Add(@"QualifiedParameter");
                id64_SomeStruct = Reader.NameTable.Add(@"SomeStruct");
                id200_B = Reader.NameTable.Add(@"B");
                id133_ArrayOfString1 = Reader.NameTable.Add(@"ArrayOfString1");
                id85_ServerSettings = Reader.NameTable.Add(@"ServerSettings");
                id156_LicenseNumber = Reader.NameTable.Add(@"LicenseNumber");
                id26_Orchestra = Reader.NameTable.Add(@"Orchestra");
                id126_ArrayOfInstrument = Reader.NameTable.Add(@"ArrayOfInstrument");
                id221_SimpleTypeValue = Reader.NameTable.Add(@"SimpleTypeValue");
                id120_ArrayOfInt = Reader.NameTable.Add(@"ArrayOfInt");
                id111_NameContent = Reader.NameTable.Add(@"NameContent");
                id217_XmlElementPropertyNode = Reader.NameTable.Add(@"XmlElementPropertyNode");
                id1_TypeWithXmlElementProperty = Reader.NameTable.Add(@"TypeWithXmlElementProperty");
                id58_SimpleDC = Reader.NameTable.Add(@"SimpleDC");
                id207_OptionullInt = Reader.NameTable.Add(@"OptionullInt");
                id35_StructNotSerializable = Reader.NameTable.Add(@"StructNotSerializable");
                id205_Optionull = Reader.NameTable.Add(@"Optionull");
                id152_TimeSpanProperty2 = Reader.NameTable.Add(@"TimeSpanProperty2");
                id184_MyEnum1 = Reader.NameTable.Add(@"MyEnum1");
                id117_Value = Reader.NameTable.Add(@"Value");
                id232_EmptyStringProperty = Reader.NameTable.Add(@"EmptyStringProperty");
                id96_MoreChoices = Reader.NameTable.Add(@"MoreChoices");
                id53_BaseClassWithSamePropertyName = Reader.NameTable.Add(@"BaseClassWithSamePropertyName");
                id110_DateContent = Reader.NameTable.Add(@"DateContent");
                id42_DCStruct = Reader.NameTable.Add(@"DCStruct");
                id72_LongEnum = Reader.NameTable.Add(@"LongEnum");
                id75_ItemChoiceType = Reader.NameTable.Add(@"ItemChoiceType");
                id159_EmployeeName = Reader.NameTable.Add(@"EmployeeName");
                id122_ArrayOfString = Reader.NameTable.Add(@"ArrayOfString");
                id170_TotalCost = Reader.NameTable.Add(@"TotalCost");
                id139_P2 = Reader.NameTable.Add(@"P2");
                id129_ArrayOfXElement = Reader.NameTable.Add(@"ArrayOfXElement");
                id6_Item = Reader.NameTable.Add(@"TypeWithDefaultTimeSpanProperty");
                id114_NMTOKENSContent = Reader.NameTable.Add(@"NMTOKENSContent");
                id121_int = Reader.NameTable.Add(@"int");
                id8_TypeWithXmlNodeArrayProperty = Reader.NameTable.Add(@"TypeWithXmlNodeArrayProperty");
                id12_Group = Reader.NameTable.Add(@"Group");
                id141_IsValved = Reader.NameTable.Add(@"IsValved");
                id216_Item = Reader.NameTable.Add(@"XmlNamespaceDeclarationsProperty");
                id155_Breed = Reader.NameTable.Add(@"Breed");
                id179_F1 = Reader.NameTable.Add(@"F1");
                id124_ArrayOfDouble = Reader.NameTable.Add(@"ArrayOfDouble");
                id210_XmlAttributeName = Reader.NameTable.Add(@"XmlAttributeName");
                id107_Amount = Reader.NameTable.Add(@"Amount");
                id237_IntArrayValue = Reader.NameTable.Add(@"IntArrayValue");
                id233_CharProperty = Reader.NameTable.Add(@"CharProperty");
                id164_Zip = Reader.NameTable.Add(@"Zip");
                id70_IntEnum = Reader.NameTable.Add(@"IntEnum");
                id231_TwoDArrayOfSimpleType = Reader.NameTable.Add(@"TwoDArrayOfSimpleType");
                id229_DS2Root = Reader.NameTable.Add(@"DS2Root");
                id172_Y = Reader.NameTable.Add(@"Y");
                id202_Int = Reader.NameTable.Add(@"Int");
                id113_NMTOKENContent = Reader.NameTable.Add(@"NMTOKENContent");
                id50_WithArrayOfXElement = Reader.NameTable.Add(@"WithArrayOfXElement");
                id132_httpmynamespace = Reader.NameTable.Add(@"http://mynamespace");
                id128_ArrayOfTypeA = Reader.NameTable.Add(@"ArrayOfTypeA");
                id9_Animal = Reader.NameTable.Add(@"Animal");
                id146_Quantity = Reader.NameTable.Add(@"Quantity");
                id183_MyStruct = Reader.NameTable.Add(@"MyStruct");
                id83_Item = Reader.NameTable.Add(@"TypeWithSchemaFormInXmlAttribute");
                id43_DCClassWithEnumAndStruct = Reader.NameTable.Add(@"DCClassWithEnumAndStruct");
                id203_Short = Reader.NameTable.Add(@"Short");
                id215_XmlEnumProperty = Reader.NameTable.Add(@"XmlEnumProperty");
                id154_Age = Reader.NameTable.Add(@"Age");
                id148_Document = Reader.NameTable.Add(@"Document");
                id54_DerivedClassWithSameProperty = Reader.NameTable.Add(@"DerivedClassWithSameProperty");
                id45_TypeA = Reader.NameTable.Add(@"TypeA");
                id25_dateTime = Reader.NameTable.Add(@"dateTime");
                id56_Item = Reader.NameTable.Add(@"TypeWithDateTimePropertyAsXmlTime");
                id109_QNameContent = Reader.NameTable.Add(@"QNameContent");
                id192_IntProperty = Reader.NameTable.Add(@"IntProperty");
                id119_ArrayOfOrderedItem = Reader.NameTable.Add(@"ArrayOfOrderedItem");
                id187_e1 = Reader.NameTable.Add(@"e1");
                id33_TypeWithGetSetArrayMembers = Reader.NameTable.Add(@"TypeWithGetSetArrayMembers");
                id60_Item = Reader.NameTable.Add(@"http://schemas.xmlsoap.org/ws/2005/04/discovery");
                id86_TypeWithXmlQualifiedName = Reader.NameTable.Add(@"TypeWithXmlQualifiedName");
                id214_XmlIncludeProperty = Reader.NameTable.Add(@"XmlIncludeProperty");
                id127_ArrayOfSimpleType = Reader.NameTable.Add(@"ArrayOfSimpleType");
                id234_EnumProperty = Reader.NameTable.Add(@"EnumProperty");
                id160_value = Reader.NameTable.Add(@"value");
                id27_Instrument = Reader.NameTable.Add(@"Instrument");
                id115_Base64BinaryContent = Reader.NameTable.Add(@"Base64BinaryContent");
                id82_MyXmlType = Reader.NameTable.Add(@"MyXmlType");
                id125_double = Reader.NameTable.Add(@"double");
                id190_Item = Reader.NameTable.Add(@"PropertyNameWithSpecialCharacters漢ñ");
                id24_ArrayOfDateTime = Reader.NameTable.Add(@"ArrayOfDateTime");
                id238_TestProperty = Reader.NameTable.Add(@"TestProperty");
                id22_BaseClass1 = Reader.NameTable.Add(@"BaseClass1");
                id79_Item = Reader.NameTable.Add(@"ClassImplementingIXmlSerialiable");
                id225_Item = Reader.NameTable.Add(@"UnqualifiedSchemaFormListProperty");
                id104_StringField2 = Reader.NameTable.Add(@"StringField2");
                id92_Item = Reader.NameTable.Add(@"KnownTypesThroughConstructorWithArrayProperties");
                id44_BuiltInTypes = Reader.NameTable.Add(@"BuiltInTypes");
                id29_Trumpet = Reader.NameTable.Add(@"Trumpet");
                id158_GroupVehicle = Reader.NameTable.Add(@"GroupVehicle");
                id46_TypeB = Reader.NameTable.Add(@"TypeB");
                id66_WithNullables = Reader.NameTable.Add(@"WithNullables");
                id37_Item = Reader.NameTable.Add(@"TypeWithReadOnlyMyCollectionProperty");
                id30_Pet = Reader.NameTable.Add(@"Pet");
                id88_Item = Reader.NameTable.Add(@"TypeWithPropertiesHavingDefaultValue");
                id186_e = Reader.NameTable.Add(@"e");
                id31_TypeWithDateTimeStringProperty = Reader.NameTable.Add(@"TypeWithDateTimeStringProperty");
                id209_Struct2 = Reader.NameTable.Add(@"Struct2");
                id36_TypeWithMyCollectionField = Reader.NameTable.Add(@"TypeWithMyCollectionField");
                id69_ShortEnum = Reader.NameTable.Add(@"ShortEnum");
                id101_LastName = Reader.NameTable.Add(@"LastName");
                id235_Foo = Reader.NameTable.Add(@"Foo");
                id68_SByteEnum = Reader.NameTable.Add(@"SByteEnum");
                id57_TypeWithByteArrayAsXmlText = Reader.NameTable.Add(@"TypeWithByteArrayAsXmlText");
                id191_StringProperty = Reader.NameTable.Add(@"StringProperty");
                id227_Item = Reader.NameTable.Add(@"QualifiedSchemaFormListProperty");
                id213_DecimalNumber = Reader.NameTable.Add(@"DecimalNumber");
                id10_Dog = Reader.NameTable.Add(@"Dog");
                id140_Name = Reader.NameTable.Add(@"Name");
                id135_ArrayOfBoolean = Reader.NameTable.Add(@"ArrayOfBoolean");
                id201_Some = Reader.NameTable.Add(@"Some");
                id32_SimpleType = Reader.NameTable.Add(@"SimpleType");
                id123_string = Reader.NameTable.Add(@"string");
                id19_Address = Reader.NameTable.Add(@"Address");
                id157_GroupName = Reader.NameTable.Add(@"GroupName");
                id206_OptionalInt = Reader.NameTable.Add(@"OptionalInt");
                id105_StringField1 = Reader.NameTable.Add(@"StringField1");
                id13_Vehicle = Reader.NameTable.Add(@"Vehicle");
                id7_TypeWithByteProperty = Reader.NameTable.Add(@"TypeWithByteProperty");
                id224_MyFieldIgnored = Reader.NameTable.Add(@"MyFieldIgnored");
                id174_Prop = Reader.NameTable.Add(@"Prop");
                id149_BinaryHexContent = Reader.NameTable.Add(@"BinaryHexContent");
                id34_TypeWithGetOnlyArrayProperties = Reader.NameTable.Add(@"TypeWithGetOnlyArrayProperties");
                id17_PurchaseOrder = Reader.NameTable.Add(@"PurchaseOrder");
                id189_list = Reader.NameTable.Add(@"list");
            }
        }

        public abstract class XmlSerializer1 : System.Xml.Serialization.XmlSerializer {
            protected override System.Xml.Serialization.XmlSerializationReader CreateReader() {
                return new XmlSerializationReader1();
            }
            protected override System.Xml.Serialization.XmlSerializationWriter CreateWriter() {
                return new XmlSerializationWriter1();
            }
        }

        public sealed class TypeWithXmlElementPropertySerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithXmlElementProperty", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write94_TypeWithXmlElementProperty(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read97_TypeWithXmlElementProperty();
            }
        }

        public sealed class TypeWithXmlDocumentPropertySerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithXmlDocumentProperty", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write95_TypeWithXmlDocumentProperty(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read98_TypeWithXmlDocumentProperty();
            }
        }

        public sealed class TypeWithBinaryPropertySerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithBinaryProperty", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write96_TypeWithBinaryProperty(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read99_TypeWithBinaryProperty();
            }
        }

        public sealed class TypeWithTimeSpanPropertySerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithTimeSpanProperty", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write97_TypeWithTimeSpanProperty(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read100_TypeWithTimeSpanProperty();
            }
        }

        public sealed class TypeWithDefaultTimeSpanPropertySerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithDefaultTimeSpanProperty", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write98_Item(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read101_Item();
            }
        }

        public sealed class TypeWithBytePropertySerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithByteProperty", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write99_TypeWithByteProperty(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read102_TypeWithByteProperty();
            }
        }

        public sealed class TypeWithXmlNodeArrayPropertySerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithXmlNodeArrayProperty", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write100_TypeWithXmlNodeArrayProperty(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read103_TypeWithXmlNodeArrayProperty();
            }
        }

        public sealed class AnimalSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"Animal", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write101_Animal(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read104_Animal();
            }
        }

        public sealed class DogSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"Dog", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write102_Dog(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read105_Dog();
            }
        }

        public sealed class DogBreedSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"DogBreed", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write103_DogBreed(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read106_DogBreed();
            }
        }

        public sealed class GroupSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"Group", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write104_Group(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read107_Group();
            }
        }

        public sealed class VehicleSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"Vehicle", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write105_Vehicle(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read108_Vehicle();
            }
        }

        public sealed class EmployeeSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"Employee", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write106_Employee(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read109_Employee();
            }
        }

        public sealed class BaseClassSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"BaseClass", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write107_BaseClass(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read110_BaseClass();
            }
        }

        public sealed class DerivedClassSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"DerivedClass", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write108_DerivedClass(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read111_DerivedClass();
            }
        }

        public sealed class PurchaseOrderSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"PurchaseOrder", @"http://www.contoso1.com");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write109_PurchaseOrder(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read112_PurchaseOrder();
            }
        }

        public sealed class AddressSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"Address", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write110_Address(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read113_Address();
            }
        }

        public sealed class OrderedItemSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"OrderedItem", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write111_OrderedItem(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read114_OrderedItem();
            }
        }

        public sealed class AliasedTestTypeSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"AliasedTestType", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write112_AliasedTestType(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read115_AliasedTestType();
            }
        }

        public sealed class BaseClass1Serializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"BaseClass1", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write113_BaseClass1(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read116_BaseClass1();
            }
        }

        public sealed class DerivedClass1Serializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"DerivedClass1", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write114_DerivedClass1(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read117_DerivedClass1();
            }
        }

        public sealed class MyCollection1Serializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"ArrayOfDateTime", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write115_ArrayOfDateTime(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read118_ArrayOfDateTime();
            }
        }

        public sealed class OrchestraSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"Orchestra", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write116_Orchestra(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read119_Orchestra();
            }
        }

        public sealed class InstrumentSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"Instrument", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write117_Instrument(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read120_Instrument();
            }
        }

        public sealed class BrassSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"Brass", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write118_Brass(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read121_Brass();
            }
        }

        public sealed class TrumpetSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"Trumpet", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write119_Trumpet(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read122_Trumpet();
            }
        }

        public sealed class PetSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"Pet", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write120_Pet(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read123_Pet();
            }
        }

        public sealed class TypeWithDateTimeStringPropertySerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithDateTimeStringProperty", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write121_TypeWithDateTimeStringProperty(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read124_TypeWithDateTimeStringProperty();
            }
        }

        public sealed class SimpleTypeSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"SimpleType", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write122_SimpleType(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read125_SimpleType();
            }
        }

        public sealed class TypeWithGetSetArrayMembersSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithGetSetArrayMembers", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write123_TypeWithGetSetArrayMembers(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read126_TypeWithGetSetArrayMembers();
            }
        }

        public sealed class TypeWithGetOnlyArrayPropertiesSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithGetOnlyArrayProperties", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write124_TypeWithGetOnlyArrayProperties(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read127_TypeWithGetOnlyArrayProperties();
            }
        }

        public sealed class StructNotSerializableSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"StructNotSerializable", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write125_StructNotSerializable(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read128_StructNotSerializable();
            }
        }

        public sealed class TypeWithMyCollectionFieldSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithMyCollectionField", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write126_TypeWithMyCollectionField(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read129_TypeWithMyCollectionField();
            }
        }

        public sealed class TypeWithReadOnlyMyCollectionPropertySerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithReadOnlyMyCollectionProperty", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write127_Item(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read130_Item();
            }
        }

        public sealed class MyListSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"ArrayOfAnyType", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write128_ArrayOfAnyType(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read131_ArrayOfAnyType();
            }
        }

        public sealed class MyEnumSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"MyEnum", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write129_MyEnum(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read132_MyEnum();
            }
        }

        public sealed class TypeWithEnumMembersSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithEnumMembers", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write130_TypeWithEnumMembers(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read133_TypeWithEnumMembers();
            }
        }

        public sealed class DCStructSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"DCStruct", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write131_DCStruct(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read134_DCStruct();
            }
        }

        public sealed class DCClassWithEnumAndStructSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"DCClassWithEnumAndStruct", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write132_DCClassWithEnumAndStruct(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read135_DCClassWithEnumAndStruct();
            }
        }

        public sealed class BuiltInTypesSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"BuiltInTypes", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write133_BuiltInTypes(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read136_BuiltInTypes();
            }
        }

        public sealed class TypeASerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeA", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write134_TypeA(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read137_TypeA();
            }
        }

        public sealed class TypeBSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeB", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write135_TypeB(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read138_TypeB();
            }
        }

        public sealed class TypeHasArrayOfASerializedAsBSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeHasArrayOfASerializedAsB", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write136_TypeHasArrayOfASerializedAsB(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read139_TypeHasArrayOfASerializedAsB();
            }
        }

        public sealed class WithXElementSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"WithXElement", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write137_WithXElement(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read140_WithXElement();
            }
        }

        public sealed class WithXElementWithNestedXElementSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"WithXElementWithNestedXElement", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write138_WithXElementWithNestedXElement(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read141_WithXElementWithNestedXElement();
            }
        }

        public sealed class WithArrayOfXElementSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"WithArrayOfXElement", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write139_WithArrayOfXElement(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read142_WithArrayOfXElement();
            }
        }

        public sealed class WithListOfXElementSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"WithListOfXElement", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write140_WithListOfXElement(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read143_WithListOfXElement();
            }
        }

        public sealed class @__TypeNameWithSpecialCharacters漢ñSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"__TypeNameWithSpecialCharacters漢ñ", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write141_Item(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read144_Item();
            }
        }

        public sealed class BaseClassWithSamePropertyNameSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"BaseClassWithSamePropertyName", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write142_BaseClassWithSamePropertyName(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read145_BaseClassWithSamePropertyName();
            }
        }

        public sealed class DerivedClassWithSamePropertySerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"DerivedClassWithSameProperty", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write143_DerivedClassWithSameProperty(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read146_DerivedClassWithSameProperty();
            }
        }

        public sealed class DerivedClassWithSameProperty2Serializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"DerivedClassWithSameProperty2", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write144_DerivedClassWithSameProperty2(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read147_DerivedClassWithSameProperty2();
            }
        }

        public sealed class TypeWithDateTimePropertyAsXmlTimeSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithDateTimePropertyAsXmlTime", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write145_Item(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read148_Item();
            }
        }

        public sealed class TypeWithByteArrayAsXmlTextSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithByteArrayAsXmlText", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write146_TypeWithByteArrayAsXmlText(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read149_TypeWithByteArrayAsXmlText();
            }
        }

        public sealed class SimpleDCSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"SimpleDC", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write147_SimpleDC(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read150_SimpleDC();
            }
        }

        public sealed class TypeWithXmlTextAttributeOnArraySerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithXmlTextAttributeOnArray", @"http://schemas.xmlsoap.org/ws/2005/04/discovery");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write148_Item(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read151_Item();
            }
        }

        public sealed class EnumFlagsSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"EnumFlags", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write149_EnumFlags(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read152_EnumFlags();
            }
        }

        public sealed class ClassImplementsInterfaceSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"ClassImplementsInterface", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write150_ClassImplementsInterface(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read153_ClassImplementsInterface();
            }
        }

        public sealed class WithStructSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"WithStruct", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write151_WithStruct(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read154_WithStruct();
            }
        }

        public sealed class SomeStructSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"SomeStruct", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write152_SomeStruct(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read155_SomeStruct();
            }
        }

        public sealed class WithEnumsSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"WithEnums", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write153_WithEnums(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read156_WithEnums();
            }
        }

        public sealed class WithNullablesSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"WithNullables", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write154_WithNullables(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read157_WithNullables();
            }
        }

        public sealed class ByteEnumSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"ByteEnum", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write155_ByteEnum(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read158_ByteEnum();
            }
        }

        public sealed class SByteEnumSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"SByteEnum", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write156_SByteEnum(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read159_SByteEnum();
            }
        }

        public sealed class ShortEnumSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"ShortEnum", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write157_ShortEnum(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read160_ShortEnum();
            }
        }

        public sealed class IntEnumSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"IntEnum", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write158_IntEnum(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read161_IntEnum();
            }
        }

        public sealed class UIntEnumSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"UIntEnum", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write159_UIntEnum(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read162_UIntEnum();
            }
        }

        public sealed class LongEnumSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"LongEnum", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write160_LongEnum(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read163_LongEnum();
            }
        }

        public sealed class ULongEnumSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"ULongEnum", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write161_ULongEnum(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read164_ULongEnum();
            }
        }

        public sealed class XmlSerializerAttributesSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"AttributeTesting", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write162_AttributeTesting(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read165_AttributeTesting();
            }
        }

        public sealed class ItemChoiceTypeSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"ItemChoiceType", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write163_ItemChoiceType(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read166_ItemChoiceType();
            }
        }

        public sealed class TypeWithAnyAttributeSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithAnyAttribute", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write164_TypeWithAnyAttribute(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read167_TypeWithAnyAttribute();
            }
        }

        public sealed class KnownTypesThroughConstructorSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"KnownTypesThroughConstructor", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write165_KnownTypesThroughConstructor(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read168_KnownTypesThroughConstructor();
            }
        }

        public sealed class SimpleKnownTypeValueSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"SimpleKnownTypeValue", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write166_SimpleKnownTypeValue(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read169_SimpleKnownTypeValue();
            }
        }

        public sealed class ClassImplementingIXmlSerialiableSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"ClassImplementingIXmlSerialiable", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write167_Item(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read170_Item();
            }
        }

        public sealed class TypeWithPropertyNameSpecifiedSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithPropertyNameSpecified", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write168_TypeWithPropertyNameSpecified(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read171_TypeWithPropertyNameSpecified();
            }
        }

        public sealed class TypeWithXmlSchemaFormAttributeSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithXmlSchemaFormAttribute", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write169_TypeWithXmlSchemaFormAttribute(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read172_TypeWithXmlSchemaFormAttribute();
            }
        }

        public sealed class TypeWithTypeNameInXmlTypeAttributeSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"MyXmlType", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write170_MyXmlType(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read173_MyXmlType();
            }
        }

        public sealed class TypeWithSchemaFormInXmlAttributeSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithSchemaFormInXmlAttribute", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write171_Item(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read174_Item();
            }
        }

        public sealed class TypeWithNonPublicDefaultConstructorSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithNonPublicDefaultConstructor", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write172_Item(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read175_Item();
            }
        }

        public sealed class ServerSettingsSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"ServerSettings", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write173_ServerSettings(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read176_ServerSettings();
            }
        }

        public sealed class TypeWithXmlQualifiedNameSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithXmlQualifiedName", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write174_TypeWithXmlQualifiedName(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read177_TypeWithXmlQualifiedName();
            }
        }

        public sealed class TypeWith2DArrayProperty2Serializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWith2DArrayProperty2", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write175_TypeWith2DArrayProperty2(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read178_TypeWith2DArrayProperty2();
            }
        }

        public sealed class TypeWithPropertiesHavingDefaultValueSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithPropertiesHavingDefaultValue", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write176_Item(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read179_Item();
            }
        }

        public sealed class TypeWithEnumPropertyHavingDefaultValueSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithEnumPropertyHavingDefaultValue", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write177_Item(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read180_Item();
            }
        }

        public sealed class TypeWithEnumFlagPropertyHavingDefaultValueSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithEnumFlagPropertyHavingDefaultValue", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write178_Item(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read181_Item();
            }
        }

        public sealed class TypeWithShouldSerializeMethodSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithShouldSerializeMethod", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write179_TypeWithShouldSerializeMethod(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read182_TypeWithShouldSerializeMethod();
            }
        }

        public sealed class KnownTypesThroughConstructorWithArrayPropertiesSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"KnownTypesThroughConstructorWithArrayProperties", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write180_Item(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read183_Item();
            }
        }

        public sealed class KnownTypesThroughConstructorWithValueSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"KnownTypesThroughConstructorWithValue", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write181_Item(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read184_Item();
            }
        }

        public sealed class TypeWithTypesHavingCustomFormatterSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithTypesHavingCustomFormatter", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write182_Item(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read185_Item();
            }
        }

        public sealed class TypeWithArrayPropertyHavingChoiceSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithArrayPropertyHavingChoice", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write183_Item(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read186_Item();
            }
        }

        public sealed class MoreChoicesSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"MoreChoices", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write184_MoreChoices(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read187_MoreChoices();
            }
        }

        public sealed class TypeWithFieldsOrderedSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"TypeWithFieldsOrdered", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write185_TypeWithFieldsOrdered(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read188_TypeWithFieldsOrdered();
            }
        }

        public sealed class PersonSerializer : XmlSerializer1 {

            public override System.Boolean CanDeserialize(System.Xml.XmlReader xmlReader) {
                return xmlReader.IsStartElement(@"Person", @"");
            }

            protected override void Serialize(object objectToSerialize, System.Xml.Serialization.XmlSerializationWriter writer) {
                ((XmlSerializationWriter1)writer).Write186_Person(objectToSerialize);
            }

            protected override object Deserialize(System.Xml.Serialization.XmlSerializationReader reader) {
                return ((XmlSerializationReader1)reader).Read189_Person();
            }
        }

        public class XmlSerializerContract : global::System.Xml.Serialization.XmlSerializerImplementation {
            public override global::System.Xml.Serialization.XmlSerializationReader Reader { get { return new XmlSerializationReader1(); } }
            public override global::System.Xml.Serialization.XmlSerializationWriter Writer { get { return new XmlSerializationWriter1(); } }
            System.Collections.Hashtable readMethods = null;
            public override System.Collections.Hashtable ReadMethods {
                get {
                    if (readMethods == null) {
                        System.Collections.Hashtable _tmp = new System.Collections.Hashtable();
                        _tmp[@"TypeWithXmlElementProperty::"] = @"Read97_TypeWithXmlElementProperty";
                        _tmp[@"TypeWithXmlDocumentProperty::"] = @"Read98_TypeWithXmlDocumentProperty";
                        _tmp[@"TypeWithBinaryProperty::"] = @"Read99_TypeWithBinaryProperty";
                        _tmp[@"TypeWithTimeSpanProperty::"] = @"Read100_TypeWithTimeSpanProperty";
                        _tmp[@"TypeWithDefaultTimeSpanProperty::"] = @"Read101_Item";
                        _tmp[@"TypeWithByteProperty::"] = @"Read102_TypeWithByteProperty";
                        _tmp[@"TypeWithXmlNodeArrayProperty:::True:"] = @"Read103_TypeWithXmlNodeArrayProperty";
                        _tmp[@"Animal::"] = @"Read104_Animal";
                        _tmp[@"Dog::"] = @"Read105_Dog";
                        _tmp[@"DogBreed::"] = @"Read106_DogBreed";
                        _tmp[@"Group::"] = @"Read107_Group";
                        _tmp[@"Vehicle::"] = @"Read108_Vehicle";
                        _tmp[@"Employee::"] = @"Read109_Employee";
                        _tmp[@"BaseClass::"] = @"Read110_BaseClass";
                        _tmp[@"DerivedClass::"] = @"Read111_DerivedClass";
                        _tmp[@"PurchaseOrder:http://www.contoso1.com:PurchaseOrder:False:"] = @"Read112_PurchaseOrder";
                        _tmp[@"Address::"] = @"Read113_Address";
                        _tmp[@"OrderedItem::"] = @"Read114_OrderedItem";
                        _tmp[@"AliasedTestType::"] = @"Read115_AliasedTestType";
                        _tmp[@"BaseClass1::"] = @"Read116_BaseClass1";
                        _tmp[@"DerivedClass1::"] = @"Read117_DerivedClass1";
                        _tmp[@"MyCollection1::"] = @"Read118_ArrayOfDateTime";
                        _tmp[@"Orchestra::"] = @"Read119_Orchestra";
                        _tmp[@"Instrument::"] = @"Read120_Instrument";
                        _tmp[@"Brass::"] = @"Read121_Brass";
                        _tmp[@"Trumpet::"] = @"Read122_Trumpet";
                        _tmp[@"Pet::"] = @"Read123_Pet";
                        _tmp[@"SerializationTypes.TypeWithDateTimeStringProperty::"] = @"Read124_TypeWithDateTimeStringProperty";
                        _tmp[@"SerializationTypes.SimpleType::"] = @"Read125_SimpleType";
                        _tmp[@"SerializationTypes.TypeWithGetSetArrayMembers::"] = @"Read126_TypeWithGetSetArrayMembers";
                        _tmp[@"SerializationTypes.TypeWithGetOnlyArrayProperties::"] = @"Read127_TypeWithGetOnlyArrayProperties";
                        _tmp[@"SerializationTypes.StructNotSerializable::"] = @"Read128_StructNotSerializable";
                        _tmp[@"SerializationTypes.TypeWithMyCollectionField::"] = @"Read129_TypeWithMyCollectionField";
                        _tmp[@"SerializationTypes.TypeWithReadOnlyMyCollectionProperty::"] = @"Read130_Item";
                        _tmp[@"SerializationTypes.MyList::"] = @"Read131_ArrayOfAnyType";
                        _tmp[@"SerializationTypes.MyEnum::"] = @"Read132_MyEnum";
                        _tmp[@"SerializationTypes.TypeWithEnumMembers::"] = @"Read133_TypeWithEnumMembers";
                        _tmp[@"SerializationTypes.DCStruct::"] = @"Read134_DCStruct";
                        _tmp[@"SerializationTypes.DCClassWithEnumAndStruct::"] = @"Read135_DCClassWithEnumAndStruct";
                        _tmp[@"SerializationTypes.BuiltInTypes::"] = @"Read136_BuiltInTypes";
                        _tmp[@"SerializationTypes.TypeA::"] = @"Read137_TypeA";
                        _tmp[@"SerializationTypes.TypeB::"] = @"Read138_TypeB";
                        _tmp[@"SerializationTypes.TypeHasArrayOfASerializedAsB::"] = @"Read139_TypeHasArrayOfASerializedAsB";
                        _tmp[@"SerializationTypes.WithXElement::"] = @"Read140_WithXElement";
                        _tmp[@"SerializationTypes.WithXElementWithNestedXElement::"] = @"Read141_WithXElementWithNestedXElement";
                        _tmp[@"SerializationTypes.WithArrayOfXElement::"] = @"Read142_WithArrayOfXElement";
                        _tmp[@"SerializationTypes.WithListOfXElement::"] = @"Read143_WithListOfXElement";
                        _tmp[@"SerializationTypes.__TypeNameWithSpecialCharacters漢ñ::"] = @"Read144_Item";
                        _tmp[@"SerializationTypes.BaseClassWithSamePropertyName::"] = @"Read145_BaseClassWithSamePropertyName";
                        _tmp[@"SerializationTypes.DerivedClassWithSameProperty::"] = @"Read146_DerivedClassWithSameProperty";
                        _tmp[@"SerializationTypes.DerivedClassWithSameProperty2::"] = @"Read147_DerivedClassWithSameProperty2";
                        _tmp[@"SerializationTypes.TypeWithDateTimePropertyAsXmlTime::"] = @"Read148_Item";
                        _tmp[@"SerializationTypes.TypeWithByteArrayAsXmlText::"] = @"Read149_TypeWithByteArrayAsXmlText";
                        _tmp[@"SerializationTypes.SimpleDC::"] = @"Read150_SimpleDC";
                        _tmp[@"SerializationTypes.TypeWithXmlTextAttributeOnArray:http://schemas.xmlsoap.org/ws/2005/04/discovery::False:"] = @"Read151_Item";
                        _tmp[@"SerializationTypes.EnumFlags::"] = @"Read152_EnumFlags";
                        _tmp[@"SerializationTypes.ClassImplementsInterface::"] = @"Read153_ClassImplementsInterface";
                        _tmp[@"SerializationTypes.WithStruct::"] = @"Read154_WithStruct";
                        _tmp[@"SerializationTypes.SomeStruct::"] = @"Read155_SomeStruct";
                        _tmp[@"SerializationTypes.WithEnums::"] = @"Read156_WithEnums";
                        _tmp[@"SerializationTypes.WithNullables::"] = @"Read157_WithNullables";
                        _tmp[@"SerializationTypes.ByteEnum::"] = @"Read158_ByteEnum";
                        _tmp[@"SerializationTypes.SByteEnum::"] = @"Read159_SByteEnum";
                        _tmp[@"SerializationTypes.ShortEnum::"] = @"Read160_ShortEnum";
                        _tmp[@"SerializationTypes.IntEnum::"] = @"Read161_IntEnum";
                        _tmp[@"SerializationTypes.UIntEnum::"] = @"Read162_UIntEnum";
                        _tmp[@"SerializationTypes.LongEnum::"] = @"Read163_LongEnum";
                        _tmp[@"SerializationTypes.ULongEnum::"] = @"Read164_ULongEnum";
                        _tmp[@"SerializationTypes.XmlSerializerAttributes::AttributeTesting:False:"] = @"Read165_AttributeTesting";
                        _tmp[@"SerializationTypes.ItemChoiceType::"] = @"Read166_ItemChoiceType";
                        _tmp[@"SerializationTypes.TypeWithAnyAttribute::"] = @"Read167_TypeWithAnyAttribute";
                        _tmp[@"SerializationTypes.KnownTypesThroughConstructor::"] = @"Read168_KnownTypesThroughConstructor";
                        _tmp[@"SerializationTypes.SimpleKnownTypeValue::"] = @"Read169_SimpleKnownTypeValue";
                        _tmp[@"SerializationTypes.ClassImplementingIXmlSerialiable::"] = @"Read170_Item";
                        _tmp[@"SerializationTypes.TypeWithPropertyNameSpecified::"] = @"Read171_TypeWithPropertyNameSpecified";
                        _tmp[@"SerializationTypes.TypeWithXmlSchemaFormAttribute:::True:"] = @"Read172_TypeWithXmlSchemaFormAttribute";
                        _tmp[@"SerializationTypes.TypeWithTypeNameInXmlTypeAttribute::"] = @"Read173_MyXmlType";
                        _tmp[@"SerializationTypes.TypeWithSchemaFormInXmlAttribute::"] = @"Read174_Item";
                        _tmp[@"SerializationTypes.TypeWithNonPublicDefaultConstructor::"] = @"Read175_Item";
                        _tmp[@"SerializationTypes.ServerSettings::"] = @"Read176_ServerSettings";
                        _tmp[@"SerializationTypes.TypeWithXmlQualifiedName::"] = @"Read177_TypeWithXmlQualifiedName";
                        _tmp[@"SerializationTypes.TypeWith2DArrayProperty2::"] = @"Read178_TypeWith2DArrayProperty2";
                        _tmp[@"SerializationTypes.TypeWithPropertiesHavingDefaultValue::"] = @"Read179_Item";
                        _tmp[@"SerializationTypes.TypeWithEnumPropertyHavingDefaultValue::"] = @"Read180_Item";
                        _tmp[@"SerializationTypes.TypeWithEnumFlagPropertyHavingDefaultValue::"] = @"Read181_Item";
                        _tmp[@"SerializationTypes.TypeWithShouldSerializeMethod::"] = @"Read182_TypeWithShouldSerializeMethod";
                        _tmp[@"SerializationTypes.KnownTypesThroughConstructorWithArrayProperties::"] = @"Read183_Item";
                        _tmp[@"SerializationTypes.KnownTypesThroughConstructorWithValue::"] = @"Read184_Item";
                        _tmp[@"SerializationTypes.TypeWithTypesHavingCustomFormatter::"] = @"Read185_Item";
                        _tmp[@"SerializationTypes.TypeWithArrayPropertyHavingChoice::"] = @"Read186_Item";
                        _tmp[@"SerializationTypes.MoreChoices::"] = @"Read187_MoreChoices";
                        _tmp[@"SerializationTypes.TypeWithFieldsOrdered::"] = @"Read188_TypeWithFieldsOrdered";
                        _tmp[@"Outer+Person::"] = @"Read189_Person";
                        if (readMethods == null) readMethods = _tmp;
                    }
                    return readMethods;
                }
            }
            System.Collections.Hashtable writeMethods = null;
            public override System.Collections.Hashtable WriteMethods {
                get {
                    if (writeMethods == null) {
                        System.Collections.Hashtable _tmp = new System.Collections.Hashtable();
                        _tmp[@"TypeWithXmlElementProperty::"] = @"Write94_TypeWithXmlElementProperty";
                        _tmp[@"TypeWithXmlDocumentProperty::"] = @"Write95_TypeWithXmlDocumentProperty";
                        _tmp[@"TypeWithBinaryProperty::"] = @"Write96_TypeWithBinaryProperty";
                        _tmp[@"TypeWithTimeSpanProperty::"] = @"Write97_TypeWithTimeSpanProperty";
                        _tmp[@"TypeWithDefaultTimeSpanProperty::"] = @"Write98_Item";
                        _tmp[@"TypeWithByteProperty::"] = @"Write99_TypeWithByteProperty";
                        _tmp[@"TypeWithXmlNodeArrayProperty:::True:"] = @"Write100_TypeWithXmlNodeArrayProperty";
                        _tmp[@"Animal::"] = @"Write101_Animal";
                        _tmp[@"Dog::"] = @"Write102_Dog";
                        _tmp[@"DogBreed::"] = @"Write103_DogBreed";
                        _tmp[@"Group::"] = @"Write104_Group";
                        _tmp[@"Vehicle::"] = @"Write105_Vehicle";
                        _tmp[@"Employee::"] = @"Write106_Employee";
                        _tmp[@"BaseClass::"] = @"Write107_BaseClass";
                        _tmp[@"DerivedClass::"] = @"Write108_DerivedClass";
                        _tmp[@"PurchaseOrder:http://www.contoso1.com:PurchaseOrder:False:"] = @"Write109_PurchaseOrder";
                        _tmp[@"Address::"] = @"Write110_Address";
                        _tmp[@"OrderedItem::"] = @"Write111_OrderedItem";
                        _tmp[@"AliasedTestType::"] = @"Write112_AliasedTestType";
                        _tmp[@"BaseClass1::"] = @"Write113_BaseClass1";
                        _tmp[@"DerivedClass1::"] = @"Write114_DerivedClass1";
                        _tmp[@"MyCollection1::"] = @"Write115_ArrayOfDateTime";
                        _tmp[@"Orchestra::"] = @"Write116_Orchestra";
                        _tmp[@"Instrument::"] = @"Write117_Instrument";
                        _tmp[@"Brass::"] = @"Write118_Brass";
                        _tmp[@"Trumpet::"] = @"Write119_Trumpet";
                        _tmp[@"Pet::"] = @"Write120_Pet";
                        _tmp[@"SerializationTypes.TypeWithDateTimeStringProperty::"] = @"Write121_TypeWithDateTimeStringProperty";
                        _tmp[@"SerializationTypes.SimpleType::"] = @"Write122_SimpleType";
                        _tmp[@"SerializationTypes.TypeWithGetSetArrayMembers::"] = @"Write123_TypeWithGetSetArrayMembers";
                        _tmp[@"SerializationTypes.TypeWithGetOnlyArrayProperties::"] = @"Write124_TypeWithGetOnlyArrayProperties";
                        _tmp[@"SerializationTypes.StructNotSerializable::"] = @"Write125_StructNotSerializable";
                        _tmp[@"SerializationTypes.TypeWithMyCollectionField::"] = @"Write126_TypeWithMyCollectionField";
                        _tmp[@"SerializationTypes.TypeWithReadOnlyMyCollectionProperty::"] = @"Write127_Item";
                        _tmp[@"SerializationTypes.MyList::"] = @"Write128_ArrayOfAnyType";
                        _tmp[@"SerializationTypes.MyEnum::"] = @"Write129_MyEnum";
                        _tmp[@"SerializationTypes.TypeWithEnumMembers::"] = @"Write130_TypeWithEnumMembers";
                        _tmp[@"SerializationTypes.DCStruct::"] = @"Write131_DCStruct";
                        _tmp[@"SerializationTypes.DCClassWithEnumAndStruct::"] = @"Write132_DCClassWithEnumAndStruct";
                        _tmp[@"SerializationTypes.BuiltInTypes::"] = @"Write133_BuiltInTypes";
                        _tmp[@"SerializationTypes.TypeA::"] = @"Write134_TypeA";
                        _tmp[@"SerializationTypes.TypeB::"] = @"Write135_TypeB";
                        _tmp[@"SerializationTypes.TypeHasArrayOfASerializedAsB::"] = @"Write136_TypeHasArrayOfASerializedAsB";
                        _tmp[@"SerializationTypes.WithXElement::"] = @"Write137_WithXElement";
                        _tmp[@"SerializationTypes.WithXElementWithNestedXElement::"] = @"Write138_WithXElementWithNestedXElement";
                        _tmp[@"SerializationTypes.WithArrayOfXElement::"] = @"Write139_WithArrayOfXElement";
                        _tmp[@"SerializationTypes.WithListOfXElement::"] = @"Write140_WithListOfXElement";
                        _tmp[@"SerializationTypes.__TypeNameWithSpecialCharacters漢ñ::"] = @"Write141_Item";
                        _tmp[@"SerializationTypes.BaseClassWithSamePropertyName::"] = @"Write142_BaseClassWithSamePropertyName";
                        _tmp[@"SerializationTypes.DerivedClassWithSameProperty::"] = @"Write143_DerivedClassWithSameProperty";
                        _tmp[@"SerializationTypes.DerivedClassWithSameProperty2::"] = @"Write144_DerivedClassWithSameProperty2";
                        _tmp[@"SerializationTypes.TypeWithDateTimePropertyAsXmlTime::"] = @"Write145_Item";
                        _tmp[@"SerializationTypes.TypeWithByteArrayAsXmlText::"] = @"Write146_TypeWithByteArrayAsXmlText";
                        _tmp[@"SerializationTypes.SimpleDC::"] = @"Write147_SimpleDC";
                        _tmp[@"SerializationTypes.TypeWithXmlTextAttributeOnArray:http://schemas.xmlsoap.org/ws/2005/04/discovery::False:"] = @"Write148_Item";
                        _tmp[@"SerializationTypes.EnumFlags::"] = @"Write149_EnumFlags";
                        _tmp[@"SerializationTypes.ClassImplementsInterface::"] = @"Write150_ClassImplementsInterface";
                        _tmp[@"SerializationTypes.WithStruct::"] = @"Write151_WithStruct";
                        _tmp[@"SerializationTypes.SomeStruct::"] = @"Write152_SomeStruct";
                        _tmp[@"SerializationTypes.WithEnums::"] = @"Write153_WithEnums";
                        _tmp[@"SerializationTypes.WithNullables::"] = @"Write154_WithNullables";
                        _tmp[@"SerializationTypes.ByteEnum::"] = @"Write155_ByteEnum";
                        _tmp[@"SerializationTypes.SByteEnum::"] = @"Write156_SByteEnum";
                        _tmp[@"SerializationTypes.ShortEnum::"] = @"Write157_ShortEnum";
                        _tmp[@"SerializationTypes.IntEnum::"] = @"Write158_IntEnum";
                        _tmp[@"SerializationTypes.UIntEnum::"] = @"Write159_UIntEnum";
                        _tmp[@"SerializationTypes.LongEnum::"] = @"Write160_LongEnum";
                        _tmp[@"SerializationTypes.ULongEnum::"] = @"Write161_ULongEnum";
                        _tmp[@"SerializationTypes.XmlSerializerAttributes::AttributeTesting:False:"] = @"Write162_AttributeTesting";
                        _tmp[@"SerializationTypes.ItemChoiceType::"] = @"Write163_ItemChoiceType";
                        _tmp[@"SerializationTypes.TypeWithAnyAttribute::"] = @"Write164_TypeWithAnyAttribute";
                        _tmp[@"SerializationTypes.KnownTypesThroughConstructor::"] = @"Write165_KnownTypesThroughConstructor";
                        _tmp[@"SerializationTypes.SimpleKnownTypeValue::"] = @"Write166_SimpleKnownTypeValue";
                        _tmp[@"SerializationTypes.ClassImplementingIXmlSerialiable::"] = @"Write167_Item";
                        _tmp[@"SerializationTypes.TypeWithPropertyNameSpecified::"] = @"Write168_TypeWithPropertyNameSpecified";
                        _tmp[@"SerializationTypes.TypeWithXmlSchemaFormAttribute:::True:"] = @"Write169_TypeWithXmlSchemaFormAttribute";
                        _tmp[@"SerializationTypes.TypeWithTypeNameInXmlTypeAttribute::"] = @"Write170_MyXmlType";
                        _tmp[@"SerializationTypes.TypeWithSchemaFormInXmlAttribute::"] = @"Write171_Item";
                        _tmp[@"SerializationTypes.TypeWithNonPublicDefaultConstructor::"] = @"Write172_Item";
                        _tmp[@"SerializationTypes.ServerSettings::"] = @"Write173_ServerSettings";
                        _tmp[@"SerializationTypes.TypeWithXmlQualifiedName::"] = @"Write174_TypeWithXmlQualifiedName";
                        _tmp[@"SerializationTypes.TypeWith2DArrayProperty2::"] = @"Write175_TypeWith2DArrayProperty2";
                        _tmp[@"SerializationTypes.TypeWithPropertiesHavingDefaultValue::"] = @"Write176_Item";
                        _tmp[@"SerializationTypes.TypeWithEnumPropertyHavingDefaultValue::"] = @"Write177_Item";
                        _tmp[@"SerializationTypes.TypeWithEnumFlagPropertyHavingDefaultValue::"] = @"Write178_Item";
                        _tmp[@"SerializationTypes.TypeWithShouldSerializeMethod::"] = @"Write179_TypeWithShouldSerializeMethod";
                        _tmp[@"SerializationTypes.KnownTypesThroughConstructorWithArrayProperties::"] = @"Write180_Item";
                        _tmp[@"SerializationTypes.KnownTypesThroughConstructorWithValue::"] = @"Write181_Item";
                        _tmp[@"SerializationTypes.TypeWithTypesHavingCustomFormatter::"] = @"Write182_Item";
                        _tmp[@"SerializationTypes.TypeWithArrayPropertyHavingChoice::"] = @"Write183_Item";
                        _tmp[@"SerializationTypes.MoreChoices::"] = @"Write184_MoreChoices";
                        _tmp[@"SerializationTypes.TypeWithFieldsOrdered::"] = @"Write185_TypeWithFieldsOrdered";
                        _tmp[@"Outer+Person::"] = @"Write186_Person";
                        if (writeMethods == null) writeMethods = _tmp;
                    }
                    return writeMethods;
                }
            }
            System.Collections.Hashtable typedSerializers = null;
            public override System.Collections.Hashtable TypedSerializers {
                get {
                    if (typedSerializers == null) {
                        System.Collections.Hashtable _tmp = new System.Collections.Hashtable();
                        _tmp.Add(@"SerializationTypes.BaseClassWithSamePropertyName::", new BaseClassWithSamePropertyNameSerializer());
                        _tmp.Add(@"SerializationTypes.TypeWithPropertyNameSpecified::", new TypeWithPropertyNameSpecifiedSerializer());
                        _tmp.Add(@"SerializationTypes.ByteEnum::", new ByteEnumSerializer());
                        _tmp.Add(@"SerializationTypes.IntEnum::", new IntEnumSerializer());
                        _tmp.Add(@"Orchestra::", new OrchestraSerializer());
                        _tmp.Add(@"SerializationTypes.ItemChoiceType::", new ItemChoiceTypeSerializer());
                        _tmp.Add(@"SerializationTypes.KnownTypesThroughConstructor::", new KnownTypesThroughConstructorSerializer());
                        _tmp.Add(@"SerializationTypes.StructNotSerializable::", new StructNotSerializableSerializer());
                        _tmp.Add(@"DerivedClass::", new DerivedClassSerializer());
                        _tmp.Add(@"Brass::", new BrassSerializer());
                        _tmp.Add(@"SerializationTypes.BuiltInTypes::", new BuiltInTypesSerializer());
                        _tmp.Add(@"SerializationTypes.WithStruct::", new WithStructSerializer());
                        _tmp.Add(@"DogBreed::", new DogBreedSerializer());
                        _tmp.Add(@"Instrument::", new InstrumentSerializer());
                        _tmp.Add(@"TypeWithDefaultTimeSpanProperty::", new TypeWithDefaultTimeSpanPropertySerializer());
                        _tmp.Add(@"SerializationTypes.TypeWithXmlQualifiedName::", new TypeWithXmlQualifiedNameSerializer());
                        _tmp.Add(@"SerializationTypes.__TypeNameWithSpecialCharacters漢ñ::", new __TypeNameWithSpecialCharacters漢ñSerializer());
                        _tmp.Add(@"SerializationTypes.WithArrayOfXElement::", new WithArrayOfXElementSerializer());
                        _tmp.Add(@"SerializationTypes.TypeWithTypeNameInXmlTypeAttribute::", new TypeWithTypeNameInXmlTypeAttributeSerializer());
                        _tmp.Add(@"Vehicle::", new VehicleSerializer());
                        _tmp.Add(@"TypeWithByteProperty::", new TypeWithBytePropertySerializer());
                        _tmp.Add(@"Outer+Person::", new PersonSerializer());
                        _tmp.Add(@"SerializationTypes.TypeB::", new TypeBSerializer());
                        _tmp.Add(@"DerivedClass1::", new DerivedClass1Serializer());
                        _tmp.Add(@"SerializationTypes.SimpleDC::", new SimpleDCSerializer());
                        _tmp.Add(@"SerializationTypes.MyList::", new MyListSerializer());
                        _tmp.Add(@"SerializationTypes.WithXElement::", new WithXElementSerializer());
                        _tmp.Add(@"SerializationTypes.ClassImplementingIXmlSerialiable::", new ClassImplementingIXmlSerialiableSerializer());
                        _tmp.Add(@"TypeWithBinaryProperty::", new TypeWithBinaryPropertySerializer());
                        _tmp.Add(@"SerializationTypes.DCStruct::", new DCStructSerializer());
                        _tmp.Add(@"TypeWithXmlDocumentProperty::", new TypeWithXmlDocumentPropertySerializer());
                        _tmp.Add(@"Animal::", new AnimalSerializer());
                        _tmp.Add(@"SerializationTypes.TypeWithAnyAttribute::", new TypeWithAnyAttributeSerializer());
                        _tmp.Add(@"OrderedItem::", new OrderedItemSerializer());
                        _tmp.Add(@"SerializationTypes.TypeWithSchemaFormInXmlAttribute::", new TypeWithSchemaFormInXmlAttributeSerializer());
                        _tmp.Add(@"SerializationTypes.UIntEnum::", new UIntEnumSerializer());
                        _tmp.Add(@"SerializationTypes.ULongEnum::", new ULongEnumSerializer());
                        _tmp.Add(@"SerializationTypes.EnumFlags::", new EnumFlagsSerializer());
                        _tmp.Add(@"SerializationTypes.TypeWithArrayPropertyHavingChoice::", new TypeWithArrayPropertyHavingChoiceSerializer());
                        _tmp.Add(@"SerializationTypes.SimpleType::", new SimpleTypeSerializer());
                        _tmp.Add(@"Pet::", new PetSerializer());
                        _tmp.Add(@"SerializationTypes.TypeWith2DArrayProperty2::", new TypeWith2DArrayProperty2Serializer());
                        _tmp.Add(@"Group::", new GroupSerializer());
                        _tmp.Add(@"SerializationTypes.TypeA::", new TypeASerializer());
                        _tmp.Add(@"BaseClass::", new BaseClassSerializer());
                        _tmp.Add(@"SerializationTypes.WithXElementWithNestedXElement::", new WithXElementWithNestedXElementSerializer());
                        _tmp.Add(@"SerializationTypes.XmlSerializerAttributes::AttributeTesting:False:", new XmlSerializerAttributesSerializer());
                        _tmp.Add(@"SerializationTypes.SomeStruct::", new SomeStructSerializer());
                        _tmp.Add(@"SerializationTypes.DCClassWithEnumAndStruct::", new DCClassWithEnumAndStructSerializer());
                        _tmp.Add(@"SerializationTypes.SByteEnum::", new SByteEnumSerializer());
                        _tmp.Add(@"SerializationTypes.TypeWithReadOnlyMyCollectionProperty::", new TypeWithReadOnlyMyCollectionPropertySerializer());
                        _tmp.Add(@"SerializationTypes.WithNullables::", new WithNullablesSerializer());
                        _tmp.Add(@"SerializationTypes.KnownTypesThroughConstructorWithArrayProperties::", new KnownTypesThroughConstructorWithArrayPropertiesSerializer());
                        _tmp.Add(@"BaseClass1::", new BaseClass1Serializer());
                        _tmp.Add(@"SerializationTypes.TypeWithEnumFlagPropertyHavingDefaultValue::", new TypeWithEnumFlagPropertyHavingDefaultValueSerializer());
                        _tmp.Add(@"SerializationTypes.TypeWithShouldSerializeMethod::", new TypeWithShouldSerializeMethodSerializer());
                        _tmp.Add(@"TypeWithXmlElementProperty::", new TypeWithXmlElementPropertySerializer());
                        _tmp.Add(@"SerializationTypes.WithListOfXElement::", new WithListOfXElementSerializer());
                        _tmp.Add(@"SerializationTypes.TypeWithGetOnlyArrayProperties::", new TypeWithGetOnlyArrayPropertiesSerializer());
                        _tmp.Add(@"SerializationTypes.TypeWithFieldsOrdered::", new TypeWithFieldsOrderedSerializer());
                        _tmp.Add(@"Dog::", new DogSerializer());
                        _tmp.Add(@"SerializationTypes.TypeWithDateTimePropertyAsXmlTime::", new TypeWithDateTimePropertyAsXmlTimeSerializer());
                        _tmp.Add(@"SerializationTypes.DerivedClassWithSameProperty::", new DerivedClassWithSamePropertySerializer());
                        _tmp.Add(@"SerializationTypes.LongEnum::", new LongEnumSerializer());
                        _tmp.Add(@"SerializationTypes.MoreChoices::", new MoreChoicesSerializer());
                        _tmp.Add(@"SerializationTypes.ShortEnum::", new ShortEnumSerializer());
                        _tmp.Add(@"SerializationTypes.KnownTypesThroughConstructorWithValue::", new KnownTypesThroughConstructorWithValueSerializer());
                        _tmp.Add(@"Employee::", new EmployeeSerializer());
                        _tmp.Add(@"MyCollection1::", new MyCollection1Serializer());
                        _tmp.Add(@"Trumpet::", new TrumpetSerializer());
                        _tmp.Add(@"Address::", new AddressSerializer());
                        _tmp.Add(@"SerializationTypes.TypeWithGetSetArrayMembers::", new TypeWithGetSetArrayMembersSerializer());
                        _tmp.Add(@"SerializationTypes.TypeWithTypesHavingCustomFormatter::", new TypeWithTypesHavingCustomFormatterSerializer());
                        _tmp.Add(@"SerializationTypes.MyEnum::", new MyEnumSerializer());
                        _tmp.Add(@"TypeWithTimeSpanProperty::", new TypeWithTimeSpanPropertySerializer());
                        _tmp.Add(@"SerializationTypes.TypeWithEnumPropertyHavingDefaultValue::", new TypeWithEnumPropertyHavingDefaultValueSerializer());
                        _tmp.Add(@"SerializationTypes.TypeWithXmlSchemaFormAttribute:::True:", new TypeWithXmlSchemaFormAttributeSerializer());
                        _tmp.Add(@"SerializationTypes.DerivedClassWithSameProperty2::", new DerivedClassWithSameProperty2Serializer());
                        _tmp.Add(@"SerializationTypes.TypeHasArrayOfASerializedAsB::", new TypeHasArrayOfASerializedAsBSerializer());
                        _tmp.Add(@"SerializationTypes.ClassImplementsInterface::", new ClassImplementsInterfaceSerializer());
                        _tmp.Add(@"TypeWithXmlNodeArrayProperty:::True:", new TypeWithXmlNodeArrayPropertySerializer());
                        _tmp.Add(@"SerializationTypes.TypeWithPropertiesHavingDefaultValue::", new TypeWithPropertiesHavingDefaultValueSerializer());
                        _tmp.Add(@"SerializationTypes.ServerSettings::", new ServerSettingsSerializer());
                        _tmp.Add(@"SerializationTypes.WithEnums::", new WithEnumsSerializer());
                        _tmp.Add(@"SerializationTypes.TypeWithEnumMembers::", new TypeWithEnumMembersSerializer());
                        _tmp.Add(@"SerializationTypes.SimpleKnownTypeValue::", new SimpleKnownTypeValueSerializer());
                        _tmp.Add(@"SerializationTypes.TypeWithMyCollectionField::", new TypeWithMyCollectionFieldSerializer());
                        _tmp.Add(@"SerializationTypes.TypeWithXmlTextAttributeOnArray:http://schemas.xmlsoap.org/ws/2005/04/discovery::False:", new TypeWithXmlTextAttributeOnArraySerializer());
                        _tmp.Add(@"SerializationTypes.TypeWithNonPublicDefaultConstructor::", new TypeWithNonPublicDefaultConstructorSerializer());
                        _tmp.Add(@"PurchaseOrder:http://www.contoso1.com:PurchaseOrder:False:", new PurchaseOrderSerializer());
                        _tmp.Add(@"SerializationTypes.TypeWithByteArrayAsXmlText::", new TypeWithByteArrayAsXmlTextSerializer());
                        _tmp.Add(@"SerializationTypes.TypeWithDateTimeStringProperty::", new TypeWithDateTimeStringPropertySerializer());
                        _tmp.Add(@"AliasedTestType::", new AliasedTestTypeSerializer());
                        if (typedSerializers == null) typedSerializers = _tmp;
                    }
                    return typedSerializers;
                }
            }
            public override System.Boolean CanSerialize(System.Type type) {
                if (type == typeof(global::TypeWithXmlElementProperty)) return true;
                if (type == typeof(global::TypeWithXmlDocumentProperty)) return true;
                if (type == typeof(global::TypeWithBinaryProperty)) return true;
                if (type == typeof(global::TypeWithTimeSpanProperty)) return true;
                if (type == typeof(global::TypeWithDefaultTimeSpanProperty)) return true;
                if (type == typeof(global::TypeWithByteProperty)) return true;
                if (type == typeof(global::TypeWithXmlNodeArrayProperty)) return true;
                if (type == typeof(global::Animal)) return true;
                if (type == typeof(global::Dog)) return true;
                if (type == typeof(global::DogBreed)) return true;
                if (type == typeof(global::Group)) return true;
                if (type == typeof(global::Vehicle)) return true;
                if (type == typeof(global::Employee)) return true;
                if (type == typeof(global::BaseClass)) return true;
                if (type == typeof(global::DerivedClass)) return true;
                if (type == typeof(global::PurchaseOrder)) return true;
                if (type == typeof(global::Address)) return true;
                if (type == typeof(global::OrderedItem)) return true;
                if (type == typeof(global::AliasedTestType)) return true;
                if (type == typeof(global::BaseClass1)) return true;
                if (type == typeof(global::DerivedClass1)) return true;
                if (type == typeof(global::MyCollection1)) return true;
                if (type == typeof(global::Orchestra)) return true;
                if (type == typeof(global::Instrument)) return true;
                if (type == typeof(global::Brass)) return true;
                if (type == typeof(global::Trumpet)) return true;
                if (type == typeof(global::Pet)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithDateTimeStringProperty)) return true;
                if (type == typeof(global::SerializationTypes.SimpleType)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithGetSetArrayMembers)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithGetOnlyArrayProperties)) return true;
                if (type == typeof(global::SerializationTypes.StructNotSerializable)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithMyCollectionField)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithReadOnlyMyCollectionProperty)) return true;
                if (type == typeof(global::SerializationTypes.MyList)) return true;
                if (type == typeof(global::SerializationTypes.MyEnum)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithEnumMembers)) return true;
                if (type == typeof(global::SerializationTypes.DCStruct)) return true;
                if (type == typeof(global::SerializationTypes.DCClassWithEnumAndStruct)) return true;
                if (type == typeof(global::SerializationTypes.BuiltInTypes)) return true;
                if (type == typeof(global::SerializationTypes.TypeA)) return true;
                if (type == typeof(global::SerializationTypes.TypeB)) return true;
                if (type == typeof(global::SerializationTypes.TypeHasArrayOfASerializedAsB)) return true;
                if (type == typeof(global::SerializationTypes.WithXElement)) return true;
                if (type == typeof(global::SerializationTypes.WithXElementWithNestedXElement)) return true;
                if (type == typeof(global::SerializationTypes.WithArrayOfXElement)) return true;
                if (type == typeof(global::SerializationTypes.WithListOfXElement)) return true;
                if (type == typeof(global::SerializationTypes.@__TypeNameWithSpecialCharacters漢ñ)) return true;
                if (type == typeof(global::SerializationTypes.BaseClassWithSamePropertyName)) return true;
                if (type == typeof(global::SerializationTypes.DerivedClassWithSameProperty)) return true;
                if (type == typeof(global::SerializationTypes.DerivedClassWithSameProperty2)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithDateTimePropertyAsXmlTime)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithByteArrayAsXmlText)) return true;
                if (type == typeof(global::SerializationTypes.SimpleDC)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithXmlTextAttributeOnArray)) return true;
                if (type == typeof(global::SerializationTypes.EnumFlags)) return true;
                if (type == typeof(global::SerializationTypes.ClassImplementsInterface)) return true;
                if (type == typeof(global::SerializationTypes.WithStruct)) return true;
                if (type == typeof(global::SerializationTypes.SomeStruct)) return true;
                if (type == typeof(global::SerializationTypes.WithEnums)) return true;
                if (type == typeof(global::SerializationTypes.WithNullables)) return true;
                if (type == typeof(global::SerializationTypes.ByteEnum)) return true;
                if (type == typeof(global::SerializationTypes.SByteEnum)) return true;
                if (type == typeof(global::SerializationTypes.ShortEnum)) return true;
                if (type == typeof(global::SerializationTypes.IntEnum)) return true;
                if (type == typeof(global::SerializationTypes.UIntEnum)) return true;
                if (type == typeof(global::SerializationTypes.LongEnum)) return true;
                if (type == typeof(global::SerializationTypes.ULongEnum)) return true;
                if (type == typeof(global::SerializationTypes.XmlSerializerAttributes)) return true;
                if (type == typeof(global::SerializationTypes.ItemChoiceType)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithAnyAttribute)) return true;
                if (type == typeof(global::SerializationTypes.KnownTypesThroughConstructor)) return true;
                if (type == typeof(global::SerializationTypes.SimpleKnownTypeValue)) return true;
                if (type == typeof(global::SerializationTypes.ClassImplementingIXmlSerialiable)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithPropertyNameSpecified)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithXmlSchemaFormAttribute)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithTypeNameInXmlTypeAttribute)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithSchemaFormInXmlAttribute)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithNonPublicDefaultConstructor)) return true;
                if (type == typeof(global::SerializationTypes.ServerSettings)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithXmlQualifiedName)) return true;
                if (type == typeof(global::SerializationTypes.TypeWith2DArrayProperty2)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithPropertiesHavingDefaultValue)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithEnumPropertyHavingDefaultValue)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithEnumFlagPropertyHavingDefaultValue)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithShouldSerializeMethod)) return true;
                if (type == typeof(global::SerializationTypes.KnownTypesThroughConstructorWithArrayProperties)) return true;
                if (type == typeof(global::SerializationTypes.KnownTypesThroughConstructorWithValue)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithTypesHavingCustomFormatter)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithArrayPropertyHavingChoice)) return true;
                if (type == typeof(global::SerializationTypes.MoreChoices)) return true;
                if (type == typeof(global::SerializationTypes.TypeWithFieldsOrdered)) return true;
                if (type == typeof(global::Outer.Person)) return true;
                return false;
            }
            public override System.Xml.Serialization.XmlSerializer GetSerializer(System.Type type) {
                if (type == typeof(global::TypeWithXmlElementProperty)) return new TypeWithXmlElementPropertySerializer();
                if (type == typeof(global::TypeWithXmlDocumentProperty)) return new TypeWithXmlDocumentPropertySerializer();
                if (type == typeof(global::TypeWithBinaryProperty)) return new TypeWithBinaryPropertySerializer();
                if (type == typeof(global::TypeWithTimeSpanProperty)) return new TypeWithTimeSpanPropertySerializer();
                if (type == typeof(global::TypeWithDefaultTimeSpanProperty)) return new TypeWithDefaultTimeSpanPropertySerializer();
                if (type == typeof(global::TypeWithByteProperty)) return new TypeWithBytePropertySerializer();
                if (type == typeof(global::TypeWithXmlNodeArrayProperty)) return new TypeWithXmlNodeArrayPropertySerializer();
                if (type == typeof(global::Animal)) return new AnimalSerializer();
                if (type == typeof(global::Dog)) return new DogSerializer();
                if (type == typeof(global::DogBreed)) return new DogBreedSerializer();
                if (type == typeof(global::Group)) return new GroupSerializer();
                if (type == typeof(global::Vehicle)) return new VehicleSerializer();
                if (type == typeof(global::Employee)) return new EmployeeSerializer();
                if (type == typeof(global::BaseClass)) return new BaseClassSerializer();
                if (type == typeof(global::DerivedClass)) return new DerivedClassSerializer();
                if (type == typeof(global::PurchaseOrder)) return new PurchaseOrderSerializer();
                if (type == typeof(global::Address)) return new AddressSerializer();
                if (type == typeof(global::OrderedItem)) return new OrderedItemSerializer();
                if (type == typeof(global::AliasedTestType)) return new AliasedTestTypeSerializer();
                if (type == typeof(global::BaseClass1)) return new BaseClass1Serializer();
                if (type == typeof(global::DerivedClass1)) return new DerivedClass1Serializer();
                if (type == typeof(global::MyCollection1)) return new MyCollection1Serializer();
                if (type == typeof(global::Orchestra)) return new OrchestraSerializer();
                if (type == typeof(global::Instrument)) return new InstrumentSerializer();
                if (type == typeof(global::Brass)) return new BrassSerializer();
                if (type == typeof(global::Trumpet)) return new TrumpetSerializer();
                if (type == typeof(global::Pet)) return new PetSerializer();
                if (type == typeof(global::SerializationTypes.TypeWithDateTimeStringProperty)) return new TypeWithDateTimeStringPropertySerializer();
                if (type == typeof(global::SerializationTypes.SimpleType)) return new SimpleTypeSerializer();
                if (type == typeof(global::SerializationTypes.TypeWithGetSetArrayMembers)) return new TypeWithGetSetArrayMembersSerializer();
                if (type == typeof(global::SerializationTypes.TypeWithGetOnlyArrayProperties)) return new TypeWithGetOnlyArrayPropertiesSerializer();
                if (type == typeof(global::SerializationTypes.StructNotSerializable)) return new StructNotSerializableSerializer();
                if (type == typeof(global::SerializationTypes.TypeWithMyCollectionField)) return new TypeWithMyCollectionFieldSerializer();
                if (type == typeof(global::SerializationTypes.TypeWithReadOnlyMyCollectionProperty)) return new TypeWithReadOnlyMyCollectionPropertySerializer();
                if (type == typeof(global::SerializationTypes.MyList)) return new MyListSerializer();
                if (type == typeof(global::SerializationTypes.MyEnum)) return new MyEnumSerializer();
                if (type == typeof(global::SerializationTypes.TypeWithEnumMembers)) return new TypeWithEnumMembersSerializer();
                if (type == typeof(global::SerializationTypes.DCStruct)) return new DCStructSerializer();
                if (type == typeof(global::SerializationTypes.DCClassWithEnumAndStruct)) return new DCClassWithEnumAndStructSerializer();
                if (type == typeof(global::SerializationTypes.BuiltInTypes)) return new BuiltInTypesSerializer();
                if (type == typeof(global::SerializationTypes.TypeA)) return new TypeASerializer();
                if (type == typeof(global::SerializationTypes.TypeB)) return new TypeBSerializer();
                if (type == typeof(global::SerializationTypes.TypeHasArrayOfASerializedAsB)) return new TypeHasArrayOfASerializedAsBSerializer();
                if (type == typeof(global::SerializationTypes.WithXElement)) return new WithXElementSerializer();
                if (type == typeof(global::SerializationTypes.WithXElementWithNestedXElement)) return new WithXElementWithNestedXElementSerializer();
                if (type == typeof(global::SerializationTypes.WithArrayOfXElement)) return new WithArrayOfXElementSerializer();
                if (type == typeof(global::SerializationTypes.WithListOfXElement)) return new WithListOfXElementSerializer();
                if (type == typeof(global::SerializationTypes.@__TypeNameWithSpecialCharacters漢ñ)) return new __TypeNameWithSpecialCharacters漢ñSerializer();
                if (type == typeof(global::SerializationTypes.BaseClassWithSamePropertyName)) return new BaseClassWithSamePropertyNameSerializer();
                if (type == typeof(global::SerializationTypes.DerivedClassWithSameProperty)) return new DerivedClassWithSamePropertySerializer();
                if (type == typeof(global::SerializationTypes.DerivedClassWithSameProperty2)) return new DerivedClassWithSameProperty2Serializer();
                if (type == typeof(global::SerializationTypes.TypeWithDateTimePropertyAsXmlTime)) return new TypeWithDateTimePropertyAsXmlTimeSerializer();
                if (type == typeof(global::SerializationTypes.TypeWithByteArrayAsXmlText)) return new TypeWithByteArrayAsXmlTextSerializer();
                if (type == typeof(global::SerializationTypes.SimpleDC)) return new SimpleDCSerializer();
                if (type == typeof(global::SerializationTypes.TypeWithXmlTextAttributeOnArray)) return new TypeWithXmlTextAttributeOnArraySerializer();
                if (type == typeof(global::SerializationTypes.EnumFlags)) return new EnumFlagsSerializer();
                if (type == typeof(global::SerializationTypes.ClassImplementsInterface)) return new ClassImplementsInterfaceSerializer();
                if (type == typeof(global::SerializationTypes.WithStruct)) return new WithStructSerializer();
                if (type == typeof(global::SerializationTypes.SomeStruct)) return new SomeStructSerializer();
                if (type == typeof(global::SerializationTypes.WithEnums)) return new WithEnumsSerializer();
                if (type == typeof(global::SerializationTypes.WithNullables)) return new WithNullablesSerializer();
                if (type == typeof(global::SerializationTypes.ByteEnum)) return new ByteEnumSerializer();
                if (type == typeof(global::SerializationTypes.SByteEnum)) return new SByteEnumSerializer();
                if (type == typeof(global::SerializationTypes.ShortEnum)) return new ShortEnumSerializer();
                if (type == typeof(global::SerializationTypes.IntEnum)) return new IntEnumSerializer();
                if (type == typeof(global::SerializationTypes.UIntEnum)) return new UIntEnumSerializer();
                if (type == typeof(global::SerializationTypes.LongEnum)) return new LongEnumSerializer();
                if (type == typeof(global::SerializationTypes.ULongEnum)) return new ULongEnumSerializer();
                if (type == typeof(global::SerializationTypes.XmlSerializerAttributes)) return new XmlSerializerAttributesSerializer();
                if (type == typeof(global::SerializationTypes.ItemChoiceType)) return new ItemChoiceTypeSerializer();
                if (type == typeof(global::SerializationTypes.TypeWithAnyAttribute)) return new TypeWithAnyAttributeSerializer();
                if (type == typeof(global::SerializationTypes.KnownTypesThroughConstructor)) return new KnownTypesThroughConstructorSerializer();
                if (type == typeof(global::SerializationTypes.SimpleKnownTypeValue)) return new SimpleKnownTypeValueSerializer();
                if (type == typeof(global::SerializationTypes.ClassImplementingIXmlSerialiable)) return new ClassImplementingIXmlSerialiableSerializer();
                if (type == typeof(global::SerializationTypes.TypeWithPropertyNameSpecified)) return new TypeWithPropertyNameSpecifiedSerializer();
                if (type == typeof(global::SerializationTypes.TypeWithXmlSchemaFormAttribute)) return new TypeWithXmlSchemaFormAttributeSerializer();
                if (type == typeof(global::SerializationTypes.TypeWithTypeNameInXmlTypeAttribute)) return new TypeWithTypeNameInXmlTypeAttributeSerializer();
                if (type == typeof(global::SerializationTypes.TypeWithSchemaFormInXmlAttribute)) return new TypeWithSchemaFormInXmlAttributeSerializer();
                if (type == typeof(global::SerializationTypes.TypeWithNonPublicDefaultConstructor)) return new TypeWithNonPublicDefaultConstructorSerializer();
                if (type == typeof(global::SerializationTypes.ServerSettings)) return new ServerSettingsSerializer();
                if (type == typeof(global::SerializationTypes.TypeWithXmlQualifiedName)) return new TypeWithXmlQualifiedNameSerializer();
                if (type == typeof(global::SerializationTypes.TypeWith2DArrayProperty2)) return new TypeWith2DArrayProperty2Serializer();
                if (type == typeof(global::SerializationTypes.TypeWithPropertiesHavingDefaultValue)) return new TypeWithPropertiesHavingDefaultValueSerializer();
                if (type == typeof(global::SerializationTypes.TypeWithEnumPropertyHavingDefaultValue)) return new TypeWithEnumPropertyHavingDefaultValueSerializer();
                if (type == typeof(global::SerializationTypes.TypeWithEnumFlagPropertyHavingDefaultValue)) return new TypeWithEnumFlagPropertyHavingDefaultValueSerializer();
                if (type == typeof(global::SerializationTypes.TypeWithShouldSerializeMethod)) return new TypeWithShouldSerializeMethodSerializer();
                if (type == typeof(global::SerializationTypes.KnownTypesThroughConstructorWithArrayProperties)) return new KnownTypesThroughConstructorWithArrayPropertiesSerializer();
                if (type == typeof(global::SerializationTypes.KnownTypesThroughConstructorWithValue)) return new KnownTypesThroughConstructorWithValueSerializer();
                if (type == typeof(global::SerializationTypes.TypeWithTypesHavingCustomFormatter)) return new TypeWithTypesHavingCustomFormatterSerializer();
                if (type == typeof(global::SerializationTypes.TypeWithArrayPropertyHavingChoice)) return new TypeWithArrayPropertyHavingChoiceSerializer();
                if (type == typeof(global::SerializationTypes.MoreChoices)) return new MoreChoicesSerializer();
                if (type == typeof(global::SerializationTypes.TypeWithFieldsOrdered)) return new TypeWithFieldsOrderedSerializer();
                if (type == typeof(global::Outer.Person)) return new PersonSerializer();
                return null;
            }
        }
//#pragma warning restore
}
