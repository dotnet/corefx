// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal sealed class PropertyReference : PropertyInfo
    {
        private readonly TypeInfo _containingType;
        private readonly Reflection.PropertyInfo _info;

        public PropertyReference(TypeInfo containingType, Reflection.PropertyInfo info)
        {
            _containingType = containingType;
            _info = info;
        }

        public override Module Module => _containingType.Module;

        [Obsolete("TODO", error: false)]
        public override PropertyAttributes Attributes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override bool CanRead
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override bool CanWrite
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override Type DeclaringType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Name => _info.Name;

        [Obsolete("TODO", error: false)]
        public override Type PropertyType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override ParameterInfo[] GetIndexParameters()
        {
            throw new NotImplementedException();
        }
    }
}
#endif
