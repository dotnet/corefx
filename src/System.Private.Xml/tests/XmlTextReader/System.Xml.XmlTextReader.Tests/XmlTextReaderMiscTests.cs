// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using Xunit;

namespace System.Xml.Tests
{
    public class XmlTextReaderMiscTests
    {
        [Fact]
        public void XmlTextReaderResetStateTest()
        {
            XmlTextReader textReader = 
                XmlTextReaderTestHelper.CreateReader(@"<List xmlns:ns='urn:NameSpace'><element1 ns:attr='val'>abc</element1></List>", new NameTable());            
            Assert.True(textReader.Read());
            Assert.Equal(ReadState.Interactive, textReader.ReadState);
            textReader.ResetState();
            Assert.Equal(ReadState.Initial, textReader.ReadState);
        }

        [Fact]
        public void XmlTextReaderGetRemainderTest()
        {
            string input = @"<List xmlns:ns='urn:NameSpace'><element1 ns:attr='val'></element1><element1 ns:attr='kal'></element1></List>";
            XmlTextReader textReader = XmlTextReaderTestHelper.CreateReader(input, new NameTable());
            Assert.True(textReader.Read());
            Assert.True(textReader.Read());
            Assert.True(textReader.Read());
            TextReader reader = textReader.GetRemainder();

            string expectedOutput = "<element1 ns:attr='kal'></element1></List>";
            var elems = new char[expectedOutput.Length];
            Assert.Equal(expectedOutput.Length, reader.Read(elems, 0, expectedOutput.Length));
            Assert.Equal(expectedOutput, new string(elems));
        }
    }
}
