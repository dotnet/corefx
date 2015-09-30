// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;

namespace System.Reflection.Metadata.Decoding
{
    public interface ISignatureTypeProvider<TType> : ITypeProvider<TType>
    {
        TType GetFunctionPointerType(MethodSignature<TType> signature);
        TType GetGenericMethodParameter(int index);
        TType GetGenericTypeParameter(int index);
        TType GetModifiedType(TType unmodifiedType, ImmutableArray<CustomModifier<TType>> customModifiers);
        TType GetPinnedType(TType elementType);
        TType GetPrimitiveType(PrimitiveTypeCode typeCode);
        TType GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, bool? isValueType);
        TType GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, bool? isValueType);
    }
}
