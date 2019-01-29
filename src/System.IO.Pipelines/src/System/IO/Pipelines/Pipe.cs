namespace System.IO.Pipelines
{
    public sealed class Pipe
    {
        private readonly Pipe<byte> _pipeImpl;

        public Pipe() : this(PipeOptions.Default)
        {
        }

        public Pipe(PipeOptions options)
        {
            _pipeImpl = new Pipe<byte>(options);
        }

        internal long Length => _pipeImpl.Length;

        /// <summary>
        /// Gets the <see cref="T:System.IO.Pipelines.PipeReader" /> for this pipe.
        /// </summary>
        public PipeReader Reader => new BytePipeReaderWrapper(_pipeImpl, _pipeImpl.Reader);

        /// <summary>
        /// Gets the <see cref="T:System.IO.Pipelines.PipeWriter" /> for this pipe.
        /// </summary>
        public PipeWriter Writer => new BytePipeWriterWrapper(_pipeImpl.Writer);

        /// <summary>
        /// Resets the pipe
        /// </summary>
        public void Reset() => _pipeImpl.Reset();
    }
}
