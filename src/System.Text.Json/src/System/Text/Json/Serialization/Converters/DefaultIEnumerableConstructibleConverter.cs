// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Concurrent;
using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class DefaultIEnumerableConstructibleConverter : JsonEnumerableConverter
    {
        private static readonly ConcurrentDictionary<Type, JsonPropertyInfo> s_objectJsonProperties = new ConcurrentDictionary<Type, JsonPropertyInfo>();

        public static JsonPropertyInfo GetElementJsonPropertyInfo(JsonClassInfo elementClassInfo, JsonSerializerOptions options)
        {
            if (elementClassInfo.ClassType != ClassType.Object)
            {
                return elementClassInfo.GetPolicyProperty();
            }

            Type objectType = elementClassInfo.Type;

            if (!s_objectJsonProperties.TryGetValue(objectType, out JsonPropertyInfo propertyInfo))
            {
                propertyInfo = JsonClassInfo.CreateProperty(objectType, objectType, null, typeof(object), options);
                s_objectJsonProperties[objectType] = propertyInfo;
            }

            return propertyInfo;
        }

        public override IEnumerable CreateFromList(ref ReadStack state, IList sourceList, JsonSerializerOptions options)
        {
            Type enumerableType = state.Current.JsonPropertyInfo.RuntimePropertyType;
            JsonClassInfo elementClassInfo = state.Current.JsonPropertyInfo.ElementClassInfo;
            JsonPropertyInfo propertyInfo = GetElementJsonPropertyInfo(elementClassInfo, options);
            return propertyInfo.CreateIEnumerableConstructibleType(enumerableType, sourceList);
        }
    }
}
