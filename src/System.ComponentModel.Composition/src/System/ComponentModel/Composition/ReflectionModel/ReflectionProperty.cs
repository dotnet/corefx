// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

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
        public ReflectionProperty(MethodInfo getMethod, MethodInfo setMethod)
        {
            if (getMethod == null && setMethod == null)
            {
                throw new Exception(SR.Diagnostic_InternalExceptionMessage);
            }

            UnderlyingGetMethod = getMethod;
            UnderlyingSetMethod = setMethod;
        }

        public override MemberInfo UnderlyingMember => (UnderlyingGetMethod ?? UnderlyingSetMethod);

        public override bool CanRead => UnderlyingGetMethod != null;

        public override bool CanWrite => UnderlyingSetMethod != null;

        public override ReflectionItemType ItemType => ReflectionItemType.Property;

        public MethodInfo UnderlyingGetMethod { get; }

        public MethodInfo UnderlyingSetMethod { get; }

        public override string Name
        {
            get
            {
                MethodInfo method = UnderlyingGetMethod ?? UnderlyingSetMethod;

                string name = method.Name;

                if (name.Length <= 4)
                {
                    throw new Exception(SR.Diagnostic_InternalExceptionMessage);
                }


                // Remove 'get_' or 'set_'
                return name.Substring(4);
            }
        }

        public override string GetDisplayName()
        {
            return ReflectionServices.GetDisplayName(DeclaringType, Name);
        }

        public override bool RequiresInstance
        {
            get
            {
                MethodInfo method = UnderlyingGetMethod ?? UnderlyingSetMethod;

                return !method.IsStatic;
            }
        }

        public override Type ReturnType
        {
            get
            {
                if (UnderlyingGetMethod != null)
                {
                    return UnderlyingGetMethod.ReturnType;
                }

                ParameterInfo[] parameters = UnderlyingSetMethod.GetParameters();

                if (parameters.Length == 0)
                {
                    throw new Exception(SR.Diagnostic_InternalExceptionMessage);
                }
                return parameters[parameters.Length - 1].ParameterType;
            }
        }        

        public override object GetValue(object instance)
        {
            if (UnderlyingGetMethod == null)
            {
                throw new Exception(SR.Diagnostic_InternalExceptionMessage);
            }

            return UnderlyingGetMethod.Invoke(instance, null);
        }

        public override void SetValue(object instance, object value)
        {
            if (UnderlyingSetMethod == null)
            {
                throw new Exception(SR.Diagnostic_InternalExceptionMessage);
            }
            UnderlyingSetMethod.Invoke(instance, new object[] { value });
        }
    }
}
