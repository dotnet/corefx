// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal class ReflectionField : ReflectionWritableMember
    {
        public ReflectionField(FieldInfo field)
        {
            UnderlyingField = field ?? throw new ArgumentNullException(nameof(field));
        }

        public FieldInfo UnderlyingField { get; }

        public override MemberInfo UnderlyingMember => UnderlyingField;

        public override bool CanRead => true;

        public override bool CanWrite => !UnderlyingField.IsInitOnly;

        public override bool RequiresInstance => !UnderlyingField.IsStatic;

        public override Type ReturnType => UnderlyingField.FieldType;

        public override ReflectionItemType ItemType => ReflectionItemType.Field;

        public override object GetValue(object instance) => UnderlyingField.GetValue(instance);

        public override void SetValue(object instance, object value)
        {
            UnderlyingField.SetValue(instance, value);
        }
    }
}
