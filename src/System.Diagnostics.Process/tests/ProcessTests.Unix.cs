// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Security;
using Xunit;
using Xunit.NetCore.Extensions;

namespace System.Diagnostics.Tests
{
    public partial class ProcessTests : ProcessTestBase
    {
        [Fact]
        private void TestWindowApisUnix()
        {
            // This tests the hardcoded implementations of these APIs on Unix.
            using (Process p = Process.GetCurrentProcess())
            {
                Assert.True(p.Responding);
                Assert.Equal(string.Empty, p.MainWindowTitle);
                Assert.False(p.CloseMainWindow());
                Assert.Throws<InvalidOperationException>(()=>p.WaitForInputIdle());
            }
        }

        [Fact]
        public void MainWindowHandle_GetUnix_ThrowsPlatformNotSupportedException()
        {
            CreateDefaultProcess();

            Assert.Equal(IntPtr.Zero, _process.MainWindowHandle);
        }

        [Fact]
        public void TestProcessOnRemoteMachineUnix()
        {
            Process currentProcess = Process.GetCurrentProcess();

            Assert.Throws<PlatformNotSupportedException>(() => Process.GetProcessesByName(currentProcess.ProcessName, "127.0.0.1"));
            Assert.Throws<PlatformNotSupportedException>(() => Process.GetProcessById(currentProcess.Id, "127.0.0.1"));
        }

        [Theory]
        [MemberData(nameof(MachineName_Remote_TestData))]
        public void GetProcessesByName_RemoteMachineNameUnix_ThrowsPlatformNotSupportedException(string machineName)
        {
            Process currentProcess = Process.GetCurrentProcess();
            Assert.Throws<PlatformNotSupportedException>(() => Process.GetProcessesByName(currentProcess.ProcessName, machineName));
        }

        [Fact]
        public void TestRootGetProcessById()
        {
            Process p = Process.GetProcessById(1);
            Assert.Equal(1, p.Id);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Linux)]
        public void ProcessStart_UseShellExecute_OnLinux_ThrowsIfNoProgramInstalled()
        {
            if (!s_allowedProgramsToRun.Any(program => IsProgramInstalled(program)))
            {
                Console.WriteLine($"None of the following programs were installed on this machine: {string.Join(",", s_allowedProgramsToRun)}.");
                Assert.Throws<Win32Exception>(() => Process.Start(new ProcessStartInfo { UseShellExecute = true, FileName = Environment.CurrentDirectory }));
            }
        }

        [Fact]
        [OuterLoop("Opens program")]
        public void ProcessStart_DirectoryNameInCurDirectorySameAsFileNameInExecDirectory_Success()
        {
            string fileToOpen = "dotnet";
            string curDir = Environment.CurrentDirectory;
            string dotnetFolder = Path.Combine(Path.GetTempPath(),"dotnet");
            bool shouldDelete = !Directory.Exists(dotnetFolder);
            try
            {
                Directory.SetCurrentDirectory(Path.GetTempPath());
                Directory.CreateDirectory(dotnetFolder);

                using (var px = Process.Start(fileToOpen))
                {
                    Assert.NotNull(px);
                }
            }
            finally
            {
                if (shouldDelete)
                {
                    Directory.Delete(dotnetFolder);
                }

                Directory.SetCurrentDirectory(curDir);
            }
        }

        [Theory, InlineData(true), InlineData(false)]
        [OuterLoop("Opens program")]
        public void ProcessStart_UseShellExecute_OnUnix_SuccessWhenProgramInstalled(bool isFolder)
        {
            string programToOpen = s_allowedProgramsToRun.FirstOrDefault(program => IsProgramInstalled(program));
            string fileToOpen;
            if (isFolder)
            {
                fileToOpen = Environment.CurrentDirectory;
            }
            else
            {
                fileToOpen = GetTestFilePath() + ".txt";
                File.WriteAllText(fileToOpen, $"{nameof(ProcessStart_UseShellExecute_OnUnix_SuccessWhenProgramInstalled)}");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || programToOpen != null)
            {
                using (var px = Process.Start(new ProcessStartInfo { UseShellExecute = true, FileName = fileToOpen }))
                {
                    Assert.NotNull(px);
                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) // on OSX, process name is dotnet for some reason. Refer to #23972
                    {
                        Assert.Equal(programToOpen, px.ProcessName);
                    }
                    px.Kill();
                    px.WaitForExit();
                    Assert.True(px.HasExited);
                }
            }
        }

        [Theory, InlineData("nano"), InlineData("vi")]
        [PlatformSpecific(TestPlatforms.Linux)]
        [OuterLoop("Opens program")]
        public void ProcessStart_OpenFileOnLinux_UsesSpecifiedProgram(string programToOpenWith)
        {
            if (IsProgramInstalled(programToOpenWith))
            {
                string fileToOpen = GetTestFilePath() + ".txt";
                File.WriteAllText(fileToOpen, $"{nameof(ProcessStart_OpenFileOnLinux_UsesSpecifiedProgram)}");
                using (var px = Process.Start(programToOpenWith, fileToOpen))
                {
                    Assert.Equal(programToOpenWith, px.ProcessName);
                    px.Kill();
                    px.WaitForExit();
                    Assert.True(px.HasExited);
                }
            }
            else
            {
                Console.WriteLine($"Program specified to open file with {programToOpenWith} is not installed on this machine.");
            }
        }

        [Theory, InlineData("/usr/bin/open"), InlineData("/usr/bin/nano")]
        [PlatformSpecific(TestPlatforms.OSX)]
        [OuterLoop("Opens program")]
        public void ProcessStart_OpenFileOnOsx_UsesSpecifiedProgram(string programToOpenWith)
        {
            string fileToOpen = GetTestFilePath() + ".txt";
            File.WriteAllText(fileToOpen, $"{nameof(ProcessStart_OpenFileOnOsx_UsesSpecifiedProgram)}");
            using (var px = Process.Start(programToOpenWith, fileToOpen))
            {
                // Assert.Equal(programToOpenWith, px.ProcessName); // on OSX, process name is dotnet for some reason. Refer to #23972
                Console.WriteLine($"in OSX, {nameof(programToOpenWith)} is {programToOpenWith}, while {nameof(px.ProcessName)} is {px.ProcessName}.");
                px.Kill();
                px.WaitForExit();
                Assert.True(px.HasExited);
            }
        }

        [Theory, InlineData("Safari"), InlineData("\"Google Chrome\"")]
        [PlatformSpecific(TestPlatforms.OSX)]
        [OuterLoop("Opens browser")]
        public void ProcessStart_OpenUrl_UsesSpecifiedApplication(string applicationToOpenWith)
        {
            using (var px = Process.Start("/usr/bin/open", "https://github.com/dotnet/corefx -a " + applicationToOpenWith))
            {
                Assert.NotNull(px);
                px.Kill();
                px.WaitForExit();
                Assert.True(px.HasExited);
            }
        }

        [Theory, InlineData("-a Safari"), InlineData("-a \"Google Chrome\"")]
        [PlatformSpecific(TestPlatforms.OSX)]
        [OuterLoop("Opens browser")]
        public void ProcessStart_UseShellExecuteTrue_OpenUrl_SuccessfullyReadsArgument(string arguments)
        {
            var startInfo = new ProcessStartInfo { UseShellExecute = true, FileName = "https://github.com/dotnet/corefx", Arguments = arguments };
            using (var px = Process.Start(startInfo))
            {
                Assert.NotNull(px);
                px.Kill();
                px.WaitForExit();
                Assert.True(px.HasExited);
            }
        }

        [Fact]
        [Trait(XunitConstants.Category, XunitConstants.RequiresElevation)]
        public void TestPriorityClassUnix()
        {
            CreateDefaultProcess();

            ProcessPriorityClass priorityClass = _process.PriorityClass;

            _process.PriorityClass = ProcessPriorityClass.Idle;
            Assert.Equal(_process.PriorityClass, ProcessPriorityClass.Idle);

            try
            {
                _process.PriorityClass = ProcessPriorityClass.High;
                Assert.Equal(_process.PriorityClass, ProcessPriorityClass.High);

                _process.PriorityClass = ProcessPriorityClass.Normal;
                Assert.Equal(_process.PriorityClass, ProcessPriorityClass.Normal);

                _process.PriorityClass = priorityClass;
            }
            catch (Win32Exception ex)
            {
                Assert.True(!PlatformDetection.IsSuperUser, $"Failed even though superuser {ex.ToString()}");
            }
        }

        [Fact]
        [Trait(XunitConstants.Category, XunitConstants.RequiresElevation)]
        public void TestBasePriorityOnUnix()
        {
            CreateDefaultProcess();

            ProcessPriorityClass originalPriority = _process.PriorityClass;
            Assert.Equal(ProcessPriorityClass.Normal, originalPriority);

            SetAndCheckBasePriority(ProcessPriorityClass.Idle, 19);

            try
            {
                SetAndCheckBasePriority(ProcessPriorityClass.Normal, 0);
                SetAndCheckBasePriority(ProcessPriorityClass.High, -11);
                _process.PriorityClass = originalPriority;
            }
            catch (Win32Exception ex)
            {
                Assert.True(!PlatformDetection.IsSuperUser, $"Failed even though superuser {ex.ToString()}");
            }
        }

        [Fact]
        public void TestStartOnUnixWithBadPermissions()
        {
            string path = GetTestFilePath();
            File.Create(path).Dispose();
            int mode = Convert.ToInt32("644", 8);

            Assert.Equal(0, chmod(path, mode));

            Win32Exception e = Assert.Throws<Win32Exception>(() => Process.Start(path));
            Assert.NotEqual(0, e.NativeErrorCode);
        }

        [Theory]
        [SkipOnTargetFramework(TargetFrameworkMonikers.UapNotUapAot, "Non applicable for uap - RemoteInvoke works differently")]
        [InlineData(@"""abc"" d e", @"abc,d,e")]
        [InlineData(@"""abc""      d e", @"abc,d,e")]
        [InlineData("\"abc\"\t\td\te", @"abc,d,e")]
        [InlineData(@"a\\b d""e f""g h", @"a\\b,de fg,h")]
        [InlineData(@"\ \\ \\\", @"\,\\,\\\")]
        [InlineData(@"a\\\""b c d", @"a\""b,c,d")]
        [InlineData(@"a\\\\""b c"" d e", @"a\\b c,d,e")]
        [InlineData(@"a""b c""d e""f g""h i""j k""l", @"ab cd,ef gh,ij kl")]
        [InlineData(@"a b c""def", @"a,b,cdef")]
        [InlineData(@"""\a\"" \\""\\\ b c", @"\a"" \\\\,b,c")]
        [InlineData("\"\" b \"\"", ",b,")]
        [InlineData("\"\"\"\" b c", "\",b,c")]
        [InlineData("c\"\"\"\" b \"\"\\", "c\",b,\\")]
        [InlineData("\"\"c \"\"b\"\" d\"\\", "c,b,d\\")]
        [InlineData("\"\"a\"\" b d", "a,b,d")]
        [InlineData("b d \"\"a\"\" ", "b,d,a")]
        [InlineData("\\\"\\\"a\\\"\\\" b d", "\"\"a\"\",b,d")]
        [InlineData("b d \\\"\\\"a\\\"\\\"", "b,d,\"\"a\"\"")]
        [InlineData("\"\"\"a\"\"\" c b", "\"a\",c,b")]
        public void TestArgumentParsing(string inputArguments, string expectedArgv)
        {
            var options = new RemoteInvokeOptions
            {
                Start = true,
                StartInfo = new ProcessStartInfo { RedirectStandardOutput = true }
            };

            using (RemoteInvokeHandle handle = RemoteInvokeRaw((Func<string, string, string, int>)RemotelyInvokable.ConcatThreeArguments, inputArguments, options))
            {
                Assert.Equal(expectedArgv, handle.Process.StandardOutput.ReadToEnd());
            }
        }

        [Fact]
        [PlatformSpecific(~TestPlatforms.OSX)] // OSX doesn't support throwing on Process.Start
        public void TestStartOnUnixWithBadFormat()
        {
            string path = GetTestFilePath();
            File.Create(path).Dispose();
            int mode = Convert.ToInt32("744", 8);

            Assert.Equal(0, chmod(path, mode)); // execute permissions

            Win32Exception e = Assert.Throws<Win32Exception>(() => Process.Start(path));
            Assert.NotEqual(0, e.NativeErrorCode);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.OSX)] // OSX doesn't support throwing on Process.Start
        public void TestStartOnOSXWithBadFormat()
        {
            string path = GetTestFilePath();
            File.Create(path).Dispose();
            int mode = Convert.ToInt32("744", 8);

            Assert.Equal(0, chmod(path, mode)); // execute permissions

            using (Process p = Process.Start(path))
            {
                p.WaitForExit();
                Assert.NotEqual(0, p.ExitCode);
            }
        }

        [DllImport("libc")]
        private static extern int chmod(string path, int mode);

        private readonly string[] s_allowedProgramsToRun = new string[] { "xdg-open", "gnome-open", "kfmclient" };
    }
}
