// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.Net.Sockets;

namespace System.IO.Ports
{
    internal sealed partial class SerialStream : Stream
    {
        private const int MaxDataBits = 8;
        private const int MinDataBits = 5;

        // members supporting properties exposed to SerialPort
        private string _portName;
        private bool _inBreak = false;
        private Handshake _handshake;

#pragma warning disable CS0067 // Events shared by Windows and Linux, on Linux we currently never call them
        // called when any of the pin/ring-related triggers occurs
        internal event SerialPinChangedEventHandler PinChanged;
        // called when any runtime error occurs on the port (frame, overrun, parity, etc.)
        internal event SerialErrorReceivedEventHandler ErrorReceived;
#pragma warning restore CS0067

        // ----SECTION: inherited properties from Stream class ------------*

        // These six properites are required for SerialStream to inherit from the abstract Stream class.
        // Note four of them are always true or false, and two of them throw exceptions, so these
        // are not usefully queried by applications which know they have a SerialStream, etc...
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

        public override void Write(byte[] array, int offset, int count)
        {
            Write(array, offset, count, WriteTimeout);
        }

        ~SerialStream()
        {
            Dispose(false);
        }

        private void CheckReadWriteArguments(byte[] array, int offset, int count)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNumRequired);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNumRequired);
            if (array.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            if (_handle == null)
                InternalResources.FileNotOpen();
        }

        private void CheckWriteArguments(byte[] array, int offset, int count)
        {
            if (_inBreak)
                throw new InvalidOperationException(SR.In_Break_State);

            CheckReadWriteArguments(array, offset, count);
        }
    }
}
