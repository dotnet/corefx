// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Security;
using System.Security.Permissions;
using System.Globalization;

namespace System.ServiceProcess
{    
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly | AttributeTargets.Event, AllowMultiple = true, Inherited = false )]
    [Serializable]
    public class ServiceControllerPermissionAttribute : CodeAccessSecurityAttribute
    {
        private string machineName;
        private string serviceName;
        private ServiceControllerPermissionAccess permissionAccess;

        public ServiceControllerPermissionAttribute(SecurityAction action): base(action)
        {
            this.machineName = ".";
            this.serviceName = "*";
            this.permissionAccess = ServiceControllerPermissionAccess.Browse;
        }        

        public string MachineName
        {
            get
            {
                return this.machineName;
            }
            
            set
            {
                if (!SyntaxCheck.CheckMachineName(value))
                    throw new ArgumentException(string.Format(SR.BadMachineName, value));
                    
                this.machineName = value;                    
            }
        }
        
        public ServiceControllerPermissionAccess PermissionAccess
        {
            get
            {
                return this.permissionAccess;
            }
            
            set
            {
                this.permissionAccess = value;
            }
        }   
        
        public string ServiceName
        {
            get
            {
                return this.serviceName;
            }
            
            set {                                                                                                                                                                  
                if (value == null)
                    throw new ArgumentNullException("value");

                if (!ServiceBase.ValidServiceName(value))
                   throw new ArgumentException(string.Format(SR.ServiceName, value, ServiceBase.MaxNameLength.ToString(CultureInfo.CurrentCulture)));                                                
                                    
                this.serviceName = value;                                    
            }
        }                         
              
        public override IPermission CreatePermission()
        {      
            if (Unrestricted) 
                return new ServiceControllerPermission(PermissionState.Unrestricted);
            
            return new ServiceControllerPermission(this.PermissionAccess, this.MachineName, this.ServiceName);
        }
    }    
}

