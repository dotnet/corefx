// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;

namespace System.Xml.Tests
{
    internal static class XmlTextReaderTestHelper
    {
        internal static XmlTextReader CreateReader(string input)
        {
            var textReader = new XmlTextReader(input, XmlNodeType.Element, null);
            return textReader;
        }

        internal static XmlTextReader CreateReaderWithStringReader(string input)
        {
            var textReader = new XmlTextReader(null as string, new StringReader(input), null);
            return textReader;
        }
    }
}
