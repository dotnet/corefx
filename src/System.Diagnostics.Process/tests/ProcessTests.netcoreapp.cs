// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public void StartProcessWithArgumentList()
        {
            List<string> argList = new List<string>();
            argList.Add("arg1");
            argList.Add("arg2");

            ProcessStartInfo psi = new ProcessStartInfo(GetCurrentProcessName(), argList);
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
        public void StartProcessWithSameArgumentList()
        {
            List<string> argList = new List<string>();
            argList.Add("arg1");
            argList.Add("arg2");

            ProcessStartInfo psi = new ProcessStartInfo(GetCurrentProcessName(), argList);
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
        public void BothArgumentAndArgumentListSet()
        {
            List<string> argList = new List<string>();
            argList.Add("arg1");
            argList.Add("arg2");

            ProcessStartInfo psi = new ProcessStartInfo(GetCurrentProcessName(), argList);
            psi.Arguments = "arg3";
            Process testProcess = CreateProcess();
            testProcess.StartInfo = psi;
            Assert.Throws<InvalidOperationException>(() => testProcess.Start());
        }
    }
}
