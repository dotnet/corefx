// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Security.Permissions;
using System.Globalization;

namespace System.ServiceProcess.ServiceController
{
    [Serializable] 
    public class ServiceControllerPermissionEntry
    {
        public ServiceControllerPermissionEntry() : this(ServiceControllerPermissionAccess.Browse, ".", "*")
        {
        }

        internal ServiceControllerPermissionEntry(ResourcePermissionBaseEntry baseEntry) : this((ServiceControllerPermissionAccess)baseEntry.PermissionAccess, baseEntry.PermissionAccessPath[0], baseEntry.PermissionAccessPath[1])
        {
        }

        public ServiceControllerPermissionEntry(ServiceControllerPermissionAccess permissionAccess, string machineName, string serviceName)
        {
            if (serviceName == null)
                throw new ArgumentNullException("serviceName");

            if (!Helpers.ValidServiceName(serviceName))
               throw new ArgumentException(string.Format(SR.ServiceName, serviceName, Helpers.MaxNameLength.ToString(CultureInfo.CurrentCulture)));
            
            if (!SyntaxCheck.CheckMachineName(machineName))
                throw new ArgumentException(string.Format(SR.BadMachineName, machineName));

            this.PermissionAccess = permissionAccess;
            this.MachineName = machineName;
            this.ServiceName = serviceName;
        }  

        public string MachineName { get; }
        
        public ServiceControllerPermissionAccess PermissionAccess { get; }
        
        public string ServiceName { get; }
        
        internal ResourcePermissionBaseEntry GetBaseEntry => new ResourcePermissionBaseEntry((int)this.PermissionAccess, new string[] {this.MachineName, this.ServiceName});
    }
}
