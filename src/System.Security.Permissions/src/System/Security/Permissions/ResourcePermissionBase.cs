// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Security.Permissions
{
    [SecurityPermission(SecurityAction.InheritanceDemand, ControlEvidence = true, ControlPolicy = true)]
    public abstract class ResourcePermissionBase : CodeAccessPermission, IUnrestrictedPermission
    {
        public const string Any = "*";
        public const string Local = ".";
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        protected ResourcePermissionBase() { }
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        protected ResourcePermissionBase(PermissionState state) { }
        // Put this in one central place.  Some resource types may require a
        // different form of string comparison.  If we need to fix this, then
        // consider making this protected & virtual, and override it where 
        // necessary.  Or consider doing this all internally so we could 
        // reimplement this permission to use a generic collection, etc.
        private static Hashtable CreateHashtable() { return null; }
        private string ComputerName { get; set; }
        private bool IsEmpty { get; }
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        protected Type PermissionAccessType { get; set; }
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        protected string[] TagNames { get; set; }
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        protected void AddPermissionAccess(ResourcePermissionBaseEntry entry) { }
        protected void Clear() { }
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override IPermission Copy() { return null; }
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        protected ResourcePermissionBaseEntry[] GetPermissionEntries() { return null; }
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override void FromXml(SecurityElement securityElement) { }
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override IPermission Intersect(IPermission target) { return null; }
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override bool IsSubsetOf(IPermission target) { return false; }
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool IsUnrestricted() { return false; }
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        protected void RemovePermissionAccess(ResourcePermissionBaseEntry entry) { }
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override SecurityElement ToXml() { return null; }
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public override IPermission Union(IPermission target) { return null; }
    }
}
