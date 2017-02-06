// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace System.Runtime.InteropServices
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
    public sealed class AutomationProxyAttribute : Attribute
    {
        public AutomationProxyAttribute(bool val)
        {
            Value = val;
        }
        public bool Value { get; }
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
    public sealed class ComAliasNameAttribute : Attribute
    {
        public ComAliasNameAttribute(string alias)
        {
            Value = alias;
        }
        public string Value { get; }
    }

    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class ComCompatibleVersionAttribute : Attribute
    {
        public ComCompatibleVersionAttribute(int major, int minor, int build, int revision)
        {
            MajorVersion = major;
            MinorVersion = minor;
            BuildNumber = build;
            RevisionNumber = revision;
        }

        public int MajorVersion { get; }
        public int MinorVersion { get; }
        public int BuildNumber { get; }
        public int RevisionNumber { get; }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public sealed class ComConversionLossAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Interface, Inherited = false)]
    public sealed class ComEventInterfaceAttribute : Attribute
    {
        public ComEventInterfaceAttribute(Type SourceInterface, Type EventProvider)
        {
            this.SourceInterface = SourceInterface;
            this.EventProvider = EventProvider;
        }

        public Type SourceInterface { get; }
        public Type EventProvider { get; }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class ComRegisterFunctionAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class ComUnregisterFunctionAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, Inherited = false)]
    [Obsolete("This attribute is deprecated and will be removed in a future version.", false)]
    public sealed class IDispatchImplAttribute : Attribute
    {
        public IDispatchImplAttribute(short implType)
        {
            Value = (IDispatchImplType)implType;
        }
        public IDispatchImplAttribute(IDispatchImplType implType)
        {
            Value = implType;
        }
        public IDispatchImplType Value { get; }
    }

    [Obsolete("The IDispatchImplAttribute is deprecated.", false)]
    [Serializable]
    public enum IDispatchImplType
    {
        CompatibleImpl = 2,
        InternalImpl = 1,
        SystemDefinedImpl = 0,
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ManagedToNativeComInteropStubAttribute : Attribute
    {
        public ManagedToNativeComInteropStubAttribute(Type classType, string methodName)
        {
            ClassType = classType;
            MethodName = methodName;
        }

        public Type ClassType { get; }
        public string MethodName { get; }
    }

    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
    public sealed class PrimaryInteropAssemblyAttribute : Attribute
    {
        public PrimaryInteropAssemblyAttribute(int major, int minor)
        {
            MajorVersion = major;
            MinorVersion = minor;
        }

        public int MajorVersion { get; }
        public int MinorVersion { get; }
    }

    [Obsolete("This attribute has been deprecated.  Application Domains no longer respect Activation Context boundaries in IDispatch calls.", false)]
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class SetWin32ContextInIDispatchAttribute : Attribute
    {
        public SetWin32ContextInIDispatchAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Interface, Inherited = false)]
    public sealed class TypeLibImportClassAttribute : Attribute
    {
        internal String _importClassName;
        public TypeLibImportClassAttribute(Type importClass)
        {
            _importClassName = importClass.ToString();
        }
        public String Value { get { return _importClassName; } }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ProgIdAttribute : Attribute
    {
        internal String _val;
        public ProgIdAttribute(String progId)
        {
            _val = progId;
        }
        public String Value { get { return _val; } }
    }

    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class ImportedFromTypeLibAttribute : Attribute
    {
        internal String _val;
        public ImportedFromTypeLibAttribute(String tlbFile)
        {
            _val = tlbFile;
        }
        public String Value { get { return _val; } }
    }

    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class TypeLibVersionAttribute : Attribute
    {
        internal int _major;
        internal int _minor;

        public TypeLibVersionAttribute(int major, int minor)
        {
            _major = major;
            _minor = minor;
        }

        public int MajorVersion { get { return _major; } }
        public int MinorVersion { get { return _minor; } }
    }
}
