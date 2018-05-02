// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Test.Common;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Http.Functional.Tests
{
    public abstract class HttpClientHandler_Asynchrony_Test : HttpClientTestBase
    {
        [Theory]
        [InlineData(false, null)]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, null)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task ResponseHeadersRead_SynchronizationContextNotUsedByHandler(bool responseHeadersRead, bool? chunked)
        {
            await Task.Run(async delegate // escape xunit's sync ctx
            {
                await LoopbackServer.CreateClientAndServerAsync(uri =>
                {
                    return Task.Run(() => // allow client and server to run concurrently even though this is all synchronous/blocking
                    {
                        var sc = new TrackingSynchronizationContext();
                        SynchronizationContext.SetSynchronizationContext(sc);

                        using (HttpClient client = CreateHttpClient())
                        {
                            if (responseHeadersRead)
                            {
                                using (HttpResponseMessage resp = client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult())
                                using (Stream respStream = resp.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                                {
                                    byte[] buffer = new byte[0x1000];
                                    while (respStream.ReadAsync(buffer, 0, buffer.Length).GetAwaiter().GetResult() > 0);
                                }
                            }
                            else
                            {
                                client.GetStringAsync(uri).GetAwaiter().GetResult();
                            }
                        }

                        Assert.True(sc.CallStacks.Count == 0, "Sync Ctx used: " + string.Join(Environment.NewLine + Environment.NewLine, sc.CallStacks));
                    });
                }, async server =>
                {
                    await server.AcceptConnectionAsync(async connection =>
                    {
                        await connection.ReadRequestHeaderAsync();

                        string responseData = string.Concat(Enumerable.Repeat('s', 10_000));
                        string response;
                        switch (chunked)
                        {
                            case false:
                                response = LoopbackServer.GetBytePerChunkHttpResponse(content: responseData);
                                break;

                            case true:
                                response = LoopbackServer.GetHttpResponse(content: responseData);
                                break;

                            default:
                                response = LoopbackServer.GetConnectionCloseResponse(content: responseData);
                                break;
                        }
                        await connection.Writer.WriteAsync(response);
                    });
                }, new LoopbackServer.Options { StreamWrapper = s => new DribbleStream(s) });
            });
        }

        private sealed class TrackingSynchronizationContext : SynchronizationContext
        {
            public readonly List<string> CallStacks = new List<string>();

            public override void OperationStarted() => CallStacks.Add(Environment.StackTrace);
            public override void OperationCompleted() => CallStacks.Add(Environment.StackTrace);

            public override void Post(SendOrPostCallback d, object state)
            {
                CallStacks.Add(Environment.StackTrace);
                ThreadPool.QueueUserWorkItem(delegate
                {
                    SetSynchronizationContext(this);
                    d(state);
                });
            }

            public override void Send(SendOrPostCallback d, object state)
            {
                CallStacks.Add(Environment.StackTrace);
                ThreadPool.QueueUserWorkItem(delegate
                {
                    SynchronizationContext orig = SynchronizationContext.Current;
                    try
                    {
                        SetSynchronizationContext(this);
                        d(state);
                    }
                    finally
                    {
                        SetSynchronizationContext(orig);
                    }
                });
            }
        }
    }
}
