// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Security;

namespace System.Xaml.Permissions
{
    /// <SecurityNote>
    /// This class is immutable. Various consumers copy references into SecurityCritical fields,
    /// and their security depends on the immutability of the members defined here.
    /// Derived classes may add mutable members, those have no impact on the consumers of this class.
    /// </SecurityNote>
    [Serializable]
    public class XamlAccessLevel
    {
        private XamlAccessLevel(string assemblyName, string typeName)
        {
            AssemblyNameString = assemblyName;
            PrivateAccessToTypeName = typeName;
        }

        public static XamlAccessLevel AssemblyAccessTo(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }
            return new XamlAccessLevel(assembly.FullName, null);
        }

        public static XamlAccessLevel AssemblyAccessTo(AssemblyName assemblyName)
        {
            if (assemblyName == null)
            {
                throw new ArgumentNullException(nameof(assemblyName));
            }
            ValidateAssemblyName(assemblyName, "assemblyName");
            return new XamlAccessLevel(assemblyName.FullName, null);
        }

        public static XamlAccessLevel PrivateAccessTo(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return new XamlAccessLevel(type.Assembly.FullName, type.FullName);
        }

        public static XamlAccessLevel PrivateAccessTo(string assemblyQualifiedTypeName)
        {
            if (assemblyQualifiedTypeName == null)
            {
                throw new ArgumentNullException(nameof(assemblyQualifiedTypeName));
            }
            int nameBoundary = assemblyQualifiedTypeName.IndexOf(',');
            if (nameBoundary < 0)
            {
                throw new ArgumentException("", nameof(assemblyQualifiedTypeName));
            }
            
            string typeName = assemblyQualifiedTypeName.Substring(0, nameBoundary).Trim();
            string assemblyFullName = assemblyQualifiedTypeName.Substring(nameBoundary + 1).Trim();
            AssemblyName assemblyName = new AssemblyName(assemblyFullName);
            ValidateAssemblyName(assemblyName, "assemblyQualifiedTypeName");
            
            return new XamlAccessLevel(assemblyName.FullName, typeName);
        }

        // Read-only: these properties should not be allowed to be modified once this object
        // has been passed to XamlLoadPermission
        
        // Stored as string: we need to store the assembly and type names, rather than Assembly or
        // Type references, because permissions can be serialized, and we don't want to force an
        // assembly load on deserialization in a different AppDomain.
        
        public AssemblyName AssemblyAccessToAssemblyName
        {
            get { return new AssemblyName(AssemblyNameString); }
        }

        public string PrivateAccessToTypeName { get; private set; }

        internal string AssemblyNameString { get; private set; }

        internal XamlAccessLevel AssemblyOnly()
        {
            return new XamlAccessLevel(AssemblyNameString, null);
        }

        internal static XamlAccessLevel FromXml(SecurityElement elem)
        {
            if (elem.Tag != XmlConstants.XamlAccessLevel)
            {
                throw new ArgumentException("", nameof(elem));
            }
            
            string assemblyNameString = elem.Attribute(XmlConstants.AssemblyName);
            if (assemblyNameString == null)
            {
                throw new ArgumentException("", nameof(elem));
            }
            AssemblyName assemblyName = new AssemblyName(assemblyNameString);
            ValidateAssemblyName(assemblyName, "elem");

            string typeName = elem.Attribute(XmlConstants.TypeName);
            if (typeName != null)
            {
                typeName = typeName.Trim();
            }

            return new XamlAccessLevel(assemblyName.FullName, typeName);
        }

        internal bool Includes(XamlAccessLevel other)
        {
            return other.AssemblyNameString == AssemblyNameString &&
                (other.PrivateAccessToTypeName == null || other.PrivateAccessToTypeName == PrivateAccessToTypeName);
        }

        internal SecurityElement ToXml()
        {
            SecurityElement element = new SecurityElement(XmlConstants.XamlAccessLevel);
            element.AddAttribute(XmlConstants.AssemblyName, AssemblyNameString);
            if (PrivateAccessToTypeName != null)
            {
                element.AddAttribute(XmlConstants.TypeName, PrivateAccessToTypeName);
            }
            return element;
        }

        private static void ValidateAssemblyName(AssemblyName assemblyName, string argName)
        {
            if (assemblyName.Name == null || assemblyName.Version == null ||
                assemblyName.CultureInfo == null || assemblyName.GetPublicKeyToken() == null)
            {
                throw new ArgumentException("", argName);
            }
        }

        private static class XmlConstants
        {
            public const string XamlAccessLevel = "XamlAccessLevel";
            public const string AssemblyName = "AssemblyName";
            public const string TypeName = "TypeName";
        }
    }
}
