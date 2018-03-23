// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Text;

namespace System.IO.Ports
{
    internal sealed partial class SerialStream : Stream
    {
        private const int infiniteTimeoutConst = -2;
        private const int MaxDataBits = 8;
        private const int MinDataBits = 5;

        private string _portName;
        private int _baudRate;
        private StopBits _stopBits;
        private Handshake _handshake;
        private Parity _parity;
        private int _dataBits = 8;
        private bool _inBreak = false;
        private SafeFileHandle _handle = null;
        private bool _rtsEnable = false;
        ArrayList selectList;
        private bool _isAsync = false;
        private int _readTimeout = 0;
        private int _writeTimeout = 0;

        // three different events, also wrapped by SerialPort.
        internal event SerialDataReceivedEventHandler DataReceived;      // called when one character is received.
        internal event SerialPinChangedEventHandler PinChanged;    // called when any of the pin/ring-related triggers occurs
        internal event SerialErrorReceivedEventHandler ErrorReceived;         // called when any runtime error occurs on the port (frame, overrun, parity, etc.)

        // ----SECTION: inherited properties from Stream class ------------*

        // These six properties are required for SerialStream to inherit from the abstract Stream class.
        // Note four of them are always true or false, and two of them throw exceptions, so these
        // are not usefully queried by applications which know they have a SerialStream, etc...

        public override int ReadTimeout => _readTimeout;
        public override int WriteTimeout => _writeTimeout;

        public override bool CanRead
        {
            get { return (_handle != null); }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanTimeout
        {
            get { return (_handle != null); }
        }

        public override bool CanWrite
        {
            get { return (_handle != null); }
        }

        public override long Length
        {
            get { throw new NotSupportedException(SR.NotSupported_UnseekableStream); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(SR.NotSupported_UnseekableStream); }
            set { throw new NotSupportedException(SR.NotSupported_UnseekableStream); }
        }

        internal int BaudRate
        {
            set
            {
                if (value != _baudRate)
                {
                    if (Interop.Sys.TerminalSetSpeed(_handle, value) != 0)
                    {
                        throw new IOException();
                    }
                    _baudRate = value;
                }
            }
            get
            {
                return Interop.Sys.TerminalGetSpeed(_handle);
            }
        }

        public bool BreakState
        {
            get { return _inBreak; }
            set
            {
                throw new System.PlatformNotSupportedException(System.SR.PlatformNotSupported_IOPorts);
            }
        }

        internal int BytesToWrite
        {
            get { return 0; }
        }

        internal int BytesToRead
        {
            get { return 0; }
        }

        internal bool CDHolding
        {
            get
            {
                int status = Interop.Sys.TerminalGetCd(_handle);
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
                int status = Interop.Sys.TerminalGetCts(_handle);
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
                int status = Interop.Sys.TerminalGetDsr(_handle);
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
                int status = Interop.Sys.TerminalGetDtr(_handle);
                if (status < 0)
                {
                    throw new IOException();
                }
                return status == 1;
            }
            set
            {
                if (Interop.Sys.TerminalSetDtr(_handle, value ? 1 : 0) != 0)
                {
                    throw new IOException();
                }
            }
        }

        internal bool RtsEnable
        {
            get
            {
                int status = Interop.Sys.TerminalGetRts(_handle);
                if (status < 0)
                {
                    throw new IOException();
                }
                return status == 1;
            }
            set
            {
                if (Interop.Sys.TerminalSetRts(_handle, value ? 1 : 0) != 0)
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

                }
            }
        }

        internal int DataBits
        {
            set
            {
                Debug.Assert(!(value < MinDataBits || value > MaxDataBits), "An invalid value was passed to DataBits");
                _dataBits = value;
            }
        }

        internal Parity Parity
        {
            set
            {
                Debug.Assert(!(value < Parity.None || value > Parity.Space), "An invalid value was passed to Parity");

                if (value != _parity)
                {

                }
            }
        }

        internal StopBits StopBits
        {
            set
            {
                Debug.Assert(!(value < StopBits.One || value > StopBits.OnePointFive), "An invalid value was passed to StopBits");

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

       // Uses Win32 method to dump out the receive buffer; analagous to MSComm's "InBufferCount = 0"
        internal void DiscardInBuffer()
        {
        }

        // Uses Win32 method to dump out the xmit buffer; analagous to MSComm's "OutBufferCount = 0"
        internal void DiscardOutBuffer()
        {
        }

       internal void SetBufferSizes(int readBufferSize, int writeBufferSize)
        {
            if (_handle == null) throw new IOException();
            //IOCTL TIOCSSERIAL 14 max
        }

        internal bool IsOpen => _handle != null;


        // Flush dumps the contents of the serial driver's internal read and write buffers.
        // We actually expose the functionality for each, but fulfilling Stream's contract
        // requires a Flush() method.  Fails if handle closed.
        // Note: Serial driver's write buffer is *already* attempting to write it, so we can only wait until it finishes.
        public override void Flush()
        {
            if (_handle == null) throw new ObjectDisposedException(SR.Port_not_open);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException(SR.NotSupported_UnseekableStream);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException(SR.NotSupported_UnseekableStream);
        }

        public override int ReadByte()
        {
            return ReadByte(ReadTimeout);
        }

        internal unsafe int ReadByte(int timeout)
        {
            if (_handle == null)  throw new IOException();

            selectList.Add(_handle);
            Socket.Select(selectList, null, null, timeout);
            if (selectList.Count == 0)
            {
                throw new TimeoutException();
            }
            byte* tempBuf = stackalloc byte[1];

            int numBytes = Interop.Sys.Read(_handle, tempBuf, 1);
            if (numBytes != 1)
            {
                throw new IOException();
            }
            return tempBuf[0];
        }

        public override int Read(byte[] array, int offset, int count)
        {
            return Read(array, offset, count, ReadTimeout);
        }

        internal unsafe int Read(byte[] array, int offset, int count, int timeout)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNumRequired);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNumRequired);
            if (array.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            if (count == 0) return 0; // return immediately if no bytes requested; no need for overhead.

            Interop.Sys.PollEvents events = Interop.Sys.PollEvents.POLLNONE;
            if (timeout > 0)
            {
                Interop.Sys.Poll(_handle, Interop.Sys.PollEvents.POLLIN | Interop.Sys.PollEvents.POLLERR, timeout,out events);

                if ((events & (Interop.Sys.PollEvents.POLLERR | Interop.Sys.PollEvents.POLLNVAL)) != 0)
                {
                    throw new IOException();
                }
            }

            int numBytes = 0;
            if ( timeout <= 0 || (events & Interop.Sys.PollEvents.POLLIN) != 0)
            {
                fixed (byte* bufPtr = array)
                {
                    numBytes = Interop.Sys.Read(_handle, bufPtr + offset, count);
                }
            }

            if (numBytes == 0)
                throw new TimeoutException();

            return numBytes;
        }

        public override void Write(byte[] array, int offset, int count)
        {
            Write(array, offset, count, WriteTimeout);
        }

        internal unsafe void Write(byte[] array, int offset, int count, int timeout)
        {

            if (_inBreak)
                throw new InvalidOperationException(SR.In_Break_State);
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedPosNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedPosNum);
            if (count == 0) return; // no need to expend overhead in creating asyncResult, etc.
            if (array.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            Debug.Assert(timeout == SerialPort.InfiniteTimeout || timeout >= 0, "Serial Stream Write - write timeout is " + timeout);

            // check for open handle, though the port is always supposed to be open
            if (_handle == null) throw new IOException(); //InternalResources.FileNotOpen();

            int numBytes;
            if (_isAsync)
            {
                throw new NotSupportedException(SR.PlatformNotSupported_IOPorts);
            }
            else
            {
                fixed (byte* bufPtr = array)
                {
                    numBytes = Interop.Sys.Write(_handle, bufPtr, count);
                }

                if (numBytes == -1)
                {
                    throw new IOException();
                }
            }

            if (numBytes == 0)
                throw new TimeoutException(SR.Write_timed_out);

        }

        internal SafeFileHandle OpenPort(string portName)
        {
            // TBD O_NOCTTY | O_NDELAY ???
            SafeFileHandle handle = Interop.Sys.Open(portName, Interop.Sys.OpenFlags.O_RDWR | Interop.Sys.OpenFlags.O_CLOEXEC, 0);

            return handle;
        }

       // this method is used by SerialPort upon SerialStream's creation
        internal SerialStream(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, int readTimeout, int writeTimeout, Handshake handshake,
            bool dtrEnable, bool rtsEnable, bool discardNull, byte parityReplace)
        {

            if (portName == null)
            {
                throw new ArgumentException(SR.Arg_InvalidSerialPort, nameof(portName));
            }

            // Error checking done in SerialPort.

            SafeFileHandle tempHandle = OpenPort(portName);

            if (tempHandle.IsInvalid)
            {
                throw new ArgumentException(SR.Arg_InvalidSerialPort, nameof(portName));
            }

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
                _parity = parity;

                selectList = new ArrayList();
                if (Interop.Sys.TerminalReset(_handle, _baudRate, _dataBits, _stopBits, _parity, _handshake) != 0)
                {
                    throw new ArgumentException();
                }

                DtrEnable = dtrEnable;
                // query and cache the initial RtsEnable value
                // so that set_RtsEnable can do the (value != rtsEnable) optimization
                //_rtsEnable = (GetDcbFlag(NativeMethods.FRTSCONTROL) == NativeMethods.RTS_CONTROL_ENABLE);
                _rtsEnable = RtsEnable;

                BaudRate = baudRate;

                  // now set this.RtsEnable to the specified value.
                // Handshake takes precedence, this will be a nop if
                // handshake is either RequestToSend or RequestToSendXOnXOff
                if ((handshake != Handshake.RequestToSend && handshake != Handshake.RequestToSendXOnXOff))
                    RtsEnable = rtsEnable;

                PinChanged = null;
                ErrorReceived = null;
                DataReceived = null;
            }
            catch
            {
                // if there are any exceptions after the call to CreateFile, we need to be sure to close the
                // handle before we let them continue up.
                tempHandle.Close();
                _handle = null;
                //_threadPoolBinding?.Dispose();
                throw;
            }
        }

        ~SerialStream()
        {
            Dispose(false);
        }

       protected override void Dispose(bool disposing)
        {
            // Signal the other side that we're closing.  Should do regardless of whether we've called
            // Close() or not Dispose()
            if (_handle != null && !_handle.IsInvalid)
            {
                   _handle.Close();
                    _handle = null;
                    if (PinChanged != null || ErrorReceived != null || DataReceived != null)
                    {
                    }
                    base.Dispose(disposing);
            }
        }
    }
}
