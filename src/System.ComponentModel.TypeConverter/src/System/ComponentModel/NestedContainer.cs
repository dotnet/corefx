// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Security.Permissions;

namespace System.ComponentModel
{
    /// <summary>
    ///     A nested container is a container that is owned by another component.  Nested
    ///     containers can be found by querying a component site's services for NestedConainter.
    ///     Nested containers are a useful tool to establish owner relationships among components.
    ///     All components within a nested container are named with the owning component's name
    ///     as a prefix.
    /// </summary>
    public class NestedContainer : Container, INestedContainer
    {
        private IComponent _owner;

        /// <summary>
        ///     Creates a new NestedContainer.
        /// </summary>
        public NestedContainer(IComponent owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }
            _owner = owner;
            _owner.Disposed += new EventHandler(OnOwnerDisposed);
        }

        /// <summary>
        ///     The component that owns this nested container.
        /// </summary>
        public IComponent Owner
        {
            get
            {
                return _owner;
            }
        }

        /// <summary>
        ///     Retrieves the name of the owning component.  This may be overridden to
        ///     provide a custom owner name.  The default searches the owner's site for
        ///     INestedSite and calls FullName, or ISite.Name if there is no nested site.
        ///     If neither is available, this returns null.
        /// </summary>
        protected virtual string OwnerName
        {
            get
            {
                string ownerName = null;
                if (_owner != null && _owner.Site != null)
                {
                    INestedSite nestedOwnerSite = _owner.Site as INestedSite;
                    if (nestedOwnerSite != null)
                    {
                        ownerName = nestedOwnerSite.FullName;
                    }
                    else
                    {
                        ownerName = _owner.Site.Name;
                    }
                }

                return ownerName;
            }
        }

        /// <summary>
        ///     Creates a site for the component within the container.
        /// </summary>
        protected override ISite CreateSite(IComponent component, string name)
        {
            if (component == null)
            {
                throw new ArgumentNullException("component");
            }
            return new Site(component, this, name);
        }

        /// <summary>
        ///    Override of Container's dispose.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _owner.Disposed -= new EventHandler(OnOwnerDisposed);
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///    <para>[To be supplied.]</para>
        /// </summary>
        protected override object GetService(Type service)
        {
            if (service == typeof(INestedContainer))
            {
                return this;
            }
            else
            {
                return base.GetService(service);
            }
        }

        /// <summary>
        ///     Called when our owning component is destroyed.
        /// </summary>
        private void OnOwnerDisposed(object sender, EventArgs e)
        {
            Dispose();
        }

        /// <summary>
        ///     Simple site implementation.  We do some special processing to name the site, but 
        ///     that's about it.
        /// </summary>
        private class Site : INestedSite
        {
            private IComponent _component;
            private NestedContainer _container;
            private string _name;

            internal Site(IComponent component, NestedContainer container, string name)
            {
                _component = component;
                _container = container;
                _name = name;
            }

            // The component sited by this component site.
            public IComponent Component
            {
                get
                {
                    return _component;
                }
            }

            // The container in which the component is sited.
            public IContainer Container
            {
                get
                {
                    return _container;
                }
            }

            public Object GetService(Type service)
            {
                return ((service == typeof(ISite)) ? this : _container.GetService(service));
            }

            // Indicates whether the component is in design mode.
            public bool DesignMode
            {
                get
                {
                    IComponent owner = _container.Owner;
                    if (owner != null && owner.Site != null)
                    {
                        return owner.Site.DesignMode;
                    }
                    return false;
                }
            }

            public string FullName
            {
                get
                {
                    if (_name != null)
                    {
                        string ownerName = _container.OwnerName;
                        string childName = _name;
                        if (ownerName != null)
                        {
                            childName = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", ownerName, childName);
                        }

                        return childName;
                    }

                    return _name;
                }
            }

            // The name of the component.
            //
            public String Name
            {
                get
                {
                    return _name;
                }
                set
                {
                    if (value == null || _name == null || !value.Equals(_name))
                    {
                        _container.ValidateName(_component, value);
                        _name = value;
                    }
                }
            }
        }
    }
}

