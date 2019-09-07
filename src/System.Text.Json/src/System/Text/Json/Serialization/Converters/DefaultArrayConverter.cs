// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class DefaultArrayConverter : JsonTemporaryListConverter
    {
        public override bool OwnsImplementedCollectionType(Type implementedCollectionType, Type collectionElementType)
        {
            return implementedCollectionType.IsArray;
        }

        public override Type ResolveRunTimeType(JsonPropertyInfo jsonPropertyInfo)
        {
            Debug.Assert(jsonPropertyInfo.DeclaredPropertyType.IsArray);

            return jsonPropertyInfo.DeclaredPropertyType;
        }

        public override object EndEnumerable(ref ReadStack state, JsonSerializerOptions options)
        {
            Debug.Assert(state.Current.EnumerableConverterState?.TemporaryList != null);

            JsonEnumerableConverterState converterState = state.Current.EnumerableConverterState;

            Array array;

            if (converterState.TemporaryList.Count > 0 && converterState.TemporaryList[0] is Array probe)
            {
                array = Array.CreateInstance(probe.GetType(), converterState.TemporaryList.Count);

                int i = 0;
                foreach (IList child in converterState.TemporaryList)
                {
                    if (child is Array childArray)
                    {
                        array.SetValue(childArray, i++);
                    }
                }
            }
            else
            {
                array = Array.CreateInstance(state.Current.JsonPropertyInfo.CollectionElementType, converterState.TemporaryList.Count);
                converterState.TemporaryList.CopyTo(array, 0);
            }

            return array;
        }
    }
}
