// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.ComponentModel
{
    /// <summary>
    /// Provides a read-only container for a collection of <see cref="IComponent" />
    /// objects.
    /// </summary>
    public partial class ComponentCollection
    {
        internal ComponentCollection() { }
    }
    /// <summary>
    /// Provides functionality required by all components.
    /// </summary>
    public partial interface IComponent : System.IDisposable
    {
        /// <summary>
        /// Gets or sets the <see cref="System.ComponentModel.ISite" /> associated with the
        /// <see cref="System.ComponentModel.IComponent" />.
        /// </summary>
        /// <returns>
        /// The <see cref="System.ComponentModel.ISite" /> object associated with the component; or
        /// null, if the component does not have a site.
        /// </returns>
        System.ComponentModel.ISite Site { get; set; }
        /// <summary>
        /// Represents the method that handles the <see cref="Disposed" />
        /// event of a component.
        /// </summary>
        event System.EventHandler Disposed;
    }
    /// <summary>
    /// Provides functionality for containers. Containers are objects that logically contain zero or
    /// more components.
    /// </summary>
    public partial interface IContainer : System.IDisposable
    {
        /// <summary>
        /// Gets all the components in the <see cref="System.ComponentModel.IContainer" />.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="System.ComponentModel.IComponent" /> objects that represents
        /// all the components in the <see cref="System.ComponentModel.IContainer" />.
        /// </returns>
        System.ComponentModel.ComponentCollection Components { get; }
        /// <summary>
        /// Adds the specified <see cref="IComponent" /> to the
        /// <see cref="IContainer" /> at the end of the list.
        /// </summary>
        /// <param name="component">The <see cref="IComponent" /> to add.</param>
        void Add(System.ComponentModel.IComponent component);
        /// <summary>
        /// Adds the specified <see cref="IComponent" /> to the
        /// <see cref="IContainer" /> at the end of the list, and assigns a name to the component.
        /// </summary>
        /// <param name="component">The <see cref="IComponent" /> to add.</param>
        /// <param name="name">
        /// The unique, case-insensitive name to assign to the component.-or- null that leaves the component
        /// unnamed.
        /// </param>
        void Add(System.ComponentModel.IComponent component, string name);
        /// <summary>
        /// Removes a component from the <see cref="IContainer" />.
        /// </summary>
        /// <param name="component">The <see cref="IComponent" /> to remove.</param>
        void Remove(System.ComponentModel.IComponent component);
    }
    /// <summary>
    /// Provides functionality required by sites.
    /// </summary>
    public partial interface ISite : System.IServiceProvider
    {
        /// <summary>
        /// Gets the component associated with the <see cref="System.ComponentModel.ISite" /> when implemented
        /// by a class.
        /// </summary>
        /// <returns>
        /// The <see cref="System.ComponentModel.IComponent" /> instance associated with the
        /// <see cref="System.ComponentModel.ISite" />.
        /// </returns>
        System.ComponentModel.IComponent Component { get; }
        /// <summary>
        /// Gets the <see cref="System.ComponentModel.IContainer" /> associated with the
        /// <see cref="System.ComponentModel.ISite" /> when implemented by a class.
        /// </summary>
        /// <returns>
        /// The <see cref="System.ComponentModel.IContainer" /> instance associated with the
        /// <see cref="System.ComponentModel.ISite" />.
        /// </returns>
        System.ComponentModel.IContainer Container { get; }
        /// <summary>
        /// Determines whether the component is in design mode when implemented by a class.
        /// </summary>
        /// <returns>
        /// true if the component is in design mode; otherwise, false.
        /// </returns>
        bool DesignMode { get; }
        /// <summary>
        /// Gets or sets the name of the component associated with the <see cref="ISite" />
        /// when implemented by a class.
        /// </summary>
        /// <returns>
        /// The name of the component associated with the <see cref="ISite" />;
        /// or null, if no name is assigned to the component.
        /// </returns>
        string Name { get; set; }
    }
}
