// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Internal;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Composition.Hosting.Core
{
    internal class CycleBreakingExportDescriptor : ExportDescriptor
    {
        private readonly Lazy<ExportDescriptor> _exportDescriptor;

        public CycleBreakingExportDescriptor(Lazy<ExportDescriptor> exportDescriptor)
        {
            _exportDescriptor = exportDescriptor;
        }

        public override CompositeActivator Activator
        {
            get
            {
                if (!_exportDescriptor.IsValueCreated)
                    return Activate;

                return _exportDescriptor.Value.Activator;
            }
        }

        public override IDictionary<string, object> Metadata
        {
            get
            {
                if (!_exportDescriptor.IsValueCreated)
                    return new CycleBreakingMetadataDictionary(_exportDescriptor);

                return _exportDescriptor.Value.Metadata;
            }
        }

        private object Activate(LifetimeContext context, CompositionOperation operation)
        {
            Assumes.IsTrue(_exportDescriptor.IsValueCreated, "Activation in progress before all descriptors fully initialized.");

            Debug.WriteLine("[System.Composition] Activating via cycle-breaking proxy.");
            return _exportDescriptor.Value.Activator(context, operation);
        }
    }
}
