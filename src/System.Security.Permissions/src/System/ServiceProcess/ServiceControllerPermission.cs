// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Permissions;

namespace System.ServiceProcess
{
    public sealed class ServiceControllerPermission : ResourcePermissionBase
    {
        private ServiceControllerPermissionEntryCollection innerCollection;
        
        public ServiceControllerPermission()
        {
            SetNames();
        }

        public ServiceControllerPermission(PermissionState state) : base(state)
        {
            SetNames();
        }

        public ServiceControllerPermission(ServiceControllerPermissionAccess permissionAccess, string machineName, string serviceName)
        {
            SetNames();
            this.AddPermissionAccess(new ServiceControllerPermissionEntry(permissionAccess, machineName, serviceName));
        }

        public ServiceControllerPermission(ServiceControllerPermissionEntry[] permissionAccessEntries)
        {
            if (permissionAccessEntries == null)
                throw new ArgumentNullException(nameof(permissionAccessEntries));

            SetNames();
            for (int index = 0; index < permissionAccessEntries.Length; ++index)
                this.AddPermissionAccess(permissionAccessEntries[index]);
        }

        public ServiceControllerPermissionEntryCollection PermissionEntries
        {
            get
            {
                if (this.innerCollection == null)
                    this.innerCollection = new ServiceControllerPermissionEntryCollection(this, base.GetPermissionEntries());

                return this.innerCollection;
            }
        }

        internal void AddPermissionAccess(ServiceControllerPermissionEntry entry) => base.AddPermissionAccess(entry.GetBaseEntry);

        internal new void Clear() => base.Clear();

        internal void RemovePermissionAccess(ServiceControllerPermissionEntry entry) => base.RemovePermissionAccess(entry.GetBaseEntry);

        private void SetNames()
        {
            this.PermissionAccessType = typeof(ServiceControllerPermissionAccess);
            this.TagNames = new string[]{"Machine", "Service"};
        }
    }
}
