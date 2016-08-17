using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace System.Linq.Expressions.Tests.IndexExpression
{
    public class IndexExpressionVisitorTests
    {
        private class IndexVisitor : ExpressionVisitor
        {
            public IndexVisitor(Expressions.IndexExpression expr, Expression newObject, Expression[] newArguments)
            {
                _dict = new Dictionary<Expression, Expression>
                {
                    {expr.Object, newObject}
                };

                for (int i = 0; i < expr.Arguments.Count; i++)
                {
                    _dict.Add(expr.Arguments[i], newArguments?[i]);
                }
            }

            private readonly Dictionary<Expression, Expression> _dict;

            public override Expression Visit(Expression node)
            {
                return _dict.ContainsKey(node)
                    ? _dict[node]
                    : base.Visit(node);
            }
        }
        
        [Fact]
        public static void RewriteObjectTest()
        {
            var instance = new SampleClassWithProperties
            {
                DefaultProperty = new List<int> { 100, 101 },
                AlternativeProperty = new List<int> { 200, 201 }
            };

            Expressions.IndexExpression expr = instance.DefaultIndexExpression;
            var newProperty = Expression.Property(Expression.Constant(instance),
                typeof(SampleClassWithProperties).GetProperty(instance.AlternativePropertyName));

            var visitor = new IndexVisitor(expr, newProperty, expr.Arguments.ToArray());
            var actual = (Expressions.IndexExpression)visitor.Visit(expr);
            var expected = Expression.MakeIndex(newProperty, expr.Indexer, expr.Arguments);

            // Object of ExpressionIndex replaced via Rewrite method call.
            IndexExpressionHelpers.AssertEqual(expected, actual);

            // Invoke to check expression.
            IndexExpressionHelpers.AssertInvokeCorrect(100, expr, instance);
            IndexExpressionHelpers.AssertInvokeCorrect(200, actual, instance);
        }

        [Fact]
        public static void RewriteArgumentsTest()
        {
            var instance = new SampleClassWithProperties {DefaultProperty = new List<int> {100, 101}};

            Expressions.IndexExpression expr = instance.DefaultIndexExpression;
            Expression[] newArguments = {Expression.Constant(1)};

            var visitor = new IndexVisitor(expr, expr.Object, newArguments);
            var expected = Expression.MakeIndex(expr.Object, expr.Indexer, newArguments);
            var actual = (Expressions.IndexExpression) visitor.Visit(expr);
            
            IndexExpressionHelpers.AssertEqual(expected, actual);

            // Invoke to check expression.
            IndexExpressionHelpers.AssertInvokeCorrect(100, expr, instance);
            IndexExpressionHelpers.AssertInvokeCorrect(101, actual, instance);
        }
    }
}
