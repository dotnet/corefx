// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal sealed class FieldReference : FieldInfo
    {
        private readonly Reflection.FieldInfo _info;
        private readonly TypeInfo _containingType;

        public FieldReference(TypeInfo containingType, Reflection.FieldInfo info)
        {
            _info = info;
            _containingType = containingType;
        }

        public override Module Module => _containingType.Module;

        [Obsolete("TODO", error: false)]
        public override Type DeclaringType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override Type FieldType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Name => _info.Name;
    }
}
#endif
