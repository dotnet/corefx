// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Configuration
{

    public class SettingsPropertyWrongTypeExceptionTests
    {
        [Fact]
        public void SingleParameterExceptionReturnsExpected()
        {
            try
            {
                throw new SettingsPropertyIsReadOnlyException("ThisIsATest");
            }
            catch (SettingsPropertyIsReadOnlyException exception)
            {
                Assert.Equal("ThisIsATest", exception.Message);
            }
        }

        [Fact]
        public void ExceptionWithInnerExceptionExceptionReturnsExpected()
        {
            try
            {
                throw new SettingsPropertyIsReadOnlyException("ThisIsATest", new AggregateException("AlsoATest"));
            }
            catch (SettingsPropertyIsReadOnlyException exception)
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
                throw new SettingsPropertyIsReadOnlyException();
            }
            catch (SettingsPropertyIsReadOnlyException exception)
            {
                Assert.IsType<SettingsPropertyIsReadOnlyException>(exception);
            }
        }
    }
}
