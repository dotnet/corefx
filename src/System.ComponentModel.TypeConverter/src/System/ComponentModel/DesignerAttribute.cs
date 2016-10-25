// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Security.Permissions;

namespace System.ComponentModel
{
    /// <summary>
    ///    <para>Specifies the class to use to implement design-time services.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public sealed class DesignerAttribute : Attribute
    {
        private readonly string _designerTypeName;
        private readonly string _designerBaseTypeName;
        private string _typeId;

        /// <summary>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.ComponentModel.DesignerAttribute'/> class using the name of the type that
        ///       provides design-time services.
        ///    </para>
        /// </summary>
        public DesignerAttribute(string designerTypeName)
        {
            string temp = designerTypeName.ToUpper(CultureInfo.InvariantCulture);
            Debug.Assert(temp.IndexOf(".DLL") == -1, "Came across: " + designerTypeName + " . Please remove the .dll extension");
            _designerTypeName = designerTypeName;
            _designerBaseTypeName = typeof(IDesigner).FullName;
        }

        /// <summary>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.ComponentModel.DesignerAttribute'/> class using the type that provides
        ///       design-time services.
        ///    </para>
        /// </summary>
        public DesignerAttribute(Type designerType)
        {
            _designerTypeName = designerType.AssemblyQualifiedName;
            _designerBaseTypeName = typeof(IDesigner).FullName;
        }

        /// <summary>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.ComponentModel.DesignerAttribute'/> class using the designer type and the
        ///       base class for the designer.
        ///    </para>
        /// </summary>
        public DesignerAttribute(string designerTypeName, string designerBaseTypeName)
        {
            string temp = designerTypeName.ToUpper(CultureInfo.InvariantCulture);
            Debug.Assert(temp.IndexOf(".DLL") == -1, "Came across: " + designerTypeName + " . Please remove the .dll extension");
            _designerTypeName = designerTypeName;
            _designerBaseTypeName = designerBaseTypeName;
        }

        /// <summary>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.ComponentModel.DesignerAttribute'/> class, using the name of the designer
        ///       class and the base class for the designer.
        ///    </para>
        /// </summary>
        public DesignerAttribute(string designerTypeName, Type designerBaseType)
        {
            string temp = designerTypeName.ToUpper(CultureInfo.InvariantCulture);
            Debug.Assert(temp.IndexOf(".DLL") == -1, "Came across: " + designerTypeName + " . Please remove the .dll extension");
            _designerTypeName = designerTypeName;
            _designerBaseTypeName = designerBaseType.AssemblyQualifiedName;
        }

        /// <summary>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.ComponentModel.DesignerAttribute'/> class using the types of the designer and
        ///       designer base class.
        ///    </para>
        /// </summary>
        public DesignerAttribute(Type designerType, Type designerBaseType)
        {
            _designerTypeName = designerType.AssemblyQualifiedName;
            _designerBaseTypeName = designerBaseType.AssemblyQualifiedName;
        }

        /// <summary>
        ///    <para>
        ///       Gets
        ///       the name of the base type of this designer.
        ///    </para>
        /// </summary>
        public string DesignerBaseTypeName
        {
            get
            {
                return _designerBaseTypeName;
            }
        }

        /// <summary>
        ///    <para>
        ///       Gets the name of the designer type associated with this designer attribute.
        ///    </para>
        /// </summary>
        public string DesignerTypeName
        {
            get
            {
                return _designerTypeName;
            }
        }

        /// <internalonly/>
        /// <summary>
        ///    <para>
        ///       This defines a unique ID for this attribute type. It is used
        ///       by filtering algorithms to identify two attributes that are
        ///       the same type. For most attributes, this just returns the
        ///       Type instance for the attribute. DesignerAttribute overrides
        ///       this to include the type of the designer base type.
        ///    </para>
        /// </summary>
        public override object TypeId
        {
            get
            {
                if (_typeId == null)
                {
                    string baseType = _designerBaseTypeName;
                    int comma = baseType.IndexOf(',');
                    if (comma != -1)
                    {
                        baseType = baseType.Substring(0, comma);
                    }
                    _typeId = GetType().FullName + baseType;
                }
                return _typeId;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            DesignerAttribute other = obj as DesignerAttribute;

            return (other != null) && other._designerBaseTypeName == _designerBaseTypeName && other._designerTypeName == _designerTypeName;
        }

        public override int GetHashCode()
        {
            return _designerTypeName.GetHashCode() ^ _designerBaseTypeName.GetHashCode();
        }
    }
}

