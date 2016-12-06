// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Permissions;

namespace System.ComponentModel.Design
{
    /// <summary>
    /// <para>Provides data for the <see cref='System.ComponentModel.Design.IComponentChangeService.ComponentRename'/> event.</para>
    /// </summary>
    public class ComponentRenameEventArgs : EventArgs
    {
        private object _component;
        private string _oldName;
        private string _newName;

        /// <summary>
        ///    <para>
        ///       Gets or sets the component that is being renamed.
        ///    </para>
        /// </summary>
        public object Component
        {
            get
            {
                return _component;
            }
        }

        /// <summary>
        ///    <para>
        ///       Gets or
        ///       sets the name of the component before the rename.
        ///    </para>
        /// </summary>
        public virtual string OldName
        {
            get
            {
                return _oldName;
            }
        }

        /// <summary>
        ///    <para>
        ///       Gets or
        ///       sets the current name of the component.
        ///    </para>
        /// </summary>
        public virtual string NewName
        {
            get
            {
                return _newName;
            }
        }

        /// <summary>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.ComponentModel.Design.ComponentRenameEventArgs'/>
        ///       class.
        ///    </para>
        /// </summary>
        public ComponentRenameEventArgs(object component, string oldName, string newName)
        {
            _oldName = oldName;
            _newName = newName;
            _component = component;
        }
    }
}
