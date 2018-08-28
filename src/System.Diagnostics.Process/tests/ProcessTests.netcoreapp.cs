﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Diagnostics.Tests
{
    public partial class ProcessTests : ProcessTestBase
    {
        [Fact]
        public void Start_HasStandardInputEncodingNonRedirected_ThrowsInvalidOperationException()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "FileName",
                    RedirectStandardInput = false,
                    StandardInputEncoding = Encoding.UTF8
                }
            };

            Assert.Throws<InvalidOperationException>(() => process.Start());
        }

        [Fact]
        public void Start_StandardInputEncodingPropagatesToStreamWriter()
        {
            var process = CreateProcessPortable(RemotelyInvokable.Dummy);
            process.StartInfo.RedirectStandardInput = true;
            var encoding = new UTF32Encoding(bigEndian: false, byteOrderMark: true);
            process.StartInfo.StandardInputEncoding = encoding;
            process.Start();

            Assert.Same(encoding, process.StandardInput.Encoding);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "Retrieving information about local processes is not supported on uap")]
        public void StartProcessWithArgumentList()
        {
            ProcessStartInfo psi = new ProcessStartInfo(GetCurrentProcessName());
            psi.ArgumentList.Add("arg1");
            psi.ArgumentList.Add("arg2");

            Process testProcess = CreateProcess();
            testProcess.StartInfo = psi;

            try
            {
                testProcess.Start();
                Assert.Equal(string.Empty, testProcess.StartInfo.Arguments);
            }
            finally
            {
                if (!testProcess.HasExited)
                    testProcess.Kill();

                Assert.True(testProcess.WaitForExit(WaitInMS));
            }
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "Retrieving information about local processes is not supported on uap")]
        public void StartProcessWithSameArgumentList()
        {
            ProcessStartInfo psi = new ProcessStartInfo(GetCurrentProcessName());
            psi.ArgumentList.Add("arg1");
            psi.ArgumentList.Add("arg2");

            Process testProcess = CreateProcess();
            Process secondTestProcess = CreateProcess();
            testProcess.StartInfo = psi;
            try
            {
                testProcess.Start();
                Assert.Equal(string.Empty, testProcess.StartInfo.Arguments);
                secondTestProcess.StartInfo = psi;
                secondTestProcess.Start();
                Assert.Equal(string.Empty, secondTestProcess.StartInfo.Arguments);
            }
            finally
            {
                if (!testProcess.HasExited)
                    testProcess.Kill();

                Assert.True(testProcess.WaitForExit(WaitInMS));

                if (!secondTestProcess.HasExited)
                    secondTestProcess.Kill();

                Assert.True(testProcess.WaitForExit(WaitInMS));
            }
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "Retrieving information about local processes is not supported on uap")]
        public void BothArgumentCtorAndArgumentListSet()
        {
            ProcessStartInfo psi = new ProcessStartInfo(GetCurrentProcessName(), "arg3");
            psi.ArgumentList.Add("arg1");
            psi.ArgumentList.Add("arg2");

            Process testProcess = CreateProcess();
            testProcess.StartInfo = psi;
            Assert.Throws<InvalidOperationException>(() => testProcess.Start());
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "Retrieving information about local processes is not supported on uap")]
        public void BothArgumentSetAndArgumentListSet()
        {
            ProcessStartInfo psi = new ProcessStartInfo(GetCurrentProcessName());
            psi.Arguments = "arg3";
            psi.ArgumentList.Add("arg1");
            psi.ArgumentList.Add("arg2");

            Process testProcess = CreateProcess();
            testProcess.StartInfo = psi;
            Assert.Throws<InvalidOperationException>(() => testProcess.Start());
        }

        [PlatformSpecific(TestPlatforms.Windows)]
        [Fact]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.Uap)]
        public void VerifyingCiBehavior()
        {
            throw new Exception();
        }

        [PlatformSpecific(TestPlatforms.Windows)]
        [Fact]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.Uap)]
        public void Kill_EntireProcessTree_ThrowsPlatformNotSupportedException()
        {
            var process = new Process();
            Assert.Throws<PlatformNotSupportedException>(() => process.Kill(entireProcessTree: true));
            Assert.Throws<PlatformNotSupportedException>(() => process.Kill(entireProcessTree: false));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap)]
        public void Kill_EntireProcessTree_True_ProcessNotStarted_ThrowsInvalidOperationException()
        {
            var process = new Process();
            Assert.Throws<InvalidOperationException>(() => process.Kill(entireProcessTree: true));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap)]
        public async Task Kill_EntireProcessTree_False_OnlyRootProcessTerminated()
        {
            IReadOnlyList<Process> tree = null;
            try
            {
                tree = CreateProcessTree();
                Process parentProcess = tree.First();

                parentProcess.Kill(entireProcessTree: false);

                // Since Kill() is fire-and-forget, wait a moment for it to take effect
                await Task.Delay(10);

                var actual = tree.Select(p => p.HasExited).ToList();
                Assert.Equal(new[] { true, false, false }, actual);
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
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap)]
        public async Task Kill_EntireProcessTree_True_EntireTreeTerminated()
        {
            IReadOnlyList<Process> tree = null;
            try
            {
                tree = CreateProcessTree();
                Process parentProcess = tree.First();

                parentProcess.Kill(entireProcessTree: true);

                // Since Kill() is fire-and-forget, wait a moment for it to take effect
                await Task.Delay(10);

                var actual = tree.Select(p => p.HasExited).ToList();
                Assert.True(actual.All(x => x == true));
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
                using (var pipeServer = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable))
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
