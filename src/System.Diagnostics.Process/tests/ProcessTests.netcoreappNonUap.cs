// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace System.Diagnostics.Tests
{
    public partial class ProcessTests : ProcessTestBase
    {
        [Fact]
        public void Kill_EntireProcessTree_True_ProcessNotStarted_ThrowsInvalidOperationException()
        {
            var process = new Process();
            Assert.Throws<InvalidOperationException>(() => process.Kill(entireProcessTree: true));
        }

        [Fact]
        public void Kill_EntireProcessTree_False_OnlyRootProcessTerminated()
        {
            IReadOnlyList<Process> tree = null;
            try
            {
                tree = CreateProcessTree();
                Process parentProcess = tree.First();

                parentProcess.Kill(entireProcessTree: false);

                Assert.Equal(new[] { true, false, false }, tree.Select(p => p.HasExited));
            }
            finally
            {
                foreach (Process process in tree)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch
                    {
                        // Best-effort attempt, so ignore any exceptions
                    }
                }
            }
        }

        [Fact]
        public void Kill_EntireProcessTree_True_EntireTreeTerminated()
        {
            IReadOnlyList<Process> tree = null;
            try
            {
                tree = CreateProcessTree();
                Process parentProcess = tree.First();

                parentProcess.Kill(entireProcessTree: true);

                Assert.True(tree.Select(p => p.HasExited).All(x => x == true));
            }
            finally
            {
                foreach (Process process in tree)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch
                    {
                        // Best-effort attempt, so ignore any exceptions
                    }
                }
            }
        }

        private IReadOnlyList<Process> CreateProcessTree()
        {
            (Process Value, string Message) rootResult = ListenForAnonymousPipeMessage(rootPipeHandleString =>
            {
                Process root = CreateProcess(rhs =>
                {
                    (Process Value, string Message) child1Result = ListenForAnonymousPipeMessage(child1PipeHandleString =>
                    {
                        Process child1 = CreateProcess(c1hs =>
                        {
                            Process child2 = CreateProcess(() => WaitForever());
                            child2.Start();

                            SendMessage(child2.Id.ToString(), c1hs);

                            return WaitForever();
                        }, child1PipeHandleString);

                        child1.Start();

                        return child1;
                    });

                    var child1ProcessId = child1Result.Value.Id;
                    var child2ProcessId = child1Result.Message;
                    SendMessage($"{child1ProcessId};{child2ProcessId}", rhs);

                    return WaitForever();
                }, rootPipeHandleString);

                root.Start();

                return root;
            });


            IEnumerable<Process> childProcesses = rootResult.Message
                .Split(';')
                .Select(x => int.Parse(x))
                .Select(pid => Process.GetProcessById(pid));

            return new[] { rootResult.Value }
                .Concat(childProcesses)
                .ToList();

            int WaitForever()
            {
                Thread.Sleep(Timeout.Infinite);

                // never reaches here -- but necessary to satisfy method's signature
                return SuccessExitCode;
            }

            void SendMessage(string message, string handleAsString)
            {
                using (var client = new AnonymousPipeClientStream(PipeDirection.Out, handleAsString))
                {
                    using (var sw = new StreamWriter(client))
                    {
                        sw.AutoFlush = true;
                        sw.WriteLine(message);
                    }
                }
            }

            (T Value, string Message) ListenForAnonymousPipeMessage<T>(Func<string, T> action)
            {
                using (AnonymousPipeServerStream pipeServer = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable))
                {
                    string handleAsString = pipeServer.GetClientHandleAsString();

                    T result = action(handleAsString);

                    pipeServer.DisposeLocalCopyOfClientHandle();

                    using (var sr = new StreamReader(pipeServer))
                    {
                        return (result, sr.ReadLine());
                    }
                }
            }
        }
    }
}
