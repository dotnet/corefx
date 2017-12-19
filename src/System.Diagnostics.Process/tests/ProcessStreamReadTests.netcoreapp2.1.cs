using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Diagnostics.Tests
{
    public partial class ProcessStreamReadTests : ProcessTestBase
    {
        [Fact]
        public void TestCustomStandardInputEncoding()
        {
            var process = CreateProcessPortable(RemotelyInvokable.ReadLineWithCustomEncodingWriteLineWithUtf8, Encoding.UTF32.WebName);
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.StandardInputEncoding = Encoding.UTF32;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.Start();

            const string TestLine = "\U0001f627\U0001f62e\U0001f62f";
            process.StandardInput.WriteLine(TestLine);
            process.StandardInput.Close();

            var output = process.StandardOutput.ReadLine();
            Assert.Equal(TestLine, output);

            Assert.True(process.WaitForExit(WaitInMS));
            Assert.Equal(RemotelyInvokable.SuccessExitCode, process.ExitCode);
        }
    }
}
