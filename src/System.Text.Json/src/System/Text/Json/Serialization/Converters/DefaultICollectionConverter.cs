﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class DefaultICollectionConverter : JsonEnumerableConverter
    {
        public override IEnumerable CreateFromList(ref ReadStack state, IList sourceList, JsonSerializerOptions options)
        {
            Type enumerableType = state.Current.JsonPropertyInfo.RuntimePropertyType;
            JsonClassInfo elementClassInfo = state.Current.JsonPropertyInfo.ElementClassInfo;
            JsonPropertyInfo propertyInfo = options.GetJsonPropertyInfoFromClassInfo(elementClassInfo, options);
            return propertyInfo.CreateIEnumerableInstance(enumerableType, sourceList, state.JsonPath, options);
        }
    }
}
