// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Composition.Hosting
{
    /// <summary>
    ///     Notifications when a ComposablePartCatalog changes.
    /// </summary>
    public interface INotifyComposablePartCatalogChanged
    {
        event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;
        event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;
    }
}
