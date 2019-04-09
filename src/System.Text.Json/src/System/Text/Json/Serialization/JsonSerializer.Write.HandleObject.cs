﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Text.Json.Serialization
{
    public static partial class JsonSerializer
    {
        private static bool WriteObject(
            JsonSerializerOptions options,
            ref Utf8JsonWriter writer,
            ref WriteStack state)
        {
            JsonClassInfo classInfo = state.Current.JsonClassInfo;

            // Write the start.
            if (!state.Current.StartObjectWritten)
            {
                if (state.Current.JsonPropertyInfo?._escapedName == null)
                {
                    writer.WriteStartObject();
                }
                else
                {
                    writer.WriteStartObject(state.Current.JsonPropertyInfo._escapedName);
                }
                state.Current.StartObjectWritten = true;
            }

            // Determine if we are done enumerating properties.
            if (state.Current.PropertyIndex != classInfo.PropertyCount)
            {
                HandleObject(options, ref writer, ref state);
                return false;
            }

            writer.WriteEndObject();

            if (state.Current.PopStackOnEndObject)
            {
                state.Pop();
            }
            else
            {
                state.Current.EndObject();
            }

            return true;
        }

        private static bool HandleObject(
                JsonSerializerOptions options,
                ref Utf8JsonWriter writer,
                ref WriteStack state)
        {
            Debug.Assert(
                state.Current.JsonClassInfo.ClassType == ClassType.Object ||
                state.Current.JsonClassInfo.ClassType == ClassType.Unknown);

            JsonPropertyInfo jsonPropertyInfo = state.Current.JsonClassInfo.GetProperty(state.Current.PropertyIndex);

            bool obtainedValue = false;
            object currentValue = null;

            // Check for polymorphism.
            if (jsonPropertyInfo.ClassType == ClassType.Unknown)
            {
                currentValue = jsonPropertyInfo.GetValueAsObject(state.Current.CurrentValue, options);
                obtainedValue = true;
                GetRuntimePropertyInfo(currentValue, state.Current.JsonClassInfo, ref jsonPropertyInfo, options);
            }

            state.Current.JsonPropertyInfo = jsonPropertyInfo;

            if (jsonPropertyInfo.ClassType == ClassType.Value)
            {
                jsonPropertyInfo.Write(options, ref state.Current, ref writer);
                state.Current.NextProperty();
                return true;
            }

            // A property that returns an enumerator keeps the same stack frame.
            if (jsonPropertyInfo.ClassType == ClassType.Enumerable)
            {
                bool endOfEnumerable = HandleEnumerable(jsonPropertyInfo.ElementClassInfo, options, ref writer, ref state);
                if (endOfEnumerable)
                {
                    state.Current.NextProperty();
                }

                return endOfEnumerable;
            }

            // A property that returns an object.
            if (!obtainedValue)
            {
                currentValue = jsonPropertyInfo.GetValueAsObject(state.Current.CurrentValue, options);
            }

            if (currentValue != null)
            {
                // A new stack frame is required.
                JsonPropertyInfo previousPropertyInfo = state.Current.JsonPropertyInfo;

                state.Current.NextProperty();

                JsonClassInfo nextClassInfo = options.GetOrAddClass(jsonPropertyInfo.RuntimePropertyType);
                state.Push(nextClassInfo, currentValue);

                // Set the PropertyInfo so we can obtain the property name in order to write it.
                state.Current.JsonPropertyInfo = previousPropertyInfo;
            }
            else
            {
                if (!jsonPropertyInfo.IgnoreNullPropertyValueOnWrite(options))
                {
                    writer.WriteNull(jsonPropertyInfo._escapedName);
                }

                state.Current.NextProperty();
            }

            return true;
        }
    }
}
