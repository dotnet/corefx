// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if FEATURE_INTERPRET
using System;
using System.Reflection;
using Xunit;

namespace System.Linq.Expressions.Tests
{
    public static partial class InterpreterTests
    {
        #region Test methods

        [Fact] // [Issue(4174, "https://github.com/dotnet/corefx/issues/4174")]
        public static void InterpretCompileCrossChecks_Address_NonWritableField()
        {
            var mtd = typeof(WriteBack).GetMethod("Test");

            foreach (var m in new Expression[]
            {
                Expression.Field(null, typeof(Holder).GetField("FSR")),
                Expression.Field(null, typeof(Holder).GetField("FC")),
                Expression.Field(Expression.Constant(new Holder()), typeof(Holder).GetField("FR")),
            })
            {
                Verify(Expression.Call(mtd, m));
            }
        }

        [Fact] // [Issue(4174, "https://github.com/dotnet/corefx/issues/4174")]
        public static void InterpretCompileCrossChecks_Address_ThrowingPropertySetter()
        {
            var mtd = typeof(WriteBack).GetMethod("Test");

            foreach (var m in new Expression[]
            {
                Expression.Property(Expression.Constant(new Holder()), typeof(Holder).GetProperty("P")),
                Expression.MakeIndex(Expression.Constant(new Holder()), typeof(Holder).GetProperty("Item"), new[] { Expression.Constant(0) }),
            })
            {
                Verify(Expression.Call(mtd, m));
            }
        }

        class WriteBack
        {
            public static void Test(ref int value)
            {
                value = -1;
            }
        }

        class Holder
        {
            public static readonly int FSR = 41;
            public readonly int FR = 42;
            public const int FC = 43;

            public int P
            {
                get { return 42; }
                set { throw new Exception("Oops!"); }
            }

            public int this[int index]
            {
                get { return 42; }
                set { throw new Exception("Oops!"); }
            }
        }

        #endregion
    }
}
#endif
