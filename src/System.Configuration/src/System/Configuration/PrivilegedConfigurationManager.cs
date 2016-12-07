// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Permissions;

namespace System.Configuration
{

    [ConfigurationPermission(SecurityAction.Assert, Unrestricted = true)]
    internal static class PrivilegedConfigurationManager
    {
        internal static ConnectionStringSettingsCollection ConnectionStrings
        {
            [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            get
            {
                return ConfigurationManager.ConnectionStrings;
            }
        }

        internal static object GetSection(string sectionName)
        {
            return ConfigurationManager.GetSection(sectionName);
        }
    }
}
