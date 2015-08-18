// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Text.RegularExpressions
{
    public partial class Capture
    {
        internal Capture() { }
        public int Index { get { return default(int); } }
        public int Length { get { return default(int); } }
        public string Value { get { return default(string); } }
        public override string ToString() { return default(string); }
    }
    public partial class CaptureCollection : System.Collections.Generic.ICollection<System.Text.RegularExpressions.Capture>, System.Collections.Generic.IEnumerable<System.Text.RegularExpressions.Capture>, System.Collections.Generic.IList<System.Text.RegularExpressions.Capture>, System.Collections.Generic.IReadOnlyCollection<System.Text.RegularExpressions.Capture>, System.Collections.Generic.IReadOnlyList<System.Text.RegularExpressions.Capture>, System.Collections.ICollection, System.Collections.IEnumerable, System.Collections.IList
    {
        internal CaptureCollection() { }
        public int Count { get { return default(int); } }
        public System.Text.RegularExpressions.Capture this[int i] { get { return default(System.Text.RegularExpressions.Capture); } }
        bool System.Collections.Generic.ICollection<System.Text.RegularExpressions.Capture>.IsReadOnly { get { return default(bool); } }
        System.Text.RegularExpressions.Capture System.Collections.Generic.IList<System.Text.RegularExpressions.Capture>.this[int index] { get { return default(System.Text.RegularExpressions.Capture); } set { } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        bool System.Collections.IList.IsFixedSize { get { return default(bool); } }
        bool System.Collections.IList.IsReadOnly { get { return default(bool); } }
        object System.Collections.IList.this[int index] { get { return default(object); } set { } }
        public void CopyTo(System.Text.RegularExpressions.Capture[] array, int arrayIndex) { }
        public System.Collections.IEnumerator GetEnumerator() { return default(System.Collections.IEnumerator); }
        void System.Collections.Generic.ICollection<System.Text.RegularExpressions.Capture>.Add(System.Text.RegularExpressions.Capture item) { }
        void System.Collections.Generic.ICollection<System.Text.RegularExpressions.Capture>.Clear() { }
        bool System.Collections.Generic.ICollection<System.Text.RegularExpressions.Capture>.Contains(System.Text.RegularExpressions.Capture item) { return default(bool); }
        bool System.Collections.Generic.ICollection<System.Text.RegularExpressions.Capture>.Remove(System.Text.RegularExpressions.Capture item) { return default(bool); }
        System.Collections.Generic.IEnumerator<System.Text.RegularExpressions.Capture> System.Collections.Generic.IEnumerable<System.Text.RegularExpressions.Capture>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<System.Text.RegularExpressions.Capture>); }
        int System.Collections.Generic.IList<System.Text.RegularExpressions.Capture>.IndexOf(System.Text.RegularExpressions.Capture item) { return default(int); }
        void System.Collections.Generic.IList<System.Text.RegularExpressions.Capture>.Insert(int index, System.Text.RegularExpressions.Capture item) { }
        void System.Collections.Generic.IList<System.Text.RegularExpressions.Capture>.RemoveAt(int index) { }
        void System.Collections.ICollection.CopyTo(System.Array array, int arrayIndex) { }
        int System.Collections.IList.Add(object value) { return default(int); }
        void System.Collections.IList.Clear() { }
        bool System.Collections.IList.Contains(object value) { return default(bool); }
        int System.Collections.IList.IndexOf(object value) { return default(int); }
        void System.Collections.IList.Insert(int index, object value) { }
        void System.Collections.IList.Remove(object value) { }
        void System.Collections.IList.RemoveAt(int index) { }
    }
    public partial class Group : System.Text.RegularExpressions.Capture
    {
        internal Group() { }
        public System.Text.RegularExpressions.CaptureCollection Captures { get { return default(System.Text.RegularExpressions.CaptureCollection); } }
        public string Name { get { return default(string); } }
        public bool Success { get { return default(bool); } }
    }
    public partial class GroupCollection : System.Collections.Generic.ICollection<System.Text.RegularExpressions.Group>, System.Collections.Generic.IEnumerable<System.Text.RegularExpressions.Group>, System.Collections.Generic.IList<System.Text.RegularExpressions.Group>, System.Collections.Generic.IReadOnlyCollection<System.Text.RegularExpressions.Group>, System.Collections.Generic.IReadOnlyList<System.Text.RegularExpressions.Group>, System.Collections.ICollection, System.Collections.IEnumerable, System.Collections.IList
    {
        internal GroupCollection() { }
        public int Count { get { return default(int); } }
        public System.Text.RegularExpressions.Group this[int groupnum] { get { return default(System.Text.RegularExpressions.Group); } }
        public System.Text.RegularExpressions.Group this[string groupname] { get { return default(System.Text.RegularExpressions.Group); } }
        bool System.Collections.Generic.ICollection<System.Text.RegularExpressions.Group>.IsReadOnly { get { return default(bool); } }
        System.Text.RegularExpressions.Group System.Collections.Generic.IList<System.Text.RegularExpressions.Group>.this[int index] { get { return default(System.Text.RegularExpressions.Group); } set { } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        bool System.Collections.IList.IsFixedSize { get { return default(bool); } }
        bool System.Collections.IList.IsReadOnly { get { return default(bool); } }
        object System.Collections.IList.this[int index] { get { return default(object); } set { } }
        public void CopyTo(System.Text.RegularExpressions.Group[] array, int arrayIndex) { }
        public System.Collections.IEnumerator GetEnumerator() { return default(System.Collections.IEnumerator); }
        void System.Collections.Generic.ICollection<System.Text.RegularExpressions.Group>.Add(System.Text.RegularExpressions.Group item) { }
        void System.Collections.Generic.ICollection<System.Text.RegularExpressions.Group>.Clear() { }
        bool System.Collections.Generic.ICollection<System.Text.RegularExpressions.Group>.Contains(System.Text.RegularExpressions.Group item) { return default(bool); }
        bool System.Collections.Generic.ICollection<System.Text.RegularExpressions.Group>.Remove(System.Text.RegularExpressions.Group item) { return default(bool); }
        System.Collections.Generic.IEnumerator<System.Text.RegularExpressions.Group> System.Collections.Generic.IEnumerable<System.Text.RegularExpressions.Group>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<System.Text.RegularExpressions.Group>); }
        int System.Collections.Generic.IList<System.Text.RegularExpressions.Group>.IndexOf(System.Text.RegularExpressions.Group item) { return default(int); }
        void System.Collections.Generic.IList<System.Text.RegularExpressions.Group>.Insert(int index, System.Text.RegularExpressions.Group item) { }
        void System.Collections.Generic.IList<System.Text.RegularExpressions.Group>.RemoveAt(int index) { }
        void System.Collections.ICollection.CopyTo(System.Array array, int arrayIndex) { }
        int System.Collections.IList.Add(object value) { return default(int); }
        void System.Collections.IList.Clear() { }
        bool System.Collections.IList.Contains(object value) { return default(bool); }
        int System.Collections.IList.IndexOf(object value) { return default(int); }
        void System.Collections.IList.Insert(int index, object value) { }
        void System.Collections.IList.Remove(object value) { }
        void System.Collections.IList.RemoveAt(int index) { }
    }
    public partial class Match : System.Text.RegularExpressions.Group
    {
        internal Match() { }
        public static System.Text.RegularExpressions.Match Empty { get { return default(System.Text.RegularExpressions.Match); } }
        public virtual System.Text.RegularExpressions.GroupCollection Groups { get { return default(System.Text.RegularExpressions.GroupCollection); } }
        public System.Text.RegularExpressions.Match NextMatch() { return default(System.Text.RegularExpressions.Match); }
        public virtual string Result(string replacement) { return default(string); }
    }
    public partial class MatchCollection : System.Collections.Generic.ICollection<System.Text.RegularExpressions.Match>, System.Collections.Generic.IEnumerable<System.Text.RegularExpressions.Match>, System.Collections.Generic.IList<System.Text.RegularExpressions.Match>, System.Collections.Generic.IReadOnlyCollection<System.Text.RegularExpressions.Match>, System.Collections.Generic.IReadOnlyList<System.Text.RegularExpressions.Match>, System.Collections.ICollection, System.Collections.IEnumerable, System.Collections.IList
    {
        internal MatchCollection() { }
        public int Count { get { return default(int); } }
        public virtual System.Text.RegularExpressions.Match this[int i] { get { return default(System.Text.RegularExpressions.Match); } }
        bool System.Collections.Generic.ICollection<System.Text.RegularExpressions.Match>.IsReadOnly { get { return default(bool); } }
        System.Text.RegularExpressions.Match System.Collections.Generic.IList<System.Text.RegularExpressions.Match>.this[int index] { get { return default(System.Text.RegularExpressions.Match); } set { } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        bool System.Collections.IList.IsFixedSize { get { return default(bool); } }
        bool System.Collections.IList.IsReadOnly { get { return default(bool); } }
        object System.Collections.IList.this[int index] { get { return default(object); } set { } }
        public void CopyTo(System.Text.RegularExpressions.Match[] array, int arrayIndex) { }
        public System.Collections.IEnumerator GetEnumerator() { return default(System.Collections.IEnumerator); }
        void System.Collections.Generic.ICollection<System.Text.RegularExpressions.Match>.Add(System.Text.RegularExpressions.Match item) { }
        void System.Collections.Generic.ICollection<System.Text.RegularExpressions.Match>.Clear() { }
        bool System.Collections.Generic.ICollection<System.Text.RegularExpressions.Match>.Contains(System.Text.RegularExpressions.Match item) { return default(bool); }
        bool System.Collections.Generic.ICollection<System.Text.RegularExpressions.Match>.Remove(System.Text.RegularExpressions.Match item) { return default(bool); }
        System.Collections.Generic.IEnumerator<System.Text.RegularExpressions.Match> System.Collections.Generic.IEnumerable<System.Text.RegularExpressions.Match>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<System.Text.RegularExpressions.Match>); }
        int System.Collections.Generic.IList<System.Text.RegularExpressions.Match>.IndexOf(System.Text.RegularExpressions.Match item) { return default(int); }
        void System.Collections.Generic.IList<System.Text.RegularExpressions.Match>.Insert(int index, System.Text.RegularExpressions.Match item) { }
        void System.Collections.Generic.IList<System.Text.RegularExpressions.Match>.RemoveAt(int index) { }
        void System.Collections.ICollection.CopyTo(System.Array array, int arrayIndex) { }
        int System.Collections.IList.Add(object value) { return default(int); }
        void System.Collections.IList.Clear() { }
        bool System.Collections.IList.Contains(object value) { return default(bool); }
        int System.Collections.IList.IndexOf(object value) { return default(int); }
        void System.Collections.IList.Insert(int index, object value) { }
        void System.Collections.IList.Remove(object value) { }
        void System.Collections.IList.RemoveAt(int index) { }
    }
    public delegate string MatchEvaluator(System.Text.RegularExpressions.Match match);
    public partial class Regex
    {
        public static readonly System.TimeSpan InfiniteMatchTimeout;
        protected Regex() { }
        public Regex(string pattern) { }
        public Regex(string pattern, System.Text.RegularExpressions.RegexOptions options) { }
        public Regex(string pattern, System.Text.RegularExpressions.RegexOptions options, System.TimeSpan matchTimeout) { }
        public static int CacheSize { get { return default(int); } set { } }
        public System.TimeSpan MatchTimeout { get { return default(System.TimeSpan); } }
        public System.Text.RegularExpressions.RegexOptions Options { get { return default(System.Text.RegularExpressions.RegexOptions); } }
        public bool RightToLeft { get { return default(bool); } }
        public static string Escape(string str) { return default(string); }
        public string[] GetGroupNames() { return default(string[]); }
        public int[] GetGroupNumbers() { return default(int[]); }
        public string GroupNameFromNumber(int i) { return default(string); }
        public int GroupNumberFromName(string name) { return default(int); }
        public bool IsMatch(string input) { return default(bool); }
        public bool IsMatch(string input, int startat) { return default(bool); }
        public static bool IsMatch(string input, string pattern) { return default(bool); }
        public static bool IsMatch(string input, string pattern, System.Text.RegularExpressions.RegexOptions options) { return default(bool); }
        public static bool IsMatch(string input, string pattern, System.Text.RegularExpressions.RegexOptions options, System.TimeSpan matchTimeout) { return default(bool); }
        public System.Text.RegularExpressions.Match Match(string input) { return default(System.Text.RegularExpressions.Match); }
        public System.Text.RegularExpressions.Match Match(string input, int startat) { return default(System.Text.RegularExpressions.Match); }
        public System.Text.RegularExpressions.Match Match(string input, int beginning, int length) { return default(System.Text.RegularExpressions.Match); }
        public static System.Text.RegularExpressions.Match Match(string input, string pattern) { return default(System.Text.RegularExpressions.Match); }
        public static System.Text.RegularExpressions.Match Match(string input, string pattern, System.Text.RegularExpressions.RegexOptions options) { return default(System.Text.RegularExpressions.Match); }
        public static System.Text.RegularExpressions.Match Match(string input, string pattern, System.Text.RegularExpressions.RegexOptions options, System.TimeSpan matchTimeout) { return default(System.Text.RegularExpressions.Match); }
        public System.Text.RegularExpressions.MatchCollection Matches(string input) { return default(System.Text.RegularExpressions.MatchCollection); }
        public System.Text.RegularExpressions.MatchCollection Matches(string input, int startat) { return default(System.Text.RegularExpressions.MatchCollection); }
        public static System.Text.RegularExpressions.MatchCollection Matches(string input, string pattern) { return default(System.Text.RegularExpressions.MatchCollection); }
        public static System.Text.RegularExpressions.MatchCollection Matches(string input, string pattern, System.Text.RegularExpressions.RegexOptions options) { return default(System.Text.RegularExpressions.MatchCollection); }
        public static System.Text.RegularExpressions.MatchCollection Matches(string input, string pattern, System.Text.RegularExpressions.RegexOptions options, System.TimeSpan matchTimeout) { return default(System.Text.RegularExpressions.MatchCollection); }
        public string Replace(string input, string replacement) { return default(string); }
        public string Replace(string input, string replacement, int count) { return default(string); }
        public string Replace(string input, string replacement, int count, int startat) { return default(string); }
        public static string Replace(string input, string pattern, string replacement) { return default(string); }
        public static string Replace(string input, string pattern, string replacement, System.Text.RegularExpressions.RegexOptions options) { return default(string); }
        public static string Replace(string input, string pattern, string replacement, System.Text.RegularExpressions.RegexOptions options, System.TimeSpan matchTimeout) { return default(string); }
        public static string Replace(string input, string pattern, System.Text.RegularExpressions.MatchEvaluator evaluator) { return default(string); }
        public static string Replace(string input, string pattern, System.Text.RegularExpressions.MatchEvaluator evaluator, System.Text.RegularExpressions.RegexOptions options) { return default(string); }
        public static string Replace(string input, string pattern, System.Text.RegularExpressions.MatchEvaluator evaluator, System.Text.RegularExpressions.RegexOptions options, System.TimeSpan matchTimeout) { return default(string); }
        public string Replace(string input, System.Text.RegularExpressions.MatchEvaluator evaluator) { return default(string); }
        public string Replace(string input, System.Text.RegularExpressions.MatchEvaluator evaluator, int count) { return default(string); }
        public string Replace(string input, System.Text.RegularExpressions.MatchEvaluator evaluator, int count, int startat) { return default(string); }
        public string[] Split(string input) { return default(string[]); }
        public string[] Split(string input, int count) { return default(string[]); }
        public string[] Split(string input, int count, int startat) { return default(string[]); }
        public static string[] Split(string input, string pattern) { return default(string[]); }
        public static string[] Split(string input, string pattern, System.Text.RegularExpressions.RegexOptions options) { return default(string[]); }
        public static string[] Split(string input, string pattern, System.Text.RegularExpressions.RegexOptions options, System.TimeSpan matchTimeout) { return default(string[]); }
        public override string ToString() { return default(string); }
        public static string Unescape(string str) { return default(string); }
    }
    public partial class RegexMatchTimeoutException : System.TimeoutException
    {
        public RegexMatchTimeoutException() { }
        public RegexMatchTimeoutException(string message) { }
        public RegexMatchTimeoutException(string message, System.Exception inner) { }
        public RegexMatchTimeoutException(string regexInput, string regexPattern, System.TimeSpan matchTimeout) { }
        public string Input { get { return default(string); } }
        public System.TimeSpan MatchTimeout { get { return default(System.TimeSpan); } }
        public string Pattern { get { return default(string); } }
    }
    [System.FlagsAttribute]
    public enum RegexOptions
    {
        Compiled = 8,
        CultureInvariant = 512,
        ECMAScript = 256,
        ExplicitCapture = 4,
        IgnoreCase = 1,
        IgnorePatternWhitespace = 32,
        Multiline = 2,
        None = 0,
        RightToLeft = 64,
        Singleline = 16,
    }
}
