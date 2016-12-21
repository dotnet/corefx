// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace System.Diagnostics.Tests
{
    public class ProcessStandardConsoleTests : ProcessTestBase
    {
        private const int s_ConsoleEncoding = 437;

        [Fact]
        public void TestChangesInConsoleEncoding()
        {
            Action<int> run = expectedCodePage =>
            {
                Process p = CreateProcessLong();
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.Start();

                Assert.Equal(p.StandardInput.Encoding.CodePage, expectedCodePage);
                Assert.Equal(p.StandardOutput.CurrentEncoding.CodePage, expectedCodePage);
                Assert.Equal(p.StandardError.CurrentEncoding.CodePage, expectedCodePage);

                p.Kill();
                Assert.True(p.WaitForExit(WaitInMS));
            };

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                run(Encoding.UTF8.CodePage);
                return;
            }

            int inputEncoding = Interop.GetConsoleCP();
            int outputEncoding = Interop.GetConsoleOutputCP();

            try
            {
                // Don't test this on Windows Nano, Windows Nano only supports UTF8.
                if (File.Exists(Path.Combine(Environment.GetEnvironmentVariable("windir"), "regedit.exe")))
                {
                    Interop.SetConsoleCP(s_ConsoleEncoding);
                    Interop.SetConsoleOutputCP(s_ConsoleEncoding);

                    run(s_ConsoleEncoding);
                }
            }
            finally
            {
                Interop.SetConsoleCP(inputEncoding);
                Interop.SetConsoleOutputCP(outputEncoding);
            }
        }
    }
}
