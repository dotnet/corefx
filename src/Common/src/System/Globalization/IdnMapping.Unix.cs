// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Globalization
{
    sealed partial class IdnMapping
    {
        private unsafe string GetAsciiCore(char* unicode, int count)
        {
            uint flags = Flags;
            CheckInvalidIdnCharacters(unicode, count, flags, "unicode");

            char[] buf = new char[count];

            for (int attempts = 2; attempts > 0; attempts--)
            {
                int realLen = Interop.GlobalizationNative.ToAscii(flags, unicode, count, buf, buf.Length);

                if (realLen == 0)
                {
                    break;
                }

                if (realLen <= buf.Length)
                {
                    return new string(buf, 0, realLen);
                }

                buf = new char[realLen];
            }

            throw new ArgumentException(SR.Argument_IdnIllegalName, nameof(unicode));
        }

        private unsafe string GetUnicodeCore(char* ascii, int count)
        {
            uint flags = Flags;
            CheckInvalidIdnCharacters(ascii, count, flags, "ascii");

            char[] buf = new char[count];

            for (int attempts = 2; attempts > 0; attempts--)
            {
                int realLen = Interop.GlobalizationNative.ToUnicode(flags, ascii, count, buf, buf.Length);

                if (realLen == 0)
                {
                    break;
                }

                if (realLen <= buf.Length)
                {
                    return new string(buf, 0, realLen);
                }

                buf = new char[realLen];
            }

            throw new ArgumentException(SR.Argument_IdnIllegalName, nameof(ascii));
        }

        // -----------------------------
        // ---- PAL layer ends here ----
        // -----------------------------

        private uint Flags
        {
            get
            {
                int flags =
                    (AllowUnassigned ? Interop.GlobalizationNative.AllowUnassigned : 0) |
                    (UseStd3AsciiRules ? Interop.GlobalizationNative.UseStd3AsciiRules : 0);
                return (uint)flags;
            }
        }

        /// <summary>
        /// ICU doesn't check for invalid characters unless the STD3 rules option
        /// is enabled.
        ///
        /// To match Windows behavior, we walk the string ourselves looking for these
        /// bad characters so we can continue to throw ArgumentException in these cases. 
        /// </summary>
        private static unsafe void CheckInvalidIdnCharacters(char* s, int count, uint flags, string paramName)
        {
            if ((flags & Interop.GlobalizationNative.UseStd3AsciiRules) == 0)
            {
                for (int i = 0; i < count; i++)
                {
                    char c = s[i];

                    // These characters are prohibited regardless of the UseStd3AsciiRules property.
                    // See https://msdn.microsoft.com/en-us/library/system.globalization.idnmapping.usestd3asciirules(v=vs.110).aspx
                    if (c <= 0x1F || c == 0x7F)
                    {
                        throw new ArgumentException(SR.Argument_IdnIllegalName, paramName);
                    }
                }
            }
        }
    }
}
