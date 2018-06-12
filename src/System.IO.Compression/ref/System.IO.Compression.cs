// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.IO.Compression
{
    public enum CompressionLevel
    {
        Fastest = 1,
        NoCompression = 2,
        Optimal = 0,
    }
    public enum CompressionMode
    {
        Compress = 1,
        Decompress = 0,
    }
    public partial class DeflateStream : System.IO.Stream
    {
        public DeflateStream(System.IO.Stream stream, System.IO.Compression.CompressionLevel compressionLevel) { }
        public DeflateStream(System.IO.Stream stream, System.IO.Compression.CompressionLevel compressionLevel, bool leaveOpen) { }
        public DeflateStream(System.IO.Stream stream, System.IO.Compression.CompressionMode mode) { }
        public DeflateStream(System.IO.Stream stream, System.IO.Compression.CompressionMode mode, bool leaveOpen) { }
        public System.IO.Stream BaseStream { get { throw null; } }
        public override bool CanRead { get { throw null; } }
        public override bool CanSeek { get { throw null; } }
        public override bool CanWrite { get { throw null; } }
        public override long Length { get { throw null; } }
        public override long Position { get { throw null; } set { } }
        public override System.Threading.Tasks.Task CopyToAsync(System.IO.Stream destination, int bufferSize, System.Threading.CancellationToken cancellationToken) { throw null; }
        protected override void Dispose(bool disposing) { }
        public override void Flush() { }
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState) { throw null; }
        public override int EndRead(IAsyncResult asyncResult) { throw null; }
        public override System.Threading.Tasks.Task FlushAsync(System.Threading.CancellationToken cancellationToken) { throw null; }
        public override int Read(byte[] array, int offset, int count) { throw null; }
        public override int Read(System.Span<byte> buffer) { throw null; }
        public override System.Threading.Tasks.Task<int> ReadAsync(byte[] array, int offset, int count, System.Threading.CancellationToken cancellationToken) { throw null; }
        public override System.Threading.Tasks.ValueTask<int> ReadAsync(System.Memory<byte> buffer, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public override int ReadByte() { throw null; }
        public override long Seek(long offset, System.IO.SeekOrigin origin) { throw null; }
        public override void SetLength(long value) { }
        public override IAsyncResult BeginWrite(byte[] array, int offset, int count, AsyncCallback asyncCallback, object asyncState) { throw null; }
        public override void EndWrite(IAsyncResult asyncResult) { }
        public override void Write(byte[] array, int offset, int count) { }
        public override void Write(System.ReadOnlySpan<byte> buffer) { }
        public override System.Threading.Tasks.Task WriteAsync(byte[] array, int offset, int count, System.Threading.CancellationToken cancellationToken) { throw null; }
        public override System.Threading.Tasks.ValueTask WriteAsync(System.ReadOnlyMemory<byte> buffer, System.Threading.CancellationToken cancellationToken = default) { throw null; }
        public override void CopyTo(Stream destination, int bufferSize) { }
    }
    public partial class GZipStream : System.IO.Stream
    {
        public GZipStream(System.IO.Stream stream, System.IO.Compression.CompressionLevel compressionLevel) { }
        public GZipStream(System.IO.Stream stream, System.IO.Compression.CompressionLevel compressionLevel, bool leaveOpen) { }
        public GZipStream(System.IO.Stream stream, System.IO.Compression.CompressionMode mode) { }
        public GZipStream(System.IO.Stream stream, System.IO.Compression.CompressionMode mode, bool leaveOpen) { }
        public System.IO.Stream BaseStream { get { throw null; } }
        public override bool CanRead { get { throw null; } }
        public override bool CanSeek { get { throw null; } }
        public override bool CanWrite { get { throw null; } }
        public override long Length { get { throw null; } }
        public override long Position { get { throw null; } set { } }
        public override void CopyTo(System.IO.Stream destination, int bufferSize) { }
        public override System.Threading.Tasks.Task CopyToAsync(System.IO.Stream destination, int bufferSize, System.Threading.CancellationToken cancellationToken) { throw null; }
        protected override void Dispose(bool disposing) { }
        public override void Flush() { }
        public override IAsyncResult BeginRead(byte[] array, int offset, int count, AsyncCallback asyncCallback, object asyncState) { throw null; }
        public override int EndRead(IAsyncResult asyncResult) { throw null; }
        public override System.Threading.Tasks.Task FlushAsync(System.Threading.CancellationToken cancellationToken) { throw null; }
        public override int ReadByte() { throw null; }
        public override int Read(byte[] array, int offset, int count) { throw null; }
        public override int Read(System.Span<byte> buffer) { throw null; }
        public override System.Threading.Tasks.Task<int> ReadAsync(byte[] array, int offset, int count, System.Threading.CancellationToken cancellationToken) { throw null; }
        public override System.Threading.Tasks.ValueTask<int> ReadAsync(System.Memory<byte> buffer, System.Threading.CancellationToken cancellationToken = default) { throw null; }
        public override long Seek(long offset, System.IO.SeekOrigin origin) { throw null; }
        public override void SetLength(long value) { }
        public override IAsyncResult BeginWrite(byte[] array, int offset, int count, AsyncCallback asyncCallback, object asyncState) { throw null; }
        public override void EndWrite(IAsyncResult asyncResult) { }
        public override void Write(byte[] array, int offset, int count) { }
        public override void Write(System.ReadOnlySpan<byte> buffer) { }
        public override System.Threading.Tasks.Task WriteAsync(byte[] array, int offset, int count, System.Threading.CancellationToken cancellationToken) { throw null; }
        public override System.Threading.Tasks.ValueTask WriteAsync(System.ReadOnlyMemory<byte> buffer, System.Threading.CancellationToken cancellationToken = default) { throw null; }
    }
    public partial class ZipArchive : System.IDisposable
    {
        public ZipArchive(System.IO.Stream stream) { }
        public ZipArchive(System.IO.Stream stream, System.IO.Compression.ZipArchiveMode mode) { }
        public ZipArchive(System.IO.Stream stream, System.IO.Compression.ZipArchiveMode mode, bool leaveOpen) { }
        public ZipArchive(System.IO.Stream stream, System.IO.Compression.ZipArchiveMode mode, bool leaveOpen, System.Text.Encoding entryNameEncoding) { }
        public System.Collections.ObjectModel.ReadOnlyCollection<System.IO.Compression.ZipArchiveEntry> Entries { get { throw null; } }
        public System.IO.Compression.ZipArchiveMode Mode { get { throw null; } }
        public System.IO.Compression.ZipArchiveEntry CreateEntry(string entryName) { throw null; }
        public System.IO.Compression.ZipArchiveEntry CreateEntry(string entryName, System.IO.Compression.CompressionLevel compressionLevel) { throw null; }
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
        public System.IO.Compression.ZipArchiveEntry GetEntry(string entryName) { throw null; }
    }
    public partial class ZipArchiveEntry
    {
        internal ZipArchiveEntry() { }
        public System.IO.Compression.ZipArchive Archive { get { throw null; } }
        public long CompressedLength { get { throw null; } }
        public int ExternalAttributes { get { throw null; } set { } }
        public string FullName { get { throw null; } }
        public System.DateTimeOffset LastWriteTime { get { throw null; } set { } }
        public long Length { get { throw null; } }
        public string Name { get { throw null; } }
        [CLSCompliant(false)]
        public uint Crc32 { get { throw null; } }
        public void Delete() { }
        public System.IO.Stream Open() { throw null; }
        public override string ToString() { throw null; }
    }
    public enum ZipArchiveMode
    {
        Create = 1,
        Read = 0,
        Update = 2,
    }
}
