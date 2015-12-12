// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data
{
    internal static class LocaleInterop
    {
        // Maps between LCIDs and LocaleName+AnsiCodePages+Encodings
        private static readonly ConcurrentDictionary<int, Tuple<string, int, Encoding>> s_cachedEncodings = new ConcurrentDictionary<int, Tuple<string, int, Encoding>>();
#if !MANAGED_SNI
        private const string ApiWinCoreLocalization = "api-ms-win-core-localization-l1-2-0.dll";
        private const string ApiWinCoreLocalizationObsolete = "api-ms-win-core-localization-obsolete-l1-2-0.dll";
#endif
        private const int LOCALE_NAME_MAX_LENGTH = 85;
        private const int ERROR_INVALID_PARAMETER = 0x57;
        private const uint LOCALE_IDEFAULTANSICODEPAGE = 0x00001004;
        private const uint LOCALE_RETURN_NUMBER = 0x20000000;

#if !MANAGED_SNI
        [DllImport(ApiWinCoreLocalization, SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int GetLocaleInfoEx(string lpLocaleName, uint LCType, [Out] out uint lpLCData, int cchData);

        [DllImport(ApiWinCoreLocalizationObsolete, SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int LCIDToLocaleName(uint Locale, StringBuilder lpName, int cchName, int dwFlags);

        [DllImport(ApiWinCoreLocalization, SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern uint LocaleNameToLCID(string lpName, uint dwFlags);

        static LocaleInterop()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        private static string LcidToLocaleNameInternal(int lcid)
        {
            StringBuilder localName = new StringBuilder(LOCALE_NAME_MAX_LENGTH);

            if (LCIDToLocaleName(unchecked((uint)lcid), localName, localName.Capacity, 0) != 0)
            {
                return localName.ToString();
            }
            else
            {
                // LCIDToLocaleName should not return any other errors if we have sufficient buffer space
                int win32Error = Marshal.GetLastWin32Error();
                Debug.Assert(win32Error == ERROR_INVALID_PARAMETER, string.Format("Unknown error returned by LCIDToLocaleName: {0}. Lcid: {1}", win32Error, lcid));

                throw new CultureNotFoundException("lcid", lcid.ToString(), message: null);
            }
        }

        private static int LocaleNameToAnsiCodePage(string localeName)
        {
            uint ansiCodePage;
            if (GetLocaleInfoEx(localeName, LOCALE_RETURN_NUMBER | LOCALE_IDEFAULTANSICODEPAGE, out ansiCodePage, sizeof(uint)) != 0)
            {
                return unchecked((int)ansiCodePage);
            }
            else
            {
                // GetLocaleInfoEx should not return any other errors if we have sufficient buffer space
                int win32Error = Marshal.GetLastWin32Error();
                Debug.Assert(win32Error == ERROR_INVALID_PARAMETER, string.Format("Unknown error returned by GetLocaleInfoEx: {0}. LocaleName: {1}", win32Error, localeName));

                throw new CultureNotFoundException("localeName", localeName, message: null);
            }
        }

        private static Tuple<string, int, Encoding> GetDetailsInternal(int lcid)
        {
            string localeName = LcidToLocaleNameInternal(lcid);
            int ansiCodePage = LocaleNameToAnsiCodePage(localeName);
            return Tuple.Create(localeName, ansiCodePage, Encoding.GetEncoding(ansiCodePage));
        }

        private static Tuple<string, int, Encoding> GetDetailsForLcid(int lcid)
        {
            if (lcid < 0)
            {
                throw ADP.ArgumentOutOfRange("lcid");
            }
            return s_cachedEncodings.GetOrAdd(lcid, GetDetailsInternal);
        }
#endif

        internal static Encoding GetEncodingForLcid(int lcid)
        {
#if MANAGED_SNI
            if (lcid != TdsEnums.DEFAULT_ENGLISH_LOCALE_ID)
            {
                throw new NotSupportedException("Only the English-US locale is currently supported on non-Windows systems. Invalid Locale ID: " + lcid);
            }
            return Encoding.UTF8;
#else
            return GetDetailsForLcid(lcid).Item3;
#endif
        }

        internal static int GetCodePageForLcid(int lcid)
        {
#if MANAGED_SNI
            if (lcid != TdsEnums.DEFAULT_ENGLISH_LOCALE_ID)
            {
                throw new NotSupportedException("Only the English-US locale is currently supported on non-Windows systems. Invalid Locale ID: " + lcid);
            }
            return TdsEnums.DEFAULT_ENGLISH_CODE_PAGE_VALUE;
#else
            return GetDetailsForLcid(lcid).Item2;
#endif
        }

        internal static string GetLocaleNameForLcid(int lcid)
        {
#if MANAGED_SNI
            if (lcid != TdsEnums.DEFAULT_ENGLISH_LOCALE_ID)
            {
                throw new NotSupportedException("Only the English-US locale is currently supported on non-Windows systems. Invalid Locale ID: " + lcid);
            }
            return TdsEnums.DEFAULT_ENGLISH_LOCALE_NAME;
#else
            return GetDetailsForLcid(lcid).Item1;
#endif
        }

        internal static int GetLcidForLocaleName(string localeName)
        {
            Debug.Assert(localeName != null, "Locale name should never be null");

#if MANAGED_SNI
            if(!localeName.Equals(TdsEnums.DEFAULT_ENGLISH_LOCALE_NAME))
            {
                throw new NotSupportedException("Only the English-US locale is currently supported on non-Windows systems. Invalid Locale Name: " + localeName);
            }
            return TdsEnums.DEFAULT_ENGLISH_LOCALE_ID;
#else
            uint lcid = LocaleNameToLCID(localeName, 0);
            if (lcid != 0)
            {
                return unchecked((int)lcid);
            }
            else
            {
                // LocaleNameToLCID should not return any other errors
                int win32Error = Marshal.GetLastWin32Error();
                Debug.Assert(win32Error == ERROR_INVALID_PARAMETER, string.Format("Unknown error returned by LocaleNameToLCID: {0}. LocaleName: {1}", win32Error, localeName));

                throw new CultureNotFoundException("localeName", localeName, message: null);
            }
#endif
        }

        internal static int GetCurrentCultureLcid()
        {
#if MANAGED_SNI
            return TdsEnums.DEFAULT_ENGLISH_LOCALE_ID;
#else
            return GetLcidForLocaleName(CultureInfo.CurrentCulture.ToString());
#endif
        }
    }
}