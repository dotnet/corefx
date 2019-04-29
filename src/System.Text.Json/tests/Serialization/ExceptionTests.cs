// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class ExceptionTests
    {
        [Fact]
        public static void RootException()
        {
            bool exceptionThrown = false;
            try
            {
                int i2 = JsonSerializer.Parse<int>("12bad");
            }
            catch (JsonSerializationException e)
            {
                exceptionThrown = true;

                Assert.Equal(0, e.LineNumber);
                Assert.Equal(2, e.BytePositionInLine);
                Assert.Equal("[System.Int32]", e.Path);
            }

            Assert.True(exceptionThrown);
        }

        [Fact]
        public static void RethrownJsonReaderException()
        {
            bool exceptionThrown = false;
            string json = Encoding.UTF8.GetString(BasicCompany.s_data);

            json = json.Replace(@"""zip"" : 98052", @"""zip"" : bad");

            try
            {
                JsonSerializer.Parse<BasicCompany>(json);
            }
            catch (JsonSerializationException e)
            {
                exceptionThrown = true;

                Assert.Equal(18, e.LineNumber);
                Assert.Equal(8, e.BytePositionInLine);
                Assert.Equal("[System.Text.Json.Serialization.Tests.BasicCompany].mainSite.zip", e.Path);
                Assert.Contains("Path: [System.Text.Json.Serialization.Tests.BasicCompany].mainSite.zip | LineNumber: 18 | BytePositionInLine: 8.",
                    e.Message);

                Assert.NotNull(e.InnerException);
                JsonReaderException inner = (JsonReaderException)e.InnerException;
                Assert.Equal(18, inner.LineNumber);
                Assert.Equal(8, inner.BytePositionInLine);
            }

            Assert.True(exceptionThrown);
        }
    }
}
