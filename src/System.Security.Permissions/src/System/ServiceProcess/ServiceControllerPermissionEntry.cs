// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceProcess
{
    public class ServiceControllerPermissionEntry
    {
        public ServiceControllerPermissionEntry() { MachineName = null; ServiceName = null; PermissionAccess = default(ServiceControllerPermissionAccess); }
        public ServiceControllerPermissionEntry(ServiceControllerPermissionAccess permissionAccess, string machineName, string serviceName) { MachineName = null; ServiceName = null; PermissionAccess = default(ServiceControllerPermissionAccess); }
        public string MachineName { get; }
        public ServiceControllerPermissionAccess PermissionAccess { get; }
        public string ServiceName { get; }
    }
}
