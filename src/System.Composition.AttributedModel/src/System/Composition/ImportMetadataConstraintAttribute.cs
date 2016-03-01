// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Composition
{
    /// <summary>
    /// When applied on an import, requires certain metadata values on the exporter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public sealed class ImportMetadataConstraintAttribute : Attribute
    {
        private readonly string _name;
        private readonly object _value;

        /// <summary>
        /// Require a specific metadata value on the exporter.
        /// </summary>
        /// <param name="name">The name of the metadata item to match.</param>
        /// <param name="value">The value to match.</param>
        public ImportMetadataConstraintAttribute(string name, object value)
        {
            _name = name;
            _value = value;
        }

        /// <summary>
        /// The metadata key to match.
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// The value to match.
        /// </summary>
        public object Value { get { return _value; } }
    }
}
