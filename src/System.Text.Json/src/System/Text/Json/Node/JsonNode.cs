// Licensed to the .NET Foundation under one or more agreements.
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
    public abstract class JsonNode
    {
        private protected JsonNode() { }

        /// <summary>
        ///   Transforms this instance into <see cref="JsonElement"/> representation.
        ///   Operations performed on this instance will modify the returned <see cref="JsonElement"/>.
        /// </summary>
        /// <returns>Mutable <see cref="JsonElement"/> with <see cref="JsonNode"/> underneath.</returns>
        public JsonElement AsJsonElement() => new JsonElement(this);

        /// <summary>
        ///   The <see cref="JsonValueKind"/> that the node is.
        /// </summary>
        public abstract JsonValueKind ValueKind { get; }

        /// <summary>
        ///   Gets the <see cref="JsonNode"/> represented by <paramref name="jsonElement"/>.
        ///   Operations performed on the returned <see cref="JsonNode"/> will modify the <paramref name="jsonElement"/>.
        /// </summary>
        /// <param name="jsonElement"><see cref="JsonElement"/> to get the <see cref="JsonNode"/> from.</param>
        /// <returns><see cref="JsonNode"/> represented by <paramref name="jsonElement"/>.</returns>
        public static JsonNode GetNode(JsonElement jsonElement) => (JsonNode)jsonElement._parent;

        /// <summary>
        ///    Gets the <see cref="JsonNode"/> represented by the <paramref name="jsonElement"/>.
        ///    Operations performed on the returned <see cref="JsonNode"/> will modify the <paramref name="jsonElement"/>.
        ///    A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="jsonElement"><see cref="JsonElement"/> to get the <see cref="JsonNode"/> from.</param>
        /// <param name="jsonNode"><see cref="JsonNode"/> represented by <paramref name="jsonElement"/>.</param>
        /// <returns>
        ///  <see langword="true"/> if the operation succeded;
        ///  otherwise, <see langword="false"/>
        /// </returns>
        public static bool TryGetNode(JsonElement jsonElement, out JsonNode jsonNode)
        {
            if (!jsonElement.IsImmutable)
            {
                jsonNode = (JsonNode)jsonElement._parent;
                return true;
            }

            jsonNode = null;
            return false;
        }

        /// <summary>
        ///   Parses a string representiong JSON document into <see cref="JsonNode"/>.
        /// </summary>
        /// <param name="json">JSON to parse.</param>
        /// <returns><see cref="JsonNode"/> representation of <paramref name="json"/>.</returns>
        public static JsonNode Parse(string json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                return DeepCopy(document.RootElement);
            }
        }

        /// <summary>
        ///   Performs a deep copy operation on this instance.
        /// </summary>
        /// <returns><see cref="JsonNode"/> which is a clone of this instance.</returns>
        public abstract JsonNode Clone();

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

            while (!recursionStack.Any())
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
                        foreach (JsonProperty property in currentJsonElement.Value.EnumerateObject())
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
                        foreach (JsonElement element in currentJsonElement.Value.EnumerateArray())
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

        /// <summary>
        ///   Converts a <see cref="string"/> to a <see cref="JsonString"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JsonNode(string value)
        {
            if (value == null)
            {
                return new JsonNull();
            }

            return new JsonString(value);
        }

        /// <summary>
        ///   Converts a <see cref="ReadOnlySpan{Char}"/> to a <see cref="JsonString"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JsonNode(ReadOnlySpan<char> value) => new JsonString(value);

        /// <summary>
        ///   Converts a <see cref="DateTime"/> to a <see cref="JsonString"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JsonNode(DateTime value) => new JsonString(value);

        /// <summary>
        ///   Converts a <see cref="DateTimeOffset"/> to a <see cref="JsonString"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JsonNode(DateTimeOffset value) => new JsonString(value);

        /// <summary>
        ///   Converts a <see cref="Guid"/> to a <see cref="JsonString"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JsonNode(Guid value) => new JsonString(value);

        /// <summary>
        ///   Converts a <see cref="bool"/> to a <see cref="JsonBoolean"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JsonNode(bool value) => new JsonBoolean(value);

        /// <summary>
        ///   Converts a <see cref="byte"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JsonNode(byte value) => new JsonNumber(value);

        /// <summary>
        ///   Converts a <see cref="short"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JsonNode(short value) => new JsonNumber(value);

        /// <summary>
        ///   Converts an <see cref="int"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JsonNode(int value) => new JsonNumber(value);

        /// <summary>
        ///   Converts a <see cref="long"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JsonNode(long value) => new JsonNumber(value);

        /// <summary>
        ///    Converts a <see cref="float"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <exception cref="ArgumentException">
        ///   Provided value is not in a legal JSON number format.
        /// </exception>
        public static implicit operator JsonNode(float value)
        {
            if (float.IsSubnormal(value))
            {
                return new JsonString(value.ToString());
            }

            return new JsonNumber(value);
        }

        /// <summary>
        ///    Converts a <see cref="double"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <exception cref="ArgumentException">
        ///   Provided value is not in a legal JSON number format.
        /// </exception>
        public static implicit operator JsonNode(double value)
        {
            if (double.IsSubnormal(value))
            {
                return new JsonString(value.ToString());
            }

            return new JsonNumber(value);
        }

        /// <summary>
        ///    Converts a <see cref="sbyte"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        [CLSCompliant(false)]
        public static implicit operator JsonNode(sbyte value) => new JsonNumber(value);

        /// <summary>
        ///    Converts a <see cref="ushort"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        [CLSCompliant(false)]
        public static implicit operator JsonNode(ushort value) => new JsonNumber(value);

        /// <summary>
        ///    Converts a <see cref="uint"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        [CLSCompliant(false)]
        public static implicit operator JsonNode(uint value) => new JsonNumber(value);

        /// <summary>
        ///    Converts a <see cref="ulong"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        [CLSCompliant(false)]
        public static implicit operator JsonNode(ulong value) => new JsonNumber(value);

        /// <summary>
        ///    Converts a <see cref="decimal"/> to a JSON number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator JsonNode(decimal value) => new JsonNumber(value);
    }
}
