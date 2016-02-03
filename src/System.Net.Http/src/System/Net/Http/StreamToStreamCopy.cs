// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;
using System.IO;
using System.Threading.Tasks;

namespace System.Net.Http
{
    // This helper class is used to copy the content of a source stream to a destination stream.
    // The type verifies if the source and/or destination stream are MemoryStreams (or derived types). If so, sync
    // read/write is used on the MemoryStream to avoid context switches.
    internal static class StreamToStreamCopy
    {
        public static async Task CopyAsync(Stream source, Stream destination, int bufferSize, bool disposeSource)
        {
            Contract.Requires(source != null);
            Contract.Requires(destination != null);
            Contract.Requires(bufferSize > 0);

            byte[] buffer = new byte[bufferSize];

            // If both streams are MemoryStreams, just copy the whole content at once to avoid context switches.
            // This will not block since it will just result in a memcopy.
            if (source is MemoryStream && destination is MemoryStream)
            {
                for (; ;)
                {
                    int bytesRead = source.Read(buffer, 0, bufferSize);
                    if (bytesRead == 0)
                        break;
                    destination.Write(buffer, 0, bytesRead);
                }
            }
            else
            {
                for (; ;)
                {
                    int bytesRead = await source.ReadAsync(buffer, 0, bufferSize).ConfigureAwait(false);
                    if (bytesRead == 0)
                        break;
                    await destination.WriteAsync(buffer, 0, bytesRead).ConfigureAwait(false);
                }
            }

            try
            {
                if (disposeSource)
                {
                    source.Dispose();
                }
            }
            catch (Exception e)
            {
                // Dispose() should never throw, but since we're on an async codepath, make sure to catch the exception.
                if (NetEventSource.Log.IsEnabled()) NetEventSource.Exception(NetEventSource.ComponentType.Http, null, "CopyAsync", e);
            }
        }
    }
}
