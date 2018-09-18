// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using Xunit;

namespace System.Diagnostics.TextWriterTraceListenerTests
{
    public class ConsoleTraceListenerTests
    {
        [Fact]
        public static void DefaultCtorPropertiesCheck()
        {
            using (var listener = new ConsoleTraceListener())
                Assert.Equal(Console.Out, listener.Writer);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static void BoolCtorPropertiesCheck(bool value)
        {
            using (TextWriter expected = value ? Console.Error : Console.Out)
            using (var listener = new ConsoleTraceListener(useErrorStream: value))
                Assert.Equal(expected, listener.Writer);
        }

        [Fact]
        public static void NopClose()
        {
            using (var listener = new ConsoleTraceListener())
            {
                listener.Close();
                Assert.NotNull(listener.Writer);
                Assert.Equal(Console.Out, listener.Writer);
            }
        }
    }
}
