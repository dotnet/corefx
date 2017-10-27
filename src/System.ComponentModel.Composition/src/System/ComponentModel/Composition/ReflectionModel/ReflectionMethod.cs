// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using Microsoft.Internal;
using System.Threading;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal partial class ReflectionMethod : ReflectionMember
    {
        private readonly MethodInfo _method;

        public ReflectionMethod(MethodInfo method)
        {
            Assumes.NotNull(method);

            this._method = method;
        }

        public MethodInfo UnderlyingMethod
        {
            get { return this._method; }
        }

        public override MemberInfo UnderlyingMember
        {
            get { return this.UnderlyingMethod; }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool RequiresInstance
        {
            get { return !this.UnderlyingMethod.IsStatic; }
        }

        public override Type ReturnType
        {
            get { return this.UnderlyingMethod.ReturnType; }
        }

        public override ReflectionItemType ItemType
        {
            get { return ReflectionItemType.Method; }
        }

        public override object GetValue(object instance)
        {
            return SafeCreateExportedDelegate(instance, _method);
        }

#if FEATURE_CAS_APTCA
        [System.Security.SecuritySafeCritical]
#endif //FEATURE_CAS_APTCA
        private static ExportedDelegate SafeCreateExportedDelegate(object instance, MethodInfo method)
        {
            // We demand member access in place of the [SecurityCritical] 
            // attribute on ExportDelegate constructor
            ReflectionInvoke.DemandMemberAccessIfNeeded(method);

            return new ExportedDelegate(instance, method);
        }
    }
}
