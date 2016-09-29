// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel
{
    /// <summary>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes")]
    [AttributeUsage(AttributeTargets.Property)]
    public class AttributeProviderAttribute : Attribute
    {
        private readonly string _typeName;
        private readonly string _propertyName;

        /// <summary>
        ///     Creates a new AttributeProviderAttribute object.
        /// </summary>
        public AttributeProviderAttribute(string typeName)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            _typeName = typeName;
        }

        /// <summary>
        ///     Creates a new AttributeProviderAttribute object.
        /// </summary>
        public AttributeProviderAttribute(string typeName, string propertyName)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException(nameof(typeName));
            }
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            _typeName = typeName;
            _propertyName = propertyName;
        }

        /// <summary>
        ///     Creates a new AttributeProviderAttribute object.
        /// </summary>
        public AttributeProviderAttribute(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _typeName = type.AssemblyQualifiedName;
        }

        /// <summary>
        ///     The TypeName property returns the assembly qualified type name 
        ///     passed into the constructor.
        /// </summary>
        public string TypeName
        {
            get
            {
                return _typeName;
            }
        }

        /// <summary>
        ///     The TypeName property returns the property name that will be used to query attributes from.
        /// </summary>
        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
        }
    }
}

