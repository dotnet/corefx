// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Diagnostics;

namespace System.Reflection.Metadata
{
    /// <summary>
    /// Decodes signature and custom attribute blobs.
    /// </summary>
    /// <typeparam name="TType">The representation for decoded types.</typeparam>
    public abstract class BlobDecoder<TType>
    {
        private readonly MetadataReader metadataReader;

        /// <summary>
        /// Initializes a new BlobDecoder instance.
        /// </summary>
        /// <param name="reader">The MetadataReader associated with the blobs that will be decoded.</param>
        protected BlobDecoder(MetadataReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            this.metadataReader = reader;
        }

        /// <summary>
        /// Gets the MetadataReader associated with this BlobDecoder.
        /// </summary>
        public MetadataReader Reader
        {
            get { return metadataReader; }
        }

        /// <summary>
        /// Decodes a type definition, reference or specification to its representation as TType.
        /// </summary>
        /// <param name="handle">A type definition, reference, or specification handle.</param>
        /// <exception cref="System.BadImageFormatException">The handle does not represent a valid type reference, definition, or specification.</exception>
        public TType DecodeType(Handle handle)
        {
            switch (handle.Kind)
            {
                case HandleKind.TypeReference:
                    return GetTypeFromReference((TypeReferenceHandle)handle);

                case HandleKind.TypeDefinition:
                    return GetTypeFromDefinition((TypeDefinitionHandle)handle);

                case HandleKind.TypeSpecification:
                    return DecodeTypeSpecification((TypeSpecificationHandle)handle);

                default:
                    throw new BadImageFormatException();
            }
        }

        /// <summary>
        /// Decodes a type specification.
        /// </summary>
        /// <param name="handle">The type specification handle.</param>
        /// <returns>The decoded type.</returns>
        /// <exception cref="System.BadImageFormatException">The type specification has an invalid signature.</exception>
        public TType DecodeTypeSpecification(TypeSpecificationHandle handle)
        {
            BlobHandle blobHandle = metadataReader.GetTypeSpecification(handle).Signature;
            BlobReader reader = metadataReader.GetBlobReader(blobHandle);
            return DecodeType(ref reader);
        }

        /// <summary>
        /// Decodes a type from within a signature from a BlobReader positioned at its leading SignatureTypeCode.
        /// </summary>
        /// <param name="reader">The blob reader.</param>
        /// <returns>The decoded type.</returns>
        /// <exception cref="System.BadImageFormatException">The reader was not positioned at a valid signature type.</exception>
        public TType DecodeType(ref BlobReader reader)
        {
            return DecodeType(ref reader, reader.ReadSignatureTypeCode());
        }

        /// <summary>
        /// Decodes a type from within a signature from a BlobReader positioned immediately past the given SignatureTypeCode.
        /// </summary>
        /// <param name="reader">The blob reader.</param>
        /// <param name="typeCode">The SignatureTypeCode that immediately preceded the reader's current position.</param>
        /// <returns>The decoded type.</returns>
        /// <exception cref="System.BadImageFormatException">The reader was not positioned at a valud signature type.</exception>
        public TType DecodeType(ref BlobReader reader, SignatureTypeCode typeCode)
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
                    return GetPrimitiveType((PrimitiveTypeCode)typeCode);

                case SignatureTypeCode.Pointer:
                    elementType = DecodeType(ref reader);
                    return GetPointerType(elementType);

                case SignatureTypeCode.ByReference:
                    elementType = DecodeType(ref reader);
                    return GetByReferenceType(elementType);

                case SignatureTypeCode.Pinned:
                    elementType = DecodeType(ref reader);
                    return GetPinnedType(elementType);

                case SignatureTypeCode.SZArray:
                    elementType = DecodeType(ref reader);
                    return GetSZArrayType(elementType);

                case SignatureTypeCode.FunctionPointer:
                    MethodSignature<TType> methodSignature = DecodeMethodSignature(ref reader);
                    return GetFunctionPointerType(methodSignature);

                case SignatureTypeCode.Array:
                    return DecodeArrayType(ref reader);

                case SignatureTypeCode.RequiredModifier:
                    return DecodeModifiedType(ref reader, isRequired: true);

                case SignatureTypeCode.OptionalModifier:
                    return DecodeModifiedType(ref reader, isRequired: false);

                case SignatureTypeCode.TypeHandle:
                    return DecodeTypeHandle(ref reader);

                case SignatureTypeCode.GenericTypeInstance:
                    return DecodeGenericTypeInstance(ref reader);

                case SignatureTypeCode.GenericTypeParameter:
                    index = reader.ReadCompressedInteger();
                    return GetGenericTypeParameter(index);

                case SignatureTypeCode.GenericMethodParameter:
                    index = reader.ReadCompressedInteger();
                    return GetGenericMethodParameter(index);

                default:
                    throw new BadImageFormatException();
            }
        }

        // Decodes a list of types preceded by their count as a compressed integer.
        private ImmutableArray<TType> DecodeTypes(ref BlobReader reader)
        {
            int count = reader.ReadCompressedInteger();
            if (count == 0)
            {
                return ImmutableArray<TType>.Empty;
            }

            var types = new TType[count];

            for (int i = 0; i < count; i++)
            {
                types[i] = DecodeType(ref reader);
            }

            return ImmutableArray.Create(types);
        }

        /// <summary>
        /// Decodes a method signature blob.
        /// </summary>
        /// <param name="handle">Handle to the method signature.</param>
        /// <returns>The decoded method signature.</returns>
        /// <exception cref="System.BadImageFormatException">The method signature is invalid.</exception>
        public MethodSignature<TType> DecodeMethodSignature(BlobHandle handle)
        {
            BlobReader reader = metadataReader.GetBlobReader(handle);
            return DecodeMethodSignature(ref reader);
        }

        /// <summary>
        /// Decodes a method signature blob.
        /// </summary>
        /// <param name="reader">BlobReader positioned at a method signature.</param>
        /// <returns>The decoded method signature.</returns>
        /// <exception cref="System.BadImageFormatException">The method signature is invalid.</exception>
        public MethodSignature<TType> DecodeMethodSignature(ref BlobReader reader)
        {
            SignatureHeader header = reader.ReadSignatureHeader();

            if (header.Kind != SignatureKind.Method && header.Kind != SignatureKind.Property)
            {
                throw new BadImageFormatException();
            }

            int genericParameterCount = 0;
            if (header.IsGeneric)
            {
                genericParameterCount = reader.ReadCompressedInteger();
            }

            int parameterCount = reader.ReadCompressedInteger();
            TType returnType = DecodeType(ref reader);

            if (parameterCount == 0)
            {
                return new MethodSignature<TType>(header, returnType, 0, genericParameterCount, ImmutableArray<TType>.Empty);
            }

            var parameterTypes = new TType[parameterCount];
            SignatureTypeCode typeCode;
            int parameterIndex;

            for (parameterIndex = 0; parameterIndex < parameterCount; parameterIndex++)
            {
                typeCode = reader.ReadSignatureTypeCode();

                if (typeCode == SignatureTypeCode.Sentinel)
                {
                    break;
                }

                parameterTypes[parameterIndex] = DecodeType(ref reader, typeCode);
            }

            int requiredParameterCount = parameterIndex;

            for (; parameterIndex < parameterCount; parameterIndex++)
            {
                parameterTypes[parameterIndex] = DecodeType(ref reader);
            }

            return new MethodSignature<TType>(header, returnType, requiredParameterCount, genericParameterCount, ImmutableArray.Create(parameterTypes));
        }

        /// <summary>
        /// Decodes a method specification signature blob.
        /// </summary>
        /// <param name="handle">The handle to the method specification signature blob. See <see cref="MethodSpecification.Signature"/>.</param>
        /// <returns>The types used to instantiate a generic method via a method specification.</returns>
        /// <exception cref="System.BadImageFormatException">The method specification signature is invalid.</exception>
        public ImmutableArray<TType> DecodeMethodSpecificationSignature(BlobHandle handle)
        {
            BlobReader reader = metadataReader.GetBlobReader(handle);
            return DecodeMethodSpecificationSignature(ref reader);
        }

        /// <summary>
        /// Decodes a method specification signature blob.
        /// </summary>
        /// <param name="reader">A BlobReader positioned at a valid method specification signature.</param>
        /// <returns>The types used to instantiate a generic method via the method specification.</returns>
        public ImmutableArray<TType> DecodeMethodSpecificationSignature(ref BlobReader reader)
        {
            SignatureHeader header = reader.ReadSignatureHeader();
            if (header.RawValue != (byte)SignatureAttributes.Generic)
            {
                throw new BadImageFormatException();
            }
         
            return DecodeTypes(ref reader);
        }

        /// <summary>
        /// Decodes a local variable signature blob.
        /// </summary>
        /// <param name="handle">The local variable signature handle.</param>
        /// <returns>The local variable types.</returns>
        /// <exception cref="System.BadImageFormatException">The local variable signature is invalid.</exception>
        public ImmutableArray<TType> DecodeLocalSignature(StandaloneSignatureHandle handle)
        {
            BlobHandle blobHandle = metadataReader.GetStandaloneSignature(handle).Signature;
            BlobReader reader = metadataReader.GetBlobReader(blobHandle);
            return DecodeLocalSignature(ref reader);
        }

        /// <summary>
        /// Decodes a local variable signature blob and advances the reader past the signature.
        /// </summary>
        /// <param name="reader">The blob reader.</param>
        /// <returns>The local variable types.</returns>
        /// <exception cref="System.BadImageFormatException">The local variable signature is invalid.</exception>
        public ImmutableArray<TType> DecodeLocalSignature(ref BlobReader reader)
        {
            SignatureHeader header = reader.ReadSignatureHeader();
            if (header.Kind != SignatureKind.LocalVariables)
            {
                throw new BadImageFormatException();
            }

            return DecodeTypes(ref reader);
        }

        /// <summary>
        /// Decodes a field signature.
        /// </summary>
        /// <param name="handle">The field signature handle.</param>
        /// <returns>The decoded field type.</returns>
        /// <exception cref="System.BadImageFormatException">The field signature is invalid.</exception>
        public TType DecodeFieldSignature(BlobHandle handle)
        {
            BlobReader reader = metadataReader.GetBlobReader(handle);
            return DecodeFieldSignature(ref reader);
        }

        /// <summary>
        /// Decodes a field signature.
        /// </summary>
        /// <returns>The decoded field type.</returns>
        public TType DecodeFieldSignature(ref BlobReader reader)
        {
            SignatureHeader header = reader.ReadSignatureHeader();

            if (header.Kind != SignatureKind.Field)
            {
                throw new BadImageFormatException();
            }

            return DecodeType(ref reader);
        }

        // Decodes a generalized (non-SZ/vector) array type represented by the element type followed by
        // its rank and optional sizes and lower bounds.
        private TType DecodeArrayType(ref BlobReader reader)
        {
            TType elementType = DecodeType(ref reader);
            int rank = reader.ReadCompressedInteger();
            var sizes = ImmutableArray<int>.Empty;
            var lowerBounds = ImmutableArray<int>.Empty;

            int sizesCount = reader.ReadCompressedInteger();
            if (sizesCount > 0)
            {
                var array = new int[sizesCount];
                for (int i = 0; i < sizesCount; i++)
                {
                    array[i] = reader.ReadCompressedInteger();
                }
                sizes = ImmutableArray.Create(array);
            }

            int lowerBoundsCount = reader.ReadCompressedInteger();
            if (lowerBoundsCount > 0)
            {
                var array = new int[lowerBoundsCount];
                for (int i = 0; i < lowerBoundsCount; i++)
                {
                    array[i] = reader.ReadCompressedSignedInteger();
                }
                lowerBounds = ImmutableArray.Create(array);
            }

            var arrayShape = new ArrayShape(rank, sizes, lowerBounds);
            return GetArrayType(elementType, arrayShape);
        }

        // Decodes a generic type instantiation encoded as the generic type followed by the types used to instantiate it.
        private TType DecodeGenericTypeInstance(ref BlobReader reader)
        {
            TType genericType = DecodeType(ref reader);
            ImmutableArray<TType> types = DecodeTypes(ref reader);
            return GetGenericInstance(genericType, types);
        }

        // Decodes a type with custom modifiers starting with the first modifier type that is required iff isRequired is passed,\
        // followed by an optional sequence of additional modifiers (<SignaureTypeCode.Required|OptionalModifier> <type>) and 
        // terminated by the unmodified type.
        private TType DecodeModifiedType(ref BlobReader reader, bool isRequired)
        {
            TType type = DecodeTypeHandle(ref reader);
            var modifier = new CustomModifier<TType>(type, isRequired);

            ImmutableArray<CustomModifier<TType>> modifiers;
            SignatureTypeCode typeCode = reader.ReadSignatureTypeCode();

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
                    type = DecodeType(ref reader, typeCode);
                    modifier = new CustomModifier<TType>(type, isRequired);
                    builder.Add(modifier);
                    typeCode = reader.ReadSignatureTypeCode();
                    isRequired = typeCode == SignatureTypeCode.RequiredModifier;
                } while (isRequired || typeCode == SignatureTypeCode.OptionalModifier);

                modifiers = builder.ToImmutable();
            }

            TType unmodifiedType = DecodeType(ref reader, typeCode);
            return GetModifiedType(unmodifiedType, modifiers);
        }

        // Decodes a type definition, reference, or specification from the type handle at the given blob reader's current position.
        private TType DecodeTypeHandle(ref BlobReader reader)
        {
            Handle handle = reader.ReadTypeHandle();
            return DecodeType(handle);
        }

        public CustomAttributeValue<TType> DecodeCustomAttribute(CustomAttributeHandle handle)
        {
            CustomAttribute attribute = metadataReader.GetCustomAttribute(handle);
            Handle constructor = attribute.Constructor;
            BlobHandle signature;

            switch (constructor.Kind)
            {
                case HandleKind.MethodDefinition:
                    MethodDefinition definition = metadataReader.GetMethodDefinition((MethodDefinitionHandle)constructor);
                    signature = definition.Signature;
                    break;

                case HandleKind.MemberReference:
                    MemberReference reference = metadataReader.GetMemberReference((MemberReferenceHandle)constructor);
                    signature = reference.Signature;
                    break;

                default:
                    throw new BadImageFormatException();
            }

            BlobReader signatureReader = metadataReader.GetBlobReader(signature);
            BlobReader valueReader = metadataReader.GetBlobReader(attribute.Value);

            UInt16 prolog = valueReader.ReadUInt16();
            if (prolog != 1)
            {
                throw new BadImageFormatException();
            }

            SignatureHeader header = signatureReader.ReadSignatureHeader();
            if (header.Kind != SignatureKind.Method || header.IsGeneric)
            {
                throw new BadImageFormatException();
            }

            int parameterCount = signatureReader.ReadCompressedInteger();
            SignatureTypeCode returnType = signatureReader.ReadSignatureTypeCode();
            if (returnType != SignatureTypeCode.Void)
            {
                throw new BadImageFormatException();
            }

            ImmutableArray<CustomAttributeTypedArgument<TType>> fixedArguments = DecodeFixedArguments(ref signatureReader, ref valueReader, parameterCount);
            ImmutableArray<CustomAttributeNamedArgument<TType>> namedArguments = DecodeNamedArguments(ref valueReader);
            return new CustomAttributeValue<TType>(fixedArguments, namedArguments);
        }

        private ImmutableArray<CustomAttributeTypedArgument<TType>> DecodeFixedArguments(ref BlobReader signatureReader, ref BlobReader valueReader, int count)
        {
            if (count == 0)
            {
                return ImmutableArray<CustomAttributeTypedArgument<TType>>.Empty;
            }

            var arguments = new CustomAttributeTypedArgument<TType>[count];

            for (int i = 0; i < count; i++)
            {
                ArgumentTypeInfo info = DecodeFixedArgumentType(ref signatureReader);
                arguments[i] = DecodeArgument(ref valueReader, info);
            }

            return ImmutableArray.Create(arguments);
        }

        private ImmutableArray<CustomAttributeNamedArgument<TType>> DecodeNamedArguments(ref BlobReader reader)
        {
            int count = reader.ReadUInt16();
            if (count == 0)
            {
                return ImmutableArray<CustomAttributeNamedArgument<TType>>.Empty;
            }

            var arguments = new CustomAttributeNamedArgument<TType>[count]; 
            for (int i = 0; i < count; i++)
            {
                CustomAttributeNamedArgumentKind kind = (CustomAttributeNamedArgumentKind)reader.ReadSerializationTypeCode();
                if (kind != CustomAttributeNamedArgumentKind.Field && kind != CustomAttributeNamedArgumentKind.Property)
                {
                    throw new BadImageFormatException();
                }

                ArgumentTypeInfo info = DecodeNamedArgumentType(ref reader);
                string name = reader.ReadSerializedString();
                CustomAttributeTypedArgument<TType> argument = DecodeArgument(ref reader, info);
                arguments[i] = new CustomAttributeNamedArgument<TType>(name, kind, argument.Type, argument.Value);
            }

            return ImmutableArray.Create(arguments);
        }

        private struct ArgumentTypeInfo
        {
            public TType Type;
            public TType ElementType;
            public SerializationTypeCode TypeCode;
            public SerializationTypeCode ElementTypeCode;
        }

        // Decodes a fixed argument type of a custom attribute from its constructor signature.
        //
        // Note that we do not decode the full constructor signature using DecodeMethodSignature
        // but instead decode one parameter at a time as we read the value blob. This is both
        // better perf-wise, but even more important is that we can't actually reason about
        // a method signature with opaque TType values without adding some unecessary chatter
        // with the concrete subclass.
        private ArgumentTypeInfo DecodeFixedArgumentType(ref BlobReader signatureReader, bool isElementType = false)
        {
            SignatureTypeCode signatureTypeCode = signatureReader.ReadSignatureTypeCode();

            var info = new ArgumentTypeInfo
            {
                TypeCode = (SerializationTypeCode)signatureTypeCode,
            };

            switch (signatureTypeCode)
            {
                case SignatureTypeCode.Boolean:
                case SignatureTypeCode.Byte:
                case SignatureTypeCode.Char:
                case SignatureTypeCode.Double:
                case SignatureTypeCode.Int16:
                case SignatureTypeCode.Int32:
                case SignatureTypeCode.Int64:
                case SignatureTypeCode.SByte:
                case SignatureTypeCode.Single:
                case SignatureTypeCode.String:
                case SignatureTypeCode.UInt16:
                case SignatureTypeCode.UInt32:
                case SignatureTypeCode.UInt64:
                    info.Type = GetPrimitiveType((PrimitiveTypeCode)signatureTypeCode);
                    break;

                case SignatureTypeCode.Object:
                    info.TypeCode = SerializationTypeCode.TaggedObject;
                    info.Type = GetPrimitiveType(PrimitiveTypeCode.Object);
                    break;

                case SignatureTypeCode.TypeHandle:
                    // Parameter is type def or ref and is only allowed to be System.Type or Enum.
                    Handle handle = signatureReader.ReadTypeHandle();
                    info.Type = GetTypeFromReferenceOrDefinition(handle);
                    info.TypeCode = IsSystemType(info.Type) ? SerializationTypeCode.Type : (SerializationTypeCode)GetUnderlyingEnumType(info.Type);
                    break;

                case SignatureTypeCode.SZArray:
                    if (isElementType)
                    {
                        // jagged arrays are not allowed.
                        throw new BadImageFormatException();
                    }

                    var elementInfo = DecodeFixedArgumentType(ref signatureReader, isElementType: true);
                    info.ElementType = elementInfo.Type;
                    info.ElementTypeCode = elementInfo.TypeCode;
                    info.Type = GetSZArrayType(info.ElementType);
                    break;

                default:
                    throw new BadImageFormatException();
            }

            return info;
        }

        private ArgumentTypeInfo DecodeNamedArgumentType(ref BlobReader valueReader, bool isElementType = false)
        {
            var info = new ArgumentTypeInfo
            {
                TypeCode = valueReader.ReadSerializationTypeCode(),
            };

            switch (info.TypeCode)
            {
                case SerializationTypeCode.Boolean:
                case SerializationTypeCode.Byte:
                case SerializationTypeCode.Char:
                case SerializationTypeCode.Double:
                case SerializationTypeCode.Int16:
                case SerializationTypeCode.Int32:
                case SerializationTypeCode.Int64:
                case SerializationTypeCode.SByte:
                case SerializationTypeCode.Single:
                case SerializationTypeCode.String:
                case SerializationTypeCode.UInt16:
                case SerializationTypeCode.UInt32:
                case SerializationTypeCode.UInt64:
                    info.Type = GetPrimitiveType((PrimitiveTypeCode)info.TypeCode);
                    break;

                case SerializationTypeCode.Type:
                    info.Type = GetSystemType();
                    break;

                case SerializationTypeCode.TaggedObject:
                    info.Type = GetPrimitiveType(PrimitiveTypeCode.Object);
                    break;

                case SerializationTypeCode.SZArray:
                    if (isElementType)
                    {
                        // jagged arrays are not allowed.
                        throw new BadImageFormatException();
                    }

                    var elementInfo = DecodeNamedArgumentType(ref valueReader, isElementType: true);
                    info.ElementType = elementInfo.Type;
                    info.ElementTypeCode = elementInfo.TypeCode;
                    info.Type = GetSZArrayType(info.ElementType);
                    break;

                case SerializationTypeCode.Enum:
                    string typeName = valueReader.ReadSerializedString();
                    info.Type = GetTypeFromSerializedName(typeName);
                    info.TypeCode = (SerializationTypeCode)GetUnderlyingEnumType(info.Type);
                    break;

                default:
                    throw new BadImageFormatException();
            }

            return info;
        }

        private CustomAttributeTypedArgument<TType> DecodeArgument(ref BlobReader valueReader, ArgumentTypeInfo info)
        {
            if (info.TypeCode == SerializationTypeCode.TaggedObject)
            {
                info = DecodeNamedArgumentType(ref valueReader);
            }

            object value;

            switch (info.TypeCode)
            {
                case SerializationTypeCode.Boolean:
                    value = valueReader.ReadBoolean();
                    break;

                case SerializationTypeCode.Byte:
                    value = valueReader.ReadByte();
                    break;

                case SerializationTypeCode.Char:
                    value = valueReader.ReadChar();
                    break;

                case SerializationTypeCode.Double:
                    value = valueReader.ReadDouble();
                    break;

                case SerializationTypeCode.Int16:
                    value = valueReader.ReadInt16();
                    break;

                case SerializationTypeCode.Int32:
                    value = valueReader.ReadInt32();
                    break;

                case SerializationTypeCode.Int64:
                    value = valueReader.ReadInt64();
                    break;

                case SerializationTypeCode.SByte:
                    value = valueReader.ReadSByte();
                    break;

                case SerializationTypeCode.Single:
                    value = valueReader.ReadSingle();
                    break;

                case SerializationTypeCode.UInt16:
                    value = valueReader.ReadUInt16();
                    break;

                case SerializationTypeCode.UInt32:
                    value = valueReader.ReadUInt32();
                    break;

                case SerializationTypeCode.UInt64:
                    value = valueReader.ReadUInt64();
                    break;

                case SerializationTypeCode.String:
                    value = valueReader.ReadSerializedString();
                    break;

                case SerializationTypeCode.Type:
                    string typeName = valueReader.ReadSerializedString();
                    value = GetTypeFromSerializedName(typeName);
                    break;

                case SerializationTypeCode.SZArray:
                    value = DecodeArrayArgument(ref valueReader, info);
                    break;

                default:
                    throw new BadImageFormatException();
            }

            return new CustomAttributeTypedArgument<TType>(info.Type, value);
        }

        private ImmutableArray<CustomAttributeTypedArgument<TType>>? DecodeArrayArgument(ref BlobReader reader, ArgumentTypeInfo info)
        {
            int count = reader.ReadInt32();
            if (count == -1)
            {
                return null;
            }

            if (count == 0)
            {
                return ImmutableArray<CustomAttributeTypedArgument<TType>>.Empty;
            }

            if (count < 0)
            {
                throw new BadImageFormatException();
            }

            var elementInfo = new ArgumentTypeInfo
            {
                Type = info.ElementType,
                TypeCode = info.ElementTypeCode,
            };

            var array = new CustomAttributeTypedArgument<TType>[count];

            for (int i = 0; i < count; i++)
            {
                array[i] = DecodeArgument(ref reader, elementInfo);
            }

            return ImmutableArray.Create(array);
        }

        private TType GetTypeFromReferenceOrDefinition(Handle handle)
        {
            switch (handle.Kind)
            {
                case HandleKind.TypeDefinition:
                    return GetTypeFromDefinition((TypeDefinitionHandle)handle);

                case HandleKind.TypeReference:
                    return GetTypeFromReference((TypeReferenceHandle)handle);

                default:
                    throw new BadImageFormatException();
            }
        }

        protected abstract TType GetPrimitiveType(PrimitiveTypeCode typeCode);
        protected abstract TType GetTypeFromDefinition(TypeDefinitionHandle handle);
        protected abstract TType GetTypeFromReference(TypeReferenceHandle handle);
        protected abstract TType GetSZArrayType(TType elementType);
        protected abstract TType GetArrayType(TType elementType, ArrayShape shape);
        protected abstract TType GetFunctionPointerType(MethodSignature<TType> signature);
        protected abstract TType GetPointerType(TType elementType);
        protected abstract TType GetByReferenceType(TType elementType);
        protected abstract TType GetModifiedType(TType unmodifiedType, ImmutableArray<CustomModifier<TType>> customModifiers);
        protected abstract TType GetGenericMethodParameter(int index);
        protected abstract TType GetGenericTypeParameter(int index);
        protected abstract TType GetGenericInstance(TType genericType, ImmutableArray<TType> typeArguments);
        protected abstract TType GetPinnedType(TType elementType);
        protected abstract TType GetTypeFromSerializedName(string name);
        protected abstract PrimitiveTypeCode GetUnderlyingEnumType(TType type);

        // TODO: Review names here. SystemType isn't obviously System DOT Type. It sounds like any type of the "system".

        /// <summary>
        /// Gets the TType representation for <see cref="System.Type"/>.
        /// </summary>
        protected abstract TType GetSystemType();

        /// <summary>
        /// Returns true if the given type represents <see cref="System.Type"/>.
        /// </summary>
        protected abstract bool IsSystemType(TType type);
    }
}
