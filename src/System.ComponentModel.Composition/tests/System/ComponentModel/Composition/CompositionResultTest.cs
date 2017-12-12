// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.UnitTesting;
using System.Linq;
using System.UnitTesting;
using Microsoft.CLR.UnitTesting;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Factories;

namespace System.ComponentModel.Composition
{
    [TestClass]
    public class CompositionResultTest
    {
        [TestMethod]
        public void Constructor1_ShouldSetErrorsPropertyToEmptyEnumerable()
        {
            var result = new CompositionResult();

            EnumerableAssert.IsEmpty(result.Errors);
        }

        [TestMethod]
        public void Constructor2_NullAsErrorsArgument_ShouldSetErrorsPropertyToEmptyEnumerable()
        {
            var result = new CompositionResult((CompositionError[])null);

            EnumerableAssert.IsEmpty(result.Errors);
        }

        [TestMethod]
        public void Constructor3_NullAsErrorsArgument_ShouldSetErrorsPropertyToEmptyEnumerable()
        {
            var result = new CompositionResult((IEnumerable<CompositionError>)null);

            EnumerableAssert.IsEmpty(result.Errors);
        }

        [TestMethod]
        public void Constructor2_EmptyAsErrorsArgument_ShouldSetErrorsPropertyToEmptyEnumerable()
        {
            var result = new CompositionResult(new CompositionError[0]);

            EnumerableAssert.IsEmpty(result.Errors);
        }

        [TestMethod]
        public void Constructor3_EmptyAsErrorsArgument_ShouldSetErrorsPropertyToEmptyEnumerable()
        {
            var result = new CompositionResult(Enumerable.Empty<CompositionError>());

            EnumerableAssert.IsEmpty(result.Errors);
        }

        [TestMethod]
        public void Constructor2_ValuesAsErrorsArgument_ShouldSetErrorsProperty()
        {
            var errors = Expectations.GetCompositionErrors();

            foreach (var e in errors)
            {
                var result = new CompositionResult(e.ToArray());

                EnumerableAssert.AreEqual(e, result.Errors);
            }
        }

        [TestMethod]
        public void Constructor3_ValuesAsErrorsArgument_ShouldSetErrorsProperty()
        {
            var errors = Expectations.GetCompositionErrors();

            foreach (var e in errors)
            {
                var result = new CompositionResult(e);

                EnumerableAssert.AreEqual(e, result.Errors);
            }
        }

        [TestMethod]
        public void Constructor1_ShouldSetSucceededPropertyToTrue()
        {
            var result = new CompositionResult();

            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public void Constructor2_NullAsErrorsArgument_ShouldSetSucceededPropertyToTrue()
        {
            var result = new CompositionResult((CompositionError[])null);

            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public void Constructor3_NullAsErrorsArgument_ShouldSetSucceededPropertyToTrue()
        {
            var result = new CompositionResult((IEnumerable<CompositionError>)null);

            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public void Constructor2_EmptyAsErrorsArgument_ShouldSetSucceededPropertyToTrue()
        {
            var result = new CompositionResult(new CompositionError[0]);

            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public void Constructor3_EmptyAsErrorsArgument_ShouldSetSucceededPropertyToTrue()
        {
            var result = new CompositionResult(Enumerable.Empty<CompositionError>());

            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public void Constructor2_ValuesAsErrorsArgument_ShouldSetSucceededPropertyToTrueIfThereAreErrors()
        {
            var errors = Expectations.GetCompositionErrors();

            foreach (var e in errors)
            {
                var result = new CompositionResult(e.ToArray());

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
        public void Constructor3_ValuesAsErrorsArgument_ShouldSetSucceededPropertyToTrueIfThereAreErrors()
        {
            var errors = Expectations.GetCompositionErrors();

            foreach (var e in errors)
            {
                var result = new CompositionResult(e);

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
        public void MergeResult_ResultWithNullErrorsAsResultArgument_ShouldReturnResultWithSameErrors()
        {
            var emptyResult = CreateCompositionResult((IEnumerable<CompositionError>)null);

            var expectations = Expectations.GetCompositionErrors();

            foreach (var e in expectations)
            {
                var result = CreateCompositionResult(e);

                var mergedResult = result.MergeResult(emptyResult);

                CompositionAssert.AreEqual(result, mergedResult);
            }
        }

        [TestMethod]
        public void MergeResult_ResultWithEmptyErrorsAsResultArgument_ShouldReturnResultWithSameErrors()
        {
            var emptyResult = CreateCompositionResult(Enumerable.Empty<CompositionError>());

            var expectations = Expectations.GetCompositionErrors();

            foreach (var e in expectations)
            {
                var result = CreateCompositionResult(e);

                var mergedResult = result.MergeResult(emptyResult);

                CompositionAssert.AreEqual(result, mergedResult);
            }
        }

        [TestMethod]
        public void MergeResult_ResultWithErrorsAsResultArgument_ShouldReturnResultWithCombinedErrors()
        {
            var expectations = Expectations.GetCompositionErrors();

            foreach (var e in expectations)
            {
                var result1 = CreateCompositionResult(e);
                var result2 = CreateCompositionResult(e);

                var mergedResult = result1.MergeResult(result2);
                var mergedErrors = result1.Errors.Concat(result2.Errors);

                EnumerableAssert.AreSequenceSame(mergedErrors, mergedResult.Errors);
                Assert.AreEqual(mergedResult.Succeeded, result1.Succeeded | result2.Succeeded);
            }
        }

        [TestMethod]
        public void MergeError_ValueAsErrorArgumentWhenMergedWithResultWithNullErrors_ShouldReturnResultWithCombinedErrors()
        {
            var result = CreateCompositionResult((IEnumerable<CompositionError>)null);
            var expectations = Expectations.GetEnumValues<CompositionErrorId>();

            foreach (var e in expectations)
            {
                var error = ErrorFactory.Create(e);

                var mergedResult = result.MergeError(error);
                var mergedErrors = result.Errors.Concat(new CompositionError[] { error });

                EnumerableAssert.AreSequenceSame(mergedErrors, mergedResult.Errors);
                Assert.IsFalse(mergedResult.Succeeded);                
            }
        }

        [TestMethod]
        public void MergeError_ValueAsErrorArgumentWhenMergedWithResultWithEmptyErrors_ShouldReturnResultWithCombinedErrors()
        {
            var result = CreateCompositionResult(Enumerable.Empty<CompositionError>());
            var expectations = Expectations.GetEnumValues<CompositionErrorId>();

            foreach (var e in expectations)
            {
                var error = ErrorFactory.Create(e);

                var mergedResult = result.MergeError(error);
                var mergedErrors = result.Errors.Concat(new CompositionError[] { error });

                EnumerableAssert.AreSequenceSame(mergedErrors, mergedResult.Errors);
                Assert.IsFalse(mergedResult.Succeeded);
            }
        }

        [TestMethod]
        public void MergeError_ValueAsErrorArgumentWhenMergedWithResultWithErrors_ShouldReturnResultWithCombinedErrors()
        {
            var result = CreateCompositionResult(2);
            var expectations = Expectations.GetEnumValues<CompositionErrorId>();

            foreach (var e in expectations)
            {
                var error = ErrorFactory.Create(e);

                var mergedResult = result.MergeError(error);
                var mergedErrors = result.Errors.Concat(new CompositionError[] { error });

                EnumerableAssert.AreSequenceSame(mergedErrors, mergedResult.Errors);
                Assert.IsFalse(mergedResult.Succeeded);
            }
        }

        [TestMethod]
        public void MergeErrors_ValueAsErrorArgumentWhenMergedWithResultWithNullErrors_ShouldReturnResultWithCombinedErrors()
        {
            var result = CreateCompositionResult((IEnumerable<CompositionError>)null);
            var expectations = Expectations.GetCompositionErrors();

            foreach (var e in expectations)
            {
                var mergedResult = result.MergeErrors(e);
                var mergedErrors = result.Errors.Concat(e);

                EnumerableAssert.AreSequenceSame(mergedErrors, mergedResult.Errors);
                Assert.AreEqual(!e.Any(), mergedResult.Succeeded);
            }
        }

        [TestMethod]
        public void MergeErrors_ValueAsErrorArgumentWhenMergedWithResultWithEmptyErrors_ShouldReturnResultWithCombinedErrors()
        {
            var result = CreateCompositionResult(Enumerable.Empty<CompositionError>());
            var expectations = Expectations.GetCompositionErrors();

            foreach (var e in expectations)
            {
                var mergedResult = result.MergeErrors(e);
                var mergedErrors = result.Errors.Concat(e);

                EnumerableAssert.AreSequenceSame(mergedErrors, mergedResult.Errors);
                Assert.AreEqual(!e.Any(), mergedResult.Succeeded);
            }
        }

        [TestMethod]
        public void MergeErrors_ValueAsErrorArgumentWhenMergedWithResultWithErrors_ShouldReturnResultWithCombinedErrors()
        {
            var result = CreateCompositionResult(2);
            var expectations = Expectations.GetCompositionErrors();

            foreach (var e in expectations)
            {
                var mergedResult = result.MergeErrors(e);
                var mergedErrors = result.Errors.Concat(e);

                EnumerableAssert.AreSequenceSame(mergedErrors, mergedResult.Errors);
                Assert.IsFalse(mergedResult.Succeeded);
            }
        }

        [TestMethod]
        public void ThrowOnErrors_NullAsErrorsArgument_ShouldNotThrow()
        {
            var result = CreateCompositionResult((IEnumerable<CompositionError>)null);

            result.ThrowOnErrors();
        }

        [TestMethod]
        public void ThrowOnErrors_EmptyEnumerableAsErrorsArgument_ShouldNotThrow()
        {
            var result = CreateCompositionResult(Enumerable.Empty<CompositionError>());

            result.ThrowOnErrors();
        }

        [TestMethod]
        public void ThrowOnErrors_SingleValueAsErrorsArgument_ShouldThrowComposition()
        {
            var errorIds = Expectations.GetEnumValues<CompositionErrorId>();

            foreach (var errorId in errorIds)
            {
                var result = CreateCompositionResult(errorId);

                CompositionAssert.ThrowsError((ErrorId)errorId, () =>
                {
                    result.ThrowOnErrors();
                });
            }
        }

        [TestMethod]
        public void ThrowOnErrors_TwoSameValuesAsErrorsArgument_ShouldThrowComposition()
        {
            var errorIds = Expectations.GetEnumValues<CompositionErrorId>();

            foreach (var errorId in errorIds)
            {
                var result = CreateCompositionResult(errorId, errorId);

                CompositionAssert.ThrowsErrors((ErrorId)errorId, (ErrorId)errorId, () =>
                {
                    result.ThrowOnErrors();
                });
            }
        }

        [TestMethod]
        public void ThrowOnErrors_TwoDifferentValuesAsErrorsArgument_ShouldThrowComposition()
        {
            var errorIds1 = Expectations.GetEnumValues<CompositionErrorId>();
            var errorIds2 = Expectations.GetEnumValues<CompositionErrorId>();

            for (int i = 0; i < errorIds1.Count(); i++)
            {
                var errorId1 = errorIds1.ElementAt(i);
                var errorId2 = errorIds1.ElementAt(errorIds2.Count() - 1 - i);

                var result = CreateCompositionResult(errorId1, errorId2);

                CompositionAssert.ThrowsErrors((ErrorId)errorId1, (ErrorId)errorId2, () =>
                {
                    result.ThrowOnErrors();
                });
            }
        }

        [TestMethod]
        public void ToResultOfT_NullAsErrorsArgument_ShouldReturnResultWithEmptyErrorsProperty()
        {
            var result = CreateCompositionResult((IEnumerable<CompositionError>)null);

            var copy = result.ToResult<string>("Value");

            EnumerableAssert.IsEmpty(copy.Errors);
        }

        [TestMethod]
        public void ToResultOfT_EmptyEnumerableAsErrorsArgument_ShouldReturnResultWithEmptyErrorsProperty()
        {
            var result = CreateCompositionResult(Enumerable.Empty<CompositionError>());

            var copy = result.ToResult<string>("Value");

            EnumerableAssert.IsEmpty(copy.Errors);
        }

        [TestMethod]
        public void ToResultOfT_ValueAsErrorsArgument_ShouldReturnResultWithErrorsPropertySet()
        {
            var expectations = Expectations.GetCompositionErrors();

            foreach (var e in expectations)
            {
                var result = CreateCompositionResult(e);

                var copy = result.ToResult<string>("Value");

                EnumerableAssert.AreSequenceSame(result.Errors, copy.Errors);
            }
        }

        [TestMethod]
        public void ToResultOfT_ReferenceValueAsValueArgument_ShouldReturnResultWithValuePropertySet()
        {
            var expectations = Expectations.GetObjectsReferenceTypes();

            foreach (var e in expectations)
            {
                var result = CreateCompositionResult();

                var copy = result.ToResult<object>(e);

                Assert.AreSame(e, copy.Value);
            }
        }

        [TestMethod]
        public void ToResultOfT_ValueTypeValueAsValueArgument_ShouldReturnResultWithValuePropertySet()
        {
            var expectations = Expectations.GetObjectsValueTypes();

            foreach (var e in expectations)
            {
                var result = CreateCompositionResult();

                var copy = result.ToResult<object>(e);

                Assert.AreEqual(e, copy.Value);
            }
        }

        [TestMethod]
        public void SucceededResult_ShouldSetSuccessPropertyToTrue()
        {
            var succeeded = CompositionResult.SucceededResult.Succeeded;

            Assert.IsTrue(succeeded);
        }

        [TestMethod]
        public void SucceededResult_ShouldSetErrorsPropertyToEmptyEnumerable()
        {
            var errors = CompositionResult.SucceededResult.Errors;

            EnumerableAssert.IsEmpty(errors);
        }

        private CompositionResult CreateCompositionResult(params CompositionErrorId[] errorIds)
        {
            return new CompositionResult(errorIds.Select(id =>
            {
                return ErrorFactory.Create(id);
            }));
        }

        private CompositionResult CreateCompositionResult(int count)
        {
            var expectations = Expectations.GetEnumValues<CompositionErrorId>();

            List<CompositionError> errors = new List<CompositionError>();

            foreach (var e in expectations.Take(count))
            {
                errors.Add(ErrorFactory.Create(e));
            }

            return CreateCompositionResult(errors);
        }

        private CompositionResult CreateCompositionResult(IEnumerable<CompositionError> errors)
        {
            return new CompositionResult(errors);
        }
    }
}
