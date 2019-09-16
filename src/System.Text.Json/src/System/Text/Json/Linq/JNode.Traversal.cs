// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace System.Text.Json.Linq
{
    /// <summary>
    ///   The base class that represents a single node within a mutable JSON document.
    /// </summary>
    public abstract partial class JNode
    {
        /// <summary>
        ///   Performs a deep copy operation on <paramref name="jsonElement"/> returning corresponding modifiable tree structure of JSON nodes.
        ///   Operations performed on returned <see cref="JNode"/> does not modify <paramref name="jsonElement"/>.
        /// </summary>
        /// <param name="jsonElement"><see cref="JsonElement"/> to copy.</param>
        /// <returns><see cref="JNode"/>  representing <paramref name="jsonElement"/>.</returns>
        public static JNode DeepCopy(JsonElement jsonElement)
        {
            if (!jsonElement.IsImmutable)
            {
                return GetNode(jsonElement).Clone();
            }

            // Iterative DFS:

            var currentNodes = new Stack<KeyValuePair<string, JNode>>(); // objects/arrays currently being created
            var recursionStack = new Stack<KeyValuePair<string, JsonElement?>>(); // null JsonElement represents end of current object/array
            JNode toReturn = null;

            recursionStack.Push(new KeyValuePair<string, JsonElement?>(null, jsonElement));

            while (recursionStack.Any())
            {
                KeyValuePair<string, JsonElement?> currentPair = recursionStack.Peek();
                JsonElement? currentJsonElement = currentPair.Value;
                recursionStack.Pop();

                if (!currentJsonElement.HasValue)
                {
                    // Current object/array is finished and can be added to its parent:

                    KeyValuePair<string, JNode> nodePair = currentNodes.Peek();
                    currentNodes.Pop();

                    Debug.Assert(nodePair.Value is JArray || nodePair.Value is JObject);

                    AddToParent(nodePair, ref currentNodes, ref toReturn);

                    continue;
                }

                switch (currentJsonElement.Value.ValueKind)
                {
                    case JsonValueKind.Object:
                        var jsonObject = new JObject();

                        // Add jsonObject to current nodes:
                        currentNodes.Push(new KeyValuePair<string, JNode>(currentPair.Key, jsonObject));

                        // Add end of object marker:
                        recursionStack.Push(new KeyValuePair<string, JsonElement?>(null, null));

                        // Add properties to recursion stack. Reverse enumerator to keep properties order:
                        foreach (JsonProperty property in currentJsonElement.Value.EnumerateObject().Reverse())
                        {
                            recursionStack.Push(new KeyValuePair<string, JsonElement?>(property.Name, property.Value));
                        }
                        break;
                    case JsonValueKind.Array:
                        var jsonArray = new JArray();

                        // Add jsonArray to current nodes:
                        currentNodes.Push(new KeyValuePair<string, JNode>(currentPair.Key, jsonArray));

                        // Add end of array marker:
                        recursionStack.Push(new KeyValuePair<string, JsonElement?>(null, null));

                        // Add elements to recursion stack. Reverse enumerator to keep items order:
                        foreach (JsonElement element in currentJsonElement.Value.EnumerateArray().Reverse())
                        {
                            recursionStack.Push(new KeyValuePair<string, JsonElement?>(null, element));
                        }
                        break;
                    case JsonValueKind.Number:
                        var jsonNumber = new JNumber(currentJsonElement.Value.GetRawText());
                        AddToParent(new KeyValuePair<string, JNode>(currentPair.Key, jsonNumber), ref currentNodes, ref toReturn);
                        break;
                    case JsonValueKind.String:
                        var jsonString = new JString(currentJsonElement.Value.GetString());
                        AddToParent(new KeyValuePair<string, JNode>(currentPair.Key, jsonString), ref currentNodes, ref toReturn);
                        break;
                    case JsonValueKind.True:
                        var jsonBooleanTrue = new JBoolean(true);
                        AddToParent(new KeyValuePair<string, JNode>(currentPair.Key, jsonBooleanTrue), ref currentNodes, ref toReturn);
                        break;
                    case JsonValueKind.False:
                        var jsonBooleanFalse = new JBoolean(false);
                        AddToParent(new KeyValuePair<string, JNode>(currentPair.Key, jsonBooleanFalse), ref currentNodes, ref toReturn);
                        break;
                    case JsonValueKind.Null:
                        var jsonNull = new JNull();
                        AddToParent(new KeyValuePair<string, JNode>(currentPair.Key, jsonNull), ref currentNodes, ref toReturn);
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
        ///   Parses a string representing JSON document into <see cref="JNode"/>.
        /// </summary>
        /// <param name="json">JSON to parse.</param>
        /// <param name="options">Options to control the parsing behavior.</param>
        /// <returns><see cref="JNode"/> representation of <paramref name="json"/>.</returns>
        public static JNode Parse(string json, JNodeOptions options = default)
        {
            Utf8JsonReader reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json), options.GetReaderOptions());

            var currentNodes = new Stack<KeyValuePair<string, JNode>>(); // nodes currently being created
            JNode toReturn = null;

            while (reader.Read())
            {
                JsonTokenType tokenType = reader.TokenType;
                KeyValuePair<string, JNode> currentPair = new KeyValuePair<string, JNode>();
                if (currentNodes.Any())
                {
                    currentPair = currentNodes.Peek();
                }

                void AddNewPair(JNode jsonNode, bool keepInCurrentNodes = false)
                {
                    KeyValuePair<string, JNode> newProperty;

                    if (currentPair.Value == null)
                    {
                        // If previous token was property name,
                        // it was added to stack with not null name and null value,
                        // otherwise, this is first JNode added
                        if (currentPair.Key != null)
                        {
                            // Create as property, keep name, replace null with new JNode:
                            currentNodes.Pop();
                            newProperty = new KeyValuePair<string, JNode>(currentPair.Key, jsonNode);
                        }
                        else
                        {
                            // Add first JNode:
                            newProperty = new KeyValuePair<string, JNode>(null, jsonNode);
                        }
                    }
                    else
                    {
                        // Create as value:
                        newProperty = new KeyValuePair<string, JNode>(null, jsonNode);
                    }

                    if (keepInCurrentNodes)
                    {
                        // If after adding property, it should be kept in currentNodes, it must be JsonObject or JsonArray
                        Debug.Assert(jsonNode.ValueKind == JsonValueKind.Object || jsonNode.ValueKind == JsonValueKind.Array);

                        currentNodes.Push(newProperty);
                    }
                    else
                    {
                        AddToParent(newProperty, ref currentNodes, ref toReturn, options.DuplicatePropertyNameHandling);
                    }
                }

                switch (tokenType)
                {
                    case JsonTokenType.StartObject:
                        AddNewPair(new JObject(), true);
                        break;
                    case JsonTokenType.EndObject:
                        Debug.Assert(currentPair.Value is JObject);

                        currentNodes.Pop();
                        AddToParent(currentPair, ref currentNodes, ref toReturn, options.DuplicatePropertyNameHandling);
                        break;
                    case JsonTokenType.StartArray:
                        AddNewPair(new JArray(), true);
                        break;
                    case JsonTokenType.EndArray:
                        Debug.Assert(currentPair.Value is JArray);

                        currentNodes.Pop();
                        AddToParent(currentPair, ref currentNodes, ref toReturn, options.DuplicatePropertyNameHandling);
                        break;
                    case JsonTokenType.PropertyName:
                        currentNodes.Push(new KeyValuePair<string, JNode>(reader.GetString(), null));
                        break;
                    case JsonTokenType.Number:
                        AddNewPair(new JNumber(JsonHelpers.Utf8GetString(reader.ValueSpan)));
                        break;
                    case JsonTokenType.String:
                        AddNewPair(new JString(reader.GetString()));
                        break;
                    case JsonTokenType.True:
                        AddNewPair(new JBoolean(true));
                        break;
                    case JsonTokenType.False:
                        AddNewPair(new JBoolean(false));
                        break;
                    case JsonTokenType.Null:
                        AddNewPair(new JNull());
                        break;
                }
            }

            Debug.Assert(toReturn != null);
            return toReturn;
        }

        /// <summary>
        ///   Writes this instance to provided writer.
        /// </summary>
        /// <param name="writer">Writer to wrtire this instance to.</param>
        public void WriteTo(Utf8JsonWriter writer)
        {
            var recursionStack = new Stack<RecursionStackFrame>();
            recursionStack.Push(new RecursionStackFrame(null, this));

            while (recursionStack.Any())
            {
                RecursionStackFrame currentFrame = recursionStack.Peek();
                recursionStack.Pop();

                if (currentFrame.PropertyValue == null)
                {
                    // Current object/array is finished.
                    // PropertyValue is null, so we need to check ValueKind:

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
                    case JObject jsonObject:
                        writer.WriteStartObject();

                        // Add end of object marker:
                        recursionStack.Push(new RecursionStackFrame(null, null, JsonValueKind.Object));

                        // Add properties to recursion stack. Reverse enumerator to keep properties order:
                        foreach (KeyValuePair<string, JNode> jsonProperty in jsonObject.Reverse())
                        {
                            recursionStack.Push(new RecursionStackFrame(jsonProperty.Key, jsonProperty.Value));
                        }
                        break;
                    case JArray jsonArray:
                        writer.WriteStartArray();

                        // Add end of array marker:
                        recursionStack.Push(new RecursionStackFrame(null, null, JsonValueKind.Array));

                        // Add items to recursion stack. Reverse enumerator to keep items order:
                        foreach (JNode item in jsonArray.Reverse())
                        {
                            recursionStack.Push(new RecursionStackFrame(null, item));
                        }
                        break;
                    case JString jsonString:
                        writer.WriteStringValue(jsonString.Value);
                        break;
                    case JNumber jsonNumber:
                        writer.WriteNumberValue(Encoding.UTF8.GetBytes(jsonNumber.ToString()));
                        break;
                    case JBoolean jsonBoolean:
                        writer.WriteBooleanValue(jsonBoolean.Value);
                        break;
                    case JNull _:
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
