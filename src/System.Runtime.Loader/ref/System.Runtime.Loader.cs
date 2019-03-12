// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Reflection.Metadata
{
    public static partial class AssemblyExtensions
    {
        [System.CLSCompliantAttribute(false)]
        public unsafe static bool TryGetRawMetadata(this System.Reflection.Assembly assembly, out byte* blob, out int length) { throw null; }
    }
}
namespace System.Runtime.Loader
{
    public sealed partial class AssemblyDependencyResolver
    {
        public AssemblyDependencyResolver(string componentAssemblyPath) { }
        public string ResolveAssemblyToPath(System.Reflection.AssemblyName assemblyName) { throw null; }
        public string ResolveUnmanagedDllToPath(string unmanagedDllName) { throw null; }
    }
    public abstract partial class AssemblyLoadContext
    {
        protected AssemblyLoadContext() { }
        protected AssemblyLoadContext(bool isCollectible) { }
        public static System.Runtime.Loader.AssemblyLoadContext Default { get { throw null; } }
        public bool IsCollectible { get { throw null; } }
        public event System.Func<System.Runtime.Loader.AssemblyLoadContext, System.Reflection.AssemblyName, System.Reflection.Assembly> Resolving { add { } remove { } }
        public event System.Func<System.Reflection.Assembly, string, System.IntPtr> ResolvingUnmanagedDll { add { } remove { } }
        public event System.Action<System.Runtime.Loader.AssemblyLoadContext> Unloading { add { } remove { } }
        public static System.Reflection.AssemblyName GetAssemblyName(string assemblyPath) { throw null; }
        public static System.Runtime.Loader.AssemblyLoadContext GetLoadContext(System.Reflection.Assembly assembly) { throw null; }
        protected abstract System.Reflection.Assembly Load(System.Reflection.AssemblyName assemblyName);
        public System.Reflection.Assembly LoadFromAssemblyName(System.Reflection.AssemblyName assemblyName) { throw null; }
        public System.Reflection.Assembly LoadFromAssemblyPath(string assemblyPath) { throw null; }
        public System.Reflection.Assembly LoadFromNativeImagePath(string nativeImagePath, string assemblyPath) { throw null; }
        public System.Reflection.Assembly LoadFromStream(System.IO.Stream assembly) { throw null; }
        public System.Reflection.Assembly LoadFromStream(System.IO.Stream assembly, System.IO.Stream assemblySymbols) { throw null; }
        protected virtual System.IntPtr LoadUnmanagedDll(string unmanagedDllName) { throw null; }
        protected System.IntPtr LoadUnmanagedDllFromPath(string unmanagedDllPath) { throw null; }
        public void SetProfileOptimizationRoot(string directoryPath) { }
        public void StartProfileOptimization(string profile) { }
        public void Unload() { }
    }
}
