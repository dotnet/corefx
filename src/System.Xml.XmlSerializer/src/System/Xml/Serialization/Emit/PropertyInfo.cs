// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal abstract class PropertyInfo : MemberInfo
    {
        [Obsolete("TODO", error: false)]
        public abstract PropertyAttributes Attributes
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public abstract bool CanRead
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public abstract bool CanWrite
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public virtual MethodInfo GetMethod
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
        public abstract Type PropertyType
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public virtual MethodInfo SetMethod
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual object GetConstantValue()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public abstract ParameterInfo[] GetIndexParameters();

        [Obsolete("TODO", error: false)]
        public object GetValue(object obj)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public virtual object GetValue(object obj, object[] index)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public void SetValue(object obj, object value)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void SetValue(object obj, object value, object[] index)
        {
            throw new NotImplementedException();
        }
    }
}
#endif
