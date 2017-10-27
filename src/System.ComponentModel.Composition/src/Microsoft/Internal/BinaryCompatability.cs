//------------------------------------------------------------------------------
// <copyright file="BinaryCompatibility.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------
using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.Versioning;
using System.Security;

namespace Microsoft.Internal
{
    // This class contains utility methods that mimic the mscorlib internal System.Runtime.Versioning.BinaryCompatibility type.
    internal sealed class BinaryCompatibility 
    {
        private static readonly Version Framework45 = new Version(4, 5);
        private static readonly BinaryCompatibility Current = new BinaryCompatibility();

        // Set the targetframework by:
        // 1.   Looking at the targetFramework provided to: AppDomainSetup
        // 2.   Failing that look at the Assemby executed with the AppDomain
        // failing that then TargetsAtLeast_Desktop_V4_5 is set to true
        public BinaryCompatibility()
        {
            string targetFrameworkName = null; // UNDONE UNDONE AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName;
            if(string.IsNullOrEmpty(targetFrameworkName))
            {
                targetFrameworkName = GetTargetFrameworkNameFromEntryAssembly();
                if(string.IsNullOrEmpty(targetFrameworkName))
                {
                    TargetsAtLeast_Desktop_V4_5 = false;
                    return;
                }
            }

            var frameworkName = new FrameworkName(targetFrameworkName);
            TargetsAtLeast_Desktop_V4_5 = (frameworkName.Version >= BinaryCompatibility.Framework45);
        }

        public static bool TargetsAtLeast_Desktop_V4_5 { get; private set; }

        [SecuritySafeCritical]
        static string GetTargetFrameworkNameFromEntryAssembly()
        {
            string targetFrameworkName = null;

            Assembly assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                TargetFrameworkAttribute[] attrs = (TargetFrameworkAttribute[])assembly.GetCustomAttributes(typeof(TargetFrameworkAttribute));
                if (attrs != null && attrs.Length > 0)
                {
                    Contract.Assert(attrs.Length == 1);
                    targetFrameworkName = attrs[0].FrameworkName;
                }
            }

            return targetFrameworkName;
        }
    }
}

