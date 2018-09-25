// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Reflection
{
    public sealed partial class TypeLoader : System.IDisposable
    {
        public TypeLoader() { }
        public TypeLoader(string coreAssemblyName) { }
        public string CoreAssemblyName { get { throw null; } set { } }
        public event System.Func<System.Reflection.TypeLoader, System.Reflection.AssemblyName, System.Reflection.Assembly> Resolving { add { } remove { } }
        public void Dispose() { }
        public System.Collections.Generic.IEnumerable<System.Reflection.Assembly> GetAssemblies() { throw null; }
        public System.Reflection.Assembly LoadFromAssemblyName(System.Reflection.AssemblyName assemblyName) { throw null; }
        public System.Reflection.Assembly LoadFromAssemblyName(string assemblyName) { throw null; }
        public System.Reflection.Assembly LoadFromAssemblyPath(string assemblyPath) { throw null; }
        public System.Reflection.Assembly LoadFromByteArray(byte[] assembly) { throw null; }
        public System.Reflection.Assembly LoadFromStream(System.IO.Stream assembly) { throw null; }
    }
}
