﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Reflection.Metadata
{
    public struct CustomAttributeTypedArgument<TType>
    {
        private readonly TType _type;
        private readonly object _value;

        public CustomAttributeTypedArgument(TType type, object value)
        {
            _type = type;
            _value = value;
        }

        public TType Type
        {
            get { return _type; }
        }

        public object Value
        {
            get { return _value; }
        }
    }
}
