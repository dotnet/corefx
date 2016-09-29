// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;

namespace Internal.Cryptography
{
    internal abstract partial class PkcsPal
    {
        private static PkcsPal s_instance;

        static PkcsPal()
        {
            throw new PlatformNotSupportedException();
        }
    }
}
