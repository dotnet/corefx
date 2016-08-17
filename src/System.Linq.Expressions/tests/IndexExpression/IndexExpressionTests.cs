using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace System.Linq.Expressions.Tests.IndexExpression
{
    public static class IndexExpressionTests
    {
        [Fact]
        public static void UpdateSameTest()
        {
            var instance = new SampleClassWithProperties { DefaultProperty = new List<int> { 100, 101 } };
            Expressions.IndexExpression expr = instance.DefaultIndexExpression;

            var exprUpdated = expr.Update(expr.Object, expr.Arguments);

            // Has to be the same, because everything is the same.
            Assert.Same(expr, exprUpdated);

            // Invoke to check expression.
            IndexExpressionHelpers.AssertInvokeCorrect(100, expr, instance);
            IndexExpressionHelpers.AssertInvokeCorrect(100, exprUpdated, instance);
        }

        [Fact]
        public static void UpdateTest()
        {
            var instance = new SampleClassWithProperties
            {
                DefaultProperty = new List<int> { 100, 101 },
                AlternativeProperty = new List<int> { 200, 201 }
            };

            var expr = instance.DefaultIndexExpression;
            var newProperty = Expression.Property(Expression.Constant(instance),
                typeof(SampleClassWithProperties).GetProperty(instance.AlternativePropertyName));
            ConstantExpression[] newArguments = {Expression.Constant(1)};

            var exprUpdated = expr.Update(newProperty, newArguments);

            // Replace Object and Arguments of IndexExpression.
            IndexExpressionHelpers.AssertEqual(
                exprUpdated,
                Expression.MakeIndex(newProperty, instance.DefaultIndexer, newArguments));

            // Invoke to check expression.
            IndexExpressionHelpers.AssertInvokeCorrect(100, expr, instance);
            IndexExpressionHelpers.AssertInvokeCorrect(201, exprUpdated, instance);
        }
    }
}
