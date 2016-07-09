// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Tests
{
    public class ConfigurablePropertyStream : Stream
    {
        private bool _canRead;
        private bool _canWrite;
        private bool _canSeek;
        private bool _canTimeout;

        private long _length;

        public ConfigurablePropertyStream()
        {
            SetLengthCore = value => _length = value;
        }

        public override bool CanRead => _canRead;
        public override bool CanWrite => _canWrite;
        public override bool CanSeek => _canSeek;
        public override bool CanTimeout => _canTimeout;

        public override long Length => _length;
        public override long Position { get; set; }

        public override int ReadTimeout { get; set; }
        public override int WriteTimeout { get; set; }

        // Method delegates
        // Default behavior of everything is to nop, except for SetLength

        public Action FlushCore { get; set; } = () => { };
        public Func<byte[], int, int, int> ReadCore { get; set; } = (buffer, offset, count) => 0;
        public Func<long, SeekOrigin, long> SeekCore { get; set; } = (offset, origin) => 0;
        public Action<long> SetLengthCore { get; set; }
        public Action<byte[], int, int> WriteCore { get; set; } = (buffer, offset, count) => { };
        
        // Methods (just wrappers for the delegates)

        public override void Flush() => FlushCore();
        public override int Read(byte[] buffer, int offset, int count) => ReadCore(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => SeekCore(offset, origin);
        public override void SetLength(long value) => SetLengthCore(value);
        public override void Write(byte[] buffer, int offset, int count) => WriteCore(buffer, offset, count);

        // 'Setters' for the get-only properties
        // Unfortunately, C# doesn't seem to support both overriding a property and adding a setter
        // e.g. override bool CanRead { get; set; } will fail, since the base class has no setter

        public void SetCanRead(bool value) => _canRead = value;
        public void SetCanWrite(bool value) => _canWrite = value;
        public void SetCanSeek(bool value) => _canSeek = value;
        public void SetCanTimeout(bool value) => _canTimeout = value;
    }
}
