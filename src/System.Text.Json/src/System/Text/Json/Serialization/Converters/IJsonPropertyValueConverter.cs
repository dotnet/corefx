// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json.Serialization.Converters
{
#if MAKE_UNREVIEWED_APIS_INTERNAL
    internal
#else
    public
#endif
    interface IJsonValueConverter<TValue>
    {
        bool TryRead(Type valueType, ref Utf8JsonReader reader, out TValue value);

        // todo: combine these Write methods if feasible so only one convertion method needs to exist for properties and elements.
        void Write(TValue value, ref Utf8JsonWriter writer);
        void Write(Span<byte> escapedPropertyName, TValue value, ref Utf8JsonWriter writer);
    }
}
