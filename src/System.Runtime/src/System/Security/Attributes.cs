// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Security
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
    [System.Runtime.InteropServices.ComVisible(true)]
    sealed public class SuppressUnmanagedCodeSecurityAttribute : System.Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Module, AllowMultiple = true, Inherited = false)]
    [System.Runtime.InteropServices.ComVisible(true)]
    sealed public class UnverifiableCodeAttribute : System.Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    [System.Runtime.InteropServices.ComVisible(true)]
    sealed public class AllowPartiallyTrustedCallersAttribute : System.Attribute
    {
        public AllowPartiallyTrustedCallersAttribute() { }

        public PartialTrustVisibilityLevel PartialTrustVisibilityLevel
        {
            get;
            set;
        }
    }

    public enum PartialTrustVisibilityLevel
    {
        VisibleToAllHosts = 0,
        NotVisibleByDefault = 1
    }

    [Obsolete("SecurityCriticalScope is only used for .NET 2.0 transparency compatibility.")]
    public enum SecurityCriticalScope
    {
        Explicit = 0,
        Everything = 0x1
    }

    [AttributeUsage(AttributeTargets.Assembly |
                    AttributeTargets.Class |
                    AttributeTargets.Struct |
                    AttributeTargets.Enum |
                    AttributeTargets.Constructor |
                    AttributeTargets.Method |
                    AttributeTargets.Field |
                    AttributeTargets.Interface |
                    AttributeTargets.Delegate,
        AllowMultiple = false,
        Inherited = false)]
    sealed public class SecurityCriticalAttribute : System.Attribute
    {
#pragma warning disable 618
        private SecurityCriticalScope _val;

        public SecurityCriticalAttribute() { }

        public SecurityCriticalAttribute(SecurityCriticalScope scope)
        {
            _val = scope;
        }

        [Obsolete("SecurityCriticalScope is only used for .NET 2.0 transparency compatibility.")]
        public SecurityCriticalScope Scope
        {
            get
            {
                return _val;
            }
        }
#pragma warning restore 618
    }

    [AttributeUsage(AttributeTargets.Assembly |
                    AttributeTargets.Class |
                    AttributeTargets.Struct |
                    AttributeTargets.Enum |
                    AttributeTargets.Constructor |
                    AttributeTargets.Method |
                    AttributeTargets.Field |
                    AttributeTargets.Interface |
                    AttributeTargets.Delegate,
        AllowMultiple = false,
        Inherited = false)]
    [Obsolete("SecurityTreatAsSafe is only used for .NET 2.0 transparency compatibility.  Please use the SecuritySafeCriticalAttribute instead.")]
    sealed public class SecurityTreatAsSafeAttribute : System.Attribute
    {
        public SecurityTreatAsSafeAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Class |
                    AttributeTargets.Struct |
                    AttributeTargets.Enum |
                    AttributeTargets.Constructor |
                    AttributeTargets.Method |
                    AttributeTargets.Field |
                    AttributeTargets.Interface |
                    AttributeTargets.Delegate,
        AllowMultiple = false,
        Inherited = false)]
    sealed public class SecuritySafeCriticalAttribute : System.Attribute
    {
        public SecuritySafeCriticalAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    sealed public class SecurityTransparentAttribute : System.Attribute
    {
        public SecurityTransparentAttribute() { }
    }

    public enum SecurityRuleSet : byte
    {
        None = 0,
        Level1 = 1,
        Level2 = 2,
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class SecurityRulesAttribute : Attribute
    {
        private SecurityRuleSet _ruleSet;

        public SecurityRulesAttribute(SecurityRuleSet ruleSet)
        {
            _ruleSet = ruleSet;
        }

        public bool SkipVerificationInFullTrust
        {
            get;
            set;
        }

        public SecurityRuleSet RuleSet
        {
            get { return _ruleSet; }
        }
    }
}
