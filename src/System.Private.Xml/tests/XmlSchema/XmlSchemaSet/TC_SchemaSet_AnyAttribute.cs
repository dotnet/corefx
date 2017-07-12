﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using Xunit.Abstractions;
using System.IO;
using System.Xml.Schema;

namespace System.Xml.Tests
{
    public class TC_SchemaSet_AnyAttribute : TC_SchemaSetBase
    {
        private readonly ITestOutputHelper _output;
        private int _errorCount;
        private int _warningCount;
        private bool _warningInnerExceptionSet;
        private bool _errorInnerExceptionSet;

        public TC_SchemaSet_AnyAttribute(ITestOutputHelper output)
        {
            _output = output;
        }

        //hook up validaton callback
        public void ValidationCallback(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
            {
                _output.WriteLine("WARNING: ");
                _warningCount++;
                _warningInnerExceptionSet = (args.Exception.InnerException != null);
                _output.WriteLine("\nInnerExceptionSet : " + _warningInnerExceptionSet + "\n");
            }
            else if (args.Severity == XmlSeverityType.Error)
            {
                _output.WriteLine("ERROR: ");
                _errorCount++;
                _errorInnerExceptionSet = (args.Exception.InnerException != null);
                _output.WriteLine("\nInnerExceptionSet : " + _errorInnerExceptionSet + "\n");
            }

            _output.WriteLine(args.Message);
        }

        public XmlSchema GetUnionSchema(string ns1, string ns2, string attrNs)
        {
            var xsd = @"<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='" + attrNs + @"' xmlns='" + attrNs + @"'>
                        <xs:attribute name='baseattribute'>
                          <xs:simpleType >
                          <xs:restriction base = 'xs:int' >
                          <xs:maxInclusive value = '1000' />
                          </xs:restriction >
                        </xs:simpleType >
                        </xs:attribute >
                        <xs:complexType name = 't'>
                            <xs:sequence >
                            <xs:element name = 'e1' type = 'xs:string' />
                            </xs:sequence >
                            <xs:anyAttribute namespace='" + ns2 + @"'/>
                        </xs:complexType>
                        <xs:complexType name = 't1'>
                          <xs:complexContent>
                           <xs:extension base='t'>
                            <xs:sequence >
                             <xs:element name = 'e2' type = 'xs:string' />
                            </xs:sequence>
                            <xs:anyAttribute namespace='" + ns1 + @"'/>
                           </xs:extension>
                         </xs:complexContent>
                        </xs:complexType>
                        <xs:complexType name = 't2'>
                           <xs:complexContent>
                            <xs:restriction base='t1'>
                            <xs:sequence >
                            <xs:element name = 'e1' type = 'xs:string' fixed='name'/>
                            <xs:element name = 'e2' type = 'xs:string' fixed='name'/>
                            </xs:sequence >
                            <xs:attribute ref='baseattribute'/>
                            </xs:restriction>
                            </xs:complexContent>
                        </xs:complexType>
                        </xs:schema>";

            return XmlSchema.Read(new StringReader(xsd), null);
        }

        public XmlSchema GetIntersectionSchema(string ns1, string ns2, string attrNs)
        {
            var xsd = @"<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='" + attrNs + @"' xmlns='" + attrNs + @"'>
                        <xs:attribute name='baseattribute'>
                          <xs:simpleType >
                          <xs:restriction base = 'xs:int' >
                          <xs:maxInclusive value = '1000' />
                          </xs:restriction >
                        </xs:simpleType >
                        </xs:attribute >
                        <xs:attributeGroup name = 'attr-gr' >
                            <xs:attribute name = 'a1' type = 'xs:int' />
                            <xs:anyAttribute namespace='" + ns2 + @"'/>
                        </xs:attributeGroup >
                        <xs:complexType name = 't'>
                            <xs:sequence >
                            <xs:element name = 'e1' type = 'xs:string' />
                            </xs:sequence >
                            <xs:attributeGroup ref='attr-gr' />
                            <xs:anyAttribute namespace='" + ns1 + @"'/>
                        </xs:complexType>
                        <xs:complexType name = 't1'>
                           <xs:complexContent>
                            <xs:restriction base='t'>
                            <xs:sequence >
                            <xs:element name = 'e1' type = 'xs:string' fixed='name'/>
                            </xs:sequence >
                            <xs:attribute ref='baseattribute'/>
                            </xs:restriction>
                            </xs:complexContent>
                        </xs:complexType>
                        </xs:schema>";

            return XmlSchema.Read(new StringReader(xsd), null);
        }

        //Intersection namespaces
        [Theory]
        //[Variation(Desc = "complextype Any ns - ##any, attrgroup Any ns2, allow ns2 attribute")]
        [InlineData("##any", "ns2", "ns2", 0)]
        //[Variation(Desc = "complextype Any ns - ##any, attrgroup Any ns2, not allow ns1 attribute")]
        [InlineData("##any", "ns2", "ns1", 1)]
        //[Variation(Desc = "complextype Any ns - ns2, attrgroup Any ##any, allow ns2 attribute")]
        [InlineData("ns2", "##any", "ns2", 0)]
        //[Variation(Desc = "complextype Any ns - ns2, attrgroup Any ##any, not allow ns1 attribute")]
        [InlineData("ns2", "##any", "ns1", 1)]
        //[Variation(Desc = "complextype Any ns - ns1 ns2, attrgroup Any ##other, not allow ns1 attribute")]
        [InlineData("ns1 ns2", "##other", "ns1", 1)]
        //[Variation(Desc = "complextype Any ns - ns1 ns2, attrgroup Any ##other, not allow ns2 attribute")]
        [InlineData("ns1 ns2", "##other", "ns2", 1)]
        //[Variation(Desc = "complextype Any ns - ns1 ns2, attrgroup Any #other, not allow ns3 attribute")]
        [InlineData("ns1 ns2", "##other", "ns3", 1)]
        //[Variation(Desc = "complextype Any ns - ##other, attrgroup Any ns1 ns2, not allow ns1 attribute")]
        [InlineData("##other", "ns1 ns2", "ns1", 1)]
        //[Variation(Desc = "complextype Any ns - ##other, attrgroup Any ns1 ns2, not allow ns2 attribute")]
        [InlineData("##other", "ns1 ns2", "ns2", 1)]
        //[Variation(Desc = "complextype Any ns - ##other, attrgroup Any ns1 ns2, not allow ns3 attribute")]
        [InlineData("##other", "ns1 ns2", "ns3", 1)]
        //[Variation(Desc = "complextype Any ns - ns1 ns3, attrgroup Any ns1 ns2, not allow ns3 attribute")]
        [InlineData("ns1 ns3", "ns1 ns2", "ns3", 1)]
        //[Variation(Desc = "complextype Any ns - ns1 ns3, attrgroup Any ns1 ns2, not allow ns2 attribute")]
        [InlineData("ns1 ns3", "ns1 ns2", "ns2", 1)]
        //[Variation(Desc = "complextype Any ns - ns1 ns3, attrgroup Any ns1 ns2, allow ns1 attribute")]
        [InlineData("ns1 ns3", "ns1 ns2", "ns1", 0)]
        //[Variation(Desc = "complextype Any ns - ##other, attrgroup Any ##other, not allow ns1 attribute")]
        [InlineData("##other", "##other", "ns1", 1)]
        public void v1(string ns1, string ns2, string attrNs, int expectedError)
        {
            XmlSchemaSet xss = new XmlSchemaSet();
            xss.XmlResolver = new XmlUrlResolver();
            xss.ValidationEventHandler += new ValidationEventHandler(ValidationCallback);
            xss.Add(GetIntersectionSchema(ns1, ns2, attrNs));
            xss.Compile();

            Assert.Equal(expectedError, _errorCount);
        }

        [Theory]
        //[Variation(Desc = "basetype Any ns - ##any, derivedType Any ns - ns1, allow ns2 attribute")]
        [InlineData("##any", "ns1", "ns2", 0)]
        //[Variation(Desc = "basetype Any ns - ns1, derivedType Any ns - ##any, allow ns2 attribute")]
        [InlineData("ns1", "##any", "ns2", 0)]
        //[Variation(Desc = "basetype Any ns - ns1 ns2, derivedType Any ns - ns2 ns3 , allow ns3 attribute")]
        [InlineData("ns1 ns2", "ns2 ns3", "ns3", 0)]
        //[Variation(Desc = "basetype Any ns - ##other, derivedType Any ns - ##other , not allow current ns")]
        [InlineData("##other", "##other", "ns1", 1)]
        //[Variation(Desc = "basetype Any ns - ns1 ns2, derivedType Any ns - ##other , allow ns1")]
        [InlineData("ns1 ns2", "##other", "ns1", 0)]
        //[Variation(Desc = "basetype Any ns - ns1 ns2, derivedType Any ns - ##other , allow ns2")]
        [InlineData("ns1 ns2", "##other", "ns2", 0)]
        //[Variation(Desc = "basetype Any ns - ##other, derivedType Any ns - ns1 ns2 , allow ns2")]
        [InlineData("##other", "ns1 ns2", "ns1", 0)]
        //[Variation(Desc = "basetype Any ns - ##other, derivedType Any ns - ns1 ns2 , allow ns2")]
        [InlineData("##other", "ns1 ns2", "ns2", 0)]
        public void v2(string ns1, string ns2, string attrNs, int expectedError)
        {
            XmlSchemaSet xss = new XmlSchemaSet();
            xss.XmlResolver = new XmlUrlResolver();
            xss.ValidationEventHandler += new ValidationEventHandler(ValidationCallback);
            xss.Add(GetUnionSchema(ns1, ns2, attrNs));
            xss.Compile();

            Assert.Equal(expectedError, _errorCount);
        }
    }
}
