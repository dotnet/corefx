// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Security.AccessControl
{
    [System.Security.SecurityCriticalAttribute]
    public sealed partial class EventWaitHandleAccessRule : System.Security.AccessControl.AccessRule
    {
        public EventWaitHandleAccessRule(System.Security.Principal.IdentityReference identity, System.Security.AccessControl.EventWaitHandleRights eventRights, System.Security.AccessControl.AccessControlType type) : base(default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AccessControlType)) { }
        public EventWaitHandleAccessRule(string identity, System.Security.AccessControl.EventWaitHandleRights eventRights, System.Security.AccessControl.AccessControlType type) : base(default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AccessControlType)) { }
        public System.Security.AccessControl.EventWaitHandleRights EventWaitHandleRights { get { return default(System.Security.AccessControl.EventWaitHandleRights); } }
    }
    [System.Security.SecurityCriticalAttribute]
    public sealed partial class EventWaitHandleAuditRule : System.Security.AccessControl.AuditRule
    {
        public EventWaitHandleAuditRule(System.Security.Principal.IdentityReference identity, System.Security.AccessControl.EventWaitHandleRights eventRights, System.Security.AccessControl.AuditFlags flags) : base(default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AuditFlags)) { }
        public System.Security.AccessControl.EventWaitHandleRights EventWaitHandleRights { get { return default(System.Security.AccessControl.EventWaitHandleRights); } }
    }
    [System.Security.SecurityCriticalAttribute]
    public sealed partial class MutexAccessRule : System.Security.AccessControl.AccessRule
    {
        public MutexAccessRule(System.Security.Principal.IdentityReference identity, System.Security.AccessControl.MutexRights eventRights, System.Security.AccessControl.AccessControlType type) : base(default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AccessControlType)) { }
        public MutexAccessRule(string identity, System.Security.AccessControl.MutexRights eventRights, System.Security.AccessControl.AccessControlType type) : base(default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AccessControlType)) { }
        public System.Security.AccessControl.MutexRights MutexRights { get { return default(System.Security.AccessControl.MutexRights); } }
    }
    [System.Security.SecurityCriticalAttribute]
    public sealed partial class MutexAuditRule : System.Security.AccessControl.AuditRule
    {
        public MutexAuditRule(System.Security.Principal.IdentityReference identity, System.Security.AccessControl.MutexRights eventRights, System.Security.AccessControl.AuditFlags flags) : base(default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AuditFlags)) { }
        public System.Security.AccessControl.MutexRights MutexRights { get { return default(System.Security.AccessControl.MutexRights); } }
    }
    [System.FlagsAttribute]
    [System.Security.SecurityCriticalAttribute]
    public enum MutexRights
    {
        ChangePermissions = 262144,
        Delete = 65536,
        FullControl = 2031617,
        Modify = 1,
        ReadPermissions = 131072,
        Synchronize = 1048576,
        TakeOwnership = 524288,
    }
    [System.Security.SecurityCriticalAttribute]
    public sealed partial class MutexSecurity : System.Security.AccessControl.NativeObjectSecurity
    {
        public MutexSecurity() : base(default(bool), default(System.Security.AccessControl.ResourceType)) { }
        public MutexSecurity(string name, System.Security.AccessControl.AccessControlSections includeSections) : base(default(bool), default(System.Security.AccessControl.ResourceType)) { }
        public override System.Type AccessRightType { get { return default(System.Type); } }
        public override System.Type AccessRuleType { get { return default(System.Type); } }
        public override System.Type AuditRuleType { get { return default(System.Type); } }
        public override System.Security.AccessControl.AccessRule AccessRuleFactory(System.Security.Principal.IdentityReference identityReference, int accessMask, bool isInherited, System.Security.AccessControl.InheritanceFlags inheritanceFlags, System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AccessControlType type) { return default(System.Security.AccessControl.AccessRule); }
        public void AddAccessRule(System.Security.AccessControl.MutexAccessRule rule) { }
        public void AddAuditRule(System.Security.AccessControl.MutexAuditRule rule) { }
        public override System.Security.AccessControl.AuditRule AuditRuleFactory(System.Security.Principal.IdentityReference identityReference, int accessMask, bool isInherited, System.Security.AccessControl.InheritanceFlags inheritanceFlags, System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AuditFlags flags) { return default(System.Security.AccessControl.AuditRule); }
        public bool RemoveAccessRule(System.Security.AccessControl.MutexAccessRule rule) { return default(bool); }
        public void RemoveAccessRuleAll(System.Security.AccessControl.MutexAccessRule rule) { }
        public void RemoveAccessRuleSpecific(System.Security.AccessControl.MutexAccessRule rule) { }
        public bool RemoveAuditRule(System.Security.AccessControl.MutexAuditRule rule) { return default(bool); }
        public void RemoveAuditRuleAll(System.Security.AccessControl.MutexAuditRule rule) { }
        public void RemoveAuditRuleSpecific(System.Security.AccessControl.MutexAuditRule rule) { }
        public void ResetAccessRule(System.Security.AccessControl.MutexAccessRule rule) { }
        public void SetAccessRule(System.Security.AccessControl.MutexAccessRule rule) { }
        public void SetAuditRule(System.Security.AccessControl.MutexAuditRule rule) { }
    }
    [System.Security.SecurityCriticalAttribute]
    public sealed partial class SemaphoreAccessRule : System.Security.AccessControl.AccessRule
    {
        public SemaphoreAccessRule(System.Security.Principal.IdentityReference identity, System.Security.AccessControl.SemaphoreRights eventRights, System.Security.AccessControl.AccessControlType type) : base(default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AccessControlType)) { }
        public SemaphoreAccessRule(string identity, System.Security.AccessControl.SemaphoreRights eventRights, System.Security.AccessControl.AccessControlType type) : base(default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AccessControlType)) { }
        public System.Security.AccessControl.SemaphoreRights SemaphoreRights { get { return default(System.Security.AccessControl.SemaphoreRights); } }
    }
    [System.Security.SecurityCriticalAttribute]
    public sealed partial class SemaphoreAuditRule : System.Security.AccessControl.AuditRule
    {
        public SemaphoreAuditRule(System.Security.Principal.IdentityReference identity, System.Security.AccessControl.SemaphoreRights eventRights, System.Security.AccessControl.AuditFlags flags) : base(default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AuditFlags)) { }
        public System.Security.AccessControl.SemaphoreRights SemaphoreRights { get { return default(System.Security.AccessControl.SemaphoreRights); } }
    }
    [System.FlagsAttribute]
    [System.Security.SecurityCriticalAttribute]
    public enum SemaphoreRights
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
    public sealed partial class SemaphoreSecurity : System.Security.AccessControl.NativeObjectSecurity
    {
        public SemaphoreSecurity() : base(default(bool), default(System.Security.AccessControl.ResourceType)) { }
        public SemaphoreSecurity(string name, System.Security.AccessControl.AccessControlSections includeSections) : base(default(bool), default(System.Security.AccessControl.ResourceType)) { }
        public override System.Type AccessRightType { get { return default(System.Type); } }
        public override System.Type AccessRuleType { get { return default(System.Type); } }
        public override System.Type AuditRuleType { get { return default(System.Type); } }
        public override System.Security.AccessControl.AccessRule AccessRuleFactory(System.Security.Principal.IdentityReference identityReference, int accessMask, bool isInherited, System.Security.AccessControl.InheritanceFlags inheritanceFlags, System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AccessControlType type) { return default(System.Security.AccessControl.AccessRule); }
        public void AddAccessRule(System.Security.AccessControl.SemaphoreAccessRule rule) { }
        public void AddAuditRule(System.Security.AccessControl.SemaphoreAuditRule rule) { }
        public override System.Security.AccessControl.AuditRule AuditRuleFactory(System.Security.Principal.IdentityReference identityReference, int accessMask, bool isInherited, System.Security.AccessControl.InheritanceFlags inheritanceFlags, System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AuditFlags flags) { return default(System.Security.AccessControl.AuditRule); }
        public bool RemoveAccessRule(System.Security.AccessControl.SemaphoreAccessRule rule) { return default(bool); }
        public void RemoveAccessRuleAll(System.Security.AccessControl.SemaphoreAccessRule rule) { }
        public void RemoveAccessRuleSpecific(System.Security.AccessControl.SemaphoreAccessRule rule) { }
        public bool RemoveAuditRule(System.Security.AccessControl.SemaphoreAuditRule rule) { return default(bool); }
        public void RemoveAuditRuleAll(System.Security.AccessControl.SemaphoreAuditRule rule) { }
        public void RemoveAuditRuleSpecific(System.Security.AccessControl.SemaphoreAuditRule rule) { }
        public void ResetAccessRule(System.Security.AccessControl.SemaphoreAccessRule rule) { }
        public void SetAccessRule(System.Security.AccessControl.SemaphoreAccessRule rule) { }
        public void SetAuditRule(System.Security.AccessControl.SemaphoreAuditRule rule) { }
    }
}
namespace System.Threading
{
    [System.Security.SecurityCriticalAttribute]
    public static partial class ThreadingAclExtensions
    {
        public static System.Security.AccessControl.EventWaitHandleSecurity GetAccessControl(this System.Threading.EventWaitHandle handle) { return default(System.Security.AccessControl.EventWaitHandleSecurity); }
        public static System.Security.AccessControl.MutexSecurity GetAccessControl(this System.Threading.Mutex mutex) { return default(System.Security.AccessControl.MutexSecurity); }
        public static System.Security.AccessControl.SemaphoreSecurity GetAccessControl(this System.Threading.Semaphore semaphore) { return default(System.Security.AccessControl.SemaphoreSecurity); }
        public static void SetAccessControl(this System.Threading.EventWaitHandle handle, System.Security.AccessControl.EventWaitHandleSecurity eventSecurity) { }
        public static void SetAccessControl(this System.Threading.Mutex mutex, System.Security.AccessControl.MutexSecurity mutexSecurity) { }
        public static void SetAccessControl(this System.Threading.Semaphore semaphore, System.Security.AccessControl.SemaphoreSecurity semaphoreSecurity) { }
    }
}
