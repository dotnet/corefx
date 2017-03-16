// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using Xunit;

namespace System.Reflection.Metadata.Decoding
{
    public partial class TypeNameParserTests
    {
        partial void ParseTypeWithDesktopReflection(string typeName, string expected)
        {
            Assert.Equal(expected, StringBasedType.ParseTypeName(typeName));
        }

        partial void ParseInvalidTypeWithDesktopReflection(string typeName)
        {
            try
            {
                string actual = StringBasedType.ParseTypeName(typeName);
                Assert.False(true, string.Format("Type name '{0}' was expected to be invalid, but was successfully parsed as '{1}' by Reflection.", typeName, actual));
            }
            catch (ArgumentException)
            {
            }
            catch (TypeLoadException)
            {
            }
            catch (FileLoadException)
            {
            }
        }
    }
}
