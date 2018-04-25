// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Text;
using Microsoft.Test.ModuleCore;
using System.IO;
using XmlCoreTest.Common;
using Xunit;

namespace CoreXml.Test.XLinq
{
    public class XNodeReaderFunctionalTests : TestModule
    {
        // Type is CoreXml.Test.XLinq.FunctionalTests
        // Test Module
        [Fact]
        [OuterLoop]
        public static void RunTests()
        {
            TestInput.CommandLine = "";
            XNodeReaderFunctionalTests module = new XNodeReaderFunctionalTests();
            module.Init();

            module.AddChild(new XNodeReaderTests() { Attribute = new TestCaseAttribute() { Name = "XNodeReader", Desc = "XLinq XNodeReader Tests" } });
            module.Execute();

            Assert.Equal(0, module.FailCount);
        }

        #region Class
        public class XNodeReaderTests : XLinqTestCase
        {
            // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests
            public override void AddChildren()
            {
                this.AddChild(new TCDispose() { Attribute = new TestCaseAttribute() { Name = "Dispose", Desc = "Dispose" } });
                this.AddChild(new TCDepth() { Attribute = new TestCaseAttribute() { Name = "Depth", Desc = "Depth" } });
                this.AddChild(new TCNamespace() { Attribute = new TestCaseAttribute() { Name = "Namespace", Desc = "Namespace" } });
                this.AddChild(new TCLookupNamespace() { Attribute = new TestCaseAttribute() { Name = "LookupNamespace", Desc = "LookupNamespace" } });
                this.AddChild(new TCHasValue() { Attribute = new TestCaseAttribute() { Name = "HasValue", Desc = "HasValue" } });
                this.AddChild(new TCIsEmptyElement2() { Attribute = new TestCaseAttribute() { Name = "IsEmptyElement", Desc = "IsEmptyElement" } });
                this.AddChild(new TCXmlSpace() { Attribute = new TestCaseAttribute() { Name = "XmlSpace", Desc = "XmlSpace" } });
                this.AddChild(new TCXmlLang() { Attribute = new TestCaseAttribute() { Name = "XmlLang", Desc = "XmlLang" } });
                this.AddChild(new TCSkip() { Attribute = new TestCaseAttribute() { Name = "Skip", Desc = "Skip" } });
                this.AddChild(new CXMLGeneralTestFunctionalTests.XNodeReaderTests.TCIsDefault() { Attribute = new TestCaseAttribute() { Name = "IsDefault", Desc = "IsDefault" } });
                this.AddChild(new TCBaseURI() { Attribute = new TestCaseAttribute() { Name = "BaseUri", Desc = "BaseUri" } });
                this.AddChild(new TCAttributeAccess() { Attribute = new TestCaseAttribute() { Name = "AttributeAccess", Desc = "AttributeAccess" } });
                this.AddChild(new TCThisName() { Attribute = new TestCaseAttribute() { Name = "ThisName", Desc = "ThisName" } });
                this.AddChild(new TCMoveToAttributeReader() { Attribute = new TestCaseAttribute() { Name = "MoveToAttribute", Desc = "MoveToAttribute" } });
                this.AddChild(new TCGetAttributeOrdinal() { Attribute = new TestCaseAttribute() { Name = "GetAttributeOrdinal", Desc = "GetAttributeOrdinal" } });
                this.AddChild(new TCGetAttributeName() { Attribute = new TestCaseAttribute() { Name = "GetAttributeName", Desc = "GetAttributeName" } });
                this.AddChild(new TCThisOrdinal() { Attribute = new TestCaseAttribute() { Name = "ThisOrdinal", Desc = "ThisOrdinal" } });
                this.AddChild(new TCMoveToAttributeOrdinal() { Attribute = new TestCaseAttribute() { Name = "MoveToAttributeOrdinal", Desc = "MoveToAttributeOrdinal" } });
                this.AddChild(new TCMoveToFirstAttribute() { Attribute = new TestCaseAttribute() { Name = "MoveToFirstAttribute", Desc = "MoveToFirstAttribute" } });
                this.AddChild(new TCMoveToNextAttribute() { Attribute = new TestCaseAttribute() { Name = "MoveToNextAttribute", Desc = "MoveToNextAttribute" } });
                this.AddChild(new TCAttributeTest() { Attribute = new TestCaseAttribute() { Name = "AttributeTest", Desc = "AttributeTest" } });
                this.AddChild(new TCXmlns() { Attribute = new TestCaseAttribute() { Name = "Xlmns", Desc = "Xlmns" } });
                this.AddChild(new TCXmlnsPrefix() { Attribute = new TestCaseAttribute() { Name = "XlmnsPrefix", Desc = "XlmnsPrefix" } });
                this.AddChild(new TCReadState() { Attribute = new TestCaseAttribute() { Name = "ReadState", Desc = "ReadState" } });
                this.AddChild(new TCReadInnerXml() { Attribute = new TestCaseAttribute() { Name = "ReadInnerXml", Desc = "ReadInnerXml" } });
                this.AddChild(new TCMoveToContent() { Attribute = new TestCaseAttribute() { Name = "MoveToContent", Desc = "MoveToContent" } });
                this.AddChild(new TCIsStartElement() { Attribute = new TestCaseAttribute() { Name = "IsStartElement", Desc = "IsStartElement" } });
                this.AddChild(new TCReadStartElement() { Attribute = new TestCaseAttribute() { Name = "ReadStartElement", Desc = "ReadStartElement" } });
                this.AddChild(new TCReadEndElement() { Attribute = new TestCaseAttribute() { Name = "ReadEndElement", Desc = "ReadEndElement" } });
                this.AddChild(new TCMoveToElement() { Attribute = new TestCaseAttribute() { Name = "MoveToElement", Desc = "MoveToElement" } });
                this.AddChild(new ErrorConditions() { Attribute = new TestCaseAttribute() { Name = "ErrorConditions" } });
                this.AddChild(new TCXMLIntegrityBase() { Attribute = new TestCaseAttribute() { Name = "XMLIntegrityBase", Desc = "XMLIntegrityBase" } });
                this.AddChild(new TCReadContentAsBase64() { Attribute = new TestCaseAttribute() { Name = "ReadContentAsBase64", Desc = "ReadContentAsBase64" } });
                this.AddChild(new TCReadElementContentAsBase64() { Attribute = new TestCaseAttribute() { Name = "ReadElementContentAsBase64", Desc = "ReadElementContentAsBase64" } });
                this.AddChild(new TCReadContentAsBinHex() { Attribute = new TestCaseAttribute() { Name = "ReadContentAsBinHex", Desc = "ReadContentAsBinHex" } });
                this.AddChild(new TCReadElementContentAsBinHex() { Attribute = new TestCaseAttribute() { Name = "ReadElementContentAsBinHex", Desc = "ReadElementContentAsBinHex" } });
                this.AddChild(new CReaderTestModule() { Attribute = new TestCaseAttribute() { Name = "ReaderProperty", Desc = "Reader Property" } });
                this.AddChild(new TCReadOuterXml() { Attribute = new TestCaseAttribute() { Name = "ReadOuterXml", Desc = "ReadOuterXml" } });
                this.AddChild(new TCReadSubtree() { Attribute = new TestCaseAttribute() { Name = "ReadSubtree", Desc = "ReadSubtree" } });
                this.AddChild(new TCReadToDescendant() { Attribute = new TestCaseAttribute() { Name = "ReadToDescendant", Desc = "ReadToDescendant" } });
                this.AddChild(new TCReadToFollowing() { Attribute = new TestCaseAttribute() { Name = "ReadToFollowing", Desc = "ReadToFollowing" } });
                this.AddChild(new TCReadToNextSibling() { Attribute = new TestCaseAttribute() { Name = "ReadToNextSibling", Desc = "ReadToNextSibling" } });
                this.AddChild(new TCReadValue() { Attribute = new TestCaseAttribute() { Name = "ReadValue", Desc = "ReadValue" } });
                this.AddChild(new XNodeReaderAPI() { Attribute = new TestCaseAttribute() { Name = "API Tests" } });
            }
            public class TCDispose : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCDispose
                // Test Case
                public override void AddChildren()
                {
                    var tcDispose = new CommonTestFunctionalTests.XNodeReaderTests.TCDispose();
                    this.AddChild(new TestVariation(tcDispose.Variation1) { Attribute = new VariationAttribute("Test Integrity of all values after Dispose") });
                    this.AddChild(new TestVariation(tcDispose.Variation2) { Attribute = new VariationAttribute("Call Dispose Multiple(3) Times") });
                }
            }
            public class TCDepth : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCDepth
                // Test Case
                public override void AddChildren()
                {
                    var tcDepth = new CXMLGeneralTestFunctionalTests.XNodeReaderTests.TCDepth();
                    this.AddChild(new TestVariation(tcDepth.TestDepth1) { Attribute = new VariationAttribute("XmlReader Depth at the Root") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcDepth.TestDepth2) { Attribute = new VariationAttribute("XmlReader Depth at Empty Tag") });
                    this.AddChild(new TestVariation(tcDepth.TestDepth3) { Attribute = new VariationAttribute("XmlReader Depth at Empty Tag with Attributes") });
                    this.AddChild(new TestVariation(tcDepth.TestDepth4) { Attribute = new VariationAttribute("XmlReader Depth at Non Empty Tag with Text") });
                }
            }
            public class TCNamespace : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCNamespace
                // Test Case
                public override void AddChildren()
                {
                    var tcNamespace = new CXMLGeneralTestFunctionalTests.XNodeReaderTests.TCNamespace();
                    this.AddChild(new TestVariation(tcNamespace.TestNamespace1) { Attribute = new VariationAttribute("Namespace test within a scope (no nested element)") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcNamespace.TestNamespace2) { Attribute = new VariationAttribute("Namespace test within a scope (with nested element)") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcNamespace.TestNamespace3) { Attribute = new VariationAttribute("Namespace test immediately outside the Namespace scope") });
                    this.AddChild(new TestVariation(tcNamespace.TestNamespace4) { Attribute = new VariationAttribute("Namespace test Attribute should has no default namespace") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcNamespace.TestNamespace5) { Attribute = new VariationAttribute("Namespace test with multiple Namespace declaration") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcNamespace.TestNamespace6) { Attribute = new VariationAttribute("Namespace test with multiple Namespace declaration, including default namespace") });
                    this.AddChild(new TestVariation(tcNamespace.TestNamespace7) { Attribute = new VariationAttribute("Namespace URI for xml prefix") { Priority = 0 } });
                }
            }
            public class TCLookupNamespace : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCLookupNamespace
                // Test Case
                public override void AddChildren()
                {
                    var tcLookupNamespace = new CXMLGeneralTestFunctionalTests.XNodeReaderTests.TCLookupNamespace();
                    this.AddChild(new TestVariation(tcLookupNamespace.LookupNamespace1) { Attribute = new VariationAttribute("LookupNamespace test within EmptyTag") });
                    this.AddChild(new TestVariation(tcLookupNamespace.LookupNamespace2) { Attribute = new VariationAttribute("LookupNamespace test with Default namespace within EmptyTag") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcLookupNamespace.LookupNamespace3) { Attribute = new VariationAttribute("LookupNamespace test within a scope (no nested element)") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcLookupNamespace.LookupNamespace4) { Attribute = new VariationAttribute("LookupNamespace test within a scope (with nested element)") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcLookupNamespace.LookupNamespace5) { Attribute = new VariationAttribute("LookupNamespace test immediately outside the Namespace scope") });
                    this.AddChild(new TestVariation(tcLookupNamespace.LookupNamespace6) { Attribute = new VariationAttribute("LookupNamespace test with multiple Namespace declaration") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcLookupNamespace.LookupNamespace7) { Attribute = new VariationAttribute("Namespace test with multiple Namespace declaration, including default namespace") });
                    this.AddChild(new TestVariation(tcLookupNamespace.LookupNamespace8) { Attribute = new VariationAttribute("LookupNamespace on whitespace node PreserveWhitespaces = true") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcLookupNamespace.LookupNamespace9) { Attribute = new VariationAttribute("Different prefix on inner element for the same namespace") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcLookupNamespace.LookupNamespace10) { Attribute = new VariationAttribute("LookupNamespace when Namespaces = false") { Priority = 0 } });
                }
            }
            public class TCHasValue : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCHasValue
                // Test Case
                public override void AddChildren()
                {
                    var tcHasValue = new CXMLGeneralTestFunctionalTests.XNodeReaderTests.TCHasValue();
                    this.AddChild(new TestVariation(tcHasValue.TestHasValueNodeType_None) { Attribute = new VariationAttribute("HasValue On None") });
                    this.AddChild(new TestVariation(tcHasValue.TestHasValueNodeType_Element) { Attribute = new VariationAttribute("HasValue On Element") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcHasValue.TestHasValue1) { Attribute = new VariationAttribute("Get node with a scalar value, verify the value with valid ReadString") });
                    this.AddChild(new TestVariation(tcHasValue.TestHasValueNodeType_Attribute) { Attribute = new VariationAttribute("HasValue On Attribute") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcHasValue.TestHasValueNodeType_Text) { Attribute = new VariationAttribute("HasValue On Text") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcHasValue.TestHasValueNodeType_CDATA) { Attribute = new VariationAttribute("HasValue On CDATA") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcHasValue.TestHasValueNodeType_ProcessingInstruction) { Attribute = new VariationAttribute("HasValue On ProcessingInstruction") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcHasValue.TestHasValueNodeType_Comment) { Attribute = new VariationAttribute("HasValue On Comment") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcHasValue.TestHasValueNodeType_DocumentType) { Attribute = new VariationAttribute("HasValue On DocumentType") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcHasValue.TestHasValueNodeType_Whitespace) { Attribute = new VariationAttribute("HasValue On Whitespace PreserveWhitespaces = true") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcHasValue.TestHasValueNodeType_EndElement) { Attribute = new VariationAttribute("HasValue On EndElement") });
                    this.AddChild(new TestVariation(tcHasValue.TestHasValueNodeType_XmlDeclaration) { Attribute = new VariationAttribute("HasValue On XmlDeclaration") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcHasValue.TestHasValueNodeType_EntityReference) { Attribute = new VariationAttribute("HasValue On EntityReference") });
                    this.AddChild(new TestVariation(tcHasValue.TestHasValueNodeType_EndEntity) { Attribute = new VariationAttribute("HasValue On EndEntity") });
                    this.AddChild(new TestVariation(tcHasValue.v13) { Attribute = new VariationAttribute("PI Value containing surrogates") { Priority = 0 } });
                }
            }
            public class TCIsEmptyElement2 : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCIsEmptyElement2
                // Test Case
                public override void AddChildren()
                {
                    var tcIsEmptyElement2 = new CXMLGeneralTestFunctionalTests.XNodeReaderTests.TCIsEmptyElement2();
                    this.AddChild(new TestVariation(tcIsEmptyElement2.TestEmpty1) { Attribute = new VariationAttribute("Set and Get an element that ends with />") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcIsEmptyElement2.TestEmpty2) { Attribute = new VariationAttribute("Set and Get an element with an attribute that ends with />") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcIsEmptyElement2.TestEmpty3) { Attribute = new VariationAttribute("Set and Get an element that ends without />") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcIsEmptyElement2.TestEmpty4) { Attribute = new VariationAttribute("Set and Get an element with an attribute that ends with />") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcIsEmptyElement2.TestEmptyNodeType_Element) { Attribute = new VariationAttribute("IsEmptyElement On Element") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcIsEmptyElement2.TestEmptyNodeType_None) { Attribute = new VariationAttribute("IsEmptyElement On None") });
                    this.AddChild(new TestVariation(tcIsEmptyElement2.TestEmptyNodeType_Text) { Attribute = new VariationAttribute("IsEmptyElement On Text") });
                    this.AddChild(new TestVariation(tcIsEmptyElement2.TestEmptyNodeType_CDATA) { Attribute = new VariationAttribute("IsEmptyElement On CDATA") });
                    this.AddChild(new TestVariation(tcIsEmptyElement2.TestEmptyNodeType_ProcessingInstruction) { Attribute = new VariationAttribute("IsEmptyElement On ProcessingInstruction") });
                    this.AddChild(new TestVariation(tcIsEmptyElement2.TestEmptyNodeType_Comment) { Attribute = new VariationAttribute("IsEmptyElement On Comment") });
                    this.AddChild(new TestVariation(tcIsEmptyElement2.TestEmptyNodeType_DocumentType) { Attribute = new VariationAttribute("IsEmptyElement On DocumentType") });
                    this.AddChild(new TestVariation(tcIsEmptyElement2.TestEmptyNodeType_Whitespace) { Attribute = new VariationAttribute("IsEmptyElement On Whitespace PreserveWhitespaces = true") });
                    this.AddChild(new TestVariation(tcIsEmptyElement2.TestEmptyNodeType_EndElement) { Attribute = new VariationAttribute("IsEmptyElement On EndElement") });
                    this.AddChild(new TestVariation(tcIsEmptyElement2.TestEmptyNodeType_EntityReference) { Attribute = new VariationAttribute("IsEmptyElement On EntityReference") });
                    this.AddChild(new TestVariation(tcIsEmptyElement2.TestEmptyNodeType_EndEntity) { Attribute = new VariationAttribute("IsEmptyElement On EndEntity") });
                }
            }
            public class TCXmlSpace : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCXmlSpace
                // Test Case
                public override void AddChildren()
                {
                    var tcXmlSpace = new CXMLGeneralTestFunctionalTests.XNodeReaderTests.TCXmlSpace();
                    this.AddChild(new TestVariation(tcXmlSpace.TestXmlSpace1) { Attribute = new VariationAttribute("XmlSpace test within EmptyTag") });
                    this.AddChild(new TestVariation(tcXmlSpace.TestXmlSpace2) { Attribute = new VariationAttribute("Xmlspace test within a scope (no nested element)") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcXmlSpace.TestXmlSpace3) { Attribute = new VariationAttribute("Xmlspace test within a scope (with nested element)") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcXmlSpace.TestXmlSpace4) { Attribute = new VariationAttribute("Xmlspace test immediately outside the XmlSpace scope") });
                    this.AddChild(new TestVariation(tcXmlSpace.TestXmlSpace5) { Attribute = new VariationAttribute("XmlSpace test with multiple XmlSpace declaration") });
                }
            }
            public class TCXmlLang : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCXmlLang
                // Test Case
                public override void AddChildren()
                {
                    var tcXmlLang = new CXMLGeneralTestFunctionalTests.XNodeReaderTests.TCXmlLang();
                    this.AddChild(new TestVariation(tcXmlLang.TestXmlLang1) { Attribute = new VariationAttribute("XmlLang test within EmptyTag") });
                    this.AddChild(new TestVariation(tcXmlLang.TestXmlLang2) { Attribute = new VariationAttribute("XmlLang test within a scope (no nested element)") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcXmlLang.TestXmlLang3) { Attribute = new VariationAttribute("XmlLang test within a scope (with nested element)") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcXmlLang.TestXmlLang4) { Attribute = new VariationAttribute("XmlLang test immediately outside the XmlLang scope") });
                    this.AddChild(new TestVariation(tcXmlLang.TestXmlLang5) { Attribute = new VariationAttribute("XmlLang test with multiple XmlLang declaration") });
                    this.AddChild(new TestVariation(tcXmlLang.TestXmlLang6) { Attribute = new VariationAttribute("XmlLang valid values") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcXmlLang.TestXmlTextReaderLang1) { Attribute = new VariationAttribute("More XmlLang valid values") });
                }
            }
            public class TCSkip : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCSkip
                // Test Case
                public override void AddChildren()
                {
                    var tcSkip = new CXMLGeneralTestFunctionalTests.XNodeReaderTests.TCSkip();
                    this.AddChild(new TestVariation(tcSkip.TestSkip1) { Attribute = new VariationAttribute("Call Skip on empty element") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcSkip.TestSkip2) { Attribute = new VariationAttribute("Call Skip on element") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcSkip.TestSkip3) { Attribute = new VariationAttribute("Call Skip on element with content") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcSkip.TestSkip4) { Attribute = new VariationAttribute("Call Skip on text node (leave node)") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcSkip.skip307543) { Attribute = new VariationAttribute("Call Skip in while read loop") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcSkip.TestSkip5) { Attribute = new VariationAttribute("Call Skip on text node with another element: <elem2>text<elem3></elem3></elem2>") });
                    this.AddChild(new TestVariation(tcSkip.TestSkip6) { Attribute = new VariationAttribute("Call Skip on attribute") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcSkip.TestSkip7) { Attribute = new VariationAttribute("Call Skip on text node of attribute") });
                    this.AddChild(new TestVariation(tcSkip.TestSkip8) { Attribute = new VariationAttribute("Call Skip on CDATA") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcSkip.TestSkip9) { Attribute = new VariationAttribute("Call Skip on Processing Instruction") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcSkip.TestSkip10) { Attribute = new VariationAttribute("Call Skip on Comment") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcSkip.TestSkip12) { Attribute = new VariationAttribute("Call Skip on Whitespace") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcSkip.TestSkip13) { Attribute = new VariationAttribute("Call Skip on EndElement") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcSkip.TestSkip14) { Attribute = new VariationAttribute("Call Skip on root Element") });
                    this.AddChild(new TestVariation(tcSkip.XmlTextReaderDoesNotThrowWhenHandlingAmpersands) { Attribute = new VariationAttribute("XmlTextReader ArgumentOutOfRangeException when handling ampersands") });
                }
            }
            public class TCBaseURI : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCBaseURI
                // Test Case
                public override void AddChildren()
                {
                    var tcBaseURI = new CXMLGeneralTestFunctionalTests.XNodeReaderTests.TCBaseURI();
                    this.AddChild(new TestVariation(tcBaseURI.TestBaseURI1) { Attribute = new VariationAttribute("BaseURI for element node") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcBaseURI.TestBaseURI2) { Attribute = new VariationAttribute("BaseURI for attribute node") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcBaseURI.TestBaseURI3) { Attribute = new VariationAttribute("BaseURI for text node") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcBaseURI.TestBaseURI4) { Attribute = new VariationAttribute("BaseURI for CDATA node") });
                    this.AddChild(new TestVariation(tcBaseURI.TestBaseURI6) { Attribute = new VariationAttribute("BaseURI for PI node") });
                    this.AddChild(new TestVariation(tcBaseURI.TestBaseURI7) { Attribute = new VariationAttribute("BaseURI for Comment node") });
                    this.AddChild(new TestVariation(tcBaseURI.TestBaseURI9) { Attribute = new VariationAttribute("BaseURI for Whitespace node PreserveWhitespaces = true") });
                    this.AddChild(new TestVariation(tcBaseURI.TestBaseURI10) { Attribute = new VariationAttribute("BaseURI for EndElement node") });
                    this.AddChild(new TestVariation(tcBaseURI.TestTextReaderBaseURI4) { Attribute = new VariationAttribute("BaseURI for external General Entity") });
                }
            }
            public class TCAttributeAccess : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCAttributeAccess
                // Test Case
                public override void AddChildren()
                {
                    var tcAttributeAccess = new CXMLReaderAttrTestFunctionalTests.XNodeReaderTests.TCAttributeAccess();
                    this.AddChild(new TestVariation(tcAttributeAccess.TestAttributeAccess1) { Attribute = new VariationAttribute("Attribute Access test using ordinal (Ascending Order)") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcAttributeAccess.TestAttributeAccess2) { Attribute = new VariationAttribute("Attribute Access test using ordinal (Descending Order)") });
                    this.AddChild(new TestVariation(tcAttributeAccess.TestAttributeAccess3) { Attribute = new VariationAttribute("Attribute Access test using ordinal (Odd number)") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcAttributeAccess.TestAttributeAccess4) { Attribute = new VariationAttribute("Attribute Access test using ordinal (Even number)") });
                    this.AddChild(new TestVariation(tcAttributeAccess.TestAttributeAccess5) { Attribute = new VariationAttribute("Attribute Access with namespace=null") });
                }
            }
            public class TCThisName : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCThisName
                // Test Case
                public override void AddChildren()
                {
                    var tcThisName = new CXMLReaderAttrTestFunctionalTests.XNodeReaderTests.TCThisName();
                    this.AddChild(new TestVariation(tcThisName.ThisWithName1) { Attribute = new VariationAttribute("This[Name] Verify with GetAttribute(Name)") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcThisName.ThisWithName2) { Attribute = new VariationAttribute("This[Name, null] Verify with GetAttribute(Name)") });
                    this.AddChild(new TestVariation(tcThisName.ThisWithName3) { Attribute = new VariationAttribute("This[Name] Verify with GetAttribute(Name,null)") });
                    this.AddChild(new TestVariation(tcThisName.ThisWithName4) { Attribute = new VariationAttribute("This[Name, NamespaceURI] Verify with GetAttribute(Name, NamespaceURI)") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcThisName.ThisWithName5) { Attribute = new VariationAttribute("This[Name, null] Verify not the same as GetAttribute(Name, NamespaceURI)") });
                    this.AddChild(new TestVariation(tcThisName.ThisWithName6) { Attribute = new VariationAttribute("This[Name, NamespaceURI] Verify not the same as GetAttribute(Name, null)") });
                    this.AddChild(new TestVariation(tcThisName.ThisWithName7) { Attribute = new VariationAttribute("This[Name] Verify with MoveToAttribute(Name)") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcThisName.ThisWithName8) { Attribute = new VariationAttribute("This[Name, null] Verify with MoveToAttribute(Name)") });
                    this.AddChild(new TestVariation(tcThisName.ThisWithName9) { Attribute = new VariationAttribute("This[Name] Verify with MoveToAttribute(Name,null)") });
                    this.AddChild(new TestVariation(tcThisName.ThisWithName10) { Attribute = new VariationAttribute("This[Name, NamespaceURI] Verify not the same as MoveToAttribute(Name, null)") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcThisName.ThisWithName11) { Attribute = new VariationAttribute("This[Name, null] Verify not the same as MoveToAttribute(Name, NamespaceURI)") });
                    this.AddChild(new TestVariation(tcThisName.ThisWithName12) { Attribute = new VariationAttribute("This[Name, namespace] Verify not the same as MoveToAttribute(Name, namespace)") });
                    this.AddChild(new TestVariation(tcThisName.ThisWithName13) { Attribute = new VariationAttribute("This(String.Empty)") });
                    this.AddChild(new TestVariation(tcThisName.ThisWithName14) { Attribute = new VariationAttribute("This[String.Empty,String.Empty]") });
                    this.AddChild(new TestVariation(tcThisName.ThisWithName15) { Attribute = new VariationAttribute("This[QName] Verify with GetAttribute(Name, NamespaceURI)") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcThisName.ThisWithName16) { Attribute = new VariationAttribute("This[QName] invalid Qname") });
                }
            }
            public class TCMoveToAttributeReader : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCMoveToAttributeReader
                // Test Case
                public override void AddChildren()
                {
                    var tcMoveToAttributeReader = new CXMLReaderAttrTestFunctionalTests.XNodeReaderTests.TCMoveToAttributeReader();
                    this.AddChild(new TestVariation(tcMoveToAttributeReader.MoveToAttributeWithName1) { Attribute = new VariationAttribute("MoveToAttribute(String.Empty)") });
                    this.AddChild(new TestVariation(tcMoveToAttributeReader.MoveToAttributeWithName2) { Attribute = new VariationAttribute("MoveToAttribute(String.Empty,String.Empty)") });
                }
            }
            public class TCGetAttributeOrdinal : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCGetAttributeOrdinal
                // Test Case
                public override void AddChildren()
                {
                    var tcGetAttributeOrdinal = new CXMLReaderAttrTestFunctionalTests.XNodeReaderTests.TCGetAttributeOrdinal();
                    this.AddChild(new TestVariation(tcGetAttributeOrdinal.GetAttributeWithGetAttrDoubleQ) { Attribute = new VariationAttribute("GetAttribute(i) Verify with This[i] - Double Quote") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcGetAttributeOrdinal.OrdinalWithGetAttrSingleQ) { Attribute = new VariationAttribute("GetAttribute[i] Verify with This[i] - Single Quote") });
                    this.AddChild(new TestVariation(tcGetAttributeOrdinal.GetAttributeWithMoveAttrDoubleQ) { Attribute = new VariationAttribute("GetAttribute(i) Verify with MoveToAttribute[i] - Double Quote") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcGetAttributeOrdinal.GetAttributeWithMoveAttrSingleQ) { Attribute = new VariationAttribute("GetAttribute(i) Verify with MoveToAttribute[i] - Single Quote") });
                    this.AddChild(new TestVariation(tcGetAttributeOrdinal.NegativeOneOrdinal) { Attribute = new VariationAttribute("GetAttribute(i) NegativeOneOrdinal") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcGetAttributeOrdinal.FieldCountOrdinal) { Attribute = new VariationAttribute("GetAttribute(i) FieldCountOrdinal") });
                    this.AddChild(new TestVariation(tcGetAttributeOrdinal.OrdinalPlusOne) { Attribute = new VariationAttribute("GetAttribute(i) OrdinalPlusOne") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcGetAttributeOrdinal.OrdinalMinusOne) { Attribute = new VariationAttribute("GetAttribute(i) OrdinalMinusOne") });
                }
            }
            public class TCGetAttributeName : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCGetAttributeName
                // Test Case
                public override void AddChildren()
                {
                    var tcGetAttributeName = new CXMLReaderAttrTestFunctionalTests.XNodeReaderTests.TCGetAttributeName();
                    this.AddChild(new TestVariation(tcGetAttributeName.GetAttributeWithName1) { Attribute = new VariationAttribute("GetAttribute(Name) Verify with This[Name]") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcGetAttributeName.GetAttributeWithName2) { Attribute = new VariationAttribute("GetAttribute(Name, null) Verify with This[Name]") });
                    this.AddChild(new TestVariation(tcGetAttributeName.GetAttributeWithName3) { Attribute = new VariationAttribute("GetAttribute(Name) Verify with This[Name,null]") });
                    this.AddChild(new TestVariation(tcGetAttributeName.GetAttributeWithName4) { Attribute = new VariationAttribute("GetAttribute(Name, NamespaceURI) Verify with This[Name, NamespaceURI]") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcGetAttributeName.GetAttributeWithName5) { Attribute = new VariationAttribute("GetAttribute(Name, null) Verify not the same as This[Name, NamespaceURI]") });
                    this.AddChild(new TestVariation(tcGetAttributeName.GetAttributeWithName6) { Attribute = new VariationAttribute("GetAttribute(Name, NamespaceURI) Verify not the same as This[Name, null]") });
                    this.AddChild(new TestVariation(tcGetAttributeName.GetAttributeWithName7) { Attribute = new VariationAttribute("GetAttribute(Name) Verify with MoveToAttribute(Name)") });
                    this.AddChild(new TestVariation(tcGetAttributeName.GetAttributeWithName8) { Attribute = new VariationAttribute("GetAttribute(Name,null) Verify with MoveToAttribute(Name)") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcGetAttributeName.GetAttributeWithName9) { Attribute = new VariationAttribute("GetAttribute(Name) Verify with MoveToAttribute(Name,null)") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcGetAttributeName.GetAttributeWithName10) { Attribute = new VariationAttribute("GetAttribute(Name, NamespaceURI) Verify not the same as MoveToAttribute(Name, null)") });
                    this.AddChild(new TestVariation(tcGetAttributeName.GetAttributeWithName11) { Attribute = new VariationAttribute("GetAttribute(Name, null) Verify not the same as MoveToAttribute(Name, NamespaceURI)") });
                    this.AddChild(new TestVariation(tcGetAttributeName.GetAttributeWithName12) { Attribute = new VariationAttribute("GetAttribute(Name, namespace) Verify not the same as MoveToAttribute(Name, namespace)") });
                    this.AddChild(new TestVariation(tcGetAttributeName.GetAttributeWithName13) { Attribute = new VariationAttribute("GetAttribute(String.Empty)") });
                    this.AddChild(new TestVariation(tcGetAttributeName.GetAttributeWithName14) { Attribute = new VariationAttribute("GetAttribute(String.Empty,String.Empty)") });
                }
            }
            public class TCThisOrdinal : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCThisOrdinal
                // Test Case
                public override void AddChildren()
                {
                    var tcThisOrdinal = new CXMLReaderAttrTestFunctionalTests.XNodeReaderTests.TCThisOrdinal();
                    this.AddChild(new TestVariation(tcThisOrdinal.OrdinalWithGetAttrDoubleQ) { Attribute = new VariationAttribute("This[i] Verify with GetAttribute[i] - Double Quote") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcThisOrdinal.OrdinalWithGetAttrSingleQ) { Attribute = new VariationAttribute("This[i] Verify with GetAttribute[i] - Single Quote") });
                    this.AddChild(new TestVariation(tcThisOrdinal.OrdinalWithMoveAttrSingleQ) { Attribute = new VariationAttribute("This[i] Verify with MoveToAttribute[i] - Single Quote") });
                    this.AddChild(new TestVariation(tcThisOrdinal.OrdinalWithMoveAttrDoubleQ) { Attribute = new VariationAttribute("This[i] Verify with MoveToAttribute[i] - Double Quote") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcThisOrdinal.NegativeOneOrdinal) { Attribute = new VariationAttribute("ThisOrdinal NegativeOneOrdinal") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcThisOrdinal.FieldCountOrdinal) { Attribute = new VariationAttribute("ThisOrdinal FieldCountOrdinal") });
                    this.AddChild(new TestVariation(tcThisOrdinal.OrdinalPlusOne) { Attribute = new VariationAttribute("ThisOrdinal OrdinalPlusOne") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcThisOrdinal.OrdinalMinusOne) { Attribute = new VariationAttribute("ThisOrdinal OrdinalMinusOne") });
                }
            }
            public class TCMoveToAttributeOrdinal : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCMoveToAttributeOrdinal
                // Test Case
                public override void AddChildren()
                {
                    var tcMoveToAttributeOrdinal = new CXMLReaderAttrTestFunctionalTests.XNodeReaderTests.TCMoveToAttributeOrdinal();
                    this.AddChild(new TestVariation(tcMoveToAttributeOrdinal.MoveToAttributeWithGetAttrDoubleQ) { Attribute = new VariationAttribute("MoveToAttribute(i) Verify with This[i] - Double Quote") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcMoveToAttributeOrdinal.MoveToAttributeWithGetAttrSingleQ) { Attribute = new VariationAttribute("MoveToAttribute(i) Verify with This[i] - Single Quote") });
                    this.AddChild(new TestVariation(tcMoveToAttributeOrdinal.MoveToAttributeWithMoveAttrDoubleQ) { Attribute = new VariationAttribute("MoveToAttribute(i) Verify with GetAttribute(i) - Double Quote") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcMoveToAttributeOrdinal.MoveToAttributeWithMoveAttrSingleQ) { Attribute = new VariationAttribute("MoveToAttribute(i) Verify with GetAttribute[i] - Single Quote") });
                    this.AddChild(new TestVariation(tcMoveToAttributeOrdinal.NegativeOneOrdinal) { Attribute = new VariationAttribute("MoveToAttribute(i) NegativeOneOrdinal") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcMoveToAttributeOrdinal.FieldCountOrdinal) { Attribute = new VariationAttribute("MoveToAttribute(i) FieldCountOrdinal") });
                    this.AddChild(new TestVariation(tcMoveToAttributeOrdinal.OrdinalPlusOne) { Attribute = new VariationAttribute("MoveToAttribute(i) OrdinalPlusOne") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcMoveToAttributeOrdinal.OrdinalMinusOne) { Attribute = new VariationAttribute("MoveToAttribute(i) OrdinalMinusOne") });
                }
            }
            public class TCMoveToFirstAttribute : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCMoveToFirstAttribute
                // Test Case
                public override void AddChildren()
                {
                    var tcMoveToFirstAttribute = new CXMLReaderAttrTestFunctionalTests.XNodeReaderTests.TCMoveToFirstAttribute();
                    this.AddChild(new TestVariation(tcMoveToFirstAttribute.MoveToFirstAttribute1) { Attribute = new VariationAttribute("MoveToFirstAttribute() When AttributeCount=0, <EMPTY1/> ") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcMoveToFirstAttribute.MoveToFirstAttribute2) { Attribute = new VariationAttribute("MoveToFirstAttribute() When AttributeCount=0, <NONEMPTY1>ABCDE</NONEMPTY1> ") });
                    this.AddChild(new TestVariation(tcMoveToFirstAttribute.MoveToFirstAttribute3) { Attribute = new VariationAttribute("MoveToFirstAttribute() When iOrdinal=0, with namespace") });
                    this.AddChild(new TestVariation(tcMoveToFirstAttribute.MoveToFirstAttribute4) { Attribute = new VariationAttribute("MoveToFirstAttribute() When iOrdinal=0, without namespace") });
                    this.AddChild(new TestVariation(tcMoveToFirstAttribute.MoveToFirstAttribute5) { Attribute = new VariationAttribute("MoveToFirstAttribute() When iOrdinal=mIddle, with namespace") });
                    this.AddChild(new TestVariation(tcMoveToFirstAttribute.MoveToFirstAttribute6) { Attribute = new VariationAttribute("MoveToFirstAttribute() When iOrdinal=mIddle, without namespace") });
                    this.AddChild(new TestVariation(tcMoveToFirstAttribute.MoveToFirstAttribute7) { Attribute = new VariationAttribute("MoveToFirstAttribute() When iOrdinal=end, with namespace") });
                    this.AddChild(new TestVariation(tcMoveToFirstAttribute.MoveToFirstAttribute8) { Attribute = new VariationAttribute("MoveToFirstAttribute() When iOrdinal=end, without namespace") });
                }
            }
            public class TCMoveToNextAttribute : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCMoveToNextAttribute
                // Test Case
                public override void AddChildren()
                {
                    var tcMoveToNextAttribute = new CXMLReaderAttrTestFunctionalTests.XNodeReaderTests.TCMoveToNextAttribute();
                    this.AddChild(new TestVariation(tcMoveToNextAttribute.MoveToNextAttribute1) { Attribute = new VariationAttribute("MoveToNextAttribute() When AttributeCount=0, <EMPTY1/> ") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcMoveToNextAttribute.MoveToNextAttribute2) { Attribute = new VariationAttribute("MoveToNextAttribute() When AttributeCount=0, <NONEMPTY1>ABCDE</NONEMPTY1> ") });
                    this.AddChild(new TestVariation(tcMoveToNextAttribute.MoveToNextAttribute3) { Attribute = new VariationAttribute("MoveToNextAttribute() When iOrdinal=0, with namespace") });
                    this.AddChild(new TestVariation(tcMoveToNextAttribute.MoveToNextAttribute4) { Attribute = new VariationAttribute("MoveToNextAttribute() When iOrdinal=0, without namespace") });
                    this.AddChild(new TestVariation(tcMoveToNextAttribute.MoveToFirstAttribute5) { Attribute = new VariationAttribute("MoveToFirstAttribute() When iOrdinal=mIddle, with namespace") });
                    this.AddChild(new TestVariation(tcMoveToNextAttribute.MoveToFirstAttribute6) { Attribute = new VariationAttribute("MoveToFirstAttribute() When iOrdinal=mIddle, without namespace") });
                    this.AddChild(new TestVariation(tcMoveToNextAttribute.MoveToFirstAttribute7) { Attribute = new VariationAttribute("MoveToFirstAttribute() When iOrdinal=end, with namespace") });
                    this.AddChild(new TestVariation(tcMoveToNextAttribute.MoveToFirstAttribute8) { Attribute = new VariationAttribute("MoveToFirstAttribute() When iOrdinal=end, without namespace") });
                }
            }
            public class TCAttributeTest : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCAttributeTest
                // Test Case
                public override void AddChildren()
                {
                    var tcAttributeTest = new CXMLReaderAttrTestFunctionalTests.XNodeReaderTests.TCAttributeTest();
                    this.AddChild(new TestVariation(tcAttributeTest.TestAttributeTestNodeType_None) { Attribute = new VariationAttribute("Attribute Test On None") });
                    this.AddChild(new TestVariation(tcAttributeTest.TestAttributeTestNodeType_Element) { Attribute = new VariationAttribute("Attribute Test  On Element") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcAttributeTest.TestAttributeTestNodeType_Text) { Attribute = new VariationAttribute("Attribute Test On Text") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcAttributeTest.TestAttributeTestNodeType_CDATA) { Attribute = new VariationAttribute("Attribute Test On CDATA") });
                    this.AddChild(new TestVariation(tcAttributeTest.TestAttributeTestNodeType_ProcessingInstruction) { Attribute = new VariationAttribute("Attribute Test On ProcessingInstruction") });
                    this.AddChild(new TestVariation(tcAttributeTest.TestAttributeTestNodeType_Comment) { Attribute = new VariationAttribute("AttributeTest On Comment") });
                    this.AddChild(new TestVariation(tcAttributeTest.TestAttributeTestNodeType_DocumentType) { Attribute = new VariationAttribute("AttributeTest On DocumentType") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcAttributeTest.TestAttributeTestNodeType_Whitespace) { Attribute = new VariationAttribute("AttributeTest On Whitespace") });
                    this.AddChild(new TestVariation(tcAttributeTest.TestAttributeTestNodeType_EndElement) { Attribute = new VariationAttribute("AttributeTest On EndElement") });
                    this.AddChild(new TestVariation(tcAttributeTest.TestAttributeTestNodeType_XmlDeclaration) { Attribute = new VariationAttribute("AttributeTest On XmlDeclaration") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcAttributeTest.TestAttributeTestNodeType_EndEntity) { Attribute = new VariationAttribute("AttributeTest On EndEntity") });
                }
            }
            public class TCXmlns : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCXmlns
                // Test Case
                public override void AddChildren()
                {
                    var tcXmlns = new CXMLReaderAttrTestFunctionalTests.XNodeReaderTests.TCXmlns();
                    this.AddChild(new TestVariation(tcXmlns.TXmlns1) { Attribute = new VariationAttribute("Name, LocalName, Prefix and Value with xmlns=ns attribute") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcXmlns.TXmlns2) { Attribute = new VariationAttribute("Name, LocalName, Prefix and Value with xmlns:p=ns attribute") });
                    this.AddChild(new TestVariation(tcXmlns.TXmlns3) { Attribute = new VariationAttribute("LookupNamespace with xmlns=ns attribute") });
                    this.AddChild(new TestVariation(tcXmlns.TXmlns4) { Attribute = new VariationAttribute("MoveToAttribute access on xmlns attribute") });
                    this.AddChild(new TestVariation(tcXmlns.TXmlns5) { Attribute = new VariationAttribute("GetAttribute access on xmlns attribute") });
                    this.AddChild(new TestVariation(tcXmlns.TXmlns6) { Attribute = new VariationAttribute("this[xmlns] attribute access") });
                }
            }
            public class TCXmlnsPrefix : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCXmlnsPrefix
                // Test Case
                public override void AddChildren()
                {
                    var tcXmlnsPrefix = new CXMLReaderAttrTestFunctionalTests.XNodeReaderTests.TCXmlnsPrefix();
                    this.AddChild(new TestVariation(tcXmlnsPrefix.TXmlnsPrefix1) { Attribute = new VariationAttribute("NamespaceURI of xmlns:a attribute") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcXmlnsPrefix.TXmlnsPrefix2) { Attribute = new VariationAttribute("NamespaceURI of element/attribute with xmlns attribute") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcXmlnsPrefix.TXmlnsPrefix3) { Attribute = new VariationAttribute("LookupNamespace with xmlns prefix") });
                    this.AddChild(new TestVariation(tcXmlnsPrefix.TXmlnsPrefix4) { Attribute = new VariationAttribute("Define prefix for 'www.w3.org/2000/xmlns'") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcXmlnsPrefix.TXmlnsPrefix5) { Attribute = new VariationAttribute("Redefine namespace attached to xmlns prefix") });
                }
            }
            public class TCReadState : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCReadState
                // Test Case
                public override void AddChildren()
                {
                    var tcReadState = new CXmlReaderReadEtcFunctionalTests.XNodeReaderTests.TCReadState();
                    this.AddChild(new TestVariation(tcReadState.ReadState1) { Attribute = new VariationAttribute("XmlReader ReadState Initial") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadState.ReadState2) { Attribute = new VariationAttribute("XmlReader ReadState Interactive") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadState.ReadState3) { Attribute = new VariationAttribute("XmlReader ReadState EndOfFile") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadState.ReadState4) { Attribute = new VariationAttribute("XmlReader ReadState Initial") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadState.ReadState5) { Attribute = new VariationAttribute("XmlReader ReadState EndOfFile") { Priority = 0 } });
                }
            }
            public class TCReadInnerXml : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCReadInnerXml
                // Test Case
                public override void AddChildren()
                {
                    var tcReadInnerXml = new CXmlReaderReadEtcFunctionalTests.XNodeReaderTests.TCReadInnerXml();
                    this.AddChild(new TestVariation(tcReadInnerXml.TestReadInnerXml1) { Attribute = new VariationAttribute("ReadInnerXml on Empty Tag") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadInnerXml.TestReadInnerXml2) { Attribute = new VariationAttribute("ReadInnerXml on non Empty Tag") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadInnerXml.TestReadInnerXml3) { Attribute = new VariationAttribute("ReadInnerXml on non Empty Tag with text content") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadInnerXml.TestReadInnerXml6) { Attribute = new VariationAttribute("ReadInnerXml with multiple Level of elements") });
                    this.AddChild(new TestVariation(tcReadInnerXml.TestReadInnerXml7) { Attribute = new VariationAttribute("ReadInnerXml with multiple Level of elements, text and attributes") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadInnerXml.TestReadInnerXml8) { Attribute = new VariationAttribute("ReadInnerXml with entity references, EntityHandling = ExpandEntities") });
                    this.AddChild(new TestVariation(tcReadInnerXml.TestReadInnerXml9) { Attribute = new VariationAttribute("ReadInnerXml on attribute node") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadInnerXml.TestReadInnerXml10) { Attribute = new VariationAttribute("ReadInnerXml on attribute node with entity reference in value") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadInnerXml.TestReadInnerXml11) { Attribute = new VariationAttribute("ReadInnerXml on Text") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadInnerXml.TestReadInnerXml12) { Attribute = new VariationAttribute("ReadInnerXml on CDATA") });
                    this.AddChild(new TestVariation(tcReadInnerXml.TestReadInnerXml13) { Attribute = new VariationAttribute("ReadInnerXml on ProcessingInstruction") });
                    this.AddChild(new TestVariation(tcReadInnerXml.TestReadInnerXml14) { Attribute = new VariationAttribute("ReadInnerXml on Comment") });
                    this.AddChild(new TestVariation(tcReadInnerXml.TestReadInnerXml16) { Attribute = new VariationAttribute("ReadInnerXml on EndElement") });
                    this.AddChild(new TestVariation(tcReadInnerXml.TestReadInnerXml17) { Attribute = new VariationAttribute("ReadInnerXml on XmlDeclaration") });
                    this.AddChild(new TestVariation(tcReadInnerXml.TestReadInnerXml18) { Attribute = new VariationAttribute("Current node after ReadInnerXml on element") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadInnerXml.TestReadInnerXml19) { Attribute = new VariationAttribute("Current node after ReadInnerXml on element") });
                    this.AddChild(new TestVariation(tcReadInnerXml.TestTextReadInnerXml2) { Attribute = new VariationAttribute("ReadInnerXml with entity references, EntityHandling = ExpandCharEntites") });
                    this.AddChild(new TestVariation(tcReadInnerXml.TestTextReadInnerXml4) { Attribute = new VariationAttribute("ReadInnerXml on EntityReference") });
                    this.AddChild(new TestVariation(tcReadInnerXml.TestTextReadInnerXml5) { Attribute = new VariationAttribute("ReadInnerXml on EndEntity") });
                    this.AddChild(new TestVariation(tcReadInnerXml.TestTextReadInnerXml18) { Attribute = new VariationAttribute("One large element") });
                }
            }
            public class TCMoveToContent : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCMoveToContent
                // Test Case
                public override void AddChildren()
                {
                    var tcMoveToContent = new CXmlReaderReadEtcFunctionalTests.XNodeReaderTests.TCMoveToContent();
                    this.AddChild(new TestVariation(tcMoveToContent.TestMoveToContent1) { Attribute = new VariationAttribute("MoveToContent on Skip XmlDeclaration") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcMoveToContent.TestMoveToContent2) { Attribute = new VariationAttribute("MoveToContent on Read through All valid Content Node(Element, Text, CDATA, and EndElement)") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcMoveToContent.TestMoveToContent3) { Attribute = new VariationAttribute("MoveToContent on Read through All invalid Content Node(PI, Comment and whitespace)") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcMoveToContent.TestMoveToContent4) { Attribute = new VariationAttribute("MoveToContent on Read through Mix valid and Invalid Content Node") });
                    this.AddChild(new TestVariation(tcMoveToContent.TestMoveToContent5) { Attribute = new VariationAttribute("MoveToContent on Attribute") { Priority = 0 } });
                }
            }
            public class TCIsStartElement : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCIsStartElement
                // Test Case
                public override void AddChildren()
                {
                    var tcIsStartElement = new CXmlReaderReadEtcFunctionalTests.XNodeReaderTests.TCIsStartElement();
                    this.AddChild(new TestVariation(tcIsStartElement.TestIsStartElement1) { Attribute = new VariationAttribute("IsStartElement on Regular Element, no namespace") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcIsStartElement.TestIsStartElement2) { Attribute = new VariationAttribute("IsStartElement on Empty Element, no namespace") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcIsStartElement.TestIsStartElement3) { Attribute = new VariationAttribute("IsStartElement on regular Element, with namespace") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcIsStartElement.TestIsStartElement4) { Attribute = new VariationAttribute("IsStartElement on Empty Tag, with default namespace") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcIsStartElement.TestIsStartElement5) { Attribute = new VariationAttribute("IsStartElement with Name=String.Empty") });
                    this.AddChild(new TestVariation(tcIsStartElement.TestIsStartElement6) { Attribute = new VariationAttribute("IsStartElement on Empty Element with Name and Namespace=String.Empty") });
                    this.AddChild(new TestVariation(tcIsStartElement.TestIsStartElement7) { Attribute = new VariationAttribute("IsStartElement on CDATA") });
                    this.AddChild(new TestVariation(tcIsStartElement.TestIsStartElement8) { Attribute = new VariationAttribute("IsStartElement on EndElement, no namespace") });
                    this.AddChild(new TestVariation(tcIsStartElement.TestIsStartElement9) { Attribute = new VariationAttribute("IsStartElement on EndElement, with namespace") });
                    this.AddChild(new TestVariation(tcIsStartElement.TestIsStartElement10) { Attribute = new VariationAttribute("IsStartElement on Attribute") });
                    this.AddChild(new TestVariation(tcIsStartElement.TestIsStartElement11) { Attribute = new VariationAttribute("IsStartElement on Text") });
                    this.AddChild(new TestVariation(tcIsStartElement.TestIsStartElement12) { Attribute = new VariationAttribute("IsStartElement on ProcessingInstruction") });
                    this.AddChild(new TestVariation(tcIsStartElement.TestIsStartElement13) { Attribute = new VariationAttribute("IsStartElement on Comment") });
                }
            }
            public class TCReadStartElement : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCReadStartElement
                // Test Case
                public override void AddChildren()
                {
                    var tcReadStartElement = new CXmlReaderReadEtcFunctionalTests.XNodeReaderTests.TCReadStartElement();
                    this.AddChild(new TestVariation(tcReadStartElement.TestReadStartElement1) { Attribute = new VariationAttribute("ReadStartElement on Regular Element, no namespace") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadStartElement.TestReadStartElement2) { Attribute = new VariationAttribute("ReadStartElement on Empty Element, no namespace") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadStartElement.TestReadStartElement3) { Attribute = new VariationAttribute("ReadStartElement on regular Element, with namespace") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadStartElement.TestReadStartElement4) { Attribute = new VariationAttribute("Passing ns=String.EmptyErrorCase: ReadStartElement on regular Element, with namespace") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadStartElement.TestReadStartElement5) { Attribute = new VariationAttribute("Passing no ns: ReadStartElement on regular Element, with namespace") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadStartElement.TestReadStartElement6) { Attribute = new VariationAttribute("ReadStartElement on Empty Tag, with namespace") });
                    this.AddChild(new TestVariation(tcReadStartElement.TestReadStartElement7) { Attribute = new VariationAttribute("ErrorCase: ReadStartElement on Empty Tag, with namespace, passing ns=String.Empty") });
                    this.AddChild(new TestVariation(tcReadStartElement.TestReadStartElement8) { Attribute = new VariationAttribute("ReadStartElement on Empty Tag, with namespace, passing no ns") });
                    this.AddChild(new TestVariation(tcReadStartElement.TestReadStartElement9) { Attribute = new VariationAttribute("ReadStartElement with Name=String.Empty") });
                    this.AddChild(new TestVariation(tcReadStartElement.TestReadStartElement10) { Attribute = new VariationAttribute("ReadStartElement on Empty Element with Name and Namespace=String.Empty") });
                    this.AddChild(new TestVariation(tcReadStartElement.TestReadStartElement11) { Attribute = new VariationAttribute("ReadStartElement on CDATA") });
                    this.AddChild(new TestVariation(tcReadStartElement.TestReadStartElement12) { Attribute = new VariationAttribute("ReadStartElement() on EndElement, no namespace") });
                    this.AddChild(new TestVariation(tcReadStartElement.TestReadStartElement13) { Attribute = new VariationAttribute("ReadStartElement(n) on EndElement, no namespace") });
                    this.AddChild(new TestVariation(tcReadStartElement.TestReadStartElement14) { Attribute = new VariationAttribute("ReadStartElement(n, String.Empty) on EndElement, no namespace") });
                    this.AddChild(new TestVariation(tcReadStartElement.TestReadStartElement15) { Attribute = new VariationAttribute("ReadStartElement() on EndElement, with namespace") });
                    this.AddChild(new TestVariation(tcReadStartElement.TestReadStartElement16) { Attribute = new VariationAttribute("ReadStartElement(n,ns) on EndElement, with namespace") });
                }
            }
            public class TCReadEndElement : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCReadEndElement
                // Test Case
                public override void AddChildren()
                {
                    var tcReadEndElement = new CXmlReaderReadEtcFunctionalTests.XNodeReaderTests.TCReadEndElement();
                    this.AddChild(new TestVariation(tcReadEndElement.TestReadEndElement3) { Attribute = new VariationAttribute("ReadEndElement on Start Element, no namespace") });
                    this.AddChild(new TestVariation(tcReadEndElement.TestReadEndElement4) { Attribute = new VariationAttribute("ReadEndElement on Empty Element, no namespace") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadEndElement.TestReadEndElement5) { Attribute = new VariationAttribute("ReadEndElement on regular Element, with namespace") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadEndElement.TestReadEndElement6) { Attribute = new VariationAttribute("ReadEndElement on Empty Tag, with namespace") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadEndElement.TestReadEndElement7) { Attribute = new VariationAttribute("ReadEndElement on CDATA") });
                    this.AddChild(new TestVariation(tcReadEndElement.TestReadEndElement9) { Attribute = new VariationAttribute("ReadEndElement on Text") });
                    this.AddChild(new TestVariation(tcReadEndElement.TestReadEndElement10) { Attribute = new VariationAttribute("ReadEndElement on ProcessingInstruction") });
                    this.AddChild(new TestVariation(tcReadEndElement.TestReadEndElement11) { Attribute = new VariationAttribute("ReadEndElement on Comment") });
                    this.AddChild(new TestVariation(tcReadEndElement.TestReadEndElement13) { Attribute = new VariationAttribute("ReadEndElement on XmlDeclaration") });
                    this.AddChild(new TestVariation(tcReadEndElement.TestTextReadEndElement1) { Attribute = new VariationAttribute("ReadEndElement on EntityReference") });
                    this.AddChild(new TestVariation(tcReadEndElement.TestTextReadEndElement2) { Attribute = new VariationAttribute("ReadEndElement on EndEntity") });
                }
            }

            public class TCMoveToElement : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCMoveToElement
                // Test Case
                public override void AddChildren()
                {
                    var tcMoveToElement = new CXmlReaderReadEtcFunctionalTests.XNodeReaderTests.TCMoveToElement();
                    this.AddChild(new TestVariation(tcMoveToElement.v1) { Attribute = new VariationAttribute("Attribute node") });
                    this.AddChild(new TestVariation(tcMoveToElement.v2) { Attribute = new VariationAttribute("Element node") });
                    this.AddChild(new TestVariation(tcMoveToElement.v5) { Attribute = new VariationAttribute("Comment node") });
                }
            }
            public class ErrorConditions : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+ErrorConditions
                // Test Case
                public override void AddChildren()
                {
                    var errorConditions = new XNodeReaderErrorConditionsFunctionalTests.XNodeReaderTests.ErrorConditions();
                    this.AddChild(new TestVariation(errorConditions.Variation1) { Attribute = new VariationAttribute("Move to Attribute using []") });
                    this.AddChild(new TestVariation(errorConditions.Variation2) { Attribute = new VariationAttribute("GetAttribute") });
                    this.AddChild(new TestVariation(errorConditions.Variation3) { Attribute = new VariationAttribute("IsStartElement") });
                    this.AddChild(new TestVariation(errorConditions.Variation4) { Attribute = new VariationAttribute("LookupNamespace") });
                    this.AddChild(new TestVariation(errorConditions.Variation5) { Attribute = new VariationAttribute("MoveToAttribute") });
                    this.AddChild(new TestVariation(errorConditions.Variation6) { Attribute = new VariationAttribute("Other APIs") });
                    this.AddChild(new TestVariation(errorConditions.Variation7) { Attribute = new VariationAttribute("ReadContentAs(null, null)") });
                    this.AddChild(new TestVariation(errorConditions.Variation8) { Attribute = new VariationAttribute("ReadContentAsBase64") });
                    this.AddChild(new TestVariation(errorConditions.Variation9) { Attribute = new VariationAttribute("ReadContentAsBinHex") });
                    this.AddChild(new TestVariation(errorConditions.Variation10) { Attribute = new VariationAttribute("ReadContentAsBoolean") });
                    this.AddChild(new TestVariation(errorConditions.Variation11b) { Attribute = new VariationAttribute("ReadContentAsDateTimeOffset") });
                    this.AddChild(new TestVariation(errorConditions.Variation12) { Attribute = new VariationAttribute("ReadContentAsDecimal") });
                    this.AddChild(new TestVariation(errorConditions.Variation13) { Attribute = new VariationAttribute("ReadContentAsDouble") });
                    this.AddChild(new TestVariation(errorConditions.Variation14) { Attribute = new VariationAttribute("ReadContentAsFloat") });
                    this.AddChild(new TestVariation(errorConditions.Variation15) { Attribute = new VariationAttribute("ReadContentAsInt") });
                    this.AddChild(new TestVariation(errorConditions.Variation16) { Attribute = new VariationAttribute("ReadContentAsLong") });
                    this.AddChild(new TestVariation(errorConditions.Variation17) { Attribute = new VariationAttribute("ReadElementContentAs(null, null)") });
                    this.AddChild(new TestVariation(errorConditions.Variation18) { Attribute = new VariationAttribute("ReadElementContentAsBase64") });
                    this.AddChild(new TestVariation(errorConditions.Variation19) { Attribute = new VariationAttribute("ReadElementContentAsBinHex") });
                    this.AddChild(new TestVariation(errorConditions.Variation20) { Attribute = new VariationAttribute("ReadElementContentAsBoolean") });
                    this.AddChild(new TestVariation(errorConditions.Variation22) { Attribute = new VariationAttribute("ReadElementContentAsDecimal") });
                    this.AddChild(new TestVariation(errorConditions.Variation23) { Attribute = new VariationAttribute("ReadElementContentAsDouble") });
                    this.AddChild(new TestVariation(errorConditions.Variation24) { Attribute = new VariationAttribute("ReadElementContentAsFloat") });
                    this.AddChild(new TestVariation(errorConditions.Variation25) { Attribute = new VariationAttribute("ReadElementContentAsInt") });
                    this.AddChild(new TestVariation(errorConditions.Variation26) { Attribute = new VariationAttribute("ReadElementContentAsLong") });
                    this.AddChild(new TestVariation(errorConditions.Variation27) { Attribute = new VariationAttribute("ReadElementContentAsObject") });
                    this.AddChild(new TestVariation(errorConditions.Variation28) { Attribute = new VariationAttribute("ReadElementContentAsString") });
                    this.AddChild(new TestVariation(errorConditions.Variation30) { Attribute = new VariationAttribute("ReadStartElement") });
                    this.AddChild(new TestVariation(errorConditions.Variation31) { Attribute = new VariationAttribute("ReadToDescendant(null)") });
                    this.AddChild(new TestVariation(errorConditions.Variation32) { Attribute = new VariationAttribute("ReadToDescendant(String.Empty)") });
                    this.AddChild(new TestVariation(errorConditions.Variation33) { Attribute = new VariationAttribute("ReadToFollowing(null)") });
                    this.AddChild(new TestVariation(errorConditions.Variation34) { Attribute = new VariationAttribute("ReadToFollowing(String.Empty)") });
                    this.AddChild(new TestVariation(errorConditions.Variation35) { Attribute = new VariationAttribute("ReadToNextSibling(null)") });
                    this.AddChild(new TestVariation(errorConditions.Variation36) { Attribute = new VariationAttribute("ReadToNextSibling(String.Empty)") });
                    this.AddChild(new TestVariation(errorConditions.Variation37) { Attribute = new VariationAttribute("ReadValueChunk") });
                    this.AddChild(new TestVariation(errorConditions.Variation38) { Attribute = new VariationAttribute("ReadElementContentAsObject") });
                    this.AddChild(new TestVariation(errorConditions.Variation39) { Attribute = new VariationAttribute("ReadElementContentAsString") });
                }
            }
            public class TCXMLIntegrityBase : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCXMLIntegrityBase
                // Test Case
                public override void AddChildren()
                {
                    var tcXMLIntegrityBase = new IntegrityTestFunctionalTests.XNodeReaderTests.TCXMLIntegrityBase();
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.GetXmlReaderNodeType) { Attribute = new VariationAttribute("NodeType") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.GetXmlReaderName) { Attribute = new VariationAttribute("Name") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.GetXmlReaderLocalName) { Attribute = new VariationAttribute("LocalName") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.Namespace) { Attribute = new VariationAttribute("NamespaceURI") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.Prefix) { Attribute = new VariationAttribute("Prefix") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.HasValue) { Attribute = new VariationAttribute("HasValue") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.GetXmlReaderValue) { Attribute = new VariationAttribute("Value") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.GetDepth) { Attribute = new VariationAttribute("Depth") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.GetBaseURI) { Attribute = new VariationAttribute("BaseURI") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.IsEmptyElement) { Attribute = new VariationAttribute("IsEmptyElement") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.IsDefault) { Attribute = new VariationAttribute("IsDefault") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.GetXmlSpace) { Attribute = new VariationAttribute("XmlSpace") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.GetXmlLang) { Attribute = new VariationAttribute("XmlLang") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.AttributeCount) { Attribute = new VariationAttribute("AttributeCount") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.HasAttribute) { Attribute = new VariationAttribute("HasAttributes") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.GetAttributeName) { Attribute = new VariationAttribute("GetAttributes(name)") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.GetAttributeEmptyName) { Attribute = new VariationAttribute("GetAttribute(String.Empty)") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.GetAttributeNameNamespace) { Attribute = new VariationAttribute("GetAttribute(name,ns)") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.GetAttributeEmptyNameNamespace) { Attribute = new VariationAttribute("GetAttribute(String.Empty, String.Empty)") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.GetAttributeOrdinal) { Attribute = new VariationAttribute("GetAttribute(i)") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.HelperThisOrdinal) { Attribute = new VariationAttribute("this[i]") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.HelperThisName) { Attribute = new VariationAttribute("this[name]") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.HelperThisNameNamespace) { Attribute = new VariationAttribute("this[name,namespace]") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.MoveToAttributeName) { Attribute = new VariationAttribute("MoveToAttribute(name)") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.MoveToAttributeNameNamespace) { Attribute = new VariationAttribute("MoveToAttributeNameNamespace(name,ns)") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.MoveToAttributeOrdinal) { Attribute = new VariationAttribute("MoveToAttribute(i)") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.MoveToFirstAttribute) { Attribute = new VariationAttribute("MoveToFirstAttribute()") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.MoveToNextAttribute) { Attribute = new VariationAttribute("MoveToNextAttribute()") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.MoveToElement) { Attribute = new VariationAttribute("MoveToElement()") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.ReadTestAfterClose) { Attribute = new VariationAttribute("Read") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.GetEOF) { Attribute = new VariationAttribute("GetEOF") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.GetReadState) { Attribute = new VariationAttribute("GetReadState") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.XMLSkip) { Attribute = new VariationAttribute("Skip") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.TestNameTable) { Attribute = new VariationAttribute("NameTable") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.ReadInnerXmlTestAfterClose) { Attribute = new VariationAttribute("ReadInnerXml") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.TestReadOuterXml) { Attribute = new VariationAttribute("ReadOuterXml") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.TestMoveToContent) { Attribute = new VariationAttribute("MoveToContent") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.TestIsStartElement) { Attribute = new VariationAttribute("IsStartElement") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.TestIsStartElementName) { Attribute = new VariationAttribute("IsStartElement(name)") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.TestIsStartElementName2) { Attribute = new VariationAttribute("IsStartElement(String.Empty)") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.TestIsStartElementNameNs) { Attribute = new VariationAttribute("IsStartElement(name, ns)") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.TestIsStartElementNameNs2) { Attribute = new VariationAttribute("IsStartElement(String.Empty,String.Empty)") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.TestReadStartElement) { Attribute = new VariationAttribute("ReadStartElement") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.TestReadStartElementName) { Attribute = new VariationAttribute("ReadStartElement(name)") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.TestReadStartElementName2) { Attribute = new VariationAttribute("ReadStartElement(String.Empty)") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.TestReadStartElementNameNs) { Attribute = new VariationAttribute("ReadStartElement(name, ns)") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.TestReadStartElementNameNs2) { Attribute = new VariationAttribute("ReadStartElement(String.Empty,String.Empty)") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.TestReadEndElement) { Attribute = new VariationAttribute("ReadEndElement") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.LookupNamespace) { Attribute = new VariationAttribute("LookupNamespace") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.ReadAttributeValue) { Attribute = new VariationAttribute("ReadAttributeValue") });
                    this.AddChild(new TestVariation(tcXMLIntegrityBase.CloseTest) { Attribute = new VariationAttribute("Close") });
                }
            }
            public class TCReadContentAsBase64 : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCReadContentAsBase64
                // Test Case
                public override void AddChildren()
                {
                    var tcReadContentAsBase64 = new ReadBase64FunctionalTests.XNodeReaderTests.TCReadContentAsBase64();
                    this.AddChild(new TestVariation(tcReadContentAsBase64.TestReadBase64_1) { Attribute = new VariationAttribute("ReadBase64 Element with all valid value") });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.TestReadBase64_2) { Attribute = new VariationAttribute("ReadBase64 Element with all valid Num value") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.TestReadBase64_3) { Attribute = new VariationAttribute("ReadBase64 Element with all valid Text value") });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.TestReadBase64_5) { Attribute = new VariationAttribute("ReadBase64 Element with all valid value (from concatenation), Priority=0") });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.TestReadBase64_6) { Attribute = new VariationAttribute("ReadBase64 Element with Long valid value (from concatenation), Priority=0") });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.ReadBase64_7) { Attribute = new VariationAttribute("ReadBase64 with count > buffer size") });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.ReadBase64_8) { Attribute = new VariationAttribute("ReadBase64 with count < 0") });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.ReadBase64_9) { Attribute = new VariationAttribute("ReadBase64 with index > buffer size") });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.ReadBase64_10) { Attribute = new VariationAttribute("ReadBase64 with index < 0") });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.ReadBase64_11) { Attribute = new VariationAttribute("ReadBase64 with index + count exceeds buffer") });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.ReadBase64_12) { Attribute = new VariationAttribute("ReadBase64 index & count =0") });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.TestReadBase64_13) { Attribute = new VariationAttribute("ReadBase64 Element multiple into same buffer (using offset), Priority=0") });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.TestReadBase64_14) { Attribute = new VariationAttribute("ReadBase64 with buffer == null") });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.TestReadBase64_15) { Attribute = new VariationAttribute("ReadBase64 after failure") });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.TestReadBase64_16) { Attribute = new VariationAttribute("Read after ReadBase64") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.TestReadBase64_17) { Attribute = new VariationAttribute("Current node on multiple calls") });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.TestReadBase64_18) { Attribute = new VariationAttribute("No op node types") });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.TestTextReadBase64_23) { Attribute = new VariationAttribute("ReadBase64 with incomplete sequence") });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.TestTextReadBase64_24) { Attribute = new VariationAttribute("ReadBase64 when end tag doesn't exist") });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.TestTextReadBase64_26) { Attribute = new VariationAttribute("ReadBase64 with whitespace in the mIddle") });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.TestTextReadBase64_27) { Attribute = new VariationAttribute("ReadBase64 with = in the mIddle") });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.RunBase64DoesnNotRunIntoOverflow) { Attribute = new VariationAttribute("ReadBase64 runs into an Overflow") { Params = new object[] { "10000000" } } });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.RunBase64DoesnNotRunIntoOverflow) { Attribute = new VariationAttribute("ReadBase64 runs into an Overflow") { Params = new object[] { "1000000" } } });
                    this.AddChild(new TestVariation(tcReadContentAsBase64.RunBase64DoesnNotRunIntoOverflow) { Attribute = new VariationAttribute("ReadBase64 runs into an Overflow") { Params = new object[] { "10000" } } });
                }
            }
            public class TCReadElementContentAsBase64 : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCReadElementContentAsBase64
                // Test Case
                public override void AddChildren()
                {
                    var tcReadElementContentAsBase64 = new ReadBase64FunctionalTests.XNodeReaderTests.TCReadElementContentAsBase64();
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.TestReadBase64_1) { Attribute = new VariationAttribute("ReadBase64 Element with all valid value") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.TestReadBase64_2) { Attribute = new VariationAttribute("ReadBase64 Element with all valid Num value") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.TestReadBase64_3) { Attribute = new VariationAttribute("ReadBase64 Element with all valid Text value") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.TestReadBase64_5) { Attribute = new VariationAttribute("ReadBase64 Element with all valid value (from concatenation), Priority=0") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.TestReadBase64_6) { Attribute = new VariationAttribute("ReadBase64 Element with Long valid value (from concatenation), Priority=0") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.ReadBase64_7) { Attribute = new VariationAttribute("ReadBase64 with count > buffer size") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.ReadBase64_8) { Attribute = new VariationAttribute("ReadBase64 with count < 0") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.ReadBase64_9) { Attribute = new VariationAttribute("ReadBase64 with index > buffer size") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.ReadBase64_10) { Attribute = new VariationAttribute("ReadBase64 with index < 0") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.ReadBase64_11) { Attribute = new VariationAttribute("ReadBase64 with index + count exceeds buffer") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.ReadBase64_12) { Attribute = new VariationAttribute("ReadBase64 index & count =0") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.TestReadBase64_13) { Attribute = new VariationAttribute("ReadBase64 Element multiple into same buffer (using offset), Priority=0") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.TestReadBase64_14) { Attribute = new VariationAttribute("ReadBase64 with buffer == null") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.TestReadBase64_15) { Attribute = new VariationAttribute("ReadBase64 after failure") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.TestReadBase64_16) { Attribute = new VariationAttribute("Read after ReadBase64") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.TestReadBase64_17) { Attribute = new VariationAttribute("Current node on multiple calls") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.TestTextReadBase64_23) { Attribute = new VariationAttribute("ReadBase64 with incomplete sequence") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.TestTextReadBase64_24) { Attribute = new VariationAttribute("ReadBase64 when end tag doesn't exist") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.TestTextReadBase64_26) { Attribute = new VariationAttribute("ReadBase64 with whitespace in the mIddle") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.TestTextReadBase64_27) { Attribute = new VariationAttribute("ReadBase64 with = in the mIddle") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.ReadBase64DoesNotRunIntoOverflow2) { Attribute = new VariationAttribute("105376: ReadBase64 runs into an Overflow") { Params = new object[] { "10000000" } } });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.ReadBase64DoesNotRunIntoOverflow2) { Attribute = new VariationAttribute("105376: ReadBase64 runs into an Overflow") { Params = new object[] { "1000000" } } });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.ReadBase64DoesNotRunIntoOverflow2) { Attribute = new VariationAttribute("105376: ReadBase64 runs into an Overflow") { Params = new object[] { "10000" } } });
                    this.AddChild(new TestVariation(tcReadElementContentAsBase64.SubtreeReaderInsertedAttributesWontWorkWithReadContentAsBase64) { Attribute = new VariationAttribute("430329: SubtreeReader inserted attributes don't work with ReadContentAsBase64") });
                }
            }
            public class TCReadContentAsBinHex : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCReadContentAsBinHex
                // Test Case
                public override void AddChildren()
                {
                    var tcReadContentAsBinHex = new ReadBinHexFunctionalTests.XNodeReaderTests.TCReadContentAsBinHex();
                    this.AddChild(new TestVariation(tcReadContentAsBinHex.TestReadBinHex_1) { Attribute = new VariationAttribute("ReadBinHex Element with all valid value") });
                    this.AddChild(new TestVariation(tcReadContentAsBinHex.TestReadBinHex_2) { Attribute = new VariationAttribute("ReadBinHex Element with all valid Num value") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadContentAsBinHex.TestReadBinHex_3) { Attribute = new VariationAttribute("ReadBinHex Element with all valid Text value") });
                    this.AddChild(new TestVariation(tcReadContentAsBinHex.TestReadBinHex_4) { Attribute = new VariationAttribute("ReadBinHex Element on CDATA") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadContentAsBinHex.TestReadBinHex_5) { Attribute = new VariationAttribute("ReadBinHex Element with all valid value (from concatenation), Priority=0") });
                    this.AddChild(new TestVariation(tcReadContentAsBinHex.TestReadBinHex_6) { Attribute = new VariationAttribute("ReadBinHex Element with all long valid value (from concatenation)") });
                    this.AddChild(new TestVariation(tcReadContentAsBinHex.TestReadBinHex_7) { Attribute = new VariationAttribute("ReadBinHex with count > buffer size") });
                    this.AddChild(new TestVariation(tcReadContentAsBinHex.TestReadBinHex_8) { Attribute = new VariationAttribute("ReadBinHex with count < 0") });
                    this.AddChild(new TestVariation(tcReadContentAsBinHex.vReadBinHex_9) { Attribute = new VariationAttribute("ReadBinHex with index > buffer size") });
                    this.AddChild(new TestVariation(tcReadContentAsBinHex.TestReadBinHex_10) { Attribute = new VariationAttribute("ReadBinHex with index < 0") });
                    this.AddChild(new TestVariation(tcReadContentAsBinHex.TestReadBinHex_11) { Attribute = new VariationAttribute("ReadBinHex with index + count exceeds buffer") });
                    this.AddChild(new TestVariation(tcReadContentAsBinHex.TestReadBinHex_12) { Attribute = new VariationAttribute("ReadBinHex index & count =0") });
                    this.AddChild(new TestVariation(tcReadContentAsBinHex.TestReadBinHex_13) { Attribute = new VariationAttribute("ReadBinHex Element multiple into same buffer (using offset), Priority=0") });
                    this.AddChild(new TestVariation(tcReadContentAsBinHex.TestReadBinHex_14) { Attribute = new VariationAttribute("ReadBinHex with buffer == null") });
                    this.AddChild(new TestVariation(tcReadContentAsBinHex.TestReadBinHex_15) { Attribute = new VariationAttribute("ReadBinHex after failed ReadBinHex") });
                    this.AddChild(new TestVariation(tcReadContentAsBinHex.TestReadBinHex_16) { Attribute = new VariationAttribute("Read after ReadBinHex") });
                    this.AddChild(new TestVariation(tcReadContentAsBinHex.TestReadBinHex_17) { Attribute = new VariationAttribute("Current node on multiple calls") });
                    this.AddChild(new TestVariation(tcReadContentAsBinHex.TestTextReadBinHex_21) { Attribute = new VariationAttribute("ReadBinHex with whitespace") });
                    this.AddChild(new TestVariation(tcReadContentAsBinHex.TestTextReadBinHex_22) { Attribute = new VariationAttribute("ReadBinHex with odd number of chars") });
                    this.AddChild(new TestVariation(tcReadContentAsBinHex.TestTextReadBinHex_23) { Attribute = new VariationAttribute("ReadBinHex when end tag doesn't exist") });
                    this.AddChild(new TestVariation(tcReadContentAsBinHex.TestTextReadBinHex_24) { Attribute = new VariationAttribute("WS:WireCompat:hex binary fails to send/return data after 1787 bytes going Whidbey to Everett") });
                    this.AddChild(new TestVariation(tcReadContentAsBinHex.DebugAssertInReadContentAsBinHex) { Attribute = new VariationAttribute("DebugAssert in ReadContentAsBinHex") });
                }
            }
            public class TCReadElementContentAsBinHex : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCReadElementContentAsBinHex
                // Test Case
                public override void AddChildren()
                {
                    var tcReadElementContentAsBinHex = new ReadBinHexFunctionalTests.XNodeReaderTests.TCReadElementContentAsBinHex();
                    this.AddChild(new TestVariation(tcReadElementContentAsBinHex.TestReadBinHex_1) { Attribute = new VariationAttribute("ReadBinHex Element with all valid value") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBinHex.TestReadBinHex_2) { Attribute = new VariationAttribute("ReadBinHex Element with all valid Num value") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadElementContentAsBinHex.TestReadBinHex_3) { Attribute = new VariationAttribute("ReadBinHex Element with all valid Text value") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBinHex.TestReadBinHex_4) { Attribute = new VariationAttribute("ReadBinHex Element with Comments and PIs") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadElementContentAsBinHex.TestReadBinHex_5) { Attribute = new VariationAttribute("ReadBinHex Element with all valid value (from concatenation), Priority=0") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBinHex.TestReadBinHex_6) { Attribute = new VariationAttribute("ReadBinHex Element with all long valid value (from concatenation)") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBinHex.TestReadBinHex_7) { Attribute = new VariationAttribute("ReadBinHex with count > buffer size") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBinHex.TestReadBinHex_8) { Attribute = new VariationAttribute("ReadBinHex with count < 0") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBinHex.vReadBinHex_9) { Attribute = new VariationAttribute("ReadBinHex with index > buffer size") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBinHex.TestReadBinHex_10) { Attribute = new VariationAttribute("ReadBinHex with index < 0") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBinHex.TestReadBinHex_11) { Attribute = new VariationAttribute("ReadBinHex with index + count exceeds buffer") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBinHex.TestReadBinHex_12) { Attribute = new VariationAttribute("ReadBinHex index & count =0") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBinHex.TestReadBinHex_13) { Attribute = new VariationAttribute("ReadBinHex Element multiple into same buffer (using offset), Priority=0") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBinHex.TestReadBinHex_14) { Attribute = new VariationAttribute("ReadBinHex with buffer == null") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBinHex.TestReadBinHex_15) { Attribute = new VariationAttribute("ReadBinHex after failed ReadBinHex") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBinHex.TestReadBinHex_16) { Attribute = new VariationAttribute("Read after ReadBinHex") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBinHex.TestTextReadBinHex_21) { Attribute = new VariationAttribute("ReadBinHex with whitespace") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBinHex.TestTextReadBinHex_22) { Attribute = new VariationAttribute("ReadBinHex with odd number of chars") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBinHex.TestTextReadBinHex_23) { Attribute = new VariationAttribute("ReadBinHex when end tag doesn't exist") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBinHex.TestTextReadBinHex_24) { Attribute = new VariationAttribute("WS:WireCompat:hex binary fails to send/return data after 1787 bytes going Whidbey to Everett") });
                    this.AddChild(new TestVariation(tcReadElementContentAsBinHex.TestTextReadBinHex_25) { Attribute = new VariationAttribute("SubtreeReader inserted attributes don't work with ReadContentAsBinHex") });
                }
            }
            public class CReaderTestModule : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+CReaderTestModule
                // Test Case
                public override void AddChildren()
                {
                    var cReaderTestModule = new ReaderPropertyFunctionalTests.XNodeReaderTests.CReaderTestModule();
                    this.AddChild(new TestVariation(cReaderTestModule.v1) { Attribute = new VariationAttribute("Reader Property empty doc") { Priority = 0 } });
                    this.AddChild(new TestVariation(cReaderTestModule.v2) { Attribute = new VariationAttribute("Reader Property after Read") { Priority = 0 } });
                    this.AddChild(new TestVariation(cReaderTestModule.v3) { Attribute = new VariationAttribute("Reader Property after EOF") { Priority = 0 } });
                    this.AddChild(new TestVariation(cReaderTestModule.v4) { Attribute = new VariationAttribute("Default Reader Settings") { Priority = 0 } });
                }
            }
            public class TCReadOuterXml : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCReadOuterXml
                // Test Case
                public override void AddChildren()
                {
                    var tcReadOuterXml = new ReadOuterXmlFunctionalTests.XNodeReaderTests.TCReadOuterXml();
                    this.AddChild(new TestVariation(tcReadOuterXml.ReadOuterXml1) { Attribute = new VariationAttribute("ReadOuterXml on empty element w/o attributes") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadOuterXml.ReadOuterXml2) { Attribute = new VariationAttribute("ReadOuterXml on empty element w/ attributes") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadOuterXml.ReadOuterXml3) { Attribute = new VariationAttribute("ReadOuterXml on full empty element w/o attributes") });
                    this.AddChild(new TestVariation(tcReadOuterXml.ReadOuterXml4) { Attribute = new VariationAttribute("ReadOuterXml on full empty element w/ attributes") });
                    this.AddChild(new TestVariation(tcReadOuterXml.ReadOuterXml5) { Attribute = new VariationAttribute("ReadOuterXml on element with text content") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadOuterXml.ReadOuterXml6) { Attribute = new VariationAttribute("ReadOuterXml on element with attributes") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadOuterXml.ReadOuterXml7) { Attribute = new VariationAttribute("ReadOuterXml on element with text and markup content") });
                    this.AddChild(new TestVariation(tcReadOuterXml.ReadOuterXml8) { Attribute = new VariationAttribute("ReadOuterXml with multiple level of elements") });
                    this.AddChild(new TestVariation(tcReadOuterXml.ReadOuterXml9) { Attribute = new VariationAttribute("ReadOuterXml with multiple level of elements, text and attributes") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadOuterXml.ReadOuterXml10) { Attribute = new VariationAttribute("ReadOuterXml on element with complex content (CDATA, PIs, Comments)") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadOuterXml.ReadOuterXml12) { Attribute = new VariationAttribute("ReadOuterXml on attribute node of empty element") });
                    this.AddChild(new TestVariation(tcReadOuterXml.ReadOuterXml13) { Attribute = new VariationAttribute("ReadOuterXml on attribute node of full empty element") });
                    this.AddChild(new TestVariation(tcReadOuterXml.ReadOuterXml14) { Attribute = new VariationAttribute("ReadOuterXml on attribute node") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadOuterXml.ReadOuterXml15) { Attribute = new VariationAttribute("ReadOuterXml on attribute with entities, EntityHandling = ExpandEntities") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadOuterXml.ReadOuterXml17) { Attribute = new VariationAttribute("ReadOuterXml on ProcessingInstruction") });
                    this.AddChild(new TestVariation(tcReadOuterXml.ReadOuterXml24) { Attribute = new VariationAttribute("ReadOuterXml on CDATA") });
                    this.AddChild(new TestVariation(tcReadOuterXml.TRReadOuterXml27) { Attribute = new VariationAttribute("ReadOuterXml on element with entities, EntityHandling = ExpandCharEntities") });
                    this.AddChild(new TestVariation(tcReadOuterXml.TRReadOuterXml28) { Attribute = new VariationAttribute("ReadOuterXml on attribute with entities, EntityHandling = ExpandCharEntites") });
                    this.AddChild(new TestVariation(tcReadOuterXml.TestTextReadOuterXml29) { Attribute = new VariationAttribute("One large element") });
                    this.AddChild(new TestVariation(tcReadOuterXml.ReadOuterXmlWhenNamespacesEqualsToFalseAndHasAnAttributeXmlns) { Attribute = new VariationAttribute("Read OuterXml when Namespaces=false and has an attribute xmlns") });
                }
            }
            public class TCReadSubtree : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCReadSubtree
                // Test Case
                public override void AddChildren()
                {
                    var tcReadSubtree = new ReadSubTreeFunctionalTests.XNodeReaderTests.TCReadSubtree();
                    this.AddChild(new TestVariation(tcReadSubtree.ReadSubtreeOnlyWorksOnElementNode) { Attribute = new VariationAttribute("ReadSubtree only works on Element Node") });
                    this.AddChild(new TestVariation(tcReadSubtree.v2) { Attribute = new VariationAttribute("ReadSubtree Test depth=1") { Params = new object[] { "elem1", "", "ELEMENT", "elem5", "", "ELEMENT" }, Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadSubtree.v2) { Attribute = new VariationAttribute("ReadSubtree Test depth=2") { Params = new object[] { "elem2", "", "ELEMENT", "elem1", "", "ENDELEMENT" }, Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadSubtree.v2) { Attribute = new VariationAttribute("ReadSubtree Test empty element") { Params = new object[] { "elem5", "", "ELEMENT", "elem6", "", "ELEMENT" }, Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadSubtree.v2) { Attribute = new VariationAttribute("ReadSubtree Test on Root") { Params = new object[] { "root", "", "ELEMENT", "", "", "NONE" }, Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadSubtree.v2) { Attribute = new VariationAttribute("ReadSubtree Test Comment after element") { Params = new object[] { "elem", "", "ELEMENT", "", "Comment", "COMMENT" }, Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadSubtree.v2) { Attribute = new VariationAttribute("ReadSubtree Test depth=4") { Params = new object[] { "elem4", "", "ELEMENT", "x:elem3", "", "ENDELEMENT" }, Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadSubtree.v2) { Attribute = new VariationAttribute("ReadSubtree Test depth=3") { Params = new object[] { "x:elem3", "", "ELEMENT", "elem2", "", "ENDELEMENT" }, Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadSubtree.v2) { Attribute = new VariationAttribute("ReadSubtree Test empty element before root") { Params = new object[] { "elem6", "", "ELEMENT", "root", "", "ENDELEMENT" }, Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadSubtree.v2) { Attribute = new VariationAttribute("ReadSubtree Test PI after element") { Params = new object[] { "elempi", "", "ELEMENT", "pi", "target", "PROCESSINGINSTRUCTION" }, Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadSubtree.v3) { Attribute = new VariationAttribute("Read with entities") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadSubtree.v4) { Attribute = new VariationAttribute("Inner XML on Subtree reader") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadSubtree.v5) { Attribute = new VariationAttribute("Outer XML on Subtree reader") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadSubtree.v6) { Attribute = new VariationAttribute("ReadString on Subtree reader") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadSubtree.v7) { Attribute = new VariationAttribute("Close on inner reader with CloseInput should not close the outer reader") { Params = new object[] { "true" }, Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadSubtree.v7) { Attribute = new VariationAttribute("Close on inner reader with CloseInput should not close the outer reader") { Params = new object[] { "false" }, Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadSubtree.v8) { Attribute = new VariationAttribute("Nested Subtree reader calls") { Priority = 2 } });
                    this.AddChild(new TestVariation(tcReadSubtree.v100) { Attribute = new VariationAttribute("ReadSubtree for element depth more than 4K chars") { Priority = 2 } });
                    this.AddChild(new TestVariation(tcReadSubtree.MultipleNamespacesOnSubtreeReader) { Attribute = new VariationAttribute("Multiple Namespaces on Subtree reader") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadSubtree.SubtreeReaderCachesNodeTypeAndReportsNodeTypeOfAttributeOnSubsequentReads) { Attribute = new VariationAttribute("Subtree Reader caches the NodeType and reports node type of Attribute on subsequent reads.") { Priority = 1 } });
                }
            }
            public class TCReadToDescendant : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCReadToDescendant
                // Test Case
                public override void AddChildren()
                {
                    var tcReadToDescendant = new ReadToDescendantFunctionalTests.XNodeReaderTests.TCReadToDescendant();                    
                    this.AddChild(new TestVariation(tcReadToDescendant.v) { Attribute = new VariationAttribute("Simple positive test") { Params = new object[] { "NNS" }, Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadToDescendant.v) { Attribute = new VariationAttribute("Simple positive test") { Params = new object[] { "DNS" }, Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadToDescendant.v) { Attribute = new VariationAttribute("Simple positive test") { Params = new object[] { "NS" }, Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadToDescendant.v2) { Attribute = new VariationAttribute("Read on a deep tree at least more than 4K boundary") { Priority = 2 } });
                    this.AddChild(new TestVariation(tcReadToDescendant.v3) { Attribute = new VariationAttribute("Read on descendant with same names") { Params = new object[] { "DNS" }, Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToDescendant.v3) { Attribute = new VariationAttribute("Read on descendant with same names") { Params = new object[] { "NNS" }, Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToDescendant.v3) { Attribute = new VariationAttribute("Read on descendant with same names") { Params = new object[] { "NS" }, Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToDescendant.v4) { Attribute = new VariationAttribute("If name not found, stop at end element of the subtree") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToDescendant.v5) { Attribute = new VariationAttribute("Positioning on a level and try to find the name which is on a level higher") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToDescendant.v6) { Attribute = new VariationAttribute("Read to Descendant on one level and again to level below it") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToDescendant.v7) { Attribute = new VariationAttribute("Read to Descendant on one level and again to level below it, with namespace") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToDescendant.v8) { Attribute = new VariationAttribute("Read to Descendant on one level and again to level below it, with prefix") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToDescendant.v9) { Attribute = new VariationAttribute("Multiple Reads to children and then next siblings, NNS") { Priority = 2 } });
                    this.AddChild(new TestVariation(tcReadToDescendant.v10) { Attribute = new VariationAttribute("Multiple Reads to children and then next siblings, DNS") { Priority = 2 } });
                    this.AddChild(new TestVariation(tcReadToDescendant.v11) { Attribute = new VariationAttribute("Multiple Reads to children and then next siblings, NS") { Priority = 2 } });
                    this.AddChild(new TestVariation(tcReadToDescendant.v12) { Attribute = new VariationAttribute("Call from different nodetypes") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToDescendant.v14) { Attribute = new VariationAttribute("Only child has namespaces and read to it") { Priority = 2 } });
                    this.AddChild(new TestVariation(tcReadToDescendant.v15) { Attribute = new VariationAttribute("Pass null to both arguments throws ArgumentException") { Priority = 2 } });
                    this.AddChild(new TestVariation(tcReadToDescendant.v17) { Attribute = new VariationAttribute("Different names, same uri works correctly") { Priority = 2 } });
                    this.AddChild(new TestVariation(tcReadToDescendant.v18) { Attribute = new VariationAttribute("On Root Node") { Params = new object[] { "DNS" }, Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadToDescendant.v18) { Attribute = new VariationAttribute("On Root Node") { Params = new object[] { "NNS" }, Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadToDescendant.v18) { Attribute = new VariationAttribute("On Root Node") { Params = new object[] { "NS" }, Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadToDescendant.v19) { Attribute = new VariationAttribute("427176	Assertion failed when call XmlReader.ReadToDescendant() for non-existing node") { Priority = 1 } });
                }
            }
            public class TCReadToFollowing : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCReadToFollowing
                // Test Case
                public override void AddChildren()
                {
                    var tcReadToFollowing = new ReadToFollowingFunctionalTests.XNodeReaderTests.TCReadToFollowing();
                    this.AddChild(new TestVariation(tcReadToFollowing.v1) { Attribute = new VariationAttribute("Simple positive test") { Params = new object[] { "DNS" }, Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadToFollowing.v1) { Attribute = new VariationAttribute("Simple positive test") { Params = new object[] { "NS" }, Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadToFollowing.v1) { Attribute = new VariationAttribute("Simple positive test") { Params = new object[] { "NNS" }, Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadToFollowing.v2) { Attribute = new VariationAttribute("Read on following with same names") { Params = new object[] { "NNS" }, Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToFollowing.v2) { Attribute = new VariationAttribute("Read on following with same names") { Params = new object[] { "NS" }, Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToFollowing.v2) { Attribute = new VariationAttribute("Read on following with same names") { Params = new object[] { "DNS" }, Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToFollowing.v4) { Attribute = new VariationAttribute("If name not found, go to eof") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToFollowing.v5_1) { Attribute = new VariationAttribute("If localname not found go to eof") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToFollowing.v5_2) { Attribute = new VariationAttribute("If uri not found, go to eof") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToFollowing.v6) { Attribute = new VariationAttribute("Read to Following on one level and again to level below it") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToFollowing.v7) { Attribute = new VariationAttribute("Read to Following on one level and again to level below it, with namespace") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToFollowing.v8) { Attribute = new VariationAttribute("Read to Following on one level and again to level below it, with prefix") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToFollowing.v9) { Attribute = new VariationAttribute("Multiple Reads to children and then next siblings, NNS") { Priority = 2 } });
                    this.AddChild(new TestVariation(tcReadToFollowing.v10) { Attribute = new VariationAttribute("Multiple Reads to children and then next siblings, DNS") { Priority = 2 } });
                    this.AddChild(new TestVariation(tcReadToFollowing.v11) { Attribute = new VariationAttribute("Multiple Reads to children and then next siblings, NS") { Priority = 2 } });
                    this.AddChild(new TestVariation(tcReadToFollowing.v14) { Attribute = new VariationAttribute("Only child has namespaces and read to it") { Priority = 2 } });
                    this.AddChild(new TestVariation(tcReadToFollowing.v15) { Attribute = new VariationAttribute("Pass null to both arguments throws ArgumentException") { Priority = 2 } });
                    this.AddChild(new TestVariation(tcReadToFollowing.v17) { Attribute = new VariationAttribute("Different names, same uri works correctly") { Priority = 2 } });
                }
            }
            public class TCReadToNextSibling : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCReadToNextSibling
                // Test Case
                public override void AddChildren()
                {
                    var tcReadToNextSibling = new ReadToNextSiblingFunctionalTests.XNodeReaderTests.TCReadToNextSibling();
                    this.AddChild(new TestVariation(tcReadToNextSibling.v) { Attribute = new VariationAttribute("Simple positive test 2") { Params = new object[] { "DNS" }, Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadToNextSibling.v) { Attribute = new VariationAttribute("Simple positive test 3") { Params = new object[] { "NS" }, Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadToNextSibling.v) { Attribute = new VariationAttribute("Simple positive test 1") { Params = new object[] { "NNS" }, Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadToNextSibling.v2) { Attribute = new VariationAttribute("Read on a deep tree at least more than 4K boundary") { Priority = 2 } });
                    this.AddChild(new TestVariation(tcReadToNextSibling.v3) { Attribute = new VariationAttribute("Read to next sibling with same names 1") { Params = new object[] { "NNS", "<root><a att='1'/><a att='2'/><a att='3'/></root>" }, Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToNextSibling.v3) { Attribute = new VariationAttribute("Read on next sibling with same names 2") { Params = new object[] { "DNS", "<root xmlns='a'><a att='1'/><a att='2'/><a att='3'/></root>" }, Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToNextSibling.v3) { Attribute = new VariationAttribute("Read on next sibling with same names 3") { Params = new object[] { "NS", "<root xmlns:a='a'><a:a att='1'/><a:a att='2'/><a:a att='3'/></root>" }, Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToNextSibling.v4) { Attribute = new VariationAttribute("If name not found, stop at end element of the subtree") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToNextSibling.v5) { Attribute = new VariationAttribute("Positioning on a level and try to find the name which is on a level higher") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToNextSibling.v6) { Attribute = new VariationAttribute("Read to next sibling on one level and again to level below it") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToNextSibling.v12) { Attribute = new VariationAttribute("Call from different nodetypes") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadToNextSibling.v16) { Attribute = new VariationAttribute("Pass null to both arguments throws ArgumentException") { Priority = 2 } });
                    this.AddChild(new TestVariation(tcReadToNextSibling.v17) { Attribute = new VariationAttribute("Different names, same uri works correctly") { Priority = 2 } });
                }
            }
            public class TCReadValue : BridgeHelpers
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+TCReadValue
                // Test Case
                public override void AddChildren()
                {
                    var tcReadValue = new ReadValueFunctionalTests.XNodeReaderTests.TCReadValue();                    
                    this.AddChild(new TestVariation(tcReadValue.TestReadValuePri0) { Attribute = new VariationAttribute("ReadValue") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadValue.TestReadValuePri0onElement) { Attribute = new VariationAttribute("ReadValue on Element") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadValue.TestReadValueOnAttribute0) { Attribute = new VariationAttribute("ReadValue on Attribute") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadValue.TestReadValueOnAttribute1) { Attribute = new VariationAttribute("ReadValue on Attribute after ReadAttributeValue") { Priority = 2 } });
                    this.AddChild(new TestVariation(tcReadValue.TestReadValue2Pri0) { Attribute = new VariationAttribute("ReadValue on empty buffer") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadValue.TestReadValue3Pri0) { Attribute = new VariationAttribute("ReadValue on negative count") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadValue.TestReadValue4Pri0) { Attribute = new VariationAttribute("ReadValue on negative offset") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadValue.TestReadValue1) { Attribute = new VariationAttribute("ReadValue with buffer = element content / 2") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadValue.TestReadValue2) { Attribute = new VariationAttribute("ReadValue entire value in one call") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadValue.TestReadValue3) { Attribute = new VariationAttribute("ReadValue bit by bit") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadValue.TestReadValue4) { Attribute = new VariationAttribute("ReadValue for value more than 4K") { Priority = 0 } });
                    this.AddChild(new TestVariation(tcReadValue.TestReadValue5) { Attribute = new VariationAttribute("ReadValue for value more than 4K and invalid element") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadValue.TestReadValue6) { Attribute = new VariationAttribute("ReadValue with Entity Reference, EntityHandling = ExpandEntities") });
                    this.AddChild(new TestVariation(tcReadValue.TestReadValue7) { Attribute = new VariationAttribute("ReadValue with count > buffer size") });
                    this.AddChild(new TestVariation(tcReadValue.TestReadValue8) { Attribute = new VariationAttribute("ReadValue with index > buffer size") });
                    this.AddChild(new TestVariation(tcReadValue.TestReadValue10) { Attribute = new VariationAttribute("ReadValue with index + count exceeds buffer") });
                    this.AddChild(new TestVariation(tcReadValue.TestReadChar11) { Attribute = new VariationAttribute("ReadValue with combination Text, CDATA and Whitespace") });
                    this.AddChild(new TestVariation(tcReadValue.TestReadChar12) { Attribute = new VariationAttribute("ReadValue with combination Text, CDATA and SignificantWhitespace") });
                    this.AddChild(new TestVariation(tcReadValue.TestReadChar13) { Attribute = new VariationAttribute("ReadValue with buffer == null") });
                    this.AddChild(new TestVariation(tcReadValue.TestReadChar14) { Attribute = new VariationAttribute("ReadValue with multiple different inner nodes") });
                    this.AddChild(new TestVariation(tcReadValue.TestReadChar15) { Attribute = new VariationAttribute("ReadValue after failed ReadValue") });
                    this.AddChild(new TestVariation(tcReadValue.TestReadChar16) { Attribute = new VariationAttribute("Read after ReadValue") });
                    this.AddChild(new TestVariation(tcReadValue.TestReadChar19) { Attribute = new VariationAttribute("Test error after successful ReadValue") });
                    this.AddChild(new TestVariation(tcReadValue.TestReadChar21) { Attribute = new VariationAttribute("Call on invalid element content after 4k boundary") { Priority = 1 } });
                    this.AddChild(new TestVariation(tcReadValue.TestTextReadValue25) { Attribute = new VariationAttribute("ReadValue with whitespace") });
                    this.AddChild(new TestVariation(tcReadValue.TestTextReadValue26) { Attribute = new VariationAttribute("ReadValue when end tag doesn't exist") });
                    this.AddChild(new TestVariation(tcReadValue.TestCharEntities0) { Attribute = new VariationAttribute("Testing with character entities") });
                    this.AddChild(new TestVariation(tcReadValue.TestCharEntities1) { Attribute = new VariationAttribute("Testing with character entities when value more than 4k") });
                    this.AddChild(new TestVariation(tcReadValue.TestCharEntities2) { Attribute = new VariationAttribute("Testing with character entities with another pattern") });
                    this.AddChild(new TestVariation(tcReadValue.TestReadValueOnBig) { Attribute = new VariationAttribute("Testing a use case pattern with large file") });
                    this.AddChild(new TestVariation(tcReadValue.TestReadValueOnComments0) { Attribute = new VariationAttribute("ReadValue on Comments with IgnoreComments") });
                    this.AddChild(new TestVariation(tcReadValue.TestReadValueOnPIs0) { Attribute = new VariationAttribute("ReadValue on PI with IgnorePI") });
                    this.AddChild(new TestVariation(tcReadValue.bug340158) { Attribute = new VariationAttribute("Skip after ReadAttributeValue/ReadValueChunk") });
                }
            }
            public class XNodeReaderAPI : XLinqTestCase
            {
                // Type is CoreXml.Test.XLinq.FunctionalTests+XNodeReaderTests+XNodeReaderAPI
                // Test Case
                public override void AddChildren()
                {
                    var xNodeReaderAPI = new XNodeReaderAPIFunctionalTests.XNodeReaderTests.XNodeReaderAPI();
                    this.AddChild(new TestVariation(xNodeReaderAPI.OpenOnNodeType) { Attribute = new VariationAttribute("Open on node type: Text") { Params = new object[] { XmlNodeType.Text, 1, new string[] { "some_text" }, 1 }, Priority = 1 } });
                    this.AddChild(new TestVariation(xNodeReaderAPI.OpenOnNodeType) { Attribute = new VariationAttribute("Open on node type: PI (root level)") { Params = new object[] { XmlNodeType.ProcessingInstruction, 0, new string[] { "PI", "" }, 1 }, Priority = 2 } });
                    this.AddChild(new TestVariation(xNodeReaderAPI.OpenOnNodeType) { Attribute = new VariationAttribute("Open on node type: Comment") { Params = new object[] { XmlNodeType.Comment, 1, new string[] { "comm2" }, 1 }, Priority = 2 } });
                    this.AddChild(new TestVariation(xNodeReaderAPI.OpenOnNodeType) { Attribute = new VariationAttribute("Open on node type: Text (root level)") { Params = new object[] { XmlNodeType.Text, 0, new string[] { "\t" }, 1 }, Priority = 0 } });
                    this.AddChild(new TestVariation(xNodeReaderAPI.OpenOnNodeType) { Attribute = new VariationAttribute("Open on node type: XElement (root)") { Params = new object[] { XmlNodeType.Element, 0, new string[] { "A", "", "" }, 15 }, Priority = 1 } });
                    this.AddChild(new TestVariation(xNodeReaderAPI.OpenOnNodeType) { Attribute = new VariationAttribute("Open on node type: PI") { Params = new object[] { XmlNodeType.ProcessingInstruction, 1, new string[] { "PIX", "click" }, 1 }, Priority = 2 } });
                    this.AddChild(new TestVariation(xNodeReaderAPI.OpenOnNodeType) { Attribute = new VariationAttribute("Open on node type: Comment (root level)") { Params = new object[] { XmlNodeType.Comment, 0, new string[] { "comment1" }, 1 }, Priority = 2 } });
                    this.AddChild(new TestVariation(xNodeReaderAPI.OpenOnNodeType) { Attribute = new VariationAttribute("Open on node type: XElement (in the mIddle)") { Params = new object[] { XmlNodeType.Element, 1, new string[] { "B", "", "x" }, 11 }, Priority = 0 } });
                    this.AddChild(new TestVariation(xNodeReaderAPI.OpenOnNodeType) { Attribute = new VariationAttribute("Open on node type: XElement (leaf I.)") { Params = new object[] { XmlNodeType.Element, 3, new string[] { "D", "", "y" }, 2 }, Priority = 0 } });
                    this.AddChild(new TestVariation(xNodeReaderAPI.OpenOnNodeType) { Attribute = new VariationAttribute("Open on node type: XElement (leaf II.)") { Params = new object[] { XmlNodeType.Element, 4, new string[] { "E", "p", "nsp" }, 1 }, Priority = 1 } });
                    this.AddChild(new TestVariation(xNodeReaderAPI.Namespaces) { Attribute = new VariationAttribute("Namespaces - Comment") { Params = new object[] { XmlNodeType.Comment, 1, new string[] { "", "x" }, new string[] { "p", "nsp" } } } });
                    this.AddChild(new TestVariation(xNodeReaderAPI.Namespaces) { Attribute = new VariationAttribute("Namespaces - root element") { Params = new object[] { XmlNodeType.Element, 0, new string[] { "", "" } } } });
                    this.AddChild(new TestVariation(xNodeReaderAPI.Namespaces) { Attribute = new VariationAttribute("Namespaces - element") { Params = new object[] { XmlNodeType.Element, 1, new string[] { "", "x" }, new string[] { "p", "nsp" } } } });
                    this.AddChild(new TestVariation(xNodeReaderAPI.Namespaces) { Attribute = new VariationAttribute("Namespaces - element, def. ns redef") { Params = new object[] { XmlNodeType.Element, 3, new string[] { "", "y" }, new string[] { "p", "nsp" } } } });
                    this.AddChild(new TestVariation(xNodeReaderAPI.ReadSubtreeSanity) { Attribute = new VariationAttribute("ReadSubtree (sanity)") { Priority = 0 } });
                    this.AddChild(new TestVariation(xNodeReaderAPI.AdjacentTextNodes1) { Attribute = new VariationAttribute("Adjacent text nodes (sanity I.)") { Priority = 0 } });
                    this.AddChild(new TestVariation(xNodeReaderAPI.AdjacentTextNodes2) { Attribute = new VariationAttribute("Adjacent text nodes (sanity II.) : ReadElementContent") { Priority = 0 } });
                    this.AddChild(new TestVariation(xNodeReaderAPI.AdjacentTextNodesI) { Attribute = new VariationAttribute("Adjacent text nodes (sanity IV.) : ReadInnerXml") { Priority = 0 } });
                    this.AddChild(new TestVariation(xNodeReaderAPI.AdjacentTextNodes3) { Attribute = new VariationAttribute("Adjacent text nodes (sanity III.) : ReadContent") { Priority = 0 } });
                }
            }
        }
        #endregion
    }
}
