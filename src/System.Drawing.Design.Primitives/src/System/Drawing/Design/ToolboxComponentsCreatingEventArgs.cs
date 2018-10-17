// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Drawing.Design {
    using System;
    using System.ComponentModel.Design;


    /// <summary>
    /// Provides data for the 'ToolboxComponentsCreatingEventArgs' event that occurs
    /// when components are added to the toolbox.
    /// </summary>
    [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.InheritanceDemand, Name="FullTrust")]
    [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.LinkDemand, Name="FullTrust")]
    public class ToolboxComponentsCreatingEventArgs : EventArgs {
        private readonly IDesignerHost host;

        /// <summary>
        /// Initializes a new instance of the <see cref='System.Drawing.Design.ToolboxComponentsCreatingEventArgs'/> object.
        /// </summary>
        /// <param name="host">host</param>
        public ToolboxComponentsCreatingEventArgs(IDesignerHost host) {
            this.host = host;
        }

        /// <summary>
        /// An instance of IDesignerHost that has made the creat request.  This can be null if no designer host
        /// was provided to the toolbox item.
        /// </summary>
        public IDesignerHost DesignerHost {
            get {
                return host;
            }
        }
    }
}
