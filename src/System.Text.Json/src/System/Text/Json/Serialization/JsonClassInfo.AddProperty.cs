// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization.Converters;

namespace System.Text.Json.Serialization
{
#if MAKE_UNREVIEWED_APIS_INTERNAL
    internal
#else
    public
#endif
    partial class JsonClassInfo
    {
        private void AddProperty(Type propertyType, PropertyInfo propertyInfo, Type classType, JsonSerializerOptions options)
        {
            JsonPropertyInfo jsonInfo = null;
            Type collectionElementType = null;

            ClassType propertyClassType = GetClassType(propertyType);

            if (propertyClassType == ClassType.Value)
            {
                bool isNullable = (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>));

                if (!isNullable)
                {
                    // More common types are listed first.
                    if (propertyType == typeof(string))
                    {
                        jsonInfo = new JsonPropertyInfoString(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(int))
                    {
                        jsonInfo = new JsonPropertyInfoInt32(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(bool))
                    {
                        jsonInfo = new JsonPropertyInfoBoolean(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(decimal))
                    {
                        jsonInfo = new JsonPropertyInfoDecimal(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(DateTime))
                    {
                        jsonInfo = new JsonPropertyInfoDateTime(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(double))
                    {
                        jsonInfo = new JsonPropertyInfoDouble(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(long))
                    {
                        jsonInfo = new JsonPropertyInfoInt64(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(short))
                    {
                        jsonInfo = new JsonPropertyInfoInt16(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(float))
                    {
                        jsonInfo = new JsonPropertyInfoSingle(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(byte))
                    {
                        jsonInfo = new JsonPropertyInfoByte(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(char))
                    {
                        jsonInfo = new JsonPropertyInfoChar(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(ushort))
                    {
                        jsonInfo = new JsonPropertyInfoUInt16(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(uint))
                    {
                        jsonInfo = new JsonPropertyInfoUInt32(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(ulong))
                    {
                        jsonInfo = new JsonPropertyInfoUInt64(classType, propertyType, propertyInfo, options);
                    }
                }
                else
                { 
                    // More common types are listed first.
                    if (propertyType == typeof(int?))
                    {
                        jsonInfo = new JsonPropertyInfoInt32Nullable(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(bool?))
                    {
                        jsonInfo = new JsonPropertyInfoBooleanNullable(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(decimal?))
                    {
                        jsonInfo = new JsonPropertyInfoDecimalNullable(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(DateTime?))
                    {
                        jsonInfo = new JsonPropertyInfoDateTimeNullable(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(double?))
                    {
                        jsonInfo = new JsonPropertyInfoDoubleNullable(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(long?))
                    {
                        jsonInfo = new JsonPropertyInfoInt64Nullable(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(short?))
                    {
                        jsonInfo = new JsonPropertyInfoInt16Nullable(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(float?))
                    {
                        jsonInfo = new JsonPropertyInfoSingleNullable(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(byte?))
                    {
                        jsonInfo = new JsonPropertyInfoByteNullable(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(char?))
                    {
                        jsonInfo = new JsonPropertyInfoCharNullable(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(ushort?))
                    {
                        jsonInfo = new JsonPropertyInfoUInt16Nullable(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(uint?))
                    {
                        jsonInfo = new JsonPropertyInfoUInt32Nullable(classType, propertyType, propertyInfo, options);
                    }
                    else if (propertyType == typeof(ulong?))
                    {
                        jsonInfo = new JsonPropertyInfoUInt64Nullable(classType, propertyType, propertyInfo, options);
                    }
                }
            }
            else if (propertyClassType == ClassType.Enumerable)
            {
                collectionElementType = GetElementType(propertyType);
                // todo: if collectionElementType is object, create loosely-typed collection (JsonArray).
            }

            // Handle the general case (slower)
            if (jsonInfo == null)
            {
                Type genericPropertyType = typeof(JsonPropertyInfo<>).MakeGenericType(propertyType);
                jsonInfo = (JsonPropertyInfo)Activator.CreateInstance(
                    genericPropertyType,
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    binder: null,
                    new object[] { classType, propertyType, propertyInfo, options, collectionElementType },
                    culture: null);
            }

            if (propertyInfo != null)
            {
                string propertyName = jsonInfo.NameConverter == null ? propertyInfo.Name : jsonInfo.NameConverter.Write(propertyInfo.Name);

                // At this point propertyName is valid UTF16, so just call the simple UTF16->UTF8 encoder.
                byte[] propertyNameBytes = Encoding.UTF8.GetBytes(propertyName);
                jsonInfo._name = propertyNameBytes;

                // Cache the escaped name.
                int valueIdx = JsonWriterHelper.NeedsEscaping(propertyNameBytes);
                if (valueIdx == -1)
                {
                    jsonInfo._escapedName = propertyNameBytes;
                }
                else
                {
                    int length = JsonWriterHelper.GetMaxEscapedLength(propertyNameBytes.Length, valueIdx);

                    byte[] tempArray = ArrayPool<byte>.Shared.Rent(length);

                    JsonWriterHelper.EscapeString(propertyNameBytes, tempArray, valueIdx, out int written);
                    jsonInfo._escapedName = new byte[written];
                    tempArray.CopyTo(jsonInfo._escapedName, 0);

                    ArrayPool<byte>.Shared.Return(tempArray);
                }

                _property_refs.Add(new PropertyRef(GetKey(propertyNameBytes), jsonInfo));
            }
            else
            {
                // A single property or an IEnumerable
                _property_refs.Add(new PropertyRef(0, jsonInfo));
            }
        }
    }
}
