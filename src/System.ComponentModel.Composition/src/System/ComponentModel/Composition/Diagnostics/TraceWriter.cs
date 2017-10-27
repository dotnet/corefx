// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System;

namespace System.ComponentModel.Composition.Diagnostics
{
    internal abstract class TraceWriter
    {
        public abstract bool CanWriteInformation
        {
            get;
        }

        public abstract bool CanWriteWarning
        {
            get;
        }

        public abstract bool CanWriteError
        {
            get;
        }

        public abstract void WriteInformation(CompositionTraceId traceId, string format, params object[] arguments);

        public abstract void WriteWarning(CompositionTraceId traceId, string format, params object[] arguments);

        public abstract void WriteError(CompositionTraceId traceId, string format, params object[] arguments);
    }
}

