// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Security.AccessControl
{
    [System.FlagsAttribute]
    [System.Security.SecurityCriticalAttribute]
    public enum EventWaitHandleRights
    {
        ChangePermissions = 262144,
        Delete = 65536,
        FullControl = 2031619,
        Modify = 2,
        ReadPermissions = 131072,
        Synchronize = 1048576,
        TakeOwnership = 524288,
    }

    [System.Security.SecurityCriticalAttribute]
    public sealed partial class EventWaitHandleSecurity : System.Security.AccessControl.NativeObjectSecurity
    {
        public EventWaitHandleSecurity() : base(default(bool), default(System.Security.AccessControl.ResourceType)) { }
        public override System.Type AccessRightType { get { return default(System.Type); } }
        public override System.Type AccessRuleType { get { return default(System.Type); } }
        public override System.Type AuditRuleType { get { return default(System.Type); } }
        public override System.Security.AccessControl.AccessRule AccessRuleFactory(System.Security.Principal.IdentityReference identityReference, int accessMask, bool isInherited, System.Security.AccessControl.InheritanceFlags inheritanceFlags, System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AccessControlType type) { return default(System.Security.AccessControl.AccessRule); }
        public void AddAccessRule(System.Security.AccessControl.EventWaitHandleAccessRule rule) { }
        public void AddAuditRule(System.Security.AccessControl.EventWaitHandleAuditRule rule) { }
        public override System.Security.AccessControl.AuditRule AuditRuleFactory(System.Security.Principal.IdentityReference identityReference, int accessMask, bool isInherited, System.Security.AccessControl.InheritanceFlags inheritanceFlags, System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AuditFlags flags) { return default(System.Security.AccessControl.AuditRule); }
        public bool RemoveAccessRule(System.Security.AccessControl.EventWaitHandleAccessRule rule) { return default(bool); }
        public void RemoveAccessRuleAll(System.Security.AccessControl.EventWaitHandleAccessRule rule) { }
        public void RemoveAccessRuleSpecific(System.Security.AccessControl.EventWaitHandleAccessRule rule) { }
        public bool RemoveAuditRule(System.Security.AccessControl.EventWaitHandleAuditRule rule) { return default(bool); }
        public void RemoveAuditRuleAll(System.Security.AccessControl.EventWaitHandleAuditRule rule) { }
        public void RemoveAuditRuleSpecific(System.Security.AccessControl.EventWaitHandleAuditRule rule) { }
        public void ResetAccessRule(System.Security.AccessControl.EventWaitHandleAccessRule rule) { }
        public void SetAccessRule(System.Security.AccessControl.EventWaitHandleAccessRule rule) { }
        public void SetAuditRule(System.Security.AccessControl.EventWaitHandleAuditRule rule) { }
    }
}
