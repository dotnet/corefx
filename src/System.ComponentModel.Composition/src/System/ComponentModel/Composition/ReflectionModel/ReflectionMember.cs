// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal abstract class ReflectionMember : ReflectionItem
    {
        public abstract bool CanRead { get; }

        public Type DeclaringType => UnderlyingMember.DeclaringType;

        public override string Name => UnderlyingMember.Name;

        public override string GetDisplayName() => UnderlyingMember.GetDisplayName();

        public abstract bool RequiresInstance { get; }

        public abstract MemberInfo UnderlyingMember { get; }

        public abstract object GetValue(object instance);
    }
}
