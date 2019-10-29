﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;

namespace System.IO.Pipelines
{
    /// <summary>
    /// Represents a set of options for controlling the creation of the <see cref="PipeWriter"/>.
    /// </summary>
    public class StreamPipeWriterOptions
    {
        private const int DefaultMinimumBufferSize = 4096;

        internal static StreamPipeWriterOptions s_default = new StreamPipeWriterOptions();

        /// <summary>
        /// Creates a new instance of <see cref="StreamPipeWriterOptions"/>.
        /// </summary>
        public StreamPipeWriterOptions(MemoryPool<byte>? pool = null, int minimumBufferSize = -1, bool leaveOpen = false)
        {
            Pool = pool ?? MemoryPool<byte>.Shared;

            MinimumBufferSize =
                minimumBufferSize == -1 ? DefaultMinimumBufferSize :
                minimumBufferSize <= 0 ? throw new ArgumentOutOfRangeException(nameof(minimumBufferSize)) :
                minimumBufferSize;

            LeaveOpen = leaveOpen;
        }

        /// <summary>
        /// The minimum buffer size to use when renting memory from the <see cref="Pool"/>.
        /// </summary>
        public int MinimumBufferSize { get; }

        /// <summary>
        /// The <see cref="MemoryPool{T}"/> to use when allocating memory.
        /// </summary>
        public MemoryPool<byte> Pool { get; }

        /// <summary>
        /// Leave underlying stream open after pipe writer completes.
        /// </summary>
        public bool LeaveOpen { get; }
    }
}
