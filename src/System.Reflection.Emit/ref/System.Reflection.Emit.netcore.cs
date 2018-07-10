// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Reflection.Emit
{
    public sealed partial class MethodBuilder : System.Reflection.MethodInfo
    {
        public override bool IsConstructedGenericMethod { get { throw null; } }
    }
    public sealed partial class EnumBuilder : System.Type
    {
        public override bool IsByRefLike { get { throw null; } }
        public override bool IsTypeDefinition { get { throw null; } }
        public override bool IsSZArray { get { throw null; } }
        public override bool IsVariableBoundArray { get { throw null; } }
    }
    public sealed partial class GenericTypeParameterBuilder : System.Type
    {
        public override bool IsByRefLike { get { throw null; } }
        public override bool IsTypeDefinition { get { throw null; } }
        public override bool IsSZArray { get { throw null; } }
        public override bool IsVariableBoundArray { get { throw null; } }
    }
    public sealed partial class TypeBuilder : System.Type
    {
        public System.Type CreateType() { throw null; }
        public override bool IsByRefLike { get { throw null; } }
        public override bool IsTypeDefinition { get { throw null; } }
        public override bool IsSZArray { get { throw null; } }
        public override bool IsVariableBoundArray { get { throw null; } }
    }
}
