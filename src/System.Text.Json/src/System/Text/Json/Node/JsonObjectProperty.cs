﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json
{
    internal class JsonObjectProperty
    {
        public string Name { get; }
        public JsonNode Value { get; set;  }
        public JsonObjectProperty Prev { get; set; }
        public JsonObjectProperty Next { get; set; }

        public JsonObjectProperty(string name, JsonNode value, JsonObjectProperty prev, JsonObjectProperty next)
        {
            Name = name;
            Value = value;
            Prev = prev;
            Next = next;
        }
    }
}
