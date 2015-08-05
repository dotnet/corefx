// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Diagnostics;

namespace System.Reflection.Metadata.Decoding
{
    internal partial class TypeNameParser<TType>
    {
        private class TypeBuilder
        {
            public string DeclaringTypeFullName;
            public ImmutableArray<string>.Builder _nestedTypeNames;
            public ImmutableArray<TType>.Builder _genericTypeArguments;
            public ImmutableArray<TypeModifier>.Builder _typeModifiers;
            public AssemblyNameComponents? AssemblyName;

            public TType BuildType(ITypeNameParserTypeProvider<TType> typeProvider)
            {
                TType type = BuildSimpleType(typeProvider);
                type = BuildGenericInstanceIfNeeded(typeProvider, type);
                type = BuildCompoundTypeIfNeeded(typeProvider, type);

                return type;
            }

            public void AddGenericTypeArgument(TType genericTypeArgument)
            {
                if (_genericTypeArguments == null)
                    _genericTypeArguments = ImmutableArray.CreateBuilder<TType>();

                _genericTypeArguments.Add(genericTypeArgument);
            }
            public void AddNestedTypeName(string nestedTypeName)
            {
                if (_nestedTypeNames == null)
                    _nestedTypeNames = ImmutableArray.CreateBuilder<string>();

                _nestedTypeNames.Add(nestedTypeName);
            }

            public void AddTypeModifier(TypeModifier typeModifier)
            {
                if (_typeModifiers == null)
                    _typeModifiers = ImmutableArray.CreateBuilder<TypeModifier>();

                _typeModifiers.Add(typeModifier);
            }

            private TType BuildSimpleType(ITypeNameParserTypeProvider<TType> typeProvider)
            {
                return typeProvider.GetTypeFromName(AssemblyName, DeclaringTypeFullName, _nestedTypeNames == null ? ImmutableArray<string>.Empty : _nestedTypeNames.ToImmutable());
            }

            private TType BuildGenericInstanceIfNeeded(ITypeNameParserTypeProvider<TType> typeProvider, TType genericType)
            {
                if (_genericTypeArguments == null)
                    return genericType;

                Debug.Assert(_genericTypeArguments.Count > 0);
                return typeProvider.GetGenericInstance(genericType, _genericTypeArguments.ToImmutableArray());
            }

            private TType BuildCompoundTypeIfNeeded(ITypeNameParserTypeProvider<TType> typeProvider, TType elementType)
            {
                if (_typeModifiers != null)
                {
                    Debug.Assert(_typeModifiers.Count > 0);

                    foreach (TypeModifier modifier in _typeModifiers)
                    {
                        switch (modifier.ModifierType)
                        {
                            case TypeModifierType.ByReference:
                                elementType = typeProvider.GetByReferenceType(elementType);
                                break;

                            case TypeModifierType.Pointer:
                                elementType = typeProvider.GetPointerType(elementType);
                                break;

                            case TypeModifierType.SZArray:
                                elementType = typeProvider.GetSZArrayType(elementType);
                                break;

                            case TypeModifierType.Array:
                                elementType = typeProvider.GetArrayType(elementType, modifier.ArrayShape);
                                break;
                        }
                    }
                }

                return elementType;
            }
        }
    }
}
