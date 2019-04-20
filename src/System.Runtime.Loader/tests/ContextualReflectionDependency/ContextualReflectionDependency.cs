// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.Loader;
using System.Runtime.Remoting;

namespace System.Runtime.Loader.Tests
{
    public interface IContextualReflectionTestFixture
    {
        AssemblyLoadContext isolatedAlc { get; }
        Assembly isolatedAlcAssembly { get; }
        Type isolatedAlcFixtureType { get; }
        IContextualReflectionTestFixture isolatedAlcFixtureInstance { get; }

        AssemblyLoadContext defaultAlc { get; }
        Assembly defaultAlcAssembly { get; }
        Type defaultAlcFixtureType { get; }

        void SetPreConditions();
        void FixtureSetupAssertions();

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        Assembly GetExecutingAssembly();

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        Assembly AssemblyLoad(AssemblyName name);

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        Assembly AssemblyLoad(string name);

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        Assembly AssemblyLoadWithPartialName(string name);

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        Type TypeGetType(string typeName);

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        Type TypeGetType(string typeName, bool throwOnError);

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        Type TypeGetType(string typeName, bool throwOnError, bool ignoreCase);

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        Type TypeGetType(string typeName, Func<AssemblyName,Assembly> assemblyResolver, Func<Assembly, string, Boolean, Type> typeResolver);

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        Type TypeGetType(string typeName, Func<AssemblyName,Assembly> assemblyResolver, Func<Assembly, string, Boolean, Type> typeResolver, bool throwOnError);

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        Type TypeGetType(string typeName, Func<AssemblyName,Assembly> assemblyResolver, Func<Assembly, string, Boolean, Type> typeResolver, bool throwOnError, bool ignoreCase);

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        Type AssemblyGetType(Assembly assembly, string typeName);

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        Type AssemblyGetType(Assembly assembly, string typeName, bool throwOnError);

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        Type AssemblyGetType(Assembly assembly, string typeName, bool throwOnError, bool ignoreCase);

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        ObjectHandle ActivatorCreateInstance(string assemblyName, string typeName);

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        ObjectHandle ActivatorCreateInstance(string assemblyName, string typeName, object[] activationAttributes);

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        ObjectHandle ActivatorCreateInstance(string assemblyName, string typeName, bool ignoreCase, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, object[] args, System.Globalization.CultureInfo culture, object[] activationAttributes);
    }
}
