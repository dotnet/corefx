// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.RegularExpressions
{
    public partial class Regex
    {
        // The APIs below should call into ROM<char>-based APIs rather
        // than allocating new String instances each time.

        public bool IsMatch(Utf8String input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return IsMatch(input.ToString());
        }

        public bool IsMatch(Utf8String input, int startat)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return IsMatch(input.ToString(), startat);
        }

        public static bool IsMatch(Utf8String input, string pattern)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return IsMatch(input.ToString(), pattern);
        }

        public static bool IsMatch(Utf8String input, string pattern, RegexOptions options)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return IsMatch(input.ToString(), pattern, options);
        }

        public static bool IsMatch(Utf8String input, string pattern, RegexOptions options, TimeSpan matchTimeout)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return IsMatch(input.ToString(), pattern, options, matchTimeout);
        }
    }
}
