﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization.Converters;
using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization
{
    /// <summary>
    /// Converter for <see cref="Enum"/> types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
#if MAKE_UNREVIEWED_APIS_INTERNAL
    internal
#else
    public
#endif
    class JsonEnumConverterAttribute : JsonValueConverterAttribute
    {
        public JsonEnumConverterAttribute()
        {
            PropertyType = typeof(Enum);
        }

        public JsonEnumConverterAttribute(bool treatAsString = default)
        {
            TreatAsString = treatAsString;
            PropertyType = typeof(Enum);
        }

        /// <summary>
        /// Determines how an enum should be converted to\from a <see cref="string"/>. By default, enums are <see cref="long"/>.
        /// </summary>
        public bool TreatAsString { get; set; }

        public override JsonValueConverter<TValue> GetConverter<TValue>()
        {
            // todo: determine caching strategy for converters
            return new DefaultEnumConverter<TValue>(TreatAsString);
        }
    }
}
