// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

#if FEATURE_REFLECTIONCONTEXT

using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Microsoft.Internal;

using PermissionSet = System.Security.PermissionSet;
using PermissionState = System.Security.Permissions.PermissionState;


namespace System.ComponentModel.Composition
{
    /// <summary>
    ///     Enables the AssemblyCatalog to discover user provided ReflectionContexts.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes")]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false,Inherited = true)]
    public class CatalogReflectionContextAttribute : Attribute
    {
        Type _reflectionContextType;

        public CatalogReflectionContextAttribute(Type reflectionContextType)
        {
            Requires.NotNull(reflectionContextType, "reflectionContextType");

            this._reflectionContextType = reflectionContextType;
        }

        public ReflectionContext CreateReflectionContext()
        {
            Assumes.NotNull<Type>(this._reflectionContextType);

            if (!this._reflectionContextType.IsPublic)
            {
                new PermissionSet(PermissionState.Unrestricted).Demand();
            }

            ReflectionContext reflectionContext = null;
            try
            {
                reflectionContext = (ReflectionContext)Activator.CreateInstance(this._reflectionContextType);
            }
            catch (InvalidCastException invalidCastException)
            {
                throw new InvalidOperationException(SR.ReflectionContext_Type_Required, invalidCastException);
            }
            catch (MissingMethodException missingMethodException)
            {
                throw new MissingMethodException(SR.ReflectionContext_Requires_DefaultConstructor, missingMethodException);
            }

            return reflectionContext;
        }
    }
}
#endif //FEATURE_REFLECTIONCONTEXT
