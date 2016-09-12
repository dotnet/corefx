﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace System.Linq.Expressions.Tests
{
    public class MemberBindTests
    {
        private class PropertyAndFields
        {
#pragma warning disable 649 // Assigned through expressions.
            public string StringProperty { get; set; }
            public string StringField;
            public readonly string ReadonlyStringField;
            public string ReadonlyStringProperty { get { return ""; } }
            public static string StaticStringProperty { get; set; }
            public static string StaticStringField;
            public const string ConstantString = "Constant";
#pragma warning restore 649
        }

        public class Inner
        {
            public int Value { get; set; }
        }

        public class Outer
        {
            public Inner InnerProperty { get; set; } = new Inner();
            public Inner InnerField = new Inner();
            public Inner ReadonlyInnerProperty { get; } = new Inner();
            public Inner WriteonlyInnerProperty { set { } }
            public readonly Inner ReadonlyInnerField = new Inner();
            public static Inner StaticInnerProperty { get; set; } = new Inner();
            public static Inner StaticInnerField = new Inner();
            public static Inner StaticReadonlyInnerProperty { get; } = new Inner();
            public static readonly Inner StaticReadonlyInnerField = new Inner();
            public static Inner StaticWriteonlyInnerProperty { set { } }
        }

        [Fact]
        public void NullMethodOrMemberInfo()
        {
            Assert.Throws<ArgumentNullException>("member", () => Expression.MemberBind(default(MemberInfo)));
            Assert.Throws<ArgumentNullException>("member", () => Expression.MemberBind(default(MemberInfo), Enumerable.Empty<MemberBinding>()));
            Assert.Throws<ArgumentNullException>("propertyAccessor", () => Expression.MemberBind(default(MethodInfo)));
            Assert.Throws<ArgumentNullException>("propertyAccessor", () => Expression.MemberBind(default(MethodInfo), Enumerable.Empty<MemberBinding>()));
        }

        [Fact]
        public void NullBindings()
        {
            var mem = typeof(PropertyAndFields).GetProperty(nameof(PropertyAndFields.StringProperty));
            var meth = mem.GetGetMethod();
            Assert.Throws<ArgumentNullException>("bindings", () => Expression.MemberBind(mem, default(MemberBinding[])));
            Assert.Throws<ArgumentNullException>("bindings", () => Expression.MemberBind(mem, default(IEnumerable<MemberBinding>)));
            Assert.Throws<ArgumentNullException>("bindings", () => Expression.MemberBind(meth, default(MemberBinding[])));
            Assert.Throws<ArgumentNullException>("bindings", () => Expression.MemberBind(meth, default(IEnumerable<MemberBinding>)));
        }

        [Fact]
        public void NullBindingInBindings()
        {
            var mem = typeof(PropertyAndFields).GetProperty(nameof(PropertyAndFields.StringProperty));
            var meth = mem.GetGetMethod();
            Assert.Throws<ArgumentNullException>("bindings", () => Expression.MemberBind(mem, default(MemberBinding)));
            Assert.Throws<ArgumentNullException>("bindings", () => Expression.MemberBind(mem, Enumerable.Repeat<MemberBinding>(null, 1)));
            Assert.Throws<ArgumentNullException>("bindings", () => Expression.MemberBind(meth, default(MemberBinding)));
            Assert.Throws<ArgumentNullException>("bindings", () => Expression.MemberBind(meth, Enumerable.Repeat<MemberBinding>(null, 1)));
        }

        [Fact]
        public void BindMethodMustBeProperty()
        {
            MemberInfo toString = typeof(object).GetMember(nameof(ToString))[0];
            MethodInfo toStringMeth = typeof(object).GetMethod(nameof(ToString));
            Assert.Throws<ArgumentException>("member", () => Expression.MemberBind(toString));
            Assert.Throws<ArgumentException>("member", () => Expression.MemberBind(toString, Enumerable.Empty<MemberBinding>()));
            Assert.Throws<ArgumentException>("propertyAccessor", () => Expression.MemberBind(toStringMeth));
            Assert.Throws<ArgumentException>("propertyAccessor", () => Expression.MemberBind(toStringMeth, Enumerable.Empty<MemberBinding>()));
        }

        [Fact]
        public void MemberBindingMustBeMemberOfType()
        {
            var bind = Expression.MemberBind(
                typeof(Outer).GetProperty(nameof(Outer.InnerProperty)),
                Expression.Bind(typeof(Inner).GetProperty(nameof(Inner.Value)), Expression.Constant(3))
                );
            var newExp = Expression.New(typeof(PropertyAndFields));
            Assert.Throws<ArgumentException>(() => Expression.MemberInit(newExp, bind));
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public void InnerProperty(bool useInterpreter)
        {
            var exp = Expression.Lambda<Func<Outer>>(
                Expression.MemberInit(
                    Expression.New(typeof(Outer)),
                    Expression.MemberBind(
                        typeof(Outer).GetProperty(nameof(Outer.InnerProperty)),
                        Expression.Bind(typeof(Inner).GetProperty(nameof(Inner.Value)), Expression.Constant(3))
                        )
                    )
                );
            var func = exp.Compile(useInterpreter);
            Assert.Equal(3, func().InnerProperty.Value);
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public void InnerField(bool useInterpreter)
        {
            var exp = Expression.Lambda<Func<Outer>>(
                Expression.MemberInit(
                    Expression.New(typeof(Outer)),
                    Expression.MemberBind(
                        typeof(Outer).GetField(nameof(Outer.InnerField)),
                        Expression.Bind(typeof(Inner).GetProperty(nameof(Inner.Value)), Expression.Constant(4))
                        )
                    )
                );
            var func = exp.Compile(useInterpreter);
            Assert.Equal(4, func().InnerField.Value);
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public void StaticInnerProperty(bool useInterpreter)
        {
            var exp = Expression.Lambda<Func<Outer>>(
                Expression.MemberInit(
                    Expression.New(typeof(Outer)),
                    Expression.MemberBind(
                        typeof(Outer).GetProperty(nameof(Outer.StaticInnerProperty)),
                        Expression.Bind(typeof(Inner).GetProperty(nameof(Inner.Value)), Expression.Constant(5))
                        )
                    )
                );
            Assert.Throws<InvalidProgramException>(() => exp.Compile(useInterpreter));
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public void StaticInnerField(bool useInterpreter)
        {
            var exp = Expression.Lambda<Func<Outer>>(
                Expression.MemberInit(
                    Expression.New(typeof(Outer)),
                    Expression.MemberBind(
                        typeof(Outer).GetField(nameof(Outer.StaticInnerField)),
                        Expression.Bind(typeof(Inner).GetProperty(nameof(Inner.Value)), Expression.Constant(6))
                        )
                    )
                );
            Assert.Throws<InvalidProgramException>(() => exp.Compile(useInterpreter));
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public void ReadonlyInnerProperty(bool useInterpreter)
        {
            var exp = Expression.Lambda<Func<Outer>>(
                Expression.MemberInit(
                    Expression.New(typeof(Outer)),
                    Expression.MemberBind(
                        typeof(Outer).GetProperty(nameof(Outer.ReadonlyInnerProperty)),
                        Expression.Bind(typeof(Inner).GetProperty(nameof(Inner.Value)), Expression.Constant(7))
                        )
                    )
                );
            var func = exp.Compile(useInterpreter);
            Assert.Equal(7, func().ReadonlyInnerProperty.Value);
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public void ReadonlyInnerField(bool useInterpreter)
        {
            var exp = Expression.Lambda<Func<Outer>>(
                Expression.MemberInit(
                    Expression.New(typeof(Outer)),
                    Expression.MemberBind(
                        typeof(Outer).GetField(nameof(Outer.ReadonlyInnerField)),
                        Expression.Bind(typeof(Inner).GetProperty(nameof(Inner.Value)), Expression.Constant(8))
                        )
                    )
                );
            var func = exp.Compile(useInterpreter);
            Assert.Equal(8, func().ReadonlyInnerField.Value);
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public void StaticReadonlyInnerProperty(bool useInterpreter)
        {
            var exp = Expression.Lambda<Func<Outer>>(
                Expression.MemberInit(
                    Expression.New(typeof(Outer)),
                    Expression.MemberBind(
                        typeof(Outer).GetProperty(nameof(Outer.StaticReadonlyInnerProperty)),
                        Expression.Bind(typeof(Inner).GetProperty(nameof(Inner.Value)), Expression.Constant(5))
                        )
                    )
                );
            Assert.Throws<InvalidProgramException>(() => exp.Compile(useInterpreter));
        }

        [Theory, ClassData(typeof(CompilationTypes))]
        public void StaticReadonlyInnerField(bool useInterpreter)
        {
            var exp = Expression.Lambda<Func<Outer>>(
                Expression.MemberInit(
                    Expression.New(typeof(Outer)),
                    Expression.MemberBind(
                        typeof(Outer).GetField(nameof(Outer.StaticReadonlyInnerField)),
                        Expression.Bind(typeof(Inner).GetProperty(nameof(Inner.Value)), Expression.Constant(6))
                        )
                    )
                );
            Assert.Throws<InvalidProgramException>(() => exp.Compile(useInterpreter));
        }

        public void WriteOnlyInnerProperty()
        {
            var bind = Expression.Bind(typeof(Inner).GetProperty(nameof(Inner.Value)), Expression.Constant(0));
            var property = typeof(Outer).GetProperty(nameof(Outer.WriteonlyInnerProperty));
            Assert.Throws<ArgumentException>(() => Expression.MemberBind(property, bind));
        }

        public void StaticWriteOnlyInnerProperty()
        {
            var bind = Expression.Bind(typeof(Inner).GetProperty(nameof(Inner.Value)), Expression.Constant(0));
            var property = typeof(Outer).GetProperty(nameof(Outer.StaticWriteonlyInnerProperty));
            Assert.Throws<ArgumentException>(() => Expression.MemberBind(property, bind));
        }
    }
}
