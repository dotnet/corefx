// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// Extension methods for working with <see cref="Utf8String"/>.
    /// </summary>
    public static class MemoryUtf8Extensions
    {
        /// <summary>
        /// Converts a <see cref="Boolean"/> to a <see cref="Utf8String"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Utf8String ToUtf8String(this bool value)
        {
            if (!value)
            {
                return RuntimeHelpers.GetUtf8StringLiteral("False");
            }
            return RuntimeHelpers.GetUtf8StringLiteral("True");
        }

        /// <summary>
        /// Converts a <see cref="Guid"/> to a <see cref="Utf8String"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Utf8String ToUtf8String(this Guid value)
        {
            return Utf8String.Create(length: 36, state: value, (buffer, state) =>
            {
                bool success = Utf8Formatter.TryFormat(state, buffer, out _);
                Debug.Assert(success);
            });
        }
    }
}
