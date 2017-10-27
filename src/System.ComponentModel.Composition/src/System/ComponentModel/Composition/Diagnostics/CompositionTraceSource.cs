﻿// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Diagnostics
{
    internal static class CompositionTraceSource
    {
#if FEATURE_TRACING
        private static readonly TraceSourceTraceWriter Source = new TraceSourceTraceWriter();
#else
        private static readonly DebuggerTraceWriter Source = new DebuggerTraceWriter();
#endif

        public static bool CanWriteInformation
        {
            get { return Source.CanWriteInformation; }
        }

        public static bool CanWriteWarning
        {
            get { return Source.CanWriteWarning; }
        }

        public static bool CanWriteError
        {
            get { return Source.CanWriteError; }
        }

        public static void WriteInformation(CompositionTraceId traceId, string format, params object[] arguments)
        {
            EnsureEnabled(CanWriteInformation);

            Source.WriteInformation(traceId, format, arguments);
        }

        public static void WriteWarning(CompositionTraceId traceId, string format, params object[] arguments)
        {
            EnsureEnabled(CanWriteWarning);

            Source.WriteWarning(traceId, format, arguments);
        }

        public static void WriteError(CompositionTraceId traceId, string format, params object[] arguments)
        {
            EnsureEnabled(CanWriteError);

            Source.WriteError(traceId, format, arguments);
        }

        private static void EnsureEnabled(bool condition)
        {
            Assumes.IsTrue(condition, "To avoid unnecessary work when a trace level has not been enabled, check CanWriteXXX before calling this method.");
        }
    }
}
