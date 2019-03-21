// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;

namespace System.Text.Json.Serialization
{
    /// <summary>
    /// Provides options to be used with <see cref="JsonSerializer"/>.
    /// </summary>
    public sealed class JsonSerializerOptions
    {
        internal const int BufferSizeDefault = 16 * 1024;

        private ClassMaterializer _classMaterializerStrategy;
        private int _defaultBufferSize = BufferSizeDefault;

        private static readonly ConcurrentDictionary<Type, JsonClassInfo> s_classes = new ConcurrentDictionary<Type, JsonClassInfo>();

        /// <summary>
        /// Constructs a new <see cref="JsonSerializerOptions"/> instance.
        /// </summary>
        public JsonSerializerOptions() { }

        internal JsonClassInfo GetOrAddClass(Type classType)
        {
            JsonClassInfo result;

            if (!s_classes.TryGetValue(classType, out result))
            {
                result = s_classes.GetOrAdd(classType, new JsonClassInfo(classType, this));
            }

            return result;
        }

        /// <summary>
        /// The <see cref="JsonReaderOptions"/> used when deserializing an object or value type.
        /// </summary>
        public JsonReaderOptions ReaderOptions { get; set; }

        /// <summary>
        /// The <see cref="JsonWriterOptions"/> used when serializing an object or value type.
        /// </summary>
        public JsonWriterOptions WriterOptions { get; set; }

        /// <summary>
        /// The default buffer size used when creating temporary buffers while serializing or deserializing.
        /// </summary>
        /// <exception cref="System.ArgumentException">Thrown when the buffer size is less than 1.</exception>
        public int DefaultBufferSize
        {
            get
            {
                return _defaultBufferSize;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException(SR.SerializationInvalidBufferSize);
                }

                _defaultBufferSize = value;
            }
        }

        /// <summary>
        /// Determines whether null properties are included when serializing.
        /// </summary>
        public bool IgnoreNullPropertyValueOnWrite { get; set; }

        /// <summary>
        /// Determines whether null properties are applied to object's properties when deserializing.
        /// </summary>
        public bool IgnoreNullPropertyValueOnRead { get; set; }

        internal ClassMaterializer ClassMaterializerStrategy
        {
            get
            {
                if (_classMaterializerStrategy == null)
                {
#if BUILDING_INBOX_LIBRARY
                    _classMaterializerStrategy = new ReflectionEmitMaterializer();
#else
                    // todo: should we attempt to detect here, or at least have a #define like #SUPPORTS_IL_EMIT
                    _classMaterializerStrategy = new ReflectionMaterializer();
#endif
                }

                return _classMaterializerStrategy;
            }
        }
    }
}
