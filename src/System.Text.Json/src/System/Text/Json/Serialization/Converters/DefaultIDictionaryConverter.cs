// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class DefaultIDictionaryConverter : JsonTemporaryDictionaryConverter
    {
        public override bool OwnsImplementedCollectionType(Type implementedCollectionType, Type collectionElementType)
        {
            throw new NotImplementedException();
        }

        public override object EndDictionary(ref ReadStack state, JsonSerializerOptions options)
        {
            Debug.Assert(state.Current.DictionaryConverterState?.TemporaryDictionary != null);

            // Note: Types are defined explicityly here for performance.
            try
            {
                /*if (parentType.FullName == JsonClassInfo.HashtableTypeName)
                {
                    return new Hashtable(sourceDictionary);
                }*/

                // ReadOnlyDictionary<,> would require a reference to System.ObjectModel

                return (IDictionary)Activator.CreateInstance(state.Current.JsonPropertyInfo.DeclaredPropertyType, state.Current.DictionaryConverterState.TemporaryDictionary);
            }
            catch (MissingMethodException)
            {
                ThrowHelper.ThrowNotSupportedException_DeserializeInstanceConstructorOfTypeNotFound(state.Current.JsonPropertyInfo.DeclaredPropertyType, state.Current.DictionaryConverterState.TemporaryDictionary.GetType());
                return null;
            }
        }
    }
}
