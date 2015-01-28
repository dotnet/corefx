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
        private readonly string name;
        private readonly CustomAttributeNamedArgumentKind kind;
        private readonly TType type;
        private readonly object value;

        public CustomAttributeNamedArgument(string name, CustomAttributeNamedArgumentKind kind, TType type, object value)
        {
            this.name = name;
            this.kind = kind;
            this.type = type;
            this.value = value;
        }

        public string Name
        {
            get { return name; }
        }

        public CustomAttributeNamedArgumentKind Kind
        {
            get { return kind; }
        }

        public TType Type
        {
            get { return type; }
        }

        public object Value
        {
            get { return type; }
        }
    }
}
