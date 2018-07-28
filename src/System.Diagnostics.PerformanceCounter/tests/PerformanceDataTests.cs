// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.PerformanceData;
using System.IO;
using Xunit;
using Xunit.NetCore.Extensions;

namespace System.Diagnostics.Tests
{
    [SkipOnTargetFramework(TargetFrameworkMonikers.Uap)] // In appcontainer, cannot write to perf counters
    public static class PerformanceDataTests
    {
        private static Guid providerId = new Guid("{51D1685C-35ED-45be-99FE-17261A4F27F3}");
        private static Guid typingCounterSetId = new Guid("{582803C9-AACD-45e5-8C30-571141A22092}");

        private static CounterSet typingCounterSet;         // Defines the counter set
        private static CounterSetInstance typingCsInstance; // Instance of the counter set

        /// <summary>
        /// We use lodctr.exe to register performance counter
        /// </summary>        
        static string RegisterCounters()
        {
            string manifestPathTmp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName(), "provider.man");
            Directory.CreateDirectory(Path.GetDirectoryName(manifestPathTmp));
            File.Copy("provider.man", manifestPathTmp);
            File.Copy("System.Diagnostics.PerformanceCounter.Tests.dll", Path.Combine(Path.GetDirectoryName(manifestPathTmp), "System.Diagnostics.PerformanceCounter.Tests.dll"));
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "lodctr";
            psi.Arguments = "/m:\"" + manifestPathTmp + "\"";
            psi.UseShellExecute = false;
            Process process = Process.Start(psi);
            process.WaitForExit();
            Assert.Equal(0, process.ExitCode);
            return manifestPathTmp;
        }

        /// <summary>
        /// We use unlodctr.exe to unregister performance counter
        /// </summary>
        /// <param name="manifestPathTmp"></param>
        static void UnregisterCounters(string manifestPathTmp)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "unlodctr";
            psi.Arguments = "/m:\"" + manifestPathTmp + "\"";
            psi.UseShellExecute = false;
            Process process = Process.Start(psi);
            process.WaitForExit();
            Assert.Equal(0, process.ExitCode);
        }

        /// <summary>
        /// This test was taken from System.Diagnostics.PerformanceData documentation https://msdn.microsoft.com/en-us/library/system.diagnostics.performancedata(v=vs.110).aspx
        /// To create provider.res file we've used some tools:
        /// ctrpp.exe -legacy provider.man
        /// rc.exe /r /i "c:\Program Files\Microsoft SDKs\Windows\v6.0\Include" provider.rc
        /// </summary>
        [ConditionalFact(typeof(Helpers), nameof(Helpers.IsElevatedAndCanWriteToPerfCounters))]
        [Trait(XunitConstants.Category, XunitConstants.RequiresElevation)]
        [Trait("MyTrait", "MyTrait")]
        public static void PerformanceCounter_PerformanceData()
        {
            string schemaPath = RegisterCounters();
            try
            {
                // Create the 'Typing' counter set.
                typingCounterSet = new CounterSet(providerId, typingCounterSetId, CounterSetInstanceType.Single);

                // Add the counters to the counter set definition.
                typingCounterSet.AddCounter(1, CounterType.RawData32, "Total Words Typed");
                typingCounterSet.AddCounter(2, CounterType.Delta32, "Words Typed In Interval");
                typingCounterSet.AddCounter(3, CounterType.RawData32, "Letter A Pressed");
                typingCounterSet.AddCounter(4, CounterType.RawData32, "Words Containing A");
                typingCounterSet.AddCounter(5, CounterType.SampleFraction, "Percent of Words Containing A");
                typingCounterSet.AddCounter(6, CounterType.SampleBase, "Percent Base");

                // Create an instance of the counter set (contains the counter data).
                typingCsInstance = typingCounterSet.CreateCounterSetInstance("Typing Instance");
                typingCsInstance.Counters[1].Value = 0;
                typingCsInstance.Counters[2].Value = 0;
                typingCsInstance.Counters[3].Value = 0;
                typingCsInstance.Counters[4].Value = 0;
                typingCsInstance.Counters[5].Value = 0;
                typingCsInstance.Counters[6].Value = 0;
                
                //Instance counters reader
                PerformanceCounter totalWordsTyped = new PerformanceCounter("Typing", "Total Words Typed");
                PerformanceCounter wordsTypedInInterval = new PerformanceCounter("Typing", "Words Typed In Interval");
                PerformanceCounter aKeyPressed = new PerformanceCounter("Typing", "Letter A Pressed");
                PerformanceCounter wordsContainingA = new PerformanceCounter("Typing", "Words Containing A");
                PerformanceCounter percentofWordsContaingA = new PerformanceCounter("Typing", "Percent of Words Containing A");                

            }
            finally
            {
                UnregisterCounters(schemaPath);
                Directory.Delete(Path.GetDirectoryName(schemaPath), true);
            }
        }
    }
}
