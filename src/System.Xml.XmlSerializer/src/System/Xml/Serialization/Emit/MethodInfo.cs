// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal abstract class MethodInfo : MethodBase
    {
        [Obsolete("TODO", error: false)]
        public virtual ParameterInfo ReturnParameter
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual Type ReturnType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual Delegate CreateDelegate(Type delegateType)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public virtual Delegate CreateDelegate(Type delegateType, object target)
        {
            throw new NotImplementedException();
        }
        [Obsolete("TODO", error: false)]
        public override Type[] GetGenericArguments()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public virtual MethodInfo GetGenericMethodDefinition()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public virtual MethodInfo MakeGenericMethod(params Type[] typeArguments)
        {
            throw new NotImplementedException();
        }
    }
}
#endif