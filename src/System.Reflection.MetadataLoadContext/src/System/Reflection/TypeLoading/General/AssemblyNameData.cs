// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

namespace System.Reflection.TypeLoading
{
    // This disambiguating "using" must be inside the "namespace" or else we'll pick up the wrong AssemblyHashAlgorithm.
    using AssemblyHashAlgorithm = global::System.Configuration.Assemblies.AssemblyHashAlgorithm;

    //
    // Collects together everything needed to generate an AssemblyName quickly. We don't want to do all the metadata analysis every time
    // GetName() is called.
    //
    internal sealed class AssemblyNameData
    {
        public AssemblyNameFlags Flags;
        public string Name;
        public Version Version;
        public string CultureName;
        public byte[] PublicKey;
        public byte[] PublicKeyToken;
        public AssemblyContentType ContentType;
        public AssemblyHashAlgorithm HashAlgorithm;
        public ProcessorArchitecture ProcessorArchitecture;

        // Creates a newly allocated AssemblyName that is safe to return out of an api.
        public AssemblyName CreateAssemblyName()
        {
            AssemblyName an = new AssemblyName();
            an.Flags = Flags;
            an.Name = Name;
            an.Version = Version;
            an.CultureName = CultureName;
            // Yes, *we* have to clone the array. AssemblyName.SetPublicKey() violates framework guidelines and doesn't make a copy.
            an.SetPublicKey(PublicKey.CloneArray());
            an.SetPublicKeyToken(PublicKeyToken.CloneArray());
            an.ContentType = ContentType;
            an.HashAlgorithm = HashAlgorithm;
            an.ProcessorArchitecture = ProcessorArchitecture;
            return an;
        }
    }
}
