// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace System.ComponentModel.Composition
{
    // Provides helpers for creating and dealing with Exports
    internal static partial class ExportServices
    {
        private static readonly MethodInfo s_createStronglyTypedLazyOfTM = typeof(ExportServices).GetMethod("CreateStronglyTypedLazyOfTM", BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly MethodInfo s_createStronglyTypedLazyOfT = typeof(ExportServices).GetMethod("CreateStronglyTypedLazyOfT", BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly MethodInfo s_createSemiStronglyTypedLazy = typeof(ExportServices).GetMethod("CreateSemiStronglyTypedLazy", BindingFlags.NonPublic | BindingFlags.Static);

        internal static readonly Type s_defaultMetadataViewType = typeof(IDictionary<string, object>);
        internal static readonly Type s_defaultExportedValueType = typeof(object);

        internal static bool IsDefaultMetadataViewType(Type metadataViewType)
        {
            if(metadataViewType == null)
            {
                throw new ArgumentNullException(nameof(metadataViewType));
            }

            // Consider all types that IDictionary<string, object> derives from, such
            // as ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>> 
            // and IEnumerable, as default metadata view
            return metadataViewType.IsAssignableFrom(s_defaultMetadataViewType);
        }

        internal static bool IsDictionaryConstructorViewType(Type metadataViewType)
        {
            if(metadataViewType == null)
            {
                throw new ArgumentNullException(nameof(metadataViewType));
            }

            // Does the view type have a constructor that is a Dictionary<string, object>
            return metadataViewType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                    Type.DefaultBinder,
                                                    new Type[] { typeof(IDictionary<string, object>) },
                                                    Array.Empty<ParameterModifier>()) != null;
        }

        internal static Func<Export, object> CreateStronglyTypedLazyFactory(Type exportType, Type metadataViewType)
        {
            MethodInfo genericMethod = null;
            if (metadataViewType != null)
            {
                genericMethod = s_createStronglyTypedLazyOfTM.MakeGenericMethod(exportType ?? ExportServices.s_defaultExportedValueType, metadataViewType);
            }
            else
            {
                genericMethod = s_createStronglyTypedLazyOfT.MakeGenericMethod(exportType ?? ExportServices.s_defaultExportedValueType);
            }

            if(genericMethod == null)
            {
                throw new ArgumentNullException(nameof(genericMethod));
            }

            return (Func<Export, object>)Delegate.CreateDelegate(typeof(Func<Export, object>), genericMethod);
        }

        internal static Func<Export, Lazy<object, object>> CreateSemiStronglyTypedLazyFactory(Type exportType, Type metadataViewType)
        {
            MethodInfo genericMethod = s_createSemiStronglyTypedLazy.MakeGenericMethod(
                exportType ?? s_defaultExportedValueType,
                metadataViewType ?? s_defaultMetadataViewType);
            if(genericMethod == null)
            {
                throw new ArgumentNullException(nameof(genericMethod));
            }
            return (Func<Export, Lazy<object, object>>)Delegate.CreateDelegate(typeof(Func<Export, Lazy<object, object>>), genericMethod);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        internal static Lazy<T, M> CreateStronglyTypedLazyOfTM<T, M>(Export export)
        {
            if (export is IDisposable disposable)
            {
                return new DisposableLazy<T, M>(
                    () => GetCastedExportedValue<T>(export),
                    AttributedModelServices.GetMetadataView<M>(export.Metadata),
                    disposable,
                    LazyThreadSafetyMode.PublicationOnly);
            }
            else
            {
                return new Lazy<T, M>(
                    () => GetCastedExportedValue<T>(export),
                    AttributedModelServices.GetMetadataView<M>(export.Metadata),
                    LazyThreadSafetyMode.PublicationOnly);
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        internal static Lazy<T> CreateStronglyTypedLazyOfT<T>(Export export)
        {
            if (export is IDisposable disposable)
            {
                return new DisposableLazy<T>(
                    () => GetCastedExportedValue<T>(export),
                    disposable,
                    LazyThreadSafetyMode.PublicationOnly);
            }
            else
            {
                return new Lazy<T>(() => GetCastedExportedValue<T>(export), LazyThreadSafetyMode.PublicationOnly);
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        internal static Lazy<object, object> CreateSemiStronglyTypedLazy<T, M>(Export export)
        {
            if (export is IDisposable disposable)
            {
                return new DisposableLazy<object, object>(
                    () => GetCastedExportedValue<T>(export),
                    AttributedModelServices.GetMetadataView<M>(export.Metadata),
                    disposable,
                    LazyThreadSafetyMode.PublicationOnly);
            }
            else
            {
                return new Lazy<object, object>(
                    () => GetCastedExportedValue<T>(export),
                    AttributedModelServices.GetMetadataView<M>(export.Metadata),
                    LazyThreadSafetyMode.PublicationOnly);
            }
        }

        internal static T GetCastedExportedValue<T>(Export export)
        {
            return CastExportedValue<T>(export.ToElement(), export.Value);
        }

        internal static T CastExportedValue<T>(ICompositionElement element, object exportedValue)
        {
            bool succeeded = ContractServices.TryCast(typeof(T), exportedValue, out object typedExportedValue);
            if (!succeeded)
            {
                throw new CompositionContractMismatchException(string.Format(CultureInfo.CurrentCulture,
                    SR.ContractMismatch_ExportedValueCannotBeCastToT,
                    element.DisplayName,
                    typeof(T)));
            }

            return (T)typedExportedValue;
        }

        internal static ExportCardinalityCheckResult CheckCardinality<T>(ImportDefinition definition, IEnumerable<T> enumerable)
        {
            EnumerableCardinality actualCardinality = (enumerable != null) ? enumerable.GetCardinality() : EnumerableCardinality.Zero;

            return MatchCardinality(actualCardinality, definition.Cardinality);
        }

        private static ExportCardinalityCheckResult MatchCardinality(EnumerableCardinality actualCardinality, ImportCardinality importCardinality)
        {
            switch (actualCardinality)
            {
                case EnumerableCardinality.Zero:
                    if (importCardinality == ImportCardinality.ExactlyOne)
                    {
                        return ExportCardinalityCheckResult.NoExports;
                    }
                    break;

                case EnumerableCardinality.TwoOrMore:
                    if (importCardinality.IsAtMostOne())
                    {
                        return ExportCardinalityCheckResult.TooManyExports;
                    }
                    break;

                default:
                    if(actualCardinality != EnumerableCardinality.One)
                    {
                        throw new Exception(SR.Diagnostic_InternalExceptionMessage);
                    }
                    break;

            }

            return ExportCardinalityCheckResult.Match;
        }
    }
}
