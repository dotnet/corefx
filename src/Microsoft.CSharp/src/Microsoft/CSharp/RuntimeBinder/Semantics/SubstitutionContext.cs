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
        public readonly int ctypeCls;
        public readonly CType[] prgtypeMeth;
        public readonly int ctypeMeth;
        public readonly SubstTypeFlags grfst;

        public SubstContext(TypeArray typeArgsCls, TypeArray typeArgsMeth, SubstTypeFlags grfst)
        {
            if (typeArgsCls != null)
            {
                typeArgsCls.AssertValid();
                ctypeCls = typeArgsCls.Count;
                prgtypeCls = typeArgsCls.Items;
            }
            else
            {
                ctypeCls = 0;
                prgtypeCls = null;
            }

            if (typeArgsMeth != null)
            {
                typeArgsMeth.AssertValid();
                ctypeMeth = typeArgsMeth.Count;
                prgtypeMeth = typeArgsMeth.Items;
            }
            else
            {
                ctypeMeth = 0;
                prgtypeMeth = null;
            }

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

        public bool FNop()
        {
            return 0 == ctypeCls && 0 == ctypeMeth;
        }
    }
}
