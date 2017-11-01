// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
    public partial class ImportEngine 
    {
        /// <summary>
        ///     Used to wrap the start and stop of enforcing export changes don't
        ///     break required imports. This context is stored in a AtomicComposition.
        /// </summary>
        private class EngineContext
        {
            private ImportEngine _importEngine;
            private List<PartManager> _addedPartManagers = new List<PartManager>();
            private List<PartManager> _removedPartManagers = new List<PartManager>();
            private EngineContext _parentEngineContext;

            public EngineContext(ImportEngine importEngine, EngineContext parentEngineContext)
            {
                this._importEngine = importEngine;
                this._parentEngineContext = parentEngineContext;
            }

            public void AddPartManager(PartManager part)
            {
                Assumes.NotNull(part);
                if (!this._removedPartManagers.Remove(part))
                {
                    this._addedPartManagers.Add(part);
                }
            }

            public void RemovePartManager(PartManager part)
            {
                Assumes.NotNull(part);
                if (!this._addedPartManagers.Remove(part))
                {
                    this._removedPartManagers.Add(part);
                }
            }

            public IEnumerable<PartManager> GetAddedPartManagers()
            {
                if (this._parentEngineContext != null)
                {
                    return this._addedPartManagers.ConcatAllowingNull(this._parentEngineContext.GetAddedPartManagers());
                }
                return this._addedPartManagers;
            }

            public IEnumerable<PartManager> GetRemovedPartManagers()
            {
                if (this._parentEngineContext != null)
                {
                    return this._removedPartManagers.ConcatAllowingNull(this._parentEngineContext.GetRemovedPartManagers());
                }
                return this._removedPartManagers;
            }

            public void Complete()
            {
                foreach (var partManager in this._addedPartManagers)
                {
                    this._importEngine.StartSatisfyingImports(partManager, null);
                }

                foreach (var partManager in this._removedPartManagers)
                {
                    this._importEngine.StopSatisfyingImports(partManager, null);
                }
            }
        }
    }
}
