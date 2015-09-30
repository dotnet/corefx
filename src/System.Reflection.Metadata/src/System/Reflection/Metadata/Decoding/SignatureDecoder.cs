// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Metadata.Decoding
{
    /// <summary>
    /// Decodes signature blobs.
    /// </summary>
    public struct SignatureDecoder<TProvider, TType> where TProvider : ISignatureTypeProvider<TType>
    {
        private readonly TProvider _provider;
        private readonly SignatureDecoderOptions _options;

        public SignatureDecoder(TProvider provider)
            : this(provider, SignatureDecoderOptions.None)
        {

        }

        public SignatureDecoder(TProvider provider, SignatureDecoderOptions options)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            _provider = provider;
            _options = options;
        }

        /// <summary>
        /// Decodes a type definition, reference or specification to its representation as TType.
        /// </summary>
        /// <param name="metadataReader">The metadata reader.</param>
        /// <param name="handle">A type definition, reference, or specification handle.</param>
        /// <exception cref="System.BadImageFormatException">The handle does not represent a valid type reference, definition, or specification.</exception>
        public TType DecodeType(MetadataReader metadataReader, EntityHandle handle)
        {
            return DecodeType(metadataReader, handle, isValueType: null);
        }

        /// <summary>
        /// Decodes a type definition, reference or specification to its representation as TType.
        /// </summary>
        /// <param name="metadataReader">The metadata reader.</param>
        /// <param name="handle">A type definition, reference, or specification handle.</param>
        /// <param name="isValueType">Is the type a class or a value type. Null signifies that the current type signature does not have the prefix</param>
        /// <exception cref="System.BadImageFormatException">The handle does not represent a valid type reference, definition, or specification.</exception>
        public TType DecodeType(MetadataReader metadataReader, EntityHandle handle, bool? isValueType)
        {
            switch (handle.Kind)
            {
                case HandleKind.TypeReference:
                    return _provider.GetTypeFromReference(metadataReader, (TypeReferenceHandle)handle, isValueType);

                case HandleKind.TypeDefinition:
                    return _provider.GetTypeFromDefinition(metadataReader, (TypeDefinitionHandle)handle, isValueType);

                case HandleKind.TypeSpecification:
                    BlobHandle blobHandle = metadataReader.GetTypeSpecification((TypeSpecificationHandle)handle).Signature;
                    BlobReader blobReader = metadataReader.GetBlobReader(blobHandle);
                    return DecodeType(metadataReader, ref blobReader);

                default:
                    throw new BadImageFormatException();
            }
        }

        /// <summary>
        /// Decodes a type from within a signature from a BlobReader positioned at the leading SignatureTypeCode.
        /// </summary>
        /// <param name="metadataReader">The metadata reader.</param>
        /// <param name="blobReader">The blob reader positioned at the leading SignatureTypeCode</param>
        /// <returns>The decoded type.</returns>
        /// <exception cref="System.BadImageFormatException">The reader was not positioned at a valid signature type.</exception>
        public TType DecodeType(MetadataReader metadataReader, ref BlobReader blobReader)
        {
            return DecodeType(metadataReader, ref blobReader, blobReader.ReadCompressedInteger());
        }

        /// <summary>
        /// Decodes a type from within a signature from a BlobReader positioned immediately past the given SignatureTypeCode.
        /// </summary>
        /// <param name="metadataReader">The metadata reader.</param>
        /// <param name="blobReader">The blob reader.</param>
        /// <param name="typeCode">The SignatureTypeCode that immediately preceded the reader's current position.</param>
        /// <returns>The decoded type.</returns>
        /// <exception cref="System.BadImageFormatException">The reader was not positioned at a valud signature type.</exception>
        private TType DecodeType(MetadataReader metadataReader, ref BlobReader blobReader, int typeCode)
        {
            TType elementType;
            int index;

            switch (typeCode)
            {
                case (int)SignatureTypeCode.Boolean:
                case (int)SignatureTypeCode.Char:
                case (int)SignatureTypeCode.SByte:
                case (int)SignatureTypeCode.Byte:
                case (int)SignatureTypeCode.Int16:
                case (int)SignatureTypeCode.UInt16:
                case (int)SignatureTypeCode.Int32:
                case (int)SignatureTypeCode.UInt32:
                case (int)SignatureTypeCode.Int64:
                case (int)SignatureTypeCode.UInt64:
                case (int)SignatureTypeCode.Single:
                case (int)SignatureTypeCode.Double:
                case (int)SignatureTypeCode.IntPtr:
                case (int)SignatureTypeCode.UIntPtr:
                case (int)SignatureTypeCode.Object:
                case (int)SignatureTypeCode.String:
                case (int)SignatureTypeCode.Void:
                case (int)SignatureTypeCode.TypedReference:
                    return _provider.GetPrimitiveType((PrimitiveTypeCode)typeCode);

                case (int)SignatureTypeCode.Pointer:
                    elementType = DecodeType(metadataReader, ref blobReader);
                    return _provider.GetPointerType(elementType);

                case (int)SignatureTypeCode.ByReference:
                    elementType = DecodeType(metadataReader, ref blobReader);
                    return _provider.GetByReferenceType(elementType);

                case (int)SignatureTypeCode.Pinned:
                    elementType = DecodeType(metadataReader, ref blobReader);
                    return _provider.GetPinnedType(elementType);

                case (int)SignatureTypeCode.SZArray:
                    elementType = DecodeType(metadataReader, ref blobReader);
                    return _provider.GetSZArrayType(elementType);

                case (int)SignatureTypeCode.FunctionPointer:
                    MethodSignature<TType> methodSignature = DecodeMethodSignature(metadataReader, ref blobReader);
                    return _provider.GetFunctionPointerType(methodSignature);

                case (int)SignatureTypeCode.Array:
                    return DecodeArrayType(metadataReader, ref blobReader);

                case (int)SignatureTypeCode.RequiredModifier:
                    return DecodeModifiedType(metadataReader, ref blobReader, isRequired: true);

                case (int)SignatureTypeCode.OptionalModifier:
                    return DecodeModifiedType(metadataReader, ref blobReader, isRequired: false);

                case (int)SignatureTypeCode.GenericTypeInstance:
                    return DecodeGenericTypeInstance(metadataReader, ref blobReader);

                case (int)SignatureTypeCode.GenericTypeParameter:
                    index = blobReader.ReadCompressedInteger();
                    return _provider.GetGenericTypeParameter(index);

                case (int)SignatureTypeCode.GenericMethodParameter:
                    index = blobReader.ReadCompressedInteger();
                    return _provider.GetGenericMethodParameter(index);

                case (int)CorElementType.ELEMENT_TYPE_CLASS:
                    return DecodeTypeHandle(metadataReader, ref blobReader, isValueType: false);

                case (int)CorElementType.ELEMENT_TYPE_VALUETYPE:
                    return DecodeTypeHandle(metadataReader, ref blobReader, isValueType: true);

                default:
                    throw new BadImageFormatException();
            }
        }

        // Decodes a list of types preceded by their count as a compressed integer.
        private ImmutableArray<TType> DecodeTypes(MetadataReader metadataReader, ref BlobReader blobReader)
        {
            int count = blobReader.ReadCompressedInteger();
            if (count == 0)
            {
                return ImmutableArray<TType>.Empty;
            }

            var types = ImmutableArray.CreateBuilder<TType>(count);

            for (int i = 0; i < count; i++)
            {
                types.Add(DecodeType(metadataReader, ref blobReader));
            }

            return types.MoveToImmutable();
        }

        /// <summary>
        /// Decodes a method signature blob.
        /// </summary>
        /// <param name="metadataReader">The metadata reader.</param>
        /// <param name="handle">Handle to the method signature.</param>
        /// <returns>The decoded method signature.</returns>
        /// <exception cref="System.BadImageFormatException">The method signature is invalid.</exception>
        public MethodSignature<TType> DecodeMethodSignature(MetadataReader metadataReader, BlobHandle handle)
        {
            BlobReader blobReader = metadataReader.GetBlobReader(handle);
            return DecodeMethodSignature(metadataReader, ref blobReader);
        }

        /// <summary>
        /// Decodes a method signature blob.
        /// </summary>
        /// <param name="metadataReader">The metadata reader.</param>
        /// <param name="blobReader">BlobReader positioned at a method signature.</param>
        /// <returns>The decoded method signature.</returns>
        /// <exception cref="System.BadImageFormatException">The method signature is invalid.</exception>
        private MethodSignature<TType> DecodeMethodSignature(MetadataReader metadataReader, ref BlobReader blobReader)
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
            TType returnType = DecodeType(metadataReader, ref blobReader);

            if (parameterCount == 0)
            {
                return new MethodSignature<TType>(header, returnType, 0, genericParameterCount, ImmutableArray<TType>.Empty);
            }

            var parameterTypes = ImmutableArray.CreateBuilder<TType>(parameterCount);
            SignatureTypeCode typeCode;
            int parameterIndex;

            for (parameterIndex = 0; parameterIndex < parameterCount; parameterIndex++)
            {
                var reader = blobReader;
                typeCode = reader.ReadSignatureTypeCode();

                if (typeCode == SignatureTypeCode.Sentinel)
                {
                    break;
                }
                parameterTypes.Add(DecodeType(metadataReader, ref blobReader));
            }

            int requiredParameterCount = parameterIndex;

            for (; parameterIndex < parameterCount; parameterIndex++)
            {
                parameterTypes.Add(DecodeType(metadataReader, ref blobReader));
            }

            return new MethodSignature<TType>(header, returnType, requiredParameterCount, genericParameterCount, parameterTypes.MoveToImmutable());
        }

        /// <summary>
        /// Decodes a method specification signature blob.
        /// </summary>
        /// <param name="metadataReader">The metadata reader.</param>
        /// <param name="handle">The handle to the method specification signature blob. See <see cref="MethodSpecification.Signature"/>.</param>
        /// <returns>The types used to instantiate a generic method via a method specification.</returns>
        /// <exception cref="System.BadImageFormatException">The method specification signature is invalid.</exception>
        public ImmutableArray<TType> DecodeMethodSpecificationSignature(MetadataReader metadataReader, BlobHandle handle)
        {
            BlobReader blobReader = metadataReader.GetBlobReader(handle);
            return DecodeMethodSpecificationSignature(metadataReader, ref blobReader);
        }

        /// <summary>
        /// Decodes a method specification signature blob.
        /// </summary>
        /// <param name="metadataReader">The metadata reader.</param>
        /// <param name="blobReader">A BlobReader positioned at a valid method specification signature.</param>
        /// <returns>The types used to instantiate a generic method via the method specification.</returns>
        public ImmutableArray<TType> DecodeMethodSpecificationSignature(MetadataReader metadataReader, ref BlobReader blobReader)
        {
            SignatureHeader header = blobReader.ReadSignatureHeader();
            if (header.Kind != SignatureKind.MethodSpecification)
            {
                throw new BadImageFormatException();
            }

            return DecodeTypes(metadataReader, ref blobReader);
        }

        /// <summary>
        /// Decodes a local variable signature blob.
        /// </summary>
        /// <param name="metadataReader">The metadata reader.</param>
        /// <param name="handle">The local variable signature handle.</param>
        /// <returns>The local variable types.</returns>
        /// <exception cref="System.BadImageFormatException">The local variable signature is invalid.</exception>
        public ImmutableArray<TType> DecodeLocalSignature(MetadataReader metadataReader, StandaloneSignatureHandle handle)
        {
            BlobHandle blobHandle = metadataReader.GetStandaloneSignature(handle).Signature;
            BlobReader blobReader = metadataReader.GetBlobReader(blobHandle);
            return DecodeLocalSignature(metadataReader, ref blobReader);
        }

        /// <summary>
        /// Decodes a local variable signature blob and advances the reader past the signature.
        /// </summary>
        /// <param name="metadataReader">The metadata reader.</param>
        /// <param name="blobReader">The blob reader positioned at a local variable signature.</param>
        /// <returns>The local variable types.</returns>
        /// <exception cref="System.BadImageFormatException">The local variable signature is invalid.</exception>
        public ImmutableArray<TType> DecodeLocalSignature(MetadataReader metadataReader, ref BlobReader blobReader)
        {
            SignatureHeader header = blobReader.ReadSignatureHeader();
            if (header.Kind != SignatureKind.LocalVariables)
            {
                throw new BadImageFormatException();
            }

            return DecodeTypes(metadataReader, ref blobReader);
        }

        /// <summary>
        /// Decodes a field signature.
        /// </summary>
        /// <param name="metadataReader">The metadata reader.</param>
        /// <param name="handle">The field signature handle.</param>
        /// <returns>The decoded field type.</returns>
        /// <exception cref="System.BadImageFormatException">The field signature is invalid.</exception>
        public TType DecodeFieldSignature(MetadataReader metadataReader, BlobHandle handle)
        {
            BlobReader blobReader = metadataReader.GetBlobReader(handle);
            return DecodeFieldSignature(metadataReader, ref blobReader);
        }

        /// <summary>
        /// Decodes a field signature.
        /// </summary>
        /// <param name="metadataReader">The metadata reader.</param>
        /// <param name="blobReader">The blob reader positioned at a field signature.</param>
        /// <returns>The decoded field type.</returns>
        public TType DecodeFieldSignature(MetadataReader metadataReader, ref BlobReader blobReader)
        {
            SignatureHeader header = blobReader.ReadSignatureHeader();

            if (header.Kind != SignatureKind.Field)
            {
                throw new BadImageFormatException();
            }

            return DecodeType(metadataReader, ref blobReader);
        }

        // Decodes a generalized (non-SZ/vector) array type represented by the element type followed by
        // its rank and optional sizes and lower bounds.
        private TType DecodeArrayType(MetadataReader metadataReader, ref BlobReader blobReader)
        {
            TType elementType = DecodeType(metadataReader, ref blobReader);
            int rank = blobReader.ReadCompressedInteger();
            var sizes = ImmutableArray<int>.Empty;
            var lowerBounds = ImmutableArray<int>.Empty;

            int sizesCount = blobReader.ReadCompressedInteger();
            if (sizesCount > 0)
            {
                var builder = ImmutableArray.CreateBuilder<int>(sizesCount);
                for (int i = 0; i < sizesCount; i++)
                {
                    builder.Add(blobReader.ReadCompressedInteger());
                }
                sizes = builder.MoveToImmutable();
            }

            int lowerBoundsCount = blobReader.ReadCompressedInteger();
            if (lowerBoundsCount > 0)
            {
                var builder = ImmutableArray.CreateBuilder<int>(lowerBoundsCount);
                for (int i = 0; i < lowerBoundsCount; i++)
                {
                    builder.Add(blobReader.ReadCompressedSignedInteger());
                }
                lowerBounds = builder.MoveToImmutable();
            }

            var arrayShape = new ArrayShape(rank, sizes, lowerBounds);
            return _provider.GetArrayType(elementType, arrayShape);
        }

        // Decodes a generic type instantiation encoded as the generic type followed by the types used to instantiate it.
        private TType DecodeGenericTypeInstance(MetadataReader metadataReader, ref BlobReader blobReader)
        {
            TType genericType = DecodeType(metadataReader, ref blobReader);
            ImmutableArray<TType> types = DecodeTypes(metadataReader, ref blobReader);
            return _provider.GetGenericInstance(genericType, types);
        }

        // Decodes a type with custom modifiers starting with the first modifier type that is required iff isRequired is passed,
        // followed by an optional sequence of additional modifiers (<SignaureTypeCode.Required|OptionalModifier> <type>) and 
        // terminated by the unmodified type.
        private TType DecodeModifiedType(MetadataReader metadataReader, ref BlobReader blobReader, bool isRequired)
        {
            TType type = DecodeTypeHandle(metadataReader, ref blobReader, isValueType: null);
            var modifier = new CustomModifier<TType>(type, isRequired);

            ImmutableArray<CustomModifier<TType>> modifiers;
            int typeCode = blobReader.ReadCompressedInteger();

            isRequired = typeCode == (int)SignatureTypeCode.RequiredModifier;
            if (!isRequired && typeCode != (int)SignatureTypeCode.OptionalModifier)
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
                    type = DecodeTypeHandle(metadataReader, ref blobReader, isValueType: null);
                    modifier = new CustomModifier<TType>(type, isRequired);
                    builder.Add(modifier);
                    typeCode = blobReader.ReadCompressedInteger();
                    isRequired = typeCode == (int)SignatureTypeCode.RequiredModifier;
                } while (isRequired || typeCode == (int)SignatureTypeCode.OptionalModifier);

                modifiers = builder.ToImmutable();
            }
            TType unmodifiedType = DecodeType(metadataReader, ref blobReader, typeCode);
            return _provider.GetModifiedType(unmodifiedType, modifiers);
        }

        // Decodes a type definition, reference, or specification from the type handle at the given blob reader's current position.
        private TType DecodeTypeHandle(MetadataReader metadataReader, ref BlobReader blobReader, bool? isValueType)
        {
            EntityHandle handle = blobReader.ReadTypeHandle();
            return DecodeType(metadataReader, handle, isValueType);
        }
    }
}
