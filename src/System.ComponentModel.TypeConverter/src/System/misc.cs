// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace System
{
    public class Stub
    {
        public static string Assembly_EscapedCodeBase()
        {
            return null;
        }

        public static string AppDomain_CurrentDomain_SetupInformation_LicenseFile()
        {
            //(string)AppDomain.CurrentDomain.SetupInformation.LicenseFile;
            return null;
        }

        public static IEnumerable<Assembly> AppDomain_CurrentDomain_GetAssemblies()
        {
            //AppDomain.CurrentDomain.GetAssemblies()
            return null;
        }

        public static Assembly ResourceManager_MainAssembly()
        {
            //MainAssembly
            return null;
        }

        public static System.Globalization.CultureInfo ResourceManager_GetNeutralResourceLanguage(Assembly assembly)
        {
            //GetNeutralResourcesLanguage(MainAssembly) -- from class derived from ResourceManager 
            return null;
        }

        public static bool ResourceManager_IgnoreCase()
        {
            return false;
        }

        public static System.Resources.ResourceSet ResourceManager_GetResourceSet(System.Globalization.CultureInfo a, bool b, bool c)
        {
            return null;
        }
    }


    [AttributeUsage(AttributeTargets.All)]
    internal sealed class SRDescriptionAttribute : DescriptionAttribute
    {
        private bool _replaced;

        public override string Description
        {
            get
            {
                if (!_replaced)
                {
                    _replaced = true;
                    base.DescriptionValue = SR.GetResourceString(base.Description, "");
                }
                return base.Description;
            }
        }

        public SRDescriptionAttribute(string description) : base(description)
        {
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    internal sealed class SRCategoryAttribute : CategoryAttribute
    {
        public SRCategoryAttribute(string category) : base(category)
        {
        }

        protected override string GetLocalizedString(string value)
        {
            return SR.GetResourceString(value, "");
        }
    }


    // from Misc/SecurityUtils.cs

    internal static class SecurityUtils
    {
        /// <summary>
        ///     This helper method provides safe access to Activator.CreateInstance.
        ///     NOTE: This overload will work only with public .ctors. 
        /// </summary>
        internal static object SecureCreateInstance(Type type)
        {
            return SecureCreateInstance(type, null);
        }

        /// <summary>
        ///     This helper method provides safe access to Activator.CreateInstance.
        ///     Set allowNonPublic to true if you want non public ctors to be used. 
        /// </summary>
        internal static object SecureCreateInstance(Type type, object[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return Activator.CreateInstance(type, args);
        }
    }

    internal static class HResults
    {
        internal const int License = unchecked((int)0x80131901);
    }
}
