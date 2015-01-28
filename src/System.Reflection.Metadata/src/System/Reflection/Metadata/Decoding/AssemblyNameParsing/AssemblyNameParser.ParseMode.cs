// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Reflection.Metadata.Decoding
{
    partial class AssemblyNameParser
    {
        private enum ParseMode : int
        {
            AssemblyName,                                   // mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
            AssemblyNameWithinGenericTypeArgument,          // [System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null]
        }
    }
}
