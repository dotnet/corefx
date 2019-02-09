using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Pipelines
{
    public static class StreamPipeExtensions
    {
        /// <summary>
        /// Asynchronously reads the bytes from the <see cref="Stream"/> and writes them to the specified <see cref="PipeWriter"/>, using a cancellation token.
        /// </summary>
        /// <param name="source">The stream from which the contents of the current stream will be copied.</param>
        /// <param name="destination">The <see cref="PipeWriter"/> to which the contents of the source stream will be copied.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A task that represents the asynchronous copy operation.</returns>
        public static Task CopyToAsync(this Stream source, PipeWriter destination, CancellationToken cancellationToken = default)
        {
            return destination.CopyFromAsync(source);
        }
    }
}
