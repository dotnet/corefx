// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Threading.Tasks.Tests
{
    public static class AggregateExceptionTests
    {
        private const string TestMessage = "AggregateException Test Message";

        [Fact]
        public static void Constructor_Default()
        {
            AggregateException ex = new AggregateException();
            Assert.Empty(ex.InnerExceptions);
            Assert.NotNull(ex.Message);
        }

        [Fact]
        public static void Constructor_Message()
        {
            AggregateException ex = new AggregateException(TestMessage);
            Assert.Empty(ex.InnerExceptions);
            Assert.Equal(TestMessage, ex.Message);
        }

        [Fact]
        public static void Constructor_Exception()
        {
            DeliberateTestException inner = new DeliberateTestException();
            AggregateException ex = new AggregateException(inner);
            Assert.Single(ex.InnerExceptions);
            Assert.Equal(inner, ex.InnerException);
            Assert.NotNull(ex.Message);
        }

        [Fact]
        public static void Constructor_Message_Exception()
        {
            DeliberateTestException inner = new DeliberateTestException();
            AggregateException ex = new AggregateException(TestMessage, inner);
            Assert.Single(ex.InnerExceptions);
            Assert.Equal(inner, ex.InnerException);
            Assert.Equal(TestMessage, ex.Message);
        }

        [Fact]
        public static void ConstructorInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new AggregateException((Exception[])null));
            Assert.Throws<ArgumentNullException>(() => new AggregateException((IEnumerable<Exception>)null));
            Assert.Throws<ArgumentNullException>(() => new AggregateException("message", (Exception)null));
            Assert.Throws<ArgumentNullException>(() => new AggregateException("message", (Exception[])null));
            Assert.Throws<ArgumentNullException>(() => new AggregateException("message", (IEnumerable<Exception>)null));
            Assert.Throws<ArgumentException>(() => new AggregateException(new Exception[] { null }));
            Assert.Throws<ArgumentException>(() => new AggregateException(Enumerable.Repeat((Exception)null, 1)));
            Assert.Throws<ArgumentException>(() => new AggregateException("message", new Exception[] { null }));
            Assert.Throws<ArgumentException>(() => new AggregateException("message", Enumerable.Repeat((Exception)null, 1)));
        }

        [Fact]
        public static void BaseException_Empty()
        {
            AggregateException ex = new AggregateException();
            Assert.Equal(ex.GetBaseException(), ex);

            ex = new AggregateException(new Exception[] { /* empty */ });
            Assert.Equal(ex.GetBaseException(), ex);

            ex = new AggregateException(Enumerable.Empty<Exception>());
            Assert.Equal(ex.GetBaseException(), ex);
        }

        [Fact]
        public static void BaseException_Single()
        {
            Exception inner = new AggregateException();
            Assert.Equal(new AggregateException(inner).GetBaseException(), inner);

            inner = new DeliberateTestException();
            Assert.Equal(new AggregateException(inner).GetBaseException(), inner);

            inner = new DeliberateTestException();
            AggregateException nest = new AggregateException(inner);
            Assert.Equal(new AggregateException(nest).GetBaseException(), inner);
        }

        [Fact]
        public static void BaseException_Multiple()
        {
            AggregateException ex = new AggregateException(Enumerable.Repeat(new AggregateException(), 2));
            Assert.Equal(ex.GetBaseException(), ex);

            ex = new AggregateException(Enumerable.Repeat(new DeliberateTestException(), 2));
            Assert.Equal(ex.GetBaseException(), ex);

            ex = new AggregateException(new AggregateException(), new DeliberateTestException());
            Assert.Equal(ex.GetBaseException(), ex);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3)]
        public static void Handle(int count)
        {
            AggregateException ex = new AggregateException(Enumerable.Repeat(new DeliberateTestException(), count));
            int handledCount = 0;

            ex.Handle(e =>
            {
                if (e is DeliberateTestException)
                {
                    handledCount++;
                    return true;
                }
                return false;
            });

            Assert.Equal(count, handledCount);
            Assert.Equal(count, ex.InnerExceptions.Count);
        }

        [Fact]
        public static void HandleInvalidCases()
        {
            Assert.Throws<ArgumentNullException>(() => new AggregateException().Handle(null));

            AggregateException ex = new AggregateException(new[] { new Exception(), new DeliberateTestException() });
            Assert.Throws<AggregateException>(() => ex.Handle(e => e is DeliberateTestException));

            ex = new AggregateException(new Exception());
            Assert.Throws<DeliberateTestException>(() => ex.Handle(e => { throw new DeliberateTestException(); }));
        }

        [Fact]
        public static void Flatten_SingleLevel()
        {
            Exception[] exceptions = new[] { new DeliberateTestException(), new DeliberateTestException(), new DeliberateTestException() };

            AggregateException ae = new AggregateException(exceptions);

            Assert.Equal(exceptions, ae.InnerExceptions);
            Assert.Equal(exceptions, ae.Flatten().InnerExceptions);
        }

        [Fact]
        public static void Flatten_Nested()
        {
            Exception[] exceptions = new[] { new DeliberateTestException(), new DeliberateTestException(), new DeliberateTestException() };

            AggregateException ae = new AggregateException(new AggregateException(exceptions), new AggregateException(exceptions));

            Assert.NotEqual(exceptions, ae.InnerExceptions);
            Assert.All(ae.InnerExceptions, e => Assert.Equal(exceptions, ((AggregateException)e).InnerExceptions));
            // Exceptions are not de-duplicated.
            Assert.Equal(exceptions.Concat(exceptions), ae.Flatten().InnerExceptions);
        }
    }
}
