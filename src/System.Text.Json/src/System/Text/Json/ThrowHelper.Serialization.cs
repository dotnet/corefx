﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace System.Text.Json
{
    internal static partial class ThrowHelper
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowArgumentException_DeserializeWrongType(Type type, object value)
        {
            throw new ArgumentException(SR.Format(SR.DeserializeWrongType, type.FullName, value.GetType().FullName));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowJsonReaderException_DeserializeUnableToConvertValue(Type propertyType, in Utf8JsonReader reader, in ReadStack state)
        {
            throw new JsonReaderException(SR.Format(SR.DeserializeUnableToConvertValue, state.PropertyPath, propertyType), reader.CurrentState);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowJsonReaderException_DeserializeCannotBeNull(in Utf8JsonReader reader, in ReadStack state)
        {
            throw new JsonReaderException(SR.Format(SR.DeserializeCannotBeNull, state.PropertyPath), reader.CurrentState);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowObjectDisposedException(string name)
        {
            throw new ObjectDisposedException(name);
        }
    }
}
