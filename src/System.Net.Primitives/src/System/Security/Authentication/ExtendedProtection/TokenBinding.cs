// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Security.Authentication.ExtendedProtection
{
    public sealed class TokenBinding
    {
        private TokenBindingType _bindingType;
        private byte[] _rawTokenBindingId;

        public TokenBinding(TokenBindingType bindingType, byte[] rawTokenBindingId)
        {
            _bindingType = bindingType;
            _rawTokenBindingId = rawTokenBindingId;
        }

        public byte[] GetRawTokenBindingId()
        {
            return (_rawTokenBindingId != null) ? (byte[])_rawTokenBindingId.Clone() : null;
        }

        public TokenBindingType BindingType
        {
            get { return _bindingType; }
        }
    }
}
