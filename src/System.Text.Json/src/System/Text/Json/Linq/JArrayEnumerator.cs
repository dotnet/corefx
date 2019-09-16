using System.Collections;
using System.Collections.Generic;

namespace System.Text.Json.Linq
{
    /// <summary>
    ///   Supports an iteration over a JSON array.
    /// </summary>
    public struct JArrayEnumerator : IEnumerator<JNode>
    {
        private List<JNode>.Enumerator _enumerator;

        /// <summary>
        ///   Initializes a new instance of the <see cref="JArrayEnumerator"/> class supporting an interation over provided JSON array.
        /// </summary>
        /// <param name="jsonArray">JSON array to iterate over.</param>
        public JArrayEnumerator(JArray jsonArray) => _enumerator = jsonArray._list.GetEnumerator();

        /// <summary>
        ///   Gets the property in the JSON array at the current position of the enumerator.
        /// </summary>
        public JNode Current => _enumerator.Current;

        /// <summary>
        ///    Gets the property in the JSON array at the current position of the enumerator.
        /// </summary>
        object IEnumerator.Current => _enumerator.Current;

        /// <summary>
        ///   Releases all resources used by the <see cref="JObjectEnumerator"/>.
        /// </summary>
        public void Dispose() => _enumerator.Dispose();

        /// <summary>
        ///   Advances the enumerator to the next property of the JSON array.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext() => _enumerator.MoveNext();

        /// <summary>
        ///   Sets the enumerator to its initial position, which is before the first element in the JSON array.
        /// </summary>
        void IEnumerator.Reset()
        {
            IEnumerator enumerator = _enumerator;
            enumerator.Reset();
            _enumerator = (List<JNode>.Enumerator)enumerator;
        }
    }
}
