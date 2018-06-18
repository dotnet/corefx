﻿// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// Authors:
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using Xunit;

namespace System.Drawing.Printing.Tests
{
    public class PrinterSettingsTests
    {
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Ctor_Default_Success()
        {
            var printerSettings = new PrinterSettings();
            Assert.NotNull(printerSettings.DefaultPageSettings);
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.AnyInstalledPrinters, Helpers.GdiplusIsAvailable)]
        public void CanDuplex_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();
            bool canDuplex = printerSettings.CanDuplex;
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Copies_Default_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();
            int copies = printerSettings.Copies;
        }

        [ConditionalTheory(Helpers.GdiplusIsAvailable)]
        [InlineData(0)]
        [InlineData(short.MaxValue)]
        public void Copies_SetValue_ReturnsExpected(short copies)
        {
            var printerSettings = new PrinterSettings()
            {
                Copies = copies
            };

            Assert.Equal(copies, printerSettings.Copies);
        }

        [ConditionalTheory(Helpers.GdiplusIsAvailable)]
        [InlineData(-1)]
        [InlineData(short.MinValue)]
        public void Copies_SetValue_ThrowsArgumentException(short copies)
        {
            var printerSettings = new PrinterSettings();
            AssertExtensions.Throws<ArgumentException>(null, () => printerSettings.Copies = copies);
        }

        [ConditionalFact(Helpers.AnyInstalledPrinters, Helpers.GdiplusIsAvailable)]
        public void Collate_Default_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();
            bool collate = printerSettings.Collate;
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void Collate_SetValue_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings()
            {
                Collate = false
            };

            Assert.Equal(false, printerSettings.Collate);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void DefaultPageSettings_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();
            Assert.NotNull(printerSettings.DefaultPageSettings);
        }

        [ConditionalTheory(Helpers.GdiplusIsAvailable)]
        [InlineData(Duplex.Simplex)]
        [InlineData(Duplex.Vertical)]
        [InlineData(Duplex.Horizontal)]
        public void Duplex_SetValue_ReturnsExpected(Duplex duplex)
        {
            var printerSettings = new PrinterSettings()
            {
                Duplex = duplex
            };

            Assert.Equal(duplex, printerSettings.Duplex);
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalTheory(Helpers.GdiplusIsAvailable)]
        [InlineData(Duplex.Default - 1)]
        [InlineData(Duplex.Horizontal + 1)]
        [InlineData((Duplex)int.MaxValue)]
        [InlineData((Duplex)int.MinValue)]
        public void Duplex_Invalid_ThrowsInvalidEnumArgumentException(Duplex duplex)
        {
            var printerSettings = new PrinterSettings();
            Assert.ThrowsAny<ArgumentException>(() => printerSettings.Duplex = duplex);
        }

        [Fact]
        public void FromPage_Default_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();

            Assert.Equal(0, printerSettings.FromPage);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public void FromPage_SetValue_ReturnsExpected(int pageNumber)
        {
            var printerSettings = new PrinterSettings()
            {
                FromPage = pageNumber
            };

            Assert.Equal(pageNumber, printerSettings.FromPage);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void FromPage_Invalid_ThrowsArgumentException(int pageNumber)
        {
            var printerSettings = new PrinterSettings();
            AssertExtensions.Throws<ArgumentException>(null, () => printerSettings.FromPage = pageNumber);
        }

        [ConditionalFact(Helpers.AnyInstalledPrinters, Helpers.GdiplusIsAvailable)]
        public void Static_InstalledPrinters_ReturnsExpected()
        {
            Assert.NotNull(PrinterSettings.InstalledPrinters);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IsDefaultPrinter_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();
            Assert.True(printerSettings.IsDefaultPrinter);
        }

        [ConditionalFact(Helpers.AnyInstalledPrinters, Helpers.GdiplusIsAvailable, Helpers.WindowsRS3OrEarlier)] // RS4 failures: https://github.com/dotnet/corefx/issues/29858
        public void IsPlotter_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();
            Assert.False(printerSettings.IsPlotter);
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        public void IsValid_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings()
            {
                PrinterName = "Invalid Printer"
            };

            Assert.False(printerSettings.IsValid);
        }

        [ConditionalFact(Helpers.AnyInstalledPrinters, Helpers.GdiplusIsAvailable)]
        public void LandscapeAngle_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();
            int[] validValues = new[] { 0, 90, 270 };
            Assert.True(validValues.Contains(printerSettings.LandscapeAngle), "PrinterSettings.LandscapeAngle must be 0, 90, or 270 degrees.");
        }

        [ConditionalFact(Helpers.AnyInstalledPrinters, Helpers.GdiplusIsAvailable)]
        public void MaximumCopies_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();
            Assert.True(printerSettings.MaximumCopies >= 0, "PrinterSettings.MaximumCopies should not be negative.");
        }

        [Fact]
        public void MaximumPage_Default_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();

            Assert.Equal(9999, printerSettings.MaximumPage);
        }

        [Theory]
        [InlineData(20)]
        [InlineData(int.MaxValue)]
        public void MaximumPage_SetValue_ReturnsExpected(int maximumPage)
        {
            var printerSettings = new PrinterSettings()
            {
                MaximumPage = maximumPage
            };

            Assert.Equal(maximumPage, printerSettings.MaximumPage);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void MaximumPage_Invalid_ThrowsArgumentException(int maximumPage)
        {
            var printerSettings = new PrinterSettings();
            AssertExtensions.Throws<ArgumentException>(null, () => printerSettings.MaximumPage = maximumPage);
        }

        [Fact]
        public void MinimumPage_Default_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();
            Assert.Equal(0, printerSettings.MinimumPage);
        }

        [Theory]
        [InlineData(20)]
        [InlineData(int.MaxValue)]
        public void MinimumPage_SetValue_ReturnsExpected(int minimumPage)
        {
            var printerSettings = new PrinterSettings()
            {
                MinimumPage = minimumPage
            };

            Assert.Equal(minimumPage, printerSettings.MinimumPage);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void MinimumPage_Invalid_ThrowsArgumentException(int minimumPage)
        {
            var printerSettings = new PrinterSettings();
            AssertExtensions.Throws<ArgumentException>(null, () => printerSettings.MinimumPage = minimumPage);
        }

        [Fact]
        public void PrintFileName_SetValue_ReturnsExpected()
        {
            var printFileName = "fileName";
            var printerSettings = new PrinterSettings()
            {
                PrintFileName = printFileName
            };

            Assert.Equal(printFileName, printerSettings.PrintFileName);
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [Fact]
        public void PrintFileName_Null_ThrowsArgumentNullException()
        {
            var printerSettings = new PrinterSettings();
            AssertExtensions.Throws<ArgumentNullException>(null, () => printerSettings.PrintFileName = null);
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [Fact]
        public void PrintFileName_Empty_ThrowsArgumentNullException()
        {
            var printerSettings = new PrinterSettings();
            AssertExtensions.Throws<ArgumentNullException>(string.Empty, () => printerSettings.PrintFileName = string.Empty);
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.AnyInstalledPrinters, Helpers.GdiplusIsAvailable)]
        public void PaperSizes_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();
            Assert.NotNull(printerSettings.PaperSizes);
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.AnyInstalledPrinters, Helpers.GdiplusIsAvailable)]
        public void PaperSources_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();
            Assert.NotNull(printerSettings.PaperSources);
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [Theory]
        [InlineData(PrintRange.AllPages)]
        [InlineData(PrintRange.CurrentPage)]
        [InlineData(PrintRange.Selection)]
        [InlineData(PrintRange.SomePages)]
        public void PrintRange_SetValue_ReturnsExpected(PrintRange printRange)
        {
            var printerSettings = new PrinterSettings()
            {
                PrintRange = printRange
            };

            Assert.Equal(printRange, printerSettings.PrintRange);
        }
        
        [Theory]
        [InlineData(PrintRange.AllPages - 1)]
        [InlineData(PrintRange.SomePages + 1)]
        [InlineData((PrintRange)int.MaxValue)]
        [InlineData((PrintRange)int.MinValue)]
        public void PrintRange_Invalid_ThrowsInvalidEnumArgumentException(PrintRange printRange)
        {
            var printerSettings = new PrinterSettings();
            Assert.ThrowsAny<ArgumentException>(() => printerSettings.PrintRange = printRange);
        }

        [Fact]
        public void PrintToFile_SetValue_ReturnsExpected()
        {
            var printToFile = true;
            var printerSettings = new PrinterSettings()
            {
                PrintToFile = printToFile
            };

            Assert.Equal(printToFile, printerSettings.PrintToFile);
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [Theory]
        [InlineData("")]
        [InlineData("My printer")]
        public void PrinterName_SetValue_ReturnsExpected(string printerName)
        {
            var printerSettings = new PrinterSettings()
            {
                PrinterName = printerName
            };

            Assert.Equal(printerName, printerSettings.PrinterName);
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.AnyInstalledPrinters, Helpers.GdiplusIsAvailable)]
        public void PrinterName_Null_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings()
            {
                PrinterName = null
            };

            Assert.NotNull(printerSettings.PrinterName);
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.AnyInstalledPrinters, Helpers.GdiplusIsAvailable)]
        public void PrinterResolutions_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();
            Assert.NotNull(printerSettings.PrinterResolutions);
        }

        public static IEnumerable<object[]> IsDirectPrintingSupported_ImageFormatSupported_TestData()
        {
            yield return new object[] { ImageFormat.Jpeg };
            yield return new object[] { ImageFormat.Png };
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalTheory(Helpers.AnyInstalledPrinters, Helpers.GdiplusIsAvailable)]
        [MemberData(nameof(IsDirectPrintingSupported_ImageFormatSupported_TestData))]
        public void IsDirectPrintingSupported_ImageFormatSupported_ReturnsExpected(ImageFormat imageFormat)
        {
            var printerSettings = new PrinterSettings();
            bool supported = printerSettings.IsDirectPrintingSupported(imageFormat);
        }

        public static IEnumerable<object[]> IsDirectPrintingSupported_ImageFormatNotSupported_TestData()
        {
            yield return new object[] { ImageFormat.Emf };
            yield return new object[] { ImageFormat.Exif };
            yield return new object[] { ImageFormat.Gif };
            yield return new object[] { ImageFormat.Icon };
            yield return new object[] { ImageFormat.MemoryBmp };
            yield return new object[] { ImageFormat.Tiff };
            yield return new object[] { ImageFormat.Wmf };
            yield return new object[] { ImageFormat.Bmp };
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [Theory]
        [MemberData(nameof(IsDirectPrintingSupported_ImageFormatNotSupported_TestData))]
        public void IsDirectPrintingSupported_ImageFormatNotSupported_ReturnsExpected(ImageFormat imageFormat)
        {
            var printerSettings = new PrinterSettings();
            Assert.Equal(false, printerSettings.IsDirectPrintingSupported(imageFormat));
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void IsDirectPrintingSupported_ImageNotSupported_ReturnsExpected()
        {
            using (var bitmap = new Bitmap(10, 10))
            {
                var printerSettings = new PrinterSettings();
                Assert.Equal(false, printerSettings.IsDirectPrintingSupported(bitmap));
            }
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.AnyInstalledPrinters, Helpers.GdiplusIsAvailable)]
        public void SupportsColor_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();
            bool supportsColor = printerSettings.SupportsColor;
        }

        [Theory]
        [InlineData(20)]
        [InlineData(int.MaxValue)]
        public void ToPage_SetValue_ReturnsExpected(int toPage)
        {
            var printerSettings = new PrinterSettings()
            {
                ToPage = toPage
            };

            Assert.Equal(toPage, printerSettings.ToPage);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void ToPage_Invalid_ThrowsArgumentException(int toPage)
        {
            var printerSettings = new PrinterSettings();
            AssertExtensions.Throws<ArgumentException>(null, () => printerSettings.ToPage = toPage);
        }

        [ConditionalFact(Helpers.AnyInstalledPrinters, Helpers.WindowsRS3OrEarlier)] // RS4 failures: https://github.com/dotnet/corefx/issues/29858
        public void Clone_Success()
        {
            var printerSettings = new PrinterSettings();
            PrinterSettings clone = Assert.IsAssignableFrom<PrinterSettings>(printerSettings.Clone());
            Assert.False(ReferenceEquals(clone, printerSettings));
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.AnyInstalledPrinters, Helpers.GdiplusIsAvailable, Helpers.WindowsRS3OrEarlier)] // RS4 failures: https://github.com/dotnet/corefx/issues/29858
        public void CreateMeasurementGraphics_Default_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();
            using (Graphics graphic = printerSettings.CreateMeasurementGraphics())
            {
                Assert.NotNull(graphic);
                Assert.Equal(printerSettings.DefaultPageSettings.Bounds.X, graphic.VisibleClipBounds.X, 0);
                Assert.Equal(printerSettings.DefaultPageSettings.Bounds.Y, graphic.VisibleClipBounds.Y, 0);
                Assert.Equal(printerSettings.DefaultPageSettings.PrintableArea.Height, graphic.VisibleClipBounds.Height, 0);
                Assert.Equal(printerSettings.DefaultPageSettings.PrintableArea.Width, graphic.VisibleClipBounds.Width, 0);
            }
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.AnyInstalledPrinters, Helpers.GdiplusIsAvailable, Helpers.WindowsRS3OrEarlier)] // RS4 failures: https://github.com/dotnet/corefx/issues/29858
        public void CreateMeasurementGraphics_Bool_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();
            using (Graphics graphic = printerSettings.CreateMeasurementGraphics(true))
            {
                Assert.NotNull(graphic);
                Assert.Equal(printerSettings.DefaultPageSettings.PrintableArea.Height, graphic.VisibleClipBounds.Height, 0);
                Assert.Equal(printerSettings.DefaultPageSettings.PrintableArea.Width, graphic.VisibleClipBounds.Width, 0);
            }
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.AnyInstalledPrinters, Helpers.GdiplusIsAvailable, Helpers.WindowsRS3OrEarlier)] // RS4 failures: https://github.com/dotnet/corefx/issues/29858
        public void CreateMeasurementGraphics_PageSettings_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();
            var pageSettings = new PageSettings();
            using (Graphics graphic = printerSettings.CreateMeasurementGraphics(pageSettings))
            {
                Assert.NotNull(graphic);
                Assert.Equal(printerSettings.DefaultPageSettings.Bounds.X, graphic.VisibleClipBounds.X, 0);
                Assert.Equal(printerSettings.DefaultPageSettings.Bounds.Y, graphic.VisibleClipBounds.Y, 0);
                Assert.Equal(printerSettings.DefaultPageSettings.PrintableArea.Height, graphic.VisibleClipBounds.Height, 0);
                Assert.Equal(printerSettings.DefaultPageSettings.PrintableArea.Width, graphic.VisibleClipBounds.Width, 0);
            }
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.AnyInstalledPrinters, Helpers.GdiplusIsAvailable, Helpers.WindowsRS3OrEarlier)] // RS4 failures: https://github.com/dotnet/corefx/issues/29858
        public void CreateMeasurementGraphics_PageSettingsBool_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();
            var pageSettings = new PageSettings();
            using (Graphics graphic = printerSettings.CreateMeasurementGraphics(pageSettings, true))
            {
                Assert.NotNull(graphic);
                Assert.Equal(printerSettings.DefaultPageSettings.PrintableArea.Height, graphic.VisibleClipBounds.Height, 0);
                Assert.Equal(printerSettings.DefaultPageSettings.PrintableArea.Width, graphic.VisibleClipBounds.Width, 0);
            }
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.GdiplusIsAvailable, Helpers.WindowsRS3OrEarlier)] // RS4 failures: https://github.com/dotnet/corefx/issues/29858
        public void CreateMeasurementGraphics_Null_ThrowsNullReferenceException()
        {
            var printerSettings = new PrinterSettings();
            Assert.Throws<NullReferenceException>(() => printerSettings.CreateMeasurementGraphics(null));
            Assert.Throws<NullReferenceException>(() => printerSettings.CreateMeasurementGraphics(null, true));
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetHdevmode_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();
            IntPtr handle = IntPtr.Zero;

            handle = printerSettings.GetHdevmode();
            Assert.NotEqual(IntPtr.Zero, handle);
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetHdevmode_PageSettings_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();
            var pageSettings = new PageSettings();
            IntPtr handle = IntPtr.Zero;

            handle = printerSettings.GetHdevmode(pageSettings);
            Assert.NotEqual(IntPtr.Zero, handle);
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetHdevmode_Null_ThrowsNullReferenceException()
        {
            var printerSettings = new PrinterSettings();
            Assert.Throws<NullReferenceException>(() => printerSettings.GetHdevmode(null));
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetHdevnames_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();
            IntPtr handle = IntPtr.Zero;

            handle = printerSettings.GetHdevnames();
            Assert.NotEqual(IntPtr.Zero, handle);
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(typeof(PrinterSettingsTests), nameof(CanTestSetHdevmode_IntPtr_Success))]
        public void SetHdevmode_IntPtr_Success()
        {
            string printerName = GetNameOfTestPrinterSuitableForDevModeTesting();
            var printerSettings = new PrinterSettings() { PrinterName = printerName, Copies = 3 };
            var newPrinterSettings = new PrinterSettings() { PrinterName = printerName, Copies = 6 };

            IntPtr handle = printerSettings.GetHdevmode();
            newPrinterSettings.SetHdevmode(handle);
            Assert.Equal(printerSettings.Copies, newPrinterSettings.Copies);
            Assert.Equal(printerSettings.Collate, newPrinterSettings.Collate);
            Assert.Equal(printerSettings.Duplex, newPrinterSettings.Duplex);
        }

        public static bool CanTestSetHdevmode_IntPtr_Success => Helpers.GetGdiplusIsAvailable() && GetNameOfTestPrinterSuitableForDevModeTesting() != null;

        private static string GetNameOfTestPrinterSuitableForDevModeTesting()
        {
            foreach (string candidate in s_TestPrinterNames)
            {
                PrinterSettings printerSettings = new PrinterSettings() { PrinterName = candidate };
                if (printerSettings.IsValid)
                    return candidate;
            }
            return null;
        }

        private static readonly string[] s_TestPrinterNames =
        {
            // Our method of testing this api requires a printer that supports multi-copy printing, collating and duplex settings. Not all printers
            // support these so rather than trust the machine running the test to have configured such a printer as the default, use the name of 
            // a known compliant printer that ships with Windows 10.
            "Microsoft Print to PDF",
            "Microsoft XPS Document Writer", // Backup for older Windows
        };

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void GetHdevmode_Zero_ThrowsArgumentException()
        {
            var printerSettings = new PrinterSettings();
            AssertExtensions.Throws<ArgumentException>(null, () => printerSettings.SetHdevmode(IntPtr.Zero));
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void SetHdevnames_IntPtr_Success()
        {
            var printerSettings = new PrinterSettings();
            var newPrinterSettings = new PrinterSettings();
            IntPtr handle = printerSettings.GetHdevnames();
            newPrinterSettings.SetHdevnames(handle);
            Assert.Equal(newPrinterSettings.PrinterName, printerSettings.PrinterName);
        }

        [ActiveIssue(20884, TestPlatforms.AnyUnix)]
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ToString_ReturnsExpected()
        {
            var printerSettings = new PrinterSettings();
            var expected = "[PrinterSettings "
                + printerSettings.PrinterName
                + " Copies=" + printerSettings.Copies.ToString(CultureInfo.InvariantCulture)
                + " Collate=" + printerSettings.Collate.ToString(CultureInfo.InvariantCulture)
                + " Duplex=" + printerSettings.Duplex.ToString()
                + " FromPage=" + printerSettings.FromPage.ToString(CultureInfo.InvariantCulture)
                + " LandscapeAngle=" + printerSettings.LandscapeAngle.ToString(CultureInfo.InvariantCulture)
                + " MaximumCopies=" + printerSettings.MaximumCopies.ToString(CultureInfo.InvariantCulture)
                + " OutputPort=" + printerSettings.PrintFileName.ToString(CultureInfo.InvariantCulture)
                + " ToPage=" + printerSettings.ToPage.ToString(CultureInfo.InvariantCulture)
                + "]";

            Assert.Equal(expected, printerSettings.ToString());
        }
    }
}
