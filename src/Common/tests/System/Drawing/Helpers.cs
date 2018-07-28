﻿// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.

using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;
using Xunit.Sdk;

namespace System.Drawing
{
    public static class Helpers
    {
        public const string GdiplusIsAvailable = nameof(Helpers) + "." + nameof(GetGdiplusIsAvailable);
        public const string RecentGdiplusIsAvailable = nameof(Helpers) + "." + nameof(GetRecentGdiPlusIsAvailable);
        public const string RecentGdiplusIsAvailable2 = nameof(Helpers) + "." + nameof(GetRecentGdiPlusIsAvailable2);
        public const string GdiPlusIsAvailableNotRedhat73 = nameof(Helpers) + "." + nameof(GetGdiPlusIsAvailableNotRedhat73);
        public const string GdiPlusIsAvailableNotWindows7 = nameof(Helpers) + "." + nameof(GetGdiPlusIsAvailableNotWindows7);
        public const string AnyInstalledPrinters = nameof(Helpers) + "." + nameof(IsAnyInstalledPrinters);
        public const string WindowsRS3OrEarlier = nameof(Helpers) + "." + nameof(IsWindowsRS3OrEarlier);

        public static bool GetGdiplusIsAvailable()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return PlatformDetection.IsNotWindowsNanoServer && PlatformDetection.IsNotWindowsServerCore;
            }
            else
            {
                IntPtr nativeLib;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    nativeLib = dlopen("libgdiplus.dylib", RTLD_NOW);
                }
                else
                {
                    nativeLib = dlopen("libgdiplus.so", RTLD_NOW);
                    if (nativeLib == IntPtr.Zero)
                    {
                        nativeLib = dlopen("libgdiplus.so.0", RTLD_NOW);
                    }
                }

                return nativeLib != IntPtr.Zero;
            }
        }

        public static bool IsNotUnix => PlatformDetection.IsWindows;

        public static bool IsWindowsRS3OrEarlier => !PlatformDetection.IsWindows10Version1803OrGreater;

        public static bool GetRecentGdiPlusIsAvailable2()
        {
            // RedHat and Ubuntu 14.04, as well as Fedora 25 and OpenSUSE 4.22 are running outdated versions of libgdiplus
            if (PlatformDetection.IsRedHatFamily || PlatformDetection.IsUbuntu1404 || PlatformDetection.IsFedora || PlatformDetection.IsOpenSUSE)
            {
                return false;
            }

            return GetGdiplusIsAvailable();
        }

        public static bool GetGdiPlusIsAvailableNotRedhat73()
        {
            if (PlatformDetection.IsRedHatFamily)
            {
                return false;
            }

            return GetGdiplusIsAvailable();
        }

        public static bool GetGdiPlusIsAvailableNotWindows7()
        {
            if (PlatformDetection.IsWindows7)
            {
                return false;
            }

            return GetGdiplusIsAvailable();
        }

        public static bool GetRecentGdiPlusIsAvailable()
        {
            // RedHat and Ubuntu 14.04 are running outdated versions of libgdiplus
            if (PlatformDetection.IsRedHatFamily || PlatformDetection.IsUbuntu1404)
            {
                return false;
            }

            return GetGdiplusIsAvailable();
        }

        public static bool IsAnyInstalledPrinters()
        {
            return PrinterSettings.InstalledPrinters.Count > 0;
        }

        [DllImport("libdl")]
        private static extern IntPtr dlopen(string libName, int flags);
        public const int RTLD_NOW = 0x002;

        public static string GetTestBitmapPath(string fileName) => GetTestPath("bitmaps", fileName);
        public static string GetTestFontPath(string fileName) => GetTestPath("fonts", fileName);
        public static string GetTestColorProfilePath(string fileName) => GetTestPath("colorProfiles", fileName);

        private static string GetTestPath(string directoryName, string fileName) => Path.Combine(AppContext.BaseDirectory, directoryName, fileName);

        public static void VerifyBitmap(Bitmap bitmap, Color[][] colors)
        {
            for (int y = 0; y < colors.Length; y++)
            {
                for (int x = 0; x < colors[y].Length; x++)
                {
                    Color expectedColor = Color.FromArgb(colors[y][x].ToArgb());
                    Color actualColor = bitmap.GetPixel(x, y);

                    if (expectedColor != actualColor)
                    {
                        throw GetBitmapEqualFailureException(bitmap, colors, x, y);
                    }
                }
            }
        }

        private static Exception GetBitmapEqualFailureException(Bitmap bitmap, Color[][] colors, int firstFailureX, int firstFailureY)
        {
            // Print out the whole bitmap to provide a view of the whole image, rather than just the difference between
            // a single pixel.
            var actualStringBuilder = new StringBuilder();
            var expectedStringBuilder = new StringBuilder();

            actualStringBuilder.AppendLine();
            expectedStringBuilder.AppendLine();

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    PrintColor(actualStringBuilder, bitmap.GetPixel(x, y));
                    PrintColor(expectedStringBuilder, colors[y][x]);
                    if (x != bitmap.Width - 1)
                    {
                        actualStringBuilder.Append(", ");
                        expectedStringBuilder.Append(", ");
                    }
                }
                actualStringBuilder.AppendLine();
                expectedStringBuilder.AppendLine();
            }

            return new AssertActualExpectedException(expectedStringBuilder.ToString(), actualStringBuilder.ToString(), $"Bitmaps were different at {firstFailureX}, {firstFailureY}.");
        }

        private static void PrintColor(StringBuilder stringBuilder, Color color)
        {
            stringBuilder.Append($"Color.FromArgb({color.A}, {color.R}, {color.G}, {color.B})");
        }

        public static Color EmptyColor => Color.FromArgb(0, 0, 0, 0);

        private static Rectangle GetRectangle(RECT rect)
        {
            return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
        }

        private const int MONITOR_DEFAULTTOPRIMARY = 1;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr MonitorFromWindow(IntPtr hWnd, int dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO monitorInfo);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        public static Rectangle GetWindowDCRect(IntPtr hdc) => GetHWndRect(WindowFromDC(hdc));

        public static Rectangle GetHWndRect(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
            {
                return GetMonitorRectForWindow(hWnd);
            }

            var rect = new RECT();
            GetClientRect(hWnd, ref rect);

            return GetRectangle(rect);
        }

        private static Rectangle GetMonitorRectForWindow(IntPtr hWnd)
        {
            IntPtr hMonitor = MonitorFromWindow(hWnd, MONITOR_DEFAULTTOPRIMARY);
            Assert.NotEqual(IntPtr.Zero, hMonitor);

            var info = new MONITORINFO();
            info.cbSize = Marshal.SizeOf(info);
            int result = GetMonitorInfo(hMonitor, ref info);
            Assert.NotEqual(0, result);

            return GetRectangle(info.rcMonitor);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetClientRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr WindowFromDC(IntPtr hdc);

        [StructLayout(LayoutKind.Sequential)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public int dwFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static void VerifyBitmapNotBlank(Bitmap bmp)
        {
            Color emptyColor = Color.FromArgb(0);
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color pixel = bmp.GetPixel(x, y);
                    if (!pixel.Equals(emptyColor))
                    {
                        return;
                    }
                }
            }

            throw new XunitException("The entire image was blank.");
        }
    }
}
