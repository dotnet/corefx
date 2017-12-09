// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using Xunit;

namespace Microsoft.CSharp.RuntimeBinder.Tests
{
    public class NamedArgumentTests
    {
        public class TypeWithMethods
        {
            public int DoStuff(int x, int y) => x + y;

            public long DoStuff(long z, long a) => z * a;

            public int DoStuff(string s, int i) => i;
        }

        [Fact]
        public void OnlyNameFirstArgument()
        {
            CallSite<Func<CallSite, object, object, object, object>> callsite =
                CallSite<Func<CallSite, object, object, object, object>>.Create(
                    Binder.InvokeMember(
                        CSharpBinderFlags.None, nameof(TypeWithMethods.DoStuff), Type.EmptyTypes, GetType(),
                        new[]
                        {
                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, ""),
                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "x"),
                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, "")
                        }));
            Func<CallSite, object, object, object, object> target = callsite.Target;
            object res = target(callsite, new TypeWithMethods(), 9, 14);
            Assert.Equal(23, res);
        }

        [Fact]
        public void OnlyNameFirstArgumentMatchesWrongType()
        {
            CallSite<Func<CallSite, object, object, object, object>> callsite =
                CallSite<Func<CallSite, object, object, object, object>>.Create(
                    Binder.InvokeMember(
                        CSharpBinderFlags.None, nameof(TypeWithMethods.DoStuff), Type.EmptyTypes, GetType(),
                        new[]
                        {
                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, ""),
                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "s"),
                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, "")
                        }));
            Func<CallSite, object, object, object, object> target = callsite.Target;
            string message = Assert.Throws<RuntimeBinderException>(() => target(callsite, new TypeWithMethods(), 9, 14))
                .Message;

            // All but the (string, int) method should have been excluded from consideration, leaving the binder to
            // complain about it not matching types.
            Assert.Contains(
                "'Microsoft.CSharp.RuntimeBinder.Tests.NamedArgumentTests.TypeWithMethods.DoStuff(string, int)'",
                message);
        }

        [Fact]
        public void ResolveThroughNameOnlyFirstArgument()
        {
            CallSite<Func<CallSite, object, object, object, object>> callsite =
                CallSite<Func<CallSite, object, object, object, object>>.Create(
                    Binder.InvokeMember(
                        CSharpBinderFlags.None, nameof(TypeWithMethods.DoStuff), Type.EmptyTypes, GetType(),
                        new[]
                        {
                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, ""),
                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "z"),
                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, "")
                        }));
            Func<CallSite, object, object, object, object> target = callsite.Target;
            object res = target(callsite, new TypeWithMethods(), 9, 14);
            Assert.Equal(126L, res);
        }

        [Fact]
        public void NonExistentNameOnlyFirstArgument()
        {
            CallSite<Func<CallSite, object, object, object, object>> callsite =
                CallSite<Func<CallSite, object, object, object, object>>.Create(
                    Binder.InvokeMember(
                        CSharpBinderFlags.None, nameof(TypeWithMethods.DoStuff), Type.EmptyTypes, GetType(),
                        new[]
                        {
                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, ""),
                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "nada"),
                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, "")
                        }));
            Func<CallSite, object, object, object, object> target = callsite.Target;
            string message = Assert.Throws<RuntimeBinderException>(() => target(callsite, new TypeWithMethods(), 9, 14)).Message;
            //  The best overload for 'DoStuff' does not have a parameter named 'nada'
            Assert.Contains("'DoStuff'", message);
            Assert.Contains("'nada'", message);
        }
    }
}
