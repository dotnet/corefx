// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Linq.Expressions.Tests
{
    public class Return : GotoExpressionTests
    {
        [Theory]
        [PerCompilationType(nameof(ConstantValueData))]
        public void JustReturnValue(object value, bool useInterpreter)
        {
            Type type = value.GetType();
            LabelTarget target = Expression.Label(type);
            Expression block = Expression.Block(
                Expression.Return(target, Expression.Constant(value)),
                Expression.Label(target, Expression.Default(type))
                );
            Expression equals = Expression.Equal(Expression.Constant(value), block);
            Assert.True(Expression.Lambda<Func<bool>>(equals).Compile(useInterpreter)());
        }

        [Theory]
        [ClassData(typeof(CompilationTypes))]
        public void ReturnToMiddle(bool useInterpreter)
        {
            // The behaviour is that return jumps to a label, but does not necessarily leave a block.
            LabelTarget target = Expression.Label(typeof(int));
            Expression block = Expression.Block(
                Expression.Return(target, Expression.Constant(1)),
                Expression.Label(target, Expression.Constant(2)),
                Expression.Constant(3)
                );
            Assert.Equal(3, Expression.Lambda<Func<int>>(block).Compile(useInterpreter)());
        }

        [Theory]
        [PerCompilationType(nameof(ConstantValueData))]
        public void ReturnJumps(object value, bool useInterpreter)
        {
            Type type = value.GetType();
            LabelTarget target = Expression.Label(type);
            Expression block = Expression.Block(
                Expression.Return(target, Expression.Constant(value)),
                Expression.Throw(Expression.Constant(new InvalidOperationException())),
                Expression.Label(target, Expression.Default(type))
                );
            Assert.True(Expression.Lambda<Func<bool>>(Expression.Equal(Expression.Constant(value), block)).Compile(useInterpreter)());
        }

        [Theory]
        [MemberData(nameof(TypesData))]
        public void NonVoidTargetReturnHasNoValue(Type type)
        {
            LabelTarget target = Expression.Label(type);
            Assert.Throws<ArgumentException>("target", () => Expression.Return(target));
        }

        [Theory]
        [MemberData(nameof(TypesData))]
        public void NonVoidTargetReturnHasNoValueTypeExplicit(Type type)
        {
            LabelTarget target = Expression.Label(type);
            Assert.Throws<ArgumentException>("target", () => Expression.Return(target, type));
        }

        [Theory]
        [ClassData(typeof(CompilationTypes))]
        public void ReturnVoidNoValue(bool useInterpreter)
        {
            LabelTarget target = Expression.Label();
            Expression block = Expression.Block(
                Expression.Return(target),
                Expression.Throw(Expression.Constant(new InvalidOperationException())),
                Expression.Label(target)
                );
            Expression.Lambda<Action>(block).Compile(useInterpreter)();
        }

        [Theory]
        [ClassData(typeof(CompilationTypes))]
        public void ReturnExplicitVoidNoValue(bool useInterpreter)
        {
            LabelTarget target = Expression.Label();
            Expression block = Expression.Block(
                Expression.Return(target, typeof(void)),
                Expression.Throw(Expression.Constant(new InvalidOperationException())),
                Expression.Label(target)
                );
            Expression.Lambda<Action>(block).Compile(useInterpreter)();
        }

        [Theory]
        [MemberData(nameof(TypesData))]
        public void NullValueOnNonVoidReturn(Type type)
        {
            Assert.Throws<ArgumentException>("target", () => Expression.Return(Expression.Label(type)));
            Assert.Throws<ArgumentException>("target", () => Expression.Return(Expression.Label(type), default(Expression)));
            Assert.Throws<ArgumentException>("target", () => Expression.Return(Expression.Label(type), null, type));
        }

        [Theory]
        [MemberData(nameof(ConstantValueData))]
        public void ExplicitNullTypeWithValue(object value)
        {
            Assert.Throws<ArgumentException>("target", () => Expression.Return(Expression.Label(value.GetType()), default(Type)));
        }

        [Fact]
        public void UnreadableLabel()
        {
            Expression value = Expression.Property(null, typeof(Unreadable<string>), "WriteOnly");
            LabelTarget target = Expression.Label(typeof(string));
            Assert.Throws<ArgumentException>("value", () => Expression.Return(target, value));
            Assert.Throws<ArgumentException>("value", () => Expression.Return(target, value, typeof(string)));
        }

        [Theory]
        [PerCompilationType(nameof(ConstantValueData))]
        public void CanAssignAnythingToVoid(object value, bool useInterpreter)
        {
            LabelTarget target = Expression.Label();
            BlockExpression block = Expression.Block(
                Expression.Return(target, Expression.Constant(value)),
                Expression.Label(target)
                );
            Assert.Equal(typeof(void), block.Type);
            Expression.Lambda<Action>(block).Compile(useInterpreter)();
        }

        [Theory]
        [MemberData(nameof(NonObjectAssignableConstantValueData))]
        public void CannotAssignValueTypesToObject(object value)
        {
            Assert.Throws<ArgumentException>(null, () => Expression.Return(Expression.Label(typeof(object)), Expression.Constant(value)));
        }

        [Theory]
        [PerCompilationType(nameof(ObjectAssignableConstantValueData))]
        public void ExplicitTypeAssigned(object value, bool useInterpreter)
        {
            LabelTarget target = Expression.Label(typeof(object));
            BlockExpression block = Expression.Block(
                Expression.Return(target, Expression.Constant(value), typeof(object)),
                Expression.Label(target, Expression.Default(typeof(object)))
                );
            Assert.Equal(typeof(object), block.Type);
            Assert.Equal(value, Expression.Lambda<Func<object>>(block).Compile(useInterpreter)());
        }

        [Fact]
        public void ReturnQuotesIfNecessary()
        {
            LabelTarget target = Expression.Label(typeof(Expression<Func<int>>));
            BlockExpression block = Expression.Block(
                Expression.Return(target, Expression.Lambda<Func<int>>(Expression.Constant(0))),
                Expression.Label(target, Expression.Default(typeof(Expression<Func<int>>)))
                );
            Assert.Equal(typeof(Expression<Func<int>>), block.Type);
        }

        [Fact]
        public void UpdateSameIsSame()
        {
            LabelTarget target = Expression.Label(typeof(int));
            Expression value = Expression.Constant(0);
            GotoExpression ret = Expression.Return(target, value);
            Assert.Same(ret, ret.Update(target, value));
            Assert.Same(ret, NoOpVisitor.Instance.Visit(ret));
        }

        [Fact]
        public void UpdateDiferentValueIsDifferent()
        {
            LabelTarget target = Expression.Label(typeof(int));
            GotoExpression ret = Expression.Return(target, Expression.Constant(0));
            Assert.NotSame(ret, ret.Update(target, Expression.Constant(0)));
        }

        [Fact]
        public void UpdateDifferentTargetIsDifferent()
        {
            Expression value = Expression.Constant(0);
            GotoExpression ret = Expression.Return(Expression.Label(typeof(int)), value);
            Assert.NotSame(ret, ret.Update(Expression.Label(typeof(int)), value));
        }

        [Fact]
        public void OpenGenericType()
        {
            Assert.Throws<ArgumentException>("type", () => Expression.Return(Expression.Label(typeof(void)), typeof(List<>)));
        }

        [Fact]
        public static void TypeContainsGenericParameters()
        {
            Assert.Throws<ArgumentException>("type", () => Expression.Return(Expression.Label(typeof(void)), typeof(List<>.Enumerator)));
            Assert.Throws<ArgumentException>("type", () => Expression.Return(Expression.Label(typeof(void)), typeof(List<>).MakeGenericType(typeof(List<>))));
        }

        [Fact]
        public void PointerType()
        {
            Assert.Throws<ArgumentException>("type", () => Expression.Return(Expression.Label(typeof(void)), typeof(int).MakePointerType()));
        }

        [Fact]
        public void ByRefType()
        {
            Assert.Throws<ArgumentException>("type", () => Expression.Return(Expression.Label(typeof(void)), typeof(int).MakeByRefType()));
        }
    }
}
