// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.UnitTesting;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.UnitTesting;
using System.Runtime.Serialization;
using Xunit;

namespace System.ComponentModel.Composition
{
    public class CompositionPartExceptionTests
    {
        [Fact]
        public void Constructor1_ShouldSetMessagePropertyToDefault()
        {
            var exception = new ComposablePartException();

            ExceptionAssert.HasDefaultMessage(exception);
        }

        [Fact]
        public void Constructor2_NullAsMessageArgument_ShouldSetMessagePropertyToDefault()
        {
            var exception = new ComposablePartException((string)null);

            ExceptionAssert.HasDefaultMessage(exception);
        }

        [Fact]
        public void Constructor3_NullAsMessageArgument_ShouldSetMessagePropertyToDefault()
        {
            var exception = new ComposablePartException((string)null, ElementFactory.Create());

            ExceptionAssert.HasDefaultMessage(exception);
        }

        [Fact]
        public void Constructor4_NullAsMessageArgument_ShouldSetMessagePropertyToDefault()
        {
            var exception = new ComposablePartException((string)null, new Exception());

            ExceptionAssert.HasDefaultMessage(exception);
        }

        [Fact]
        public void Constructor5_NullAsMessageArgument_ShouldSetMessagePropertyToDefault()
        {
            var exception = new ComposablePartException((string)null, ElementFactory.Create(), new Exception());

            ExceptionAssert.HasDefaultMessage(exception);
        }

        [Fact]
        public void Constructor6_NullAsMessageArgument_ShouldSetMessagePropertyToDefault()
        {
            var exception = new ComposablePartException((string)null);

            ExceptionAssert.HasDefaultMessage(exception);
        }

        [Fact]
        public void Constructor7_NullAsMessageArgument_ShouldSetMessagePropertyToDefault()
        {
            var exception = new ComposablePartException((string)null, new Exception());

            ExceptionAssert.HasDefaultMessage(exception);
        }

        [Fact]
        public void Constructor8_NullAsMessageArgument_ShouldSetMessagePropertyToDefault()
        {
            var exception = new ComposablePartException((string)null, ElementFactory.Create());

            ExceptionAssert.HasDefaultMessage(exception);
        }

        [Fact]
        public void Constructor9_NullAsMessageArgument_ShouldSetMessagePropertyToDefault()
        {
            var exception = new ComposablePartException((string)null, ElementFactory.Create(), new Exception());

            ExceptionAssert.HasDefaultMessage(exception);
        }

        [Fact]
        public void Constructor2_ValueAsMessageArgument_ShouldSetMessageProperty()
        {
            var expectations = Expectations.GetExceptionMessages();

            foreach (var e in expectations)
            {
                var exception = new ComposablePartException(e);

                Assert.Equal(e, exception.Message);
            }
        }

        [Fact]
        public void Constructor3_ValueAsMessageArgument_ShouldSetMessageProperty()
        {
            var expectations = Expectations.GetExceptionMessages();

            foreach (var e in expectations)
            {
                var exception = new ComposablePartException(e, ElementFactory.Create());

                Assert.Equal(e, exception.Message);
            }
        }

        [Fact]
        public void Constructor4_ValueAsMessageArgument_ShouldSetMessageProperty()
        {
            var expectations = Expectations.GetExceptionMessages();

            foreach (var e in expectations)
            {
                var exception = new ComposablePartException(e, new Exception());

                Assert.Equal(e, exception.Message);
            }
        }

        [Fact]
        public void Constructor5_ValueAsMessageArgument_ShouldSetMessageProperty()
        {
            var expectations = Expectations.GetExceptionMessages();

            foreach (var e in expectations)
            {
                var exception = new ComposablePartException(e, ElementFactory.Create(), new Exception());

                Assert.Equal(e, exception.Message);
            }
        }

        [Fact]
        public void Constructor6_ValueAsMessageArgument_ShouldSetMessageProperty()
        {
            var expectations = Expectations.GetExceptionMessages();

            foreach (var e in expectations)
            {
                var exception = new ComposablePartException(e);

                Assert.Equal(e, exception.Message);
            }
        }

        [Fact]
        public void Constructor7_ValueAsMessageArgument_ShouldSetMessageProperty()
        {
            var expectations = Expectations.GetExceptionMessages();

            foreach (var e in expectations)
            {
                var exception = new ComposablePartException(e, new Exception());

                Assert.Equal(e, exception.Message);
            }
        }

        [Fact]
        public void Constructor8_ValueAsMessageArgument_ShouldSetMessageProperty()
        {
            var expectations = Expectations.GetExceptionMessages();

            foreach (var e in expectations)
            {
                var exception = new ComposablePartException(e, ElementFactory.Create());

                Assert.Equal(e, exception.Message);
            }
        }

        [Fact]
        public void Constructor9_ValueAsMessageArgument_ShouldSetMessageProperty()
        {
            var expectations = Expectations.GetExceptionMessages();

            foreach (var e in expectations)
            {
                var exception = new ComposablePartException(e, ElementFactory.Create(), new Exception());

                Assert.Equal(e, exception.Message);
            }
        }

        [Fact]
        public void Constructor1_ShouldSetInnerExceptionPropertyToNull()
        {
            var exception = new ComposablePartException();

            Assert.Null(exception.InnerException);
        }

        [Fact]
        public void Constructor2_ShouldSetInnerExceptionPropertyToNull()
        {
            var exception = new ComposablePartException("Message");

            Assert.Null(exception.InnerException);
        }

        [Fact]
        public void Constructor3_ShouldSetInnerExceptionPropertyToNull()
        {
            var exception = new ComposablePartException("Message", ElementFactory.Create());

            Assert.Null(exception.InnerException);
        }

        [Fact]
        public void Constructor4_NullAsInnerExceptionArgument_ShouldSetInnerExceptionPropertyToNull()
        {
            var exception = new ComposablePartException("Message", (Exception)null);

            Assert.Null(exception.InnerException);
        }

        [Fact]
        public void Constructor5_NullAsInnerExceptionArgument_ShouldSetInnerExceptionPropertyToNull()
        {
            var exception = new ComposablePartException("Message", ElementFactory.Create(), (Exception)null);

            Assert.Null(exception.InnerException);
        }

        [Fact]
        public void Constructor6_ShouldSetInnerExceptionPropertyToNull()
        {
            var exception = new ComposablePartException("Message");

            Assert.Null(exception.InnerException);
        }

        [Fact]
        public void Constructor7_NullAsInnerExceptionArgument_ShouldSetInnerExceptionPropertyToNull()
        {
            var exception = new ComposablePartException("Message", (Exception)null);

            Assert.Null(exception.InnerException);
        }

        [Fact]
        public void Constructor8_ShouldSetInnerExceptionPropertyToNull()
        {
            var exception = new ComposablePartException("Message", ElementFactory.Create());

            Assert.Null(exception.InnerException);
        }

        [Fact]
        public void Constructor9_NullAsInnerExceptionArgument_ShouldSetInnerExceptionPropertyToNull()
        {
            var exception = new ComposablePartException("Message", ElementFactory.Create(), (Exception)null);

            Assert.Null(exception.InnerException);
        }

        [Fact]
        public void Constructor4_ValueAsInnerExceptionArgument_ShouldSetInnerExceptionProperty()
        {
            var expectations = Expectations.GetInnerExceptions();

            foreach (var e in expectations)
            {
                var exception = new ComposablePartException("Message", e);

                Assert.Same(e, exception.InnerException);
            }
        }

        [Fact]
        public void Constructor5_ValueAsInnerExceptionArgument_ShouldSetInnerExceptionProperty()
        {
            var expectations = Expectations.GetInnerExceptions();

            foreach (var e in expectations)
            {
                var exception = new ComposablePartException("Message", ElementFactory.Create(), e);

                Assert.Same(e, exception.InnerException);
            }
        }

        [Fact]
        public void Constructor7_ValueAsInnerExceptionArgument_ShouldSetInnerExceptionProperty()
        {
            var expectations = Expectations.GetInnerExceptions();

            foreach (var e in expectations)
            {
                var exception = new ComposablePartException("Message", e);

                Assert.Same(e, exception.InnerException);
            }
        }

        [Fact]
        public void Constructor9_ValueAsInnerExceptionArgument_ShouldSetInnerExceptionProperty()
        {
            var expectations = Expectations.GetInnerExceptions();

            foreach (var e in expectations)
            {
                var exception = new ComposablePartException("Message", ElementFactory.Create(), e);

                Assert.Same(e, exception.InnerException);
            }
        }

        [Fact]
        public void Constructor1_ShouldSetElementPropertyToNull()
        {
            var exception = new ComposablePartException();

            Assert.Null(exception.Element);
        }

        [Fact]
        public void Constructor2_ShouldSetElementPropertyToNull()
        {
            var exception = new ComposablePartException("Message");

            Assert.Null(exception.Element);
        }

        [Fact]
        public void Constructor3_NullAsElementArgument_ShouldSetElementPropertyToNull()
        {
            var exception = new ComposablePartException("Message", (ICompositionElement)null);

            Assert.Null(exception.Element);
        }

        [Fact]
        public void Constructor4_ShouldSetElementPropertyToNull()
        {
            var exception = new ComposablePartException("Message", new Exception());

            Assert.Null(exception.Element);
        }

        [Fact]
        public void Constructor5_NullAsElementArgument_ShouldSetElementPropertyToNull()
        {
            var exception = new ComposablePartException("Message", (ICompositionElement)null, new Exception());

            Assert.Null(exception.Element);
        }

        [Fact]
        public void Constructor6_ShouldSetElementPropertyToNull()
        {
            var exception = new ComposablePartException("Message");

            Assert.Null(exception.Element);
        }

        [Fact]
        public void Constructor7_ShouldSetElementPropertyToNull()
        {
            var exception = new ComposablePartException("Message", new Exception());

            Assert.Null(exception.Element);
        }

        [Fact]
        public void Constructor8_NullAsElementArgument_ShouldSetElementPropertyToNull()
        {
            var exception = new ComposablePartException("Message", (ICompositionElement)null);

            Assert.Null(exception.Element);
        }

        [Fact]
        public void Constructor9_NullAsElementArgument_ShouldSetElementPropertyToNull()
        {
            var exception = new ComposablePartException("Message", (ICompositionElement)null, new Exception());

            Assert.Null(exception.Element);
        }

        [Fact]
        public void Constructor3_ValueAsElementArgument_ShouldSetElementProperty()
        {
            var expectations = Expectations.GetCompositionElements();

            foreach (var e in expectations)
            {
                var exception = new ComposablePartException("Message", (ICompositionElement)e);

                Assert.Same(e, exception.Element);
            }
        }

        [Fact]
        public void Constructor5_ValueAsElementArgument_ShouldSetElementProperty()
        {
            var expectations = Expectations.GetCompositionElements();

            foreach (var e in expectations)
            {
                var exception = new ComposablePartException("Message", (ICompositionElement)e, new Exception());

                Assert.Same(e, exception.Element);
            }
        }

        [Fact]
        public void Constructor8_ValueAsElementArgument_ShouldSetElementProperty()
        {
            var expectations = Expectations.GetCompositionElements();

            foreach (var e in expectations)
            {
                var exception = new ComposablePartException("Message", (ICompositionElement)e);

                Assert.Same(e, exception.Element);
            }
        }

        [Fact]
        public void Constructor9_ValueAsElementArgument_ShouldSetElementProperty()
        {
            var expectations = Expectations.GetCompositionElements();

            foreach (var e in expectations)
            {
                var exception = new ComposablePartException("Message", (ICompositionElement)e, new Exception());

                Assert.Same(e, exception.Element);
            }
        }

#if FEATURE_SERIALIZATION

        [Fact]
        public void Constructor10_NullAsInfoArgument_ShouldThrowArgumentNull()
        {
            var context = new StreamingContext();

            ExceptionAssert.ThrowsArgument<ArgumentNullException>("info", () =>
            {
                SerializationTestServices.Create<ComposablePartException>((SerializationInfo)null, context);
            });
        }

        [Fact]
        public void Constructor10_SerializationInfoWithMissingElementEntryAsInfoArgument_ShouldThrowSerialization()
        {
            var info = SerializationTestServices.CreateSerializationInfoRemovingMember<ComposablePartException>("Element");
            var context = new StreamingContext();

            ExceptionAssert.ThrowsSerialization("Element", () =>
            {
                SerializationTestServices.Create<ComposablePartException>(info, context);
            });
        }

        [Fact]
        public void Constructor10_SerializationInfoWithWrongTypeForElementEntryAsInfoArgument_ShouldThrowInvalidCast()
        {
            var info = SerializationTestServices.CreateSerializationInfoReplacingMember<ComposablePartException>("Element", 10);
            var context = new StreamingContext();

            ExceptionAssert.Throws<InvalidCastException>(() =>
            {
                SerializationTestServices.Create<ComposablePartException>(info, context);
            });
        }

        [Fact]
        public void Element_CanBeSerialized()
        {
            var expectations = Expectations.GetCompositionElementsWithNull();

            foreach (var e in expectations)
            {
                var exception = CreateComposablePartException(e);

                var result = SerializationTestServices.RoundTrip(exception);

                ElementAssert.Equal(exception.Element, result.Element);
            }
        }

        [Fact]
        public void InnerException_CanBeSerialized()
        {
            var expectations = Expectations.GetInnerExceptions();

            foreach (var e in expectations)
            {
                var exception = CreateComposablePartException(e);

                var result = SerializationTestServices.RoundTrip(exception);

                ExtendedAssert.IsInstanceOfSameType(exception.InnerException, result.InnerException);
            }
        }

        [Fact]
        public void Message_CanBeSerialized()
        {
            var expectations = Expectations.GetExceptionMessages();

            foreach (var e in expectations)
            {
                var exception = CreateComposablePartException(e);

                var result = SerializationTestServices.RoundTrip(exception);

                Assert.Equal(exception.Message, result.Message);
            }
        }

#endif //FEATURE_SERIALIZATION

        private static ComposablePartException CreateComposablePartException()
        {
            return CreateComposablePartException(CompositionErrorId.Unknown, (string)null, (ICompositionElement)null, (Exception)null);
        }

        private static ComposablePartException CreateComposablePartException(string message)
        {
            return CreateComposablePartException(CompositionErrorId.Unknown, message, (ICompositionElement)null, (Exception)null);
        }

        private static ComposablePartException CreateComposablePartException(CompositionErrorId id)
        {
            return CreateComposablePartException(CompositionErrorId.Unknown, (string)null, (ICompositionElement)null, (Exception)null);
        }

        private static ComposablePartException CreateComposablePartException(ICompositionElement element)
        {
            return CreateComposablePartException(CompositionErrorId.Unknown, (string)null, element, (Exception)null);
        }

        private static ComposablePartException CreateComposablePartException(Exception innerException)
        {
            return CreateComposablePartException(CompositionErrorId.Unknown, (string)null, (ICompositionElement)null, innerException);
        }

        private static ComposablePartException CreateComposablePartException(CompositionErrorId id, string message, ICompositionElement element, Exception innerException)
        {
            return new ComposablePartException(message, element, innerException);
        }
    }
}
