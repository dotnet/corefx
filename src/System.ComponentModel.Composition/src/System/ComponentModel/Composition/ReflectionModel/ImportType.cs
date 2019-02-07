// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition.ReflectionModel
{
    // Describes the import type of a Reflection-based import definition
    internal class ImportType
    {
        private static readonly Type s_lazyOfTType = typeof(Lazy<>);
        private static readonly Type s_lazyOfTMType = typeof(Lazy<,>);
        private static readonly Type s_exportFactoryOfTType = typeof(ExportFactory<>);
        private static readonly Type s_exportFactoryOfTMType = typeof(ExportFactory<,>);
        private Func<Export, object> _castSingleValue;
        private readonly bool _isOpenGeneric = false;

        [ThreadStatic]
        internal static Dictionary<Type, Func<Export, object>> s_castSingleValueCache;

        private static Dictionary<Type, Func<Export, object>> CastSingleValueCache
        {
            get
            {
                return s_castSingleValueCache = s_castSingleValueCache ?? new Dictionary<Type, Func<Export, object>>();
            }
        }

        public ImportType(Type type, ImportCardinality cardinality)
        {
            ActualType = type ?? throw new ArgumentNullException(nameof(type));
            Type contractType = type;

            if (cardinality == ImportCardinality.ZeroOrMore)
            {
                IsAssignableCollectionType = IsTypeAssignableCollectionType(type);
                contractType = CheckForCollection(type);
            }

            // This sets contract type, metadata and the cast function
            _isOpenGeneric = type.ContainsGenericParameters;
            Initialize(contractType);
        }

        public bool IsAssignableCollectionType { get; }

        public Type ElementType { get; private set; }

        public Type ActualType { get; }

        public bool IsPartCreator { get; private set; }

        public Type ContractType { get; private set; }

        public Func<Export, object> CastExport
        {
            get
            {
                if (_isOpenGeneric)
                {
                    throw new Exception(SR.Diagnostic_InternalExceptionMessage);
                }
                return _castSingleValue;
            }
        }

        public Type MetadataViewType { get; private set; }

        private Type CheckForCollection(Type type)
        {
            ElementType = CollectionServices.GetEnumerableElementType(type);
            if (ElementType != null)
            {
                return ElementType;
            }
            return type;
        }

        private static bool IsGenericDescendentOf(Type type, Type baseGenericTypeDefinition)
        {
            if (type == typeof(object) || type == null)
            {
                return false;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == baseGenericTypeDefinition)
            {
                return true;
            }

            return IsGenericDescendentOf(type.BaseType, baseGenericTypeDefinition);
        }

        public static bool IsDescendentOf(Type type, Type baseType)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (baseType == null)
            {
                throw new ArgumentNullException(nameof(baseType));
            }

            if (!baseType.IsGenericTypeDefinition)
            {
                return baseType.IsAssignableFrom(type);
            }

            return IsGenericDescendentOf(type, baseType.GetGenericTypeDefinition());
        }

        private void Initialize(Type type)
        {
            if (!type.IsGenericType)
            {
                // no cast function, the original type is the contract type
                ContractType = type;
                return;
            }

            Type[] arguments = type.GetGenericArguments();
            Type genericType = type.GetGenericTypeDefinition().UnderlyingSystemType;

            // Look up the cast function
            if (!CastSingleValueCache.TryGetValue(type, out _castSingleValue))
            {
                if (!TryGetCastFunction(genericType, _isOpenGeneric, arguments, out _castSingleValue))
                {
                    // in this case, even though the type is generic, it's nothing we have recognized,
                    // thereforeit's the same as the non-generic case
                    ContractType = type;
                    return;
                }

                CastSingleValueCache.Add(type, _castSingleValue);
            }

            // we have found the cast function, which means, that we have found either Lazy of EF
            // in this case the contract is always argument[0] and the metadata view is always argument[1]
            IsPartCreator = !IsLazyGenericType(genericType) && (genericType != null);
            ContractType = arguments[0];
            if (arguments.Length == 2)
            {
                MetadataViewType = arguments[1];
            }
        }

        private static bool IsLazyGenericType(Type genericType)
        {
            return (genericType == s_lazyOfTType) || (genericType == s_lazyOfTMType);
        }

        private static bool TryGetCastFunction(Type genericType, bool isOpenGeneric, Type[] arguments, out Func<Export, object> castFunction)
        {
            castFunction = null;

            if (genericType == s_lazyOfTType)
            {
                if (!isOpenGeneric)
                {
                    castFunction = ExportServices.CreateStronglyTypedLazyFactory(arguments[0].UnderlyingSystemType, null);
                }
                return true;
            }

            if (genericType == s_lazyOfTMType)
            {
                if (!isOpenGeneric)
                {
                    castFunction = ExportServices.CreateStronglyTypedLazyFactory(arguments[0].UnderlyingSystemType, arguments[1].UnderlyingSystemType);
                }
                return true;
            }

            if (genericType != null && IsDescendentOf(genericType, s_exportFactoryOfTType))
            {
                if (arguments.Length == 1)
                {
                    if (!isOpenGeneric)
                    {
                        castFunction = new ExportFactoryCreator(genericType).CreateStronglyTypedExportFactoryFactory(arguments[0].UnderlyingSystemType, null);
                    }
                    return true;
                }
                else if (arguments.Length == 2)
                {
                    if (!isOpenGeneric)
                    {
                        castFunction = new ExportFactoryCreator(genericType).CreateStronglyTypedExportFactoryFactory(arguments[0].UnderlyingSystemType, arguments[1].UnderlyingSystemType);
                    }
                    return true;
                }
                else
                {
                    throw ExceptionBuilder.ExportFactory_TooManyGenericParameters(genericType.FullName);
                }
            }

            return false;
        }

        private static bool IsTypeAssignableCollectionType(Type type)
        {
            if (type.IsArray || CollectionServices.IsEnumerableOfT(type))
            {
                return true;
            }

            return false;
        }
    }
}
