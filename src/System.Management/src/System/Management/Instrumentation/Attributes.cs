//------------------------------------------------------------------------------
// <copyright from='1997' to='2001' company='Microsoft Corporation'>           
//    Copyright (c) Microsoft Corporation. All Rights Reserved.                
//    Information Contained Herein is Proprietary and Confidential.            
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace System.Management.Instrumentation
{
    using System;
    using System.Reflection;
    using System.Collections;
    using System.Text.RegularExpressions;
    using System.Management;
    using System.Globalization;

    /// <summary>
    ///    <para>Specifies that this assembly provides management instrumentation. This attribute should appear one time per assembly.</para>
    /// </summary>
    /// <remarks>
    /// <para>For more information about using attributes, see <see topic="cpconExtendingMetadataUsingAttributes" title="Extending Metadata Using Attributes"/> .</para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class InstrumentedAttribute : Attribute
    {
        string namespaceName;
        string securityDescriptor;

        /// <overload>
        ///    Initializes a new instance
        ///    of the <see cref='System.Management.Instrumentation.InstrumentedAttribute'/> class.
        /// </overload>
        /// <summary>
        /// <para>Initializes a new instance of the <see cref='System.Management.Instrumentation.InstrumentedAttribute'/> 
        /// class that is set for the root\default namespace. This is the default constructor.</para>
        /// </summary>
        public InstrumentedAttribute() : this(null, null) {}

        /// <summary>
        /// <para>Initializes a new instance of the <see cref='System.Management.Instrumentation.InstrumentedAttribute'/> class that is set to the specified namespace for instrumentation within this assembly.</para>
        /// </summary>
        /// <param name='namespaceName'>The namespace for instrumentation instances and events.</param>
        public InstrumentedAttribute(string namespaceName) : this(namespaceName, null) {}

        /// <summary>
        /// <para>Initializes a new instance of the <see cref='System.Management.Instrumentation.InstrumentedAttribute'/> class that is set to the specified namespace and security settings for instrumentation within this assembly.</para>
        /// </summary>
        /// <param name='namespaceName'>The namespace for instrumentation instances and events.</param>
        /// <param name='securityDescriptor'> A security descriptor that allows only the specified users or groups to run applications that provide the instrumentation supported by this assembly.</param>
        public InstrumentedAttribute(string namespaceName, string securityDescriptor)
        {
            // TODO: Do we need validation
            // bug#62511 - always use backslash in name
            if(namespaceName != null)
                namespaceName = namespaceName.Replace('/', '\\');

            if(namespaceName == null || namespaceName.Length == 0)
                namespaceName = "root\\default"; // bug#60933 Use a default namespace if null


            bool once = true;
            foreach(string namespacePart in namespaceName.Split('\\'))
            {
                if(     namespacePart.Length == 0
                    ||  (once && String.Compare(namespacePart, "root", StringComparison.OrdinalIgnoreCase) != 0)  // Must start with 'root'
                    ||  !Regex.Match(namespacePart, @"^[a-z,A-Z]").Success // All parts must start with letter
                    ||  Regex.Match(namespacePart, @"_$").Success // Must not end with an underscore
                    ||  Regex.Match(namespacePart, @"[^a-z,A-Z,0-9,_,\u0080-\uFFFF]").Success) // Only letters, digits, or underscores
                {
                    ManagementException.ThrowWithExtendedInfo(ManagementStatus.InvalidNamespace);
                }
                once = false;
            }

            this.namespaceName = namespaceName;
            this.securityDescriptor = securityDescriptor;
        }

        /// <summary>
        ///    <para>Gets or sets the namespace for instrumentation instances and events in this assembly.</para>
        /// </summary>
        /// <value>
        ///    <para>If not specified, the default namespace will be set as "\\.\root\default". 
        ///       Otherwise, a string indicating the name of the namespace for instrumentation
        ///       instances and events in this assembly.</para>
        /// </value>
        /// <remarks>
        ///    It is highly recommended that the namespace name be specified by the
        ///    assembly, and that it should be a unique namespace per assembly, or per
        ///    application. Having a specific namespace for each assembly or
        ///    application instrumentation allows more granularity for securing access to
        ///    instrumentation provided by different assemblies or applications.
        /// </remarks>
        public string NamespaceName 
        {
            get { return namespaceName == null ? string.Empty : namespaceName; }
        }
        
        /// <summary>
        ///    <para> Gets or sets a security descriptor that allows only the specified users or groups to run
        ///       applications that provide the instrumentation supported by this assembly.</para>
        /// </summary>
        /// <value>
        ///    <para> 
        ///       If null, the default value is defined to include the
        ///       following security groups : <see langword='Local Administrators'/>, <see langword='Local System'/>, <see langword='Local Service'/>, <see langword='Network Service'/> and <see langword='Batch Logon'/>. This will only allow
        ///       members of these security groups
        ///       to publish data and fire events from this assembly.</para>
        ///    <para>Otherwise, this is a string in SDDL format representing the security 
        ///       descriptor that defines which users and groups can provide instrumentation data
        ///       and events from this application.</para>
        /// </value>
        /// <remarks>
        ///    <para>Users or groups not specified in this
        ///       security descriptor may still run the application, but cannot provide
        ///       instrumentation from this assembly.</para>
        /// </remarks>
        /// <example>
        ///    <para>The SDDL representing the default set of security groups defined above is as 
        ///       follows :</para>
        ///    <para>O:BAG:BAD:(A;;0x10000001;;;BA)(A;;0x10000001;;;SY)(A;;0x10000001;;;LA)(A;;0x10000001;;;S-1-5-20)(A;;0x10000001;;;S-1-5-19)</para>
        /// <para>To add the <see langword='Power Users'/> group to the users allowed to fire events or publish 
        ///    instances from this assembly, the attribute should be specificed as
        ///    follows :</para>
        /// <para>[Instrumented("root\\MyApplication", "O:BAG:BAD:(A;;0x10000001;;;BA)(A;;0x10000001;;;SY)(A;;0x10000001;;;LA)(A;;0x10000001;;;S-1-5-20)(A;;0x10000001;;;S-1-5-19)(A;;0x10000001;;;PU)")]</para>
        /// </example>
        public string SecurityDescriptor
        {
            get
            {
                // This will never return an empty string.  Instead, it will
                // return null, or a non-zero length string
                if(null == securityDescriptor || securityDescriptor.Length == 0)
                    return null;
                return securityDescriptor;
            }
        }

        internal static InstrumentedAttribute GetAttribute(Assembly assembly)
        {
            Object [] rg = assembly.GetCustomAttributes(typeof(InstrumentedAttribute), false);
            if(rg.Length > 0)
                return ((InstrumentedAttribute)rg[0]);
            return new InstrumentedAttribute();
        }

        internal static Type[] GetInstrumentedTypes(Assembly assembly)
        {
            ArrayList types = new ArrayList();

            //
            // The recursion has been moved to the parent level to avoid ineffiency of wading through all types
            // at each stage of recursion. Also, the recursive method has been replaced with the more correct:
            // GetInstrumentedParentTypes method (see comments on header).
            //
            foreach (Type type in assembly.GetTypes())
            {
                if (IsInstrumentationClass(type))
                {
                    GetInstrumentedParentTypes(types, type);
                }
            }
            return (Type[])types.ToArray(typeof(Type));
        }

        //
        // Recursive function that adds the type to the array and recurses on the parent type. The end condition
        // is either no parent type or a parent type which is not marked as instrumented.
        //
        static void GetInstrumentedParentTypes(ArrayList types, Type childType)
        {
            if (types.Contains(childType) == false)
            {
                Type parentType = InstrumentationClassAttribute.GetBaseInstrumentationType(childType) ;

                //
                // If we have a instrumented base type and it has not already 
                // been included in the list of instrumented types
                // traverse the inheritance hierarchy.
                //
                if (parentType != null)
                {
                    GetInstrumentedParentTypes(types, parentType);
                }
                types.Add(childType);
            }
        }

        static bool IsInstrumentationClass(Type type)
        {
            return (null != InstrumentationClassAttribute.GetAttribute(type));
        }

    }
    
    /// <summary>
    ///    <para>Specifies the type of instrumentation provided by a class.</para>
    /// </summary>
    /// <example>
    ///    <code lang='C#'>using System;
    /// using System.Management;
    /// using System.Configuration.Install;
    /// using System.Management.Instrumentation;
    /// 
    /// // This example demonstrates how to create a Management Event class by using
    /// // the InstrumentationClass attribute and to fire a Management Event from
    /// // managed code.
    /// 
    /// // Specify which namespace the Management Event class is created in
    /// [assembly:Instrumented("Root/Default")]
    /// 
    /// // Let the system know you will run InstallUtil.exe utility against
    /// // this assembly
    /// [System.ComponentModel.RunInstaller(true)]
    /// public class MyInstaller : DefaultManagementProjectInstaller {}
    /// 
    /// // Create a Management Instrumentation Event class
    /// [InstrumentationClass(InstrumentationType.Event)]
    /// public class MyEvent
    /// {
    ///     public string EventName;
    /// }
    /// 
    /// public class WMI_InstrumentedEvent_Example
    /// {
    ///     public static void Main() {
    ///         MyEvent e = new MyEvent();
    ///         e.EventName = "Hello";
    ///         
    ///         // Fire a Management Event
    ///         Instrumentation.Fire(e);
    ///         
    ///         return;
    ///     }
    /// }
    ///    </code>
    ///    <code lang='VB'>Imports System
    /// Imports System.Management
    /// Imports System.Configuration.Install
    /// Imports System.Management.Instrumentation
    /// 
    /// ' This sample demonstrates how to create a Management Event class by using
    /// ' the InstrumentationClass attribute and to fire a Management Event from
    /// ' managed code.
    /// 
    /// ' Specify which namespace the Manaegment Event class is created in
    /// &lt;assembly: Instrumented("Root/Default")&gt;
    /// 
    /// ' Let the system know InstallUtil.exe utility will be run against
    /// ' this assembly
    /// &lt;System.ComponentModel.RunInstaller(True)&gt; _
    /// Public Class MyInstaller
    ///     Inherits DefaultManagementProjectInstaller
    /// End Class 'MyInstaller
    /// 
    /// ' Create a Management Instrumentation Event class
    /// &lt;InstrumentationClass(InstrumentationType.Event)&gt; _ 
    /// Public Class MyEvent
    ///     Public EventName As String
    /// End Class
    /// 
    /// Public Class Sample_EventProvider
    ///     Public Shared Function Main(args() As String) As Integer
    ///         Dim e As New MyEvent()
    ///         e.EventName = "Hello"
    ///         
    ///         ' Fire a Management Event
    ///         Instrumentation.Fire(e)
    ///         
    ///         Return 0
    ///     End Function
    /// End Class
    ///    </code>
    /// </example>
    public enum InstrumentationType
    {
        /// <summary>
        ///    <para>Specifies that the class provides instances for management instrumentation.</para>
        /// </summary>
        Instance,
        /// <summary>
        ///    <para>Specifies that the class provides events for management instrumentation.</para>
        /// </summary>
        Event,
        /// <summary>
        ///    <para>Specifies that the class defines an abstract class for management instrumentation.</para>
        /// </summary>
        Abstract
    }

    /// <summary>
    /// Specifies that a class provides event or instance instrumentation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class InstrumentationClassAttribute : Attribute
    {
        InstrumentationType instrumentationType;
        string managedBaseClassName;

        /// <overload>
        ///    Initializes a new instance
        ///    of the <see cref='System.Management.Instrumentation.InstrumentationClassAttribute'/> class.
        /// </overload>
        /// <summary>
        /// <para>Initializes a new instance of the <see cref='System.Management.Instrumentation.InstrumentationClassAttribute'/> class that is used if this type is derived from another type that has the <see cref='System.Management.Instrumentation.InstrumentationClassAttribute'/> attribute, or if this is a 
        ///    top-level instrumentation class (for example, an instance or abstract class
        ///    without a base class, or an event derived from <see langword='__ExtrinsicEvent'/>).</para>
        /// </summary>
        /// <param name='instrumentationType'>The type of instrumentation provided by this class.</param>
        public InstrumentationClassAttribute(InstrumentationType instrumentationType)
        {
            this.instrumentationType = instrumentationType;
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref='System.Management.Instrumentation.InstrumentationClassAttribute'/> class that
        ///    has schema for an existing base class. The class must contain
        ///    proper member definitions for the properties of the existing
        ///    WMI base class.</para>
        /// </summary>
        /// <param name='instrumentationType'>The type of instrumentation provided by this class.</param>
        /// <param name='managedBaseClassName'>The name of the base class.</param>
        public InstrumentationClassAttribute(InstrumentationType instrumentationType, string managedBaseClassName)
        {
            this.instrumentationType = instrumentationType;
            this.managedBaseClassName = managedBaseClassName;
        }

        /// <summary>
        ///    <para>Gets or sets the type of instrumentation provided by this class.</para>
        /// </summary>
        /// <value>
        ///    Contains an <see cref='System.Management.Instrumentation.InstrumentationType'/> value that
        ///    indicates whether this is an instrumented event, instance or abstract class.
        /// </value>
        public InstrumentationType InstrumentationType
        {
            get { return instrumentationType; }
        }

        /// <summary>
        ///    <para>Gets or sets the name of the base class of this instrumentation class.</para>
        /// </summary>
        /// <value>
        ///    <para>If not null, this string indicates the WMI baseclass that this class inherits
        ///       from in the CIM schema.</para>
        /// </value>
        public string ManagedBaseClassName
        {
            get
            {
                // This will never return an empty string.  Instead, it will
                // return null, or a non-zero length string
                if(null == managedBaseClassName || managedBaseClassName.Length == 0)
                    return null;

                return managedBaseClassName;
            }
        }

        internal static InstrumentationClassAttribute GetAttribute(Type type)
        {
            // We don't want BaseEvent or Instance to look like that have an 'InstrumentedClass' attribute
            if(type == typeof(BaseEvent) || type == typeof(Instance))
                return null;

            // We will inherit the 'InstrumentedClass' attribute from a base class
            Object [] rg = type.GetCustomAttributes(typeof(InstrumentationClassAttribute), true);
            if(rg.Length > 0)
                return ((InstrumentationClassAttribute)rg[0]);
            return null;
        }

        /// <summary>
        /// <para>Displays the <see langword='Type'/> of the base class.</para>
        /// </summary>
        /// <param name='type'></param>
        /// <returns>
        /// <para>The <see langword='Type'/> of the base class, if this class is derived from another 
        ///    instrumentation class; otherwise, null.</para>
        /// </returns>
        internal static Type GetBaseInstrumentationType(Type type)
        {
            // If the BaseType has a InstrumentationClass attribute,
            // we return the BaseType
            if(GetAttribute(type.BaseType) != null)
                return type.BaseType;
            return null;
        }
    }

    /// <summary>
    ///    <para>Allows an instrumented class, or member of an instrumented class,
    ///       to present an alternate name through management instrumentation.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property  | AttributeTargets.Method)]
    public class ManagedNameAttribute : Attribute
    {
        string name;

        /// <summary>
        /// <para>Gets the name of the managed entity.</para>
        /// </summary>
        /// <value>
        /// Contains the name of the managed entity.
        /// </value>
        public string Name
        {
            get { return name ; }
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref='System.Management.Instrumentation.ManagedNameAttribute'/> class that allows the alternate name to be specified
        ///    for the type, field, property, method, or parameter to which this attribute is applied.</para>
        /// </summary>
        /// <param name='name'>The alternate name for the type, field, property, method, or parameter to which this attribute is applied.</param>
        public ManagedNameAttribute(string name)
        {
            this.name = name;
        }

        internal static string GetMemberName(MemberInfo member)
        {
            // This works for all sorts of things: Type, MethodInfo, PropertyInfo, FieldInfo
            Object [] rg = member.GetCustomAttributes(typeof(ManagedNameAttribute), false);
            if(rg.Length > 0)
            {
                // bug#69115 - if null or empty string are passed, we just ignore this attribute
                ManagedNameAttribute attr = (ManagedNameAttribute)rg[0];
                if(attr.name != null && attr.name.Length != 0)
                    return attr.name;
            }

            return member.Name;
        }

        internal static string GetBaseClassName(Type type)
        {
            InstrumentationClassAttribute attr = InstrumentationClassAttribute.GetAttribute(type);
            string name = attr.ManagedBaseClassName;
            if(name != null)
                return name;
            
            // Get managed base type's attribute
            InstrumentationClassAttribute attrParent = InstrumentationClassAttribute.GetAttribute(type.BaseType);

            // If the base type does not have a InstrumentationClass attribute,
            // return a base type based on the InstrumentationType
            if(null == attrParent)
            {
                switch(attr.InstrumentationType)
                {
                    case InstrumentationType.Abstract:
                        return null;
                    case InstrumentationType.Instance:
                        return null;
                    case InstrumentationType.Event:
                        return "__ExtrinsicEvent";
                    default:
                        break;
                }
            }

            // Our parent was also a managed provider type.  Use it's managed name.
            return GetMemberName(type.BaseType);
        }
    }

    /// <summary>
    ///    <para>Allows a particular member of an instrumented class to be ignored
    ///       by management instrumentation</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property  | AttributeTargets.Method)]
    public class IgnoreMemberAttribute : Attribute
    {
    }

#if REQUIRES_EXPLICIT_DECLARATION_OF_INHERITED_PROPERTIES
    /// <summary>
    ///    <para>[To be supplied.]</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class InheritedPropertyAttribute : Attribute
    {
        internal static InheritedPropertyAttribute GetAttribute(FieldInfo field)
        {
            Object [] rg = field.GetCustomAttributes(typeof(InheritedPropertyAttribute), false);
            if(rg.Length > 0)
                return ((InheritedPropertyAttribute)rg[0]);
            return null;
        }
    }
#endif

#if SUPPORTS_WMI_DEFAULT_VAULES
    [AttributeUsage(AttributeTargets.Field)]
    internal class ManagedDefaultValueAttribute : Attribute
    {
        Object defaultValue;
        public ManagedDefaultValueAttribute(Object defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        public static Object GetManagedDefaultValue(FieldInfo field)
        {
            Object [] rg = field.GetCustomAttributes(typeof(ManagedDefaultValueAttribute), false);
            if(rg.Length > 0)
                return ((ManagedDefaultValueAttribute)rg[0]).defaultValue;

            return null;
        }
    }
#endif

#if SUPPORTS_ALTERNATE_WMI_PROPERTY_TYPE
    [AttributeUsage(AttributeTargets.Field)]
    internal class ManagedTypeAttribute : Attribute
    {
        Type type;
        public ManagedTypeAttribute(Type type)
        {
            this.type = type;
        }

        public static Type GetManagedType(FieldInfo field)
        {
            Object [] rg = field.GetCustomAttributes(typeof(ManagedTypeAttribute), false);
            if(rg.Length > 0)
                return ((ManagedTypeAttribute)rg[0]).type;

            return field.FieldType;
        }
    }
#endif
}
