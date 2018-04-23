// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Configuration
{
    public class SettingsPropertyNotFoundExceptionTests
    {
        [Fact]
        public void SingleParameterExceptionReturnsExpected()
        {
            try
            {
                throw new SettingsPropertyNotFoundException("ThisIsATest");
            }
            catch (SettingsPropertyNotFoundException exception)
            {
                Assert.Equal("ThisIsATest", exception.Message);
            }
        }

        [Fact]
        public void ExceptionWithInnerExceptionExceptionReturnsExpected()
        {
            try
            {
                throw new SettingsPropertyNotFoundException("ThisIsATest", new AggregateException("AlsoATest"));
            }
            catch (SettingsPropertyNotFoundException exception)
            {
                Assert.Equal("ThisIsATest", exception.Message);
                Assert.Equal("AlsoATest", exception.InnerException.Message);
                Assert.IsType<AggregateException>(exception.InnerException);
            }
        }

        [Fact]
        public void ExceptionEmptyConstructorReturnsExpected()
        {
            try
            {
                throw new SettingsPropertyNotFoundException();
            }
            catch (SettingsPropertyNotFoundException exception)
            {
                Assert.IsType<SettingsPropertyNotFoundException>(exception);
            }
        }
    }
}
