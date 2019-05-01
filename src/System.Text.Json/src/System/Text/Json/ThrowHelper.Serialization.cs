// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Reflection;
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
        public static void ThrowJsonSerializationException_DeserializeUnableToConvertValue(Type propertyType, in Utf8JsonReader reader, string path)
        {
            ThowJsonSerializationException(SR.Format(SR.DeserializeUnableToConvertValue, propertyType.FullName), in reader, path);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowJsonSerializationException_DeserializeCannotBeNull(in Utf8JsonReader reader, string path)
        {
            ThowJsonSerializationException(SR.DeserializeCannotBeNull, in reader, path);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowObjectDisposedException(string name)
        {
            throw new ObjectDisposedException(name);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowInvalidOperationException_SerializerOptionsImmutable()
        {
            throw new InvalidOperationException(SR.SerializerOptionsImmutable);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowInvalidOperationException_SerializerPropertyNameConflict(JsonClassInfo jsonClassInfo, JsonPropertyInfo jsonPropertyInfo)
        {
            throw new InvalidOperationException(SR.Format(SR.SerializerPropertyNameConflict, jsonClassInfo.Type.FullName, jsonPropertyInfo.PropertyInfo.Name));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowInvalidOperationException_SerializerPropertyNameNull(JsonClassInfo jsonClassInfo, JsonPropertyInfo jsonPropertyInfo)
        {
            throw new InvalidOperationException(SR.Format(SR.SerializerPropertyNameNull, jsonClassInfo.Type.FullName, jsonPropertyInfo.PropertyInfo.Name));
        }

        public static void ThrowJsonSerializationException_DeserializeDataRemaining(long length, long bytesRemaining)
        {
            throw new JsonSerializationException(SR.Format(SR.DeserializeDataRemaining, length, bytesRemaining), "");
        }

        public static void ThrowJsonSerializationException_DeserializeDuplicateKey(string key, in Utf8JsonReader reader, string path)
        {
            ThowJsonSerializationException(SR.Format(SR.DeserializeDuplicateKey, key), in reader, path);
        }

        private static void ThowJsonSerializationException(string message, in Utf8JsonReader reader, string path)
        {
            long lineNumber = reader.CurrentState._lineNumber;
            long bytePositionInLine = reader.CurrentState._bytePositionInLine;

            message += $" Path: {path} | LineNumber: {lineNumber} | BytePositionInLine: {bytePositionInLine}.";
            throw new JsonSerializationException(message, path, lineNumber, bytePositionInLine);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowSerializationException(JsonReaderException exception, string path)
        {
            string message = exception.Message;

            // Insert the "Path" portion before "LineNumber" and "BytePositionInLine".
            int iPos = message.LastIndexOf(" LineNumber: ", StringComparison.InvariantCulture);
            if (iPos >= 0)
            {
                message = $"{message.Substring(0, iPos)} Path: {path} |{message.Substring(iPos)}";
            }
            else
            {
                message += $" Path: {path}.";
            }

            throw new JsonSerializationException(message, path, exception.LineNumber, exception.BytePositionInLine, exception);
        }
    }
}
