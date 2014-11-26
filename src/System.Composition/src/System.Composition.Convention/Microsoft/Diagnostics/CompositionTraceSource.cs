// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// -----------------------------------------------------------------------
// -----------------------------------------------------------------------

using System;
using Microsoft.Internal;

namespace Microsoft.Composition.Diagnostics
{
    internal static class CompositionTraceSource
    {
#if FEATURE_TRACING
        private static readonly TraceSourceTraceWriter Source = new TraceSourceTraceWriter();
#else
        private static readonly DebuggerTraceWriter _Source = new DebuggerTraceWriter();
#endif

        public static bool CanWriteInformation
        {
            get { return _Source.CanWriteInformation; }
        }

        public static bool CanWriteWarning
        {
            get { return _Source.CanWriteWarning; }
        }

        public static bool CanWriteError
        {
            get { return _Source.CanWriteError; }
        }

        public static void WriteInformation(CompositionTraceId traceId, string format, params object[] arguments)
        {
            EnsureEnabled(CanWriteInformation);

            _Source.WriteInformation(traceId, format, arguments);
        }

        public static void WriteWarning(CompositionTraceId traceId, string format, params object[] arguments)
        {
            EnsureEnabled(CanWriteWarning);

            _Source.WriteWarning(traceId, format, arguments);
        }

        public static void WriteError(CompositionTraceId traceId, string format, params object[] arguments)
        {
            EnsureEnabled(CanWriteError);

            _Source.WriteError(traceId, format, arguments);
        }

        private static void EnsureEnabled(bool condition)
        {
            Assumes.IsTrue(condition, "To avoid unnecessary work when a trace level has not been enabled, check CanWriteXXX before calling this method.");
        }
    }
}
