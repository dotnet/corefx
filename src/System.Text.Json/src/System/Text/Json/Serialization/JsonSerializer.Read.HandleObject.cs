// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Text.Json.Serialization
{
    public static partial class JsonSerializer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void HandleStartObject(JsonSerializerOptions options, ref ReadStack state)
        {
            Type objType;

            if (state.Current.IsEnumerable() || state.Current.IsPropertyEnumerable())
            {
                // An array of objects either on the current property or on a list
                objType = state.Current.GetElementType();
                state.Push();
                state.Current.JsonClassInfo = options.GetOrAddClass(objType);
            }
            else if (state.Current.JsonPropertyInfo != null)
            {
                // Nested object
                objType = state.Current.JsonPropertyInfo.PropertyType;
                state.Push();
                state.Current.JsonClassInfo = options.GetOrAddClass(objType);
            }
            else if (!state.IsLastFrame)
            {
                // todo: deserialize loosely-typed object if there is a callback for IJsonTypeConverterOnDeserialized, and then invoke that callback.
                return;
            }

            JsonClassInfo classInfo = state.Current.JsonClassInfo;
            state.Current.ReturnValue = classInfo.CreateObject();
            state.Current.TypeConverter = classInfo.CreateTypeConverter(state.Current.ReturnValue);
            classInfo.CallOnDeserializing(state.Current.TypeConverter, state.Current.ReturnValue, options);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HandleEndObject(JsonSerializerOptions options, ref ReadStack state)
        {
            if (state.Current.JsonPropertyInfo == null && !state.IsLastFrame)
            {
                // the json is not being applied to the object.
                return false;
            }

            object value = state.Current.ReturnValue;

            state.Current.JsonClassInfo.CallOnDeserialized(state.Current.TypeConverter, state.Current.ReturnValue, options);

            if (state.IsLastFrame)
            {
                state.Current.Reset();
                state.Current.ReturnValue = value;
                return true;
            }

            state.Pop();
            ReadStackFrame.SetReturnValue(value, options, ref state.Current);
            return false;
        }
    }
}
