﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace System.Text.Json
{
    /// <summary>
    ///   The base class that represents a single node within a mutable JSON document.
    /// </summary>
    public abstract partial class JsonNode
    {
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

            void AddToCurrentNodes(KeyValuePair<string, JsonNode> nodePair, ref JsonNode toReturn)
            {
                if (currentNodes.Any())
                {
                    KeyValuePair<string, JsonNode> parentPair = currentNodes.Peek();

                    Debug.Assert(parentPair.Value is JsonArray || parentPair.Value is JsonObject);

                    if (parentPair.Value is JsonObject jsonObject)
                    {
                        Debug.Assert(nodePair.Key != null);
                        jsonObject.Add(nodePair);
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

                    AddToCurrentNodes(nodePair, ref toReturn);

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
                        AddToCurrentNodes(new KeyValuePair<string, JsonNode>(currentPair.Key, jsonNumber), ref toReturn);
                        break;
                    case JsonValueKind.String:
                        var jsonString = new JsonString(currentJsonElement.Value.GetString());
                        AddToCurrentNodes(new KeyValuePair<string, JsonNode>(currentPair.Key, jsonString), ref toReturn);
                        break;
                    case JsonValueKind.True:
                        var jsonBooleanTrue = new JsonBoolean(true);
                        AddToCurrentNodes(new KeyValuePair<string, JsonNode>(currentPair.Key, jsonBooleanTrue), ref toReturn);
                        break;
                    case JsonValueKind.False:
                        var jsonBooleanFalse = new JsonBoolean(false);
                        AddToCurrentNodes(new KeyValuePair<string, JsonNode>(currentPair.Key, jsonBooleanFalse), ref toReturn);
                        break;
                    case JsonValueKind.Null:
                        var jsonNull = new JsonNull();
                        AddToCurrentNodes(new KeyValuePair<string, JsonNode>(currentPair.Key, jsonNull), ref toReturn);
                        break;
                    default:
                        Debug.Assert(jsonElement.ValueKind == JsonValueKind.Undefined, "No handler for JsonValueKind.{jsonElement.ValueKind}");
                        throw ThrowHelper.GetJsonElementWrongTypeException(JsonValueKind.Undefined, jsonElement.ValueKind);
                }
            }

            Debug.Assert(toReturn != null);

            return toReturn;
        }
    }
}
