// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.UnitTesting;
using System.ComponentModel.Composition.Factories;
using System.Linq;
using System.UnitTesting;
using Microsoft.CLR.UnitTesting;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class CompositionResultOfTTest
    {
        [TestMethod]
        public void Constructor1_ShouldSetErrorsPropertyToEmptyEnumerable()
        {
            var result = new CompositionResult<string>();

            EnumerableAssert.IsEmpty(result.Errors);
        }

        [TestMethod]
        public void Constructor2_ShouldSetErrorsPropertyToEmptyEnumerable()
        {
            var result = new CompositionResult<string>("Value");

            EnumerableAssert.IsEmpty(result.Errors);
        }

        [TestMethod]
        public void Constructor3_NullAsErrorsArgument_ShouldSetErrorsPropertyToEmptyEnumerable()
        {
            var result = new CompositionResult<string>((CompositionError[])null);

            EnumerableAssert.IsEmpty(result.Errors);
        }

        [TestMethod]
        public void Constructor4_NullAsErrorsArgument_ShouldSetErrorsPropertyToEmptyEnumerable()
        {
            var result = new CompositionResult<string>((IEnumerable<CompositionError>)null);

            EnumerableAssert.IsEmpty(result.Errors);
        }

        [TestMethod]
        public void Constructor5_NullAsErrorsArgument_ShouldSetErrorsPropertyToEmptyEnumerable()
        {
            var result = new CompositionResult<string>("Value", (IEnumerable<CompositionError>)null);

            EnumerableAssert.IsEmpty(result.Errors);
        }

        [TestMethod]
        public void Constructor3_EmptyAsErrorsArgument_ShouldSetErrorsPropertyToEmptyEnumerable()
        {
            var result = new CompositionResult<string>(new CompositionError[0]);

            EnumerableAssert.IsEmpty(result.Errors);
        }

        [TestMethod]
        public void Constructor4_EmptyAsErrorsArgument_ShouldSetErrorsPropertyToEmptyEnumerable()
        {
            var result = new CompositionResult<string>(Enumerable.Empty<CompositionError>());

            EnumerableAssert.IsEmpty(result.Errors);
        }

        [TestMethod]
        public void Constructor5_EmptyAsErrorsArgument_ShouldSetErrorsPropertyToEmptyEnumerable()
        {
            var result = new CompositionResult<string>("Value", Enumerable.Empty<CompositionError>());

            EnumerableAssert.IsEmpty(result.Errors);
        }

        [TestMethod]
        public void Constructor3_ValuesAsErrorsArgument_ShouldSetErrorsProperty()
        {
            var errors = Expectations.GetCompositionErrors();

            foreach (var e in errors)
            {
                var result = new CompositionResult<string>(e.ToArray());

                EnumerableAssert.AreEqual(e, result.Errors);
            }
        }

        [TestMethod]
        public void Constructor4_ValuesAsErrorsArgument_ShouldSetErrorsProperty()
        {
            var errors = Expectations.GetCompositionErrors();

            foreach (var e in errors)
            {
                var result = new CompositionResult<string>(e);

                EnumerableAssert.AreEqual(e, result.Errors);
            }
        }

        [TestMethod]
        public void Constructor5_ValuesAsErrorsArgument_ShouldSetErrorsProperty()
        {
            var errors = Expectations.GetCompositionErrors();

            foreach (var e in errors)
            {
                var result = new CompositionResult<string>("Value", e);

                EnumerableAssert.AreEqual(e, result.Errors);
            }
        }

        [TestMethod]
        public void Constructor1_ShouldSetSucceededPropertyToTrue()
        {
            var result = new CompositionResult<string>();

            Assert.IsTrue(result.Succeeded);
        }


        [TestMethod]
        public void Constructor2_ShouldSetSucceededPropertyToTrue()
        {
            var result = new CompositionResult<string>("Value");

            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public void Constructor3_NullAsErrorsArgument_ShouldSetSucceededPropertyToTrue()
        {
            var result = new CompositionResult<string>((CompositionError[])null);

            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public void Constructor4_NullAsErrorsArgument_ShouldSetSucceededPropertyToTrue()
        {
            var result = new CompositionResult<string>((IEnumerable<CompositionError>)null);

            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public void Constructor5_NullAsErrorsArgument_ShouldSetSucceededPropertyToTrue()
        {
            var result = new CompositionResult<string>("Value", (IEnumerable<CompositionError>)null);

            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public void Constructor3_EmptyAsErrorsArgument_ShouldSetSucceededPropertyToTrue()
        {
            var result = new CompositionResult<string>(new CompositionError[0]);

            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public void Constructor4_EmptyAsErrorsArgument_ShouldSetSucceededPropertyToTrue()
        {
            var result = new CompositionResult<string>(Enumerable.Empty<CompositionError>());

            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public void Constructor5_EmptyAsErrorsArgument_ShouldSetSucceededPropertyToTrue()
        {
            var result = new CompositionResult<string>("Value", Enumerable.Empty<CompositionError>());

            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public void Constructor3_ValuesAsErrorsArgument_ShouldSetSucceededPropertyToTrueIfThereAreErrors()
        {
            var errors = Expectations.GetCompositionErrors();

            foreach (var e in errors)
            {
                var result = new CompositionResult<string>(e.ToArray());

                if (e.Count() > 0)
                {
                    Assert.IsFalse(result.Succeeded);
                }
                else
                {
                    Assert.IsTrue(result.Succeeded);
                }
            }
        }

        [TestMethod]
        public void Constructor4_ValuesAsErrorsArgument_ShouldSetSucceededPropertyToTrueIfThereAreErrors()
        {
            var errors = Expectations.GetCompositionErrors();

            foreach (var e in errors)
            {
                var result = new CompositionResult<string>(e);

                if (e.Count() > 0)
                {
                    Assert.IsFalse(result.Succeeded);
                }
                else
                {
                    Assert.IsTrue(result.Succeeded);
                }
            }
        }

        [TestMethod]
        public void Constructor5_ValuesAsErrorsArgument_ShouldSetSucceededPropertyToTrueIfThereAreErrors()
        {
            var errors = Expectations.GetCompositionErrors();

            foreach (var e in errors)
            {
                var result = new CompositionResult<string>("Value", e);

                if (e.Count() > 0)
                {
                    Assert.IsFalse(result.Succeeded);
                }
                else
                {
                    Assert.IsTrue(result.Succeeded);
                }
            }
        }

        [TestMethod]
        public void ToResult_NullAsErrorsArgument_ShouldReturnResultWithEmptyErrorsProperty()
        {
            var result = CreateCompositionResult<string>((IEnumerable<CompositionError>)null);

            var copy = result.ToResult();

            EnumerableAssert.IsEmpty(copy.Errors);
        }

        [TestMethod]
        public void ToResult_EmptyEnumerableAsErrorsArgument_ShouldReturnResultWithEmptyErrorsProperty()
        {
            var result = CreateCompositionResult<string>(Enumerable.Empty<CompositionError>());

            var copy = result.ToResult();

            EnumerableAssert.IsEmpty(copy.Errors);
        }

        [TestMethod]
        public void ToResult_ValueAsErrorsArgument_ShouldReturnResultWithErrorsPropertySet()
        {
            var expectations = Expectations.GetCompositionErrors();

            foreach (var e in expectations)
            {
                var result = CreateCompositionResult<string>(e);

                var copy = result.ToResult();

                EnumerableAssert.AreSequenceSame(result.Errors, copy.Errors);
            }
        }

        [TestMethod]
        public void ToResultOfT_NullAsErrorsArgument_ShouldReturnResultWithEmptyErrorsProperty()
        {
            var result = CreateCompositionResult<string>((IEnumerable<CompositionError>)null);

            var copy = result.ToResult<string>();

            EnumerableAssert.IsEmpty(copy.Errors);
        }

        [TestMethod]
        public void ToResultOfT_EmptyEnumerableAsErrorsArgument_ShouldReturnResultWithEmptyErrorsProperty()
        {
            var result = CreateCompositionResult<string>(Enumerable.Empty<CompositionError>());

            var copy = result.ToResult<string>();

            EnumerableAssert.IsEmpty(copy.Errors);
        }

        [TestMethod]
        public void ToResultOfT_ValueAsErrorsArgument_ShouldReturnResultWithErrorsPropertySet()
        {
            var expectations = Expectations.GetCompositionErrors();

            foreach (var e in expectations)
            {
                var result = CreateCompositionResult<string>(e);

                var copy = result.ToResult<string>();

                EnumerableAssert.AreSequenceSame(result.Errors, copy.Errors);
            }
        }

        [TestMethod]
        public void ToResultOfT_ReferenceValueAsValueArgument_ShouldReturnResultWithNullValueProperty()
        {
            var expectations = Expectations.GetObjectsReferenceTypes();

            foreach (var e in expectations)
            {
                var result = CreateCompositionResult<object>(e);

                var copy = result.ToResult<object>();

                Assert.IsNull(copy.Value);                
            }
        }

        [TestMethod]
        public void ToResultOfT_ValueTypeValueAsValueArgument_ShouldReturnResultWithNullValueProperty()
        {
            var expectations = Expectations.GetObjectsValueTypes();

            foreach (var e in expectations)
            {
                var result = CreateCompositionResult<ValueType>(e);

                var copy = result.ToResult<ValueType>();

                Assert.IsNull(copy.Value);
            }
        }

        [TestMethod]
        public void Value_NullAsErrorsArgumentAndValueAsValueArgument_ShouldReturnValue()
        {
            var expectations = Expectations.GetObjectsReferenceTypes();

            foreach (var e in expectations)
            {
                var result = CreateCompositionResult<object>(e, (IEnumerable<CompositionError>)null);

                Assert.AreEqual(e, result.Value);
            }
        }

        [TestMethod]
        public void Value_EmptyEnumerableAsErrorsArgumentAndValueAsValueArgument_ShouldReturnValue()
        {
            var expectations = Expectations.GetObjectsReferenceTypes();

            foreach (var e in expectations)
            {
                var result = CreateCompositionResult<object>(e, Enumerable.Empty<CompositionError>());

                Assert.AreEqual(e, result.Value);
            }
        }

        [TestMethod]
        public void Value_SingleValueAsErrorsArgument_ShouldThrowComposition()
        {
            var errorIds = Expectations.GetEnumValues<CompositionErrorId>();

            foreach (var errorId in errorIds)
            {
                var result = CreateCompositionResult<string>(errorId);

                CompositionAssert.ThrowsError((ErrorId)errorId, () =>
                {
                    var value = result.Value;
                });
            }
        }

        [TestMethod]
        public void Value_TwoSameValuesAsErrorsArgument_ShouldThrowComposition()
        {
            var errorIds = Expectations.GetEnumValues<CompositionErrorId>();

            foreach (var errorId in errorIds)
            {
                var result = CreateCompositionResult<string>(errorId, errorId);

                CompositionAssert.ThrowsErrors((ErrorId)errorId, (ErrorId)errorId, () =>
                {
                    var value = result.Value;
                });
            }
        }

        [TestMethod]
        public void Value_TwoDifferentValuesAsErrorsArgument_ShouldThrowComposition()
        {
            var errorIds1 = Expectations.GetEnumValues<CompositionErrorId>();
            var errorIds2 = Expectations.GetEnumValues<CompositionErrorId>();

            for (int i = 0; i < errorIds1.Count(); i++)
            {
                var errorId1 = errorIds1.ElementAt(i);
                var errorId2 = errorIds1.ElementAt(errorIds2.Count() - 1 - i);

                var result = CreateCompositionResult<string>(errorId1, errorId2);

                CompositionAssert.ThrowsErrors((ErrorId)errorId1, (ErrorId)errorId2, () =>
                {
                    var value = result.Value;
                });
            }
        }

        private CompositionResult<T> CreateCompositionResult<T>(params CompositionErrorId[] errorIds)
        {
            return new CompositionResult<T>(errorIds.Select(id =>
            {
                return ErrorFactory.Create(id);
            }));
        }

        private CompositionResult<T> CreateCompositionResult<T>(int count)
        {
            var expectations = Expectations.GetEnumValues<CompositionErrorId>();

            List<CompositionError> errors = new List<CompositionError>();

            foreach (var e in expectations.Take(count))
            {
                errors.Add(ErrorFactory.Create(e));
            }

            return CreateCompositionResult<T>(errors);
        }

        private CompositionResult<T> CreateCompositionResult<T>(IEnumerable<CompositionError> errors)
        {
            return new CompositionResult<T>(errors);
        }

        private CompositionResult<T> CreateCompositionResult<T>(T value)
        {
            return new CompositionResult<T>(value);
        }

        private CompositionResult<T> CreateCompositionResult<T>(T value, IEnumerable<CompositionError> errors)
        {
            return new CompositionResult<T>(value, errors);
        }
    }
}
