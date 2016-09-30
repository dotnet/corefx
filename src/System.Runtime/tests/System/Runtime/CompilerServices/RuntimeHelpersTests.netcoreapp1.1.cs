// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.CompilerServices.Tests
{
    public static partial class RuntimeHelpersTests
    {
        [Fact]
        public static void TryEnsureSufficientExecutionStack_SpaceAvailable_ReturnsTrue()
        {
            Assert.True(RuntimeHelpers.TryEnsureSufficientExecutionStack());
        }

        [Fact]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void TryEnsureSufficientExecutionStack_NoSpaceAvaiable_ReturnsFalse()
        {
            // This test will fail with a StackOverflowException if TryEnsureSufficientExecutionStack() doesn't
            // return false. No exception is thrown and the test finishes when TryEnsureSufficientExecutionStack()
            // returns true.
            if (!RuntimeHelpers.TryEnsureSufficientExecutionStack())
            {
                Assert.Throws<InsufficientExecutionStackException>(() => RuntimeHelpers.EnsureSufficientExecutionStack());
                return;
            }
            TryEnsureSufficientExecutionStack_NoSpaceAvaiable_ReturnsFalse();   
        }
    }
}
