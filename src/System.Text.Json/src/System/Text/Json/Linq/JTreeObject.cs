// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;

namespace System.Text.Json.Linq
{
    /// <summary>
    ///  Represents a mutable JSON object.
    /// </summary>
    public sealed partial class JTreeObject : JTreeNode, IEnumerable<KeyValuePair<string, JTreeNode>>
    {
        internal readonly Dictionary<string, JTreeObjectProperty> _dictionary;
        internal JTreeObjectProperty _first;
        internal JTreeObjectProperty _last;
        internal int _version;

        /// <summary>
        ///   Initializes a new instance of the <see cref="JTreeObject"/> class representing the empty object.
        /// </summary>
        public JTreeObject()
        {
            _dictionary = new Dictionary<string, JTreeObjectProperty>();
            _version = 0;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="JTreeObject"/> class representing provided set of JSON properties.
        /// </summary>
        /// <param name="jsonProperties">>Properties to represent as a JSON object.</param>
        public JTreeObject(IEnumerable<KeyValuePair<string, JTreeNode>> jsonProperties)
            : this()
            => AddRange(jsonProperties);

        /// <summary>
        ///   Parses a string representing JSON document into a <see cref="JTreeObject"/>.
        /// </summary>
        /// <param name="json">JSON to parse.</param>
        /// <param name="options">Options to control the parsing behavior.</param>
        /// <returns><see cref="JTreeObject"/> representation of <paramref name="json"/>.</returns>
        /// <exception cref="InvalidOperationException">
        ///   Provided json text is not a JSON object.
        /// </exception>
        public static new JTreeObject Parse(string json, JTreeNodeOptions options = default)
        {
            JTreeNode node = JTreeNode.Parse(json, options);
            if (node is JTreeObject JTreeObject)
            {
                return JTreeObject;
            }
            throw new InvalidOperationException(SR.Format(SR.JsonTypeMismatch, typeof(JTreeObject), node.GetType()));
        }

        /// <summary>
        ///   Gets or sets the value of the specified property.
        /// </summary>
        /// <param name="propertyName">The property name of the value to get or set.</param>
        /// <exception cref="ArgumentNullException">
        ///   Provided property name is null.
        /// </exception>
        public JTreeNode this[string propertyName]
        {
            get => propertyName != null ? GetPropertyValue(propertyName) : throw new ArgumentNullException(nameof(propertyName));
            set
            {
                if (propertyName == null)
                {
                    throw new ArgumentNullException(nameof(propertyName));
                }

                if (_dictionary.ContainsKey(propertyName))
                {
                    _dictionary[propertyName].Value = value ?? new JTreeNull();
                }
                else
                {
                    Add(propertyName, value);
                }

                _version++;
            }
        }

        /// <summary>
        ///   Adds the specified property to the JSON object.
        /// </summary>
        /// <param name="jsonProperty">The property to add.</param>
        /// <exception cref="ArgumentException">
        ///   Property name to add already exists.
        /// </exception>
        public void Add(KeyValuePair<string, JTreeNode> jsonProperty) => Add(jsonProperty.Key, jsonProperty.Value);

        /// <summary>
        ///   Adds the specified <see cref="JTreeNode"/> property to the JSON object.
        /// </summary>
        /// <param name="propertyName">Name of the property to add.</param>
        /// <param name="propertyValue">Value of the property to add.</param>
        /// <exception cref="ArgumentNullException">
        ///   Provided property name is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Property name to add already exists.
        /// </exception>
        /// <remarks>Null value is allowed and will be converted to the <see cref="JTreeNull"/> instance.</remarks>
        public void Add(string propertyName, JTreeNode propertyValue)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (_dictionary.ContainsKey(propertyName))
            {
                throw new ArgumentException(SR.Format(SR.JsonObjectDuplicateKey, propertyName));
            }

            // Add property to linked list:
            if (_last == null)
            {
                _last = new JTreeObjectProperty(propertyName, propertyValue ?? new JTreeNull(), null, null);
                _first = _last;
            }
            else
            {
                var newJsonObjectProperty = new JTreeObjectProperty(propertyName, propertyValue ?? new JTreeNull(), _last, null);
                _last.Next = newJsonObjectProperty;
                _last = newJsonObjectProperty;
            }

            // Add property to dictionary:
            _dictionary[propertyName] = _last;

            _version++;
        }

        /// <summary>
        ///   Adds the properties from the specified collection to the JSON object.
        /// </summary>
        /// <param name="jsonProperties">Properties to add.</param>
        /// <exception cref="ArgumentException">
        ///   Provided collection contains duplicates.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   Some of property names are null.
        /// </exception>
        public void AddRange(IEnumerable<KeyValuePair<string, JTreeNode>> jsonProperties)
        {
            foreach (KeyValuePair<string, JTreeNode> property in jsonProperties)
            {
                Add(property);
            }
        }

        /// <summary>
        ///   Removes the property with the specified name.
        /// </summary>
        /// <param name="propertyName">>Name of a property to remove.</param>
        /// <returns>
        ///   <see langword="true"/> if the property is successfully found in a JSON object and removed,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   Provided property name is null.
        /// </exception>
        public bool Remove(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

#if BUILDING_INBOX_LIBRARY
            if (_dictionary.Remove(propertyName, out JTreeObjectProperty value))
            {
                AdjustLinkedListPointers(value);
                _version++;

                return true;
            }
#else
            if (_dictionary.TryGetValue(propertyName, out JTreeObjectProperty value))
            {
                AdjustLinkedListPointers(value);
                _dictionary.Remove(propertyName);
                _version++;

                return true;
            }
#endif

            return false;
        }

        /// <summary>
        ///   Removes the property with the specified name.
        /// </summary>
        /// <param name="propertyName">>Name of a property to remove.</param>
        /// <param name="stringComparison">The culture and case to be used when comparing string value.</param>
        /// <returns>
        ///   <see langword="true"/> if the property is successfully found in a JSON object and removed,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   Provided property name is null.
        /// </exception>
        public bool Remove(string propertyName, StringComparison stringComparison)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            JTreeObjectProperty _current = _first;

            while (_current != null && !string.Equals(_current.Name, propertyName, stringComparison))
            {
                _current = _current.Next;
            }

            if (_current != null)
            {
                AdjustLinkedListPointers(_current);
                _dictionary.Remove(_current.Name);
                return true;
            }

            return false;
        }

        private void AdjustLinkedListPointers(JTreeObjectProperty propertyToRemove)
        {
            // Adjust linked list pointers:

            if (propertyToRemove.Prev == null)
            {
                _first = propertyToRemove.Next;
            }
            else
            {
                propertyToRemove.Prev.Next = propertyToRemove.Next;
            }

            if (propertyToRemove.Next == null)
            {
                _last = propertyToRemove.Prev;
            }
            else
            {
                propertyToRemove.Next.Prev = propertyToRemove.Prev;
            }
        }

        /// <summary>
        ///   Determines whether a property is in a JSON object.
        /// </summary>
        /// <param name="propertyName">Name of the property to check.</param>
        /// <returns>
        ///   <see langword="true"/> if the property is successfully found in a JSON object,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   Provided property name is null.
        /// </exception>
        public bool ContainsProperty(string propertyName) => propertyName != null ? _dictionary.ContainsKey(propertyName) : throw new ArgumentNullException(nameof(propertyName));

        /// <summary>
        ///   Determines whether a property is in a JSON object.
        /// </summary>
        /// <param name="propertyName">Name of the property to check.</param>
        /// <param name="stringComparison">The culture and case to be used when comparing string value.</param>
        /// <returns>
        ///   <see langword="true"/> if the property is successfully found in a JSON object,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   Provided property name is null.
        /// </exception>
        public bool ContainsProperty(string propertyName, StringComparison stringComparison)
        {
            foreach (KeyValuePair<string, JTreeNode> property in this)
            {
                if (string.Equals(property.Key, propertyName, stringComparison))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///   Returns the value of a property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property to return.</param>
        /// <returns>Value of the property with the specified name.</returns>
        /// <exception cref="KeyNotFoundException">
        ///   Property with specified name is not found in JSON object.
        /// </exception>
        public JTreeNode GetPropertyValue(string propertyName)
        {
            if (!TryGetPropertyValue(propertyName, out JTreeNode jsonNode))
            {
                throw new KeyNotFoundException(SR.Format(SR.PropertyNotFound, propertyName));
            }

            return jsonNode;
        }

        /// <summary>
        ///   Returns the value of a property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property to return.</param>
        /// <param name="stringComparison">The culture and case to be used when comparing string value.</param>
        /// <returns>Value of the property with the specified name.</returns>
        /// <exception cref="KeyNotFoundException">
        ///   Property with specified name is not found in JSON object.
        /// </exception>
        public JTreeNode GetPropertyValue(string propertyName, StringComparison stringComparison)
        {
            if (!TryGetPropertyValue(propertyName, stringComparison, out JTreeNode jsonNode))
            {
                throw new KeyNotFoundException(SR.Format(SR.PropertyNotFound, propertyName));
            }

            return jsonNode;
        }

        /// <summary>
        ///   Returns the value of a property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property to return.</param>
        /// <param name="jsonNode">Value of the property with specified name.</param>
        /// <returns>
        ///  <see langword="true"/> if property with specified name was found;
        ///  otherwise, <see langword="false"/>
        /// </returns>
        /// <remarks>
        ///   When returns <see langword="false"/>, the value of <paramref name="jsonNode"/> is meaningless.
        /// </remarks>
        public bool TryGetPropertyValue(string propertyName, out JTreeNode jsonNode)
        {
            if (_dictionary.TryGetValue(propertyName, out JTreeObjectProperty jsonObjectProperty))
            {
                jsonNode = jsonObjectProperty.Value;
                return true;
            }

            jsonNode = null;
            return false;
        }

        /// <summary>
        ///   Returns the value of a property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property to return.</param>
        /// <param name="stringComparison">The culture and case to be used when comparing string value.</param>
        /// <param name="jsonNode">Value of the property with specified name.</param>
        /// <returns>
        ///  <see langword="true"/> if property with specified name was found;
        ///  otherwise, <see langword="false"/>
        /// </returns>
        /// <remarks>
        ///   When returns <see langword="false"/>, the value of <paramref name="jsonNode"/> is meaningless.
        /// </remarks>
        public bool TryGetPropertyValue(string propertyName, StringComparison stringComparison, out JTreeNode jsonNode)
        {
            foreach (KeyValuePair<string, JTreeNode> property in this)
            {
                if (string.Equals(property.Key, propertyName, stringComparison))
                {
                    jsonNode = property.Value;
                    return true;
                }
            }

            jsonNode = null;
            return false;
        }

        /// <summary>
        ///   Returns the JSON object value of a property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property to return.</param>
        /// <returns>JSON objectvalue of a property with the specified name.</returns>
        /// <exception cref="KeyNotFoundException">
        ///   Property with specified name is not found in JSON object.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Property with specified name is not a JSON object.
        /// </exception>
        public JTreeObject GetJsonObjectPropertyValue(string propertyName)
        {
            if (GetPropertyValue(propertyName) is JTreeObject jsonObject)
            {
                return jsonObject;
            }

            throw new ArgumentException(SR.Format(SR.PropertyTypeMismatch, propertyName));
        }

        /// <summary>
        ///   Returns the JSON object value of a property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property to return.</param>
        /// <param name="stringComparison">The culture and case to be used when comparing string value.</param>
        /// <returns>JSON objectvalue of a property with the specified name.</returns>
        /// <exception cref="KeyNotFoundException">
        ///   Property with specified name is not found in JSON object.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Property with specified name is not a JSON object.
        /// </exception>
        public JTreeObject GetJsonObjectPropertyValue(string propertyName, StringComparison stringComparison)
        {
            if (GetPropertyValue(propertyName, stringComparison) is JTreeObject jsonObject)
            {
                return jsonObject;
            }

            throw new ArgumentException(SR.Format(SR.PropertyTypeMismatch, propertyName));
        }

        /// <summary>
        ///   Returns the JSON object value of a property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property to return.</param>
        /// <param name="jsonObject">JSON object value of the property with specified name.</param>
        /// <returns>
        ///  <see langword="true"/> if JSON object property with specified name was found;
        ///  otherwise, <see langword="false"/>
        /// </returns>
        public bool TryGetJsonObjectPropertyValue(string propertyName, out JTreeObject jsonObject)
        {
            if (TryGetPropertyValue(propertyName, out JTreeNode jsonNode))
            {
                jsonObject = jsonNode as JTreeObject;
                return jsonObject != null;
            }

            jsonObject = null;
            return false;
        }

        /// <summary>
        ///   Returns the JSON object value of a property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property to return.</param>
        /// <param name="stringComparison">The culture and case to be used when comparing string value.</param>
        /// <param name="jsonObject">JSON object value of the property with specified name.</param>
        /// <returns>
        ///  <see langword="true"/> if JSON object property with specified name was found;
        ///  otherwise, <see langword="false"/>
        /// </returns>
        public bool TryGetJsonObjectPropertyValue(string propertyName, StringComparison stringComparison, out JTreeObject jsonObject)
        {
            if (TryGetPropertyValue(propertyName, stringComparison, out JTreeNode jsonNode))
            {
                jsonObject = jsonNode as JTreeObject;
                return jsonObject != null;
            }

            jsonObject = null;
            return false;
        }

        /// <summary>
        ///   Returns the JSON array value of a property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property to return.</param>
        /// <returns>JSON objectvalue of a property with the specified name.</returns>
        /// <exception cref="KeyNotFoundException">
        ///   Property with specified name is not found in JSON array.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Property with specified name is not a JSON array.
        /// </exception>
        public JTreeArray GetJsonArrayPropertyValue(string propertyName)
        {
            if (GetPropertyValue(propertyName) is JTreeArray jsonArray)
            {
                return jsonArray;
            }

            throw new ArgumentException(SR.Format(SR.PropertyTypeMismatch, propertyName));
        }

        /// <summary>
        ///   Returns the JSON array value of a property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property to return.</param>
        /// <param name="stringComparison">The culture and case to be used when comparing string value.</param>
        /// <returns>JSON objectvalue of a property with the specified name.</returns>
        /// <exception cref="KeyNotFoundException">
        ///   Property with specified name is not found in JSON array.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Property with specified name is not a JSON array.
        /// </exception>
        public JTreeArray GetJsonArrayPropertyValue(string propertyName, StringComparison stringComparison)
        {
            if (GetPropertyValue(propertyName, stringComparison) is JTreeArray jsonArray)
            {
                return jsonArray;
            }

            throw new ArgumentException(SR.Format(SR.PropertyTypeMismatch, propertyName));
        }

        /// <summary>
        ///   Returns the JSON array value of a property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property to return.</param>
        /// <param name="jsonArray">JSON array value of the property with specified name.</param>
        /// <returns>
        ///  <see langword="true"/> if JSON array property with specified name was found;
        ///  otherwise, <see langword="false"/>
        /// </returns>
        public bool TryGetJsonArrayPropertyValue(string propertyName, out JTreeArray jsonArray)
        {
            if (TryGetPropertyValue(propertyName, out JTreeNode jsonNode))
            {
                jsonArray = jsonNode as JTreeArray;
                return jsonArray != null;
            }

            jsonArray = null;
            return false;
        }

        /// <summary>
        ///   Returns the JSON array value of a property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property to return.</param>
        /// <param name="stringComparison">The culture and case to be used when comparing string value.</param>
        /// <param name="jsonArray">JSON array value of the property with specified name.</param>
        /// <returns>
        ///  <see langword="true"/> if JSON array property with specified name was found;
        ///  otherwise, <see langword="false"/>
        /// </returns>
        public bool TryGetJsonArrayPropertyValue(string propertyName, StringComparison stringComparison, out JTreeArray jsonArray)
        {
            if (TryGetPropertyValue(propertyName, stringComparison, out JTreeNode jsonNode))
            {
                jsonArray = jsonNode as JTreeArray;
                return jsonArray != null;
            }

            jsonArray = null;
            return false;
        }

        /// <summary>
        ///   A collection containing the property names of JSON object.
        /// </summary>
        public IReadOnlyCollection<string> GetPropertyNames() => _dictionary.Keys;

        /// <summary>
        ///  A collection containing the property values of JSON object.
        /// </summary>
        public IReadOnlyCollection<JTreeNode> GetPropertyValues()
        {
            var list = new List<JTreeNode>(_dictionary.Count);
            foreach (KeyValuePair<string, JTreeObjectProperty> item in _dictionary)
            {
                list.Add(item.Value.Value);
            }
            return list;
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the JSON object properties.
        /// </summary>
        /// <returns>An enumerator structure for the <see cref="JTreeObject"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        ///   Returns an enumerator that iterates through the JSON object properties.
        /// </summary>
        /// <returns>An enumerator structure for the JSON object.</returns>
        IEnumerator<KeyValuePair<string, JTreeNode>> IEnumerable<KeyValuePair<string, JTreeNode>>.GetEnumerator() => new Enumerator(this);

        /// <summary>
        ///   Returns an enumerator that iterates through the JSON object properties.
        /// </summary>
        /// <returns>An enumerator structure for the JSON object.</returns>
        public Enumerator GetEnumerator() => new Enumerator(this);

        /// <summary>
        ///   Creates a new JSON object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new JSON object that is a copy of this instance.</returns>
        public override JTreeNode Clone()
        {
            var jsonObject = new JTreeObject();

            foreach (KeyValuePair<string, JTreeNode> property in this)
            {
                jsonObject.Add(property.Key, property.Value.Clone());
            }

            return jsonObject;
        }

        /// <summary>
        ///   Returns <see cref="JsonValueKind.Object"/>
        /// </summary>
        public override JsonValueKind ValueKind { get => JsonValueKind.Object; }
    }
}
