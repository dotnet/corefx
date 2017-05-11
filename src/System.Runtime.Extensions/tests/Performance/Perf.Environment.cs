// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Tests
{
    public class Perf_Environment
    {
        [Benchmark]
        public void GetEnvironmentVariable()
        {
            PerfUtils utils = new PerfUtils();
            string env = utils.CreateString(15);
            try
            {
                // setup the environment variable so we can read it
                Environment.SetEnvironmentVariable(env, "value");

                // read the valid environment variable for the test
                foreach (var iteration in Benchmark.Iterations)
                    using (iteration.StartMeasurement())
                        for (int i = 0; i < 40000; i++)
                        {
                            Environment.GetEnvironmentVariable(env); Environment.GetEnvironmentVariable(env); Environment.GetEnvironmentVariable(env);
                            Environment.GetEnvironmentVariable(env); Environment.GetEnvironmentVariable(env); Environment.GetEnvironmentVariable(env);
                            Environment.GetEnvironmentVariable(env); Environment.GetEnvironmentVariable(env); Environment.GetEnvironmentVariable(env);
                        }
            }
            finally
            {
                // clear the variable that we set
                Environment.SetEnvironmentVariable(env, null);
            }
        }

        [Benchmark]
        public void ExpandEnvironmentVariables()
        {
            PerfUtils utils = new PerfUtils();
            string env = utils.CreateString(15);
            string inputEnv = "%" + env + "%";
            try
            {
                // setup the environment variable so we can read it
                Environment.SetEnvironmentVariable(env, "value");

                // read the valid environment variable
                foreach (var iteration in Benchmark.Iterations)
                    using (iteration.StartMeasurement())
                        for (int i = 0; i < 40000; i++)
                        {
                            Environment.ExpandEnvironmentVariables(inputEnv); Environment.ExpandEnvironmentVariables(inputEnv);
                            Environment.ExpandEnvironmentVariables(inputEnv); Environment.ExpandEnvironmentVariables(inputEnv);
                            Environment.ExpandEnvironmentVariables(inputEnv); Environment.ExpandEnvironmentVariables(inputEnv);
                            Environment.ExpandEnvironmentVariables(inputEnv); Environment.ExpandEnvironmentVariables(inputEnv);
                            Environment.ExpandEnvironmentVariables(inputEnv); Environment.ExpandEnvironmentVariables(inputEnv);
                        }
            }
            finally
            {
                // clear the variable that we set
                Environment.SetEnvironmentVariable(env, null);
            }
        }

        [Benchmark]
        public void GetEnvironmentVariables()
        {
            PerfUtils utils = new PerfUtils();
            string env = utils.CreateString(15);
            try
            {
                // setup the environment variable so we can read it
                Environment.SetEnvironmentVariable(env, "value");

                // read the valid environment variable for the test
                foreach (var iteration in Benchmark.Iterations)
                    using (iteration.StartMeasurement())
                        for (int i = 0; i < 2000; i++)
                        {
                            Environment.GetEnvironmentVariables(); Environment.GetEnvironmentVariables(); Environment.GetEnvironmentVariables();
                            Environment.GetEnvironmentVariables(); Environment.GetEnvironmentVariables(); Environment.GetEnvironmentVariables();
                            Environment.GetEnvironmentVariables(); Environment.GetEnvironmentVariables(); Environment.GetEnvironmentVariables();
                        }
            }
            finally
            {
                // clear the variable that we set
                Environment.SetEnvironmentVariable(env, null);
            }
        }

        [Benchmark]
        [PlatformSpecific(TestPlatforms.OSX)]
        [InlineData(Environment.SpecialFolder.System, Environment.SpecialFolderOption.None)]
        public void GetFolderPath_OSX(Environment.SpecialFolder folder, Environment.SpecialFolderOption option)
        {
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < 20000; i++)
                    {
                        Environment.GetFolderPath(folder, option); Environment.GetFolderPath(folder, option); Environment.GetFolderPath(folder, option);
                        Environment.GetFolderPath(folder, option); Environment.GetFolderPath(folder, option); Environment.GetFolderPath(folder, option);
                        Environment.GetFolderPath(folder, option); Environment.GetFolderPath(folder, option); Environment.GetFolderPath(folder, option);
                    }
        }

        [Benchmark]
        [PlatformSpecific(TestPlatforms.AnyUnix)]
        [InlineData(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.DoNotVerify)]
        public void GetFolderPath_Unix(Environment.SpecialFolder folder, Environment.SpecialFolderOption option)
        {
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < 20000; i++)
                    {
                        Environment.GetFolderPath(folder, option); Environment.GetFolderPath(folder, option); Environment.GetFolderPath(folder, option);
                        Environment.GetFolderPath(folder, option); Environment.GetFolderPath(folder, option); Environment.GetFolderPath(folder, option);
                        Environment.GetFolderPath(folder, option); Environment.GetFolderPath(folder, option); Environment.GetFolderPath(folder, option);
                    }
        }

        [Benchmark]
        [PlatformSpecific(TestPlatforms.Windows)]
        [InlineData(Environment.SpecialFolder.Windows, Environment.SpecialFolderOption.None)]
        public void GetFolderPath_Windows(Environment.SpecialFolder folder, Environment.SpecialFolderOption option)
        {
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < 20000; i++)
                    {
                        Environment.GetFolderPath(folder, option); Environment.GetFolderPath(folder, option); Environment.GetFolderPath(folder, option);
                        Environment.GetFolderPath(folder, option); Environment.GetFolderPath(folder, option); Environment.GetFolderPath(folder, option);
                        Environment.GetFolderPath(folder, option); Environment.GetFolderPath(folder, option); Environment.GetFolderPath(folder, option);
                    }
        }

        [Benchmark]
        [PlatformSpecific(TestPlatforms.AnyUnix)]
        public void GetLogicalDrives_Unix()
        {
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < 40000; i++)
                    {
                        Environment.GetLogicalDrives(); Environment.GetLogicalDrives(); Environment.GetLogicalDrives();
                        Environment.GetLogicalDrives(); Environment.GetLogicalDrives(); Environment.GetLogicalDrives();
                        Environment.GetLogicalDrives(); Environment.GetLogicalDrives(); Environment.GetLogicalDrives();
                    }
        }

        [Benchmark]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void GetLogicalDrives_Windows()
        {
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < 40000; i++)
                    {
                        Environment.GetLogicalDrives(); Environment.GetLogicalDrives(); Environment.GetLogicalDrives();
                        Environment.GetLogicalDrives(); Environment.GetLogicalDrives(); Environment.GetLogicalDrives();
                        Environment.GetLogicalDrives(); Environment.GetLogicalDrives(); Environment.GetLogicalDrives();
                    }
        }
    }
}
