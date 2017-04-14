// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal sealed class ExprMemberGroup : ExprWithType, IExprWithObject
    {
        public ExprMemberGroup(CType type)
            : base(ExpressionKind.MemberGroup, type)
        {
        }

        public Name Name { get; set; }

        public TypeArray TypeArgs { get; set; }

        public SYMKIND SymKind { get; set; }
        // The type containing the members. This may be a TypeParameterType or an AggregateType.
        // This may be NULL (if types is not NULL).

        // This is normally NULL - meaning the particular member
        // is unknown. For the invoke method on a delegate it is typically known.
        // After binding the group to arguments, this is set to the meth sym that is bound.
        // The object expression. NULL for a static invocation.
        public Expr OptionalObject { get; set; }

        // The lhs that was bound to resolve this invocation. Set for static invocations only.
        // We don't use the regular defines because we don't want to visit this guy in regular visitors,
        // just in the visitors that create the node maps for LAF and refactoring.
        public Expr OptionalLHS { get; set; }

        // The owning call of this member group. NEVER visit this thing.
        // The list of the methods that the memgroup is binding to. This list is formulated after binding
        // the name of the method. When we've attempted to bind the arguments, we populate the MethPropWithInst list.
        public CMemberLookupResults MemberLookupResults { get; set; }

        public CType ParentType { get; set; }

        public bool IsDelegate => (Flags & EXPRFLAG.EXF_DELEGATE) != 0;

        public void SetInaccessibleBit() => SetError();
    }
}
