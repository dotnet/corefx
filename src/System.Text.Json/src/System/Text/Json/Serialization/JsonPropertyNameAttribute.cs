// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
#if MAKE_UNREVIEWED_APIS_INTERNAL
    internal
#else
    public
#endif
    class JsonPropertyNameAttribute : JsonPropertyNamePolicyAttribute
    {
        public JsonPropertyNameAttribute() { }

        public JsonPropertyNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public override string Read(string value)
        {
            return value;
        }

        public override string Write(string value)
        {
            return Name;
        }
    }
}
