// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Reflection.Metadata
{
    public struct CustomAttributeNamedArgument<TType>
    {
        private readonly string _name;
        private readonly CustomAttributeNamedArgumentKind _kind;
        private readonly TType _type;
        private readonly object _value;

        public CustomAttributeNamedArgument(string name, CustomAttributeNamedArgumentKind kind, TType type, object value)
        {
            _name = name;
            _kind = kind;
            _type = type;
            _value = value;
        }

        public string Name
        {
            get { return _name; }
        }

        public CustomAttributeNamedArgumentKind Kind
        {
            get { return _kind; }
        }

        public TType Type
        {
            get { return _type; }
        }

        public object Value
        {
            get { return _type; }
        }
    }
}
