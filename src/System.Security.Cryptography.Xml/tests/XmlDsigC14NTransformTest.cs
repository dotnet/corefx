//
// XmlDsigC14NTransformTest.cs - Test Cases for XmlDsigC14NTransform
//
// Author:
//	Sebastien Pouliot <sebastien@ximian.com>
//	Aleksey Sanin (aleksey@aleksey.com)
//
// (C) 2002, 2003 Motus Technologies Inc. (http://www.motus.com)
// (C) 2003 Aleksey Sanin (aleksey@aleksey.com)
// (C) 2004 Novell (http://www.novell.com)
//
// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Text;
using System.Xml;
using Xunit;

namespace System.Security.Cryptography.Xml.Tests
{

    // Note: GetInnerXml is protected in XmlDsigC14NTransform making it
    // difficult to test properly. This class "open it up" :-)
    public class UnprotectedXmlDsigC14NTransform : XmlDsigC14NTransform
    {
        public UnprotectedXmlDsigC14NTransform()
        {
        }

        public UnprotectedXmlDsigC14NTransform(bool includeComments)
            : base(includeComments)
        {
        }

        public XmlNodeList UnprotectedGetInnerXml()
        {
            return base.GetInnerXml();
        }
    }

    public class XmlDsigC14NTransformTest
    {

        protected UnprotectedXmlDsigC14NTransform transform;

        public XmlDsigC14NTransformTest()
        {
            transform = new UnprotectedXmlDsigC14NTransform();
        }

        [Fact] // ctor ()
        public void Constructor1()
        {
            Assert.Equal("http://www.w3.org/TR/2001/REC-xml-c14n-20010315", transform.Algorithm);
            CheckProperties(transform);
        }

        [Fact] // ctor (Boolean)
        public void Constructor2()
        {
            transform = new UnprotectedXmlDsigC14NTransform(true);
            Assert.Equal("http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments", transform.Algorithm);
            CheckProperties(transform);

            transform = new UnprotectedXmlDsigC14NTransform(false);
            Assert.Equal("http://www.w3.org/TR/2001/REC-xml-c14n-20010315", transform.Algorithm);
            CheckProperties(transform);
        }

        void CheckProperties(XmlDsigC14NTransform transform)
        {
            Type[] input = transform.InputTypes;
            Assert.True((input.Length == 3), "Input #");
            // check presence of every supported input types
            bool istream = false;
            bool ixmldoc = false;
            bool ixmlnl = false;
            foreach (Type t in input)
            {
                if (t.ToString() == "System.IO.Stream")
                    istream = true;
                if (t.ToString() == "System.Xml.XmlDocument")
                    ixmldoc = true;
                if (t.ToString() == "System.Xml.XmlNodeList")
                    ixmlnl = true;
            }
            Assert.True(istream, "Input Stream");
            Assert.True(ixmldoc, "Input XmlDocument");
            Assert.True(ixmlnl, "Input XmlNodeList");

            Type[] output = transform.OutputTypes;
            Assert.True((output.Length == 1), "Output #");
            // check presence of every supported output types
            bool ostream = false;
            foreach (Type t in output)
            {
                if (t.ToString() == "System.IO.Stream")
                    ostream = true;
            }
            Assert.True(ostream, "Output Stream");
        }

        [Fact]
        public void GetInnerXml()
        {
            XmlNodeList xnl = transform.UnprotectedGetInnerXml();
            Assert.Null(xnl);
        }

        private string Stream2String(Stream s)
        {
            StringBuilder sb = new StringBuilder();
            int b = s.ReadByte();
            while (b != -1)
            {
                sb.Append(Convert.ToChar(b));
                b = s.ReadByte();
            }
            return sb.ToString();
        }

        static string xml = "<Test  attrib='at ' xmlns=\"http://www.go-mono.com/\" > \r\n &#xD; <Toto/> text &amp; </Test   >";
        // GOOD for Stream input
        static string c14xml2 = "<Test xmlns=\"http://www.go-mono.com/\" attrib=\"at \"> \n &#xD; <Toto></Toto> text &amp; </Test>";
        // GOOD for XmlDocument input. The difference is because once
        // xml string is loaded to XmlDocument, there is no difference
        // between \r and &#xD;, so every \r must be handled as &#xD;.
        static string c14xml3 = "<Test xmlns=\"http://www.go-mono.com/\" attrib=\"at \"> &#xD;\n &#xD; <Toto></Toto> text &amp; </Test>";

        private XmlDocument GetDoc()
        {
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.LoadXml(xml);
            return doc;
        }

        [Fact]
        public void LoadInputAsXmlDocument()
        {
            XmlDocument doc = GetDoc();
            transform.LoadInput(doc);
            Stream s = (Stream)transform.GetOutput();
            string output = Stream2String(s);
            Assert.Equal(c14xml3, output);
        }

        [Fact(Skip = "TODO: fix me")]
        // see LoadInputAsXmlNodeList2 description
        public void LoadInputAsXmlNodeList()
        {
            XmlDocument doc = GetDoc();
            // Argument list just contains element Test.
            transform.LoadInput(doc.ChildNodes);
            Stream s = (Stream)transform.GetOutput();
            string output = Stream2String(s);
            Assert.Equal("<Test></Test>", output);
        }

        [Fact(Skip = "TODO: fix me")]
        // MS has a bug that those namespace declaration nodes in
        // the node-set are written to output. Related spec section is:
        // http://www.w3.org/TR/2001/REC-xml-c14n-20010315#ProcessingModel
        public void LoadInputAsXmlNodeList2()
        {
            XmlDocument doc = GetDoc();
            transform.LoadInput(doc.SelectNodes("//*"));
            Stream s = (Stream)transform.GetOutput();
            string output = Stream2String(s);
            string expected = @"<Test><Toto></Toto></Test>";
            Assert.Equal(expected, output);
        }

        [Fact]
        public void LoadInputAsStream()
        {
            MemoryStream ms = new MemoryStream();
            byte[] x = Encoding.ASCII.GetBytes(xml);
            ms.Write(x, 0, x.Length);
            ms.Position = 0;
            transform.LoadInput(ms);
            Stream s = (Stream)transform.GetOutput();
            string output = Stream2String(s);
            Assert.Equal(c14xml2, output);
        }

        [Fact]
        public void LoadInputWithUnsupportedType()
        {
            byte[] bad = { 0xBA, 0xD };
            Assert.Throws<ArgumentException>(() => transform.LoadInput(bad));
        }

        [Fact]
        public void UnsupportedOutput()
        {
            XmlDocument doc = new XmlDocument();
            Assert.Throws<ArgumentException>(() => transform.GetOutput(doc.GetType()));
        }

        [Fact]
        public void C14NSpecExample1()
        {
            using (TestHelpers.CreateTestDtdFile(GetType().Name + "." + nameof(C14NSpecExample1)))
            {
                string res = ExecuteXmlDSigC14NTransform(C14NSpecExample1Input);
                Assert.Equal(C14NSpecExample1Output, res);
            }
        }

        [Fact]
        public void C14NSpecExample2()
        {
            string res = ExecuteXmlDSigC14NTransform(C14NSpecExample2Input);
            Assert.Equal(C14NSpecExample2Output, res);
        }

        [Fact]
        public void C14NSpecExample3()
        {
            string res = ExecuteXmlDSigC14NTransform(C14NSpecExample3Input);
            Assert.Equal(C14NSpecExample3Output, res);
        }

        [Fact]
        //		[Ignore ("This test should be fine, but it does not pass under MS.NET")]
        public void C14NSpecExample4()
        {
            string res = ExecuteXmlDSigC14NTransform(C14NSpecExample4Input);
            Assert.Equal(C14NSpecExample4Output, res);
        }

        [Fact(Skip = "TODO: fix me")]
        public void C14NSpecExample5()
        {
            string testName = GetType().Name + "." + nameof(C14NSpecExample5);
            using (TestHelpers.CreateTestTextFile(testName, "world"))
            {
                string res = ExecuteXmlDSigC14NTransform(C14NSpecExample5Input(testName));
                Assert.Equal(C14NSpecExample5Output, res);
            }
        }

        [Fact]
        public void C14NSpecExample6()
        {
            string res = ExecuteXmlDSigC14NTransform(C14NSpecExample6Input);
            Assert.Equal(C14NSpecExample6Output, res);
        }

        private string ExecuteXmlDSigC14NTransform(string InputXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.LoadXml(InputXml);

            // Testing default attribute support with
            // vreader.ValidationType = ValidationType.None.
            //
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] data = utf8.GetBytes(InputXml.ToString());
            Stream stream = new MemoryStream(data);
            using (XmlReader reader = XmlReader.Create(stream, new XmlReaderSettings { ValidationType = ValidationType.None, DtdProcessing = DtdProcessing.Parse }))
            {
                doc.Load(reader);

                transform.LoadInput(doc);
                return Stream2String((Stream)transform.GetOutput());
            }
        }

        //
        // Example 1 from C14N spec - PIs, Comments, and Outside of Document Element: 
        // http://www.w3.org/TR/xml-c14n#Example-OutsideDoc
        // 
        // Aleksey: 
        // removed reference to an empty external DTD
        //
        static string C14NSpecExample1Input =
                "<?xml version=\"1.0\"?>\n" +
                "\n" +
                "<?xml-stylesheet   href=\"doc.xsl\"\n" +
                "   type=\"text/xsl\"   ?>\n" +
                "\n" +
                // "<!DOCTYPE doc SYSTEM \"doc.dtd\">\n" +
                "\n" +
                "<doc>Hello, world!<!-- Comment 1 --></doc>\n" +
                "\n" +
                "<?pi-without-data     ?>\n\n" +
                "<!-- Comment 2 -->\n\n" +
                "<!-- Comment 3 -->\n";
        static string C14NSpecExample1Output =
                "<?xml-stylesheet href=\"doc.xsl\"\n" +
                "   type=\"text/xsl\"   ?>\n" +
                "<doc>Hello, world!</doc>\n" +
                "<?pi-without-data?>";

        //
        // Example 2 from C14N spec - Whitespace in Document Content: 
        // http://www.w3.org/TR/xml-c14n#Example-WhitespaceInContent
        // 
        static string C14NSpecExample2Input =
                "<doc>\n" +
                "  <clean>   </clean>\n" +
                "   <dirty>   A   B   </dirty>\n" +
                "   <mixed>\n" +
                "      A\n" +
                "      <clean>   </clean>\n" +
                "      B\n" +
                "      <dirty>   A   B   </dirty>\n" +
                "      C\n" +
                "   </mixed>\n" +
                "</doc>\n";
        static string C14NSpecExample2Output =
                "<doc>\n" +
                "  <clean>   </clean>\n" +
                "   <dirty>   A   B   </dirty>\n" +
                "   <mixed>\n" +
                "      A\n" +
                "      <clean>   </clean>\n" +
                "      B\n" +
                "      <dirty>   A   B   </dirty>\n" +
                "      C\n" +
                "   </mixed>\n" +
                "</doc>";

        //
        // Example 3 from C14N spec - Start and End Tags: 
        // http://www.w3.org/TR/xml-c14n#Example-SETags
        //
        static string C14NSpecExample3Input =
                "<!DOCTYPE doc [<!ATTLIST e9 attr CDATA \"default\">]>\n" +
                "<doc>\n" +
                "   <e1   />\n" +
                "   <e2   ></e2>\n" +
                "   <e3    name = \"elem3\"   id=\"elem3\"    />\n" +
                "   <e4    name=\"elem4\"   id=\"elem4\"    ></e4>\n" +
                "   <e5 a:attr=\"out\" b:attr=\"sorted\" attr2=\"all\" attr=\"I\'m\"\n" +
                "       xmlns:b=\"http://www.ietf.org\" \n" +
                "       xmlns:a=\"http://www.w3.org\"\n" +
                "       xmlns=\"http://www.uvic.ca\"/>\n" +
                "   <e6 xmlns=\"\" xmlns:a=\"http://www.w3.org\">\n" +
                "       <e7 xmlns=\"http://www.ietf.org\">\n" +
                "           <e8 xmlns=\"\" xmlns:a=\"http://www.w3.org\">\n" +
                "               <e9 xmlns=\"\" xmlns:a=\"http://www.ietf.org\"/>\n" +
                "           </e8>\n" +
                "       </e7>\n" +
                "   </e6>\n" +
                "</doc>\n";
        static string C14NSpecExample3Output =
                "<doc>\n" +
                "   <e1></e1>\n" +
                "   <e2></e2>\n" +
                "   <e3 id=\"elem3\" name=\"elem3\"></e3>\n" +
                "   <e4 id=\"elem4\" name=\"elem4\"></e4>\n" +
                "   <e5 xmlns=\"http://www.uvic.ca\" xmlns:a=\"http://www.w3.org\" xmlns:b=\"http://www.ietf.org\" attr=\"I\'m\" attr2=\"all\" b:attr=\"sorted\" a:attr=\"out\"></e5>\n" +
                    "   <e6 xmlns:a=\"http://www.w3.org\">\n" +
                "       <e7 xmlns=\"http://www.ietf.org\">\n" +
                "           <e8 xmlns=\"\">\n" +
                "               <e9 xmlns:a=\"http://www.ietf.org\" attr=\"default\"></e9>\n" +
                //	    	        "               <e9 xmlns:a=\"http://www.ietf.org\"></e9>\n" +
                "           </e8>\n" +
                "       </e7>\n" +
                "   </e6>\n" +
                "</doc>";


        //
        // Example 4 from C14N spec - Character Modifications and Character References: 
        // http://www.w3.org/TR/xml-c14n#Example-Chars
        //
        // Aleksey: 
        // This test does not include "normId" element
        // because it has an invalid ID attribute "id" which
        // should be normalized by XML parser. Currently Mono
        // does not support this (see comment after this example
        // in the spec).
        static string C14NSpecExample4Input =
                "<!DOCTYPE doc [<!ATTLIST normId id ID #IMPLIED>]>\n" +
                "<doc>\n" +
                "   <text>First line&#x0d;&#10;Second line</text>\n" +
                "   <value>&#x32;</value>\n" +
                "   <compute><![CDATA[value>\"0\" && value<\"10\" ?\"valid\":\"error\"]]></compute>\n" +
                "   <compute expr=\'value>\"0\" &amp;&amp; value&lt;\"10\" ?\"valid\":\"error\"\'>valid</compute>\n" +
                "   <norm attr=\' &apos;   &#x20;&#13;&#xa;&#9;   &apos; \'/>\n" +
                // "   <normId id=\' &apos;   &#x20;&#13;&#xa;&#9;   &apos; \'/>\n" +
                "</doc>\n";
        static string C14NSpecExample4Output =
                "<doc>\n" +
                "   <text>First line&#xD;\n" +
                "Second line</text>\n" +
                "   <value>2</value>\n" +
                "   <compute>value&gt;\"0\" &amp;&amp; value&lt;\"10\" ?\"valid\":\"error\"</compute>\n" +
                "   <compute expr=\"value>&quot;0&quot; &amp;&amp; value&lt;&quot;10&quot; ?&quot;valid&quot;:&quot;error&quot;\">valid</compute>\n" +
                "   <norm attr=\" \'    &#xD;&#xA;&#x9;   \' \"></norm>\n" +
                // "   <normId id=\"\' &#xD;&#xA;&#x9; \'\"></normId>\n" +
                "</doc>";

        //
        // Example 5 from C14N spec - Entity References: 
        // http://www.w3.org/TR/xml-c14n#Example-Entities
        //
        static string C14NSpecExample5Input(string worldName) =>
                "<!DOCTYPE doc [\n" +
                "<!ATTLIST doc attrExtEnt ENTITY #IMPLIED>\n" +
                "<!ENTITY ent1 \"Hello\">\n" +
                $"<!ENTITY ent2 SYSTEM \"{worldName}.txt\">\n" +
                "<!ENTITY entExt SYSTEM \"earth.gif\" NDATA gif>\n" +
                "<!NOTATION gif SYSTEM \"viewgif.exe\">\n" +
                "]>\n" +
                "<doc attrExtEnt=\"entExt\">\n" +
                "   &ent1;, &ent2;!\n" +
                "</doc>\n" +
                "\n" +
                "<!-- Let world.txt contain \"world\" (excluding the quotes) -->\n";

        static string C14NSpecExample5Output =
                "<doc attrExtEnt=\"entExt\">\n" +
                "   Hello, world!\n" +
                "</doc>";

        //
        // Example 6 from C14N spec - UTF-8 Encoding: 
        // http://www.w3.org/TR/xml-c14n#Example-UTF8
        // 
        static string C14NSpecExample6Input =
                    "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>\n" +
                    "<doc>&#169;</doc>\n";
        static string C14NSpecExample6Output =
                "<doc>\xC2\xA9</doc>";

        [Fact]
        public void SimpleNamespacePrefixes()
        {
            string input = "<a:Action xmlns:a='urn:foo'>http://tempuri.org/IFoo/Echo</a:Action>";
            string expected = @"<a:Action xmlns:a=""urn:foo"">http://tempuri.org/IFoo/Echo</a:Action>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(input);
            XmlDsigC14NTransform t = new XmlDsigC14NTransform();
            t.LoadInput(doc);
            Stream s = t.GetOutput() as Stream;
            Assert.Equal(new StreamReader(s, Encoding.UTF8).ReadToEnd(), expected);
        }

        [Fact]
        public void OrdinalSortForAttributes()
        {
            XmlDocument doc = new XmlDocument();
            string xml = "<foo Aa=\"one\" Bb=\"two\" aa=\"three\" bb=\"four\"><bar></bar></foo>";
            doc.LoadXml(xml);

            transform.LoadInput(doc);
            Stream s = (Stream)transform.GetOutput();
            string output = Stream2String(s);
            Assert.Equal(xml, output);
        }

        [Fact(Skip = "TODO: fix me")]
        public void PrefixlessNamespaceOutput()
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement("foo", "urn:foo"));
            doc.DocumentElement.AppendChild(doc.CreateElement("bar", "urn:bar"));
            Assert.Equal(String.Empty, doc.DocumentElement.GetAttribute("xmlns"));
            XmlDsigC14NTransform t = new XmlDsigC14NTransform();
            t.LoadInput(doc);
            Stream s = t.GetOutput() as Stream;
            Assert.Equal(new StreamReader(s, Encoding.UTF8).ReadToEnd(), "<foo xmlns=\"urn:foo\"><bar xmlns=\"urn:bar\"></bar></foo>");
            Assert.Equal("urn:foo", doc.DocumentElement.GetAttribute("xmlns"));
        }

        [Fact(Skip = "TODO: fix me")]
        public void PropagatedNamespaces()
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement("foo", "urn:foo"));
            doc.DocumentElement.AppendChild(doc.CreateElement("bar", "urn:bar"));
            Assert.Equal(String.Empty, doc.DocumentElement.GetAttribute("xmlns:f"));
            XmlDsigExcC14NTransform t = new XmlDsigExcC14NTransform();
            t.LoadInput(doc);
            t.PropagatedNamespaces.Add("f", "urn:foo");
            t.PropagatedNamespaces.Add("f", "urn:foo");
            t.PropagatedNamespaces.Add("f", "urn:foo");
            t.PropagatedNamespaces.Add("b", "urn:bar");
            Stream s = t.GetOutput() as Stream;
            Assert.Equal(new StreamReader(s, Encoding.UTF8).ReadToEnd(), "<f:foo xmlns:f=\"urn:foo\"><b:bar xmlns:b=\"urn:bar\"></b:bar></f:foo>");
            Assert.Equal("urn:foo", doc.DocumentElement.GetAttribute("xmlns:f"));
        }

        [Fact]
        public void GetDigestedOutput_Null()
        {
            Assert.Throws< NullReferenceException>(() => new XmlDsigExcC14NTransform().GetDigestedOutput(null));
        }
    }
}
