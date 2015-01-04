// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using System.Diagnostics;
using Xunit;

namespace Test_System_Diagnostics_Process
{
    public partial class ProcessTest
    {
        [System.Runtime.InteropServices.DllImport("api-ms-win-core-console-l1-1-0.dll")]
        private extern static int GetConsoleCP();

        [System.Runtime.InteropServices.DllImport("api-ms-win-core-console-l1-1-0.dll")]
        private extern static int GetConsoleOutputCP();

        [System.Runtime.InteropServices.DllImport("api-ms-win-core-console-l1-1-0.dll")]
        private extern static int SetConsoleCP(int codePage);

        [System.Runtime.InteropServices.DllImport("api-ms-win-core-console-l1-1-0.dll")]
        private extern static int SetConsoleOutputCP(int codePage);

        private const int s_ConsoleEncoding = 437;

        [Fact]
        [ActiveIssue(335)]
        public static void Process_EncodingBeforeProvider()
        {
            SetConsoleCP(s_ConsoleEncoding);
            SetConsoleOutputCP(s_ConsoleEncoding);

            int inputEncoding = GetConsoleCP();
            int outputEncoding = GetConsoleOutputCP();

            try
            {
                Process p = CreateProcessInfinite();
                p.StartInfo.RedirectStandardInput = true;
                p.Start();
                Assert.True(p.StandardInput.Encoding.CodePage == Encoding.UTF8.CodePage, "Process_EncodingBeforeProvider001 failed");
                p.Kill();
                p.WaitForExit();

                p = CreateProcessInfinite();
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                Assert.True(p.StandardOutput.CurrentEncoding.CodePage == Encoding.UTF8.CodePage, "Process_EncodingBeforeProvider002 failed");
                p.Kill();
                p.WaitForExit();

                p = CreateProcessInfinite();
                p.StartInfo.RedirectStandardError = true;
                p.Start();
                Assert.True(p.StandardError.CurrentEncoding.CodePage == Encoding.UTF8.CodePage, "Process_EncodingBeforeProvider003 failed");
                p.Kill();
                p.WaitForExit();


                // Register the codeprovider which will ensure 437 is enabled.
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                p = CreateProcessInfinite();
                p.StartInfo.RedirectStandardInput = true;
                p.Start();
                Assert.True(p.StandardInput.Encoding.CodePage == inputEncoding, "Process_EncodingBeforeProvider004 failed");
                p.Kill();
                p.WaitForExit();

                p = CreateProcessInfinite();
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                Assert.True(p.StandardOutput.CurrentEncoding.CodePage == outputEncoding, "Process_EncodingBeforeProvider005 failed");
                p.Kill();
                p.WaitForExit();

                p = CreateProcessInfinite();
                p.StartInfo.RedirectStandardError = true;
                p.Start();
                Assert.True(p.StandardError.CurrentEncoding.CodePage == outputEncoding, "Process_EncodingBeforeProvider006 failed");
                p.Kill();
                p.WaitForExit();
            }
            finally
            {
                foreach (Process p in Process.GetProcessesByName(s_ProcessName))
                {
                    p.Kill();
                    p.WaitForExit();
                }
            }
        }
    }
}