// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace Microsoft.Win32
{
    [System.Security.SecurityCriticalAttribute]
    public static partial class RegistryAclExtensions
    {
        public static System.Security.AccessControl.RegistrySecurity GetAccessControl(this Microsoft.Win32.RegistryKey key) { return default(System.Security.AccessControl.RegistrySecurity); }
        public static System.Security.AccessControl.RegistrySecurity GetAccessControl(this Microsoft.Win32.RegistryKey key, System.Security.AccessControl.AccessControlSections includeSections) { return default(System.Security.AccessControl.RegistrySecurity); }
        public static void SetAccessControl(this Microsoft.Win32.RegistryKey key, System.Security.AccessControl.RegistrySecurity registrySecurity) { }
    }
}
namespace System.Security.AccessControl
{
    /// <summary>
    /// Represents a set of access rights allowed or denied for a user or group. This class cannot
    /// be inherited.
    /// </summary>
    [System.Security.SecurityCriticalAttribute]
    public sealed partial class RegistryAccessRule : System.Security.AccessControl.AccessRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryAccessRule" />
        /// class, specifying the user or group the rule applies to, the access rights, and whether
        /// the specified access rights are allowed or denied.
        /// </summary>
        /// <param name="identity">
        /// The user or group the rule applies to. Must be of type
        /// <see cref="Principal.SecurityIdentifier" /> or a type such as <see cref="Principal.NTAccount" /> that can be converted
        /// to type <see cref="Principal.SecurityIdentifier" />.
        /// </param>
        /// <param name="registryRights">
        /// A bitwise combination of <see cref="AccessControl.RegistryRights" /> values
        /// indicating the rights allowed or denied.
        /// </param>
        /// <param name="type">
        /// One of the <see cref="AccessControlType" /> values indicating
        /// whether the rights are allowed or denied.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="registryRights" /> specifies an invalid value.-or-<paramref name="type" />
        /// specifies an invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="identity" /> is null. -or-<paramref name="eventRights" /> is zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="identity" /> is neither of type <see cref="Principal.SecurityIdentifier" />
        /// nor of a type such as <see cref="Principal.NTAccount" /> that can be
        /// converted to type <see cref="Principal.SecurityIdentifier" />.
        /// </exception>
        public RegistryAccessRule(System.Security.Principal.IdentityReference identity, System.Security.AccessControl.RegistryRights registryRights, System.Security.AccessControl.AccessControlType type) : base(default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AccessControlType)) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryAccessRule" />
        /// class, specifying the user or group the rule applies to, the access rights, the inheritance
        /// flags, the propagation flags, and whether the specified access rights are allowed or denied.
        /// </summary>
        /// <param name="identity">
        /// The user or group the rule applies to. Must be of type
        /// <see cref="Principal.SecurityIdentifier" /> or a type such as <see cref="Principal.NTAccount" /> that can be converted
        /// to type <see cref="Principal.SecurityIdentifier" />.
        /// </param>
        /// <param name="registryRights">
        /// A bitwise combination of <see cref="AccessControl.RegistryRights" /> values
        /// specifying the rights allowed or denied.
        /// </param>
        /// <param name="inheritanceFlags">
        /// A bitwise combination of <see cref="InheritanceFlags" /> flags
        /// specifying how access rights are inherited from other objects.
        /// </param>
        /// <param name="propagationFlags">
        /// A bitwise combination of <see cref="PropagationFlags" /> flags
        /// specifying how access rights are propagated to other objects.
        /// </param>
        /// <param name="type">
        /// One of the <see cref="AccessControlType" /> values specifying
        /// whether the rights are allowed or denied.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="registryRights" /> specifies an invalid value.-or-<paramref name="type" />
        /// specifies an invalid value.-or-<paramref name="inheritanceFlags" /> specifies an invalid value.-or-
        /// <paramref name="propagationFlags" /> specifies an invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="identity" /> is null.-or-<paramref name="registryRights" /> is zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="identity" /> is neither of type <see cref="Principal.SecurityIdentifier" />,
        /// nor of a type such as <see cref="Principal.NTAccount" /> that can be
        /// converted to type <see cref="Principal.SecurityIdentifier" />.
        /// </exception>
        public RegistryAccessRule(System.Security.Principal.IdentityReference identity, System.Security.AccessControl.RegistryRights registryRights, System.Security.AccessControl.InheritanceFlags inheritanceFlags, System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AccessControlType type) : base(default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AccessControlType)) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryAccessRule" />
        /// class, specifying the name of the user or group the rule applies to, the access rights,
        /// and whether the specified access rights are allowed or denied.
        /// </summary>
        /// <param name="identity">The name of the user or group the rule applies to.</param>
        /// <param name="registryRights">
        /// A bitwise combination of <see cref="AccessControl.RegistryRights" /> values
        /// indicating the rights allowed or denied.
        /// </param>
        /// <param name="type">
        /// One of the <see cref="AccessControlType" /> values indicating
        /// whether the rights are allowed or denied.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="registryRights" /> specifies an invalid value.-or-<paramref name="type" />
        /// specifies an invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="registryRights" /> is zero.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="identity" /> is null.-or-<paramref name="identity" /> is a zero-length string.-or-
        /// <paramref name="identity" /> is longer than 512 characters.
        /// </exception>
        public RegistryAccessRule(string identity, System.Security.AccessControl.RegistryRights registryRights, System.Security.AccessControl.AccessControlType type) : base(default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AccessControlType)) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryAccessRule" />
        /// class, specifying the name of the user or group the rule applies to, the access rights,
        /// the inheritance flags, the propagation flags, and whether the specified access rights are
        /// allowed or denied.
        /// </summary>
        /// <param name="identity">The name of the user or group the rule applies to.</param>
        /// <param name="registryRights">
        /// A bitwise combination of <see cref="AccessControl.RegistryRights" /> values
        /// indicating the rights allowed or denied.
        /// </param>
        /// <param name="inheritanceFlags">
        /// A bitwise combination of <see cref="InheritanceFlags" /> flags
        /// specifying how access rights are inherited from other objects.
        /// </param>
        /// <param name="propagationFlags">
        /// A bitwise combination of <see cref="PropagationFlags" /> flags
        /// specifying how access rights are propagated to other objects.
        /// </param>
        /// <param name="type">
        /// One of the <see cref="AccessControlType" /> values specifying
        /// whether the rights are allowed or denied.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="registryRights" /> specifies an invalid value.-or-<paramref name="type" />
        /// specifies an invalid value.-or-<paramref name="inheritanceFlags" /> specifies an invalid value.-or-
        /// <paramref name="propagationFlags" /> specifies an invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="eventRights" /> is zero.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="identity" /> is null.-or-<paramref name="identity" /> is a zero-length string.-or-
        /// <paramref name="identity" /> is longer than 512 characters.
        /// </exception>
        public RegistryAccessRule(string identity, System.Security.AccessControl.RegistryRights registryRights, System.Security.AccessControl.InheritanceFlags inheritanceFlags, System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AccessControlType type) : base(default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AccessControlType)) { }
        /// <summary>
        /// Gets the rights allowed or denied by the access rule.
        /// </summary>
        /// <returns>
        /// A bitwise combination of <see cref="AccessControl.RegistryRights" /> values
        /// indicating the rights allowed or denied by the access rule.
        /// </returns>
        public System.Security.AccessControl.RegistryRights RegistryRights { get { return default(System.Security.AccessControl.RegistryRights); } }
    }
    /// <summary>
    /// Represents a set of access rights to be audited for a user or group. This class cannot be inherited.
    /// </summary>
    [System.Security.SecurityCriticalAttribute]
    public sealed partial class RegistryAuditRule : System.Security.AccessControl.AuditRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryAuditRule" />
        /// class, specifying the user or group to audit, the rights to audit, whether to take inheritance
        /// into account, and whether to audit success, failure, or both.
        /// </summary>
        /// <param name="identity">
        /// The user or group the rule applies to. Must be of type
        /// <see cref="Principal.SecurityIdentifier" /> or a type such as <see cref="Principal.NTAccount" /> that can be converted
        /// to type <see cref="Principal.SecurityIdentifier" />.
        /// </param>
        /// <param name="registryRights">
        /// A bitwise combination of <see cref="AccessControl.RegistryRights" /> values
        /// specifying the kinds of access to audit.
        /// </param>
        /// <param name="inheritanceFlags">
        /// A bitwise combination of <see cref="InheritanceFlags" /> values
        /// specifying whether the audit rule applies to subkeys of the current key.
        /// </param>
        /// <param name="propagationFlags">
        /// A bitwise combination of <see cref="PropagationFlags" /> values
        /// that affect the way an inherited audit rule is propagated to subkeys of the current key.
        /// </param>
        /// <param name="flags">
        /// A bitwise combination of <see cref="AuditFlags" /> values
        /// specifying whether to audit success, failure, or both.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="eventRights" /> specifies an invalid value.-or-<paramref name="flags" /> specifies
        /// an invalid value.-or-<paramref name="inheritanceFlags" /> specifies an invalid value.-or-
        /// <paramref name="propagationFlags" /> specifies an invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="identity" /> is null. -or-<paramref name="registryRights" /> is zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="identity" /> is neither of type <see cref="Principal.SecurityIdentifier" />
        /// nor of a type such as <see cref="Principal.NTAccount" /> that can be
        /// converted to type <see cref="Principal.SecurityIdentifier" />.
        /// </exception>
        public RegistryAuditRule(System.Security.Principal.IdentityReference identity, System.Security.AccessControl.RegistryRights registryRights, System.Security.AccessControl.InheritanceFlags inheritanceFlags, System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AuditFlags flags) : base(default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AuditFlags)) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryAuditRule" />
        /// class, specifying the name of the user or group to audit, the rights to audit, whether
        /// to take inheritance into account, and whether to audit success, failure, or both.
        /// </summary>
        /// <param name="identity">The name of the user or group the rule applies to.</param>
        /// <param name="registryRights">
        /// A bitwise combination of <see cref="AccessControl.RegistryRights" /> values
        /// specifying the kinds of access to audit.
        /// </param>
        /// <param name="inheritanceFlags">
        /// A combination of <see cref="InheritanceFlags" /> flags that
        /// specifies whether the audit rule applies to subkeys of the current key.
        /// </param>
        /// <param name="propagationFlags">
        /// A combination of <see cref="PropagationFlags" /> flags that
        /// affect the way an inherited audit rule is propagated to subkeys of the current key.
        /// </param>
        /// <param name="flags">
        /// A bitwise combination of <see cref="AuditFlags" /> values
        /// specifying whether to audit success, failure, or both.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="eventRights" /> specifies an invalid value.-or-<paramref name="flags" /> specifies
        /// an invalid value.-or-<paramref name="inheritanceFlags" /> specifies an invalid value.-or-
        /// <paramref name="propagationFlags" /> specifies an invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="registryRights" /> is zero.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="identity" /> is null.-or-<paramref name="identity" /> is a zero-length string.-or-
        /// <paramref name="identity" /> is longer than 512 characters.
        /// </exception>
        public RegistryAuditRule(string identity, System.Security.AccessControl.RegistryRights registryRights, System.Security.AccessControl.InheritanceFlags inheritanceFlags, System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AuditFlags flags) : base(default(System.Security.Principal.IdentityReference), default(int), default(bool), default(System.Security.AccessControl.InheritanceFlags), default(System.Security.AccessControl.PropagationFlags), default(System.Security.AccessControl.AuditFlags)) { }
        /// <summary>
        /// Gets the access rights affected by the audit rule.
        /// </summary>
        /// <returns>
        /// A bitwise combination of <see cref="AccessControl.RegistryRights" /> values
        /// that indicates the rights affected by the audit rule.
        /// </returns>
        public System.Security.AccessControl.RegistryRights RegistryRights { get { return default(System.Security.AccessControl.RegistryRights); } }
    }
    /// <summary>
    /// Represents the Windows access control security for a registry key. This class cannot be inherited.
    /// </summary>
    [System.Security.SecurityCriticalAttribute]
    public sealed partial class RegistrySecurity : System.Security.AccessControl.NativeObjectSecurity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrySecurity" />
        /// class with default values.
        /// </summary>
        public RegistrySecurity() : base(default(bool), default(System.Security.AccessControl.ResourceType)) { }
        /// <summary>
        /// Gets the enumeration type that the <see cref="RegistrySecurity" />
        /// class uses to represent access rights.
        /// </summary>
        /// <returns>
        /// A <see cref="Type" /> object representing the
        /// <see cref="RegistryRights" /> enumeration.
        /// </returns>
        public override System.Type AccessRightType { get { return default(System.Type); } }
        /// <summary>
        /// Gets the type that the <see cref="RegistrySecurity" /> class
        /// uses to represent access rules.
        /// </summary>
        /// <returns>
        /// A <see cref="Type" /> object representing the
        /// <see cref="RegistryAccessRule" /> class.
        /// </returns>
        public override System.Type AccessRuleType { get { return default(System.Type); } }
        /// <summary>
        /// Gets the type that the <see cref="RegistrySecurity" /> class
        /// uses to represent audit rules.
        /// </summary>
        /// <returns>
        /// A <see cref="Type" /> object representing the
        /// <see cref="RegistryAuditRule" /> class.
        /// </returns>
        public override System.Type AuditRuleType { get { return default(System.Type); } }
        /// <summary>
        /// Creates a new access control rule for the specified user, with the specified access rights,
        /// access control, and flags.
        /// </summary>
        /// <param name="identityReference">
        /// An <see cref="Principal.IdentityReference" /> that identifies the user or
        /// group the rule applies to.
        /// </param>
        /// <param name="accessMask">
        /// A bitwise combination of <see cref="RegistryRights" /> values
        /// specifying the access rights to allow or deny, cast to an integer.
        /// </param>
        /// <param name="isInherited">A Boolean value specifying whether the rule is inherited.</param>
        /// <param name="inheritanceFlags">
        /// A bitwise combination of <see cref="InheritanceFlags" /> values
        /// specifying how the rule is inherited by subkeys.
        /// </param>
        /// <param name="propagationFlags">
        /// A bitwise combination of <see cref="PropagationFlags" /> values
        /// that modify the way the rule is inherited by subkeys. Meaningless if the value of
        /// <paramref name="inheritanceFlags" /> is <see cref="InheritanceFlags.None" />.
        /// </param>
        /// <param name="type">
        /// One of the <see cref="AccessControlType" /> values specifying
        /// whether the rights are allowed or denied.
        /// </param>
        /// <returns>
        /// A <see cref="RegistryAccessRule" /> object representing the
        /// specified rights for the specified user.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="accessMask" />, <paramref name="inheritanceFlags" />, <paramref name="propagationFlags" />,
        /// or <paramref name="type" /> specifies an invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="identityReference" /> is null. -or-<paramref name="accessMask" /> is zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="identityReference" /> is neither of type
        /// <see cref="Principal.SecurityIdentifier" />, nor of a type such as <see cref="Principal.NTAccount" /> that can be
        /// converted to type <see cref="Principal.SecurityIdentifier" />.
        /// </exception>
        public override System.Security.AccessControl.AccessRule AccessRuleFactory(System.Security.Principal.IdentityReference identityReference, int accessMask, bool isInherited, System.Security.AccessControl.InheritanceFlags inheritanceFlags, System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AccessControlType type) { return default(System.Security.AccessControl.AccessRule); }
        /// <summary>
        /// Searches for a matching access control with which the new rule can be merged. If none are found,
        /// adds the new rule.
        /// </summary>
        /// <param name="rule">The access control rule to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rule" /> is null.</exception>
        public void AddAccessRule(System.Security.AccessControl.RegistryAccessRule rule) { }
        /// <summary>
        /// Searches for an audit rule with which the new rule can be merged. If none are found, adds the
        /// new rule.
        /// </summary>
        /// <param name="rule">The audit rule to add. The user specified by this rule determines the search.</param>
        public void AddAuditRule(System.Security.AccessControl.RegistryAuditRule rule) { }
        /// <summary>
        /// Creates a new audit rule, specifying the user the rule applies to, the access rights to audit,
        /// the inheritance and propagation of the rule, and the outcome that triggers the rule.
        /// </summary>
        /// <param name="identityReference">
        /// An <see cref="Principal.IdentityReference" /> that identifies the user or
        /// group the rule applies to.
        /// </param>
        /// <param name="accessMask">
        /// A bitwise combination of <see cref="RegistryRights" /> values
        /// specifying the access rights to audit, cast to an integer.
        /// </param>
        /// <param name="isInherited">A Boolean value specifying whether the rule is inherited.</param>
        /// <param name="inheritanceFlags">
        /// A bitwise combination of <see cref="InheritanceFlags" /> values
        /// specifying how the rule is inherited by subkeys.
        /// </param>
        /// <param name="propagationFlags">
        /// A bitwise combination of <see cref="PropagationFlags" /> values
        /// that modify the way the rule is inherited by subkeys. Meaningless if the value of
        /// <paramref name="inheritanceFlags" /> is <see cref="InheritanceFlags.None" />.
        /// </param>
        /// <param name="flags">
        /// A bitwise combination of <see cref="AuditFlags" /> values
        /// specifying whether to audit successful access, failed access, or both.
        /// </param>
        /// <returns>
        /// A <see cref="RegistryAuditRule" /> object representing the
        /// specified audit rule for the specified user, with the specified flags. The return type of
        /// the method is the base class, <see cref="AuditRule" />, but
        /// the return value can be cast safely to the derived class.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="accessMask" />, <paramref name="inheritanceFlags" />, <paramref name="propagationFlags" />,
        /// or <paramref name="flags" /> specifies an invalid value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="identityReference" /> is null. -or-<paramref name="accessMask" /> is zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="identityReference" /> is neither of type
        /// <see cref="Principal.SecurityIdentifier" />, nor of a type such as <see cref="Principal.NTAccount" /> that can be
        /// converted to type <see cref="Principal.SecurityIdentifier" />.
        /// </exception>
        public override System.Security.AccessControl.AuditRule AuditRuleFactory(System.Security.Principal.IdentityReference identityReference, int accessMask, bool isInherited, System.Security.AccessControl.InheritanceFlags inheritanceFlags, System.Security.AccessControl.PropagationFlags propagationFlags, System.Security.AccessControl.AuditFlags flags) { return default(System.Security.AccessControl.AuditRule); }
        /// <summary>
        /// Searches for an access control rule with the same user and
        /// <see cref="AccessControlType" /> (allow or deny) as the specified access rule, and with compatible inheritance and propagation
        /// flags; if such a rule is found, the rights contained in the specified access rule are removed
        /// from it.
        /// </summary>
        /// <param name="rule">
        /// A <see cref="RegistryAccessRule" /> that specifies the user
        /// and <see cref="AccessControlType" /> to search for, and a
        /// set of inheritance and propagation flags that a matching rule, if found, must be compatible
        /// with. Specifies the rights to remove from the compatible rule, if found.
        /// </param>
        /// <returns>
        /// true if a compatible rule is found; otherwise false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="rule" /> is null.</exception>
        public bool RemoveAccessRule(System.Security.AccessControl.RegistryAccessRule rule) { return default(bool); }
        /// <summary>
        /// Searches for all access control rules with the same user and
        /// <see cref="AccessControlType" /> (allow or deny) as the specified rule and, if found, removes them.
        /// </summary>
        /// <param name="rule">
        /// A <see cref="RegistryAccessRule" /> that specifies the user
        /// and <see cref="AccessControlType" /> to search for. Any rights,
        /// inheritance flags, or propagation flags specified by this rule are ignored.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="rule" /> is null.</exception>
        public void RemoveAccessRuleAll(System.Security.AccessControl.RegistryAccessRule rule) { }
        /// <summary>
        /// Searches for an access control rule that exactly matches the specified rule and, if found,
        /// removes it.
        /// </summary>
        /// <param name="rule">The <see cref="RegistryAccessRule" /> to remove.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rule" /> is null.</exception>
        public void RemoveAccessRuleSpecific(System.Security.AccessControl.RegistryAccessRule rule) { }
        /// <summary>
        /// Searches for an audit control rule with the same user as the specified rule, and with compatible
        /// inheritance and propagation flags; if a compatible rule is found, the rights contained in the
        /// specified rule are removed from it.
        /// </summary>
        /// <param name="rule">
        /// A <see cref="RegistryAuditRule" /> that specifies the user
        /// to search for, and a set of inheritance and propagation flags that a matching rule, if found,
        /// must be compatible with. Specifies the rights to remove from the compatible rule, if found.
        /// </param>
        /// <returns>
        /// true if a compatible rule is found; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="rule" /> is null.</exception>
        public bool RemoveAuditRule(System.Security.AccessControl.RegistryAuditRule rule) { return default(bool); }
        /// <summary>
        /// Searches for all audit rules with the same user as the specified rule and, if found, removes
        /// them.
        /// </summary>
        /// <param name="rule">
        /// A <see cref="RegistryAuditRule" /> that specifies the user
        /// to search for. Any rights, inheritance flags, or propagation flags specified by this rule
        /// are ignored.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="rule" /> is null.</exception>
        public void RemoveAuditRuleAll(System.Security.AccessControl.RegistryAuditRule rule) { }
        /// <summary>
        /// Searches for an audit rule that exactly matches the specified rule and, if found, removes it.
        /// </summary>
        /// <param name="rule">The <see cref="RegistryAuditRule" /> to be removed.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rule" /> is null.</exception>
        public void RemoveAuditRuleSpecific(System.Security.AccessControl.RegistryAuditRule rule) { }
        /// <summary>
        /// Removes all access control rules with the same user as the specified rule, regardless of
        /// <see cref="AccessControlType" />, and then adds the specified rule.
        /// </summary>
        /// <param name="rule">
        /// The <see cref="RegistryAccessRule" /> to add. The user specified
        /// by this rule determines the rules to remove before this rule is added.
        /// </param>
        public void ResetAccessRule(System.Security.AccessControl.RegistryAccessRule rule) { }
        /// <summary>
        /// Removes all access control rules with the same user and
        /// <see cref="AccessControlType" /> (allow or deny) as the specified rule, and then adds the specified rule.
        /// </summary>
        /// <param name="rule">
        /// The <see cref="RegistryAccessRule" /> to add. The user and
        /// <see cref="AccessControlType" /> of this rule determine the
        /// rules to remove before this rule is added.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="rule" /> is null.</exception>
        public void SetAccessRule(System.Security.AccessControl.RegistryAccessRule rule) { }
        /// <summary>
        /// Removes all audit rules with the same user as the specified rule, regardless of the
        /// <see cref="AuditFlags" /> value, and then adds the specified rule.
        /// </summary>
        /// <param name="rule">
        /// The <see cref="RegistryAuditRule" /> to add. The user specified
        /// by this rule determines the rules to remove before this rule is added.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="rule" /> is null.</exception>
        public void SetAuditRule(System.Security.AccessControl.RegistryAuditRule rule) { }
    }
}
