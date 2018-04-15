// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Used for associating a metadata class with the entity class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class MetadataTypeAttribute : Attribute {
 
        private Type _metadataClassType;
 
        public Type MetadataClassType {
            get {
                if (_metadataClassType == null) {
                    throw new InvalidOperationException(SR.MetadataTypeAttribute_TypeCannotBeNull);
                }
 
                return _metadataClassType;
            }
        }
 
        public MetadataTypeAttribute(Type metadataClassType) {
            _metadataClassType = metadataClassType;
        }
 
    }
}
