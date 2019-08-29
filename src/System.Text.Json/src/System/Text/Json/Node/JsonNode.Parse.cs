// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace System.Text.Json
{
    /// <summary>
    ///   The base class that represents a single node within a mutable JSON document.
    /// </summary>
    public abstract partial class JsonNode
    {
        private static void AddToParent(
            KeyValuePair<string, JsonNode> nodePair,
            ref Stack<KeyValuePair<string, JsonNode>> currentNodes,
            ref JsonNode toReturn,
            DuplicatePropertyNameHandling duplicatePropertyNameHandling = DuplicatePropertyNameHandling.Replace)
        {
            if (currentNodes.Any())
            {
                KeyValuePair<string, JsonNode> parentPair = currentNodes.Peek();

                Debug.Assert(parentPair.Value is JsonArray || parentPair.Value is JsonObject);

                if (parentPair.Value is JsonObject jsonObject)
                {
                    Debug.Assert(nodePair.Key != null);
                    if (jsonObject._dictionary.ContainsKey(nodePair.Key))
                    {
                        switch (duplicatePropertyNameHandling)
                        {
                            case DuplicatePropertyNameHandling.Replace:
                                jsonObject[nodePair.Key] = nodePair.Value;
                                break;
                            case DuplicatePropertyNameHandling.Error:
                                throw new ArgumentException(SR.JsonObjectDuplicateKey);
                            case DuplicatePropertyNameHandling.Ignore:
                                break;
                        }
                    }
                    else
                    {
                        jsonObject.Add(nodePair);
                    }
                }
                else if (parentPair.Value is JsonArray jsonArray)
                {
                    Debug.Assert(nodePair.Key == null);
                    jsonArray.Add(nodePair.Value);
                }
            }
            else
            {
                toReturn = nodePair.Value;
            }
        }

        /// <summary>
        ///   Performs a deep copy operation on <paramref name="jsonElement"/> returning corresponding modifiable tree structure of JSON nodes.
        ///   Operations performed on returned <see cref="JsonNode"/> does not modify <paramref name="jsonElement"/>.
        /// </summary>
        /// <param name="jsonElement"><see cref="JsonElement"/> to copy.</param>
        /// <returns><see cref="JsonNode"/>  representing <paramref name="jsonElement"/>.</returns>
        public static JsonNode DeepCopy(JsonElement jsonElement)
        {
            if (!jsonElement.IsImmutable)
            {
                return GetNode(jsonElement).Clone();
            }

            // Iterative DFS:

            var currentNodes = new Stack<KeyValuePair<string, JsonNode>>(); // objects/arrays currently being created
            var recursionStack = new Stack<KeyValuePair<string, JsonElement?>>(); // null JsonElement represents end of current object/array
            JsonNode toReturn = null;

            recursionStack.Push(new KeyValuePair<string, JsonElement?>(null, jsonElement));

            while (recursionStack.Any())
            {
                KeyValuePair<string, JsonElement?> currentPair = recursionStack.Peek();
                JsonElement? currentJsonElement = currentPair.Value;
                recursionStack.Pop();

                if (!currentJsonElement.HasValue)
                {
                    // Current object/array is finished and can be added to its parent:

                    KeyValuePair<string, JsonNode> nodePair = currentNodes.Peek();
                    currentNodes.Pop();

                    Debug.Assert(nodePair.Value is JsonArray || nodePair.Value is JsonObject);

                    AddToParent(nodePair, ref currentNodes, ref toReturn);

                    continue;
                }

                switch (currentJsonElement.Value.ValueKind)
                {
                    case JsonValueKind.Object:
                        var jsonObject = new JsonObject();

                        // Add jsonObject to current nodes:
                        currentNodes.Push(new KeyValuePair<string, JsonNode>(currentPair.Key, jsonObject));

                        // Add end of object marker:
                        recursionStack.Push(new KeyValuePair<string, JsonElement?> (null, null));

                        // Add properties to recursion stack:
                        foreach (JsonProperty property in currentJsonElement.Value.EnumerateObject().Reverse())
                        {
                            recursionStack.Push(new KeyValuePair<string, JsonElement?>(property.Name, property.Value));
                        }
                        break;
                    case JsonValueKind.Array:
                        var jsonArray = new JsonArray();

                        // Add jsonArray to current nodes:
                        currentNodes.Push(new KeyValuePair<string, JsonNode>(currentPair.Key, jsonArray));

                        // Add end of array marker:
                        recursionStack.Push(new KeyValuePair<string, JsonElement?>(null, null));

                        // Add elements to recursion stack:
                        foreach (JsonElement element in currentJsonElement.Value.EnumerateArray().Reverse())
                        {
                            recursionStack.Push(new KeyValuePair<string, JsonElement?>(null, element));
                        }
                        break;
                    case JsonValueKind.Number:
                        var jsonNumber = new JsonNumber(currentJsonElement.Value.GetRawText());
                        AddToParent(new KeyValuePair<string, JsonNode>(currentPair.Key, jsonNumber), ref currentNodes, ref toReturn);
                        break;
                    case JsonValueKind.String:
                        var jsonString = new JsonString(currentJsonElement.Value.GetString());
                        AddToParent(new KeyValuePair<string, JsonNode>(currentPair.Key, jsonString), ref currentNodes, ref toReturn);
                        break;
                    case JsonValueKind.True:
                        var jsonBooleanTrue = new JsonBoolean(true);
                        AddToParent(new KeyValuePair<string, JsonNode>(currentPair.Key, jsonBooleanTrue), ref currentNodes, ref toReturn);
                        break;
                    case JsonValueKind.False:
                        var jsonBooleanFalse = new JsonBoolean(false);
                        AddToParent(new KeyValuePair<string, JsonNode>(currentPair.Key, jsonBooleanFalse), ref currentNodes, ref toReturn);
                        break;
                    case JsonValueKind.Null:
                        var jsonNull = new JsonNull();
                        AddToParent(new KeyValuePair<string, JsonNode>(currentPair.Key, jsonNull), ref currentNodes, ref toReturn);
                        break;
                    default:
                        Debug.Assert(jsonElement.ValueKind == JsonValueKind.Undefined, "No handler for JsonValueKind.{jsonElement.ValueKind}");
                        throw ThrowHelper.GetJsonElementWrongTypeException(JsonValueKind.Undefined, jsonElement.ValueKind);
                }
            }

            Debug.Assert(toReturn != null);

            return toReturn;
        }

        /// <summary>
        ///   Parses a string representiong JSON document into <see cref="JsonNode"/>.
        /// </summary>
        /// <param name="json">JSON to parse.</param>
        /// <param name="options">Options to control the reader behavior during parsing.</param>
        /// <param name="duplicatePropertyNameHandling">Specifies the way of handling duplicate property names.</param>
        /// <returns><see cref="JsonNode"/> representation of <paramref name="json"/>.</returns>
        public static JsonNode Parse(string json, JsonReaderOptions options = default, DuplicatePropertyNameHandling duplicatePropertyNameHandling = DuplicatePropertyNameHandling.Replace)
        {
            Utf8JsonReader reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json), options);

            var currentNodes = new Stack<KeyValuePair<string, JsonNode>>(); // nodes currently being created
            JsonNode toReturn = null;

            while (reader.Read())
            {
                JsonTokenType tokenType = reader.TokenType;
                KeyValuePair<string, JsonNode> currentPair = new KeyValuePair<string, JsonNode>();
                if (currentNodes.Any())
                {
                    currentPair = currentNodes.Peek();
                }

                void AddNewPair(JsonNode jsonNode, bool keepInCurrentNodes = false)
                {
                    KeyValuePair<string, JsonNode> newProperty;

                    if (currentPair.Value == null)
                    {
                        // If previous token was property name,
                        // it was added to stack with not null name and null value,
                        // otherwise, this is first property added
                        if (currentPair.Key != null)
                        {
                            // Create as property, keep name, replace null with new JsonNode:
                            currentNodes.Pop();
                            newProperty = new KeyValuePair<string, JsonNode>(currentPair.Key, jsonNode);
                        }
                        else
                        {
                            newProperty = new KeyValuePair<string, JsonNode>(null, jsonNode);
                        }
                    }
                    else
                    {
                        // Create as value:
                        newProperty = new KeyValuePair<string, JsonNode>(null, jsonNode);
                    }

                    if (keepInCurrentNodes)
                    {
                        currentNodes.Push(newProperty);
                    }
                    else
                    {
                        AddToParent(newProperty, ref currentNodes, ref toReturn, duplicatePropertyNameHandling);
                    }
                }

                switch (tokenType)
                {
                    case JsonTokenType.StartObject:
                        AddNewPair(new JsonObject(), true);
                        break;
                    case JsonTokenType.EndObject:
                        Debug.Assert(currentPair.Value is JsonObject);

                        currentNodes.Pop();
                        AddToParent(currentPair, ref currentNodes, ref toReturn, duplicatePropertyNameHandling);
                        break;
                    case JsonTokenType.StartArray:
                        AddNewPair(new JsonArray(), true);
                        break;
                    case JsonTokenType.EndArray:
                        Debug.Assert(currentPair.Value is JsonArray);

                        currentNodes.Pop();
                        AddToParent(currentPair, ref currentNodes, ref toReturn, duplicatePropertyNameHandling);
                        break;
                    case JsonTokenType.PropertyName:
                        currentNodes.Push(new KeyValuePair<string, JsonNode>(reader.GetString(), null));
                        break;
                    case JsonTokenType.Number:
                        AddNewPair(new JsonNumber(JsonHelpers.Utf8GetString(reader.ValueSpan)));
                        break;
                    case JsonTokenType.String:
                        AddNewPair(new JsonString(reader.GetString()));
                        break;
                    case JsonTokenType.True:
                        AddNewPair(new JsonBoolean(true));
                        break;
                    case JsonTokenType.False:
                        AddNewPair(new JsonBoolean(false));
                        break;
                    case JsonTokenType.Null:
                        AddNewPair(new JsonNull());
                        break;
                }
            }

            Debug.Assert(toReturn != null);
            return toReturn;
        }

        private struct StackFrame
        {
            public string PropertyName { get; set; }
            public JsonNode PropertyValue { get; set; }
            public JsonValueKind ValueKind { get; set; }

            public StackFrame(string propertyName, JsonNode propertyValue, JsonValueKind valueKind)
            {
                PropertyName = propertyName;
                PropertyValue = propertyValue;
                ValueKind = valueKind;
            }

            public StackFrame(string propertyName, JsonNode propertyValue) : this(propertyName, propertyValue, propertyValue.ValueKind)
            {
            }
        }

        internal void WriteTo(Utf8JsonWriter writer)
        {
            var recursionStack = new Stack<StackFrame>();
            recursionStack.Push(new StackFrame(null, this));

            while (recursionStack.Any())
            {
                StackFrame currentFrame = recursionStack.Peek();
                recursionStack.Pop();

                if (currentFrame.PropertyValue == null)
                {
                    // Current object/array is finished:
                    Debug.Assert(currentFrame.ValueKind == JsonValueKind.Object || currentFrame.ValueKind == JsonValueKind.Array);

                    if (currentFrame.ValueKind == JsonValueKind.Object)
                    {
                        writer.WriteEndObject();
                    }
                    if (currentFrame.ValueKind == JsonValueKind.Array)
                    {
                        writer.WriteEndArray();
                    }

                    continue;
                }

                if (currentFrame.PropertyName != null)
                {
                    writer.WritePropertyName(currentFrame.PropertyName);
                }

                switch (currentFrame.PropertyValue)
                {
                    case JsonObject jsonObject:
                        writer.WriteStartObject();

                        // Add end of object marker:
                        recursionStack.Push(new StackFrame(null, null, JsonValueKind.Object));

                        foreach (KeyValuePair<string, JsonNode> jsonProperty in jsonObject.Reverse())
                        {
                            recursionStack.Push(new StackFrame(jsonProperty.Key, jsonProperty.Value));
                        }
                        break;
                    case JsonArray jsonArray:
                        writer.WriteStartArray();

                        // Add end of array marker:
                        recursionStack.Push(new StackFrame(null, null, JsonValueKind.Array));

                        foreach (JsonNode item in jsonArray.Reverse())
                        {
                            recursionStack.Push(new StackFrame(null, item));
                        }
                        break;
                    case JsonString jsonString:
                        writer.WriteStringValue(jsonString.Value);
                        break;
                    case JsonNumber jsonNumber:
                        writer.WriteNumberValue(Encoding.UTF8.GetBytes(jsonNumber.ToString()));
                        break;
                    case JsonBoolean jsonBoolean:
                        writer.WriteBooleanValue(jsonBoolean.Value);
                        break;
                    case JsonNull _:
                        writer.WriteNullValue();
                        break;
                }

                writer.Flush();
            }

            writer.Flush();
        }

        /// <summary>
        ///   Converts the current instance to string in JSON format.
        /// </summary>
        /// <returns>JSON representation of current instance.</returns>
        public string ToJsonString()
        {
            var stream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(stream))
            {
                WriteTo(writer);
                return JsonHelpers.Utf8GetString(stream.ToArray());
            }
        }
    }
}
