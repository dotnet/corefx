// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
#if MAKE_UNREVIEWED_APIS_INTERNAL
    internal
#else
    public
#endif
    class JsonCamelCasingConverterAttribute : JsonPropertyNamePolicyAttribute
    {
        public JsonCamelCasingConverterAttribute() { }

        public override string Read(string value)
        {
            return CamelCasingPolicy.Read(value);
        }

        public override string Write(string value)
        {
            return CamelCasingPolicy.Write(value);
        }
    }
}
