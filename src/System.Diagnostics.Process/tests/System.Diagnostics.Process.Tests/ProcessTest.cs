// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using Xunit;
using System.Text;

namespace System.Diagnostics.ProcessTests
{
    public partial class ProcessTest : IDisposable
    {

        private const string s_ProcessName = "ProcessTest_ConsoleApp.exe";
        private static bool s_Process_EnableRaiseEvents_isExitedEventHandlerCalled = false;
        private Process s_process;

        public ProcessTest()
        {
            s_process = CreateProcessInfinite();
            s_process.Start();
        }

        public Process CreateProcessInfinite()
        {
            Process p = new Process();
            p.StartInfo.FileName = s_ProcessName;
            p.StartInfo.Arguments = "infinite";
            return p;
        }

        Process CreateProcess()
        {
            // Create a process that immediately exits
            Process p = new Process();
            p.StartInfo.FileName = s_ProcessName;
            return p;
        }

        public void SetAndCheckBasePriority(ProcessPriorityClass exPriorityClass, int priority)
        {
            s_process.PriorityClass = exPriorityClass;
            s_process.Refresh();
            Assert.Equal(priority, s_process.BasePriority);
        }

        [Fact]
        public void Process_BasePriority()
        {

            //SetAndCheckBasePriority(ProcessPriorityClass.RealTime, 24);
            SetAndCheckBasePriority(ProcessPriorityClass.High, 13);
            SetAndCheckBasePriority(ProcessPriorityClass.Idle, 4);
            SetAndCheckBasePriority(ProcessPriorityClass.Normal, 8);
        }

        public void DelayTask(double delayInMs)
        {
            var t = Task.Run(async delegate
            {
                await Task.Delay(TimeSpan.FromMilliseconds(delayInMs));
            });
            t.Wait();
        }

        public void DelayTask()
        {
            DelayTask(50D);
        }

        public void StartAndKillProcessWithDelay(Process p)
        {
            p.Start();
            DelayTask();
            p.Kill();
            p.WaitForExit();
        }

        [Fact]
        public void Process_EnableRaiseEvents()
        {
            {
                // Test behavior when EnableRaisingEvent = true;
                // Ensure event is called.
                s_Process_EnableRaiseEvents_isExitedEventHandlerCalled = false;

                Process p = CreateProcessInfinite();
                p.EnableRaisingEvents = true;
                p.Exited += p_Exited;
                StartAndKillProcessWithDelay(p);
                Assert.True(s_Process_EnableRaiseEvents_isExitedEventHandlerCalled, String.Format("Process_CanRaiseEvents0001: {0}", "isExited Event not called when EnableRaisingEvent is set to true."));
            }

            {
                s_Process_EnableRaiseEvents_isExitedEventHandlerCalled = false;

                // Check with the default settings (false, events will not be raised)
                Process p = CreateProcessInfinite();
                p.Exited += p_Exited;
                StartAndKillProcessWithDelay(p);
                Assert.False(s_Process_EnableRaiseEvents_isExitedEventHandlerCalled, String.Format("Process_CanRaiseEvents0002: {0}", "isExited Event called with the default settings for EnableRaiseEvents"));
            }

            {
                s_Process_EnableRaiseEvents_isExitedEventHandlerCalled = false;

                // Same test, this time explicitly set the property to false
                Process p = CreateProcessInfinite();
                p.EnableRaisingEvents = false;
                p.Exited += p_Exited;
                StartAndKillProcessWithDelay(p);
                Assert.False(s_Process_EnableRaiseEvents_isExitedEventHandlerCalled, String.Format("Process_CanRaiseEvents0003: {0}", "isExited Event called with the EnableRaiseEvents = false"));
            }
        }

        void p_Exited(object sender, EventArgs e)
        {
            s_Process_EnableRaiseEvents_isExitedEventHandlerCalled = true;
        }

        [Fact]
        public void Process_ExitCode()
        {
            {
                Process p = CreateProcess();
                p.Start();
                DelayTask();
                Assert.Equal(p.ExitCode, 100);
            }

            {
                Process p = CreateProcessInfinite();
                StartAndKillProcessWithDelay(p);
                Assert.True(p.ExitCode < 0, String.Format("Process_ExitCode: Unexpected Exit code {0}", p.ExitCode));
            }
        }

        [Fact]
        public void Process_ExitTime()
        {
            Process p = CreateProcessInfinite();
            StartAndKillProcessWithDelay(p);
            Assert.True(p.ExitTime < DateTime.Now, "Process_ExitTime is incorrect.");
        }


        [Fact]
        public void Process_GetHandle()
        {
            Assert.Equal(s_process.Id, Interop.GetProcessId(s_process.SafeHandle));
        }

        [Fact]
        public void Process_HasExited()
        {
            {
                Process p = CreateProcess();
                p.Start();
                DelayTask();
                Assert.True(p.HasExited, "Process_HasExited001 failed");
            }

            {
                Process p = CreateProcessInfinite();
                p.Start();
                try
                {
                    Assert.False(p.HasExited, "Process_HasExited002 failed");
                }
                finally
                {
                    p.Kill();
                    p.WaitForExit();
                }

                Assert.True(p.HasExited, "Process_HasExited003 failed");
            }
        }

        [Fact]
        public void Process_MachineName()
        {
            // Checking that the MachineName returns some value.
            Assert.NotNull(s_process.MachineName);
        }

        [Fact]
        public void Process_MainModule()
        {
            // Get MainModule property from a Process object
            string moduleName = s_process.MainModule.ModuleName;
            Assert.Equal(s_ProcessName, moduleName);

            // Check that the mainModule is present in the modules list.
            bool foundMainModule = false;
            foreach (ProcessModule pModule in s_process.Modules)
            {
                if (String.Equals(moduleName, s_ProcessName, StringComparison.OrdinalIgnoreCase))
                {
                    foundMainModule = true;
                    break;
                }
            }
            Assert.True(foundMainModule, "Could not found Module " + moduleName);
        }

        [Fact]
        public void Process_MaxWorkingSet()
        {
            IntPtr min, max;
            uint flags;

            int intCurrValue = (Int32)s_process.MaxWorkingSet;
            s_process.MaxWorkingSet = (IntPtr)(intCurrValue + 1024);
            Interop.GetProcessWorkingSetSizeEx(s_process.SafeHandle, out min, out max, out flags);
            intCurrValue = (int)max;
            s_process.Refresh();
            Assert.Equal(intCurrValue, (int)s_process.MaxWorkingSet);
        }

        [Fact]
        public void Process_MinWorkingSet()
        {
            int intCurrValue = (Int32)s_process.MinWorkingSet;
            IntPtr min;
            IntPtr max;
            uint flags;

            s_process.MinWorkingSet = (IntPtr)(intCurrValue - 1024);
            Interop.GetProcessWorkingSetSizeEx(s_process.SafeHandle, out min, out max, out flags);
            intCurrValue = (int)min;
            s_process.Refresh();
            Assert.Equal(intCurrValue, (int)s_process.MinWorkingSet);
        }

        [Fact]
        public void Process_Modules()
        {
            foreach (ProcessModule pModule in s_process.Modules)
            {
                // Validated that we can get a value for each of the following.
                Assert.NotNull(pModule);
                Assert.NotNull(pModule.BaseAddress);
                Assert.NotNull(pModule.EntryPointAddress);
                Assert.NotNull(pModule.FileName);
                int memSize = pModule.ModuleMemorySize;
                Assert.NotNull(pModule.ModuleName);
            }
        }

        public void DoNothing(TimeSpan ignoreValue)
        {
            // This method does nothing.
        }

        [Fact]
        public void Process_NonpagedSystemMemorySize64()
        {
            Assert.NotEqual(0L, s_process.NonpagedSystemMemorySize64);
        }

        [Fact]
        public void Process_PagedMemorySize64()
        {
            Assert.NotEqual(0L, s_process.PagedMemorySize64);
        }

        [Fact]
        public void Process_PagedSystemMemorySize64()
        {
            Assert.NotEqual(0L, s_process.PagedSystemMemorySize64);
        }

        [Fact]
        public void Process_PeakPagedMemorySize64()
        {
            Assert.NotEqual(0L, s_process.PeakPagedMemorySize64);
        }

        [Fact]
        public void Process_PeakVirtualMemorySize64()
        {
            Assert.NotEqual(0L, s_process.PeakVirtualMemorySize64);
        }

        [Fact]
        public void Process_PeakWorkingSet64()
        {
            Assert.NotEqual(0L, s_process.PeakWorkingSet64);
        }

        [Fact]
        public void Process_PrivateMemorySize64()
        {
            Assert.NotEqual(0L, s_process.PrivateMemorySize64);
        }

        [Fact]
        public void Process_PrivilegedProcessorTime()
        {
            // There is no good way to test the actual values of these
            // w/o the user of Performance Counters or a ton of pinvokes, and so for now we simply check
            // they do not throw exception when called.
            DoNothing(s_process.UserProcessorTime);
            DoNothing(s_process.PrivilegedProcessorTime);
            DoNothing(s_process.TotalProcessorTime);
        }

        [Fact]
        public void Process_ProcessorAffinity()
        {
            IntPtr curProcessorAffinity = s_process.ProcessorAffinity;
            s_process.ProcessorAffinity = new IntPtr(0x1);
            Assert.Equal(new IntPtr(0x1), s_process.ProcessorAffinity);

            s_process.ProcessorAffinity = curProcessorAffinity;
            Assert.Equal(curProcessorAffinity, s_process.ProcessorAffinity);
        }

        [Fact]
        public void Process_PriorityBoostEnabled()
        {
            bool isPriorityBoostEnabled = s_process.PriorityBoostEnabled;

            s_process.PriorityBoostEnabled = true;
            Assert.True(s_process.PriorityBoostEnabled, "Process_PriorityBoostEnabled001 failed");

            s_process.PriorityBoostEnabled = false;
            Assert.False(s_process.PriorityBoostEnabled, "Process_PriorityBoostEnabled002 failed");

            s_process.PriorityBoostEnabled = isPriorityBoostEnabled;

        }

        public void Process_PriorityClass()
        {

            ProcessPriorityClass priorityClass = s_process.PriorityClass;

            s_process.PriorityClass = ProcessPriorityClass.High;
            Assert.Equal(s_process.PriorityClass, ProcessPriorityClass.High);

            s_process.PriorityClass = ProcessPriorityClass.Normal;
            Assert.Equal(s_process.PriorityClass, ProcessPriorityClass.Normal);

            s_process.PriorityClass = priorityClass;
        }

        [Fact]
        public void Process_InvalidPriorityClass()
        {
            Process p = new Process();
            Assert.Throws<ArgumentException>(() => { p.PriorityClass = ProcessPriorityClass.Normal | ProcessPriorityClass.Idle; });
        }

        [Fact]
        public void Process_ProcessName()
        {
            Assert.Equal(s_process.ProcessName, "ProcessTest_ConsoleApp");
        }


        [DllImport("api-ms-win-core-processthreads-l1-1-0.dll")]
        internal static extern int GetCurrentProcessId();

        [Fact]
        public void Process_GetCurrentProcess()
        {
            int currentProcessId = ProcessTest.GetCurrentProcessId();
            Assert.NotSame(0, currentProcessId);
            Assert.Equal(currentProcessId, Process.GetCurrentProcess().Id);
            Assert.Equal(Process.GetProcessById(currentProcessId).ProcessName, Process.GetCurrentProcess().ProcessName);
        }

        [Fact]
        public void Process_GetProcesses()
        {
            // Get all the processes running on the machine.
            Process currentProcess = Process.GetCurrentProcess();
            bool foundCurrentProcess = false;
            foreach (Process p in Process.GetProcesses())
            {
                if ((p.Id == currentProcess.Id) && (p.ProcessName.Equals(currentProcess.ProcessName)))
                    foundCurrentProcess = true;
            }
            Assert.True(foundCurrentProcess, "Process_GetProcesses001 failed");

            foundCurrentProcess = false;
            string machineName = currentProcess.MachineName;
            foreach (Process p in Process.GetProcesses(machineName))
            {
                if ((p.Id == currentProcess.Id) && (p.ProcessName.Equals(currentProcess.ProcessName)))
                    foundCurrentProcess = true;
            }
            Assert.True(foundCurrentProcess, "Process_GetProcesses002 failed");
        }

        [Fact]
        public void Process_GetProcessesByName()
        {
            // Get the current process using its name
            Process currentProcess = Process.GetCurrentProcess();
            string processName = currentProcess.ProcessName;
            string machineName = currentProcess.MachineName;
            bool found = false;
            foreach (Process p in Process.GetProcessesByName(processName))
            {
                found = true;
                break;
            }
            Assert.True(found, "Process_GetProcessesByName001 failed");

            found = false;
            foreach (Process p in Process.GetProcessesByName(processName, machineName))
            {
                found = true;
                break;
            }
            Assert.True(found, "Process_GetProcessesByName002 failed");
        }

        [Fact]
        public void Process_Environment()
        {
            ProcessStartInfo psi = new ProcessStartInfo();

            // Creating a detached ProcessStartInfo will pre-populate the environment
            // with current environmental variables. 

            // When used with an existing Process.ProcessStartInfo the following behavior
            //  * Desktop - Populates with current EnvironmentVariable
            //  * Project K - Does NOT pre-populate environment.

            var Environment2 = psi.Environment;

            Assert.NotEqual(Environment2.Count, 0);

            int CountItems = Environment2.Count;

            Environment2.Add("NewKey", "NewValue");
            Environment2.Add("NewKey2", "NewValue2");

            Assert.Equal(CountItems + 2, Environment2.Count);
            Environment2.Remove("NewKey");
            Assert.Equal(CountItems + 1, Environment2.Count);

            //Exception not thrown with invalid key
            Assert.Throws<ArgumentException>(() => { Environment2.Add("NewKey2", "NewValue2"); });

            //Clear
            Environment2.Clear();
            Assert.Equal(0, Environment2.Count);

            //ContainsKey 
            Environment2.Add("NewKey", "NewValue");
            Environment2.Add("NewKey2", "NewValue2");
            Assert.True(Environment2.ContainsKey("NewKey"));
            Assert.False(Environment2.ContainsKey("NewKey99"));

            //Iterating
            string result = null;
            int index = 0;
            foreach (string e1 in Environment2.Values)
            {
                index++;
                result += e1;
            }
            Assert.Equal(2, index);
            Assert.Equal("NewValueNewValue2", result);

            result = null;
            index = 0;
            foreach (string e1 in Environment2.Keys)
            {
                index++;
                result += e1;
            }
            Assert.Equal("NewKeyNewKey2", result);
            Assert.Equal(2, index);

            result = null;
            index = 0;
            foreach (System.Collections.Generic.KeyValuePair<string, string> e1 in Environment2)
            {
                index++;
                result += e1.Key;
            }
            Assert.Equal("NewKeyNewKey2", result);
            Assert.Equal(2, index);

            //Contains
            Assert.True(Environment2.Contains(new System.Collections.Generic.KeyValuePair<string, string>("NewKey", "NewValue")));
            Assert.False(Environment2.Contains(new System.Collections.Generic.KeyValuePair<string, string>("NewKey99", "NewValue99")));

            //Exception not thrown with invalid key
            Assert.Throws<ArgumentNullException>(() =>
            {
                Environment2.Contains(new System.Collections.Generic.KeyValuePair<string, string>(null, "NewValue99"));
            }
            );

            Environment2.Add(new System.Collections.Generic.KeyValuePair<string, string>("NewKey98", "NewValue98"));

            //Indexed
            string newIndexItem = Environment2["NewKey98"];
            Assert.Equal("NewValue98", newIndexItem);

            //TryGetValue
            string stringout = null;
            bool retval = false;
            retval = Environment2.TryGetValue("NewKey", out stringout);
            Assert.Equal("NewValue", stringout);
            Assert.True(retval);

            stringout = null;
            retval = false;
            retval = Environment2.TryGetValue("NewKey99", out stringout);
            Assert.Equal(null, stringout);
            Assert.False(retval);

            //Exception not thrown with invalid key
            Assert.Throws<ArgumentNullException>(() =>
            {
                string stringout1 = null;
                bool retval1 = false;
                retval1 = Environment2.TryGetValue(null, out stringout1);
            }
            );

            //Exception not thrown with invalid key
            Assert.Throws<ArgumentNullException>(() =>
            {
                Environment2.Add(null, "NewValue2");
            }
            );

            //Invalid Key to add
            Assert.Throws<ArgumentException>(() =>
            {
                Environment2.Add("NewKey2", "NewValue2");
            }
            );
            //Remove Item
            Environment2.Remove("NewKey98");
            Environment2.Remove("NewKey98");   //2nd occurrence should not assert

            //Exception not thrown with null key
            Assert.Throws<ArgumentNullException>(() => { Environment2.Remove(null); });

            //"Exception not thrown with null key"
            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() => { string a1 = Environment2["1bB"]; });

            Assert.True(Environment2.Contains(new System.Collections.Generic.KeyValuePair<string, string>("NewKey2", "NewValue2")));
            Assert.False(Environment2.Contains(new System.Collections.Generic.KeyValuePair<string, string>("newkey2", "NewValue2")));
            Assert.False(Environment2.Contains(new System.Collections.Generic.KeyValuePair<string, string>("NewKey2", "newvalue2")));
            Assert.False(Environment2.Contains(new System.Collections.Generic.KeyValuePair<string, string>("newkey2", "newvalue2")));

            //Use KeyValuePair Enumerator
            var x = Environment2.GetEnumerator();
            x.MoveNext();
            var y1 = x.Current;
            Assert.Equal("NewKey NewValue", y1.Key + " " + y1.Value);
            x.MoveNext();
            y1 = x.Current;
            Assert.Equal("NewKey2 NewValue2", y1.Key + " " + y1.Value);

            //IsReadonly
            Assert.False(Environment2.IsReadOnly);

            Environment2.Add(new System.Collections.Generic.KeyValuePair<string, string>("NewKey3", "NewValue3"));
            Environment2.Add(new System.Collections.Generic.KeyValuePair<string, string>("NewKey4", "NewValue4"));


            //CopyTo
            System.Collections.Generic.KeyValuePair<String, String>[] kvpa = new System.Collections.Generic.KeyValuePair<string, string>[10];
            Environment2.CopyTo(kvpa, 0);
            Assert.Equal("NewKey", kvpa[0].Key);
            Assert.Equal("NewKey3", kvpa[2].Key);

            Environment2.CopyTo(kvpa, 6);
            Assert.Equal("NewKey", kvpa[6].Key);

            //Exception not thrown with null key
            Assert.Throws<System.ArgumentOutOfRangeException>(() => { Environment2.CopyTo(kvpa, -1); });

            //Exception not thrown with null key
            Assert.Throws<System.ArgumentException>(() => { Environment2.CopyTo(kvpa, 9); });

            //Exception not thrown with null key
            Assert.Throws<System.ArgumentNullException>(() =>
            {
                System.Collections.Generic.KeyValuePair<String, String>[] kvpanull = null;
                Environment2.CopyTo(kvpanull, 0);
            }
            );
        }

        public void Dispose()
        {
            s_process.Kill();
            s_process.WaitForExit();

            // Also ensure that there are no open processes with the same name as s_ProcessName
            foreach (Process p in Process.GetProcessesByName(s_ProcessName))
            {
                if (!p.HasExited)
                {
                    p.Kill();
                    p.WaitForExit();
                }
            }
        }
    }
}