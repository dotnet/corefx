// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Reflection;
using Xunit;

namespace System.Tests
{
    public static class ArgIteratorTests
    {
        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsArgIteratorSupported))]
        public static void ArgIterator_GetRemainingCount_GetNextArg()
        {
            object[] result = UseArgIterator("a", "r", "g", "s", __arglist(1, 2, 3));
            Assert.Equal(new object[] {"a", "r", "g", "s", 1, 2, 3}, result);
        }

        private static object[] UseArgIterator(Object arg0, Object arg1, Object arg2, Object arg3, __arglist) 
        {
            ArgIterator args = new ArgIterator(__arglist);
            int argCount = args.GetRemainingCount() + 4;
            object[] objArgs = new Object[argCount];

            // Handle the hard-coded arguments
            objArgs[0] = arg0;
            objArgs[1] = arg1;
            objArgs[2] = arg2;
            objArgs[3] = arg3;
            
            // Walk all of the args in the variable part of the argument list.
            for (int i = 4; i < argCount; i++)
            {
                objArgs[i] = TypedReference.ToObject(args.GetNextArg());
            }

            return objArgs;
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsArgIteratorNotSupported))]
        public unsafe static void ArgIterator_Throws_PlatformNotSupportedException()
        {
            Assert.Throws<PlatformNotSupportedException>(() => new ArgIterator(new RuntimeArgumentHandle()));
            Assert.Throws<PlatformNotSupportedException>(() => {
                fixed (void* p = "test")
                {
                    new ArgIterator(new RuntimeArgumentHandle(), p);
                }
            });
            Assert.Throws<PlatformNotSupportedException>(() => new ArgIterator().End());
            Assert.Throws<PlatformNotSupportedException>(() => new ArgIterator().Equals(new object()));
            Assert.Throws<PlatformNotSupportedException>(() => new ArgIterator().GetHashCode());
            Assert.Throws<PlatformNotSupportedException>(() => new ArgIterator().GetNextArg());
            Assert.Throws<PlatformNotSupportedException>(() => new ArgIterator().GetNextArg(new RuntimeTypeHandle()));
            Assert.Throws<PlatformNotSupportedException>(() => new ArgIterator().GetNextArgType());
            Assert.Throws<PlatformNotSupportedException>(() => new ArgIterator().GetRemainingCount());
        }
    }
}
