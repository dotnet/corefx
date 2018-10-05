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
        private ConcurrentQueue<SerialStreamAsyncResult> _readQueue = new ConcurrentQueue<SerialStreamAsyncResult>();
        private ConcurrentQueue<SerialStreamAsyncResult> _writeQueue = new ConcurrentQueue<SerialStreamAsyncResult>();

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
                    throw new ObjectDisposedException(SR.Port_not_open);
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
                    throw new ObjectDisposedException(SR.Port_not_open);
                }
                _writeTimeout = value;
            }
        }

        static void CheckBaudRate(int baudRate)
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
                        throw new IOException();
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
                    throw new IOException();
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
                    throw new IOException();
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
                    throw new IOException();
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
                    throw new IOException();
                }

                return status == 1;
            }

            set
            {
                if (Interop.Termios.TermiosGetSignal(_handle, Interop.Termios.Signals.SignalDtr, value ? 1 : 0) != 0)
                {
                    throw new IOException();
                }
            }
        }

        private bool RtsEnabledNative()
        {
            int status = Interop.Termios.TermiosGetSignal(_handle, Interop.Termios.Signals.SignalRts);
            if (status < 0)
            {
                throw new IOException();
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
                    throw new IOException();
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
            if (_handle == null) throw new ObjectDisposedException(SR.Port_not_open);
            // This may or may not work depending on hardware.
            Interop.Termios.TermiosDiscard(_handle, Interop.Termios.Queue.ReceiveQueue);
        }

        internal void DiscardOutBuffer()
        {
            if (_handle == null) throw new ObjectDisposedException(SR.Port_not_open);
            // This may or may not work depending on hardware.
            Interop.Termios.TermiosDiscard(_handle, Interop.Termios.Queue.SendQueue);
        }

        internal void SetBufferSizes(int readBufferSize, int writeBufferSize)
        {
            if (_handle == null) throw new ObjectDisposedException(SR.Port_not_open);

            // Ignore for now.
        }

        internal bool IsOpen => _handle != null;


        // Flush dumps the contents of the serial driver's internal read and write buffers.
        // We actually expose the functionality for each, but fulfilling Stream's contract
        // requires a Flush() method.  Fails if handle closed.
        // Note: Serial driver's write buffer is *already* attempting to write it, so we can only wait until it finishes.
        public override void Flush()
        {
            if (_handle == null) throw new ObjectDisposedException(SR.Port_not_open);
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
            Task<int> t = ReadAsync(array, offset, count, GetCancellationTokenFromTimeout(timeout));

            try
            {
                return t.Result;
            }
            catch (AggregateException ae)
            {
                if (ae.InnerException is TaskCanceledException)
                {
                    throw new TimeoutException();
                }

                throw ae.InnerException;
            }
        }

        private IAsyncResult BeginReadCore(int timeout, Memory<byte> buffer, AsyncCallback userCallback, object stateObject)
        {
            SerialStreamAsyncResult result = new SerialStreamAsyncResult(
                GetCancellationTokenFromTimeout(timeout),
                buffer);

            _readQueue.Enqueue(result);

            return TaskToApm.Begin((Task)result.UnderlyingTask, userCallback, stateObject);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
                throw new ArgumentNullException(nameof(asyncResult));

            try
            {
                return TaskToApm.End<int>(asyncResult);
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException();
            }
        }

        public override Task<int> ReadAsync(byte[] array, int offset, int count, CancellationToken cancellationToken)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNumRequired);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNumRequired);
            if (array.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            if (count == 0)
                return Task<int>.FromResult(0); // return immediately if no bytes requested; no need for overhead.

            if (_handle == null)
                throw new ObjectDisposedException(SR.Port_not_open);

            Memory<byte> buffer = new Memory<byte>(array, offset, count);
            SerialStreamAsyncResult result = new SerialStreamAsyncResult(cancellationToken, buffer);
            _readQueue.Enqueue(result);

            return result.UnderlyingTask;
        }

        public override Task WriteAsync(byte[] array, int offset, int count, CancellationToken cancellationToken)
        {
            if (_inBreak)
                throw new InvalidOperationException(SR.In_Break_State);
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedPosNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedPosNum);
            if (array.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            if (count == 0)
                return Task.CompletedTask; // return immediately if no bytes to write; no need for overhead.

            // check for open handle, though the port is always supposed to be open
            if (_handle == null)
                throw new ObjectDisposedException(SR.Port_not_open);

            Memory<byte> buffer = new Memory<byte>(array, offset, count);
            SerialStreamAsyncResult result = new SerialStreamAsyncResult(cancellationToken, buffer);
            _writeQueue.Enqueue(result);
            return result.UnderlyingTask;
        }

        public override IAsyncResult BeginRead(byte[] array, int offset, int numBytes, AsyncCallback userCallback, object stateObject)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNumRequired);
            if (numBytes < 0)
                throw new ArgumentOutOfRangeException(nameof(numBytes), SR.ArgumentOutOfRange_NeedNonNegNumRequired);
            if (array.Length - offset < numBytes)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            if (_handle == null)
                throw new ObjectDisposedException(SR.Port_not_open);

            Memory<byte> buffer = new Memory<byte>(array, offset, numBytes);
            return BeginReadCore(SerialPort.InfiniteTimeout, buffer, userCallback, stateObject);
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
            Task t = WriteAsync(array, offset, count, GetCancellationTokenFromTimeout(timeout));

            try
            {
                t.Wait();
            }
            catch (AggregateException ae)
            {
                if (ae.InnerException is TaskCanceledException)
                {
                    throw new TimeoutException();
                }

                throw ae.InnerException;
            }
        }

        public override IAsyncResult BeginWrite(byte[] array, int offset, int count, AsyncCallback userCallback, object stateObject)
        {
            if (_inBreak)
                throw new InvalidOperationException(SR.In_Break_State);
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedPosNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedPosNum);
            if (array.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            // check for open handle, though the port is always supposed to be open
            if (_handle == null)
                throw new ObjectDisposedException(SR.Port_not_open);

            Memory<byte> buffer = new Memory<byte>(array, offset, count);
            return BeginWriteCore(SerialPort.InfiniteTimeout, buffer, userCallback, stateObject);
        }

        private IAsyncResult BeginWriteCore(int timeout, Memory<byte> buffer, AsyncCallback userCallback, object stateObject)
        {
            SerialStreamAsyncResult result = new SerialStreamAsyncResult(
                GetCancellationTokenFromTimeout(timeout),
                buffer);
            _writeQueue.Enqueue(result);

            return TaskToApm.Begin((Task)result.UnderlyingTask, userCallback, stateObject);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
                throw new ArgumentNullException(nameof(asyncResult));

            try
            {
                TaskToApm.End(asyncResult);
            }
            catch (TaskCanceledException)
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

            _ioLoop = Task.Factory.StartNew(IOLoop, TaskCreationOptions.LongRunning);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _ioLoopFinished = true;
                _ioLoop?.Wait();
                _ioLoop = null;

                while (_readQueue.TryDequeue(out SerialStreamAsyncResult r))
                {
                    r.Complete(new ObjectDisposedException(SR.Port_not_open));
                }

                while (_writeQueue.TryDequeue(out SerialStreamAsyncResult r))
                {
                    r.Complete(new ObjectDisposedException(SR.Port_not_open));
                }

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
        private delegate int RequestProcessor(SerialStreamAsyncResult r);

        private unsafe int ProcessRead(SerialStreamAsyncResult r)
        {
            Span<byte> buff = r.Buffer.Span;
            fixed (byte* bufPtr = buff)
            {
                // assumes dequeue-ing happens on a single thread
                int numBytes = Interop.Sys.Read(_handle, bufPtr, buff.Length);

                Interop.Error lastError = numBytes < 0 ? Interop.Sys.GetLastError() : 0;
                if (numBytes > 0)
                {
                    r.Complete(numBytes);
                    return numBytes;
                }
                else if (numBytes == 0)
                {
                    ThreadPool.QueueUserWorkItem(s => {
                            var thisRef = (SerialStream)s;
                            thisRef.DataReceived(thisRef, new SerialDataReceivedEventArgs(SerialData.Eof));
                        }, this);
                }
                // ignore EWOULDBLOCK since we handle timeout elsewhere
                else if (lastError != Interop.Error.EWOULDBLOCK)
                {
                    r.Complete(new IOException());
                }
            }

            return 0;
        }

        private unsafe int ProcessWrite(SerialStreamAsyncResult r)
        {
            ReadOnlySpan<byte> buff = r.Buffer.Span;
            fixed (byte* bufPtr = buff)
            {
                // assumes dequeue-ing happens on a single thread
                int numBytes = Interop.Sys.Write(_handle, bufPtr, buff.Length);

                Interop.Error lastError = numBytes < 0 ? Interop.Sys.GetLastError() : 0;
                if (numBytes > 0)
                {
                    r.ProcessBytes(numBytes);

                    if (r.Buffer.Length == 0)
                    {
                        r.Complete();
                    }

                    return numBytes;
                }
                // ignore EWOULDBLOCK since we handle timeout elsewhere
                // numBytes == 0 means that there might be an error
                else if (lastError != Interop.Error.SUCCESS && lastError != Interop.Error.EWOULDBLOCK)
                {
                    r.Complete(new IOException());
                }
            }

            return 0;
        }

        // returns number of bytes read/written
        private static int DoIORequest(ConcurrentQueue<SerialStreamAsyncResult> q, RequestProcessor op)
        {
            // assumes dequeue-ing happens on a single thread
            while (q.TryPeek(out SerialStreamAsyncResult r))
            {
                if (r.IsCompleted)
                {
                    Debug.Assert(q.TryDequeue(out _));
                    // take another item since we haven't processed anything
                    continue;
                }

                int ret = op(r);
                Debug.Assert(ret >= 0);

                if (r.IsCompleted)
                {
                    Debug.Assert(q.TryDequeue(out _));
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
                    break;
                }

                if (events.HasFlag(Interop.Sys.PollEvents.POLLIN))
                {
                    int bytesRead = DoIORequest(_readQueue, ProcessRead);
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

                    Task.Factory.StartNew(s => {
                            var thisRef = (SerialStream)s;
                            thisRef.DataReceived(thisRef, new SerialDataReceivedEventArgs(SerialData.Chars));
                        },
                        this,
                        CancellationToken.None,
                        TaskCreationOptions.DenyChildAttach,
                        TaskScheduler.Default);
                }

                if (events.HasFlag(Interop.Sys.PollEvents.POLLOUT))
                {
                    DoIORequest(_writeQueue, ProcessWrite);
                }
            }
        }

        private static CancellationToken GetCancellationTokenFromTimeout(int timeoutMs)
        {
            return timeoutMs == SerialPort.InfiniteTimeout ?
                CancellationToken.None :
                (new CancellationTokenSource(Math.Max(timeoutMs, TimeoutResolution))).Token;
        }

        class SerialStreamAsyncResult
        {
            public Memory<byte> Buffer { get; private set; }
            public bool IsCompleted => _tcs.Task.IsCompleted;
            public Task<int> UnderlyingTask => _tcs.Task;

            private TaskCompletionSource<int> _tcs = new TaskCompletionSource<int>();
            private CancellationToken _cancellationToken;

            public SerialStreamAsyncResult(CancellationToken ct, Memory<byte> buffer)
            {
                _cancellationToken = ct;
                ct.Register(() => _tcs.TrySetCanceled());

                Buffer = buffer;
            }

            internal void Complete()
            {
                Debug.Assert(Buffer.Length == 0);
                _tcs.TrySetResult(Buffer.Length);
            }

            internal void Complete(int numBytes)
            {
                _tcs.TrySetResult(numBytes);
            }

            internal void Complete(Exception exception)
            {
                _tcs.TrySetException(exception);
            }

            internal void ProcessBytes(int numBytes)
            {
                Buffer = Buffer.Slice(numBytes);
            }
        }
    }
}
