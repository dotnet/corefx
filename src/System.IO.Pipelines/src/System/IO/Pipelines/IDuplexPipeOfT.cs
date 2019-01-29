// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IO.Pipelines
{
    /// <summary>
    /// Defines a class that provides a duplex pipe from which data can be read from and written to.
    /// </summary>
    public interface IDuplexPipe<T>
    {
        /// <summary>
        /// Gets the <see cref="PipeReader{T}"/> half of the duplex pipe.
        /// </summary>
        PipeReader<T> Input { get; }

        /// <summary>
        /// Gets the <see cref="PipeWriter{T}"/> half of the duplex pipe.
        /// </summary>
        PipeWriter<T> Output { get; }
    }
}
