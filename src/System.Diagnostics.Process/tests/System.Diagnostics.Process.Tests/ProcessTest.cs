// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Xunit;

namespace Test_System_Diagnostics_Process
{
    public partial class ProcessTest
    {
        private const string s_ProcessName = "ProcessTest_ConsoleApp.exe";

        static Process CreateProcessInfinite()
        {
            // Create a process that does not exit and check the base priority of the Process.
            Process p = new Process();
            p.StartInfo.FileName = s_ProcessName;
            p.StartInfo.Arguments = "infinite";
            return p;
        }

        static Process CreateProcessError()
        {
            // Create a process that does not exit and check the base priority of the Process.
            Process p = new Process();
            p.StartInfo.FileName = s_ProcessName;
            p.StartInfo.Arguments = "error";
            return p;
        }

        static Process CreateProcessInput()
        {
            // Create a process that does not exit and check the base priority of the Process.
            Process p = new Process();
            p.StartInfo.FileName = s_ProcessName;
            p.StartInfo.Arguments = "input";
            return p;
        }

        static Process CreateProcess()
        {
            // Create a process that does not exit and check the base priority of the Process.
            Process p = new Process();
            p.StartInfo.FileName = s_ProcessName;
            return p;
        }

        [Fact]
        public static void Process_BasePriority()
        {
            Process p = CreateProcessInfinite();
            p.Start();
            Assert.True(p.BasePriority == 8 /*Normal*/, String.Format("Err:Process_BasePriority. Incorrect Process.BasePriority of {0}", p.BasePriority));
            p.Kill();
            p.WaitForExit();
        }

        private static bool s_Process_EnableRaiseEvents_isExitedEventHandlerCalled = false;

        [Fact]
        public static void Process_EnableRaiseEvents()
        {
            // EnableRaisingEvent = true;
            // Check whether the event is called.
            Process p = CreateProcessInfinite();
            p.EnableRaisingEvents = true;
            p.Exited += p_Exited;
            p.Start();
            p.Kill();
            p.WaitForExit();
            Assert.True(s_Process_EnableRaiseEvents_isExitedEventHandlerCalled, String.Format("Process_CanRaiseEvents0001: {0}", "isExited Event not called when EnableRaisingEvent is set to true."));

            s_Process_EnableRaiseEvents_isExitedEventHandlerCalled = false;

            // Check with the default settings which is set this to false.
            p.Refresh();
            p = CreateProcessInfinite();
            p.Exited += p_Exited;
            p.Start();
            p.Kill();
            p.WaitForExit();
            Assert.True(!s_Process_EnableRaiseEvents_isExitedEventHandlerCalled, String.Format("Process_CanRaiseEvents0002: {0}", "isExited Event called with the default settings for EnableRaiseEvents"));

            s_Process_EnableRaiseEvents_isExitedEventHandlerCalled = false;

            // Check with the default settings which is set this to false.
            p = CreateProcessInfinite();
            p.EnableRaisingEvents = false;
            p.Exited += p_Exited;
            p.Start();
            p.Kill();
            p.WaitForExit();
            Assert.True(!s_Process_EnableRaiseEvents_isExitedEventHandlerCalled, String.Format("Process_CanRaiseEvents0003: {0}", "isExited Event called with the EnableRaiseEvents = false"));

            s_Process_EnableRaiseEvents_isExitedEventHandlerCalled = false;
        }

        static void p_Exited(object sender, EventArgs e)
        {
            s_Process_EnableRaiseEvents_isExitedEventHandlerCalled = true;
        }

        [Fact]
        public static void Process_ExitCode()
        {
            Process p = CreateProcess();

            p.Start();
            p.WaitForExit();
            Assert.True(p.ExitCode == 100, String.Format("Process_ExitCode001:Unexpected Exit code {0}", p.ExitCode));

            p = CreateProcessInfinite();
            p.Start();
            p.Kill();
            p.WaitForExit();
            Assert.True(p.ExitCode < 0, String.Format("Process_ExitCode002:Unexpected Exit code {0}", p.ExitCode));
        }

        [Fact]
        public static void Process_ExitTime()
        {
            Process p = CreateProcessInfinite();
            p.Start();
            p.Kill();
            p.WaitForExit();
            DateTime exitTime = p.ExitTime;
            DateTime dt2 = DateTime.Now;
            TimeSpan elapsedTime = new TimeSpan(dt2.Ticks - exitTime.Ticks);
            Assert.True(elapsedTime.Seconds <= 1, "Process_ExitTime001 is incorrect.");
        }

        [DllImport("api-ms-win-core-processthreads-l1-1-0.dll")]
        static extern int GetProcessId(SafeProcessHandle nativeHandle);

        [Fact]
        public static void Process_GetHandle()
        {
            Process p = CreateProcessInfinite();
            p.Start();
            Assert.True(p.Id == GetProcessId(p.SafeHandle), String.Format("Process_GetHandle returned incorrect value Expected {0} Recieved {1}", p.Id, GetProcessId(p.SafeHandle)));
            p.Kill();
            p.WaitForExit();
        }

        [Fact]
        public static void Process_HasExited()
        {
            Process p = CreateProcess();
            p.Start();
            p.WaitForExit();
            Assert.True(p.HasExited, "Process_HasExited001 failed");

            p = CreateProcessInfinite();
            p.Start();
            Assert.True(!p.HasExited, "Process_HasExited002 failed");
            p.Kill();
            p.WaitForExit();
            Assert.True(p.HasExited, "Process_HasExited003 failed");
        }

        [Fact]
        public static void Process_MachineName()
        {
            // Checking that the MachineName returns some value.
            Process p = CreateProcessInfinite();
            p.Start();
            try
            {
                Console.WriteLine(p.MachineName);
            }
            catch (Exception)
            {
                Assert.True(false, "Process_MachineName failed");
            }
            finally
            {
                p.Kill();
                p.WaitForExit();
            }
        }

        [Fact]
        public static void Process_MainModule()
        {
            // Checking that the MachineName returns some value.
            Process p = CreateProcessInfinite();
            p.Start();
            try
            {
                ProcessModule pMainModule = p.MainModule;

                // Check that the mainModule is present in the modules list.
                bool foundMainModule = false;
                foreach (ProcessModule pModule in p.Modules)
                {
                    if ((pModule.BaseAddress == pMainModule.BaseAddress) && pModule.FileName.Equals(pMainModule.FileName))
                    {
                        foundMainModule = true;
                        break;
                    }
                }

                Assert.True(foundMainModule, "Process_MainModule set to incorrect module");
                Assert.True("ProcessTest_ConsoleApp.exe" == pMainModule.ModuleName, "MainModule.ModuleName failed");
                Assert.True(pMainModule.FileName.Contains(pMainModule.ModuleName), "MainModule.FileName failed");
            }
            catch (Exception)
            {
                Assert.True(false, "Process_MainModule failed");
            }
            finally
            {
                p.Kill();
                p.WaitForExit();
            }
        }

        [Fact]
        public static void Process_MaxWorkingSet()
        {
            // Checking that the MachineName returns some value.
            Process p = CreateProcessInfinite();
            p.Start();
            try
            {
                Assert.True(p.MaxWorkingSet != null, "Process_MaxWorkingSet can't be null");
            }
            catch (Exception)
            {
                Assert.True(false, "Process_MaxWorkingSet failed");
            }
            finally
            {
                p.Kill();
                p.WaitForExit();
            }
        }

        [Fact]
        public static void Process_MinWorkingSet()
        {
            // Checking that the MachineName returns some value.
            Process p = CreateProcessInfinite();
            p.Start();
            try
            {
                Assert.True(p.MinWorkingSet != null, "Process_MinWorkingSet can't be null");
            }
            catch (Exception)
            {
                Assert.True(false, "Process_MinWorkingSet failed");
            }
            finally
            {
                p.Kill();
                p.WaitForExit();
            }
        }

        [Fact]
        public static void Process_Modules()
        {
            // Checking that the MachineName returns some value.
            Process p = CreateProcessInfinite();
            p.Start();
            try
            {
                foreach (ProcessModule pModule in p.Modules)
                {
                    // Validated that we can get a value for each of the following.
                    Assert.True(pModule != null, "Process_Modules pModule can't be null");
                    Assert.True(pModule.BaseAddress != null, "Process_Modules BaseAddress can't be null");
                    Assert.True(pModule.EntryPointAddress != null, "Process_Modules EntryPointAddress can't be null");
                    Assert.True(pModule.FileName != null, "Process_Modules FileName can't be null");
                    int memSize = pModule.ModuleMemorySize;
                    Assert.True(pModule.ModuleName != null, "Process_Modules ModuleName can't be null");
                }
            }
            catch (Exception)
            {
                Assert.True(false, "Process_MaxWorkingSet failed");
            }
            finally
            {
                p.Kill();
                p.WaitForExit();
            }
        }

        [DllImport("psapi.dll", SetLastError = true)]
        static extern bool GetProcessMemoryInfo(IntPtr hProcess, out PROCESS_MEMORY_COUNTERS counters, uint size);


        [StructLayout(LayoutKind.Sequential, Size = 40)]
        private struct PROCESS_MEMORY_COUNTERS
        {
            public uint cb;
            public uint PageFaultCount;
            public uint PeakWorkingSetSize;
            public uint WorkingSetSize;
            public uint QuotaPeakPagedPoolUsage;
            public uint QuotaPagedPoolUsage;
            public uint QuotaPeakNonPagedPoolUsage;
            public uint QuotaNonPagedPoolUsage;
            public uint PagefileUsage;
            public uint PeakPagefileUsage;
        }

        [DllImport("api-ms-win-core-memory-l1-1-1.dll")]
        static extern bool GetProcessWorkingSetSizeEx(SafeProcessHandle hProcess,
   out IntPtr lpMinimumWorkingSetSize, out IntPtr lpMaximumWorkingSetSize, out uint flags);

        [Fact]
        public static void Process_WorkingSet()
        {
            Process p = CreateProcessInfinite();
            p.Start();
            IntPtr minWorkingSet, maxWorkingset;
            uint flags;
            GetProcessWorkingSetSizeEx(p.SafeHandle, out minWorkingSet, out maxWorkingset, out flags);
            Assert.True(p.MinWorkingSet == minWorkingSet, "Process_WorkingSet001 failed");
            Assert.True(p.MaxWorkingSet == maxWorkingset, "Process_WorkingSet002 failed");
            p.Kill();
            p.WaitForExit();
        }

        [DllImport("api-ms-win-core-processthreads-l1-1-0", CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        static extern int GetPriorityClass(SafeProcessHandle handle);

        [Fact]
        public static void Process_NonpagedSystemMemorySize64()
        {
            // Checking that the MachineName returns some value.
            Process p = CreateProcessInfinite();
            p.Start();
            try
            {
                long x = p.NonpagedSystemMemorySize64;
            }
            catch (Exception)
            {
                Assert.True(false, "Process_NonpagedSystemMemorySize64 failed");
            }
            finally
            {
                p.Kill();
                p.WaitForExit();
            }
        }

        [Fact]
        public static void Process_PagedMemorySize64()
        {
            // Checking that the MachineName returns some value.
            Process p = CreateProcessInfinite();
            p.Start();
            try
            {
                long x = p.PagedMemorySize64;
            }
            catch (Exception)
            {
                Assert.True(false, "Process_PagedMemorySize64 failed");
            }
            finally
            {
                p.Kill();
                p.WaitForExit();
            }
        }

        [Fact]
        public static void Process_PagedSystemMemorySize64()
        {
            // Checking that the MachineName returns some value.
            Process p = CreateProcessInfinite();
            p.Start();
            try
            {
                long x = p.PagedSystemMemorySize64;
            }
            catch (Exception)
            {
                Assert.True(false, "Process_PagedSystemMemorySize64 failed");
            }
            finally
            {
                p.Kill();
                p.WaitForExit();
            }
        }

        [Fact]
        public static void Process_PeakPagedMemorySize64()
        {
            // Checking that the MachineName returns some value.
            Process p = CreateProcessInfinite();
            p.Start();
            try
            {
                long x = p.PeakPagedMemorySize64;
            }
            catch (Exception)
            {
                Assert.True(false, "Process_PeakPagedMemorySize64 failed");
            }
            finally
            {
                p.Kill();
                p.WaitForExit();
            }
        }

        [Fact]
        public static void Process_PeakVirtualMemorySize64()
        {
            // Checking that the MachineName returns some value.
            Process p = CreateProcessInfinite();
            p.Start();
            try
            {
                long x = p.PeakVirtualMemorySize64;
            }
            catch (Exception)
            {
                Assert.True(false, "Process_PeakVirtualMemorySize64 failed");
            }
            finally
            {
                p.Kill();
                p.WaitForExit();
            }
        }

        [Fact]
        public static void Process_PeakWorkingSet64()
        {
            // Checking that the MachineName returns some value.
            Process p = CreateProcessInfinite();
            p.Start();
            try
            {
                long x = p.PeakWorkingSet64;
            }
            catch (Exception)
            {
                Assert.True(false, "Process_PeakWorkingSet64 failed");
            }
            finally
            {
                p.Kill();
                p.WaitForExit();
            }
        }

        [Fact]
        public static void Process_PrivateMemorySize64()
        {
            // Checking that the MachineName returns some value.
            Process p = CreateProcessInfinite();
            p.Start();
            try
            {
                long x = p.PrivateMemorySize64;
            }
            catch (Exception)
            {
                Assert.True(false, "Process_PeakWorkingSet64 failed");
            }
            finally
            {
                p.Kill();
                p.WaitForExit();
            }
        }

        [Fact]
        public static void Process_PrivilegedProcessorTime()
        {
            // Checking that the MachineName returns some value.
            Process p = CreateProcessInfinite();
            p.Start();
            try
            {
                TimeSpan x = p.PrivilegedProcessorTime;
            }
            catch (Exception)
            {
                Assert.True(false, "Process_PeakWorkingSet64 failed");
            }
            finally
            {
                p.Kill();
                p.WaitForExit();
            }
        }

        [Fact]
        public static void Process_ProcessorAffinity()
        {
            // Checking that the MachineName returns some value.
            Process p = CreateProcessInfinite();
            p.Start();
            try
            {
                IntPtr x = p.ProcessorAffinity;
            }
            catch (Exception)
            {
                Assert.True(false, "Process_PeakWorkingSet64 failed");
            }
            finally
            {
                p.Kill();
                p.WaitForExit();
            }
        }

        [Fact]
        public static void Process_SessionId()
        {
            // Checking that the MachineName returns some value.
            Process p = CreateProcessInfinite();
            p.Start();
            try
            {
                int x = p.SessionId;
            }
            catch (Exception ex)
            {
                Assert.True(false, String.Format("Process_PeakWorkingSet64 failed. {0}", ex.ToString()));
            }
            finally
            {
                p.Kill();
                p.WaitForExit();
            }
        }

        [Fact]
        public static void Process_PriorityBoostEnabled()
        {
            // Checking that the MachineName returns some value.
            Process p = CreateProcessInfinite();
            p.Start();
            Assert.True(p.PriorityBoostEnabled, "Process_PriorityBoostEnabled001 failed");
            p.PriorityBoostEnabled = false;
            Assert.True(!p.PriorityBoostEnabled, "Process_PriorityBoostEnabled002 failed");
            p.Kill();
            p.WaitForExit();
        }

        [Fact]
        public static void Process_PriorityClass()
        {
            // Checking that the MachineName returns some value.
            Process p = CreateProcessInfinite();
            p.Start();
            Assert.True(p.PriorityClass == ProcessPriorityClass.Normal, "Process_PriorityBoostEnabled001 failed");
            p.PriorityClass = ProcessPriorityClass.High;
            Assert.True(p.PriorityClass == ProcessPriorityClass.High, "Process_PriorityBoostEnabled002 failed");
            p.Kill();
            p.WaitForExit();
        }

        [Fact]
        public static void Process_InvalidPriorityClass()
        {
            Process p = new Process();
            Assert.Throws<ArgumentException>(() => { p.PriorityClass = ProcessPriorityClass.Normal | ProcessPriorityClass.Idle; });
        }

        [Fact]
        public static void Process_ProcessName()
        {
            // Checking that the MachineName returns some value.
            Process p = CreateProcessInfinite();
            p.Start();
            Assert.True(p.ProcessName == "ProcessTest_ConsoleApp", "Process_ProcessName failed");
            p.Kill();
            p.WaitForExit();
        }


        [DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        public static extern SafeProcessHandle GetCurrentProcess();

        [Fact]
        public static void Process_GetCurrentProcess()
        {
            string processName = Process.GetCurrentProcess().ProcessName;
            Assert.True(processName == "CoreRun", String.Format("Process_GetCurrentProcess() failed. Actual process name found is {0}", processName));
        }

        [Fact]
        public static void Process_GetProcessById()
        {
            Process p = Process.GetCurrentProcess();
            Process p_copy = Process.GetProcessById(p.Id);
            Assert.True(p.ProcessName == p_copy.ProcessName, "Process_GetProcessById001 failed");
            Assert.True(p.Id == p_copy.Id, "Process_GetProcessById002 failed");
        }

        [Fact]
        public static void Process_GetProcesses()
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
        public static void Process_GetProcessesByName()
        {
            // Get all the processes running on the machine.
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
        public static void Process_Environment()
        {
            ProcessStartInfo psi = new ProcessStartInfo();

            // Creating a detached ProcessStartInfo will pre-populate the environment
            // with current environmental variables. 

            // When used with an existing Process.ProcessStartInfo the following behavior
            //  * Desktop - Populates with current EnvironmentVariable
            //  * Project K - Does NOT pre-populate environment.

            var Environment2 = psi.Environment;

            Assert.True(Environment2.Count != 0);

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
            Environment2.Remove("NewKey98");   //2nd occurence should not assert

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
            Assert.True("NewKey NewValue" == y1.Key + " " + y1.Value, "Did not return expected value");
            x.MoveNext();
            y1 = x.Current;
            Assert.True("NewKey2 NewValue2" == y1.Key + " " + y1.Value, "Did not return expected value");

            //IsReadonly
            Assert.False(Environment2.IsReadOnly);

            Environment2.Add(new System.Collections.Generic.KeyValuePair<string, string>("NewKey3", "NewValue3"));
            Environment2.Add(new System.Collections.Generic.KeyValuePair<string, string>("NewKey4", "NewValue4"));


            //CopyTo
            System.Collections.Generic.KeyValuePair<String, String>[] kvpa = new System.Collections.Generic.KeyValuePair<string, string>[10];
            Environment2.CopyTo(kvpa, 0);
            Assert.True("NewKey" == kvpa[0].Key, "Did not return expected value");
            Assert.True("NewKey3" == kvpa[2].Key, "Did not return expected value");

            Environment2.CopyTo(kvpa, 6);
            Assert.True("NewKey" == kvpa[6].Key, "Did not return expected value");

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
    }
}