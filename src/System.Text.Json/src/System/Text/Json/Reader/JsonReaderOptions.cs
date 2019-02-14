// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json
{
    /// <summary>
    /// Provides the ability for the user to define custom behavior when reading JSON
    /// using the <see cref="Utf8JsonReader"/> that may deviate from strict adherence
    /// to the JSON specification (as per the JSON RFC - https://tools.ietf.org/html/rfc8259),
    /// which is the default behavior. It also lets the user define the maximum depth to read up to.
    /// </summary>
    public struct JsonReaderOptions
    {
        private int _maxDepth;

        /// <summary>
        /// Defines how the <see cref="Utf8JsonReader"/> should handle comments when reading through the JSON.
        /// By default the reader will throw a <exception cref="JsonReaderException"/> if it encounters a comment.
        /// </summary>
        public JsonCommentHandling CommentHandling { get; set; }

        /// <summary>
        /// Gets or sets the maximum depth allowed when reading JSON, with the default (i.e. 0) indicating a max depth of 64.
        /// Reading past this depth will throw a <exception cref="JsonReaderException"/>.
        /// </summary>
        public int MaxDepth
        {
            get
            {
                return _maxDepth;
            }
            set
            {
                if (value < 0)
                    throw ThrowHelper.GetArgumentException_MaxDepthMustBePositive();

                _maxDepth = value;
            }
        }
    }
}
