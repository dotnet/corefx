
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using Xunit.Abstractions;
using System.Collections;
using System.IO;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace System.Xml.Tests
{
    //[TestCase(Name = "Null argument tests", Desc = "This testcase passes NULL arguments to all XslTransform methods")]
    public class CNullArgumentTest : XsltApiTestCaseBase
    {
        private ITestOutputHelper _output;
        public CNullArgumentTest(ITestOutputHelper output) : base(output)
        {
            _output = output;
        }

        //[Variation("Load(IXPathNavigable = null)", Pri = 1)]
        [InlineData()]
        [Theory]
        public void Var1()
        {
            try
            {
#pragma warning disable 0618
                new XslTransform().Load((IXPathNavigable)null);
#pragma warning restore 0618
            }
            catch (ArgumentNullException)
            {
                return;
            }
            catch (Exception e)
            {
                _output.WriteLine(e.ToString());
                _output.WriteLine("Did not throw ArgumentNullException");
            }
            Assert.True(false);
        }

        //[Variation("Load(XmlReader = null)", Pri = 1)]
        [InlineData()]
        [Theory]
        public void Var2()
        {
            try
            {
#pragma warning disable 0618
                new XslTransform().Load((XmlReader)null);
#pragma warning restore 0618
            }
            catch (ArgumentNullException)
            {
                return;
            }
            catch (Exception e)
            {
                _output.WriteLine(e.ToString());
                _output.WriteLine("Did not throw ArgumentNullException");
            }
            Assert.True(false);
        }

        //[Variation("Load(IXPathNavigable = null, XmlResolver = null)", Pri = 1)]
        [InlineData()]
        [Theory]
        public void Var3()
        {
            try
            {
#pragma warning disable 0618
                new XslTransform().Load((IXPathNavigable)null, (XmlResolver)null);
#pragma warning restore 0618
            }
            catch (ArgumentNullException)
            {
                return;
            }
            catch (Exception e)
            {
                _output.WriteLine(e.ToString());
                _output.WriteLine("Did not throw ArgumentNullException");
            }
            Assert.True(false);
        }

        //[Variation("Load(XmlReader = null, XmlResolver = null)", Pri = 1)]
        [InlineData()]
        [Theory]
        public void Var4()
        {
            try
            {
#pragma warning disable 0618
                new XslTransform().Load((XmlReader)null, (XmlResolver)null);
#pragma warning restore 0618
            }
            catch (ArgumentNullException)
            {
                return;
            }
            catch (Exception e)
            {
                _output.WriteLine(e.ToString());
                _output.WriteLine("Did not throw ArgumentNullException");
            }
            Assert.True(false);
        }

        //[Variation("Load(IXPathNavigable = null, XmlResolver = null, Evidence = null)", Pri = 1)]
        [InlineData()]
        [Theory]
        public void Var5()
        {
            /*try
            {
#pragma warning disable 0618
                new XslTransform().Load((IXPathNavigable)null, (XmlResolver)null, (Evidence)null);
#pragma warning restore 0618
            }
            catch (ArgumentNullException)
            {
                return;
            }
            catch (Exception e)
            {
                _output.WriteLine(e.ToString());
                _output.WriteLine("Did not throw ArgumentNullException");
            }
            Assert.True(false);*/
        }

        //[Variation("Load(XmlReader = null, XmlResolver = null, Evidence = null)", Pri = 1)]
        [InlineData()]
        [Theory]
        public void Var6()
        {
            /*try
            {
#pragma warning disable 0618
                new XslTransform().Load((XmlReader)null, (XmlResolver)null, (Evidence)null);
#pragma warning restore 0618
            }
            catch (ArgumentNullException)
            {
                return;
            }
            catch (Exception e)
            {
                _output.WriteLine(e.ToString());
                _output.WriteLine("Did not throw ArgumentNullException");
            }
            Assert.True(false);*/
        }

        //[Variation("Transform(IXPathNavigable = null, XsltArgumentList = null)", Pri = 1)]
        [InlineData()]
        [Theory]
        public void Var7()
        {
            try
            {
#pragma warning disable 0618
                new XslTransform().Transform((IXPathNavigable)null, (XsltArgumentList)null);
#pragma warning restore 0618
            }
            catch (ArgumentNullException)
            {
                return;
            }
            catch (Exception e)
            {
                _output.WriteLine(e.ToString());
                _output.WriteLine("Did not throw ArgumentNullException");
            }
            Assert.True(false);
        }

        //[Variation("Transform(IXPathNavigable = null, XsltArgumentList = null, XmlResolver = null)", Pri = 1)]
        [InlineData()]
        [Theory]
        public void Var8()
        {
            try
            {
#pragma warning disable 0618
                new XslTransform().Transform((IXPathNavigable)null, (XsltArgumentList)null, (XmlResolver)null);
#pragma warning restore 0618
            }
            catch (ArgumentNullException)
            {
                return;
            }
            catch (Exception e)
            {
                _output.WriteLine(e.ToString());
                _output.WriteLine("Did not throw ArgumentNullException");
            }
            Assert.True(false);
        }

        //[Variation("Transform(IXPathNavigable = null, XsltArgumentList = null, TextWriter = null)", Pri = 1)]
        [InlineData()]
        [Theory]
        public void Var9()
        {
            try
            {
#pragma warning disable 0618
                new XslTransform().Transform((IXPathNavigable)null, (XsltArgumentList)null, (TextWriter)null);
#pragma warning restore 0618
            }
            catch (ArgumentNullException)
            {
                return;
            }
            catch (Exception e)
            {
                _output.WriteLine(e.ToString());
                _output.WriteLine("Did not throw ArgumentNullException");
            }
            Assert.True(false);
        }

        //[Variation("Transform(IXPathNavigable = null, XsltArgumentList = null, TextWriter = null, XmlResolver = null)", Pri = 1)]
        [InlineData()]
        [Theory]
        public void Var10()
        {
            try
            {
#pragma warning disable 0618
                new XslTransform().Transform((IXPathNavigable)null, (XsltArgumentList)null, (TextWriter)null, (XmlResolver)null);
#pragma warning restore 0618
            }
            catch (ArgumentNullException)
            {
                return;
            }
            catch (Exception e)
            {
                _output.WriteLine(e.ToString());
                _output.WriteLine("Did not throw ArgumentNullException");
            }
            Assert.True(false);
        }

        //[Variation("Transform(IXPathNavigable = null, XsltArgumentList = null, Stream = null)", Pri = 1)]
        [InlineData()]
        [Theory]
        public void Var11()
        {
            try
            {
#pragma warning disable 0618
                new XslTransform().Transform((IXPathNavigable)null, (XsltArgumentList)null, (Stream)null);
#pragma warning restore 0618
            }
            catch (ArgumentNullException)
            {
                return;
            }
            catch (Exception e)
            {
                _output.WriteLine(e.ToString());
                _output.WriteLine("Did not throw ArgumentNullException");
            }
            Assert.True(false);
        }

        //[Variation("Transform(IXPathNavigable = null, XsltArgumentList = null, Stream = null, XmlResolver = null)", Pri = 1)]
        [InlineData()]
        [Theory]
        public void Var12()
        {
            try
            {
#pragma warning disable 0618
                new XslTransform().Transform((IXPathNavigable)null, (XsltArgumentList)null, (Stream)null, (XmlResolver)null);
#pragma warning restore 0618
            }
            catch (ArgumentNullException)
            {
                return;
            }
            catch (Exception e)
            {
                _output.WriteLine(e.ToString());
                _output.WriteLine("Did not throw ArgumentNullException");
            }
            Assert.True(false);
        }

        //[Variation("Transform(IXPathNavigable = null, XsltArgumentList = null, XmlWriter = null)", Pri = 1)]
        [InlineData()]
        [Theory]
        public void Var13()
        {
            try
            {
#pragma warning disable 0618
                new XslTransform().Transform((IXPathNavigable)null, (XsltArgumentList)null, (XmlWriter)null);
#pragma warning restore 0618
            }
            catch (ArgumentNullException)
            {
                return;
            }
            catch (Exception e)
            {
                _output.WriteLine(e.ToString());
                _output.WriteLine("Did not throw ArgumentNullException");
            }
            Assert.True(false);
        }

        //[Variation("Transform(IXPathNavigable = null, XsltArgumentList = null, XmlWriter = null, XmlResolver = null)", Pri = 1)]
        [InlineData()]
        [Theory]
        public void Var14()
        {
            try
            {
#pragma warning disable 0618
                new XslTransform().Transform((IXPathNavigable)null, (XsltArgumentList)null, (XmlWriter)null, (XmlResolver)null);
#pragma warning restore 0618
            }
            catch (ArgumentNullException)
            {
                return;
            }
            catch (Exception e)
            {
                _output.WriteLine(e.ToString());
                _output.WriteLine("Did not throw ArgumentNullException");
            }
            Assert.True(false);
        }
    }

    /***********************************************************/
    /*          XslTransform.Resolver - Integrity              */
    /***********************************************************/

    //[TestCase(Name = "XsltTransform.XmlResolver : Reader, Reader", Desc = "READER,READER")]
    //[TestCase(Name = "XsltTransform.XmlResolver : Reader, Stream", Desc = "READER,STREAM")]
    //[TestCase(Name = "XsltTransform.XmlResolver : Reader, Writer", Desc = "READER,WRITER")]
    //[TestCase(Name = "XsltTransform.XmlResolver : Reader, TextWriter", Desc = "READER,TEXTWRITER")]
    //[TestCase(Name = "XsltTransform.XmlResolver : URI, Reader", Desc = "URI,READER")]
    //[TestCase(Name = "XsltTransform.XmlResolver : URI, Stream", Desc = "URI,STREAM")]
    //[TestCase(Name = "XsltTransform.XmlResolver : URI, Writer", Desc = "URI,WRITER")]
    //[TestCase(Name = "XsltTransform.XmlResolver : URI, TextWriter", Desc = "URI,TEXTWRITER")]
    //[TestCase(Name = "XsltTransform.XmlResolver : Navigator, Reader", Desc = "NAVIGATOR,READER")]
    //[TestCase(Name = "XsltTransform.XmlResolver : Navigator, Stream", Desc = "NAVIGATOR,STREAM")]
    //[TestCase(Name = "XsltTransform.XmlResolver : Navigator, Writer", Desc = "NAVIGATOR,WRITER")]
    //[TestCase(Name = "XsltTransform.XmlResolver : Navigator, TextWriter", Desc = "NAVIGATOR,TEXTWRITER")]
    public class CXmlResolverTest : XsltApiTestCaseBase
    {
        private ITestOutputHelper _output;
        public CXmlResolverTest(ITestOutputHelper output) : base(output)
        {
            _output = output;
        }

        //[Variation("Set XmlResolver property to null, load style sheet with import/include, should not affect transform")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void XmlResolver1(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);

            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><result><fruit>Apple</fruit><fruit>orange</fruit></result>";

            try
            {
                if (LoadXSL("XmlResolver_Main.xsl", inputType, readerType) == 1)
                {
                    xslt.XmlResolver = null;
                    if (Transform("fruits.xml", transformType, docType) == 1)
                    {
                        VerifyResult(expected);
                        return;
                    }
                    else
                        Assert.True(false);
                }
            }
            catch (Exception e)
            {
                _output.WriteLine("Should not throw error loading stylesheet with include/import when resolver property is set to NULL!");
                _output.WriteLine(e.ToString());
                Assert.True(false);
            }
            Assert.True(false);
        }

        //[Variation("Set XmlResolver property to null, load style sheet with document function, should not resolve during transform")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void XmlResolver2(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><result></result>";

            // "xmlResolver_document_function.xsl" contains
            // <xsl:for-each select="document('xmlResolver_document_function.xml')//elem">

            if (LoadXSL("xmlResolver_document_function.xsl", inputType, readerType) == 1)
            {
                xslt.XmlResolver = null;
                if (Transform("fruits.xml", transformType, docType) == 1)
                {
                    VerifyResult(expected);
                    return;
                }
            }
            else
            {
                _output.WriteLine("Problem loading stylesheet with document function and resolver set to NULL!");
                Assert.True(false);
            }
            Assert.True(false);
        }

        //[Variation("Default XmlResolver, load style sheet with document function, should resolve during transform")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory(Skip = "SQLBU Defect Tracking Bug 430834: Skipping when input type is URI, see bug for more details.")]
        public void XmlResolver3(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            // "xmlResolver_document_function.xsl" contains
            // <xsl:for-each select="document('xmlResolver_document_function.xml')//elem">

            // SQLBU Defect Tracking Bug 430834: Skipping when input type is URI, see bug for more details.
            //if (MyInputType() == InputType.URI)
            //    return TEST_SKIPPED;

            if (LoadXSL("xmlResolver_document_function.xsl", inputType, readerType) == 1)
            {
                if ((Transform("fruits.xml", transformType, docType) == 1) && (CheckResult(377.8217373898, transformType) == 1))
                    return;
            }
            else
            {
                _output.WriteLine("Problem loading stylesheet with document function and default resolver!");
                Assert.True(false);
            }
            Assert.True(false);
        }

        //[Variation("document() has absolute URI")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory(Skip = "When style sheet URI = Intranet zone, XmlSecureResolver does not resolve document function")]
        public void XmlResolver7(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            // Skip this test for Load(URI)
            // Reason: When style sheet URI = Intranet zone, XmlSecureResolver does not resolve document function

            //if (MyInputType() == InputType.URI)
            //    return TEST_SKIPPED;

            // copy file on the local machine

            try
            {
                string tempDir = Path.GetTempPath();
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }
                string xmlFile = FullFilePath("xmlResolver_document_function.xml");
                File.Copy(xmlFile, Path.Combine(tempDir, "xmlResolver_document_function.xml"), true);
            }
            catch (Exception e)
            {
                _output.WriteLine(e.ToString());
                _output.WriteLine("Could not copy file to local. Some other issues prevented this test from running");
                return; //return TEST_SKIPPED;
            }

            if (LoadXSL("xmlResolver_document_function_absolute_uri.xsl", inputType, readerType) == 1)
            {
                if ((Transform("fruits.xml", transformType, docType) == 1) && (CheckResult(377.8217373898, transformType) == 1))
                    return;
                else
                {
                    _output.WriteLine("Failed to resolve document function with absolute URI.");
                    Assert.True(false);
                }
            }
            else
            {
                _output.WriteLine("Failed to load style sheet!");
                Assert.True(false);
            }
        }
    }

    /***********************************************************/
    /*          XslTransform.Load - Integrity                  */
    /***********************************************************/

    //[TestCase(Name = "XsltTransform.Load() - Integrity : Reader, Reader", Desc = "READER,READER")]
    //[TestCase(Name = "XsltTransform.Load() - Integrity : Reader, Stream", Desc = "READER,STREAM")]
    //[TestCase(Name = "XsltTransform.Load() - Integrity : Reader, Writer", Desc = "READER,WRITER")]
    //[TestCase(Name = "XsltTransform.Load() - Integrity : Reader, TextWriter", Desc = "READER,TEXTWRITER")]
    //[TestCase(Name = "XsltTransform.Load() - Integrity : URI, Reader", Desc = "URI,READER")]
    //[TestCase(Name = "XsltTransform.Load() - Integrity : URI, Stream", Desc = "URI,STREAM")]
    //[TestCase(Name = "XsltTransform.Load() - Integrity : URI, Writer", Desc = "URI,WRITER")]
    //[TestCase(Name = "XsltTransform.Load() - Integrity : URI, TextWriter", Desc = "URI,TEXTWRITER")]
    //[TestCase(Name = "XsltTransform.Load() - Integrity : Navigator, Reader", Desc = "NAVIGATOR,READER")]
    //[TestCase(Name = "XsltTransform.Load() - Integrity : Navigator, Stream", Desc = "NAVIGATOR,STREAM")]
    //[TestCase(Name = "XsltTransform.Load() - Integrity : Navigator, Writer", Desc = "NAVIGATOR,WRITER")]
    //[TestCase(Name = "XsltTransform.Load() - Integrity : Navigator, TextWriter", Desc = "NAVIGATOR,TEXTWRITER")]
    public class CLoadTest : XsltApiTestCaseBase
    {
        private ITestOutputHelper _output;
        public CLoadTest(ITestOutputHelper output) : base(output)
        {
            _output = output;
        }

        //[Variation("Call Load with null value")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory]
        public void LoadGeneric1(InputType inputType, ReaderType readerType)
        {
            try
            {
                LoadXSL(null, inputType, readerType);
            }
            catch (System.ArgumentException)
            {
                // System.Xml.XmlUrlResolver.ResolveUri(Uri baseUri, String relativeUri) throws System.ArgumentException here for null
                return;
            }
            _output.WriteLine("Exception not generated for null parameter name");
            Assert.True(false);
        }

        //[Variation("Load with valid, then invalid, then valid again")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void LoadGeneric2(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            if (LoadXSL("showParam.xsl", inputType, readerType) == 1)
            {
                try
                {
                    LoadXSL("IDontExist.xsl", inputType, readerType);
                }
                catch (System.IO.FileNotFoundException)
                {
                    try
                    {
                        Transform("fruits.xml", transformType, docType);
                    }
                    catch (System.InvalidOperationException e)
                    {
                        CheckExpectedError(e, "System.Xml", "Xslt_NoStylesheetLoaded", new string[] { "" });
                        return;
                    }
                }
            }
            else
            {
                _output.WriteLine("Failed to load style sheet!");
                Assert.True(false);
            }
            Assert.True(false);
        }

        //[Variation("Load an invalid, then a valid and transform")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void LoadGeneric3(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><result>
		1.No Value Specified
		2.No Value Specified
		3.No Value Specified
		4.No Value Specified
		5.No Value Specified
		6.No Value Specified</result>";

            try
            {
                LoadXSL("IDontExist.xsl", inputType, readerType);
            }
            catch (System.IO.FileNotFoundException)
            {
                if ((LoadXSL("showParam.xsl", inputType, readerType) == 1) && (Transform("fruits.xml", transformType, docType) == 1))
                {
                    VerifyResult(expected);
                    return;
                }
            }
            _output.WriteLine("Exception not generated for non-existent file name");
            Assert.True(false);
        }

        //[Variation("Call several overloaded functions")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void LoadGeneric4(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><result>
		1.No Value Specified
		2.No Value Specified
		3.No Value Specified
		4.No Value Specified
		5.No Value Specified
		6.No Value Specified</result>";

            if (inputType != InputType.Reader)
                LoadXSL("showParamLongName.xsl", InputType.Reader, readerType);
            if (inputType != InputType.URI)
                LoadXSL("showParamLongName.xsl", InputType.URI, readerType);
            if (inputType != InputType.Navigator)
                LoadXSL("showParamLongName.xsl", InputType.Navigator, readerType);

            if ((LoadXSL("showParam.xsl", inputType, readerType) == 0) || (Transform("fruits.xml", transformType, docType) == 0))
                Assert.True(false);

            VerifyResult(expected);

            if (inputType != InputType.Navigator)
                LoadXSL("showParamLongName.xsl", InputType.Navigator, readerType);
            if (inputType != InputType.URI)
                LoadXSL("showParamLongName.xsl", InputType.URI, readerType);
            if (inputType != InputType.Reader)
                LoadXSL("showParamLongName.xsl", InputType.Reader, readerType);

            if ((LoadXSL("showParam.xsl", inputType, readerType) == 1) && (Transform("fruits.xml", transformType, docType) == 1))
            {
                VerifyResult(expected);
                return;
            }

            Assert.True(false);
        }

        //[Variation("Call same overloaded Load() many times then transform")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void LoadGeneric5(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><result>
		1.No Value Specified
		2.No Value Specified
		3.No Value Specified
		4.No Value Specified
		5.No Value Specified
		6.No Value Specified</result>";

            for (int i = 0; i < 100; i++)
            {
                if (LoadXSL("showParam.xsl", inputType, readerType) != 1)
                {
                    _output.WriteLine("Failed to load stylesheet showParam.xsl on the {0} attempt", i);
                    Assert.True(false);
                }
            }
            if ((LoadXSL("showParam.xsl", inputType, readerType) == 1) && (Transform("fruits.xml", transformType, docType) == 1))
            {
                VerifyResult(expected);
                return;
            }
            Assert.True(false);
        }

        //[Variation("Call load with non-existing stylesheet")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory]
        public void LoadGeneric6(InputType inputType, ReaderType readerType)
        {
            try
            {
                LoadXSL("IDontExist.xsl", inputType, readerType);
            }
            catch (System.IO.FileNotFoundException)
            {
                return;
            }
            _output.WriteLine("Exception not generated for non-existent file parameter name");
            Assert.True(false);
        }

        //[Variation("Verify that style sheet is closed properly after Load - Shared Read Access")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void LoadGeneric7(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);

            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><result><fruit>Apple</fruit><fruit>orange</fruit></result>";

            FileStream s2;

            // check immediately after load and after transform
            if (LoadXSL("XmlResolver_Main.xsl", inputType, readerType) == 1)
            {
                s2 = new FileStream(FullFilePath("XmlResolver_Main.xsl"), FileMode.Open, FileAccess.Read, FileShare.Read);
                s2.Dispose();
                if (Transform("fruits.xml", transformType, docType) == 1)
                {
                    VerifyResult(expected);
                    s2 = new FileStream(FullFilePath("XmlResolver_Main.xsl"), FileMode.Open, FileAccess.Read, FileShare.Read);
                    s2.Dispose();
                    return;
                }
            }
            Assert.True(false);
        }

        //[Variation("Verify that included files are closed properly after Load - Read Access")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void LoadGeneric9(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);

            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><result><fruit>Apple</fruit><fruit>orange</fruit></result>";

            FileStream s2;

            // check immediately after load and after transform
            if (LoadXSL("XmlResolver_Main.xsl", inputType, readerType) == 1)
            {
                s2 = new FileStream(FullFilePath("XmlResolver_Sub.xsl"), FileMode.Open, FileAccess.Read);
                s2.Dispose();
                if (Transform("fruits.xml", transformType, docType) == 1)
                {
                    VerifyResult(expected);
                    s2 = new FileStream(FullFilePath("XmlResolver_Include.xsl"), FileMode.Open, FileAccess.Read, FileShare.Read);
                    s2.Dispose();
                    return;
                }
            }
            _output.WriteLine("Appeared to not close file properly after loading.");
            Assert.True(false);
        }

        //[Variation("Load stylesheet with entity reference: Bug #68450 ")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void LoadGeneric11(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><Book>
			Name
		</Book>";

            if (docType.ToString() == "DataDocument")
                // Skip the test for DataDocument
                return;
            else
            {
                if (LoadXSL("books_entity_ref.xsl", InputType.Reader, readerType) != 1)
                {
                    _output.WriteLine("Failed to load stylesheet books_entity_ref.xsl");
                    Assert.True(false);
                }
                if ((LoadXSL("books_entity_ref.xsl", inputType, readerType) == 1) && (Transform("books_entity_ref.xml", transformType, docType) == 1))
                {
                    VerifyResult(expected);
                    return;
                }
                Assert.True(false);
            }
        }

        //[Variation("Load with invalid stylesheet and verify that file is closed properly")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory]
        public void LoadGeneric12(InputType inputType, ReaderType readerType)
        {
            Stream strmTemp;

            try
            {
                int i = LoadXSL("xslt_error.xsl", inputType, readerType);
            }
            catch (System.Xml.Xsl.XsltCompileException)
            {
                // Try to open the xsl file
                try
                {
                    strmTemp = new FileStream(FullFilePath("xslt_error.xsl"), FileMode.Open, FileAccess.Read);
                }
                catch (Exception ex)
                {
                    _output.WriteLine("Did not close stylesheet properly after load");
                    _output.WriteLine(ex.Message);
                    Assert.True(false);
                }
                return;
            }
            _output.WriteLine("Did not throw compile exception for stylesheet");
            Assert.True(false);
        }
    }

    /**************************************************************************/
    /*          XslTransform.Load(,XmlResolver) - Integrity   */
    /**************************************************************************/

    //[TestCase(Name = "XsltTransform.Load(,XmlResolver) - Integrity : Reader, Reader", Desc = "READER,READER")]
    //[TestCase(Name = "XsltTransform.Load(,XmlResolver) - Integrity : Reader, Stream", Desc = "READER,STREAM")]
    //[TestCase(Name = "XsltTransform.Load(,XmlResolver) - Integrity : Reader, Writer", Desc = "READER,WRITER")]
    //[TestCase(Name = "XsltTransform.Load(,XmlResolver) - Integrity : Reader, TextWriter", Desc = "READER,TEXTWRITER")]
    //[TestCase(Name = "XsltTransform.Load(,XmlResolver) - Integrity : Navigator, Reader", Desc = "NAVIGATOR,READER")]
    //[TestCase(Name = "XsltTransform.Load(,XmlResolver) - Integrity : Navigator, Stream", Desc = "NAVIGATOR,STREAM")]
    //[TestCase(Name = "XsltTransform.Load(,XmlResolver) - Integrity : Navigator, Writer", Desc = "NAVIGATOR,WRITER")]
    //[TestCase(Name = "XsltTransform.Load(,XmlResolver) - Integrity : Navigator, TextWriter", Desc = "NAVIGATOR,TEXTWRITER")]
    //[TestCase(Name = "XsltTransform.Load(,XmlResolver) - Integrity : URI, Reader", Desc = "URI,READER")]
    //[TestCase(Name = "XsltTransform.Load(,XmlResolver) - Integrity : URI, Stream", Desc = "URI,STREAM")]
    //[TestCase(Name = "XsltTransform.Load(,XmlResolver) - Integrity : URI, Writer", Desc = "URI,WRITER")]
    //[TestCase(Name = "XsltTransform.Load(,XmlResolver) - Integrity : URI, TextWriter", Desc = "URI,TEXTWRITER")]
    public class CLoadXmlResolverTest : XsltApiTestCaseBase
    {
        private ITestOutputHelper _output;
        public CLoadXmlResolverTest(ITestOutputHelper output) : base(output)
        {
            _output = output;
        }

        //[Variation("Call Load with null source value and null resolver")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory]
        public void LoadGeneric1(InputType inputType, ReaderType readerType)
        {
            try
            {
                LoadXSL_Resolver(null, null, inputType, readerType);
            }
            catch (System.ArgumentException)
            {
                // System.Xml.XmlUrlResolver.ResolveUri(Uri baseUri, String relativeUri) throws ArgumentException
                return;
            }
            _output.WriteLine("Did not throw an exception for null argument!");
            Assert.True(false);
        }

        //[Variation("Call Load with null source value and valid resolver")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory]
        public void LoadGeneric2(InputType inputType, ReaderType readerType)
        {
            try
            {
                LoadXSL_Resolver(null, new XmlUrlResolver(), inputType, readerType);
            }
            catch (System.ArgumentException)
            {
                // System.Xml.XmlUrlResolver.ResolveUri(Uri baseUri, String relativeUri) throws ArgumentException
                return;
            }
            _output.WriteLine("Did not throw an exception for null argument!");
            Assert.True(false);
        }

        //[Variation("Call Load with null XmlResolver, style sheet does not have include/import, should not error")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void LoadGeneric3(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><result>
		1.No Value Specified
		2.No Value Specified
		3.No Value Specified
		4.No Value Specified
		5.No Value Specified
		6.No Value Specified</result>";

            if (LoadXSL_Resolver("showParam.xsl", null, inputType, readerType) == 1)
            {
                if (Transform("fruits.xml", transformType, docType) == 1)
                {
                    VerifyResult(expected);
                    return;
                }
                else
                    Assert.True(false);
            }
            else
            {
                _output.WriteLine("Failed to load style sheet!");
                Assert.True(false);
            }
        }

        //[Variation("Call Load with null XmlResolver and style sheet has import/include, should fail")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory]
        public void LoadGeneric4(InputType inputType, ReaderType readerType)
        {
            try
            {
                LoadXSL_Resolver("XmlResolver_Main.xsl", null, inputType, readerType);
            }
            catch (System.Xml.Xsl.XsltCompileException e)
            {
                CheckExpectedError(e.InnerException, "System.Xml", "Xml_NullResolver", new string[] { "" });
                return;
            }
            _output.WriteLine("Exception not thrown for null resolver");
            Assert.True(false);
        }

        //[Variation("Call Load with null custom resolver and style sheet has import/include, should fail")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory(Skip = "By design bug #84957: Skip this test for Load(url, resolver)")]
        public void LoadGeneric5(InputType inputType, ReaderType readerType)
        {
            // By design bug #84957: Skip this test for Load(url, resolver)
            //if (MyInputType() == InputType.URI)
            //{
            //    _output.WriteLine("By design bug #84957: Skip this test for Load(url, resolver)");
            //    return TEST_SKIPPED;
            //}

            CustomNullResolver myResolver = new CustomNullResolver(null);
            try
            {
                LoadXSL_Resolver("XmlResolver_Main.xsl", myResolver, inputType, readerType);
            }
            catch (System.Xml.Xsl.XsltCompileException e)
            {
                CheckExpectedError(e.InnerException, "System.Data.Sqlxml", "Xslt_CantResolve", new string[] { new Uri(FullFilePath("XmlResolver_Include.xsl", false)).ToString() });
                return;
            }
            _output.WriteLine("Exception not thrown for null resolver");
            Assert.True(false);
        }

        //[Variation("Call Load with null custom resolver and style sheet has no import/include, should not error")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory(Skip = "By design bug #84957: Skip this test for Load(url, resolver)")]
        public void LoadGeneric6(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            // By design bug #84957: Skip this test for Load(url, resolver)

            //if (MyInputType() == InputType.URI)
            //{
            //    _output.WriteLine("By design bug #84957: Skip this test for Load(url, resolver)");
            //    return TEST_SKIPPED;
            //}

            CustomNullResolver myResolver = new CustomNullResolver(null);
            if (LoadXSL_Resolver("showParam.xsl", myResolver, inputType, readerType) == 1)
            {
                if ((Transform("fruits.xml", transformType, docType) == 1) && (CheckResult(466.5112789241, transformType) == 1))
                    return;
                else
                    Assert.True(false);
            }
            else
            {
                _output.WriteLine("Failed to load style sheet!");
                Assert.True(false);
            }
        }

        //[Variation("Style sheet has import/include, call Load first with custom null resolver and then default resolver, should not fail")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory(Skip = "By design bug #84957: Skip this test for Load(url, resolver)")]
        public void LoadGeneric7(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            // By design bug #84957: Skip this test for Load(url, resolver)

            //if (MyInputType() == InputType.URI)
            //{
            //    _output.WriteLine("By design bug #84957: Skip this test for Load(url, resolver)");
            //    return TEST_SKIPPED;
            //}

            CustomNullResolver myResolver = new CustomNullResolver(null);

            try
            {
                LoadXSL_Resolver("XmlResolver_Main.xsl", myResolver, inputType, readerType);
            }
            catch (System.Xml.Xsl.XsltCompileException e)
            {
                CheckExpectedError(e.InnerException, "System.Data.Sqlxml", "Xslt_CantResolve", new string[] { new Uri(FullFilePath("XmlResolver_Include.xsl", false)).ToString() });
                if (LoadXSL("XmlResolver_Main.xsl", inputType, readerType) == 1)
                {
                    if ((Transform("fruits.xml", transformType, docType) == 1) && (CheckResult(428.8541842246, transformType) == 1))
                        return;
                    else
                        Assert.True(false);
                }
                else
                {
                    _output.WriteLine("Failed to load stylesheet using default resolver");
                    Assert.True(false);
                }
            }
            Assert.True(false);
        }

        //[Variation("Style sheet has import/include, call Load first with default resolver and then with custom null resolver, should fail")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory(Skip = "By design bug #84957: Skip this test for Load(url, resolver)")]
        public void LoadGeneric8(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            // By design bug #84957: Skip this test for Load(url, resolver)

            //if (MyInputType() == InputType.URI)
            //{
            //    _output.WriteLine("By design bug #84957: Skip this test for Load(url, resolver)");
            //    return TEST_SKIPPED;
            //}

            CustomNullResolver myResolver = new CustomNullResolver(null);

            if ((LoadXSL("XmlResolver_Main.xsl", inputType, readerType) == 1))
            {
                try
                {
                    LoadXSL_Resolver("XmlResolver_Main.xsl", myResolver, inputType, readerType);
                }
                catch (System.Xml.Xsl.XsltCompileException e)
                {
                    CheckExpectedError(e.InnerException, "System.Data.Sqlxml", "Xslt_CantResolve", new string[] { new Uri(FullFilePath("XmlResolver_Include.xsl", false)).ToString() });
                    return;
                }
                _output.WriteLine("No exception generated when loading with an invalid resolver after loading with valid resolver");
                Assert.True(false);
            }
            _output.WriteLine("Could not load style sheet with default resolver");
            Assert.True(false);
        }

        //[Variation("Load with resolver with credentials, then load XSL that does not need cred.")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void LoadGeneric9(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);

            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><result><fruit>Apple</fruit><fruit>orange</fruit></result>";

            if ((LoadXSL_Resolver("XmlResolver_Main.xsl", GetDefaultCredResolver(), inputType, readerType) == 1))
            {
                if ((LoadXSL("XmlResolver_Main.xsl", inputType, readerType) == 1) && (Transform("fruits.xml", transformType, docType) == 1))
                {
                    VerifyResult(expected);
                    return;
                }
            }
            else
            {
                _output.WriteLine("Failed to load!");
                Assert.True(false);
            }
            Assert.True(false);
        }

        //[Variation("Call Load() many times with null resolver then perform a transform")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void LoadGeneric10(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><result>
		1.No Value Specified
		2.No Value Specified
		3.No Value Specified
		4.No Value Specified
		5.No Value Specified
		6.No Value Specified</result>";

            for (int i = 0; i < 100; i++)
            {
                if (LoadXSL_Resolver("showParam.xsl", null, inputType, readerType) != 1)
                {
                    _output.WriteLine("Failed to load stylesheet showParam.xsl on the {0} attempt", i);
                    Assert.True(false);
                }
            }
            if ((LoadXSL_Resolver("showParam.xsl", null, inputType, readerType) == 1) && (Transform("fruits.xml", transformType, docType) == 1))
            {
                VerifyResult(expected);
                return;
            }
            Assert.True(false);
        }

        //[Variation("Call Load with null Resolver, file does not exist")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory]
        public void LoadGeneric11(InputType inputType, ReaderType readerType)
        {
            try
            {
                LoadXSL_Resolver("IDontExist.xsl", null, inputType, readerType);
            }
            catch (System.IO.FileNotFoundException)
            {
                return;
            }
            _output.WriteLine("Exception not generated for non-existent file parameter name");
            Assert.True(false);
        }

        //[Variation("Load non existing stylesheet with null resolver and try to transform")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void LoadGeneric12(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            if (LoadXSL_Resolver("showParam.xsl", null, inputType, readerType) == 1)
            {
                try
                {
                    LoadXSL_Resolver("IDontExist.xsl", null, inputType, readerType);
                }
                catch (System.IO.FileNotFoundException)
                {
                    //no stylesheet loaded, should throw error
                    try
                    {
                        Transform("fruits.xml", transformType, docType);
                    }
                    catch (System.InvalidOperationException e2)
                    {
                        CheckExpectedError(e2, "system.xml", "Xslt_NoStylesheetLoaded", new string[] { "IDontExist.xsl" });
                        return;
                    }
                }
                _output.WriteLine("Exception not generated for non-existent file parameter name");
            }
            else
            {
                _output.WriteLine("Errors loading initial file");
                Assert.True(false);
            }
            Assert.True(false);
        }
    }

    /***********************************************************/
    /*          XslTransform.Load(Url, Resolver)               */
    /***********************************************************/

    //[TestCase(Name = "XsltTransform.Load(Url, Resolver) : URI, Reader", Desc = "URI,READER")]
    //[TestCase(Name = "XsltTransform.Load(Url, Resolver) : URI, Stream", Desc = "URI,STREAM")]
    //[TestCase(Name = "XsltTransform.Load(Url, Resolver) : URI, Writer", Desc = "URI,WRITER")]
    //[TestCase(Name = "XsltTransform.Load(Url, Resolver) : URI, TextWriter", Desc = "URI,TEXTWRITER")]
    public class CLoadUrlResolverTest : XsltApiTestCaseBase
    {
        private ITestOutputHelper _output;
        public CLoadUrlResolverTest(ITestOutputHelper output) : base(output)
        {
            _output = output;
        }

        //[Variation("Basic check for usage of credentials on resolver, load XSL that needs cred. with correct resolver")]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void LoadUrlResolver1(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><out>Red50Orange25</out>";

            // XsltResolverTestMain.xsl is placed in IIS virtual directory
            // which requires integrated Windows NT authentication
            if ((LoadXSL_Resolver(("XmlResolver/XsltResolverTestMain.xsl"), GetDefaultCredResolver(), inputType, readerType) == 1) &&
                (Transform("fruits.xml", transformType, docType) == 1))
            {
                VerifyResult(expected);
                return;
            }

            Assert.True(false);
        }

        //[Variation("Load XSL that needs cred. with null resolver, should fail")]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [Theory]
        public void LoadUrlResolver2(InputType inputType, ReaderType readerType)
        {
            try
            {
                LoadXSL_Resolver(("XmlResolver/XsltResolverTestMain.xsl"), null, inputType, readerType);
            }
            catch (System.Xml.Xsl.XsltException)
            {
                //return CheckExpectedError(e, "system.xml", "Xml_NullResolver");
                return;
            }
            _output.WriteLine("Should not have been able to retrieve and resolve style sheet with null resolver");
            Assert.True(false);
        }

        //[Variation("Call Load with null source value")]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [Theory]
        public void LoadUrlResolver3(InputType inputType, ReaderType readerType)
        {
            try
            {
                LoadXSL_Resolver(null, new XmlUrlResolver(), inputType, readerType);
            }
            catch (System.ArgumentException)
            {
                // System.Xml.XmlUrlResolver.ResolveUri(Uri baseUri, String relativeUri) throws ArgumentException
                return;
            }
            _output.WriteLine("Did not throw an exception for null argument!");
            Assert.True(false);
        }
    }

    /***********************************************************/
    /*          XslTransform.Load(Url)                         */
    /***********************************************************/

    //[TestCase(Name = "XsltTransform.Load(Url) Integrity : URI, Stream", Desc = "URI,STREAM")]
    public class CLoadStringTest : XsltApiTestCaseBase
    {
        private ITestOutputHelper _output;
        public CLoadStringTest(ITestOutputHelper output) : base(output)
        {
            _output = output;
        }

        //[Variation("Call Load with an invalid uri")]
        [InlineData(ReaderType.XmlValidatingReader)]
        [Theory]
        public void LoadUrl1(ReaderType readerType)
        {
            try
            {
                LoadXSL("IDontExist.xsl", InputType.URI, readerType);
            }
            catch (System.IO.FileNotFoundException)
            {
                return;
            }
            _output.WriteLine("Exception not generated for non-existent file parameter name");
            Assert.True(false);
        }

        //[Variation("Load file with empty string")]
        [InlineData(ReaderType.XmlValidatingReader)]
        [Theory]
        public void LoadUrl2(ReaderType readerType)
        {
            try
            {
                LoadXSL(szEmpty, InputType.URI, readerType);
            }
            catch (System.ArgumentException)
            {
                // System.Xml.XmlUrlResolver.ResolveUri(Uri baseUri, String relativeUri) throws ArgumentException
                return;
            }
            _output.WriteLine("Exception not generated for an empty string filename");
            Assert.True(false);
        }

        //[Variation("Load with \".\"")]
        [InlineData(ReaderType.XmlValidatingReader)]
        [Theory]
        public void LoadUrl3(ReaderType readerType)
        {
            try
            {
                LoadXSL(".", InputType.URI, readerType);
            }
            catch (System.UnauthorizedAccessException)
            {
                return;
            }
            _output.WriteLine("Exception not generated for non-existent file parameter name");
            Assert.True(false);
        }

        //[Variation("Load with \"..\"")]
        [InlineData(ReaderType.XmlValidatingReader)]
        [Theory]
        public void LoadUrl(ReaderType readerType)
        {
            try
            {
                LoadXSL("..", InputType.URI, readerType);
            }
            catch (System.UnauthorizedAccessException)
            {
                return;
            }
            _output.WriteLine("Exception not generated for non-existent file parameter name");
            Assert.True(false);
        }

        //[Variation("Load with \"\\\\\"")]
        [PlatformSpecific(TestPlatforms.Windows)] //Not a valid path on Unix
        [InlineData(ReaderType.XmlValidatingReader)]
        [Theory]
        public void LoadUrl5(ReaderType readerType)
        {
            try
            {
                LoadXSL("\\\\", InputType.URI, readerType);
            }
            catch (System.ArgumentException)
            {
                // System.Xml.XmlUrlResolver.ResolveUri(Uri baseUri, String relativeUri) throws ArgumentException
                return;
            }
            _output.WriteLine("Exception not generated for non-existent file parameter name");
            Assert.True(false);
        }
    }

    /***********************************************************/
    /*          XslTransform.Load(IXPathNavigable)             */
    /***********************************************************/

    //[TestCase(Name = "XsltTransform .Load(IXPathNavigable) : Navigator, TextWriter", Desc = "NAVIGATOR,TEXTWRITER")]
    public class CLoadXPathNavigableTest : XsltApiTestCaseBase
    {
        private ITestOutputHelper _output;
        public CLoadXPathNavigableTest(ITestOutputHelper output) : base(output)
        {
            _output = output;
        }

        //[Variation("Basic Verification Test")]
        [InlineData(TransformType.Stream, DocType.XPathDocument)]
        [Theory]
        public void LoadNavigator1(TransformType transformType, DocType docType)
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><result>
		1.No Value Specified
		2.No Value Specified
		3.No Value Specified
		4.No Value Specified
		5.No Value Specified
		6.No Value Specified</result>";

#pragma warning disable 0618
            xslt = new XslTransform();
#pragma warning restore 0618
            String _strXslFile = "showParam.xsl";

            _strXslFile = FullFilePath(_strXslFile);
            _output.WriteLine("Compiling {0}", _strXslFile);

#pragma warning disable 0618
            XmlValidatingReader xrLoad = new XmlValidatingReader(new XmlTextReader(_strXslFile));
#pragma warning restore 0618
            XPathDocument xdTemp = new XPathDocument(xrLoad, XmlSpace.Preserve);
            xrLoad.Dispose();
            xslt.Load(xdTemp);

            if (Transform("fruits.xml", transformType, docType) == 1)
            {
                VerifyResult(expected);
                return;
            }
            Assert.True(false);
        }

        //[Variation("Create Navigator and navigate away from root")]
        [InlineData(TransformType.Stream, DocType.XPathDocument)]
        [Theory]
        public void LoadNavigator2(TransformType transformType, DocType docType)
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><result>
		1.No Value Specified
		2.No Value Specified
		3.No Value Specified
		4.No Value Specified
		5.No Value Specified
		6.No Value Specified</result>";

#pragma warning disable 0618
            xslt = new XslTransform();
            XmlValidatingReader xrLoad = new XmlValidatingReader(new XmlTextReader(FullFilePath("showParam.xsl")));
#pragma warning restore 0618
            XPathDocument xdTemp = new XPathDocument(xrLoad, XmlSpace.Preserve);
            xrLoad.Dispose();
            XPathNavigator xP = ((IXPathNavigable)xdTemp).CreateNavigator();

            xP.MoveToNext();
            xslt.Load(xP);

            if (Transform("fruits.xml", transformType, docType) == 1)
            {
                VerifyResult(expected);
                return;
            }
            Assert.True(false);
        }

        //[Variation("Basic check for usage of credentials on resolver, load XSL that needs cred. with correct resolver")]
        [InlineData(TransformType.Stream, DocType.XPathDocument)]
        [Theory]
        public void LoadNavigator3(TransformType transformType, DocType docType)
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><out>Red50Orange25</out>";

#pragma warning disable 0618
            xslt = new XslTransform();
            XmlValidatingReader xrLoad = new XmlValidatingReader(new XmlTextReader(FullFilePath("XmlResolver/XsltResolverTestMain.xsl")));
#pragma warning restore 0618
            xrLoad.XmlResolver = GetDefaultCredResolver();

            XPathDocument xdTemp = new XPathDocument(xrLoad, XmlSpace.Preserve);
            XPathNavigator xP = ((IXPathNavigable)xdTemp).CreateNavigator();

            xslt.Load(xP, GetDefaultCredResolver());
            if (Transform("fruits.xml", transformType, docType) == 1)
            {
                VerifyResult(expected);
                return;
            }

            Assert.True(false);
        }

        //[Variation("Regression case for bug 80768")]
        [InlineData(TransformType.Stream)]
        [Theory(Skip = "Not InProc")]
        public void LoadNavigator4(TransformType transformType)
        {
            //if (_isInProc)
            //    return TEST_SKIPPED;

#pragma warning disable 0618
            xslt = new XslTransform();

            XmlValidatingReader xrLoad = new XmlValidatingReader(new XmlTextReader(FullFilePath("Bug80768.xsl")));
#pragma warning restore 0618
            XPathDocument xd = new XPathDocument(xrLoad, XmlSpace.Preserve);

            xslt.Load(xd);

            FileStream fs = new FileStream(_strOutFile, FileMode.Create, FileAccess.ReadWrite);
            XPathNavigator xn = new MyNavigator(FullFilePath("foo.xml"));
            xslt.Transform(xn, null, fs);
            fs.Dispose();

            if (CheckResult(383.0855503831, transformType) == 1)
                return;
            else
                Assert.True(false);
        }
    }

    /***********************************************************/
    /*          XslTransform.Load(Reader)                      */
    /***********************************************************/

    //[TestCase(Name = "XsltTransform.Load(Reader) : Reader, Stream", Desc = "READER,STREAM")]
    public class CLoadReaderTest : XsltApiTestCaseBase
    {
        private ITestOutputHelper _output;
        public CLoadReaderTest(ITestOutputHelper output) : base(output)
        {
            _output = output;
        }

        //[Variation("Basic Verification Test")]
        [InlineData(TransformType.Stream, DocType.XPathDocument)]
        [Theory]
        public void LoadXmlReader1(TransformType transformType, DocType docType)
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><result>
		1.No Value Specified
		2.No Value Specified
		3.No Value Specified
		4.No Value Specified
		5.No Value Specified
		6.No Value Specified</result>";

            Boolean fTEST_FAIL = false;
#pragma warning disable 0618
            xslt = new XslTransform();

            XmlValidatingReader xrTemp = new XmlValidatingReader(new XmlTextReader(FullFilePath("showParam.xsl")));
#pragma warning restore 0618
            xrTemp.ValidationType = ValidationType.None;
            xrTemp.EntityHandling = EntityHandling.ExpandEntities;
            try
            {
                xslt.Load(xrTemp);
            }
            catch (Exception ex)
            {
                fTEST_FAIL = true;
                throw (ex);
            }
            finally
            {
                xrTemp.Dispose();
            }
            if (fTEST_FAIL)
                Assert.True(false);
            if (Transform("fruits.xml", transformType, docType) == 1)
            {
                VerifyResult(expected);
                return;
            }
            Assert.True(false);
        }

        //[Variation("Calling with a closed reader, should throw exception")]
        [InlineData()]
        [Theory]
        public void LoadXmlReader2()
        {
#pragma warning disable 0618
            xslt = new XslTransform();

            XmlValidatingReader xrTemp = new XmlValidatingReader(new XmlTextReader(FullFilePath("showParam.xsl")));
#pragma warning restore 0618
            xrTemp.ValidationType = ValidationType.None;
            xrTemp.EntityHandling = EntityHandling.ExpandEntities;
            xrTemp.Dispose();

            try
            {
                xslt.Load(xrTemp);
            }
            catch (System.Xml.Xsl.XsltCompileException e)
            {
                if (PlatformDetection.IsFullFramework)
                    CheckExpectedError(e.InnerException, "system.data.sqlxml", "Xslt_WrongStylesheetElement", new string[] { "" });
                else
                    CheckExpectedError(e.InnerException, "system.xml", "Xslt_WrongStylesheetElement", new string[] { "" });
                return;
            }
            _output.WriteLine("No exception thrown for a loading a closed reader!");
            Assert.True(false);
        }

        //[Variation("Verify Reader isn""t closed after Load")]
        [InlineData()]
        [Theory]
        public void LoadXmlReader3()
        {
            Boolean fTEST_FAIL = false;
#pragma warning disable 0618
            xslt = new XslTransform();

            XmlValidatingReader xrTemp = new XmlValidatingReader(new XmlTextReader(FullFilePath("showParam.xsl")));
#pragma warning restore 0618
            xrTemp.ValidationType = ValidationType.None;
            xrTemp.EntityHandling = EntityHandling.ExpandEntities;
            try
            {
                xslt.Load(xrTemp);
            }
            catch (Exception ex)
            {
                fTEST_FAIL = true;
                throw (ex);
            }
            finally
            {
                if (!fTEST_FAIL || (xrTemp.ReadState != ReadState.Closed))
                    fTEST_FAIL = false;
                xrTemp.Dispose();
            }
            if (fTEST_FAIL)
            {
                _output.WriteLine("Appear to have accidently closed the Reader");
                Assert.True(false);
            }
            return;
        }

        //[Variation("Verify position of node in Reader is at EOF after Load")]
        [InlineData()]
        [Theory]
        public void LoadXmlReader4()
        {
            Boolean fTEST_FAIL = false;
#pragma warning disable 0618
            xslt = new XslTransform();

            XmlValidatingReader xrTemp = new XmlValidatingReader(new XmlTextReader(FullFilePath("showParam.xsl")));
#pragma warning restore 0618
            xrTemp.ValidationType = ValidationType.None;
            xrTemp.EntityHandling = EntityHandling.ExpandEntities;
            try
            {
                xslt.Load(xrTemp);
            }
            catch (Exception ex)
            {
                fTEST_FAIL = true;
                throw (ex);
            }
            finally
            {
                if (!fTEST_FAIL && (!xrTemp.EOF))
                    fTEST_FAIL = false;
                xrTemp.Dispose();
            }
            if (fTEST_FAIL)
            {
                _output.WriteLine("Reader does not appear to be at the end of file.");
                Assert.True(false);
            }
            return;
        }

        //[Variation("Load with reader position at EOF, should throw exception")]
        [InlineData()]
        [Theory]
        public void LoadXmlReader5()
        {
            Boolean fTEST_FAIL = false;
#pragma warning disable 0618
            xslt = new XslTransform();

            XmlValidatingReader xrTemp = new XmlValidatingReader(new XmlTextReader(FullFilePath("showParam.xsl")));
#pragma warning restore 0618
            xrTemp.ValidationType = ValidationType.None;
            xrTemp.EntityHandling = EntityHandling.ExpandEntities;
            xslt.Load(xrTemp);
            try
            {
                xslt.Load(xrTemp);  // should now be at end and should give exception
                fTEST_FAIL = true;
            }
            catch (System.Xml.Xsl.XsltCompileException e)
            {

                if (PlatformDetection.IsFullFramework)
                    CheckExpectedError(e.InnerException, "system.data.sqlxml", "Xslt_WrongStylesheetElement", new string[] { "" });
                else
                    CheckExpectedError(e.InnerException, "system.xml", "Xslt_WrongStylesheetElement", new string[] { "" });
            }
            finally
            {
                xrTemp.Dispose();
            }
            if (fTEST_FAIL)
                Assert.True(false);
            return;
        }

        //[Variation("Load with NULL reader, should throw System.ArgumentNullException")]
        [InlineData()]
        [Theory]
        public void LoadXmlReader6()
        {
#pragma warning disable 0618
            xslt = new XslTransform();
#pragma warning restore 0618

            XmlTextReader xrTemp = null;

            try
            {
                xslt.Load(xrTemp);  // should now be at end and should give exception
            }
            catch (System.ArgumentNullException)
            {
                return;
            }
            _output.WriteLine("Failed to throw System.ArgumentNullException for NULL reader input");
            Assert.True(false);
        }

        //[Variation("Basic check for usage of credentials on resolver, load XSL that needs cred. with correct resolver")]
        [InlineData(TransformType.Stream, DocType.XPathDocument)]
        [Theory]
        public void LoadXmlReader7(TransformType transformType, DocType docType)
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><out>Red50Orange25</out>";

#pragma warning disable 0618
            xslt = new XslTransform();
            XmlValidatingReader xrLoad = new XmlValidatingReader(new XmlTextReader(FullFilePath("XmlResolver/XsltResolverTestMain.xsl")));
#pragma warning restore 0618
            xrLoad.XmlResolver = GetDefaultCredResolver();
            xslt.Load(xrLoad, GetDefaultCredResolver());
            xrLoad.Dispose();

            if (Transform("fruits.xml", transformType, docType) == 1)
            {
                VerifyResult(expected);
                return;
            }

            Assert.True(false);
        }
    }

    /***********************************************************/
    /*          XslTransform.Transform - Integrity             */
    /***********************************************************/

    //[TestCase(Name = "XsltTransform.Transform() Integrity : Reader , Reader", Desc = "READER,READER")]
    //[TestCase(Name = "XsltTransform.Transform() Integrity : Reader, Stream", Desc = "READER,STREAM")]
    //[TestCase(Name = "XsltTransform.Transform() Integrity : Reader, Writer", Desc = "READER,WRITER")]
    //[TestCase(Name = "XsltTransform.Transform() Integrity : Reader, TextWriter", Desc = "READER,TEXTWRITER")]
    //[TestCase(Name = "XsltTransform.Transform() Integrity : URI, Reader", Desc = "URI,READER")]
    //[TestCase(Name = "XsltTransform.Transform() Integrity : URI, Stream", Desc = "URI,STREAM")]
    //[TestCase(Name = "XsltTransform.Transform() Integrity : URI, Writer", Desc = "URI,WRITER")]
    //[TestCase(Name = "XsltTransform.Transform() Integrity : URI, TextWriter", Desc = "URI,TEXTWRITER")]
    //[TestCase(Name = "XsltTransform.Transform() Integrity : Navigator, Reader", Desc = "NAVIGATOR,READER")]
    //[TestCase(Name = "XsltTransform.Transform() Integrity : Navigator, Stream", Desc = "NAVIGATOR,STREAM")]
    //[TestCase(Name = "XsltTransform.Transform() Integrity : Navigator, Writer", Desc = "NAVIGATOR,WRITER")]
    //[TestCase(Name = "XsltTransform.Transform() Integrity : Navigator, TextWriter", Desc = "NAVIGATOR,TEXTWRITER")]
    public class CTransformTestGeneric : XsltApiTestCaseBase
    {
        private ITestOutputHelper _output;
        public CTransformTestGeneric(ITestOutputHelper output) : base(output)
        {
            _output = output;
        }

        //[Variation("Basic Verification Test")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void TransformGeneric1(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><result>
		1.No Value Specified
		2.No Value Specified
		3.No Value Specified
		4.No Value Specified
		5.No Value Specified
		6.No Value Specified</result>";

            if ((LoadXSL("showParam.xsl", inputType, readerType) == 1) && (Transform("fruits.xml", transformType, docType) == 1))
            {
                VerifyResult(expected);
                return;
            }
            else
                Assert.True(false);
        }

        //[Variation("Load and Transform multiple times")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void TransformGeneric2(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><result>
		1.No Value Specified
		2.No Value Specified
		3.No Value Specified
		4.No Value Specified
		5.No Value Specified
		6.No Value Specified</result>";

            for (int i = 0; i < 5; i++)
            {
                if ((LoadXSL("showParam.xsl", inputType, readerType) != 1) || (Transform("fruits.xml", transformType, docType) != 1))
                    Assert.True(false);
                VerifyResult(expected);
            }
            return;
        }

        //[Variation("Load once, Transform many times")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void TransformGeneric3(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><result>
		1.No Value Specified
		2.No Value Specified
		3.No Value Specified
		4.No Value Specified
		5.No Value Specified
		6.No Value Specified</result>";

            if (LoadXSL("showParam.xsl", inputType, readerType) == 1)
            {
                for (int i = 0; i < 100; i++)
                {
                    if (Transform("fruits.xml", transformType, docType) != 1)
                    {
                        _output.WriteLine("Test failed to transform after {0} iterations", i);
                        Assert.True(false);
                    }
                    VerifyResult(expected);
                }
                return;
            }
            Assert.True(false);
        }

        //[Variation("Call Transform without loading")]
        [InlineData(TransformType.Reader, DocType.XPathDocument)]
        [InlineData(TransformType.Stream, DocType.XPathDocument)]
        [InlineData(TransformType.Writer, DocType.XPathDocument)]
        [InlineData(TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(TransformType.Reader, DocType.XPathDocument)]
        [InlineData(TransformType.Writer, DocType.XPathDocument)]
        [InlineData(TransformType.Stream, DocType.XPathDocument)]
        [InlineData(TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(TransformType.Reader, DocType.XPathDocument)]
        [InlineData(TransformType.Writer, DocType.XPathDocument)]
        [InlineData(TransformType.Stream, DocType.XPathDocument)]
        [InlineData(TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void TransformGeneric4(TransformType transformType, DocType docType)
        {
#pragma warning disable 0618
            xslt = new XslTransform();
#pragma warning restore 0618
            try
            {
                Transform("fruits.xml", transformType, docType);
            }
            catch (System.InvalidOperationException e)
            {
                CheckExpectedError(e, "system.xml", "Xslt_NoStylesheetLoaded", new string[] { "" });
                return;
            }
            _output.WriteLine("Exception not given for a transform that didn't have a Load method instantiated");
            Assert.True(false);
        }

        //[Variation("Closing XSL and XML files used in transform, Read access")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void TransformGeneric5(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            FileStream s2;

            if ((LoadXSL("showParam.xsl", inputType, readerType) == 1) && (Transform("fruits.xml", transformType, docType) == 1))
            {
                s2 = new FileStream(FullFilePath("showParam.xsl"), FileMode.Open, FileAccess.Read);
                s2.Dispose();

                s2 = new FileStream(FullFilePath("fruits.xml"), FileMode.Open, FileAccess.Read);
                s2.Dispose();

                return;
            }
            _output.WriteLine("Encountered errors performing transform and could not verify if files were closed");
            Assert.True(false);
        }

        //[Variation("Bug358103 - ArgumentOutOfRangeException in forwards-compatible mode for <foo bar='{+1}'/>")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void TransformGeneric7(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            try
            {
                LoadXSL("ForwardComp2.xsl", inputType, readerType);
                Transform("data.xml", true, transformType, docType);
            }
            catch (XsltException)
            {
                //LineInfo lInfo = new LineInfo(e.LineNumber, e.LinePosition, new Uri(FullFilePath("forwardcomp2.xsl", false), UriKind.Relative).ToString());
                //CheckExpectedError(e, "System.xml", "Xslt_InvalidXPath", new string[] { "string(+1)" }, lInfo);
                return;
            }
            _output.WriteLine("XsltException (Xslt_InvalidXPath) was expected");
            Assert.True(false);
        }
    }

    /*************************************************************/
    /*          XslTransform(,Resolver) - Integrity              */
    /*************************************************************/

    //[TestCase(Name = "XsltTransform.Transform(,XmlResolver) : Reader, Reader", Desc = "READER,READER")]
    //[TestCase(Name = "XsltTransform.Transform(,XmlResolver) : Reader, Stream", Desc = "READER,STREAM")]
    //[TestCase(Name = "XsltTransform.Transform(,XmlResolver) : Reader, Writer", Desc = "READER,WRITER")]
    //[TestCase(Name = "XsltTransform.Transform(,XmlResolver) : Reader, TextWriter", Desc = "READER,TEXTWRITER")]
    //[TestCase(Name = "XsltTransform.Transform(,XmlResolver) : URI, Reader", Desc = "URI,READER")]
    //[TestCase(Name = "XsltTransform.Transform(,XmlResolver) : URI, Stream", Desc = "URI,STREAM")]
    //[TestCase(Name = "XsltTransform.Transform(,XmlResolver) : URI, Writer", Desc = "URI,WRITER")]
    //[TestCase(Name = "XsltTransform.Transform(,XmlResolver) : URI, TextWriter", Desc = "URI,TEXTWRITER")]
    //[TestCase(Name = "XsltTransform.Transform(,XmlResolver) : Navigator, Reader", Desc = "NAVIGATOR,READER")]
    //[TestCase(Name = "XsltTransform.Transform(,XmlResolver) : Navigator, Stream", Desc = "NAVIGATOR,STREAM")]
    //[TestCase(Name = "XsltTransform.Transform(,XmlResolver) : Navigator, Writer", Desc = "NAVIGATOR,WRITER")]
    //[TestCase(Name = "XsltTransform.Transform(,XmlResolver) : Navigator, TextWriter", Desc = "NAVIGATOR,TEXTWRITER")]
    public class CTransformResolverTest : XsltApiTestCaseBase
    {
        private ITestOutputHelper _output;
        public CTransformResolverTest(ITestOutputHelper output) : base(output)
        {
            _output = output;
        }

        //[Variation("Pass null XmlResolver, load style sheet with import/include, should not affect transform")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void XmlResolver1(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);

            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><result><fruit>Apple</fruit><fruit>orange</fruit></result>";

            try
            {
                if (LoadXSL("XmlResolver_Main.xsl", inputType, readerType) == 1)
                {
                    if (TransformResolver("fruits.xml", transformType, docType, null) == 1)
                    {
                        VerifyResult(expected);
                        return;
                    }
                    else
                        Assert.True(false);
                }
            }
            catch (Exception e)
            {
                _output.WriteLine(e.ToString());
                Assert.True(false);
            }
            Assert.True(false);
        }

        //[Variation("Pass null XmlResolver, load style sheet with document function, should not resolve during transform")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void XmlResolver2(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><result></result>";

            // "xmlResolver_document_function.xsl" contains
            // <xsl:for-each select="document('xmlResolver_document_function.xml')//elem">

            if (LoadXSL("xmlResolver_document_function.xsl", inputType, readerType) == 1)
            {
                if (TransformResolver("fruits.xml", transformType, docType, null) == 1)
                {
                    VerifyResult(expected);
                    return;
                }
            }
            else
            {
                _output.WriteLine("Problem loading stylesheet!");
                Assert.True(false);
            }
            Assert.True(false);
        }

        //[Variation("Default XmlResolver, load style sheet with document function, should resolve during transform")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory(Skip = "SQLBU Defect Tracking Bug 430834: Skipping when input type is URI, see bug for more details.")]
        public void XmlResolver3(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            // "xmlResolver_document_function.xsl" contains
            // <xsl:for-each select="document('xmlResolver_document_function.xml')//elem">

            // SQLBU Defect Tracking Bug 430834: Skipping when input type is URI, see bug for more details.
            //if (MyInputType() == InputType.URI)
            //    return TEST_SKIPPED;

            if (LoadXSL("xmlResolver_document_function.xsl", inputType, readerType) == 1)
            {
                if ((Transform("fruits.xml", transformType, docType) == 1) && (CheckResult(377.8217373898, transformType) == 1))
                    return;
            }
            else
            {
                _output.WriteLine("Problem loading stylesheet with document function and default resolver!");
                Assert.True(false);
            }
            Assert.True(false);
        }

        //[Variation("document() has absolute URI")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Reader, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Writer, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory(Skip = "When style sheet URI = Intranet zone, XmlSecureResolver does not resolve document function")]
        public void XmlResolver5(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            // Skip this test for Load(URI)
            // Reason: When style sheet URI = Intranet zone, XmlSecureResolver does not resolve document function

            //if (MyInputType() == InputType.URI)
            //    return TEST_SKIPPED;

            // copy file on the local machine

            try
            {
                string tempDir = Path.GetTempPath();
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }
                string xmlFile = FullFilePath("xmlResolver_document_function.xml");
                File.Copy(xmlFile, Path.Combine(tempDir, "xmlResolver_document_function.xml"), true);
            }
            catch (Exception e)
            {
                _output.WriteLine(e.ToString());
                _output.WriteLine("Could not copy file to local. Some other issues prevented this test from running");
                return; //return TEST_SKIPPED;
            }

            if (LoadXSL("xmlResolver_document_function_absolute_uri.xsl", inputType, readerType) == 1)
            {
                if ((TransformResolver("fruits.xml", transformType, docType, new XmlUrlResolver()) == 1) && (CheckResult(377.8217373898, transformType) == 1))
                    return;
                else
                {
                    _output.WriteLine("Failed to resolve document function with absolute URI.");
                    Assert.True(false);
                }
            }
            else
            {
                _output.WriteLine("Failed to load style sheet!");
                Assert.True(false);
            }
        }
    }

    /***********************************************************/
    /*          XslTransform.Transform - (String, String)                    */
    /***********************************************************/

    //[TestCase(Name = "XsltTransform.Transform(String, String) : Reader , String", Desc = "READER,STREAM")]
    //[TestCase(Name = "XsltTransform.Transform(String, String) : URI, String", Desc = "URI,STREAM")]
    //[TestCase(Name = "XsltTransform.Transform(String, String) : Navigator, String", Desc = "NAVIGATOR,STREAM")]
    public class CTransformStrStrTest : XsltApiTestCaseBase
    {
        private ITestOutputHelper _output;
        public CTransformStrStrTest(ITestOutputHelper output) : base(output)
        {
            _output = output;
        }

        //[Variation("Basic Verification Test")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory]
        public void TransformStrStr1(InputType inputType, ReaderType readerType)
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><result>
		1.No Value Specified
		2.No Value Specified
		3.No Value Specified
		4.No Value Specified
		5.No Value Specified
		6.No Value Specified</result>";

            String szFullFilename = FullFilePath("fruits.xml");

            if (LoadXSL("showParam.xsl", inputType, readerType) == 1)
            {
                CallTransform(xslt, szFullFilename, _strOutFile);
                VerifyResult(expected);
                return;
            }
            Assert.True(false);
        }

        //[Variation("Input is null")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory]
        public void TransformStrStr2(InputType inputType, ReaderType readerType)
        {
            if (LoadXSL("showParam.xsl", inputType, readerType) == 1)
            {
                try
                {
                    CallTransform(xslt, null, _strOutFile);
                }
                catch (System.ArgumentException)
                { return; }
            }
            _output.WriteLine("Exception not generated for null input filename");
            Assert.True(false);
        }

        //[Variation("Output file is null")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory]
        public void TransformStrStr3(InputType inputType, ReaderType readerType)
        {
            String szFullFilename = FullFilePath("fruits.xml");

            if (LoadXSL("showParam.xsl", inputType, readerType) == 1)
            {
                try
                {
                    xslt.Transform(szFullFilename, null);
                }
                catch (System.ArgumentException)
                {
                    return;
                }
            }
            _output.WriteLine("Exception not generated for null output filename");
            Assert.True(false);
        }

        //[Variation("Input is nonexisting file")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory]
        public void TransformStrStr4(InputType inputType, ReaderType readerType)
        {
            if (LoadXSL("showParam.xsl", inputType, readerType) == 1)
            {
                try
                {
                    CallTransform(xslt, "IDontExist.xsl", _strOutFile);
                }
                catch (System.IO.FileNotFoundException)
                {
                    return;
                }
            }
            _output.WriteLine("Exception not generated for invalid input file");
            Assert.True(false);
        }

        //[Variation("Output file is invalid")]
        [PlatformSpecific(TestPlatforms.Windows)] //Output file name is valid on Unix
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory]
        public void TransformStrStr5(InputType inputType, ReaderType readerType)
        {
            String szFullFilename = FullFilePath("fruits.xml");

            if (LoadXSL("showParam.xsl", inputType, readerType) == 1)
            {
                try
                {
                    xslt.Transform(szFullFilename, szInvalid);
                }
                catch (System.ArgumentException)
                {
                    return;
                }
            }
            _output.WriteLine("Exception not generated for invalid output destination");
            Assert.True(false);
        }

        //[Variation("Input is empty string")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory]
        public void TransformStrStr6(InputType inputType, ReaderType readerType)
        {
            if (LoadXSL("showParam.xsl", inputType, readerType) == 1)
            {
                try
                {
                    CallTransform(xslt, szEmpty, _strOutFile);
                }
                catch (System.ArgumentException)
                {
                    return;
                }
            }
            _output.WriteLine("Exception not generated for empty string input file");
            Assert.True(false);
        }

        //[Variation("Output file is empty string")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory]
        public void TransformStrStr7(InputType inputType, ReaderType readerType)
        {
            String szFullFilename = FullFilePath("fruits.xml");

            if (LoadXSL("showParam.xsl", inputType, readerType) == 1)
            {
                try
                {
                    xslt.Transform(szFullFilename, szEmpty);
                }
                catch (System.ArgumentException)
                {
                    return;
                }
            }
            _output.WriteLine("Exception not generated for empty output file name");
            Assert.True(false);
        }

        //[Variation("Call Transform many times")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory]
        public void TransformStrStr8(InputType inputType, ReaderType readerType)
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><result>
		1.No Value Specified
		2.No Value Specified
		3.No Value Specified
		4.No Value Specified
		5.No Value Specified
		6.No Value Specified</result>";

            String szFullFilename = FullFilePath("fruits.xml");

            for (int i = 0; i < 50; i++)
            {
                if (LoadXSL("showParam.xsl", inputType, readerType) == 1)
                {
                    CallTransform(xslt, szFullFilename, _strOutFile);
                    try
                    {
                        VerifyResult(expected);
                    }
                    catch(Exception)
                    {
                        _output.WriteLine("Failed to process Load after calling {0} times", i);
                        throw;
                    }
                }
            }
            return;
        }

        //[Variation("Call without loading")]
        [InlineData()]
        [Theory]
        public void TransformStrStr9()
        {
#pragma warning disable 0618
            xslt = new XslTransform();
#pragma warning restore 0618
            try
            {
                CallTransform(xslt, FullFilePath("fruits.xml"), _strOutFile);
            }
            catch (System.InvalidOperationException e)
            {
                CheckExpectedError(e, "System.xml", "Xslt_NoStylesheetLoaded", new string[] { "" });
                return;
            }
            _output.WriteLine("Exception attempting a transform without loading an XSL file");
            Assert.True(false);
        }

        //[Variation("Output to unreachable destination")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory]
        public void TransformStrStr10(InputType inputType, ReaderType readerType)
        {
            if (LoadXSL("showParam.xsl", inputType, readerType) == 1)
            {
                try
                {
                    xslt.Transform(FullFilePath("fruits.xml"), "http://www.IdontExist.com/index.xml");
                }
                catch (System.Exception e)
                {
                    _output.WriteLine(e.ToString());
                    return;
                }
            }
            _output.WriteLine("Exception not generated for invalid output destination");
            Assert.True(false);
        }

        //[Variation("Input filename is \'.\', \'..\', and \'\\\\\'")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory]
        public void TransformStrStr11(InputType inputType, ReaderType readerType)
        {
            int iCount = 0;
            if (LoadXSL("showParam.xsl", inputType, readerType) == 1)
            {
                try
                {
                    CallTransform(xslt, "..", _strOutFile);
                }
                catch (System.Exception)
                {
                    iCount++;
                }

                try
                {
                    CallTransform(xslt, ".", _strOutFile);
                }
                catch (System.Exception)
                {
                    iCount++;
                }

                try
                {
                    CallTransform(xslt, "\\\\", _strOutFile);
                }
                catch (System.Exception)
                {
                    iCount++;
                }
            }

            if (iCount.Equals(3))
                return;

            _output.WriteLine("Exception not generated for invalid input sources");
            Assert.True(false);
        }

        //[Variation("Output filename is \'.\' and \'..\')]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory]
        public void TransformStrStr12(InputType inputType, ReaderType readerType)
        {
            String szFullFilename = FullFilePath("fruits.xml");
            int iCount = 0;
            if (LoadXSL("showParam.xsl", inputType, readerType) == 1)
            {
                try
                {
                    xslt.Transform(szFullFilename, "..");
                }
                catch (System.Exception)
                {
                    iCount++;
                }

                try
                {
                    xslt.Transform(szFullFilename, ".");
                }
                catch (System.Exception)
                {
                    iCount++;
                }

            }

            if (iCount.Equals(2))
                return;
            _output.WriteLine("Exception not generated for invalid ouput destinations");
            Assert.True(false);
        }

        //[Variation("Output filename is \'\\\\\'")]
        [PlatformSpecific(TestPlatforms.Windows)]  // Invalid path specific to Windows
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory]
        public void TransformStrStr12_win(InputType inputType, ReaderType readerType)
        {
            String szFullFilename = FullFilePath("fruits.xml");

            if (LoadXSL("showParam.xsl", inputType, readerType) == 1)
            {
                Assert.Throws<System.ArgumentException>(() => xslt.Transform(szFullFilename, "\\\\"));
                return;
            }

            _output.WriteLine("Exception not generated for invalid ouput destination");
            Assert.True(false);
        }

        //[Variation("Closing files after transform")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory]
        public void TransformStrStr13(InputType inputType, ReaderType readerType)
        {
            String szFullFilename = FullFilePath("fruits.xml");
            Stream strmTemp;

            if (LoadXSL("showParam.xsl", inputType, readerType) == 1)
            {
                CallTransform(xslt, szFullFilename, _strOutFile);

                // check if I can open and close the xml file
                File.ReadAllText(szFullFilename);

                strmTemp = new FileStream(szFullFilename, FileMode.Open, FileAccess.Read);
                strmTemp.Dispose();

                // check if I can open and close the output file
                File.ReadAllText(_strOutFile);

                strmTemp = new FileStream(_strOutFile, FileMode.Open, FileAccess.Read);
                strmTemp.Dispose();

                return;
            }
            Assert.True(false);
        }
    }

    /***********************************************************/
    /*          XslTransform.Transform - (String, String, Resolver)          */
    /***********************************************************/

    //[TestCase(Name = "XsltTransform.Transform(String, String, Resolver) : Reader , String", Desc = "READER,STREAM")]
    //[TestCase(Name = "XsltTransform.Transform(String, String, Resolver) : URI, String", Desc = "URI,STREAM")]
    //[TestCase(Name = "XsltTransform.Transform(String, String, Resolver) : Navigator, String", Desc = "NAVIGATOR,STREAM")]
    public class CTransformStrStrResolverTest : XsltApiTestCaseBase
    {
        private ITestOutputHelper _output;
        public CTransformStrStrResolverTest(ITestOutputHelper output) : base(output)
        {
            _output = output;
        }

        //[Variation("Pass null XmlResolver, load style sheet with import/include, should not affect transform")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory]
        public void TransformStrStrResolver1(InputType inputType, ReaderType readerType)
        {
            AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);

            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><result><fruit>Apple</fruit><fruit>orange</fruit></result>";
            String szFullFilename = FullFilePath("fruits.xml");

            try
            {
                if (LoadXSL("XmlResolver_Main.xsl", inputType, readerType) == 1)
                {
                    CallTransform(xslt, szFullFilename, "out.xml", null);
                    VerifyResult(expected);
                    return;
                }
            }
            catch (Exception e)
            {
                _output.WriteLine(e.ToString());
                Assert.True(false);
            }
            Assert.True(false);
        }

        //[Variation("Pass null XmlResolver, load style sheet with document function, should not resolve during transform")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader)]
        [Theory]
        public void TransformStrStrResolver2(InputType inputType, ReaderType readerType)
        {
            // "xmlResolver_document_function.xsl" contains
            // <xsl:for-each select="document('xmlResolver_document_function.xml')//elem">
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><result></result>";
            String szFullFilename = FullFilePath("fruits.xml");

            if (LoadXSL("xmlResolver_document_function.xsl", inputType, readerType) == 1)
            {
                CallTransform(xslt, szFullFilename, "out.xml", null);
                VerifyResult(expected);
                return;
            }
            else
            {
                _output.WriteLine("Problem loading stylesheet!");
                Assert.True(false);
            }
            Assert.True(false);
        }

        //[Variation("Pass XmlUrlResolver, load style sheet with document function, should resolve during transform")]
        [InlineData(InputType.Reader, ReaderType.XmlValidatingReader, TransformType.Stream)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.Stream)]
        [Theory(Skip = "SQLBU Defect Tracking Bug 430834: Skipping when input type is URI, see bug for more details.")]
        public void TransformStrStrResolver3(InputType inputType, ReaderType readerType, TransformType transformType)
        {
            // "xmlResolver_document_function.xsl" contains
            // <xsl:for-each select="document('xmlResolver_document_function.xml')//elem">

            // SQLBU Defect Tracking Bug 430834: Skipping when input type is URI, see bug for more details.
            //if (MyInputType() == InputType.URI)
            //    return TEST_SKIPPED;

            String szFullFilename = FullFilePath("fruits.xml");

            if (LoadXSL("xmlResolver_document_function.xsl", inputType, readerType) == 1)
            {
                CallTransform(xslt, szFullFilename, "out.xml", new XmlUrlResolver());
                if (CheckResult(377.8217373898, transformType) == 1)
                    return;
            }
            else
            {
                _output.WriteLine("Problem loading stylesheet with document function and default resolver!");
                Assert.True(false);
            }
            Assert.True(false);
        }
    }

    // This testcase is for bugs 109429, 111075 and 109644 fixed in Everett SP1
    //[TestCase(Name = "NDP1_1SP1 Bugs (URI,STREAM)", Desc = "URI,STREAM")]
    //[TestCase(Name = "NDP1_1SP1 Bugs (NAVIGATOR,TEXTWRITER)", Desc = "NAVIGATOR,TEXTWRITER")]
    public class CNDP1_1SP1Test : XsltApiTestCaseBase
    {
        private ITestOutputHelper _output;
        public CNDP1_1SP1Test(ITestOutputHelper output) : base(output)
        {
            _output = output;
        }

        //[Variation("Local parameter gets overwritten with global param value", Pri = 1)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void var1(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><out>
param1 (correct answer is 'local-param1-arg'): local-param1-arg
param2 (correct answer is 'local-param2-arg'): local-param2-arg
</out>";

            m_xsltArg = new XsltArgumentList();
            m_xsltArg.AddParam("param1", string.Empty, "global-param1-arg");

            if ((LoadXSL("paramScope.xsl", inputType, readerType) == 1) && (Transform_ArgList("fruits.xml", transformType, docType) == 1))
            {
                VerifyResult(expected);
                return;
            }
            else
                Assert.True(false);
        }

        //[Variation("Local parameter gets overwritten with global variable value", Pri = 1)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void var2(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?><out>
param1 (correct answer is 'local-param1-arg'): local-param1-arg
param2 (correct answer is 'local-param2-arg'): local-param2-arg
</out>";

            m_xsltArg = new XsltArgumentList();
            m_xsltArg.AddParam("param1", string.Empty, "global-param1-arg");

            if ((LoadXSL("varScope.xsl", inputType, readerType) == 1) && (Transform_ArgList("fruits.xml", transformType, docType) == 1))
            {
                VerifyResult(expected);
                return;
            }
            else
                Assert.True(false);
        }

        //[Variation("Subclassed XPathNodeIterator returned from an extension object or XsltFunction is not accepted by XPath", Pri = 1)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory(Skip = "SQLBU Defect Tracking Bug 430834: Skipping when input type is URI, see bug for more details.")]
        public void var3(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            // SQLBU Defect Tracking Bug 430834: Skipping when input type is URI, see bug for more details.
            //if (MyInputType() == InputType.URI)
            //    return TEST_SKIPPED;

            m_xsltArg = new XsltArgumentList();
            m_xsltArg.AddExtensionObject("http://foo.com", new MyXsltExtension());

            if ((LoadXSL("Bug111075.xsl", inputType, readerType) == 1) && (Transform_ArgList("Bug111075.xml", transformType, docType) == 1) && (CheckResult(444.7202431861, transformType) == 1))
                return;
            else
                Assert.True(false);
        }

        //[Variation("Iterator using for-each over a variable is not reset correctly while using msxsl:node-set()", Pri = 1)]
        [InlineData(InputType.URI, ReaderType.XmlValidatingReader, TransformType.Stream, DocType.XPathDocument)]
        [InlineData(InputType.Navigator, ReaderType.XmlValidatingReader, TransformType.TextWriter, DocType.XPathDocument)]
        [Theory]
        public void var4(InputType inputType, ReaderType readerType, TransformType transformType, DocType docType)
        {
            string expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
		Node Count: {3}

		
		Correct Output: (1)(2)(3)
		Incorrect Output: [1][2][3]";

            if ((LoadXSL("Bug109644.xsl", inputType, readerType) == 1) && (Transform("foo.xml", transformType, docType) == 1))
            {
                Assert.Equal(expected, File.ReadAllText("out.xml"));
                return;
            }
            else
                Assert.True(false);
        }
    }

    internal class MyArrayIterator : XPathNodeIterator
    {
        protected ArrayList array;
        protected int index;

        public MyArrayIterator(ArrayList array)
        {
            this.array = array;
            this.index = 0;
        }

        public MyArrayIterator(MyArrayIterator it)
        {
            this.array = it.array;
            this.index = it.index;
        }

        public override XPathNodeIterator Clone()
        {
            return new MyArrayIterator(this);
        }

        public override bool MoveNext()
        {
            if (index < array.Count)
            {
                index++;
                return true;
            }
            return false;
        }

        public override XPathNavigator Current
        {
            get
            {
                return (index > 0) ? (XPathNavigator)array[index - 1] : null;
            }
        }

        public override int CurrentPosition
        {
            get
            {
                return index;
            }
        }

        public override int Count
        {
            get
            {
                return array.Count;
            }
        }

        public void Reset()
        {
            index = 0;
        }

        // BUGBUG: DCR 104760
        public override IEnumerator GetEnumerator()
        {
            MyArrayIterator it = (MyArrayIterator)this.Clone();
            it.Reset();
            return (IEnumerator)it;
        }
    }

    internal class MyXsltExtension
    {
        public XPathNodeIterator distinct(XPathNodeIterator nodeset)
        {
            Hashtable nodelist = new Hashtable();
            while (nodeset.MoveNext())
            {
                if (!nodelist.Contains(nodeset.Current.Value))
                {
                    nodelist.Add(nodeset.Current.Value, nodeset.Current);
                }
            }
            return new MyArrayIterator(new ArrayList(nodelist.Values));
        }
    }
}
