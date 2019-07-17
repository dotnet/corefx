// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.

//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.IO;
using System.Xml;

using Xunit;

namespace System.Data.Tests
{
    public class DataSetWriteXmlTest
    {
        [Fact]
        public void WriteSimpleAuto()
        {
            string sampleXml = @"<NewDataSet>
                <xs:schema id='NewDataSet' xmlns='' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:msdata='urn:schemas-microsoft-com:xml-msdata'>
                    <xs:element name='NewDataSet' msdata:IsDataSet='true' msdata:UseCurrentLocale='true'>
                        <xs:complexType>
                        <xs:choice minOccurs='0' maxOccurs='unbounded'>
                            <xs:element name='Table'>
                            <xs:complexType>
                                <xs:sequence>
                                <xs:element name='ServerName' msdata:DataType='System.Object, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e' type='xs:anyType' minOccurs='0' />
                                </xs:sequence>
                            </xs:complexType>
                            </xs:element>
                        </xs:choice>
                        </xs:complexType>
                    </xs:element>
                </xs:schema>
                <Table>
                    <ServerName xsi:type='xs:string' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>MACHINENAME</ServerName>
                </Table>
            </NewDataSet>";
            DataSet ds = new DataSet();
            System.IO.StringReader xmlSR = new System.IO.StringReader(sampleXml);

            ds.ReadXml(xmlSR, XmlReadMode.ReadSchema);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            using (System.Xml.XmlWriter xw = System.Xml.XmlWriter.Create(sb, new System.Xml.XmlWriterSettings() { }))
            {
                ds.WriteXml(xw, System.Data.XmlWriteMode.WriteSchema);
            }
            Assert.Equal(@"ServerName", ds.Tables[0].Columns[0].ColumnName);
            Assert.Equal(@"MACHINENAME", ds.Tables[0].Rows[0].Field<string>(ds.Tables[0].Columns[0]));

            using (System.IO.StringWriter sw = new System.IO.StringWriter(sb))
            {
                ds.WriteXml(sw, System.Data.XmlWriteMode.WriteSchema);
            }
            Assert.Equal(@"ServerName", ds.Tables[0].Columns[0].ColumnName);
            Assert.Equal(@"MACHINENAME", ds.Tables[0].Rows[0].Field<string>(ds.Tables[0].Columns[0]));
        }
    }
}
