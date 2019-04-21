// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Text.Json.Serialization
{
    public static partial class JsonSerializer
    {
        private static void HandleStartObject(JsonSerializerOptions options, ref ReadStack state)
        {
            if (state.Current.Skip())
            {
                state.Push();
                state.Current.Drain = true;
                return;
            }

            if (state.Current.IsEnumerable() || state.Current.IsPropertyEnumerable())
            {
                // An array of objects either on the current property or on a list
                Type objType = state.Current.GetElementType();
                state.Push();

                state.Current.JsonClassInfo = options.GetOrAddClass(objType);
            }
            else if (state.Current.IsDictionary())
            {
                JsonPropertyInfo jsonPropertyInfo = state.Current.JsonPropertyInfo;

                bool skip = jsonPropertyInfo != null && !jsonPropertyInfo.ShouldDeserialize;
                if (skip || state.Current.Skip())
                {
                    // The dictionary is not being applied to the object.
                    state.Push();
                    state.Current.Drain = true;
                    return;
                }

                Type elementType = state.Current.JsonClassInfo.ElementClassInfo.Type;

                state.Current.TempDictKeys = new List<string>();
                state.Current.TempDictValues = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                state.Current.ReturnValue = (IDictionary)Activator.CreateInstance(state.Current.JsonClassInfo.Type);

                return;
            }
            else if (state.Current.JsonPropertyInfo != null)
            {
                // Nested object
                Type objType = state.Current.JsonPropertyInfo.RuntimePropertyType;
                state.Push();
                state.Current.JsonClassInfo = options.GetOrAddClass(objType);
            }

            JsonClassInfo classInfo = state.Current.JsonClassInfo;
            state.Current.ReturnValue = classInfo.CreateObject();
        }

        private static bool HandleEndObject(JsonSerializerOptions options, ref ReadStack state)
        {
            bool isLastFrame = state.IsLastFrame;
            if (state.Current.Drain)
            {
                state.Pop();
                return isLastFrame;
            }

            state.Current.JsonClassInfo.UpdateSortedPropertyCache(ref state.Current);

            if (state.Current.IsDictionary())
            {
                ReadStackFrame frame = state.Current;

                int keyCount = ((IList)frame.TempDictKeys).Count;

                Debug.Assert(keyCount == (frame.TempDictValues).Count);

                IList keys = frame.TempDictKeys;
                IList values = frame.TempDictValues;

                for (int i = 0; i < keyCount; i++)
                {
                    ((IDictionary)state.Current.ReturnValue).Add(keys[i], values[i]);
                }
            }

            object value = state.Current.ReturnValue;

            if (isLastFrame)
            {
                state.Current.Reset();
                state.Current.ReturnValue = value;
                return true;
            }

            state.Pop();
            ApplyObjectToEnumerable(value, options, ref state.Current);
            return false;
        }

        internal static void ApplyValueToDictionary<TProperty>(ref TProperty value, JsonSerializerOptions options, ref ReadStackFrame frame)
        {
            Debug.Assert(frame.IsDictionary());
            ((List<TProperty>)frame.TempDictValues).Add(value);
            Debug.Assert(((IList)frame.TempDictKeys).Count == ((IList)frame.TempDictValues).Count);
        }

        internal static void ApplyObjectToDictionary(object value, JsonSerializerOptions options, ref ReadStackFrame frame)
        {
            Debug.Assert(frame.IsDictionary());
            (frame.TempDictValues).Add(value);
            Debug.Assert(((IList)frame.TempDictKeys).Count == ((IList)frame.TempDictValues).Count);
        }
    }
}
