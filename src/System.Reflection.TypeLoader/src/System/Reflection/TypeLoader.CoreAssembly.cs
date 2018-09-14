// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Reflection.TypeLoading;

namespace System.Reflection
{
    public sealed partial class TypeLoader
    {
        //
        // Cache loaded coreAssembly and core types.
        //
        internal RoAssembly TryGetCoreAssembly(out Exception e)
        {
            e = null;
            RoAssembly coreAssembly = _lazyCoreAssembly;
            if (object.ReferenceEquals(coreAssembly, Sentinels.RoAssembly))
            {
                string coreAssemblyName = GetCommittedCoreAssemblyName();
                if (coreAssemblyName == null)
                {
                    e = new InvalidOperationException(SR.NoCoreAssemblyDefined);
                    return null;
                }

                RoAssemblyName roAssemblyName = new AssemblyName(coreAssemblyName).ToRoAssemblyName();
                coreAssembly = _lazyCoreAssembly = TryResolveAssembly(roAssemblyName, out e);
            }
            return coreAssembly;
        }

        private volatile RoAssembly _lazyCoreAssembly = Sentinels.RoAssembly;

        /// <summary>
        /// Returns a lazily created and cached Type instance corresponding to the indicated core type. This method throws 
        /// if the core assembly name wasn't supplied, the core assembly could not be loaded for some reason or if the specified
        /// type does not exist in the core assembly.
        /// </summary>
        internal RoType GetCoreType(CoreType coreType)
        {
            CoreTypes coreTypes = GetAllFoundCoreTypes();
            RoType t = TryGetCoreType(coreType);
            return t ?? throw coreTypes.GetException(coreType);
        }

        /// <summary>
        /// Returns a lazily created and cached Type instance corresponding to the indicated core type. This method returns null
        /// if the core assembly name wasn't supplied, the core assembly could not be loaded for some reason or if the specified
        /// type does not exist in the core assembly.
        /// </summary>
        internal RoType TryGetCoreType(CoreType coreType)
        {
            CoreTypes coreTypes = GetAllFoundCoreTypes();
            return coreTypes[coreType];
        }

        /// <summary>
        /// Returns a lazily created and cached array containing the resolved CoreTypes, indexed by the CoreType enum cast to an int.
        /// If the core assembly was not specified, not locatable or if one or more core types aren't present in the core assembly,
        /// the corresponding elements will be null.
        /// </summary>
        internal CoreTypes GetAllFoundCoreTypes() => _lazyCoreTypes ?? (_lazyCoreTypes = new CoreTypes(this));
        private volatile CoreTypes _lazyCoreTypes;

        /// <summary>
        /// Calling this commits the TypeLoader to use the currently set CoreAssemblyName. The CoreAssemblyName property can no longer be set after this.
        /// </summary>
        private string GetCommittedCoreAssemblyName()
        {
            Interlocked.CompareExchange(ref _lazyCommitedCoreAssemblyName, CoreAssemblyName, s_committedCoreAssemblyNameSentinel);
            return (string)_lazyCommitedCoreAssemblyName;
        }

        private volatile object _lazyCommitedCoreAssemblyName = s_committedCoreAssemblyNameSentinel;
        private static readonly object s_committedCoreAssemblyNameSentinel = new object();

        //
        // Seriously, ugh - the default binder for Reflection has a dependency on checking types for equality with System.Object - for that
        // one reason, we have to instance it per TypeLoader.
        //
        internal Binder GetDefaultBinder() => _lazyDefaultBinder ?? (_lazyDefaultBinder = new DefaultBinder(this));
        private volatile Binder _lazyDefaultBinder;
    }
}
