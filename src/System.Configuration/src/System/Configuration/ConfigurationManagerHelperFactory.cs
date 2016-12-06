// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Configuration.Internal;

namespace System.Configuration
{
    internal static class ConfigurationManagerHelperFactory
    {
        private const string ConfigurationManagerHelperTypeString =
            "System.Configuration.Internal.ConfigurationManagerHelper, " + AssemblyRef.System;

        private static volatile IConfigurationManagerHelper s_instance;

        internal static IConfigurationManagerHelper Instance
            => s_instance ?? (s_instance = CreateConfigurationManagerHelper());

        private static IConfigurationManagerHelper CreateConfigurationManagerHelper()
        {
            return TypeUtil.CreateInstance<IConfigurationManagerHelper>(ConfigurationManagerHelperTypeString);
        }
    }
}