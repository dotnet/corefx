﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Xunit.Performance;
using Xunit;

namespace System.IO.Pipes.Tests
{
    public class Perf_AnonymousPipeStream_ServerIn_ClientOut : Perf_PipeTest
    {
        protected override ServerClientPair CreateServerClientPair()
        {
            ServerClientPair ret = new ServerClientPair();
            ret.readablePipe = new AnonymousPipeServerStream(PipeDirection.In);
            ret.writeablePipe = new AnonymousPipeClientStream(PipeDirection.Out, ((AnonymousPipeServerStream)ret.readablePipe).ClientSafePipeHandle);
            return ret;
        }
    }

    public class Perf_AnonymousPipeStream_ServerOut_ClientIn : Perf_PipeTest
    {
        protected override ServerClientPair CreateServerClientPair()
        {
            ServerClientPair ret = new ServerClientPair();
            ret.writeablePipe = new AnonymousPipeServerStream(PipeDirection.Out);
            ret.readablePipe = new AnonymousPipeClientStream(PipeDirection.In, ((AnonymousPipeServerStream)ret.writeablePipe).ClientSafePipeHandle);
            return ret;
        }
    }
}
