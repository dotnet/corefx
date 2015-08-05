// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Reflection.Metadata.Decoding
{
    internal enum TypeModifierType : int
    {
        SZArray,        // System.Int32[]
        Array,          // System.Int32[*], System.Int32[,], etc
        Pointer,        // System.Int32*
        ByReference,    // System.Int32&
    }
}
