// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Security.Permissions;
using System.Globalization;

namespace System.ServiceProcess
{
    [Serializable] 
    public class ServiceControllerPermissionEntry
    {
        private string machineName;
        private string serviceName;
        private ServiceControllerPermissionAccess permissionAccess;

        public ServiceControllerPermissionEntry()
        {
            this.machineName = ".";
            this.serviceName = "*";
            this.permissionAccess = ServiceControllerPermissionAccess.Browse;
        }

        public ServiceControllerPermissionEntry(ServiceControllerPermissionAccess permissionAccess, string machineName, string serviceName)
        {
            if (serviceName == null)
                throw new ArgumentNullException("serviceName");

            if (!ServiceBase.ValidServiceName(serviceName))
               throw new ArgumentException(string.Format(SR.ServiceName, serviceName, ServiceBase.MaxNameLength.ToString(CultureInfo.CurrentCulture)));
            
            if (!SyntaxCheck.CheckMachineName(machineName))
                throw new ArgumentException(string.Format(SR.BadMachineName, machineName));

            this.permissionAccess = permissionAccess;
            this.machineName = machineName;
            this.serviceName = serviceName;
        }  
        
        internal ServiceControllerPermissionEntry(ResourcePermissionBaseEntry baseEntry)
        {
            this.permissionAccess = (ServiceControllerPermissionAccess)baseEntry.PermissionAccess;
            this.machineName = baseEntry.PermissionAccessPath[0]; 
            this.serviceName = baseEntry.PermissionAccessPath[1];  
        }

        public string MachineName
        {
            get
            {
                return this.machineName;
            }
        }
        
        public ServiceControllerPermissionAccess PermissionAccess
        {
            get
            {
                return this.permissionAccess;
            }
        }   
        
        public string ServiceName
        {
            get
            {
                return this.serviceName;
            }
        }
        
        internal ResourcePermissionBaseEntry GetBaseEntry()
        {
            ResourcePermissionBaseEntry baseEntry = new ResourcePermissionBaseEntry((int)this.PermissionAccess, new string[] {this.MachineName, this.ServiceName});
            return baseEntry;
        }
    }
}
