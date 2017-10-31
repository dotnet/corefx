// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Reflection;
using Microsoft.Internal;
using System.Threading;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal class ReflectionField : ReflectionWritableMember
    {
        private readonly FieldInfo _field;

        public ReflectionField(FieldInfo field)
        {
            Assumes.NotNull(field);

            this._field = field;
        }

        public FieldInfo UndelyingField
        {
            get { return this._field; }
        }

        public override MemberInfo UnderlyingMember
        {
            get { return this.UndelyingField; }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return !this.UndelyingField.IsInitOnly; }
        }

        public override bool RequiresInstance
        {
            get { return !this.UndelyingField.IsStatic; }
        }

        public override Type ReturnType
        {
            get { return this.UndelyingField.FieldType; }
        }

        public override ReflectionItemType ItemType
        {
            get { return ReflectionItemType.Field; }
        }

        public override object GetValue(object instance)
        {
            return this.UndelyingField.SafeGetValue(instance);
        }

        public override void SetValue(object instance, object value)
        {
            this.UndelyingField.SafeSetValue(instance, value);
        }
    }
}
