// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.ComponentModel
{
    /// <devdoc>
    ///    <para> 
    ///       Provides information about a context to a type converter or a value editor,
    ///       so that the type converter or editor can perform a conversion.</para>
    /// </devdoc>
    public interface ITypeDescriptorContext : IServiceProvider
    {
        /// <devdoc>
        ///    <para>Gets the container with the set of objects for this formatter.</para>
        /// </devdoc>
        IContainer Container { get; }

        /// <devdoc>
        ///    <para>Gets the instance that is invoking the method on the formatter object.</para>
        /// </devdoc>
        object Instance { get; }

        /// <devdoc>
        ///      Retrieves the PropertyDescriptor that is surfacing the given context item.
        /// </devdoc>
        PropertyDescriptor PropertyDescriptor { get; }

        /// <devdoc>
        ///    <para>Gets a value indicating whether this object can be changed.</para>
        /// </devdoc>
        bool OnComponentChanging();

        /// <devdoc>
        /// <para>Raises the <see cref='System.ComponentModel.Design.IComponentChangeService.ComponentChanged'/>
        /// event.</para>
        /// </devdoc>
        void OnComponentChanged();
    }
}

