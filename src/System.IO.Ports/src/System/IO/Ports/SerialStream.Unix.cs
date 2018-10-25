// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace System.IO.Ports
{
    internal sealed partial class SerialStream : Stream
    {
        private const int TimeoutResolution = 30;
        private bool _ioLoopFinished = false;

        private int _baudRate;
        private StopBits _stopBits;
        private Parity _parity;
        private int _dataBits = 8;
        private bool _rtsEnable = false;
        private int _readTimeout = 0;
        private int _writeTimeout = 0;
        private byte[] _tempBuf = new byte[1];
        private Task _ioLoop;
        private ConcurrentQueue<SerialStreamIORequest> _readQueue = new ConcurrentQueue<SerialStreamIORequest>();
        private ConcurrentQueue<SerialStreamIORequest> _writeQueue = new ConcurrentQueue<SerialStreamIORequest>();

        // ----SECTION: inherited properties from Stream class ------------*

        // These six properties are required for SerialStream to inherit from the abstract Stream class.
        // Note four of them are always true or false, and two of them throw exceptions, so these
        // are not usefully queried by applications which know they have a SerialStream, etc...

        public override int ReadTimeout
        {
            get { return _readTimeout; }
            set
            {
                if (value < 0 && value != SerialPort.InfiniteTimeout)
                    throw new ArgumentOutOfRangeException(nameof(ReadTimeout), SR.ArgumentOutOfRange_Timeout);
                if (_handle == null) {
                    InternalResources.FileNotOpen();
                }
                _readTimeout = value;
            }
        }

        public override int WriteTimeout
        {
            get { return _writeTimeout; }
            set
            {
                if (value < 0 && value != SerialPort.InfiniteTimeout)
                    throw new ArgumentOutOfRangeException(nameof(ReadTimeout), SR.ArgumentOutOfRange_Timeout);
                if (_handle == null) {
                    InternalResources.FileNotOpen();
                }
                _writeTimeout = value;
            }
        }

        private static void CheckBaudRate(int baudRate)
        {
            if (baudRate <= 0 || baudRate > 230400)
            {
                throw new ArgumentOutOfRangeException(nameof(BaudRate), SR.ArgumentOutOfRange_NeedPosNum);
            }
        }

        internal int BaudRate
        {
            set
            {
                if (value != _baudRate)
                {
                    CheckBaudRate(value);

                    if (Interop.Termios.TermiosSetSpeed(_handle, value) < 0)
                    {
                        throw GetLastIOError();
                    }

                    _baudRate = value;
                }
            }

            get
            {
                return Interop.Termios.TermiosGetSpeed(_handle);
            }
        }

        public bool BreakState
        {
            get { return _inBreak; }
            set
            {
                if (value)
                {
                    // Unlike Windows, there is no infinite break and positive value is platform dependent.
                    // As best guess, send break with default duration.
                    Interop.Termios.TermiosSendBreak(_handle, 0);
                }
                _inBreak = value;
            }
        }

        internal int BytesToWrite
        {
            get { return Interop.Termios.TermiosGetAvailableBytes(_handle, Interop.Termios.Queue.SendQueue); }
        }

        internal int BytesToRead
        {
            get { return Interop.Termios.TermiosGetAvailableBytes(_handle, Interop.Termios.Queue.ReceiveQueue); }
        }

        internal bool CDHolding
        {
            get
            {
                int status = Interop.Termios.TermiosGetSignal(_handle, Interop.Termios.Signals.SignalDcd);
                if (status < 0)
                {
                    throw GetLastIOError();
                }

                return status == 1;
            }
        }

        internal bool CtsHolding
        {
            get
            {
                int status = Interop.Termios.TermiosGetSignal(_handle, Interop.Termios.Signals.SignalCts);
                if (status < 0)
                {
                    throw GetLastIOError();
                }

                return status == 1;
            }
        }

        internal bool DsrHolding
        {
            get
            {
                int status = Interop.Termios.TermiosGetSignal(_handle, Interop.Termios.Signals.SignalDsr);
                if (status < 0)
                {
                    throw GetLastIOError();
                }

                return status == 1;
            }
        }

        internal bool DtrEnable
        {
            get
            {
                int status = Interop.Termios.TermiosGetSignal(_handle, Interop.Termios.Signals.SignalDtr);
                if (status < 0)
                {
                    throw GetLastIOError();
                }

                return status == 1;
            }

            set
            {
                if (Interop.Termios.TermiosGetSignal(_handle, Interop.Termios.Signals.SignalDtr, value ? 1 : 0) != 0)
                {
                    throw GetLastIOError();
                }
            }
        }

        private bool RtsEnabledNative()
        {
            int status = Interop.Termios.TermiosGetSignal(_handle, Interop.Termios.Signals.SignalRts);
            if (status < 0)
            {
                throw GetLastIOError();
            }

            return status == 1;
        }

        internal bool RtsEnable
        {
            get
            {
                if ((_handshake == Handshake.RequestToSend || _handshake == Handshake.RequestToSendXOnXOff))
                    throw new InvalidOperationException(SR.CantSetRtsWithHandshaking);

                return RtsEnabledNative();
            }

            set
            {
                if ((_handshake == Handshake.RequestToSend || _handshake == Handshake.RequestToSendXOnXOff))
                    throw new InvalidOperationException(SR.CantSetRtsWithHandshaking);

                if (Interop.Termios.TermiosGetSignal(_handle, Interop.Termios.Signals.SignalRts, value ? 1 : 0) != 0)
                {
                    throw GetLastIOError();
                }
            }
        }

        internal Handshake Handshake
        {
            set
            {
                Debug.Assert(!(value < Handshake.None || value > Handshake.RequestToSendXOnXOff),
                    "An invalid value was passed to Handshake");

                if (value != _handshake)
                {
                    if (Interop.Termios.TermiosReset(_handle, _baudRate, _dataBits, _stopBits, _parity, value) != 0)
                    {
                        throw new ArgumentException();
                    }

                    _handshake = value;
                }
            }
        }

        internal int DataBits
        {
            set
            {
                Debug.Assert(!(value < MinDataBits || value > MaxDataBits), "An invalid value was passed to DataBits");
                if (value != _dataBits)
                {
                    if (Interop.Termios.TermiosReset(_handle, _baudRate, value, _stopBits, _parity, _handshake) != 0)
                    {
                        throw new ArgumentException();
                    }

                    _dataBits = value;
                }
            }
        }

        internal Parity Parity
        {
            set
            {
                Debug.Assert(!(value < Parity.None || value > Parity.Space), "An invalid value was passed to Parity");

                if (value != _parity)
                {
                    if (Interop.Termios.TermiosReset(_handle, _baudRate, _dataBits, _stopBits, value, _handshake) != 0)
                    {
                        throw new ArgumentException();
                    }

                    _parity = value;
                }
            }
        }

        internal StopBits StopBits
        {
            set
            {
                Debug.Assert(!(value < StopBits.One || value > StopBits.OnePointFive), "An invalid value was passed to StopBits");
                if (value != _stopBits)
                {
                    if (Interop.Termios.TermiosReset(_handle, _baudRate, _dataBits, value, _parity, _handshake) != 0)
                    {
                        throw new ArgumentException();
                    }

                    _stopBits = value;
                }
            }
        }

        internal bool DiscardNull
        {
            set
            {
                // Ignore.
            }
        }

        internal byte ParityReplace
        {
            set
            {
                // Ignore.
            }
        }

        internal void DiscardInBuffer()
        {
            if (_handle == null) InternalResources.FileNotOpen();
            // This may or may not work depending on hardware.
            Interop.Termios.TermiosDiscard(_handle, Interop.Termios.Queue.ReceiveQueue);
        }

        internal void DiscardOutBuffer()
        {
            if (_handle == null) InternalResources.FileNotOpen();
            // This may or may not work depending on hardware.
            Interop.Termios.TermiosDiscard(_handle, Interop.Termios.Queue.SendQueue);
        }

        internal void SetBufferSizes(int readBufferSize, int writeBufferSize)
        {
            if (_handle == null) InternalResources.FileNotOpen();

            // Ignore for now.
        }

        internal bool IsOpen => _handle != null;


        // Flush dumps the contents of the serial driver's internal read and write buffers.
        // We actually expose the functionality for each, but fulfilling Stream's contract
        // requires a Flush() method.  Fails if handle closed.
        // Note: Serial driver's write buffer is *already* attempting to write it, so we can only wait until it finishes.
        public override void Flush()
        {
            if (_handle == null) InternalResources.FileNotOpen();
            Interop.Termios.TermiosDiscard(_handle, Interop.Termios.Queue.AllQueues);
        }

        internal int ReadByte(int timeout)
        {
            Read(_tempBuf, 0, 1, timeout);
            return _tempBuf[0];
        }

        public override int Read(byte[] array, int offset, int count)
        {
            return Read(array, offset, count, ReadTimeout);
        }

        internal unsafe int Read(byte[] array, int offset, int count, int timeout)
        {
            using (CancellationTokenSource cts = GetCancellationTokenSourceFromTimeout(timeout))
            {
                Task<int> t = ReadAsync(array, offset, count, cts?.Token ?? CancellationToken.None);

                try
                {
                    return t.GetAwaiter().GetResult();
                }
                catch (OperationCanceledException)
                {
                    throw new TimeoutException();
                }
            }
        }

        public override int EndRead(IAsyncResult asyncResult)
            => EndReadWrite(asyncResult);

        public override Task<int> ReadAsync(byte[] array, int offset, int count, CancellationToken cancellationToken)
        {
            CheckReadWriteArguments(array, offset, count);

            if (count == 0)
                return Task<int>.FromResult(0); // return immediately if no bytes requested; no need for overhead.

            Memory<byte> buffer = new Memory<byte>(array, offset, count);
            SerialStreamIORequest result = new SerialStreamIORequest(cancellationToken, buffer);
            _readQueue.Enqueue(result);

            return result.Task;
        }

        public override Task WriteAsync(byte[] array, int offset, int count, CancellationToken cancellationToken)
        {
            CheckWriteArguments(array, offset, count);

            if (count == 0)
                return Task.CompletedTask; // return immediately if no bytes to write; no need for overhead.

            Memory<byte> buffer = new Memory<byte>(array, offset, count);
            SerialStreamIORequest result = new SerialStreamIORequest(cancellationToken, buffer);
            _writeQueue.Enqueue(result);
            return result.Task;
        }

        public override IAsyncResult BeginRead(byte[] array, int offset, int numBytes, AsyncCallback userCallback, object stateObject)
        {
            return TaskToApm.Begin(ReadAsync(array, offset, numBytes), userCallback, stateObject);
        }

        // Will wait `timeout` miliseconds or until reading or writing is possible
        // If no operation is requested it will wait.
        // Returns event which has happened
        private Interop.Sys.PollEvents PollEvents(int timeout, bool pollReadEvents, bool pollWriteEvents)
        {
            if (!pollReadEvents && !pollWriteEvents)
            {
                Thread.Sleep(timeout);
                return Interop.Sys.PollEvents.POLLNONE;
            }

            Interop.Sys.PollEvents eventsToPoll = Interop.Sys.PollEvents.POLLERR;

            if (pollReadEvents)
            {
                eventsToPoll |= Interop.Sys.PollEvents.POLLIN;
            }

            if (pollWriteEvents)
            {
                eventsToPoll |= Interop.Sys.PollEvents.POLLOUT;
            }

            Interop.Sys.PollEvents events = Interop.Sys.PollEvents.POLLNONE;
            Interop.Sys.Poll(_handle,
                             eventsToPoll,
                             timeout,
                             out events);
            return events;
        }

        internal void Write(byte[] array, int offset, int count, int timeout)
        {
            using (CancellationTokenSource cts = GetCancellationTokenSourceFromTimeout(timeout))
            {
                Task t = WriteAsync(array, offset, count, cts?.Token ?? CancellationToken.None);

                try
                {
                    t.GetAwaiter().GetResult();
                }
                catch (OperationCanceledException)
                {
                    throw new TimeoutException();
                }
            }
        }

        public override IAsyncResult BeginWrite(byte[] array, int offset, int count, AsyncCallback userCallback, object stateObject)
        {
            return TaskToApm.Begin(WriteAsync(array, offset, count), userCallback, stateObject);
        }

        public override void EndWrite(IAsyncResult asyncResult)
            => EndReadWrite(asyncResult);

        private int EndReadWrite(IAsyncResult asyncResult)
        {
            try
            {
                return TaskToApm.End<int>(asyncResult);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException();
            }
        }

        internal SafeFileHandle OpenPort(string portName)
        {
            SafeFileHandle handle = Interop.Serial.SerialPortOpen(portName);
            if (handle.IsInvalid)
            {
                throw new UnauthorizedAccessException(string.Format(SR.UnauthorizedAccess_IODenied_Port, portName));
            }

            return handle;
        }

        // this method is used by SerialPort upon SerialStream's creation
        internal SerialStream(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, int readTimeout, int writeTimeout, Handshake handshake,
            bool dtrEnable, bool rtsEnable, bool discardNull, byte parityReplace)
        {
            if (portName == null)
            {
                throw new ArgumentNullException(nameof(portName));
            }

            CheckBaudRate(baudRate);

            // Error checking done in SerialPort.

            SafeFileHandle tempHandle = OpenPort(portName);

            try
            {
                _handle = tempHandle;
                // set properties of the stream that exist as members in SerialStream
                _portName = portName;
                _handshake = handshake;
                _parity = parity;
                _readTimeout = readTimeout;
                _writeTimeout = writeTimeout;
                _baudRate = baudRate;
                _stopBits = stopBits;
                _dataBits = dataBits;

                if (Interop.Termios.TermiosReset(_handle, _baudRate, _dataBits, _stopBits, _parity, _handshake) != 0)
                {
                    throw new ArgumentException();
                }

                DtrEnable = dtrEnable;
                BaudRate = baudRate;

                // now set this.RtsEnable to the specified value.
                // Handshake takes precedence, this will be a nop if
                // handshake is either RequestToSend or RequestToSendXOnXOff
                if ((handshake != Handshake.RequestToSend && handshake != Handshake.RequestToSendXOnXOff))
                {
                    // query and cache the initial RtsEnable value
                    // so that set_RtsEnable can do the (value != rtsEnable) optimization
                    _rtsEnable = RtsEnabledNative();
                    RtsEnable = rtsEnable;
                }
            }
            catch
            {
                // if there are any exceptions after the call to CreateFile, we need to be sure to close the
                // handle before we let them continue up.
                tempHandle.Close();
                _handle = null;
                throw;
            }

            _processReadDelegate = ProcessRead;
            _processWriteDelegate = ProcessWrite;
            _ioLoop = Task.Factory.StartNew(
                IOLoop,
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        private void FinishPendingIORequests()
        {
            while (_readQueue.TryDequeue(out SerialStreamIORequest r))
            {
                r.Complete(InternalResources.FileNotOpenException());
            }

            while (_writeQueue.TryDequeue(out SerialStreamIORequest r))
            {
                r.Complete(InternalResources.FileNotOpenException());
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _ioLoopFinished = true;
                _ioLoop?.Wait();
                _ioLoop = null;

                FinishPendingIORequests();

                // Signal the other side that we're closing.  Should do regardless of whether we've called
                // Close() or not Dispose()
                if (_handle != null && !_handle.IsInvalid)
                {
                    Interop.Sys.Shutdown(_handle, SocketShutdown.Both);
                    _handle.Close();
                    _handle = null;
                }
            }

            base.Dispose(disposing);
        }

        // should return non-negative integer meaning numbers of bytes read/written (0 for errors)
        private delegate int RequestProcessor(SerialStreamIORequest r);
        private RequestProcessor _processReadDelegate;
        private RequestProcessor _processWriteDelegate;

        private unsafe int ProcessRead(SerialStreamIORequest r)
        {
            Span<byte> buff = r.Buffer.Span;
            fixed (byte* bufPtr = buff)
            {
                // assumes dequeue-ing happens on a single thread
                int numBytes = Interop.Sys.Read(_handle, bufPtr, buff.Length);

                if (numBytes < 0)
                {
                    Interop.ErrorInfo lastError = Interop.Sys.GetLastErrorInfo();

                    // ignore EWOULDBLOCK since we handle timeout elsewhere
                    if (lastError.Error != Interop.Error.EWOULDBLOCK)
                    {
                        r.Complete(Interop.GetIOException(lastError));
                    }
                }
                else if (numBytes > 0)
                {
                    r.Complete(numBytes);
                    return numBytes;
                }
                else // numBytes == 0
                {
                    ThreadPool.QueueUserWorkItem(s => {
                            var thisRef = (SerialStream)s;
                            thisRef.DataReceived(thisRef, new SerialDataReceivedEventArgs(SerialData.Eof));
                        }, this);
                }
            }

            return 0;
        }

        private unsafe int ProcessWrite(SerialStreamIORequest r)
        {
            ReadOnlySpan<byte> buff = r.Buffer.Span;
            fixed (byte* bufPtr = buff)
            {
                // assumes dequeue-ing happens on a single thread
                int numBytes = Interop.Sys.Write(_handle, bufPtr, buff.Length);

                if (numBytes <= 0)
                {
                    Interop.ErrorInfo lastError = Interop.Sys.GetLastErrorInfo();

                    // ignore EWOULDBLOCK since we handle timeout elsewhere
                    // numBytes == 0 means that there might be an error
                    if (lastError.Error != Interop.Error.SUCCESS && lastError.Error != Interop.Error.EWOULDBLOCK)
                    {
                        r.Complete(Interop.GetIOException(lastError));
                    }
                }
                else
                {
                    r.ProcessBytes(numBytes);

                    if (r.Buffer.Length == 0)
                    {
                        r.Complete();
                    }

                    return numBytes;
                }
            }

            return 0;
        }

        // returns number of bytes read/written
        private static int DoIORequest(ConcurrentQueue<SerialStreamIORequest> q, RequestProcessor op)
        {
            // assumes dequeue-ing happens on a single thread
            while (q.TryPeek(out SerialStreamIORequest r))
            {
                if (r.IsCompleted)
                {
                    q.TryDequeue(out _);
                    // take another item since we haven't processed anything
                    continue;
                }

                int ret = op(r);
                Debug.Assert(ret >= 0);

                if (r.IsCompleted)
                {
                    q.TryDequeue(out _);
                }

                return ret;
            }

            return 0;
        }

        private unsafe void IOLoop()
        {
            bool eofReceived = false;
            //bool readyForReceivedEvent = true;
            long totalBytesRead = 0;
            // last value of totalBytesRead + BytesToRead
            long lastTotalBytesAvailable = BytesToRead;
            while (IsOpen && !eofReceived && !_ioLoopFinished)
            {
                // PollEvents will wait if both flags are false
                Interop.Sys.PollEvents events = PollEvents(1,
                                                           pollReadEvents: _readQueue.Count > 0,
                                                           pollWriteEvents: _writeQueue.Count > 0);

                if (events.HasFlag(Interop.Sys.PollEvents.POLLNVAL) ||
                    events.HasFlag(Interop.Sys.PollEvents.POLLERR))
                {
                    // bad descriptor or some other error we can't handle
                    FinishPendingIORequests();
                    break;
                }

                if (events.HasFlag(Interop.Sys.PollEvents.POLLIN))
                {
                    int bytesRead = DoIORequest(_readQueue, _processReadDelegate);
                    totalBytesRead += bytesRead;
                }

                // check if there is any new data (either already read or in the driver input)
                // this event is private and handled inside of SerialPort
                // which then throttles it with the threshold
                long totalBytesAvailable = totalBytesRead + BytesToRead;
                if (totalBytesAvailable > lastTotalBytesAvailable)
                {
                    lastTotalBytesAvailable = totalBytesAvailable;

                    // We need new task so that this thread doesn't get deadlocked when someone calls
                    // Read from within the event

                    ThreadPool.QueueUserWorkItem(s => {
                            var thisRef = (SerialStream)s;
                            thisRef.DataReceived(thisRef, new SerialDataReceivedEventArgs(SerialData.Chars));
                        }, this);
                }

                if (events.HasFlag(Interop.Sys.PollEvents.POLLOUT))
                {
                    DoIORequest(_writeQueue, _processWriteDelegate);
                }
            }
        }

        private static CancellationTokenSource GetCancellationTokenSourceFromTimeout(int timeoutMs)
        {
            return timeoutMs == SerialPort.InfiniteTimeout ?
                null :
                new CancellationTokenSource(Math.Max(timeoutMs, TimeoutResolution));
        }

        private static Exception GetLastIOError()
        {
            return Interop.GetIOException(Interop.Sys.GetLastErrorInfo());
        }

        private class SerialStreamIORequest : TaskCompletionSource<int>
        {
            public Memory<byte> Buffer { get; private set; }
            public bool IsCompleted => Task.IsCompleted;
            private CancellationToken _cancellationToken;

            public SerialStreamIORequest(CancellationToken ct, Memory<byte> buffer)
                : base(TaskCreationOptions.RunContinuationsAsynchronously)
            {
                _cancellationToken = ct;
                ct.Register(s => ((TaskCompletionSource<int>)s).TrySetCanceled(), this);

                Buffer = buffer;
            }

            internal void Complete()
            {
                Debug.Assert(Buffer.Length == 0);
                TrySetResult(Buffer.Length);
            }

            internal void Complete(int numBytes)
            {
                TrySetResult(numBytes);
            }

            internal void Complete(Exception exception)
            {
                TrySetException(exception);
            }

            internal void ProcessBytes(int numBytes)
            {
                Buffer = Buffer.Slice(numBytes);
            }
        }
    }
}
