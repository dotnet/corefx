using Xunit;

namespace System.Linq.Expressions.Tests.IndexExpression
{
    public static class IndexExpressionHelpers
    {
        internal static void AssertEqual(Expressions.IndexExpression expected, Expressions.IndexExpression actual)
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
    }
}
