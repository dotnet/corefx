// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Permissions;

namespace System.ServiceProcess
{
    public sealed class ServiceControllerPermission : ResourcePermissionBase
    {
        public ServiceControllerPermission() { PermissionEntries = null; }
        public ServiceControllerPermission(PermissionState state) : base(state) { PermissionEntries = null; }
        public ServiceControllerPermission(ServiceControllerPermissionAccess permissionAccess, string machineName, string serviceName) { PermissionEntries = null; }
        public ServiceControllerPermission(ServiceControllerPermissionEntry[] permissionAccessEntries) { PermissionEntries = null; }
        public ServiceControllerPermissionEntryCollection PermissionEntries { get; }
    }
}
