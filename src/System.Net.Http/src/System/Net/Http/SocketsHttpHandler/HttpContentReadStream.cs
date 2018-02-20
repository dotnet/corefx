﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Threading;

namespace System.Net.Http
{
    internal abstract class HttpContentReadStream : HttpContentStream
    {
        public HttpContentReadStream(HttpConnection connection) : base(connection)
        {
        }

        public sealed override bool CanRead => true;
        public sealed override bool CanWrite => false;

        public sealed override void Flush() { }

        public sealed override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        public sealed override int Read(byte[] buffer, int offset, int count)
        {
            ValidateBufferArgs(buffer, offset, count);
            return ReadAsync(new Memory<byte>(buffer, offset, count), CancellationToken.None).GetAwaiter().GetResult();
        }

        public sealed override void CopyTo(Stream destination, int bufferSize) =>
            CopyToAsync(destination, bufferSize, CancellationToken.None).GetAwaiter().GetResult();

        // Maximum request drain size, 16K.
        protected const int s_maxDrainBytes = 16 * 1024;
    }
}
