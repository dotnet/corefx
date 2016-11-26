// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Xunit;

namespace System.Linq.Expressions.Tests
{
    public static class ArrayAccessTests
    {
        [Theory]
        [ClassData(typeof(CompilationTypes))]
        public static void ArrayAccess_MultiDimensionalOf1(bool useInterpreter)
        {
            Type arrayType = typeof(int).MakeArrayType(1);
            ConstructorInfo arrayCtor = arrayType.GetTypeInfo().DeclaredConstructors.Single(ctor => ctor.GetParameters().Length == 2);
            var arr = (System.Array)arrayCtor.Invoke(new object[] { 1, 1 });
            ConstantExpression c = Expression.Constant(arr);
            IndexExpression e = Expression.ArrayAccess(c, Expression.Constant(1));

            Action set = Expression.Lambda<Action>(Expression.Assign(e, Expression.Constant(42))).Compile(useInterpreter);
            set();
            Assert.Equal(42, arr.GetValue(1));

            Func<int> get = Expression.Lambda<Func<int>>(e).Compile(useInterpreter);
            Assert.Equal(42, get());
        }

        [Theory]
        [ClassData(typeof(CompilationTypes))]
        public static void ArrayIndex_MultiDimensionalOf1(bool useInterpreter)
        {
            Type arrayType = typeof(int).MakeArrayType(1);
            ConstructorInfo arrayCtor = arrayType.GetTypeInfo().DeclaredConstructors.Single(ctor => ctor.GetParameters().Length == 2);
            var arr = (System.Array)arrayCtor.Invoke(new object[] { 1, 1 });
            arr.SetValue(42, 1);
            ConstantExpression c = Expression.Constant(arr);
            BinaryExpression e = Expression.ArrayIndex(c, Expression.Constant(1));

            Func<int> get = Expression.Lambda<Func<int>>(e).Compile(useInterpreter);
            Assert.Equal(42, get());
        }
    }
}
