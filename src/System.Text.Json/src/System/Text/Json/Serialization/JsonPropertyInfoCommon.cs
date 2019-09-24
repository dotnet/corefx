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

        public override bool TryCreateDictionaryAddMethod(MethodInfo addMethod, object target, JsonSerializerOptions options)
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

            foreach (Type @interface in target.GetType().GetInterfaces())
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

            try
            {
                if (returnType == typeof(void))
                {
                    AddItemToDictionary = options.MemberAccessorStrategy.CreateAddDelegateForDictionary<TDeclaredProperty>(addMethod, target);
                    return true;
                }
            }
            // Thrown when key type of generic dictionary is not string.
            catch (ArgumentException)
            {
                return false;
            }

            return false;
        }

        public override bool TryCreateExtensionDataAddMethod(MethodInfo addMethod, object target, JsonSerializerOptions options)
        {

            if (addMethod == default)
            {
                return false;
            }

            Type returnType = addMethod.ReturnType;

            try
            {
                if (returnType == typeof(void))
                {
                    AddItemToExtensionData = options.MemberAccessorStrategy.CreateAddDelegateForDictionary<TDeclaredProperty>(addMethod, target);
                    return true;
                }
            }
            // Thrown when key type of generic dictionary is not string.
            catch (ArgumentException e)
            {
                Exception x = e;
                return false;
            }

            return false;
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

        public override void AddValueToEnumerable(object value)
        {
            TDeclaredProperty typedValue = (TDeclaredProperty)value;

            if (AddItemToEnumerable != null)
            {
                AddItemToEnumerable(typedValue);
            }
            else if (AddItemToEnumerableInt32 != null)
            {
                AddItemToEnumerableInt32(typedValue);
            }
            else if (AddItemToEnumerableBool != null)
            {
                AddItemToEnumerableBool(typedValue);
            }
        }

        public override void AddValueToDictionary(string key, object value)
        {
            if (AddItemToDictionary != null)
            {
                try
                {
                    AddItemToDictionary(key, (TDeclaredProperty)value);
                }
                // Handle duplicate keys
                catch (ArgumentException)
                {
                    object target = AddItemToDictionary.Target;

                    if (target is IDictionary<string, TDeclaredProperty> genericDict)
                    {
                        genericDict[key] = (TDeclaredProperty)value;
                    }
                    else if (target is IDictionary dict)
                    {
                        dict[key] = value;
                    }
                }
            }
        }

        public override void AddValueToExtensionData(string key, object value)
        {
            if (AddItemToExtensionData != null)
            {
                try
                {
                    AddItemToExtensionData(key, (TDeclaredProperty)value);
                }
                // Handle duplicate keys
                catch (ArgumentException)
                {
                    object target = AddItemToExtensionData.Target;

                    if (target is IDictionary<string, TDeclaredProperty> genericDict)
                    {
                        genericDict[key] = (TDeclaredProperty)value;
                    }
                    else if (target is IDictionary dict)
                    {
                        dict[key] = value;
                    }
                }
            }
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
