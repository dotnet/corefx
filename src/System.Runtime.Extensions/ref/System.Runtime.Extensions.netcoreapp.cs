// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System
{
    public static partial class BitConverter
    {
        [CLSCompliant(false)]
        public static ushort ByteSwap(ushort value) => throw null;
        [CLSCompliant(false)]
        public static uint ByteSwap(uint value) => throw null;
        [CLSCompliant(false)]
        public static ulong ByteSwap(ulong value) => throw null;
    }
    public abstract partial class Utf8StringComparer : System.Collections.Generic.IEqualityComparer<Utf8String>
    {
        private Utf8StringComparer() { }
        public static Utf8StringComparer CurrentCulture { get { throw null; } }
        public static Utf8StringComparer CurrentCultureIgnoreCase { get { throw null; } }
        public static Utf8StringComparer InvariantCulture { get { throw null; } }
        public static Utf8StringComparer InvariantCultureIgnoreCase { get { throw null; } }
        public static Utf8StringComparer Ordinal { get { throw null; } }
        public static Utf8StringComparer OrdinalIgnoreCase { get { throw null; } }
        public static Utf8StringComparer Create(System.Globalization.CultureInfo culture, bool ignoreCase) { throw null; }
        public static Utf8StringComparer Create(System.Globalization.CultureInfo culture, System.Globalization.CompareOptions options) { throw null; }
        public abstract bool Equals(Utf8String x, Utf8String y);
        public static Utf8StringComparer FromComparison(StringComparison comparisonType) { throw null; }
        public abstract int GetHashCode(Utf8String obj);
    }
    public abstract partial class StringSegmentComparer : System.Collections.Generic.IComparer<StringSegment>, System.Collections.Generic.IEqualityComparer<StringSegment>
    {
        private StringSegmentComparer() { }
        public static StringSegmentComparer CurrentCulture => throw null;
        public static StringSegmentComparer CurrentCultureIgnoreCase => throw null;
        public static StringSegmentComparer InvariantCulture => throw null;
        public static StringSegmentComparer InvariantCultureIgnoreCase => throw null;
        public static StringSegmentComparer Ordinal => throw null;
        public static StringSegmentComparer OrdinalIgnoreCase => throw null;
        public static StringSegmentComparer Create(System.Globalization.CultureInfo culture, bool ignoreCase) => throw null;
        public static StringSegmentComparer Create(System.Globalization.CultureInfo culture, System.Globalization.CompareOptions options) => throw null;
        public abstract int Compare(StringSegment x, StringSegment y);
        public abstract bool Equals(StringSegment x, StringSegment y);
        public static StringSegmentComparer FromComparison(StringComparison comparisonType) => throw null;
        public abstract int GetHashCode(StringSegment obj);
    }
}

namespace System.IO
{
    public partial class StringWriter : System.IO.TextWriter
    {
        public override void Write(StringSegment value) { }
        public override System.Threading.Tasks.Task WriteLineAsync(StringSegment value) { throw null; }
    }
    public abstract partial class TextWriter : System.MarshalByRefObject, System.IDisposable
    {
        public virtual void Write(StringSegment value) { }
        public virtual void Write(Utf8String value) { }
        public virtual void Write(Utf8StringSegment value) { }
        public virtual System.Threading.Tasks.Task WriteAsync(StringSegment value) { throw null; }
        public virtual System.Threading.Tasks.Task WriteAsync(Utf8String value) { throw null; }
        public virtual System.Threading.Tasks.Task WriteAsync(Utf8StringSegment value) { throw null; }
        public virtual void WriteLine(StringSegment value) { }
        public virtual void WriteLine(Utf8String value) { }
        public virtual void WriteLine(Utf8StringSegment value) { }
        public virtual System.Threading.Tasks.Task WriteLineAsync(StringSegment value) { throw null; }
        public virtual System.Threading.Tasks.Task WriteLineAsync(Utf8String value) { throw null; }
        public virtual System.Threading.Tasks.Task WriteLineAsync(Utf8StringSegment value) { throw null; }
    }
}
