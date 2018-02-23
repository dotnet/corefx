// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Xunit;

namespace System.Diagnostics.Tests
{
    public partial class ProcessTestBase : RemoteExecutorTestBase
    {
        protected const int WaitInMS = 30 * 1000;
        protected Process _process;
        protected readonly List<Process> _processes = new List<Process>();

        protected void CreateDefaultProcess()
        {
            _process = CreateProcessLong();
            _process.Start();
        }

        protected override void Dispose(bool disposing)
        {
            // Wait for all started processes to complete
            foreach (Process p in _processes)
            {
                try
                {
                    if (!p.HasExited)
                    {
                        p.Kill();
                        Assert.True(p.WaitForExit(WaitInMS));
                    }
                }
                catch (InvalidOperationException) { } // in case it was never started
            }

            base.Dispose(disposing);
        }

        protected void AddProcessForDispose(Process p)
        {
            lock (_processes)
            {
                _processes.Add(p);
            }
        }

        protected Process CreateProcess(Func<int> method = null)
        {
            Process p = RemoteInvoke(method ?? (() => SuccessExitCode), new RemoteInvokeOptions { Start = false }).Process;
            AddProcessForDispose(p);
            return p;
        }

        protected Process CreateProcess(Func<string, int> method, string arg)
        {
            Process p = RemoteInvoke(method, arg, new RemoteInvokeOptions { Start = false }).Process;
            AddProcessForDispose(p);
            return p;
        }

        protected void StartSleepKillWait(Process p)
        {
            p.Start();
            Thread.Sleep(200);
            p.Kill();
            Assert.True(p.WaitForExit(WaitInMS));
        }

        /// <summary>
        /// Checks if the program is installed
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        protected static bool IsProgramInstalled(string program)
        {
            string path;
            string pathEnvVar = Environment.GetEnvironmentVariable("PATH");
            char separator = PlatformDetection.IsWindows ? ';' : ':';
            if (pathEnvVar != null)
            {
                var pathParser = new StringParser(pathEnvVar, separator, skipEmpty: true);
                while (pathParser.MoveNext())
                {
                    string subPath = pathParser.ExtractCurrent();
                    path = Path.Combine(subPath, program);
                    if (File.Exists(path))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
