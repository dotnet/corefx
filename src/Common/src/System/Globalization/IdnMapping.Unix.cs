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
            CheckInvalidIdnCharacters(unicode, count, flags, nameof(unicode));

            const int StackAllocThreshold = 512;
            if (count < StackAllocThreshold)
            {
                char* output = stackalloc char[count];
                return GetAsciiCore(unicode, count, flags, output, count, reattempt: true);
            }
            else
            {
                char[] output = new char[count];
                fixed (char* pOutput = output)
                {
                    return GetAsciiCore(unicode, count, flags, pOutput, count, reattempt: true);
                }
            }
        }

        private unsafe string GetAsciiCore(char* unicode, int count, uint flags, char* output, int outputLength, bool reattempt)
        {
            int realLen = Interop.GlobalizationNative.ToAscii(flags, unicode, count, output, outputLength);

            if (realLen == 0)
            {
                throw new ArgumentException(SR.Argument_IdnIllegalName, nameof(unicode));
            }
            else if (realLen <= outputLength)
            {
                return new string(output, 0, realLen);
            }
            else if (reattempt)
            {
                char[] newOutput = new char[realLen];
                fixed (char* pNewOutput = newOutput)
                {
                    return GetAsciiCore(unicode, count, flags, pNewOutput, realLen, reattempt: false);
                }
            }

            throw new ArgumentException(SR.Argument_IdnIllegalName, nameof(unicode));
        }

        private unsafe string GetUnicodeCore(char* ascii, int count)
        {
            uint flags = Flags;
            CheckInvalidIdnCharacters(ascii, count, flags, nameof(ascii));

            const int StackAllocThreshold = 512;
            if (count < StackAllocThreshold)
            {
                char* output = stackalloc char[count];
                return GetUnicodeCore(ascii, count, flags, output, count, reattempt: true);
            }
            else
            {
                char[] output = new char[count];
                fixed (char* pOutput = output)
                {
                    return GetUnicodeCore(ascii, count, flags, pOutput, count, reattempt: true);
                }
            }
        }

        private unsafe string GetUnicodeCore(char* ascii, int count, uint flags, char* output, int outputLength, bool reattempt)
        {
            int realLen = Interop.GlobalizationNative.ToUnicode(flags, ascii, count, output, outputLength);

            if (realLen == 0)
            {
                throw new ArgumentException(SR.Argument_IdnIllegalName, nameof(ascii));
            }
            else if (realLen <= outputLength)
            {
                return new string(output, 0, realLen);
            }
            else if (reattempt)
            {
                char[] newOutput = new char[realLen];
                fixed (char* pNewOutput = newOutput)
                {
                    return GetUnicodeCore(ascii, count, flags, pNewOutput, realLen, reattempt: false);
                }
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
