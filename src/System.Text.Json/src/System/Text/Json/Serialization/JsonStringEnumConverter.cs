﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Text.Json.Serialization.Converters;

namespace System.Text.Json.Serialization
{
    /// <summary>
    /// Converter to convert enums to and from strings.
    /// </summary>
    /// <remarks>
    /// Reading is case insensitive, writing can be customized via a <see cref="JsonNamingPolicy" />.
    /// </remarks>
    public sealed class JsonStringEnumConverter : JsonConverterFactory
    {
        private readonly JsonNamingPolicy _namingPolicy;
        private readonly EnumConverterOptions _converterOptions;

        /// <summary>
        /// Constructor. Creates the <see cref="JsonStringEnumConverter"/> with the
        /// default naming policy and allows integer values.
        /// </summary>
        public JsonStringEnumConverter()
            : this(namingPolicy: null, allowIntegerValues: true)
        {
            // An empty constructor is needed for construction via attributes
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="namingPolicy">
        /// Optional naming policy for writing enum values.
        /// </param>
        /// <param name="allowIntegerValues">
        /// True to allow undefined enum values. When true, if an enum value isn't
        /// defined it will output as a number rather than a string.
        /// </param>
        public JsonStringEnumConverter(JsonNamingPolicy namingPolicy = null, bool allowIntegerValues = true)
        {
            _namingPolicy = namingPolicy;
            _converterOptions = allowIntegerValues
                ? EnumConverterOptions.AllowNumbers | EnumConverterOptions.AllowStrings
                : EnumConverterOptions.AllowStrings;
        }

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum;
        }

        /// <inheritdoc />
        protected override JsonConverter CreateConverter(Type typeToConvert)
        {
            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                typeof(JsonConverterEnum<>).MakeGenericType(typeToConvert),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                new object[] { _converterOptions, _namingPolicy },
                culture: null);

            return converter;
        }
    }
}
