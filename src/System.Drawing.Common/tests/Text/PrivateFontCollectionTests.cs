﻿// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing.Tests;
using System.IO;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Drawing.Text.Tests
{
    public class PrivateFontCollectionTests
    {
        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void Ctor_Default()
        {
            using (var fontCollection = new PrivateFontCollection())
            {
                Assert.Empty(fontCollection.Families);
            }
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void AddFontFile_FontFile_Success()
        {
            // GDI+ on Windows 7 incorrectly throws a FileNotFoundException.
            if (PlatformDetection.IsWindows7)
            {
                return;
            }

            using (var fontCollection = new PrivateFontCollection())
            {
                fontCollection.AddFontFile(Helpers.GetTestBitmapPath("empty.file"));
                fontCollection.AddFontFile(Helpers.GetTestFontPath("CodeNewRoman.otf"));

                FontFamily font = Assert.Single(fontCollection.Families);
                Assert.Equal("Code New Roman", font.Name);
            }
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [ActiveIssue(21558, TargetFrameworkMonikers.Netcoreapp)]
        public void AddFontFile_NullFileName_ThrowsArgumentNullException()
        {
            using (var fontCollection = new PrivateFontCollection())
            {
                AssertExtensions.Throws<ArgumentNullException>("path", () => fontCollection.AddFontFile(null));
            }
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [ActiveIssue(21558, TargetFrameworkMonikers.Netcoreapp)]
        public void AddFontFile_InvalidPath_ThrowsArgumentException()
        {
            using (var fontCollection = new PrivateFontCollection())
            {
                AssertExtensions.Throws<ArgumentException>(null, () => fontCollection.AddFontFile(string.Empty));
            }
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [ActiveIssue(21558, TargetFrameworkMonikers.Netcoreapp)]
        public void AddFontFile_NoSuchFilePath_ThrowsArgumentException()
        {
            using (var fontCollection = new PrivateFontCollection())
            {
                Assert.Throws<FileNotFoundException>(() => fontCollection.AddFontFile("fileName"));
            }
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [ActiveIssue(21558, TargetFrameworkMonikers.Netcoreapp)]
        public void AddFontFile_LongFilePath_ThrowsPathTooLongException()
        {
            using (var fontCollection = new PrivateFontCollection())
            {
                Assert.Throws<PathTooLongException>(() => fontCollection.AddFontFile(new string('a', 261)));
            }
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void AddFontFile_Directory_ThrowsExternalException()
        {
            // GDI+ on Windows 7 and Windows 8.1 incorrectly does not throw.
            if (PlatformDetection.IsWindows || PlatformDetection.IsWindows8x)
            {
                return;
            }

            using (var fontCollection = new PrivateFontCollection())
            {
                Assert.Throws<ExternalException>(() => fontCollection.AddFontFile(AppContext.BaseDirectory));
            }
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void AddFontFile_Disposed_ThrowsArgumentException()
        {
            var fontCollection = new PrivateFontCollection();
            fontCollection.Dispose();

            AssertExtensions.Throws<ArgumentException>(null, () => fontCollection.AddFontFile("fileName"));
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void AddMemoryFont_ValidMemory_Success()
        {
            using (var fontCollection = new PrivateFontCollection())
            {
                byte[] data = File.ReadAllBytes(Helpers.GetTestFontPath("CodeNewRoman.otf"));

                IntPtr fontBuffer = Marshal.AllocCoTaskMem(data.Length);
                try
                {
                    Marshal.Copy(data, 0, fontBuffer, data.Length);
                    fontCollection.AddMemoryFont(fontBuffer, data.Length);

                    FontFamily font = Assert.Single(fontCollection.Families);
                    Assert.Equal("Code New Roman", font.Name);
                }
                finally
                {
                    Marshal.FreeCoTaskMem(fontBuffer);
                }
            }
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void AddMemoryFont_ZeroMemory_ThrowsArgumentException()
        {
            using (var fontCollection = new PrivateFontCollection())
            {
                AssertExtensions.Throws<ArgumentException>(null, () => fontCollection.AddMemoryFont(IntPtr.Zero, 100));
            }
        }

        [ConditionalTheory(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [InlineData(0)]
        [InlineData(-1)]
        public void AddMemoryFont_InvalidLength_ThrowsArgumentException(int length)
        {
            // GDI+ on Windows 7 incorrectly throws a FileNotFoundException.
            if (PlatformDetection.IsWindows)
            {
                return;
            }

            using (var fontCollection = new PrivateFontCollection())
            {
                byte[] data = File.ReadAllBytes(Helpers.GetTestFontPath("CodeNewRoman.otf"));

                IntPtr fontBuffer = Marshal.AllocCoTaskMem(data.Length);
                try
                {
                    Marshal.Copy(data, 0, fontBuffer, data.Length);
                    AssertExtensions.Throws<ArgumentException>(null, () => fontCollection.AddMemoryFont(fontBuffer, length));
                }
                finally
                {
                    Marshal.FreeCoTaskMem(fontBuffer);
                }
            }
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void AddMemoryFont_Disposed_ThrowsArgumentException()
        {
            var fontCollection = new PrivateFontCollection();
            fontCollection.Dispose();

            AssertExtensions.Throws<ArgumentException>(null, () => fontCollection.AddMemoryFont((IntPtr)10, 100));
        }
        
        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void Families_GetWhenDisposed_ThrowsArgumentException()
        {
            var fontCollection = new PrivateFontCollection();
            fontCollection.Dispose();

            AssertExtensions.Throws<ArgumentException>(null, () => fontCollection.Families);
        }

        [ConditionalFact(nameof(PlatformDetection) + "." + nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void Dispose_MultipleTimes_Nop()
        {
            var fontCollection = new PrivateFontCollection();
            fontCollection.Dispose();
            fontCollection.Dispose();
        }
    }
}
