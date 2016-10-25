// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;

using Xunit;

namespace System.Net.Primitives.Functional.Tests
{
    public static class TransportContextTest
    {
        private class MyChannelBinding : ChannelBinding
        {
            public const int BindingSize = 17;  // arbitrary for testing

            public ChannelBindingKind _kind;

            public MyChannelBinding(ChannelBindingKind kind)
            {
                _kind = kind;
            }

            public override int Size
            {
                get { return BindingSize; }
            }

            protected override bool ReleaseHandle()
            {
                throw new NotImplementedException();
            }
        }

        private class MyTransportContext : TransportContext
        {
            private TokenBinding[] _tokenBindings;

            public MyTransportContext(params TokenBinding[] tokenBindings)
            {
                _tokenBindings = tokenBindings;
            }

            public override ChannelBinding GetChannelBinding(ChannelBindingKind kind)
            {
                return new MyChannelBinding(kind);
            }

            public override IEnumerable<TokenBinding> GetTlsTokenBindings()
            {
                return _tokenBindings;
            }
        }

        [Theory]
        [InlineData(ChannelBindingKind.Endpoint)]
        [InlineData(ChannelBindingKind.Unique)]
        [InlineData(ChannelBindingKind.Unknown)]
        public static void TransportContext_GetChannelBinding_Success(ChannelBindingKind kind)
        {
            var tc = new MyTransportContext();

            var binding = tc.GetChannelBinding(kind);

            Assert.IsType<MyChannelBinding>(binding);

            var mybinding = (MyChannelBinding)binding;

            Assert.Equal(mybinding._kind, kind);
            Assert.Equal(mybinding.Size, MyChannelBinding.BindingSize);
        }

        [Fact]
        public static void TransportContext_GetTlsTokenBindings_Success()
        {
            byte[] rawBytes1 = new byte[] { 1, 2, 3 };
            byte[] rawBytes2 = new byte[] { 4, 5, 6 };

            TokenBinding[] bindings = new TokenBinding[]
            {
                new TokenBinding(TokenBindingType.Provided, rawBytes1),
                new TokenBinding(TokenBindingType.Referred, rawBytes2)
            };

            var tc = new MyTransportContext(bindings);

            TokenBinding[] retrievedBindings = tc.GetTlsTokenBindings().ToArray();

            Assert.Equal(bindings.Length, retrievedBindings.Length);
            bindings.Zip(retrievedBindings, (first, second) =>
            {
                Assert.Equal(first.BindingType, second.BindingType);
                Assert.True(first.GetRawTokenBindingId().SequenceEqual(second.GetRawTokenBindingId()));

                return true; // unused
            });
        }
    }
}
