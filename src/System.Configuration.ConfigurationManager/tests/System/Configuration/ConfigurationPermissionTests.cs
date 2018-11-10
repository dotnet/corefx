// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Configuration;
using System.Security;
using System.Security.Permissions;
using Xunit;

namespace System.ConfigurationTests
{
    public class ConfigurationPermissionTests
    {
        [Fact]
        public static void ConfigurationPermissionCallMethods()
        {
            ConfigurationPermission cp = new ConfigurationPermission(new PermissionState());
            bool isunrestricted = cp.IsUnrestricted();
            IPermission other = cp.Copy();
            other = cp.Union(other);
            other = cp.Intersect(other);
            bool isSubsetOf = cp.IsSubsetOf(other);
            SecurityElement se = cp.ToXml();
            cp.FromXml(se);
        }

        [Fact]
        public static void ConfigurationPermissionAttributeCallMethods()
        {
            ConfigurationPermissionAttribute cpa = new ConfigurationPermissionAttribute(new SecurityAction());
            IPermission ip = cpa.CreatePermission();
        }
    }
}