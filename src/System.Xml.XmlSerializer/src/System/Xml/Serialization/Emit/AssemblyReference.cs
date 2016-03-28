// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal class AssemblyReference : Assembly
    {
        private readonly Reflection.Assembly _info;

        public AssemblyReference(Reflection.Assembly info)
        {
            _info = info;
        }
    }
}
#endif