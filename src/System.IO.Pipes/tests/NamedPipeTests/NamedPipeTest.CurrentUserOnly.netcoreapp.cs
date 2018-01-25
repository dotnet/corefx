// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.IO.Pipes.Tests
{
    /// <summary>
    /// Tests for the constructors for NamedPipeClientStream
    /// </summary>
    public class NamedPipeTest_CurrentUserOnly : NamedPipeTestBase
    {
        [Fact]
        public static void CreateClient_CurrentUserOnly()
        {
            // Should not throw.
            new NamedPipeClientStream(".", GetUniquePipeName(), PipeDirection.InOut, PipeOptions.CurrentUserOnly).Dispose();
        }
        
        [Fact]
        public static void CreateServer_CurrentUserOnly()
        {
            // Should not throw.
            new NamedPipeServerStream(GetUniquePipeName(), PipeDirection.InOut, 2, PipeTransmissionMode.Byte, PipeOptions.CurrentUserOnly).Dispose();
        }
    }
}