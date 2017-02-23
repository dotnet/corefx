using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xunit.Performance.Api;

public class PerfHarness
{
    public static void Main(string[] args)
    {
        using (XunitPerformanceHarness harness = new XunitPerformanceHarness(args))
        {
            foreach(var testName in GetTestAssemblies())
            {
                harness.RunBenchmarks(GetTestAssembly(testName));
            }
        }
    }

    private static string GetTestAssembly(string testName)
    {
        // Assume test assemblies are colocated/restored next to the PerfHarness.
        return Path.Combine(
            Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), testName);
    }

    private static IEnumerable<string> GetTestAssemblies()
    {
        return Directory.EnumerateFiles(".", "*.Performance.Tests.dll");
    }
}