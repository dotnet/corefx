// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Microsoft.Internal;
using Microsoft.Internal.Collections;
using System.Collections;

namespace System.ComponentModel.Composition.Hosting
{
    /// <summary>
    ///     This class implements a threadsafe ICollection{T} of ComposablePartCatalog.
    ///     It is exposed as an ICollection(ComposablePartCatalog)
    ///     It is threadsafe, notifications are not marshalled using a SynchronizationContext.
    ///     It is Disposable.
    /// </summary>
    internal class ComposablePartCatalogCollection : ICollection<ComposablePartCatalog>, INotifyComposablePartCatalogChanged, IDisposable
    {
        private readonly Lock _lock = new Lock();
        private Action<ComposablePartCatalogChangeEventArgs> _onChanged;
        private Action<ComposablePartCatalogChangeEventArgs> _onChanging;
        private List<ComposablePartCatalog> _catalogs = new List<ComposablePartCatalog>();
        private volatile bool _isCopyNeeded = false;
        private volatile bool _isDisposed = false;
        private bool _hasChanged = false;

        public ComposablePartCatalogCollection(
            IEnumerable<ComposablePartCatalog> catalogs,
            Action<ComposablePartCatalogChangeEventArgs> onChanged,
            Action<ComposablePartCatalogChangeEventArgs> onChanging)
        {
            catalogs = catalogs ?? Enumerable.Empty<ComposablePartCatalog>();
            this._catalogs = new List<ComposablePartCatalog>(catalogs);
            this._onChanged = onChanged;
            this._onChanging = onChanging;

            SubscribeToCatalogNotifications(catalogs);
        }

        public void Add(ComposablePartCatalog item)
        {
            Requires.NotNull(item, "item");

            this.ThrowIfDisposed();

            var addedParts = new Lazy<IEnumerable<ComposablePartDefinition>>(() => item.ToArray(), LazyThreadSafetyMode.PublicationOnly);

            using (var atomicComposition = new AtomicComposition())
            {
                this.RaiseChangingEvent(addedParts, null, atomicComposition);
 
                using (new WriteLock(this._lock))
                {
                    if (this._isCopyNeeded)
                    {
                        this._catalogs = new List<ComposablePartCatalog>(this._catalogs);
                        this._isCopyNeeded = false;
                    }
                    this._hasChanged = true;
                    this._catalogs.Add(item);
                }
                
                this.SubscribeToCatalogNotifications(item);

                // Complete after the catalog changes are written
                atomicComposition.Complete();
            }

            this.RaiseChangedEvent(addedParts, null);
        }

        /// <summary>
        /// Notify when the contents of the Catalog has changed.
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;

        /// <summary>
        /// Notify when the contents of the Catalog has changing.
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

        public void Clear()
        {
            this.ThrowIfDisposed();

            // No action is required if we are already empty
            ComposablePartCatalog[] catalogs = null;
            using (new ReadLock(this._lock))
            {
                if (this._catalogs.Count == 0)
                {
                    return;
                }
                catalogs = this._catalogs.ToArray();
            }

            // We are doing this outside of the lock, so it's possible that the catalog will continute propagating events from things
            // we are about to unsubscribe from. Given the non-specificity of our event, in the worst case scenario we would simply fire 
            // unnecessary events.
            var removedParts = new Lazy<IEnumerable<ComposablePartDefinition>>(() => catalogs.SelectMany(catalog => catalog).ToArray(), LazyThreadSafetyMode.PublicationOnly);

            // Validate the changes before applying them
            using (var atomicComposition = new AtomicComposition())
            {
                this.RaiseChangingEvent(null, removedParts, atomicComposition);
                this.UnsubscribeFromCatalogNotifications(catalogs);

                using (new WriteLock(this._lock))
                {
                    this._catalogs = new List<ComposablePartCatalog>();

                    this._isCopyNeeded = false;
                    this._hasChanged = true;
                }

                // Complete after the catalog changes are written
                atomicComposition.Complete();
            }

            this.RaiseChangedEvent(null, removedParts);
        }

        public bool Contains(ComposablePartCatalog item)
        {
            Requires.NotNull(item, "item");

            this.ThrowIfDisposed();

            using (new ReadLock(this._lock))
            {
                return this._catalogs.Contains(item);
            }
        }

        public void CopyTo(ComposablePartCatalog[] array, int arrayIndex)
        {
            this.ThrowIfDisposed();

            using (new ReadLock(this._lock))
            {
                this._catalogs.CopyTo(array, arrayIndex);
            }
        }

        public int Count
        {
            get 
            {
                this.ThrowIfDisposed();

                using (new ReadLock(this._lock))
                {
                    return this._catalogs.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                this.ThrowIfDisposed();

                return false;
            }
        }

        public bool Remove(ComposablePartCatalog item)
        {
            Requires.NotNull(item, "item");

            this.ThrowIfDisposed();

            using (new ReadLock(this._lock))
            {
                if (!this._catalogs.Contains(item))
                {
                    return false;
                }
            }

            bool isSuccessfulRemoval = false;

            var removedParts = new Lazy<IEnumerable<ComposablePartDefinition>>(() => item.ToArray(), LazyThreadSafetyMode.PublicationOnly);
            using (var atomicComposition = new AtomicComposition())
            {
                this.RaiseChangingEvent(null, removedParts, atomicComposition);

                using (new WriteLock(this._lock))
                {
                    if (_isCopyNeeded)
                    {
                        this._catalogs = new List<ComposablePartCatalog>(this._catalogs);
                        this._isCopyNeeded = false;
                    }

                    isSuccessfulRemoval = this._catalogs.Remove(item);
                    if (isSuccessfulRemoval)
                    {
                        this._hasChanged = true;
                    }
                }

                this.UnsubscribeFromCatalogNotifications(item);

                // Complete after the catalog changes are written
                atomicComposition.Complete();
            }

            this.RaiseChangedEvent(null, removedParts);

            return isSuccessfulRemoval;
        }

        internal bool HasChanged
        {
            get
            {
                this.ThrowIfDisposed();

                using (new ReadLock(this._lock))
                {
                    return this._hasChanged;
                }
            }
        }

        public IEnumerator<ComposablePartCatalog> GetEnumerator()
        {
            this.ThrowIfDisposed();

            using (new WriteLock(this._lock))
            {
                IEnumerator<ComposablePartCatalog> enumerator = this._catalogs.GetEnumerator();
                this._isCopyNeeded = true;
                return enumerator;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!this._isDisposed)
                {
                    bool disposeLock = false;
                    IEnumerable<ComposablePartCatalog> catalogs = null;
                    try
                    {
                        using (new WriteLock(this._lock))
                        {
                            if (!this._isDisposed)
                            {
                                disposeLock = true;

                                catalogs = this._catalogs;
                                this._catalogs = null;

                                this._isDisposed = true;
                            }
                        }
                    }
                    finally
                    {
                        if (catalogs != null)
                        {
                            this.UnsubscribeFromCatalogNotifications(catalogs);
                            catalogs.ForEach(catalog => catalog.Dispose());
                        }

                        if (disposeLock)
                        {
                            this._lock.Dispose();
                        }
                    }
                }
            }
        }

        private void RaiseChangedEvent(
            Lazy<IEnumerable<ComposablePartDefinition>> addedDefinitions,
            Lazy<IEnumerable<ComposablePartDefinition>> removedDefinitions)
        {
            if (this._onChanged == null || this.Changed == null)
            {
                return;
            }

            var added = (addedDefinitions == null ? Enumerable.Empty<ComposablePartDefinition>() : addedDefinitions.Value);
            var removed = (removedDefinitions == null ? Enumerable.Empty<ComposablePartDefinition>() : removedDefinitions.Value);

            this._onChanged.Invoke(new ComposablePartCatalogChangeEventArgs(added, removed, null));
        }

        public void OnChanged(object sender, ComposablePartCatalogChangeEventArgs e)
        {
            var changedEvent = this.Changed;
            if (changedEvent != null)
            {
                changedEvent(sender, e);
            }
        }

        private void RaiseChangingEvent(
           Lazy<IEnumerable<ComposablePartDefinition>> addedDefinitions,
           Lazy<IEnumerable<ComposablePartDefinition>> removedDefinitions,
           AtomicComposition atomicComposition)
        {
            if (this._onChanging == null || this.Changing == null)
            {
                return;
            }
            var added = (addedDefinitions == null ? Enumerable.Empty<ComposablePartDefinition>() : addedDefinitions.Value);
            var removed = (removedDefinitions == null ? Enumerable.Empty<ComposablePartDefinition>() : removedDefinitions.Value);

            this._onChanging.Invoke(new ComposablePartCatalogChangeEventArgs(added, removed, atomicComposition));
        }

        public void OnChanging(object sender, ComposablePartCatalogChangeEventArgs e)
        {
            var changingEvent = this.Changing;
            if (changingEvent != null)
            {
                changingEvent(sender, e);
            }
        }

        private void OnContainedCatalogChanged(object sender, ComposablePartCatalogChangeEventArgs e)
        {
            if (this._onChanged == null || this.Changed == null)
            {
                return;
            }

            this._onChanged.Invoke(e);
        }

        private void OnContainedCatalogChanging(object sender, ComposablePartCatalogChangeEventArgs e)
        {
            if (this._onChanging == null || this.Changing == null)
            {
                return;
            }

            this._onChanging.Invoke(e);
        }

        private void SubscribeToCatalogNotifications(ComposablePartCatalog catalog)
        {
            INotifyComposablePartCatalogChanged notifyCatalog = catalog as INotifyComposablePartCatalogChanged;
            if (notifyCatalog != null)
            {
                notifyCatalog.Changed += this.OnContainedCatalogChanged;
                notifyCatalog.Changing += this.OnContainedCatalogChanging;
            }
        }

        private void SubscribeToCatalogNotifications(IEnumerable<ComposablePartCatalog> catalogs)
        {
            foreach (var catalog in catalogs)
            {
                SubscribeToCatalogNotifications(catalog);
            }
        }

        private void UnsubscribeFromCatalogNotifications(ComposablePartCatalog catalog)
        {
            INotifyComposablePartCatalogChanged notifyCatalog = catalog as INotifyComposablePartCatalogChanged;
            if (notifyCatalog != null)
            {
                notifyCatalog.Changed -= this.OnContainedCatalogChanged;
                notifyCatalog.Changing -= this.OnContainedCatalogChanging;
            }
        }

        private void UnsubscribeFromCatalogNotifications(IEnumerable<ComposablePartCatalog> catalogs)
        {
            foreach (var catalog in catalogs)
            {
                UnsubscribeFromCatalogNotifications(catalog);
            }
        }

        private void ThrowIfDisposed()
        {
            if (this._isDisposed)
            {
                throw ExceptionBuilder.CreateObjectDisposed(this);
            }
        }
    }
}
