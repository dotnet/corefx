// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.UnitTesting;
using Microsoft.CLR.UnitTesting;
using System.Runtime.Serialization;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class CardinalityMismatchExceptionTests
    {
        [TestMethod]
        public void Constructor1_ShouldSetMessagePropertyToDefault()
        {
            var exception = new ImportCardinalityMismatchException();

            ExceptionAssert.HasDefaultMessage(exception);
        }

        [TestMethod]
        public void Constructor2_NullAsMessageArgument_ShouldSetMessagePropertyToDefault()
        {
            var exception = new ImportCardinalityMismatchException((string)null);

            ExceptionAssert.HasDefaultMessage(exception);
        }

        [TestMethod]
        public void Constructor3_NullAsMessageArgument_ShouldSetMessagePropertyToDefault()
        {
            var exception = new ImportCardinalityMismatchException((string)null, new Exception());

            ExceptionAssert.HasDefaultMessage(exception);
        }

        [TestMethod]
        public void Constructor2_ValueAsMessageArgument_ShouldSetMessageProperty()
        {
            var expectations = Expectations.GetExceptionMessages();

            foreach (var e in expectations)
            {
                var exception = new ImportCardinalityMismatchException(e);

                Assert.AreEqual(e, exception.Message);
            }
        }

        [TestMethod]
        public void Constructor3_ValueAsMessageArgument_ShouldSetMessageProperty()
        {
            var expectations = Expectations.GetExceptionMessages();

            foreach (var e in expectations)
            {
                var exception = new ImportCardinalityMismatchException(e, new Exception());

                Assert.AreEqual(e, exception.Message);
            }
        }

        [TestMethod]
        public void Constructor1_ShouldSetInnerExceptionPropertyToNull()
        {
            var exception = new ImportCardinalityMismatchException();

            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void Constructor2_ShouldSetInnerExceptionPropertyToNull()
        {
            var exception = new ImportCardinalityMismatchException("Message");

            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void Constructor3_NullAsInnerExceptionArgument_ShouldSetInnerExceptionPropertyToNull()
        {
            var exception = new ImportCardinalityMismatchException("Message", (Exception)null);

            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public void Constructor3_ValueAsInnerExceptionArgument_ShouldSetInnerExceptionProperty()
        {
            var expectations = Expectations.GetInnerExceptions();

            foreach (var e in expectations)
            {
                var exception = new ImportCardinalityMismatchException("Message", e);

                Assert.AreSame(e, exception.InnerException);
            }
        }

#if FEATURE_SERIALIZATION

        [TestMethod]
        public void Constructor4_NullAsInfoArgument_ShouldThrowArgumentNull()
        {
            var context = new StreamingContext();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("info", () =>
            {
                SerializationTestServices.Create<ImportCardinalityMismatchException>((SerializationInfo)null, context);
            });
        }

        [TestMethod]
        public void InnerException_CanBeSerialized()
        {
            var expectations = Expectations.GetInnerExceptionsWithNull();

            foreach (var e in expectations)
            {
                var exception = CreateCardinalityMismatchException(e);

                var result = SerializationTestServices.RoundTrip(exception);

                ExtendedAssert.IsInstanceOfSameType(exception.InnerException, result.InnerException);
            }
        }

        [TestMethod]
        public void Message_CanBeSerialized()
        {
            var expectations = Expectations.GetExceptionMessages();

            foreach (var e in expectations)
            {
                var exception = CreateCardinalityMismatchException(e);

                var result = SerializationTestServices.RoundTrip(exception);

                Assert.AreEqual(exception.Message, result.Message);
            }
        }

#endif //FEATURE_SERIALIZATION

        private static ImportCardinalityMismatchException CreateCardinalityMismatchException()
        {
            return CreateCardinalityMismatchException((string)null, (Exception)null);
        }

        private static ImportCardinalityMismatchException CreateCardinalityMismatchException(string message)
        {
            return CreateCardinalityMismatchException(message, (Exception)null);
        }

        private static ImportCardinalityMismatchException CreateCardinalityMismatchException(Exception innerException)
        {
            return CreateCardinalityMismatchException((string)null, innerException);
        }

        private static ImportCardinalityMismatchException CreateCardinalityMismatchException(string message, Exception innerException)
        {
            return new ImportCardinalityMismatchException(message, innerException);
        }
    }
}
