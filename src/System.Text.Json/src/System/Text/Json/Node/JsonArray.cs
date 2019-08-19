// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;

namespace System.Text.Json
{
    /// <summary>
    ///   Represents a JSON array.
    /// </summary>
    public partial class JsonArray : JsonNode, IList<JsonNode>, IReadOnlyList<JsonNode>
    {
        private readonly List<JsonNode> _list;

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonArray"/> class representing the empty array.
        /// </summary>
        public JsonArray() => _list = new List<JsonNode>();

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonArray"/> class representing the specified collection.
        /// </summary>
        /// <param name="values">Collection to represent.</param>
        public JsonArray(IEnumerable<JsonNode> values) => _list = new List<JsonNode>(values);

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonArray"/> class representing the specified collection of <see cref="string"/>s.
        /// </summary>
        /// <param name="values">Collection to represent.</param>
        /// <exception cref="ArgumentNullException">
        ///   Some of provided values are null.
        /// </exception>
        public JsonArray(IEnumerable<string> values) : this()
        {
            foreach (string value in values)
            {
                _list.Add(new JsonString(value));
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonArray"/> class representing the specified collection of <see cref="byte"/>s.
        /// </summary>
        /// <param name="values">Collection to represent.</param>
        public JsonArray(IEnumerable<bool> values) : this()
        {
            foreach (bool value in values)
            {
                _list.Add(new JsonBoolean(value));
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonArray"/> class representing the specified collection of <see cref="byte"/>s.
        /// </summary>
        /// <param name="values">Collection to represent.</param>
        public JsonArray(IEnumerable<byte> values) : this()
        {
            foreach (byte value in values)
            {
                _list.Add(new JsonNumber(value));
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonArray"/> class representing the specified collection of <see cref="short"/>s.
        /// </summary>
        /// <param name="values">Collection to represent.</param>
        public JsonArray(IEnumerable<short> values) : this()
        {
            foreach (short value in values)
            {
                _list.Add(new JsonNumber(value));
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonArray"/> class representing the specified collection of <see cref="int"/>s.
        /// </summary>
        /// <param name="values">Collection to represent.</param>
        public JsonArray(IEnumerable<int> values) : this()
        {
            foreach (int value in values)
            {
                _list.Add(new JsonNumber(value));
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonArray"/> class representing the specified collection of <see cref="long"/>s.
        /// </summary>
        /// <param name="values">Collection to represent.</param>
        public JsonArray(IEnumerable<long> values) : this()
        {
            foreach (long value in values)
            {
                _list.Add(new JsonNumber(value));
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonArray"/> class representing the specified collection of <see cref="float"/>s.
        /// </summary>
        /// <param name="values">Collection to represent.</param>
        /// <exception cref="ArgumentException">
        ///   Some of provided values are not in a legal JSON number format.
        /// </exception>
        public JsonArray(IEnumerable<float> values) : this()
        {
            foreach (float value in values)
            {
                _list.Add(new JsonNumber(value));
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonArray"/> class representing the specified collection of <see cref="double"/>s.
        /// </summary>
        /// <param name="values">Collection to represent.</param>
        /// <exception cref="ArgumentException">
        ///   Some of provided values are not in a legal JSON number format.
        /// </exception>
        public JsonArray(IEnumerable<double> values) : this()
        {
            foreach (double value in values)
            {
                _list.Add(new JsonNumber(value));
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonArray"/> class representing the specified collection of <see cref="sbyte"/>s.
        /// </summary>
        /// <param name="values">Collection to represent.</param>
        [CLSCompliant(false)]
        public JsonArray(IEnumerable<sbyte> values) : this()
        {
            foreach (sbyte value in values)
            {
                _list.Add(new JsonNumber(value));
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonArray"/> class representing the specified collection of <see cref="ushort"/>s.
        /// </summary>
        /// <param name="values">Collection to represent.</param>
        [CLSCompliant(false)]
        public JsonArray(IEnumerable<ushort> values) : this()
        {
            foreach (ushort value in values)
            {
                _list.Add(new JsonNumber(value));
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonArray"/> class representing the specified collection of <see cref="uint"/>s.
        /// </summary>
        /// <param name="values">Collection to represent.</param>
        [CLSCompliant(false)]
        public JsonArray(IEnumerable<uint> values) : this()
        {
            foreach (uint value in values)
            {
                _list.Add(new JsonNumber(value));
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonArray"/> class representing the specified collection of <see cref="ulong"/>s.
        /// </summary>
        /// <param name="values">Collection to represent.</param>
        [CLSCompliant(false)]
        public JsonArray(IEnumerable<ulong> values) : this()
        {
            foreach (ulong value in values)
            {
                _list.Add(new JsonNumber(value));
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="JsonArray"/> class representing the specified collection of <see cref="decimal"/>s.
        /// </summary>
        /// <param name="values">Collection to represent.</param>
        public JsonArray(IEnumerable<decimal> values) : this()
        {
            foreach (decimal value in values)
            {
                _list.Add(new JsonNumber(value));
            }
        }

        /// <summary>
        ///   Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="idx">The zero-based index of the element to get or set.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   Index is less than 0.
        /// </exception>
        public JsonNode this[int idx]
        {
            get => 0 <= idx && idx < _list.Count ? _list[idx] : throw new ArgumentOutOfRangeException();
            set
            {
                if (idx < 0 || idx >= _list.Count)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _list[idx] = value;
            }
        }

        /// <summary>
        ///   Adds the specified <see cref="JsonNode"/> value to the JSON array.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <remarks>Null value is allowed and represents a null JSON node.</remarks>
        public void Add(JsonNode value) => _list.Add(value);

        /// <summary>
        ///   Adds the specified value as a <see cref="JsonString"/> to the JSON array.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <exception cref="ArgumentNullException">
        ///   Provided value is null.
        /// </exception>
        public void Add(string value) => Add(new JsonString(value));

        /// <summary>
        ///   Adds the specified value as a <see cref="JsonBoolean"/> to the JSON array.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public void Add(bool value) => Add(new JsonBoolean(value));

        /// <summary>
        ///   Adds the specified value as a <see cref="JsonNumber"/> to the JSON array.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public void Add(byte value) => Add(new JsonNumber(value));

        /// <summary>
        ///   Adds the specified value as a <see cref="JsonNumber"/> to the JSON array.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public void Add(short value) => Add(new JsonNumber(value));

        /// <summary>
        ///   Adds the specified value as a <see cref="JsonNumber"/> to the JSON array.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public void Add(int value) => Add(new JsonNumber(value));

        /// <summary>
        ///   Adds the specified value as a <see cref="JsonNumber"/> to the JSON array.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public void Add(long value) => Add(new JsonNumber(value));

        /// <summary>
        ///   Adds the specified value as a <see cref="JsonNumber"/> to the JSON array.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <exception cref="ArgumentException">
        ///   Provided value is not in a legal JSON number format.
        /// </exception>
        public void Add(float value) => Add(new JsonNumber(value));

        /// <summary>
        ///   Adds the specified value as a <see cref="JsonNumber"/> to the JSON array.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <exception cref="ArgumentException">
        ///   Provided value is not in a legal JSON number format.
        /// </exception>
        public void Add(double value) => Add(new JsonNumber(value));

        /// <summary>
        ///   Adds the specified value as a <see cref="JsonNumber"/> to the JSON array.
        /// </summary>
        /// <param name="value">The value to add.</param>
        [CLSCompliant(false)]
        public void Add(sbyte value) => Add(new JsonNumber(value));

        /// <summary>
        ///   Adds the specified value as a <see cref="JsonNumber"/> to the JSON array.
        /// </summary>
        /// <param name="value">The value to add.</param>
        [CLSCompliant(false)]
        public void Add(ushort value) => Add(new JsonNumber(value));

        /// <summary>
        ///   Adds the specified value as a <see cref="JsonNumber"/> to the JSON array.
        /// </summary>
        /// <param name="value">The value to add.</param>
        [CLSCompliant(false)]
        public void Add(uint value) => Add(new JsonNumber(value));

        /// <summary>
        ///   Adds the specified value as a <see cref="JsonNumber"/> to the JSON array.
        /// </summary>
        /// <param name="value">The value to add.</param>
        [CLSCompliant(false)]
        public void Add(ulong value) => Add(new JsonNumber(value));

        /// <summary>
        ///   Adds the specified value as a <see cref="JsonNumber"/> to the JSON array.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public void Add(decimal value) => Add(new JsonNumber(value));

        /// <summary>
        ///   Inserts the specified item at the specified index of the JSON array.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The item to add.</param>
        /// <remarks>Null item is allowed and represents a null JSON node.</remarks>
        public void Insert(int index, JsonNode item) => _list.Insert(index, item);

        /// <summary>
        ///   Inserts the specified item as a <see cref="JsonString"/> at the specified index of the JSON array.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The item to add.</param>
        public void Insert(int index, string item) => Insert(index, new JsonString(item));

        /// <summary>
        ///   Inserts the specified item as a <see cref="JsonBoolean"/> at the specified index of the JSON array.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The item to add.</param>
        public void Insert(int index, bool item) => Insert(index, new JsonBoolean(item));

        /// <summary>
        ///   Inserts the specified item as a <see cref="JsonNumber"/> at the specified index of the JSON array.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The item to add.</param>
        public void Insert(int index, byte item) => Insert(index, new JsonNumber(item));

        /// <summary>
        ///   Inserts the specified item as a <see cref="JsonNumber"/> at the specified index of the JSON array.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The item to add.</param>
        public void Insert(int index, short item) => Insert(index, new JsonNumber(item));

        /// <summary>
        ///   Inserts the specified item as a <see cref="JsonNumber"/> at the specified index of the JSON array.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The item to add.</param>
        public void Insert(int index, int item) => Insert(index, new JsonNumber(item));

        /// <summary>
        ///   Inserts the specified item as a <see cref="JsonNumber"/> at the specified index of the JSON array.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The item to add.</param>
        public void Insert(int index, long item) => Insert(index, new JsonNumber(item));

        /// <summary>
        ///   Inserts the specified item as a <see cref="JsonNumber"/> at the specified index of the JSON array.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The item to add.</param>
        /// <exception cref="ArgumentException">
        ///   Provided value is not in a legal JSON number format.
        /// </exception>
        public void Insert(int index, float item) => Insert(index, new JsonNumber(item));

        /// <summary>
        ///   Inserts the specified item as a <see cref="JsonNumber"/> at the specified index of the JSON array.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The item to add.</param>
        /// <exception cref="ArgumentException">
        ///   Provided value is not in a legal JSON number format.
        /// </exception>
        public void Insert(int index, double item) => Insert(index, new JsonNumber(item));

        /// <summary>
        ///   Inserts the specified item as a <see cref="JsonNumber"/> at the specified index of the JSON array.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The item to add.</param>
        [CLSCompliant(false)]
        public void Insert(int index, sbyte item) => Insert(index, new JsonNumber(item));

        /// <summary>
        ///   Inserts the specified item as a <see cref="JsonNumber"/> at the specified index of the JSON array.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The item to add.</param>
        [CLSCompliant(false)]
        public void Insert(int index, ushort item) => Insert(index, new JsonNumber(item));

        /// <summary>
        ///   Inserts the specified item as a <see cref="JsonNumber"/> at the specified index of the JSON array.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The item to add.</param>
        [CLSCompliant(false)]
        public void Insert(int index, uint item) => Insert(index, new JsonNumber(item));

        /// <summary>
        ///   Inserts the specified item as a <see cref="JsonNumber"/> at the specified index of the JSON array.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The item to add.</param>
        [CLSCompliant(false)]
        public void Insert(int index, ulong item) => Insert(index, new JsonNumber(item));

        /// <summary>
        ///   Inserts the specified item as a <see cref="JsonNumber"/> at the specified index of the JSON array.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The item to add.</param>
        public void Insert(int index, decimal item) => Insert(index, new JsonNumber(item));

        /// <summary>
        ///   Determines whether a specified <see cref="JsonNode"/> element is in a JSON array.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>
        ///   <see langword="true"/> if the value is successfully found in a JSON array,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public bool Contains(JsonNode value) => _list.Contains(value);

        /// <summary>
        ///   Determines whether a <see cref="JsonString"/> representing provided <see cref="string"/> value is in a JSON array.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>
        ///   <see langword="true"/> if the value is successfully found in a JSON array,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   Provided value is null.
        /// </exception>
        public bool Contains(string value) => Contains(new JsonString(value));

        /// <summary>
        ///   Determines whether a <see cref="JsonBoolean"/> representing provided <see cref="bool"/> value is in a JSON array.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>
        ///   <see langword="true"/> if the value is successfully found in a JSON array,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public bool Contains(bool value) => Contains(new JsonBoolean(value));

        /// <summary>
        ///   Determines whether a <see cref="JsonNumber"/> representing provided <see cref="byte"/> value is in a JSON array.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>
        ///   <see langword="true"/> if the value is successfully found in a JSON array,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public bool Contains(byte value) => Contains(new JsonNumber(value));

        /// <summary>
        ///   Determines whether a <see cref="JsonNumber"/> representing provided <see cref="short"/> value is in a JSON array.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>
        ///   <see langword="true"/> if the value is successfully found in a JSON array,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public bool Contains(short value) => Contains(new JsonNumber(value));

        /// <summary>
        ///   Determines whether a <see cref="JsonNumber"/> representing provided <see cref="int"/> value is in a JSON array.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>
        ///   <see langword="true"/> if the value is successfully found in a JSON array,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public bool Contains(int value) => Contains(new JsonNumber(value));

        /// <summary>
        ///   Determines whether a <see cref="JsonNumber"/> representing provided <see cref="long"/> value is in a JSON array.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>
        ///   <see langword="true"/> if the value is successfully found in a JSON array,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public bool Contains(long value) => Contains(new JsonNumber(value));

        /// <summary>
        ///   Determines whether a <see cref="JsonNumber"/> representing provided <see cref="float"/> value is in a JSON array.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>
        ///   <see langword="true"/> if the value is successfully found in a JSON array,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///   Provided value is not in a legal JSON number format.
        /// </exception>
        public bool Contains(float value) => Contains(new JsonNumber(value));

        /// <summary>
        ///   Determines whether a <see cref="JsonNumber"/> representing provided <see cref="double"/> value is in a JSON array.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>
        ///   <see langword="true"/> if the value is successfully found in a JSON array,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///   Provided value is not in a legal JSON number format.
        /// </exception>
        public bool Contains(double value) => Contains(new JsonNumber(value));

        /// <summary>
        ///   Determines whether a <see cref="JsonNumber"/> representing provided <see cref="sbyte"/> value is in a JSON array.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>
        ///   <see langword="true"/> if the value is successfully found in a JSON array,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        [CLSCompliant(false)]
        public bool Contains(sbyte value) => Contains(new JsonNumber(value));

        /// <summary>
        ///   Determines whether a <see cref="JsonNumber"/> representing provided <see cref="ushort"/> value is in a JSON array.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>
        ///   <see langword="true"/> if the value is successfully found in a JSON array,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        [CLSCompliant(false)]
        public bool Contains(ushort value) => Contains(new JsonNumber(value));

        /// <summary>
        ///   Determines whether a <see cref="JsonNumber"/> representing provided <see cref="uint"/> value is in a JSON array.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>
        ///   <see langword="true"/> if the value is successfully found in a JSON array,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        [CLSCompliant(false)]
        public bool Contains(uint value) => Contains(new JsonNumber(value));

        /// <summary>
        ///   Determines whether a <see cref="JsonNumber"/> representing provided <see cref="ulong"/> value is in a JSON array.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>
        ///   <see langword="true"/> if the value is successfully found in a JSON array,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        [CLSCompliant(false)]
        public bool Contains(ulong value) => Contains(new JsonNumber(value));

        /// <summary>
        ///   Determines whether a <see cref="JsonNumber"/> representing provided <see cref="decimal"/> value is in a JSON array.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>
        ///   <see langword="true"/> if the value is successfully found in a JSON array,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public bool Contains(decimal value) => Contains(new JsonNumber(value));

        /// <summary>
        ///   Gets the number of elements contained in the JSON array.
        /// </summary>
        public int Count => _list.Count;

        /// <summary>
        ///   Gets a value indicating whether the JSON array is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        ///   Returns the zero-based index of the first occurrence of a specified item in the JSON array.
        /// </summary>
        /// <param name="item">Item to find.</param>
        /// <returns>The zero-based starting index of the search. 0 (zero) is valid in an empty list.</returns>
        public int IndexOf(JsonNode item) => _list.IndexOf(item);

        /// <summary>
        ///   Returns the zero-based index of the last occurrence of a specified item in the JSON array.
        /// </summary>
        /// <param name="item">Item to find.</param>
        /// <returns>The zero-based starting index of the search. 0 (zero) is valid in an empty list.</returns>
        public int LastIndexOf(JsonNode item) => _list.LastIndexOf(item);

        /// <summary>
        ///   Removes all elements from the JSON array.
        /// </summary>
        public void Clear() => _list.Clear();

        /// <summary>
        ///   Removes the first occurrence of a specific object from the JSON array.
        /// </summary>
        /// <param name="item">
        ///   The object to remove from the JSON array. The value can be null and it represents null JSON node.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if the item is successfully found in a JSON array and removed,
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public bool Remove(JsonNode item) => _list.Remove(item);

        /// <summary>
        ///   Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">
        ///   Thepredicate delegate that defines the conditions of the elements to remove.
        /// </param>
        /// <returns>The number of elements removed from the JSOJN array.</returns>
        public int RemoveAll(Predicate<JsonNode> match) => _list.RemoveAll(match);

        /// <summary>
        ///   Removes the item at the specified index of the JSON array.
        /// </summary>
        /// <param name="index">
        ///   The zero-based index of the element to remove.
        /// </param>
        public void RemoveAt(int index) => _list.RemoveAt(index);

        /// <summary>
        ///   Copies the JSON array or a portion of it to an array.
        /// </summary>
        /// <param name="array">
        ///   The one-dimensional Array that is the destination of the elements copied from JSON array.
        ///   The Array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        void ICollection<JsonNode>.CopyTo(JsonNode[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        /// <summary>
        ///   Returns an enumerator that iterates through the JSON array values.
        /// </summary>
        /// <returns>An enumerator structure for the <see cref="JsonArray"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

        /// <summary>
        ///   Returns an enumerator that iterates through the JSON array values.
        /// </summary>
        /// <returns>An enumerator structure for the <see cref="JsonArray"/>.</returns>
        public IEnumerator<JsonNode> GetEnumerator() => _list.GetEnumerator();

        /// <summary>
        ///   Creates a new JSON array that is a copy of the current instance.
        /// </summary>
        /// <returns>A new JSON array that is a copy of this instance.</returns>
        public override JsonNode Clone() => new JsonArray(_list);

        /// <summary>
        ///   Returns <see cref="JsonNodeKind.Array"/>
        /// </summary>
        public override JsonNodeKind NodeKind { get => JsonNodeKind.Array;}
    }
}
