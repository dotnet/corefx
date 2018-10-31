// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

using System.Collections.Generic;

namespace System.Reflection
{
    public abstract class MetadataAssemblyResolver
    {
        public abstract Assembly Resolve(MetadataLoadContext context, AssemblyName assemblyName = null);
    }

    public class PathAssemblyResolver : MetadataAssemblyResolver
    {
        public PathAssemblyResolver(IEnumerable<string> assemblyPaths) { }
        public override Assembly Resolve(MetadataLoadContext context, AssemblyName assemblyName) { throw null; }
    }

    public sealed partial class MetadataLoadContext : System.IDisposable
    {
        public MetadataLoadContext(MetadataAssemblyResolver resolver, string coreAssemblyName = null) { }
        public string CoreAssemblyName { get { throw null; } set { } }
        public void Dispose() { }
        public System.Collections.Generic.IEnumerable<System.Reflection.Assembly> GetAssemblies() { throw null; }
        public System.Reflection.Assembly LoadFromAssemblyName(System.Reflection.AssemblyName assemblyName) { throw null; }
        public System.Reflection.Assembly LoadFromAssemblyName(string assemblyName) { throw null; }
        public System.Reflection.Assembly LoadFromAssemblyPath(string assemblyPath) { throw null; }
        public System.Reflection.Assembly LoadFromByteArray(byte[] assembly) { throw null; }
        public System.Reflection.Assembly LoadFromStream(System.IO.Stream assembly) { throw null; }
    }
}
