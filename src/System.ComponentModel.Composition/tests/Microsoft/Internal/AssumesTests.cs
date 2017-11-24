// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.Internal
{
    [ActiveIssue(123456789)]
    public class AssumesTests
    {
        [Fact]
        public void NotNullOfT_NullAsValueArgument_ShouldThrowInternalErrorException()
        {
            Throws(() =>
            {
                Assumes.NotNull<string>((string)null);
            });
        }

        [Fact]
        public void NotNullOfT1T2_NullAsValue1Argument_ShouldThrowInternalErrorException()
        {
            Throws(() =>
            {
                Assumes.NotNull<string, string>((string)null, "Value");
            });
        }

        [Fact]
        public void NotNullOfT1T2_NullAsValue2Argument_ShouldThrowInternalErrorException()
        {
            Throws(() =>
            {
                Assumes.NotNull<string, string>("Value", (string)null);
            });
        }

        [Fact]
        public void NotNullOfT1T2_NullAsValue1ArgumentAndValue2Argument_ShouldThrowInternalErrorException()
        {
            Throws(() =>
            {
                Assumes.NotNull<string, string>((string)null, (string)null);
            });
        }

        [Fact]
        public void NotNullOfT1T2T3_NullAsValue1Argument_ShouldThrowInternalErrorException()
        {
            Throws(() =>
            {
                Assumes.NotNull<string, string, string>((string)null, "Value", "Value");
            });
        }

        [Fact]
        public void NotNullOfT1T2T3_NullAsValue2Argument_ShouldThrowInternalErrorException()
        {
            Throws(() =>
            {
                Assumes.NotNull<string, string, string>("Value", (string)null, "Value");
            });
        }

        [Fact]
        public void NotNullOfT1T2T3_NullAsValue3Argument_ShouldThrowInternalErrorException()
        {
            Throws(() =>
            {
                Assumes.NotNull<string, string, string>("Value", "Value", (string)null);
            });
        }

        [Fact]
        public void NotNullOfT1T2T3_NullAsValue1ArgumentAndValue2Argument_ShouldThrowInternalErrorException()
        {
            Throws(() =>
            {
                Assumes.NotNull<string, string, string>((string)null, (string)null, "Value");
            });
        }

        [Fact]
        public void NotNullOfT1T2T3_NullAsValue1ArgumentAnd3_ShouldThrowInternalErrorException()
        {
            Throws(() =>
            {
                Assumes.NotNull<string, string, string>((string)null, "Value", (string)null);
            });
        }

        [Fact]
        public void NotNullOfT1T2T3_NullAsValue2ArgumentAndValue3Argument_ShouldThrowInternalErrorException()
        {
            Throws(() =>
            {
                Assumes.NotNull<string, string, string>("Value", (string)null, (string)null);
            });
        }

        [Fact]
        public void NotNullOfT_ValueAsValueArgument_ShouldNotThrow()
        {
            Assumes.NotNull<string>("Value");
        }

        [Fact]
        public void NotNullOfT1T2_ValueAsValue1ArgumentAndValue2Argument_ShouldNotThrow()
        {
            Assumes.NotNull<string, string>("Value", "Value");
        }

        [Fact]
        public void NotNullOfT1T2T3_ValueAsValue1ArgumentAndValue2ArgumentAndValue3Argument_ShouldNotThrow()
        {
            Assumes.NotNull<string, string, string>("Value", "Value", "Value");
        }

        [Fact]
        public void NotNullOrEmpty_NullAsValueArgument_ShouldThrowInternalErrorException()
        {
            Throws(() =>
            {
                Assumes.NotNullOrEmpty((string)null);
            });
        }

        [Fact]
        public void NotNullOrEmpty_EmptyAsValueArgument_ShouldThrowInternalErrorException()
        {
            Throws(() =>
            {
                Assumes.NotNullOrEmpty("");
            });
        }

        [Fact]
        public void NotNullOrEmpty_ValueAsValueArgument_ShouldNotThrow()
        {
            var expectations = new List<string>();
            expectations.Add(" ");
            expectations.Add("  ");
            expectations.Add("   ");
            expectations.Add("Value");

            foreach (var e in expectations)
            {
                Assumes.NotNullOrEmpty(e);
            }
        }

        [Fact]
        public void IsTrue1_FalseAsConditionArgument_ShouldThrowInternalErrorException()
        {
            Throws(() =>
            {
                Assumes.IsTrue(false);
            });
        }

        [Fact]
        public void IsTrue2_FalseAsConditionArgument_ShouldThrowInternalErrorException()
        {
            Throws(() =>
            {
                Assumes.IsTrue(false, "Message");
            });
        }

        [Fact]
        public void IsTrue1_TrueAsConditionArgument_ShouldNotThrow()
        {
            Assumes.IsTrue(true);
        }

        [Fact]
        public void IsTrue2_TrueAsConditionArgument_ShouldNotThrow()
        {
            Assumes.IsTrue(true, "Message");
        }

        [Fact]
        public void NotReachable_ShouldAlwaysThrow()
        {
            Throws(() =>
            {
                Assumes.NotReachable<object>();
            });
        }

        private static void Throws(Action action)
        {
            try
            {
                action();
                Assert.False(true);
            }
            catch (Exception ex)
            {
                Type exceptionType = ex.GetType();

                // The exception should not be a 
                // publicily catchable exception
                Assert.False(exceptionType.IsVisible);
            }
        }

#if FEATURE_SERIALIZATION
        [Fact]
        public void Message_CanBeSerialized()
        {
            var expectations = Expectations.GetExceptionMessages();

            foreach (var e in expectations)
            {
                var exception = CreateInternalErrorException(e);

                var result = SerializationTestServices.RoundTrip(exception);

                Assert.Equal(exception.Message, result.Message);
            }
        }

#endif //FEATURE_SERIALIZATION
        private static Exception CreateInternalErrorException()
        {
            return CreateInternalErrorException((string)null);
        }

        private static Exception CreateInternalErrorException(string message) => Assert.ThrowsAny<Exception>(() => Assumes.IsTrue(false, message));
    }
}
