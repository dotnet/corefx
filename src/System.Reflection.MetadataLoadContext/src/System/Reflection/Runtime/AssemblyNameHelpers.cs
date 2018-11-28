// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Collections.Generic;

namespace System.Reflection
{
    internal static partial class AssemblyNameHelpers
    {
        //
        // These helpers convert between the combined flags+contentType+processorArchitecture value and the separated parts.
        //
        // Since these are only for trusted callers, they do NOT check for out of bound bits. 
        //

        internal static AssemblyContentType ExtractAssemblyContentType(this AssemblyNameFlags flags)
        {
            return (AssemblyContentType)((((int)flags) >> 9) & 0x7);
        }

        internal static ProcessorArchitecture ExtractProcessorArchitecture(this AssemblyNameFlags flags)
        {
            return (ProcessorArchitecture)((((int)flags) >> 4) & 0x7);
        }

        public static AssemblyNameFlags ExtractAssemblyNameFlags(this AssemblyNameFlags combinedFlags)
        {
            return combinedFlags & unchecked((AssemblyNameFlags)0xFFFFF10F);
        }

        internal static AssemblyNameFlags CombineAssemblyNameFlags(AssemblyNameFlags flags, AssemblyContentType contentType, ProcessorArchitecture processorArchitecture)
        {
            return (AssemblyNameFlags)(((int)flags) | (((int)contentType) << 9) | ((int)processorArchitecture << 4));
        }
    }
}
