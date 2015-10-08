// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OLEDB.Test.ModuleCore;
using Xunit;

namespace System.Xml.Tests
{
    public partial class CharCheckingReaderTest : CGenericTestModule
    {
        private static void RunTestCaseAsync(Func<CTestBase> testCaseGenerator)
        {
            CModInfo.CommandLine = "/async";
            RunTestCase(testCaseGenerator);
        }

        private static void RunTestCase(Func<CTestBase> testCaseGenerator)
        {
            var module = new CharCheckingReaderTest();

            module.Init(null);
            module.AddChild(testCaseGenerator());
            module.Execute();

            Assert.Equal(0, module.FailCount);
        }

        private static void RunTest(Func<CTestBase> testCaseGenerator)
        {
            RunTestCase(testCaseGenerator);
            RunTestCaseAsync(testCaseGenerator);
        }

        [Fact]
        [OuterLoop]
        public static void ErrorConditionReader()
        {
            RunTest(() => new TCErrorConditionReader() { Attribute = new TestCase() { Name = "ErrorCondition", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void DepthReader()
        {
            RunTest(() => new TCDepthReader() { Attribute = new TestCase() { Name = "Depth", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void NamespaceReader()
        {
            RunTest(() => new TCNamespaceReader() { Attribute = new TestCase() { Name = "Namespace", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void IsEmptyElementReader()
        {
            RunTest(() => new TCIsEmptyElementReader() { Attribute = new TestCase() { Name = "IsEmptyElement", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void XmlSpaceReader()
        {
            RunTest(() => new TCXmlSpaceReader() { Attribute = new TestCase() { Name = "XmlSpace", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void XmlLangReader()
        {
            RunTest(() => new TCXmlLangReader() { Attribute = new TestCase() { Name = "XmlLang", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void SkipReader()
        {
            RunTest(() => new TCSkipReader() { Attribute = new TestCase() { Name = "Skip", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void InvalidXMLReader()
        {
            RunTest(() => new TCInvalidXMLReader() { Attribute = new TestCase() { Name = "InvalidXML", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void ReadOuterXmlReader()
        {
            RunTest(() => new TCReadOuterXmlReader() { Attribute = new TestCase() { Name = "ReadOuterXml", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void AttributeAccessReader()
        {
            RunTest(() => new TCAttributeAccessReader() { Attribute = new TestCase() { Name = "AttributeAccess", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void ThisNameReader()
        {
            RunTest(() => new TCThisNameReader() { Attribute = new TestCase() { Name = "This(Name) and This(Name, Namespace)", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void MoveToAttributeReader()
        {
            RunTest(() => new TCMoveToAttributeReader() { Attribute = new TestCase() { Name = "MoveToAttribute(Name) and MoveToAttribute(Name, Namespace)", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void GetAttributeOrdinalReader()
        {
            RunTest(() => new TCGetAttributeOrdinalReader() { Attribute = new TestCase() { Name = "GetAttribute (Ordinal)", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void GetAttributeNameReader()
        {
            RunTest(() => new TCGetAttributeNameReader() { Attribute = new TestCase() { Name = "GetAttribute(Name) and GetAttribute(Name, Namespace)", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void ThisOrdinalReader()
        {
            RunTest(() => new TCThisOrdinalReader() { Attribute = new TestCase() { Name = "This [Ordinal]", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void MoveToAttributeOrdinalReader()
        {
            RunTest(() => new TCMoveToAttributeOrdinalReader() { Attribute = new TestCase() { Name = "MoveToAttribute(Ordinal)", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void MoveToFirstAttributeReader()
        {
            RunTest(() => new TCMoveToFirstAttributeReader() { Attribute = new TestCase() { Name = "MoveToFirstAttribute()", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void MoveToNextAttributeReader()
        {
            RunTest(() => new TCMoveToNextAttributeReader() { Attribute = new TestCase() { Name = "MoveToNextAttribute()", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void AttributeTestReader()
        {
            RunTest(() => new TCAttributeTestReader() { Attribute = new TestCase() { Name = "Attribute Test when NodeType != Attributes", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void AttributeXmlDeclarationReader()
        {
            RunTest(() => new TCAttributeXmlDeclarationReader() { Attribute = new TestCase() { Name = "Attributes test on XmlDeclaration", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void XmlnsReader()
        {
            RunTest(() => new TCXmlnsReader() { Attribute = new TestCase() { Name = "xmlns as local name", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void XmlnsPrefixReader()
        {
            RunTest(() => new TCXmlnsPrefixReader() { Attribute = new TestCase() { Name = "bounded namespace to xmlns prefix", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void ReadInnerXmlReader()
        {
            RunTest(() => new TCReadInnerXmlReader() { Attribute = new TestCase() { Name = "ReadInnerXml", Desc = "CharCheckingReader" } });
        }
        [Fact]
        [OuterLoop]
        public static void MoveToContentReader()
        {
            RunTest(() => new TCMoveToContentReader() { Attribute = new TestCase() { Name = "MoveToContent", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void IsStartElementReader()
        {
            RunTest(() => new TCIsStartElementReader() { Attribute = new TestCase() { Name = "IsStartElement", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void ReadStartElementReader()
        {
            RunTest(() => new TCReadStartElementReader() { Attribute = new TestCase() { Name = "ReadStartElement", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void ReadEndElementReader()
        {
            RunTest(() => new TCReadEndElementReader() { Attribute = new TestCase() { Name = "ReadEndElement", Desc = "CharCheckingReader" } });
        }
        [Fact]
        [OuterLoop]
        public static void ResolveEntityReader()
        {
            RunTest(() => new TCResolveEntityReader() { Attribute = new TestCase() { Name = "ResolveEntity and ReadAttributeValue", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void HasValueReader()
        {
            RunTest(() => new TCHasValueReader() { Attribute = new TestCase() { Name = "HasValue", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void ReadAttributeValueReader()
        {
            RunTest(() => new TCReadAttributeValueReader() { Attribute = new TestCase() { Name = "ReadAttributeValue", Desc = "CharCheckingReader" } });
        }
        [Fact]
        [OuterLoop]
        public static void ReadReader()
        {
            RunTest(() => new TCReadReader() { Attribute = new TestCase() { Name = "Read", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void MoveToElementReader()
        {
            RunTest(() => new TCMoveToElementReader() { Attribute = new TestCase() { Name = "MoveToElement", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void DisposeReader()
        {
            RunTest(() => new TCDisposeReader() { Attribute = new TestCase() { Name = "Dispose", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void BufferBoundariesReader()
        {
            RunTest(() => new TCBufferBoundariesReader() { Attribute = new TestCase() { Name = "Buffer Boundaries", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void XmlNodeIntegrityTestFileBeforeRead()
        {
            RunTest(() => new TCXmlNodeIntegrityTestFile() { Attribute = new TestCase() { Name = "BeforeRead", Desc = "BeforeRead" } });
        }

        [Fact]
        [OuterLoop]
        public static void XmlNodeIntegrityTestFileAfterCloseInMiddle()
        {
            RunTest(() => new TCXmlNodeIntegrityTestFile() { Attribute = new TestCase() { Name = "AfterCloseInTheMiddle", Desc = "AfterCloseInTheMiddle" } });
        }

        [Fact]
        [OuterLoop]
        public static void XmlNodeIntegrityTestFileAfterClose()
        {
            RunTest(() => new TCXmlNodeIntegrityTestFile() { Attribute = new TestCase() { Name = "AfterClose", Desc = "AfterClose" } });
        }

        [Fact]
        [OuterLoop]
        public static void XmlNodeIntegrityTestFileAfterReadIsFalse()
        {
            RunTest(() => new TCXmlNodeIntegrityTestFile() { Attribute = new TestCase() { Name = "AfterReadIsFalse", Desc = "AfterReadIsFalse" } });
        }

        [Fact]
        [OuterLoop]
        public static void ReadSubtreeReader()
        {
            RunTest(() => new TCReadSubtreeReader() { Attribute = new TestCase() { Name = "Read Subtree", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void ReadToDescendantReader()
        {
            RunTest(() => new TCReadToDescendantReader() { Attribute = new TestCase() { Name = "ReadToDescendant", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void ReadToNextSiblingReader()
        {
            RunTest(() => new TCReadToNextSiblingReader() { Attribute = new TestCase() { Name = "ReadToNextSibling", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void ReadValueReader()
        {
            RunTest(() => new TCReadValueReader() { Attribute = new TestCase() { Name = "ReadValue", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void ReadContentAsBase64Reader()
        {
            RunTest(() => new TCReadContentAsBase64Reader() { Attribute = new TestCase() { Name = "ReadContentAsBase64", Desc = "CharCheckingReader" } });
        }
        [Fact]
        [OuterLoop]
        public static void ReadElementContentAsBase64Reader()
        {
            RunTest(() => new TCReadElementContentAsBase64Reader() { Attribute = new TestCase() { Name = "ReadElementContentAsBase64", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void ReadContentAsBinHexReader()
        {
            RunTest(() => new TCReadContentAsBinHexReader() { Attribute = new TestCase() { Name = "ReadContentAsBinHex", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void ReadElementContentAsBinHexReader()
        {
            RunTest(() => new TCReadElementContentAsBinHexReader() { Attribute = new TestCase() { Name = "ReadElementContentAsBinHex", Desc = "CharCheckingReader" } });
        }

        [Fact]
        [OuterLoop]
        public static void ReadToFollowingReader()
        {
            RunTest(() => new TCReadToFollowingReader() { Attribute = new TestCase() { Name = "ReadToFollowing", Desc = "CharCheckingReader" } });
        }
    }
}
