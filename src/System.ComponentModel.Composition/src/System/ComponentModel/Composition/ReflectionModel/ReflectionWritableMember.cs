// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal abstract class ReflectionWritableMember : ReflectionMember
    {
        public abstract bool CanWrite
        {
            get;
        }

        public abstract void SetValue(object instance, object value);
    }
}
