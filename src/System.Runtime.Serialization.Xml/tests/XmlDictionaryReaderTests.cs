// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Xunit;

namespace System.Runtime.Serialization.Xml.Tests
{
    public static class XmlDictionaryReaderTests
    {
        [Fact]
        public static void ReadValueChunkReadEncodedDoubleWideChars()
        {
            // The test is to verify the fix made for the following issue:
            // When reading value chunk from XmlReader where Encoding.UTF8 is used, and where the
            // encoded bytes contains 4-byte UTF-8 encoded characters: if the 4 byte character is decoded 
            // into 2 chars and the char[] only has one space left, an ArgumentException will be thrown
            // stating that there is not enough space to decode the bytes.
            string xmlPayloadHolder = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/""><s:Body><Response xmlns=""http://tempuri.org/""><Result>{0}</Result></Response></s:Body></s:Envelope>";
            int startWideChars = 0;
            int endWideChars = 128;
            int incrementWideChars = 1;

            for (int wideChars = startWideChars; wideChars < endWideChars; wideChars += incrementWideChars)
            {
                for (int singleByteChars = 0; singleByteChars < 4; singleByteChars++)
                {
                    string testString = GenerateDoubleWideTestString(wideChars, singleByteChars);
                    string returnedString;
                    string xmlContent = string.Format(xmlPayloadHolder, testString);
                    using (Stream stream = GenerateStreamFromString(xmlContent))
                    {
                        var encoding = Encoding.UTF8;
                        var quotas = new XmlDictionaryReaderQuotas();
                        XmlReader reader = XmlDictionaryReader.CreateTextReader(stream, encoding, quotas, null);

                        reader.ReadStartElement(); // <s:Envelope>
                        reader.ReadStartElement(); // <s:Body>
                        reader.ReadStartElement(); // <Response>
                        reader.ReadStartElement(); // <Result>

                        Assert.True(reader.CanReadValueChunk, "reader.CanReadValueChunk is expected to be true, but it returned false.");

                        var resultChars = new List<char>();
                        var buffer = new char[256];
                        int count = 0;
                        while ((count = reader.ReadValueChunk(buffer, 0, buffer.Length)) > 0)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                resultChars.Add(buffer[i]);
                            }
                        }

                        returnedString = new string(resultChars.ToArray());
                    }

                    Assert.StrictEqual(testString, returnedString);
                }
            }
        }

        [Fact]
        public static void ReadElementContentAsStringDataExceedsMaxBytesPerReadQuota()
        {
            XmlDictionaryReaderQuotas quotas = new XmlDictionaryReaderQuotas();
            quotas.MaxBytesPerRead = 4096;
            int contentLength = 8176;

            string testString = new string('a', contentLength);
            string returnedString;
            XmlDictionary dict = new XmlDictionary();
            XmlDictionaryString dictEntry = dict.Add("Value");

            using (var ms = new MemoryStream())
            {
                XmlDictionaryWriter xmlWriter = XmlDictionaryWriter.CreateBinaryWriter(ms, dict);
                xmlWriter.WriteElementString(dictEntry, XmlDictionaryString.Empty, testString);
                xmlWriter.Flush();
                ms.Position = 0;
                XmlDictionaryReader xmlReader = XmlDictionaryReader.CreateBinaryReader(ms, dict, quotas);
                xmlReader.Read();
                returnedString = xmlReader.ReadElementContentAsString();
            }

            Assert.StrictEqual(testString, returnedString);
        }

        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private static string GenerateDoubleWideTestString(int charsToGenerate, int singleByteChars)
        {
            int count = 0;
            int startChar = 0x10000;

            var sb = new StringBuilder();

            while (count < singleByteChars)
            {
                sb.Append((char)('a' + count % 26));
                count++;
            }

            count = 0;

            while (count < charsToGenerate)
            {
                sb.Append(char.ConvertFromUtf32(startChar + count % 65535));
                count++;
            }

            return sb.ToString();
        }
    }
}
