// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using BenchmarkDotNet.Attributes;

namespace System.ConsoleTests
{
    /// <summary>
    /// Perf tests for Console are chosen based on which functions have PAL code. They are:
    /// 
    /// - OpenStandardInput, OpenStandardOutput, OpenStandardError
    /// - ForegroundColor, BackgroundColor, ResetColor
    /// </summary>
    [GcForce(true)] // forces full GC cleanup after every iteration, so streams allocated in OpenStandard* benchmarks are going to be finalized
    [KeepBenchmarkFiles]
    public class Perf_Console
    {
        [Benchmark]
        public Stream OpenStandardInput() => Console.OpenStandardInput();

        [Benchmark]
        public Stream OpenStandardOutput() => Console.OpenStandardOutput();

        [Benchmark]
        public Stream OpenStandardError() => Console.OpenStandardError();

        [Benchmark(OperationsPerInvoke = 8)]
        public void ForegroundColor()
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.White;
        }

        [GlobalCleanup(Target = nameof(ForegroundColor))]
        public void ForegroundColorCleanup() => Console.ResetColor();

        [Benchmark(OperationsPerInvoke = 8)]
        public void BackgroundColor()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.BackgroundColor = ConsoleColor.White;
        }

        [GlobalCleanup(Target = nameof(BackgroundColor))]
        public void BackgroundColorCleanup() => Console.ResetColor();

        [Benchmark]
        public void ResetColor()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.ResetColor();
        }
    }
}