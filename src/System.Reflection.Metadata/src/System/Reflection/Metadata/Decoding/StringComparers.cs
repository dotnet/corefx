// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace System.Reflection.Metadata.Decoding
{
    internal static class StringComparers
    {
        public static IEqualityComparer<string> AssemblyNameComponent = StringComparer.OrdinalIgnoreCase;
    }
}
