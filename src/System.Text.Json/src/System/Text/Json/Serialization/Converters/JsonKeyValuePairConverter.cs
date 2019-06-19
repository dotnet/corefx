﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class JsonKeyValuePairConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
                return false;

            Type generic = typeToConvert.GetGenericTypeDefinition();
            return (generic == typeof(KeyValuePair<,>));
        }

        protected override JsonConverter CreateConverter(Type type)
        {
            Type keyType = type.GetGenericArguments()[0];
            Type valueType = type.GetGenericArguments()[1];

            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                typeof(JsonKeyValuePairConverter<,>).MakeGenericType(new Type[] { keyType, valueType }),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: null,
                culture: null);

            return converter;
        }
    }
}
