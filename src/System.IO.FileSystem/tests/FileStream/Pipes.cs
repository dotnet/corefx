﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Win32.SafeHandles;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Tests
{
    public class Pipes : FileSystemTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task AnonymousPipeWriteViaFileStream(bool asyncWrites)
        {
            using (var server = new AnonymousPipeServerStream(PipeDirection.In))
            {
                Task serverTask = Task.Run(() =>
                {
                    for (int i = 0; i < 6; i++)
                        Assert.Equal(i, server.ReadByte());
                });

                using (var client = new FileStream(new SafeFileHandle(server.ClientSafePipeHandle.DangerousGetHandle(), false), FileAccess.Write, bufferSize: 3))
                {
                    var data = new[] { new byte[] { 0, 1 }, new byte[] { 2, 3 }, new byte[] { 4, 5 } };
                    foreach (byte[] arr in data)
                    {
                        if (asyncWrites)
                            await client.WriteAsync(arr, 0, arr.Length);
                        else
                            client.Write(arr, 0, arr.Length);
                    }
                }

                await serverTask;
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task AnonymousPipeReadViaFileStream(bool asyncReads)
        {
            using (var server = new AnonymousPipeServerStream(PipeDirection.Out))
            {
                Task serverTask = server.WriteAsync(new byte[] { 0, 1, 2, 3, 4, 5 }, 0, 6);

                using (var client = new FileStream(new SafeFileHandle(server.ClientSafePipeHandle.DangerousGetHandle(), false), FileAccess.Read, bufferSize: 3))
                {
                    var arr = new byte[1];
                    for (int i = 0; i < 6; i++)
                    {
                        Assert.Equal(1, asyncReads ?
                            await client.ReadAsync(arr, 0, 1) :
                            client.Read(arr, 0, 1));
                        Assert.Equal(i, arr[0]);
                    }
                }

                await serverTask;
            }
        }

        [PlatformSpecific(PlatformID.Windows)] // Uses P/Invokes to create async pipe handle
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task NamedPipeWriteViaAsyncFileStream(bool asyncWrites)
        {
            string name = Guid.NewGuid().ToString("N");
            using (var server = new NamedPipeServerStream(name, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
            {
                Task serverTask = Task.Run(async () =>
                {
                    await server.WaitForConnectionAsync();
                    for (int i = 0; i < 6; i++)
                        Assert.Equal(i, server.ReadByte());
                });

                WaitNamedPipeW(@"\\.\pipe\" + name, -1);
                using (SafeFileHandle clientHandle = CreateFileW(@"\\.\pipe\" + name, GENERIC_WRITE, FileShare.None, IntPtr.Zero, FileMode.Open, (int)PipeOptions.Asynchronous, IntPtr.Zero))
                using (var client = new FileStream(clientHandle, FileAccess.Write, bufferSize: 3, isAsync: true))
                {
                    var data = new[] { new byte[] { 0, 1 }, new byte[] { 2, 3 }, new byte[] { 4, 5 } };
                    foreach (byte[] arr in data)
                    {
                        if (asyncWrites)
                            await client.WriteAsync(arr, 0, arr.Length);
                        else
                            client.Write(arr, 0, arr.Length);
                    }
                }

                await serverTask;
            }
        }

        [PlatformSpecific(PlatformID.Windows)] // Uses P/Invokes to create async pipe handle
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task NamedPipeReadViaAsyncFileStream(bool asyncReads)
        {
            string name = Guid.NewGuid().ToString("N");
            using (var server = new NamedPipeServerStream(name, PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
            {
                Task serverTask = Task.Run(async () =>
                {
                    await server.WaitForConnectionAsync();
                    await server.WriteAsync(new byte[] { 0, 1, 2, 3, 4, 5 }, 0, 6);
                });

                WaitNamedPipeW(@"\\.\pipe\" + name, -1);
                using (SafeFileHandle clientHandle = CreateFileW(@"\\.\pipe\" + name, GENERIC_READ, FileShare.None, IntPtr.Zero, FileMode.Open, (int)PipeOptions.Asynchronous, IntPtr.Zero))
                using (var client = new FileStream(clientHandle, FileAccess.Read, bufferSize: 3, isAsync: true))
                {
                    var arr = new byte[1];
                    for (int i = 0; i < 6; i++)
                    {
                        Assert.Equal(1, asyncReads ?
                            await client.ReadAsync(arr, 0, 1) :
                            client.Read(arr, 0, 1));
                        Assert.Equal(i, arr[0]);
                    }
                }

                await serverTask;
            }
        }

        #region Windows P/Invokes
        // We need to P/Invoke to test the named pipe async behavior with FileStream
        // because NamedPipeClientStream internally binds the created handle,
        // and that then prevents FileStream's constructor from working with the handle
        // when trying to set isAsync to true.

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WaitNamedPipeW(string name, int timeout);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern SafeFileHandle CreateFileW(
            string lpFileName, int dwDesiredAccess, FileShare dwShareMode, 
            IntPtr securityAttrs, FileMode dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

        internal const int GENERIC_READ = unchecked((int)0x80000000);
        internal const int GENERIC_WRITE = 0x40000000;
        #endregion
    }
}
