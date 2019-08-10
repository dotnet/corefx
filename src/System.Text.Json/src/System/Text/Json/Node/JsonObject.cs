// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;

namespace System.Text.Json
{
    /// <summary>
    ///  Represents a JSON object.
    /// </summary>
    public sealed class JsonObject : JsonNode, IEnumerable<KeyValuePair<string, JsonNode>>
    {
        private readonly Dictionary<string, JsonNode> _dictionary;
        private readonly DuplicatePropertyNameHandling _duplicatePropertyNameHandling;

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonObject"/> class representing the empty object.
        /// </summary>
        /// <param name="duplicatePropertyNameHandling">Specifies the way of handling duplicate property names.</param>
        public JsonObject(DuplicatePropertyNameHandling duplicatePropertyNameHandling = default)
        {
            _dictionary = new Dictionary<string, JsonNode>();
            _duplicatePropertyNameHandling = duplicatePropertyNameHandling;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonObject"/> class representing provided set of JSON properties.
        /// </summary>
        /// <param name="jsonProperties">>Properties to represent as a JSON object.</param>
        /// <param name="duplicatePropertyNameHandling">Specifies the way of handling duplicate property names.</param>
        /// <exception cref="ArgumentException">
        ///   Provided collection contains duplicates if handling duplicates is set to <see cref="DuplicatePropertyNameHandling.Error"/>.
        /// </exception>
        public JsonObject(IEnumerable<KeyValuePair<string, JsonNode>> jsonProperties, DuplicatePropertyNameHandling duplicatePropertyNameHandling = default)
            : this(duplicatePropertyNameHandling)
            => AddRange(jsonProperties);

        /// <summary>
        ///   Gets or sets the value of the specified property.
        /// </summary>
        /// <param name="propertyName">The property name of the value to get or set.</param>
        public JsonNode this[string propertyName]
        {
            get => GetProperty(propertyName);
            set => _dictionary[propertyName] = value;
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the JSON object properties.
        /// </summary>
        /// <returns>An enumerator structure for the JSON object.</returns>
        /// <exception cref="ArgumentException">
        ///   Property name to set already exists if handling duplicates is set to <see cref="DuplicatePropertyNameHandling.Error"/>.
        /// </exception>
        public IEnumerator<KeyValuePair<string, JsonNode>> GetEnumerator() => _dictionary.GetEnumerator();

        /// <summary>
        ///   Adds the specified property to the JSON object.
        /// </summary>
        /// <param name="jsonProperty">The property to add.</param>
        /// <exception cref="ArgumentException">
        ///   Property name to set already exists if handling duplicates is set to <see cref="DuplicatePropertyNameHandling.Error"/>.
        /// </exception>
        public void Add(KeyValuePair<string, JsonNode> jsonProperty) => Add(jsonProperty.Key, jsonProperty.Value);

        /// <summary>
        ///   Adds the specified <see cref="JsonNode"/> property to the JSON object.
        /// </summary>
        /// <param name="propertyName">Name of the property to add.</param>
        /// <param name="propertyValue">Value of the property to add.</param>
        /// <exception cref="ArgumentException">
        ///   Property name to set already exists if handling duplicates is set to <see cref="DuplicatePropertyNameHandling.Error"/>.
        /// </exception>
        public void Add(string propertyName, JsonNode propertyValue)
        {
            if (_duplicatePropertyNameHandling == DuplicatePropertyNameHandling.Replace)
            {
                _dictionary[propertyName] = propertyValue;
                return;
            }

            if (!_dictionary.TryAdd(propertyName, propertyValue))
            {
                switch (_duplicatePropertyNameHandling)
                {
                    case DuplicatePropertyNameHandling.Ignore:
                        return;
                    case DuplicatePropertyNameHandling.Error:
                        throw new ArgumentException(SR.JsonObjectDuplicateKey);
                }
            }
        }

        /// <summary>
        ///   Adds the specified property as a <see cref="JsonString"/> to the JSON object.
        /// </summary>
        /// <param name="propertyName">Name of the property to add.</param>
        /// <param name="propertyValue"><see cref="string"/> value of the property to add.</param>
        /// <exception cref="ArgumentException">
        ///   Property name to set already exists if handling duplicates is set to <see cref="DuplicatePropertyNameHandling.Error"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   Provided value is null.
        /// </exception>
        public void Add(string propertyName, string propertyValue) => _dictionary.Add(propertyName, new JsonString(propertyValue));

        /// <summary>
        ///   Adds the specified property as a <see cref="JsonBoolean"/> to the JSON object.
        /// </summary>
        /// <param name="propertyName">Name of the property to add.</param>
        /// <param name="propertyValue"><see cref="string"/> value of the property to add.</param>
        /// <exception cref="ArgumentException">
        ///   Property name to set already exists if handling duplicates is set to <see cref="DuplicatePropertyNameHandling.Error"/>.
        /// </exception>
        public void Add(string propertyName, bool propertyValue) => _dictionary.Add(propertyName, new JsonBoolean(propertyValue));

        /// <summary>
        ///   Adds the specified property as a <see cref="JsonNumber"/> to the JSON object.
        /// </summary>
        /// <param name="propertyName">Name of the property to add.</param>
        /// <param name="propertyValue"><see cref="byte"/> value of the property to add.</param>
        /// <exception cref="ArgumentException">
        ///   Property name to set already exists if handling duplicates is set to <see cref="DuplicatePropertyNameHandling.Error"/>.
        /// </exception>
        public void Add(string propertyName, byte propertyValue) => _dictionary.Add(propertyName, new JsonNumber(propertyValue));

        /// <summary>
        ///   Adds the specified property as a <see cref="JsonNumber"/> to the JSON object.
        /// </summary>
        /// <param name="propertyName">Name of the property to add.</param>
        /// <param name="propertyValue"><see cref="short"/> value of the property to add.</param>
        /// <exception cref="ArgumentException">
        ///   Property name to set already exists if handling duplicates is set to <see cref="DuplicatePropertyNameHandling.Error"/>.
        /// </exception>
        public void Add(string propertyName, short propertyValue) => _dictionary.Add(propertyName, new JsonNumber(propertyValue));

        /// <summary>
        ///   Adds the specified property as a <see cref="JsonNumber"/> to the JSON object.
        /// </summary>
        /// <param name="propertyName">Name of the property to add.</param>
        /// <param name="propertyValue"><see cref="int"/> value of the property to add.</param>
        /// <exception cref="ArgumentException">
        ///   Property name to set already exists if handling duplicates is set to <see cref="DuplicatePropertyNameHandling.Error"/>.
        /// </exception>
        public void Add(string propertyName, int propertyValue) => _dictionary.Add(propertyName, new JsonNumber(propertyValue));

        /// <summary>
        ///   Adds the specified property as a <see cref="JsonNumber"/> to the JSON object.
        /// </summary>
        /// <param name="propertyName">Name of the property to add.</param>
        /// <param name="propertyValue"><see cref="long"/> value of the property to add.</param>
        /// <exception cref="ArgumentException">
        ///   Property name to set already exists if handling duplicates is set to <see cref="DuplicatePropertyNameHandling.Error"/>.
        /// </exception>
        public void Add(string propertyName, long propertyValue) => _dictionary.Add(propertyName, new JsonNumber(propertyValue));

        /// <summary>
        ///   Adds the specified property as a <see cref="JsonNumber"/> to the JSON object.
        /// </summary>
        /// <param name="propertyName">Name of the property to add.</param>
        /// <param name="propertyValue"><see cref="float"/> value of the property to add.</param>
        /// <exception cref="ArgumentException">
        ///   Property name to set already exists if handling duplicates is set to <see cref="DuplicatePropertyNameHandling.Error"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Provided value is not in a legal JSON number format.
        /// </exception>
        public void Add(string propertyName, float propertyValue) => _dictionary.Add(propertyName, new JsonNumber(propertyValue));

        /// <summary>
        ///   Adds the specified property as a <see cref="JsonNumber"/> to the JSON object.
        /// </summary>
        /// <param name="propertyName">Name of the property to add.</param>
        /// <param name="propertyValue"><see cref="double"/> value of the property to add.</param>
        /// <exception cref="ArgumentException">
        ///   Property name to set already exists if handling duplicates is set to <see cref="DuplicatePropertyNameHandling.Error"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Provided value is not in a legal JSON number format.
        /// </exception>
        public void Add(string propertyName, double propertyValue) => _dictionary.Add(propertyName, new JsonNumber(propertyValue));

        /// <summary>
        ///   Adds the specified property as a <see cref="JsonNumber"/> to the JSON object.
        /// </summary>
        /// <param name="propertyName">Name of the property to add.</param>
        /// <param name="propertyValue"><see cref="sbyte"/> value of the property to add.</param>
        /// <exception cref="ArgumentException">
        ///   Property name to set already exists if handling duplicates is set to <see cref="DuplicatePropertyNameHandling.Error"/>.
        /// </exception>
        [CLSCompliant(false)]
        public void Add(string propertyName, sbyte propertyValue) => _dictionary.Add(propertyName, new JsonNumber(propertyValue));

        /// <summary>
        ///   Adds the specified property as a <see cref="JsonNumber"/> to the JSON object.
        /// </summary>
        /// <param name="propertyName">Name of the property to add.</param>
        /// <param name="propertyValue"><see cref="ushort"/> value of the property to add.</param>
        /// <exception cref="ArgumentException">
        ///   Property name to set already exists if handling duplicates is set to <see cref="DuplicatePropertyNameHandling.Error"/>.
        /// </exception>
        [CLSCompliant(false)]
        public void Add(string propertyName, ushort propertyValue) => _dictionary.Add(propertyName, new JsonNumber(propertyValue));

        /// <summary>
        ///   Adds the specified property as a <see cref="JsonNumber"/> to the JSON object.
        /// </summary>
        /// <param name="propertyName">Name of the property to add.</param>
        /// <param name="propertyValue"><see cref="uint"/> value of the property to add.</param>
        /// <exception cref="ArgumentException">
        ///   Property name to set already exists if handling duplicates is set to <see cref="DuplicatePropertyNameHandling.Error"/>.
        /// </exception>
        [CLSCompliant(false)]
        public void Add(string propertyName, uint propertyValue) => _dictionary.Add(propertyName, new JsonNumber(propertyValue));

        /// <summary>
        ///   Adds the specified property as a <see cref="JsonNumber"/> to the JSON object.
        /// </summary>
        /// <param name="propertyName">Name of the property to add.</param>
        /// <param name="propertyValue"><see cref="ulong"/> value of the property to add.</param>
        /// <exception cref="ArgumentException">
        ///   Property name to set already exists if handling duplicates is set to <see cref="DuplicatePropertyNameHandling.Error"/>.
        /// </exception>
        [CLSCompliant(false)]
        public void Add(string propertyName, ulong propertyValue) => _dictionary.Add(propertyName, new JsonNumber(propertyValue));

        /// <summary>
        ///   Adds the specified property as a <see cref="JsonNumber"/> to the JSON object.
        /// </summary>
        /// <param name="propertyName">Name of the property to add.</param>
        /// <param name="propertyValue"><see cref="decimal"/> value of the property to add.</param>
        /// <exception cref="ArgumentException">
        ///   Property name to set already exists if handling duplicates is set to <see cref="DuplicatePropertyNameHandling.Error"/>.
        /// </exception>
        public void Add(string propertyName, decimal propertyValue) => _dictionary.Add(propertyName, new JsonNumber(propertyValue));

        /// <summary>
        ///   Adds the properties from the specified collection to the JSON object.
        /// </summary>
        /// <param name="jsonProperties">Properties to add.</param>
        /// <exception cref="ArgumentException">
        ///   Provided collection contains duplicates if handling duplicates is set to <see cref="DuplicatePropertyNameHandling.Error"/>.
        /// </exception>
        public void AddRange(IEnumerable<KeyValuePair<string, JsonNode>> jsonProperties)
        {
            foreach (KeyValuePair<string, JsonNode> property in jsonProperties)
            {
                Add(property);
            }
        }

        /// <summary>
        ///   Removes the property with the specified name.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool Remove(string propertyName) => _dictionary.Remove(propertyName);

        /// <summary>
        ///   Determines whether a property is in a JSON object.
        /// </summary>
        /// <param name="propertyName">Name of the property to check.</param>
        /// <returns>
        ///   <see langword="true"/> if the property is found in a JSON object,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public bool ContainsProperty(string propertyName) => _dictionary.ContainsKey(propertyName);

        /// <summary>
        ///   Modifies the name of specified property.
        /// </summary>
        /// <param name="oldName">Old name of the property to change.</param>
        /// <param name="newName">New name of the property to change.</param>
        /// <exception cref="ArgumentException">
        ///   Property name to set already exists if handling duplicates is set to <see cref="DuplicatePropertyNameHandling.Error"/>.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        ///   Property with specified name is not found in JSON object.
        /// </exception>
        public void ModifyPropertyName(string oldName, string newName)
        {
            if (!_dictionary.ContainsKey(oldName))
                throw new KeyNotFoundException(SR.PropertyNotFound);

            JsonNode previousValue = _dictionary[oldName];
            _dictionary.Remove(oldName);
            _dictionary.Add(newName, previousValue);
        }

        /// <summary>
        ///   Returns the property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property to return.</param>
        /// <returns>Value of the property with the specified name.</returns>
        /// <exception cref="KeyNotFoundException">
        ///   Property with specified name is not found in JSON object.
        /// </exception>
        public JsonNode GetProperty(string propertyName)
        {
            if (!TryGetProperty(propertyName, out JsonNode jsonNode))
            {
                throw new KeyNotFoundException(SR.PropertyNotFound);
            }

            return jsonNode;
        }

        /// <summary>
        ///   Returns the property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property to return.</param>
        /// <param name="jsonNode">Value of the property with specified name.</param>
        /// <returns>
        ///  <see langword="true"/> if property with specified name was found;
        ///  otherwise, <see langword="false"/>
        /// </returns>
        public bool TryGetProperty(string propertyName, out JsonNode jsonNode)
        {
            if (!_dictionary.ContainsKey(propertyName))
            {
                jsonNode = null;
                return false;
            }

            jsonNode = _dictionary[propertyName];
            return true;
        }

        /// <summary>
        ///   Returns the JSON object property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property to return.</param>
        /// <returns>JSON object property with the specified name.</returns>
        /// <exception cref="KeyNotFoundException">
        ///   Property with specified name is not found in JSON object.
        /// </exception>
        /// <exception cref="InvalidCastException">
        ///   Property with specified name is not a JSON object.
        /// </exception>
        public JsonObject GetJsonObjectProperty(string propertyName)
        {
            if(GetProperty(propertyName) is JsonObject jsonObject)
            {
                return jsonObject;
            }

            throw new InvalidCastException(SR.PropertyTypeMismatch);
        }

        /// <summary>
        ///   Returns the JSON object property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property to return.</param>
        /// <param name="jsonObject">JSON objec of the property with specified name.</param>
        /// <returns>
        ///  <see langword="true"/> if JSON object property with specified name was found;
        ///  otherwise, <see langword="false"/>
        /// </returns>
        public bool TryGetJsonObjectProperty(string propertyName, out JsonObject jsonObject)
        {
            if (TryGetProperty(propertyName, out JsonNode jsonNode))
            {
                if (jsonNode is JsonObject jsonNodeCasted)
                {
                    jsonObject = jsonNodeCasted;
                    return true;
                }
            }

            jsonObject = null;
            return false;
        }

        /// <summary>
        ///   A collection containing the property names of JSON object.
        /// </summary>
        public ICollection<string> PropertyNames => _dictionary.Keys;

        /// <summary>
        ///  A collection containing the property values of JSON object.
        /// </summary>
        public ICollection<JsonNode> Values => _dictionary.Values;

        /// <summary>
        ///   Returns an enumerator that iterates through the JSON object properties.
        /// </summary>
        /// <returns>An enumerator structure for the <see cref="JsonObject"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();
    }
}
