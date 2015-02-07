// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Diagnostics;

namespace System.Reflection.Metadata.Decoding
{
    /// <summary>
    /// Decodes signature blobs.
    /// </summary>
    public static class SignatureDecoder
    {
        /// <summary>
        /// Decodes a type definition, reference or specification to its representation as TType.
        /// </summary>
        /// <param name="handle">A type definition, reference, or specification handle.</param>
        /// <param name="provider">The type provider.</param>
        /// <exception cref="System.BadImageFormatException">The handle does not represent a valid type reference, definition, or specification.</exception>
        public static TType DecodeType<TType>(Handle handle, ISignatureTypeProvider<TType> provider)
        {
            switch (handle.Kind)
            {
                case HandleKind.TypeReference:
                    return provider.GetTypeFromReference((TypeReferenceHandle)handle);

                case HandleKind.TypeDefinition:
                    return provider.GetTypeFromDefinition((TypeDefinitionHandle)handle);

                case HandleKind.TypeSpecification:
                    return DecodeTypeSpecification((TypeSpecificationHandle)handle, provider);

                default:
                    throw new BadImageFormatException();
            }
        }

        /// <summary>
        /// Decodes a type specification.
        /// </summary>
        /// <param name="handle">The type specification handle.</param>
        /// <param name="provider">The type provider.</param>
        /// <returns>The decoded type.</returns>
        /// <exception cref="System.BadImageFormatException">The type specification has an invalid signature.</exception>
        private static TType DecodeTypeSpecification<TType>(TypeSpecificationHandle handle, ISignatureTypeProvider<TType> provider)
        {
            BlobHandle blobHandle = provider.Reader.GetTypeSpecification(handle).Signature;
            BlobReader blobReader = provider.Reader.GetBlobReader(blobHandle);
            return DecodeType(ref blobReader, provider);
        }

        /// <summary>
        /// Decodes a type from within a signature from a BlobReader positioned at its leading SignatureTypeCode.
        /// </summary>
        /// <param name="blobReader">The blob reader.</param>
        /// <param name="provider">The type provider.</param>
        /// <returns>The decoded type.</returns>
        /// <exception cref="System.BadImageFormatException">The reader was not positioned at a valid signature type.</exception>
        private static TType DecodeType<TType>(ref BlobReader blobReader, ISignatureTypeProvider<TType> provider)
        {
            return DecodeType(ref blobReader, blobReader.ReadSignatureTypeCode(), provider);
        }

        /// <summary>
        /// Decodes a type from within a signature from a BlobReader positioned immediately past the given SignatureTypeCode.
        /// </summary>
        /// <param name="blobReader">The blob reader.</param>
        /// <param name="typeCode">The SignatureTypeCode that immediately preceded the reader's current position.</param>
        /// <param name="provider">The type provider.</param>
        /// <returns>The decoded type.</returns>
        /// <exception cref="System.BadImageFormatException">The reader was not positioned at a valud signature type.</exception>
        private static TType DecodeType<TType>(ref BlobReader blobReader, SignatureTypeCode typeCode, ISignatureTypeProvider<TType> provider)
        {
            TType elementType;
            int index;

            switch (typeCode)
            {
                case SignatureTypeCode.Boolean:
                case SignatureTypeCode.Char:
                case SignatureTypeCode.SByte:
                case SignatureTypeCode.Byte:
                case SignatureTypeCode.Int16:
                case SignatureTypeCode.UInt16:
                case SignatureTypeCode.Int32:
                case SignatureTypeCode.UInt32:
                case SignatureTypeCode.Int64:
                case SignatureTypeCode.UInt64:
                case SignatureTypeCode.Single:
                case SignatureTypeCode.Double:
                case SignatureTypeCode.IntPtr:
                case SignatureTypeCode.UIntPtr:
                case SignatureTypeCode.Object:
                case SignatureTypeCode.String:
                case SignatureTypeCode.Void:
                case SignatureTypeCode.TypedReference:
                    return provider.GetPrimitiveType((PrimitiveTypeCode)typeCode);

                case SignatureTypeCode.Pointer:
                    elementType = DecodeType(ref blobReader, provider);
                    return provider.GetPointerType(elementType);

                case SignatureTypeCode.ByReference:
                    elementType = DecodeType(ref blobReader, provider);
                    return provider.GetByReferenceType(elementType);

                case SignatureTypeCode.Pinned:
                    elementType = DecodeType(ref blobReader, provider);
                    return provider.GetPinnedType(elementType);

                case SignatureTypeCode.SZArray:
                    elementType = DecodeType(ref blobReader, provider);
                    return provider.GetSZArrayType(elementType);

                case SignatureTypeCode.FunctionPointer:
                    MethodSignature<TType> methodSignature = DecodeMethodSignature(ref blobReader, provider);
                    return provider.GetFunctionPointerType(methodSignature);

                case SignatureTypeCode.Array:
                    return DecodeArrayType(ref blobReader, provider);

                case SignatureTypeCode.RequiredModifier:
                    return DecodeModifiedType(ref blobReader, provider, isRequired: true);

                case SignatureTypeCode.OptionalModifier:
                    return DecodeModifiedType(ref blobReader, provider, isRequired: false);

                case SignatureTypeCode.TypeHandle:
                    return DecodeTypeHandle(ref blobReader, provider);

                case SignatureTypeCode.GenericTypeInstance:
                    return DecodeGenericTypeInstance(ref blobReader, provider);

                case SignatureTypeCode.GenericTypeParameter:
                    index = blobReader.ReadCompressedInteger();
                    return provider.GetGenericTypeParameter(index);

                case SignatureTypeCode.GenericMethodParameter:
                    index = blobReader.ReadCompressedInteger();
                    return provider.GetGenericMethodParameter(index);

                default:
                    throw new BadImageFormatException();
            }
        }

        // Decodes a list of types preceded by their count as a compressed integer.
        private static ImmutableArray<TType> DecodeTypes<TType>(ref BlobReader blobReader, ISignatureTypeProvider<TType> provider)
        {
            int count = blobReader.ReadCompressedInteger();
            if (count == 0)
            {
                return ImmutableArray<TType>.Empty;
            }

            var types = new TType[count];

            for (int i = 0; i < count; i++)
            {
                types[i] = DecodeType(ref blobReader, provider);
            }

            return ImmutableArray.Create(types);
        }

        /// <summary>
        /// Decodes a method signature blob.
        /// </summary>
        /// <param name="handle">Handle to the method signature.</param>
        /// <returns>The decoded method signature.</returns>
        /// <param name="provider">The type provider.</param>
        /// <exception cref="System.BadImageFormatException">The method signature is invalid.</exception>
        public static MethodSignature<TType> DecodeMethodSignature<TType>(BlobHandle handle, ISignatureTypeProvider<TType> provider)
        {
            BlobReader blobReader = provider.Reader.GetBlobReader(handle);
            return DecodeMethodSignature(ref blobReader, provider);
        }

        /// <summary>
        /// Decodes a method signature blob.
        /// </summary>
        /// <param name="blobReader">BlobReader positioned at a method signature.</param>
        /// <param name="provider">The type provider.</param>
        /// <returns>The decoded method signature.</returns>
        /// <exception cref="System.BadImageFormatException">The method signature is invalid.</exception>
        private static MethodSignature<TType> DecodeMethodSignature<TType>(ref BlobReader blobReader, ISignatureTypeProvider<TType> provider)
        {
            SignatureHeader header = blobReader.ReadSignatureHeader();

            if (header.Kind != SignatureKind.Method && header.Kind != SignatureKind.Property)
            {
                throw new BadImageFormatException();
            }

            int genericParameterCount = 0;
            if (header.IsGeneric)
            {
                genericParameterCount = blobReader.ReadCompressedInteger();
            }

            int parameterCount = blobReader.ReadCompressedInteger();
            TType returnType = DecodeType(ref blobReader, provider);

            if (parameterCount == 0)
            {
                return new MethodSignature<TType>(header, returnType, 0, genericParameterCount, ImmutableArray<TType>.Empty);
            }

            var parameterTypes = new TType[parameterCount];
            SignatureTypeCode typeCode;
            int parameterIndex;

            for (parameterIndex = 0; parameterIndex < parameterCount; parameterIndex++)
            {
                typeCode = blobReader.ReadSignatureTypeCode();

                if (typeCode == SignatureTypeCode.Sentinel)
                {
                    break;
                }

                parameterTypes[parameterIndex] = DecodeType(ref blobReader, typeCode, provider);
            }

            int requiredParameterCount = parameterIndex;

            for (; parameterIndex < parameterCount; parameterIndex++)
            {
                parameterTypes[parameterIndex] = DecodeType(ref blobReader, provider);
            }

            return new MethodSignature<TType>(header, returnType, requiredParameterCount, genericParameterCount, ImmutableArray.Create(parameterTypes));
        }

        /// <summary>
        /// Decodes a method specification signature blob.
        /// </summary>
        /// <param name="handle">The handle to the method specification signature blob. See <see cref="MethodSpecification.Signature"/>.</param>
        /// <param name="provider">The type provider.</param>
        /// <returns>The types used to instantiate a generic method via a method specification.</returns>
        /// <exception cref="System.BadImageFormatException">The method specification signature is invalid.</exception>
        public static ImmutableArray<TType> DecodeMethodSpecificationSignature<TType>(BlobHandle handle, ISignatureTypeProvider<TType> provider)
        {
            BlobReader blobReader = provider.Reader.GetBlobReader(handle);
            return DecodeMethodSpecificationSignature(ref blobReader, provider);
        }

        /// <summary>
        /// Decodes a method specification signature blob.
        /// </summary>
        /// <param name="blobReader">A BlobReader positioned at a valid method specification signature.</param>
        /// <param name="provider">The type provider.</param>
        /// <returns>The types used to instantiate a generic method via the method specification.</returns>
        public static ImmutableArray<TType> DecodeMethodSpecificationSignature<TType>(ref BlobReader blobReader, ISignatureTypeProvider<TType> provider)
        {
            SignatureHeader header = blobReader.ReadSignatureHeader();
            if (header.RawValue != (byte)SignatureAttributes.Generic)
            {
                throw new BadImageFormatException();
            }

            return DecodeTypes(ref blobReader, provider);
        }

        /// <summary>
        /// Decodes a local variable signature blob.
        /// </summary>
        /// <param name="handle">The local variable signature handle.</param>
        /// <param name="provider">The type provider.</param>
        /// <returns>The local variable types.</returns>
        /// <exception cref="System.BadImageFormatException">The local variable signature is invalid.</exception>
        public static ImmutableArray<TType> DecodeLocalSignature<TType>(StandaloneSignatureHandle handle, ISignatureTypeProvider<TType> provider)
        {
            BlobHandle blobHandle = provider.Reader.GetStandaloneSignature(handle).Signature;
            BlobReader blobReader = provider.Reader.GetBlobReader(blobHandle);
            return DecodeLocalSignature(ref blobReader, provider);
        }

        /// <summary>
        /// Decodes a local variable signature blob and advances the reader past the signature.
        /// </summary>
        /// <param name="blobReader">The blob reader.</param>
        /// <param name="provider">The type provider.</param>
        /// <returns>The local variable types.</returns>
        /// <exception cref="System.BadImageFormatException">The local variable signature is invalid.</exception>
        public static ImmutableArray<TType> DecodeLocalSignature<TType>(ref BlobReader blobReader, ISignatureTypeProvider<TType> provider)
        {
            SignatureHeader header = blobReader.ReadSignatureHeader();
            if (header.Kind != SignatureKind.LocalVariables)
            {
                throw new BadImageFormatException();
            }

            return DecodeTypes(ref blobReader, provider);
        }

        /// <summary>
        /// Decodes a field signature.
        /// </summary>
        /// <param name="handle">The field signature handle.</param>
        /// <param name="provider">The type provider.</param>
        /// <returns>The decoded field type.</returns>
        /// <exception cref="System.BadImageFormatException">The field signature is invalid.</exception>
        public static TType DecodeFieldSignature<TType>(BlobHandle handle, ISignatureTypeProvider<TType> provider)
        {
            BlobReader blobReader = provider.Reader.GetBlobReader(handle);
            return DecodeFieldSignature(ref blobReader, provider);
        }

        /// <summary>
        /// Decodes a field signature.
        /// </summary>
        /// <returns>The decoded field type.</returns>
        public static TType DecodeFieldSignature<TType>(ref BlobReader blobReader, ISignatureTypeProvider<TType> provider)
        {
            SignatureHeader header = blobReader.ReadSignatureHeader();

            if (header.Kind != SignatureKind.Field)
            {
                throw new BadImageFormatException();
            }

            return DecodeType(ref blobReader, provider);
        }

        // Decodes a generalized (non-SZ/vector) array type represented by the element type followed by
        // its rank and optional sizes and lower bounds.
        private static TType DecodeArrayType<TType>(ref BlobReader blobReader, ISignatureTypeProvider<TType> provider)
        {
            TType elementType = DecodeType(ref blobReader, provider);
            int rank = blobReader.ReadCompressedInteger();
            var sizes = ImmutableArray<int>.Empty;
            var lowerBounds = ImmutableArray<int>.Empty;

            int sizesCount = blobReader.ReadCompressedInteger();
            if (sizesCount > 0)
            {
                var array = new int[sizesCount];
                for (int i = 0; i < sizesCount; i++)
                {
                    array[i] = blobReader.ReadCompressedInteger();
                }
                sizes = ImmutableArray.Create(array);
            }

            int lowerBoundsCount = blobReader.ReadCompressedInteger();
            if (lowerBoundsCount > 0)
            {
                var array = new int[lowerBoundsCount];
                for (int i = 0; i < lowerBoundsCount; i++)
                {
                    array[i] = blobReader.ReadCompressedSignedInteger();
                }
                lowerBounds = ImmutableArray.Create(array);
            }

            var arrayShape = new ArrayShape(rank, sizes, lowerBounds);
            return provider.GetArrayType(elementType, arrayShape);
        }

        // Decodes a generic type instantiation encoded as the generic type followed by the types used to instantiate it.
        private static TType DecodeGenericTypeInstance<TType>(ref BlobReader blobReader, ISignatureTypeProvider<TType> provider)
        {
            TType genericType = DecodeType(ref blobReader, provider);
            ImmutableArray<TType> types = DecodeTypes(ref blobReader, provider);
            return provider.GetGenericInstance(genericType, types);
        }

        // Decodes a type with custom modifiers starting with the first modifier type that is required iff isRequired is passed,\
        // followed by an optional sequence of additional modifiers (<SignaureTypeCode.Required|OptionalModifier> <type>) and 
        // terminated by the unmodified type.
        private static TType DecodeModifiedType<TType>(ref BlobReader blobReader, ISignatureTypeProvider<TType> provider, bool isRequired)
        {
            TType type = DecodeTypeHandle(ref blobReader, provider);
            var modifier = new CustomModifier<TType>(type, isRequired);

            ImmutableArray<CustomModifier<TType>> modifiers;
            SignatureTypeCode typeCode = blobReader.ReadSignatureTypeCode();

            isRequired = typeCode == SignatureTypeCode.RequiredModifier;
            if (!isRequired && typeCode != SignatureTypeCode.OptionalModifier)
            {
                // common case: 1 modifier.
                modifiers = ImmutableArray.Create(modifier);
            }
            else
            {
                // uncommon case: multiple modifiers.
                var builder = ImmutableArray.CreateBuilder<CustomModifier<TType>>();
                builder.Add(modifier);

                do
                {
                    type = DecodeType(ref blobReader, typeCode, provider);
                    modifier = new CustomModifier<TType>(type, isRequired);
                    builder.Add(modifier);
                    typeCode = blobReader.ReadSignatureTypeCode();
                    isRequired = typeCode == SignatureTypeCode.RequiredModifier;
                } while (isRequired || typeCode == SignatureTypeCode.OptionalModifier);

                modifiers = builder.ToImmutable();
            }

            TType unmodifiedType = DecodeType(ref blobReader, typeCode, provider);
            return provider.GetModifiedType(unmodifiedType, modifiers);
        }

        // Decodes a type definition, reference, or specification from the type handle at the given blob reader's current position.
        private static TType DecodeTypeHandle<TType>(ref BlobReader blobReader, ISignatureTypeProvider<TType> provider)
        {
            Handle handle = blobReader.ReadTypeHandle();
            return DecodeType(handle, provider);
        }
    }
}
