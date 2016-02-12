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
        [CLSCompliant(false)] // out byte* blob
        public unsafe static bool TryGetRawMetadata(this System.Reflection.Assembly assembly, out byte* blob, out int length) { blob = default(byte*); length = default(int); return default(bool); }
    }
}
namespace System.Runtime.Loader
{
    public abstract partial class AssemblyLoadContext
    {
        protected AssemblyLoadContext() { }
        public static System.Runtime.Loader.AssemblyLoadContext Default { get { return default(System.Runtime.Loader.AssemblyLoadContext); } }
        public static System.Reflection.AssemblyName GetAssemblyName(string assemblyPath) { return default(System.Reflection.AssemblyName); }
        public static System.Runtime.Loader.AssemblyLoadContext GetLoadContext(System.Reflection.Assembly assembly) { return default(System.Runtime.Loader.AssemblyLoadContext); }
        public static void InitializeDefaultContext(System.Runtime.Loader.AssemblyLoadContext context) { }
        protected abstract System.Reflection.Assembly Load(System.Reflection.AssemblyName assemblyName);
        public System.Reflection.Assembly LoadFromAssemblyName(System.Reflection.AssemblyName assemblyName) { return default(System.Reflection.Assembly); }
        public System.Reflection.Assembly LoadFromAssemblyPath(string assemblyPath) { return default(System.Reflection.Assembly); }
        public System.Reflection.Assembly LoadFromNativeImagePath(string nativeImagePath, string assemblyPath) { return default(System.Reflection.Assembly); }
        public System.Reflection.Assembly LoadFromStream(System.IO.Stream assembly) { return default(System.Reflection.Assembly); }
        public System.Reflection.Assembly LoadFromStream(System.IO.Stream assembly, System.IO.Stream assemblySymbols) { return default(System.Reflection.Assembly); }
        protected System.IntPtr LoadUnmanagedDllFromPath(string unmanagedDllPath) { return default(System.IntPtr); }
        protected virtual System.IntPtr LoadUnmanagedDll(string unmanagedDllName) { return default(System.IntPtr); }
        public void SetProfileOptimizationRoot(string directoryPath) { }
        public void StartProfileOptimization(string profile) { }
        public event Func<AssemblyLoadContext, System.Reflection.AssemblyName, System.Reflection.Assembly> Resolving;
        public event Action<AssemblyLoadContext> Unloading;
    }
}
