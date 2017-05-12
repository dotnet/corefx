// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using Microsoft.Xunit.Performance;
using Xunit;

namespace System.IO.Tests
{
    public class Perf_Path
    {
        [Benchmark(InnerIterationCount = 150000)]
        public void Combine()
        {
            PerfUtils utils = new PerfUtils();
            string testPath1 = utils.GetTestFilePath();
            string testPath2 = utils.CreateString(10);

            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Path.Combine(testPath1, testPath2); Path.Combine(testPath1, testPath2); Path.Combine(testPath1, testPath2);
                        Path.Combine(testPath1, testPath2); Path.Combine(testPath1, testPath2); Path.Combine(testPath1, testPath2);
                        Path.Combine(testPath1, testPath2); Path.Combine(testPath1, testPath2); Path.Combine(testPath1, testPath2);
                    }
        }

        [Benchmark(InnerIterationCount = 100000)]
        public void GetFileName()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.GetTestFilePath();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Path.GetFileName(testPath); Path.GetFileName(testPath); Path.GetFileName(testPath);
                        Path.GetFileName(testPath); Path.GetFileName(testPath); Path.GetFileName(testPath);
                        Path.GetFileName(testPath); Path.GetFileName(testPath); Path.GetFileName(testPath);
                    }
        }

        [Benchmark(InnerIterationCount = 80000)]
        public void GetDirectoryName()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.GetTestFilePath();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Path.GetDirectoryName(testPath); Path.GetDirectoryName(testPath); Path.GetDirectoryName(testPath);
                        Path.GetDirectoryName(testPath); Path.GetDirectoryName(testPath); Path.GetDirectoryName(testPath);
                        Path.GetDirectoryName(testPath); Path.GetDirectoryName(testPath); Path.GetDirectoryName(testPath);
                    }
        }

        [Benchmark(InnerIterationCount = 120000)]
        public void ChangeExtension()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.GetTestFilePath();
            string extension = utils.CreateString(4);

            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Path.ChangeExtension(testPath, extension); Path.ChangeExtension(testPath, extension); Path.ChangeExtension(testPath, extension);
                        Path.ChangeExtension(testPath, extension); Path.ChangeExtension(testPath, extension); Path.ChangeExtension(testPath, extension);
                        Path.ChangeExtension(testPath, extension); Path.ChangeExtension(testPath, extension); Path.ChangeExtension(testPath, extension);
                    }
        }

        [Benchmark(InnerIterationCount = 150000)]
        public void GetExtension()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.GetTestFilePath();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Path.GetExtension(testPath); Path.GetExtension(testPath); Path.GetExtension(testPath);
                        Path.GetExtension(testPath); Path.GetExtension(testPath); Path.GetExtension(testPath);
                        Path.GetExtension(testPath); Path.GetExtension(testPath); Path.GetExtension(testPath);
                    }
        }

        [Benchmark(InnerIterationCount = 100000)]
        public void GetFileNameWithoutExtension()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.GetTestFilePath();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Path.GetFileNameWithoutExtension(testPath); Path.GetFileNameWithoutExtension(testPath); Path.GetFileNameWithoutExtension(testPath);
                        Path.GetFileNameWithoutExtension(testPath); Path.GetFileNameWithoutExtension(testPath); Path.GetFileNameWithoutExtension(testPath);
                        Path.GetFileNameWithoutExtension(testPath); Path.GetFileNameWithoutExtension(testPath); Path.GetFileNameWithoutExtension(testPath);
                    }
        }

        [Benchmark(InnerIterationCount = 20000)]
        public void GetFullPathForLegacyLength()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.CreateString(length: 200);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Path.GetFullPath(testPath); Path.GetFullPath(testPath); Path.GetFullPath(testPath);
                        Path.GetFullPath(testPath); Path.GetFullPath(testPath); Path.GetFullPath(testPath);
                        Path.GetFullPath(testPath); Path.GetFullPath(testPath); Path.GetFullPath(testPath);
                    }
        }

        [Benchmark(InnerIterationCount = 10000)]
        public void GetFullPathForTypicalLongPath()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.CreateString(length: 500);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Path.GetFullPath(testPath); Path.GetFullPath(testPath); Path.GetFullPath(testPath);
                        Path.GetFullPath(testPath); Path.GetFullPath(testPath); Path.GetFullPath(testPath);
                        Path.GetFullPath(testPath); Path.GetFullPath(testPath); Path.GetFullPath(testPath);
                    }
        }

        [Benchmark(InnerIterationCount = 5000)]
        public void GetFullPathForReallyLongPath()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.CreateString(length: 1000);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Path.GetFullPath(testPath); Path.GetFullPath(testPath); Path.GetFullPath(testPath);
                        Path.GetFullPath(testPath); Path.GetFullPath(testPath); Path.GetFullPath(testPath);
                        Path.GetFullPath(testPath); Path.GetFullPath(testPath); Path.GetFullPath(testPath);
                    }
        }

        [Benchmark(InnerIterationCount = 100000)]
        public void GetPathRoot()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.GetTestFilePath();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Path.GetPathRoot(testPath); Path.GetPathRoot(testPath); Path.GetPathRoot(testPath);
                        Path.GetPathRoot(testPath); Path.GetPathRoot(testPath); Path.GetPathRoot(testPath);
                        Path.GetPathRoot(testPath); Path.GetPathRoot(testPath); Path.GetPathRoot(testPath);
                    }
        }

        [Benchmark(InnerIterationCount = 150000)]
        public void GetRandomFileName()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.GetTestFilePath();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Path.GetRandomFileName(); Path.GetRandomFileName(); Path.GetRandomFileName();
                        Path.GetRandomFileName(); Path.GetRandomFileName(); Path.GetRandomFileName();
                        Path.GetRandomFileName(); Path.GetRandomFileName(); Path.GetRandomFileName();
                    }
        }

        [Benchmark(InnerIterationCount = 20000)]
        public void GetTempPath()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.GetTestFilePath();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Path.GetTempPath(); Path.GetTempPath(); Path.GetTempPath();
                        Path.GetTempPath(); Path.GetTempPath(); Path.GetTempPath();
                        Path.GetTempPath(); Path.GetTempPath(); Path.GetTempPath();
                    }
        }

        [Benchmark(InnerIterationCount = 200000)]
        public void HasExtension()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.GetTestFilePath();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Path.HasExtension(testPath); Path.HasExtension(testPath); Path.HasExtension(testPath);
                        Path.HasExtension(testPath); Path.HasExtension(testPath); Path.HasExtension(testPath);
                        Path.HasExtension(testPath); Path.HasExtension(testPath); Path.HasExtension(testPath);
                    }
        }

        [Benchmark(InnerIterationCount = 250000)]
        public void IsPathRooted()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.GetTestFilePath();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Path.IsPathRooted(testPath); Path.IsPathRooted(testPath); Path.IsPathRooted(testPath);
                        Path.IsPathRooted(testPath); Path.IsPathRooted(testPath); Path.IsPathRooted(testPath);
                        Path.IsPathRooted(testPath); Path.IsPathRooted(testPath); Path.IsPathRooted(testPath);
                    }
        }
    }
}
