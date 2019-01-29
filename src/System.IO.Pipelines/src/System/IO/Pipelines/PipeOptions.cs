using System.Buffers;

namespace System.IO.Pipelines
{
    /// <summary>
    /// Represents a set of <see cref="Pipe"/> options
    /// </summary>
    public class PipeOptions : PipeOptions<byte>
    {
        private const int DefaultMinimumSegmentSize = 2048;

        private const int DefaultResumeWriterThreshold = DefaultMinimumSegmentSize * Pipe<byte>.SegmentPoolSize / 2;

        private const int DefaultPauseWriterThreshold = DefaultMinimumSegmentSize * Pipe<byte>.SegmentPoolSize;

        /// <summary>
        /// Default instance of <see cref="PipeOptions"/>
        /// </summary>
        public new static PipeOptions Default { get; } = new PipeOptions();

        public PipeOptions(
            MemoryPool<byte> pool = null,
            PipeScheduler readerScheduler = null,
            PipeScheduler writerScheduler = null,
            long pauseWriterThreshold = DefaultPauseWriterThreshold,
            long resumeWriterThreshold = DefaultResumeWriterThreshold,
            int minimumSegmentSize = DefaultMinimumSegmentSize,
            bool useSynchronizationContext = true)
            : base(pool, readerScheduler, writerScheduler,
                  pauseWriterThreshold, resumeWriterThreshold, minimumSegmentSize,
                  useSynchronizationContext)
        { }
    }
}
