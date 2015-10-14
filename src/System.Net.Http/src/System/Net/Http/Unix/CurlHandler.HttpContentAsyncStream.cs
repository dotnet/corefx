// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    // libcurl has a pull-based model and needs to be able to read data on-demand
    // from the request content stream.  HttpContent provides a ReadAsStreamAsync method
    // that would theoretically enable this, however it's actually not intended for such
    // scenarios and has some deficiencies in this regard.  For example, in certain scenarios,
    // we may need to retry an HTTP request, which means sending the data again.  In such
    // situations, libcurl will ask us to rewind the stream.  We can do this with ReadAsStreamAsync,
    // but only if the returned stream supports seeking.  If it doesn't support seeking, we
    // could ask for a new stream again via calling ReadAsStreamAsync again, but it doesn't
    // do content consumption detection like you get with HttpContent.CopyToAsync.  CopyToAsync
    // is the preferred, supported mechanism for a handler getting data from the HttpContent,
    // and is what WinHttpHandler uses, but it also provides a push-based model.  In order to
    // be able to pull from a push-based model, we need an adapter stream that straddles
    // the divide.  This implementation provides that.

    internal partial class CurlHandler : HttpMessageHandler
    {
        /// <summary>
        /// Provides a stream that lets CurlHandler read the data from an
        /// HttpContent via HttpContent's CopyToAsync.
        /// </summary>
        private sealed class HttpContentAsyncStream : Stream
        {
            /// <summary>Cached task that contains the Int32 value 0.</int></summary>
            private static readonly Task<int> s_zeroTask = Task.FromResult(0);

            /// <summary>Object used to synchronize all activity on the stream.</summary>
            private readonly object _syncObj = new object();
            /// <summary>The HttpContent to read from, potentially repeatedly until a response is received.</summary>
            private readonly HttpContent _content;

            /// <summary>true if the stream has been disposed; otherwise, false.</summary>
            private bool _disposed;
            /// <summary>Task representing the transfer of data from the HttpContent to this stream.</summary>
            private Task _copyTask;

            /// <summary>TaskCompletionSource for the currently outstanding read or write operation.</summary>
            private TaskCompletionSource<int> _asyncOp;
            /// <summary>Cancellation registration associated with the currently pending async operation.</summary>
            private CancellationTokenRegistration _cancellationRegistration;

            /// <summary>Whether the currently oustanding async operation is a reader.</summary>
            private bool _waiterIsReader;
            /// <summary>The current outstanding async operation's buffer.</summary>
            private byte[] _buffer;
            /// <summary>The current outstanding async operations offset.</summary>
            private int _offset;
            /// <summary>The current outstanding async operations count.</summary>
            private int _count;

            /// <summary>Initializes the stream.</summary>
            /// <param name="content">The content the stream should read from.</param>
            internal HttpContentAsyncStream(HttpContent content)
            {
                Debug.Assert(content != null, "Expected non-null content");
                _content = content;
            }

            /// <summary>Gets whether the stream is readable.  It is, but it should only be read from by CurlHandler's libcurl callbacks.</summary>
            public override bool CanRead { get { return true; } }

            /// <summary>Gets whether the stream is writable.  It is, but it should only be written to by HttpContent.CopyToAsync.</summary>
            public override bool CanWrite { get { return true; } }
            
            /// <summary>Gets whether the stream is seekable. It's not.</summary>
            public override bool CanSeek { get { return false; } }

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                // If no data was requested, give back 0 bytes.
                if (count == 0)
                {
                    return s_zeroTask;
                }

                // If cancellation was requested, bail.
                if (cancellationToken.IsCancellationRequested)
                {
                    return Task.FromCanceled<int>(cancellationToken);
                }

                lock (_syncObj)
                {
                    // If we've been disposed, return that no data is available.
                    if (_disposed)
                    {
                        return s_zeroTask;
                    }

                    // If we've already completed, return 0 bytes or fail with the same error that the CopyToAsync failed with.
                    if (_copyTask.IsCompleted)
                    {
                        return _copyTask.Status == TaskStatus.RanToCompletion ?
                            s_zeroTask :
                            PropagateError<int>(_copyTask);
                    }

                    // If there's no writer waiting for us, register our buffer/offset/count and return.
                    if (_buffer == null)
                    {
                        VerboseTrace("No waiting writer. Queueing.");
                        Update(true, buffer, offset, count, NewTcs());
                        RegisterCancellation(cancellationToken);
                        return _asyncOp.Task;
                    }

                    // There's writer data available to use.  Read it to satisfy this request.
                    int bytesToCopy = Math.Min(_count, count);
                    Debug.Assert(bytesToCopy > 0, "Expected to be copying at least 1 byte");
                    Buffer.BlockCopy(_buffer, _offset, buffer, offset, bytesToCopy);
                    VerboseTrace("Read " + bytesToCopy + " from writer.");

                    if (bytesToCopy == _count)
                    {
                        // We exhausted all of the writer's data, so complete the writer
                        // and clear out the state.
                        TaskCompletionSource<int> tcs = _asyncOp;
                        Update(false, null, 0, 0, null);
                        _cancellationRegistration.Dispose();
                        bool writerCompleted = tcs.TrySetResult(default(int)); // value doesn't matter, as the Task is a writer
                        Debug.Assert(writerCompleted, "No one else should have completed the writer");
                    }
                    else
                    {
                        // The writer still has more data to provide to a future read.
                        // Update based on this read.
                        _offset += bytesToCopy;
                        _count -= bytesToCopy;
                    }

                    // Return the number of bytes read.
                    return Task.FromResult(bytesToCopy);
                }
            }

            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                // If no data was provided, we're done.
                if (count == 0)
                {
                    return Task.CompletedTask;
                }

                // If cancellation was requested, bail.
                if (cancellationToken.IsCancellationRequested)
                {
                    return Task.FromCanceled<int>(cancellationToken);
                }

                lock (_syncObj)
                {
                    // If we've been disposed, throw an exception so as to end the CopyToAsync operation.
                    if (_disposed)
                    {
                        VerboseTrace("WriteAsync when already disposed");
                        throw CreateHttpRequestException();
                    }

                    if (_buffer == null)
                    {
                        // There's no waiting reader.  Store everything for later.
                        VerboseTrace("No waiting reader. Queueing.");
                        Debug.Assert(_asyncOp == null, "No one else should be waiting");
                        Update(false, buffer, offset, count, NewTcs());
                        RegisterCancellation(cancellationToken);
                        return _asyncOp.Task;
                    }

                    // There's a waiting reader.  We'll give them our data and store
                    // for later anything they can't use.
                    Debug.Assert(_waiterIsReader, "Concurrent WriteAsync calls are not permitted");
                    Debug.Assert(_count > 0, "Expected reader to need at least 1 byte");
                    Debug.Assert(_asyncOp != null, "Reader should have created a task to complete");

                    // Complete the waiting reader with however much we can give them
                    int bytesToCopy = Math.Min(count, _count);
                    Buffer.BlockCopy(buffer, offset, _buffer, _offset, bytesToCopy);
                    _cancellationRegistration.Dispose();
                    bool completed = _asyncOp.TrySetResult(bytesToCopy);
                    Debug.Assert(completed, "No one else should have completed the reader");
                    VerboseTrace("Writer transferred " + bytesToCopy + " to reader.");

                    if (bytesToCopy < count)
                    {
                        // If we have more bytes from this write, store the arguments
                        // for use later and return a task to be completed by a reader.
                        Update(false, buffer, offset + bytesToCopy, count - bytesToCopy, NewTcs());
                        RegisterCancellation(cancellationToken);
                        return _asyncOp.Task;
                    }
                    else
                    {
                        // We used up all of the data in this write, so complete
                        // the caller synchronously.
                        Update(false, null, 0, 0, null);
                        return Task.CompletedTask;
                    }
                }
            }

            protected override void Dispose(bool disposing)
            {
                lock (_syncObj)
                {
                    if (!_disposed)
                    {
                        // Mark ourselves as disposed, and make sure to wake
                        // up any writer or reader who's been waiting.
                        _disposed = true;
                        if (_asyncOp != null)
                        {
                            _cancellationRegistration.Dispose();
                            bool canceled = _asyncOp.TrySetCanceled();
                            Debug.Assert(canceled, "No one else should have completed the operation.");
                        }
                        Update(false, null, 0, 0, null);
                    }
                }

                base.Dispose(disposing);
            }

            /// <summary>Registers with the specified cancellation token to cancel the pending operation.</summary>
            private void RegisterCancellation(CancellationToken cancellationToken)
            {
                if (!cancellationToken.CanBeCanceled)
                    return;

                _cancellationRegistration = cancellationToken.Register(s =>
                {
                    VerboseTrace("Cancellation requested");
                    var thisRef = (HttpContentAsyncStream)s;
                    lock (thisRef._syncObj)
                    {
                        if (thisRef._asyncOp != null)
                        {
                            bool canceled = thisRef._asyncOp.TrySetCanceled();
                            Debug.Assert(canceled, "Expected to be able to cancel pending async op");
                            thisRef.Update(false, null, 0, 0, null);
                        }
                    }
                }, this);
            }

            /// <summary>
            /// Try to rewind the stream to the beginning.  If successful, the next call to ReadAsync
            /// will initiate a new CopyToAsync operation.  Reset is only successful when there's
            /// no CopyToAsync in progress.
            /// </summary>
            /// <returns></returns>
            internal bool TryReset()
            {
                lock (_syncObj)
                {
                    if (_copyTask == null || _copyTask.IsCompleted)
                    {
                        VerboseTrace("TryReset successful");
                        _copyTask = null;
                        return true;
                    }
                }

                VerboseTrace("TryReset failed");
                return false;
            }

            /// <summary>Initiate a new copy from the HttpContent to this stream. This is not thread-safe.</summary>
            internal void Run()
            {
                Debug.Assert(!Monitor.IsEntered(_syncObj), "Should not be invoked while holding the lock");
                Debug.Assert(_copyTask == null, "Should only be invoked after construction or a reset");

                // Start the copy and store the task to represent it
                _copyTask = StartCopyToAsync();

                // Fix up the instance when it's done
                _copyTask.ContinueWith((t, s) => ((HttpContentAsyncStream)s).EndCopyToAsync(t), this,
                    CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            }

            /// <summary>
            /// Initiates a new CopyToAsync from the HttpContent to this stream, and should only be called when the caller
            /// can be sure that no one else is accessing the stream or attempting to initiate a copy.
            /// </summary>
            private Task StartCopyToAsync()
            {
                Debug.Assert(!Monitor.IsEntered(_syncObj), "Should not be invoked while holding the lock");
                Debug.Assert(_copyTask == null, "Should only be invoked after construction or a reset");

                // Transfer the data from the content to this stream
                try
                {
                    VerboseTrace("Initiating new CopyToAsync");
                    return _content.CopyToAsync(this);
                }
                catch (Exception exc)
                {
                    // CopyToAsync allows some exceptions to propagate synchronously, including exceptions
                    // indicating that a stream can't be re-copied.
                    return Task.FromException(exc);
                }
            }

            /// <summary>Completes a CopyToAsync; called only from StartCopyToAsync.</summary>
            private void EndCopyToAsync(Task completedCopy)
            {
                Debug.Assert(!Monitor.IsEntered(_syncObj), "Should not be invoked while holding the lock");
                lock (_syncObj)
                {
                    VerboseTrace("CopyToAsync completed " + completedCopy.Status + " " + completedCopy.Exception);

                    // We're done transferring, but a reader doesn't know that until they successfully
                    // read 0 bytes, which won't happen until either a) we complete an already waiting
                    // reader or b) a new reader comes along and see _copyTask completed.  We also need
                    // to make sure that, once we do signal completion by giving a reader 0 bytes, we're
                    // immediately ready for a TryReset, which means the _copyTask needs to be completed.
                    // As such, while we're holding the lock, we need to both give 0 bytes to any waiting
                    // reader and force _copyTask to complete, which we do by replacing this Task's
                    // reference with one known to already be complete.

                    if (_asyncOp != null)
                    {
                        _cancellationRegistration.Dispose();
                        Debug.Assert(_waiterIsReader, "We're done writing, so a waiter must be a reader.");
                        if (completedCopy.IsFaulted)
                        {
                            _asyncOp.TrySetException(completedCopy.Exception.InnerException);
                            _copyTask = completedCopy;
                        }
                        else if (completedCopy.IsCanceled)
                        {
                            _asyncOp.TrySetCanceled();
                            _copyTask = completedCopy;
                        }
                        else
                        {
                            _asyncOp.TrySetResult(0);
                            _copyTask = s_zeroTask;
                        }
                        Update(false, null, 0, 0, null);
                    }
                    else
                    {
                        _copyTask = completedCopy.Status == TaskStatus.RanToCompletion ? s_zeroTask : completedCopy;
                    }
                }
            }

            /// <summary>Returns a task that's faulted or canceled based on the specified task's state.</summary>
            private static async Task<T> PropagateError<T>(Task failedTask)
            {
                Debug.Assert(failedTask.IsFaulted || failedTask.IsCanceled, "Task must have already faulted or been canceled");
                await failedTask;
                return default(T); // should never get here
            }

            /// <summary>Updates the state on the stream to the specified values.</summary>
            private void Update(bool isReader, byte[] buffer, int offset, int count, TaskCompletionSource<int> asyncOp)
            {
                _waiterIsReader = isReader;
                _buffer = buffer;
                _offset = offset;
                _count = count;
                _asyncOp = asyncOp;
            }

            /// <summary>Creates a new TaskCompletionSource to use to represent a pending read or write.</summary>
            private static TaskCompletionSource<int> NewTcs()
            {
                return new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
            }

            public override long Length
            {
                get { throw new NotSupportedException(); }
            }

            public override long Position
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }

            public override void Flush() { }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }
        }
    }
}
