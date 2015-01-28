// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;

namespace System.Reflection.Metadata
{
    public struct MethodSignature<TType>
    {
        private readonly SignatureHeader header;
        private readonly TType returnType;
        private readonly int requiredParameterCount;
        private readonly int genericParameterCount;
        private readonly ImmutableArray<TType> parameterTypes;

        public MethodSignature(SignatureHeader header, TType returnType, int requiredParameterCount, int genericParameterCount, ImmutableArray<TType> parameterTypes)
        {
            this.header = header;
            this.returnType = returnType;
            this.genericParameterCount = 0;
            this.requiredParameterCount = requiredParameterCount;
            this.parameterTypes = parameterTypes;
        }

        public SignatureHeader Header
        {
            get { return this.header; }
        }

        public TType ReturnType
        {
            get { return returnType; }
        }

        public int RequiredParameterCount
        {
            get { return requiredParameterCount; }
        }

        public int GenericParameterCount
        {
            get { return genericParameterCount; }
        }

        public ImmutableArray<TType> ParameterTypes
        {
            get { return parameterTypes; }
        }
    }
}
