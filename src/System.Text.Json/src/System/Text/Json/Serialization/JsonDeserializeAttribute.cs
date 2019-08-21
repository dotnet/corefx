// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json.Serialization
{
    /// <summary>
    /// Flags a private property for inclusion during deserialization.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     By default only properties with public get &amp; set operations defined will be deserialized.
    ///     Use <see cref="JsonDeserializeAttribute"/> to indicate the instance returned by the get method should be used
    ///     to deserialize a property.
    ///   </para>
    ///   <para>
    ///     For collections the instance returned should implement <see cref="System.Collections.IList"/>. For dictionaries
    ///     the instance returned should implement <see cref="System.Collections.IDictionary"/>. In both cases the instance
    ///     should not be read-only or fixed-size (arrays).
    ///   </para>
    ///   <para>
    ///     <see cref="JsonDeserializeAttribute"/> is ignored when a public set operation is defined.
    ///   </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class JsonDeserializeAttribute : JsonAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="JsonDeserializeAttribute"/>.
        /// </summary>
        public JsonDeserializeAttribute() { }
    }
}
