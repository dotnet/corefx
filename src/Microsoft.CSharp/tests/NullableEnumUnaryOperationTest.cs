using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Xunit;

namespace Microsoft.CSharp.RuntimeBinder.Tests
{
    public class NullableEnumUnaryOperationTest
    {
        public static IEnumerable<object[]> Int32Enums() => Enum.GetValues(typeof(StringComparison))
            .Cast<StringComparison>()
            .Select(x => new object[] {x});

        public static IEnumerable<object[]> Int32NullableEnums() => Int32Enums().Append(new object[] {null});

        [Theory, MemberData(nameof(Int32NullableEnums))]
        public void ComplementInt32NullableEnum(StringComparison? value)
        {
            CSharpArgumentInfo x = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null);
            CallSiteBinder binder =
                Binder.UnaryOperation(
                    CSharpBinderFlags.None, ExpressionType.OnesComplement,
                    GetType(), new[] { x });
            CallSite<Func<CallSite, StringComparison?, object>> site = CallSite<Func<CallSite, StringComparison?, object>>.Create(binder);
            Func<CallSite, StringComparison?, object> targ = site.Target;
            object result = targ(site, value);
            Assert.Equal(~value, result);
        }

        [Theory, MemberData(nameof(Int32Enums))]
        public void IncrementInt32NullableEnum(StringComparison? value)
        {
            CSharpArgumentInfo x = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null);
            CallSiteBinder binder =
                Binder.UnaryOperation(
                    CSharpBinderFlags.None, ExpressionType.Increment,
                    GetType(), new[] { x });
            CallSite<Func<CallSite, StringComparison?, object>> site = CallSite<Func<CallSite, StringComparison?, object>>.Create(binder);
            Func<CallSite, StringComparison?, object> targ = site.Target;
            object result = targ(site, value);
            Assert.Equal(++value, result); // Note that there is no writeback to value.
        }

        [Theory, MemberData(nameof(Int32Enums))]
        public void DecrementInt32NullableEnum(StringComparison? value)
        {
            CSharpArgumentInfo x = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null);
            CallSiteBinder binder =
                Binder.UnaryOperation(
                    CSharpBinderFlags.None, ExpressionType.Decrement,
                    GetType(), new[] { x });
            CallSite<Func<CallSite, StringComparison?, object>> site = CallSite<Func<CallSite, StringComparison?, object>>.Create(binder);
            Func<CallSite, StringComparison?, object> targ = site.Target;
            object result = targ(site, value);
            Assert.Equal(--value, result); // Note that there is no writeback to value.
        }

        [Fact]
        public void CannotIncrementNullNullableEnum()
        {
            CSharpArgumentInfo x = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null);
            CallSiteBinder binder =
                Binder.UnaryOperation(
                    CSharpBinderFlags.None, ExpressionType.Increment,
                    GetType(), new[] { x });
            CallSite<Func<CallSite, StringComparison?, object>> site = CallSite<Func<CallSite, StringComparison?, object>>.Create(binder);
            Func<CallSite, StringComparison?, object> targ = site.Target;
            Assert.Throws<RuntimeBinderException>(() => targ(site, default));
        }

        [Fact]
        public void CannotDecrementNullNullableEnum()
        {
            CSharpArgumentInfo x = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null);
            CallSiteBinder binder =
                Binder.UnaryOperation(
                    CSharpBinderFlags.None, ExpressionType.Decrement,
                    GetType(), new[] { x });
            CallSite<Func<CallSite, StringComparison?, object>> site = CallSite<Func<CallSite, StringComparison?, object>>.Create(binder);
            Func<CallSite, StringComparison?, object> targ = site.Target;
            Assert.Throws<RuntimeBinderException>(() => targ(site, default));
        }
    }
}
