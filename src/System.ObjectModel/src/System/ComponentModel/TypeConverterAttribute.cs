// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel
{
    /// <summary>
    /// Specifies what type to use as a converter for the object this attribute is
    /// bound to. This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public sealed class TypeConverterAttribute : Attribute
    {
        /// <summary>
        /// Specifies the type to use as a converter for the object this attribute is
        /// bound to. This <see langword='static '/>field is read-only.
        /// </summary>
        public static readonly TypeConverterAttribute Default = new TypeConverterAttribute();

        /// <summary>
        /// Initializes a new instance of the <see cref='System.ComponentModel.TypeConverterAttribute'/>
        /// class with the default type converter, which is an empty string ("").
        /// </summary>
        public TypeConverterAttribute()
        {
            ConverterTypeName = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='System.ComponentModel.TypeConverterAttribute'/>
        /// class, using the specified type as the data converter for the object this attribute
        /// is bound to.
        /// </summary>
        public TypeConverterAttribute(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            ConverterTypeName = type.AssemblyQualifiedName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='System.ComponentModel.TypeConverterAttribute'/>
        /// class, using the specified type name as the data converter for the object this attribute
        /// is bound to.
        /// </summary>
        public TypeConverterAttribute(string typeName)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            ConverterTypeName = typeName;
        }

        /// <summary>
        /// Gets the fully qualified type name of the <see cref='System.Type'/> to use as a
        /// converter for the object this attribute is bound to.
        /// </summary>
        public string ConverterTypeName { get; }

        public override bool Equals(object obj)
        {
            return
                obj is TypeConverterAttribute other &&
                other.ConverterTypeName == ConverterTypeName;
        }

        public override int GetHashCode() => ConverterTypeName.GetHashCode();
    }
}
