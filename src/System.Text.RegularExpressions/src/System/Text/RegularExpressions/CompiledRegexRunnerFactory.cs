﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection.Emit;

namespace System.Text.RegularExpressions
{
    internal sealed class CompiledRegexRunnerFactory : RegexRunnerFactory
    {
        private readonly DynamicMethod _goMethod;
        private readonly DynamicMethod _findFirstCharMethod;
        private readonly DynamicMethod _initTrackCountMethod;

        internal CompiledRegexRunnerFactory(DynamicMethod go, DynamicMethod firstChar, DynamicMethod trackCount)
        {
            _goMethod = go;
            _findFirstCharMethod = firstChar;
            _initTrackCountMethod = trackCount;
        }

        protected internal override RegexRunner CreateInstance()
        {
            CompiledRegexRunner runner = new CompiledRegexRunner();
            runner.SetDelegates((NoParamDelegate)_goMethod.CreateDelegate(typeof(NoParamDelegate)),
                                (FindFirstCharDelegate)_findFirstCharMethod.CreateDelegate(typeof(FindFirstCharDelegate)),
                                (NoParamDelegate)_initTrackCountMethod.CreateDelegate(typeof(NoParamDelegate)));

            return runner;
        }
    }

    internal delegate RegexRunner CreateInstanceDelegate();
}
