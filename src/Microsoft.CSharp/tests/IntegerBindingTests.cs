// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Xunit;

namespace Microsoft.CSharp.RuntimeBinder.Tests
{
    public class IntegerBindingTests
    {
        private static readonly IEnumerable<ExpressionType> DivisionExpressionTypes = new[]
        {
            ExpressionType.Divide, ExpressionType.DivideAssign, ExpressionType.Modulo, ExpressionType.ModuloAssign
        };

        private static IEnumerable<object[]> DivisionOperations()
        {
            foreach (ExpressionType type in DivisionExpressionTypes)
            {
                yield return new object[] {type, true};
                yield return new object[] {type, false};
            }
        }

        private static CallSite<Func<CallSite, T, T, object>> GetZeroConstantDivisionCallSite<T>(ExpressionType operation, bool constantDividend)
        {
            CSharpArgumentInfo x = CSharpArgumentInfo.Create(constantDividend ? CSharpArgumentInfoFlags.Constant : CSharpArgumentInfoFlags.None, null);
            CSharpArgumentInfo y = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null);
            CallSiteBinder binder = Binder.BinaryOperation(CSharpBinderFlags.None, operation, typeof(IntegerBindingTests), new[] { x, y });
            return CallSite<Func<CallSite, T, T, object>>.Create(binder);
        }

        [Theory, MemberData(nameof(DivisionOperations))]
        public void DivConstantZeroSByte(ExpressionType operation, bool constantDividend)
        {
            CallSite<Func<CallSite, sbyte, sbyte, object>> callsite = GetZeroConstantDivisionCallSite<sbyte>(operation, constantDividend);
            Assert.Throws<RuntimeBinderException>(() => callsite.Target(callsite, 8, 0));
        }

        [Theory, MemberData(nameof(DivisionOperations))]
        public void DivConstantZeroByte(ExpressionType operation, bool constantDividend)
        {
            CallSite<Func<CallSite, byte, byte, object>> callsite = GetZeroConstantDivisionCallSite<byte>(operation, constantDividend);
            Assert.Throws<RuntimeBinderException>(() => callsite.Target(callsite, 8, 0));
        }

        [Theory, MemberData(nameof(DivisionOperations))]
        public void DivConstantZeroInt16(ExpressionType operation, bool constantDividend)
        {
            CallSite<Func<CallSite, short, short, object>> callsite = GetZeroConstantDivisionCallSite<short>(operation, constantDividend);
            Assert.Throws<RuntimeBinderException>(() => callsite.Target(callsite, 8, 0));
        }

        [Theory, MemberData(nameof(DivisionOperations))]
        public void DivConstantZeroUInt16(ExpressionType operation, bool constantDividend)
        {
            CallSite<Func<CallSite, ushort, ushort, object>> callsite = GetZeroConstantDivisionCallSite<ushort>(operation, constantDividend);
            Assert.Throws<RuntimeBinderException>(() => callsite.Target(callsite, 8, 0));
        }

        [Theory, MemberData(nameof(DivisionOperations))]
        public void DivConstantZeroChar(ExpressionType operation, bool constantDividend)
        {
            CallSite<Func<CallSite, char, char, object>> callsite = GetZeroConstantDivisionCallSite<char>(operation, constantDividend);
            Assert.Throws<RuntimeBinderException>(() => callsite.Target(callsite, '\b', '\0'));
        }

        [Theory, MemberData(nameof(DivisionOperations))]
        public void DivConstantZeroInt32(ExpressionType operation, bool constantDividend)
        {
            CallSite<Func<CallSite, int, int, object>> callsite = GetZeroConstantDivisionCallSite<int>(operation, constantDividend);
            Assert.Throws<RuntimeBinderException>(() => callsite.Target(callsite, 8, 0));
        }

        [Theory, MemberData(nameof(DivisionOperations))]
        public void DivConstantZeroUInt32(ExpressionType operation, bool constantDividend)
        {
            CallSite<Func<CallSite, uint, uint, object>> callsite = GetZeroConstantDivisionCallSite<uint>(operation, constantDividend);
            Assert.Throws<RuntimeBinderException>(() => callsite.Target(callsite, 8, 0));
        }

        [Theory, MemberData(nameof(DivisionOperations))]
        public void DivConstantZeroInt64(ExpressionType operation, bool constantDividend)
        {
            CallSite<Func<CallSite, long, long, object>> callsite = GetZeroConstantDivisionCallSite<long>(operation, constantDividend);
            Assert.Throws<RuntimeBinderException>(() => callsite.Target(callsite, 8, 0));
        }

        [Theory, MemberData(nameof(DivisionOperations))]
        public void DivConstantZeroUInt64(ExpressionType operation, bool constantDividend)
        {
            CallSite<Func<CallSite, ulong, ulong, object>> callsite = GetZeroConstantDivisionCallSite<ulong>(operation, constantDividend);
            Assert.Throws<RuntimeBinderException>(() => callsite.Target(callsite, 8, 0));
        }
    }
}
