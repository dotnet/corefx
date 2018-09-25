This document will be kept up-to-date as new builds are made available. See https://github.com/dotnet/corefx/tree/feature/utf8string/Documentation/utf8string/README.md for documentation.

To use any of the below versions, set your `.csproj`'s `RuntimeFrameworkVersion` element to the version listed.

We integrate coreclr / corefx `master` into `feature/utf8string` regularly. Any commits to `master` (bug fixes, new JIT features, and so on) will generally make their way to `feature/utf8string` within a few days. This allows developers who are early adopters of the Utf8String feature to also take advantage of other new features of .NET Core 3.

# Version history

## 3.0.0-alphautf8string-26922-02
_Sep. 22, 2018_

Initial prototype release. Includes skeleton `Utf8String` APIs and ancillary helper types like `System.Text.UnicodeScalar`.

```cs
namespace System {
    public static class MemoryExtensions {
        public static ReadOnlySpan<byte> AsSpan(this Utf8String text);
        public static ReadOnlySpan<byte> AsSpan(this Utf8String text, int start);
        public static ReadOnlySpan<byte> AsSpan(this Utf8String text, int start, int length);
        public static bool Contains<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T>;
        public static bool Contains<T>(this Span<T> span, T value) where T : IEquatable<T>;
    }
    public sealed class String : ICloneable, IComparable, IComparable<string>, IConvertible, IEnumerable, IEnumerable<char>, IEquatable<string> {
        public static int GetHashCode(ReadOnlySpan<char> value);
        public static int GetHashCode(ReadOnlySpan<char> value, StringComparison comparisonType);
    }
    public sealed class Utf8String : IEquatable<Utf8String> {
        public static readonly Utf8String Empty;
        public unsafe Utf8String(byte* value);
        public Utf8String(byte[] value, int startIndex, int length);
        public unsafe Utf8String(char* value);
        public Utf8String(char[] value, int startIndex, int length);
        public Utf8String(ReadOnlySpan<byte> value);
        public Utf8String(ReadOnlySpan<char> value);
        public Utf8String(string value);
        public int Length { get; }
        public static Utf8String Concat(Utf8String str0, Utf8String str1);
        public static Utf8String Concat(Utf8String str0, Utf8String str1, Utf8String str2);
        public static Utf8String Concat(Utf8String str0, Utf8String str1, Utf8String str2, Utf8String str3);
        public static Utf8String Create(ReadOnlySpan<byte> value, InvalidSequenceBehavior behavior = (InvalidSequenceBehavior)(1));
        public static Utf8String Create<TState>(int length, TState state, SpanAction<byte, TState> action, InvalidSequenceBehavior behavior = (InvalidSequenceBehavior)(1));
        public bool EndsWith(UnicodeScalar value);
        public override bool Equals(object obj);
        public bool Equals(Utf8String value);
        public static bool Equals(Utf8String a, Utf8String b);
        public static bool Equals(Utf8String a, Utf8String b, StringComparison comparisonType);
        public override int GetHashCode();
        public static int GetHashCode(ReadOnlySpan<byte> value);
        public static int GetHashCode(ReadOnlySpan<byte> value, StringComparison comparisonType);
        public int GetHashCode(StringComparison comparisonType);
        public ref readonly byte GetPinnableReference();
        public int IndexOf(char value);
        public int IndexOf(char value, int startIndex);
        public int IndexOf(char value, int startIndex, int count);
        public int IndexOf(UnicodeScalar value);
        public int IndexOf(UnicodeScalar value, int startIndex);
        public int IndexOf(UnicodeScalar value, int startIndex, int count);
        public static bool IsEmptyOrWhiteSpace(ReadOnlySpan<byte> value);
        public static bool IsNullOrEmpty(Utf8String value);
        public static bool IsNullOrWhiteSpace(Utf8String value);
        public static Utf8String Literal(string value);
        public static bool operator ==(Utf8String a, Utf8String b);
        public static bool operator !=(Utf8String a, Utf8String b);
        public bool StartsWith(UnicodeScalar value);
        public Utf8String Substring(int startIndex);
        public Utf8String Substring(int startIndex, int length);
        public override string ToString();
        public Utf8String Trim();
        public Utf8String TrimEnd();
        public Utf8String TrimStart();
    }
    public abstract class Utf8StringComparer : IEqualityComparer<Utf8String> {
        public static Utf8StringComparer CurrentCulture { get; }
        public static Utf8StringComparer CurrentCultureIgnoreCase { get; }
        public static Utf8StringComparer InvariantCulture { get; }
        public static Utf8StringComparer InvariantCultureIgnoreCase { get; }
        public static Utf8StringComparer Ordinal { get; }
        public static Utf8StringComparer OrdinalIgnoreCase { get; }
        public static Utf8StringComparer Create(CultureInfo culture, bool ignoreCase);
        public static Utf8StringComparer Create(CultureInfo culture, CompareOptions options);
        public abstract bool Equals(Utf8String x, Utf8String y);
        public static Utf8StringComparer FromComparison(StringComparison comparisonType);
        public abstract int GetHashCode(Utf8String obj);
    }
}
namespace System.Globalization {
    public class CompareInfo : IDeserializationCallback {
        public int GetHashCode(ReadOnlySpan<char> source, CompareOptions options);
    }
}
namespace System.IO {
    public static class File {
        public static Utf8String ReadAllTextUtf8(string path);
        public static void WriteAllTextUtf8(string path, Utf8String contents);
    }
    public class StreamWriter : TextWriter {
        public override void Write(Utf8String value);
    }
    public abstract class TextWriter : MarshalByRefObject, IDisposable {
        public virtual void Write(Utf8String value);
        public virtual Task WriteAsync(Utf8String value);
        public virtual void WriteLine(Utf8String value);
        public virtual Task WriteLineAsync(Utf8String value);
    }
}
namespace System.Runtime.CompilerServices {
    public static class RuntimeHelpers {
        public static Utf8String GetUtf8StringLiteral(string s);
    }
}
namespace System.Text {
    public enum InvalidSequenceBehavior {
        Fail = 0,
        LeaveUnchanged = 2,
        ReplaceInvalidSequence = 1,
    }
    public readonly struct UnicodeScalar : IComparable<UnicodeScalar>, IEquatable<UnicodeScalar> {
        public UnicodeScalar(char ch);
        public UnicodeScalar(int scalarValue);
        public UnicodeScalar(uint scalarValue);
        public bool IsAscii { get; }
        public bool IsBmp { get; }
        public int Plane { get; }
        public static UnicodeScalar ReplacementChar { get; }
        public int Utf16SequenceLength { get; }
        public int Utf8SequenceLength { get; }
        public uint Value { get; }
        public int CompareTo(UnicodeScalar other);
        public static UnicodeScalar DangerousCreateWithoutValidation(uint scalarValue);
        public override bool Equals(object obj);
        public bool Equals(UnicodeScalar other);
        public override int GetHashCode();
        public static double GetNumericValue(UnicodeScalar s);
        public static UnicodeCategory GetUnicodeCategory(UnicodeScalar s);
        public static bool IsControl(UnicodeScalar s);
        public static bool IsDigit(UnicodeScalar s);
        public static bool IsLetter(UnicodeScalar s);
        public static bool IsLetterOrDigit(UnicodeScalar s);
        public static bool IsLower(UnicodeScalar s);
        public static bool IsNumber(UnicodeScalar s);
        public static bool IsPunctuation(UnicodeScalar s);
        public static bool IsSeparator(UnicodeScalar s);
        public static bool IsSymbol(UnicodeScalar s);
        public static bool IsUpper(UnicodeScalar s);
        public static bool IsValid(int value);
        public static bool IsWhiteSpace(UnicodeScalar s);
        public static bool operator ==(UnicodeScalar a, UnicodeScalar b);
        public static bool operator >(UnicodeScalar a, UnicodeScalar b);
        public static bool operator >=(UnicodeScalar a, UnicodeScalar b);
        public static bool operator !=(UnicodeScalar a, UnicodeScalar b);
        public static bool operator <(UnicodeScalar a, UnicodeScalar b);
        public static bool operator <=(UnicodeScalar a, UnicodeScalar b);
        public static UnicodeScalar ToLower(UnicodeScalar s, CultureInfo culture);
        public static UnicodeScalar ToLowerInvariant(UnicodeScalar s);
        public override string ToString();
        public static UnicodeScalar ToUpper(UnicodeScalar s, CultureInfo culture);
        public static UnicodeScalar ToUpperInvariant(UnicodeScalar s);
        public int ToUtf16(Span<char> output);
        public int ToUtf8(Span<byte> output);
        public Utf8String ToUtf8String();
        public static bool TryCreate(int value, out UnicodeScalar result);
        public static bool TryCreate(uint value, out UnicodeScalar result);
    }
}
```
