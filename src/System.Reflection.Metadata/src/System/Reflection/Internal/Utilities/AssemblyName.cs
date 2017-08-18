// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Reflection.Metadata
{
    internal static class AssemblyNameUtils
    {
        internal static void SetHashAlgorithm(this AssemblyName assemblyName, AssemblyHashAlgorithm assemblyHashAlgorithm)
        {
            var hashAlgorithmMappings = new Dictionary<AssemblyHashAlgorithm, Configuration.Assemblies.AssemblyHashAlgorithm>()
            {
                [AssemblyHashAlgorithm.MD5] = Configuration.Assemblies.AssemblyHashAlgorithm.MD5,
                [AssemblyHashAlgorithm.Sha1] = Configuration.Assemblies.AssemblyHashAlgorithm.SHA1,
                [AssemblyHashAlgorithm.Sha256] = Configuration.Assemblies.AssemblyHashAlgorithm.SHA256,
                [AssemblyHashAlgorithm.Sha384] = Configuration.Assemblies.AssemblyHashAlgorithm.SHA384,
                [AssemblyHashAlgorithm.Sha512] = Configuration.Assemblies.AssemblyHashAlgorithm.SHA512
            };

            foreach (var hashAlgorithmItem in hashAlgorithmMappings)
            {
                bool hasValue = (assemblyHashAlgorithm & hashAlgorithmItem.Key) != 0;

                if (hasValue)
                {
                    assemblyName.HashAlgorithm |= hashAlgorithmItem.Value;
                }
                else
                {
                    assemblyName.HashAlgorithm &= ~hashAlgorithmItem.Value;
                }
            }
        }

        internal static void SetFlags(this AssemblyName assemblyName, AssemblyFlags flags)
        {
            var flagMappings = new Dictionary<AssemblyFlags, AssemblyNameFlags>()
            {
                [AssemblyFlags.PublicKey] = AssemblyNameFlags.PublicKey,
                [AssemblyFlags.Retargetable] = AssemblyNameFlags.Retargetable,
                [AssemblyFlags.EnableJitCompileTracking] = AssemblyNameFlags.EnableJITcompileTracking
            };

            foreach (var flagItem in flagMappings)
            {
                bool flagExists = (flags & flagItem.Key) != 0;

                if (flagExists)
                {
                    assemblyName.Flags |= flagItem.Value;
                }
                else
                {
                    assemblyName.Flags &= ~flagItem.Value;
                }
            }

            SetJitCompileOptimizerEnabled(assemblyName, flags);

            // the following two flags have no match in AssemblyNameFlags
            bool hasContentTypeMask = (flags & AssemblyFlags.ContentTypeMask) != 0;
            bool hasWindowsRuntime = (flags & AssemblyFlags.WindowsRuntime) != 0;
        }

        private static void SetJitCompileOptimizerEnabled(AssemblyName assemblyName, AssemblyFlags flags)
        {
            // notice we are setting EnableJITcompileOptimizer from DisableJitCompileOptimizer. Logic is flipped
            bool jitCompileTrackingDisabled = (flags & AssemblyFlags.DisableJitCompileOptimizer) != 0;

            if (!jitCompileTrackingDisabled)
            {
                assemblyName.Flags |= AssemblyNameFlags.EnableJITcompileOptimizer;
            }
            else
            {
                assemblyName.Flags &= ~AssemblyNameFlags.EnableJITcompileOptimizer;
            }
        }
    }
}
