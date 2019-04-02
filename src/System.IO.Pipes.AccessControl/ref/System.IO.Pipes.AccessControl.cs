// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.IO.Pipes
{
    [System.FlagsAttribute]
    public enum PipeAccessRights
    {
        AccessSystemSecurity = 16777216,
        ChangePermissions = 262144,
        CreateNewInstance = 4,
        Delete = 65536,
        FullControl = 2032031,
        Read = 131209,
        ReadAttributes = 128,
        ReadData = 1,
        ReadExtendedAttributes = 8,
        ReadPermissions = 131072,
        ReadWrite = 131483,
        Synchronize = 1048576,
        TakeOwnership = 524288,
        Write = 274,
        WriteAttributes = 256,
        WriteData = 2,
        WriteExtendedAttributes = 16,
    }
    public sealed partial class PipeAccessRule : System.Security.AccessControl.AccessRule
    {
        public PipeAccessRule(System.Security.Principal.IdentityReference identity, System.IO.Pipes.PipeAccessRights rights, System.Security.AccessControl.AccessControlType type) : base (default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AccessControlType)) { }
        public PipeAccessRule(string identity, System.IO.Pipes.PipeAccessRights rights, System.Security.AccessControl.AccessControlType type) : base (default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AccessControlType)) { }
        public System.IO.Pipes.PipeAccessRights PipeAccessRights { get { throw null; } }
    }
    public sealed partial class PipeAuditRule : System.Security.AccessControl.AuditRule
    {
        public PipeAuditRule(System.Security.Principal.IdentityReference identity, System.IO.Pipes.PipeAccessRights rights, System.Security.AccessControl.AuditFlags flags) : base (default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AuditFlags)) { }
        public PipeAuditRule(string identity, System.IO.Pipes.PipeAccessRights rights, System.Security.AccessControl.AuditFlags flags) : base (default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AuditFlags)) { }
        public System.IO.Pipes.PipeAccessRights PipeAccessRights { get { throw null; } }
    }
    public static partial class PipesAclExtensions
    {
        public static System.IO.Pipes.PipeSecurity GetAccessControl(this System.IO.Pipes.PipeStream stream) { throw null; }
        public static void SetAccessControl(this System.IO.Pipes.PipeStream stream, System.IO.Pipes.PipeSecurity pipeSecurity) { }
    }
    public partial class PipeSecurity : System.Security.AccessControl.NativeObjectSecurity
    {
        public PipeSecurity() : base (default(bool), default(System.Security.AccessControl.ResourceType)) { }
        public override System.Type AccessRightType { get { throw null; } }
        public override System.Type AccessRuleType { get { throw null; } }
        public override System.Type AuditRuleType { get { throw null; } }
        public override System.Security.AccessControl.AccessRule AccessRuleFactory(System.Security.Principal.IdentityReference identityReference, int accessMask, bool isInherited, System.Security.AccessControl.InheritanceFlags inheritanceFlags, System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AccessControlType type) { throw null; }
        public void AddAccessRule(System.IO.Pipes.PipeAccessRule rule) { }
        public void AddAuditRule(System.IO.Pipes.PipeAuditRule rule) { }
        public sealed override System.Security.AccessControl.AuditRule AuditRuleFactory(System.Security.Principal.IdentityReference identityReference, int accessMask, bool isInherited, System.Security.AccessControl.InheritanceFlags inheritanceFlags, System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AuditFlags flags) { throw null; }
        protected internal void Persist(System.Runtime.InteropServices.SafeHandle handle) { }
        protected internal void Persist(string name) { }
        public bool RemoveAccessRule(System.IO.Pipes.PipeAccessRule rule) { throw null; }
        public void RemoveAccessRuleSpecific(System.IO.Pipes.PipeAccessRule rule) { }
        public bool RemoveAuditRule(System.IO.Pipes.PipeAuditRule rule) { throw null; }
        public void RemoveAuditRuleAll(System.IO.Pipes.PipeAuditRule rule) { }
        public void RemoveAuditRuleSpecific(System.IO.Pipes.PipeAuditRule rule) { }
        public void ResetAccessRule(System.IO.Pipes.PipeAccessRule rule) { }
        public void SetAccessRule(System.IO.Pipes.PipeAccessRule rule) { }
        public void SetAuditRule(System.IO.Pipes.PipeAuditRule rule) { }
    }
}
