// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json.Linq
{
    internal class JObjectProperty
    {
        internal string Name { get; }
        internal JNode Value { get; set;  }
        internal JObjectProperty Prev { get; set; }
        internal JObjectProperty Next { get; set; }

        public JObjectProperty(string name, JNode value, JObjectProperty prev, JObjectProperty next)
        {
            Name = name;
            Value = value;
            Prev = prev;
            Next = next;
        }
    }
}
