using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace System.Linq.Expressions.Tests.IndexExpression
{
    public static class IndexExpressionTests
    {
        #region region Helpers

        private static readonly SampleClassWithProperties DefaultObjectWithProperty = new SampleClassWithProperties
        {
            DefaultProperty = new List<int> {0},
            AlternativeProperty = new List<int> {0}
        };

        private const string DefaultPropertyName = "DefaultProperty";
        private const string AlternativePropertyName = "AlternativeProperty";

        private static readonly PropertyInfo DefaultIndexer = typeof(List<int>).GetProperty("Item");

        private static readonly MemberExpression DefaultProperty =
            Expression.Property(Expression.Constant(DefaultObjectWithProperty),
                typeof(SampleClassWithProperties).GetProperty(DefaultPropertyName));

        private static readonly ConstantExpression[] DefaultArguments = { Expression.Constant(0) };

        #endregion

        #region Test Methods

        [Fact]
        public static void UpdateSameTest()
        {
            Expressions.IndexExpression expr = Expression.MakeIndex(DefaultProperty, DefaultIndexer, DefaultArguments);
            var exprUpdated = expr.Update(expr.Object, expr.Arguments);

            // Has to be the same, because everything is the same.
            Assert.Same(expr, exprUpdated);
        }

        [Fact]
        public static void UpdateTest()
        {
            var expr = Expression.MakeIndex(DefaultProperty, DefaultIndexer, DefaultArguments);
            var constExpression = Expression.Constant(1);
            var newProperty = Expression.Property(Expression.Constant(DefaultObjectWithProperty),
                typeof(SampleClassWithProperties).GetProperty("AlternativeProperty"));

            var exprUpdated = expr.Update(newProperty, new[] { constExpression });

            // Replace Object and Arguments of IndexExpression.
            AssertEqual(exprUpdated, Expression.MakeIndex(newProperty, DefaultIndexer, new[] { constExpression }));
        }

        [Fact]
        public static void RewriteTest()
        {
            Expressions.IndexExpression expr = Expression.MakeIndex(DefaultProperty, DefaultIndexer, DefaultArguments);
            var newProperty = Expression.Property(Expression.Constant(DefaultObjectWithProperty),
                typeof(SampleClassWithProperties).GetProperty(AlternativePropertyName));

            var actual = (Expressions.IndexExpression) expr.Rewrite(newProperty, expr.Arguments.ToArray());
            var expected = Expression.MakeIndex(newProperty, expr.Indexer, expr.Arguments);

            // Object of ExpressionIndex replaced via Rewrite method call.
            AssertEqual(expected, actual);
        }

        [Fact]
        public static void RewriteNullArgumentsTest()
        {
            Expressions.IndexExpression expr = Expression.MakeIndex(DefaultProperty, DefaultIndexer, DefaultArguments);

            // Null value will be ignored.
            var actual = (Expressions.IndexExpression)expr.Rewrite(expr.Object, null);

            Assert.Equal(DefaultArguments, actual.Arguments);
            Assert.NotNull(actual.Arguments);
        }

        #endregion

        #region Test verifiers

        private static void AssertEqual(Expressions.IndexExpression expected, Expressions.IndexExpression actual)
        {
            Assert.Equal(expected.Object, actual.Object);
            Assert.Equal(expected.Indexer, actual.Indexer);
            Assert.Equal(expected.Arguments, actual.Arguments);
        }

        #endregion
    }
}
