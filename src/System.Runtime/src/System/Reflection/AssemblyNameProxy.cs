// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Reflection {
    using System;
    using System.Runtime.Versioning;
    using System.Runtime.Loader;

    [System.Runtime.InteropServices.ComVisible(true)]
    public class AssemblyNameProxy : MarshalByRefObject
    {
        public AssemblyName GetAssemblyName(String assemblyFile)
        {
            return AssemblyLoadContext.GetAssemblyName(assemblyFile);
        }
    }
}
