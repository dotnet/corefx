// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Text.RegularExpressions
{
    public partial class Regex
    {
        public static void CompileToAssembly(RegexCompilationInfo[] regexinfos, System.Reflection.AssemblyName assemblyname) { }
        public static void CompileToAssembly(RegexCompilationInfo[] regexinfos, System.Reflection.AssemblyName assemblyname, System.Reflection.Emit.CustomAttributeBuilder[] attributes) { }
        public static void CompileToAssembly(RegexCompilationInfo[] regexinfos, System.Reflection.AssemblyName assemblyname, System.Reflection.Emit.CustomAttributeBuilder[] attributes, string resourceFile) { }

        /* UTF8 APIs */

        public bool IsMatch(Utf8String input) { throw null; }
        public bool IsMatch(Utf8String input, int startat) { throw null; }
        public static bool IsMatch(Utf8String input, string pattern) { throw null; }
        public static bool IsMatch(Utf8String input, string pattern, System.Text.RegularExpressions.RegexOptions options) { throw null; }
        public static bool IsMatch(Utf8String input, string pattern, System.Text.RegularExpressions.RegexOptions options, System.TimeSpan matchTimeout) { throw null; }
    }
}
