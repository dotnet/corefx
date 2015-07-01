// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using Xunit;

public class Directory_GetActualPath
{
    [Theory]
    [PlatformSpecific(PlatformID.Windows)]
    [InlineData(TestInfo.CurrentDirectory)]
    [InlineData(@"\\localhost\" + TestInfo.CurrentDirectory.Replace(':', '$'))]
    public static void TrueCasingTest_Truthy(string path)
    {
        Assert.Equal(path, Directory.GetActualCasing(path.ToUpper()));
    }

    [Theory]
    [PlatformSpecific(PlatformID.Windows)]
    [InlineData(Environment.SystemDirectory)]
    [InlineData(Environment.SystemDirectory.ToUpper())]
    public static void TrueCasingTest_Falsy(string path)
    {
        Assert.NotEqual(path, Directory.GetActualCasing(path));
    }

    [Theory]
    [PlatformSpecific(PlatformID.Windows)]
    [InlineData(@"\\rand0m-server\devnul")]
    [InlineData(Environment.SystemDirectory + @"\KERNEL32.DlL")]
    [InlineData(Environment.SystemDirectory + @"\RANDOM-NON_EXISTING-Slug\")]
    public static void TrueCasingTest_Throwy(string path)
    {
        Assert.Throws(typeof(DirectoryNotFoundException), Directory.GetActualCasing(path));
    }
}
