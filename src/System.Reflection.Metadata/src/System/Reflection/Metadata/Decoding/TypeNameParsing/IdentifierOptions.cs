// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Reflection.Metadata.Decoding
{
    [Flags]
    internal enum IdentifierOptions
    {
        None = 0,
        TrimStart = 1,
        Trim = 2,
        Required = 4,
    }
}
