// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
    /// <summary>
    ///     A mutable collection of <see cref="ComposablePartCatalog"/>s.  
    /// </summary>
    /// <remarks>
    ///     This type is thread safe.
    /// </remarks>
    public class CompositionService : ICompositionService, IDisposable
    {
        private CompositionContainer _compositionContainer = null;
        private INotifyComposablePartCatalogChanged _notifyCatalog = null;

        internal CompositionService(ComposablePartCatalog composablePartCatalog)
        {
            Assumes.NotNull(composablePartCatalog);
            this._notifyCatalog = composablePartCatalog as INotifyComposablePartCatalogChanged;
            try
            {
                if(this._notifyCatalog != null)
                {
                    this._notifyCatalog.Changing += this.OnCatalogChanging;
                }

                var compositionOptions = CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe | CompositionOptions.ExportCompositionService;
                var compositionContainer = new CompositionContainer(composablePartCatalog, compositionOptions);
    
                this._compositionContainer = compositionContainer;
            }
            catch
            {
                if(this._notifyCatalog != null)
                {
                    this._notifyCatalog.Changing -= this.OnCatalogChanging;
                }
                throw;
            }
        }

        public void SatisfyImportsOnce(ComposablePart part)
        {
            Requires.NotNull(part, "part");
            Assumes.NotNull(this._compositionContainer);
            this._compositionContainer.SatisfyImportsOnce(part);
        }

        public void Dispose()
        {
            Assumes.NotNull(this._compositionContainer);
            
            // Delegates are cool there is no concern if you try to remove an item from them and they don't exist
            if (this._notifyCatalog != null)
            {
                this._notifyCatalog.Changing -= this.OnCatalogChanging;
            }
            this._compositionContainer.Dispose();
        }

        private void OnCatalogChanging(object sender, ComposablePartCatalogChangeEventArgs e)
        {
            throw new ChangeRejectedException(SR.NotSupportedCatalogChanges);
        }
    }
}
