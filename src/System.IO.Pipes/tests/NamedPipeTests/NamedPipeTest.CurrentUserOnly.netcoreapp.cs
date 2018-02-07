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

        [Fact]
        public static void CreateServer_ConnectClient()
        {
            var name = GetUniquePipeName();
            using (var server = new NamedPipeServerStream(name, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.CurrentUserOnly))
            {
                using (var client = new NamedPipeClientStream(".", name, PipeDirection.InOut, PipeOptions.CurrentUserOnly))
                {
                    // Should not fail to connect since both, the server and client have the same owner.
                    client.Connect();
                }
            }
        }
    }
}