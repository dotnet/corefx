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
        public bool ForceDefaultIdFormat { get { throw null; } set { } }
        public string Id { get { throw null; } }
        public System.Diagnostics.ActivityIdFormat IdFormat { get { throw null; } }
        public string OperationName { get { throw null; } }
        public System.Diagnostics.Activity Parent { get { throw null; } }
        public string ParentId { get { throw null; } }
        public string RootId { get { throw null; } }
        public ref System.Diagnostics.SpanId SpanId { get { throw null; } }
        public System.DateTime StartTimeUtc { get { throw null; } }
        public System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> Tags { get { throw null; } }
        public ref System.Diagnostics.TraceId TraceId { get { throw null; } }
        public string TraceStateString { get { throw null; } set { } }
        public System.Diagnostics.Activity AddBaggage(string key, string value) { throw null; }
        public System.Diagnostics.Activity AddTag(string key, string value) { throw null; }
        public string GetBaggageItem(string key) { throw null; }
        public System.Diagnostics.Activity SetEndTime(System.DateTime endTimeUtc) { throw null; }
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

    public partial struct SpanId
    {
        private object _dummy;
        private int _dummyPrimitive;
        public SpanId(long id) { throw null; }
        public SpanId(string hexId) { throw null; }
        public string AsHexString { get { throw null; } }
        public long AsLong { get { throw null; } }
        public override string ToString() { throw null; }
    }
    public partial struct TraceId
    {
        private object _dummy;
        private int _dummyPrimitive;
        public TraceId(System.Span<byte> idBytes) { throw null; }
        public TraceId(string hexString) { throw null; }
        public string AsHexString { get { throw null; } }
        public void CopyToAsBinary(System.Span<byte> outputBuffer) { }
        public override string ToString() { throw null; }
    }

    public abstract partial class DiagnosticSource
    {
        public Activity StartActivity(Activity activity, object args) { throw null; }
        public void StopActivity(Activity activity, object args) { }
    }


}
