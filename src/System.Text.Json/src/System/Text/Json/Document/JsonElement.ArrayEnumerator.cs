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
            private readonly int _endIdx; // version for JsonArray

            internal ArrayEnumerator(JsonElement target)
            {
                _target = target;
                _curIdx = -1;

                if (target._parent is JsonDocument document)
                {
                    Debug.Assert(target.TokenType == JsonTokenType.StartArray);

                    _endIdx = document.GetEndIndex(_target._idx, includeEndElement: false);
                }
                else
                {
                    var jsonArray = (JsonArray)target._parent;

                    _endIdx = jsonArray._version;
                }
            }

            /// <inheritdoc />
            public JsonElement Current
            {
                get
                {
                    if (_target._parent is JsonArray jsonArray)
                    {
                        Debug.Assert(jsonArray._version == _endIdx);

                        return jsonArray[_curIdx].AsJsonElement();
                    }

                    var document = _target._parent as JsonDocument;

                    if (_curIdx < 0)
                    {
                        return default;
                    }

                    return new JsonElement(document, _curIdx);
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
            }

            /// <inheritdoc />
            public void Reset()
            {
                _curIdx = -1;
            }

            /// <inheritdoc />
            object IEnumerator.Current => Current;

            /// <inheritdoc />
            public bool MoveNext()
            {
                if (_target._parent is JsonArray jsonArray)
                {
                    Debug.Assert(jsonArray._version == _endIdx);

                    _curIdx++;

                    return _curIdx < jsonArray.Count;
                }

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
        }
    }
}
