using System.Collections;
using System.Collections.Generic;

namespace System.Text.Json.Linq
{
    /// <summary>
    ///  Represents a mutable JSON object.
    /// </summary>
    public sealed partial class JTreeObject : JTreeNode, IEnumerable<KeyValuePair<string, JTreeNode>>
    {
        /// <summary>
        ///   Supports an iteration over a JSON object.
        /// </summary>
        public struct Enumerator : IEnumerator<KeyValuePair<string, JTreeNode>>
        {
            private JTreeObjectProperty _first;
            private JTreeObjectProperty _current;

            /// <summary>
            ///   Initializes a new instance of the <see cref="Enumerator"/> class supporting an interation over provided JSON object.
            /// </summary>
            /// <param name="jsonObject">JSON object to iterate over.</param>
            public Enumerator(JTreeObject jsonObject)
            {
                _first = jsonObject._first;
                _current = null;
            }

            /// <summary>
            ///   Gets the property in the JSON object at the current position of the enumerator.
            /// </summary>
            public KeyValuePair<string, JTreeNode> Current
            {
                get
                {
                    if (_current == null)
                    {
                        return default;
                    }

                    return new KeyValuePair<string, JTreeNode>(_current.Name, _current.Value);
                }
            }

            /// <summary>
            ///    Gets the property in the JSON object at the current position of the enumerator.
            /// </summary>
            object IEnumerator.Current => Current;

            /// <summary>
            ///   Releases all resources used by the <see cref="Enumerator"/>.
            /// </summary>
            public void Dispose() => _current = null;

            /// <summary>
            ///   Advances the enumerator to the next property of the JSON object.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                if (_first == null)
                {
                    return false;
                }

                if (_current == null)
                {
                    _current = _first;
                    return true;
                }

                if (_current.Next != null)
                {
                    _current = _current.Next;
                    return true;
                }

                return false;
            }

            /// <summary>
            ///   Sets the enumerator to its initial position, which is before the first element in the JSON object.
            /// </summary>
            void IEnumerator.Reset() => _current = null;
        }
    }
}
