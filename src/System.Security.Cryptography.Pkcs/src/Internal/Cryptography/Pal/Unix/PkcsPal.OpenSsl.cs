// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Internal.Cryptography.Pal.OpenSsl;

namespace Internal.Cryptography
{
    internal abstract partial class PkcsPal
    {
        private static readonly PkcsPal s_instance = new PkcsPalOpenSsl();        
    }
}
