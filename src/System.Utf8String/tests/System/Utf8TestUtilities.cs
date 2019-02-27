// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace System.Tests
{
    public static class Utf8TestUtilities
    {
        /// <summary>
        /// Mimics returning a literal <see cref="Utf8String"/> instance.
        /// </summary>
        public static Utf8String u8(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return Utf8String.Empty;
            }

            // TODO: Call into ctor.

            return new Utf8String(str);
        }
    }
}
