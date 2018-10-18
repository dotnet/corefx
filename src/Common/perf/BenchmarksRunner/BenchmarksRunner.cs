// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Horology;
using BenchmarkDotNet.Running;

public class BenchmarksRunner
{
    public static int Main(string[] args)
    {
        try
        {
            BenchmarkSwitcher
                .FromAssemblies(GetTestAssemblies().Select(Assembly.LoadFrom).ToArray())
                .Run(args, GetConfig());

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Benchmark execution failed.");
            Console.WriteLine($"  {ex.ToString()}");
            return 1;
        }
    }

    private static IEnumerable<string> GetTestAssemblies()
    {
        return Directory.EnumerateFiles(".", "*.Performance.Tests.dll");
    }
    
    private static IConfig GetConfig()
        => DefaultConfig.Instance
            .With(Job.Default
                .WithWarmupCount(1) // 1 warmup is enough for our purpose
                .WithIterationTime(TimeInterval.FromMilliseconds(250)) // the default is 0.5s per iteration, which is slighlty too much for us
                .WithMaxIterationCount(15) 
                .WithMaxIterationCount(20) // we don't want to run more that 20 iterations
                .With(new CoreFxToolchain("netcoreapp")) // CoreFxToolchain is responsible for building the benchmarks in CoreFX way
                .AsDefault()) // tell BDN that this are our default settings
            .With(MemoryDiagnoser.Default); // MemoryDiagnoser is enabled by default
}
