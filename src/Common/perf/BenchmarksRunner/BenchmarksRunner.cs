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
            BenchmarkSwitcher
                .FromAssemblies(
                    GetTestAssemblies().Select(Assembly.LoadFrom).ToArray())
                .Run(
                    args,
                    DefaultConfig.Instance.With(Job.ShortRun.With(new CoreFxToolchain("netcoreapp"))));

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Benchmark execution failed.");
            Console.WriteLine($"  {ex.ToString()}");
            return 1;
        }
    }

    private static string GetTestAssembly(string testName)
    {
        // Assume test assemblies are colocated/restored next to the BenchmarksRunner.
        return Path.Combine(
            Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), testName);
    }

    private static IEnumerable<string> GetTestAssemblies()
    {
        return Directory.EnumerateFiles(".", "*.Performance.Tests.dll");
    }
}
