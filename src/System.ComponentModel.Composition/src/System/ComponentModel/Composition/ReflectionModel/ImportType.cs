// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Primitives;
using Microsoft.Internal;
using Microsoft.Internal.Collections;
using System.Reflection;
using System.Collections.Generic;

namespace System.ComponentModel.Composition.ReflectionModel
{
    // Describes the import type of a Reflection-based import definition
    internal class ImportType
    {
        private static readonly Type LazyOfTType = typeof(Lazy<>);
        private static readonly Type LazyOfTMType = typeof(Lazy<,>);
        private static readonly Type ExportFactoryOfTType = typeof(ExportFactory<>);
        private static readonly Type ExportFactoryOfTMType = typeof(ExportFactory<,>);

        private readonly Type _type;
        private readonly bool _isAssignableCollectionType;
        private Type _contractType;
        private Func<Export, object> _castSingleValue;
        private bool _isOpenGeneric = false;

        [ThreadStatic]
        internal static Dictionary<Type, Func<Export, object>> _castSingleValueCache;

        private static Dictionary<Type, Func<Export, object>> CastSingleValueCache
        {
            get
            {
                return _castSingleValueCache = _castSingleValueCache ?? new Dictionary<Type, Func<Export, object>>();
            }
        }


        public ImportType(Type type, ImportCardinality cardinality)
        {
            Assumes.NotNull(type);

            this._type = type;
            Type contractType = type;

            if (cardinality == ImportCardinality.ZeroOrMore)
            {
                this._isAssignableCollectionType = IsTypeAssignableCollectionType(type);
                contractType = CheckForCollection(type);
            }

            // This sets contract type, metadata and the cast function
            this._isOpenGeneric = type.ContainsGenericParameters;
            Initialize(contractType);
        }

        public bool IsAssignableCollectionType
        {
            get { return this._isAssignableCollectionType; }
        }

        public Type ElementType { get; private set; }

        public Type ActualType
        {
            get { return this._type; }
        }

        public bool IsPartCreator { get; private set; }

        public Type ContractType { get { return this._contractType; } }

        public Func<Export, object> CastExport
        {
            get
            {
                Assumes.IsTrue(!this._isOpenGeneric);
                return this._castSingleValue;
            }
        }

        public Type MetadataViewType { get; private set; }

        private Type CheckForCollection(Type type)
        {
            this.ElementType = CollectionServices.GetEnumerableElementType(type);
            if (this.ElementType != null)
            {
                return this.ElementType;
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
            Assumes.NotNull(type);
            Assumes.NotNull(baseType);

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
                this._contractType = type;
                return;
            }

            Type[] arguments = type.GetGenericArguments();
            Type genericType = type.GetGenericTypeDefinition().UnderlyingSystemType;

            // Look up the cast function
            if (!CastSingleValueCache.TryGetValue(type, out this._castSingleValue))
            {
                if (!TryGetCastFunction(genericType, this._isOpenGeneric, arguments, out this._castSingleValue))
                {
                    // in this case, even though the type is generic, it's nothing we have recognized,
                    // thereforeit's the same as the non-generic case
                    this._contractType = type;
                    return;
                }

                CastSingleValueCache.Add(type, this._castSingleValue);
            }

            // we have found the cast function, which means, that we have found either Lazy of EF
            // in this case the contract is always argument[0] and the metadata view is always argument[1]
            this.IsPartCreator = !IsLazyGenericType(genericType) && (genericType != null);
            this._contractType = arguments[0];
            if (arguments.Length == 2)
            {
                this.MetadataViewType = arguments[1];
            }
        }

        private static bool IsLazyGenericType(Type genericType)
        {
            return (genericType == LazyOfTType) || (genericType == LazyOfTMType);
        }

        private static bool TryGetCastFunction(Type genericType, bool isOpenGeneric, Type[] arguments, out Func<Export, object> castFunction)
        {
            castFunction = null;

            if (genericType == LazyOfTType)
            {
                if (!isOpenGeneric)
                {
                    castFunction = ExportServices.CreateStronglyTypedLazyFactory(arguments[0].UnderlyingSystemType, null);
                }
                return true;
            }

            if (genericType == LazyOfTMType)
            {
                if (!isOpenGeneric)
                {
                    castFunction = ExportServices.CreateStronglyTypedLazyFactory(arguments[0].UnderlyingSystemType, arguments[1].UnderlyingSystemType);
                }
                return true;
            }

            if (genericType != null && IsDescendentOf(genericType, ExportFactoryOfTType))
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
