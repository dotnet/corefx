// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization.Converters;

namespace System.Text.Json.Serialization
{
#if MAKE_UNREVIEWED_APIS_INTERNAL
    internal
#else
    public
#endif
    partial class JsonClassInfo
    {
        private bool _hasOnSerializing;
        private bool _hasOnSerialized;
        private bool _hasOnDeserializing;
        private bool _hasOnDeserialized;

        private void InitializeTypeConverterCallbacks()
        {
            // Use the converter type if specified, otherwise use the poco type.
            Type type;
            if (_converterType != null)
            {
                type = _converterType;
            }
            else
            {
                type = Type;
            }

            // Cache whether the type implements the interface so we can do a faster direct cast.
            _hasOnSerializing = typeof(IJsonTypeConverterOnSerializing).IsAssignableFrom(type);
            _hasOnSerialized = typeof(IJsonTypeConverterOnSerialized).IsAssignableFrom(type);
            _hasOnDeserializing = typeof(IJsonTypeConverterOnDeserializing).IsAssignableFrom(type);
            _hasOnDeserialized = typeof(IJsonTypeConverterOnDeserialized).IsAssignableFrom(type);
        }

        internal object CreateTypeConverter(object obj)
        {
            if (_converterType != null)
            {
                return Activator.CreateInstance(_converterType, nonPublic: true);
            }

            return obj;
        }

        internal void CallOnSerializing(
            object typeConverter,
            object obj,
            ref Utf8JsonWriter writer,
            JsonSerializerOptions options)
        {
            if (_hasOnSerializing)
            {
                var tc = (IJsonTypeConverterOnSerializing)typeConverter;
                tc.OnSerializing(obj, this, ref writer, options);
            }
        }

        internal void CallOnSerialized(
            object typeConverter,
            object obj,
            ref Utf8JsonWriter writer,
            JsonSerializerOptions options)
        {
            if (_hasOnSerialized)
            {
                var tc = (IJsonTypeConverterOnSerialized)typeConverter;
                tc.OnSerialized(obj, this, ref writer, options);
            }
        }

        internal void CallOnDeserializing(
            object typeConverter,
            object obj,
            JsonSerializerOptions options)
        {
            if (_hasOnDeserializing)
            {
                var tc = (IJsonTypeConverterOnDeserializing)typeConverter;
                tc.OnDeserializing(obj, this, options);
            }
        }

        internal void CallOnDeserialized(
            object typeConverter,
            object obj,
            JsonSerializerOptions options)
        {
            if (_hasOnDeserialized)
            {
                var tc = (IJsonTypeConverterOnDeserialized)typeConverter;
                tc.OnDeserialized(obj, this, options);
            }
        }
    }
}
