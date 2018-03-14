// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Permissions;

namespace System.ServiceProcess
{
    public class ServiceControllerPermissionEntry
    {
        public ServiceControllerPermissionEntry() { }
        internal ServiceControllerPermissionEntry(ResourcePermissionBaseEntry baseEntry) { }
        public ServiceControllerPermissionEntry(ServiceControllerPermissionAccess permissionAccess, string machineName, string serviceName) { }
        public string MachineName { get; }
        public ServiceControllerPermissionAccess PermissionAccess { get; }
        public string ServiceName { get; }
        internal ResourcePermissionBaseEntry GetBaseEntry() { return default(ResourcePermissionBaseEntry); }
    }
}
