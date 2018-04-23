// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Configuration
{

    public class SettingsPropertyIsReadOnlyExceptionTests
    {
        [Fact]
        public void SingleParameterExceptionReturnsExpected()
        {
            try
            {
                throw new SettingsPropertyWrongTypeException("ThisIsATest");
            }
            catch (SettingsPropertyWrongTypeException exception)
            {
                Assert.Equal("ThisIsATest", exception.Message);
            }
        }

        [Fact]
        public void ExceptionWithInnerExceptionExceptionReturnsExpected()
        {
            try
            {
                throw new SettingsPropertyWrongTypeException("ThisIsATest", new AggregateException("AlsoATest"));
            }
            catch (SettingsPropertyWrongTypeException exception)
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
                throw new SettingsPropertyWrongTypeException();
            }
            catch (SettingsPropertyWrongTypeException exception)
            {
                Assert.IsType<SettingsPropertyWrongTypeException>(exception);
            }
        }
    }
}
