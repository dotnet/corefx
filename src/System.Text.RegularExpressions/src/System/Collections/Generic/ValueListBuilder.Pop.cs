// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    /// <summary>
    /// These public methods are required by RegexWriter.
    /// </summary>
    internal ref partial struct ValueListBuilder<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Pop()
        {
            _pos--;
            return ref _span[_pos];
        }
    }
}
