// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

public class BenchmarksRunner
{
    public static int Main(string[] args)
    {
        try
        {
            var config = DefaultConfig.Instance
                .With(Job.Default.With(new CoreFxToolchain("netcoreapp"))); // CoreFxToolchain is responsible for building the benchmarks in CoreFX way

            BenchmarkSwitcher
                .FromAssemblies(GetTestAssemblies().Select(Assembly.LoadFrom).ToArray())
                .Run(args, config);

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
}
