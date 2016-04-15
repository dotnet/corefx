// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
    internal static class TaskCache
    {
        private static Task<int> s_zero;
        private static Task<string> s_nullString;
        private static Task<string> s_emptyString;
        
        public static Task<int> Zero =>
            s_zero ?? (s_zero = Task.FromResult(0));
        
        public static Task<string> NullString =>
            s_nullString ?? (s_nullString = Task.FromResult<string>(null));
        
        public static Task<string> EmptyString =>
            s_emptyString ?? (s_emptyString = Task.FromResult(string.Empty));
    }
}