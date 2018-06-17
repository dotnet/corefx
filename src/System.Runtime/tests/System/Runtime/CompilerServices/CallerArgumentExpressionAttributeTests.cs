// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.CompilerServices.Tests
{
    public static class CallerArgumentExpressionAttributeTests
    {
        [Theory, InlineData("testParamName"), InlineData(""), InlineData(null)]
        public static void ArgumentToCallerArgumentExpressionSetsParameterNameProperty(string paramName)
        {
            var attr = new CallerArgumentExpressionAttribute(paramName);

            Assert.Equal(paramName, attr.ParameterName);
        }

        [Fact]
        public static void SuppliedArgumentOverridesExpression()
        {
            int notVal = 0;

            string suppliedVal = "supplied value";
            Assert.Equal(suppliedVal, IntParamMethod(notVal, suppliedVal));

            Assert.Equal(IntParamMethod(notVal), IntParamMethodPassthrough(notVal));
        }

        private static string IntParamMethodPassthrough(int val, [CallerArgumentExpression("val")] string expr = null)
        {
            return IntParamMethod(val, expr);
        }

        [Fact]
        public static void InvalidParameterName()
        {
            int notVal = 0;

            Assert.Null(InvalidParameterNameMethod(notVal));
        }

        private static string InvalidParameterNameMethod(int val, [CallerArgumentExpression("notVal")] string expr = null)
        {
            return expr;
        }

        [Fact]
        public static void NullParameterName()
        {
            int notVal = 0;

            Assert.Null(NullParameterNameMethod(notVal));
        }

        private static string NullParameterNameMethod(int val, [CallerArgumentExpression(null)] string expr = null)
        {
            return expr;
        }

        public static string IntParamMethod(int val, [CallerArgumentExpression("val")] string expr = null)
        {
            return expr;
        }
    }
}
