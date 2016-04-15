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
    internal sealed class AssemblyBuilder : Assembly
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

        [Obsolete("TODO", error: false)]
        public static AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, IEnumerable<CustomAttributeBuilder> assemblyAttributes)
        {
            throw new NotImplementedException();
        }

        public ModuleBuilder DefineDynamicModule(string name)
        {
            if (_lazyModuleBuilder != null)
            {
                throw new InvalidOperationException();
            }

            return _lazyModuleBuilder = new ModuleBuilder(this, name);
        }

        [Obsolete("TODO", error: false)]
        public override string FullName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override bool IsDynamic
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override Module ManifestModule
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override IEnumerable<TypeInfo> DefinedTypes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override IEnumerable<Module> Modules
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public ModuleBuilder GetDynamicModule(string name)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override ManifestResourceInfo GetManifestResourceInfo(string resourceName)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override string[] GetManifestResourceNames()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override Stream GetManifestResourceStream(string name)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
        {
            throw new NotImplementedException();
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