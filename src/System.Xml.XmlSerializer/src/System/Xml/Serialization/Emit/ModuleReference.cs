// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal sealed class ModuleReference : Module
    {
        private readonly Reflection.Module _info;
        private readonly AssemblyReference _assembly;

        public ModuleReference(Reflection.Module info)
        {
            _info = info;
            _assembly = new AssemblyReference(info.Assembly);
        }

        public override Assembly Assembly
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
#endif