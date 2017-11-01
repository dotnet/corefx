// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Reflection;
using Microsoft.Internal;
using System.Threading;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal abstract class ReflectionMember : ReflectionItem
    {
        public abstract bool CanRead
        {
            get;
        }
        
        public Type DeclaringType
        {
            get { return this.UnderlyingMember.DeclaringType; }
        }

        public override string Name
        {
            get { return this.UnderlyingMember.Name; }
        }

        public override string GetDisplayName()
        {
            return this.UnderlyingMember.GetDisplayName();
        }

        public abstract bool RequiresInstance
        {
            get;
        }

        public abstract MemberInfo UnderlyingMember { get; }

        public abstract object GetValue(object instance);
    }
}
