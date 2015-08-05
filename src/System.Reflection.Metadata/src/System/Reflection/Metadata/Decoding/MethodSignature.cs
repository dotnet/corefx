// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;

namespace System.Reflection.Metadata
{
    public struct MethodSignature<TType>
    {
        private readonly SignatureHeader _header;
        private readonly TType _returnType;
        private readonly int _requiredParameterCount;
        private readonly int _genericParameterCount;
        private readonly ImmutableArray<TType> _parameterTypes;

        public MethodSignature(SignatureHeader header, TType returnType, int requiredParameterCount, int genericParameterCount, ImmutableArray<TType> parameterTypes)
        {
            _header = header;
            _returnType = returnType;
            _genericParameterCount = genericParameterCount;
            _requiredParameterCount = requiredParameterCount;
            _parameterTypes = parameterTypes;
        }

        public SignatureHeader Header
        {
            get { return _header; }
        }

        public TType ReturnType
        {
            get { return _returnType; }
        }

        public int RequiredParameterCount
        {
            get { return _requiredParameterCount; }
        }

        public int GenericParameterCount
        {
            get { return _genericParameterCount; }
        }

        public ImmutableArray<TType> ParameterTypes
        {
            get { return _parameterTypes; }
        }
    }
}
