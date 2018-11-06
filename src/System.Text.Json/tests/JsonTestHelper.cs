﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace System.Text.Json.Tests
{
    internal static class JsonTestHelper
    {
        public static string NewtonsoftReturnStringHelper(TextReader reader)
        {
            var sb = new StringBuilder();
            var json = new JsonTextReader(reader);
            while (json.Read())
            {
                if (json.Value != null)
                {
                    sb.Append(json.Value).Append(", ");
                }
            }
            return sb.ToString();
        }

        public static string WriteDepth(int depth)
        {
            var sb = new StringBuilder();
            var textWriter = new StringWriter();
            var json = new JsonTextWriter(textWriter);
            json.WriteStartObject();
            for (int i = 0; i < depth; i++)
            {
                json.WritePropertyName("message" + i);
                json.WriteStartObject();
            }
            json.WritePropertyName("message" + depth);
            json.WriteValue("Hello, World!");
            for (int i = 0; i < depth; i++)
            {
                json.WriteEndObject();
            }
            json.WriteEndObject();
            json.Flush();

            return textWriter.ToString();
        }

        public static byte[] JsonLabReturnBytesHelper(byte[] data, out int length, JsonCommentHandling commentHandling = JsonCommentHandling.Default)
        {
            var state = new JsonReaderState(commentHandling: commentHandling);
            var reader = new Utf8JsonReader(data, true, state);
            return JsonLabReaderLoop(data.Length, out length, ref reader);
        }

        public static object JsonLabReturnObjectHelper(byte[] data, JsonCommentHandling commentHandling = JsonCommentHandling.Default)
        {
            var state = new JsonReaderState(commentHandling: commentHandling);
            var reader = new Utf8JsonReader(data, true, state);
            return JsonLabReaderLoop(ref reader);
        }

        public static string ObjectToString(object jsonValues)
        {
            string s = "";
            if (jsonValues is List<object> jsonList)
                s = ListToString(jsonList);
            else if (jsonValues is Dictionary<string, object> jsonDictionary)
                s = DictionaryToString(jsonDictionary);
            return s;
        }

        public static string DictionaryToString(Dictionary<string, object> dictionary)
        {
            var builder = new StringBuilder();
            foreach (KeyValuePair<string, object> entry in dictionary)
            {
                if (entry.Value is Dictionary<string, object> nestedDictionary)
                    builder.Append(entry.Key).Append(", ").Append(DictionaryToString(nestedDictionary));
                else if (entry.Value is List<object> nestedList)
                    builder.Append(entry.Key).Append(", ").Append(ListToString(nestedList));
                else
                    builder.Append(entry.Key).Append(", ").Append(entry.Value).Append(", ");
            }
            return builder.ToString();
        }

        public static string ListToString(List<object> list)
        {
            var builder = new StringBuilder();
            foreach (object entry in list)
            {
                if (entry is Dictionary<string, object> nestedDictionary)
                    builder.Append(DictionaryToString(nestedDictionary));
                else if (entry is List<object> nestedList)
                    builder.Append(ListToString(nestedList));
                else
                    builder.Append(entry).Append(", ");
            }
            return builder.ToString();
        }

        public static byte[] JsonLabReaderLoop(int inpuDataLength, out int length, ref Utf8JsonReader json)
        {
            byte[] outputArray = new byte[inpuDataLength];
            Span<byte> destination = outputArray;

            while (json.Read())
            {
                JsonTokenType tokenType = json.TokenType;
                ReadOnlySpan<byte> valueSpan = json.IsValueMultiSegment ? json.ValueSequence.ToArray() : json.ValueSpan;
                switch (tokenType)
                {
                    case JsonTokenType.PropertyName:
                        valueSpan.CopyTo(destination);
                        destination[valueSpan.Length] = (byte)',';
                        destination[valueSpan.Length + 1] = (byte)' ';
                        destination = destination.Slice(valueSpan.Length + 2);
                        break;
                    case JsonTokenType.Number:
                    case JsonTokenType.String:
                    case JsonTokenType.Comment:
                        valueSpan.CopyTo(destination);
                        destination[valueSpan.Length] = (byte)',';
                        destination[valueSpan.Length + 1] = (byte)' ';
                        destination = destination.Slice(valueSpan.Length + 2);
                        break;
                    case JsonTokenType.True:
                        // Special casing True/False so that the casing matches with Json.NET
                        destination[0] = (byte)'T';
                        destination[1] = (byte)'r';
                        destination[2] = (byte)'u';
                        destination[3] = (byte)'e';
                        destination[valueSpan.Length] = (byte)',';
                        destination[valueSpan.Length + 1] = (byte)' ';
                        destination = destination.Slice(valueSpan.Length + 2);
                        break;
                    case JsonTokenType.False:
                        destination[0] = (byte)'F';
                        destination[1] = (byte)'a';
                        destination[2] = (byte)'l';
                        destination[3] = (byte)'s';
                        destination[4] = (byte)'e';
                        destination[valueSpan.Length] = (byte)',';
                        destination[valueSpan.Length + 1] = (byte)' ';
                        destination = destination.Slice(valueSpan.Length + 2);
                        break;
                    case JsonTokenType.Null:
                        // Special casing Null so that it matches what JSON.NET does
                        break;
                    default:
                        break;
                }
            }
            length = outputArray.Length - destination.Length;
            return outputArray;
        }

        public static object JsonLabReaderLoop(ref Utf8JsonReader json)
        {
            object root = null;

            while (json.Read())
            {
                JsonTokenType tokenType = json.TokenType;
                ReadOnlySpan<byte> valueSpan = json.ValueSpan;
                switch (tokenType)
                {
                    case JsonTokenType.True:
                    case JsonTokenType.False:
                        root = valueSpan[0] == 't';
                        break;
                    case JsonTokenType.Number:
                        json.TryGetValueAsDouble(out double valueDouble);
                        root = valueDouble;
                        break;
                    case JsonTokenType.String:
                        json.TryGetValueAsString(out string valueString);
                        root = valueString;
                        break;
                    case JsonTokenType.Null:
                        break;
                    case JsonTokenType.StartObject:
                        root = JsonLabReaderDictionaryLoop(ref json);
                        break;
                    case JsonTokenType.StartArray:
                        root = JsonLabReaderListLoop(ref json);
                        break;
                    case JsonTokenType.EndObject:
                    case JsonTokenType.EndArray:
                        break;
                    case JsonTokenType.None:
                    case JsonTokenType.Comment:
                    default:
                        break;
                }
            }
            return root;
        }

        public static Dictionary<string, object> JsonLabReaderDictionaryLoop(ref Utf8JsonReader json)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            string key = "";
            object value = null;

            while (json.Read())
            {
                JsonTokenType tokenType = json.TokenType;
                ReadOnlySpan<byte> valueSpan = json.ValueSpan;
                switch (tokenType)
                {
                    case JsonTokenType.PropertyName:
                        json.TryGetValueAsString(out key);
                        dictionary.Add(key, null);
                        break;
                    case JsonTokenType.True:
                    case JsonTokenType.False:
                        value = valueSpan[0] == 't';
                        if (dictionary.TryGetValue(key, out _))
                        {
                            dictionary[key] = value;
                        }
                        else
                        {
                            dictionary.Add(key, value);
                        }
                        break;
                    case JsonTokenType.Number:
                        json.TryGetValueAsDouble(out double valueDouble);
                        if (dictionary.TryGetValue(key, out _))
                        {
                            dictionary[key] = valueDouble;
                        }
                        else
                        {
                            dictionary.Add(key, valueDouble);
                        }
                        break;
                    case JsonTokenType.String:
                        json.TryGetValueAsString(out string valueString);
                        if (dictionary.TryGetValue(key, out _))
                        {
                            dictionary[key] = valueString;
                        }
                        else
                        {
                            dictionary.Add(key, valueString);
                        }
                        break;
                    case JsonTokenType.Null:
                        value = null;
                        if (dictionary.TryGetValue(key, out _))
                        {
                            dictionary[key] = value;
                        }
                        else
                        {
                            dictionary.Add(key, value);
                        }
                        break;
                    case JsonTokenType.StartObject:
                        value = JsonLabReaderDictionaryLoop(ref json);
                        if (dictionary.TryGetValue(key, out _))
                        {
                            dictionary[key] = value;
                        }
                        else
                        {
                            dictionary.Add(key, value);
                        }
                        break;
                    case JsonTokenType.StartArray:
                        value = JsonLabReaderListLoop(ref json);
                        if (dictionary.TryGetValue(key, out _))
                        {
                            dictionary[key] = value;
                        }
                        else
                        {
                            dictionary.Add(key, value);
                        }
                        break;
                    case JsonTokenType.EndObject:
                        return dictionary;
                    case JsonTokenType.None:
                    case JsonTokenType.Comment:
                    default:
                        break;
                }
            }
            return dictionary;
        }

        public static List<object> JsonLabReaderListLoop(ref Utf8JsonReader json)
        {
            List<object> arrayList = new List<object>();

            object value = null;

            while (json.Read())
            {
                JsonTokenType tokenType = json.TokenType;
                ReadOnlySpan<byte> valueSpan = json.ValueSpan;
                switch (tokenType)
                {
                    case JsonTokenType.True:
                    case JsonTokenType.False:
                        value = valueSpan[0] == 't';
                        arrayList.Add(value);
                        break;
                    case JsonTokenType.Number:
                        json.TryGetValueAsDouble(out double doubleValue);
                        arrayList.Add(doubleValue);
                        break;
                    case JsonTokenType.String:
                        json.TryGetValueAsString(out string valueString);
                        arrayList.Add(valueString);
                        break;
                    case JsonTokenType.Null:
                        value = null;
                        arrayList.Add(value);
                        break;
                    case JsonTokenType.StartObject:
                        value = JsonLabReaderDictionaryLoop(ref json);
                        arrayList.Add(value);
                        break;
                    case JsonTokenType.StartArray:
                        value = JsonLabReaderListLoop(ref json);
                        arrayList.Add(value);
                        break;
                    case JsonTokenType.EndArray:
                        return arrayList;
                    case JsonTokenType.None:
                    case JsonTokenType.Comment:
                    default:
                        break;
                }
            }
            return arrayList;
        }
    }
}
