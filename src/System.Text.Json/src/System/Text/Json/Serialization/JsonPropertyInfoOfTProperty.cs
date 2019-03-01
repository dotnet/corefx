// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Text.Json.Serialization.Converters;
using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization
{
#if MAKE_UNREVIEWED_APIS_INTERNAL
    internal
#else
    public
#endif
    abstract class JsonPropertyInfo<TProperty> : JsonPropertyInfo
    {
        // Constructor used for internal identifiers
        internal JsonPropertyInfo() { }

        internal JsonPropertyInfo(Type classType, Type propertyType, PropertyInfo propertyInfo, Type elementType, JsonSerializerOptions options) :
            base(classType, propertyType, propertyInfo, elementType, options)
        { }

        public JsonValueConverter<TProperty> ValueConverter { get; internal set; }

        internal override void GetPolicies(JsonSerializerOptions options)
        {
            ValueConverter = DefaultConverters.GetPropertyValueConverter<TProperty>(ParentClassType, PropertyInfo, PropertyType, options);
            base.GetPolicies(options);
        }
    }
}
