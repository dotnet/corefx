// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal abstract class EventInfo : MemberInfo
    {
        [Obsolete("TODO", error: false)]
        public virtual MethodInfo AddMethod
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public abstract EventAttributes Attributes
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public virtual Type EventHandlerType
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
        public virtual MethodInfo RaiseMethod
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual MethodInfo RemoveMethod
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        internal EventInfo()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void AddEventHandler(object target, Delegate handler)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public virtual void RemoveEventHandler(object target, Delegate handler)
        {
            throw new NotImplementedException();
        }
    }
}
#endif
