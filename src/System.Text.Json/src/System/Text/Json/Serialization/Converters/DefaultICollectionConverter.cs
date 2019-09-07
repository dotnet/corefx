// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class DefaultICollectionConverter : JsonTemporaryListConverter
    {
        public override bool OwnsImplementedCollectionType(Type implementedCollectionType, Type collectionElementType)
        {
            //Queues, Stacks, SortedSets, readonly collections
            return false;
        }

        public override Type ResolveRunTimeType(JsonPropertyInfo jsonPropertyInfo)
        {
            // Only things you can't spin on should go here.

            // todo: Figure out what the runtime type was before for these collections.

            return typeof(List<>).MakeGenericType(jsonPropertyInfo.CollectionElementType);
        }

        public override object EndEnumerable(ref ReadStack state, JsonSerializerOptions options)
        {
            Debug.Assert(state.Current.EnumerableConverterState?.TemporaryList != null);

            try
            {
                /*
                // Note: Types are defined explicityly here for performance.
                if (parentType.IsGenericType)
                {
                    Type genericTypeDefinition = parentType.GetGenericTypeDefinition();

                    IList<TDeclaredProperty> typedList = (IList<TDeclaredProperty>)sourceList;

                    if (genericTypeDefinition == typeof(Stack<>))
                    {
                        return new Stack<TDeclaredProperty>(typedList);
                    }
                    else if (genericTypeDefinition == typeof(Queue<>))
                    {
                        return new Queue<TDeclaredProperty>(typedList);
                    }
                    else if (genericTypeDefinition == typeof(HashSet<>))
                    {
                        return new HashSet<TDeclaredProperty>(typedList);
                    }
                    else if (genericTypeDefinition == typeof(LinkedList<>))
                    {
                        return new LinkedList<TDeclaredProperty>(typedList);
                    }
                    else if (genericTypeDefinition == typeof(ReadOnlyCollection<>))
                    {
                        return new ReadOnlyCollection<TDeclaredProperty>(typedList);
                    }
                    else if (genericTypeDefinition.FullName == JsonClassInfo.ReadOnlyObservableCollectionGenericTypeName)
                    {
                        // new ObservableCollection<TDeclaredProperty>(typedList)
                        object ObservableCollection = Activator.CreateInstance(
                            parentType.Assembly.GetType(JsonClassInfo.ObservableCollectionGenericTypeName).MakeGenericType(typeof(TDeclaredProperty)),
                            typedList);

                        // new ReadOnlyObservableCollection<TDeclaredProperty>(ObservableCollection);
                        return (IEnumerable)Activator.CreateInstance(parentType, ObservableCollection);
                    }
                }
                else
                {
                    if (parentType == typeof(ArrayList))
                    {
                        return new ArrayList(sourceList);
                    }

                    //Stack & Queue would require a reference to System.Collections.NonGeneric
                }
                */

                return (IEnumerable)Activator.CreateInstance(state.Current.JsonPropertyInfo.DeclaredPropertyType, state.Current.EnumerableConverterState.TemporaryList);
            }
            catch (MissingMethodException)
            {
                ThrowHelper.ThrowNotSupportedException_DeserializeInstanceConstructorOfTypeNotFound(state.Current.JsonPropertyInfo.DeclaredPropertyType, state.Current.EnumerableConverterState.TemporaryList.GetType());
                return null;
            }
        }
    }
}
