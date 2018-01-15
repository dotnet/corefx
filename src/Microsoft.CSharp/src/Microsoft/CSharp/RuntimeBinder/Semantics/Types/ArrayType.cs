// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    // ----------------------------------------------------------------------------
    // ArrayType - a symbol representing an array.
    // ----------------------------------------------------------------------------

    internal sealed class ArrayType : CType
    {
        public ArrayType(CType elementType, int rank, bool isSZArray)
            : base(TypeKind.TK_ArrayType)
        {
            Rank = rank;
            IsSZArray = isSZArray;
            ElementType = elementType;
        }

        public int Rank { get; }

        public bool IsSZArray { get; }

        public CType ElementType { get; }

        // Returns the first non-array type in the parent chain.
        public CType BaseElementType
        {
            get
            {
                CType type = ElementType;
                while (type is ArrayType arr)
                {
                    type = arr.ElementType;
                }

                return type;
            }
        }
    }
}

