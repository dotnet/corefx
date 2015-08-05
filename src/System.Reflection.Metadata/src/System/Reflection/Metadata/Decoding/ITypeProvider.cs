// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace System.Reflection.Metadata.Decoding
{
    public interface ITypeProvider<TType>
    {
        TType GetGenericInstance(TType genericType, ImmutableArray<TType> typeArguments);
        TType GetArrayType(TType elementType, ArrayShape shape);
        TType GetByReferenceType(TType elementType);
        TType GetSZArrayType(TType elementType);
        TType GetPointerType(TType elementType);
    }
}
