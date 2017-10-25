// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.
//
// PrintingServicesUnix class unit tests
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
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using Xunit;

namespace MonoTests.System.Drawing.Printing
{

    public class PrintingServicesUnixTest
    {
        #region Novell Bug #602934

        #region CUPS methods and structs

        [StructLayout(LayoutKind.Sequential)]
        struct CUPS_DEST
        {
            public string Name;
            public string Instance;
            public int IsDefault;
            public int NumOptions;
            public IntPtr Options;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct CUPS_OPTION
        {
            public string Name;
            public string Value;
        }

        readonly IntPtr CUPS_HTTP_DEFAULT = IntPtr.Zero;

        [DllImport("libcups")]
        static extern IntPtr cupsGetNamedDest(IntPtr http, string name, string instance);

        [DllImport("libcups")]
        static extern void cupsFreeDests(int num_dests, IntPtr dests);

        [DllImport("libcups")]
        static extern void cupsFreeDests(int num_dests, ref CUPS_DEST dests);

        #endregion

        Dictionary<string, string> GetOptionsOfFirstPrinterThroughCups()
        {
            var options = new Dictionary<string, string>();

            var destPtr = cupsGetNamedDest(CUPS_HTTP_DEFAULT, PrinterSettings.InstalledPrinters[0], null);
            var dest = (CUPS_DEST)Marshal.PtrToStructure(destPtr, typeof(CUPS_DEST));
            var optionPtr = dest.Options;
            int cupsOptionSize = Marshal.SizeOf(typeof(CUPS_OPTION));
            for (int i = 0; i < dest.NumOptions; i++)
            {
                var cupsOption = (CUPS_OPTION)Marshal.PtrToStructure(optionPtr, typeof(CUPS_OPTION));
                options.Add(cupsOption.Name, cupsOption.Value);
                optionPtr = (IntPtr)((long)optionPtr + cupsOptionSize);
            }
            cupsFreeDests(1, destPtr);
            return options;
        }

        [ActiveIssue(20844)]
        public void Bug602934_PrinterSettingsReturnActualValues()
        {
            if (PrinterSettings.InstalledPrinters.Count < 1)
                Assert.True(false, "Need at least one printer installed.");

            var options = GetOptionsOfFirstPrinterThroughCups();

            var settings = new PrinterSettings() { PrinterName = PrinterSettings.InstalledPrinters[0] };
            Assert.Equal(options["PageSize"], settings.DefaultPageSettings.PaperSize.PaperName);
            if (options.ContainsKey("Resolution"))
                Assert.Equal(options["Resolution"], string.Format("{0}dpi", settings.DefaultPageSettings.PrinterResolution.X));
        }

        #endregion

    }
}
