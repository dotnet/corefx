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
        [Benchmark]
        public void Combine()
        {
            PerfUtils utils = new PerfUtils();
            string testPath1 = utils.GetTestFilePath();
            string testPath2 = utils.CreateString(10);

            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < 150000; i++)
                    {
                        Path.Combine(testPath1, testPath2); Path.Combine(testPath1, testPath2); Path.Combine(testPath1, testPath2);
                        Path.Combine(testPath1, testPath2); Path.Combine(testPath1, testPath2); Path.Combine(testPath1, testPath2);
                        Path.Combine(testPath1, testPath2); Path.Combine(testPath1, testPath2); Path.Combine(testPath1, testPath2);
                    }
        }

        [Benchmark]
        public void GetFileName()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.GetTestFilePath();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < 100000; i++)
                    {
                        Path.GetFileName(testPath); Path.GetFileName(testPath); Path.GetFileName(testPath);
                        Path.GetFileName(testPath); Path.GetFileName(testPath); Path.GetFileName(testPath);
                        Path.GetFileName(testPath); Path.GetFileName(testPath); Path.GetFileName(testPath);
                    }
        }

        [Benchmark]
        public void GetDirectoryName()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.GetTestFilePath();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < 80000; i++)
                    {
                        Path.GetDirectoryName(testPath); Path.GetDirectoryName(testPath); Path.GetDirectoryName(testPath);
                        Path.GetDirectoryName(testPath); Path.GetDirectoryName(testPath); Path.GetDirectoryName(testPath);
                        Path.GetDirectoryName(testPath); Path.GetDirectoryName(testPath); Path.GetDirectoryName(testPath);
                    }
        }

        [Benchmark]
        public void ChangeExtension()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.GetTestFilePath();
            string extension = utils.CreateString(4);

            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < 120000; i++)
                    {
                        Path.ChangeExtension(testPath, extension); Path.ChangeExtension(testPath, extension); Path.ChangeExtension(testPath, extension);
                        Path.ChangeExtension(testPath, extension); Path.ChangeExtension(testPath, extension); Path.ChangeExtension(testPath, extension);
                        Path.ChangeExtension(testPath, extension); Path.ChangeExtension(testPath, extension); Path.ChangeExtension(testPath, extension);
                    }
        }

        [Benchmark]
        public void GetExtension()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.GetTestFilePath();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < 150000; i++)
                    {
                        Path.GetExtension(testPath); Path.GetExtension(testPath); Path.GetExtension(testPath);
                        Path.GetExtension(testPath); Path.GetExtension(testPath); Path.GetExtension(testPath);
                        Path.GetExtension(testPath); Path.GetExtension(testPath); Path.GetExtension(testPath);
                    }
        }

        [Benchmark]
        public void GetFileNameWithoutExtension()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.GetTestFilePath();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < 100000; i++)
                    {
                        Path.GetFileNameWithoutExtension(testPath); Path.GetFileNameWithoutExtension(testPath); Path.GetFileNameWithoutExtension(testPath);
                        Path.GetFileNameWithoutExtension(testPath); Path.GetFileNameWithoutExtension(testPath); Path.GetFileNameWithoutExtension(testPath);
                        Path.GetFileNameWithoutExtension(testPath); Path.GetFileNameWithoutExtension(testPath); Path.GetFileNameWithoutExtension(testPath);
                    }
        }

        [Benchmark]
        public void GetFullPathForLegacyLength()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.CreateString(length: 200);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < 20000; i++)
                    {
                        Path.GetFullPath(testPath); Path.GetFullPath(testPath); Path.GetFullPath(testPath);
                        Path.GetFullPath(testPath); Path.GetFullPath(testPath); Path.GetFullPath(testPath);
                        Path.GetFullPath(testPath); Path.GetFullPath(testPath); Path.GetFullPath(testPath);
                    }
        }

        [Benchmark]
        public void GetFullPathForTypicalLongPath()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.CreateString(length: 500);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < 10000; i++)
                    {
                        Path.GetFullPath(testPath); Path.GetFullPath(testPath); Path.GetFullPath(testPath);
                        Path.GetFullPath(testPath); Path.GetFullPath(testPath); Path.GetFullPath(testPath);
                        Path.GetFullPath(testPath); Path.GetFullPath(testPath); Path.GetFullPath(testPath);
                    }
        }

        [Benchmark]
        public void GetFullPathForReallyLongPath()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.CreateString(length: 1000);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < 5000; i++)
                    {
                        Path.GetFullPath(testPath); Path.GetFullPath(testPath); Path.GetFullPath(testPath);
                        Path.GetFullPath(testPath); Path.GetFullPath(testPath); Path.GetFullPath(testPath);
                        Path.GetFullPath(testPath); Path.GetFullPath(testPath); Path.GetFullPath(testPath);
                    }
        }

        [Benchmark]
        public void GetPathRoot()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.GetTestFilePath();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < 100000; i++)
                    {
                        Path.GetPathRoot(testPath); Path.GetPathRoot(testPath); Path.GetPathRoot(testPath);
                        Path.GetPathRoot(testPath); Path.GetPathRoot(testPath); Path.GetPathRoot(testPath);
                        Path.GetPathRoot(testPath); Path.GetPathRoot(testPath); Path.GetPathRoot(testPath);
                    }
        }

        [Benchmark]
        public void GetRandomFileName()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.GetTestFilePath();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < 150000; i++)
                    {
                        Path.GetRandomFileName(); Path.GetRandomFileName(); Path.GetRandomFileName();
                        Path.GetRandomFileName(); Path.GetRandomFileName(); Path.GetRandomFileName();
                        Path.GetRandomFileName(); Path.GetRandomFileName(); Path.GetRandomFileName();
                    }
        }

        [Benchmark]
        public void GetTempPath()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.GetTestFilePath();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < 20000; i++)
                    {
                        Path.GetTempPath(); Path.GetTempPath(); Path.GetTempPath();
                        Path.GetTempPath(); Path.GetTempPath(); Path.GetTempPath();
                        Path.GetTempPath(); Path.GetTempPath(); Path.GetTempPath();
                    }
        }

        [Benchmark]
        public void HasExtension()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.GetTestFilePath();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < 200000; i++)
                    {
                        Path.HasExtension(testPath); Path.HasExtension(testPath); Path.HasExtension(testPath);
                        Path.HasExtension(testPath); Path.HasExtension(testPath); Path.HasExtension(testPath);
                        Path.HasExtension(testPath); Path.HasExtension(testPath); Path.HasExtension(testPath);
                    }
        }

        [Benchmark]
        public void IsPathRooted()
        {
            PerfUtils utils = new PerfUtils();
            string testPath = utils.GetTestFilePath();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < 250000; i++)
                    {
                        Path.IsPathRooted(testPath); Path.IsPathRooted(testPath); Path.IsPathRooted(testPath);
                        Path.IsPathRooted(testPath); Path.IsPathRooted(testPath); Path.IsPathRooted(testPath);
                        Path.IsPathRooted(testPath); Path.IsPathRooted(testPath); Path.IsPathRooted(testPath);
                    }
        }
    }
}
