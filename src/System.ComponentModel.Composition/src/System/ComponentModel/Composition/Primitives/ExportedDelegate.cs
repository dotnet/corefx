// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Internal;
using System.Linq.Expressions;

namespace System.ComponentModel.Composition.Primitives
{
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public class ExportedDelegate
    {
        private object _instance;
        private MethodInfo _method;

        protected ExportedDelegate() { }

#if FEATURE_CAS_APTCA
        [System.Security.SecurityCritical]
#endif //FEATURE_CAS_APTCA
        public ExportedDelegate(object instance, MethodInfo method)
        {
            Requires.NotNull(method, "method");

            this._instance = instance;
            this._method = method;
        }

        public virtual Delegate CreateDelegate(Type delegateType) 
        {
            Requires.NotNull(delegateType, "delegateType");

            if (delegateType == typeof(Delegate) || delegateType == typeof(MulticastDelegate))
            {
                delegateType = this.CreateStandardDelegateType();
            }
            try
            { 
                return this._method.CreateDelegate(delegateType, this._instance);
            }
            catch(ArgumentException)
            {
                //Bind failure occurs return null;
                return null;
            }
        }

        private Type CreateStandardDelegateType()
        {
            ParameterInfo[] parameters = this._method.GetParameters();

            // This array should contains a lit of all argument types, and the last one is the return type (could be void)
            Type[] parameterTypes = new Type[parameters.Length + 1];
            parameterTypes[parameters.Length] = this._method.ReturnType;
            for (int i = 0; i < parameters.Length; i++ )
            {
                parameterTypes[i] = parameters[i].ParameterType;
            }

            return Expression.GetDelegateType(parameterTypes);
        }
    }
}
