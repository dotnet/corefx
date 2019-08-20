// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Text.Json
{
    public partial struct JsonElement
    {
        /// <summary>
        ///   An enumerable and enumerator for the contents of a JSON array.
        /// </summary>
        [DebuggerDisplay("{Current,nq}")]
        public struct ArrayEnumerator : IEnumerable<JsonElement>, IEnumerator<JsonElement>
        {
            private readonly JsonElement _target;
            private int _curIdx;
            private readonly int _endIdx;
            private readonly JsonArrayEnumerator? _jsonArrayEnumerator;

            internal ArrayEnumerator(JsonElement target)
            {
                if (target._parent is JsonDocument document)
                {
                    Debug.Assert(target.TokenType == JsonTokenType.StartArray);

                    _target = target;
                    _curIdx = -1;
                    _endIdx = document.GetEndIndex(_target._idx, includeEndElement: false);
                    _jsonArrayEnumerator = null;
                }
                else
                {
                    _target = target;
                    _curIdx = -1;
                    _endIdx = -1;

                    var jsonArray = (JsonArray)target._parent;
                    _jsonArrayEnumerator = new JsonArrayEnumerator(jsonArray);
                }
            }

            /// <inheritdoc />
            public JsonElement Current
            {
                get
                {
                    if (_target._parent == null || _target._parent is JsonDocument)
                    {
                        var document = _target._parent as JsonDocument;
                        if (_curIdx < 0)
                        {
                            return default;
                        }
                        return new JsonElement(document, _curIdx);
                    }

                    Debug.Assert(_jsonArrayEnumerator.HasValue);
                    return _jsonArrayEnumerator.Value.Current.AsJsonElement();
                }
            }

            /// <summary>
            ///   Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>
            ///   An <see cref="ArrayEnumerator"/> value that can be used to iterate
            ///   through the array.
            /// </returns>
            public ArrayEnumerator GetEnumerator()
            {
                ArrayEnumerator ator = this;
                ator._curIdx = -1;
                return ator;
            }

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            /// <inheritdoc />
            IEnumerator<JsonElement> IEnumerable<JsonElement>.GetEnumerator() => GetEnumerator();

            /// <inheritdoc />
            public void Dispose()
            {
                _curIdx = _endIdx;
                if (_jsonArrayEnumerator.HasValue)
                {
                    _jsonArrayEnumerator.Value.Dispose();
                }
            }

            /// <inheritdoc />
            public void Reset()
            {
                _curIdx = -1;
                if (_jsonArrayEnumerator.HasValue)
                {
                    _jsonArrayEnumerator.Value.Reset();
                }
            }

            /// <inheritdoc />
            object IEnumerator.Current => Current;

            /// <inheritdoc />
            public bool MoveNext()
            {
                if (_target._parent == null || _target._parent is JsonDocument)
                {
                    var document = _target._parent as JsonDocument;

                    if (_curIdx >= _endIdx)
                    {
                        return false;
                    }

                    if (_curIdx < 0)
                    {
                        _curIdx = _target._idx + JsonDocument.DbRow.Size;
                    }
                    else
                    {
                        _curIdx = document.GetEndIndex(_curIdx, includeEndElement: true);
                    }

                    return _curIdx < _endIdx;
                }

                Debug.Assert(_jsonArrayEnumerator.HasValue);
                return _jsonArrayEnumerator.Value.MoveNext();
            }
        }
    }
}
