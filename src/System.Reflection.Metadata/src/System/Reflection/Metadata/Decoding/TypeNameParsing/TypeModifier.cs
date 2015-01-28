// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace System.Reflection.Metadata.Decoding
{
    internal struct TypeModifier
    {
        public readonly static TypeModifier SZArray = new TypeModifier(TypeModifierType.SZArray, new ArrayShape());
        public readonly static TypeModifier Pointer = new TypeModifier(TypeModifierType.Pointer, new ArrayShape());
        public readonly static TypeModifier Reference = new TypeModifier(TypeModifierType.ByReference, new ArrayShape());

        private readonly TypeModifierType _modifierType;
        private readonly ArrayShape _arrayShape;        
        
        private TypeModifier(TypeModifierType modifierType, ArrayShape arrayShape)
        {
            _modifierType = modifierType;
            _arrayShape = arrayShape;
        }

        public ArrayShape ArrayShape
        {
            get 
            { 
                Debug.Assert(_modifierType == TypeModifierType.Array);

                return _arrayShape;  
            }
        }

        public TypeModifierType ModifierType
        {
            get { return _modifierType; }
        }

        public static TypeModifier Array(int rank)
        {
            ArrayShape shape = new ArrayShape(rank, ImmutableArray<int>.Empty, ImmutableArray<int>.Empty);

            return new TypeModifier(TypeModifierType.Array, shape);
        }
    }
}
