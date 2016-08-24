// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace System.Linq.Expressions.Tests
{
    public static class CompilerTests
    {
        [Theory]
        [ClassData(typeof(CompilationTypes))]
        public static void CompileDeepTree_NoStackOverflow(bool useInterpreter)
        {
            var e = (Expression)Expression.Constant(0);

            var n = 10000;

            for (var i = 0; i < n; i++)
                e = Expression.Add(e, Expression.Constant(1));

            var f = Expression.Lambda<Func<int>>(e).Compile(useInterpreter);

            Assert.Equal(n, f());
        }
    }
}
