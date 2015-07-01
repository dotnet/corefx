// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using Xunit;

public class File_GetActualPath
{
    [Fact]
    [PlatformSpecific(PlatformID.Windows)]
    public static void TrueCasingTest_Truthy()
    {
        string unicodedFileName = Path.Combine(TestInfo.CurrentDirectory, "тратата.text.txt");
        
        File.Create(unicodedFileName).Dispose();

        Assert.Equal(unicodedFileName, File.GetActualCasing(unicodedFileName.ToUpper()));

        File.Delete(unicodedFileName);

        string path = @"\\localhost\" + Environment.SystemDirectory.Replace(':', '$') + @"\kernel32.dll";

        Assert.Equal(path, File.GetActualCasing(path.ToUpper()));
    }

    [Theory]
    [PlatformSpecific(PlatformID.Windows)]
    [InlineData(Environment.SystemDirectory + @"\KERNEL32.DlL")]
    [InlineData(Environment.SystemDirectory.ToUpper() + @"\kernel32.dll")]
    public static void TrueCasingTest_Falsy(string path)
    {
        Assert.NotEqual(path, File.GetActualCasing(path));
    }

    [Theory]
    [PlatformSpecific(PlatformID.Windows)]
    [InlineData(@"\\rand0m-server\devnul\blah.txt")]
    [InlineData(Environment.SystemDirectory + @"\RANDOM-NON_EXISTING-Slug\KERNEL32_INVALID.DlL")]
    public static void TrueCasingTest_Throwy(string path)
    {
        Assert.Throws(typeof(FileNotFoundException), File.GetActualCasing(path));
    }
}
