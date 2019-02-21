// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System
{
    public sealed partial class Utf8String : IEquatable<Utf8String>
    {
        public static readonly Utf8String Empty;
        public Utf8String(ReadOnlySpan<byte> value) { }
        public Utf8String(byte[] value, int startIndex, int length) { }
        [CLSCompliant(false)]
        public unsafe Utf8String(byte* value) { }
        public Utf8String(ReadOnlySpan<char> value) { }
        public Utf8String(char[] value, int startIndex, int length) { }
        [CLSCompliant(false)]
        public unsafe Utf8String(char* value) { }
        public Utf8String(string value) { }
        public static bool operator ==(Utf8String a, Utf8String b) => throw null;
        public static bool operator !=(Utf8String a, Utf8String b) => throw null;
        public int Length => throw null;
        [ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)] // for compiler use only
        public override bool Equals(object obj) => throw null;
        public bool Equals(Utf8String value) => throw null;
        public static bool Equals(Utf8String a, Utf8String b) => throw null;
        public override int GetHashCode() => throw null;
        public ref readonly byte GetPinnableReference() => throw null;
        public override string ToString() => throw null;
    }
}
