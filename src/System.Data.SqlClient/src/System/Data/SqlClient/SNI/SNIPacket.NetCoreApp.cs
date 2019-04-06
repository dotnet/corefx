// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.SqlClient.SNI
{
    internal partial class SNIPacket
    {
        /// <summary>
        /// Read data from a stream asynchronously
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <param name="callback">Completion callback</param>
        public void ReadFromStreamAsync(Stream stream, SNIAsyncCallback callback)
        {
            static async Task ReadFromStreamAsync(SNIPacket packet, SNIAsyncCallback cb, ValueTask<int> valueTask)
            {
                uint errorCode = TdsEnums.SNI_SUCCESS;
                try
                {
                    packet._length = await valueTask.ConfigureAwait(false);
                    if (packet._length == 0)
                    {
                        SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.TCP_PROV, 0, SNICommon.ConnTerminatedError, string.Empty);
                        errorCode = TdsEnums.SNI_WSAECONNRESET;
                    }
                }
                catch (IOException ioException) when ( 
                    ioException?.InnerException is SocketException socketException && 
                    socketException!=null && 
                    socketException.SocketErrorCode==SocketError.OperationAborted
                )
                {
                    SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.TCP_PROV, SNICommon.InternalExceptionError, socketException);
                    errorCode = TdsEnums.SNI_WSAECONNRESET;
                }
                catch (Exception ex)
                {
                    SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.TCP_PROV, SNICommon.InternalExceptionError, ex);
                    errorCode = TdsEnums.SNI_ERROR;
                }

                if (errorCode != TdsEnums.SNI_SUCCESS)
                {
                    packet.Release();
                }

                cb(packet, errorCode);
            }

            ValueTask<int> vt = stream.ReadAsync(new Memory<byte>(_data, 0, _capacity), CancellationToken.None);

            if (vt.IsCompletedSuccessfully)
            {
                _length = vt.Result;
                // Zero length to go via async local function as is error condition
                if (_length > 0)
                {
                    callback(this, TdsEnums.SNI_SUCCESS);

                    // Completed
                    return;
                }
            }

            // Not complete or error call the async local function to complete
            _ = ReadFromStreamAsync(this, callback, vt);
        }

        /// <summary>
        /// Write data to a stream asynchronously
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        public void WriteToStreamAsync(Stream stream, SNIAsyncCallback callback, SNIProviders provider, bool disposeAfterWriteAsync = false)
        {
            static async Task WriteToStreamAsync(SNIPacket packet, SNIAsyncCallback cb, SNIProviders providers, bool disposeAfter, ValueTask valueTask)
            {
                uint status = TdsEnums.SNI_SUCCESS;
                try
                {
                    await valueTask.ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    SNILoadHandle.SingletonInstance.LastError = new SNIError(providers, SNICommon.InternalExceptionError, e);
                    status = TdsEnums.SNI_ERROR;
                }

                cb(packet, status);

                if (disposeAfter)
                {
                    packet.Dispose();
                }
            }

            ValueTask vt = stream.WriteAsync(new Memory<byte>(_data, 0, _length), CancellationToken.None);

            if (vt.IsCompletedSuccessfully)
            {
                // Read the result to register as complete for the ValueTask
                vt.GetAwaiter().GetResult();

                callback(this, TdsEnums.SNI_SUCCESS);

                if (disposeAfterWriteAsync)
                {
                    Dispose();
                }

                // Completed
                return;
            }

            // Not complete or error call the async local function to complete
            _ = WriteToStreamAsync(this, callback, provider, disposeAfterWriteAsync, vt);
        }
    }
}
