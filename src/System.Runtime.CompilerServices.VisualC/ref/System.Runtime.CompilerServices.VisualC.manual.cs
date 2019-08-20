// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly:System.Runtime.CompilerServices.InternalsVisibleTo("mscorlib, PublicKey=00000000000000000400000000000000")]

namespace System.Runtime.CompilerServices
{
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    internal sealed class AssemblyAttributesGoHere
    {
        internal AssemblyAttributesGoHere()
        {
        }
    }
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    internal sealed class AssemblyAttributesGoHereS
    {
        internal AssemblyAttributesGoHereS()
        {
        }
    }
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    internal sealed class AssemblyAttributesGoHereM
    {
        internal AssemblyAttributesGoHereM()
        {
        }
    }
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    internal sealed class AssemblyAttributesGoHereSM
    {
        internal AssemblyAttributesGoHereSM()
        {
        }
    }
    [System.AttributeUsage(System.AttributeTargets.All)]
    internal sealed class DecoratedNameAttribute : System.Attribute
    {
        public DecoratedNameAttribute(string decoratedName) {}
    }
    [AttributeUsage(AttributeTargets.Class |
                    AttributeTargets.Constructor |
                    AttributeTargets.Method |
                    AttributeTargets.Field |
                    AttributeTargets.Event |
                    AttributeTargets.Property)]
    [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    internal sealed class SuppressMergeCheckAttribute : Attribute
    {
        public SuppressMergeCheckAttribute()
        {}
    }
}
