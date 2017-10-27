// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
    /// <summary>
    ///     A mutable collection of <see cref="ComposablePartCatalog"/>s.  
    /// </summary>
    /// <remarks>
    ///     This type is thread safe.
    /// </remarks>
    public class AggregateCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        private ComposablePartCatalogCollection _catalogs = null;
        private volatile int _isDisposed = 0;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AggregateCatalog"/> class.
        /// </summary>
        public AggregateCatalog()
            : this((IEnumerable<ComposablePartCatalog>)null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AggregateCatalog"/> class 
        ///     with the specified catalogs.
        /// </summary>
        /// <param name="catalogs">
        ///     An <see cref="Array"/> of <see cref="ComposablePartCatalog"/> objects to add to the 
        ///     <see cref="AggregateCatalog"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="catalogs"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="catalogs"/> contains an element that is <see langword="null"/>.
        /// </exception>
        public AggregateCatalog(params ComposablePartCatalog[] catalogs)
            : this((IEnumerable<ComposablePartCatalog>)catalogs)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AggregateCatalog"/> class
        ///     with the specified catalogs.
        /// </summary>
        /// <param name="catalogs">
        ///     An <see cref="IEnumerable{T}"/> of <see cref="ComposablePartCatalog"/> objects to add
        ///     to the <see cref="AggregateCatalog"/>; or <see langword="null"/> to 
        ///     create an <see cref="AggregateCatalog"/> that is empty.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     <paramref name="catalogs"/> contains an element that is <see langword="null"/>.
        /// </exception>
        public AggregateCatalog(IEnumerable<ComposablePartCatalog> catalogs)
        {
            Requires.NullOrNotNullElements(catalogs, "catalogs");

            this._catalogs = new ComposablePartCatalogCollection(catalogs, this.OnChanged, this.OnChanging);
        }

        /// <summary>
        /// Notify when the contents of the Catalog has changed.
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed
        {
            add
            {
                this._catalogs.Changed += value;
            }
            remove
            {
                this._catalogs.Changed -= value;
            }
        }

        /// <summary>
        /// Notify when the contents of the Catalog has changing.
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing
        {
            add
            {
                this._catalogs.Changing += value;
            }
            remove
            {
                this._catalogs.Changing -= value;
            }
        }

        /// <summary>
        ///     Returns the export definitions that match the constraint defined by the specified definition.
        /// </summary>
        /// <param name="definition">
        ///     The <see cref="ImportDefinition"/> that defines the conditions of the 
        ///     <see cref="ExportDefinition"/> objects to return.
        /// </param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}"/> of <see cref="Tuple{T1, T2}"/> containing the 
        ///     <see cref="ExportDefinition"/> objects and their associated 
        ///     <see cref="ComposablePartDefinition"/> for objects that match the constraint defined 
        ///     by <paramref name="definition"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="definition"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="AggregateCatalog"/> has been disposed of.
        /// </exception>
        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            this.ThrowIfDisposed();

            Requires.NotNull(definition, "definition");

            // We optimize for the case where the result is comparible with the requested cardinality, though we do remain correct in all cases.
            // We do so to avoid any unnecessary allocations
            IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> result = null;
            List<Tuple<ComposablePartDefinition, ExportDefinition>> aggregateResult = null;

            foreach (var catalog in this._catalogs)
            {
                var catalogExports = catalog.GetExports(definition);
                if (catalogExports != ComposablePartCatalog._EmptyExportsList)
                {
                    // ideally this is is the case we will always hit
                    if (result == null)
                    {
                        result = catalogExports;
                    }
                    else
                    {
                        // sadly the result has already been assigned, which means we are in the aggregate case
                        if (aggregateResult == null)
                        {
                            aggregateResult = new List<Tuple<ComposablePartDefinition, ExportDefinition>>(result);
                            result = aggregateResult;
                        }
                        aggregateResult.AddRange(catalogExports);
                    }
                }
            }
            return result ?? ComposablePartCatalog._EmptyExportsList;
        }

        /// <summary>
        ///     Gets the underlying catalogs of the catalog.
        /// </summary>
        /// <value>
        ///     An <see cref="ICollection{T}"/> of underlying <see cref="ComposablePartCatalog"/> objects
        ///     of the <see cref="AggregateCatalog"/>.
        /// </value>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="AggregateCatalog"/> has been disposed of.
        /// </exception>
        public ICollection<ComposablePartCatalog> Catalogs
        {
            get
            {
                this.ThrowIfDisposed();
                Contract.Ensures(Contract.Result<ICollection<ComposablePartCatalog>>() != null);

                return this._catalogs;
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    // NOTE : According to http://msdn.microsoft.com/en-us/library/4bw5ewxy.aspx, the warning is bogus when used with Interlocked API.
#pragma warning disable 420
                    if (Interlocked.CompareExchange(ref this._isDisposed, 1, 0) == 0)
#pragma warning restore 420
                    {
                        this._catalogs.Dispose();
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public override IEnumerator<ComposablePartDefinition> GetEnumerator()
        {
            return this._catalogs.SelectMany(catalog => catalog).GetEnumerator();
        }

        /// <summary>
        ///     Raises the <see cref="INotifyComposablePartCatalogChanged.Changed"/> event.
        /// </summary>
        /// <param name="e">
        ///     An <see cref="ComposablePartCatalogChangeEventArgs"/> containing the data for the event.
        /// </param>
        protected virtual void OnChanged(ComposablePartCatalogChangeEventArgs e)
        {
            this._catalogs.OnChanged(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="INotifyComposablePartCatalogChanged.Changing"/> event.
        /// </summary>
        /// <param name="e">
        ///     An <see cref="ComposablePartCatalogChangeEventArgs"/> containing the data for the event.
        /// </param>
        protected virtual void OnChanging(ComposablePartCatalogChangeEventArgs e)
        {
            this._catalogs.OnChanging(this, e);
        }

        [DebuggerStepThrough]
        [ContractArgumentValidator]
        [SuppressMessage("Microsoft.Contracts", "CC1053", Justification = "Suppressing warning because this validator has no public contract")]
        private void ThrowIfDisposed()
        {
            if (this._isDisposed == 1)
            {
                throw ExceptionBuilder.CreateObjectDisposed(this);
            }
        }
    }
}