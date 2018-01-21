// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    // Used to specify whether and which type variables should be normalized.
    [Flags]
    internal enum SubstTypeFlags
    {
        NormNone = 0x00,
        DenormMeth = 0x08,   // Replace normalized (standard) method type variables with the given method type args.
    }

    internal sealed class SubstContext
    {
        public readonly CType[] prgtypeCls;
        public readonly CType[] prgtypeMeth;
        public readonly SubstTypeFlags grfst;

        public SubstContext(TypeArray typeArgsCls, TypeArray typeArgsMeth, SubstTypeFlags grfst)
        {
            typeArgsCls?.AssertValid();
            prgtypeCls = typeArgsCls?.Items ?? Array.Empty<CType>();
            typeArgsMeth?.AssertValid();
            prgtypeMeth = typeArgsMeth?.Items ?? Array.Empty<CType>();
            this.grfst = grfst;
        }

        public SubstContext(AggregateType type)
            : this(type, null, SubstTypeFlags.NormNone)
        {
        }

        public SubstContext(AggregateType type, TypeArray typeArgsMeth)
            : this(type, typeArgsMeth, SubstTypeFlags.NormNone)
        {
        }

        private SubstContext(AggregateType type, TypeArray typeArgsMeth, SubstTypeFlags grfst)
            : this(type?.TypeArgsAll, typeArgsMeth, grfst)
        {
        }

        public bool IsNop => prgtypeCls.Length == 0 & prgtypeMeth.Length == 0;
    }
}
