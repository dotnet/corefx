// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.ReflectionModel
{
    public static class ReflectionModelServices
    {
        public static Lazy<Type> GetPartType(ComposablePartDefinition partDefinition)
        {
            Requires.NotNull(partDefinition, nameof(partDefinition));
            Contract.Ensures(Contract.Result<Lazy<Type>>() != null);

            if (partDefinition is ReflectionComposablePartDefinition reflectionPartDefinition)
            {
                return reflectionPartDefinition.GetLazyPartType();
            }

            throw ExceptionBuilder.CreateReflectionModelInvalidPartDefinition(nameof(partDefinition), partDefinition.GetType());
        }

        public static bool IsDisposalRequired(ComposablePartDefinition partDefinition)
        {
            Requires.NotNull(partDefinition, nameof(partDefinition));

            if (partDefinition is ReflectionComposablePartDefinition reflectionPartDefinition)
            {
                return reflectionPartDefinition.IsDisposalRequired;
            }

            throw ExceptionBuilder.CreateReflectionModelInvalidPartDefinition(nameof(partDefinition), partDefinition.GetType());
        }

        public static LazyMemberInfo GetExportingMember(ExportDefinition exportDefinition)
        {
            Requires.NotNull(exportDefinition, nameof(exportDefinition));

            if (exportDefinition is ReflectionMemberExportDefinition reflectionExportDefinition)
            {
                return reflectionExportDefinition.ExportingLazyMember;
            }

            throw new ArgumentException(
                string.Format(CultureInfo.CurrentCulture, SR.ReflectionModel_InvalidExportDefinition, exportDefinition.GetType()),
                nameof(exportDefinition));
        }

        public static LazyMemberInfo GetImportingMember(ImportDefinition importDefinition)
        {
            Requires.NotNull(importDefinition, nameof(importDefinition));

            if (importDefinition is ReflectionMemberImportDefinition reflectionMemberImportDefinition)
            {
                return reflectionMemberImportDefinition.ImportingLazyMember;
            }

            throw new ArgumentException(
                string.Format(CultureInfo.CurrentCulture, SR.ReflectionModel_InvalidMemberImportDefinition, importDefinition.GetType()),
                nameof(importDefinition));
        }

        public static Lazy<ParameterInfo> GetImportingParameter(ImportDefinition importDefinition)
        {
            Requires.NotNull(importDefinition, nameof(importDefinition));
            Contract.Ensures(Contract.Result<Lazy<ParameterInfo>>() != null);

            if (importDefinition is ReflectionParameterImportDefinition reflectionParameterImportDefinition)
            {
                return reflectionParameterImportDefinition.ImportingLazyParameter;
            }

            throw new ArgumentException(
                string.Format(CultureInfo.CurrentCulture, SR.ReflectionModel_InvalidParameterImportDefinition, importDefinition.GetType()),
                nameof(importDefinition));
        }

        public static bool IsImportingParameter(ImportDefinition importDefinition)
        {
            Requires.NotNull(importDefinition, nameof(importDefinition));

            if (importDefinition is ReflectionImportDefinition reflectionImportDefinition)
            {
                return importDefinition is ReflectionParameterImportDefinition;
            }

            throw new ArgumentException(
                string.Format(CultureInfo.CurrentCulture, SR.ReflectionModel_InvalidImportDefinition, importDefinition.GetType()),
                nameof(importDefinition));
        }

        public static bool IsExportFactoryImportDefinition(ImportDefinition importDefinition)
        {
            Requires.NotNull(importDefinition, nameof(importDefinition));

            return importDefinition is IPartCreatorImportDefinition;
        }

        public static ContractBasedImportDefinition GetExportFactoryProductImportDefinition(ImportDefinition importDefinition)
        {
            Requires.NotNull(importDefinition, nameof(importDefinition));
            Contract.Ensures(Contract.Result<ContractBasedImportDefinition>() != null);

            if (importDefinition is IPartCreatorImportDefinition partCreatorImportDefinition)
            {
                return partCreatorImportDefinition.ProductImportDefinition;
            }

            throw new ArgumentException(
                string.Format(CultureInfo.CurrentCulture, SR.ReflectionModel_InvalidImportDefinition, importDefinition.GetType()),
                nameof(importDefinition));
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static ComposablePartDefinition CreatePartDefinition(
            Lazy<Type> partType,
            bool isDisposalRequired,
            Lazy<IEnumerable<ImportDefinition>> imports,
            Lazy<IEnumerable<ExportDefinition>> exports,
            Lazy<IDictionary<string, object>> metadata,
            ICompositionElement origin)
        {
            Requires.NotNull(partType, nameof(partType));
            Contract.Ensures(Contract.Result<ComposablePartDefinition>() != null);

            return new ReflectionComposablePartDefinition(
                new ReflectionPartCreationInfo(
                    partType,
                    isDisposalRequired,
                    imports,
                    exports,
                    metadata,
                    origin));
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static ExportDefinition CreateExportDefinition(
            LazyMemberInfo exportingMember,
            string contractName,
            Lazy<IDictionary<string, object>> metadata,
            ICompositionElement origin)
        {
            if (contractName == null)
            {
                throw new ArgumentNullException(contractName);
            }

            if (contractName.Length == 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, SR.ArgumentException_EmptyString, contractName), contractName);
            }

            MemberTypes validMemberTypes = MemberTypes.Field | MemberTypes.Property | MemberTypes.NestedType | MemberTypes.TypeInfo | MemberTypes.Method;
            if ((exportingMember.MemberType & validMemberTypes) != exportingMember.MemberType ||
                (exportingMember.MemberType & (exportingMember.MemberType - 1)) != 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, SR.ArgumentOutOfRange_InvalidEnumInSet, nameof(exportingMember), exportingMember.MemberType, validMemberTypes.ToString()), nameof(exportingMember));
            }

            Contract.Ensures(Contract.Result<ExportDefinition>() != null);

            return new ReflectionMemberExportDefinition(
                exportingMember,
                new LazyExportDefinition(contractName, metadata),
                origin);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static ContractBasedImportDefinition CreateImportDefinition(
            LazyMemberInfo importingMember,
            string contractName,
            string requiredTypeIdentity,
            IEnumerable<KeyValuePair<string, Type>> requiredMetadata,
            ImportCardinality cardinality,
            bool isRecomposable,
            CreationPolicy requiredCreationPolicy,
            ICompositionElement origin)
        {
            return CreateImportDefinition(importingMember, contractName, requiredTypeIdentity, requiredMetadata, cardinality, isRecomposable, requiredCreationPolicy, MetadataServices.EmptyMetadata, false, origin);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static ContractBasedImportDefinition CreateImportDefinition(
            LazyMemberInfo importingMember,
            string contractName,
            string requiredTypeIdentity,
            IEnumerable<KeyValuePair<string, Type>> requiredMetadata,
            ImportCardinality cardinality,
            bool isRecomposable,
            CreationPolicy requiredCreationPolicy,
            IDictionary<string, object> metadata,
            bool isExportFactory,
            ICompositionElement origin)
        {
            return CreateImportDefinition(
                importingMember,
                contractName,
                requiredTypeIdentity,
                requiredMetadata,
                cardinality,
                isRecomposable,
                false,
                requiredCreationPolicy,
                metadata,
                isExportFactory,
                origin);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static ContractBasedImportDefinition CreateImportDefinition(
            LazyMemberInfo importingMember,
            string contractName,
            string requiredTypeIdentity,
            IEnumerable<KeyValuePair<string, Type>> requiredMetadata,
            ImportCardinality cardinality,
            bool isRecomposable,
            bool isPreRequisite,
            CreationPolicy requiredCreationPolicy,
            IDictionary<string, object> metadata,
            bool isExportFactory,
            ICompositionElement origin)
        {
            if (contractName == null)
            {
                throw new ArgumentNullException(contractName);
            }

            if (contractName.Length == 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, SR.ArgumentException_EmptyString, contractName), contractName);
            }

            MemberTypes validMemberTypes = MemberTypes.Field | MemberTypes.Property;
            if ((importingMember.MemberType & validMemberTypes) != importingMember.MemberType ||
                (importingMember.MemberType & (importingMember.MemberType - 1)) != 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, SR.ArgumentOutOfRange_InvalidEnumInSet, nameof(importingMember), importingMember.MemberType, validMemberTypes.ToString()), nameof(importingMember));
            }

            Contract.Ensures(Contract.Result<ContractBasedImportDefinition>() != null);

            if (isExportFactory)
            {
                return new PartCreatorMemberImportDefinition(
                    importingMember,
                    origin,
                    new ContractBasedImportDefinition(
                        contractName,
                        requiredTypeIdentity,
                        requiredMetadata,
                        cardinality,
                        isRecomposable,
                        isPreRequisite,
                        CreationPolicy.NonShared,
                        metadata));
            }
            else
            {
                return new ReflectionMemberImportDefinition(
                    importingMember,
                    contractName,
                    requiredTypeIdentity,
                    requiredMetadata,
                    cardinality,
                    isRecomposable,
                    isPreRequisite,
                    requiredCreationPolicy,
                    metadata,
                    origin);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static ContractBasedImportDefinition CreateImportDefinition(
            Lazy<ParameterInfo> parameter,
            string contractName,
            string requiredTypeIdentity,
            IEnumerable<KeyValuePair<string, Type>> requiredMetadata,
            ImportCardinality cardinality,
            CreationPolicy requiredCreationPolicy,
            ICompositionElement origin)
        {
            return CreateImportDefinition(parameter, contractName, requiredTypeIdentity, requiredMetadata, cardinality, requiredCreationPolicy, MetadataServices.EmptyMetadata, false, origin);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static ContractBasedImportDefinition CreateImportDefinition(
            Lazy<ParameterInfo> parameter,
            string contractName,
            string requiredTypeIdentity,
            IEnumerable<KeyValuePair<string, Type>> requiredMetadata,
            ImportCardinality cardinality,
            CreationPolicy requiredCreationPolicy,
            IDictionary<string, object> metadata,
            bool isExportFactory,
            ICompositionElement origin)
        {
            Requires.NotNull(parameter, nameof(parameter));

            if (contractName == null)
            {
                throw new ArgumentNullException(contractName);
            }

            if (contractName.Length == 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, SR.ArgumentException_EmptyString, contractName), contractName);
            }

            Contract.Ensures(Contract.Result<ContractBasedImportDefinition>() != null);

            if (isExportFactory)
            {
                return new PartCreatorParameterImportDefinition(
                    parameter,
                    origin,
                    new ContractBasedImportDefinition(
                        contractName,
                        requiredTypeIdentity,
                        requiredMetadata,
                        cardinality,
                        false,
                        true,
                        CreationPolicy.NonShared,
                        metadata));
            }
            else
            {
                return new ReflectionParameterImportDefinition(
                    parameter,
                    contractName,
                    requiredTypeIdentity,
                    requiredMetadata,
                    cardinality,
                    requiredCreationPolicy,
                    metadata,
                    origin);
            }
        }

        public static bool TryMakeGenericPartDefinition(ComposablePartDefinition partDefinition, IEnumerable<Type> genericParameters, out ComposablePartDefinition specialization)
        {
            Requires.NotNull(partDefinition, nameof(partDefinition));

            specialization = null;
            if (partDefinition is ReflectionComposablePartDefinition reflectionPartDefinition)
            {
                return reflectionPartDefinition.TryMakeGenericPartDefinition(genericParameters.ToArray(), out specialization);
            }

            throw ExceptionBuilder.CreateReflectionModelInvalidPartDefinition(nameof(partDefinition), partDefinition.GetType());
        }
    }

    internal class ReflectionPartCreationInfo : IReflectionPartCreationInfo
    {
        private readonly Lazy<Type> _partType;
        private readonly Lazy<IEnumerable<ImportDefinition>> _imports;
        private readonly Lazy<IEnumerable<ExportDefinition>> _exports;
        private readonly Lazy<IDictionary<string, object>> _metadata;
        private ConstructorInfo _constructor;

        public ReflectionPartCreationInfo(
            Lazy<Type> partType,
            bool isDisposalRequired,
            Lazy<IEnumerable<ImportDefinition>> imports,
            Lazy<IEnumerable<ExportDefinition>> exports,
            Lazy<IDictionary<string, object>> metadata,
            ICompositionElement origin)
        {
            _partType = partType ?? throw new ArgumentNullException(nameof(partType));
            IsDisposalRequired = isDisposalRequired;
            _imports = imports;
            _exports = exports;
            _metadata = metadata;
            Origin = origin;
        }

        public string DisplayName => GetPartType().GetDisplayName();

        public Type GetPartType() => _partType.GetNotNullValue("type");

        public Lazy<Type> GetLazyPartType() => _partType;

        public ConstructorInfo GetConstructor()
        {
            if (_constructor == null)
            {
                ConstructorInfo[] constructors = null;
                constructors = GetImports()
                    .OfType<ReflectionParameterImportDefinition>()
                    .Select(parameterImport => parameterImport.ImportingLazyParameter.Value.Member)
                    .OfType<ConstructorInfo>()
                    .Distinct()
                    .ToArray();

                if (constructors.Length == 1)
                {
                    _constructor = constructors[0];
                }
                else if (constructors.Length == 0)
                {
                    _constructor = GetPartType().GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
                }
            }
            return _constructor;
        }

        public bool IsDisposalRequired { get; }

        public bool IsIdentityComparison => true;

        public IDictionary<string, object> GetMetadata() => _metadata?.Value;

        public IEnumerable<ExportDefinition> GetExports()
        {
            if (_exports == null)
            {
                yield break;
            }

            IEnumerable<ExportDefinition> exports = _exports.Value;

            if (exports == null)
            {
                yield break;
            }

            foreach (ExportDefinition export in exports)
            {
                if (export is ReflectionMemberExportDefinition reflectionExport)
                {
                    yield return reflectionExport;
                }

                throw new InvalidOperationException(
                        string.Format(CultureInfo.CurrentCulture, SR.ReflectionModel_InvalidExportDefinition, export.GetType()));
            }
        }

        public IEnumerable<ImportDefinition> GetImports()
        {
            if (_imports == null)
            {
                yield break;
            }

            IEnumerable<ImportDefinition> imports = _imports.Value;

            if (imports == null)
            {
                yield break;
            }

            foreach (ImportDefinition import in imports)
            {
                if (import is ReflectionImportDefinition reflectionImport)
                {
                    yield return reflectionImport;
                }

                throw new InvalidOperationException(
                        string.Format(CultureInfo.CurrentCulture, SR.ReflectionModel_InvalidMemberImportDefinition, import.GetType()));
            }
        }

        public ICompositionElement Origin { get; }
    }

    internal class LazyExportDefinition : ExportDefinition
    {
        private readonly Lazy<IDictionary<string, object>> _metadata;

        public LazyExportDefinition(string contractName, Lazy<IDictionary<string, object>> metadata)
            : base(contractName, null)
        {
            _metadata = metadata;
        }

        public override IDictionary<string, object> Metadata => _metadata.Value ?? MetadataServices.EmptyMetadata
    }
}
