// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Text;
using System.IO;

namespace System
{
    /// <summary>Base class for performance test classes</summary>
    public abstract class PerfTestBase
    {
        /// <summary>
        /// Helper method to create a string containing a number of random
        /// characters equal to the specified length
        /// </summary>
        public static string CreateString(int length)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder empty = new StringBuilder();
            while (builder.Length < length)
            {
                string toAppend = Guid.NewGuid().ToString();
                int toAppendLength = Math.Min(toAppend.Length, length - builder.Length);
                builder.Append(toAppend, 0, toAppendLength);
            }
            return builder.ToString();
        }

        /// <summary>Gets a test file full path that is associated with the call site.</summary>
        /// <param name="index">An optional index value to use as a suffix on the file name.  Typically a loop index.</param>
        /// <param name="memberName">The member name of the function calling this method.</param>
        /// <param name="lineNumber">The line number of the function calling this method.</param>
        protected string GetTestFilePath(int? index = null, [CallerMemberName] string memberName = null, [CallerLineNumber] int lineNumber = 0)
        {
            return Path.Combine(Path.GetTempPath(), string.Format(
                index.HasValue ? "{0}_{1}_{2}_{3}_{4}" : "{0}_{1}_{2}_{3}",
                memberName ?? "TestBase", lineNumber, GetType().Name, Path.GetRandomFileName(), 
                index.GetValueOrDefault()));
        }
    }
}