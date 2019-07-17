// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// for now disabling error caused by not adding documentation to methods 
#pragma warning disable CS1591

namespace System.Text.Json
{
    public partial class JsonBoolean : JsonNode
    {
        public JsonBoolean() { }
        public JsonBoolean(bool value) { }

        public bool Value { get; set; }

        public static implicit operator JsonBoolean(bool value) { throw null; }

        public override bool Equals(object obj) { throw null; }
        public override int GetHashCode() { throw null; }
    }
}

#pragma warning restore CS1591
