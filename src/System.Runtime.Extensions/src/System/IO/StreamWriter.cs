// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace System.IO
{
    // This class implements a TextWriter for writing characters to a Stream.
    // This is designed for character output in a particular Encoding, 
    // whereas the Stream class is designed for byte input and output.  
    public class StreamWriter : TextWriter
    {
        // For UTF-8, the values of 1K for the default buffer size and 4K for the
        // file stream buffer size are reasonable & give very reasonable
        // performance for in terms of construction time for the StreamWriter and
        // write perf.  Note that for UTF-8, we end up allocating a 4K byte buffer,
        // which means we take advantage of adaptive buffering code.
        // The performance using UnicodeEncoding is acceptable.  
        private const int DefaultBufferSize = 1024;   // char[]
        private const int DefaultFileStreamBufferSize = 4096;
        private const int MinBufferSize = 128;

        private const int DontCopyOnWriteThreshold = 512;
        private const int RentFromPoolThreshold = 64;

        // Bit bucket - Null has no backing store. Non closable.
        public new static readonly StreamWriter Null = new StreamWriter(Stream.Null, UTF8NoBOM, MinBufferSize, true);

        // EncoderNLS.GetBytes allocates if given an empty input array
        public static readonly char[] SingleCharArray = new char[1];

        private Stream _stream;
        private Encoding _encoding;
        private Encoder _encoder;
        private char[] _rentedCharBuffer;
        private byte[] _rentedByteBuffer;
        private int _charPos;
        private int _charLen;
        private int _byteLen;
        private bool _autoFlush;
        private bool _haveWrittenPreamble;
        private bool _closable;

        private int _maxFallbackBufferCount;

        // We don't guarantee thread safety on StreamWriter, but we should at 
        // least prevent users from trying to write anything while an Async
        // write from the same thread is in progress.
        private volatile Task _asyncWriteTask;

        private void CheckAsyncTaskInProgress()
        {
            // We are not locking the access to _asyncWriteTask because this is not meant to guarantee thread safety. 
            // We are simply trying to deter calling any Write APIs while an async Write from the same thread is in progress.

            Task t = _asyncWriteTask;

            if (t != null && !t.IsCompleted)
            {
                // Use void returning throw helper to shink this method and allow it to inline
                // rather than throwing directly and calling SR for message
                ThrowInvalidOperation_AsyncIOInProgress();
            }
        }

        private void ThrowInvalidOperation_AsyncIOInProgress()
        {
            throw new InvalidOperationException(SR.InvalidOperation_AsyncIOInProgress);
        }

        // The high level goal is to be tolerant of encoding errors when we read and very strict 
        // when we write. Hence, default StreamWriter encoding will throw on encoding error.   
        // Note: when StreamWriter throws on invalid encoding chars (for ex, high surrogate character 
        // D800-DBFF without a following low surrogate character DC00-DFFF), it will cause the 
        // internal StreamWriter's state to be irrecoverable as it would have buffered the 
        // illegal chars and any subsequent call to Flush() would hit the encoding error again. 
        // Even Close() will hit the exception as it would try to flush the unwritten data. 
        // Maybe we can add a DiscardBufferedData() method to get out of such situation (like 
        // StreamReader though for different reason). Either way, the buffered data will be lost!
        private static Encoding UTF8NoBOM => EncodingCache.UTF8NoBOM;


        internal StreamWriter() : base(null)
        { // Ask for CurrentCulture all the time 
        }

        public StreamWriter(Stream stream)
            : this(stream, UTF8NoBOM, DefaultBufferSize, false)
        {
        }

        public StreamWriter(Stream stream, Encoding encoding)
            : this(stream, encoding, DefaultBufferSize, false)
        {
        }

        // Creates a new StreamWriter for the given stream.  The 
        // character encoding is set by encoding and the buffer size, 
        // in number of 16-bit characters, is set by bufferSize.  
        // 
        public StreamWriter(Stream stream, Encoding encoding, int bufferSize)
            : this(stream, encoding, bufferSize, false)
        {
        }

        public StreamWriter(Stream stream, Encoding encoding, int bufferSize, bool leaveOpen)
            : base(null) // Ask for CurrentCulture all the time
        {
            if (stream == null || encoding == null)
            {
                throw new ArgumentNullException(stream == null ? nameof(stream) : nameof(encoding));
            }
            if (!stream.CanWrite)
            {
                throw new ArgumentException(SR.Argument_StreamNotWritable);
            }
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize), SR.ArgumentOutOfRange_NeedPosNum);
            }

            Init(stream, encoding, bufferSize, leaveOpen);
        }

        public StreamWriter(string path)
            : this(path, false, UTF8NoBOM, DefaultBufferSize)
        {
        }

        public StreamWriter(string path, bool append)
            : this(path, append, UTF8NoBOM, DefaultBufferSize)
        {
        }

        public StreamWriter(string path, bool append, Encoding encoding)
            : this(path, append, encoding, DefaultBufferSize)
        {
        }

        public StreamWriter(string path, bool append, Encoding encoding, int bufferSize)
        { 
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));
            if (path.Length == 0)
                throw new ArgumentException(SR.Argument_EmptyPath);
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize), SR.ArgumentOutOfRange_NeedPosNum);

            Stream stream = new FileStream(path, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read,
                DefaultFileStreamBufferSize, FileOptions.SequentialScan);
            Init(stream, encoding, bufferSize, shouldLeaveOpen: false);
        }

        private void Init(Stream streamArg, Encoding encodingArg, int bufferSize, bool shouldLeaveOpen)
        {
            _stream = streamArg;
            _encoding = encodingArg;
            _encoder = _encoding.GetEncoder();
            _charLen = bufferSize > MinBufferSize ? bufferSize : MinBufferSize;
            _byteLen = encodingArg.GetMaxByteCount(_charLen);
            _maxFallbackBufferCount = _encoding.GetMaxByteCount(2);
            // If we're appending to a Stream that already has data, don't write
            // the preamble.
            if (_stream.CanSeek && _stream.Position > 0)
            {
                _haveWrittenPreamble = true;
            }

            _closable = !shouldLeaveOpen;
        }

        public override void Close()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                // We need to flush any buffered data if we are being closed/disposed.
                // Also, we never close the handles for stdout & friends.  So we can safely 
                // write any buffered data to those streams even during finalization, which 
                // is generally the right thing to do.
                if (_stream != null)
                {
                    // Note: flush on the underlying stream can throw (ex., low disk space)
                    if (disposing /* || (LeaveOpen && stream is __ConsoleStream) */)
                    {
                        CheckAsyncTaskInProgress();

                        Debug.Assert(_rentedCharBuffer != null || _charPos == 0);
                        Flush(_rentedCharBuffer, 0, _charPos, flushStream: true, flushEncoder: true);
                    }
                }
            }
            finally
            {
                // Dispose of our resources if this StreamWriter is closable. 
                // Note: Console.Out and other such non closable streamwriters should be left alone 
                if (!LeaveOpen && _stream != null)
                {
                    try
                    {
                        // Attempt to close the stream even if there was an IO error from Flushing.
                        // Note that Stream.Close() can potentially throw here (may or may not be
                        // due to the same Flush error). In this case, we still need to ensure 
                        // cleaning up internal resources, hence the finally block.  
                        if (disposing)
                        {
                            _stream.Close();
                        }
                    }
                    finally
                    {
                        _stream = null;
                        _encoding = null;
                        _encoder = null;
                        if (_rentedCharBuffer != null || _rentedByteBuffer != null)
                        {
                            // If earlier Flush threw an exception we may still have a rented buffer
                            // return it now
                            ReturnBuffers();
                        }
                        _charLen = 0;
                        base.Dispose(disposing);
                    }
                }
            }
        }

        public override void Flush()
        {
            CheckAsyncTaskInProgress();

            Debug.Assert(_rentedCharBuffer != null || _charPos == 0);
            Flush(_rentedCharBuffer, 0, _charPos, flushStream: true, flushEncoder: true);
        }

        private void Flush(char[] charBuffer, int offset, int length, bool flushStream, bool flushEncoder)
        {
            // flushEncoder should be true at the end of the file and if
            // the user explicitly calls Flush (though not if AutoFlush is true).
            // This is required to flush any dangling characters from our UTF-7 
            // and UTF-8 encoders.  
            if (_stream == null)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_WriterClosed);
            }

            // Perf boost for Flush on non-dirty writers.
            if (length == 0 && !flushStream && !flushEncoder)
            {
                return;
            }

            if (!_haveWrittenPreamble)
            {
                WritePreamble();
            }

            if (charBuffer == null)
            {
                Debug.Assert(length == 0);
                charBuffer = SingleCharArray;
            }

            int count;
            if (_rentedByteBuffer == null && (count = _encoding.GetMaxByteCount(length + FallbackBufferRemaining(flushEncoder))) <= RentFromPoolThreshold)
            {
                if (count > 0)
                {
                    unsafe
                    {
                        byte* bytes = stackalloc byte[count];
                        Span<byte> byteSpan = new Span<byte>(bytes, count);
                        count = _encoder.GetBytes(new ReadOnlySpan<char>(charBuffer, offset, length), byteSpan, flushEncoder);
                        _charPos = 0;
                        if (count > 0)
                        {
                            _stream.Write(byteSpan.Slice(0, count));
                        }
                    }
                }
            }
            else
            {
                byte[] byteBuffer = ByteBuffer;
                count = _encoder.GetBytes(charBuffer, offset, length, byteBuffer, 0, flushEncoder);
                _charPos = 0;
                if (count > 0)
                {
                    _stream.Write(byteBuffer, 0, count);
                }
            }

            // By definition, calling Flush should flush the stream, but this is
            // only necessary if we passed in true for flushStream.  The Web
            // Services guys have some perf tests where flushing needlessly hurts.
            if (flushStream)
            {
                ReturnBuffers();
                _stream.Flush();
            }
        }

        private void WritePreamble()
        {
            _haveWrittenPreamble = true;
            ReadOnlySpan<byte> preamble = _encoding.Preamble;
            if (preamble.Length > 0)
            {
                _stream.Write(preamble);
            }
        }

        // To not allocate on checking this wants to be: 
        // _encoder.InternalHasFallbackBuffer ? _encoder.FallbackBuffer.Remaining : 0
        // However the property is internal to coreclr, so we'll use max 2 char in fallback if flushing encoder
        private int FallbackBufferRemaining(bool flushEncoder) => flushEncoder ? _maxFallbackBufferCount : 0;

        public virtual bool AutoFlush
        {
            get { return _autoFlush; }

            set
            {
                CheckAsyncTaskInProgress();

                _autoFlush = value;
                if (value)
                {
                    Debug.Assert(_rentedCharBuffer != null || _charPos == 0);
                    Flush(_rentedCharBuffer, 0, _charPos, flushStream: true, flushEncoder: false);
                }
            }
        }

        public virtual Stream BaseStream
        {
            get { return _stream; }
        }

        internal bool LeaveOpen
        {
            get { return !_closable; }
        }

        internal bool HaveWrittenPreamble
        {
            set { _haveWrittenPreamble = value; }
        }

        public override Encoding Encoding
        {
            get { return _encoding; }
        }

        public override void Write(char value)
        {
            CheckAsyncTaskInProgress();

            int charPos = _charPos;
            char[] charBuffer = CharBuffer;
            if (charPos == _charLen)
            {
                Flush(charBuffer, 0, charPos, flushStream: false, flushEncoder: false);
                charPos = 0;
            }

            charBuffer[charPos] = value;
            charPos++;
            _charPos = charPos;
            if (_autoFlush)
            {
                Flush(charBuffer, 0, charPos, flushStream: true, flushEncoder: false);
            }
        }

        public override void Write(char[] buffer)
        {
            // This may be faster than the one with the index & count since it
            // has to do less argument checking.
            if (buffer == null)
            {
                return;
            }

            WriteInternal(buffer, 0, buffer.Length);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), SR.ArgumentNull_Buffer);
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (buffer.Length - index < count)
            {
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            }

            WriteInternal(buffer, index, count);
        }

        private void WriteInternal(char[] buffer, int index, int count)
        {
            CheckAsyncTaskInProgress();

            // Threshold of 4 was chosen after running perf tests
            int charPos = _charPos;
            int charLen = _charLen;
            char[] charBuffer = null;
            if (count <= 4)
            {
                charBuffer = CharBuffer;
                while (count > 0)
                {
                    if (charPos == charLen)
                    {
                        Flush(charBuffer, 0, charPos, flushStream: false, flushEncoder: false);
                        charPos = 0;
                    }

                    Debug.Assert(charLen - charPos > 0, "StreamWriter::Write(char[]) isn't making progress!  This is most likely a race in user code.");
                    charBuffer[charPos] = buffer[index];
                    charPos++;
                    index++;
                    count--;
                }

                _charPos = charPos;
            }
            else
            {
                // Loop entry checked by if
                do
                {
                    if (charPos == charLen)
                    {
                        if (charBuffer == null)
                        {
                            // Haven't rented buffer yet, get it now.
                            charBuffer = CharBuffer;
                        }
                        Flush(charBuffer, 0, charPos, flushStream: false, flushEncoder: false);
                        charPos = 0;
                    }

                    int n;
                    if (charPos == 0 && count >= DontCopyOnWriteThreshold)
                    {
                        // Flush using input buffer directly
                        n = count;
                        if (n > charLen)
                        {
                            // Don't flush more than the byteBuffer can hold
                            n = charLen;
                        }

                        Flush(buffer, index, n, flushStream: false, flushEncoder: false);
                    }
                    else
                    {
                        if (charBuffer == null)
                        {
                            // Haven't rented buffer yet, get it now.
                            charBuffer = CharBuffer;
                        }
                        
                        n = charLen - charPos;
                        if (n > count)
                        {
                            n = count;
                        }

                        Debug.Assert(n > 0, "StreamWriter::Write(char[], int, int) isn't making progress!  This is most likely a race condition in user code.");
                        Buffer.BlockCopy(buffer, index * sizeof(char), charBuffer, charPos * sizeof(char), n * sizeof(char));
                        charPos += n;
                    }

                    index += n;
                    count -= n;
                } while (count > 0);

                _charPos = charPos;
            }

            if (_autoFlush)
            {
                Flush(charBuffer, 0, charPos, flushStream: true, flushEncoder: false);
            }
        }

        public override void Write(string value)
        {
            WriteInternal(value);

            if (_autoFlush)
            {
                Debug.Assert(_rentedCharBuffer != null || _charPos == 0);
                Flush(_rentedCharBuffer, 0, _charPos, flushStream: true, flushEncoder: false);
            }
        }

        //
        // Optimize the most commonly used WriteLine overload. This optimization is important for System.Console in particular
        // because of it will make one WriteLine equal to one call to the OS instead of two in the common case.
        //
        public override void WriteLine(string value)
        {
            WriteInternal(value);

            // Threshold of 4 was chosen after running perf tests
            char[] charBuffer = CharBuffer;
            int charPos = _charPos;
            int charLen = _charLen;

            char[] coreNewLine = CoreNewLine;
            for (int i = 0; i < coreNewLine.Length; i++)   // Expect 2 iterations, no point calling BlockCopy
            {
                if (charPos == charLen)
                {
                    Flush(charBuffer, 0, charPos, flushStream: false, flushEncoder: false);
                    charPos = 0;
                }

                charBuffer[charPos] = coreNewLine[i];
                charPos++;
            }

            _charPos = charPos;

            if (_autoFlush)
            {
                Flush(charBuffer, 0, charPos, flushStream: true, flushEncoder: false);
            }
        }

        private void WriteInternal(string value)
        {
            if (value == null)
            {
                return;
            }

            CheckAsyncTaskInProgress();

            // Threshold of 4 was chosen after running perf tests
            char[] charBuffer = CharBuffer;
            int charPos = _charPos;
            int charLen = _charLen;
            if (value.Length <= 4)
            {
                foreach (char ch in value)
                {
                    if (charPos == charLen)
                    {
                        Flush(charBuffer, 0, charPos, flushStream: false, flushEncoder: false);
                        charPos = 0;
                    }

                    Debug.Assert(charLen - charPos > 0, "StreamWriter::Write(String) isn't making progress!  This is most likely a race condition in user code.");
                    charBuffer[charPos] = ch;
                    charPos++;
                }

                _charPos = charPos;
            }
            else
            {
                int count = value.Length;
                int index = 0;
                while (count > 0)
                {
                    if (charPos == charLen)
                    {
                        Flush(charBuffer, 0, charPos, flushStream: false, flushEncoder: false);
                        charPos = 0;
                    }

                    int n = charLen - charPos;
                    if (n > count)
                    {
                        n = count;
                    }

                    Debug.Assert(n > 0, "StreamWriter::Write(String) isn't making progress!  This is most likely a race condition in user code.");
                    value.CopyTo(index, charBuffer, charPos, n);
                    charPos += n;
                    index += n;
                    count -= n;
                }

                _charPos = charPos;
            }
        }

        #region Task based Async APIs
        public override Task WriteAsync(char value)
        {
            // If we have been inherited into a subclass, the following implementation could be incorrect
            // since it does not call through to Write() which a subclass might have overridden.  
            // To be safe we will only use this implementation in cases where we know it is safe to do so,
            // and delegate to our base class (which will call into Write) when we are not sure.
            if (GetType() != typeof(StreamWriter))
            {
                return base.WriteAsync(value);
            }

            if (_stream == null)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_WriterClosed);
            }

            CheckAsyncTaskInProgress();

            Task task = WriteAsyncInternal(this, value, CharBuffer, _charPos, _charLen, CoreNewLine, _autoFlush, appendNewLine: false);
            _asyncWriteTask = task;

            return task;
        }

        // We pass in private instance fields of this MarshalByRefObject-derived type as local params
        // to ensure performant access inside the state machine that corresponds this async method.
        // Fields that are written to must be assigned at the end of the method *and* before instance invocations.
        private static async Task WriteAsyncInternal(StreamWriter _this, char value,
                                                     char[] charBuffer, int charPos, int charLen, char[] coreNewLine,
                                                     bool autoFlush, bool appendNewLine)
        {
            if (charPos == charLen)
            {
                await _this.FlushAsyncInternal(false, false, charBuffer, charPos).ConfigureAwait(false);
                Debug.Assert(_this._charPos == 0);
                charPos = 0;
            }

            charBuffer[charPos] = value;
            charPos++;

            if (appendNewLine)
            {
                for (int i = 0; i < coreNewLine.Length; i++)   // Expect 2 iterations, no point calling BlockCopy
                {
                    if (charPos == charLen)
                    {
                        await _this.FlushAsyncInternal(false, false, charBuffer, charPos).ConfigureAwait(false);
                        Debug.Assert(_this._charPos == 0);
                        charPos = 0;
                    }

                    charBuffer[charPos] = coreNewLine[i];
                    charPos++;
                }
            }

            if (autoFlush)
            {
                await _this.FlushAsyncInternal(true, false, charBuffer, charPos).ConfigureAwait(false);
                Debug.Assert(_this._charPos == 0);
                charPos = 0;
            }

            _this.CharPos_Prop = charPos;
        }

        public override Task WriteAsync(string value)
        {
            // If we have been inherited into a subclass, the following implementation could be incorrect
            // since it does not call through to Write() which a subclass might have overridden.  
            // To be safe we will only use this implementation in cases where we know it is safe to do so,
            // and delegate to our base class (which will call into Write) when we are not sure.
            if (GetType() != typeof(StreamWriter))
            {
                return base.WriteAsync(value);
            }

            if (value != null)
            {
                if (_stream == null)
                {
                    throw new ObjectDisposedException(null, SR.ObjectDisposed_WriterClosed);
                }

                CheckAsyncTaskInProgress();

                Task task = WriteAsyncInternal(this, value, CharBuffer, _charPos, _charLen, CoreNewLine, _autoFlush, appendNewLine: false);
                _asyncWriteTask = task;

                return task;
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        // We pass in private instance fields of this MarshalByRefObject-derived type as local params
        // to ensure performant access inside the state machine that corresponds this async method.
        // Fields that are written to must be assigned at the end of the method *and* before instance invocations.
        private static async Task WriteAsyncInternal(StreamWriter _this, string value,
                                                     char[] charBuffer, int charPos, int charLen, char[] coreNewLine,
                                                     bool autoFlush, bool appendNewLine)
        {
            Debug.Assert(value != null);

            int count = value.Length;
            int index = 0;

            while (count > 0)
            {
                if (charPos == charLen)
                {
                    await _this.FlushAsyncInternal(false, false, charBuffer, charPos).ConfigureAwait(false);
                    Debug.Assert(_this._charPos == 0);
                    charPos = 0;
                }

                int n = charLen - charPos;
                if (n > count)
                {
                    n = count;
                }

                Debug.Assert(n > 0, "StreamWriter::Write(String) isn't making progress!  This is most likely a race condition in user code.");

                value.CopyTo(index, charBuffer, charPos, n);

                charPos += n;
                index += n;
                count -= n;
            }

            if (appendNewLine)
            {
                for (int i = 0; i < coreNewLine.Length; i++)   // Expect 2 iterations, no point calling BlockCopy
                {
                    if (charPos == charLen)
                    {
                        await _this.FlushAsyncInternal(false, false, charBuffer, charPos).ConfigureAwait(false);
                        Debug.Assert(_this._charPos == 0);
                        charPos = 0;
                    }

                    charBuffer[charPos] = coreNewLine[i];
                    charPos++;
                }
            }

            if (autoFlush)
            {
                await _this.FlushAsyncInternal(true, false, charBuffer, charPos).ConfigureAwait(false);
                Debug.Assert(_this._charPos == 0);
                charPos = 0;
            }

            _this.CharPos_Prop = charPos;
        }

        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), SR.ArgumentNull_Buffer);
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (buffer.Length - index < count)
            {
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            }

            // If we have been inherited into a subclass, the following implementation could be incorrect
            // since it does not call through to Write() which a subclass might have overridden.  
            // To be safe we will only use this implementation in cases where we know it is safe to do so,
            // and delegate to our base class (which will call into Write) when we are not sure.
            if (GetType() != typeof(StreamWriter))
            {
                return base.WriteAsync(buffer, index, count);
            }

            if (_stream == null)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_WriterClosed);
            }

            CheckAsyncTaskInProgress();

            Task task = WriteAsyncInternal(this, buffer, index, count, CharBuffer, _charPos, _charLen, CoreNewLine, _autoFlush, appendNewLine: false);
            _asyncWriteTask = task;

            return task;
        }

        // We pass in private instance fields of this MarshalByRefObject-derived type as local params
        // to ensure performant access inside the state machine that corresponds this async method.
        // Fields that are written to must be assigned at the end of the method *and* before instance invocations.
        private static async Task WriteAsyncInternal(StreamWriter _this, char[] buffer, int index, int count,
                                                     char[] charBuffer, int charPos, int charLen, char[] coreNewLine,
                                                     bool autoFlush, bool appendNewLine)
        {
            Debug.Assert(count == 0 || (count > 0 && buffer != null));
            Debug.Assert(index >= 0);
            Debug.Assert(count >= 0);
            Debug.Assert(buffer == null || (buffer != null && buffer.Length - index >= count));

            while (count > 0)
            {
                if (charPos == charLen)
                {
                    await _this.FlushAsyncInternal(false, false, charBuffer, charPos).ConfigureAwait(false);
                    Debug.Assert(_this._charPos == 0);
                    charPos = 0;
                }

                int n = charLen - charPos;
                if (n > count)
                {
                    n = count;
                }

                Debug.Assert(n > 0, "StreamWriter::Write(char[], int, int) isn't making progress!  This is most likely a race condition in user code.");

                Buffer.BlockCopy(buffer, index * sizeof(char), charBuffer, charPos * sizeof(char), n * sizeof(char));

                charPos += n;
                index += n;
                count -= n;
            }

            if (appendNewLine)
            {
                for (int i = 0; i < coreNewLine.Length; i++)   // Expect 2 iterations, no point calling BlockCopy
                {
                    if (charPos == charLen)
                    {
                        await _this.FlushAsyncInternal(false, false, charBuffer, charPos).ConfigureAwait(false);
                        Debug.Assert(_this._charPos == 0);
                        charPos = 0;
                    }

                    charBuffer[charPos] = coreNewLine[i];
                    charPos++;
                }
            }

            if (autoFlush)
            {
                await _this.FlushAsyncInternal(true, false, charBuffer, charPos).ConfigureAwait(false);
                Debug.Assert(_this._charPos == 0);
                charPos = 0;
            }

            _this.CharPos_Prop = charPos;
        }

        public override Task WriteLineAsync()
        {
            // If we have been inherited into a subclass, the following implementation could be incorrect
            // since it does not call through to Write() which a subclass might have overridden.  
            // To be safe we will only use this implementation in cases where we know it is safe to do so,
            // and delegate to our base class (which will call into Write) when we are not sure.
            if (GetType() != typeof(StreamWriter))
            {
                return base.WriteLineAsync();
            }

            if (_stream == null)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_WriterClosed);
            }

            CheckAsyncTaskInProgress();

            Task task = WriteAsyncInternal(this, null, 0, 0, CharBuffer, _charPos, _charLen, CoreNewLine, _autoFlush, appendNewLine: true);
            _asyncWriteTask = task;

            return task;
        }


        public override Task WriteLineAsync(char value)
        {
            // If we have been inherited into a subclass, the following implementation could be incorrect
            // since it does not call through to Write() which a subclass might have overridden.  
            // To be safe we will only use this implementation in cases where we know it is safe to do so,
            // and delegate to our base class (which will call into Write) when we are not sure.
            if (GetType() != typeof(StreamWriter))
            {
                return base.WriteLineAsync(value);
            }

            if (_stream == null)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_WriterClosed);
            }

            CheckAsyncTaskInProgress();

            Task task = WriteAsyncInternal(this, value, CharBuffer, _charPos, _charLen, CoreNewLine, _autoFlush, appendNewLine: true);
            _asyncWriteTask = task;

            return task;
        }


        public override Task WriteLineAsync(string value)
        {
            if (value == null)
            {
                return WriteLineAsync();
            }

            // If we have been inherited into a subclass, the following implementation could be incorrect
            // since it does not call through to Write() which a subclass might have overridden.  
            // To be safe we will only use this implementation in cases where we know it is safe to do so,
            // and delegate to our base class (which will call into Write) when we are not sure.
            if (GetType() != typeof(StreamWriter))
            {
                return base.WriteLineAsync(value);
            }

            if (_stream == null)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_WriterClosed);
            }

            CheckAsyncTaskInProgress();

            Task task = WriteAsyncInternal(this, value, CharBuffer, _charPos, _charLen, CoreNewLine, _autoFlush, appendNewLine: true);
            _asyncWriteTask = task;

            return task;
        }


        public override Task WriteLineAsync(char[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), SR.ArgumentNull_Buffer);
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (buffer.Length - index < count)
            {
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            }

            // If we have been inherited into a subclass, the following implementation could be incorrect
            // since it does not call through to Write() which a subclass might have overridden.  
            // To be safe we will only use this implementation in cases where we know it is safe to do so,
            // and delegate to our base class (which will call into Write) when we are not sure.
            if (GetType() != typeof(StreamWriter))
            {
                return base.WriteLineAsync(buffer, index, count);
            }

            if (_stream == null)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_WriterClosed);
            }

            CheckAsyncTaskInProgress();

            Task task = WriteAsyncInternal(this, buffer, index, count, CharBuffer, _charPos, _charLen, CoreNewLine, _autoFlush, appendNewLine: true);
            _asyncWriteTask = task;

            return task;
        }


        public override Task FlushAsync()
        {
            // If we have been inherited into a subclass, the following implementation could be incorrect
            // since it does not call through to Flush() which a subclass might have overridden.  To be safe 
            // we will only use this implementation in cases where we know it is safe to do so,
            // and delegate to our base class (which will call into Flush) when we are not sure.
            if (GetType() != typeof(StreamWriter))
            {
                return base.FlushAsync();
            }

            // flushEncoder should be true at the end of the file and if
            // the user explicitly calls Flush (though not if AutoFlush is true).
            // This is required to flush any dangling characters from our UTF-7 
            // and UTF-8 encoders.  
            if (_stream == null)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_WriterClosed);
            }

            CheckAsyncTaskInProgress();

            Task task = FlushAsyncInternal(true, true, CharBuffer, _charPos);
            _asyncWriteTask = task;

            return task;
        }

        private int CharPos_Prop
        {
            set { _charPos = value; }
        }

        private bool HaveWrittenPreamble_Prop
        {
            set { _haveWrittenPreamble = value; }
        }

        private Task FlushAsyncInternal(bool flushStream, bool flushEncoder,
                                        char[] sCharBuffer, int sCharPos)
        {
            // Perf boost for Flush on non-dirty writers.
            if (sCharPos == 0 && !flushStream && !flushEncoder)
            {
                return Task.CompletedTask;
            }

            Task flushTask = FlushAsyncInternal(this, flushStream, flushEncoder, sCharBuffer, sCharPos, _haveWrittenPreamble,
                                                _encoding, _encoder, _stream);

            _charPos = 0;
            return flushTask;
        }


        // We pass in private instance fields of this MarshalByRefObject-derived type as local params
        // to ensure performant access inside the state machine that corresponds this async method.
        private static async Task FlushAsyncInternal(StreamWriter _this, bool flushStream, bool flushEncoder,
                                                     char[] charBuffer, int charPos, bool haveWrittenPreamble,
                                                     Encoding encoding, Encoder encoder, Stream stream)
        {
            if (!haveWrittenPreamble)
            {
                _this.HaveWrittenPreamble_Prop = true;
                byte[] preamble = encoding.GetPreamble();
                if (preamble.Length > 0)
                {
                    await stream.WriteAsync(preamble, 0, preamble.Length).ConfigureAwait(false);
                }
            }

            byte[] byteBuffer = _this.ByteBuffer;
            int count = encoder.GetBytes(charBuffer, 0, charPos, byteBuffer, 0, flushEncoder);
            if (count > 0)
            {
                await stream.WriteAsync(byteBuffer, 0, count).ConfigureAwait(false);
            }

            // By definition, calling Flush should flush the stream, but this is
            // only necessary if we passed in true for flushStream.  The Web
            // Services guys have some perf tests where flushing needlessly hurts.
            if (flushStream)
            {
                _this.ReturnBuffers();
                await stream.FlushAsync().ConfigureAwait(false);
            }
        }
        #endregion

        private char[] CharBuffer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _rentedCharBuffer ?? RentCharBuffer();
        }

        private byte[] ByteBuffer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _rentedByteBuffer ?? RentByteBuffer();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private char[] RentCharBuffer() => _rentedCharBuffer = ArrayPool<char>.Shared.Rent(_charLen);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private byte[] RentByteBuffer() => _rentedByteBuffer = ArrayPool<byte>.Shared.Rent(_byteLen);

        private void ReturnBuffers()
        {
            byte[] byteBuffer = Interlocked.Exchange(ref _rentedByteBuffer, null);
            if (byteBuffer != null)
            {
                ArrayPool<byte>.Shared.Return(byteBuffer);
            }
            char[] charBuffer = Interlocked.Exchange(ref _rentedCharBuffer, null);
            if (charBuffer != null)
            {
                ArrayPool<char>.Shared.Return(charBuffer);
            }
        }
    }  // class StreamWriter
}  // namespace
