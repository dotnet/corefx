using System.Buffers;

namespace System.IO.Pipelines
{
    public readonly struct ReadResult
    {
        internal readonly ReadResult<byte> _readResult;

        internal ReadResult(ReadResult<byte> byteResult)
        {
            _readResult = byteResult;
        }

        public ReadResult(ReadOnlySequence<byte> buffer, bool isCanceled, bool isCompleted)
        {
            _readResult = new ReadResult<byte>(buffer, isCanceled, isCompleted);
        }

        /// <summary>
        /// The <see cref="ReadOnlySequence{Byte}"/> that was read.
        /// </summary>
        public ReadOnlySequence<byte> Buffer => _readResult.Buffer;

        /// <summary>
        /// True if the current <see cref="PipeReader{Byte}.ReadAsync"/> operation was canceled, otherwise false.
        /// </summary>
        public bool IsCanceled => _readResult.IsCanceled;

        /// <summary>
        /// True if the <see cref="PipeReader"/> is complete.
        /// </summary>
        public bool IsCompleted => _readResult.IsCompleted;

        public static implicit operator ReadResult(ReadResult<byte> byteResult) =>
            new ReadResult(byteResult);
    }
}
