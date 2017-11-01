// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Reflection;
using Microsoft.Internal;
using System.Threading;

namespace System.ComponentModel.Composition.ReflectionModel
{
    // Instead of representing properties as an actual PropertyInfo, we need to 
    // represent them as two MethodInfo objects one for each accessor. This is so 
    // that cached attribute part can go from a metadata token -> XXXInfo without 
    // needing to walk all members of a particular type. Unfortunately, (probably 
    // because you never see one of them in an IL stream), Reflection does not allow 
    // you to go from a metadata token -> PropertyInfo like it does with types, 
    // fields, and methods.

    internal class ReflectionProperty : ReflectionWritableMember
    {
        private readonly MethodInfo _getMethod;
        private readonly MethodInfo _setMethod;

        public ReflectionProperty(MethodInfo getMethod, MethodInfo setMethod)
        {
            Assumes.IsTrue(getMethod != null || setMethod != null);

            this._getMethod = getMethod;
            this._setMethod = setMethod;
        }

        public override MemberInfo UnderlyingMember
        {
            get { return this.UnderlyingGetMethod ?? this.UnderlyingSetMethod; }
        }

        public override bool CanRead
        {
            get { return this.UnderlyingGetMethod != null; }
        }

        public override bool CanWrite
        {
            get { return this.UnderlyingSetMethod != null; }
        }

        public MethodInfo UnderlyingGetMethod
        {
            get { return this._getMethod; }
        }

        public MethodInfo UnderlyingSetMethod
        {
            get { return this._setMethod; }
        }

        public override string Name
        {
            get
            {
                MethodInfo method = this.UnderlyingGetMethod ?? this.UnderlyingSetMethod;

                string name = method.Name;

                Assumes.IsTrue(name.Length > 4);

                // Remove 'get_' or 'set_'
                return name.Substring(4);
            }
        }

        public override string GetDisplayName()
        {
            return ReflectionServices.GetDisplayName(this.DeclaringType, this.Name);
        }

        public override bool RequiresInstance
        {
            get
            {
                MethodInfo method = this.UnderlyingGetMethod ?? this.UnderlyingSetMethod;

                return !method.IsStatic;
            }
        }

        public override Type ReturnType
        {
            get
            {
                if (this.UnderlyingGetMethod != null)
                {
                    return this.UnderlyingGetMethod.ReturnType;
                }

                ParameterInfo[] parameters = this.UnderlyingSetMethod.GetParameters();

                Assumes.IsTrue(parameters.Length > 0);

                return parameters[parameters.Length - 1].ParameterType;
            }
        }

        public override ReflectionItemType ItemType
        {
            get { return ReflectionItemType.Property; }
        }

        public override object GetValue(object instance)
        {
            Assumes.NotNull(this._getMethod);

            return this.UnderlyingGetMethod.SafeInvoke(instance);
        }

        public override void SetValue(object instance, object value)
        {
            Assumes.NotNull(this._setMethod);

            this.UnderlyingSetMethod.SafeInvoke(instance, value);
        }

    }
}
