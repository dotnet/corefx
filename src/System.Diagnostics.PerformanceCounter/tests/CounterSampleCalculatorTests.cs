// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Specialized;
using Xunit;

namespace System.Diagnostics.Tests
{
    [SkipOnTargetFramework(TargetFrameworkMonikers.Uap)] // In appcontainer, cannot write to perf counters
    public static class CounterSampleCalculatorTests
    {
        [ConditionalFact(typeof(Helpers), nameof(Helpers.IsElevatedAndCanWriteToPerfCounters))]
        public static void CounterSampleCalculator_ElapsedTime()
        {
            var name = nameof(CounterSampleCalculator_ElapsedTime) + "_Counter";

            PerformanceCounter counterSample = CreateCounter(name, PerformanceCounterType.ElapsedTime);

            counterSample.RawValue = Stopwatch.GetTimestamp();
            DateTime Start = DateTime.Now;
            counterSample.NextValue();

            System.Threading.Thread.Sleep(500);

            var counterVal = counterSample.NextValue();
            var dateTimeVal = DateTime.Now.Subtract(Start).TotalSeconds;
            DeleteCategory(name);
            Assert.True(Math.Abs(dateTimeVal - counterVal) < .05);
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.IsElevatedAndCanWriteToPerfCounters))]
        public static void CounterSampleCalculator_CounterTimer()
        {
            var name = nameof(CounterSampleCalculator_CounterTimer) + "_Counter";

            PerformanceCounter counterSample = CreateCounter(name, PerformanceCounterType.CounterTimer);

            counterSample.RawValue = 0;
            counterSample.NextValue();

            var val = RunScenario(counterSample);

            DeleteCategory(name);

            Assert.True(val > 0);
        }

        public static PerformanceCounter CreateCounter(string name, PerformanceCounterType counterType)
        {
            var category = name + "_Category";
            var instance = name + "_Instance";

            CounterCreationDataCollection ccdc = new CounterCreationDataCollection();
            CounterCreationData ccd = new CounterCreationData();
            ccd.CounterType = counterType;
            ccd.CounterName = name;
            ccdc.Add(ccd);

            DeleteCategory(name);
            PerformanceCounterCategory.Create(category, "description", PerformanceCounterCategoryType.SingleInstance, ccdc);

            Assert.True(Helpers.PerformanceCounterCategoryCreated(category));

            return new PerformanceCounter(category, name, false);
        }

        public static float RunScenario(PerformanceCounter counter)
        {
            Random r = new Random(-55);
            for (int i = 0; i < 50; i++)
            {
                int randomValue = r.Next(1, 100000);
                counter.IncrementBy(randomValue);
                System.Threading.Thread.Sleep(50);
            }

            return counter.NextValue();
        }

        public static void DeleteCategory(string name)
        {
            var category = name + "_Category";
            if (PerformanceCounterCategory.Exists(category))
            {
                PerformanceCounterCategory.Delete(category);
            }
        }
    }
}
