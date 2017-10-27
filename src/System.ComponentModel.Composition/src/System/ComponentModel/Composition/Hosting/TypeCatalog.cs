// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.AttributedModel;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
    /// <summary>
    ///     An immutable ComposablePartCatalog created from a type array or a list of managed types.  This class is threadsafe.
    ///     It is Disposable.
    /// </summary>
    [DebuggerTypeProxy(typeof(ComposablePartCatalogDebuggerProxy))]
    public class TypeCatalog : ComposablePartCatalog, ICompositionElement
    {
        private readonly object _thisLock = new object();
        private Type[] _types = null;
        private volatile List<ComposablePartDefinition> _parts;
        private volatile bool _isDisposed = false;
        private readonly ICompositionElement _definitionOrigin;
        private readonly Lazy<IDictionary<string, List<ComposablePartDefinition>>> _contractPartIndex;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeCatalog"/> class 
        ///     with the specified types.
        /// </summary>
        /// <param name="types">
        ///     An <see cref="Array"/> of attributed <see cref="Type"/> objects to add to the 
        ///     <see cref="TypeCatalog"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="types"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="types"/> contains an element that is <see langword="null"/>.
        ///     <para>
        ///         -or-
        ///     </para>
        ///     <paramref name="types"/> contains an element that was loaded in the Reflection-only context.
        /// </exception>
        public TypeCatalog(params Type[] types) : this((IEnumerable<Type>)types)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeCatalog"/> class
        ///     with the specified types.
        /// </summary>
        /// <param name="types">
        ///     An <see cref="IEnumerable{T}"/> of attributed <see cref="Type"/> objects to add 
        ///     to the <see cref="TypeCatalog"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="types"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="types"/> contains an element that is <see langword="null"/>.
        ///     <para>
        ///         -or-
        ///     </para>
        ///     <paramref name="types"/> contains an element that was loaded in the reflection-only context.
        /// </exception>
        public TypeCatalog(IEnumerable<Type> types)
        {
            Requires.NotNull(types, "types");

            InitializeTypeCatalog(types);

            this._definitionOrigin = this;
            this._contractPartIndex = new Lazy<IDictionary<string, List<ComposablePartDefinition>>>(this.CreateIndex, true);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeCatalog"/> class
        ///     with the specified types.
        /// </summary>
        /// <param name="types">
        ///     An <see cref="IEnumerable{T}"/> of attributed <see cref="Type"/> objects to add 
        ///     to the <see cref="TypeCatalog"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="types"/> is <see langword="null"/>.
        /// </exception>
        /// <param name="definitionOrigin">
        ///     The <see cref="ICompositionElement"/> CompositionElement used by Diagnostics to identify the source for parts.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     <paramref name="types"/> contains an element that is <see langword="null"/>.
        /// </exception>
        public TypeCatalog(IEnumerable<Type> types, ICompositionElement definitionOrigin)
        {
            Requires.NotNull(types, "types");
            Requires.NotNull(definitionOrigin, "definitionOrigin");

            InitializeTypeCatalog(types);

            this._definitionOrigin = definitionOrigin;
            this._contractPartIndex = new Lazy<IDictionary<string, List<ComposablePartDefinition>>>(this.CreateIndex, true);
        }

#if FEATURE_REFLECTIONCONTEXT
        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeCatalog"/> class
        ///     with the specified types.
        /// </summary>
        /// <param name="types">
        ///     An <see cref="IEnumerable{T}"/> of attributed <see cref="Type"/> objects to add 
        ///     to the <see cref="TypeCatalog"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="types"/> is <see langword="null"/>.
        /// </exception>
        /// <param name="reflectionContext">
        ///     The <see cref="ReflectionContext"/> a context used by the catalog when 
        ///     interpreting the types to inject attributes into the type definition.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     <paramref name="types"/> contains an element that is <see langword="null"/>.
        /// </exception>
        public TypeCatalog(IEnumerable<Type> types, ReflectionContext reflectionContext)
        {
            Requires.NotNull(types, "types");
            Requires.NotNull(reflectionContext, "reflectionContext");

            InitializeTypeCatalog(types, reflectionContext);

            this._definitionOrigin = this;
            this._contractPartIndex = new Lazy<IDictionary<string, List<ComposablePartDefinition>>>(this.CreateIndex, true);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeCatalog"/> class
        ///     with the specified types.
        /// </summary>
        /// <param name="types">
        ///     An <see cref="IEnumerable{T}"/> of attributed <see cref="Type"/> objects to add 
        ///     to the <see cref="TypeCatalog"/>.
        /// </param>
        /// <param name="reflectionContext">
        ///     The <see cref="ReflectionContext"/> a context used by the catalog when 
        ///     interpreting the types to inject attributes into the type definition.
        /// </param>
        /// <param name="definitionOrigin">
        ///     The <see cref="ICompositionElement"/> CompositionElement used by Diagnostics to identify the source for parts.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="types"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="types"/> contains an element that is <see langword="null"/>.
        /// </exception>
        public TypeCatalog(IEnumerable<Type> types, ReflectionContext reflectionContext, ICompositionElement definitionOrigin)
        {
            Requires.NotNull(types, "types");
            Requires.NotNull(reflectionContext, "reflectionContext");
            Requires.NotNull(definitionOrigin, "definitionOrigin");

            InitializeTypeCatalog(types, reflectionContext);

            this._definitionOrigin = definitionOrigin;
            this._contractPartIndex = new Lazy<IDictionary<string, List<ComposablePartDefinition>>>(this.CreateIndex, true);
        }

        private void InitializeTypeCatalog(IEnumerable<Type> types, ReflectionContext reflectionContext)
        {
            var typesList = new List<Type>();
            foreach(var type in types)
            {
                if (type == null)
                {
                    throw ExceptionBuilder.CreateContainsNullElement("types");
                }
#if FEATURE_REFLECTIONONLY
                if (type.Assembly.ReflectionOnly)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, SR.Argument_ElementReflectionOnlyType, "types"), "types");
                }
#endif
                var typeInfo = type.GetTypeInfo();
                var lclType = (reflectionContext != null) ? reflectionContext.MapType(typeInfo) : typeInfo;
                
                // It is valid for the reflectionContext to delete types by mapping them to null
                if(lclType != null)
                {
#if FEATURE_REFLECTIONONLY
                    // The final mapped type may be activated so we check to see if it is in a reflect only assembly
                    if (lclType.Assembly.ReflectionOnly)
                    {
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, SR.Argument_ReflectionContextReturnsReflectionOnlyType, "reflectionContext"), "reflectionContext");
                    }
#endif
                    typesList.Add(lclType);
                }
            }
            this._types = typesList.ToArray();
        }
#endif //FEATURE_REFLECTIONCONTEXT

        private void InitializeTypeCatalog(IEnumerable<Type> types)
        {
            foreach(var type in types)
            {
                if (type == null)
                {
                    throw ExceptionBuilder.CreateContainsNullElement("types");
                }
#if FEATURE_REFLECTIONONLY
                else if (type.Assembly.ReflectionOnly)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, SR.Argument_ElementReflectionOnlyType, "types"), "types");
                }
#endif //FEATURE_REFLECTIONONLY
            }
            this._types = types.ToArray();
        }


        public override IEnumerator<ComposablePartDefinition> GetEnumerator()
        {
            this.ThrowIfDisposed();
            return this.PartsInternal.GetEnumerator();
        }

        /// <summary>
        ///     Gets the display name of the type catalog.
        /// </summary>
        /// <value>
        ///     A <see cref="String"/> containing a human-readable display name of the <see cref="TypeCatalog"/>.
        /// </value>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        string ICompositionElement.DisplayName
        {
            get { return this.GetDisplayName(); }
        }

        /// <summary>
        ///     Gets the composition element from which the type catalog originated.
        /// </summary>
        /// <value>
        ///     This property always returns <see langword="null"/>.
        /// </value>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        ICompositionElement ICompositionElement.Origin
        {
            get { return null; }
        }

        private IEnumerable<ComposablePartDefinition> PartsInternal
        {
            get
            {
                if (this._parts == null)
                {
                    lock (this._thisLock)
                    {
                        if (this._parts == null)
                        {
                            Assumes.NotNull(this._types);

                            var collection = new List<ComposablePartDefinition>();
                            foreach (Type type in this._types)
                            {
                                var definition = AttributedModelDiscovery.CreatePartDefinitionIfDiscoverable(type, _definitionOrigin);
                                if (definition != null)
                                {
                                    collection.Add(definition);
                                }
                            }
                            Thread.MemoryBarrier();

                            this._types = null;
                            this._parts = collection;
                        }
                    }
                }

                return this._parts;
            }
        }

        internal override IEnumerable<ComposablePartDefinition> GetCandidateParts(ImportDefinition definition)
        {
            Assumes.NotNull(definition);

            string contractName = definition.ContractName;
            if (string.IsNullOrEmpty(contractName))
            {
                return this.PartsInternal;
            }

            string genericContractName = definition.Metadata.GetValue<string>(CompositionConstants.GenericContractMetadataName);

            List<ComposablePartDefinition> nonGenericMatches = this.GetCandidateParts(contractName);
            List<ComposablePartDefinition> genericMatches = this.GetCandidateParts(genericContractName);

            return nonGenericMatches.ConcatAllowingNull(genericMatches);
        }

        private List<ComposablePartDefinition> GetCandidateParts(string contractName)
        {
            if (contractName == null)
            {
                return null;
            }

            List<ComposablePartDefinition> contractCandidateParts = null;
            this._contractPartIndex.Value.TryGetValue(contractName, out contractCandidateParts);
            return contractCandidateParts;
        }

        private IDictionary<string, List<ComposablePartDefinition>> CreateIndex()
        {
            Dictionary<string, List<ComposablePartDefinition>> index = new Dictionary<string, List<ComposablePartDefinition>>(StringComparers.ContractName);

            foreach (var part in this.PartsInternal)
            {
                foreach (string contractName in part.ExportDefinitions.Select(export => export.ContractName).Distinct())
                {
                    List<ComposablePartDefinition> contractParts = null;
                    if (!index.TryGetValue(contractName, out contractParts))
                    {
                        contractParts = new List<ComposablePartDefinition>();
                        index.Add(contractName, contractParts);
                    }
                    contractParts.Add(part);
                }
            }
            return index;
        }

        /// <summary>
        ///     Returns a string representation of the type catalog.
        /// </summary>
        /// <returns>
        ///     A <see cref="String"/> containing the string representation of the <see cref="TypeCatalog"/>.
        /// </returns>
        public override string ToString()
        {
            return this.GetDisplayName();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._isDisposed = true;
            }

            base.Dispose(disposing);
        }

        private string GetDisplayName()
        {
            return String.Format(CultureInfo.CurrentCulture,
                                SR.TypeCatalog_DisplayNameFormat,
                                this.GetType().Name,
                                this.GetTypesDisplay());
        }

        private string GetTypesDisplay()
        {
            int count = this.PartsInternal.Count();
            if (count == 0)
            {
                return SR.TypeCatalog_Empty;
            }

            const int displayCount = 2;
            StringBuilder builder = new StringBuilder();
            foreach (ReflectionComposablePartDefinition definition in this.PartsInternal.Take(displayCount))
            {
                if (builder.Length > 0)
                {
                    builder.Append(CultureInfo.CurrentCulture.TextInfo.ListSeparator);
                    builder.Append(" ");
                }

                builder.Append(definition.GetPartType().GetDisplayName());
            }

            if (count > displayCount)
            {   // Add an elipse to indicate that there 
                // are more types than actually listed
                builder.Append(CultureInfo.CurrentCulture.TextInfo.ListSeparator);
                builder.Append(" ...");
            }

            return builder.ToString();
        }

        [DebuggerStepThrough]
        [ContractArgumentValidator]
        [SuppressMessage("Microsoft.Contracts", "CC1053", Justification = "Suppressing warning because this validator has no public contract")]
        private void ThrowIfDisposed()
        {
            if (this._isDisposed)
            {
                throw ExceptionBuilder.CreateObjectDisposed(this);
            }
        }
    }
}
