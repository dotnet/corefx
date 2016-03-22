// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal abstract class MethodBase : MemberInfo
    {
        [Obsolete("TODO", error: false)]
        public abstract MethodAttributes Attributes
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public virtual CallingConventions CallingConvention
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual bool ContainsGenericParameters
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsAbstract
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsAssembly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsConstructor
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsFamily
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsFamilyAndAssembly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsFamilyOrAssembly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsFinal
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual bool IsGenericMethod
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual bool IsGenericMethodDefinition
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsHideBySig
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsPrivate
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsPublic
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsSpecialName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsStatic
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsVirtual
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public abstract MethodImplAttributes MethodImplementationFlags
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public virtual Type[] GetGenericArguments()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public static MethodBase GetMethodFromHandle(RuntimeMethodHandle handle)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public static MethodBase GetMethodFromHandle(RuntimeMethodHandle handle, RuntimeTypeHandle declaringType)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public abstract ParameterInfo[] GetParameters();

        [Obsolete("TODO", error: false)]
        public object Invoke(object obj, object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
#endif