// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Diagnostics
{
    public partial class Activity
    {
        public Activity(string operationName) { }
        public System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> Baggage { get { throw null; } }
        public static Activity Current
        {
#if ALLOW_PARTIALLY_TRUSTED_CALLERS
        [System.Security.SecuritySafeCriticalAttribute]
#endif
            get { throw null; }
#if ALLOW_PARTIALLY_TRUSTED_CALLERS
       [System.Security.SecuritySafeCriticalAttribute]
#endif
            set { }
        }
        public static System.Diagnostics.ActivityIdFormat DefaultIdFormat { get { throw null; } set { } }
        public System.TimeSpan Duration { get { throw null; } }
        public static bool ForceDefaultIdFormat { get { throw null; } set { } }
        public string Id { get { throw null; } }
        public System.Diagnostics.ActivityIdFormat IdFormat { get { throw null; } }
        public string OperationName { get { throw null; } }
        public System.Diagnostics.Activity Parent { get { throw null; } }
        public string ParentId { get { throw null; } }
        public ref readonly System.Diagnostics.SpanId ParentSpanId { get { throw null; } }
        public string RootId { get { throw null; } }
        public ref readonly System.Diagnostics.SpanId SpanId { get { throw null; } }
        public System.DateTime StartTimeUtc { get { throw null; } }
        public System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> Tags { get { throw null; } }
        public ref readonly System.Diagnostics.TraceId TraceId { get { throw null; } }
        public string TraceStateString { get { throw null; } set { } }
        public System.Diagnostics.Activity AddBaggage(string key, string value) { throw null; }
        public System.Diagnostics.Activity AddTag(string key, string value) { throw null; }
        public string GetBaggageItem(string key) { throw null; }
        public System.Diagnostics.Activity SetEndTime(System.DateTime endTimeUtc) { throw null; }
        public System.Diagnostics.Activity SetParentId(in System.Diagnostics.TraceId traceId, in System.Diagnostics.SpanId spanId) { throw null; }
        public System.Diagnostics.Activity SetParentId(string parentId) { throw null; }
        public System.Diagnostics.Activity SetStartTime(System.DateTime startTimeUtc) { throw null; }
        public System.Diagnostics.Activity Start() { throw null; }
        public void Stop() { }
    }
    public enum ActivityIdFormat : byte
    {
        Hierarchical = (byte)1,
        Unknown = (byte)0,
        W3C = (byte)2,
    }

    public partial struct SpanId : IEquatable<SpanId>
    {
        private object _dummy;
        private int _dummyPrimitive;
        public SpanId(System.ReadOnlySpan<byte> idData, bool isUtf8Chars = false) { throw null; }
        public SpanId(System.ReadOnlySpan<char> idData) { throw null; }
        public System.ReadOnlySpan<byte> AsBytes { get { throw null; } }
        public string AsHexString { get { throw null; } }
        public bool Equals(System.Diagnostics.SpanId spanId) { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static System.Diagnostics.SpanId NewSpanId() { throw null; }
        public static bool operator ==(in System.Diagnostics.SpanId spanId1, in System.Diagnostics.SpanId spandId2) { throw null; }
        public static bool operator !=(in System.Diagnostics.SpanId spanId1, in System.Diagnostics.SpanId spandId2) { throw null; }
        public override string ToString() { throw null; }
    }
    public partial struct TraceId : IEquatable<TraceId>
    {
        private object _dummy;
        private int _dummyPrimitive;
        public TraceId(System.ReadOnlySpan<byte> idData, bool isUtf8Chars = false) { throw null; }
        public TraceId(System.ReadOnlySpan<char> idData) { throw null; }
        public System.ReadOnlySpan<byte> AsBytes { get { throw null; } }
        public string AsHexString { get { throw null; } }
        public bool Equals(System.Diagnostics.TraceId traceId) { throw null; }
        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
        public static System.Diagnostics.TraceId NewTraceId() { throw null; }
        public static bool operator ==(in System.Diagnostics.TraceId traceId1, in System.Diagnostics.TraceId traceId2) { throw null; }
        public static bool operator !=(in System.Diagnostics.TraceId traceId1, in System.Diagnostics.TraceId traceId2) { throw null; }
        public override string ToString() { throw null; }
    }

    public abstract partial class DiagnosticSource
    {
        public Activity StartActivity(Activity activity, object args) { throw null; }
        public void StopActivity(Activity activity, object args) { }
    }


}
