// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Tests
{
    public static class Utf8TestUtilities
    {
        public static Utf8String U(string value)
        {
            return (value != null) ? new Utf8String(value) : null;
        }
    }
}
