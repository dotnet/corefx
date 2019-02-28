// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.IO
{
    public static partial class FileSystemAclExtensions
    {
        public static System.Security.AccessControl.DirectorySecurity GetAccessControl(this System.IO.DirectoryInfo directoryInfo) { throw null; }
        public static System.Security.AccessControl.DirectorySecurity GetAccessControl(this System.IO.DirectoryInfo directoryInfo, System.Security.AccessControl.AccessControlSections includeSections) { throw null; }
        public static System.Security.AccessControl.FileSecurity GetAccessControl(this System.IO.FileInfo fileInfo) { throw null; }
        public static System.Security.AccessControl.FileSecurity GetAccessControl(this System.IO.FileInfo fileInfo, System.Security.AccessControl.AccessControlSections includeSections) { throw null; }
        public static System.Security.AccessControl.FileSecurity GetAccessControl(this System.IO.FileStream fileStream) { throw null; }
        public static void SetAccessControl(this System.IO.DirectoryInfo directoryInfo, System.Security.AccessControl.DirectorySecurity directorySecurity) { }
        public static void SetAccessControl(this System.IO.FileInfo fileInfo, System.Security.AccessControl.FileSecurity fileSecurity) { }
        public static void SetAccessControl(this System.IO.FileStream fileStream, System.Security.AccessControl.FileSecurity fileSecurity) { }
    }
}
namespace System.Security.AccessControl
{
    public abstract partial class DirectoryObjectSecurity : System.Security.AccessControl.ObjectSecurity
    {
        protected DirectoryObjectSecurity() { }
        protected DirectoryObjectSecurity(System.Security.AccessControl.CommonSecurityDescriptor securityDescriptor) { }
        public virtual System.Security.AccessControl.AccessRule AccessRuleFactory(System.Security.Principal.IdentityReference identityReference, int accessMask, bool isInherited, System.Security.AccessControl.InheritanceFlags inheritanceFlags, System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AccessControlType type, System.Guid objectType, System.Guid inheritedObjectType) { throw null; }
        protected void AddAccessRule(System.Security.AccessControl.ObjectAccessRule rule) { }
        protected void AddAuditRule(System.Security.AccessControl.ObjectAuditRule rule) { }
        public virtual System.Security.AccessControl.AuditRule AuditRuleFactory(System.Security.Principal.IdentityReference identityReference, int accessMask, bool isInherited, System.Security.AccessControl.InheritanceFlags inheritanceFlags, System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AuditFlags flags, System.Guid objectType, System.Guid inheritedObjectType) { throw null; }
        public System.Security.AccessControl.AuthorizationRuleCollection GetAccessRules(bool includeExplicit, bool includeInherited, System.Type targetType) { throw null; }
        public System.Security.AccessControl.AuthorizationRuleCollection GetAuditRules(bool includeExplicit, bool includeInherited, System.Type targetType) { throw null; }
        protected override bool ModifyAccess(System.Security.AccessControl.AccessControlModification modification, System.Security.AccessControl.AccessRule rule, out bool modified) { throw null; }
        protected override bool ModifyAudit(System.Security.AccessControl.AccessControlModification modification, System.Security.AccessControl.AuditRule rule, out bool modified) { throw null; }
        protected bool RemoveAccessRule(System.Security.AccessControl.ObjectAccessRule rule) { throw null; }
        protected void RemoveAccessRuleAll(System.Security.AccessControl.ObjectAccessRule rule) { }
        protected void RemoveAccessRuleSpecific(System.Security.AccessControl.ObjectAccessRule rule) { }
        protected bool RemoveAuditRule(System.Security.AccessControl.ObjectAuditRule rule) { throw null; }
        protected void RemoveAuditRuleAll(System.Security.AccessControl.ObjectAuditRule rule) { }
        protected void RemoveAuditRuleSpecific(System.Security.AccessControl.ObjectAuditRule rule) { }
        protected void ResetAccessRule(System.Security.AccessControl.ObjectAccessRule rule) { }
        protected void SetAccessRule(System.Security.AccessControl.ObjectAccessRule rule) { }
        protected void SetAuditRule(System.Security.AccessControl.ObjectAuditRule rule) { }
    }
    public sealed partial class DirectorySecurity : System.Security.AccessControl.FileSystemSecurity
    {
        public DirectorySecurity() { }
        public DirectorySecurity(string name, System.Security.AccessControl.AccessControlSections includeSections) { }
    }
    public sealed partial class FileSecurity : System.Security.AccessControl.FileSystemSecurity
    {
        public FileSecurity() { }
        public FileSecurity(string fileName, System.Security.AccessControl.AccessControlSections includeSections) { }
    }
    public sealed partial class FileSystemAccessRule : System.Security.AccessControl.AccessRule
    {
        public FileSystemAccessRule(System.Security.Principal.IdentityReference identity, System.Security.AccessControl.FileSystemRights fileSystemRights, System.Security.AccessControl.AccessControlType type) : base (default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AccessControlType)) { }
        public FileSystemAccessRule(System.Security.Principal.IdentityReference identity, System.Security.AccessControl.FileSystemRights fileSystemRights, System.Security.AccessControl.InheritanceFlags inheritanceFlags, System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AccessControlType type) : base (default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AccessControlType)) { }
        public FileSystemAccessRule(string identity, System.Security.AccessControl.FileSystemRights fileSystemRights, System.Security.AccessControl.AccessControlType type) : base (default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AccessControlType)) { }
        public FileSystemAccessRule(string identity, System.Security.AccessControl.FileSystemRights fileSystemRights, System.Security.AccessControl.InheritanceFlags inheritanceFlags, System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AccessControlType type) : base (default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AccessControlType)) { }
        public System.Security.AccessControl.FileSystemRights FileSystemRights { get { throw null; } }
    }
    public sealed partial class FileSystemAuditRule : System.Security.AccessControl.AuditRule
    {
        public FileSystemAuditRule(System.Security.Principal.IdentityReference identity, System.Security.AccessControl.FileSystemRights fileSystemRights, System.Security.AccessControl.AuditFlags flags) : base (default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AuditFlags)) { }
        public FileSystemAuditRule(System.Security.Principal.IdentityReference identity, System.Security.AccessControl.FileSystemRights fileSystemRights, System.Security.AccessControl.InheritanceFlags inheritanceFlags, System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AuditFlags flags) : base (default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AuditFlags)) { }
        public FileSystemAuditRule(string identity, System.Security.AccessControl.FileSystemRights fileSystemRights, System.Security.AccessControl.AuditFlags flags) : base (default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AuditFlags)) { }
        public FileSystemAuditRule(string identity, System.Security.AccessControl.FileSystemRights fileSystemRights, System.Security.AccessControl.InheritanceFlags inheritanceFlags, System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AuditFlags flags) : base (default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AuditFlags)) { }
        public System.Security.AccessControl.FileSystemRights FileSystemRights { get { throw null; } }
    }
    [System.FlagsAttribute]
    public enum FileSystemRights
    {
        AppendData = 4,
        ChangePermissions = 262144,
        CreateDirectories = 4,
        CreateFiles = 2,
        Delete = 65536,
        DeleteSubdirectoriesAndFiles = 64,
        ExecuteFile = 32,
        FullControl = 2032127,
        ListDirectory = 1,
        Modify = 197055,
        Read = 131209,
        ReadAndExecute = 131241,
        ReadAttributes = 128,
        ReadData = 1,
        ReadExtendedAttributes = 8,
        ReadPermissions = 131072,
        Synchronize = 1048576,
        TakeOwnership = 524288,
        Traverse = 32,
        Write = 278,
        WriteAttributes = 256,
        WriteData = 2,
        WriteExtendedAttributes = 16,
    }
    public abstract partial class FileSystemSecurity : System.Security.AccessControl.NativeObjectSecurity
    {
        internal FileSystemSecurity() : base (default(bool), default(System.Security.AccessControl.ResourceType)) { }
        public override System.Type AccessRightType { get { throw null; } }
        public override System.Type AccessRuleType { get { throw null; } }
        public override System.Type AuditRuleType { get { throw null; } }
        public sealed override System.Security.AccessControl.AccessRule AccessRuleFactory(System.Security.Principal.IdentityReference identityReference, int accessMask, bool isInherited, System.Security.AccessControl.InheritanceFlags inheritanceFlags, System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AccessControlType type) { throw null; }
        public void AddAccessRule(System.Security.AccessControl.FileSystemAccessRule rule) { }
        public void AddAuditRule(System.Security.AccessControl.FileSystemAuditRule rule) { }
        public sealed override System.Security.AccessControl.AuditRule AuditRuleFactory(System.Security.Principal.IdentityReference identityReference, int accessMask, bool isInherited, System.Security.AccessControl.InheritanceFlags inheritanceFlags, System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AuditFlags flags) { throw null; }
        public bool RemoveAccessRule(System.Security.AccessControl.FileSystemAccessRule rule) { throw null; }
        public void RemoveAccessRuleAll(System.Security.AccessControl.FileSystemAccessRule rule) { }
        public void RemoveAccessRuleSpecific(System.Security.AccessControl.FileSystemAccessRule rule) { }
        public bool RemoveAuditRule(System.Security.AccessControl.FileSystemAuditRule rule) { throw null; }
        public void RemoveAuditRuleAll(System.Security.AccessControl.FileSystemAuditRule rule) { }
        public void RemoveAuditRuleSpecific(System.Security.AccessControl.FileSystemAuditRule rule) { }
        public void ResetAccessRule(System.Security.AccessControl.FileSystemAccessRule rule) { }
        public void SetAccessRule(System.Security.AccessControl.FileSystemAccessRule rule) { }
        public void SetAuditRule(System.Security.AccessControl.FileSystemAuditRule rule) { }
    }
}
