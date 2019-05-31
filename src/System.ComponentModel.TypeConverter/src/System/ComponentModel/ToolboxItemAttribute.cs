// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Security", "CA2113:SecureLateBindingMethods", Scope = "member", Target = "System.ComponentModel.ToolboxItemAttribute.get_ToolboxItemType():System.Type")]

namespace System.ComponentModel
{
    /// <summary>
    /// Specifies attributes for a toolbox item.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class ToolboxItemAttribute : Attribute
    {
        private Type _toolboxItemType;
        private string _toolboxItemTypeName;

        /// <summary>
        /// Initializes a new instance of ToolboxItemAttribute and sets the type to
        /// </summary>
        public static readonly ToolboxItemAttribute Default = new ToolboxItemAttribute("System.Drawing.Design.ToolboxItem, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");

        /// <summary>
        /// Initializes a new instance of ToolboxItemAttribute and sets the type to
        /// <see langword='null'/>.
        /// </summary>
        public static readonly ToolboxItemAttribute None = new ToolboxItemAttribute(false);

        /// <summary>
        /// Gets whether the attribute is the default attribute.
        /// </summary>
        public override bool IsDefaultAttribute() => Equals(Default);

        /// <summary>
        /// Initializes a new instance of ToolboxItemAttribute and specifies if default values should be used.
        /// </summary>
        public ToolboxItemAttribute(bool defaultType)
        {
            if (defaultType)
            {
                _toolboxItemTypeName = "System.Drawing.Design.ToolboxItem, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
            }
        }

        /// <summary>
        /// Initializes a new instance of ToolboxItemAttribute and specifies the name of the type.
        /// </summary>
        public ToolboxItemAttribute(string toolboxItemTypeName)
        {
            _toolboxItemTypeName = toolboxItemTypeName ?? throw new ArgumentNullException(nameof(toolboxItemTypeName));
        }

        /// <summary>
        /// Initializes a new instance of ToolboxItemAttribute and specifies the type of the toolbox item.
        /// </summary>
        public ToolboxItemAttribute(Type toolboxItemType)
        {
            if (toolboxItemType == null)
            {
                throw new ArgumentNullException(nameof(toolboxItemType));
            }

            _toolboxItemType = toolboxItemType;
            _toolboxItemTypeName = toolboxItemType.AssemblyQualifiedName;
        }

        /// <summary>
        /// Gets the toolbox item's type.
        /// </summary>
        public Type ToolboxItemType
        {
            get
            {
                if (_toolboxItemType == null)
                {
                    if (_toolboxItemTypeName != null)
                    {
                        try
                        {
                            _toolboxItemType = Type.GetType(_toolboxItemTypeName, true);
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentException(SR.Format(SR.ToolboxItemAttributeFailedGetType, _toolboxItemTypeName), ex);
                        }
                    }
                }
                return _toolboxItemType;
            }
        }

        public string ToolboxItemTypeName => _toolboxItemTypeName ?? string.Empty;

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            return (obj is ToolboxItemAttribute other) && (other.ToolboxItemTypeName == ToolboxItemTypeName);
        }

        public override int GetHashCode()
        {
            if (_toolboxItemTypeName != null)
            {
                return _toolboxItemTypeName.GetHashCode();
            }
            return base.GetHashCode();
        }
    }
}
