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
    public partial class FilteredCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        private Func<ComposablePartDefinition, bool> _filter;
        private ComposablePartCatalog _innerCatalog;
        private FilteredCatalog _complement;
        private object _lock = new object();
        private volatile bool _isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredCatalog"/> class.
        /// </summary>
        /// <param name="catalog">The catalog.</param>
        /// <param name="filter">The filter.</param>
        public FilteredCatalog(ComposablePartCatalog catalog, Func<ComposablePartDefinition, bool> filter) : 
            this(catalog, filter, null)
        {
        }

        internal FilteredCatalog(ComposablePartCatalog catalog, Func<ComposablePartDefinition, bool> filter, FilteredCatalog complement)
        {
            Requires.NotNull(catalog, "catalog");
            Requires.NotNull(filter, "filter");

            this._innerCatalog = catalog;
            this._filter = (p) => filter.Invoke(p.GetGenericPartDefinition() ?? p);
            this._complement = complement;

            INotifyComposablePartCatalogChanged notifyCatalog = this._innerCatalog as INotifyComposablePartCatalogChanged;
            if (notifyCatalog != null)
            {
                notifyCatalog.Changed += this.OnChangedInternal;
                notifyCatalog.Changing += this.OnChangingInternal;
            }
        }


        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if(!this._isDisposed)
                    {
                        INotifyComposablePartCatalogChanged notifyCatalog = null;
                        try
                        {
                            lock (this._lock)
                            {
                                if (!this._isDisposed)
                                {
                                    this._isDisposed = true;
                                    notifyCatalog = this._innerCatalog as INotifyComposablePartCatalogChanged;
                                    this._innerCatalog = null;
                                }
                            }
                        }
                        finally
                        {
                            if (notifyCatalog != null)
                            {
                                notifyCatalog.Changed -= this.OnChangedInternal;
                                notifyCatalog.Changing -= this.OnChangingInternal;
                            }
                        }
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
            return this._innerCatalog.Where(this._filter).GetEnumerator();
        }

        /// <summary>
        /// Gets the complement.
        /// </summary>
        /// <value>The complement.</value>
        public FilteredCatalog Complement
        {
            get
            {
                this.ThrowIfDisposed();

                if (this._complement == null)
                {
                    FilteredCatalog complement = new FilteredCatalog(this._innerCatalog, p => !this._filter(p), this);
                    lock (this._lock)
                    {
                        if (this._complement == null)
                        {
                            Thread.MemoryBarrier();
                            this._complement = complement;
                            complement = null;
                        }
                    }

                    if (complement != null)
                    {
                        complement.Dispose();
                    }
                }

                return this._complement;
            }
        }

        /// <summary>
        /// Returns the export definitions that match the constraint defined by the specified definition.
        /// </summary>
        /// <param name="definition">The <see cref="ImportDefinition"/> that defines the conditions of the
        /// <see cref="ExportDefinition"/> objects to return.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="Tuple{T1, T2}"/> containing the
        /// <see cref="ExportDefinition"/> objects and their associated
        /// <see cref="ComposablePartDefinition"/> for objects that match the constraint defined
        /// by <paramref name="definition"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// 	<paramref name="definition"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="ComposablePartCatalog"/> has been disposed of.
        /// </exception>
        /// <remarks>
        /// 	<note type="inheritinfo">
        /// Overriders of this property should never return <see langword="null"/>, if no
        /// <see cref="ExportDefinition"/> match the conditions defined by
        /// <paramref name="definition"/>, return an empty <see cref="IEnumerable{T}"/>.
        /// </note>
        /// </remarks>
        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            this.ThrowIfDisposed();
            Requires.NotNull(definition, "definition");

            var exports = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();
            foreach(var export in this._innerCatalog.GetExports(definition))
            {
                if (this._filter(export.Item1))
                {
                    exports.Add(export);
                }
            }

            return exports;
        }

        /// <summary>
        /// Notify when the contents of the Catalog has changed.
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;


        /// <summary>
        /// Notify when the contents of the Catalog is changing.
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;


        /// <summary>
        /// Raises the <see cref="E:Changed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.Composition.Hosting.ComposablePartCatalogChangeEventArgs"/> instance containing the event data.</param>
        protected virtual void OnChanged(ComposablePartCatalogChangeEventArgs e)
        {
            EventHandler<ComposablePartCatalogChangeEventArgs> changedEvent = this.Changed;
            if (changedEvent != null)
            {
                changedEvent.Invoke(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Changing"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.Composition.Hosting.ComposablePartCatalogChangeEventArgs"/> instance containing the event data.</param>
        protected virtual void OnChanging(ComposablePartCatalogChangeEventArgs e)
        {
            EventHandler<ComposablePartCatalogChangeEventArgs> changingEvent = this.Changing;
            if (changingEvent != null)
            {
                changingEvent.Invoke(this, e);
            }
        }

        private void OnChangedInternal(object sender, ComposablePartCatalogChangeEventArgs e)
        {
            var processedArgs = ProcessEventArgs(e);
            if (processedArgs != null)
            {
                this.OnChanged(this.ProcessEventArgs(processedArgs));
            }
        }

        private void OnChangingInternal(object sender, ComposablePartCatalogChangeEventArgs e)
        {
            var processedArgs = ProcessEventArgs(e);
            if (processedArgs != null)
            {
                this.OnChanging(this.ProcessEventArgs(processedArgs));
            }
        }

        private ComposablePartCatalogChangeEventArgs ProcessEventArgs(ComposablePartCatalogChangeEventArgs e)
        {
            // the constructor for ComposablePartCatalogChangeEventArgs takes a snapshot of the arguments, so we don't have to
            var result = new ComposablePartCatalogChangeEventArgs(
                e.AddedDefinitions.Where(this._filter),
                e.RemovedDefinitions.Where(this._filter),
                e.AtomicComposition);

            // Only fire if we need to
            if (result.AddedDefinitions.FastAny() || result.RemovedDefinitions.FastAny())
            {
                return result;
            }
            else
            {
                return null;
            }
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
