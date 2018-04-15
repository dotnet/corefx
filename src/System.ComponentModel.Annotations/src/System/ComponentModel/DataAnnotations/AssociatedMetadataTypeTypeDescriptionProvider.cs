// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel.DataAnnotations
{
    public class AssociatedMetadataTypeTypeDescriptionProvider : TypeDescriptionProvider
    {
        private Type _associatedMetadataType;
        public AssociatedMetadataTypeTypeDescriptionProvider(Type type)
            : base(TypeDescriptor.GetProvider(type))
        {
        }

        public AssociatedMetadataTypeTypeDescriptionProvider(Type type, Type associatedMetadataType)
            : this(type)
        {
            if (associatedMetadataType == null)
            {
                throw new ArgumentNullException("associatedMetadataType");
            }

            _associatedMetadataType = associatedMetadataType;
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            ICustomTypeDescriptor baseDescriptor = base.GetTypeDescriptor(objectType, instance);
            return new AssociatedMetadataTypeTypeDescriptor(baseDescriptor, objectType, _associatedMetadataType);
        }
    }
}
