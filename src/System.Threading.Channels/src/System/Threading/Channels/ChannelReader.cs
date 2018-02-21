﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace System.Threading.Channels
{
    /// <summary>
    /// Provides a base class for reading from a channel.
    /// </summary>
    /// <typeparam name="T">Specifies the type of data that may be read from the channel.</typeparam>
    public abstract class ChannelReader<T>
    {
        /// <summary>
        /// Gets a <see cref="Task"/> that completes when no more data will ever
        /// be available to be read from this channel.
        /// </summary>
        public virtual Task Completion => ChannelUtilities.s_neverCompletingTask;

        /// <summary>Attempts to read an item to the channel.</summary>
        /// <param name="item">The read item, or a default value if no item could be read.</param>
        /// <returns>true if an item was read; otherwise, false if no item was read.</returns>
        public abstract bool TryRead(out T item);

        /// <summary>Returns a <see cref="Task{Boolean}"/> that will complete when data is available to read.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the wait operation.</param>
        /// <returns>
        /// A <see cref="Task{Boolean}"/> that will complete with a <c>true</c> result when data is available to read
        /// or with a <c>false</c> result when no further data will ever be available to be read.
        /// </returns>
        public abstract Task<bool> WaitToReadAsync(CancellationToken cancellationToken = default);

        /// <summary>Asynchronously reads an item from the channel.</summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the read operation.</param>
        /// <returns>A <see cref="ValueTask{TResult}"/> that represents the asynchronous read operation.</returns>
        public virtual ValueTask<T> ReadAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return new ValueTask<T>(Task.FromCanceled<T>(cancellationToken));
            }

            try
            {
                if (TryRead(out T fastItem))
                {
                    return new ValueTask<T>(fastItem);
                }
            }
            catch (Exception exc) when (!(exc is ChannelClosedException || exc is OperationCanceledException))
            {
                return new ValueTask<T>(Task.FromException<T>(exc));
            }

            return ReadAsyncCore(cancellationToken);

            async ValueTask<T> ReadAsyncCore(CancellationToken ct)
            {
                while (true)
                {
                    if (!await WaitToReadAsync(ct).ConfigureAwait(false))
                    {
                        throw new ChannelClosedException();
                    }

                    if (TryRead(out T item))
                    {
                        return item;
                    }
                }
            }
        }
    }
}
