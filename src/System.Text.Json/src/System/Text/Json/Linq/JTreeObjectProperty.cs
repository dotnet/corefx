// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json.Linq
{
    internal class JTreeObjectProperty
    {
        internal string Name { get; }
        internal JTreeNode Value { get; set;  }
        internal JTreeObjectProperty Prev { get; set; }
        internal JTreeObjectProperty Next { get; set; }

        public JTreeObjectProperty(string name, JTreeNode value, JTreeObjectProperty prev, JTreeObjectProperty next)
        {
            Name = name;
            Value = value;
            Prev = prev;
            Next = next;
        }
    }
}
