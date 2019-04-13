// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.SqlClient
{
    internal class TdsParserStateObjectNative : TdsParserStateObject
    {
        private SNIHandle _sessionHandle = null;              // the SNI handle we're to work on

        private SNIPacket _sniPacket = null;                // Will have to re-vamp this for MARS
        internal SNIPacket _sniAsyncAttnPacket = null;                // Packet to use to send Attn
        private readonly WritePacketCache _writePacketCache = new WritePacketCache(); // Store write packets that are ready to be re-used

        public TdsParserStateObjectNative(TdsParser parser) : base(parser) { }

        private GCHandle _gcHandle;                                    // keeps this object alive until we're closed.

        private Dictionary<IntPtr, SNIPacket> _pendingWritePackets = new Dictionary<IntPtr, SNIPacket>(); // Stores write packets that have been sent to SNI, but have not yet finished writing (i.e. we are waiting for SNI's callback)

        internal TdsParserStateObjectNative(TdsParser parser, TdsParserStateObject physicalConnection, bool async) :
            base(parser, physicalConnection, async)
        {
        }

        internal SNIHandle Handle => _sessionHandle;

        internal override uint Status => _sessionHandle != null ? _sessionHandle.Status : TdsEnums.SNI_UNINITIALIZED;

        internal override SessionHandle SessionHandle => SessionHandle.FromNativeHandle(_sessionHandle);

        protected override void CreateSessionHandle(TdsParserStateObject physicalConnection, bool async)
        {
            Debug.Assert(physicalConnection is TdsParserStateObjectNative, "Expected a stateObject of type " + this.GetType());
            TdsParserStateObjectNative nativeSNIObject = physicalConnection as TdsParserStateObjectNative;
            SNINativeMethodWrapper.ConsumerInfo myInfo = CreateConsumerInfo(async);
            _sessionHandle = new SNIHandle(myInfo, nativeSNIObject.Handle);
        }

        private SNINativeMethodWrapper.ConsumerInfo CreateConsumerInfo(bool async)
        {
            SNINativeMethodWrapper.ConsumerInfo myInfo = new SNINativeMethodWrapper.ConsumerInfo();

            Debug.Assert(_outBuff.Length == _inBuff.Length, "Unexpected unequal buffers.");

            myInfo.defaultBufferSize = _outBuff.Length; // Obtain packet size from outBuff size.

            if (async)
            {
                myInfo.readDelegate = SNILoadHandle.SingletonInstance.ReadAsyncCallbackDispatcher;
                myInfo.writeDelegate = SNILoadHandle.SingletonInstance.WriteAsyncCallbackDispatcher;
                _gcHandle = GCHandle.Alloc(this, GCHandleType.Normal);
                myInfo.key = (IntPtr)_gcHandle;
            }
            return myInfo;
        }

        internal override void CreatePhysicalSNIHandle(string serverName, bool ignoreSniOpenTimeout, long timerExpire, out byte[] instanceName, ref byte[] spnBuffer, bool flushCache, bool async, bool fParallel, bool isIntegratedSecurity)
        {
            // We assume that the loadSSPILibrary has been called already. now allocate proper length of buffer
            spnBuffer = null;
            if (isIntegratedSecurity)
            {
                // now allocate proper length of buffer
                spnBuffer = new byte[SNINativeMethodWrapper.SniMaxComposedSpnLength];
            }

            SNINativeMethodWrapper.ConsumerInfo myInfo = CreateConsumerInfo(async);

            // Translate to SNI timeout values (Int32 milliseconds)
            long timeout;
            if (long.MaxValue == timerExpire)
            {
                timeout = int.MaxValue;
            }
            else
            {
                timeout = ADP.TimerRemainingMilliseconds(timerExpire);
                if (timeout > int.MaxValue)
                {
                    timeout = int.MaxValue;
                }
                else if (0 > timeout)
                {
                    timeout = 0;
                }
            }

            _sessionHandle = new SNIHandle(myInfo, serverName, spnBuffer, ignoreSniOpenTimeout, checked((int)timeout), out instanceName, flushCache, !async, fParallel);
        }

        protected override uint SNIPacketGetData(PacketHandle packet, byte[] _inBuff, ref uint dataSize)
        {
            Debug.Assert(packet.Type == PacketHandle.NativePointerType, "unexpected packet type when requiring NativePointer");
            return SNINativeMethodWrapper.SNIPacketGetData(packet.NativePointer, _inBuff, ref dataSize);
        }

        protected override bool CheckPacket(PacketHandle packet, TaskCompletionSource<object> source)
        {
            Debug.Assert(packet.Type == PacketHandle.NativePointerType, "unexpected packet type when requiring NativePointer");
            IntPtr ptr = packet.NativePointer;
            return IntPtr.Zero == ptr || IntPtr.Zero != ptr && source != null;
        }

        public void ReadAsyncCallback(IntPtr key, IntPtr packet, uint error) => ReadAsyncCallback(key, packet, error);

        public void WriteAsyncCallback(IntPtr key, IntPtr packet, uint sniError) => WriteAsyncCallback(key, packet, sniError);

        protected override void RemovePacketFromPendingList(PacketHandle ptr)
        {
            Debug.Assert(ptr.Type == PacketHandle.NativePointerType, "unexpected packet type when requiring NativePointer");
            IntPtr pointer = ptr.NativePointer;

            SNIPacket recoveredPacket;

            lock (_writePacketLockObject)
            {
                if (_pendingWritePackets.TryGetValue(pointer, out recoveredPacket))
                {
                    _pendingWritePackets.Remove(pointer);
                    _writePacketCache.Add(recoveredPacket);
                }
#if DEBUG
                else
                {
                    Debug.Fail("Removing a packet from the pending list that was never added to it");
                }
#endif
            }
        }


        internal override void Dispose()
        {
            SafeHandle packetHandle = _sniPacket;
            SafeHandle sessionHandle = _sessionHandle;
            SafeHandle asyncAttnPacket = _sniAsyncAttnPacket;

            _sniPacket = null;
            _sessionHandle = null;
            _sniAsyncAttnPacket = null;

            DisposeCounters();

            if (null != sessionHandle || null != packetHandle)
            {
                packetHandle?.Dispose();
                asyncAttnPacket?.Dispose();
                    
                if (sessionHandle != null)
                {
                    sessionHandle.Dispose();
                    DecrementPendingCallbacks(true); // Will dispose of GC handle.
                }
            }

            DisposePacketCache();
        }

        protected override void FreeGcHandle(int remaining, bool release)
        {
            if ((0 == remaining || release) && _gcHandle.IsAllocated)
            {
                _gcHandle.Free();
            }
        }

        internal override bool IsFailedHandle() => _sessionHandle.Status != TdsEnums.SNI_SUCCESS;

        internal override PacketHandle ReadSyncOverAsync(int timeoutRemaining, out uint error)
        {
            SNIHandle handle = Handle;
            if (handle == null)
            {
                throw ADP.ClosedConnectionError();
            }
            IntPtr readPacketPtr = IntPtr.Zero;
            error = SNINativeMethodWrapper.SNIReadSyncOverAsync(handle, ref readPacketPtr, GetTimeoutRemaining());
            return PacketHandle.FromNativePointer(readPacketPtr);
        }

        protected override PacketHandle EmptyReadPacket => PacketHandle.FromNativePointer(default);

        internal override bool IsPacketEmpty(PacketHandle readPacket)
        {
            Debug.Assert(readPacket.Type == PacketHandle.NativePointerType || readPacket.Type == 0, "unexpected packet type when requiring NativePointer");
            return IntPtr.Zero == readPacket.NativePointer;
        }

        internal override void ReleasePacket(PacketHandle syncReadPacket)
        {
            Debug.Assert(syncReadPacket.Type == PacketHandle.NativePointerType, "unexpected packet type when requiring NativePointer");
            SNINativeMethodWrapper.SNIPacketRelease(syncReadPacket.NativePointer);
        }

        internal override uint CheckConnection()
        {
            SNIHandle handle = Handle;
            return handle == null ? TdsEnums.SNI_SUCCESS : SNINativeMethodWrapper.SNICheckConnection(handle);
        }

        internal override PacketHandle ReadAsync(SessionHandle handle, out uint error)
        {
            Debug.Assert(handle.Type == SessionHandle.NativeHandleType, "unexpected handle type when requiring NativePointer");
            IntPtr readPacketPtr = IntPtr.Zero;
            error = SNINativeMethodWrapper.SNIReadAsync(handle.NativeHandle, ref readPacketPtr);
            return PacketHandle.FromNativePointer(readPacketPtr);
        }

        internal override PacketHandle CreateAndSetAttentionPacket()
        {
            SNIHandle handle = Handle;
            SNIPacket attnPacket = new SNIPacket(handle);
            _sniAsyncAttnPacket = attnPacket;
            SetPacketData(PacketHandle.FromNativePacket(attnPacket), SQL.AttentionHeader, TdsEnums.HEADER_LEN);
            return PacketHandle.FromNativePacket(attnPacket);
        }

        internal override uint WritePacket(PacketHandle packet, bool sync)
        {
            Debug.Assert(packet.Type == PacketHandle.NativePacketType, "unexpected packet type when requiring NativePacket");
            return SNINativeMethodWrapper.SNIWritePacket(Handle, packet.NativePacket, sync);
        }

        internal override PacketHandle AddPacketToPendingList(PacketHandle packetToAdd)
        {
            Debug.Assert(packetToAdd.Type == PacketHandle.NativePacketType, "unexpected packet type when requiring NativePacket");
            SNIPacket packet = packetToAdd.NativePacket;
            Debug.Assert(packet == _sniPacket, "Adding a packet other than the current packet to the pending list");
            _sniPacket = null;
            IntPtr pointer = packet.DangerousGetHandle();

            lock (_writePacketLockObject)
            {
                _pendingWritePackets.Add(pointer, packet);
            }

            return PacketHandle.FromNativePointer(pointer);
        }

        internal override bool IsValidPacket(PacketHandle packetPointer)
        {
            Debug.Assert(packetPointer.Type == PacketHandle.NativePointerType || packetPointer.Type==PacketHandle.NativePacketType, "unexpected packet type when requiring NativePointer");
            return (
                (packetPointer.Type == PacketHandle.NativePointerType && packetPointer.NativePointer != IntPtr.Zero)
                ||
                (packetPointer.Type == PacketHandle.NativePacketType && packetPointer.NativePacket != null)
            );
        }

        internal override PacketHandle GetResetWritePacket()
        {
            if (_sniPacket != null)
            {
                SNINativeMethodWrapper.SNIPacketReset(Handle, SNINativeMethodWrapper.IOType.WRITE, _sniPacket, SNINativeMethodWrapper.ConsumerNumber.SNI_Consumer_SNI);
            }
            else
            {
                lock (_writePacketLockObject)
                {
                    _sniPacket = _writePacketCache.Take(Handle);
                }
            }
            return PacketHandle.FromNativePacket(_sniPacket);
        }

        internal override void ClearAllWritePackets()
        {
            if (_sniPacket != null)
            {
                _sniPacket.Dispose();
                _sniPacket = null;
            }
            lock (_writePacketLockObject)
            {
                Debug.Assert(_pendingWritePackets.Count == 0 && _asyncWriteCount == 0, "Should not clear all write packets if there are packets pending");
                _writePacketCache.Clear();
            }
        }

        private void SetPacketData(PacketHandle packet, byte[] buffer, int bytesUsed)
        {
            Debug.Assert(packet.Type == PacketHandle.NativePacketType, "unexpected packet type when requiring NativePacket");
            SNINativeMethodWrapper.SNIPacketSetData(packet.NativePacket, buffer, bytesUsed);
        }

        internal override uint SniGetConnectionId(ref Guid clientConnectionId) 
            => SNINativeMethodWrapper.SniGetConnectionId(Handle, ref clientConnectionId);

        internal override uint DisabeSsl() 
            => SNINativeMethodWrapper.SNIRemoveProvider(Handle, SNINativeMethodWrapper.ProviderEnum.SSL_PROV);

        internal override uint EnableMars(ref uint info) 
            => SNINativeMethodWrapper.SNIAddProvider(Handle, SNINativeMethodWrapper.ProviderEnum.SMUX_PROV, ref info);

        internal override uint EnableSsl(ref uint info)
        {
            // Add SSL (Encryption) SNI provider.
            return SNINativeMethodWrapper.SNIAddProvider(Handle, SNINativeMethodWrapper.ProviderEnum.SSL_PROV, ref info);
        }

        internal override uint SetConnectionBufferSize(ref uint unsignedPacketSize) 
            => SNINativeMethodWrapper.SNISetInfo(Handle, SNINativeMethodWrapper.QTypes.SNI_QUERY_CONN_BUFSIZE, ref unsignedPacketSize);

        internal override uint GenerateSspiClientContext(byte[] receivedBuff, uint receivedLength, ref byte[] sendBuff, ref uint sendLength, byte[] _sniSpnBuffer) 
            => SNINativeMethodWrapper.SNISecGenClientContext(Handle, receivedBuff, receivedLength, sendBuff, ref sendLength, _sniSpnBuffer);
        

        internal override uint WaitForSSLHandShakeToComplete() 
            => SNINativeMethodWrapper.SNIWaitForSSLHandshakeToComplete(Handle, GetTimeoutRemaining());

        internal override void DisposePacketCache()
        {
            lock (_writePacketLockObject)
            {
                _writePacketCache.Dispose();
                // Do not set _writePacketCache to null, just in case a WriteAsyncCallback completes after this point
            }
        }
        
        internal sealed class WritePacketCache : IDisposable
        {
            private bool _disposed;
            private Stack<SNIPacket> _packets;

            public WritePacketCache()
            {
                _disposed = false;
                _packets = new Stack<SNIPacket>();
            }

            public SNIPacket Take(SNIHandle sniHandle)
            {
                SNIPacket packet;
                if (_packets.Count > 0)
                {
                    // Success - reset the packet
                    packet = _packets.Pop();
                    SNINativeMethodWrapper.SNIPacketReset(sniHandle, SNINativeMethodWrapper.IOType.WRITE, packet, SNINativeMethodWrapper.ConsumerNumber.SNI_Consumer_SNI);
                }
                else
                {
                    // Failed to take a packet - create a new one
                    packet = new SNIPacket(sniHandle);
                }
                return packet;
            }

            public void Add(SNIPacket packet)
            {
                if (!_disposed)
                {
                    _packets.Push(packet);
                }
                else
                {
                    // If we're disposed, then get rid of any packets added to us
                    packet.Dispose();
                }
            }

            public void Clear()
            {
                while (_packets.Count > 0)
                {
                    _packets.Pop().Dispose();
                }
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _disposed = true;
                    Clear();
                }
            }
        }


        internal override Task WriteSni(bool canAccumulate)
        {
            // Prepare packet, and write to packet.
            PacketHandle packet = GetResetWritePacket();

            SetBufferSecureStrings();
            SetPacketData(packet, _outBuff, _outBytesUsed);

            uint sniError;
            Debug.Assert(Parser.Connection._parserLock.ThreadMayHaveLock(), "Thread is writing without taking the connection lock");
            Task task = SNIWritePacket(packet, out sniError, canAccumulate, callerHasConnectionLock: true);

            // Check to see if the timeout has occurred.  This time out code is special case code to allow BCP writes to timeout. Eventually we should make all writes timeout.
            if (_bulkCopyOpperationInProgress && 0 == GetTimeoutRemaining())
            {
                _parser.Connection.ThreadHasParserLockForClose = true;
                try
                {
                    Debug.Assert(_parser.Connection != null, "SqlConnectionInternalTds handler can not be null at this point.");
                    AddError(new SqlError(TdsEnums.TIMEOUT_EXPIRED, (byte)0x00, TdsEnums.MIN_ERROR_CLASS, _parser.Server, _parser.Connection.TimeoutErrorInternal.GetErrorMessage(), "", 0, TdsEnums.SNI_WAIT_TIMEOUT));
                    _bulkCopyWriteTimeout = true;
                    SendAttention();
                    _parser.ProcessPendingAck(this);
                    ThrowExceptionAndWarning();
                }
                finally
                {
                    _parser.Connection.ThreadHasParserLockForClose = false;
                }
            }

            // Special case logic for encryption removal.
            if (_parser.State == TdsParserState.OpenNotLoggedIn &&
                _parser.EncryptionOptions == EncryptionOptions.LOGIN)
            {
                // If no error occurred, and we are Open but not logged in, and
                // our encryptionOption state is login, remove the SSL Provider.
                // We only need encrypt the very first packet of the login message to the server.

                // We wanted to encrypt entire login channel, but there is
                // currently no mechanism to communicate this.  Removing encryption post 1st packet
                // is a hard-coded agreement between client and server.  We need some mechanism or
                // common change to be able to make this change in a non-breaking fashion.
                _parser.RemoveEncryption();                        // Remove the SSL Provider.
                _parser.EncryptionOptions = EncryptionOptions.OFF; // Turn encryption off.

                // Since this packet was associated with encryption, dispose and re-create.
                ClearAllWritePackets();
            }

            SniWriteStatisticsAndTracing();

            ResetBuffer();

            AssertValidState();
            return task;
        }

        // Sends an attention signal - executing thread will consume attn.
        internal override void SendAttention(bool mustTakeWriteLock = false)
        {
            if (!_attentionSent)
            {
                // Dumps contents of buffer to OOB write (currently only used for
                // attentions.  There is no body for this message
                // Doesn't touch this._outBytesUsed
                if (_parser.State == TdsParserState.Closed || _parser.State == TdsParserState.Broken)
                {
                    return;
                }

                PacketHandle attnPacket = CreateAndSetAttentionPacket();

                try
                {
                    // Dev11 #344723: SqlClient stress hang System_Data!Tcp::ReadSync via a call to SqlDataReader::Close
                    // Set _attentionSending to true before sending attention and reset after setting _attentionSent
                    // This prevents a race condition between receiving the attention ACK and setting _attentionSent
                    _attentionSending = true;

#if DEBUG
                    if (!_skipSendAttention)
                    {
#endif
                        // Take lock and send attention
                        bool releaseLock = false;
                        if ((mustTakeWriteLock) && (!_parser.Connection.ThreadHasParserLockForClose))
                        {
                            releaseLock = true;
                            _parser.Connection._parserLock.Wait(canReleaseFromAnyThread: false);
                            _parser.Connection.ThreadHasParserLockForClose = true;
                        }
                        try
                        {
                            // Check again (just in case the connection was closed while we were waiting)
                            if (_parser.State == TdsParserState.Closed || _parser.State == TdsParserState.Broken)
                            {
                                return;
                            }

                            uint sniError;
                            _parser._asyncWrite = false; // stop async write 
                            SNIWritePacket(attnPacket, out sniError, canAccumulate: false, callerHasConnectionLock: false);
                        }
                        finally
                        {
                            if (releaseLock)
                            {
                                _parser.Connection.ThreadHasParserLockForClose = false;
                                _parser.Connection._parserLock.Release();
                            }
                        }
#if DEBUG
                    }
#endif

                    SetTimeoutSeconds(AttentionTimeoutSeconds); // Initialize new attention timeout of 5 seconds.
                    _attentionSent = true;
                }
                finally
                {
                    _attentionSending = false;
                }


                AssertValidState();
            }
        }

        internal Task SNIWritePacket(PacketHandle packet, out uint sniError, bool canAccumulate, bool callerHasConnectionLock)
        {
            // Check for a stored exception
            var delayedException = Interlocked.Exchange(ref _delayedWriteAsyncCallbackException, null);
            if (delayedException != null)
            {
                throw delayedException;
            }

            Task task = null;
            _writeCompletionSource = null;

            PacketHandle packetPointer = EmptyReadPacket;

            bool sync = !_parser._asyncWrite;
            if (sync && _asyncWriteCount > 0)
            { // for example, SendAttention while there are writes pending
                Task waitForWrites = WaitForAccumulatedWrites();
                if (waitForWrites != null)
                {
                    try
                    {
                        waitForWrites.Wait();
                    }
                    catch (AggregateException ae)
                    {
                        throw ae.InnerException;
                    }
                }
                Debug.Assert(_asyncWriteCount == 0, "All async write should be finished");
            }
            if (!sync)
            {
                // Add packet to the pending list (since the callback can happen any time after we call SNIWritePacket)
                packetPointer = AddPacketToPendingList(packet);
            }

            // Async operation completion may be delayed (success pending).
            try
            {
            }
            finally
            {
                sniError = WritePacket(packet, sync);
            }

            if (sniError == TdsEnums.SNI_SUCCESS_IO_PENDING)
            {
                Debug.Assert(!sync, "Completion should be handled in SniManagedWwrapper");
                Interlocked.Increment(ref _asyncWriteCount);
                Debug.Assert(_asyncWriteCount >= 0);
                if (!canAccumulate)
                {
                    // Create completion source (for callback to complete)
                    _writeCompletionSource = new TaskCompletionSource<object>();
                    task = _writeCompletionSource.Task;

                    // Ensure that setting _writeCompletionSource completes before checking _delayedWriteAsyncCallbackException
                    Interlocked.MemoryBarrier();

                    // Check for a stored exception
                    delayedException = Interlocked.Exchange(ref _delayedWriteAsyncCallbackException, null);
                    if (delayedException != null)
                    {
                        throw delayedException;
                    }

                    // If there are no outstanding writes, see if we can shortcut and return null
                    if ((_asyncWriteCount == 0) && ((!task.IsCompleted) || (task.Exception == null)))
                    {
                        task = null;
                    }
                }
            }
#if DEBUG
            else if (!sync && !canAccumulate && SqlCommand.DebugForceAsyncWriteDelay > 0)
            {
                // Executed synchronously - callback will not be called 
                TaskCompletionSource<object> completion = new TaskCompletionSource<object>();
                uint error = sniError;
                new Timer(obj =>
                {
                    try
                    {
                        if (_parser.MARSOn)
                        { // Only take reset lock on MARS.
                            CheckSetResetConnectionState(error, CallbackType.Write);
                        }

                        if (error != TdsEnums.SNI_SUCCESS)
                        {
                            AddError(_parser.ProcessSNIError(this));
                            ThrowExceptionAndWarning();
                        }
                        AssertValidState();
                        completion.SetResult(null);
                    }
                    catch (Exception e)
                    {
                        completion.SetException(e);
                    }
                }, null, SqlCommand.DebugForceAsyncWriteDelay, Timeout.Infinite);
                task = completion.Task;
            }
#endif
            else
            {
                if (_parser.MARSOn)
                { // Only take reset lock on MARS.
                    CheckSetResetConnectionState(sniError, CallbackType.Write);
                }

                if (sniError == TdsEnums.SNI_SUCCESS)
                {
                    _lastSuccessfulIOTimer._value = DateTime.UtcNow.Ticks;

                    if (!sync)
                    {
                        // Since there will be no callback, remove the packet from the pending list
                        Debug.Assert(IsValidPacket(packetPointer), "Packet added to list has an invalid pointer, can not remove from pending list");
                        RemovePacketFromPendingList(packetPointer);
                    }
                }
                else
                {
                    AddError(_parser.ProcessSNIError(this));
                    ThrowExceptionAndWarning(callerHasConnectionLock);
                }
                AssertValidState();
            }
            return task;
        }
    }
}
