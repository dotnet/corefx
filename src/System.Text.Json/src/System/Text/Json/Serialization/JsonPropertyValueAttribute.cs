// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json.Serialization
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
#if MAKE_UNREVIEWED_APIS_INTERNAL
    internal
#else
    public
#endif
    class JsonPropertyValueAttribute : Attribute
    {
        public bool? IgnoreNullValueOnRead { get; set; }
        public bool? IgnoreNullValueOnWrite { get; set; }
    }
}
