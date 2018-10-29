// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;

namespace System.Reflection.Tests
{
    public class CoreAssemblyMetadataAssemblyResolver : MetadataAssemblyResolver
    {
        public CoreAssemblyMetadataAssemblyResolver()
        {
        }

        public override Assembly Resolve(System.Reflection.MetadataLoadContext context, AssemblyName assemblyName)
        {
            if (assemblyName.Name == "mscorlib")
            {
                if (_coreAssembly == null)
                {
                    _coreAssembly = context.LoadFromStream(TestUtils.CreateStreamForCoreAssembly());
                }

                return _coreAssembly;
            }

            return null;
        }

        private Assembly _coreAssembly;
    }
}
