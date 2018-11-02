// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Text.Json
{
    /// <summary>
    /// Defines an opaque type that holds and saves all the relevant state information which must be provided
    /// to the <see cref="JsonUtf8Reader"/> to continue reading after processing incomplete data.
    /// This type is required to support reentrancy when reading incomplete data, and to continue
    /// reading once more data is available. Unlike the <see cref="JsonUtf8Reader"/>, which is a ref struct,
    /// this type can survive across async/await boundaries and hence this type is required to provide
    /// support for reading in more data asynchronously before continuing with a new instance of the <see cref="JsonUtf8Reader"/>.
    /// </summary>
    public struct JsonReaderState
    {
        // We are using a ulong to represent our nested state, so we can only
        // go 64 levels deep without having to allocate a stack.
        internal const int StackFreeMaxDepth = sizeof(ulong) * 8;

        // This ulong container represents a tiny stack to track the state during nested transitions.
        // The first bit represents the state of the current depth (1 == object, 0 == array).
        // Each subsequent bit is the parent / containing type (object or array). Since this
        // reader does a linear scan, we only need to keep a single path as we go through the data.
        // This is primarily used as an optimization to avoid having to allocate a stack object for
        // depths up to 64 (which is the default max depth).
        internal ulong _stackFreeContainer;
        internal long _lineNumber;
        internal long _lineBytePosition;
        internal long _bytesConsumed;
        internal int _currentDepth;
        internal int _maxDepth;
        internal bool _inObject;
        internal bool _isNotPrimitive;
        internal JsonTokenType _tokenType;
        internal JsonReaderOptions _readerOptions;
        internal Stack<JsonTokenType> _stack;
        internal SequencePosition _sequencePosition;

        /// <summary>
        /// Returns the current <see cref="SequencePosition"/> within the provided UTF-8 encoded
        /// input ReadOnlySequence&lt;byte&gt;. If the <see cref="JsonUtf8Reader"/> was constructed
        /// with a ReadOnlySpan&lt;byte&gt; instead, this will always return a default <see cref="SequencePosition"/>.
        /// </summary>
        public SequencePosition Position => _sequencePosition;

        /// <summary>
        /// Returns the total amount of bytes consumed by the <see cref="JsonUtf8Reader"/> so far
        /// for the given UTF-8 encoded input text.
        /// </summary>
        public long BytesConsumed => _bytesConsumed;

        /// <summary>
        /// Constructs a new <see cref="JsonReaderState"/> instance.
        /// </summary>
        /// <param name="maxDepth">Sets the maximum depth allowed when reading JSON, with the default set as 64.
        /// Reading past this depth will throw a <exception cref="JsonReaderException"/>.</param>
        /// <param name="commentHandling">Defines how the <see cref="JsonUtf8Reader"/> should handle comments when reading through the JSON.
        /// By default, the <see cref="JsonUtf8Reader"/> treats comments within the JSON as invalid.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the max depth is set to a non-positive value (&lt;= 0)
        /// </exception>
        /// <remarks>
        /// An instance of this state must be passed to the <see cref="JsonUtf8Reader"/> ctor with the JSON data.
        /// Unlike the <see cref="JsonUtf8Reader"/>, which is a ref struct, the state can survive
        /// across async/await boundaries and hence this type is required to provide support for reading
        /// in more data asynchronously before continuing with a new instance of the <see cref="JsonUtf8Reader"/>.
        /// </remarks>
        public JsonReaderState(int maxDepth = StackFreeMaxDepth, JsonCommentHandling commentHandling = JsonCommentHandling.Default)
        {
            if (maxDepth <= 0)
                ThrowHelper.ThrowArgumentException_MaxDepthMustBePositive();

            _stackFreeContainer = default;
            _lineNumber = default;
            _lineBytePosition = default;
            _bytesConsumed = default;
            _currentDepth = default;
            _maxDepth = maxDepth;
            _inObject = default;
            _isNotPrimitive = default;
            _tokenType = default;
            _readerOptions = new JsonReaderOptions { CommentHandling = commentHandling };
            _sequencePosition = default;

            // Only allocate the stack if the user explicitly sets the JsonReaderOptions
            // by providing a custom JsonCommentHandling OR if the user explicitly sets the
            // max depth to be larger than 64. This way we avoid allocations in the common, default cases.
            if (commentHandling == JsonCommentHandling.AllowComments || maxDepth > StackFreeMaxDepth)
                _stack = new Stack<JsonTokenType>();
            else
                _stack = null;
        }

        /// <summary>
        /// Gets the custom behaviour when reading JSON using
        /// the <see cref="JsonUtf8Reader"/> that may deviate from strict adherence
        /// to the JSON specification, which is the default behaviour.
        /// </summary>
        public JsonReaderOptions Options => _readerOptions;

        /// <summary>
        /// Gets or sets the maximum depth allowed when reading JSON.
        /// Reading past this depth will throw a <exception cref="JsonReaderException"/>.
        /// </summary>
        public int MaxDepth => _maxDepth;
    }
}
