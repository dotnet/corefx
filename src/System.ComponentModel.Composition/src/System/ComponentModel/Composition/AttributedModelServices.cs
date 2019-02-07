// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel.Composition.AttributedModel;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace System.ComponentModel.Composition
{
    public static class AttributedModelServices
    {
        [SuppressMessage("Microsoft.Design", "CA1004")]
        public static TMetadataView GetMetadataView<TMetadataView>(IDictionary<string, object> metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }
            Contract.Ensures(Contract.Result<TMetadataView>() != null);

            return MetadataViewProvider.GetMetadataView<TMetadataView>(metadata);
        }

        public static ComposablePart CreatePart(object attributedPart)
        {
            if (attributedPart == null)
            {
                throw new ArgumentNullException(nameof(attributedPart));
            }
            Contract.Ensures(Contract.Result<ComposablePart>() != null);

            return AttributedModelDiscovery.CreatePart(attributedPart);
        }

        public static ComposablePart CreatePart(object attributedPart, ReflectionContext reflectionContext)
        {
            if (attributedPart == null)
            {
                throw new ArgumentNullException(nameof(attributedPart));
            }
            if (reflectionContext == null)
            {
                throw new ArgumentNullException(nameof(reflectionContext));
            }
            Contract.Ensures(Contract.Result<ComposablePart>() != null);

            return AttributedModelDiscovery.CreatePart(attributedPart, reflectionContext);
        }

        public static ComposablePart CreatePart(ComposablePartDefinition partDefinition, object attributedPart)
        {
            if (partDefinition == null)
            {
                throw new ArgumentNullException(nameof(partDefinition));
            }
            if (attributedPart == null)
            {
                throw new ArgumentNullException(nameof(attributedPart));
            }
            Contract.Ensures(Contract.Result<ComposablePart>() != null);

            if (partDefinition is ReflectionComposablePartDefinition reflectionComposablePartDefinition)
            {
                return AttributedModelDiscovery.CreatePart(reflectionComposablePartDefinition, attributedPart);
            }

            throw ExceptionBuilder.CreateReflectionModelInvalidPartDefinition(nameof(partDefinition), partDefinition.GetType());
        }

        public static ComposablePartDefinition CreatePartDefinition(Type type, ICompositionElement origin)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            Contract.Ensures(Contract.Result<ComposablePartDefinition>() != null);

            return AttributedModelServices.CreatePartDefinition(type, origin, false);
        }

        public static ComposablePartDefinition CreatePartDefinition(Type type, ICompositionElement origin, bool ensureIsDiscoverable)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (ensureIsDiscoverable)
            {
                return AttributedModelDiscovery.CreatePartDefinitionIfDiscoverable(type, origin);
            }
            else
            {
                return AttributedModelDiscovery.CreatePartDefinition(type, null, false, origin);
            }
        }

        public static string GetTypeIdentity(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

            return ContractNameServices.GetTypeIdentity(type);
        }

        public static string GetTypeIdentity(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

            return ContractNameServices.GetTypeIdentityFromMethod(method);
        }

        public static string GetContractName(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

            return AttributedModelServices.GetTypeIdentity(type);
        }

        public static ComposablePart AddExportedValue<T>(this CompositionBatch batch, T exportedValue)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }
            Contract.Ensures(Contract.Result<ComposablePart>() != null);

            string contractName = AttributedModelServices.GetContractName(typeof(T));

            return batch.AddExportedValue<T>(contractName, exportedValue);
        }

        public static void ComposeExportedValue<T>(this CompositionContainer container, T exportedValue)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue<T>(exportedValue);
            container.Compose(batch);
        }

        public static ComposablePart AddExportedValue<T>(this CompositionBatch batch, string contractName, T exportedValue)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }
            Contract.Ensures(Contract.Result<ComposablePart>() != null);

            string typeIdentity = AttributedModelServices.GetTypeIdentity(typeof(T));

            IDictionary<string, object> metadata = new Dictionary<string, object>
            {
                { CompositionConstants.ExportTypeIdentityMetadataName, typeIdentity }
            };

            return batch.AddExport(new Export(contractName, metadata, () => exportedValue));
        }

        public static void ComposeExportedValue<T>(this CompositionContainer container, string contractName, T exportedValue)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue<T>(contractName, exportedValue);
            container.Compose(batch);
        }

        public static ComposablePart AddPart(this CompositionBatch batch, object attributedPart)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }
            if (attributedPart == null)
            {
                throw new ArgumentNullException(nameof(attributedPart));
            }
            Contract.Ensures(Contract.Result<ComposablePart>() != null);

            ComposablePart part = CreatePart(attributedPart);
            batch.AddPart(part);

            return part;
        }

        public static void ComposeParts(this CompositionContainer container, params object[] attributedParts)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (attributedParts == null)
            {
                throw new ArgumentNullException(nameof(attributedParts));
            }

            if (!Contract.ForAll(attributedParts, (value) => value != null))
            {
                throw ExceptionBuilder.CreateContainsNullElement(nameof(attributedParts));
            }

            CompositionBatch batch = new CompositionBatch(
                attributedParts.Select(attributedPart => AttributedModelServices.CreatePart(attributedPart)).ToArray(),
                Enumerable.Empty<ComposablePart>());

            container.Compose(batch);
        }

        /// <summary>
        ///     Satisfies the imports of the specified attributed object exactly once and they will not
        ///     ever be recomposed.
        /// </summary>
        /// <param name="attributedPart">
        ///     The attributed object to set the imports.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="compositionService"/> or <paramref name="attributedPart"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="CompositionException">
        ///     An error occurred during composition. <see cref="CompositionException.Errors"/> will
        ///     contain a collection of errors that occurred.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="ICompositionService"/> has been disposed of.
        /// </exception>
        public static ComposablePart SatisfyImportsOnce(this ICompositionService compositionService, object attributedPart)
        {
            if (compositionService == null)
            {
                throw new ArgumentNullException(nameof(compositionService));
            }
            if (attributedPart == null)
            {
                throw new ArgumentNullException(nameof(attributedPart));
            }
            Contract.Ensures(Contract.Result<ComposablePart>() != null);

            ComposablePart part = AttributedModelServices.CreatePart(attributedPart);
            compositionService.SatisfyImportsOnce(part);

            return part;
        }

        /// <summary>
        ///     Satisfies the imports of the specified attributed object exactly once and they will not
        ///     ever be recomposed.
        /// </summary>
        /// <param name="attributedPart">
        ///     The attributed object to set the imports.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="compositionService"/> or <paramref name="attributedPart"/>  or <paramref name="reflectionContext"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="CompositionException">
        ///     An error occurred during composition. <see cref="CompositionException.Errors"/> will
        ///     contain a collection of errors that occurred.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="ICompositionService"/> has been disposed of.
        /// </exception>
        public static ComposablePart SatisfyImportsOnce(this ICompositionService compositionService, object attributedPart, ReflectionContext reflectionContext)
        {
            if (compositionService == null)
            {
                throw new ArgumentNullException(nameof(compositionService));
            }
            if (attributedPart == null)
            {
                throw new ArgumentNullException(nameof(attributedPart));
            }
            if (reflectionContext == null)
            {
                throw new ArgumentNullException(nameof(reflectionContext));
            }
            Contract.Ensures(Contract.Result<ComposablePart>() != null);

            ComposablePart part = AttributedModelServices.CreatePart(attributedPart, reflectionContext);
            compositionService.SatisfyImportsOnce(part);

            return part;
        }

        /// <summary>
        /// Determines whether the specified part exports the specified contract.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// 	<c>true</c> if the specified part exports the specified contract; otherwise, <c>false</c>.
        /// </returns>
        public static bool Exports(this ComposablePartDefinition part, Type contractType)
        {
            if (part == null)
            {
                throw new ArgumentNullException(nameof(part));
            }
            if (contractType == null)
            {
                throw new ArgumentNullException(nameof(contractType));
            }

            return part.Exports(AttributedModelServices.GetContractName(contractType));
        }

        /// <summary>
        /// Determines whether the specified part exports the specified contract.
        /// </summary>
        /// <typeparam name="T">Type of the contract.</typeparam>
        /// <param name="part">The part.</param>
        /// <returns>
        /// 	<c>true</c> if the specified part exports the specified contract; otherwise, <c>false</c>.
        /// </returns>
        public static bool Exports<T>(this ComposablePartDefinition part)
        {
            if (part == null)
            {
                throw new ArgumentNullException(nameof(part));
            }

            return part.Exports(typeof(T));
        }

        /// <summary>
        /// Determines whether the specified part imports the specified contract.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// 	<c>true</c> if the specified part imports the specified contract; otherwise, <c>false</c>.
        /// </returns>
        public static bool Imports(this ComposablePartDefinition part, Type contractType)
        {
            if (part == null)
            {
                throw new ArgumentNullException(nameof(part));
            }
            if (contractType == null)
            {
                throw new ArgumentNullException(nameof(contractType));
            }

            return part.Imports(AttributedModelServices.GetContractName(contractType));
        }

        /// <summary>
        /// Determines whether the specified part imports the specified contract.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <typeparam name="T">Type of the contract.</typeparam>
        /// <returns>
        /// 	<c>true</c> if the specified part imports the specified contract; otherwise, <c>false</c>.
        /// </returns>
        public static bool Imports<T>(this ComposablePartDefinition part)
        {
            if (part == null)
            {
                throw new ArgumentNullException(nameof(part));
            }

            return part.Imports(typeof(T));
        }

        /// <summary>
        /// Determines whether the specified part imports the specified contract with the given cardinality.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="importCardinality">The import cardinality.</param>
        /// <returns>
        /// 	<c>true</c> if the specified part imports the specified contract with the given cardinality; otherwise, <c>false</c>.
        /// </returns>
        public static bool Imports(this ComposablePartDefinition part, Type contractType, ImportCardinality importCardinality)
        {
            if (part == null)
            {
                throw new ArgumentNullException(nameof(part));
            }
            if (contractType == null)
            {
                throw new ArgumentNullException(nameof(contractType));
            }

            return part.Imports(AttributedModelServices.GetContractName(contractType), importCardinality);
        }

        /// <summary>
        /// Determines whether the specified part imports the specified contract with the given cardinality.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <typeparam name="T">Type of the contract.</typeparam>
        /// <param name="importCardinality">The import cardinality.</param>
        /// <returns>
        /// 	<c>true</c> if the specified part imports the specified contract with the given cardinality; otherwise, <c>false</c>.
        /// </returns>
        public static bool Imports<T>(this ComposablePartDefinition part, ImportCardinality importCardinality)
        {
            if (part == null)
            {
                throw new ArgumentNullException(nameof(part));
            }

            return part.Imports(typeof(T), importCardinality);
        }
    }
}
