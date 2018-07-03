// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Text;

internal static partial class Interop
{
    internal static partial class Globalization
    {
        [DllImport(Libraries.GlobalizationNative, CharSet = CharSet.Unicode, EntryPoint = "GlobalizationNative_GetLocaleName")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern unsafe bool GetLocaleName(string localeName, [Out] StringBuilder value, int valueLength);

        [DllImport(Libraries.GlobalizationNative, CharSet = CharSet.Unicode, EntryPoint = "GlobalizationNative_GetLocaleInfoString")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern unsafe bool GetLocaleInfoString(string localeName, uint localeStringData, [Out] StringBuilder value, int valueLength);

        [DllImport(Libraries.GlobalizationNative, CharSet = CharSet.Unicode, EntryPoint = "GlobalizationNative_GetDefaultLocaleName")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern unsafe bool GetDefaultLocaleName([Out] StringBuilder value, int valueLength);

        [DllImport(Libraries.GlobalizationNative, CharSet = CharSet.Unicode, EntryPoint = "GlobalizationNative_GetLocaleTimeFormat")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern unsafe bool GetLocaleTimeFormat(string localeName, bool shortFormat, [Out] StringBuilder value, int valueLength);

        [DllImport(Libraries.GlobalizationNative, CharSet = CharSet.Unicode, EntryPoint = "GlobalizationNative_GetLocaleInfoInt")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern unsafe bool GetLocaleInfoInt(string localeName, uint localeNumberData, ref int value);

        [DllImport(Libraries.GlobalizationNative, CharSet = CharSet.Unicode, EntryPoint = "GlobalizationNative_GetLocaleInfoGroupingSizes")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern unsafe bool GetLocaleInfoGroupingSizes(string localeName, uint localeGroupingData, ref int primaryGroupSize, ref int secondaryGroupSize);

        [DllImport(Libraries.GlobalizationNative, CharSet = CharSet.Unicode, EntryPoint = "GlobalizationNative_GetLocales")]
        internal static extern unsafe int GetLocales([Out] char[] value, int valueLength);
    }
}
