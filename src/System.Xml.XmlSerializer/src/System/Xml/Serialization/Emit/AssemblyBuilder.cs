// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AssemblyBuilderAccess = System.Reflection.Emit.AssemblyBuilderAccess;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal class AssemblyBuilder : Assembly
    {
        private readonly AssemblyName _name;

        private List<CustomAttributeBuilder> _lazyCustomAttributes;
        private ModuleBuilder _lazyModuleBuilder;

        private AssemblyBuilder(AssemblyName name)
        {
            _name = name;
        }

        public static AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access)
        {
            return new AssemblyBuilder(name);
        }

        public ModuleBuilder DefineDynamicModule(string name)
        {
            if (_lazyModuleBuilder != null)
            {
                throw new InvalidOperationException();
            }

            return _lazyModuleBuilder = new ModuleBuilder(this, name);
        }

        public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
        {
            if (_lazyCustomAttributes == null)
            {
                _lazyCustomAttributes = new List<CustomAttributeBuilder>();
            }

            _lazyCustomAttributes.Add(customBuilder);
        }

        public void Emit(Stream peImage)
        {
            // TODO:
            throw new NotImplementedException();
        }
    }
}
#endif