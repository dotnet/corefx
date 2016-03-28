// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal class ModuleBuilder : Module
    {
        private readonly AssemblyBuilder _containingAssembly;
        private readonly List<TypeBuilder> _types;

        public ModuleBuilder(AssemblyBuilder containingAssembly, string name)
        {
            _containingAssembly = containingAssembly;
            _types = new List<TypeBuilder>();
        }

        public override Assembly Assembly => _containingAssembly;

        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, Type[] interfaces)
        {
            var type = new TypeBuilder(this, name, attr, parent, interfaces);
            _types.Add(type);
            return type;
        }
    }
}
#endif