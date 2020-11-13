// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System
{
    internal static partial class LocalAppContextSwitches
    {
        private static int s_useNonRandomizedHashSeed;
        public static bool UseNonRandomizedHashSeed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => GetCachedSwitchValue("Switch.System.Data.UseNonRandomizedHashSeed", ref s_useNonRandomizedHashSeed);
        }
    }
}
