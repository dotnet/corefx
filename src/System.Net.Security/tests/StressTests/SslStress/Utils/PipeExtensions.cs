// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace SslStress.Utils
{
    public static class PipeExtensions
    {
        // Adapted from https://devblogs.microsoft.com/dotnet/system-io-pipelines-high-performance-io-in-net/
        public static async Task ReadLinesUsingPipesAsync(this Stream stream, Func<ReadOnlySequence<byte>, Task> callback, CancellationToken token = default, char separator = '\n')
        {
            var pipe = new Pipe();
            PipeWriter writer = pipe.Writer;
            PipeReader reader = pipe.Reader;

            try
            {
                await TaskExtensions.WhenAllCancelOnFirstException(token, FillPipeAsync, ReadPipeAsync);
            }
            catch when (token.IsCancellationRequested)
            {

            }

            async Task FillPipeAsync(CancellationToken token)
            {
                while (!token.IsCancellationRequested)
                {
                    Memory<byte> memory = writer.GetMemory(512);
                    int bytesRead = await stream.ReadAsync(memory, token);

                    if (bytesRead == 0)
                    {
                        break;
                    }

                    writer.Advance(bytesRead);
                    FlushResult result = await writer.FlushAsync(token);

                    if (result.IsCompleted)
                    {
                        break;
                    }
                }

                writer.Complete();
            }

            async Task ReadPipeAsync(CancellationToken token)
            {
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        ReadResult result = await reader.ReadAsync(token);
                        ReadOnlySequence<byte> buffer = result.Buffer;
                        SequencePosition? position;

                        do
                        {
                            position = buffer.PositionOf((byte)separator);

                            if (position != null)
                            {
                                await callback(buffer.Slice(0, position.Value));
                                buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
                            }
                        }
                        while (position != null);

                        reader.AdvanceTo(buffer.Start, buffer.End);

                        if (result.IsCompleted)
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    reader.Complete();
                }
            }
        }
    }
}
