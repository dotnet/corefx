// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.DotNet.RemoteExecutor;
using Xunit;

namespace System.IO.Tests
{
    public sealed class MiniDumpTest
    {
        [PlatformSpecific(TestPlatforms.Windows)]
        [Fact]
        public void TestMiniDumpWrite()
        {
            RemoteInvokeHandle handle = RemoteExecutor.Invoke(() =>
            {
                Console.WriteLine(DateTime.Now.ToString());
                Thread.CurrentThread.Join();
            }, new RemoteInvokeOptions() { CheckExitCode = false, StartInfo = new ProcessStartInfo() { RedirectStandardOutput = true } });

            string s = Environment.GetEnvironmentVariable("HELIX_WORKITEM_UPLOAD_ROOT");
            Assert.NotEmpty(s);
            Console.WriteLine(s);

            string dest = Path.Combine(s, Path.GetRandomFileName() + ".dmp");
            Console.WriteLine(dest);

            using (handle)
            using (var fs = File.Create(dest))
            {
                Assert.NotEmpty(handle.Process.StandardOutput.ReadLine());

                const MINIDUMP_TYPE MiniDumpType =
                    MINIDUMP_TYPE.MiniDumpWithFullMemory |
                    MINIDUMP_TYPE.MiniDumpIgnoreInaccessibleMemory;

                int result = MiniDumpWriteDump(handle.Process.Handle, handle.Process.Id, fs.SafeFileHandle.DangerousGetHandle(), MiniDumpType, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                int lastError = Marshal.GetLastWin32Error();

                handle.Process.Kill();
                if (result == 0)
                {
                    throw new Win32Exception(lastError);
                }

                throw new Exception("generated minidump for child");
            }
        }

        [DllImport("DbgHelp.dll", SetLastError = true)]
        private static extern int MiniDumpWriteDump(
            IntPtr hProcess,
            int ProcessId,
            IntPtr hFile,
            MINIDUMP_TYPE DumpType,
            IntPtr ExceptionParam,
            IntPtr UserStreamParam,
            IntPtr CallbackParam);

        private enum MINIDUMP_TYPE : int
        {
            MiniDumpNormal = 0x00000000,
            MiniDumpWithDataSegs = 0x00000001,
            MiniDumpWithFullMemory = 0x00000002,
            MiniDumpWithHandleData = 0x00000004,
            MiniDumpFilterMemory = 0x00000008,
            MiniDumpScanMemory = 0x00000010,
            MiniDumpWithUnloadedModules = 0x00000020,
            MiniDumpWithIndirectlyReferencedMemory = 0x00000040,
            MiniDumpFilterModulePaths = 0x00000080,
            MiniDumpWithProcessThreadData = 0x00000100,
            MiniDumpWithPrivateReadWriteMemory = 0x00000200,
            MiniDumpWithoutOptionalData = 0x00000400,
            MiniDumpWithFullMemoryInfo = 0x00000800,
            MiniDumpWithThreadInfo = 0x00001000,
            MiniDumpWithCodeSegs = 0x00002000,
            MiniDumpWithoutAuxiliaryState = 0x00004000,
            MiniDumpWithFullAuxiliaryState = 0x00008000,
            MiniDumpWithPrivateWriteCopyMemory = 0x00010000,
            MiniDumpIgnoreInaccessibleMemory = 0x00020000,
            MiniDumpWithTokenInformation = 0x00040000,
            MiniDumpWithModuleHeaders = 0x00080000,
            MiniDumpFilterTriage = 0x00100000,
            MiniDumpValidTypeFlags = 0x001fffff
        }
    }

    public class File_AppendText : File_ReadWriteAllText
    {
        protected override void Write(string path, string content)
        {
            var writer = File.AppendText(path);
            writer.Write(content);
            writer.Dispose();
        }

        [Fact]
        public override void Overwrite()
        {
            string path = GetTestFilePath();
            string lines = new string('c', 200);
            string appendLines = new string('b', 100);
            Write(path, lines);
            Write(path, appendLines);
            Assert.Equal(lines + appendLines, Read(path));
        }
    }

    public class File_AppendAllText : File_ReadWriteAllText
    {
        protected override void Write(string path, string content)
        {
            File.AppendAllText(path, content);
        }

        [Fact]
        public override void Overwrite()
        {
            string path = GetTestFilePath();
            string lines = new string('c', 200);
            string appendLines = new string('b', 100);
            Write(path, lines);
            Write(path, appendLines);
            Assert.Equal(lines + appendLines, Read(path));
        }
    }

    public class File_AppendAllText_Encoded : File_AppendAllText
    {
        protected override void Write(string path, string content)
        {
            File.AppendAllText(path, content, new UTF8Encoding(false));
        }

        [Fact]
        public void NullEncoding()
        {
            Assert.Throws<ArgumentNullException>(() => File.AppendAllText(GetTestFilePath(), "Text", null));
        }
    }

    public class File_AppendAllLines : File_ReadWriteAllLines_Enumerable
    {
        protected override void Write(string path, string[] content)
        {
            File.AppendAllLines(path, content);
        }

        [Fact]
        public override void Overwrite()
        {
            string path = GetTestFilePath();
            string[] lines = new string[] { new string('c', 200) };
            string[] appendLines = new string[] { new string('b', 100) };
            Write(path, lines);
            Write(path, appendLines);
            Assert.Equal(new string[] { lines[0], appendLines[0] }, Read(path));
        }
    }

    public class File_AppendAllLines_Encoded : File_AppendAllLines
    {
        protected override void Write(string path, string[] content)
        {
            File.AppendAllLines(path, content, new UTF8Encoding(false));
        }

        [Fact]
        public void NullEncoding()
        {
            Assert.Throws<ArgumentNullException>(() => File.AppendAllLines(GetTestFilePath(), new string[] { "Text" }, null));
        }
    }
}
