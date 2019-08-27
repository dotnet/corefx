// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json
{
    internal class JsonObjectProperty
    {
        internal string _name { get; set; }
        internal JsonNode _value { get; set; }
        internal JsonObjectProperty _prev { get; set; }
        internal JsonObjectProperty _next { get; set; }

        public JsonObjectProperty(string name, JsonNode value, JsonObjectProperty prev, JsonObjectProperty next)
        {
            _name = name;
            _value = value;
            _prev = prev;
            _next = next;
        }
    }
}
