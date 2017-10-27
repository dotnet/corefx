// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    public class ExportFactory<T, TMetadata> : ExportFactory<T>
    {
        private readonly TMetadata _metadata;

        public ExportFactory(Func<Tuple<T, Action>> exportLifetimeContextCreator, TMetadata metadata)
            : base(exportLifetimeContextCreator)
        {
            this._metadata = metadata;
        }

        public TMetadata Metadata
        {
            get { return this._metadata; }
        }
    }
}

