// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json.Serialization
{
    /// <summary>
    /// Controls how the <see cref="JsonIgnoreAttribute"/> will decide to ignore properties.
    /// </summary>
    public enum JsonIgnoreCondition
    {
        /// <summary>
        /// Property will always be ignored.
        /// </summary>
        Always,
        /// <summary>
        /// Property will only be ignored if it is null.
        /// </summary>
        WhenNull
    }

    /// <summary>
    /// Prevents a property from being serialized or deserialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class JsonIgnoreAttribute : JsonAttribute
    {
        /// <summary>
        /// Specifies the condition that must be met before a property will be ignored. Default value: <see cref="JsonIgnoreCondition.Always"/>.
        /// </summary>
        public JsonIgnoreCondition Condition { get; set; } = JsonIgnoreCondition.Always;

        /// <summary>
        /// Initializes a new instance of <see cref="JsonIgnoreAttribute"/>.
        /// </summary>
        public JsonIgnoreAttribute() { }
    }
}
