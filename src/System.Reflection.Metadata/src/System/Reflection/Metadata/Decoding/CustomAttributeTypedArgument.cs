// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Reflection.Metadata
{
    public struct CustomAttributeTypedArgument<TType>
    {
        private readonly TType type;
        private readonly object value;

        public CustomAttributeTypedArgument(TType type, object value)
        {
            this.type = type;
            this.value = value;
        }

        public TType Type
        {
            get { return type; }
        }

        public object Value
        {
            get { return value; }
        }
    }
}
