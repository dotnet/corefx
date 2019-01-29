// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Threading;

namespace System.IO.Pipelines
{
    /// <summary>
    /// Represents a set of <see cref="Pipe{T}"/> options
    /// </summary>
    public class PipeOptions<T>
    {
        private const int DefaultMinimumSegmentSize = 2048;

        private const int DefaultResumeWriterThreshold = DefaultMinimumSegmentSize * Pipe<T>.SegmentPoolSize / 2;

        private const int DefaultPauseWriterThreshold = DefaultMinimumSegmentSize * Pipe<T>.SegmentPoolSize;

        /// <summary>
        /// Default instance of <see cref="PipeOptions{T}"/>
        /// </summary>
        public static PipeOptions<T> Default { get; } = new PipeOptions<T>();

        /// <summary>
        /// Creates a new instance of <see cref="PipeOptions{T}"/>
        /// </summary>
        public PipeOptions(
            MemoryPool<T> pool = null,
            PipeScheduler readerScheduler = null,
            PipeScheduler writerScheduler = null,
            long? pauseWriterThreshold = null,
            long? resumeWriterThreshold = null,
            int? minimumSegmentSize = null,
            bool useSynchronizationContext = true)
        {
            if (pauseWriterThreshold.HasValue && pauseWriterThreshold < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.pauseWriterThreshold);
            }

            if (resumeWriterThreshold.HasValue && resumeWriterThreshold < 0 && resumeWriterThreshold > pauseWriterThreshold)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.resumeWriterThreshold);
            }

            Pool = pool ?? MemoryPool<T>.Shared;
            ReaderScheduler = readerScheduler ?? PipeScheduler.ThreadPool;
            WriterScheduler = writerScheduler ?? PipeScheduler.ThreadPool;
            PauseWriterThreshold = pauseWriterThreshold ?? DefaultPauseWriterThreshold;
            ResumeWriterThreshold = resumeWriterThreshold ?? ResumeWriterThreshold;
            MinimumSegmentSize = minimumSegmentSize ?? MinimumSegmentSize;
            UseSynchronizationContext = useSynchronizationContext;
        }

        /// <summary>
        /// Gets a value that determines if asynchronous callbacks should be executed on the <see cref="SynchronizationContext" /> they were captured on.
        /// This takes precedence over the schedulers specified in <see cref="ReaderScheduler"/> and <see cref="WriterScheduler"/>.
        /// </summary>
        public bool UseSynchronizationContext { get; }

        /// <summary>
        /// Gets amount of bytes in <see cref="Pipe"/> when <see cref="PipeWriter{T}.FlushAsync"/> starts blocking
        /// </summary>
        public long PauseWriterThreshold { get; }

        /// <summary>
        /// Gets amount of bytes in <see cref="Pipe"/> when <see cref="PipeWriter{T}.FlushAsync"/> stops blocking
        /// </summary>
        public long ResumeWriterThreshold { get; }

        /// <summary>
        /// Gets minimum size of segment requested from <see cref="Pool"/>
        /// </summary>
        public int MinimumSegmentSize { get; }

        /// <summary>
        /// Gets the <see cref="PipeScheduler"/> used to execute <see cref="PipeWriter"/> callbacks
        /// </summary>
        public PipeScheduler WriterScheduler { get; }

        /// <summary>
        /// Gets the <see cref="PipeScheduler"/> used to execute <see cref="PipeReader"/> callbacks
        /// </summary>
        public PipeScheduler ReaderScheduler { get; }

        /// <summary>
        /// Gets the <see cref="MemoryPool{T}"/> instances used for buffer management
        /// </summary>
        public MemoryPool<T> Pool { get; }
    }
}
