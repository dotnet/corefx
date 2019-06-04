// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json.Serialization
{
    /// <summary>
    /// When placed on a property or type, the specified converter type will be used.
    /// </summary>
    /// <remarks>
    /// The specified converter type must derive from <see cref="JsonConverter"/>.
    /// When placed on a property, the specified converter will always be used.
    /// When placed on a type, the specified converter will be used unless a compatible converter is added to <see cref="JsonSerializerOptions.Converters"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class JsonConverterAttribute : JsonAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="JsonConverterAttribute"/> with the specified converter type.
        /// </summary>
        /// <param name="converterType">The type of the converter.</param>
        public JsonConverterAttribute(Type converterType)
        {
            ConverterType = converterType;
        }

        /// <summary>
        /// The type of the converter.
        /// </summary>
        public Type ConverterType { get; private set; }

        /// <summary>
        /// If overriden, allows a custom attribute to create the converter in order to pass additional state.
        /// </summary>
        /// <returns>The custom converter, or null if the serializer should create the custom converter.</returns>
        public virtual JsonConverter CreateConverter(Type typeToConvert)
        {
            return null;
        }
    }
}
