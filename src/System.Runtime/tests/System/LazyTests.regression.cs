using System.Reflection;
using System.Threading;
using Xunit;

namespace System
{
	/// <summary>
    /// This regression suite was designed for testing of a replacement for the original Lazy implementation.
    /// It does contain some overlap with the existing tests, but I feel that it is still of worth to keep
    /// the original suite as well as this one. These tests were written with the express purpose of testing
    /// that the implementation conforms to the existing implementation, rather than to specification.
    /// The two reasons obviously have major overlap (obviously), but leads to the flavour of this suite 
    /// being rather dense and repeditive, which for regression I feel is suitable.
    /// </summary>
    public static partial class LazyTests
    {
        class MyException
            : Exception
        {
            public int Value { get; }

            public MyException(int value)
            {
                Value = value;
            }
        }

        public class Simple
        {
            public int Value { get; }

            public Simple(int value)
            {
                Value = value;
            }

            public Simple()
            {
                Value = 42;
            }
        }

        private static void CheckValueIs42(Lazy<int> lazy)
        {
            Assert.False(lazy.IsValueCreated);
            Assert.Equal(lazy.Value, 42);
            Assert.True(lazy.IsValueCreated);
        }

        private static void CheckValueIs42(Lazy<Simple> lazy)
        {
            Assert.False(lazy.IsValueCreated);
            Assert.Equal(lazy.Value.Value, 42);
            Assert.True(lazy.IsValueCreated);
        }

        [Fact]
        public static void TrivialBaselineCheck()
        {
            CheckValueIs42(new Lazy<int>(() => 42));
            CheckValueIs42(new Lazy<int>(() => 42, true));
            CheckValueIs42(new Lazy<int>(() => 42, false));
            CheckValueIs42(new Lazy<int>(() => 42, LazyThreadSafetyMode.ExecutionAndPublication));
            CheckValueIs42(new Lazy<int>(() => 42, LazyThreadSafetyMode.None));
            CheckValueIs42(new Lazy<int>(() => 42, LazyThreadSafetyMode.PublicationOnly));

            CheckValueIs42(new Lazy<Simple>());
            CheckValueIs42(new Lazy<Simple>(true));
            CheckValueIs42(new Lazy<Simple>(false));
            CheckValueIs42(new Lazy<Simple>(LazyThreadSafetyMode.ExecutionAndPublication));
            CheckValueIs42(new Lazy<Simple>(LazyThreadSafetyMode.None));
            CheckValueIs42(new Lazy<Simple>(LazyThreadSafetyMode.PublicationOnly));
            CheckValueIs42(new Lazy<Simple>(() => new Simple(42)));
            CheckValueIs42(new Lazy<Simple>(() => new Simple(42), true));
            CheckValueIs42(new Lazy<Simple>(() => new Simple(42), false));
            CheckValueIs42(new Lazy<Simple>(() => new Simple(42), LazyThreadSafetyMode.ExecutionAndPublication));
            CheckValueIs42(new Lazy<Simple>(() => new Simple(42), LazyThreadSafetyMode.None));
            CheckValueIs42(new Lazy<Simple>(() => new Simple(42), LazyThreadSafetyMode.PublicationOnly));
        }

        public class SimpleException
        {
            public SimpleException() : this(99) { }

            public SimpleException(int value)
            {
                throw new MyException(value);
            }
        }

        private static void CheckException<T>(Type expected, Lazy<T> lazy)
        {
            Exception e = null;
            try
            {
                var t = lazy.Value;
            }
            catch (Exception ex)
            {
                e = ex;
            }
            Assert.NotNull(e);
            Assert.Same(expected, e.GetType());
        }

        [Fact]
        public static void CheckExceptions()
        {
            CheckException(typeof(MyException), new Lazy<int>(() => { throw new MyException(99); }));
            CheckException(typeof(MyException), new Lazy<int>(() => { throw new MyException(99); }, true));
            CheckException(typeof(MyException), new Lazy<int>(() => { throw new MyException(99); }, false));
            CheckException(typeof(MyException), new Lazy<int>(() => { throw new MyException(99); }, LazyThreadSafetyMode.ExecutionAndPublication));
            CheckException(typeof(MyException), new Lazy<int>(() => { throw new MyException(99); }, LazyThreadSafetyMode.None));
            CheckException(typeof(MyException), new Lazy<int>(() => { throw new MyException(99); }, LazyThreadSafetyMode.PublicationOnly));

			// These functions, possibly unintuitively, return a TargetInvocationException
            CheckException(typeof(TargetInvocationException), new Lazy<SimpleException>());
            CheckException(typeof(TargetInvocationException), new Lazy<SimpleException>(true));
            CheckException(typeof(TargetInvocationException), new Lazy<SimpleException>(false));
            CheckException(typeof(TargetInvocationException), new Lazy<SimpleException>(LazyThreadSafetyMode.ExecutionAndPublication));
            CheckException(typeof(TargetInvocationException), new Lazy<SimpleException>(LazyThreadSafetyMode.None));
            CheckException(typeof(TargetInvocationException), new Lazy<SimpleException>(LazyThreadSafetyMode.PublicationOnly));

            CheckException(typeof(MyException), new Lazy<SimpleException>(() => new SimpleException(99)));
            CheckException(typeof(MyException), new Lazy<SimpleException>(() => new SimpleException(99), true));
            CheckException(typeof(MyException), new Lazy<SimpleException>(() => new SimpleException(99), false));
            CheckException(typeof(MyException), new Lazy<SimpleException>(() => new SimpleException(99), LazyThreadSafetyMode.ExecutionAndPublication));
            CheckException(typeof(MyException), new Lazy<SimpleException>(() => new SimpleException(99), LazyThreadSafetyMode.None));
            CheckException(typeof(MyException), new Lazy<SimpleException>(() => new SimpleException(99), LazyThreadSafetyMode.PublicationOnly));
        }

        private static void SameException<T>(Lazy<T> x)
        {
            Exception first = null;
            try
            {
                var _ = x.Value;
            }
            catch (Exception thrown1)
            {
                first = thrown1;
            }
            Assert.NotNull(first);

            try
            {
                var _ = x.Value;
            }
            catch (MyException thrown2)
            {
                Assert.Same(first, thrown2);
            }
        }

        private static void DifferentException<T>(Lazy<T> x)
        {
            Exception first = null;
            try
            {
                var _ = x.Value;
            }
            catch (Exception thrown1)
            {
                first = thrown1;
            }
            Assert.NotNull(first);

            Exception second = null;
            try
            {
                var _ = x.Value;
            }
            catch (Exception thrown2)
            {
                second = thrown2;
            }
            Assert.NotNull(second);

            Assert.NotEqual(first, second);
        }

        [Fact]
        public static void SameOrDifferentException()
        {
            SameException(new Lazy<int>(() => { throw new MyException(99); }));
            SameException(new Lazy<int>(() => { throw new MyException(99); }, true));
            SameException(new Lazy<int>(() => { throw new MyException(99); }, false));
            SameException(new Lazy<int>(() => { throw new MyException(99); }, LazyThreadSafetyMode.ExecutionAndPublication));
            SameException(new Lazy<int>(() => { throw new MyException(99); }, LazyThreadSafetyMode.None));

            DifferentException(new Lazy<int>(() => { throw new MyException(99); }, LazyThreadSafetyMode.PublicationOnly));

            DifferentException(new Lazy<SimpleException>());
            DifferentException(new Lazy<SimpleException>(true));
            DifferentException(new Lazy<SimpleException>(false));
            DifferentException(new Lazy<SimpleException>(LazyThreadSafetyMode.ExecutionAndPublication));
            DifferentException(new Lazy<SimpleException>(LazyThreadSafetyMode.None));

            DifferentException(new Lazy<SimpleException>(LazyThreadSafetyMode.PublicationOnly));

            SameException(new Lazy<SimpleException>(() => new SimpleException(99)));
            SameException(new Lazy<SimpleException>(() => new SimpleException(99), true));
            SameException(new Lazy<SimpleException>(() => new SimpleException(99), false));
            SameException(new Lazy<SimpleException>(() => new SimpleException(99), LazyThreadSafetyMode.ExecutionAndPublication));
            SameException(new Lazy<SimpleException>(() => new SimpleException(99), LazyThreadSafetyMode.None));

            DifferentException(new Lazy<SimpleException>(() => new SimpleException(99), LazyThreadSafetyMode.PublicationOnly));
        }

        private static void MultipleCallsToIntValue(Lazy<int> lazy, ref int counter, int expected)
        {
            counter = 0;
            var result = 0;
            for (var i = 0; i < 10; ++i)
            {
                try { result = lazy.Value; } catch (Exception) { }
            }
            Assert.Equal(result, expected);
        }

        private static void MultipleCallsToObjectValue(Lazy<Simple> lazy, ref int counter, int? expected)
        {
            counter = 0;
            var result = default(Simple);
            for (var i = 0; i < 10; ++i)
            {
                try { result = lazy.Value; } catch (Exception) { }
            }
            if (expected == null)
                Assert.Null(result);
            else
                Assert.Equal(result.Value, expected.Value);
        }

        [Fact]
        public static void MultipleValueCalls()
        {
            var counter = default(int); // set in test function

            var fint = new Func<int>   (() => { if (++counter < 5) throw new MyException(42); else return counter; });
            var fobj = new Func<Simple>(() => { if (++counter < 5) throw new MyException(42); else return new Simple(counter); });

            MultipleCallsToIntValue(new Lazy<int>(fint), ref counter, 0);
            MultipleCallsToIntValue(new Lazy<int>(fint, true), ref counter, 0);
            MultipleCallsToIntValue(new Lazy<int>(fint, false), ref counter, 0);
            MultipleCallsToIntValue(new Lazy<int>(fint, LazyThreadSafetyMode.ExecutionAndPublication), ref counter, 0);
            MultipleCallsToIntValue(new Lazy<int>(fint, LazyThreadSafetyMode.None), ref counter, 0);
            MultipleCallsToIntValue(new Lazy<int>(fint, LazyThreadSafetyMode.PublicationOnly), ref counter, 5);

            MultipleCallsToObjectValue(new Lazy<Simple>(fobj), ref counter, null);
            MultipleCallsToObjectValue(new Lazy<Simple>(fobj, true), ref counter, null);
            MultipleCallsToObjectValue(new Lazy<Simple>(fobj, false), ref counter, null);
            MultipleCallsToObjectValue(new Lazy<Simple>(fobj, LazyThreadSafetyMode.ExecutionAndPublication), ref counter, null);
            MultipleCallsToObjectValue(new Lazy<Simple>(fobj, LazyThreadSafetyMode.None), ref counter, null);
            MultipleCallsToObjectValue(new Lazy<Simple>(fobj, LazyThreadSafetyMode.PublicationOnly), ref counter, 5);
        }

        class SimpleConstructor
        {
            public static int counter = 0;
            public static int getValue()
            {
                if (++counter < 5)
                    throw new MyException(42);
                else
                    return counter;
            }

            public int Value { get; }

            public SimpleConstructor()
            {
                Value = getValue();
            }
        }

        private static void MultipleCallsToConstructor(Lazy<SimpleConstructor> lazy, int? expected)
        {
            SimpleConstructor.counter = 0;
            var result = default(SimpleConstructor);
            for (var i = 0; i < 10; ++i)
            {
                try { result = lazy.Value; } catch (Exception) { }
            }
            if (expected == null)
                Assert.Null(result);
            else
                Assert.Equal(result.Value, expected.Value);
        }

        [Fact]
        public static void MultipleCallsToExceptionThrowingConstructor()
        {
            MultipleCallsToConstructor(new Lazy<SimpleConstructor>(), 5);
            MultipleCallsToConstructor(new Lazy<SimpleConstructor>(true), 5);
            MultipleCallsToConstructor(new Lazy<SimpleConstructor>(false), 5);
            MultipleCallsToConstructor(new Lazy<SimpleConstructor>(LazyThreadSafetyMode.ExecutionAndPublication), 5);
            MultipleCallsToConstructor(new Lazy<SimpleConstructor>(LazyThreadSafetyMode.None), 5);
            MultipleCallsToConstructor(new Lazy<SimpleConstructor>(LazyThreadSafetyMode.PublicationOnly), 5);
        }

        private static void CheckForInvalidOperationException<T>(ref Lazy<T> x, Lazy<T> lazy)
        {
            x = lazy;

            var correct = false;
            try
            {
                var _ = lazy.Value;
            }
            catch (InvalidOperationException)
            {
                correct = true;
            }
            Assert.True(correct);
        }

        [Fact]
        public static void LazyRecursion()
        {
            Lazy<int> x = null;
            Func<int> f = () => x.Value;

            CheckForInvalidOperationException(ref x, new Lazy<int>(f));
            CheckForInvalidOperationException(ref x, new Lazy<int>(f, true));
            CheckForInvalidOperationException(ref x, new Lazy<int>(f, false));
            CheckForInvalidOperationException(ref x, new Lazy<int>(f, LazyThreadSafetyMode.ExecutionAndPublication));
            CheckForInvalidOperationException(ref x, new Lazy<int>(f, LazyThreadSafetyMode.None));

            // this just stackoverflows in current and new implementation
            // CheckForInvalidOperationException(ref x, new Lazy<int>(f, LazyThreadSafetyMode.PublicationOnly));
        }
    }
}
