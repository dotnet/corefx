﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Metadata.Decoding
{
    /// <summary>
    /// Decodes signature blobs.
    /// See Metadata Specification section II.23.2: Blobs and signatures.
    /// </summary>
    public struct SignatureDecoder<TType>
    {
        private readonly ISignatureTypeProvider<TType> _provider;
        private readonly MetadataReader _metadataReaderOpt;
        private readonly SignatureDecoderOptions _options;

        /// <summary>
        /// Creates a new SignatureDecoder.
        /// </summary>
        /// <param name="provider">The provider used to obtain type symbols as the signature is decoded.</param>
        /// <param name="metadataReader">
        /// The metadata reader from which the signature was obtained. It may be null if the given provider allows it.
        /// However, if <see cref="SignatureDecoderOptions.DifferentiateClassAndValueTypes"/> is specified, it should
        /// be non-null to evaluate WinRT projections from class to value type or vice-versa correctly.
        /// </param>
        /// <param name="options">Set of optional decoder features to enable.</param>
        public SignatureDecoder(
            ISignatureTypeProvider<TType> provider, 
            MetadataReader metadataReader = null, 
            SignatureDecoderOptions options = SignatureDecoderOptions.None)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            _metadataReaderOpt = metadataReader;
            _provider = provider;
            _options = options;
        }

        /// <summary>
        /// Decodes a type embedded in a signature and advances the reader past the type.
        /// </summary>
        /// <param name="blobReader">The blob reader positioned at the leading SignatureTypeCode</param>
        /// <returns>The decoded type.</returns>
        /// <exception cref="System.BadImageFormatException">The reader was not positioned at a valid signature type.</exception>
        public TType DecodeType(ref BlobReader blobReader)
        {
            return DecodeType(ref blobReader, blobReader.ReadCompressedInteger());
        }

        private TType DecodeType(ref BlobReader blobReader, int typeCode)
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
                    elementType = DecodeType(ref blobReader);
                    return _provider.GetPointerType(elementType);

                case (int)SignatureTypeCode.ByReference:
                    elementType = DecodeType(ref blobReader);
                    return _provider.GetByReferenceType(elementType);

                case (int)SignatureTypeCode.Pinned:
                    elementType = DecodeType(ref blobReader);
                    return _provider.GetPinnedType(elementType);

                case (int)SignatureTypeCode.SZArray:
                    elementType = DecodeType(ref blobReader);
                    return _provider.GetSZArrayType(elementType);

                case (int)SignatureTypeCode.FunctionPointer:
                    MethodSignature<TType> methodSignature = DecodeMethodSignature(ref blobReader);
                    return _provider.GetFunctionPointerType(methodSignature);

                case (int)SignatureTypeCode.Array:
                    return DecodeArrayType(ref blobReader);

                case (int)SignatureTypeCode.RequiredModifier:
                    return DecodeModifiedType(ref blobReader, isRequired: true);

                case (int)SignatureTypeCode.OptionalModifier:
                    return DecodeModifiedType(ref blobReader, isRequired: false);

                case (int)SignatureTypeCode.GenericTypeInstance:
                    return DecodeGenericTypeInstance(ref blobReader);

                case (int)SignatureTypeCode.GenericTypeParameter:
                    index = blobReader.ReadCompressedInteger();
                    return _provider.GetGenericTypeParameter(index);

                case (int)SignatureTypeCode.GenericMethodParameter:
                    index = blobReader.ReadCompressedInteger();
                    return _provider.GetGenericMethodParameter(index);

                case (int)SignatureTypeHandleCode.Class:
                case (int)SignatureTypeHandleCode.ValueType:
                    return DecodeTypeDefOrRef(ref blobReader, (SignatureTypeHandleCode)typeCode);

                default:
                    throw new BadImageFormatException(SR.Format(SR.UnexpectedSignatureTypeCode, typeCode));
            }
        }

        /// <summary> 
        /// Decodes a list of types, with at least one instance that is preceded by its count as a compressed integer.
        /// </summary>
        private ImmutableArray<TType> DecodeTypeSequence(ref BlobReader blobReader)
        {
            int count = blobReader.ReadCompressedInteger();
            if (count == 0)
            {
                // This method is used for Local signatures and method specs, neither of which can have
                // 0 elements. Parameter sequences can have 0 elements, but they are handled separately
                // to deal with the sentinel/varargs case.
                throw new BadImageFormatException(SR.SignatureTypeSequenceMustHaveAtLeastOneElement);
            }

            var types = ImmutableArray.CreateBuilder<TType>(count);

            for (int i = 0; i < count; i++)
            {
                types.Add(DecodeType(ref blobReader));
            }

            return types.MoveToImmutable();
        }

        /// <summary>
        /// Decodes a method (definition, reference, or standalone) or property signature blob.
        /// </summary>
        /// <param name="blobReader">BlobReader positioned at a method signature.</param>
        /// <returns>The decoded method signature.</returns>
        /// <exception cref="System.BadImageFormatException">The method signature is invalid.</exception>
        public MethodSignature<TType> DecodeMethodSignature(ref BlobReader blobReader)
        {
            SignatureHeader header = blobReader.ReadSignatureHeader();
            CheckMethodOrPropertyHeader(header);

            int genericParameterCount = 0;
            if (header.IsGeneric)
            {
                genericParameterCount = blobReader.ReadCompressedInteger();
            }

            int parameterCount = blobReader.ReadCompressedInteger();
            TType returnType = DecodeType(ref blobReader);
            ImmutableArray<TType> parameterTypes;
            int requiredParameterCount;

            if (parameterCount == 0)
            {
                requiredParameterCount = 0;
                parameterTypes = ImmutableArray<TType>.Empty;
            }
            else
            {
                var parameterBuilder = ImmutableArray.CreateBuilder<TType>(parameterCount);
                int parameterIndex;

                for (parameterIndex = 0; parameterIndex < parameterCount; parameterIndex++)
                {
                    int typeCode = blobReader.ReadCompressedInteger();
                    if (typeCode == (int)SignatureTypeCode.Sentinel)
                    {
                        break;
                    }
                    parameterBuilder.Add(DecodeType(ref blobReader, typeCode));
                }

                requiredParameterCount = parameterIndex;
                for (; parameterIndex < parameterCount; parameterIndex++)
                {
                    parameterBuilder.Add(DecodeType(ref blobReader));
                }
                parameterTypes = parameterBuilder.MoveToImmutable();
            }

            return new MethodSignature<TType>(header, returnType, requiredParameterCount, genericParameterCount, parameterTypes);
        }

        /// <summary>
        /// Decodes a method specification signature blob and advances the reader past the signature.
        /// </summary>
        /// <param name="blobReader">A BlobReader positioned at a valid method specification signature.</param>
        /// <returns>The types used to instantiate a generic method via the method specification.</returns>
        public ImmutableArray<TType> DecodeMethodSpecificationSignature(ref BlobReader blobReader)
        {
            SignatureHeader header = blobReader.ReadSignatureHeader();
            CheckHeader(header, SignatureKind.MethodSpecification);
            return DecodeTypeSequence(ref blobReader);
        }

        /// <summary>
        /// Decodes a local variable signature blob and advances the reader past the signature.
        /// </summary>
        /// <param name="blobReader">The blob reader positioned at a local variable signature.</param>
        /// <returns>The local variable types.</returns>
        /// <exception cref="System.BadImageFormatException">The local variable signature is invalid.</exception>
        public ImmutableArray<TType> DecodeLocalSignature(ref BlobReader blobReader)
        {
            SignatureHeader header = blobReader.ReadSignatureHeader();
            CheckHeader(header, SignatureKind.LocalVariables);
            return DecodeTypeSequence(ref blobReader);
        }

        /// <summary>
        /// Decodes a field signature blob and advances the reader past the signature.
        /// </summary>
        /// <param name="blobReader">The blob reader positioned at a field signature.</param>
        /// <returns>The decoded field type.</returns>
        public TType DecodeFieldSignature(ref BlobReader blobReader)
        {
            SignatureHeader header = blobReader.ReadSignatureHeader();
            CheckHeader(header, SignatureKind.Field);
            return DecodeType(ref blobReader);
        }

        private TType DecodeArrayType(ref BlobReader blobReader)
        {
            // PERF_TODO: Cache/reuse common case of small number of all-zero lower-bounds.

            TType elementType = DecodeType(ref blobReader);
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

        private TType DecodeGenericTypeInstance(ref BlobReader blobReader)
        {
            TType genericType = DecodeType(ref blobReader);
            ImmutableArray<TType> types = DecodeTypeSequence(ref blobReader);
            return _provider.GetGenericInstance(genericType, types);
        }

        private TType DecodeModifiedType(ref BlobReader blobReader, bool isRequired)
        {
            EntityHandle modifier = blobReader.ReadTypeHandle();
            TType unmodifiedType = DecodeType(ref blobReader);
            return _provider.GetModifiedType(_metadataReaderOpt, isRequired, modifier, unmodifiedType);
        }

        private TType DecodeTypeDefOrRef(ref BlobReader blobReader, SignatureTypeHandleCode code)
        {
            // Force no differentiation of class vs. value type unless the option is enabled.
            // Avoids cost of WinRT projection.
            if ((_options & SignatureDecoderOptions.DifferentiateClassAndValueTypes) == 0)
            {
                code = SignatureTypeHandleCode.Unresolved; 
            }
 
            EntityHandle handle = blobReader.ReadTypeHandle();
            switch (handle.Kind)
            {
                case HandleKind.TypeDefinition:
                    var typeDef = (TypeDefinitionHandle)handle;
                    return _provider.GetTypeFromDefinition(_metadataReaderOpt, typeDef, code);

                case HandleKind.TypeReference:
                    var typeRef = (TypeReferenceHandle)handle;
                    if (code != SignatureTypeHandleCode.Unresolved)
                    {
                        ProjectClassOrValueType(typeRef, ref code);
                    }
                    return _provider.GetTypeFromReference(_metadataReaderOpt, typeRef, code);

                default:
                    // To prevent cycles, the token following (CLASS | VALUETYPE) must not be a type spec
                    // https://github.com/dotnet/coreclr/blob/8ff2389204d7c41b17eff0e9536267aea8d6496f/src/md/compiler/mdvalidator.cpp#L6154-L6160
                    throw new BadImageFormatException(SR.NotTypeDefOrRefHandle);
            }
        }

        private void ProjectClassOrValueType(TypeReferenceHandle handle, ref SignatureTypeHandleCode code)
        {
            Debug.Assert(code != SignatureTypeHandleCode.Unresolved);
            Debug.Assert((_options & SignatureDecoderOptions.DifferentiateClassAndValueTypes) != 0);

            if (_metadataReaderOpt == null)
            {
                // If we're asked to differentiate value types without a reader, then 
                // return the designation unprojected as it occurs in the signature blob.
                return; 
            }

            TypeReference typeRef = _metadataReaderOpt.GetTypeReference(handle);
            switch (typeRef.SignatureTreatment)
            {
                case TypeRefSignatureTreatment.ProjectedToClass:
                    code = SignatureTypeHandleCode.Class;
                    break;
                case TypeRefSignatureTreatment.ProjectedToValueType:
                    code = SignatureTypeHandleCode.ValueType;
                    break;
            }
        }

        private void CheckHeader(SignatureHeader header, SignatureKind expectedKind)
        {
            if (header.Kind != expectedKind)
            {
                throw new BadImageFormatException(SR.Format(SR.UnexpectedSignatureHeader, expectedKind, header.Kind, header.RawValue));
            }
        }

        private void CheckMethodOrPropertyHeader(SignatureHeader header)
        {
            SignatureKind kind = header.Kind;
            if (kind != SignatureKind.Method && kind != SignatureKind.Property)
            {
                throw new BadImageFormatException(SR.Format(SR.UnexpectedSignatureHeader2, SignatureKind.Property, SignatureKind.Method, header.Kind, header.RawValue));
            }
        }
    }
}
