// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;

namespace System.IO.Pipelines
{
    /// <summary>
    /// The result of a <see cref="PipeReader{T}.ReadAsync"/> call.
    /// </summary>
    public readonly struct ReadResult<T>
    {
        internal readonly ReadOnlySequence<T> _resultBuffer;
        internal readonly ResultFlags _resultFlags;

        /// <summary>
        /// Creates a new instance of <see cref="ReadResult{T}"/> setting <see cref="IsCanceled"/> and <see cref="IsCompleted"/> flags.
        /// </summary>
        public ReadResult(ReadOnlySequence<T> buffer, bool isCanceled, bool isCompleted)
        {
            _resultBuffer = buffer;
            _resultFlags = ResultFlags.None;

            if (isCompleted)
            {
                _resultFlags |= ResultFlags.Completed;
            }
            if (isCanceled)
            {
                _resultFlags |= ResultFlags.Canceled;
            }
        }

        /// <summary>
        /// The <see cref="ReadOnlySequence{T}"/> that was read.
        /// </summary>
        public ReadOnlySequence<T> Buffer => _resultBuffer;

        /// <summary>
        /// True if the current <see cref="PipeReader{T}.ReadAsync"/> operation was canceled, otherwise false.
        /// </summary>
        public bool IsCanceled => (_resultFlags & ResultFlags.Canceled) != 0;

        /// <summary>
        /// True if the <see cref="PipeReader{T}"/> is complete.
        /// </summary>
        public bool IsCompleted => (_resultFlags & ResultFlags.Completed) != 0;
    }
}
