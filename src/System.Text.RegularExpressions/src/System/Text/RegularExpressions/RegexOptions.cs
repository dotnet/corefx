// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.RegularExpressions
{
    [Flags]
    public enum RegexOptions
    {
        None                    = 0x0000,
        IgnoreCase              = 0x0001, // "i"
        Multiline               = 0x0002, // "m"
        ExplicitCapture         = 0x0004, // "n"
        Compiled                = 0x0008, // "c"
        Singleline              = 0x0010, // "s"
        IgnorePatternWhitespace = 0x0020, // "x"
        RightToLeft             = 0x0040, // "r"

#if DEBUG
        Debug                   = 0x0080, // "d"
#endif

        ECMAScript              = 0x0100, // "e"
        CultureInvariant        = 0x0200,
    }

    internal static class RegexOptionsExtensions
    {
        /// <summary>
        /// True if N option disabling '(' autocapture is on.
        /// </summary>
        public static bool UseOptionN(this RegexOptions options) => (options & RegexOptions.ExplicitCapture) != 0;

        /// <summary>
        /// True if I option enabling case-insensitivity is on.
        /// </summary>
        public static bool UseOptionI(this RegexOptions options) => (options & RegexOptions.IgnoreCase) != 0;

        /// <summary>
        /// True if M option altering meaning of $ and ^ is on.
        /// </summary>
        public static bool UseOptionM(this RegexOptions options) => (options & RegexOptions.Multiline) != 0;

        /// <summary>
        /// True if S option altering meaning of . is on.
        /// </summary>
        public static bool UseOptionS(this RegexOptions options) => (options & RegexOptions.Singleline) != 0;

        /// <summary>
        /// True if X option enabling whitespace/comment mode is on.
        /// </summary>
        public static bool UseOptionX(this RegexOptions options) => (options & RegexOptions.IgnorePatternWhitespace) != 0;

        /// <summary>
        /// True if E option enabling ECMAScript behavior is on.
        /// </summary>
        public static bool UseOptionE(this RegexOptions options) => (options & RegexOptions.ECMAScript) != 0;

        /// <summary>
        /// True if Invariant option enabling culture invariant behavior is on.
        /// </summary>
        public static bool UseOptionInvariant(this RegexOptions options) => (options & RegexOptions.CultureInvariant) != 0;

        /// <summary>
        /// True if C option enabling compiled regexes is on.
        /// </summary>
        public static bool UseOptionC(this RegexOptions options) => (options & RegexOptions.Compiled) != 0;

        /// <summary>
        /// True if R option enabling right-to-left mode is on.
        /// </summary>
        public static bool UseOptionR(this RegexOptions options) => (options & RegexOptions.RightToLeft) != 0;

#if DEBUG
        /// <summary>
        /// True if the regex has debugging enabled
        /// </summary>
        public static bool IsDebug(this RegexOptions options) => (options & RegexOptions.Debug) != 0;
#endif
    }
}
