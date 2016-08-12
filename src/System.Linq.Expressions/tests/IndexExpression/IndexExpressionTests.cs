using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace System.Linq.Expressions.Tests.IndexExpression
{
    public static class IndexExpressionTests
    {
        #region region Helpers

        private const string DefaultPropertyName = "DefaultProperty";
        private const string AlternativePropertyName = "AlternativeProperty";

        private static readonly PropertyInfo DefaultIndexer = typeof(List<int>).GetProperty("Item");
        private static readonly ConstantExpression[] DefaultArguments = { Expression.Constant(0) };

        private static MemberExpression GetDefaultPropertyExpression(SampleClassWithProperties instance)
        {
            return Expression.Property(Expression.Constant(instance),
                typeof(SampleClassWithProperties).GetProperty(DefaultPropertyName));
        }

        #endregion

        #region Test Methods

        [Fact]
        public static void UpdateSameTest()
        {
            var instance = new SampleClassWithProperties { DefaultProperty = new List<int> { 100, 101 } };
            MemberExpression propertyExpression = GetDefaultPropertyExpression(instance);

            Expressions.IndexExpression expr = Expression.MakeIndex(propertyExpression, DefaultIndexer, DefaultArguments);
            var exprUpdated = expr.Update(expr.Object, expr.Arguments);

            // Has to be the same, because everything is the same.
            Assert.Same(expr, exprUpdated);

            // Invoke to check expression.
            AssertInvokeCorrect(100, expr, instance);
            AssertInvokeCorrect(100, exprUpdated, instance);
        }

        [Fact]
        public static void UpdateTest()
        {
            var instance = new SampleClassWithProperties
            {
                DefaultProperty = new List<int> { 100, 101 },
                AlternativeProperty = new List<int> { 200, 201 }
            };

            MemberExpression propertyExpression = GetDefaultPropertyExpression(instance);

            var expr = Expression.MakeIndex(propertyExpression, DefaultIndexer, DefaultArguments);
            var constExpression = Expression.Constant(1);
            var newProperty = Expression.Property(Expression.Constant(instance),
                typeof(SampleClassWithProperties).GetProperty("AlternativeProperty"));

            var exprUpdated = expr.Update(newProperty, new[] { constExpression });

            // Replace Object and Arguments of IndexExpression.
            AssertEqual(exprUpdated, Expression.MakeIndex(newProperty, DefaultIndexer, new[] { constExpression }));

            // Invoke to check expression.
            AssertInvokeCorrect(100, expr, instance);
            AssertInvokeCorrect(201, exprUpdated, instance);
        }

        [Fact]
        public static void RewriteTest()
        {
            var instance = new SampleClassWithProperties
            {
                DefaultProperty = new List<int> { 100, 101 },
                AlternativeProperty = new List<int> { 200, 201 }
            };

            MemberExpression propertyExpression = GetDefaultPropertyExpression(instance);

            Expressions.IndexExpression expr = Expression.MakeIndex(propertyExpression, DefaultIndexer, DefaultArguments);
            var newProperty = Expression.Property(Expression.Constant(instance),
                typeof(SampleClassWithProperties).GetProperty(AlternativePropertyName));

            var actual = (Expressions.IndexExpression) expr.Rewrite(newProperty, expr.Arguments.ToArray());
            var expected = Expression.MakeIndex(newProperty, expr.Indexer, expr.Arguments);

            // Object of ExpressionIndex replaced via Rewrite method call.
            AssertEqual(expected, actual);

            // Invoke to check expression.
            AssertInvokeCorrect(100, expr, instance);
            AssertInvokeCorrect(200, actual, instance);
        }

        [Fact]
        public static void RewriteNullArgumentsTest()
        {
            var instance = new SampleClassWithProperties { DefaultProperty = new List<int> { 100, 101 } };
            MemberExpression propertyExpression = GetDefaultPropertyExpression(instance);
            Expressions.IndexExpression expr = Expression.MakeIndex(propertyExpression, DefaultIndexer, DefaultArguments);

            // Null value will be ignored.
            var actual = (Expressions.IndexExpression)expr.Rewrite(expr.Object, null);

            Assert.Equal(DefaultArguments, actual.Arguments);
            Assert.NotNull(actual.Arguments);

            // Invoke to check expression.
            AssertInvokeCorrect(100, expr, instance);
            AssertInvokeCorrect(100, actual, instance);
        }

        #endregion

        #region Test verifiers

        private static void AssertEqual(Expressions.IndexExpression expected, Expressions.IndexExpression actual)
        {
            Assert.Equal(expected.Object, actual.Object);
            Assert.Equal(expected.Indexer, actual.Indexer);
            Assert.Equal(expected.Arguments, actual.Arguments);
        }

        internal static void AssertInvokeCorrect<T>(T expected, Expressions.IndexExpression expr, SampleClassWithProperties parameter)
        {
            var lambda = Expression.Lambda<Func<T>>(expr);
            
            // Compile and evaluate with interpretation flag and without
            // in case there are bugs in the compiler/interpreter. 
            Assert.Equal(expected, lambda.Compile(false).Invoke());
            Assert.Equal(expected, lambda.Compile(true).Invoke());
        }

        #endregion
    }
}
