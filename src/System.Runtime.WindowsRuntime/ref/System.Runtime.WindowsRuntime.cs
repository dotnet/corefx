// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System
{
    [System.CLSCompliantAttribute(false)]
    public static partial class WindowsRuntimeSystemExtensions
    {
        public static Windows.Foundation.IAsyncAction AsAsyncAction(this System.Threading.Tasks.Task source) { throw null; }
        public static Windows.Foundation.IAsyncOperation<TResult> AsAsyncOperation<TResult>(this System.Threading.Tasks.Task<TResult> source) { throw null; }
        public static System.Threading.Tasks.Task AsTask(this Windows.Foundation.IAsyncAction source) { throw null; }
        public static System.Threading.Tasks.Task AsTask(this Windows.Foundation.IAsyncAction source, System.Threading.CancellationToken cancellationToken) { throw null; }
        public static System.Threading.Tasks.Task AsTask<TProgress>(this Windows.Foundation.IAsyncActionWithProgress<TProgress> source) { throw null; }
        public static System.Threading.Tasks.Task AsTask<TProgress>(this Windows.Foundation.IAsyncActionWithProgress<TProgress> source, System.IProgress<TProgress> progress) { throw null; }
        public static System.Threading.Tasks.Task AsTask<TProgress>(this Windows.Foundation.IAsyncActionWithProgress<TProgress> source, System.Threading.CancellationToken cancellationToken) { throw null; }
        public static System.Threading.Tasks.Task AsTask<TProgress>(this Windows.Foundation.IAsyncActionWithProgress<TProgress> source, System.Threading.CancellationToken cancellationToken, System.IProgress<TProgress> progress) { throw null; }
        public static System.Threading.Tasks.Task<TResult> AsTask<TResult>(this Windows.Foundation.IAsyncOperation<TResult> source) { throw null; }
        public static System.Threading.Tasks.Task<TResult> AsTask<TResult>(this Windows.Foundation.IAsyncOperation<TResult> source, System.Threading.CancellationToken cancellationToken) { throw null; }
        public static System.Threading.Tasks.Task<TResult> AsTask<TResult, TProgress>(this Windows.Foundation.IAsyncOperationWithProgress<TResult, TProgress> source) { throw null; }
        public static System.Threading.Tasks.Task<TResult> AsTask<TResult, TProgress>(this Windows.Foundation.IAsyncOperationWithProgress<TResult, TProgress> source, System.IProgress<TProgress> progress) { throw null; }
        public static System.Threading.Tasks.Task<TResult> AsTask<TResult, TProgress>(this Windows.Foundation.IAsyncOperationWithProgress<TResult, TProgress> source, System.Threading.CancellationToken cancellationToken) { throw null; }
        public static System.Threading.Tasks.Task<TResult> AsTask<TResult, TProgress>(this Windows.Foundation.IAsyncOperationWithProgress<TResult, TProgress> source, System.Threading.CancellationToken cancellationToken, System.IProgress<TProgress> progress) { throw null; }
        public static System.Runtime.CompilerServices.TaskAwaiter GetAwaiter(this Windows.Foundation.IAsyncAction source) { throw null; }
        public static System.Runtime.CompilerServices.TaskAwaiter GetAwaiter<TProgress>(this Windows.Foundation.IAsyncActionWithProgress<TProgress> source) { throw null; }
        public static System.Runtime.CompilerServices.TaskAwaiter<TResult> GetAwaiter<TResult>(this Windows.Foundation.IAsyncOperation<TResult> source) { throw null; }
        public static System.Runtime.CompilerServices.TaskAwaiter<TResult> GetAwaiter<TResult, TProgress>(this Windows.Foundation.IAsyncOperationWithProgress<TResult, TProgress> source) { throw null; }
    }
}
namespace System.IO
{
    public static partial class WindowsRuntimeStorageExtensions
    {
        [System.CLSCompliantAttribute(false)]
        public static Microsoft.Win32.SafeHandles.SafeFileHandle CreateSafeFileHandle(this Windows.Storage.IStorageFile windowsRuntimeFile, System.IO.FileAccess access = System.IO.FileAccess.ReadWrite, System.IO.FileShare share = System.IO.FileShare.Read, System.IO.FileOptions options = System.IO.FileOptions.None) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static Microsoft.Win32.SafeHandles.SafeFileHandle CreateSafeFileHandle(this Windows.Storage.IStorageFolder rootDirectory, string relativePath, System.IO.FileMode mode) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static Microsoft.Win32.SafeHandles.SafeFileHandle CreateSafeFileHandle(this Windows.Storage.IStorageFolder rootDirectory, string relativePath, System.IO.FileMode mode, System.IO.FileAccess access, System.IO.FileShare share = System.IO.FileShare.Read, System.IO.FileOptions options = System.IO.FileOptions.None) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static System.Threading.Tasks.Task<System.IO.Stream> OpenStreamForReadAsync(this Windows.Storage.IStorageFile windowsRuntimeFile) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static System.Threading.Tasks.Task<System.IO.Stream> OpenStreamForReadAsync(this Windows.Storage.IStorageFolder rootDirectory, string relativePath) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static System.Threading.Tasks.Task<System.IO.Stream> OpenStreamForWriteAsync(this Windows.Storage.IStorageFile windowsRuntimeFile) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static System.Threading.Tasks.Task<System.IO.Stream> OpenStreamForWriteAsync(this Windows.Storage.IStorageFolder rootDirectory, string relativePath, Windows.Storage.CreationCollisionOption creationCollisionOption) { throw null; }
    }
    public static partial class WindowsRuntimeStreamExtensions
    {
        [System.CLSCompliantAttribute(false)]
        public static Windows.Storage.Streams.IInputStream AsInputStream(this System.IO.Stream stream) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static Windows.Storage.Streams.IOutputStream AsOutputStream(this System.IO.Stream stream) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static Windows.Storage.Streams.IRandomAccessStream AsRandomAccessStream(this System.IO.Stream stream) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static System.IO.Stream AsStream(this Windows.Storage.Streams.IRandomAccessStream windowsRuntimeStream) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static System.IO.Stream AsStream(this Windows.Storage.Streams.IRandomAccessStream windowsRuntimeStream, int bufferSize) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static System.IO.Stream AsStreamForRead(this Windows.Storage.Streams.IInputStream windowsRuntimeStream) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static System.IO.Stream AsStreamForRead(this Windows.Storage.Streams.IInputStream windowsRuntimeStream, int bufferSize) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static System.IO.Stream AsStreamForWrite(this Windows.Storage.Streams.IOutputStream windowsRuntimeStream) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static System.IO.Stream AsStreamForWrite(this Windows.Storage.Streams.IOutputStream windowsRuntimeStream, int bufferSize) { throw null; }
    }
}
namespace System.Runtime.InteropServices.WindowsRuntime
{
    [System.CLSCompliantAttribute(false)]
    public static partial class AsyncInfo
    {
        public static Windows.Foundation.IAsyncAction Run(System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task> taskProvider) { throw null; }
        public static Windows.Foundation.IAsyncActionWithProgress<TProgress> Run<TProgress>(System.Func<System.Threading.CancellationToken, System.IProgress<TProgress>, System.Threading.Tasks.Task> taskProvider) { throw null; }
        public static Windows.Foundation.IAsyncOperation<TResult> Run<TResult>(System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task<TResult>> taskProvider) { throw null; }
        public static Windows.Foundation.IAsyncOperationWithProgress<TResult, TProgress> Run<TResult, TProgress>(System.Func<System.Threading.CancellationToken, System.IProgress<TProgress>, System.Threading.Tasks.Task<TResult>> taskProvider) { throw null; }
    }
    public sealed partial class WindowsRuntimeBuffer
    {
        internal WindowsRuntimeBuffer() { }
        [System.CLSCompliantAttribute(false)]
        public static Windows.Storage.Streams.IBuffer Create(byte[] data, int offset, int length, int capacity) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static Windows.Storage.Streams.IBuffer Create(int capacity) { throw null; }
    }
    public static partial class WindowsRuntimeBufferExtensions
    {
        [System.CLSCompliantAttribute(false)]
        public static Windows.Storage.Streams.IBuffer AsBuffer(this byte[] source) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static Windows.Storage.Streams.IBuffer AsBuffer(this byte[] source, int offset, int length) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static Windows.Storage.Streams.IBuffer AsBuffer(this byte[] source, int offset, int length, int capacity) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static System.IO.Stream AsStream(this Windows.Storage.Streams.IBuffer source) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static void CopyTo(this byte[] source, int sourceIndex, Windows.Storage.Streams.IBuffer destination, uint destinationIndex, int count) { }
        [System.CLSCompliantAttribute(false)]
        public static void CopyTo(this byte[] source, Windows.Storage.Streams.IBuffer destination) { }
        [System.CLSCompliantAttribute(false)]
        public static void CopyTo(this Windows.Storage.Streams.IBuffer source, byte[] destination) { }
        [System.CLSCompliantAttribute(false)]
        public static void CopyTo(this Windows.Storage.Streams.IBuffer source, uint sourceIndex, byte[] destination, int destinationIndex, int count) { }
        [System.CLSCompliantAttribute(false)]
        public static void CopyTo(this Windows.Storage.Streams.IBuffer source, uint sourceIndex, Windows.Storage.Streams.IBuffer destination, uint destinationIndex, uint count) { }
        [System.CLSCompliantAttribute(false)]
        public static void CopyTo(this Windows.Storage.Streams.IBuffer source, Windows.Storage.Streams.IBuffer destination) { }
        [System.CLSCompliantAttribute(false)]
        public static byte GetByte(this Windows.Storage.Streams.IBuffer source, uint byteOffset) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static Windows.Storage.Streams.IBuffer GetWindowsRuntimeBuffer(this System.IO.MemoryStream underlyingStream) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static Windows.Storage.Streams.IBuffer GetWindowsRuntimeBuffer(this System.IO.MemoryStream underlyingStream, int positionInStream, int length) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static bool IsSameData(this Windows.Storage.Streams.IBuffer buffer, Windows.Storage.Streams.IBuffer otherBuffer) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static byte[] ToArray(this Windows.Storage.Streams.IBuffer source) { throw null; }
        [System.CLSCompliantAttribute(false)]
        public static byte[] ToArray(this Windows.Storage.Streams.IBuffer source, uint sourceIndex, int count) { throw null; }
    }
}
namespace Windows.Foundation
{
    public partial struct Point : System.IFormattable
    {
        private int _dummyPrimitive;
        public Point(double x, double y) { throw null; }
        public double X { get { throw null; } set { } }
        public double Y { get { throw null; } set { } }
        public override bool Equals(object o) { throw null; }
        public bool Equals(Windows.Foundation.Point value) { throw null; }
        public override int GetHashCode() { throw null; }
        public static bool operator ==(Windows.Foundation.Point point1, Windows.Foundation.Point point2) { throw null; }
        public static bool operator !=(Windows.Foundation.Point point1, Windows.Foundation.Point point2) { throw null; }
        string System.IFormattable.ToString(string format, System.IFormatProvider provider) { throw null; }
        public override string ToString() { throw null; }
        public string ToString(System.IFormatProvider provider) { throw null; }
    }
    public partial struct Rect : System.IFormattable
    {
        private int _dummyPrimitive;
        public Rect(double x, double y, double width, double height) { throw null; }
        public Rect(Windows.Foundation.Point point1, Windows.Foundation.Point point2) { throw null; }
        public Rect(Windows.Foundation.Point location, Windows.Foundation.Size size) { throw null; }
        public double Bottom { get { throw null; } }
        public static Windows.Foundation.Rect Empty { get { throw null; } }
        public double Height { get { throw null; } set { } }
        public bool IsEmpty { get { throw null; } }
        public double Left { get { throw null; } }
        public double Right { get { throw null; } }
        public double Top { get { throw null; } }
        public double Width { get { throw null; } set { } }
        public double X { get { throw null; } set { } }
        public double Y { get { throw null; } set { } }
        public bool Contains(Windows.Foundation.Point point) { throw null; }
        public override bool Equals(object o) { throw null; }
        public bool Equals(Windows.Foundation.Rect value) { throw null; }
        public override int GetHashCode() { throw null; }
        public void Intersect(Windows.Foundation.Rect rect) { }
        public static bool operator ==(Windows.Foundation.Rect rect1, Windows.Foundation.Rect rect2) { throw null; }
        public static bool operator !=(Windows.Foundation.Rect rect1, Windows.Foundation.Rect rect2) { throw null; }
        string System.IFormattable.ToString(string format, System.IFormatProvider provider) { throw null; }
        public override string ToString() { throw null; }
        public string ToString(System.IFormatProvider provider) { throw null; }
        public void Union(Windows.Foundation.Point point) { }
        public void Union(Windows.Foundation.Rect rect) { }
    }
    public partial struct Size
    {
        private int _dummyPrimitive;
        public Size(double width, double height) { throw null; }
        public static Windows.Foundation.Size Empty { get { throw null; } }
        public double Height { get { throw null; } set { } }
        public bool IsEmpty { get { throw null; } }
        public double Width { get { throw null; } set { } }
        public override bool Equals(object o) { throw null; }
        public bool Equals(Windows.Foundation.Size value) { throw null; }
        public override int GetHashCode() { throw null; }
        public static bool operator ==(Windows.Foundation.Size size1, Windows.Foundation.Size size2) { throw null; }
        public static bool operator !=(Windows.Foundation.Size size1, Windows.Foundation.Size size2) { throw null; }
        public override string ToString() { throw null; }
    }
}
namespace Windows.UI
{
    public partial struct Color : System.IFormattable
    {
        private int _dummyPrimitive;
        public byte A { get { throw null; } set { } }
        public byte B { get { throw null; } set { } }
        public byte G { get { throw null; } set { } }
        public byte R { get { throw null; } set { } }
        public override bool Equals(object o) { throw null; }
        public bool Equals(Windows.UI.Color color) { throw null; }
        public static Windows.UI.Color FromArgb(byte a, byte r, byte g, byte b) { throw null; }
        public override int GetHashCode() { throw null; }
        public static bool operator ==(Windows.UI.Color color1, Windows.UI.Color color2) { throw null; }
        public static bool operator !=(Windows.UI.Color color1, Windows.UI.Color color2) { throw null; }
        string System.IFormattable.ToString(string format, System.IFormatProvider provider) { throw null; }
        public override string ToString() { throw null; }
        public string ToString(System.IFormatProvider provider) { throw null; }
    }
}
