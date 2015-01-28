// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;

namespace System.Reflection.Metadata
{
    public struct CustomAttributeValue<TType>
    {
        private readonly ImmutableArray<CustomAttributeTypedArgument<TType>> fixedArguments;
        private readonly ImmutableArray<CustomAttributeNamedArgument<TType>> namedArguments;

        public CustomAttributeValue(ImmutableArray<CustomAttributeTypedArgument<TType>> fixedArguments, ImmutableArray<CustomAttributeNamedArgument<TType>> namedArguments)
        {
            this.fixedArguments = fixedArguments;
            this.namedArguments = namedArguments;
        }

        public ImmutableArray<CustomAttributeTypedArgument<TType>> FixedArguments
        {
            get { return fixedArguments; }
        }

        public ImmutableArray<CustomAttributeNamedArgument<TType>> NamedArguments
        {
            get { return namedArguments; }
        }
    }
}
