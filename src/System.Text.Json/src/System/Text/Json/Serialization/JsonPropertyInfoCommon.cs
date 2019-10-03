// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;

namespace System.Text.Json
{
    /// <summary>
    /// Represents a strongly-typed property to prevent boxing and to create a direct delegate to the getter\setter.
    /// </summary>
    internal abstract class JsonPropertyInfoCommon<TClass, TDeclaredProperty, TRuntimeProperty, TConverter> : JsonPropertyInfo
    {
        public Func<object, TDeclaredProperty> Get { get; private set; }
        public Action<object, TDeclaredProperty> Set { get; private set; }

        public Action<TDeclaredProperty> AddItemToEnumerable { get; private set; }
        public Func<TDeclaredProperty, int> AddItemToEnumerableInt32 { get; private set; }
        public Func<TDeclaredProperty, bool> AddItemToEnumerableBool { get; private set; }

        public Action<string, TDeclaredProperty> AddItemToDictionary { get; private set; }

        public Action<string, TDeclaredProperty> AddItemToExtensionData { get; private set; }

        public JsonConverter<TConverter> Converter { get; internal set; }

        public override void Initialize(
            Type parentClassType,
            Type declaredPropertyType,
            Type runtimePropertyType,
            ClassType runtimeClassType,
            PropertyInfo propertyInfo,
            Type elementType,
            JsonConverter converter,
            JsonSerializerOptions options)
        {
            base.Initialize(
                parentClassType,
                declaredPropertyType,
                runtimePropertyType,
                runtimeClassType,
                propertyInfo,
                elementType,
                converter,
                options);

            if (propertyInfo != null &&
                // We only want to get the getter and setter if we are going to use them.
                // If the declared type is not the property info type, then we are just
                // getting metadata on how best to (de)serialize derived types.
                declaredPropertyType == propertyInfo.PropertyType)
            {
                if (propertyInfo.GetMethod?.IsPublic == true)
                {
                    HasGetter = true;
                    Get = options.MemberAccessorStrategy.CreatePropertyGetter<TClass, TDeclaredProperty>(propertyInfo);
                }

                if (propertyInfo.SetMethod?.IsPublic == true)
                {
                    HasSetter = true;
                    Set = options.MemberAccessorStrategy.CreatePropertySetter<TClass, TDeclaredProperty>(propertyInfo);
                }
            }
            else
            {
                IsPropertyPolicy = true;
                HasGetter = true;
                HasSetter = true;
            }

            GetPolicies();
        }

        public override JsonConverter ConverterBase
        {
            get
            {
                return Converter;
            }
            set
            {
                Debug.Assert(Converter == null);
                Debug.Assert(value is JsonConverter<TConverter>);

                Converter = (JsonConverter<TConverter>)value;
            }
        }

        public override object GetValueAsObject(object obj)
        {
            if (IsPropertyPolicy)
            {
                return obj;
            }

            Debug.Assert(HasGetter);
            return Get(obj);
        }

        public override void SetValueAsObject(object obj, object value)
        {
            Debug.Assert(HasSetter);
            TDeclaredProperty typedValue = (TDeclaredProperty)value;

            if (typedValue != null || !IgnoreNullValues)
            {
                Set(obj, typedValue);
            }
        }

        public override bool TryCreateEnumerableAddMethod(MethodInfo addMethod, object target, JsonSerializerOptions options)
        {
            AddItemToEnumerable = null;
            AddItemToEnumerableInt32 = null;
            AddItemToEnumerableBool = null;
            AddItemToDictionary = null;

            if (addMethod == default)
            {
                return false;
            }

            Type returnType = addMethod.ReturnType;

            Debug.Assert(addMethod.GetParameters().Length == 1);

            if (returnType == typeof(void))
            {
                AddItemToEnumerable = options.MemberAccessorStrategy.CreateAddDelegate<TDeclaredProperty>(addMethod, target);
            }
            else if (returnType == typeof(int))
            {
                AddItemToEnumerableInt32 = options.MemberAccessorStrategy.CreateAddDelegateInt32<TDeclaredProperty>(addMethod, target);
            }
            else if (returnType == typeof(bool))
            {
                AddItemToEnumerableBool = options.MemberAccessorStrategy.CreateAddDelegateBool<TDeclaredProperty>(addMethod, target);
            }
            else
            {
                return false;
            }

            return true;
        }

        public override void AddObjectToEnumerable(object target, object value)
        {
            if (target is ICollection<TDeclaredProperty> collection)
            {
                Debug.Assert(!collection.IsReadOnly);
                collection.Add((TDeclaredProperty)value);
            }
            else if (target is IList list)
            {
                Debug.Assert(!list.IsReadOnly);
                list.Add(value);
            }
            else if (target is Stack<TDeclaredProperty> stack)
            {
                stack.Push((TDeclaredProperty)value);
            }
            else if (target is Queue<TDeclaredProperty> queue)
            {
                queue.Enqueue((TDeclaredProperty)value);
            }
            else
            {
                AddObjectToEnumerableWithReflection(target, value);
            }
        }

        public override void AddObjectToEnumerableWithReflection(object target, object value)
        {
            if (AddItemToEnumerable != null)
            {
                try
                {
                    AddItemToEnumerable((TDeclaredProperty)value);
                }
                catch (NotSupportedException)
                {
                    throw ThrowHelper.GetNotSupportedException_SerializationNotSupportedCollection(target.GetType(), parentType: null, memberInfo: null);
                }
            }
            else if (AddItemToEnumerableInt32 != null)
            {
                try
                {
                    AddItemToEnumerableInt32((TDeclaredProperty)value);
                }
                catch (NotSupportedException)
                {
                    throw ThrowHelper.GetNotSupportedException_SerializationNotSupportedCollection(target.GetType(), parentType: null, memberInfo: null);
                }
            }
            else if (AddItemToEnumerableBool != null)
            {
                try
                {
                    AddItemToEnumerableBool((TDeclaredProperty)value);
                }
                catch (NotSupportedException)
                {
                    throw ThrowHelper.GetNotSupportedException_SerializationNotSupportedCollection(target.GetType(), parentType: null, memberInfo: null);
                }
            }
            else
            {
                throw ThrowHelper.GetNotSupportedException_SerializationNotSupportedCollection(target.GetType(), parentType: null, memberInfo: null);
            }
        }

        public override void AddObjectToDictionary(object target, string key, object value)
        {
            if (target is IDictionary dict)
            {
                Debug.Assert(!dict.IsReadOnly);
                dict[key] = value;
            }
            else if (target is IDictionary<string, TDeclaredProperty> genericDict)
            {
                Debug.Assert(!genericDict.IsReadOnly);
                genericDict[key] = (TDeclaredProperty)value;
            }
            else
            {
                throw ThrowHelper.GetNotSupportedException_SerializationNotSupportedCollection(target.GetType(), parentType: null, memberInfo: null);
            }
        }

        public override bool CanPopulateEnumerableWithoutReflection(object target)
        {
            if (target is ICollection<TDeclaredProperty> collection && !collection.IsReadOnly)
            {
                return true;
            }
            else if (target is IList list && !list.IsReadOnly)
            {
                return true;
            }
            else if (target is Stack<TDeclaredProperty>)
            {
                return true;
            }
            else if (target is Queue<TDeclaredProperty>)
            {
                return true;
            }

            return false;
        }

        public override bool CanPopulateDictionary(object target)
        {
            if (target is IDictionary<string, TDeclaredProperty> genericDict && !genericDict.IsReadOnly)
            {
                return true;
            }
            else if (target is IDictionary dict && !dict.IsReadOnly)
            {
                foreach (Type @interface in dict.GetType().GetInterfaces())
                {
                    if (!@interface.IsGenericType)
                    {
                        continue;
                    }

                    Type genericDef = @interface.GetGenericTypeDefinition();
                    if (genericDef == typeof(IDictionary<,>) || genericDef == typeof(IReadOnlyDictionary<,>))
                    {
                        if (@interface.GetGenericArguments()[0] != typeof(string))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public override IList CreateConverterList()
        {
            return new List<TDeclaredProperty>();
        }

        // Creates an IEnumerable<TDeclaredPropertyType> and populates it with the items in the
        // sourceList argument then uses the delegateKey argument to identify the appropriate cached
        // CreateRange<TDeclaredPropertyType> method to create and return the desired immutable collection type.
        public override IEnumerable CreateImmutableCollectionInstance(ref ReadStack state, Type collectionType, string delegateKey, IList sourceList, JsonSerializerOptions options)
        {
            IEnumerable collection = null;

            if (!options.TryGetCreateRangeDelegate(delegateKey, out ImmutableCollectionCreator creator) ||
                !creator.CreateImmutableEnumerable(sourceList, out collection))
            {
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(collectionType, state.JsonPath());
            }

            return collection;
        }

        // Creates an IEnumerable<TDeclaredPropertyType> and populates it with the items in the
        // sourceList argument then uses the delegateKey argument to identify the appropriate cached
        // CreateRange<TDeclaredPropertyType> method to create and return the desired immutable collection type.
        public override IDictionary CreateImmutableDictionaryInstance(ref ReadStack state, Type collectionType, string delegateKey, IDictionary sourceDictionary, JsonSerializerOptions options)
        {
            IDictionary collection = null;

            if (!options.TryGetCreateRangeDelegate(delegateKey, out ImmutableCollectionCreator creator) ||
                !creator.CreateImmutableDictionary(sourceDictionary, out collection))
            {
                ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(collectionType, state.JsonPath());
            }

            return collection;
        }
    }
}
