// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization.Converters;

namespace System.Text.Json.Serialization.Policies
{
#if MAKE_UNREVIEWED_APIS_INTERNAL
    internal
#else
    public
#endif
    abstract class JsonValueConverter<TValue> : IJsonValueConverter<TValue>
    {
        public abstract bool TryRead(Type valueType, ref Utf8JsonReader reader, out TValue value);
        public abstract void Write(TValue value, ref Utf8JsonWriter writer);
        public abstract void Write(Span<byte> escapedPropertyName, TValue value, ref Utf8JsonWriter writer);
    }
}
