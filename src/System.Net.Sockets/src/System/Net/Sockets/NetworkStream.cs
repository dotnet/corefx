// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Sockets
{
    /// <devdoc>
    ///    <para>
    ///       Provides the underlying stream of data for network access.
    ///    </para>
    /// </devdoc>
    public class NetworkStream : Stream
    {
        /// <devdoc>
        ///    <para>
        ///       Used by the class to hold the underlying socket the stream uses.
        ///    </para>
        /// </devdoc>
        private Socket _streamSocket;

        /// <devdoc>
        ///    <para>
        ///       Used by the class to indicate that the stream is m_Readable.
        ///    </para>
        /// </devdoc>
        private bool _readable;

        /// <devdoc>
        ///    <para>
        ///       Used by the class to indicate that the stream is writable.
        ///    </para>
        /// </devdoc>
        private bool _writeable;

        private bool _ownsSocket;

        /// <devdoc>
        /// <para>Creates a new instance of the <see cref='System.Net.Sockets.NetworkStream'/> without initalization.</para>
        /// </devdoc>
        internal NetworkStream()
        {
            _ownsSocket = true;
        }


        // Can be constructed directly out of a socket
        /// <devdoc>
        /// <para>Creates a new instance of the <see cref='System.Net.Sockets.NetworkStream'/> class for the specified <see cref='System.Net.Sockets.Socket'/>.</para>
        /// </devdoc>
        public NetworkStream(Socket socket)
        {
#if DEBUG
            using (GlobalLog.SetThreadKind(ThreadKinds.User))
            {
#endif
                if (socket == null)
                {
                    throw new ArgumentNullException("socket");
                }
                InitNetworkStream(socket);
#if DEBUG
            }
#endif
        }

        //UEUE (see FileStream)
        // ownsHandle: true if the file handle will be owned by this NetworkStream instance; otherwise, false.
        public NetworkStream(Socket socket, bool ownsSocket)
        {
#if DEBUG
            using (GlobalLog.SetThreadKind(ThreadKinds.User))
            {
#endif
                if (socket == null)
                {
                    throw new ArgumentNullException("socket");
                }
                InitNetworkStream(socket);
                _ownsSocket = ownsSocket;
#if DEBUG
            }
#endif
        }

        internal NetworkStream(NetworkStream networkStream, bool ownsSocket)
        {
            Socket socket = networkStream.Socket;
            if (socket == null)
            {
                throw new ArgumentNullException("networkStream");
            }
            InitNetworkStream(socket);
            _ownsSocket = ownsSocket;
        }

        //
        // Socket - provides access to socket for stream closing
        //
        protected Socket Socket
        {
            get
            {
                return _streamSocket;
            }
        }

        internal Socket InternalSocket
        {
            get
            {
                Socket chkSocket = _streamSocket;
                if (_cleanedUp || chkSocket == null)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                return chkSocket;
            }
        }

        internal void InternalAbortSocket()
        {
            if (!_ownsSocket)
            {
                throw new InvalidOperationException();
            }

            Socket chkSocket = _streamSocket;
            if (_cleanedUp || chkSocket == null)
            {
                return;
            }

            try
            {
                chkSocket.Close(0);
            }
            catch (ObjectDisposedException)
            {
            }
        }

        internal void ConvertToNotSocketOwner()
        {
            _ownsSocket = false;
            // Suppress for finialization still allow proceed the requests
            GC.SuppressFinalize(this);
        }

        /// <devdoc>
        ///    <para>
        ///       Used by the class to indicate that the stream is m_Readable.
        ///    </para>
        /// </devdoc>
        protected bool Readable
        {
            get
            {
                return _readable;
            }
            set
            {
                _readable = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Used by the class to indicate that the stream is writable.
        ///    </para>
        /// </devdoc>
        protected bool Writeable
        {
            get
            {
                return _writeable;
            }
            set
            {
                _writeable = value;
            }
        }


        /// <devdoc>
        ///    <para>
        ///       Indicates that data can be read from the stream.
        ///         We return the readability of this stream. This is a read only property.
        ///    </para>
        /// </devdoc>
        public override bool CanRead
        {
            get
            {
                return _readable;
            }
        }


        /// <devdoc>
        ///    <para>
        ///       Indicates that the stream can seek a specific location
        ///       in the stream. This property always returns <see langword='false'/>
        ///       .
        ///    </para>
        /// </devdoc>
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }


        /// <devdoc>
        ///    <para>
        ///       Indicates that data can be written to the stream.
        ///    </para>
        /// </devdoc>
        public override bool CanWrite
        {
            get
            {
                return _writeable;
            }
        }


        /// <devdoc>
        ///    <para>Indicates whether we can timeout</para>
        /// </devdoc>
        public override bool CanTimeout
        {
            get
            {
                return true; // should we check for Connected state?
            }
        }


        /// <devdoc>
        ///    <para>Set/Get ReadTimeout, note of a strange behavior, 0 timeout == infinite for sockets,
        ///         so we map this to -1, and if you set 0, we cannot support it</para>
        /// </devdoc>
        public override int ReadTimeout
        {
            get
            {
#if DEBUG
                using (GlobalLog.SetThreadKind(ThreadKinds.User | ThreadKinds.Async))
                {
#endif
                    int timeout = (int)_streamSocket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout);
                    if (timeout == 0)
                    {
                        return -1;
                    }
                    return timeout;
#if DEBUG
                }
#endif
            }
            set
            {
#if DEBUG
                using (GlobalLog.SetThreadKind(ThreadKinds.User | ThreadKinds.Async))
                {
#endif
                    if (value <= 0 && value != System.Threading.Timeout.Infinite)
                    {
                        throw new ArgumentOutOfRangeException("value", SR.net_io_timeout_use_gt_zero);
                    }
                    SetSocketTimeoutOption(SocketShutdown.Receive, value, false);
#if DEBUG
                }
#endif
            }
        }

        /// <devdoc>
        ///    <para>Set/Get WriteTimeout, note of a strange behavior, 0 timeout == infinite for sockets,
        ///         so we map this to -1, and if you set 0, we cannot support it</para>
        /// </devdoc>
	    public override int WriteTimeout
        {
            get
            {
#if DEBUG
                using (GlobalLog.SetThreadKind(ThreadKinds.User | ThreadKinds.Async))
                {
#endif
                    int timeout = (int)_streamSocket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout);
                    if (timeout == 0)
                    {
                        return -1;
                    }
                    return timeout;
#if DEBUG
                }
#endif
            }
            set
            {
#if DEBUG
                using (GlobalLog.SetThreadKind(ThreadKinds.User | ThreadKinds.Async))
                {
#endif
                    if (value <= 0 && value != System.Threading.Timeout.Infinite)
                    {
                        throw new ArgumentOutOfRangeException("value", SR.net_io_timeout_use_gt_zero);
                    }
                    SetSocketTimeoutOption(SocketShutdown.Send, value, false);
#if DEBUG
                }
#endif
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Indicates data is available on the stream to be read.
        ///         This property checks to see if at least one byte of data is currently available            
        ///    </para>
        /// </devdoc>
        public virtual bool DataAvailable
        {
            get
            {
#if DEBUG
                using (GlobalLog.SetThreadKind(ThreadKinds.User | ThreadKinds.Async))
                {
#endif
                    if (_cleanedUp)
                    {
                        throw new ObjectDisposedException(this.GetType().FullName);
                    }

                    Socket chkStreamSocket = _streamSocket;
                    if (chkStreamSocket == null)
                    {
                        throw new IOException(SR.Format(SR.net_io_readfailure, SR.net_io_connectionclosed));
                    }

                    // Ask the socket how many bytes are available. If it's
                    // not zero, return true.

                    return chkStreamSocket.Available != 0;
#if DEBUG
                }
#endif
            }
        }


        /// <devdoc>
        ///    <para>
        ///       The length of data available on the stream. Always throws <see cref='NotSupportedException'/>.
        ///    </para>
        /// </devdoc>
        public override long Length
        {
            get
            {
                throw new NotSupportedException(SR.net_noseek);
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the position in the stream. Always throws <see cref='NotSupportedException'/>.
        ///    </para>
        /// </devdoc>
        public override long Position
        {
            get
            {
                throw new NotSupportedException(SR.net_noseek);
            }

            set
            {
                throw new NotSupportedException(SR.net_noseek);
            }
        }


        /// <devdoc>
        ///    <para>
        ///       Seeks a specific position in the stream. This method is not supported by the
        ///    <see cref='NetworkStream'/> class.
        ///    </para>
        /// </devdoc>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException(SR.net_noseek);
        }

        internal void InitNetworkStream(Socket socket)
        {
            if (!socket.Blocking)
            {
                throw new IOException(SR.net_sockets_blocking);
            }
            if (!socket.Connected)
            {
                throw new IOException(SR.net_notconnected);
            }
            if (socket.SocketType != SocketType.Stream)
            {
                throw new IOException(SR.net_notstream);
            }

            _streamSocket = socket;
            _readable = true;
            _writeable = true;
        }

        internal bool PollRead()
        {
            if (_cleanedUp)
            {
                return false;
            }
            Socket chkStreamSocket = _streamSocket;
            if (chkStreamSocket == null)
            {
                return false;
            }
            return chkStreamSocket.Poll(0, SelectMode.SelectRead);
        }

        internal bool Poll(int microSeconds, SelectMode mode)
        {
            if (_cleanedUp)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            Socket chkStreamSocket = _streamSocket;
            if (chkStreamSocket == null)
            {
                throw new IOException(SR.Format(SR.net_io_readfailure, SR.net_io_connectionclosed));
            }

            return chkStreamSocket.Poll(microSeconds, mode);
        }


        /*++
            Read - provide core Read functionality.

            Provide core read functionality. All we do is call through to the
            socket Receive functionality.

            Input:

                Buffer  - Buffer to read into.
                Offset  - Offset into the buffer where we're to read.
                Count   - Number of bytes to read.

            Returns:

                Number of bytes we read, or 0 if the socket is closed.

        --*/

        /// <devdoc>
        ///    <para>
        ///       Reads data from the stream.
        ///    </para>
        /// </devdoc>
        //UEUE
        public override int Read([In, Out] byte[] buffer, int offset, int size)
        {
#if DEBUG
            using (GlobalLog.SetThreadKind(ThreadKinds.User | ThreadKinds.Sync))
            {
#endif
                bool canRead = CanRead;  // Prevent race with Dispose.
                if (_cleanedUp)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                if (!canRead)
                {
                    throw new InvalidOperationException(SR.net_writeonlystream);
                }
                //
                // parameter validation
                //
                if (buffer == null)
                {
                    throw new ArgumentNullException("buffer");
                }
                if (offset < 0 || offset > buffer.Length)
                {
                    throw new ArgumentOutOfRangeException("offset");
                }
                if (size < 0 || size > buffer.Length - offset)
                {
                    throw new ArgumentOutOfRangeException("size");
                }


                Socket chkStreamSocket = _streamSocket;
                if (chkStreamSocket == null)
                {
                    throw new IOException(SR.Format(SR.net_io_readfailure, SR.net_io_connectionclosed));
                }

                try
                {
                    int bytesTransferred = chkStreamSocket.Receive(buffer, offset, size, 0);
                    return bytesTransferred;
                }
                catch (Exception exception)
                {
                    if (exception is OutOfMemoryException)
                    {
                        throw;
                    }

                    //
                    // some sort of error occured on the socket call,
                    // set the SocketException as InnerException and throw
                    //
                    throw new IOException(SR.Format(SR.net_io_readfailure, exception.Message), exception);
                }
#if DEBUG
            }
#endif
        }

        /*++
            Write - provide core Write functionality.

            Provide core write functionality. All we do is call through to the
            socket Send method..

            Input:

                Buffer  - Buffer to write from.
                Offset  - Offset into the buffer from where we'll start writing.
                Count   - Number of bytes to write.

            Returns:

                Number of bytes written. We'll throw an exception if we
                can't write everything. It's brutal, but there's no other
                way to indicate an error.
        --*/

        /// <devdoc>
        ///    <para>
        ///       Writes data to the stream..
        ///    </para>
        /// </devdoc>
        public override void Write(byte[] buffer, int offset, int size)
        {
#if DEBUG
            using (GlobalLog.SetThreadKind(ThreadKinds.User | ThreadKinds.Sync))
            {
#endif
                bool canWrite = CanWrite; // Prevent race with Dispose.
                if (_cleanedUp)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                if (!canWrite)
                {
                    throw new InvalidOperationException(SR.net_readonlystream);
                }
                //
                // parameter validation
                //
                if (buffer == null)
                {
                    throw new ArgumentNullException("buffer");
                }
                if (offset < 0 || offset > buffer.Length)
                {
                    throw new ArgumentOutOfRangeException("offset");
                }
                if (size < 0 || size > buffer.Length - offset)
                {
                    throw new ArgumentOutOfRangeException("size");
                }


                Socket chkStreamSocket = _streamSocket;
                if (chkStreamSocket == null)
                {
                    throw new IOException(SR.Format(SR.net_io_writefailure, SR.net_io_connectionclosed));
                }

                try
                {
                    //
                    // since the socket is in blocking mode this will always complete
                    // after ALL the requested number of bytes was transferred
                    //
                    chkStreamSocket.Send(buffer, offset, size, SocketFlags.None);
                }
                catch (Exception exception)
                {
                    if (exception is OutOfMemoryException)
                    {
                        throw;
                    }

                    //
                    // some sort of error occured on the socket call,
                    // set the SocketException as InnerException and throw
                    //
                    throw new IOException(SR.Format(SR.net_io_writefailure, exception.Message), exception);
                }
#if DEBUG
            }
#endif
        }

        private int _closeTimeout = Socket.DefaultCloseTimeout; // 1 ms; -1 = respect linger options

        public void Close(int timeout)
        {
#if DEBUG
            using (GlobalLog.SetThreadKind(ThreadKinds.User | ThreadKinds.Sync))
            {
#endif
                if (timeout < -1)
                {
                    throw new ArgumentOutOfRangeException("timeout");
                }
                _closeTimeout = timeout;
                Dispose();
#if DEBUG
            }
#endif
        }

        private volatile bool _cleanedUp = false;
        protected override void Dispose(bool disposing)
        {
#if DEBUG
            using (GlobalLog.SetThreadKind(ThreadKinds.User))
            {
#endif
                // Mark this as disposed before changing anything else.
                bool cleanedUp = _cleanedUp;
                _cleanedUp = true;
                if (!cleanedUp && disposing)
                {
                    //
                    // only resource we need to free is the network stream, since this
                    // is based on the client socket, closing the stream will cause us
                    // to flush the data to the network, close the stream and (in the
                    // NetoworkStream code) close the socket as well.
                    //
                    if (_streamSocket != null)
                    {
                        _readable = false;
                        _writeable = false;
                        if (_ownsSocket)
                        {
                            //
                            // if we own the Socket (false by default), close it
                            // ignoring possible exceptions (eg: the user told us
                            // that we own the Socket but it closed at some point of time,
                            // here we would get an ObjectDisposedException)
                            //
                            Socket chkStreamSocket = _streamSocket;
                            if (chkStreamSocket != null)
                            {
                                chkStreamSocket.InternalShutdown(SocketShutdown.Both);
                                chkStreamSocket.Close(_closeTimeout);
                            }
                        }
                    }
                }
#if DEBUG
            }
#endif
            base.Dispose(disposing);
        }

        ~NetworkStream()
        {
#if DEBUG
            GlobalLog.SetThreadSource(ThreadKinds.Finalization);
            // using (GlobalLog.SetThreadKind(ThreadKinds.System | ThreadKinds.Async)) {
#endif
            Dispose(false);
#if DEBUG
            // }
#endif
        }

        /// <devdoc>
        ///    <para>
        ///       Indicates whether the stream is still connected
        ///    </para>
        /// </devdoc>
        internal bool Connected
        {
            get
            {
                Socket socket = _streamSocket;
                if (!_cleanedUp && socket != null && socket.Connected)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        /*++
            BeginRead - provide async read functionality.

            This method provides async read functionality. All we do is
            call through to the underlying socket async read.

            Input:

                buffer  - Buffer to read into.
                offset  - Offset into the buffer where we're to read.
                size   - Number of bytes to read.

            Returns:

                An IASyncResult, representing the read.

        --*/

        /// <devdoc>
        ///    <para>
        ///       Begins an asychronous read from a stream.
        ///    </para>
        /// </devdoc>
        public IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, Object state)
        {
#if DEBUG
            using (GlobalLog.SetThreadKind(ThreadKinds.User | ThreadKinds.Async))
            {
#endif
                bool canRead = CanRead; // Prevent race with Dispose.
                if (_cleanedUp)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                if (!canRead)
                {
                    throw new InvalidOperationException(SR.net_writeonlystream);
                }
                //
                // parameter validation
                //
                if (buffer == null)
                {
                    throw new ArgumentNullException("buffer");
                }
                if (offset < 0 || offset > buffer.Length)
                {
                    throw new ArgumentOutOfRangeException("offset");
                }
                if (size < 0 || size > buffer.Length - offset)
                {
                    throw new ArgumentOutOfRangeException("size");
                }

                Socket chkStreamSocket = _streamSocket;
                if (chkStreamSocket == null)
                {
                    throw new IOException(SR.Format(SR.net_io_readfailure, SR.net_io_connectionclosed));
                }

                try
                {
                    IAsyncResult asyncResult =
                        chkStreamSocket.BeginReceive(
                            buffer,
                            offset,
                            size,
                            SocketFlags.None,
                            callback,
                            state);

                    return asyncResult;
                }
                catch (Exception exception)
                {
                    if (exception is OutOfMemoryException)
                    {
                        throw;
                    }

                    //
                    // some sort of error occured on the socket call,
                    // set the SocketException as InnerException and throw
                    //
                    throw new IOException(SR.Format(SR.net_io_readfailure, exception.Message), exception);
                }
#if DEBUG
            }
#endif
        }

        internal virtual IAsyncResult UnsafeBeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, Object state)
        {
            bool canRead = CanRead; // Prevent race with Dispose.
            if (_cleanedUp)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            if (!canRead)
            {
                throw new InvalidOperationException(SR.net_writeonlystream);
            }

            Socket chkStreamSocket = _streamSocket;
            if (chkStreamSocket == null)
            {
                throw new IOException(SR.Format(SR.net_io_readfailure, SR.net_io_connectionclosed));
            }

            try
            {
                IAsyncResult asyncResult = chkStreamSocket.UnsafeBeginReceive(
                    buffer,
                    offset,
                    size,
                    SocketFlags.None,
                    callback,
                    state);

                return asyncResult;
            }
            catch (Exception exception)
            {
                if (ExceptionCheck.IsFatal(exception)) throw;

                //
                // some sort of error occured on the socket call,
                // set the SocketException as InnerException and throw
                //
                throw new IOException(SR.Format(SR.net_io_readfailure, exception.Message), exception);
            }
        }

        /*++
            EndRead - handle the end of an async read.

            This method is called when an async read is completed. All we
            do is call through to the core socket EndReceive functionality.
            Input:

                buffer  - Buffer to read into.
                offset  - Offset into the buffer where we're to read.
                size   - Number of bytes to read.

            Returns:

                The number of bytes read. May throw an exception.

        --*/

        /// <devdoc>
        ///    <para>
        ///       Handle the end of an asynchronous read.
        ///    </para>
        /// </devdoc>
        public int EndRead(IAsyncResult asyncResult)
        {
#if DEBUG
            using (GlobalLog.SetThreadKind(ThreadKinds.User))
            {
#endif
                if (_cleanedUp)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                //
                // parameter validation
                //
                if (asyncResult == null)
                {
                    throw new ArgumentNullException("asyncResult");
                }

                Socket chkStreamSocket = _streamSocket;
                if (chkStreamSocket == null)
                {
                    throw new IOException(SR.Format(SR.net_io_readfailure, SR.net_io_connectionclosed));
                }

                try
                {
                    int bytesTransferred = chkStreamSocket.EndReceive(asyncResult);
                    return bytesTransferred;
                }
                catch (Exception exception)
                {
                    if (exception is OutOfMemoryException)
                    {
                        throw;
                    }

                    //
                    // some sort of error occured on the socket call,
                    // set the SocketException as InnerException and throw
                    //
                    throw new IOException(SR.Format(SR.net_io_readfailure, exception.Message), exception);
                }
#if DEBUG
            }
#endif
        }

        /*++
            BeginWrite - provide async write functionality.

            This method provides async write functionality. All we do is
            call through to the underlying socket async send.

            Input:

                buffer  - Buffer to write into.
                offset  - Offset into the buffer where we're to write.
                size   - Number of bytes to written.

            Returns:

                An IASyncResult, representing the write.

        --*/

        /// <devdoc>
        ///    <para>
        ///       Begins an asynchronous write to a stream.
        ///    </para>
        /// </devdoc>
        public IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, Object state)
        {
#if DEBUG
            using (GlobalLog.SetThreadKind(ThreadKinds.User | ThreadKinds.Async))
            {
#endif
                bool canWrite = CanWrite; // Prevent race with Dispose.
                if (_cleanedUp)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
                if (!canWrite)
                {
                    throw new InvalidOperationException(SR.net_readonlystream);
                }
                //
                // parameter validation
                //
                if (buffer == null)
                {
                    throw new ArgumentNullException("buffer");
                }
                if (offset < 0 || offset > buffer.Length)
                {
                    throw new ArgumentOutOfRangeException("offset");
                }
                if (size < 0 || size > buffer.Length - offset)
                {
                    throw new ArgumentOutOfRangeException("size");
                }

                Socket chkStreamSocket = _streamSocket;
                if (chkStreamSocket == null)
                {
                    throw new IOException(SR.Format(SR.net_io_writefailure, SR.net_io_connectionclosed));
                }

                try
                {
                    //
                    // call BeginSend on the Socket.
                    //
                    IAsyncResult asyncResult =
                        chkStreamSocket.BeginSend(
                            buffer,
                            offset,
                            size,
                            SocketFlags.None,
                            callback,
                            state);

                    return asyncResult;
                }
                catch (Exception exception)
                {
                    if (exception is OutOfMemoryException)
                    {
                        throw;
                    }

                    //
                    // some sort of error occured on the socket call,
                    // set the SocketException as InnerException and throw
                    //
                    throw new IOException(SR.Format(SR.net_io_writefailure, exception.Message), exception);
                }
#if DEBUG
            }
#endif
        }



        internal virtual IAsyncResult UnsafeBeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, Object state)
        {
#if DEBUG
            using (GlobalLog.SetThreadKind(ThreadKinds.User | ThreadKinds.Async))
            {
#endif
                bool canWrite = CanWrite; // Prevent race with Dispose.
                if (_cleanedUp)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                if (!canWrite)
                {
                    throw new InvalidOperationException(SR.net_readonlystream);
                }

                Socket chkStreamSocket = _streamSocket;
                if (chkStreamSocket == null)
                {
                    throw new IOException(SR.Format(SR.net_io_writefailure, SR.net_io_connectionclosed));
                }

                try
                {
                    //
                    // call BeginSend on the Socket.
                    //
                    IAsyncResult asyncResult =
                        chkStreamSocket.UnsafeBeginSend(
                            buffer,
                            offset,
                            size,
                            SocketFlags.None,
                            callback,
                            state);

                    return asyncResult;
                }
                catch (Exception exception)
                {
                    if (exception is OutOfMemoryException)
                    {
                        throw;
                    }

                    //
                    // some sort of error occured on the socket call,
                    // set the SocketException as InnerException and throw
                    //
                    throw new IOException(SR.Format(SR.net_io_writefailure, exception.Message), exception);
                }
#if DEBUG
            }
#endif
        }



        /// <devdoc>
        ///    <para>
        ///       Handle the end of an asynchronous write.
        ///       This method is called when an async write is completed. All we
        ///       do is call through to the core socket EndSend functionality.
        ///       Returns:  The number of bytes read. May throw an exception.
        ///    </para>
        /// </devdoc>
        public void EndWrite(IAsyncResult asyncResult)
        {
#if DEBUG
            using (GlobalLog.SetThreadKind(ThreadKinds.User))
            {
#endif
                if (_cleanedUp)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }

                //
                // parameter validation
                //
                if (asyncResult == null)
                {
                    throw new ArgumentNullException("asyncResult");
                }

                Socket chkStreamSocket = _streamSocket;
                if (chkStreamSocket == null)
                {
                    throw new IOException(SR.Format(SR.net_io_writefailure, SR.net_io_connectionclosed));
                }

                try
                {
                    chkStreamSocket.EndSend(asyncResult);
                }
                catch (Exception exception)
                {
                    if (exception is OutOfMemoryException)
                    {
                        throw;
                    }

                    //
                    // some sort of error occured on the socket call,
                    // set the SocketException as InnerException and throw
                    //
                    throw new IOException(SR.Format(SR.net_io_writefailure, exception.Message), exception);
                }
#if DEBUG
            }
#endif
        }


        /// <devdoc>
        ///    <para>
        ///       Performs a sync Write of an array of buffers.
        ///    </para>
        /// </devdoc>
        internal virtual void MultipleWrite(BufferOffsetSize[] buffers)
        {
            GlobalLog.ThreadContract(ThreadKinds.Sync, "NetworkStream#" + Logging.HashString(this) + "::MultipleWrite");

            //
            // parameter validation
            //
            if (buffers == null)
            {
                throw new ArgumentNullException("buffers");
            }

            Socket chkStreamSocket = _streamSocket;
            if (chkStreamSocket == null)
            {
                throw new IOException(SR.Format(SR.net_io_writefailure, SR.net_io_connectionclosed));
            }

            try
            {
                chkStreamSocket.MultipleSend(
                    buffers,
                    SocketFlags.None);
            }
            catch (Exception exception)
            {
                if (exception is OutOfMemoryException)
                {
                    throw;
                }

                //
                // some sort of error occured on the socket call,
                // set the SocketException as InnerException and throw
                //
                throw new IOException(SR.Format(SR.net_io_writefailure, exception.Message), exception);
            }
        }


        /// <devdoc>
        ///    <para>
        ///       Starts off an async Write of an array of buffers.
        ///    </para>
        /// </devdoc>
        internal virtual IAsyncResult BeginMultipleWrite(
            BufferOffsetSize[] buffers,
            AsyncCallback callback,
            Object state)
        {
#if DEBUG
            GlobalLog.ThreadContract(ThreadKinds.Unknown, "NetworkStream#" + Logging.HashString(this) + "::BeginMultipleWrite");
            using (GlobalLog.SetThreadKind(ThreadKinds.Async))
            {
#endif

                //
                // parameter validation
                //
                if (buffers == null)
                {
                    throw new ArgumentNullException("buffers");
                }

                Socket chkStreamSocket = _streamSocket;
                if (chkStreamSocket == null)
                {
                    throw new IOException(SR.Format(SR.net_io_writefailure, SR.net_io_connectionclosed));
                }

                try
                {
                    //
                    // call BeginMultipleSend on the Socket.
                    //
                    IAsyncResult asyncResult =
                        chkStreamSocket.BeginMultipleSend(
                            buffers,
                            SocketFlags.None,
                            callback,
                            state);

                    return asyncResult;
                }
                catch (Exception exception)
                {
                    if (exception is OutOfMemoryException)
                    {
                        throw;
                    }

                    //
                    // some sort of error occured on the socket call,
                    // set the SocketException as InnerException and throw
                    //
                    throw new IOException(SR.Format(SR.net_io_writefailure, exception.Message), exception);
                }
#if DEBUG
            }
#endif
        }


        internal virtual IAsyncResult UnsafeBeginMultipleWrite(
            BufferOffsetSize[] buffers,
            AsyncCallback callback,
            Object state)
        {
#if DEBUG
            GlobalLog.ThreadContract(ThreadKinds.Unknown, "NetworkStream#" + Logging.HashString(this) + "::BeginMultipleWrite");
            using (GlobalLog.SetThreadKind(ThreadKinds.Async))
            {
#endif

                //
                // parameter validation
                //
                if (buffers == null)
                {
                    throw new ArgumentNullException("buffers");
                }

                Socket chkStreamSocket = _streamSocket;
                if (chkStreamSocket == null)
                {
                    throw new IOException(SR.Format(SR.net_io_writefailure, SR.net_io_connectionclosed));
                }

                try
                {
                    //
                    // call BeginMultipleSend on the Socket.
                    //
                    IAsyncResult asyncResult =
                        chkStreamSocket.UnsafeBeginMultipleSend(
                            buffers,
                            SocketFlags.None,
                            callback,
                            state);

                    return asyncResult;
                }
                catch (Exception exception)
                {
                    if (exception is OutOfMemoryException)
                    {
                        throw;
                    }

                    //
                    // some sort of error occured on the socket call,
                    // set the SocketException as InnerException and throw
                    //
                    throw new IOException(SR.Format(SR.net_io_writefailure, exception.Message), exception);
                }
#if DEBUG
            }
#endif
        }


        internal virtual void EndMultipleWrite(IAsyncResult asyncResult)
        {
            GlobalLog.ThreadContract(ThreadKinds.Unknown, "NetworkStream#" + Logging.HashString(this) + "::EndMultipleWrite");

            //
            // parameter validation
            //
            if (asyncResult == null)
            {
                throw new ArgumentNullException("asyncResult");
            }

            Socket chkStreamSocket = _streamSocket;
            if (chkStreamSocket == null)
            {
                throw new IOException(SR.Format(SR.net_io_writefailure, SR.net_io_connectionclosed));
            }

            try
            {
                chkStreamSocket.EndMultipleSend(asyncResult);
            }
            catch (Exception exception)
            {
                if (exception is OutOfMemoryException)
                {
                    throw;
                }

                //
                // some sort of error occured on the socket call,
                // set the SocketException as InnerException and throw
                //
                throw new IOException(SR.Format(SR.net_io_writefailure, exception.Message), exception);
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Flushes data from the stream.  This is meaningless for us, so it does nothing.
        ///    </para>
        /// </devdoc>
        public override void Flush()
        {
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <devdoc>
        ///    <para>
        ///       Sets the length of the stream. Always throws <see cref='NotSupportedException'/>
        ///    </para>
        /// </devdoc>
        public override void SetLength(long value)
        {
            throw new NotSupportedException(SR.net_noseek);
        }

        private int _currentReadTimeout = -1;
        private int _currentWriteTimeout = -1;
        internal void SetSocketTimeoutOption(SocketShutdown mode, int timeout, bool silent)
        {
            GlobalLog.Print("NetworkStream#" + Logging.HashString(this) + "::SetSocketTimeoutOption() mode:" + mode + " silent:" + silent + " timeout:" + timeout + " m_CurrentReadTimeout:" + _currentReadTimeout + " m_CurrentWriteTimeout:" + _currentWriteTimeout);
            GlobalLog.ThreadContract(ThreadKinds.Unknown, "NetworkStream#" + Logging.HashString(this) + "::SetSocketTimeoutOption");

            if (timeout < 0)
            {
                timeout = 0; // -1 becomes 0 for the winsock stack
            }

            Socket chkStreamSocket = _streamSocket;
            if (chkStreamSocket == null)
            {
                return;
            }
            if (mode == SocketShutdown.Send || mode == SocketShutdown.Both)
            {
                if (timeout != _currentWriteTimeout)
                {
                    chkStreamSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, timeout, silent);
                    _currentWriteTimeout = timeout;
                }
            }
            if (mode == SocketShutdown.Receive || mode == SocketShutdown.Both)
            {
                if (timeout != _currentReadTimeout)
                {
                    chkStreamSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, timeout, silent);
                    _currentReadTimeout = timeout;
                }
            }
        }
#if TRAVE
        [System.Diagnostics.Conditional("TRAVE")]
        internal void DebugMembers()
        {
            if (m_StreamSocket != null)
            {
                GlobalLog.Print("m_StreamSocket:");
                m_StreamSocket.DebugMembers();
            }
        }
#endif
    }
}
