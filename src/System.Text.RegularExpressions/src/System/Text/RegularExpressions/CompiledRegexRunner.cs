// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.RegularExpressions
{
    internal sealed class CompiledRegexRunner : RegexRunner
    {
        private NoParamDelegate _goMethod;
        private FindFirstCharDelegate _findFirstCharMethod;
        private NoParamDelegate _initTrackCountMethod;

        internal CompiledRegexRunner() { }

        internal void SetDelegates(NoParamDelegate go, FindFirstCharDelegate firstChar, NoParamDelegate trackCount)
        {
            _goMethod = go;
            _findFirstCharMethod = firstChar;
            _initTrackCountMethod = trackCount;
        }

        protected override void Go()
        {
            _goMethod(this);
        }

        protected override bool FindFirstChar()
        {
            return _findFirstCharMethod(this);
        }

        protected override void InitTrackCount()
        {
            _initTrackCountMethod(this);
        }
    }

    internal delegate void NoParamDelegate(RegexRunner r);
    internal delegate bool FindFirstCharDelegate(RegexRunner r);
}
