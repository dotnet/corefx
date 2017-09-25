// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Text.RegularExpressions
{
    public partial class RegexCompilationInfo
    {
        public RegexCompilationInfo(string pattern, RegexOptions options, string name, string fullnamespace, bool ispublic) { }
        public RegexCompilationInfo(string pattern, RegexOptions options, string name, string fullnamespace, bool ispublic, TimeSpan matchTimeout) { }
        public bool IsPublic { get; set; }
        public TimeSpan MatchTimeout { get; set; }
        public string Name { get; set; }
        public string Namespace { get; set; }
        public RegexOptions Options { get; set; }
        public string Pattern { get; set; }
    }
}
