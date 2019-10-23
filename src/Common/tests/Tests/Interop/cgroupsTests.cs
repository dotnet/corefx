// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using Xunit;

namespace Common.Tests
{
    public class cgroupsTests
    {
        [Theory]
        [InlineData(true, "0",  0)]
        [InlineData(false, "max",  0)]
        [InlineData(true, "1k",  1024)]
        [InlineData(true, "1K",  1024)]
        public static void ValidateTryReadMemoryValue(bool expectedResult, string valueText, ulong expectedValue)
        {
            string path = Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, valueText);

                bool result = Interop.cgroups.TryReadMemoryValueFromFile(path, out ulong val);

                Assert.Equal(expectedResult, result);
                if (result)
                {
                    Assert.Equal(expectedValue, val);
                }
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Theory]
        [InlineData(false, "0 0 0:0 / /foo ignore ignore - overlay overlay ignore", "ignore", 0, "/", "/")]
        [InlineData(true, "0 0 0:0 / /foo ignore ignore - cgroup2 cgroup2 ignore", "ignore", 2, "/", "/foo")]
        [InlineData(true, "0 0 0:0 / /foo ignore ignore - cgroup2 cgroup2 ignore", "memory", 2, "/", "/foo")]
        [InlineData(true, "0 0 0:0 / /foo ignore ignore - cgroup2 cgroup2 ignore", "cpu", 2, "/", "/foo")]
        [InlineData(true, "0 0 0:0 / /foo ignore - cgroup2 cgroup2 ignore", "cpu", 2, "/", "/foo")]
        [InlineData(true, "0 0 0:0 / /foo ignore ignore ignore - cgroup2 cgroup2 ignore", "cpu", 2, "/", "/foo")]
        [InlineData(true, "0 0 0:0 / /foo-with-dashes ignore ignore - cgroup2 cgroup2 ignore", "ignore", 2, "/", "/foo-with-dashes")]
        [InlineData(true, "0 0 0:0 / /foo ignore ignore - cgroup cgroup memory", "memory", 1, "/", "/foo")]
        [InlineData(true, "0 0 0:0 / /foo-with-dashes ignore ignore - cgroup cgroup memory", "memory", 1, "/", "/foo-with-dashes")]
        [InlineData(true, "0 0 0:0 / /foo ignore ignore - cgroup cgroup cpu,memory", "memory", 1, "/", "/foo")]
        [InlineData(true, "0 0 0:0 / /foo ignore ignore - cgroup cgroup memory,cpu", "memory", 1, "/", "/foo")]
        [InlineData(false, "0 0 0:0 / /foo ignore ignore - cgroup cgroup cpu", "memory", 0, "/", "/foo")]
        public static void ParseValidateMountInfo(bool found, string procSelfMountInfoText, string subsystem, int expectedVersion, string expectedRoot, string expectedMount)
        {
            string path = Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, procSelfMountInfoText);

                bool result = Interop.cgroups.TryFindHierarchyMount(path, subsystem,  out Interop.cgroups.CGroupVersion version, out string root, out string mount);

                Assert.Equal(found, result);
                if (found)
                {
                    Assert.Equal(expectedVersion, (int)version);
                    Assert.Equal(expectedRoot, root);
                    Assert.Equal(expectedMount, mount);
                }
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Theory]
        [InlineData(true, "0::/foo", "ignore", "/foo")]
        [InlineData(true, "0::/bar", "ignore", "/bar")]
        [InlineData(true, "0::frob", "ignore", "frob")]
        [InlineData(false, "1::frob", "ignore", "ignore")]
        [InlineData(true, "1:foo:bar", "foo", "bar")]
        [InlineData(true, "2:foo:bar", "foo", "bar")]
        [InlineData(false, "2:foo:bar", "bar", "ignore")]
        [InlineData(true, "1:foo:bar\n2:eggs:spam", "foo", "bar")]
        [InlineData(true, "1:foo:bar\n2:eggs:spam", "eggs", "spam")]
        public static void ParseValidateProcCGroup(bool found, string procSelfCgroupText, string subsystem, string expectedMountPath)
        {
            string path = Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, procSelfCgroupText);

                bool result = Interop.cgroups.TryFindCGroupPathForSubsystem(path, subsystem,  out string mountPath);

                Assert.Equal(found, result);
                if (found)
                {
                    Assert.Equal(expectedMountPath, mountPath);
                }
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}
